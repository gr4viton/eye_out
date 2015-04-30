using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamController = Basler.Pylon.Controls.WPF.StreamController;
using ImageViewer = Basler.Pylon.Controls.WPF.ImageViewer;
using CameraLister = Basler.Pylon.Controls.WPF.CameraLister;
using BaslerCamera = Basler.Pylon.Camera;

using Basler.Pylon;

using System.Windows;
using System.Threading;
//using System.Windows.Threading;
using System.ComponentModel;

namespace EyeOut
{
    class C_cameraTry
    {
        public ImageViewer imageViewer;
        public StreamController streamController;
        public CameraLister cameraLister;
        public BaslerCamera camera;
        static public PixelDataConverter converter;
        static long destinationBufferSize;
        static PixelType sourcePixelType = PixelType.BayerRG8;

        public C_cameraTry(StreamController guiStreamController, ImageViewer guiImageViewer, CameraLister guiCameraLister)
        {

            camera = new BaslerCamera();

            //camera.CameraOpened += Basler.Pylon.Configuration.AcquireContinuous;


            // Open the connection to the camera device.
            camera.Open();


            if (camera.IsOpen == false)
                LOG("neni otevrena");

            LOG(string.Format("Model            : {0}",
                camera.Parameters[PLCamera.DeviceModelName].GetValue()
                ));

            converter = new PixelDataConverter();
            converter.OutputPixelFormat = PixelType.RGB8planar;
            


            
            //// Close the connection to the camera device.
            //camera.Close();


            //SETUP();

        }
        
        public void StartGrabbing()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += GrabImageLoop_DoWork;
            bw.RunWorkerAsync();
        }

        public void GrabImageLoop_DoWork(object sender, DoWorkEventArgs e)
        {
            GrabTen();

            // Set a handler for processing the images.
            camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;

            // Start grabbing using the grab loop thread. This is done by setting the grabLoopType parameter
            // to GrabLoop.ProvidedByStreamGrabber. The grab results are delivered to the image event handler OnImageGrabbed.
            // The default grab strategy (GrabStrategy_OneByOne) is used.
            //camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);

            bool GrabImages = true ;
            // Software triggering is used to trigger the camera device.
            while (GrabImages)
            {
                if (camera.StreamGrabber.IsGrabbing)
                {
                    // Execute the software trigger. Wait up to 100 ms until the camera is ready for trigger.
                    //if (camera.WaitForFrameTriggerReady(5000, TimeoutHandling.ThrowException))
                    if (camera.WaitForFrameTriggerReady(100, TimeoutHandling.Return) == true)
                    {
                        camera.ExecuteSoftwareTrigger();
                        Thread.Sleep(10);
                    }
                    else
                    {
                        LOG("WaitForFrameTriggerReady didn't waited enaugh");
                    }
                }
                else
                {
                    GrabImages = false;
                }
            }

            // Stop grabbing.
            //camera.StreamGrabber.Stop();
        }

