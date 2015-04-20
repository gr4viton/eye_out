using System;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using SharpOVR;
using System.Diagnostics; // StopWatch

using System.Collections.Generic; // list

using SharpDX.Toolkit;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

using System.Threading.Tasks;

using EyeOut;

using System.Runtime.InteropServices; // marshal

using BaslerImage = Basler.Pylon.IImage;
using ToolkitImage = SharpDX.Toolkit.Graphics.Image;
//using ToolkitTexture = SharpDX.Toolkit.Graphics.Texture2D;
//using SharpDX.Toolkit.Graphics;

namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;

    /// <summary>
    /// 
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        
        //private List<GeometricPrimitive> primitives;


        ToolkitImage cameraImage;
        PixelFormat cameraPixelFormat = PixelFormat.R8G8B8A8.UNorm;
        //PixelFormat cameraPixelFormat = PixelFormat.R8;
        private BasicEffect cameraBasicEffect;
        private Texture2D cameraTexture;
        private GeometricPrimitive cameraSurface;
        //private byte[] pixelData;
        private byte[] rgbPixelData;
        private bool notGrabbedYet = true;

        public float cameraSurfaceX;
        public float cameraSurfaceY;


        public void CAPTURE_cameraImage()
        {
            // ask the camera for capture - rest is done in the StreamGrabber_ImageGrabbed handler function
            config.streamController.TakeSingleSnapshot();
            //config.streamController.
        }


        bool first = true;
        Basler.Pylon.PixelDataConverter pixelDataConverter;
        int width;
        int height;
        long rgbLen;
        long rgbaLen;
        bool first2 = true;
        byte[] pixelData;

        void StreamGrabber_ImageGrabbed(object sender, Basler.Pylon.ImageGrabbedEventArgs e)
        {
            // writes the grabbed image from camera into the texture
            Basler.Pylon.IImage baslerImage = (Basler.Pylon.IImage)e.GrabResult;
            
            if (baslerImage == null)
            {
                LOG("Could not retrieve grabbed image");
            }
            else
            {
                LOG("Retrieved grabbed image");

                if (first == true)
                {
                    pixelDataConverter = new Basler.Pylon.PixelDataConverter();
                    //pixelDataConverter.OutputPixelFormat = Basler.Pylon.PixelType.RGB8planar;
                    //pixelDataConverter.OutputPixelFormat = Basler.Pylon.PixelType.RGB8packed;
                    pixelDataConverter.OutputPixelFormat = Basler.Pylon.PixelType.BGR8packed;
                    //cameraPixelFormat = PixelFormat..R8G8B8A8.UNorm;
                    cameraPixelFormat = PixelFormat.B8G8R8X8.UNorm;
                    width = baslerImage.Width;
                    height = baslerImage.Height;
                    rgbLen = pixelDataConverter.GetBufferSizeForConversion(pixelDataConverter.OutputPixelFormat, width, height);
                    rgbPixelData = new byte[rgbLen];
                    rgbaLen = (rgbLen / 3) * 4;
                    pixelData = new byte[rgbaLen];
                    
                    //cameraImage = ToolkitImage.New2D(width, height, 1, cameraPixelFormat);
                    //cameraTexture = Texture2D.New(GraphicsDevice, cameraImage);
                    //cameraTexture = Texture2D.New(GraphicsDevice, width, height, 0, Usage.Shared, cameraPixelFormat);
                    cameraTexture = Texture2D.New(GraphicsDevice, width, height, cameraPixelFormat, pixelData, TextureFlags.ShaderResource, ResourceUsage.Dynamic);
                }
                LOG("Start conversion");
                pixelDataConverter.Convert(rgbPixelData, baslerImage);
                LOG("Stop conversion");
                LOG("Start add alpha");
                int q = 0;
                for (int i = 0; i < rgbLen; i++)
                {
                    pixelData[q] = rgbPixelData[i];
                    q++;
                    // if first
                    if (i % 3 == 2) // every third pixel color 
                    {
                        pixelData[q] = 255;
                        q++;
                    }
                }
                LOG("Stop add alpha");

                //cameraImage.PixelBuffer[0].SetPixels(rgbPixelData);
                //cameraImage.PixelBuffer[0].SetPixels(pixelData);
                //cameraImage.PixelBuffer[0].CopyTo(
                //if (first == true)
                //{

                // too slow
                //cameraTexture = Texture2D.New(GraphicsDevice, cameraImage);

                // copy our buffer to the texture
                //cameraTexture.SetData<byte>(pixelData);
                cameraTexture.SetData<byte>(rgbPixelData);
                //Size of toData (12521520 bytes) is not compatible expected size (16695360 bytes) : Width * Height * Depth * sizeof(PixelFormat) size in bytes


                //    first = false;
                //}
                //cameraTexture.SetData(GraphicsDevice, cameraImage);
                //cameraTexture.SetData<byte>(GraphicsDevice, pixelData);
                //cameraTexture.Description.Format = Format.R8G8B8A8_UInt;

                //cameraTexture.SetData<byte>(pixelData, 0, rgbaLen, SetDataOptions.NoOverwrite);
                //cameraTexture.SetData<byte>(pixelData, 0, 0);
                //http://stackoverflow.com/questions/2869801/is-there-a-fast-alternative-to-creating-a-texture2d-from-a-bitmap-object-in-xna

                //try
                //{
                //}
                //catch (Exception ex)
                //{
                //    LOG("catched ex="+ ex.Message.ToString());
                //}
                //cameraBasicEffect.Texture = cameraTexture;

            }

        }

        protected virtual void Draw_BaslerCamera(GameTime _gameTime)
        {
            int i = 0;
            // Calculate the translation
            float dx = ((i + 1) % 4);
            float dy = ((i + 1) / 4);


            //float gameTime = _gameTime.ElapsedGameTime.Milliseconds/100;
            float gameTime = 0;
            // Setup the World matrice for this primitive
            cameraBasicEffect.World = 
                Matrix.Scaling(0.5f) 
                * Matrix.RotationX(gameTime) 
                * Matrix.RotationY(gameTime * 2.0f) 
                * Matrix.RotationZ(gameTime * .7f) 
                * Matrix.Translation(cameraSurfaceX, cameraSurfaceY, 0);
            
            // Disable Cull only for the plane primitive, otherwise use standard culling
            //GraphicsDevice.SetRasterizerState(i == 0 ? GraphicsDevice.RasterizerStates.CullNone : GraphicsDevice.RasterizerStates.CullBack);

            //GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullNone);
            //cameraBasicEffect.VertexColorEnabled = true;
            cameraBasicEffect.TextureEnabled = true;
            cameraBasicEffect.Texture = cameraTexture;
            //cameraSurface.VertexBuffer = 
            // Draw the primitive using BasicEffect
            cameraSurface.Draw(cameraBasicEffect);
        }

        public void Constructor_BaslerCamera()
        {
            if (config.ReadCameraStream == true)
            {
                config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            }
            cameraSurfaceX = 0.0f;
            cameraSurfaceY = 0.0f;
        }

        public void LoadContent_BaslerCamera()
        {
            //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            cameraBasicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            cameraSurface = ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice));

            // Load the texture
            //cameraTexture = Content.Load<Texture2D>("speaker");
            cameraTexture = Content.Load<Texture2D>("cameraDefault_2015-04-20_09-34-31");
            
            
            //cameraTexture = Texture2D.New(GraphicsDevice, width, height, PixelFormat.B8G8R8A8.UNorm);
            cameraBasicEffect.Texture = cameraTexture;
            cameraBasicEffect.TextureEnabled = true;
        }
        void ConvertTexture()
        {
            //http://sharpdx.org/forum/5-api-usage/3214-d3d11-surface-datastream-to-bitmap
            //using (var stagingTexture = CreateStagingTexture())
            //{
            //    _deviceContext.CopyResource(renderToTextureRtv.Resource, stagingTexture);

            //    DataStream stream;
            //    var wb = new WriteableBitmap(Width, Height);

            //    try
            //    {
            //        var dataBox = _deviceContext.MapSubresource(stagingTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None, out stream);
            //        var line = new byte[Width * 4];

            //        using (var s = wb.PixelBuffer.AsStream())
            //        {
            //            for (int i = 0; i < Height; i++)
            //            {
            //                stream.Seek(i * dataBox.RowPitch, System.IO.SeekOrigin.Begin);
            //                stream.Read(line, 0, line.Length);

            //                s.Seek(i * Width * 4, System.IO.SeekOrigin.Begin);
            //                s.Write(line, 0, line.Length);
            //            }
            //        }
            //    }
            //    finally
            //    {
            //        if (stream != null)
            //            stream.Dispose();
            //        _deviceContext.UnmapSubresource(stagingTexture, 0);
            //        wb.Invalidate();
            //    }
            //    return wb;
            //}        
        }
    }
}
