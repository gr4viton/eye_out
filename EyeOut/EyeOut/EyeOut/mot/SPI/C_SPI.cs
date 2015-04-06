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
        sent = 1, received, receivedEchoOf, receivedStatusPacket, receivedCheckNot, receivedWithError
    };


    internal class C_SPI
    {
        private static object spi_locker = new object();
        private static object queue_locker = new object();
        
        public static SerialPort spi;
        //private static BackgroundWorker worker_SEND;
        private static Queue<C_Packet> packetQueue;

        public static byte[] readBuff;
        public static int i_readBuff = 0;
        public static byte this_byte;
        
        //public static C_Packet packetSent;
        //public static C_Packet packetReceived;

        public static List<byte> receivedPacketBytes;
        public static int i_receivedByte;

        //public static byte[] curCmd;
        //public static int i_curCmd;
        //public static byte[] lastCmd;

        static C_CounterDown openConnection = new C_CounterDown(10); // try to open connection x-times
        static C_CounterDown readReturn = new C_CounterDown(10); // try to read return status packet x-times
        public static int timeoutExceptionPeriod = 10;

        // const!?
        public static int packetNumOfBytes; // number of bytes in received packet - including PACKETSTART bytes
        //public static int i_cmdId = 0;     // = first byte in status packet (not counting 0xff 0xff)
        //public static int i_cmdError = 2;  // = third byte in status packet (not counting 0xff 0xff)

        //public static byte curCmd_id;
        //public static byte curCmd_len;

        public static bool INCOMING_PACKET = false;


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // SPI hang - thread: http://www.codeproject.com/Questions/179614/Serial-Port-in-WPF-Application

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public C_SPI()
        {
            i_readBuff = 0;
            readBuff = new byte[1024];
            //curCmd = new byte[1];

            timeoutExceptionPeriod = 10; // according to datahseet.?.
            // spi
            //spi = new SerialPort("COM6", 57600, Parity.None, 8, StopBits.One);
            spi = new SerialPort("COM6", 57600, Parity.None, 8, StopBits.One);

            /*
            SPI.Handshake = System.IO.Ports.Handshake.None;
            SPI.ReadTimeout = 200;
            SPI.WriteTimeout = 50;*/

            // NOT NEEDED as all the motors are just CLIENTS - only responding to my (SERVER) orders
            //spi.DataReceived += new SerialDataReceivedEventHandler(SPI_DataReceivedHandler);

            packetQueue = new Queue<C_Packet>();
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
                LOG_ex(ex); 
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
        //// I allways need to know if the command send will produce some command receive!
        //public static void SEND_data(byte[] data)
        //{
        //    SEND_data(e_cmdEcho.noEcho, data);
        //}
       
        public static void SEND_data(C_Packet instructionPacket)
        {
            QUEUE_data(instructionPacket);

            BackgroundWorker worker_SEND = new BackgroundWorker();
            worker_SEND.DoWork += workerSEND_DoWork;
            worker_SEND.RunWorkerCompleted += workerSEND_RunWorkerCompleted;
            //worker_SEND.RunWorkerAsync((object)echo);
            worker_SEND.RunWorkerAsync();
        }


        private static void QUEUE_data(C_Packet instructionPacket)
        {
            // adds data to sending queue
            lock (queue_locker)
            {
                packetQueue.Enqueue(instructionPacket);
            }
        }

        private static void workerSEND_DoWork(object sender, DoWorkEventArgs e)
        {
            C_Packet thisInstructionPacket;
            lock (spi_locker)
            {
                if (spi.IsOpen == true)
                {
                    spi.DiscardInBuffer();
                    spi.DiscardOutBuffer();
                }
                lock (queue_locker)
                {
                    if (packetQueue.Count != 0)
                    {
                        thisInstructionPacket = packetQueue.Dequeue();
                        if (C_SPI.WRITE_instructionPacket(thisInstructionPacket) == false)
                        {
                            LOG_err(string.Format(
                                "Cannot open the serial port. Tried [{0}]-times", openConnection.ValDef
                                ));
                        }
                    }
                    else
                    {
#if (!DEBUG)
                        throw new InvalidOperationException(
                            "An error occured when tried to send data!\nThe queue of data to send was empty!");
#endif
                        LOG_err("Packet queue is empty! Cannot send packet");
                        return;
                    }
                }

                if (C_Packet.IS_statusPacketFollowing(thisInstructionPacket) == true)
                {
                    TRY_READ_packet(thisInstructionPacket); 
                }
            }
        }

        private static void TRY_READ_packet(C_Packet instructionPacket)
        {
            bool packetRead = false;
            readReturn.Restart();

            // TODO: implement timeoutException
            //try
            //{

            //}
            //catch (TimeoutException ex)
            //{

            //}

            while (readReturn.Decrement() != 0)
            {
                // read out echo
                packetRead = READ_packet(instructionPacket);
                if (packetRead == true)
                {
                    break;
                }
            }
            if (packetRead == false)
            {
                // didn't read anything
                LOG_err(string.Format(
                    "Cannot read the response status packet from serial port. Tried [{0}]-times", readReturn.ValDef
                    ));
            }
        }

        private static void workerSEND_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // catch if response was A-OK
            if (e.Error != null)
            {
                LOG_ex(e.Error);
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
                    byte[] data = instructionPacket.PacketBytes;
                    WRITE_byteArray(data);
                    C_Packet.LOG_sent(instructionPacket);
                    LOG_cmd(data, e_cmd.sent);
                    //packetSent = instructionPacket;

                    return true;
                }
                else
                {
                    OPEN_connection();
                }
            }
            return false; 
        }

        private static void WRITE_byteArray(byte[] data)
        {
            spi.Write(data, 0, data.Length);
            //lastCmd = data;

            // IS NEEDED another zeros for slowing down serial commands
            // can be deleted if the Resets Return Delay Time is set greater - as in Example 9 from RX-64 Manual
            byte[] zeros = new byte[20];
            for (int q = 0; q < 20; q++)
            {
                zeros[q] = 0;
            }
            spi.Write(zeros, 0, zeros.Length);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Writing
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Reading
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        public static bool READ_packet(C_Packet lastSent)
        {
            if (C_State.prog == e_stateProg.closing)
            {
                System.Threading.Thread.CurrentThread.Abort();
            }

            // this function tries to read echo / return message of the motor after sending [lastSentCmd]
            int numPacket = 0;           

            lock (spi_locker)
            {
                try
                {
                    receivedPacketBytes = new List<byte>();
                    
                    while (0 != C_SPI.spi.BytesToRead)
                    {
                        this_byte = (byte)C_SPI.spi.ReadByte();
                        readBuff[i_readBuff] = this_byte;
                        if (INCOMING_PACKET == true)
                        {
                            receivedPacketBytes.Add(this_byte); // we already have PACKETSTART bytes filled in


                            if (i_receivedByte == C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET) // get the LENGTH_BYTE to find out on which byte the packet ends
                            {
                                    // this_byte = LENGTH_BYTE = NParams + 2
                                    // packetNumOfBytes = PACKETSTART[2] + ID + Length + Error + Nparam + CheckSum = NParams + 6
                                    packetNumOfBytes = (int)this_byte + 4;
                            }
                            if (i_receivedByte == packetNumOfBytes - 1) // this_byte is the last received byte from this packet
                            {
                                INCOMING_PACKET = false; 
                                C_Packet.PROCESS_receivedPacket(lastSent, receivedPacketBytes, numPacket);
                                numPacket++;
                            }
                            i_receivedByte++;
                        }
                        // not detected NEW message yet
                        if (i_readBuff > 0)
                        {
                            // C_DynAdd.PACKETSTART detection
                            if ((readBuff[i_readBuff] == 0xFF) && (readBuff[i_readBuff - 1] == 0xFF))
                            {
                                INCOMING_PACKET = true;
                                i_readBuff = 0; // reset index counters
                                receivedPacketBytes = new List<byte>();

                                // insert the PACKETSTART sequence 
                                for( int q = 0; q< C_DynAdd.SIZEOF_PACKETSTART; q++)
                                {
                                    receivedPacketBytes.Add(C_DynAdd.PACKETSTART[q]);
                                }
                                i_receivedByte = receivedPacketBytes.Count; // as we have already detected the PACKETSTART sequence
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
                    LOG_ex(ex);
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
        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.spi, _msg);
        }

        public static void LOG_ex(Exception ex)
        {
            string msg = "Catched exception: " + ex.Message; 
            C_Logger.Instance.LOG_err(e_LogMsgSource.spi, msg);
        }




        public static void LOG_cmd(byte[] cmd, e_cmd type)
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

        //// not used
        //private static void SPI_CHECK_receivedCmd(byte[] cmdWithoutChecksumByte, byte rec_checkSum)
        //{
        //    // check for [checksum error] and cmd [error byte] sub-bites disambiguation
        //    byte calc_checkSum = C_CheckSum.GET_checkSum(cmdWithoutChecksumByte);

        //    if (C_CheckSum.CHECK_checkSum(calc_checkSum, rec_checkSum))
        //    //if( calc_check == 0 )
        //    {
        //        if (cmdWithoutChecksumByte[i_cmdError] == 0)
        //            // no error
        //            LOG_cmd(cmdWithoutChecksumByte, e_cmd.received);
        //        else
        //        {
        //            LOG_cmd(cmdWithoutChecksumByte, e_cmd.receivedWithError);
        //            LOG_cmdError(cmdWithoutChecksumByte[i_cmdId], cmdWithoutChecksumByte[i_cmdError]);
        //        }
        //    }
        //    else
        //    {
        //        LOG_cmd(cmdWithoutChecksumByte, e_cmd.receivedCheckNot);
        //        LOG(String.Format("CheckSumGot != CheckSumCounted :: {0} != {1}", (byte)rec_checkSum, (byte)calc_checkSum));
        //        //LOG_msgAppendLine(String.Format("CheckSumGot = {0} ", (byte)calc_check));
        //    }
        //}
    }

}
