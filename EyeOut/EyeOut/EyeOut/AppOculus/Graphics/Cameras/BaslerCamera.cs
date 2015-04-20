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

        private BasicEffect cameraBasicEffect;
        private Texture2D cameraTexture;
        private List<GeometricPrimitive> primitives;
        private GeometricPrimitive cameraSurface;


        public void CAPTURE_cameraImage()
        {
            // ask the camera for capture - rest is done in the StreamGrabber_ImageGrabbed handler function
            config.streamController.TakeSingleSnapshot();
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

                //byte[] pixelData = (byte[])baslerImage.PixelData; // R,G,B,A

                //DataBox[] db = new DataBox()[];

                //IntPtr ptr = ref pixelData;
                //int arraySize = pixelData.Length;

                //IntPtr unmanagedPointer = Marshal.AllocHGlobal(arraySize);
                //Marshal.Copy(pixelData, 0, unmanagedPointer, arraySize);
                // Call unmanaged code

                //, arraySize,
                //unmanagedPointer);
                cameraImage.PixelBuffer[0].SetPixels((byte[])baslerImage.PixelData);


                //ToolkitImage imCam = ToolkitImage.Load(pixelData, true);
                try
                {
                    cameraTexture = Texture2D.New(GraphicsDevice, cameraImage);
                }
                catch (Exception ex)
                {
                    LOG("Catched exception when creating texture from camera: " + ex.Message);
                }

                //cameraImage.Dispose();
                //Marshal.FreeHGlobal(unmanagedPointer);

                //Texture2D txu = Texture2D.New(
                //                                GrahpicsDevice, 
                //                                im.Width, 
                //                                im.Height, 
                //                                1, 
                //                                PixelFormat.B8G8R8X8.UNorm, 
                //                                db);

                //,TextureFlags.None );


                //CamTexture.Load(device, stream);
                //texture = CamTexture.New(GraphicsDevice, imCam, TextureFlags.None, ResourceUsage.Default);
                //, baCam.Width, baCam.Height, imCam);

                //public static Texture2D New(GraphicsDevice device, Image image, 
                //TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable);
                //texture = 
                //throw new NotImplementedException();
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
            cameraBasicEffect.Texture = cameraTexture;
            cameraBasicEffect.TextureEnabled = true;
        }
    }
}
