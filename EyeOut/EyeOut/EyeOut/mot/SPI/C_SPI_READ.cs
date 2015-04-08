using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Threading;

namespace EyeOut
{
    /// <summary>
    /// read part of C_SPI
    /// </summary>
    internal partial class C_SPI
    {
        static C_CounterDown readReturn = new C_CounterDown(10); // try to read return status packet x-times

        private static object queueSent_locker = new object();
        private static List<Queue<C_Packet>> queueSent; // packets which was written and are waiting for getting some return status packet

        const int packetLength_min = 6; // shortest packet consists of 6bytes
        const int IndexOfLength = C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET;

        static Queue<byte> readBuffer = new Queue<byte>();
        static List<byte> packetBytes = new List<byte>();
        static int packetBytes_Count = 0;
        static int i_packetByte = 0;
        static int packetLength = packetLength_min;
        static bool INCOMING_PACKET = false;
        // for three motors
        static List<C_Packet> lastSent = new List<C_Packet>()
                    {
                        new C_Packet(),
                        new C_Packet(),
                        new C_Packet()
                    };
        static List<bool> lastSent_returnStatusPacketProcessed 
            = new List<bool> { true, true, true };
        static byte receivedByte;


        private static void SPI_DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            lock (spi_locker)
            {
                SerialPort sp = (SerialPort)sender;
                //bool READ_packets = true;
                int numPacket = 0;
                LOG_debug("Start to read packet");
                try
                {

                    // when item is true -> lastSent packet for this motor (index) was already processed
                    // load new lastSent packet to this index from queueSent[index]


                    // read all the bytes to read
                    while (0 != sp.BytesToRead)
                    {
                        receivedByte = (byte)C_SPI.spi.ReadByte();
                        readBuffer.Enqueue(receivedByte);
                    }

                    while (readBuffer.Count != 0)
                    {
                        receivedByte = readBuffer.Dequeue();
                        packetBytes.Add(receivedByte);
                        packetBytes_Count = packetBytes.Count;
                        if (packetBytes_Count > 1)
                        {
                            i_packetByte = packetBytes_Count - 1;

                            // PACKETSTART DETECTION
                            // it should be even inside another packet
                            //if (INCOMING_PACKET == false) 
                            if (
                                (packetBytes[i_packetByte] == C_DynAdd.PACKETSTART[1])
                                ||
                                (packetBytes[i_packetByte - 1] == C_DynAdd.PACKETSTART[0])
                                )
                            {
                                if (packetBytes_Count > 2)
                                {
                                    FLUSH_forNextIncomingPackage("found another start packet sequence");
                                    continue;
                                }
                                INCOMING_PACKET = true;
                            }

                            // PACKETSTART DETECTED 
                            if (INCOMING_PACKET == true)
                            {
                                if (i_packetByte == C_DynAdd.INDEXOF_ID_IN_STATUSPACKET)
                                {
                                    if (receivedByte > C_DynAdd.ID_BROADCAST)
                                    {
                                        // not allowed value for id -> the packet must have shifted
                                        // trim it from left
                                        packetBytes.Remove(0); // assume that it starts with PACKETSTART as it went through checking algorithms
                                    }
                                }

                                if (i_packetByte == IndexOfLength) // get the LENGTH_BYTE 
                                {
                                    // LENGTH_BYTE = N*[Params] + 1*[LEN] + 1*[INS/ERROR]
                                    // packetLength = [LENGTH_BYTE] + 1*[ID] + 1*[CheckSum] + 2*[PacketStart]
                                    packetLength = (int)receivedByte + 4;
                                }

                                if (i_packetByte >= packetLength - 1) // last byte ==
                                {
                                    numPacket++;

                                    LOG_cmd(packetBytes.ToArray(), e_cmd.received);
                                    LOG_debug("end of packet");
                                    // chose lastSent by receivedPacketBytes idByte
                                    if (i_packetByte > packetLength)
                                    {
                                        // trim the packet
                                        LOG_err(String.Format(
                                            "Packet too long, there were more bytes read from packet than should: {0} from {1}! it will by cutted",
                                            i_packetByte, packetLength));
                                        packetBytes = packetBytes.GetRange(0, packetLength);
                                    }
                                    PAIR_andProcessStatusPacket();

                                    FLUSH_forNextIncomingPackage("normal end of packet");
                                }

                                if (i_packetByte > packetLength)
                                {
                                    LOG_err(String.Format(
                                        "Strange thing happened, there were more bytes read from packet than should: {0} from {1}",
                                        i_packetByte, packetLength));

                                    FLUSH_forNextIncomingPackage("packet too long");
                                }

                            }
                        }

                        //if (INCOMING_PACKET == true)
                        //{
                        //    //LOG_err(string.Format(
                        //    //    "There are no more [BytesToRead] in the serial port readBuffer now! Packet bytes read [{0}/{1}] = [{2}]\nWaiting for more!",
                        //    //    i_packetByte, packetLength, C_CONV.byteArray2strHex_space(packetBytes.ToArray())
                        //    //    ));
                        //}

                        if (C_State.prog == e_stateProg.closing)
                        {
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOG_err(GET_exInfo(ex));
                    FLUSH_forNextIncomingPackage("exception");
                }

            }
        }

        public static void FLUSH_forNextIncomingPackage(string because)
        {
            LOG_debug("Stopped receiving this packet : " + 
                C_CONV.byteArray2strHex_space( packetBytes.ToArray())
                + "| because:" + because);
            INCOMING_PACKET = false;
            packetBytes.Clear();
            i_packetByte = 0;
            packetLength = packetLength_min;
            LOG_debug("flushed packet residuum");
        }

        public static bool PAIR_andProcessStatusPacket()
        {
            lock (queueSent_locker)
            {
                int thisStatusId = packetBytes[C_DynAdd.INDEXOF_ID_IN_STATUSPACKET];
                e_rot rot;
                bool foundMotor = C_MotorControl.GET_motorRotFromId(thisStatusId, out rot);
                if (foundMotor == true)
                {
                    int index = (int)rot;
                    LOG_debug("index = " + index.ToString());

                    // acquire the unprocessed lastSent package from queue if needed
                    if (lastSent_returnStatusPacketProcessed[index] == true)
                    {
                        if (queueSent[index].Count > 0)
                        {
                            lastSent[index] = (queueSent[index]).Dequeue();
                        }
                        else
                        { 
                            return false; 
                        }
                    }

                    if (lastSent.Count >= index + 1)
                    {
                        LOG_sent(string.Format(
                        "Paired status package: {0}\n with this sentPackage: {1}",
                        C_CONV.byteArray2strHex_space(packetBytes.ToArray()),
                        lastSent[index].PacketBytes_toString
                        ));

                        LOG_debug("now processing packet");

                        // process paired lastSent and this statusPacket
                        lastSent_returnStatusPacketProcessed[index] =
                            C_Packet.PROCESS_receivedPacket(lastSent[index], packetBytes);

                        return lastSent_returnStatusPacketProcessed[index];
                    }

                }
                return false;
            }
        }

    }
}
