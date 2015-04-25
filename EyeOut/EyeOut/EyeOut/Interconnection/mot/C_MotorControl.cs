using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    public class C_MotorControl 
    {
        private static List<C_Motor> M;
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

        public static C_Motor GET_M(e_rot rot)
        {
            return M[(int)rot];
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
            actMrot = e_rot.yaw;
        }

        public void INIT_listElementsOfAllMotors()
        {
            int numOfMot = Enum.GetValues(typeof(e_rot)).Length;
            for (int imot = 0; imot < numOfMot; imot++)
            {
                M.Add(new C_Motor((byte)imot));
            }
        }
        public static void INIT_groupSettings()
        {
            foreach (C_Motor mot in M)
            {
                // call it twice
                // 1) to set it 
                // 2) we must set the StatusReturnLevel value in register to always (because default is never)
                // 2) to actualize motorRegister stored in pc and to know that it is set to always
                mot.WRITE(C_DynAdd.STATUS_RETURN_LEVEL, C_DynVal.STATUS_RETURN_LEVEL_ONREAD);
                mot.StatusReturnLevel = e_statusReturnLevel.onRead;
                //mot.WRITE(C_DynAdd.STATUS_RETURN_LEVEL, C_DynVal.STATUS_RETURN_LEVEL_ONREAD); 

                // read out all values
                mot.READ_wholeRegister();
            }
        }

        public static void ACTUALIZE_queueCounts(List<Queue<C_Packet>> queueSent)
        {

            foreach (C_Motor mot in M)
            {
                //queueSent_Count[rotMot] = listSent.Count;
                mot.packetsInLastSent = C_SPI.queueSent_Count[(int)mot.rotMotor];
            }
            C_SPI.LOG_debug(string.Format(
                    "queueSent[ yaw=[{0}]; pitch=[{1}]; roll=[{2}] ]",
                    GET_M(e_rot.yaw).packetsInLastSent, 
                    GET_M(e_rot.pitch).packetsInLastSent, 
                    GET_M(e_rot.roll).packetsInLastSent
                    ));
            MainWindow.REFRESH_motorData();
        }

        public void INIT_individualMotors()
        {
            INIT_listElementsOfAllMotors();

            // Motor Yaw
            Yaw = new C_Motor(e_rot.yaw, 1, // id
                    new C_Value(C_Value.angleFull, 0, 360, 180), // angle
                    new C_Value(C_Value.speedFull, 0, 101, 20) // speed
                );
            // Motor Pitch
            Pitch = new C_Motor(e_rot.pitch, 2, // id
                    new C_Value(C_Value.angleFull, 111, 292, 200), // angle
                    new C_Value(C_Value.speedFull, 0, 101, 20) // speed
                );
            // Motor Roll
            Roll = new C_Motor(e_rot.roll, 3, // id
                    new C_Value(C_Value.angleFull, 156, 248, 200), // angle
                    new C_Value(C_Value.speedFull, 0, 101, 20) // speed
                );
        }


        public static void ACTUALIZE_motorRegister(e_rot rot, e_regByteType type, List<byte> pars)
        {
            if (pars.Count > 0)
            {
                byte addressByte = pars[0];
                byte[] parValues = pars.Skip(1).ToArray();

                C_Packet.LOG_statusPacket(string.Format(
                    "Status OK - actualizing mot[{0}] register type[{1}]: From address[{2}]=[{3}], these values[{4}]",
                    rot, type, addressByte, MainWindow.Ms[rot].Reg.GET_name(addressByte),
                    C_CONV.byteArray2strHex_space(parValues)));
                foreach (byte byteValue in parValues)
                {
                    C_SPI.LOG_unimportant(string.Format(
                        "going to acualize mot[{0}] register on address [{1}]",
                        rot, addressByte
                        ));
                    MainWindow.Ms[rot].ACTUALIZE_register(addressByte, byteValue, type);
                    addressByte++;
                }
            }
        }

        
        public static bool GET_motorRotFromId(int id, out e_rot rot)
        {
            if (id == C_DynAdd.ID_BROADCAST)
            {
                rot = e_rot.yaw;
                return false;
            }
            else
            {
                foreach (C_Motor mot in M)
                {
                    if (mot.id == id)
                    {
                        rot = mot.rotMotor;
                        return true;
                    }
                }
                rot = e_rot.yaw;
                return false;
            }
        }
    }
}
