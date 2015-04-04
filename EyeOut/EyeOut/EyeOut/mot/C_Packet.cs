using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    // can be rewritten as 
    // public class C_InstructionPacket : C_Packet
    public class C_InstructionPacket
    {
        private byte idByte;
        private byte lengthByte; //  The length is calculated as “the number of Parameters (N) + 2”
        //private byte lengthByteReceived;
        
        private byte instructionByte; // instruction
        private List<byte> par; // parameters
        private byte checkSumByte;
        //private byte checkSumByteReceived;

        private int packetNumOfBytes;

        private const int maxParameters = C_DynAdd.MAX_PARAMETERS;

        private static const int PacketLengthAddition = C_DynAdd.SIZEOF_PACKETSTART + C_DynAdd.SIZEOF_ID +
             C_DynAdd.SIZEOF_LENGTH_BYTE + C_DynAdd.SIZEOF_INSTRUCTION + C_DynAdd.SIZEOF_CHECKSUM;
        
        private static const int IndexOfId = C_DynAdd.INDEXOF_ID_IN_INSTRUCTIONPACKET;
        private static const int IndexOfLength = C_DynAdd.INDEXOF_LENGTH_IN_INSTRUCTIONPACKET;
        private static const int IndexOfInstruction = C_DynAdd.INDEXOF_INSTRUCTION_IN_INSTRUCTIONPACKET;
        private static const int IndexOfFirstParam = C_DynAdd.INDEXOF_FIRSTPARAM_IN_INSTRUCTIONPACKET;

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public byte IdByte
        {
            get { return idByte; }
            set { idByte = value; }
        }
        public byte LengthByteReceived
        {
            get { return lengthByteReceived; }
            set { lengthByteReceived = value; }
        }
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
                    C_SPI.LOG_err(
                        string.Format(
                        "Inner packet is longer than maximal value. [{0}] < [{1}]",
                        value.Count, maxParameters
                        ));
                }
            }
        }
        
        public List<byte> lsPacketBytes // returns whole cmd
        {
            get
            {
                return CREATE_instructionPacket_list();
            }
        }
        public byte[] PacketBytes // returns whole cmd
        {
            get
            {
                return CREATE_instructionPacket_bytes();
            }
            set
            {
                C_InstructionPacket pack;
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
                
                this = new C_InstructionPacket(pack); // how to asign
            }
        }


        public void REFRESH_checkSum()
        {
            // count checksum from whole cmd
        }
        public void REFRESH_length()
        {
            int parCount = par.Count;
            lengthByte = (byte)(par.Count + 2); // as defined
            packetNumOfBytes = parCount + PacketLengthAddition;
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region constructor
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_InstructionPacket()
        {
            idByte = 0;
            par = new List<byte>();
        }
        public C_InstructionPacket(C_Motor mot, List<byte> _par)
        {
            idByte = mot.id;
            //mot.motorEcho

            Par = _par;
        }

        public C_InstructionPacket(C_Motor mot, List<object> _par)
        {
            idByte = mot.id;
            // like create_cmdFromInner -> possible byte arrays & bytes in list
            //par = _par;
        }
        
        public C_InstructionPacket(byte[] receivedBytes)
        {
            PacketBytes = receivedBytes;
            //this = C_InstructionPacket(PARSE_byteInstructionPakcet(receivedBytes)); // HOW TO 
        }
        
        public C_InstructionPacket(List<byte> receivedBytes)
        {
            PacketBytes = receivedBytes
            try
            {
                C_InstructionPacket pack = PARSE_instructionPacket_list(receivedBytes);
            }
            catch(
            //this = C_InstructionPacket(PARSE_byteInstructionPakcet(receivedBytes)); // HOW TO 
        }

        public C_InstructionPacket(C_InstructionPacket pack)
        {
            IdByte = pack.idByte;
            lengthByte = pack.lengthByte;
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion constructor
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

        
        public List<byte> CREATE_instructionPacket_list()
        {
            List<byte> lsBy = new List<byte>();
            lsBy.AddRange(CREATE_instructionPacket_bytes()); 
            return lsBy;
        }
        public Byte[] CREATE_instructionPacket_bytes()
        {
            // Instruction Packet = from pc to servo
            // OXFF | 0XFF | ID | LENGTH | INSTRUCTION | PARAMETER_1 | … | PARAMETER_N | CHECK_SUM 

            // this function adds first two startBytes [0xFF,0xFF], its id Byte, length Byte, instruction Byte and Checksum Byte 
            // around parameters and returns the byte array of it

            byte[] cmd = new byte[packetNumOfBytes];

            int q = 0;
            // packet start
            for (q = IndexOfFirstParam; q < C_DynAdd.SIZEOF_PACKETSTART; q++)
            {
                cmd[q] = C_DynAdd.PACKETSTART[q];
            }
            // id
            cmd[IndexOfId] = idByte;
            // length byte
            cmd[IndexOfLength] = lengthByte;
            // instruction
            cmd[IndexOfInstruction] = instructionByte;

            // parameters
            q = IndexOfFirstParam;
            foreach (Byte by in par)
            {
                cmd[q] = by;
                q++;
            }
            // checksum
            cmd[q] = C_CheckSum.GET_checkSum(cmd);
            return cmd;
        }
        
        public C_InstructionPacket  PARSE_instructionPacket_list(List<byte> _pack)
        {
            return PARSE_instructionPacket_byte(_pack.ToArray());
        }
    
        public C_InstructionPacket PARSE_instructionPacket_byte(byte[] _packetBytes)
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

            C_InstructionPacket pack = new C_InstructionPacket();
            int IndexOfCheckSum = _packetBytes.Length - 1;

            // fill in instructionByte
            pack.instructionByte = _packetBytes[IndexOfInstruction];
            // fill in id
            pack.idByte = _packetBytes[IndexOfInstruction];
            // fill in params (from const to length (checksum))
            for(int q = IndexOfFirstParam; q < IndexOfCheckSum; q++)
            {
                pack.Par.Add(_packetBytes[q]);
            }
            
            if(pack.lengthByte != _packetBytes[IndexOfLength])
            {
                // bad - but should never happen 
                // as the length byte directly creates the length of the byte array 
                // in the serial read function
                throw new Exception("bad"
            }
            
            if(pack.CheckSumByte != _packetBytes[IndexOfCheckSum])
            {
                // bad
            }

            /*
             // how to 
            int from = C_DynAdd.INDEXOF_STATUS_FIRSTPAR;
            int count = _packetBytes.Length - 1 - from;
            byte a = _packetBytes[from];
            pack.Par.Add(a.Take(count));
            */
            // message bytes after C_DynAdd.MSG_START was detected
                switch (q) // byte index in current cmd
                {
                    case (C_DynAdd.INDEXOF_ID_IN_STATUSPACKET):  // ID
                        idByte = _packetBytes[q];
                        break;
                    case (1): // LENGTH
                        lengthByteReceived = _packetBytes
                        received.= this_byte; // length of current cmd
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
                            // this byte = Checksum byte 
                            START_NEW_MSG = false; // so end of msg
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
                                byte[] cmdWithoutChecksumByte = 
                                List<byte> cmdWithoutChecksumByte = new List<byte>(curCmd);
                                SPI_CHECK_receivedCmd(curCmd, this_byte);
                            }
                        }
                        break;
                }
            }
        }
    }
}
