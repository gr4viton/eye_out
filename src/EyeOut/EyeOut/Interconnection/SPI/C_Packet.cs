using System;
using System.Collections.Generic;
using System.Linq; // SequenceEqual
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; // description

namespace EyeOut
{
    //public enum e_statusType
    //{
    //    noReturn, 
    //    presentPosition, 
    //    presentSpeed //, presentLoad , ...
    //}

    //// decide if the sent command should produce returning echo message
    //public enum e_packetEcho
    //{
    //    noEcho = 0, echoLast = 1
    //}

    public enum e_packetType
    {
        echoOfInstructionPacket, statusPacket, instructionPacket
    }


    //// queue of last sent packets to one motor 
    //public class C_queueOfLastSentPacketToMotor
    //{
    //    private Queue<C_Packet> queue;
    //    public Queue<C_Packet> Queue
    //    {

    //    }

    //    public C_queueOfLastSentPacketToMotor()
    //    {
    //        queue_lock = new object();
    //        queue = new Queue<C_Packet>();
    //    }
    //}
    
    //public class C_InstructionPacket : C_Packet
    //{
    //    // new for hiding inherited acceptance
    //    /*
    //    new public const int PacketLengthAddition = C_DynAdd.SIZEOF_PACKETSTART + C_DynAdd.SIZEOF_ID +
    //         C_DynAdd.SIZEOF_LENGTH + C_DynAdd.SIZEOF_INSTRUCTION + C_DynAdd.SIZEOF_CHECKSUM;

    //    new public const int IndexOfId = C_DynAdd.INDEXOF_ID_IN_INSTRUCTIONPACKET;
    //    new public const int IndexOfLength = C_DynAdd.INDEXOF_LENGTH_IN_INSTRUCTIONPACKET;
    //    new public const int IndexOfInstructionOrError = C_DynAdd.INDEXOF_INSTRUCTION_IN_INSTRUCTIONPACKET;
    //    new public const int IndexOfFirstParam = C_DynAdd.INDEXOF_FIRSTPARAM_IN_INSTRUCTIONPACKET;
    //    */
    //    public C_InstructionPacket(byte[] receivedBytes) : base(receivedBytes) { }
    //    public C_InstructionPacket(List<byte> lsReceivedBytes) : base(lsReceivedBytes) { }
    //}

    //public class C_StatusPacket : C_Packet
    //{
    //    // it still does not see them
    //    public override int PacketLengthAddition
    //    {
    //        get
    //        {
    //            return C_DynAdd.SIZEOF_PACKETSTART + C_DynAdd.SIZEOF_ID +
    //                C_DynAdd.SIZEOF_LENGTH + C_DynAdd.SIZEOF_ERROR + C_DynAdd.SIZEOF_CHECKSUM ;
    //        }
    //    }

    //    public override int IndexOfId 
    //    { 
    //        get { return C_DynAdd.INDEXOF_ID_IN_STATUSPACKET; } 
    //    }
    //    public override int IndexOfLength 
    //    { 
    //        get { return C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET; } 
    //    }
    //    public override int IndexOfInstructionOrError 
    //    { 
    //        get { return C_DynAdd.INDEXOF_ID_IN_STATUSPACKET;}
    //    }
    //    public override int IndexOfFirstParam 
    //    { 
    //        get { return C_DynAdd.INDEXOF_FIRSTPARAM_IN_STATUSPACKET;}
    //    }

    //    public C_StatusPacket(byte[] receivedBytes)
    //        : base(receivedBytes) 
    //    {
    //    }
    //    public C_StatusPacket(List<byte> lsReceivedBytes) : base(lsReceivedBytes) 
    //    { 
            
    //    }


    //}


    //public abstract class C_Packet
    public partial class C_Packet
    {
        public e_rot rotMotor;
        protected byte byteId;
        protected byte byteLength; //  The length is calculated as “the number of Parameters (N) + 2”
        protected byte byteInstructionOrError; // instruction
        protected byte byteCheckSum;

        protected List<byte> par; // parameters

        public bool IsConsistent = true;
        protected int packetNumOfBytes;
        protected const int maxParameters = C_DynAdd.MAX_PARAMETERS;

        protected e_packetType packetType = e_packetType.instructionPacket;

        protected e_statusReturnLevel returnStatusLevel = e_statusReturnLevel.never;
        protected e_motorDataType motorDataType = e_motorDataType.angleSeen;

