using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace EyeOut
{
    public class C_DynAdd
    {

        //public static Byte[] byStart = { 0xFF, 0xFF };
        // cmds
        public const Byte INS_PING = 1;
        public const Byte INS_READ = 2;
        public const Byte INS_WRITE = 3;
        public const Byte INS_REG_WRITE = 4;
        public const Byte INS_ACTION = 5;
        public const Byte INS_RESET = 6;
        public const Byte INS_SYNC_WRITE = 131;

        public const Byte BROAD_CAST    = 254;
        public const Byte ID_MIN        = 0;
        public const Byte ID_MAX        = BROAD_CAST;

        public Byte[] MSG_START = { 255, 255 };
        // ____________________________________________________Goal position
        public const Byte GOAL_POS_L = 30;
        public const Byte GOAL_POS_H = 31;
        public const Byte GOAL_POS = GOAL_POS_L;
        // ____________________________________________________Moving speed
        public const Byte MOV_SPEED_L = 32;
        public const Byte MOV_SPEED_H = 33;
        public const Byte MOV_SPEED = MOV_SPEED_L;
        //public const Byte[] MOV_SPEED = { MOV_SPEED_L, MOV_SPEED_H };
        // ____________________________________________________Present speed
        public const Byte CUR_SPEED_L = 38;
        public const Byte CUR_SPEED_H = 39;


    }
}
