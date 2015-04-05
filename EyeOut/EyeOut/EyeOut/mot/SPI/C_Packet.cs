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

    public enum e_returnStatusLevel // address 16
    {
        [Description("0 No return against all instructions")] never = 0,
        [Description("1 Retrun only for the READ_DATA command")] onRead = 1,
        [Description("2 Return for all Instructions")] allways = 2,
    }

    public class C_InstructionPacket : C_Packet
    {
        // new for hiding inherited acceptance
        /*
        new public const int PacketLengthAddition = C_DynAdd.SIZEOF_PACKETSTART + C_DynAdd.SIZEOF_ID +
             C_DynAdd.SIZEOF_LENGTH + C_DynAdd.SIZEOF_INSTRUCTION + C_DynAdd.SIZEOF_CHECKSUM;

        new public const int IndexOfId = C_DynAdd.INDEXOF_ID_IN_INSTRUCTIONPACKET;
        new public const int IndexOfLength = C_DynAdd.INDEXOF_LENGTH_IN_INSTRUCTIONPACKET;
        new public const int IndexOfInstructionOrError = C_DynAdd.INDEXOF_INSTRUCTION_IN_INSTRUCTIONPACKET;
        new public const int IndexOfFirstParam = C_DynAdd.INDEXOF_FIRSTPARAM_IN_INSTRUCTIONPACKET;
        */
        public C_InstructionPacket(byte[] receivedBytes) : base(receivedBytes) { }
        public C_InstructionPacket(List<byte> lsReceivedBytes) : base(lsReceivedBytes) { }
    }

    public class C_StatusPacket : C_Packet
    {
        // it still does not see them
        public override int PacketLengthAddition
        {
            get
            {
                return C_DynAdd.SIZEOF_PACKETSTART + C_DynAdd.SIZEOF_ID +
                    C_DynAdd.SIZEOF_LENGTH + C_DynAdd.SIZEOF_ERROR + C_DynAdd.SIZEOF_CHECKSUM ;
            }
        }

        public override int IndexOfId 
        { 
            get { return C_DynAdd.INDEXOF_ID_IN_STATUSPACKET; } 
        }
        public override int IndexOfLength 
        { 
            get { return C_DynAdd.INDEXOF_LENGTH_IN_STATUSPACKET; } 
        }
        public override int IndexOfInstructionOrError 
        { 
            get { return C_DynAdd.INDEXOF_ID_IN_STATUSPACKET;}
        }
        public override int IndexOfFirstParam 
        { 
            get { return C_DynAdd.INDEXOF_FIRSTPARAM_IN_STATUSPACKET;}
        }

        public C_StatusPacket(byte[] receivedBytes)
            : base(receivedBytes) 
        {
        }
        public C_StatusPacket(List<byte> lsReceivedBytes) : base(lsReceivedBytes) 
        { 
            
        }


    }


    //public abstract class C_Packet
    public partial class C_Packet
    {
        public e_rot rotMotor;
        protected byte idByte;
        protected byte lengthByte; //  The length is calculated as “the number of Parameters (N) + 2”
        protected e_returnStatusLevel returnStatusLevel = e_returnStatusLevel.never;
            
        //protected e_packetEcho packetEcho = e_packetEcho.echoLast;
        protected e_motorDataType motorDataType = e_motorDataType.anglePresent;
        protected e_packetType packetType = e_packetType.instructionPacket;
        
        protected byte instructionOrErrorByte; // instruction
        protected List<byte> par; // parameters
        protected byte checkSumByte;

        protected int packetNumOfBytes;

        protected const int maxParameters = C_DynAdd.MAX_PARAMETERS;

        public bool IsConsistent=true;

        public static bool operator ==(C_Packet x, C_Packet y) 
        {
            return x.PacketBytes.SequenceEqual(y.PacketBytes);
        }

        public static bool operator !=(C_Packet x, C_Packet y)
        {
            return !(x.PacketBytes.SequenceEqual(y.PacketBytes));
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public byte IdByte
        {
            get { return idByte; }
            set { idByte = value; }
        }

        public e_packetType PacketType
        {
            get { return packetType; }
        }
        //public e_statusType StatusType
        //{
        //    get { return motorDataType; }
        //}
        public byte CheckSumByte
        {
            get { return checkSumByte; }
        }

        public List<Byte> Par
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
                Par = C_CONV.listOfByteAndByteArrays2listOfbytes(value);
            }
        }
        
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
            idByte = 0;
            par = new List<byte>();
        }
        

        public C_Packet(byte[] receivedBytes)
        {
            idByte = 0;
            //statusType = e_statusType.noReturn;
            packetType = e_packetType.instructionPacket;
            par = new List<byte>();

            try
            {
                RESET_from_packetBytes(receivedBytes);
            }
            catch(Exception e)
            {
                C_Packet.LOG_ex(this, e);
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
            checkSumByte = C_CheckSum.GET_checkSum(PacketBytes_forChecksum);
        }
        public void REFRESH_length()
        {
            int parCount = par.Count;
            lengthByte = (byte)(par.Count + 2); // as defined
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
            return (IS_consistentLengthByteReceived() 
                 && IS_consistentCheckSumByteReceived());
        }

        public bool IS_consistentLengthByteReceived()
        {
            // make sure that all of these are correct
            // checkSumByteReceived
            return true;
        }

        public bool IS_consistentCheckSumByteReceived()
        {
            // make sure that all of these are correct
            // lengthByteReceived
            return true;
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

        
        public Byte[] CREATE_instructionPacket_bytes()
        {
            return CREATE_instructionPacket_bytes(false);
        }

        public Byte[] CREATE_instructionPacket_bytes(bool forChecksum)
        {
            // Instruction Packet = from pc to servo
            // OXFF | 0XFF | ID | LENGTH | INSTRUCTION | PARAMETER_1 | … | PARAMETER_N | CHECK_SUM 

            // this function adds first two startBytes [0xFF,0xFF], its id Byte, length Byte, instruction Byte and Checksum Byte 
            // around parameters and returns the byte array of it

            byte[] _packetBytes = new byte[packetNumOfBytes];

            int q = 0;
            // id
            _packetBytes[IndexOfId] = idByte;
            // length byte
            _packetBytes[IndexOfLength] = lengthByte;
            // instruction
            _packetBytes[IndexOfInstructionOrError] = instructionOrErrorByte;

            // parameters
            q = IndexOfFirstParam;
            foreach (Byte by in par)
            {
                _packetBytes[q] = by;
                q++;
            }
            // checksum - counted without PACKETSTART
            _packetBytes[q] = C_CheckSum.GET_checkSum(_packetBytes);

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
        public void RESET_from_lsPack(List<byte> _lsPack)
        {
            RESET_from_packetBytes(_lsPack.ToArray());
        }
    
        public void RESET_from_packetBytes(byte[] _packetBytes)
        {
            // fill in the new instructionPacket:
            //      id | instruction/error | params  | 
            // -> it'll compute automatically: 
            //      LengthByte, ChecksumByte
            // get PacketBytes 
            // compare:
            //      LengthByte ?= LengthByteReceived
            //      ChecksumByte ?= ChecksumByteReceived
            // if anything goes wrong throw an exception

            int IndexOfCheckSum = _packetBytes.Length - 1;

            // fill in instructionByte
            instructionOrErrorByte = _packetBytes[IndexOfInstructionOrError];
            // fill in id
            idByte = _packetBytes[IndexOfId];
            // fill in params (from const to length (checksum))
            List<byte> _par = new List<byte>();
            for (int q = IndexOfFirstParam; q < IndexOfCheckSum; q++)
            {
                _par.Add(_packetBytes[q]);
            }
            Par = _par;
            
            if(lengthByte != _packetBytes[IndexOfLength])
            {
                IsConsistent = false;
                // bad - but should never happen 
                // as the length byte directly creates the length of the byte array 
                // in the serial read function
                throw new Exception( String.Format(
                    "The LENGTH_BYTE counted from PACKET bytes =[{0}] is different from the value of LENGTH_BYTE =[{1}] received in the PACKET.",
                    lengthByte, _packetBytes[IndexOfLength]
                    ));
            }
            
            if(CheckSumByte != _packetBytes[IndexOfCheckSum])
            {
                IsConsistent = false;
                throw new Exception( String.Format(
                    "The CHECKSUM_BYTE counted from PACKET bytes =[{0}] is different from the value of CHECKSUM_BYTE =[{1}] received in the PACKET.",
                    CheckSumByte, _packetBytes[IndexOfCheckSum]
                    ));
            }
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion RESET
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        //public static Byte[] CREATE_cmdFromCmdInner(Byte[] byCmdin, Byte id)
        //{
        //    // Instruction Packet = from pc to servo
        //    // OXFF 0XFF ID LENGTH INSTRUCTION PARAMETER1 …PARAMETER N CHECK SUM 
        //    // inner contains only these bytes:
        //    // INSTRUCTION, PARAMETER_1, ..., PARAMETER_N

        //    // this function adds first two startBytes [0xFF,0xFF], its id Byte, length Byte and Checksum Byte

        //    // make it into ArrayList
        //    Byte[] cmd = new Byte[5 + byCmdin.Length];
        //    //ArrayList a_cmd = new ArrayList();
        //    //a_cmd.Add({0xFF, 0xFF})
        //    //{ 0xFF, 0xFF, id, len, inner, 0x00 };
        //    //{ 0   , 1   , 2 , 3  , 4...., last };
        //    cmd[2] = id;
        //    int q = 4;
        //    foreach (Byte by in byCmdin)
        //    {
        //        cmd[q] = by;
        //        q++;
        //    }
        //    cmd[3] = (Byte)(byCmdin.Length + 1); // = paramN+Instruction + 1 = paramN + 2 = len
        //    cmd[q] = C_CheckSum.GET_checkSum(cmd);
        //    cmd[0] = cmd[1] = 0xFF;
        //    return cmd;
        //}



        public static void SEND_packet(C_Packet packet)
        {
            C_SPI.SEND_data(packet);
        }

        

        // Optional parameters - i.e. INS_ACTION don't need any parameters
        public C_Packet(C_Motor _mot, byte _instructionByte, List<object> _lsParameters)
        {
            idByte = _mot.id;
            rotMotor = _mot.rotMotor;
            returnStatusLevel = _mot.ReturnStatusLevel;
            instructionOrErrorByte = _instructionByte;
            Par_obj = _lsParameters;
        }

        public C_Packet(C_Motor _mot, byte _instructionByte, List<byte> _lsParameters = null)
        {
            idByte = _mot.id;
            rotMotor = _mot.rotMotor;
            returnStatusLevel = _mot.ReturnStatusLevel;
            instructionOrErrorByte = _instructionByte;
            Par = _lsParameters;
        }

        public C_Packet(byte _instructionByte, List<object> _lsParameters)
        {
            idByte = C_DynAdd.ID_BROADCAST;
            instructionOrErrorByte = _instructionByte;
            Par_obj = _lsParameters;
        }

        public C_Packet(byte _instructionByte, List<byte> _lsParameters = null)
        {
            idByte = C_DynAdd.ID_BROADCAST;
            instructionOrErrorByte = _instructionByte;
            Par = _lsParameters;
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static bool IS_error(C_Packet received, List<byte> receivedBytes)
        {
            if (receivedBytes[C_DynAdd.INDEXOF_ERROR_IN_STATUSPACKET] != 0) // instead of echo ther may be error returned
            {
                // it is error - log error and dont process anything else as it won't come
                C_Packet.LOG_errorByte(received);
                return true;
            }
            return false;
        }
        
        public static bool IS_statusPacketFollowing(C_Packet lastSent)
        {
            if(
                (lastSent.idByte == C_DynAdd.ID_BROADCAST) // never no matter of returnStatusLevel
                ||
                (lastSent.returnStatusLevel == e_returnStatusLevel.never) 
                )
            {
                return false;
            }
            else if (
                (lastSent.instructionOrErrorByte == C_DynAdd.INS_PING)
                ||
                (lastSent.returnStatusLevel == e_returnStatusLevel.allways) // always no matter of returnStatusLevel
                )
            {
                return true;
            }
            else 
            {
                // (lastSent.returnStatusLevel == e_returnStatusLevel.onRead)
                if(lastSent.instructionOrErrorByte == C_DynAdd.INS_READ)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        // returns false when you dont have to continue
        public static void PROCESS_receivedPacket(C_Packet lastSent, List<byte> receivedBytes, int numPacket)
        {
            // we have received one whole packet
            C_Packet received = new C_StatusPacket(receivedBytes); // constructor throws error if incosistent

            if (numPacket == 0)
            { 
                // it is echo or error
                if (IS_error(received, receivedBytes) == true)
                {
                    return;
                }
                else
                {
                    PROCESS_statusPacket(received, lastSent);
                }
            }
        }

        public static void PROCESS_statusPacket(C_Packet received, C_Packet lastSent)
        {
            // we have no error in statusPacket
            if (lastSent.instructionOrErrorByte == C_DynAdd.INS_WRITE)
            {
                // probably just normal status packet with no parameters 
                //LOG_statusPacket(string.Format("Status OK after INS_WRITE:\t{0}", received.PacketBytes_toString));
                // actualize the parameters which were written into motors

                //switch (lastSent.statusType)
                //{

                //}
                //C_MotorControl.ACTUALIZE_motorStatus();
                //C_MotorControl.ACTUALIZE_motorStatus(lastSent.rotMotor, lastSent.motorDataType, lastSent.Par);
            }
            else
            {
                //C_MotorControl.ACTUALIZE_motorStatus(lastSent.rotMotor, lastSent.motorDataType, received.Par);
            }
            LOG_statusPacket(string.Format("Status OK:\t{0}", received.PacketBytes_toString));

        }

    }
}
