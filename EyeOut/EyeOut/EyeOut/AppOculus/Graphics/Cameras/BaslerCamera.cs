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
using ToolkitTexture = SharpDX.Toolkit.Graphics.Texture2D;
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


        private BasicEffect cameraBasicEffect;
        private Texture2D cameraTexture;
        private GeometricPrimitive cameraSurface;
        private byte[] pixelData;
        private bool notGrabbedYet = true;

        public void CAPTURE_cameraImage()
        {
            // ask the camera for capture - rest is done in the StreamGrabber_ImageGrabbed handler function
            config.streamController.TakeSingleSnapshot();
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
        void StreamGrabber_ImageGrabbed(object sender, Basler.Pylon.ImageGrabbedEventArgs e)
        {
            // writes the grabbed image from camera into the texture

            Basler.Pylon.IImage baslerImage = (Basler.Pylon.IImage)e.GrabResult;
            //guiImageViewer.CaptureImage();

            INIT_cameraImage(baslerImage);

            if (baslerImage == null)
            {
                LOG("Could not retrieve grabbed image");
            }
            else
            {
                LOG("Retrieved grabbed image");

                //DataBox[] db = new DataBox()[];
                //cameraImage.Description.Format = Format.R8G8B8A8_UNorm_SRgb;
                pixelData = (byte[])baslerImage.PixelData;

                int length = pixelData.Length;
                int width = baslerImage.Width;
                int height = baslerImage.Height;

                //byte[] txuData = new byte[length];
                
                //int i = 0;
                //for (int dif = 0; dif < 3; dif++)
                //{
                //    for (int x = 0; x < width; x++)
                //    {
                //        txuData[x+dif] = pixelData[x*3+dif];
                //    }
                //}


                if (notGrabbedYet == true)
                {

                    cameraImage = ToolkitImage.New2D(width, height, 1, cameraPixelFormat);
                    cameraImage.PixelBuffer[0].SetPixels(pixelData, 0);
                    notGrabbedYet = false;
                }
                else
                {


                    //txuData = pixelData;
                    int indexRed = 0;
                    Color pixelColor;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            //txuData[y+x*y] = pixelData
                            indexRed = 3 * (x + y * width);
                            if (indexRed + 2 < length)
                            {
                                pixelColor = new Color(pixelData[indexRed], pixelData[indexRed + 1], pixelData[indexRed + 2]);
                                cameraImage.PixelBuffer[0].SetPixel(x, y, pixelColor);

                            }
                        }
                    }
                    //cameraImage.PixelBuffer[0].SetPixels(txuData,0);
                }

                
        //SharpDX.WIC.PixelFormat.Format32bppPRGBA,
                
 
                //var bitmapDecoder = new SharpDX.WIC.BitmapDecoder( devices.WICFactory, filepath, SharpDX.WIC.DecodeOptions.CacheOnDemand );
             
                //var formatConverter = new SharpDX.WIC.FormatConverter( devices.WICFactory );
 
                //formatConverter.Initialize(
                //        bitmapDecoder.GetFrame(0),
                //        SharpDX.WIC.PixelFormat.Format32bppPRGBA,
                //        SharpDX.WIC.BitmapDitherType.None,
                //        null,
                //        0.0,
                //        SharpDX.WIC.BitmapPaletteType.Custom);
 
                //Texture2D tex = CreateTexture2DFromBitmap(devices.3D3Device, formatConverter, ref desc);
 
                //formatConverter.Dispose();
                //bitmapDecoder.Dispose();

                try
                {
                    cameraTexture = Texture2D.New(GraphicsDevice, cameraImage);
                    //cameraTexture = Texture2D.New(GraphicsDevice, width, height, PixelFormat.B8G8R8A8.UNorm);
                    //cameraTexture.SetData<Byte>(pixelData);\
                    //cameraTexture.SetData(cameraImage);
                }
                catch (Exception ex)
                {
                    LOG("Catched exception when creating texture from camera: " + ex.Message);
                }
            }

        }

        protected virtual void Draw_BaslerCamera(GameTime gameTime)
        {
            int i = 0;
            // Calculate the translation
            float dx = ((i + 1) % 4);
            float dy = ((i + 1) / 4);

            float x = (dx - 1.5f) * 1.7f;
            float y = 1.0f - 2.0f * dy;

            // Setup the World matrice for this primitive
            //basicEffect.World = Matrix.Scaling((float)Math.Sin(gameTime * 1.5f) * 0.2f + 1.0f) * Matrix.RotationX(gameTime) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f) * Matrix.Translation(x, y, 0);
            
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
    }
}
