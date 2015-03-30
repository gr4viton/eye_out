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
    public partial class C_Motor
    {
        public e_rot motorPlacement;
        private e_LogMsgSource motorLog;
        public byte id;
        public C_Value angle;
        public C_Value speed;

        public double lastSend_angle;
        public double lastSend_speed;

        /*
        private double angle;
        private byte[] angleHex;
        private double speed;
        private byte[] speedHex;
        */
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
        }

        public C_Motor(e_rot _rot, byte _id, C_Value _angle, C_Value _speed)
        {
            id = _id;
            angle = _angle;
            speed = _speed;
            
            motorPlacement = _rot;
            switch(motorPlacement)
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
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync((object)cmd);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = C_SPI.WriteData(e.Argument as byte[]);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // catch if response was A-OK
            if (e.Error != null)
            {
                LOG_err(String.Format("Motor id#{2} had an error:\n{0}\n{1}", e.Error.Data, e.Error.Message, id));
                //ie Helpers.HandleCOMException(e.Error);
            }
            else
            {
                //e.Result = "tocovrati writeData";
                //MyResultObject result = (MyResultObject)e.Result;

                //LOG("DATA SENT");
                //var results = e.Result as List<object>;
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // broadcast send - may be done differently if have upper class C_MotorSet
        public static void SEND_BROADCAST_cmd(byte[] cmd)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += workerBC_DoWork;
            worker.RunWorkerAsync((object)cmd);
        }

        private static void workerBC_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = C_SPI.WriteData(e.Argument as byte[]);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void SEND_example(int num)
        {
            SEND_cmdInner(cmdinEx[num].byCmdin);
        }


        private void SEND_cmdInner(Byte inner)
        {
            Byte[] bys_inner = new Byte[1] { inner };
            SEND_cmd(C_Motor.CREATE_cmdFromInner(bys_inner, id));
        }

        private void SEND_cmdInner(Byte[] inner)
        {
            SEND_cmd(C_Motor.CREATE_cmdFromInner(inner, id));
        }

        private static void SEND_BROADCAST_cmdInner(Byte[] inner)
        {
            SEND_BROADCAST_cmd(C_Motor.CREATE_cmdFromInner(inner, C_DynAdd.BROAD_CAST));
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Sending
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static Byte[] CREATE_cmdFromInner(Byte[] byCmdin, Byte id)
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

        public static byte[] CREATE_cmdInner(List<object> L)
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
        #region ORDER REGISTER SETUP
        // ORDER functions sends the data directly (INS_WRITE)
        // REGISTER functions sends the data to register (INS_REG_WRITE)
        // SETUP functions is called from both previous with the instruction as argument
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void ORDER_ping()
        {
            SEND_cmdInner(C_DynAdd.INS_PING);
        }

        /*
         // no use - always broadcast!
        public static void ORDER_Action(List<C_Motor> Ms)
        {
            foreach (C_Motor mot in Ms)
            {
                mot.ORDER_Action();
            }
        }
        */

        public static void ORDER_ActionToAll()
        {
            SEND_BROADCAST_cmdInner(CREATE_cmdInner(new List<object> { 
                C_DynAdd.INS_ACTION 
                }));
            //LOG_mot("Broadcast to all motors: ACTION");
            /*
            angle.UPDATE_lastSent();
            speed.UPDATE_lastSent();
             */
        }

        // move with speed 
        public void ORDER_move() 
        {
            SETUP_move(C_DynAdd.INS_WRITE);
        }

        public void REGISTER_move()
        {
            SETUP_move(C_DynAdd.INS_REG_WRITE);
        }

        public void SETUP_move(byte INSTRUCTION_BYTE)
        {
            if ((angle.Dec != angle.DecLast) || (speed.Dec != speed.DecLast))
            {
                SEND_cmdInner(CREATE_cmdInner(new List<object> { 
                    INSTRUCTION_BYTE, C_DynAdd.GOAL_POS_L, angle.Hex, speed.Hex 
                }));
                LOG_SETUP_moveSpeed(INSTRUCTION_BYTE, angle, speed);
            }
        }

        
        // move without speed control - does not change current speed in motor class instance
        public void ORDER_moveBrisk()
        {
            C_Value lastSpeed = speed;
            speed.Dec = C_DynAdd.SET_MOV_SPEED_NOCONTROL;
            ORDER_move();
            speed = lastSpeed;

            //SEND_cmdInner(CREATE_cmdInner(new List<object> { 
            //    C_DynAdd.INS_WRITE, C_DynAdd.GOAL_POS_L, angle.Hex, C_DynAdd.SET_MOV_SPEED_NOCONTROL
            //}));
        }

        public void LOG_SETUP_moveSpeed(byte INSTRUCTION_BYTE, C_Value _angle, C_Value _speed)
        {
            string prefix = "ODD_MOVE";
            switch(INSTRUCTION_BYTE)
            {
                case (C_DynAdd.INS_WRITE): prefix = "ORDER_move"; break;
                case (C_DynAdd.INS_REG_WRITE): prefix = "REGISTER_move"; break;
                case (C_DynAdd.INS_READ): prefix = "READ_move"; break;
            }
                
                    
            if (_speed.Dec != C_DynAdd.SET_MOV_SPEED_NOCONTROL)
            {
                LOG(String.Format("{0}: [angle];[speed] = [{1}];[{3}] = {2}°; {4}%",
                    prefix,
                    byteArray2strHex_space(_angle.Hex.Reverse().ToArray()), _angle.Dec,
                    byteArray2strHex_space(_speed.Hex.Reverse().ToArray()), _speed.Dec
                    ));
            }
            else
            {
                LOG(String.Format("{0}: [angle];[speed] = [{1}];[{3}] = {2}°; No speed control",
                    prefix,
                    byteArray2strHex_space(_angle.Hex.Reverse().ToArray()), _angle.Dec,
                    byteArray2strHex_space(_speed.Hex.Reverse().ToArray())
                    ));
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion ORDER
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region SETUP 
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //static setup
        
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion SETUP
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