        public void StartCapturing()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += CaptureImageLoop_DoWork;
            bw.RunWorkerAsync();
        }

        public void CaptureImage()
        {
            LOG(string.Format("RGB of first pixel: {0}|{1}|{2}", destinationBuffer[0], destinationBuffer[1], destinationBuffer[2]));

        }

        public void CaptureImageLoop_DoWork(object sender, DoWorkEventArgs e)
        {
            bool CaptureImages = true;
            while (CaptureImages)
            {
                CaptureImage();
                Thread.Sleep(1);
            }
        }

        public void StopGrabbing()
        {
            if (camera.StreamGrabber.IsGrabbing)
            {
                camera.StreamGrabber.Stop();
            }
        }

        public void GrabTen()
        {
            // Start grabbing.
            camera.StreamGrabber.Start();

            // Grab a number of images.
            //for (int i = 0; i < 2; ++i)
            while(initialized == false)
            {
                // Wait for an image and then retrieve it. A timeout of 5000 ms is used.
                IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(100, TimeoutHandling.ThrowException);
                using (grabResult)
                {
                    // Image grabbed successfully?
                    if (grabResult.GrabSucceeded)
                    {
                        // Access the image data.
                        LOG(string.Format("XY={0}|{1} ", grabResult.Width, grabResult.Height));
                        byte[] sourceBuffer = grabResult.PixelData as byte[];

                        ////grabResult.Width
                        //LOG(string.Format("Gray value of first pixel: {0}", buffer[0]));
                        LOG(string.Format(""));


                        //IImage
                        destinationBufferSize = converter.GetBufferSizeForConversion(sourcePixelType, grabResult.Width, grabResult.Height);

                        lock (destinationBuffer_locker)
                        {
                            destinationBuffer = new byte[destinationBufferSize];
                            converter.Convert<byte, byte>(destinationBuffer, sourceBuffer,
                                sourcePixelType, grabResult.Width, grabResult.Height,
                                grabResult.PaddingX, grabResult.Orientation);
                            initialized = true;
                        }
                    }
                    else
                    {
                        LOG(string.Format("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription));
                    }
                }
            }

            // Stop grabbing.
            camera.StreamGrabber.Stop();
        }

        static object destinationBuffer_locker = new object();
        static byte[] destinationBuffer;
        static bool initialized = false;

        // Example of an image event handler.
        static void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            if (initialized == true)
            {
                // The grab result is automatically disposed when the event call back returns.
                // The grab result can be cloned using IGrabResult.Clone if you want to keep a copy of it (not shown in this sample).
                IGrabResult grabResult = e.GrabResult;
                // Image grabbed successfully?
                if (grabResult.GrabSucceeded)
                {
                    // Access the image data.
                    LOG(string.Format("GrabbedImage XY={0}|{1} ", grabResult.Width, grabResult.Height));
                    byte[] sourceBuffer = grabResult.PixelData as byte[];


                    lock (destinationBuffer_locker)
                    {
                        converter.Convert<byte, byte>(destinationBuffer, sourceBuffer,
                            sourcePixelType, grabResult.Width, grabResult.Height,
                            grabResult.PaddingX, grabResult.Orientation);

                    }
                }
                else
                {
                    LOG(string.Format("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription));
                }
            }
        }





        public void GetFirstCameraFromCameraLister()
        {
            if (cameraLister.Camera == null)
            {
                int res = 0;
                foreach (var cameraModel in cameraLister.CameraList)
                {
                    if (res == 0)
                    {
                        //camera.Camera //cameraLister.Camera = cameraModel.Camera;
                    }
                    res++;
                }
                //if (cameraLister.Camera != null)
                //{
                //    // inform that this telepresence will use the first camera from connected camera list
                //    MessageBox.Show("No Basler camera selected in 'Basler camera' tab!\nThis telepresence session will use the first one from the camera list:\n"
                //    + cameraLister.Camera.CameraInfo.ToString());
                //}
                if (res == 0) // no cameras found
                {
                    // inform that the camera is not going to be assigned in this telepresence settings as the camera is not connected
                    MessageBox.Show("No Basler camera found!\nThere will be no streamed camera image in this telepresence session!\n"
                        + "Please connect Basler camera to some port (USB3.0 for Basler acA2040-90uc) and check whether it is found in 'Basler camera' tab!",
                        "No Basler camera found!", MessageBoxButton.OK);
                }
            }
        }

        //public void CaptureImageLoop(object sender, DoWorkEventArgs e)
        //{
        //    WaitHandle[] events = new WaitHandle[]{ 
        //            streamController.Camera.StreamGrabber.GrabResultWaitHandle
        //            //streamController.Camera.StreamGrabber.GrabStopWaitHandle
        //        };
        //    while (true)
        //    {
        //        CAPTURE_image();

        //    }
        //}
        
        public void CAPTURE_image()
        {
            if (imageViewer != null)
            {
                Basler.Pylon.IImage thisBaslerImage = imageViewer.CaptureImage();
                if (thisBaslerImage != null)
                {
                    int pixelDataSize = thisBaslerImage.Width * thisBaslerImage.Height * 4; 
                    byte[] pixelData = (byte[])thisBaslerImage.PixelData; // reference
                    byte[] pixelDataToAdd = new byte[pixelDataSize]; // new data space
                    pixelData.CopyTo(pixelDataToAdd, 0); // copy
                }
            }
        }

        public void SETUP()
        {
            OPEN_camera();
            START_streaming();
            
        }

        public void START_streaming()
        {
            if (streamController.Camera.StreamGrabber.IsGrabbing == false)
            {
                streamController.StartStreaming();
            }
        }

        public void STOP_streaming()
        {
            if (streamController.Camera.StreamGrabber.IsGrabbing == true)
            {
                streamController.StopStreaming();
            }
        }

        public void OPEN_camera()
        {
            if (streamController.Camera.IsOpen == false)
            {
                streamController.Camera.Open();
            }
        }
        public void CLOSE_camera()
        {
            if (streamController.Camera.IsOpen == true)
            {
                streamController.Camera.Close();
            }
        }


        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.cam, _msg);
        }

        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.mot, _msg);
        }
    }
}






















        //    //public void CAPTURE_cameraImage_new()
        //    //{
        //    //    if (C_State.FURTHER(e_stateBaslerCam.initialized))
        //    //    {
        //    //        START_streaming();
        //    //        //baslerImage = config.streamController.Camera.StreamGrabber();
        //    //        baslerImage = config.ImageViewer.CaptureImage();

        //    //        if (baslerImage != null)
        //    //        {
        //    //            pixelData = (byte[])baslerImage.PixelData;
        //    //            if (pixelData.Length != CameraTexture.Width * CameraTexture.Height * 4)
        //    //            {
        //    //                SETUP_BaslerCamera();
        //    //            }
        //    //            else
        //    //            {
        //    //                CameraTexture.SetData<byte>(pixelData);
        //    //            }
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        SETUP_BaslerCamera();
        //    //    }
        //    //}

        //public bool firstPass = true;
        //public int pixelDataSize;
        //public void CAPTURE_cameraImage()
        //{
        //    LOG("CAPTURE_cameraImage start");

        //    //if( config.streamController.Camera.StreamGrabber.IsGrabbing

        //    //      This method throws exceptions on timeout Basler.Pylon.TimeoutHandling.
        //    //     Grabbing single images using a software trigger (see Basler.Pylon.Configuration.SoftwareTrigger(System.Object,System.EventArgs))
        //    //     is recommended if you want to maximize frame rate.  This is because the overhead
        //    //     per grabbed image is reduced compared to single frame acquisition (see Basler.Pylon.Configuration.AcquireSingleFrame(System.Object,System.EventArgs)).
        //    //      The grabbing can be started using Basler.Pylon.IStreamGrabber.Start(Basler.Pylon.GrabStrategy,Basler.Pylon.GrabLoop).
        //    //      Images are grabbed using the Basler.Pylon.ICamera.WaitForFrameTriggerReady(System.Int32,Basler.Pylon.TimeoutHandling),
        //    //     Basler.Pylon.ICamera.ExecuteSoftwareTrigger(), and Basler.Pylon.IStreamGrabber.RetrieveResult(System.Int32,Basler.Pylon.TimeoutHandling)
        //    //     methods instead of using Basler.Pylon.IStreamGrabber.GrabOne(System.Int32).
        //    //      Grabbing can be stopped using Basler.Pylon.IStreamGrabber.Stop() when done.

        //    //config.streamController.StartStreaming();

        //    //config.ImageViewer.Camera.WaitForFrameTriggerReady(1, Basler.Pylon.TimeoutHandling.Return);
        //    //config.ImageViewer.Camera.ExecuteSoftwareTrigger();
        //    //Basler.Pylon.IGrabResult res = config.ImageViewer.Camera.StreamGrabber.RetrieveResult(1, Basler.Pylon.TimeoutHandling.Return); // another stream is waiting for the result
        //    //HUD.AppendLine(res.PixelTypeValue.ToString());

        //    //lock (locker_pixelData)
        //    //if(false)
        //    {
        //        if (firstPass == true)
        //        {
        //            if (config.streamController.Camera.IsOpen == false)
        //            {
        //                config.streamController.Camera.Open();
        //            }

        //            //config.streamController.Camera.StreamGrabber.GrabStopped += StreamGrabber_GrabStopped;
        //            //config.streamController.Camera.StreamGrabber.GrabStarting += StreamGrabber_GrabStarting;
        //            //config.streamController.Camera.StreamGrabber.GrabStarted += StreamGrabber_GrabStarted;

        //            config.streamController.StartStreaming();
        //            //config.streamController.TakeSingleSnapshot();

        //            cameraPixelFormat = PixelFormat.B8G8R8X8.UNorm;

        //            if (config.ImageViewer != null)
        //            {
        //                Basler.Pylon.IImage thisBaslerImage = config.ImageViewer.CaptureImage();
        //                //config.imageViewer.Visibility = System.Windows.Visibility.Hidden;
        //                if (thisBaslerImage != null)
        //                {
        //                    pixelData = (byte[])thisBaslerImage.PixelData;
                            
        //                    width = config.ImageViewer.CaptureImage().Width;
        //                    height = config.ImageViewer.CaptureImage().Height;
        //                    pixelDataSize = width*height*4;
        //                    CameraTexture = Texture2D.New(GraphicsDevice, width, height, cameraPixelFormat, pixelData, 
                                
        //                        TextureFlags.ShaderResource, ResourceUsage.Dynamic);

                            
        //                    firstPass = false;

        //                }
        //                //config.streamController
        //                //config.imageViewer.Visibility = System.Windows.Visibility.Visible;
        //            }
        //        }
        //        else
        //        {
        //            //config.streamController.TakeSingleSnapshot();
        //            //config.guiDispatcher.Thread.Suspend();
        //            //pixelData = (byte[])config.imageViewer.CaptureImage().PixelData;
        //            //byte[] thisPixelData = new byte[pixelDataSize];
        //            //thisPixelData = (byte[])config.imageViewer.CaptureImage().PixelData;
        //            //((byte[])baslerImage.PixelData).CopyTo(thisPixelData, 0);

        //            //LOG("reading data start");
        //            //byte[] thisPixelData = new byte[pixelDataSize];
        //            //byte[] thisPixelData2 = new byte[pixelDataSize];
        //            ////config.ImageViewer.Camera.StreamGrabber.GrabResultWaitHandle.WaitOne(10);
        //            //config.ImageViewer.Camera.StreamGrabber.Stop();
        //            //thisPixelData = (byte[])config.ImageViewer.CaptureImage().PixelData;
        //            //LOG("reading data end");
        //            //LOG("copying byte array start");

        //            ////config.ImageViewer.Camera.StreamGrabber.GrabStopWaitHandle.WaitOne(10);

        //            //thisPixelData.CopyTo(thisPixelData2,0);
        //            //config.ImageViewer.Camera.StreamGrabber.Start();

        //            //LOG("setting texture start");
        //            //CameraTexture.SetData<byte>(thisPixelData2);
        //            //LOG("setting texture end");

        //            if (config.ImageViewer != null)
        //            {
        //                WaitHandle[] events = new WaitHandle[]{ 
        //                    config.streamController.Camera.StreamGrabber.GrabResultWaitHandle
                            
        //                    //config.streamController.Camera.StreamGrabber.GrabStopWaitHandle
        //                };
                        
        //                //LOG("IsGrabbing="+
        //                //    config.streamController.Camera.StreamGrabber.IsGrabbing.ToString()
        //                //);
        //                Basler.Pylon.IImage thisBaslerImage = config.ImageViewer.CaptureImage();

        //                //config.imageViewer.Visibility = System.Windows.Visibility.Hidden;
        //                if (thisBaslerImage != null)
        //                {

        //                    pixelData = (byte[])thisBaslerImage.PixelData;

        //                    byte[] pixelDataToAdd = new byte[pixelDataSize];
        //                    pixelData.CopyTo(pixelDataToAdd, 0);
                            
        //                    queuePixelData.Enqueue(pixelDataToAdd);
        //                    //cameraTexture.SetData<byte>((byte[])config.ImageViewer.CaptureImage().PixelData);
        //                    //WaitHandle.WaitAll(events);
        //                    if (queuePixelData.Count == config.cameraFrameQueueLength )
        //                    {
        //                        cameraTexture.SetData<byte>(queuePixelData.Dequeue());
        //                    }
        //                }
        //            }
        //            //cameraTexture.SetData<byte>(thisPixelData);
        //            //config.guiDispatcher.Thread.Resume();
        //        }
        //    }
        //    //LOG("CAPTURE_cameraImage end");
        //}

        //}