using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO.Ports;
using System.Threading;
using System.Timers;

using System.Collections;

using System.Data; // datagrid
using System.Collections.ObjectModel; // ObservableCollection
using System.Windows.Threading; // dispatcherTimer

namespace EyeOut
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer timSim;
        public static C_Packet raw;

        //public Keyboard Keyboard { get; }

        public MainWindow()
        {
            C_State.prog = e_stateProg.initializing;
            InitializeComponent();

            /*
            EV_connection(e_con.port_closed);
            */
            // new
            INIT_logger();
            INIT_spi();
            INIT_allMotors();
            INIT_timSim();
            //INIT_cam();
            //INIT_Telepresence();
            INIT_about();
            INIT_keyMapping();

            C_State.prog = e_stateProg.initialized;

        }

        public void HOTKEY_StartTelepresence()
        {
            tbtToggleTP.IsChecked = true;
            START_TP_withCaution();
        }
        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void StartTelepresence(object sender, ExecutedRoutedEventArgs e)
        {
            HOTKEY_StartTelepresence();
            e.Handled = true;
        }


        private void INIT_keyMapping()
        {
        }
        //private void OnWindowKeyUp(object source, KeyEventArgs e)
        //{

        //    if (e.Key == Key.T && (Keyboard.Modifiers & Key.LeftCtrl) == Key.LeftCtrl)
        //    {

        //    }

        //    //Do whatever you like with e.Key and Keyboard.Modifiers
        //}


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region prog status
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            C_State.CLOSE_program();
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion prog status
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void INIT_timSim()
        { 
            timSim = new DispatcherTimer();
            timSim.Tick += new EventHandler(timSim_Tick);
            timSim.Interval = new TimeSpan(0, 0, 0, 100);
            //timSim.Start();
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properies
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%





        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properies
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region simulation timer
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        private void timSim_Tick(object sender, EventArgs e)
        {
            /*
            mot1.MOVE_absPosLastSpeed(act_ang);
            tbAng.Value = act_ang;
            txAng.Text = act_ang.ToString();
            act_ang = act_ang + 1;*/
            //M(actMrot).angle.Dec += 0.1;
        }

        private void btnTimSim_Toggle(object sender, EventArgs e)
        {
            switch (timSim.IsEnabled)
            {
                case (true):
                    /*
                        btnTimSim.Background = Color.OrangeRed;
                        btnTimSim.Text = "START";*/
                    timSim.Stop();
                    break;

                case (false):
                    /*
                        btnTimSim.Background = Color.LimeGreen;
                        btnTimSim.Text = "STOP";*/
                    timSim.Start();
                    break;
            }
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion simulation timer
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // redo by binding

        private void status_connected()
        {
            tslConnected.Content = "Connected";
            tslConnected.Background = Brushes.LimeGreen;
            //spsControl.Enabled = false;
            gpCmds.IsEnabled = true;

            btnConnect.Content = "Disconnect Serial";

        }
        private void status_disconnected()
        {
            tslConnected.Content = "Not connected";
            tslConnected.Background = Brushes.OrangeRed;
            //spsControl.Enabled = true;
            gpCmds.IsEnabled = false;

            btnConnect.Content = "Connect Serial";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSendRawBytes_Click(object sender, RoutedEventArgs e)
        {
            C_Packet.SEND_packet(raw);
        }

        private void txRawBytes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (C_State.FURTHER(e_stateProg.initialized))
            {
                try
                {
                    raw = new C_Packet(C_CONV.strHex2byteArray(txRawBytes.Text, " "));
                }
                finally
                {
                    btnSendRawBytes.IsEnabled = raw.IsConsistent;
                }
            }
        }

        private void btnDontTry_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Ms[e_rot.yaw].ACTUALIZE_register(36, 20, e_regByteType.sentValue);
        }
        private void btnTry_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(
                MainWindow.Ms[e_rot.yaw].Reg.GET(36, e_regByteType.sentValue).Val.ToString()
            );
            MainWindow.Ms[e_rot.yaw].ACTUALIZE_register(36, 10, e_regByteType.sentValue);
            
            str.AppendLine(
                MainWindow.Ms[e_rot.yaw].Reg.GET(36, e_regByteType.sentValue).Val.ToString()
                );

            MessageBox.Show(str.ToString());
        }

        private void tcMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (C_State.FURTHER(e_stateProg.initialized))
            {

                if (tiBaslerCamera.IsSelected == true)
                {
                    dpBaslerCamera.Visibility = Visibility.Visible;
                }
                else
                {
                    dpBaslerCamera.Visibility = Visibility.Hidden;
                }
            }
        }
        
        //public event EventHandler<ImageGrabbedEventArgs> ImageGrabbed


        void GUI_StreamGrabber_ImageGrabbed(object sender, Basler.Pylon.ImageGrabbedEventArgs e)
        {

            Basler.Pylon.IImage im = (Basler.Pylon.IImage)e.GrabResult;
                //guiImageViewer.CaptureImage();

            if (im == null)
            {
                LOG_gui("Could not retrieve grabbed image");
            }
            else
            {
                //byte[] pixelData = (byte[])im.PixelData;
                LOG_gui("Retrieved grabbed image (in gui event handler)");
            }
            //throw new NotImplementedException();
        }


        private void lsBaslerCameraCommands_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (lsBaslerCameraCommands.SelectedIndex)
            {
                case(0): 
                    // event EventHandler<ImageGrabbedEventArgs> ImageGrabbed // - possible continuous grabbing, and choose from grabbed list when drawing into scene
                    if(guiStreamController.Camera.IsOpen)
                    //if (streamController.Camera.StreamGrabber.IsGrabbing == false)
                    {

                        //event EventHandler<ImageGrabbedEventArgs> ImageGrabbed
                        guiStreamController.Camera.StreamGrabber.ImageGrabbed += GUI_StreamGrabber_ImageGrabbed;
                        //streamController.Camera.StreamGrabber.
                        // setup grabber
                        // ...
                        // start grabber
                        guiStreamController.Camera.Open();
                    }
                    //streamController.Camera.Open();
                    break;
                case (1): 
                    if(guiStreamController.Camera.IsOpen)
                        //if (streamController.Camera.StreamGrabber.IsGrabbing == false)
                        {
                            //streamController.Camera.StreamGrabber.Start();
                            guiStreamController.StartStreaming();
                            LOG_gui("Started streamGrabbing");
                        }
                    break;
                case (2):
                    if (guiStreamController.Camera.IsOpen)
                        //if (streamController.Camera.StreamGrabber.IsGrabbing == true)
                        {
                            //streamController.Camera.StreamGrabber.Stop();
                            guiStreamController.StopStreaming();
                            LOG_gui("Stopped streamGrabbing");
                        }

                    break;
                case (3):
                    if (guiStreamController.Camera.IsOpen)
                    {
                        guiStreamController.TakeSingleSnapshot();// Camera.StreamGrabber.take;
                        LOG_gui("Took single snapshot");
                    }

                    break;

            }
        }


        BaslerCameraControl camTry;
        private void btnStartBaslerCameraImageAcquisition_Click(object sender, RoutedEventArgs e)
        {
            if (guiCameraLister.Camera != null)
            {
                guiCameraLister.Camera.Close();
            }
            if (camTry == null)
                camTry = new BaslerCameraControl();//(guiStreamController, guiImageViewer, guiCameraLister);

            camTry.StartGrabbing();
        }


        private void btnAcquireImage_Click(object sender, RoutedEventArgs e)
        {
            //if (camTry != null)
            //    camTry.CaptureImage();
        }

        private void btnStopBaslerCameraImageAcquisition_Click(object sender, RoutedEventArgs e)
        {
            if (camTry != null)
                camTry.StopGrabbing();
        }

        private void btnStartCaptureLoop_Click(object sender, RoutedEventArgs e)
        {
            //if (camTry != null)
            //    camTry.StartCapturingLoop();
        }

        private void btnCloseCamera_Click(object sender, RoutedEventArgs e)
        {
            if (camTry != null)
                camTry.CloseCamera();
        }

        private void btnSetLogFilePath_Click(object sender, RoutedEventArgs e)
        {
            C_Logger.Instance.filePath = @txLogFilePath.Text;
        }





    }
}
