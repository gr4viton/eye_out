using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; // description

namespace EyeOut
{

    public enum e_motorDataType
    {
        // Wanted = value stored in C_motor
        // Sent = Goal - sent into the Dynamixel Motor
        // Actual = Present - received as actual Dynamixel Motor position

        // if not said otherwise the unit is degree for angle and RPM for speed
        [Description("Wanted angle")] angleWanted = 0,
        [Description("Sent angle")] angleSent,
        [Description("Actual angle")] angleSeen,
        [Description("Wanted speed")] speedWanted,
        [Description("Goal speed")] speedSent,
        [Description("Actual speed")] speedSeen,
        [Description("Status Level")] statusReturnLevel,
        [Description("LED wanted")] LED,
        [Description("LED seen")]        LED_seen,
        [Description("Torque enable")] torqueEnable,
        [Description("In motion")] isMoving,
        [Description("Return Delay")]returnDelayTime
        
    }

    public class C_MotorDataRow
    {
        public e_motorDataType type { get; set; }
        public string name { get; set; }

        //public static event EventHandler yawChanged;
        //public static event EventHandler pitchChanged;
        //public static event EventHandler rollChanged;

        public string yaw
        {
            get { return GET_ypr(e_rot.yaw); }
            set
            {
                SET_ypr(e_rot.yaw, value);
                //EventHandler handler = yawChanged;
                //if (handler != null)
                //    handler(null, EventArgs.Empty);
            }
        }
        public string pitch
        {
            get { return GET_ypr(e_rot.pitch); }
            set { SET_ypr(e_rot.pitch, value); }
        }
        public string roll
        {
            get { return GET_ypr(e_rot.roll); }
            set { SET_ypr(e_rot.roll, value); }
        }

        private string[] ypr;

        public string GET_ypr(e_rot rot)
        {
            return ypr[(int)rot];
        }

        public void SET_ypr(e_rot rot, string value)
        {
            ypr[(int)rot] = value;
            //switch (rot)
            //{
            //    case (e_rot.yaw): yaw = value; break;
            //}
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_MotorDataRow(e_motorDataType _type)
        {
            type = _type;
            name = EnumGetDescription.GetDescription((e_motorDataType)type);
            ypr = new string[3];
            REFRESH();
        }

        public void REFRESH()
        {
            GET_data(type);
        }

        static string form_2dec = "{0:0.00}";

        public void GET_data(e_motorDataType _type)
        {

            //SystMessageBox.Show(str.ToString());

            foreach (C_Motor mot in MainWindow.Ms)
            {
                switch (_type)
                {
                    case (e_motorDataType.angleWanted):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "°", mot.angleWanted.Dec));
                        break;
                    case (e_motorDataType.angleSent):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "°", mot.angleSent.Dec));
                        break;
                    case (e_motorDataType.angleSeen):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "°", mot.angleSeen.Dec));
                        break;
                    case (e_motorDataType.speedWanted):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "RPM", mot.speedWanted.Dec_inRPM));
                        break;
                    case (e_motorDataType.speedSent):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "RPM", mot.speedSent.Dec));
                        break;
                    case (e_motorDataType.speedSeen):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "RPM", mot.speedSeen.Dec_inRPM));
                        break;
                    case (e_motorDataType.LED):
                        SET_ypr(mot.rotMotor, mot.LedValue.ToString());
                        break;
                    case (e_motorDataType.LED_seen):
                        SET_ypr(mot.rotMotor, mot.LedValueSeen.ToString());
                        break;
                    case (e_motorDataType.statusReturnLevel):
                        SET_ypr(mot.rotMotor, mot.StatusReturnLevel.ToString());
                        break;
                    case (e_motorDataType.torqueEnable):
                        SET_ypr(mot.rotMotor, mot.torqueEnable.ToString());
                        break;
                    case (e_motorDataType.isMoving):
                        SET_ypr(mot.rotMotor, mot.isMoving.ToString());
                        break;
                    case (e_motorDataType.returnDelayTime):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec+"us",mot.returnDelayTime*2));
                        break;
                        
                }
            }
        }
    }
}
