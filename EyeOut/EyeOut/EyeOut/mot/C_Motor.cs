using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.ComponentModel; // backgroundWorker

//using System.Linq;

namespace EyeOut
{
    public enum e_rot
    {
        [Description("Roll = Around sight axis")]
        roll = 2,
        [Description("Pitch = Up/Down = nod")]
        pitch = 1,
        [Description("Yaw = Left/Right = zenit turn")]
        yaw = 0
    }

    public enum e_ledValue
    {
        Off = 0,
        Shining = 1
    }
    public enum e_enabled
    {
        Disabled = 0,
        Enabled = 1
    }
    public enum e_bool
    {
        False = 0,
        True = 1
    }

    public enum e_statusReturnLevel // address 16
    {
        never = 0,
        onRead = 1,
        allways = 2
    }

    //public enum e_statusReturnLevel // address 16
    //{
    //    [Description("0 No return against all instructions")]
    //    never = 0,
    //    [Description("1 Retrun only for the READ_DATA command")]
    //    onRead = 1,
    //    [Description("2 Return for all Instructions")]
    //    allways = 2,
    //}

    public partial class C_Motor
    {
        public e_rot rotMotor;
        private e_LogMsgSource motorLog;
        public byte id;

        // values to set
        public C_Value angleWanted;
        public C_Value speedWanted;

        // values sent to motor
        public C_Value angleSent;
        public C_Value speedSent;

        // values from motor
        public C_Value angleSeen;
        public C_Value speedSeen;
        
        //public e_packetEcho motorEcho;
        protected e_statusReturnLevel statusReturnLevel = e_statusReturnLevel.never; // befor we set it we will ignore the statusPackets

        // only manageable by functions REG_write REG_read
        private C_ByteRegister reg = new C_ByteRegister();

        public C_ByteRegister Reg
        {
            get
            {
                return reg;
            }
            // set not allowed - use ACUTALIZE_register() we need to knwo the byte changing on C_motor level
        }
        public e_ledValue LedValue;
        public e_ledValue LedValueSeen;
        public e_enabled torqueEnable;
        public e_bool isMoving;
        public int returnDelayTime;

        public e_statusReturnLevel StatusReturnLevel
        {
            get { return statusReturnLevel; }
            set { 
                statusReturnLevel = value; 
            }
        }

        
                        

        // cmd examples
        //public static List<C_cmdin> cmdinEx;
        public static List<C_Packet> cmdPackets;
        public static List<string> cmdEx_str;
        


        /*
        public List<String> CmdinEx_str
        {
            get { return new List<String> { "One", "Two", "Three" }; }
        }*/
        private static bool cmdinEx_initialized = false;

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region constructor
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_Motor(byte _id) // because of search motor
        {
            id = 0;
            motorLog = e_LogMsgSource.mot;
            reg = new C_ByteRegister();

            angleWanted = new C_Value();
            speedWanted = new C_Value();
            angleSeen = new C_Value(angleWanted);
            speedSeen = new C_Value(speedWanted);
            angleSent = new C_Value(angleWanted);
            speedSent = new C_Value(speedWanted);

        }
        public C_Motor(e_rot _rot, byte _id, C_Value _angle, C_Value _speed) 
        {
            id = _id;
            reg = new C_ByteRegister();

            angleWanted = _angle;
            speedWanted = _speed;
            angleSeen = new C_Value(angleWanted);
            speedSeen = new C_Value(speedWanted);
            angleSent = new C_Value(angleWanted);
            speedSent = new C_Value(speedWanted);


            rotMotor = _rot;
            switch(rotMotor)
            {
                case(e_rot.yaw):
                    motorLog = e_LogMsgSource.mot_yaw;
                    break;
                case (e_rot.pitch):
                    motorLog = e_LogMsgSource.mot_pitch;
                    break;
                case (e_rot.roll):
                    motorLog = e_LogMsgSource.mot_roll;
                    break;
            }
                
            //angleHex = new byte[2] { 0, 0 };
            if (cmdinEx_initialized == false)
            {
                INIT_cmdinEx();
            }
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion constructor
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region INIT
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void INIT_cmdinEx()
        {
            string fname_cmdEx = @".\Content\mot\cmdInEx.txt";

            char del = '|';
            LOAD_examples(fname_cmdEx, del);
        }

        public void LOAD_examples(string fname, char del)
        {
            C_Motor.cmdPackets = new List<C_Packet>();
            C_Motor.cmdEx_str = new List<string>();

            string strHex_concantenated;
            string name;
            string[] strArr;

            string[] lines;
            if (!System.IO.File.Exists(fname))
            {
                LOG_err(string.Format("{0}\n{1}",
                    "File with command examples not found!Searched in:",
                    fname.ToString()
                    ));
            }
            else
            {
                LOG("File with command examples found, starting to load cmdInners");

                lines = System.IO.File.ReadAllLines(fname, Encoding.ASCII);

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    strArr = line.Split(del);
                    strHex_concantenated = strArr[0];
                    name = strArr[1];

                    cmdEx_str.Add(string.Format("{0} - {1}", name, strHex_concantenated));

                    // this can be added as a new constructor of C_Packet(mot, insAndPar)
                    List<byte> insAndParams = new List<byte>();
                    //byte[] insAndParams = 
                    insAndParams.AddRange(C_CONV.strHex2byteArray(strHex_concantenated));

                    C_Motor.cmdPackets.Add(
                        new C_Packet(
                            this, insAndParams[0],
                            insAndParams.GetRange(1, insAndParams.Count - 1)
                            ));
                }
                LOG("Command examples loaded succesfully!");
                cmdinEx_initialized = true;
            }
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion INIT
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Sending
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        
        public void SEND_example(int num)
        {
            C_Packet.SEND_packet(
                new C_Packet(
                    this, cmdPackets[num].ByteInstructionOrError, cmdPackets[num].Par
                    ));
        }

        public void SEND_packet(byte INSTRUCTION_BYTE, List<object> _lsParameters = null)
        {
            C_Packet.SEND_packet(new C_Packet(this, INSTRUCTION_BYTE, _lsParameters));
        }
        
        public static void SEND_packetToAll(byte INSTRUCTION_BYTE, List<object> _lsParameters = null)
        {
            C_Packet.SEND_packet(new C_Packet(INSTRUCTION_BYTE, _lsParameters));
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Sending
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void LOG(string _msg)
        {
            C_Logger.Instance.LOG(motorLog, _msg);
        }

        public void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.mot, _msg);
        }

