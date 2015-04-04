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
        public e_rot rotationMotor;
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

        public e_cmdEcho motorEcho;

        // cmd examples
        public static List<C_cmdin> cmdinEx;
        public static List<string> cmdinEx_str;
        /*
        public List<String> CmdinEx_str
        {
            get { return new List<String> { "One", "Two", "Three" }; }
        }*/
        private static bool cmdinEx_initialized = false;

        public C_Motor(byte _id) // because of search motor
        {
            id = 0;
            angle = new C_Value();
            speed = new C_Value();
            motorLog = e_LogMsgSource.mot;
            motorEcho = e_cmdEcho.echo;
        }

        public C_Motor(e_rot _rot, byte _id, C_Value _angle, C_Value _speed)
        {
            id = _id;
            angle = _angle;
            speed = _speed;
            
            rotationMotor = _rot;
            switch(rotationMotor)
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
            motorEcho = e_cmdEcho.echo;
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properties
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
            C_Motor.cmdinEx = new List<C_cmdin>();
            C_Motor.cmdinEx_str = new List<string>();

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

                    cmdinEx_str.Add(string.Format("{0} - {1}", name, strHex_concantenated));
                    C_Motor.cmdinEx.Add(new C_cmdin(strHex_concantenated, name));
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

        public void SEND_cmd(byte[] cmd)
        {
            SEND_cmd(motorEcho, cmd);
        }

        public void SEND_cmd(e_cmdEcho echo, byte[] cmd)
        {
            C_SPI.SEND_data(echo, cmd);
        }

        public static void SEND_BROADCAST_cmd(byte[] cmd)
        {
            C_SPI.SEND_data(e_cmdEcho.noEcho, cmd);
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void SEND_example(int num)
        {
            SEND_cmdInner(cmdinEx[num].byCmdin);
            // later write what echou should come into the [cmdInEx.txt] to each cmd
        }

        // one byte inner command with motor echo
        private void SEND_cmdInner(Byte cmdInner)
        {
            SEND_cmdInner(motorEcho, cmdInner);
        }

        // command with motor echo
        private void SEND_cmdInner(Byte[] cmdInner)
        {
            SEND_cmdInner(motorEcho, cmdInner);
        }

        // one byte inner command with given echo
        private void SEND_cmdInner(e_cmdEcho echo, Byte cmdInner)
        {
            Byte[] bys_inner = new Byte[1] { cmdInner };
            SEND_cmd(echo, C_Motor.CREATE_cmdFromCmdInner(bys_inner, id));
        }

        // command with given echo
        private void SEND_cmdInner(e_cmdEcho echo, Byte[] cmdInner)
        {
            SEND_cmd(echo, C_Motor.CREATE_cmdFromCmdInner(cmdInner, id));
        }

        private static void SEND_BROADCAST_cmdInner(Byte[] cmdInner)
        {
            SEND_BROADCAST_cmd(C_Motor.CREATE_cmdFromCmdInner(cmdInner, C_DynAdd.BROAD_CAST));
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Sending
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static Byte[] CREATE_cmdFromCmdInner(Byte[] byCmdin, Byte id)
        {
            // Instruction Packet = from pc to servo
            // OXFF 0XFF ID LENGTH INSTRUCTION PARAMETER1 …PARAMETER N CHECK SUM 
            // inner contains only these bytes:
            // INSTRUCTION, PARAMETER_1, ..., PARAMETER_N

            // this function adds first two startBytes [0xFF,0xFF], its id Byte, length Byte and Checksum Byte

            // make it into ArrayList
            Byte[] cmd = new Byte[5 + byCmdin.Length];
            //ArrayList a_cmd = new ArrayList();
            //a_cmd.Add({0xFF, 0xFF})

            //{ 0xFF, 0xFF, id, len, inner, 0x00 };
            //{ 0   , 1   , 2 , 3  , 4...., last };
            cmd[2] = id;
            int q = 4;
            foreach (Byte by in byCmdin)
            {
                cmd[q] = by;
                q++;
            }
            cmd[3] = (Byte)(byCmdin.Length + 1); // = paramN+Instruction + 1 = paramN + 2 = len
            cmd[q] = C_CheckSum.GET_checkSum(cmd);
            cmd[0] = cmd[1] = 0xFF;
            return cmd;
        }

        private static Byte[] CREATE_cmdFromStr(string str)
        {
            str.Replace(" ", ", 0x");
            string[] words = str.Split(' ');

            byte[] bytes = new byte[words.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public static byte[] CREATE_cmdInnerFromBytes(List<object> L)
        {
            // creates byte array out of list of byte / byte arrays - concatenates them
            List<byte> liby = new List<byte>();
            foreach (object o in L)
            {
                if (o is byte)
                {
                    liby.Add((byte)o);
                }
                else if (o is byte[])
                {
                    if (((byte[])o).Length > 0)
                    {
                        liby.AddRange((byte[])o);
                    }
                }
                else if (o is UInt16)
                {
                    liby.AddRange(BitConverter.GetBytes((UInt16)o));
                }
            }
            return liby.ToArray();
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion static functions
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


    public class C_cmdin
    {
        //public int lsIndex;
        //public double num;
        public Byte[] byCmdin;
        public string cmdStr;
        public C_cmdin(Byte[] _byHex, string _cmdStr)
        {
            //num = _num;
            byCmdin = _byHex;
            cmdStr = _cmdStr;
        }
        public C_cmdin(string _strHex_concatenated, string _cmdStr)
        {
            //num = _num;
            byCmdin = C_Motor.strHex2byteArray(_strHex_concatenated);
            cmdStr = _cmdStr;
        }
        public C_cmdin(string _strHex, string _del, string _cmdStr)
        {
            //num = _num;
            byCmdin = C_Motor.strHex2byteArray(_strHex, _del);
            cmdStr = _cmdStr;
        }


    }

}


