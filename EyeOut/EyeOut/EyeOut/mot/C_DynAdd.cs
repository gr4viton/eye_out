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

        public Byte[] MSG_START = { 255, 255 }; // Starting bytes of message
        // ____________________________________________________ Goal position
        public const Byte GOAL_POS_L = 30;
        public const Byte GOAL_POS_H = 31;
        public const Byte GOAL_POS = GOAL_POS_L;
        // ____________________________________________________ Present position
        public const Byte PRESENT_POS_L = 36;
        public const Byte PRESENT_POS_H = 37;
        // ____________________________________________________ Moving
        public const Byte IS_MOVING = 46;
        // ____________________________________________________ Lock
        public const Byte LOCK = 47;
        // ____________________________________________________ Moving speed
        public const Byte MOV_SPEED_L = 32;
        public const Byte MOV_SPEED_H = 33;
        public const Byte MOV_SPEED = MOV_SPEED_L;
        // ____________________________________________________ Present speed
        public const Byte PRESENT_SPEED_L = 38;
        public const Byte PRESENT_SPEED_H = 39;
        // ____________________________________________________ Torque enable
        public const Byte TORQUE_ENABLE = 24;
        // ____________________________________________________ LED
        public const Byte LED_ENABLE = 25; // one = turned on
        // ____________________________________________________ Compliance - Margin & Slope
        // setting of the pattern of output torque (viz Manual)
        public const Byte COMPLIANCE_CW_SLOPE = 28;
        public const Byte COMPLIANCE_CW_MARGIN = 26;
        public const Byte COMPLIANCE_CCW_SLOPE = 27;
        public const Byte COMPLIANCE_CCW_MARGIN = 29;
        public const Byte COMPLIANCE_PUNCH_L = 48;
        public const Byte COMPLIANCE_PUNCH_H = 49;
        // ____________________________________________________ Torque Limit  
        public const Byte TORQUE_LIMIT_L = 34;
        public const Byte TORQUE_LIMIT_H = 35;
        // ____________________________________________________ Present Load   
        public const Byte PRESENT_LOAD_L = 40;
        public const Byte PRESENT_LOAD_H = 41;
        // ____________________________________________________ Present Voltage   
        public const Byte PRESENT_VOLTAGE = 42;
        // ____________________________________________________ Present Temperature   
        public const Byte PRESENT_TEMPERATURE = 43;
        // ____________________________________________________ Registered Instruction
        public const Byte REGISTRED_INSTRUCTION = 44;
        // ____________________________________________________  Angle Limits - use with caution!
        public const Byte ANGLE_LIMIT_CW_L = 6;
        public const Byte ANGLE_LIMIT_CW_H = 7;
        public const Byte ANGLE_LIMIT_CCW_L = 8;
        public const Byte ANGLE_LIMIT_CCW_H = 9;
        
    }


    /// <summary>
    /// Value ranges of Dynamixel MX-64AR Servomotor
    /// </summary>
    public sealed partial class C_DynAdd
    {
        // ____________________________________________________ Goal position
        public const Byte SET_GOAL_POS_MIN = 0;
        //public const UInt16 SET_GOAL_POS_MAX = 0x3ff ; // according to doc.. but it is too low *4 is 0-360°
        public const UInt16 SET_GOAL_POS_MAX = 0x0FFC; // this is 0-360°
        //public Byte[] SET_GOAL_POS_MAX = { 0x3, 0xff };
        // ____________________________________________________ Moving speed
        //public const Byte SET_MOV_SPEED_NOCONTROL = 0;
        public const UInt16 SET_MOV_SPEED_NOCONTROL = 0; // need 2 bytes for CREATE_cmdInner
        //public const Byte SET_MOV_SPEED_MIN = 1;
        public const UInt16 SET_MOV_SPEED_MIN = 1;

        public const UInt16 SET_MOV_SPEED_MAX = 0x023C; // 63.7 RPM - maximal capable at 18V
        //public Byte[] SET_MOV_SPEED_MAX = {0x02, 0x3C}; 

        public const UInt16 GET_MOV_SPEED_MAX = 0x03FF; // 114 RPM - with outside accelerator (maximal measurable)
        //public Byte[] GET_MOV_SPEED_MAX = {0x03, 0xFF}; 

        // conversion to RPM = MOV_SPEED * 0.111

    }
}
