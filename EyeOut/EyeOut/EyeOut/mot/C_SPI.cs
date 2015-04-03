using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Threading;

using System.ComponentModel; // backgroundWorker

namespace EyeOut
{
    // conection_status 
    enum e_con
    {
        port_opened = 1, cannot_open_port, port_closed
    };
    enum e_cmd
    {
        sent = 1, received, receivedCheckNot, receivedWithError
    };


    internal class C_SPI
    {
        private static object spi_locker = new object();
        private static object queue_locker = new object();
        
        public static SerialPort spi;
        //private static BackgroundWorker worker_SEND;
        private static Queue<byte[]> queueData;

        public static Byte[] readBuff;
        public static int i_readBuff = 0;
        public static Byte this_byte;

        public static Byte[] curCmd;
        public static int i_curCmd;
        public static Byte[] lastCmd;

        // const!?
        public static int i_cmdId = 0;     // = first byte in status packet (not counting 0xff 0xff)
        public static int i_cmdError = 2;  // = third byte in status packet (not counting 0xff 0xff)

        public static Byte curCmd_id;
        public static Byte curCmd_len;

        public static bool START_NEW_MSG = false;

        public static int timeoutExceptionPeriod = 10;
        // make it into HASHTABLE
        static string[] errStr = {     
                                     "Input Voltage Error"
                                  , "Angle Limit Error"
                                  , "Overheating Error"
                                  , "Range Error"
                                  , "Checksum Error"
                                  , "Overload Error"
                                  , "Instruction Error"
                              };

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // SPI hang - thread: http://www.codeproject.com/Questions/179614/Serial-Port-in-WPF-Application

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public C_SPI()
        {
            i_readBuff = 0;
            readBuff = new Byte[1024];
            curCmd = new Byte[1];

            timeoutExceptionPeriod = 10; // according to datahseet.?.
            // spi
            //spi = new SerialPort("COM6", 57600, Parity.None, 8, StopBits.One);
            spi = new SerialPort("COM6", 57600, Parity.None, 8, StopBits.One);

            /*
            SPI.Handshake = System.IO.Ports.Handshake.None;
            SPI.ReadTimeout = 200;
            SPI.WriteTimeout = 50;*/

            // NOT NEEDED as all the motors are just CLIENTS - only responding to my (SERVER) orders
            // when I sent some cmd -> I call C_SPI.READ_cmd() afterwards if I want to read some echo or response
            //spi.DataReceived += new SerialDataReceivedEventHandler(SPI_DataReceivedHandler);

            queueData = new Queue<byte[]>();

            
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Open close
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static bool OPEN_connection()
        {
            //UPDATE_SPI_Settings();
            //SPI_UPDATE_baudRate();
            //SPI_UPDATE_portName();
            /*
            if (C_State.FURTHER(e_stateSPI.notConnected))
            else
            */
            if (C_State.FURTHER(e_stateSPI.connected))
            {
                // we need to close it first
                CLOSE_connection();
            }

            C_State.Spi = e_stateSPI.connecting;

            try
            {
                spi.Open();
                //SET_state(E_GUI_MainState.error);
            }
            catch (Exception ex)
            {
                LOG("Port could not be opened");
                LOG_err(ex); 
                //SET_state(E_GUI_MainState.error);
                C_State.Spi = e_stateSPI.disconnected;
                return false;
            }

            C_State.Spi = e_stateSPI.connected;
            LOG(String.Format("Port {0} opened successfuly with {1} bps",
                        spi.PortName, spi.BaudRate.ToString())
                        );

            return spi.IsOpen;
        }

        public static void CLOSE_connection()
        {
            if (spi.IsOpen)
            {
                spi.DiscardOutBuffer();
                spi.DiscardInBuffer();
                spi.Close();

                LOG(String.Format("Port {0} closed", spi.PortName));
                C_State.Spi = e_stateSPI.disconnected;
            }
            else
            {
                LOG("There is no open port to close!");
            }

        }

        public static void CLOSE_connectionAndAbortThread()
        {
            CLOSE_connection();
            Thread.CurrentThread.Abort();
        }



        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Open close
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Writing
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static void SEND_data(byte[] data)
        {
            QUEUE_data(data);

            BackgroundWorker worker_SEND = new BackgroundWorker();
            worker_SEND.RunWorkerCompleted += workerSEND_RunWorkerCompleted;
            worker_SEND.DoWork += workerSEND_DoWork;
            worker_SEND.RunWorkerAsync();
        }

        private static void QUEUE_data(byte[] data)
        {
            // adds data to sending queue
            lock (queue_locker)
            {
                queueData.Enqueue(data);
            }
        }

        private static void workerSEND_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (spi_locker)
            {
                lock (queue_locker)
                {
                    if (queueData.Count != 0)
                    {
                        e.Result = C_SPI.WriteData(queueData.Dequeue());
                    }
                    else
                    {
#if (!DEBUG)
                        throw new InvalidOperationException(
                            "An error occured when tried to send data!\nThe queue of data to send was empty!");
#endif
                    }
                }
            }
        }

// Not used
        private static void workerSEND_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // catch if response was A-OK
            if (e.Error != null)
            {
                LOG_err(e.Error);
                //LOG_err(String.Format("Motor id#{2} had an error:\n{0}\n{1}", e.Error.Data, e.Error.Message, id));
                //ie Helpers.HandleCOMException(e.Error);
            }
            else
            {
                //e.Result = "tocovrati writeData";
                //MyResultObject result = (MyResultObject)e.Result;

                //LOG("DATA SENT");
                //var results = e.Result as List<object>;
            }
        }

