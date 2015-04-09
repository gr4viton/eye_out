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

        //const int packetLength_min = 6; // shortest packet consists of 6bytes
        //const int IndexOfLength = C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET;

        //static Queue<byte> readBuffer = new Queue<byte>();
        static List<byte> packetBytes = new List<byte>();
        //static int cnt = 0;
        //static int i_packetByte = 0;
        //static int packetLength = packetLength_min;
        //static bool INCOMING_PACKET = false;
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

        private static int GET_packetLength(byte lengthByte)
        {
            return lengthByte + 4;
        }

        private static int GET_packetLength(List<byte> packetBytes)
        {

            if (packetBytes.Count - 1 >= C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET)
            {
                return GET_packetLength(packetBytes[C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET]);
            }
            else
            {
                return C_DynAdd.MIN_BYTES_OF_PACKET;
            }
        }


        private static int PACKETSTART_detector(List<byte> packetBytes, bool searchFor2nd = true)
        {
            // [return]
            //  -1 if there is no PACKETSTART sequention detected
            // index of the first byte of the first PACKETSTART if its the only one (0 if its the first bytes)
            // index of the first byte of the second PACKETSTART sequention if there are more than one

            List<int> i = new List<int>();
            for(int q=0; q<packetBytes.Count()-1; q++)
            {
                if(
                    (packetBytes[q] == C_DynAdd.PACKETSTART[0])
                    ||
                    (packetBytes[q+1] == C_DynAdd.PACKETSTART[1])
                    )
                {
                    // found the first one
                    i.Add(q);
                    if(searchFor2nd == true)
                    {
                        // serch for second in subList
                        int second = PACKETSTART_detector(new List<byte>(packetBytes.Skip(q + 1)), false);
                        if (second == -1)
                        {
                            return q;
                        }
                        else
                        {
                            return second;
                        }
                    }
                }

            }
            return -1;
        }
        private static void SPI_DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            lock (spi_locker)
            {
                SerialPort sp = (SerialPort)sender;
                Queue<byte> readBuffer = new Queue<byte>();

                LOG_debug("Start to read packet");
                try
                {

                    // when item is true -> lastSent packet for this motor (index) was already processed
                    // load new lastSent packet to this index from queueSent[index]
                    //int cnt = 0;
                    bool INCOMING_PACKET;//START_withPacketStartBytes(packetBytes);
                    
                    int packetLength = GET_packetLength(packetBytes);

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
                        //cnt = packetBytes.Count;

                        if (packetBytes.Count >= 2)
                        {
                            // PACKETSTART detector - detect whether the start sequence is not only in the first byte but wherever in the packetBytes array
                            int i_start = PACKETSTART_detector(packetBytes); 
                            if ( i_start == 0 )
                            {
                                INCOMING_PACKET = true;
                            }
                            else if (i_start > 0)
                            {
                                LOG_debug(string.Format(
                                    "Found another PACKETSTART sequence inside recieving packetBytes on index [{0}] in : {1}"
                                    + "\nthis is not allowed - skip this packet and use the next PACKETSTART sequence",
                                    i_start, C_CONV.byteArray2strHex_space (packetBytes.ToArray())
                                    ));
                                packetBytes.RemoveRange(0, packetBytes.Count - i_start - 1); // -1??
                                // try to use the now trimmed part of packet as a full packet ?
                                INCOMING_PACKET = true;
                                continue;
                                
                            }
                            else if (i_start == -1)
                            {
                                LOG_debug("Did not found any PACKETSTART sequence in receiving packet : " +
                                    C_CONV.byteArray2strHex_space(packetBytes.ToArray()).ToString());
                                INCOMING_PACKET = false;
                                continue;
                            }
                            else
                            {
                                LOG_debug("Not allowed state, the PACKETSTART not detected nor detected - INCOMING_PACKET=false");
                                INCOMING_PACKET = false;
                            }
                            if (INCOMING_PACKET == true)
                            {
                                if (packetBytes.Count - 1 == C_DynAdd.INDEXOF_ID_IN_STATUSPACKET)
                                {
                                    if (receivedByte > C_DynAdd.ID_BROADCAST)
                                    {
                                        LOG_debug("Not allowed value of id : " + receivedByte.ToString());
                                        packetBytes.RemoveAt(0); // remove the first byte as it disables the PACKETSTART detection with positioning the id byte as this
                                        INCOMING_PACKET = false;
                                        continue; // it will be catched by PACKETSTART detector
                                    }
                                }

                                if (packetBytes.Count - 1 == C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET) // get the LENGTH_BYTE 
                                {
                                    packetLength = GET_packetLength(packetBytes);
                                    if (packetLength > C_DynAdd.MAX_BYTES_OF_PACKET)
                                    {
                                        // too long packet
                                        LOG_debug(string.Format(
                                            "Packet length byte greater than allowed = {0} > {1}",
                                            packetLength, C_DynAdd.MAX_BYTES_OF_PACKET
                                            ));
                                        INCOMING_PACKET = false;
                                        continue;
                                    }
                                    if (packetLength < C_DynAdd.MIN_BYTES_OF_PACKET)
                                    {
                                        // too short packet
                                        LOG_debug(string.Format(
                                            "Packet length byte smaller than allowed = {0} < {1}",
                                            packetLength, C_DynAdd.MIN_BYTES_OF_PACKET
                                            ));
                                        INCOMING_PACKET = false;
                                        continue;
                                    }
                                    // LENGTH_BYTE = N*[Params] + 1*[LEN] + 1*[INS/ERROR]
                                    // packetLength = [LENGTH_BYTE] + 1*[ID] + 1*[CheckSum] + 2*[PacketStart]
                                    //packetLength = (int)receivedByte + 4;
                                }
                                packetLength = GET_packetLength(packetBytes);
                                if (packetBytes.Count >= packetLength)
                                {
                                    // the last added byte to packetBytes was the most likely last byte of this package
                                    INCOMING_PACKET = false;
                                    LOG_cmd(packetBytes.ToArray(), e_cmd.received);
                                    LOG_debug("end of packet");

                                    // chose lastSent by receivedPacketBytes idByte
                                    PAIR_andProcessStatusPacket(packetBytes);
                                    packetBytes.Clear();
                                }
                                if (packetBytes.Count > packetLength)
                                {
                                    // trim the packet
                                    LOG_err(String.Format(
                                        "Packet too long, there were more bytes read from packet than the length byte proposes: "+
                                        "{0} from {1}! LENBYTE={2}! \nIt will by cutted! : {3}",
                                        packetBytes.Count, packetLength, 
                                        packetBytes[C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET],
                                        C_CONV.byteArray2strHex_space(packetBytes.ToArray())
                                        ));
                                    packetBytes = packetBytes.GetRange(0, GET_packetLength(packetBytes));
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
                    //FLUSH_forNextIncomingPackage("exception");
                }
                // try if there is not a full packet in the packetBytes
                if (packetBytes.Count == GET_packetLength(packetBytes))
                {
                    if (PACKETSTART_detector(packetBytes) == 0)
                    {
                        PAIR_andProcessStatusPacket(packetBytes);
                        packetBytes.Clear();
                    }
                }

            }
            LOG_debug(string.Format("packetBytes=[{3}]; queueSent: yaw=[{0}]; pitch=[{1}]; roll=[{2}]",
                queueSent[0].Count, queueSent[1].Count, queueSent[2].Count, packetBytes.Count));
        }

        //public static void FLUSH_forNextIncomingPackage(string because)
        //{
        //    LOG_debug("Stopped receiving this packet : " + 
        //        C_CONV.byteArray2strHex_space( packetBytes.ToArray())
        //        + "| because:" + because);
        //    INCOMING_PACKET = false;
        //    packetBytes.Clear();
        //    i_packetByte = 0;
        //    packetLength = packetLength_min;
        //    LOG_debug("flushed packet residuum");
        //}

        public static bool PAIR_andProcessStatusPacket(List<byte> packetBytes)
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
