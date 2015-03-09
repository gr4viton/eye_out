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

namespace EyeOut
{
    //This delegate can be used to point to methods
    //which return void and take a string.
    public delegate void d_SEND_bytes2serial(Byte[] cmd);

    public delegate void d_LOG_msg_2logger(e_logger logger, e_how how, string msg);
    public delegate void d_LOG_logger_2gui(e_logger logger, e_how how, string msg);


    public enum e_logger
    {
        logAll = 0, logMot, logMotGot, logMotSent, logCam, logOculus
    }
    public enum e_how
    {
        renew = 0, appendLine, append
    }

    public delegate void d_del1(string str);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class C_cl1
    {
        string prefix;

        public event d_del1 event_ev1;

        public void h_handle1(string str)
        {
            MessageBox.Show(string.Format("{0}{1}",prefix,str));
        }
        public C_cl1(string _pre)
        {
            prefix = _pre;
            event_ev1 += new d_del1(h_handle1);
        }
    }

    public partial class MainWindow : Window
    {
        public event d_del1 event_ev1;

        public MainWindow()
        {
            InitializeComponent();

            cLog = new C_controlLog();
            //this.event_LOG_msg_2logger += new d_LOG_msg_2logger(cLog.h_LOG_msg_2logger);

            C_cl1 cl1_ins = new C_cl1("Taktedy:");
            this.event_ev1 += new d_del1(cl1_ins.h_handle1);
            
            try
            {
                event_ev1("Hello zmrde");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", ex.Data, ex.StackTrace, ex.TargetSite , ex.Message));
            }


            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            // real one
            this.event_LOG_logger_2gui += new d_LOG_logger_2gui(h_LOG_logger_2gui);

            string msg = "aosd";
            try
            {
                event_LOG_msg_2logger(e_logger.logMot, e_how.appendLine, msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", ex.Data, ex.StackTrace, ex.TargetSite , ex.Message));
            }

            INIT_LOG();
            
            INIT_GUI();
            INIT_SPI();
            INIT_controlMot();

            EV_connection(e_con.port_closed);

        }


        public event d_LOG_msg_2logger event_LOG_msg_2logger;
        public event d_LOG_logger_2gui event_LOG_logger_2gui;


        C_DynMot actMot;
        C_controlLog cLog;
        public static Byte nudId = 1;


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


        public void INIT_controlMot()
        {
            actMot = new C_DynMot(1);

            //I am creating a delegate (pointer) to HandleSomethingHappened
            //and adding it to SomethingHappened's list of "Event Handlers".

            actMot.event_SPI_bytes2serial_send += new d_SEND_bytes2serial(h_SPI_bytes2serial_send);
                
            
        }
        public void INIT_LOG()
        {

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        public void h_LOG_logger_2gui(e_logger logger, e_how how, string str)
        {
            object obj = GET_guiObject(logger);
            TextBox tx = (TextBox)obj;
            switch (how)
            {
                case (e_how.renew):
                    tx.Text = str;
                    break;
                case (e_how.appendLine):
                    tx.AppendText(str + "\r\n");
                    break;
                case (e_how.append):
                    tx.AppendText(str);
                    break;
            }
        }

        public object GET_guiObject(e_logger logger)
        {
            switch(logger)
            {
                case (e_logger.logAll): return txMotLog;
                case (e_logger.logCam): return txMotLog;
                case (e_logger.logMot): return txMotLog;
                case (e_logger.logMotGot): return txMotLog;
                case (e_logger.logMotSent): return txMotLog;
                case (e_logger.logOculus): return txMotLog;
                default: return txMotLog;
            }
            //txReceived.Select(txReceived.Text.Length, 0);
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
           //LOG_UPDATE_tx();
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
            INIT_cmdinEx();
            //INIT_lsCmdEx();

        }

        public void INIT_cmdinEx()
        {
            //string fname_cmdEx = @"B:\__DIP\dev\_main_dev\EyeOut\cmdInEx\cmdInEx.txt";
            string fname_cmdEx = @"..\..\..\..\cmdInEx\cmdInEx.txt";
            
            char del = '|';
            LOAD_examples(fname_cmdEx, del);
        }

        

        public void LOAD_examples(string fname, char del)
        {
            C_DynMot.cmdinEx = new List<C_cmdin>();
            
            string strHex_concantenated;
            string name;
            string[] strArr;

            string[] lines;
            if (!System.IO.File.Exists(fname)) return;
            lines = System.IO.File.ReadAllLines(fname, Encoding.ASCII);
            //lines = System.IO.File.ReadAllLines(fname);
            //string[] lines = System.IO.File.ReadAllLines(fname);

            // Display the file contents by using a foreach loop.
            System.Console.WriteLine("Contents of WriteLines2.txt = ");
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                strArr = line.Split(del);
                strHex_concantenated = strArr[0];
                name = strArr[1];

                //lsCmdEx.Items.Add(string.Format("{0} - {1}", Convert.ToString(c.byCmdin), c.cmdStr));
                lsCmdEx.Items.Add(name);
                C_DynMot.cmdinEx.Add(new C_cmdin(strHex_concantenated, name));
                // Use a tab to indent each line of the file.
                //Console.WriteLine("\t" + line);
            }
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region arguments
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



        private void btnStartMotors_Click(object sender, RoutedEventArgs e)
        {

            // connect serial port and search for motors
            string strCmd = "FF0A11";
            string strCmd_delimited = "FF FE FD 00 01 0A";
            string strDelimiter = " ";

            //C_DynMot.CONV_strHex2byteArray(strCmd2);
            byte[] bys;

            bys = C_CONV.strHex2byteArray(strCmd_delimited, strDelimiter);
            C_CONV.PRINT_byteArray(bys);
            bys = C_CONV.strHex2byteArray(strCmd);
            C_CONV.PRINT_byteArray(bys);

            
            //Byte[] by = C_DynMot.CONV_str2by(strCmd2, strDelimiter);
            /*
             * byte[] by = System.Text.Encoding.UTF8.GetBytes(strCmd) ;

            for (int q = 0; q < by.Length; q = q + 2)
            {
                MessageBox.Show(string.Format("{0}-{1}", Convert.ToByte( y[q], by[q+1]));
                
            }*/
        }

        private void lsCmdEx_wannaSend(object sender, MouseButtonEventArgs e)
        {
            lsCmdEx_SEND_selected();
        }

        private void lsCmdEx_wannaSend(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Space))
                lsCmdEx_SEND_selected();
        }

        private Byte ID_fromNUDid()
        {
            //return Convert.ToByte(nudID.Text);
            return nudId;
        }

        public void lsCmdEx_SEND_selected()
        {

            if (cbExampleDoubleClick.IsChecked == true)
                actMot.SEND_example(lsCmdEx.SelectedIndex);
                //lsCmdEx_SEND_selected();            
                
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
