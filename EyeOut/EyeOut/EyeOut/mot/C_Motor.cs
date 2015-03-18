using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.ComponentModel;

//using System.Linq;

namespace EyeOut
{
    public partial class C_Motor
    {
        public byte id;
        public C_Value angle;
        public C_Value speed;

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

        public C_Motor()
        {
            id = 0;
        }

        public C_Motor(byte _id)
        {
            id = _id;
            angle = new C_Value();

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
            string fname_cmdEx = @"B:\__DIP\dev\_main_dev\EyeOut\cmdInEx\cmdInEx.txt";
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
                //lines = System.IO.File.ReadAllLines(fname);
                //string[] lines = System.IO.File.ReadAllLines(fname);

                // Display the file contents by using a foreach loop.
                // System.Console.WriteLine("Contents of WriteLines2.txt = ");

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    strArr = line.Split(del);
                    strHex_concantenated = strArr[0];
                    name = strArr[1];

                    //lsCmdEx.Items.Add(string.Format("{0} - {1}", Convert.ToString(c.byCmdin), c.cmdStr));
                    cmdinEx_str.Add(string.Format("{0} - {1}", name, strHex_concantenated));
                    //cmdinEx.Items.Add(name);
                    C_Motor.cmdinEx.Add(new C_cmdin(strHex_concantenated, name));
                    // Use a tab to indent each line of the file.
                    //Console.WriteLine("\t" + line);
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
                LOG(String.Format("Motor id#{2} had an error:\n{0}\n{1}", e.Error.Data, e.Error.Message, id));
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
            //string hex = BitConverter.ToString(read_buff).Replace("-", " ");
            str.Replace(" ", ", 0x");
            string[] words = str.Split(' ');
            //int len = words.Length;
            //Byte[] cmd = new Byte[len];
            //for(;len>=0; len--)
            //  cmd[len] = (Byte) BitConverter.(words[len]);
            //foreach (string word in words)

            byte[] bytes = new byte[words.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public byte[] CREATE_cmdInner(List<object> L)
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
            }
            return liby.ToArray();
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.mot, _msg);
        }

        public static void LOG_err(string _msg)
        {
            // afterwards -> through another type
            //C_Logger.Instance.LOG(e_LogMsgSource.mot, _msg, type = error); 
            C_Logger.Instance.LOG_err(e_LogMsgSource.mot, _msg);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region ORDER
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void ORDER_ping()
        {
            SEND_cmdInner(C_DynAdd.INS_PING);
        }
        // move with speed stored in motor - not writing to control speed register
        public void ORDER_moveLastSpeed()
        {
            SEND_cmdInner(CREATE_cmdInner(new List<object> { 
                C_DynAdd.INS_WRITE, C_DynAdd.GOAL_POS_L, angle.Hex
            }));
            LOG(String.Format("ORDER_move: [{0}] = {1}°", byteArray2strHex_space(angle.Hex), angle.Dec));
        }

        // move with speed 
        public void ORDER_move() 
        {
            SEND_cmdInner(CREATE_cmdInner(new List<object> { 
                C_DynAdd.INS_WRITE, C_DynAdd.GOAL_POS_L, angle.Hex //, speedHex 
            }));
            LOG(String.Format("ORDER_move: [{0}] = {1}°", byteArray2strHex_space(angle.Hex), angle.Dec));
        }
        // move without speed control
        public void ORDER_moveBrisk()
        {
            SEND_cmdInner(CREATE_cmdInner(new List<object> { 
                C_DynAdd.INS_WRITE, C_DynAdd.GOAL_POS_L, angle.Hex, C_DynAdd.SET_MOV_SPEED_NOCONTROL
            }));
            LOG(String.Format("ORDER_move: [{0}] = {1}°", byteArray2strHex_space(angle.Hex), angle.Dec));
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion ORDER
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region MOVE
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        /*
        public void MOVE_absPosLastSpeed(int abs_deg)
        {
            // Goal Position - Address 30, 31 (0X1E, 0x1F) 
            // CW Angle Limit ? Goal Potion ? CCW Angle Limit; 

            // ptat se na boundary CW angle limit a CCW angle limit
            Byte[] byAng = CONV_ang_deg2by(abs_deg);

            Byte[] cmdInner = new Byte[4];
            //cmdInner[0] = INS_
            cmdInner[0] = C_DynAdd.INS_WRITE;
            cmdInner[1] = C_DynAdd.GOAL_POS_L;
            cmdInner[2] = byAng[0];
            cmdInner[3] = byAng[1];
            //{ 0x07, 0x03, 0x1E, 0x00, 0x02, 0x00, 0x02 };
            //  len , writ, addr
            SEND_cmd(CREATE_cmdFromInner(cmdInner, id));

        }
        */
        /*
        public void MOVE_relPos(Byte id, int rel_deg)
        {
            // Goal Position - Address 30, 31 (0X1E, 0x1F) 
            // CW Angle Limit ? Goal Potion ? CCW Angle Limit; 

        }*/

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion CONV
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


