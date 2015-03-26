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
        public MainWindow()
        {
            C_State.prog = e_state.initializing;
            InitializeComponent();

            /*
            EV_connection(e_con.port_closed);
            */
            // new
            INIT_logger();
            INIT_spi();
            INIT_mot();
            INIT_tim();
            INIT_cam();
            C_State.prog = e_state.initialized;
        }
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
        public void INIT_tim()
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


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //LOG_UPDATE_tx();
        }

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









    }
}
