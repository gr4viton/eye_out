using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;
using System.Threading;



namespace SerialPortExample
{
    
    //This delegate can be used to point to methods
    //which return void and take a string.
    public delegate void h_LOG_String(string msg);
    public delegate void h_SEND_Bytes(Byte[] cmd);



    public partial class Form1 : Form
    //, C_DynamixelAddresses
    {
        //SerialPortInterface SPI = new SerialPortInterface();
        
        // constructor
        public Form1()
        {
            
            InitializeComponent();
            //spsControl.WorkingObject = SPI;
            //SPI.DataReceived += new dataReceived(SPI_DataReceived);

            SPI.DataReceived += new SerialDataReceivedEventHandler(SPI_DataReceivedHandler);

            int[] bdrts = { 57600, 10000000 };
            foreach (int i in bdrts)
            {
                lsBaud.Items.Add(i);
            }
            //spi_1.DataReceived += new dataReceived(SPI_DataReceived);
            EV_connection(e_con.port_closed);
            GUI_rescanPorts();
            readBuff = new Byte[1024];
            i_readBuff = 0;
            curCmd = new Byte[1];

            mot1 = new C_DynMot(1);

            //I am creating a delegate (pointer) to HandleSomethingHappened
            //and adding it to SomethingHappened's list of "Event Handlers".
            mot1.WANNA_LOG_msgAppendLine += new h_LOG_String(h_LOG_msgAppendLine);
            mot1.WANNA_SEND_cmd += new h_SEND_Bytes(h_SEND_cmd);
        }

       
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // EVENTinionation
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        //Here is some code I want to be executed
        //when WANNA_LOG_msgAppendLine fires.
        public void h_LOG_msgAppendLine(string msg)
        {
            LOG_msgAppendLine(msg);
        }

        public void h_SEND_cmd(Byte[] cmd)
        {
            WRITE_cmd(cmd);
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // local arguments
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        //C_DynAddresses dynAdd = new C_DynAddresses();

        C_DynMot mot1;

        string log;
        string logSent;
        string logRec;
        public static bool logChanged = false;
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

        //public static Byte[] read_buff;
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

        e_con act_con_status;

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // enums
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // conection_status 
        enum e_con
        {
            port_opened = 1
           ,
            cannot_open_port
                , port_closed
        };

        enum e_cmd
        {
            sent = 1
           ,
            received
                ,
            receivedCheckNot
                , receivedWithError


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
        // GUIfication
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void status_connected()
        {
            tslConnected.Text = "Connected";
            tslConnected.BackColor = Color.LimeGreen;
            //spsControl.Enabled = false;
            gpCmds.Enabled = true;

            btnOpen.Enabled = false;
            btnClose.Enabled = true;
        }
        private void status_disconnected()
        {
            tslConnected.Text = "Not connected";
            tslConnected.BackColor = Color.OrangeRed;
            //spsControl.Enabled = true;
            gpCmds.Enabled = false;

            btnOpen.Enabled = true;
            btnClose.Enabled = false;
        }

        private void EV_connection(e_con status)
        {
            act_con_status = status;
            switch (status)
            {
                case (e_con.port_opened):
                    //MessageBox.Show("Port Opened Successfuly");
                    LOG_msgAppendLine(
                        String.Format("Port {0} opened successfuly with {1} bps",
                        //SPI.PortName, SPI.BaudRate.ToString() )
                        SPI.PortName, SPI.BaudRate.ToString())
                        );
                    status_connected();
                    break;
                case (e_con.cannot_open_port):
                    //MessageBox.Show("Port Opened Successfuly");
                    LOG_msgAppendLine("Port could not be opened");
                    status_disconnected();
                    break;
                case (e_con.port_closed):
                    //MessageBox.Show("Port Opened Successfuly");
                    LOG_msgAppendLine(String.Format("Port {0} closed", SPI.PortName));
                    status_disconnected();
                    break;
            }
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // SPI control
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private bool SPI_openConnection()
        {
            //SPI.BaudRate = 1000000;
            //SPI.BaudRate = 57600;
            //SPI.BaudRate = 9600;
            //SPI.PortName = "COM6";
            //SPI.PortName = "COM4";
            SPI.BaudRate = Convert.ToInt32(txBaud.Text);
            SPI.PortName = cbPorts.Text; //cbPorts.Items[]
            //SPI
            SPI.Open();

            return SPI.IsOpen;
        }

        private void WRITE_cmd(Byte[] cmd)
        {
            //SPI.Send(cmd);
            SPI.Write(cmd, 0x00, cmd.Length);
            lastCmd = cmd;
            LOG_cmdSent(cmd);
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
            SPI.Close();
            EV_connection(e_con.port_closed);
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            WANNA_SPI_OpenConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            WANNA_SPI_CloseConnection();
        }
        //void SPI_DataReceived(object sender, SerialPortEventArgs arg)
        /*void SPI_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //this.ReceivedText.Text += arg.ReceivedData;
            //arg.ReceivedData - can it handle bytes??
        }
        */
        public bool START_NEW_MSG = false;
        private void SPI_DataReceivedHandler(
                            object sender,
                            SerialDataReceivedEventArgs e)
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
                                            LOG_msgAppendLine("Echo confirmation");
                                            // and reset last Cmd in the case the next Status Msg is the same as the command
                                            lastCmd = new Byte[0];
                                        }
                                    }
                                    else
                                    {
                                        // it's different command
                                        LOG_cmdRec_check(curCmd, this_byte);
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
                char[] stmmsg = new char[MSG_LEN];
                char ch = '0';
                int i = 0;

                while ((srpOdo.IsOpen) && (srpOdo.ReadBufferSize != 0) && (ch != '$') && (ch != '\n') && (i < MSG_LEN))
                {
                    //while ((srpOdo.IsOpen) && (srpOdo.ReadBufferSize != 0) && (ch != '\n') && (i < MSG_LEN)) {
                    ch = (char)srpOdo.ReadByte();
                    stmmsg[i++] = ch;
                    fLog.log.lastReceivedChar = ch;
                }
                i = 0;
                string str = new string(stmmsg);
                if (str.IndexOf('^') >= 0 || str.IndexOf('_') >= 0)
                {
                    fLog.log.lastMsg = str;
                    if (fLog.log.lastMsg == "") fLog.log.lastMsg = "nothing";

                    //ODOM_ProcessMsg();
                    // new thread
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
                LOG_msgAppendLine("Catched exception = " + ex.Message);
                //SET_state(E_GUI_MainState.error);

            }
        }




        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // BTNS
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void btnSetID1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Do you really want to broadcast write of ID = 1 to all connected servos?\n In case of multiple servos with the same id there may be malfunctious consequencies",
                "You have been warned!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                mot1.SEND_example(2);
            }
            /*
        else if(result == DialogResult.No)
        {
          //code for No
        }
        else if (result == DialogResult.Cancel)
        {
            //code for Cancel
        }*/


        }

