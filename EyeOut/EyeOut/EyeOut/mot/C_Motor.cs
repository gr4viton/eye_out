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
        public C_Value angle;
        public C_Value speed;

        // values from motor
        public C_Value angleActual;
        public C_Value speedActual;

        public double lastSend_angle;
        public double lastSend_speed;

        public e_packetEcho motorEcho;

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
            angle = new C_Value();
            speed = new C_Value();
            angleActual = angle;
            speedActual = speed;
            motorLog = e_LogMsgSource.mot;
            motorEcho = e_packetEcho.echoLast;
        }
        public C_Motor(e_rot _rot, byte _id, C_Value _angle, C_Value _speed) 
        {
            id = _id;
            angle = _angle;
            speed = _speed;
            angleActual = new C_Value();
            speedActual = new C_Value();

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
            motorEcho = e_packetEcho.echoLast;
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
                    List<byte> insAndParams = C_CONV.stringOfBytes2listOfBytes(strHex_concantenated);
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
                    this, cmdPackets[num].IdByte, cmdPackets[num].Par
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
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    }
}


