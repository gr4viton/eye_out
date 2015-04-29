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

using StreamController = Basler.Pylon.Controls.WPF.StreamController;
using ImageViewer = Basler.Pylon.Controls.WPF.ImageViewer;

using System.Threading;


namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;



    public class StreamControllerThreadSafe : StreamController
    {

    }

    public class BaslerCameraInterface
    {
    //    public override void 
        Basler.Pylon.Camera cam;
        //Basler.Pylon.CameraFinder camFinder;
        StreamController strc;
        //Basler.Pylon.

        public BaslerCameraInterface()
        {
            //strc.Camera.StreamGrabber.GrabStopping
            //camFinder = new Basler.Pylon.CameraFinder.
        }
    }

    //public class grabResult : IGrab
    //public class ImageViewerThreadSafe : ImageViewer
    //{

    //    public override void 
    //}
    /// <summary>
    /// Basler camera part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        
        //private List<GeometricPrimitive> primitives;
        //StreamController streamController;
        //ImageViewer imageViewer;

        Basler.Pylon.IImage baslerImage;
        //ToolkitImage cameraImage;
        PixelFormat cameraPixelFormat = PixelFormat.R8G8B8A8.UNorm;
        //PixelFormat cameraPixelFormat = PixelFormat.R8;
        private BasicEffect cameraBasicEffect;

        private object locker_cameraTexture = new object();
        
        private Texture2D cameraTexture;
        private Texture2D CameraTexture
        {
            get
            {
                lock (locker_cameraTexture)
                {
                    return cameraTexture;
                }
            }
            set
            {
                lock (locker_cameraTexture)
                {
                    cameraTexture = value;
                }
            }
        }

        private GeometricPrimitive cameraSurface;

        
        //Basler.Pylon.PixelDataConverter pixelDataConverter;
        int width;
        int height;
        byte[] pixelData;

        object locker_pixelData = new object();


        //public void CAPTURE_cameraImage_new()
        //{
        //    if (C_State.FURTHER(e_stateBaslerCam.initialized))
        //    {
        //        START_streaming();
        //        //baslerImage = config.streamController.Camera.StreamGrabber();
        //        baslerImage = config.ImageViewer.CaptureImage();

        //        if (baslerImage != null)
        //        {
        //            pixelData = (byte[])baslerImage.PixelData;
        //            if (pixelData.Length != CameraTexture.Width * CameraTexture.Height * 4)
        //            {
        //                SETUP_BaslerCamera();
        //            }
        //            else
        //            {
        //                CameraTexture.SetData<byte>(pixelData);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        SETUP_BaslerCamera();
        //    }
        //}

        

        public bool firstPass = true;
        public int pixelDataSize;
        public void CAPTURE_cameraImage()
        {
            LOG("CAPTURE_cameraImage start");

            //if( config.streamController.Camera.StreamGrabber.IsGrabbing

            //      This method throws exceptions on timeout Basler.Pylon.TimeoutHandling.
            //     Grabbing single images using a software trigger (see Basler.Pylon.Configuration.SoftwareTrigger(System.Object,System.EventArgs))
            //     is recommended if you want to maximize frame rate.  This is because the overhead
            //     per grabbed image is reduced compared to single frame acquisition (see Basler.Pylon.Configuration.AcquireSingleFrame(System.Object,System.EventArgs)).
            //      The grabbing can be started using Basler.Pylon.IStreamGrabber.Start(Basler.Pylon.GrabStrategy,Basler.Pylon.GrabLoop).
            //      Images are grabbed using the Basler.Pylon.ICamera.WaitForFrameTriggerReady(System.Int32,Basler.Pylon.TimeoutHandling),
            //     Basler.Pylon.ICamera.ExecuteSoftwareTrigger(), and Basler.Pylon.IStreamGrabber.RetrieveResult(System.Int32,Basler.Pylon.TimeoutHandling)
            //     methods instead of using Basler.Pylon.IStreamGrabber.GrabOne(System.Int32).
            //      Grabbing can be stopped using Basler.Pylon.IStreamGrabber.Stop() when done.

            //config.streamController.StartStreaming();

            //config.ImageViewer.Camera.WaitForFrameTriggerReady(1, Basler.Pylon.TimeoutHandling.Return);
            //config.ImageViewer.Camera.ExecuteSoftwareTrigger();
            //Basler.Pylon.IGrabResult res = config.ImageViewer.Camera.StreamGrabber.RetrieveResult(1, Basler.Pylon.TimeoutHandling.Return); // another stream is waiting for the result
            //HUD.AppendLine(res.PixelTypeValue.ToString());

            lock (locker_pixelData)
            //if(false)
            {
                if (firstPass == true)
                {
                    if (config.streamController.Camera.IsOpen == false)
                    {
                        config.streamController.Camera.Open();
                    }

                    //config.streamController.Camera.StreamGrabber.GrabStopped += StreamGrabber_GrabStopped;
                    //config.streamController.Camera.StreamGrabber.GrabStarting += StreamGrabber_GrabStarting;
                    //config.streamController.Camera.StreamGrabber.GrabStarted += StreamGrabber_GrabStarted;

                    config.streamController.StartStreaming();
                    //config.streamController.TakeSingleSnapshot();

                    cameraPixelFormat = PixelFormat.B8G8R8X8.UNorm;

                    if (config.ImageViewer != null)
                    {
                        Basler.Pylon.IImage thisBaslerImage = config.ImageViewer.CaptureImage();
                        //config.imageViewer.Visibility = System.Windows.Visibility.Hidden;
                        if (thisBaslerImage != null)
                        {
                            pixelData = (byte[])thisBaslerImage.PixelData;
                            width = config.ImageViewer.CaptureImage().Width;
                            height = config.ImageViewer.CaptureImage().Height;
                            pixelDataSize = width*height*4;
                            CameraTexture = Texture2D.New(GraphicsDevice, width, height, cameraPixelFormat, pixelData, 
                                
                                TextureFlags.ShaderResource, ResourceUsage.Dynamic);

                            
                            firstPass = false;

                        }
                        //config.streamController
                        //config.imageViewer.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    //config.streamController.TakeSingleSnapshot();
                    //config.guiDispatcher.Thread.Suspend();
                    //pixelData = (byte[])config.imageViewer.CaptureImage().PixelData;
                    //byte[] thisPixelData = new byte[pixelDataSize];
                    //thisPixelData = (byte[])config.imageViewer.CaptureImage().PixelData;
                    //((byte[])baslerImage.PixelData).CopyTo(thisPixelData, 0);

                    //LOG("reading data start");
                    //byte[] thisPixelData = new byte[pixelDataSize];
                    //byte[] thisPixelData2 = new byte[pixelDataSize];
                    ////config.ImageViewer.Camera.StreamGrabber.GrabResultWaitHandle.WaitOne(10);
                    //config.ImageViewer.Camera.StreamGrabber.Stop();
                    //thisPixelData = (byte[])config.ImageViewer.CaptureImage().PixelData;
                    //LOG("reading data end");
                    //LOG("copying byte array start");

                    ////config.ImageViewer.Camera.StreamGrabber.GrabStopWaitHandle.WaitOne(10);

                    //thisPixelData.CopyTo(thisPixelData2,0);
                    //config.ImageViewer.Camera.StreamGrabber.Start();

                    //LOG("setting texture start");
                    //CameraTexture.SetData<byte>(thisPixelData2);
                    //LOG("setting texture end");



                    if (config.ImageViewer != null)
                    {
                        WaitHandle[] events = new WaitHandle[]{ 
                            config.streamController.Camera.StreamGrabber.GrabResultWaitHandle
                            
                            //config.streamController.Camera.StreamGrabber.GrabStopWaitHandle
                        };
                        
                        //LOG("IsGrabbing="+
                        //    config.streamController.Camera.StreamGrabber.IsGrabbing.ToString()
                        //);
                        Basler.Pylon.IImage thisBaslerImage = config.ImageViewer.CaptureImage();

                        //config.imageViewer.Visibility = System.Windows.Visibility.Hidden;
                        if (thisBaslerImage != null)
                        {

                            pixelData = (byte[])thisBaslerImage.PixelData;
                            //cameraTexture.SetData<byte>((byte[])config.ImageViewer.CaptureImage().PixelData);
                            WaitHandle.WaitAll(events);
                            cameraTexture.SetData<byte>(pixelData);
                        }
                    }
                    //cameraTexture.SetData<byte>(thisPixelData);
                    //config.guiDispatcher.Thread.Resume();



                }
            
            }
            LOG("CAPTURE_cameraImage end");
        }

        //void StreamGrabber_GrabStarted(object sender, EventArgs e)
        //{
        //    LOG("StreamGrabber_GrabStarted");
        //}

        //void StreamGrabber_GrabStarting(object sender, EventArgs e)
        //{
        //    LOG("StreamGrabber_GrabStarting");
        //}

        //void StreamGrabber_GrabStopped(object sender, Basler.Pylon.GrabStopEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    LOG("StreamGrabber_GrabStopped");
        //}

        protected virtual void Draw_BaslerCamera(GameTime _gameTime)
        {
            cameraBasicEffect.Texture = CameraTexture;

            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            cameraBasicEffect.Projection = eyeProjection;
            cameraBasicEffect.View = eyeView;
            cameraBasicEffect.World = ra.cameraSurfaceWorld;
            
            // Draw the primitive using BasicEffect
            cameraSurface.Draw(cameraBasicEffect);
        }

        public void START_streaming()
        {
            if (config.streamController.Camera.StreamGrabber.IsGrabbing == false)
            {
                config.streamController.StartStreaming();
                SETUP_BaslerCamera();
            }
            if(C_State.FURTHER(e_stateBaslerCam.initialized))
            {
                C_State.SET_state( e_stateBaslerCam.streaming );
            }
        }

        public void STOP_streaming()
        {
            config.streamController.StopStreaming();
            if(C_State.FURTHER(e_stateBaslerCam.initialized))
            {
                C_State.SET_state(e_stateBaslerCam.notStreaming);
            }
        }

        public void SETUP_BaslerCamera()
        {
            START_streaming();
            cameraPixelFormat = PixelFormat.B8G8R8X8.UNorm;
            //baslerImage.PixelTypeValue = Basler.Pylon.PixelType.BGR8packed;
            if (config.ImageViewer != null)
            {

//                config.guiDispatcher.DisableProcessing();
                
                //config.guiDispatcher.Thread.Abort();
                baslerImage = config.ImageViewer.CaptureImage();
                
                if (baslerImage != null)
                {
                    //pixelData = (byte[])baslerImage.PixelData;
                    ((byte[])baslerImage.PixelData).CopyTo(pixelData, 0);
                    
                    width = config.ImageViewer.CaptureImage().Width;
                    height = config.ImageViewer.CaptureImage().Height;

                    CameraTexture = Texture2D.New(GraphicsDevice, width, height, cameraPixelFormat, pixelData, TextureFlags.ShaderResource, ResourceUsage.Dynamic);

                    CameraTexture.SetData<byte>(pixelData);

                    if (C_State.FURTHER(e_stateBaslerCam.initialized) == false)
                    {
                        C_State.SET_state(e_stateBaslerCam.initialized);
                    }
                }
                //config.guiDispatcher.Thread.Resume();
            }
        }
        public void Constructor_BaslerCamera()
        {
            //streamController = new StreamController();
            //imageViewer = new ImageViewer();


            //streamController.

            //config.streamController = streamController;
            //config.imageViewer = imageViewer;

            if (config.streamController.Camera != null)
            {
                C_State.SET_state(e_stateBaslerCam.initializing);
            }
            if (config.ReadCameraStream == true)
            {
                SETUP_BaslerCamera();
                //config.streamController.StartStreaming();
                //if (config.streamController.Camera == null)
                //{
                //    // add camear
                //    config.streamController.
                //}
                //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
                //config.imageViewer.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            }
        }

        public void LoadContent_BaslerCamera()
        {
            //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            cameraBasicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));

            // size of imaginary camera picture plane [mm]
            float sizeX = 250; // [mm]
            float sizeY = 250; // [mm]
            cameraSurface = ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice, sizeX, sizeY));
            
            // Load the texture
            //cameraTexture = Content.Load<Texture2D>("speaker");
            CameraTexture = Content.Load<Texture2D>("cameraDefault_2015-04-20_09-34-31");
            
            //cameraTexture = Texture2D.New(GraphicsDevice, width, height, PixelFormat.B8G8R8A8.UNorm);
            cameraBasicEffect.Texture = CameraTexture;
            cameraBasicEffect.TextureEnabled = true;
        }

    }
}
