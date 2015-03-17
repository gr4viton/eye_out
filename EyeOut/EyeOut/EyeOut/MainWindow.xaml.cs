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

namespace EyeOut
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        C_Motor actMot;

        public static Byte nudId = 1;

        public MainWindow()
        {
            InitializeComponent();


            actMot = new C_Motor(1);

            // old
            /*
            INIT_LOG();
            
            INIT_GUI();
            INIT_SPI();
            INIT_controlMot();

            EV_connection(e_con.port_closed);

            */
            // new
            INIT_logger();
            INIT_spi();
        }

        
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region prog status
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        bool PROG_QUITTING = false;



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            C_State.CLOSE_program();
        }


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion prog status
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void INIT_tim()
        {

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 100);
            dispatcherTimer.Start();
        }


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region arguments
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%





        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion arguments
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region simulation timer
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //LOG_UPDATE_tx();
        }

        /*
         * 
        int act_ang = 180;
        private void timSim_Tick(object sender, EventArgs e)
        {
            mot1.MOVE_absPosLastSpeed(act_ang);
            tbAng.Value = act_ang;
            txAng.Text = act_ang.ToString();
            act_ang = act_ang + 1;
        }

        private void btnTimSim_Click(object sender, EventArgs e)
        {
            switch (timSim.Enabled)
            {
                case(true):
                    btnTimSim.Background = Color.OrangeRed;
                    btnTimSim.Text = "START";
                    timSim.Enabled = false;
                    break;

                case (false):
                    btnTimSim.Background = Color.LimeGreen;
                    btnTimSim.Text = "STOP";
                    timSim.Enabled = true;
                    break;
            }
        */


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

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }



    }
}
