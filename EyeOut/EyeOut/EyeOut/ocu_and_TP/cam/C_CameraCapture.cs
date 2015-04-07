using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; // backgroundWorker

using SharpDX;
using SharpDX.Direct3D11;
using SharpOVR;

using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

using System.Windows.Media.Imaging; // BitmapSource 
using System.Runtime.InteropServices;
using System.Windows.Threading; // dispatcherTimer

//using SharpDX.Direct2D1; // text d3d10
//using SharpDX.DirectWrite; // text d3d10
using SharpDX.Direct3D9; // text d3d9

namespace EyeOut
{
    public class C_CaptureData
    {
        //cv::Mat image;
        BitmapSource image;
        //OVR.posef pose;
        public C_CaptureData(BitmapSource _image)
        {
            image = _image;
        }

        public BitmapSource Image
        {
            get { return image; }
        }
    }

    public class C_CameraCaptureHandler
    {
        // instance of class interacting with camera
        private C_Camera cam;  // resp in fact I can use Capture & all the conversion would be defined here..
        //private Capture capture;        //takes images from camera as image frames
        //public static int actualId;

        private C_CaptureData captureData;

        private object captureData_locker = new object();

        private SharpOVR.HMD hmd; // for fetching the headpose
        bool isStopped;

        public C_CameraCaptureHandler(SharpOVR.HMD _hmd, int _camId)
        {
            // open the camera and set it up
            cam = new C_Camera(_camId);
            hmd = _hmd;
            isStopped = true;
        }

        public void startCapture()
        {
            isStopped = false;
            startCaptureLoop();
        }

        private void startCaptureLoop()
        {
            // better to create [Backgroundworker with the loop] inside C_CaptureDataHandler then in upper
            // because if I would have more cameras each would create its loop separately :)

            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += captureLoop_RunWorkerCompleted;
            worker.DoWork += captureLoop_DoWork;
            //worker.RunWorkerAsync((object)cmd);
            worker.RunWorkerAsync();
        }

        public void stopCapture()
        {
            isStopped = true;
        }

        bool newImgReady = true;

        // with a locker
        public C_CaptureData CaptureData // nullable
        {
            get
            {
                lock (captureData_locker)
                {
                    if (newImgReady)
                    {
                        // with act MOTOR pose
                        // act Cam img
                        return captureData;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                lock (captureData_locker)
                {
                    captureData = value;
                }
            }
        }


        private void captureLoop_DoWork(object sender, DoWorkEventArgs e)
        {

            while (isStopped == false)
            {
                captureData = new C_CaptureData(
                    cam.GET_txu()
                    //,MOT.GET_position();
                        );


                //// capture the headpose
                //S_CaptureData captured;
                //double captureTime = OVR.GetTimeInSeconds() - cam.CamLatency; // subtract a camera lag from pose

                // SDK can not predict in back time 
                // -> so tweak the sdk 
                // -> or make a abstract layer to store older positions
                // looks like this SDK through SharpDX cannot predict at all
                // so make predictions?

                //SharpOVR.TrackingState tracking = SharpOVR.TrackingCapabilities.Orientation(hmd, captureTime);

                //var pose = SharpOVR.TrackingCapabilities.Orientation;

                //captured.pose = SharpOVR.TrackingCapabilities.Position;

                //// capture the image
                //if(!videoCapture.grab() || !videoCapture.retrieve(captured.image))
                //{
                //    //Failed video capture
                //    LOG_err("Failed video capture");
                //}

                //cv::flip(captured.image.clone(), captured.image, 0); // opencv vs opengl ? vs directX
                //setResult(captured);

            }
            //e.Result = C_SPI.WriteData(e.Argument as byte[]);
        }

        private void captureLoop_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // catch if response was A-OK
            if (e.Error != null)
            {
                LOG_err(String.Format("Camera id#{2} had an error:\n{0}\n{1}", e.Error.Data, e.Error.Message, cam.id));
                //ie Helpers.HandleCOMException(e.Error);
            }
            else
            {
                //e.Result = "tocovrati writeData";
                //MyResultObject result = (MyResultObject)e.Result;

                //LOG("DATA SENT");
                //var results = e.Result as List<object>;
            }
        }


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.EyeOut_cam, _msg);
        }

        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.EyeOut_cam, _msg);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }
}