        public DateTime sentTime;
        public DateTime statusReceivedTime;

        // how long can the instructionPacket be waiting for its return packet
        //public TimeSpan allowedReturnTimeDelay = new TimeSpan(0, 0, 1, 0, 30); // ms
        public TimeSpan allowedReturnTimeDelay = new TimeSpan(0, 0, 0, 0, 30); // ms
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region operators
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static bool operator ==(C_Packet x, C_Packet y) 
        {
            return (x.PacketBytes.SequenceEqual(y.PacketBytes));
                //&&
                //(x.byteId == y.byteId)
                //&&
                //(x.byteInstructionOrError == y.byteInstructionOrError)
                //&&
                //(x.byteLength = y.byteLength)
                //&& 
                //(x.byteCheckSum
        }

        public static bool operator !=(C_Packet x, C_Packet y)
        {
            return !(x.PacketBytes.SequenceEqual(y.PacketBytes));
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion operators
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // ____________________________________________________ BYTES
        public byte ByteId
        {
            get { return byteId; }
            set { byteId = value; }
        }
        public byte ByteLength
        {
            get { return byteLength; }
        }
        public byte ByteInstructionOrError
        {
            get { return byteInstructionOrError; }
            set { byteInstructionOrError = value; }

        }
        public byte ByteCheckSum
        {
            get { return byteCheckSum; }
        }
        // ____________________________________________________ PARAMETERS
        public List<byte> Par
        {
            get { return par; }
            set
            {
                if (value.Count <= maxParameters)
                {
                    par = value;
                    REFRESH_length();
                    REFRESH_checkSum();
                }
                else
                {
                    C_Packet.LOG_err(this,
                        string.Format(
                        "Inner packet is longer than maximal value. [{0}] < [{1}]",
                        value.Count, maxParameters
                        ));
                }
            }
        }
        
        public List<Object> Par_obj
        {
            set
            {
                Par = C_CONV.listOfObjects2listOfBytes(value);
            }
        }

        public e_packetType PacketType
        {
            get { return packetType; }
        }

        // ____________________________________________________ PACKET BYTES
        public List<byte> lsPacketBytes // returns whole cmd
        {
            get
            {
                return CREATE_instructionPacket_list();
            }
            //set
            //{
            //    PacketBytes = value.ToArray();
            //}
        }
        public byte[] PacketBytes // returns whole packet with all parts (including PACKETSTART bytes)
        {
            get
            {
                return CREATE_instructionPacket_bytes();
            }
            /*             
            set
            {
                C_Packet pack;
                try
                {
                    // parse byte array into individual parameters
                    pack = PARSE_instructionPacket_byte(value);
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        C_SPI.LOG_ex(ex);
                    }
                    return;
                }
                
                SET_this(pack)
                this = new C_Packet(pack); // how to asign
            }
            */
        }

        public byte[] PacketBytes_forChecksum // only those parts for counting checksum
        {
            get
            {
                return CREATE_instructionPacket_bytes(true);
            }
        }

        public string PacketBytes_toString
        {
            get
            {
                return C_CONV.byteArray2strHex_space(PacketBytes);
            }
            //set
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region virtual properties 
        // gets overriden in subclasses
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public virtual int PacketLengthAddition
        {
            get
            {
                return C_DynAdd.SIZEOF_PACKETSTART + C_DynAdd.SIZEOF_ID +
                    C_DynAdd.SIZEOF_LENGTH + C_DynAdd.SIZEOF_ERROR + C_DynAdd.SIZEOF_CHECKSUM;
            }
        }

        public virtual int IndexOfId
        {
            get { return C_DynAdd.INDEXOF_ID_IN_STATUSPACKET; }
        }
        public virtual int IndexOfLength
        {
            get { return C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET; }
        }
        public virtual int IndexOfInstructionOrError
        {
            get { return C_DynAdd.INDEXOF_ERROR_IN_STATUSPACKET; }
        }
        public virtual int IndexOfFirstParam
        {
            get { return C_DynAdd.INDEXOF_FIRSTPARAM_IN_STATUSPACKET; }
        }
        public virtual int IndexOfPacketStart
        {
            get { return C_DynAdd.INDEXOF_PACKETSTART_IN_STATUSPACKET; }
        }
        

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion virtual properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region constructor
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_Packet()
        {
            byteId = 0;
            par = new List<byte>();
        }

        public C_Packet(C_Packet packet)
        {
            byteId = packet.byteId;
            byteInstructionOrError = packet.ByteInstructionOrError;
            byteLength = packet.byteLength;
            byteCheckSum = packet.byteCheckSum;
            Par = packet.Par;
            returnStatusLevel = packet.returnStatusLevel;
            motorDataType = packet.motorDataType;
        }

        public C_Packet(byte[] receivedBytes)
        {

            //statusType = e_statusType.noReturn;
            packetType = e_packetType.instructionPacket;

            int IndexOfCheckSum = receivedBytes.Length - 1;

            byteId = receivedBytes[IndexOfId];
            byteInstructionOrError = receivedBytes[IndexOfInstructionOrError];
            // fill in params (from const to length (checksum))
            List<byte> _par = new List<byte>();
            for (int q = IndexOfFirstParam; q < IndexOfCheckSum; q++)
            {
                _par.Add(receivedBytes[q]);
            }
            Par = _par;
            //Par = _par;

            //    RESET_from_packetBytes(receivedBytes);
            IS_consistent();

            if (byteLength != receivedBytes[IndexOfLength])
            {
                IsConsistent = false;
                // bad - but should never happen 
                // as the length byte directly creates the length of the byte array 
                // in the serial read function
                throw new Exception(GET_ByteFailInfo("LENGTH_BYTE", byteLength, receivedBytes[IndexOfLength]));
            }

            if (ByteCheckSum != receivedBytes[IndexOfCheckSum])
            {
                IsConsistent = false;
                throw new Exception(GET_ByteFailInfo("CHECKSUM_BYTE", ByteCheckSum, receivedBytes[IndexOfCheckSum]));
            }
        }
        
        public C_Packet(List<byte> lsReceivedBytes): this(lsReceivedBytes.ToArray())
        {
        }

        //public C_Packet(C_Packet pack):
        //{
        //    IdByte = pack.idByte;
        //    lengthByte = pack.lengthByte;
        //}

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion constructor
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region REFRESH
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void REFRESH_checkSum()
        {
            // count checksum from whole cmd
            byteCheckSum = C_CheckSum.GET_checkSum_fromWholePacket(PacketBytes);
        }
        public void REFRESH_length()
        {
            int parCount = par.Count;
            byteLength = (byte)(par.Count + 2); // as defined
            packetNumOfBytes = parCount + PacketLengthAddition;
        }
        
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion REFRESH
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region IS consistent
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public bool IS_consistent()
        {
            // called after the Packet is filled from serial port
            // make sure that all of these are correct
            // lengthByteReceived, checkSumByteReceived

            // just try to create it from bytes and than find out if its equal
            //return true;
            return (IS_consistentLengthByte() 
                 && IS_consistentCheckSumByte());
        }

        public bool IS_consistentLengthByte()
        {
            if (byteLength == Par.Count+2)
                return true;
            else
                return false;
        }

        public bool IS_consistentCheckSumByte()
        {
            if (C_CheckSum.GET_checkSum_fromWholePacket(PacketBytes) == 0)
                return true;
            else
                return false;
        }

        public bool IS_fresh(DateTime receivedTime)
        {
            if ((receivedTime - sentTime) > allowedReturnTimeDelay)
                return false;
            else
                return true;
        }
        public TimeSpan GET_freshness(DateTime receivedTime)
        {
            return receivedTime - sentTime;
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion IS consistent
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region CREATE
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        
        public List<byte> CREATE_instructionPacket_list()
        {
            List<byte> lsBy = new List<byte>();
            lsBy.AddRange(CREATE_instructionPacket_bytes()); 
            return lsBy;
        }

        
        public byte[] CREATE_instructionPacket_bytes()
        {
            return CREATE_instructionPacket_bytes(false);
        }

        public byte[] CREATE_instructionPacket_bytes(bool forChecksum)
        {
            // Instruction Packet = from pc to servo
            // OXFF | 0XFF | ID | LENGTH | INSTRUCTION | PARAMETER_1 | … | PARAMETER_N | CHECK_SUM 

            // this function adds first two startBytes [0xFF,0xFF], its id byte, length byte, instruction byte and Checksum byte 
            // around parameters and returns the byte array of it

            //packetNumOfBytes = par.Count + 4;
            byte[] _packetBytes = new byte[packetNumOfBytes];

            //List<byte> _lsPacketBytes = new List<byte>(5);
            int q = 0;
            // id
            _packetBytes[IndexOfId] = byteId;
            // length byte
            _packetBytes[IndexOfLength] = byteLength;
            // instruction
            _packetBytes[IndexOfInstructionOrError] = byteInstructionOrError;

            // parameters
            q = IndexOfFirstParam;
            foreach (byte by in par)
            {
                _packetBytes[q] = by;
                q++;
            }
            // checksum - counted without PACKETSTART
            _packetBytes[q] = C_CheckSum.GET_checkSum_fromDataBytes(_packetBytes);

            // packet start
            for (q = IndexOfPacketStart; q < C_DynAdd.SIZEOF_PACKETSTART; q++)
            {
                _packetBytes[q] = C_DynAdd.PACKETSTART[q];
            }

            if (forChecksum == true)
            {

                int from = C_DynAdd.INDEXOF_ID_IN_STATUSPACKET;
                int count = packetNumOfBytes - from - C_DynAdd.SIZEOF_CHECKSUM;
                
                return (new ArraySegment<byte>(_packetBytes, from, count)).ToArray();
            }
            else
            {
                return _packetBytes;
            }
        }
        
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion CREATE
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region RESET
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public string GET_ByteFailInfo(string byteName, byte csCounted, byte csReceived)
        {
            return String.Format(
                    "The {2} counted from PACKET bytes =[{0}={0:X}] is different from the value of {2} =[{1}={1:X}] received in the PACKET.",
                    csCounted, csReceived, byteName
                    );
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion RESET
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        
        public static void SEND_packet(C_Packet packet)
        {
            C_SPI.SEND_data(packet);
        }

        // Optional parameters - i.e. INS_ACTION don't need any parameters
        public C_Packet(C_Motor _mot, byte _instructionByte, List<object> _lsParameters)
        {
            byteId = _mot.id;
            rotMotor = _mot.rotMotor;
            returnStatusLevel = _mot.StatusReturnLevel;
            byteInstructionOrError = _instructionByte;
            Par_obj = _lsParameters;
        }

        public C_Packet(C_Motor _mot, byte _instructionByte, List<byte> _lsParameters = null)
        {
            byteId = _mot.id;
            rotMotor = _mot.rotMotor;
            returnStatusLevel = _mot.StatusReturnLevel;
            byteInstructionOrError = _instructionByte;
            Par = _lsParameters;
        }

        public C_Packet(byte _instructionByte, List<object> _lsParameters)
        {
            byteId = C_DynAdd.ID_BROADCAST;
            byteInstructionOrError = _instructionByte;
            Par_obj = _lsParameters;
        }

        public C_Packet(byte _instructionByte, List<byte> _lsParameters = null)
        {
            byteId = C_DynAdd.ID_BROADCAST;
            byteInstructionOrError = _instructionByte;
            Par = _lsParameters;
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public bool IS_answerOf(C_Packet lastSent)
        {
            // checks if this packet is possible status message of lastSent packet
            // byteId
            // length of bytes wanted 
            if (lastSent.ByteId == ByteId)
            {
                if (lastSent.ByteInstructionOrError == C_DynAdd.INS_READ)
                {
                    int numOfWantedParams = lastSent.Par[1];
                    if (Par.Count == numOfWantedParams)
                    {
                        // this is probably the one 
                        return true;
                    }
                    else
                    {
                        return false; // not suitable number of parameters
                    }
                }
                else
                {
                    return false; // last packet wasn't a read packet
                }
            }
            else
            {
                return false;
            }
        }

        public bool IS_error() 
        {
            // checks error byte
            if (PacketBytes[C_DynAdd.INDEXOF_ERROR_IN_STATUSPACKET] != 0)
            {
                // it is error - log error and dont process anything else as it won't come
                C_Packet.LOG_errorByte(this);
                return true;
            }
            return false;
        }
        
        public static bool IS_statusPacketFollowing(C_Packet lastSent)
        {
            if(
                (lastSent.byteId == C_DynAdd.ID_BROADCAST) // never no matter of returnStatusLevel
                ||
                (lastSent.returnStatusLevel == e_statusReturnLevel.never) 
                )
            {
                return false;
            }
            else if (
                (lastSent.byteInstructionOrError == C_DynAdd.INS_PING)
                ||
                (lastSent.returnStatusLevel == e_statusReturnLevel.allways) // always no matter of returnStatusLevel
                )
            {
                return true;
            }
            else 
            {
                // (lastSent.returnStatusLevel == e_returnStatusLevel.onRead)
                if(lastSent.byteInstructionOrError == C_DynAdd.INS_READ)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool PROCESS_statusPacket(C_Packet status, C_Packet pairedLastSent)
        {
            // no echo should get here (FIND_bestPairInQueue) sort it out
            if (status.IS_error() == true) // status is consistent and correct, but may have error
            {
                C_SPI.LOG_debug("The processed package has an error! : "
                    + status.PacketBytes_toString);
            }
            else
            {
                C_SPI.LOG_debug("The processed package does not contain any error, going to process statusPacket");
                ACTUALIZE_motorRegistersFromStatusPacket(status, pairedLastSent);
                C_SPI.LOG_debug("Status packet processing ended");
            }
            return true;
        }

        public static void PROCESS_instructionPacket(C_Packet lastSent)
        {
            if (IS_packetChangingValue(lastSent) == true)
            {
                if (lastSent.byteId != C_DynAdd.ID_BROADCAST)
                {
                    C_MotorControl.ACTUALIZE_motorRegister(
                        lastSent.rotMotor,
                        e_regByteType.sentValue, // as we written them just now
                        lastSent.Par);
                    LOG_instruPacket(string.Format(
                        "Written OK (blind) - actualizing motor [{1}] register sentValue: \t{0}",
                        lastSent.PacketBytes_toString, lastSent.rotMotor));
                }
                else
                {
                    // parse it and process each part alone
                    //LOG_instruPacket(string.Format(
                    //    "Written OK (blind) - actualizing motors (broadcast) register sentValue: \t{0}",
                    //    lastSent.PacketBytes_toString));
                    LOG_instruPacket(string.Format(
                        "Written OK (blind) - NOT actualizing motors (broadcast) register sentValue: \t{0}",
                        lastSent.PacketBytes_toString));
                }
            }
        }
        public static bool IS_packetChangingValue(C_Packet packet)
        {
            switch (packet.byteInstructionOrError)
            {
                case (C_DynAdd.INS_ACTION): return true; // i think so - at least registered byte
                case (C_DynAdd.INS_REG_WRITE): return true;
                case (C_DynAdd.INS_SYNC_WRITE): return true;
                case (C_DynAdd.INS_PING): return true;
                case (C_DynAdd.INS_WRITE): return true;
                case (C_DynAdd.INS_READ): return false;
            }
            return false;
            //if(packet.byteId == C_DynAdd.ID_BROADCAST)
        }
        public static void ACTUALIZE_motorRegistersFromStatusPacket(C_Packet received, C_Packet pairedLastSent)
        {
            try
            {
                if (received.byteId == pairedLastSent.byteId)
                {
                    // we have no error in statusPacket
                    if (pairedLastSent.byteInstructionOrError == C_DynAdd.INS_WRITE)
                    {
                        // actualize the parameters which were written into motors - and we know they were written good
                        C_MotorControl.ACTUALIZE_motorRegister(
                            pairedLastSent.rotMotor,
                            e_regByteType.seenValue, // as we received statusMessage after we written the value
                            pairedLastSent.Par);
                    }
                    else if (pairedLastSent.byteInstructionOrError == C_DynAdd.INS_READ)
                    {
                        // actualize the parameters which were read from motors
                        
                        C_MotorControl.ACTUALIZE_motorRegister(
                            pairedLastSent.rotMotor,
                            e_regByteType.seenValue,
                            C_CONV.listOfObjects2listOfBytes(new List<object>(){
                                pairedLastSent.Par[0], received.Par })
                            );
                    }
                    else
                    {
                        C_SPI.LOG_debug(string.Format(
                            "lastSent packet: {0}\nPaired with: {1}\n{2}",
                            pairedLastSent.PacketBytes_toString,
                            received.PacketBytes_toString,
                            "wasn't read neither write and yet still it was supposed to be processed as a statusPacket. Not processing!"
                            ));                            
                    }

                }
                else
                {
                    LOG_statusPacket(string.Format(
                        "The received status packet :\t{0}\nDoes not belong to the lastSent: \t{1}",
                            received.PacketBytes_toString, pairedLastSent.PacketBytes_toString
                            ));
                }
            }
            catch (Exception e)
            {
                C_SPI.LOG_debug("tak tady to padá! " + e.Message);
            }
        }
        


    }
}
