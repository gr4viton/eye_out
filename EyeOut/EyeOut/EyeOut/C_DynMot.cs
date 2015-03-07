using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace EyeOut
{

    // class motor 
    //- last know angle, last known rozsah, id, etc..
    // init
    // Status Return Level    Address 16 (0X10)

    // handler which converts the nudId static argument of C_dynMot .. binded to change of nud

    public class C_DynMot
    {
        Byte id = 0;
        public C_DynMot(Byte a_id)
        {
            id = a_id;
        }


        //This event can cause any method which conforms
        //to MyEventHandler to be called.
        public event h_LOG_String WANNA_LOG_msgAppendLine;
        public event h_SEND_Bytes WANNA_SEND_cmd;


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // COMMANDS - LOW LEVEL
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // act_con_status change -> bind the EV_connection function

        private void CREATE_cmd()
        {

            Byte[] byStart = { 0xFF, 0xFF };
            Byte[] byID = { 0x01 };
            Byte[] byLen = { 0x01 }; // = num of param(N) + 2
            /* 
             * 0x01 = ping [0params]
             * 0x02 = read [2]
             * 0x03 = write [2+] 
             * 0x04 = reg write - starts after action command [2+]
             * 0x05 = action command [0]
             * 0x06 = reset to factory settings [0]
             * 0x83 = sync write - simulateneous to more servos [4+]
             */

            Byte[] byPing = { 0x01 };
            Byte[] byRead = { 0x02 };
            Byte[] byWrite = { 0x03 };

            // 

            Byte[] byREAD_ID = { 0x03, 0x01 };
            Byte[] byPING_id1 = { 0x01, 0x01 };

            Byte[] cmdPING_id1 = { 0xFF, 0xFF, 0x01, 0x02, 0x01, 0xFB };

            // byStart = new byte[2];
            //byStart[0] = 0xFF;
            Byte[] cmd = cmdPING_id1;

            SEND_cmd(cmd);
        }



        private void SEND_cmdInner(Byte[] inner)
        {
            SEND_cmd(CREATE_cmdFromInner(inner, id));
        }
        private Byte[] CREATE_cmdFromInner(Byte[] inner, Byte id)
        {
            // Instruction Packet = from pc to servo
            // OXFF 0XFF ID LENGTH INSTRUCTION PARAMETER1 …PARAMETER N CHECK SUM 
            // inner contains only these bytes:
            // INSTRUCTION, PARAMETER_1, ..., PARAMETER_N

            // this function adds first two startBytes [0xFF,0xFF], its id Byte, length Byte and Checksum Byte

            // make it into ArrayList
            Byte[] cmd = new Byte[5 + inner.Length];
            //ArrayList a_cmd = new ArrayList();
            //a_cmd.Add({0xFF, 0xFF})

            //{ 0xFF, 0xFF, id, len, inner, 0x00 };
            //{ 0   , 1   , 2 , 3  , 4...., last };
            cmd[2] = id;
            int q = 4;
            foreach (Byte by in inner)
            {
                cmd[q] = by;
                q++;
            }
            cmd[3] = (Byte)(inner.Length + 1); // = paramN+Instruction + 1 = paramN + 2 = len
            cmd[q] = C_CheckSum.GET_checkSum(cmd);
            cmd[0] = cmd[1] = 0xFF;
            return cmd;
        }
        private Byte[] CREATE_cmdFromStr(string str)
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

        public static Byte nudId = 1;
        private Byte ID_fromNUDid()
        {
            //return Convert.ToByte(nudID.Text);
            return nudId;
        }

        private void SEND_cmdID(Byte[] cmd, Byte id)
        {
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //id = 254;
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            cmd[2] = id;
            SEND_cmd(cmd);
        }

        // Instruction Packet = from pc to servo
        //  OXFF 0XFF ID LENGTH INSTRUCTION PARAMETER1 …PARAMETER N CHECK SUM 
        // Status packet = 
        // OXFF  0XFF  ID  LENGTH  ERROR  PARAMETER1  PARAMETER2…PARAMETER  N CHECK SUM 
        Byte[] cmdPING_id1 = { 0xFF, 0xFF, 0x01, 0x02, 0x01, 0xFB };
        Byte[] cmdEx1 = { 0xFF, 0xFF, 0x01, 0x04, 0x02, 0x2b, 0x01, 0xCC }; // Read internal temeperature
        Byte[] cmdEx2 = { 0XFF, 0XFF, 0XFE, 0X04, 0X03, 0X03, 0X01, 0XF6 }; // Sets the ID of RX-64 as “1’”.  = BROADCAST !!!
        Byte[] cmdEx16 = { 0xFF, 0xFF, 0x01, 0x05, 0x03, 0x18, 0x01, 0x01, 0xDD }; //  Turns on the LED and enables Torque. 
        Byte[] cmdEx17 = { 0xFF, 0xFF, 0x01, 0x07, 0x03, 0x1E, 0x00, 0x02, 0x00, 0x02, 0xD2 }; // Locates at the Position 180° with the speed of 57RPM. 

        // without first 3 bytes and checksum byte
        Byte[] cmdinEx0 = { 0x01 }; // Ping
        Byte[] cmdinEx1 = { 0x02, 0x2b, 0x01 }; // Read internal temeperature
        Byte[] cmdinEx16 = { 0x03, 0x18, 0x01, 0x01 }; //  Turns on the LED and enables Torque. 
        Byte[] cmdinEx17 = { 0x03, 0x1E, 0x00, 0x02, 0x00, 0x02 }; // Locates at the Position 180° with the speed of 57RPM. 
        Byte[] cmdinEx17_2 = { 0x03, 0x1E, 0x00, 0x03, 0x00, 0x02 }; // Locates at the Position 185° with the speed of 57RPM. 

        public void SEND_example(int ex)
        {
            switch (ex)
            {
                case (0): SEND_cmdInner(cmdinEx0); break;
                case (1): SEND_cmdInner(cmdinEx1); break;
                case (2): SEND_cmd(cmdEx2); break; // broadcast
                case (16): SEND_cmdInner(cmdinEx16); break;
                case (17): SEND_cmdInner(cmdinEx17); break;
                case (172): SEND_cmdInner(cmdinEx17_2); break;
            }
        }


        private void SEND_cmd(Byte[] cmd)
        {
            WANNA_SEND_cmd(cmd);
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // COMMANDS - MEDIUM LEVEL
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void MOVE_absPosLastSpeed(int abs_deg)
        {
            id_MOVE_absPosLastSpeed(id, abs_deg);
        }

        public void id_MOVE_absPosLastSpeed(Byte id, int abs_deg)
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
            SEND_cmd(CREATE_cmdFromInner(cmdInner, ID_fromNUDid()));

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
                    WANNA_LOG_msgAppendLine(String.Format(
                        "Tried to calculate angle bigger then boundary {0} > [max{1}] deg. Used the maximum value.",
                        deg, max));
                    break;
                case (e_bounds.smaller):
                    WANNA_LOG_msgAppendLine(String.Format(
                        "Tried to calculate angle lower then boundary {0} < [min{1}] deg. Used the minimum value.",
                        deg, min));
                    break;
            }



            UInt16 degconv = Convert.ToUInt16(1023 * GET_bounded(deg, min, max) / 300);

            Byte H = (byte)(degconv >> 8);
            Byte L = (byte)(degconv & 0xff);

            return new Byte[] { L, H };
        }
    }
}
