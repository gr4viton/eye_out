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
        // if not said otherwise the unit is degree for angle and RPM for speed
        [Description("Goal angle")]
        angleGoal = 0,
        [Description("Actual angle")]
        anglePresent,
        [Description("Goal speed")]
        speedGoal,
        [Description("Actual speed")]
        speedPresent,
    }

    public class C_MotorDataRow
    {
        public e_motorDataType type { get; set; }
        public string name { get; set; }

        public string yaw
        {
            get { return GET_ypr(e_rot.yaw); }
            set { yaw = value; }
            //set { SET_ypr(e_rot.yaw, value); }
        }
        public string pitch
        {
            get { return GET_ypr(e_rot.pitch); }
            set { pitch = value; }
            //set { SET_ypr(e_rot.pitch, value); }
        }
        public string roll
        {
            get { return GET_ypr(e_rot.roll); }
            set { roll = value; }
            //set { SET_ypr(e_rot.roll, value); }
        }
        //private string[] ypr;

        /*
        public void REFRESH_ypr(e_rot rot)
        {
            switch (rot)
            {
                case (e_rot.yaw): SET_ypr(rot, yaw); break;
                case (e_rot.pitch): SET_ypr(rot, pitch); break;
                case (e_rot.roll): SET_ypr(rot, roll); break;
            }
        }*/
        public string GET_ypr(e_rot rot)
        {
            switch (rot)
            {
                case (e_rot.yaw): return yaw; break;
                case (e_rot.pitch): return pitch; break;
                case (e_rot.roll): return roll; break;
            }
            return null;
        }
        
        public void SET_ypr(e_rot rot, string value)
        {
            switch (rot)
            {
                case (e_rot.yaw): yaw = value; break;
                case (e_rot.pitch): pitch = value; break;
                case (e_rot.roll): roll = value; break;
            }
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_MotorDataRow(e_motorDataType _type)
        {
            type = _type;
            name = EnumGetDescription.GetDescription((e_motorDataType)type);
            REFRESH();
        }

        public void REFRESH()
        {
            GET_data(type);
            
        }

        static string form_2dec = "{0:0.00}";

        public void GET_data(e_motorDataType _type)
        {
            foreach (C_Motor mot in MainWindow.Ms)
            {
                switch (_type)
                {
                    case (e_motorDataType.angleGoal):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "°", mot.angle.Dec));
                        break;
                    case (e_motorDataType.anglePresent):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "°", mot.angleActual.Dec));
                        break;
                    case (e_motorDataType.speedGoal):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "RPM", mot.speed.Dec_inRPM));
                        break;
                    case (e_motorDataType.speedPresent):
                        SET_ypr(mot.rotMotor, string.Format(form_2dec + "RPM", mot.speed.Dec_inRPM));
                        break;
                }
            }
        }
    }
}