        private void btnCurTemp_Click(object sender, EventArgs e)
        {
            mot1.SEND_example(1);
        }
        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            mot1.SEND_example(0);

        }

        private void btnSetPos_Click(object sender, EventArgs e)
        {
            mot1.SEND_example(17);

        }

        private void btnPos185_Click(object sender, EventArgs e)
        {
            mot1.SEND_example(172);

        }
        private void btnTorqueLed_Click(object sender, EventArgs e)
        {
            mot1.SEND_example(16);
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // LOG
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void LOG_cmdSent(Byte[] cmd)
        {
            LOG_cmd(cmd, e_cmd.sent);
        }
        private void LOG_cmdRec(Byte[] cmd)
        {
            LOG_cmd(cmd, e_cmd.received);
        }

        private void LOG_cmdRec_check(Byte[] cmd, Byte rec_checkSum)
        {
            // check for [checksum error] and cmd [error byte] sub-bites disambiguation
            Byte calc_checkSum = C_CheckSum.GET_checkSum(cmd);
            if (C_CheckSum.CHECK_checkSum(calc_checkSum, rec_checkSum))
            //if( calc_check == 0 )
            {
                //MessageBox.Show(string.Format("cmd[{0}] = {1}", i_cmdError, cmd[i_cmdError]));
                if (cmd[i_cmdError] == 0)
                    // no error
                    LOG_cmd(cmd, e_cmd.received);
                else
                {
                    LOG_cmd(cmd, e_cmd.receivedWithError);
                    LOG_cmdError(cmd[i_cmdId], cmd[i_cmdError]);
                }
            }
            else
            {
                LOG_cmd(cmd, e_cmd.receivedCheckNot);
                LOG_msgAppendLine(String.Format("CheckSumGot != CheckSumCounted :: {0} != {1}", (Byte)rec_checkSum, (Byte)calc_checkSum));
                //LOG_msgAppendLine(String.Format("CheckSumGot = {0} ", (Byte)calc_check));
            }
        }

        public bool GET_bit(Byte by, int bitNumber)
        {
            return (by & (1 << bitNumber)) != 0;
        }

        private void LOG_cmdError(Byte byId, Byte byError)
        {

            for (int b = 0; b < 7; b++)
                if (GET_bit(byError, b) == true)
                {
                    LOG_msgAppendLine(
                        string.Format("ID[{0}] error: {1}", byId, errStr[b])
                        );
                    logChanged = true;
                }


        }

        private void LOG_cmd(Byte[] cmd, e_cmd type)
        {
            string prefix = "";
            string hex = BitConverter.ToString(cmd).Replace("-", " ");
            string line = hex + "\r\n";

            switch (type)
            {
                case (e_cmd.received):
                    prefix = "Got : ";
                    logRec += line;
                    break;
                case (e_cmd.receivedCheckNot):
                    prefix = "! Got with wrong Checksum: ";
                    logRec += line;
                    break;
                case (e_cmd.receivedWithError):
                    prefix = "! Got with an Error: ";
                    logRec += line;
                    break;
                case (e_cmd.sent):
                    prefix = "Sent: ";
                    logSent += line;
                    break;

            }

            log += prefix + line;
            //log = hex;

            // raise event LOG_UPDATE_tx!!
            logChanged = true;
            //LOG_msgAppendLine(prefix + hex);
            //txLog.AppendText(prefix + line);
            //txSent.AppendText(line);
            //txReceived.AppendText(line);
        }
        private void LOG_msgAppendLine(string msg)
        {
            LOG_msgAppend(msg + "\r\n");
        }

        private void LOG_msgAppend(string msg)
        {
            log += msg;
            logChanged = true;
            // raise event LOG_UPDATE_tx!!
            //txLog.AppendText(msg);
        }




        private void LOG_UPDATE_tx()
        {
            logChanged = false;
            txReceived.Text = logRec;
            txSent.Text = logSent;
            txLog.Text = log;

            txLog.AppendText(" ");
            txSent.AppendText(" ");
            txReceived.AppendText(" ");

            txReadBuff.Text = BitConverter.ToString(readBuff).Replace("-", " ");
            txCurCmd.Text = BitConverter.ToString(curCmd).Replace("-", " ");

            //txReceived.Select(txReceived.Text.Length, 0);
            //txSent.Select(txSent.Text.Length, 0);
            //txLog.Select(txLog.Text.Length, 0);
        }
        /*
        public event EventHandler LOG_changed;
        protected virtual void OnLOG_changed(EventArgs e)
        {
            EventHandler handler = LOG_changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        ThresholdReached += c_ThresholdReached;
         */

        //fit i'll just be a stripper

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // not classified yet
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void timLOG_Tick(object sender, EventArgs e)
        {
            if (logChanged)
            {
                LOG_UPDATE_tx();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        private void btnRescanPorts_Click(object sender, EventArgs e)
        {
            GUI_rescanPorts();
        }
        private void CLOSE_PORT()
        {
            //srpOdo.DiscardInBuffer();
            //srpOdo.DiscardOutBuffer();
            SPI.Close();
            Thread.CurrentThread.Abort();
            PROG_QUITTING = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            PROG_QUITTING = true;
            Thread dexter = new Thread(new ThreadStart(CLOSE_PORT)); // the serial_killer
            dexter.Start();

            System.Threading.Thread.Sleep(500);
            if (SPI.IsOpen == false)
            {
                txLog.Text = "Port closed";
                PROG_QUITTING = false;
            }
        }


        private void GUI_rescanPorts()
        {

            string[] allPorts;
            try
            {/*
                mainState = E_GUI_All_MainState.port_ScanningPorts;
                GUI.SC.portState = E_PortState.notConnected;
                GUI_SC_UpdateToolStrips();
                */

                cbPorts.Items.Clear();
                allPorts = SerialPort.GetPortNames();

                foreach (string port in allPorts)
                {
                    //if(port[port.Length] != 'o')
                    cbPorts.Items.Add(port);
                }

                cbPorts.SelectedIndex = 1;
            }
            catch (Exception)//Win32Exception)
            {/*
                MessageBox.Show(fStringResources.msg_port_NoCOMPortFound.Text);
                GUI.SC.portState = E_PortState.noPortsFound;
                GUI_SC_UpdateToolStrips();
              */
            }


        }

        private void LOG_clear()
        {          
            log = "";
            logChanged = true;
        }
        private void btnLogClear_Click(object sender, EventArgs e)
        {
            LOG_clear();
        }

        private void btnSendStrCmd_Click(object sender, EventArgs e)
        {
            //Byte[] cmd = CREATE_cmdFromStr(txStrCmd.Text);
            //SEND_cmd(cmd);
        }

        private void lsBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            txBaud.Text = Convert.ToString(lsBaud.SelectedItem);

        }

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
                    btnTimSim.BackColor = Color.OrangeRed;
                    btnTimSim.Text = "START";
                    timSim.Enabled = false;
                    break;

                case (false):
                    btnTimSim.BackColor = Color.LimeGreen;
                    btnTimSim.Text = "STOP";
                    timSim.Enabled = true;
                    break;
            }
        }

        private void tbAng_ValueChanged(object sender, EventArgs e)
        {
            act_ang = tbAng.Value;
            mot1.MOVE_absPosLastSpeed(act_ang);
            txAng.Text = act_ang.ToString();
        }




    }
    //public const Byte 


}