        public void LOG_reg(string _msg) // may be different in the future?
        {
            C_Logger.Instance.LOG_err(motorLog, _msg);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void ACTUALIZE_register(byte addressByte, byte byteValue, e_regByteType type)
        {
            Reg.SET(addressByte, byteValue, type);
            ACTUALIZE_registerBinding(addressByte);
        }

        public void ACTUALIZE_registerBinding(byte addressByte)
        {
            // after change in register this is called
            // react only on high bytes 
            // -> as I always send command to actualize both L and H 
            // -> I only need to react on the second one written to register (H)
            switch (addressByte)
            {
                case(C_DynAdd.PRESENT_POS_H):
                    ACTUALIZE_valueAndLogIt(ref angleSeen,C_DynAdd.PRESENT_POS_L,C_DynAdd.PRESENT_POS_H,e_regByteType.seenValue);
                    break;
                case (C_DynAdd.GOAL_POS_H):
                    ACTUALIZE_valueAndLogIt(ref angleSent, C_DynAdd.GOAL_POS_L, C_DynAdd.GOAL_POS_H, e_regByteType.sentValue);
                    break;

                case (C_DynAdd.PRESENT_SPEED_H):
                    ACTUALIZE_valueAndLogIt(ref speedSeen, C_DynAdd.PRESENT_SPEED_L, C_DynAdd.PRESENT_SPEED_H, e_regByteType.seenValue);
                    break;
                case (C_DynAdd.MOV_SPEED_H):
                    ACTUALIZE_valueAndLogIt(ref speedSent, C_DynAdd.MOV_SPEED_L, C_DynAdd.MOV_SPEED_H, e_regByteType.sentValue);
                    break;

                case (C_DynAdd.LED_ENABLE):
                    LedValue = (e_ledValue)(reg.GET(C_DynAdd.LED_ENABLE, e_regByteType.sentValue).Val);
                    LOG_reg("[LED sent] actualized form motor!");
                    LedValueSeen = (e_ledValue)(reg.GET(C_DynAdd.LED_ENABLE, e_regByteType.seenValue).Val);
                    LOG_reg("[LED seen] actualized form motor!");
                    break;
                case (C_DynAdd.STATUS_RETURN_LEVEL):
                    statusReturnLevel = (e_statusReturnLevel)(ACTUALIZE_byteAndLogIt(C_DynAdd.STATUS_RETURN_LEVEL, e_regByteType.sentValue));
                    break;
                case (C_DynAdd.TORQUE_ENABLE):
                    torqueEnable = (e_enabled)(ACTUALIZE_byteAndLogIt(C_DynAdd.TORQUE_ENABLE, e_regByteType.sentValue));
                    break;
                case (C_DynAdd.IS_MOVING):
                    isMoving = (e_bool)(ACTUALIZE_byteAndLogIt(C_DynAdd.TORQUE_ENABLE, e_regByteType.sentValue));
                    break;
                case(C_DynAdd.RETURN_DELAY_TIME ):
                    returnDelayTime = ACTUALIZE_byteAndLogIt(C_DynAdd.RETURN_DELAY_TIME, e_regByteType.sentValue);
                    break;
                    
            }
        }

        public byte ACTUALIZE_byteAndLogIt(byte add, e_regByteType type)
        {
            byte val = reg.GET(add, type).Val;
            LOG_reg(string.Format(
                "[{0}]-{1} actualized form motor! \t[{2}]", reg.GET_name(add), type, val.ToString()
                ));
            return val;
        }
        public void ACTUALIZE_valueAndLogIt(ref C_Value val, byte add_L, byte add_H, e_regByteType type)
        {
            val.Hex = GET_2bytesFromReg(add_L, add_H, type);
            LOG_reg(string.Format(
                "[{0}]-{1} actualized form motor! \t[{1}]", reg.GET_name(add_L), type, val.ToString()
                ));
        }

        public byte[] GET_2bytesFromReg(byte add_L, byte add_H, e_regByteType type)
        {
            return new byte[] { reg.GET((int)add_L, type).Val, reg.GET((int)add_H, type).Val };
        }

    }
}


