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

//using System.Timers;

namespace EyeOut
{
    public class BaslerCameraControl
    {
        public StreamController streamController;
        public BaslerCamera camera;
        public static PixelDataConverter converter;
        public static long destinationBufferSize;
        public static PixelType sourcePixelType = PixelType.BayerRG8;

        private static IGrabResult storedGrabResult;
        private static bool storedNewGrabResult = true;
        private static object storedGrabResult_locker = new object();

        public static bool StoredNewGrabResult
        {
            get
            {
                lock (storedGrabResult_locker)
                {
                    return storedNewGrabResult;
                }
            }
        }

        private static int executedShots = 0;
        private static object executedShots_locker = new object();

        private static Timer ShooterTimer;


        public static bool initialized = false;
        public static object initialize_locker = new object();

        public BaslerCameraControl()//StreamController guiStreamController, ImageViewer guiImageViewer, CameraLister guiCameraLister)
        {
            camera = new BaslerCamera(CameraSelectionStrategy.FirstFound);
            streamController = new StreamController();

            converter = new PixelDataConverter();
            //converter.OutputPixelFormat = PixelType.RGB8planar; // planar BBBBB ??
            converter.OutputPixelFormat = PixelType.RGB8packed; // RGB?

            camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;

            OpenCamera();
            LOG(string.Format("Model: {0}", camera.Parameters[PLCamera.DeviceModelName].GetValue() ));

            StartGrabbing();

            ////if (camera.WaitForFrameTriggerReady(100, TimeoutHandling.Return) == true)
            //{
            //    camera.ExecuteSoftwareTrigger();

        }

        public bool OpenCamera()
        {
            if (camera.IsOpen == false)
            {
                //camera.CameraOpened += Configuration.SoftwareTrigger;
                camera.CameraOpened += Configuration.AcquireContinuous;
                //camera.CameraOpened += Configuration.;

                camera.Open();
                LOG("camera opened");
                camera.Parameters[PLCamera.ExposureMode].SetValue(PLCamera.ExposureMode.Timed);
                camera.Parameters[PLCamera.ExposureTime].SetValue(10000); // in [us]
                camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(50);

                //camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);

                //streamController.Camera = camera;
                //streamController.Camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                //streamController.Camera.StreamGrabber.Stop();

                lock (initialize_locker)
                {
                    initialized = false;
                }
                return true;
            }
            return false;
        }
        public bool CloseCamera()
        {
            if (camera.IsOpen == true)
            {
                camera.Close();
                LOG("camera closed");
                return true;
            }
            return false;
        }

        public bool StartGrabbing()
        {
            if (camera.StreamGrabber.IsGrabbing == false)
            {
                camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                //camera.StreamGrabber.Start();
                //streamController.StartStreaming();
                LOG("grabbing started");
                return true;
            }
            return false;
        }

        public bool StopGrabbing()
        {
            if (camera.StreamGrabber.IsGrabbing)
            {
                //streamController.StopStreaming();
                camera.StreamGrabber.Stop();
                LOG("grabbing stopped");
                return true;
            }
            return false;
        }
        // Example of an image event handler.
        void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            LOG("OnImageGrabbed started");
            IGrabResult grabResult = e.GrabResult;
            if (grabResult.GrabSucceeded) // Image grabbed successfully?
            {
                LOG(string.Format("GrabbedImage XY={0}|{1} ", grabResult.Width, grabResult.Height));

                lock (storedGrabResult_locker)
                {
                    lock (initialize_locker)
                    {
                        if (initialized == false)
                        {
                            destinationBufferSize = converter.GetBufferSizeForConversion(sourcePixelType, grabResult.Width, grabResult.Height);
                            initialized = true;
                            LOG("destinationBufferSize initialized!");
                        }
                    }

                    storedGrabResult = grabResult.Clone();
                    storedNewGrabResult = true;
                    //LOG("Started to convert grabbed result!");
                    //byte[] rgb = ConvertGrabResultToByteArray(grabResult);
                    //LOG("Grabbed result converted to byte array!");

                }

            }
            else
            {
                LOG_err(string.Format("Unsuccessfull grab - Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription));

            } 
        }

        public byte[] ConvertGrabResultToByteArray(IGrabResult grabResult)
        {
            byte[] destinationBuffer = new byte[destinationBufferSize];
            converter.Convert<byte, byte>(destinationBuffer, (byte[])grabResult.PixelData,
                sourcePixelType, grabResult.Width, grabResult.Height,
                grabResult.PaddingX, grabResult.Orientation);
            initialized = true;
            return destinationBuffer;
        }


        public bool StartCapturingLoop()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += CaptureImageLoop_DoWork;
            bw.RunWorkerAsync();
            return true;
        }

        public void CaptureImage()
        {
            byte[] destinationBuffer = ConvertGrabResultToByteArray(storedGrabResult);
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
        
        
        public byte[] ConvertStoredGrabResultToByteArray(out int GrabResultWidth, out int GrabResultHeight)
        {
            lock (storedGrabResult_locker)
            {
                if (storedGrabResult != null)
                {
                    GrabResultWidth = storedGrabResult.Width;
                    GrabResultHeight = storedGrabResult.Height;
                    return ConvertGrabResultToByteArray(storedGrabResult);
                }
            }
            GrabResultHeight = GrabResultWidth = 0;
            return null;
        }

        public byte[] ConvertStoredGrabResultToByteArray()
        {
            lock (storedGrabResult_locker)
            {
                if (storedGrabResult != null)
                {
                    return ConvertGrabResultToByteArray(storedGrabResult);
                }
            }
            return null;
        }



        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.cam, _msg);
        }

        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.cam, _msg);
        }


        
        public void InitCameraDestinationBuffer_dontUse()
        {
            // Start grabbing.
            StartGrabbing();

            // Grab a number of images.
            //for (int i = 0; i < 2; ++i)
            while(initialized == false)
            {
                // Wait for an image and then retrieve it. A timeout of 5000 ms is used.
                IGrabResult grabResult;
                try
                {

                    if (camera.WaitForFrameTriggerReady(200, TimeoutHandling.Return) == true)
                    {
                        camera.ExecuteSoftwareTrigger();
                        Thread.Sleep(30);
                    }
                    else
                    {
                        LOG("WaitForFrameTriggerReady didn't waited enaugh");
                    }
                    grabResult = camera.StreamGrabber.RetrieveResult(200, TimeoutHandling.ThrowException);
                    using (grabResult)
                    {
                        // Image grabbed successfully?
                        if (grabResult.GrabSucceeded)
                        {
                            destinationBufferSize = converter.GetBufferSizeForConversion(sourcePixelType, grabResult.Width, grabResult.Height);

                            lock (storedGrabResult_locker)
                            {
                                storedGrabResult = grabResult.Clone();
                                initialized = true;
                            }
                        }
                        else
                        {
                            LOG_err(string.Format("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription));
                        }
                    }
                }
                catch(Exception ex)
                {
                    LOG_err(string.Format("Exception: {0}", ex.Message));
                }
            }

            // Stop grabbing.
            StopGrabbing();
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