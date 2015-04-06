using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    public class C_MotorControl : List<C_Motor>
    {
        public e_rot actMrot;
        private object lock_yaw;
        private object lock_pitch;
        private object lock_roll;

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_Motor Yaw
        {
            get
            {
                //return GET_M(e_rot.yaw);
                return this[(int)e_rot.yaw];
            }
            set
            {
                SET_M(e_rot.yaw, value);
            }
        }
        public C_Motor Pitch
        {
            get
            {
                //return GET_M(e_rot.pitch);
                return this[(int)e_rot.pitch];
            }
            set
            {
                SET_M(e_rot.pitch, value);
            }
        }
        public C_Motor Roll
        {
            get
            {
                //return GET_M(e_rot.roll);
                return this[(int)e_rot.roll];
            }
            set
            {
                SET_M(e_rot.roll, value);
            }
        }
        public C_Motor ActualMotor
        {
            get
            {
                return GET_M(actMrot);
            }
            set
            {
                SET_M(actMrot, value);
            }
        }
        public C_Motor GET_M(e_rot rot)
        {
            return (this[(int)rot] as C_Motor);
        }
        public void SET_M(e_rot rot, C_Motor _mot)
        {
            if (GET_M(rot) != null) // is already initialized
            {
                this[(int)rot] = _mot;
            }
            //else
            //{
            //    INIT_listElementsOfAllMotors();
            //}
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_MotorControl()
            : base()
        {
            lock_yaw = new object();
            lock_pitch = new object();
            lock_roll = new object();
            INIT_individualMotors();
            INIT_groupSettings();
            actMrot = e_rot.yaw;
        }
        //public new void Add(C_Motor mot)
        //{   
        //    base.Add(mot);
        //    REFRESH_motors();
        //}

        public void INIT_listElementsOfAllMotors()
        {
            int numOfMot = Enum.GetValues(typeof(e_rot)).Length;
            for (int imot = 0; imot < numOfMot; imot++)
            {
                Add(new C_Motor((byte)imot));
            }
        }
        public void INIT_groupSettings()
        {
            foreach (C_Motor mot in this)
            {
                mot.ORDER_SET(C_DynAdd.STATUS_RETURN_LEVEL, C_DynVal.STATUS_RETURN_LEVEL_ALWAYS);
            }
        }
        public void INIT_individualMotors()
        {
            INIT_listElementsOfAllMotors();

            C_Value angleFull = new C_Value(0, 360, C_DynVal.SET_GOAL_POS_MIN, C_DynVal.SET_GOAL_POS_MAX);
            //C_Value speedFull = new C_Value(0, 100, C_DynVal.SET_MOV_SPEED_MIN, C_DynVal.SET_MOV_SPEED_MAX, 20);
            C_Value speedFull = new C_Value(0, 101, C_DynVal.SET_MOV_SPEED_NOCONTROL, C_DynVal.SET_MOV_SPEED_MAX, 5); // no control as 0

            // Motor Yaw
            Yaw = new C_Motor(e_rot.yaw, 1, // id
                    new C_Value(angleFull, 0, 360, 200), // angle
                    new C_Value(speedFull, 0, 101, 20) // speed
                );
            // Motor Pitch
            Pitch = new C_Motor(e_rot.pitch, 2, // id
                    new C_Value(angleFull, 111, 292, 200), // angle
                    new C_Value(speedFull, 0, 101, 20) // speed
                );
            // Motor Roll
            Roll = new C_Motor(e_rot.roll, 3, // id
                    new C_Value(angleFull, 156, 248, 200), // angle
                    new C_Value(speedFull, 0, 101, 20) // speed
                );
        }


        public static void ACTUALIZE_motorRegister(e_rot rot, e_regByteType type, List<byte> pars)
        {
            //lock()
            //lock(lock_roll)
            C_Motor mot = MainWindow.Ms.GET_M(rot);
            // BROADCAST??
            
            bool addGot = false;
            byte addressByte = 0;
            foreach(byte by in pars)
            {
                if(addGot == false)
                {
                    addressByte = by;
                }
                else
                {
                    mot.Reg.SET(addressByte, by, type);
                    mot.ACTUALIZE_registerBinding(addressByte);
                    addressByte++;
                }
            }

            //type = GET_typeFromParams(parsSent, parsGot);

            // _par = parameters
            //List<byte> subset12 = pars.GetRange(1, 2);
            //List<byte> subset34 = pars.GetRange(3, 2);

           
            //SET_valueFromAddress(
            //switch (type)
            //{
            //    case (e_motorDataType.anglePresent):
            //        //C_Value presentPosition = new C_Value(
            //        C_Packet.LOG_statusPacket(string.Format("Motor position = \t[{0:X} {1:X}]", pars[1], pars[2]));

            //        mot.anglePresent.Hex = subset12.ToArray();
            //        break;
            //}
        }

        //public static e_motorDataType GET_typeFromParams(List<byte> parsSent, List<byte> parsGot)
        //{
        //    //switch (parsSent.Count)
        //    //{
        //    //    case(0):
        //    //        //return e_motorDataType.ping;
        //    //        break;

        //    //    case(2): // writing 2 bytes on address
        //    //    case(3): // writing 2 bytes on address
        //    //        return e_motorDataType.ping;
        //    //        break;
        //    //}
        //}
    }
}
