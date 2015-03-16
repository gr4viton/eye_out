using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.ComponentModel;

namespace EyeOut
{

    public class SEND_cmd_eventArgs : DoWorkEventArgs
    {
        public byte[] cmd;
        // nebo tady rovnou řešit to inner etc
        public SEND_cmd_eventArgs(byte id, byte[] innerCmd)
            : base(null)
        {
            cmd = CREATE_cmdFromInner(id, innerCmd);
            //base((object)cmd);
        }

        private byte[] CREATE_cmdFromInner(byte id, byte[] innerCmd)
        {
            //..check, length, id etc
            return (innerCmd);
        }
    }

    internal class C_Motor
    {
        public byte id;
        public static List<C_cmdin> cmdinEx;
        private static bool cmdinEx_initialized = false;

        public C_Motor(byte _id)
        {
            id = _id;
            if (cmdinEx_initialized == false)
            {
                INIT_cmdinEx();
            }
        }



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
                //LOG("DATA SENT");
                //var results = e.Result as List<object>;
            }
        }

        public void SEND_example(int num)
        {
            SEND_cmdInner(cmdinEx[num].byCmdin, id);
        }


        private void SEND_cmdInner(Byte[] inner, Byte id)
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
            cmd[q] = C_Motor.GET_checkSum(cmd);
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

        public static Byte GET_checkSum(Byte[] cmd)
        {
            Byte calc_check = 0x00;
            unchecked // Let overflow occur without exceptions
            {
                foreach (Byte ch in cmd)
                {
                    calc_check += ch;
                }
            }

            calc_check = (Byte)~calc_check;
            return calc_check;
        }

        public static bool CHECK_checkSum(Byte check1, Byte check2)
        {
            return (Byte)check1 == (Byte)(check2);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion static functions
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region CONV
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static Byte strHex2byte(string strHex)
        {
            byte by = strHex2byteArray(strHex)[0];
            return by;
        }

        public static Byte[] strHex2byteArray(string strHex, string delimiter)
        {
            string[] strHexDoubles = strHex.Split(' ');
            return strHexDoubles2byteArray(strHexDoubles);
        }
        public static Byte[] strHexDoubles2byteArray(string[] strHexDoubles)
        {
            Byte[] by = new Byte[strHexDoubles.Length];
            int i = 0;
            foreach (String hex in strHexDoubles)
            {
                by[i] = (Byte)Convert.ToInt32(hex, 16);
                //Console.WriteLine("int value = {0} ", by[i]);
                i++;
            }
            return by;
        }
        public static Byte[] strHex2byteArray(string strHex_concatenated)
        {
            int numOfDoubles = strHex_concatenated.Length / 2;
            string[] strHexDoubles = new string[numOfDoubles];

            for (int q = 0; q < numOfDoubles; q++)
            {
                strHexDoubles[q] = strHex_concatenated.Substring(q * 2, 2);
            }
            return strHexDoubles2byteArray(strHexDoubles);
        }

        public static void PRINT_byteArray(Byte[] bys)
        {
            foreach (byte by in bys)
                Console.WriteLine("dec {0}\t= 0x{0:X}", by);
        }

        public static bool GET_bit(Byte by, int bitNumber)
        {
            return (by & (1 << bitNumber)) != 0;
        }


        private Byte[] CONV_ang_deg2by(int deg)
        {
            // by = 0 to 1023 (0x3FF)
            // ang = 0 to 300
            //(Byte) 1023*
            int min = 0;
            int max = 300;
            e_bounds e = NOTIN_bounds(deg, min, max);
            switch (e)
            {
                case (e_bounds.bigger):
                    LOG(String.Format(
                        "Tried to calculate angle bigger then boundary {0} > [max{1}] deg. Used the maximum value.",
                        deg, max));
                    break;
                case (e_bounds.smaller):
                    LOG(String.Format(
                        "Tried to calculate angle lower then boundary {0} < [min{1}] deg. Used the minimum value.",
                        deg, min));
                    break;
            }

            UInt16 degconv = Convert.ToUInt16(1023 * GET_bounded(deg, min, max) / 300);

            Byte H = (byte)(degconv >> 8);
            Byte L = (byte)(degconv & 0xff);

            return new Byte[] { L, H };
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion CONV
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

        enum e_bounds
        {
            in_bounds = 0
            ,
            bigger = 1
                , smaller = 2
        };
        public void MOVE_relPos(Byte id, int rel_deg)
        {
            // Goal Position - Address 30, 31 (0X1E, 0x1F) 
            // CW Angle Limit ? Goal Potion ? CCW Angle Limit; 

        }

        private e_bounds NOTIN_bounds(int num, int min, int max)
        {
            if (num > max)
                return e_bounds.bigger;
            else if (num < min)
                return e_bounds.smaller;
            else
                return e_bounds.in_bounds;
        }
        private int GET_bounded(int num, int min, int max)
        {
            if (num > max)
                return max;
            else if (num < min)
                return min;
            else
                return num;
        }

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


