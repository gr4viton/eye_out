using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace EyeOut
{
    // RX-64_Manual.pdf
    // 18-03-2015

    /// <summary>
    /// Addresses of Dynamixel MX-64AR Servomotor
    /// </summary>
    public sealed partial class C_DynAdd
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
        public const Byte ID_MAX = BROAD_CAST;

        public Byte[] MSG_START = { 255, 255 }; // CONST??
        // ____________________________________________________Goal position
        public const Byte GOAL_POS_L = 30;
        public const Byte GOAL_POS_H = 31;
        public const Byte GOAL_POS = GOAL_POS_L;
        // ____________________________________________________Moving speed
        public const Byte MOV_SPEED_L = 32;
        public const Byte MOV_SPEED_H = 33;
        public const Byte MOV_SPEED = MOV_SPEED_L;
        // ____________________________________________________Present speed
        public const Byte CUR_SPEED_L = 38;
        public const Byte CUR_SPEED_H = 39;
    }


    /// <summary>
    /// Value ranges of Dynamixel MX-64AR Servomotor
    /// </summary>
    public sealed partial class C_DynAdd
    {
        // ____________________________________________________Goal position
        public const Byte SET_GOAL_POS_MIN = 0;
        //public const UInt16 SET_GOAL_POS_MAX = 0x3ff ; // according to doc.. but it is too low *4 is 0-360°
        public const UInt16 SET_GOAL_POS_MAX = 0x0FFC; // this is 0-360°
        //public Byte[] SET_GOAL_POS_MAX = { 0x3, 0xff };
        // ____________________________________________________Moving speed
        //public const Byte SET_MOV_SPEED_NOCONTROL = 0;
        public const UInt16 SET_MOV_SPEED_NOCONTROL = 0; // need 2 bytes for CREATE_cmdInner
        //public const Byte SET_MOV_SPEED_MIN = 1;
        public const UInt16 SET_MOV_SPEED_MIN = 1;

        public const UInt16 SET_MOV_SPEED_MAX = 0x023C; // 63.7 RPM - maximal capable at 18V
        //public Byte[] SET_MOV_SPEED_MAX = {0x02, 0x3C}; 

        public const UInt16 GET_MOV_SPEED_MAX = 0x03FF; // 114 RPM - with outside accelerator (maximal measurable)
        //public Byte[] GET_MOV_SPEED_MAX = {0x03, 0xFF}; 

    }
}
