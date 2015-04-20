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
    }
}
