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
        private C_ByteRegister reg;

        public C_ByteRegister Reg
        {
            get
            {
                return reg;
                //Actualize reg binding
            }
        }


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
            angleSeen = angleWanted;
            speedSeen = speedWanted;
            angleSent = angleWanted;
            speedSent = speedWanted;
        }
        public C_Motor(e_rot _rot, byte _id, C_Value _angle, C_Value _speed) 
        {
            id = _id;
            reg = new C_ByteRegister();

            angleWanted = _angle;
            speedWanted = _speed;
            angleSeen = new C_Value();
            speedSeen = new C_Value();
            angleSent = new C_Value();
            speedSent = new C_Value();

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
            //string fname_cmdEx = @"B:\__DIP\dev\_main_dev\EyeOut\EyeOut\EyeOut\Content\mot\cmdInEx.txt";
            string fname_cmdEx = @".\Content\mot\cmdInEx.txt";

            //string bar;
            

            //assembly.GetExecutingAssembly().GetManifestResourceStream(name);

            //string fname_cmdEx = @"..\..\..\..\cmdInEx\cmdInEx.txt";

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
                    this, cmdPackets[num].InstructionOrErrorByte, cmdPackets[num].Par
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

        public void ACTUALIZE_registerBinding(byte add)
        {
            // after change in register this is called
            // react only on high bytes 
            // -> as I always send command to actualize both L and H 
            // -> I only need to react on the second one written to register (H)
            switch (add)
            {
                case(C_DynAdd.PRESENT_POS_H):
                    angleSeen.Hex = GET_2bytesFromReg(C_DynAdd.PRESENT_POS_L, C_DynAdd.PRESENT_POS_H, e_regByteType.lastReceived);
                    LOG_reg("[Present Position] actualized form motor!");
                    break;
                case (C_DynAdd.GOAL_POS_H):
                    angleSent.Hex = GET_2bytesFromReg(C_DynAdd.GOAL_POS_L, C_DynAdd.GOAL_POS_H, e_regByteType.lastReceived);
                    LOG_reg("[Goal Position] actualized form motor!");
                    break;
                case (C_DynAdd.PRESENT_SPEED_H):
                    speedSeen.Hex = GET_2bytesFromReg(C_DynAdd.PRESENT_SPEED_L, C_DynAdd.PRESENT_SPEED_H, e_regByteType.lastReceived);
                    LOG_reg("[Present Speed] actualized form motor!");
                    break;
                case (C_DynAdd.MOV_SPEED_H):
                    speedSent.Hex = GET_2bytesFromReg(C_DynAdd.MOV_SPEED_L, C_DynAdd.MOV_SPEED_H, e_regByteType.lastReceived);
                    LOG_reg("[Moving Speed] actualized form motor!");
                    break;
                case (C_DynAdd.STATUS_RETURN_LEVEL):
                    statusReturnLevel = (e_statusReturnLevel)(reg.GET(C_DynAdd.STATUS_RETURN_LEVEL, e_regByteType.sentValue).Val);
                    LOG_reg("[Status Return Level] actualized form motor!");
                    break;
            }
        }

        public byte[] GET_2bytesFromReg(byte add_L, byte add_H, e_regByteType type)
        {
            return new byte[] { reg.GET((int)add_L, type).Val, reg.GET((int)add_H, type).Val };
        }


    }
}


