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
        sending = 1, received, receivedEchoOf, receivedStatusPacket, receivedCheckNot, receivedWithError
    };


    internal partial class C_SPI
    {
        private static object spiSent_locker = new object();
        private static object queueToSent_locker = new object();
        
        public static SerialPort spi;
        private static Queue<C_Packet> queueToSent; // packets which are going to be sent

        static C_CounterDown openConnection = new C_CounterDown(10); // try to open connection x-times
        public static int timeoutExceptionPeriod = 10;

        private static int timeWaitBeforeRtsEnable_ms = 15; // 15 on 9600
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // SPI hang - thread: http://www.codeproject.com/Questions/179614/Serial-Port-in-WPF-Application

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public C_SPI()
        {
            //i_readBuff = 0;
            //readBuff = new byte[1024];
            //curCmd = new byte[1];

            timeoutExceptionPeriod = 10; // according to datahseet.?.
            // spi
            //spi = new SerialPort("COM6", 57600, Parity.None, 8, StopBits.One);
            //spi = new SerialPort("COM6", 57600, Parity.None, 8, StopBits.One);
            spi = new SerialPort("COM6", 1000000, Parity.None, 8, StopBits.One);

            /*
            spi.Handshake = System.IO.Ports.Handshake.None;
            spi.ReadTimeout = 200;
            spi.WriteTimeout = 50;*/
            spi.Handshake = System.IO.Ports.Handshake.None;
            spi.ReadTimeout = 500;
            spi.WriteTimeout = 500;
            spi.DtrEnable = true;
            spi.RtsEnable = true;

            // NOT NEEDED as all the motors are just CLIENTS - only responding to my (SERVER) orders
            spi.DataReceived += new SerialDataReceivedEventHandler(SPI_DataReceivedHandler);

            //worker_READ.DoWork += worker_READ_DoWork;

            queueToSent = new Queue<C_Packet>();
            queueSent = new List<Queue<C_Packet>>()
            {
                new Queue<C_Packet>(),
                new Queue<C_Packet>(),
                new Queue<C_Packet>()
            };


        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Open close
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static bool OPEN_connection()
        {
            lock (spiSent_locker)
            {
                //UPDATE_SPI_Settings();
                //UPDATE_baudRate();
                if ( UPDATE_portName() == false )
                    return false;

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
                    timeWaitBeforeRtsEnable_ms = REFRESH_timeWaitBeforeRtsEnable_ms();
                    spi.Open();
                    //SET_state(E_GUI_MainState.error);
                }
                catch (Exception ex)
                {
                    LOG("Port could not be opened");
                    LOG(GET_exInfo(ex));
                    //SET_state(E_GUI_MainState.error);
                    C_State.Spi = e_stateSPI.disconnected;
                    return false;
                }

                C_State.Spi = e_stateSPI.connected;
                LOG(String.Format("Port {0} opened successfuly with {1} bps",
                            spi.PortName, spi.BaudRate.ToString())
                            );
                if (spi.IsOpen == true)
                {
                    spi.DiscardInBuffer();
                    spi.DiscardOutBuffer();
                }
            }
            return spi.IsOpen;
        }

        public static bool UPDATE_portName()
        {
            // returns false if no COM port is found
            string[] portNames = SerialPort.GetPortNames();
            if(portNames == null)
            {
                return false;
            }
            else
            {
                // select the last one
                spi.PortName = portNames[portNames.Length-1];
                if(spi.PortName == "COM6")
                {
                    C_MotorControl.INIT_groupSettings();
                    return true;
                }
                else 
                    return false;
            }
        }
        public static int REFRESH_timeWaitBeforeRtsEnable_ms()
        {
            double numOfBytesInMessage = 1 + spi.DataBits;
            if (spi.StopBits == StopBits.None)
                numOfBytesInMessage += 0;
            else if (spi.StopBits == StopBits.One)
                numOfBytesInMessage += 1;
            else if ((spi.StopBits == StopBits.OnePointFive) || (spi.StopBits == StopBits.Two))
                numOfBytesInMessage += 2;
            else
                numOfBytesInMessage += 3; // should not happen

            return (int)((numOfBytesInMessage/(double)(spi.BaudRate)) + 2); // two more for safety
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
        //// I allways need to know if the command send will produce some command receive!
        //public static void SEND_data(byte[] data)
        //{
        //    SEND_data(e_cmdEcho.noEcho, data);
        //}
       
        public static void SEND_data(C_Packet instructionPacket)
        {
            QUEUE_PacketToSent(instructionPacket);

            BackgroundWorker worker_SEND = new BackgroundWorker();
            worker_SEND.DoWork += workerSEND_DoWork;
            worker_SEND.RunWorkerCompleted += workerSEND_RunWorkerCompleted;
            //worker_SEND.RunWorkerAsync((object)echo);
            worker_SEND.RunWorkerAsync();
        }


        private static void QUEUE_PacketToSent(C_Packet instructionPacket)
        {
            // adds data to sending queue
            lock (queueToSent_locker)
            {
                queueToSent.Enqueue(instructionPacket);
            }
        }

        public static void QUEUE_PacketSent(C_Packet instructionPacket)
        {
            int rotMot = (int)instructionPacket.rotMotor;
            if (C_Packet.IS_statusPacketFollowing(instructionPacket) == true)
            {
                lock (queueSent_locker)
                {
                    instructionPacket.sentTime = DateTime.UtcNow;
                    queueSent[rotMot].Enqueue(instructionPacket);
                    queueSent_Count[rotMot] = queueSent[rotMot].Count;
                    C_MotorControl.ACTUALIZE_queueCounts(queueSent);
                }
            }
        }

        private static void workerSEND_DoWork(object sender, DoWorkEventArgs e)
        {
            C_Packet thisInstructionPacket;
            LOG_debug("Start to read packet");
            lock (spiSent_locker)
            {
                if (spi.IsOpen == true)
                {
                    spi.DiscardInBuffer();
                //    spi.DiscardOutBuffer();
                }
                
                lock (queueToSent_locker)
                {
                    if (queueToSent.Count != 0)
                    {
                        thisInstructionPacket = queueToSent.Dequeue();
                        if (C_SPI.WRITE_instructionPacket(thisInstructionPacket) == false)
                        {
                            LOG_err(string.Format(
                                "Cannot open the serial port {0}. Tried [{1}]-times", spi.PortName, openConnection.ValDef
                                ));
                            return;
                        }
                    }
                    else
                    {
#if (!DEBUG)
                        throw new InvalidOperationException(
                            "An error occured when tried to send data!\nThe queue of data to send was empty!");rr
#endif
                        LOG_err("Packet queue is empty! Cannot send packet");
                        return;
                    }
                    spi.DiscardOutBuffer();
                }


            }
        }

        private static void workerSEND_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // catch if response was A-OK
            if (e.Error != null)
            {
                LOG_sent(GET_exInfo(e.Error));
            }
            else
            {
                //e.Result = "tocovrati writeData";
            }
        }

        private static bool WRITE_instructionPacket(C_Packet instructionPacket)
        {
            openConnection.Restart();
            while (openConnection.Decrement() != 0)
            {
                if (spi.IsOpen)
                {
                    QUEUE_PacketSent(instructionPacket);
                    byte[] data = instructionPacket.PacketBytes;
                    LOG_cmd(data, e_cmd.sending);
                    WRITE_byteArray(data);
                   
                    C_Packet.PROCESS_instructionPacket(instructionPacket);

                    return true;
                }
                else
                {
                    OPEN_connection();
                }
            }
            return false; 
        }

        private static void WRITE_byteArray(byte[] byteArray)
        {
            spi.RtsEnable = false;
            spi.Write(byteArray, 0, byteArray.Length);
            // Wait for the dispatch of bytes and enable response mode afterward
            Thread.Sleep(timeWaitBeforeRtsEnable_ms); 
            spi.RtsEnable = true;
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Writing
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Reading
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static bool START_withPacketStartBytes(List<byte> packetBytes)
        {
            int q = 0;
            foreach (byte by in C_DynAdd.PACKETSTART)
            {
                if (packetBytes[q] != by)
                {
                    LOG_unimportant(string.Format(
                        "This packet does not start with PACKETSTART bytes: [{0}]",
                        C_CONV.byteArray2strHex_space(packetBytes.ToArray())
                        ));
                    return false;
                }
            }
            return true;
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
        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.spi_err, _msg);
        }

        public static string GET_exInfo(Exception ex)
        {
            return "Catched exception: " + ex.Message; 
        }

        public static void LOG_debug(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.debug, _msg);
        }

        public static void LOG_unimportant(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.unimportant, _msg);
        }


        public static void LOG_cmd(byte[] cmd, e_cmd type)
        {
            string prefix = "";
            string hex = BitConverter.ToString(cmd).Replace("-", " ");

            switch (type)
            {
                case (e_cmd.sending):
                    prefix = "Sending:\t";
                    LOG_sent(hex);
                    break;
                case (e_cmd.received):
                    prefix = "Got:\t";
                    LOG_got(hex);
                    break;
                case (e_cmd.receivedEchoOf):
                    prefix = "Echo confirm:\t";
                    LOG_got(hex);
                    break;
                case (e_cmd.
                    receivedStatusPacket):
                    prefix = "Got Status:\t";
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
    }

}
