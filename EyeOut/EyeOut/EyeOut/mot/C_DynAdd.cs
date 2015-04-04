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
        // sizeof individual parts of packet (instruction and status
        public const int SIZEOF_PACKETSTART = 2*sizeof(byte);
        public const int SIZEOF_ID = sizeof(byte);
        public const int SIZEOF_LENGTH = sizeof(byte);
        public const int SIZEOF_INSTRUCTION = sizeof(byte);
        public const int SIZEOF_CHECKSUM = sizeof(byte);

        public const int SIZEOF_ERROR = sizeof(byte);

        // index of individual bytes in instruction & status(return) packet
        public const int INDEXOF_PACKETSTART_IN_INSTRUCTIONPACKET   = 0;
        public const int INDEXOF_PACKETSTART_IN_STATUSPACKET        = 0;
        public const int INDEXOF_ID_IN_INSTRUCTIONPACKET            = 2;
        public const int INDEXOF_ID_IN_STATUSPACKET                 = 2;
        public const int INDEXOF_LENGTH_IN_INSTRUCTIONPACKET        = 3;
        public const int INDEXOF_LENGTH_IN_STATUSPACKET             = 3;
        public const int INDEXOF_INSTRUCTION_IN_INSTRUCTIONPACKET   = 4;
        public const int INDEXOF_ERROR_IN_STATUSPACKET              = 4;
        public const int INDEXOF_FIRSTPARAM_IN_INSTRUCTIONPACKET    = 5;
        public const int INDEXOF_FIRSTPARAM_IN_STATUSPACKET         = 5;


        //public static byte[] byStart = { 0xFF, 0xFF };
        // cmds
        public const byte INS_PING = 1;
        public const byte INS_READ = 2;
        public const byte INS_WRITE = 3;
        public const byte INS_REG_WRITE = 4;
        public const byte INS_ACTION = 5;
        public const byte INS_RESET = 6;
        public const byte INS_SYNC_WRITE = 131;

        public const byte BROAD_CAST    = 254;
        public const byte ID_MIN        = 0;
        public const byte ID_MAX = BROAD_CAST;

        public static byte[] PACKETSTART = { 255, 255 }; // Starting bytes of message
        public const int RECEIVING_BUFFER_VOLUME = 143; // the volume of receiving buffer of RX-64 
        public const int MAX_BYTES_OF_PACKET = RECEIVING_BUFFER_VOLUME;
        public const int MAX_PARAMETERS = MAX_BYTES_OF_PACKET - 6; // 6 = STARTING ZEROS, ID, LENGTH, INSTRUCTION, CHECKSUM
        // ____________________________________________________ Goal position
        public const byte GOAL_POS_L = 30;
        public const byte GOAL_POS_H = 31;
        public const byte GOAL_POS = GOAL_POS_L;
        // ____________________________________________________ Present position
        public const byte PRESENT_POS_L = 36;
        public const byte PRESENT_POS_H = 37;
        // ____________________________________________________ Moving
        public const byte IS_MOVING = 46;
        // ____________________________________________________ Lock
        public const byte LOCK = 47;
        // ____________________________________________________ Moving speed
        public const byte MOV_SPEED_L = 32;
        public const byte MOV_SPEED_H = 33;
        public const byte MOV_SPEED = MOV_SPEED_L;
        // ____________________________________________________ Present speed
        public const byte PRESENT_SPEED_L = 38;
        public const byte PRESENT_SPEED_H = 39;
        // ____________________________________________________ Torque enable
        public const byte TORQUE_ENABLE = 24;
        // ____________________________________________________ LED
        public const byte LED_ENABLE = 25; // one = turned on
        // ____________________________________________________ Compliance - Margin & Slope
        // setting of the pattern of output torque (viz Manual)
        public const byte COMPLIANCE_CW_SLOPE = 28;
        public const byte COMPLIANCE_CW_MARGIN = 26;
        public const byte COMPLIANCE_CCW_SLOPE = 27;
        public const byte COMPLIANCE_CCW_MARGIN = 29;
        public const byte COMPLIANCE_PUNCH_L = 48;
        public const byte COMPLIANCE_PUNCH_H = 49;
        // ____________________________________________________ Torque Limit  
        public const byte TORQUE_LIMIT_L = 34;
        public const byte TORQUE_LIMIT_H = 35;
        // ____________________________________________________ Present Load   
        public const byte PRESENT_LOAD_L = 40;
        public const byte PRESENT_LOAD_H = 41;
        // ____________________________________________________ Present Voltage   
        public const byte PRESENT_VOLTAGE = 42;
        // ____________________________________________________ Present Temperature   
        public const byte PRESENT_TEMPERATURE = 43;
        // ____________________________________________________ Registered Instruction
        public const byte REGISTRED_INSTRUCTION = 44;
        // ____________________________________________________  Angle Limits - use with caution!
        public const byte ANGLE_LIMIT_CW_L = 6;
        public const byte ANGLE_LIMIT_CW_H = 7;
        public const byte ANGLE_LIMIT_CCW_L = 8;
        public const byte ANGLE_LIMIT_CCW_H = 9;
        
    }


    /// <summary>
    /// Value ranges of Dynamixel MX-64AR Servomotor
    /// </summary>
    public sealed partial class C_DynAdd
    {
        // ____________________________________________________ Goal position
        public const byte SET_GOAL_POS_MIN = 0;
        //public const UInt16 SET_GOAL_POS_MAX = 0x3ff ; // according to doc.. but it is too low *4 is 0-360°
        public const UInt16 SET_GOAL_POS_MAX = 0x0FFC; // this is 0-360°
        //public byte[] SET_GOAL_POS_MAX = { 0x3, 0xff };
        // ____________________________________________________ Moving speed
        //public const byte SET_MOV_SPEED_NOCONTROL = 0;
        public const UInt16 SET_MOV_SPEED_NOCONTROL = 0; // need 2 bytes for CREATE_cmdInner
        //public const byte SET_MOV_SPEED_MIN = 1;
        public const UInt16 SET_MOV_SPEED_MIN = 1;

        public const UInt16 SET_MOV_SPEED_MAX = 0x023C; // 63.7 RPM - maximal capable at 18V
        //public byte[] SET_MOV_SPEED_MAX = {0x02, 0x3C}; 

        public const UInt16 GET_MOV_SPEED_MAX = 0x03FF; // 114 RPM - with outside accelerator (maximal measurable)
        //public byte[] GET_MOV_SPEED_MAX = {0x03, 0xFF}; 

        // conversion to RPM = MOV_SPEED * 0.111

    }
}
