using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    public class C_MotorControl 
    {
        private List<C_Motor> M;
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
                return this[e_rot.yaw];
            }
            set
            {
                this[e_rot.yaw] = value;
                //SET_M(e_rot.yaw, value);
            }
        }
        public C_Motor Pitch
        {
            get
            {
                //return GET_M(e_rot.pitch);
                return this[e_rot.pitch];
            }
            set
            {
                this[e_rot.pitch] = value;
                //SET_M(e_rot.pitch, value);
            }
        }
        public C_Motor Roll
        {
            get
            {
                //return GET_M(e_rot.roll);
                return this[e_rot.roll];
            }
            set
            {
                this[e_rot.roll] = value;
                //SET_M(e_rot.roll, value);
            }
        }
        public C_Motor ActualMotor
        {
            get
            {
                return this[actMrot];
            }
            set
            {
                this[actMrot] = value;
                //SET_M(actMrot, value);
            }
        }
        //public C_Motor GET_M(e_rot rot)
        //{
        //    return M[(int)rot];
        //}
        //public void SET_M(e_rot rot, C_Motor _mot)
        //{
        //    if (GET_M(rot) != null) // is already initialized
        //    {
        //        M[(int)rot] = _mot;
        //    }
        //    //else
        //    //{
        //    //    INIT_listElementsOfAllMotors();
        //    //}
        //}


        public IEnumerator<C_Motor> GetEnumerator()
        {
            return M.GetEnumerator();
        }
        public C_Motor this[int i]
        {
            get { return M[i]; }
            set { M[i] = value; }
        }

        public C_Motor this[e_rot rot]
        {
            get { return M[(int)rot]; }
            set { M[(int)rot] = value; }
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
            M = new List<C_Motor>();

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
                M.Add(new C_Motor((byte)imot));
            }
        }
        public void INIT_groupSettings()
        {
            foreach (C_Motor mot in M)
            {
                // call it twice
                // 1) to set it 
                // 2) we must set the StatusReturnLevel value in register to always (because default is never)
                // 2) to actualize motorRegister stored in pc and to know that it is set to always
                mot.WRITE(C_DynAdd.STATUS_RETURN_LEVEL, C_DynVal.STATUS_RETURN_LEVEL_ONREAD);
                mot.StatusReturnLevel = e_statusReturnLevel.onRead;
                mot.WRITE(C_DynAdd.STATUS_RETURN_LEVEL, C_DynVal.STATUS_RETURN_LEVEL_ONREAD); 
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
            //C_Motor mot = ;
            // BROADCAST??
            
            bool addGot = false;
            byte addressByte = 0;
            //StringBuilder str = new StringBuilder();

            foreach(byte byteValue in pars)
            {
                if(addGot == false)
                {
                    addressByte = byteValue;
                    addGot = true;
                }
                else
                {
                    //str = new StringBuilder();
                    //str.AppendLine(string.Format(
                    //    "Want to set = mot[{0}].add[{1}].type[{2}] = [{3}]",
                    //    rot, addressByte, type, byteValue
                    //    ));
                    /*
                    str.AppendLine(string.Format(
                        "Current value = mot[{0}].add[{1}].type[{2}] = [{3}]",
                        rot, addressByte, type, MainWindow.Ms[rot].Reg.GET(addressByte, type).Val
                        ));*/
                    MainWindow.Ms[rot].ACTUALIZE_register(addressByte, byteValue, type);
                    addressByte++;
                    
                    //str.AppendLine(string.Format(
                    //    "New value = mot[{0}].add[{1}].type[{2}] = [{3}]",
                    //    rot, addressByte, type, MainWindow.Ms[rot].Reg.GET(addressByte, type).Val
                    //    ));
                    //System.Windows.Forms.MessageBox.Show(str.ToString());
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
