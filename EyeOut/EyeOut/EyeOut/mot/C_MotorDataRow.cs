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
        [Description("Return Delay")]returnDelayTime,
        [Description("Register byte value")] regByteValue,
        
    }
   
    public class C_MotorDataRow
    {
        public e_motorDataType dataType { get; private set; }
        public e_regByteType regByteType { get; private set; }

        public byte address { get; private set; }
        public string name { get; private set; }

        //public static event EventHandler yawChanged;
        //public static event EventHandler pitchChanged;
        //public static event EventHandler rollChanged;

        public string yaw
        {
            get { return GET_motStrings(e_rot.yaw); }
            set { SET_motStrings(e_rot.yaw, value); }
        }
        public string pitch
        {
            get { return GET_motStrings(e_rot.pitch); }
            set { SET_motStrings(e_rot.pitch, value); }
        }
        public string roll
        {
            get { return GET_motStrings(e_rot.roll); }
            set { SET_motStrings(e_rot.roll, value); }
        }

        private string[] motStrings;

        public string this[int rot]
        {
            get { return motStrings[rot]; }
            set { motStrings[rot] = value; }
        }
        
        public string this[e_rot rot]
        {
            get { return motStrings[(int)rot]; }
            set { motStrings[(int)rot] = value; }
        }

        private string GET_motStrings(e_rot rot)
        {
            return motStrings[(int)rot];
        }

        private void SET_motStrings(e_rot rot, string value)
        {
            motStrings[(int)rot] = value;
            OnCollectionChanged();
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_MotorDataRow(e_motorDataType _dataType)
        {
            if (dataType == e_motorDataType.regByteValue)
            {
                throw new Exception("Cannot create MotorDataRow with motorDataType = regByteValue, without specifying register byte address - please use different constructor!");
            }
            dataType = _dataType;
            name = EnumGetDescription.GetDescription((e_motorDataType)dataType);
            motStrings = new string[3];
            REFRESH();
        }

        public C_MotorDataRow(byte _address, e_regByteType _regByteType)
        {
            dataType = e_motorDataType.regByteValue ;
            address = _address;
            regByteType = _regByteType;
            name = MainWindow.Ms.Yaw.Reg.GET_name(address);

            motStrings = new string[3];
            REFRESH();
        }

        public void REFRESH()
        {
            GET_dataStringsForMotors(dataType);
        }

        static string form_2dec = "{0:0.00}";

        public void GET_dataStringsForMotors(e_motorDataType _dataType)
        {

            //SystMessageBox.Show(str.ToString());

            foreach (C_Motor mot in MainWindow.Ms)
            {
                e_rot rot = mot.rotMotor;
                switch (_dataType)
                {
                    case (e_motorDataType.angleWanted):
                        SET_motStrings(rot, string.Format(form_2dec + "°", mot.angleWanted.Dec));
                        break;
                    case (e_motorDataType.angleSent):
                        SET_motStrings(rot, string.Format(form_2dec + "°", mot.angleSent.Dec));
                        break;
                    case (e_motorDataType.angleSeen):
                        SET_motStrings(rot, string.Format(form_2dec + "°", mot.angleSeen.Dec));
                        break;
                    case (e_motorDataType.speedWanted):
                        SET_motStrings(rot, string.Format(form_2dec + "RPM", mot.speedWanted.Dec_inRPM));
                        break;
                    case (e_motorDataType.speedSent):
                        SET_motStrings(rot, string.Format(form_2dec + "RPM", mot.speedSent.Dec));
                        break;
                    case (e_motorDataType.speedSeen):
                        SET_motStrings(rot, string.Format(form_2dec + "RPM", mot.speedSeen.Dec_inRPM));
                        break;
                    case (e_motorDataType.LED):
                        SET_motStrings(rot, mot.LedValue.ToString());
                        break;
                    case (e_motorDataType.LED_seen):
                        SET_motStrings(rot, mot.LedValueSeen.ToString());
                        break;
                    case (e_motorDataType.statusReturnLevel):
                        SET_motStrings(rot, mot.StatusReturnLevel.ToString());
                        break;
                    case (e_motorDataType.torqueEnable):
                        SET_motStrings(rot, mot.torqueEnable.ToString());
                        break;
                    case (e_motorDataType.isMoving):
                        SET_motStrings(rot, mot.isMoving.ToString());
                        break;
                    case (e_motorDataType.returnDelayTime):
                        SET_motStrings(rot, string.Format(form_2dec+"us",mot.returnDelayTime*2));
                        break;
                    case (e_motorDataType.regByteValue):
                        SET_motStringsFromRegister(mot);
                        break;
                }
            }
        }

        private void SET_motStringsFromRegister(C_Motor mot)
        {
            e_rot rot = mot.rotMotor;
            // show it as byte
            SET_motStrings(rot, mot.Reg.GET(address, regByteType).Val.ToString());
        }
    }
}
