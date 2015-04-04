using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    // decide if the sent command should produce any returned message
    public enum e_cmdEchoType
    {
        noEcho = 0, echoLast = 1, presentPosition, presentSpeed //, presentLoad , ...
    }
    /// <summary>
    /// C_Motor - ORDERS etc.
    /// ORDER functions sends the data directly (INS_WRITE)
    /// REGISTER functions sends the data to register (INS_REG_WRITE)
    /// SETUP functions is called from both previous with the instruction as argument

    /// all functions send spi commands and wants to get echo - as setuped
    /// </summary>
    public partial class C_Motor
    {

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Other
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void ORDER_ping()
        {
            SEND_cmdInner(C_DynAdd.INS_PING);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Other ORDERs
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Action 
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void ORDER_Action(List<C_Motor> Ms)
        {
            foreach (C_Motor mot in Ms)
            {
                mot.ORDER_Action();
            }
        }

        public void ORDER_Action()
        {
            SEND_cmdInner(CREATE_cmdInnerFromBytes(new List<object> { 
                C_DynAdd.INS_ACTION 
                }));
        }

        // broadcasting
        public static void ORDER_ActionToAll()
        {
            SEND_BROADCAST_cmdInner(CREATE_cmdInnerFromBytes(new List<object> { 
                C_DynAdd.INS_ACTION 
                }));
            //LOG_mot("Broadcast to all motors: ACTION");
            /*
            angle.UPDATE_lastSent();
            speed.UPDATE_lastSent();
             */
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Action
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region SET move and speed 
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void ORDER_moveAllAtOnce()
        {
            //3-5-7 SYNC WRITE  - cannot read at the same time as it is broadcasted
        }
        // move with speed 
        public void ORDER_move()
        {
            SETUP_move(C_DynAdd.INS_WRITE);
        }

        public void REGISTER_move()
        {
            SETUP_move(C_DynAdd.INS_REG_WRITE);
        }

        public void SETUP_move(byte INSTRUCTION_BYTE)
        {
            if ((angle.Dec != angle.DecLast) || (speed.Dec != speed.DecLast))
            {
                SEND_cmdInner(CREATE_cmdInnerFromBytes(new List<object> { 
                    INSTRUCTION_BYTE, C_DynAdd.GOAL_POS_L, angle.Hex, speed.Hex 
                }));
                LOG_SETUP_moveSpeed(INSTRUCTION_BYTE, angle, speed);
            }
        }

        // move without speed control - does not change current speed in motor class instance
        public void ORDER_moveBrisk()
        {
            C_Value lastSpeed = speed;
            speed.Dec = C_DynAdd.SET_MOV_SPEED_NOCONTROL;
            ORDER_move();
            speed = lastSpeed;

            //SEND_cmdInner(CREATE_cmdInner(new List<object> { 
            //    C_DynAdd.INS_WRITE, C_DynAdd.GOAL_POS_L, angle.Hex, C_DynAdd.SET_MOV_SPEED_NOCONTROL
            //}));
        }

        public void LOG_SETUP_moveSpeed(byte INSTRUCTION_BYTE, C_Value _angle, C_Value _speed)
        {
            string prefix = "ODD_MOVE";
            switch (INSTRUCTION_BYTE)
            {
                case (C_DynAdd.INS_WRITE): prefix = "ORDER_move"; break;
                case (C_DynAdd.INS_REG_WRITE): prefix = "REGISTER_move"; break;
                case (C_DynAdd.INS_READ): prefix = "READ_move"; break;
            }


            if (_speed.Dec != C_DynAdd.SET_MOV_SPEED_NOCONTROL)
            {
                LOG(String.Format("{0}:[angle];[speed] =\t[{1}]; [{3}] =\t{2:0.00}°;\t{4:0.00}%, [angle-default]=\t{5:0.00}°",
                    prefix,
                    byteArray2strHex_space(_angle.Hex.Reverse().ToArray()), _angle.Dec,
                    byteArray2strHex_space(_speed.Hex.Reverse().ToArray()), _speed.Dec,
                    _angle.Dec_FromDefault
                    ));
            }
            else
            {
                LOG(String.Format("{0}: [angle];[speed] = [{1}];[{3}] = {2}°; No speed control",
                    prefix,
                    byteArray2strHex_space(_angle.Hex.Reverse().ToArray()), _angle.Dec,
                    byteArray2strHex_space(_speed.Hex.Reverse().ToArray())
                    ));
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion SET move and speed 
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region GET position
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void ORDER_getPosition()
        {
            SETUP_getPosition(C_DynAdd.INS_READ);
        }

        public void SETUP_getPosition(byte INSTRUCTION_BYTE)
        {
            byte BYTE_LENGTH = 2;
            SEND_cmdInner(e_cmdEchoType.presentPosition,
                CREATE_cmdInnerFromBytes(new List<object> { 
                    INSTRUCTION_BYTE, C_DynAdd.PRESENT_POS_L, BYTE_LENGTH
                }));
            //LOG_SETUP_moveSpeed(INSTRUCTION_BYTE, angle, speed);
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion GET position
    }
}
