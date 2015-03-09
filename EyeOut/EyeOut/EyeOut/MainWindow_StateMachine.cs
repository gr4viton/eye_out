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



namespace EyeOut
{
    /// <summary>
    /// State machine - status changing - gui modifications..
    /// </summary>
    public partial class MainWindow : Window
    {


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // GUIfication
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
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
        void LOG_motGot(string msg)
        {
            try
            {
                event_LOG_msg_2logger(e_logger.logMot, e_how.appendLine, msg);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EV_connection(e_con status)
        {
            act_con_status = status; 
            
            switch (status)
            {
                case (e_con.port_opened):
                    //MessageBox.Show("Port Opened Successfuly");
                    LOG_motGot(String.Format("Port {0} opened successfuly with {1} bps",
                        SPI.PortName, SPI.BaudRate.ToString())
                        );
                    status_connected();
                    break;
                case (e_con.cannot_open_port):
                    //MessageBox.Show("Port Opened Successfuly");
                    LOG_motGot("Port could not be opened");
                    status_disconnected();
                    break;
                case (e_con.port_closed):
                    //MessageBox.Show("Port Opened Successfuly");
                    LOG_motGot(String.Format("Port {0} closed", SPI.PortName));
                    status_disconnected();
                    break;
            }
        }






        private void WANNA_CLOSE_program()
        {
            PROG_QUITTING = true;
            Thread dexter = new Thread(new ThreadStart(CLOSE_PORT)); // the serial_killer
            dexter.Start();

            System.Threading.Thread.Sleep(500);
            if (SPI.IsOpen == false)
            {
                txMotLog.Text = "Port closed";
                PROG_QUITTING = false;
            }
        }


    }
}
