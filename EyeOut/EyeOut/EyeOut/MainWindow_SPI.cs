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
    /// Serial port communication
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort SPI;

        public static Byte[] readBuff;
        public static int i_readBuff = 0;
        public static Byte this_byte;

        public Byte[] curCmd;
        public int i_curCmd;
        public Byte[] lastCmd;

        public const int i_cmdId = 0;     // = first byte in status packet (not counting 0xff 0xff)
        public const int i_cmdError = 2;  // = third byte in status packet (not counting 0xff 0xff)
        public Byte curCmd_id;
        public Byte curCmd_len;

        public bool START_NEW_MSG = false;


        // SPI hang - thread: http://www.codeproject.com/Questions/179614/Serial-Port-in-WPF-Application
        private void INIT_GUI_lsBaudRate()
        {
            int[] brs = { 9600, 57600, 1000000 };
            foreach (int br in brs)
            {
                lsBaudRate.Items.Add(Convert.ToString(br));
            }
            lsBaudRate.SelectedIndex = 1;
        }

        public void INIT_SPI()
        {

            i_readBuff = 0;
            readBuff = new Byte[1024];
            curCmd = new Byte[1];

            SPI = new SerialPort();

            //SPI = new System.IO.Ports.SerialPort(this.components);

            SPI_rescanPorts();
            INIT_GUI_lsBaudRate();

            SPI_UPDATE_portName();
            SPI_UPDATE_baudRate();
            SPI.Handshake = System.IO.Ports.Handshake.None;
            SPI.Parity = Parity.None;
            SPI.DataBits = 8;
            SPI.StopBits = StopBits.One;
            SPI.ReadTimeout = 200;
            SPI.WriteTimeout = 50;

            SPI.DataReceived += new SerialDataReceivedEventHandler(SPI_DataReceivedHandler);
        }
        public void SPI_UPDATE_baudRate()
        {
            SPI.BaudRate = Convert.ToInt32(txBaudRate.Text); //COM Port Sp
        }
        public void SPI_UPDATE_portName()
        {
            //SPI.PortName = "COM4"; //Com Port Name     
            if (cbPort.SelectedIndex != -1)
            {
                string port = cbPort.SelectedValue.ToString();
                if (string.IsNullOrEmpty(port) == false)
                    SPI.PortName = port; //Com Port Name                
            }
        }

        private void SPI_rescanPorts()
        {
            string[] allPorts;
            try
            {
                /*
                mainState = E_GUI_All_MainState.port_ScanningPorts;
                GUI.SC.portState = E_PortState.notConnected;
                GUI_SC_UpdateToolStrips();
                */

                cbPort.Items.Clear();
                allPorts = SerialPort.GetPortNames();

                foreach (string port in allPorts)
                {
                    //if(port[port.Length] != 'o')
                    cbPort.Items.Add(port);
                }

                //cbPort.SelectedIndex = 0;
                cbPort.SelectedIndex = cbPort.Items.Count -1;
            }
            catch (Exception)//Win32Exception)
            {/*
                MessageBox.Show(fStringResources.msg_port_NoCOMPortFound.Text);
                GUI.SC.portState = E_PortState.noPortsFound;
                GUI_SC_UpdateToolStrips();
              */
            }

        }
        e_con act_con_status;
        
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // enums
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // conection_status 
        enum e_con
        {
            port_opened = 1, cannot_open_port, port_closed
        };
        enum e_cmd
        {
            sent = 1, received, receivedCheckNot, receivedWithError
        };

        /*
        // colors
        enum e_cols : Color
        {
            connected = Color.LimeGreen
            , disconnected = Color.OrangeRed
        }
         */
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // EVENTinionation
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        public void h_SPI_bytes2serial_send(Byte[] cmd)
        {
            WRITE_cmd(cmd);
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // SPI control
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private bool SPI_openConnection()
        {
            SPI_UPDATE_baudRate();
            SPI_UPDATE_portName();
            try
            {
                SPI.Open();
            }
            catch (Exception ex)
            {
                //ODOM_ProcessMsg("exception = " + ex.Message);
                SPI_LOG_mot("Catched exception = " + ex.Message);
                //SET_state(E_GUI_MainState.error);

            }
            return SPI.IsOpen;
        }


        e_logger logger = e_logger.logMot;
        //e_how how = e_how.appendLine;
        public void SPI_LOG_mot(string msg)
        {
            event_LOG_msg2logger(e_logger.logMot, e_how.appendLine, msg);
        }

        public void SPI_LOG_motGot(string msg)
        {
            event_LOG_msg2logger(e_logger.logMotGot, e_how.appendLine, msg);
        }
        public void SPI_LOG_motSent(string msg)
        {
            event_LOG_msg2logger(e_logger.logMotSent, e_how.appendLine, msg);
        }

        private void WRITE_cmd(Byte[] cmd)
        {
            //SPI.Send(cmd);
            SPI.Write(cmd, 0x00, cmd.Length);
            lastCmd = cmd;
            SPI_LOG_cmd(cmd,e_cmd.sent);
        }
        
        public void WANNA_SPI_OpenConnection()
        {
            if (SPI_openConnection())
                EV_connection(e_con.port_opened);
            else
                EV_connection(e_con.cannot_open_port);
        }
        private void WANNA_SPI_CloseConnection()
        {
            // SPI.DataReceived -= Receive;
            if (SPI.IsOpen)
                SPI.Close();
            EV_connection(e_con.port_closed);
        }



        private void SPI_DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //}
            //private void srpOdo_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
            //{
            //fLog.log.notProcessed = true;
            if (PROG_QUITTING)
                System.Threading.Thread.CurrentThread.Abort();

            try
            {
                SerialPort sp = (SerialPort)sender;
                //string indata = sp.ReadExisting();
                //Console.WriteLine("Data Received:");
                //Console.Write(indata);

                int b2r = sp.BytesToRead;
                //read_buff = new Byte[b2r];
                //read_buff = new Byte()

                while (0 != sp.BytesToRead)
                //for (int i = 0; i < b2r; i++)
                {
                    this_byte = (Byte)sp.ReadByte();
                    readBuff[i_readBuff] = this_byte;
                    if (START_NEW_MSG == true)
                    {
                        switch (i_curCmd)
                        {
                            case (0):
                                curCmd_id = this_byte;
                                break;
                            case (1):
                                curCmd_len = this_byte;
                                // len = Nparam+2   = Nparam + Error + Length
                                // len + 1          = Nparam + Error + Length + ID 
                                // len + 1 + 1      = zero indexing correction?
                                curCmd = new Byte[curCmd_len + 1];
                                curCmd[0] = curCmd_id;
                                curCmd[1] = curCmd_len;

                                break;
                            default: // 2 and more
                                // ERROR, PARAM1 .. PARAMN, CHECKSUM
                                if (i_curCmd > curCmd_len)
                                {
                                    // end of current cmd message = the checksum
                                    //curCmd[i_curCmd] = this_byte; // checksum
                                    START_NEW_MSG = false;
                                    // check if it is the lastCmd echo from the motor

                                    if (curCmd.Length == lastCmd.Length - 3)
                                    {
                                        // the lenght is the same as the last sent lastCmd 
                                        // curCmd is without [0xFF 0xFF] and without checksum = [-3]
                                        int qmax = curCmd.Length;
                                        bool the_same = true;
                                        for (int q = 0; q < qmax; q++)
                                        {
                                            if (curCmd[q] != lastCmd[q + 2])
                                            {
                                                the_same = false;
                                                break;
                                            }
                                        }
                                        if (the_same == true)
                                        {
                                            // the recieved curCmd command is the same as the last sent lastCmd
                                            // so print only Echo confirmation
                                            SPI_LOG_mot("Echo confirmation");
                                            // and reset last Cmd in the case the next Status Msg is the same as the command
                                            lastCmd = new Byte[0];
                                        }
                                    }
                                    else
                                    {
                                        // it's not the echo command of the last send
                                        SPI_CHECK_receivedCmd(curCmd, this_byte);
                                    }
                                }
                                else
                                {
                                    // ERROR, PARAM1 .. PARAMN
                                    curCmd[i_curCmd] = this_byte;
                                }


                                break;
                        }
                        i_curCmd++;

                    }


                    if (i_readBuff > 0)
                    {
                        if ((readBuff[i_readBuff] == 0xFF) && (readBuff[i_readBuff - 1] == 0xFF))
                        {
                            START_NEW_MSG = true;
                            i_curCmd = 0;
                            curCmd_len = 0;
                            i_readBuff = 0;
                        }
                        else
                        {
                            i_readBuff++;
                        }
                    }
                    else
                    {
                        i_readBuff++;

                    }

                }

                /*
                    ODOM_ProcessMsg(fLog.log.lastMsg);

                    SET_state(E_GUI_MainState.recieved_stmmsg);
                    //txLast.Text = fLog.log.lastMsg;
                    fLog.log.notProcessed = false;
                }
                 */
            }
            catch (Exception ex)
            {
                //ODOM_ProcessMsg("exception = " + ex.Message);
                SPI_LOG_mot("Catched exception = " + ex.Message);
                //SET_state(E_GUI_MainState.error);

            }
        }


        private void SPI_CHECK_receivedCmd(Byte[] cmd, Byte rec_checkSum)
        {
            // check for [checksum error] and cmd [error byte] sub-bites disambiguation
            Byte calc_checkSum = C_CheckSum.GET_checkSum(cmd);
            if (C_CheckSum.CHECK_checkSum(calc_checkSum, rec_checkSum))
            //if( calc_check == 0 )
            {
                //MessageBox.Show(string.Format("cmd[{0}] = {1}", i_cmdError, cmd[i_cmdError]));
                if (cmd[i_cmdError] == 0)
                    // no error
                    SPI_LOG_cmd(cmd, e_cmd.received);
                else
                {
                    SPI_LOG_cmd(cmd, e_cmd.receivedWithError);
                    SPI_LOG_cmdError(cmd[i_cmdId], cmd[i_cmdError]);
                }
            }
            else
            {
                SPI_LOG_cmd(cmd, e_cmd.receivedCheckNot);
                SPI_LOG_mot(String.Format("CheckSumGot != CheckSumCounted :: {0} != {1}", (Byte)rec_checkSum, (Byte)calc_checkSum));
                //LOG_msgAppendLine(String.Format("CheckSumGot = {0} ", (Byte)calc_check));
            }
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // LOG
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



        private void SPI_LOG_cmdError(Byte byId, Byte byError)
        {
            for (int b = 0; b < 7; b++)
                if (C_CONV.GET_bit(byError, b) == true)
                {
                    SPI_LOG_mot(
                        string.Format("ID[{0}] error: {1}", byId, errStr[b])
                        );
                }
        }

        private void SPI_LOG_cmd(Byte[] cmd, e_cmd type)
        {
            string prefix = "";
            string hex = BitConverter.ToString(cmd).Replace("-", " ");
            string line = hex + "\r\n";

            switch (type)
            {
                case (e_cmd.received):
                    prefix = "Got : ";
                    SPI_LOG_motGot(line);
                    break;
                case (e_cmd.receivedCheckNot):
                    prefix = "! Got with wrong Checksum: ";
                    SPI_LOG_motGot(line);
                    break;
                case (e_cmd.receivedWithError):
                    prefix = "! Got with an Error: ";
                    SPI_LOG_motGot(line);
                    break;
                case (e_cmd.sent):
                    prefix = "Sent: ";
                    SPI_LOG_motSent(line);
                    break;
            }



            SPI_LOG_mot(prefix + line);
        }


        private void CLOSE_PORT()
        {
            if (SPI.IsOpen)
            {
                SPI.DiscardOutBuffer();
                SPI.DiscardInBuffer();
                SPI.Close();
            }
            Thread.CurrentThread.Abort();
            PROG_QUITTING = false;
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region BTNs
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (act_con_status != e_con.port_opened)
                WANNA_SPI_OpenConnection();
            else
                WANNA_SPI_CloseConnection();
        }
        private void btnRescanPort_Click(object sender, RoutedEventArgs e)
        {
            SPI_rescanPorts();
        }
        #endregion BTNs
    }
}
