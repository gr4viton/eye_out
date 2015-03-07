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

namespace EyeOut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        C_DynMot mot1;
        bool PROG_QUITTING = false;
        // make it into HASHTABLE
        string[] errStr = {     "Input Voltage Error"
                                  , "Angle Limit Error"
                                  , "Overheating Error"
                                  , "Range Error"
                                  , "Checksum Error"
                                  , "Overload Error"
                                  , "Instruction Error"
                              };

        public MainWindow()
        {
            InitializeComponent();

            INIT_GUI();

            EV_connection(e_con.port_closed);
            
            mot1 = new C_DynMot(1);

            //I am creating a delegate (pointer) to HandleSomethingHappened
            //and adding it to SomethingHappened's list of "Event Handlers".
            mot1.WANNA_LOG_msgAppendLine += new h_LOG_String(h_LOG_msgAppendLine);
            mot1.WANNA_SEND_cmd += new h_SEND_Bytes(h_SEND_cmd);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
           LOG_UPDATE_tx();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WANNA_CLOSE_program();
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void INIT_GUI()
        {
            INIT_GUI_controlMot();
        }

        public void INIT_GUI_controlMot()
        {
            INIT_SPI();

        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region arguments
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



        private void btnStartMotors_Click(object sender, RoutedEventArgs e)
        {
            // connect serial port and search for motors
        }



        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion arguments
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



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

    }
}
