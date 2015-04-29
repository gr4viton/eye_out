using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

using SharpOVR; // PoseF
using EyeOut;


namespace EyeOut_Telepresence
{
    //class MotorPosture
    //{
        
    /// <summary>
    /// MotorPosture part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {

        
        //public C_CounterDown readPosition = new C_CounterDown(10);
        int iReadMotor = 0;
        public void CONTROL_motors()
        {
            POSITION_motors();

            if (config.ReadCameraStream == true)
            {
                CAPTURE_cameraImage();
            }
            else
            {
                STOP_streaming();
            }
            if (config.READ_dataFromMotors == true)
            {

                //MainWindow.Ms.Yaw.READ_position();
                //readPosition(
                //foreach (C_Motor mot in MainWindow.Ms)
                //{
                //    mot.READ_positionSpeed();
                //}

                if (C_State.FURTHER(e_stateProg.initialized))
                {
                    MainWindow.Ms[iReadMotor].READ_positionSpeed();

                    iReadMotor++;
                    if (iReadMotor >= 3)
                    {
                        iReadMotor = 0;
                    }
                }
            }
        }



        public void POSITION_motors()
        {
            // background worker - if it takes too long

            Update_PlayerFromHmd(0);

            if (config.WRITE_dataToMotors == true)
            {
                MainWindow.Ms.SYNC_WRITE_moveSpeed();
            }
        }

        public void Update_MotorWantedAnglesFromRobotArmWantedAngles()
        {
            foreach (C_Motor mot in MainWindow.Ms)
            {
                mot.angleWanted.Dec_FromDefault = C_Value.CONV_rad2deg(ra[mot.rotMotor]);
                if (config.motorSpeedControl == true)
                {
                    mot.speedWanted.Dec = mot.speedWanted.DecMax;
                }
                else
                {
                    mot.speedWanted.Dec = C_DynVal.SET_MOV_SPEED_NOCONTROL;
                }
            }
        }

    }
}
