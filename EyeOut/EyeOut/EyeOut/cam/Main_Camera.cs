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

using System.Windows.Controls; // SelectionChangedEventArgs

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

//DiresctShow
using DirectShowLib; // search for videosources


using System.Collections.ObjectModel; // ObservableCollection
using System.Windows.Data; //CollectionViewSource


namespace EyeOut
{
    /// <summary>
    /// Camera - gui
    /// </summary>
    public partial class MainWindow : Window
    {

        public DispatcherTimer timCam;
        public static List<System.Windows.Controls.Image> camImages;

        private object dgCam_lock; // lock for datagrid
        public List<C_Camera> Cs;

        private void INIT_cam()
        {
            INIT_allSources();
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
            if (tbtActivePreviewList.IsChecked == true)
            {
                PLOT_listPreviewImages();
            }

            if (tbtActivePreviewActual.IsChecked == true)
            {
                PLOT_activePreviewImage();
            }

            if (C_State.FURTHER(e_state.closing))
            {
                timCam.Stop();
            }
        }
        void PLOT_listPreviewImages()
        {
            for (int q = 0; q < C_Camera.numCamSources; q++)
            {
                camImages[q].Source = Cs[q].GET_frame();
            }
        }
        void PLOT_activePreviewImage()
        {
            if (tcMain.SelectedItem == tiCamera)
            {
                imgMain.Source = Cs[C_Camera.actualId].GET_frame();
            }
            else if (tcMain.SelectedItem == tiTelepresence)
            {
                imgMain_TP.Source = Cs[C_Camera.actualId].GET_frame();
            }
        }

        void DISPOSE_old()
        {
            if(C_State.FURTHER(e_state.initialized))
            {
                if(Cs.Count > 0)
                {
                    foreach(C_Camera cam in Cs)
                    {
                        cam.ReleaseData();
                    }
                }
            }
        }
        void INIT_allSources()
        {
            DISPOSE_old();
            // ??????
            //Add the image processing to the dispatcher
            this.Dispatcher.Hooks.DispatcherInactive += new EventHandler(dispatcherTimer_Tick);

            //-> Find systems cameras with DirectShow.Net dll
            //Get the information about the installed cameras and add the combobox items 
            DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            C_Camera.numCamSources = _SystemCamereas.Length;
            C_Camera.camList = new ObservableCollection<C_VideoDevice>();
            //C_Camera.camList = new ObservableCollection<C_VideoDevice>(C_Camera.numCamSources);

            camImages = new List<System.Windows.Controls.Image>(C_Camera.numCamSources);

            for (int i = 0; i < C_Camera.numCamSources; i++)
            {
                //fill web cam array
                C_Camera.camList.Add(
                    new C_VideoDevice(i, _SystemCamereas[i].Name, _SystemCamereas[i].ClassID)
                    );
                camImages.Add(new System.Windows.Controls.Image());
                
                
                //lsCams.Items.Add(C_Camera.camList[i].ToString()); // --> will be done through binding
            }

            // load up cameras
            Cs = new List<C_Camera>(C_Camera.numCamSources);
            foreach (C_VideoDevice source in C_Camera.camList)
            {
                Cs.Add(new C_Camera(source.deviceID));
            }

            INIT_dgCam();
            UPDATE_spCamPreview();

            INIT_timCam();
            if (dgCams.Items.Count > 0)
            {
                dgCams.SelectedIndex = 0; //Set the selected device the default
                //captureButton.Enabled = true; //Enable the start
            }
            else
            {
                C_Camera.LOG_err("No source video found! Connect some video capture equipment and click [Rescan video sources]");
                return; // error - no sources of data - clic on refresh after you connect some video equipment

            }
            PLOT_listPreviewImages();
            PLOT_activePreviewImage();
        }

        private void btnCamSourcesRefresh_Click(object sender, RoutedEventArgs e)
        {
            timCam.Stop();
            INIT_allSources();
        }



        private void UPDATE_spCamPreview()
        {
            spCamPreview.Children.Clear();
            foreach(System.Windows.Controls.Image im in camImages)
            {

                spCamPreview.Children.Add(im);
            }
        }
        private void INIT_dgCam()
        {

            // binding
            CollectionViewSource ItemCollectionViewSource_cam;
            ItemCollectionViewSource_cam = (CollectionViewSource)(FindResource("ItemCollectionViewSource_cam"));
            ItemCollectionViewSource_cam.Source = C_Camera.camList;

            // when binding is changing inner guts of dataGrid from different thread
            dgCam_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(C_Camera.camList, dgCam_lock); // for multi-thread updating

        }

        private void dgCams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            C_Camera.actualId = dgCams.SelectedIndex;
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
