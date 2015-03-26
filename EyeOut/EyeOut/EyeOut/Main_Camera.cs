using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows;


using System.Windows.Media.Imaging; // BitmapSource 
using System.Runtime.InteropServices;
using System.Windows.Threading; // dispatcherTimer


using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;




namespace EyeOut
{
    /// <summary>
    /// Camera - gui
    /// </summary>
    /// 

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


    public class C_Camera
    {
        private Capture capture;        //takes images from camera as image frames
        private bool captureInProgress;
        //System.Windows.Controls.Image img;
        public int id;


        public C_Camera(int _id)
        {
            //haarCascade = new HaarCascade(@"haarcascade_frontalface_alt_tree.xml");
            id = _id;
            INIT_capture();            
        }
        public void INIT_capture()
        {
            if (capture == null)
            {
                try
                {                 
                    capture = new Capture(id);
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
        }

        public BitmapSource GET_frame()
        {
            Image<Bgr, Byte> ImageFrame = capture.QueryFrame();

            return BitmapSourceConvert.ToBitmapSource(ImageFrame);
        }

        public void TOGGLE_capture()
        {
            captureInProgress = !captureInProgress;
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }
    }


    public partial class MainWindow : Window
    {

        public DispatcherTimer timCam;
        public List<C_Camera> Cs;

        private void INIT_cam()
        {
            Cs = new List<C_Camera>();
            Cs.Add(new C_Camera(0));
            Cs.Add(new C_Camera(1));
            INIT_timCam();
        }

        private void INIT_timCam()
        {
            timCam = new DispatcherTimer();
            timCam.Tick += new EventHandler(timer_Tick);
            timCam.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timCam.Start();
        }


        void timer_Tick(object sender, EventArgs e)
        {
            //ProcessFrame();
            /*
            foreach (C_Camera cam in Cs)
            {
                imgMain.Source = cam.GET_frame();
                imgMini.Source = cam.GET_frame();
            }*/
            imgMain.Source = Cs[0].GET_frame();
            imgMini.Source = Cs[1].GET_frame();
        }

        public void TIMER_run()
        {
            /*
            if (captureInProgress)
            {  //if camera is getting frames then stop the capture and set button Text
                timCam.Stop();
                //Application.Idle -= ProcessFrame;
            }
            else
            {
                //if camera is NOT getting frames then start the capture and set button
                timCam.Start();
                //Application.Idle += ProcessFrame;
            }
            captureInProgress = !captureInProgress;
             */
        }
        //btnStart_Click() function is the one that handles our "Start!" button' click 
        //event. it creates a new capture object if its not created already. e.g at first time
        //starting. once the capture is created, it checks if the capture is still in progress,
        //if so the

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Cs.TOGGLE_capture();
        }


        private void CameraCapture_Load(object sender, EventArgs e)
        {

        }
    }
}
