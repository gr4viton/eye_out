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
        private static object queueToSent_locker = new object();
        private static object queueSent_locker = new object();
        
        public static SerialPort spi;
        private static Queue<C_Packet> queueToSent; // packets which are going to be sent
        private static List<Queue<C_Packet>> queueSent; // packets which was written and are waiting for getting some return status packet

        static C_CounterDown openConnection = new C_CounterDown(10); // try to open connection x-times
        static C_CounterDown readReturn = new C_CounterDown(10); // try to read return status packet x-times
        public static int timeoutExceptionPeriod = 10;
        

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
            SPI.Handshake = System.IO.Ports.Handshake.None;
            SPI.ReadTimeout = 200;
            SPI.WriteTimeout = 50;*/
            spi.Handshake = System.IO.Ports.Handshake.None;
            spi.ReadTimeout = 1000;

            // NOT NEEDED as all the motors are just CLIENTS - only responding to my (SERVER) orders
            //spi.DataReceived += new SerialDataReceivedEventHandler(SPI_DataReceivedHandler);

            queueToSent = new Queue<C_Packet>();
            queueSent = new List<Queue<C_Packet>>()
            {
                new Queue<C_Packet>(),
                new Queue<C_Packet>(),
                new Queue<C_Packet>(),
            };
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
                LOG(GET_exInfo(ex)); 
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
            lock (queueToSent_locker)
            {
                queueToSent.Enqueue(instructionPacket);
            }
        }

        private static void workerSEND_DoWork(object sender, DoWorkEventArgs e)
        {
            C_Packet thisInstructionPacket;
            LOG_debug("Start to read packet");
            lock (spi_locker)
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
                                "Cannot open the serial port. Tried [{0}]-times", openConnection.ValDef
                                ));
                            return;
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
                    spi.DiscardOutBuffer();
                }

                if (C_Packet.IS_statusPacketFollowing(thisInstructionPacket) == true)
                {
                    lock (queueSent_locker)
                    {
                        queueSent[(int)thisInstructionPacket.rotMotor].Enqueue(thisInstructionPacket);
                    }
                    int packetRead = 0;
                    packetRead = READ_packet(thisInstructionPacket);
                    LOG_got("["+packetRead.ToString()+"] successfully read");

                    //TRY_READ_packet(thisInstructionPacket); 
                    //spi.DiscardInBuffer();
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
                    byte[] data = instructionPacket.PacketBytes;
                    WRITE_byteArray(data);
                    LOG_cmd(data, e_cmd.sent);
                   
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

        private static void WRITE_byteArray(byte[] data)
        {
            spi.Write(data, 0, data.Length);
            //lastCmd = data;

            // IS NEEDED another zeros for slowing down serial commands
            // can be deleted if the Resets Return Delay Time is set greater - as in Example 9 from RX-64 Manual
            //byte[] zeros = new byte[20];
            //for (int q = 0; q < 20; q++)
            //{
            //    zeros[q] = 0;
            //}
            //spi.Write(zeros, 0, zeros.Length);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Writing
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Reading
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static bool IS_withStartPacketBytes(List<byte> packetBytes)
        {

            int q = 0;
            foreach (byte by in C_DynAdd.PACKETSTART)
            {
                if (packetBytes[q] != by)
                {
                    LOG_err(string.Format(
                        "This packet does not start with PACKETSTART bytes: [{0}]",
                        C_CONV.byteArray2strHex_space(packetBytes.ToArray())
                        ));
                    return false;
                }
            }
            return true;
        }
        public static int READ_packet(C_Packet lastSent)
        {
            if (C_State.prog == e_stateProg.closing)
            {
                System.Threading.Thread.CurrentThread.Abort();
            }
            int numPacket = 0;
            LOG_debug("Start to read packet");
            try
            {
                LOG_debug("locked spi");
                //lock (spi_locker)
                {
                    const int packetLength_min = 6; // shortest packet consists of 6bytes
                    const int IndexOfLength = C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET;

                    List<byte> packetBytes = new List<byte>();
                    int i_packetByte = 0;
                    int packetLength = packetLength_min;
                    bool INCOMING_PACKET = false;

                    byte receivedByte;
                    while (
                        (0 != C_SPI.spi.BytesToRead)
                        ||
                        (INCOMING_PACKET == true)
                        )
                    {
                        receivedByte = (byte)C_SPI.spi.ReadByte();
                        packetBytes.Add(receivedByte);

                        if (INCOMING_PACKET == false) // PACKETSTART DETECTION
                        {
                            if (IS_withStartPacketBytes(packetBytes) == true)
                            {
                                INCOMING_PACKET = true;
                            }
                            else
                            {
                                packetBytes.Clear(); 
                                return numPacket;
                            }
                        }
                        else if (INCOMING_PACKET == true)
                        {
                            i_packetByte = packetBytes.Count-1;
                            if (i_packetByte == IndexOfLength) // get the LENGTH_BYTE 
                            {
                                // LENGTH_BYTE = N*[Params] + 1*[LEN] + 1*[INS/ERROR]
                                // packetLength = [LENGTH_BYTE] + 1*[ID] + 1*[CheckSum] + 2*[PacketStart]
                                packetLength = (int)receivedByte + 4;
                            }
                            i_packetByte++;
                            if (i_packetByte == packetLength ) // last byte
                            {
                                INCOMING_PACKET = false;
                                numPacket++;

                                LOG_cmd(packetBytes.ToArray(), e_cmd.received);
                                C_Packet.PROCESS_receivedPacket(lastSent, packetBytes, numPacket);

                                packetBytes.Clear();
                                i_packetByte = 0;
                                packetLength = packetLength_min;
                                //continue;
                            }
                            else if (i_packetByte > packetLength)
                            {
                                LOG_err(String.Format(
                                    "Strange thing happened, there were more bytes read from packet than should: {0} from {1}",
                                    i_packetByte, packetLength));

                                    
                                INCOMING_PACKET = false;
                                packetBytes.Clear();
                                i_packetByte = 0;
                                packetLength = packetLength_min;
                            }

                        }


                        if (numPacket == 2)
                        {
                            break; // get only one packet
                        }
                    }
                    if (INCOMING_PACKET == true)
                    {
                        LOG_err(string.Format(
                            "There are no more [BytesToRead]! Packet bytes read [{0}/{1}] = [{2}]",
                            i_packetByte, packetLength, C_CONV.byteArray2strHex_space(packetBytes.ToArray())
                            ));
                    }
                }
            }
            catch (Exception ex)
            {
                LOG_got(GET_exInfo(ex));
            }
            return numPacket;
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
