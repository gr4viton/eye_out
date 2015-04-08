using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Windows.Media.Imaging; // BitmapSource 
using System.Runtime.InteropServices;
using System.Windows.Threading; // dispatcherTimer

using System.Windows.Controls; // SelectionChangedEventArgs

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

//DiresctShow
using DirectShowLib; // search for videosources

using System.Collections.ObjectModel; // ObservableCollection
using System.Windows; // Window
using System.Windows.Data; //CollectionViewSource

namespace EyeOut
{
    public static class BitmapSourceConvert
    {
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bs;
            }
        }
    }

    public class C_VideoDevice
    {
        public string deviceName;
        public int deviceID;
        public Guid identifier;

        public C_VideoDevice(int ID, string Name, Guid Identity = new Guid())
        {
            deviceID = ID;
            deviceName = Name;
            identifier = Identity;
        }
        public string DeviceName
        {
            get
            {
                return deviceName;
            }
        }
        public int DeviceID
        {
            get
            {
                return deviceID;
            }
        }
        public Guid Identifier
        {
            get
            {
                return identifier;
            }
        }

        /// <summary>
        /// Represent the Device as a String
        /// </summary>
        /// <returns>The string representation of this color</returns>
        public override string ToString()
        {
            return String.Format("[{0}] {1}:\n\t{2}", deviceID, deviceName, identifier);
        }
    }


    public class C_Camera
    {
        public static ObservableCollection<C_VideoDevice> camList;
        public static int numCamSources;
        public static int actualId;

        private Capture capture;        //takes images from camera as image frames
        public int id;
        private double camLatency;

        private bool captureInProgress; // not used anymore?

        public C_Camera(int _id)
        {
            id = _id;
            INIT_capture();
        }

        public C_Camera(int _id, double _camLatency)
        {
            id = _id;
            camLatency = _camLatency; // 40ms - 
            INIT_capture();
        }

        public void INIT_capture()
        {
            if (capture == null)
            {
                try
                {
                    capture = new Capture(id);
                    //videoCapture.set(CV_CAP_PROP_FRAM_WIDTH, CAMERA_WIDTH);
                    //vid.width =cam.width
                    //vid.height =cam.height
                    //videoCapture.set(CV_CAP_PROP_FPS, 60);
                    // that is took care of automatically
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
        }

        public BitmapSource GET_frame()
        {
            Image<Bgr, byte> ImageFrame = capture.QueryFrame();

            return BitmapSourceConvert.ToBitmapSource(ImageFrame);
        }

        public BitmapSource GET_txu()
        {
            Image<Bgr, byte> ImageFrame = capture.QueryFrame();

            return BitmapSourceConvert.ToBitmapSource(ImageFrame);
        }


        public void TOGGLE_capture()
        {
            captureInProgress = !captureInProgress;
        }

        public void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }


        public double CamLatency
        {
            get
            {
                return camLatency;
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.cam, _msg);
        }

        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.cam, _msg);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }
}
