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
        private static object spiGot_locker = new object();
        static C_CounterDown readReturn = new C_CounterDown(10); // try to read return status packet x-times

        private static object queueSent_locker = new object();
        public static int[] queueSent_Count = new int[] { 0,0,0 };
        private static List<Queue<C_Packet>> queueSent; // packets which was written and are waiting for getting some return status packet

        //const int packetLength_min = 6; // shortest packet consists of 6bytes
        //const int IndexOfLength = C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET;

        //static Queue<byte> readBuffer = new Queue<byte>();
        static List<byte> receivingPacketBytes = new List<byte>();
        //static int cnt = 0;
        //static int i_packetByte = 0;
        //static int packetLength = packetLength_min;
        //static bool INCOMING_PACKET = false;
        // for three motors
        //static List<C_Packet> lastSent = new List<C_Packet>()
        //            {
        //                new C_Packet(),
        //                new C_Packet(),
        //                new C_Packet()
        //            };
        //static List<bool> lastSent_returnStatusPacketProcessed 
        //    = new List<bool> { true, true, true };
        //static byte receivedByte;

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
            if (packetBytes.Count >= 2)
            {
                List<int> i = new List<int>();
                for (int q = 0; q < packetBytes.Count() - 1; q++)
                {
                    if (
                        (packetBytes[q] == C_DynAdd.PACKETSTART[0])
                        ||
                        (packetBytes[q + 1] == C_DynAdd.PACKETSTART[1])
                        )
                    {
                        // found the first one
                        i.Add(q);
                        if (searchFor2nd == true)
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
            }
            return -1;
        }
        private static void SPI_DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            lock (spiGot_locker)
            {
                SerialPort sp = (SerialPort)sender;
                Queue<byte> readBuffer = new Queue<byte>();
                byte receivedByte = 0;
                LOG_unimportant("Start to read packet");
                try
                {

                    // when item is true -> lastSent packet for this motor (index) was already processed
                    // load new lastSent packet to this index from queueSent[index]
                    //int cnt = 0;
                    bool INCOMING_PACKET = false;//= START_withPacketStartBytes(packetBytes);

                    if (receivingPacketBytes.Count <= GET_packetLength(receivingPacketBytes))
                    {
                        if (PACKETSTART_detector(receivingPacketBytes) == 0)
                        {
                            INCOMING_PACKET = true;
                        }
                    }
                    int packetLength = GET_packetLength(receivingPacketBytes);

                    // read all the bytes to read
                    while (0 != sp.BytesToRead)
                    {
                        receivedByte = (byte)C_SPI.spi.ReadByte();
                        readBuffer.Enqueue(receivedByte);
                    }

                    while (readBuffer.Count != 0)
                    {
                        receivedByte = readBuffer.Dequeue();
                        receivingPacketBytes.Add(receivedByte);
                        //cnt = packetBytes.Count;

                        if (receivingPacketBytes.Count >= 2)
                        {
                            // PACKETSTART detector - detect whether the start sequence is not only in the first byte but wherever in the packetBytes array
                            int i_start = PACKETSTART_detector(receivingPacketBytes); 
                            if ( i_start == 0 )
                            {
                                INCOMING_PACKET = true;
                            }
                            else if (i_start > 0)
                            {
                                LOG_debug(string.Format(
                                    "Found another PACKETSTART sequence inside recieving packetBytes on index [{0}] in : {1}"
                                    + "\nthis is not allowed - skip this packet and use the next PACKETSTART sequence",
                                    i_start, C_CONV.byteArray2strHex_space (receivingPacketBytes.ToArray())
                                    ));
                                receivingPacketBytes.RemoveRange(0, receivingPacketBytes.Count - i_start - 1); // -1??
                                // try to use the now trimmed part of packet as a full packet ?
                                INCOMING_PACKET = true;
                                continue;
                                
                            }
                            else if (i_start == -1)
                            {
                                LOG_debug("Did not found any PACKETSTART sequence in receiving packet : " +
                                    C_CONV.byteArray2strHex_space(receivingPacketBytes.ToArray()).ToString());
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
                                if (receivingPacketBytes.Count - 1 == C_DynAdd.INDEXOF_ID_IN_STATUSPACKET)
                                {
                                    if (receivedByte > C_DynAdd.ID_BROADCAST)
                                    {
                                        LOG_debug("Not allowed value of id : " + receivedByte.ToString());
                                        receivingPacketBytes.RemoveAt(0); // remove the first byte as it disables the PACKETSTART detection with positioning the id byte as this
                                        INCOMING_PACKET = false;
                                        continue; // it will be catched by PACKETSTART detector
                                    }
                                }

                                if (receivingPacketBytes.Count - 1 == C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET) // get the LENGTH_BYTE 
                                {
                                    packetLength = GET_packetLength(receivingPacketBytes);
                                    if (packetLength > C_DynAdd.MAX_BYTES_OF_PACKET)
                                    {
                                        // too long packet
                                        LOG_debug(string.Format(
                                            "Packet length byte greater than allowed = {0} > {1}",
                                            packetLength, C_DynAdd.MAX_BYTES_OF_PACKET
                                            ));
                                        receivingPacketBytes.RemoveAt(0); // remove the first byte as it disables the PACKETSTART detection with positioning the id byte as this
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
                                        receivingPacketBytes.RemoveAt(0); // remove the first byte as it disables the PACKETSTART detection with positioning the id byte as this
                                        INCOMING_PACKET = false;
                                        continue;
                                    }
                                    // LENGTH_BYTE = N*[Params] + 1*[LEN] + 1*[INS/ERROR]
                                    // packetLength = [LENGTH_BYTE] + 1*[ID] + 1*[CheckSum] + 2*[PacketStart]
                                    //packetLength = (int)receivedByte + 4;
                                }
                                packetLength = GET_packetLength(receivingPacketBytes);
                                if (receivingPacketBytes.Count >= packetLength)
                                {
                                    // the last added byte to packetBytes was the most likely last byte of this package
                                    INCOMING_PACKET = false;
                                    LOG_cmd(receivingPacketBytes.ToArray(), e_cmd.received);
                                    LOG_unimportant("end of packet");

                                    // chose lastSent by receivedPacketBytes idByte
                                    List<byte> statusBytes = new List<byte>(receivingPacketBytes);
                                    receivingPacketBytes.Clear();
                                    LOG_debug("Sent packetBytes to process and 'cleared from readBuffer' :" +
                                        C_CONV.byteArray2strHex_space(statusBytes.ToArray()));
                                    // process it
                                    PROCESS_receivedPacket(statusBytes);
                                }
                                if (receivingPacketBytes.Count > packetLength)
                                {
                                    // trim the packet
                                    LOG_err(String.Format(
                                        "Packet too long, there were more bytes read from packet than the length byte proposes: "+
                                        "{0} from {1}! LENBYTE={2}! \nIt will by cutted! : {3}",
                                        receivingPacketBytes.Count, packetLength, 
                                        receivingPacketBytes[C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET],
                                        C_CONV.byteArray2strHex_space(receivingPacketBytes.ToArray())
                                        ));
                                    receivingPacketBytes = receivingPacketBytes.GetRange(0, GET_packetLength(receivingPacketBytes));
                                }
                            }
                        }
                        if (C_State.prog == e_stateProg.closing)
                        {
                            return;
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    LOG_err("Exception in packet read algorithm :" + GET_exInfo(ex));
                    //FLUSH_forNextIncomingPackage("exception");
                }
                // try if there is not a full packet in the packetBytes
                if (receivingPacketBytes.Count == GET_packetLength(receivingPacketBytes))
                {
                    if (PACKETSTART_detector(receivingPacketBytes) == 0)
                    {
                        PROCESS_receivedPacket(receivingPacketBytes);
                        receivingPacketBytes.Clear();
                    }
                }
            }

        }

        public static bool PROCESS_receivedPacket(List<byte> receivedBytes)
        {
            // we have received one whole packet
            /*
                -> is Consistent ?
                -> pair with best from lastSent queues
                -> PROCESS_statusPacket(received, paired)
                */
            try 
            {
                C_Packet received = new C_Packet(receivedBytes); // constructor checks consistency
                received.statusReceivedTime = DateTime.Now;
                return PAIR_andProcessStatusPacket(received);
            }
            catch (Exception e)
            {
                LOG_debug(string.Format(
                    "Will not process received packet because of its consistency error: {0} Exception:{1}",
                    C_CONV.byteArray2strHex_space(receivedBytes), e.Message
                    ));
                return false;
            }
        }
        public static bool PAIR_andProcessStatusPacket(C_Packet received)
        {
            //LOG_debug("index = " + index.ToString());
            C_Packet pairedLastSent = new C_Packet();
            bool foundBestPair = FIND_bestPairInLastSentQueues(received, ref pairedLastSent);
            // acquire the unprocessed lastSent package from queue if needed

            if (foundBestPair == true)
            {
                LOG_debug(string.Format(
                    "Paired status package: {0}\n with this sentPackage: {1}",
                    received.PacketBytes_toString,
                    pairedLastSent.PacketBytes_toString
                    ));

                LOG_debug("now processing packet");

                // process paired lastSent and this statusPacket
                try
                {
                    C_Packet.PROCESS_statusPacket(received, pairedLastSent);
                }
                catch (Exception ex)
                {
                    LOG_err("Exception in processing received packet :"+GET_exInfo(ex));
                }
            }

            return false;
        }

        public static bool FIND_bestPairInLastSentQueues(C_Packet received, ref C_Packet paired)
        {
            // return through ref the best pairedPacket
            e_rot rot;
            bool foundMotor = C_MotorControl.GET_motorRotFromId(received.ByteId, out rot);
            int rotMot = (int)rot;

            if (foundMotor == true)
            {
                lock (queueSent_locker)
                {
                    if (queueSent[rotMot].Count > 0)
                    {
                        List<C_Packet> listSent = (queueSent[rotMot]).ToList();
                        bool foundBestPair = FIND_bestPairInQueue(received, ref paired, ref listSent);
                        queueSent[rotMot] = new Queue<C_Packet>(listSent);
                        queueSent_Count[rotMot] = listSent.Count;
                        C_MotorControl.ACTUALIZE_queueCounts(queueSent);
                        return foundBestPair;
                    }   
                    else
                    {
                        LOG_debug(string.Format("lastSent queue of this motor[{0}] was empty!", rot));
                        return false;
                    }
                }
            }
            else
            {
                LOG_debug("Did not found any motor connected with this StatusPacket id : "
                    + received.ByteId.ToString());
            }
            return false;
        }

        public static bool FIND_bestPairInQueue(C_Packet received, ref C_Packet pairedPacket, ref List<C_Packet> listLastSent)
        {
            // go through whole queue
            // find the best suitable pair for this packet in this queue: (id, read bytes wanted, timeSpan freshness)
            // remove it from his queue and return it

            // if you found echo -> remove it from queue, log it, and continue with search

            List<int> suitableIndexes = new List<int>();
            List<TimeSpan> age = new List<TimeSpan>();
            
            for(int q = 0; q < listLastSent.Count(); q++)
            {
                if (received == listLastSent[q]) // echo - possibly from uart2usb converter
                {
                    C_Packet.LOG_statusPacket(string.Format(
                        "Processed echo of : [{0}] which was sent at [{1}]",
                        C_Packet.GET_packetInfo(listLastSent[q]),
                        listLastSent[q].sentTime.ToString("HH:mm:ss.fff")
                        ));
                    if (C_Packet.IS_statusPacketFollowing(listLastSent[q]) == false)
                    {
                        // if there is not supposed to be more packets comming (not mentioning this echo)
                        // for this lastSent packet - remove it from the listLastSent 
                        // (why it was there in the first place then?) - to be removed here
                        listLastSent.RemoveAt(q);
                    }
                    return false; // found that this is echo of last sent - best pair, 
                    // but pairing is just deleting from listLastSent! - for now
                }
                if (listLastSent[q].IS_fresh(received.statusReceivedTime) == true) // suitability
                {
                    if (received.IS_answerOf(listLastSent[q]) == true)
                    {
                        // this is probably the one as the not fresh ones are already dead
                        suitableIndexes.Add(q);
                        age.Add(listLastSent[q].GET_freshness(received.statusReceivedTime));
                    }
                    else
                    {
                        // just not suitable for this status, but maybe will be for another
                    }
                }
                else
                {
                    listLastSent.RemoveAt(q); // it is too old - remove it
                    q--; // and analyse new lastSent on this index 
                    continue;
                    // possibly kills correct ones (if they are too old)
                }   
            }

            // find the freshest from the suitable
            if( age.Count > 0)
            {
                // from the suitable ones get the most fresh one -
                // - return it as through ref as Paired and leave the other one in the listSent (which is also ref)
                int minimumValueIndex = age.IndexOf(age.Min());
                pairedPacket = listLastSent[minimumValueIndex];
                listLastSent.RemoveAt(minimumValueIndex);

                if (age.Count > 1)
                {
                    LOG_debug(string.Format(
                        "[{0}] packets suitable pairs in the motor.listLastSent for this status, but only one was selceted. (age={1}ms)",
                                  suitableIndexes.Count - 1, age[minimumValueIndex]
                                ));
                }
                return true;
            }
            else
            {
                    return false;
            }
        }

    }
}