        private static bool WriteData(byte[] data)
        {
            int q = 10; // try q-times
            while (q > 0)
            {
                if (spi.IsOpen)
                {
                    WriteSerialPort(data);
                    //SPI.Send(cmd);

                    return true;
                    //responseBuffer = ReadSerialPort(8);
                }
                else
                {
                    OPEN_connection();
                }
                q--;
            }
            return false; // should never run as far as to this line
        }

        private static void WriteSerialPort(byte[] data)
        {
            spi.Write(data, 0, data.Length);
            lastCmd = data;

            // another zeros for slowing down serial commands
            byte[] zeros = new byte[20];
            for(int q = 0;q<20;q++)
            {
                zeros[q] = 0;
            }
            spi.Write(zeros, 0, zeros.Length);
            LOG_cmd(data, e_cmd.sent);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Writing
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Reading
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static bool READ_cmd()
        {
            // this function tries to read echo of the motor after sending [lastSentCmd]

            if (C_State.prog == e_stateProg.closing)
            {
                System.Threading.Thread.CurrentThread.Abort();
            }

            lock (spi_locker)
            {
                try
                {
                    while (0 != C_SPI.spi.BytesToRead)
                    {
                        this_byte = (Byte)C_SPI.spi.ReadByte();
                        readBuff[i_readBuff] = this_byte;
                        if (START_NEW_MSG == true)
                        {
                            // message bytes after C_DynAdd.MSG_START was detected
                            switch (i_curCmd) // byte index in current cmd
                            {
                                case (0): // as the C_DynAdd.MSG_START of each message are cutted away
                                    curCmd_id = this_byte;
                                    break;
                                case (1):
                                    curCmd_len = this_byte; // lengthe of current cmd
                                    // len = Nparam+2   = Nparam + Error + Length
                                    // len + 1          = Nparam + Error + Length + ID 
                                    curCmd = new Byte[curCmd_len + 1];
                                    curCmd[0] = curCmd_id; // does not need it
                                    curCmd[1] = curCmd_len;

                                    break;
                                default: // [2] and next bytes = [2]ERROR, [3]PARAM1 .. [LEN]PARAMN, [LEN+1]CHECKSUM
                                    if (i_curCmd <= curCmd_len)
                                    {
                                        // store bytes
                                        curCmd[i_curCmd] = this_byte;
                                    }
                                    else
                                    {
                                        // Checksum byte
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
                                                LOG("Echo confirmation");
                                                LOG_cmd(lastCmd, e_cmd.received);
                                                // and reset last Cmd in the case the next Status Msg is the same as the command
                                                lastCmd = new Byte[0];
                                            }
                                        }
                                        else
                                        { // it's not the echo command of the last send
                                            SPI_CHECK_receivedCmd(curCmd, this_byte);
                                        }
                                    }
                                    break;
                            }
                            i_curCmd++;
                        }
                        // not detected NEW message yet
                        if (i_readBuff > 0)
                        {
                            // C_DynAdd.MSG_START detection
                            if ((readBuff[i_readBuff] == 0xFF) && (readBuff[i_readBuff - 1] == 0xFF))
                            {
                                START_NEW_MSG = true;
                                // reset indexes and counters
                                i_curCmd = 0;
                                curCmd_len = 0;
                                i_readBuff = 0;
                            }
                            else
                            {
                                // if C_DynAdd.MSG_START not detected - read buffer but don't do anything about it
                                i_readBuff++;
                            }
                        }
                        else
                        {
                            i_readBuff++;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LOG_err(ex);
                    return false;
                }
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Reading
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        int BaudRate 
        { 
            get { return spi.BaudRate; }
            set { spi.BaudRate = value;}
        }
        string PortName
        { 
            get { return spi.PortName; }
            set { spi.PortName = value;}
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.spi, _msg);
        }
        public static void LOG_sent(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.spi_sent, _msg);
        }
        public static void LOG_got(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.spi_got, _msg);
        }


        public static void LOG_err(Exception ex)
        {
            string msg = "Catched exception = " + ex.Message; 
            C_Logger.Instance.LOG_err(e_LogMsgSource.spi, msg);
        }



        private static void LOG_cmdError(Byte byId, Byte byError)
        {
            for (int b = 0; b < 7; b++)
                if (C_Motor.GET_bit(byError, b) == true)
                {
                    LOG(
                        string.Format("ID[{0}] error: {1}", byId, errStr[b])
                        );
                }
        }

        private static void LOG_cmd(Byte[] cmd, e_cmd type)
        {
            string prefix = "";
            string hex = BitConverter.ToString(cmd).Replace("-", " ");

            switch (type)
            {
                case (e_cmd.sent):
                    prefix = "Sent:\t";
                    LOG_sent(hex);
                    break;
                case (e_cmd.received):
                    prefix = "Got:\t";
                    LOG_got(hex);
                    break;
                case (e_cmd.receivedCheckNot):
                    prefix = "! Got with wrong Checksum: ";
                    LOG_got(hex);
                    break;
                case (e_cmd.receivedWithError):
                    prefix = "! Got with an Error: ";
                    LOG_got(hex);
                    break;
            }
            LOG(prefix + hex);
        }


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private static void SPI_CHECK_receivedCmd(Byte[] cmd, Byte rec_checkSum)
        {
            // check for [checksum error] and cmd [error byte] sub-bites disambiguation
            Byte calc_checkSum = C_CheckSum.GET_checkSum(cmd);
            if (C_CheckSum.CHECK_checkSum(calc_checkSum, rec_checkSum))
            //if( calc_check == 0 )
            {
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
                LOG(String.Format("CheckSumGot != CheckSumCounted :: {0} != {1}", (Byte)rec_checkSum, (Byte)calc_checkSum));
                //LOG_msgAppendLine(String.Format("CheckSumGot = {0} ", (Byte)calc_check));
            }
        }
    }

}
