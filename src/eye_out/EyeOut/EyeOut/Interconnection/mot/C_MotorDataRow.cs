﻿using System;
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
        [Description("Angle Wanted")] angleWanted = 0,
        [Description("Angle Sent")] angleSent,
        [Description("Angle Actual")]angleSeen,
        [Description("Speed Wanted")]speedWanted,
        [Description("Speed Goal speed")]speedSent,
        [Description("Speed Actual speed")] speedSeen,
        [Description("Status Level")] statusReturnLevel,
        [Description("LED wanted")] LED,
        [Description("LED seen")]        LED_seen,
        [Description("Torque enable")] torqueEnable,
        [Description("In motion")] isMoving,
        [Description("Return Delay")]returnDelayTime,
        [Description("Register byte value")]regByteValue,
        [Description("Packets in LastSent queue")]
        packetsInLastSent,
        [Description("packetsDiedOfOldAge")]
        packetsDiedOfOldAge
        
    }

    public class C_MotorDataRow : INotifyPropertyChanged
    {
        public e_motorDataType dataType { get; private set; }
        public e_regByteType regByteType { get; private set; }
        public char letter_regByteType { get; private set; } // W = sent, R = seen, D = default, ' ' = undefined

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
            get { return GET_motStrings(rot); }
            set { SET_motStrings(rot, value); }
        }
        
        public string this[e_rot rot]
        {
            get { return GET_motStrings(rot); }
            set { SET_motStrings(rot, value); }
        }

        private string GET_motStrings(e_rot rot)
        {
            return motStrings[(int)rot];
        }

        private string GET_motStrings(int rot)
        {
            return motStrings[rot];
        }

        private void SET_motStrings(e_rot rot, string value)
        {
            RAISE_PropertyChanged(rot);
            motStrings[(int)rot] = value;
        }

        private void SET_motStrings(int rot, string value)
        {
            RAISE_PropertyChanged((e_rot)rot);
            motStrings[rot] = value;
        }
        private void RAISE_PropertyChanged(e_rot rot)
        {
            switch (rot)
            {
                case (e_rot.yaw): onPropertyChanged(this, "yaw"); break;
                case (e_rot.pitch): onPropertyChanged(this, "pitch"); break;
                case (e_rot.roll): onPropertyChanged(this, "roll"); break;
            }
        }

        // Declare the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        private void onPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
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
            letter_regByteType = ' ';
            address = 0;
            REFRESH();
        }

        public C_MotorDataRow(byte _address, e_regByteType _regByteType)
        {
            dataType = e_motorDataType.regByteValue ;
            address = _address;
            regByteType = _regByteType;
            letter_regByteType = 'D';
            if(_regByteType == e_regByteType.seenValue)
            {
                letter_regByteType = 'R';
            }
            if(_regByteType == e_regByteType.sentValue)
            {
                letter_regByteType = 'W';
            }
            name = string.Format("{0}", MainWindow.Ms.Yaw.Reg.GET_name(address) );

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

                    case (e_motorDataType.packetsDiedOfOldAge):
                        SET_motStrings(rot, mot.packetsDiedOfOldAge.ToString());
                        break;
                    case (e_motorDataType.packetsInLastSent):
                        SET_motStrings(rot,  mot.packetsDiedOfOldAge.ToString());
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
            SET_motStrings(rot,
                string.Format("0x{0:X2}=\t{0}", //{0,4:###0}",
                mot.Reg.GET(address, regByteType).Val
                ));
        }
    }
}
