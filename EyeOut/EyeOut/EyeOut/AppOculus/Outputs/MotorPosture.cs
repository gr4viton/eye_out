﻿using System;
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
            LOG("start order motors");
            // background worker - if it takes too long
            int ieye = 0;
            int q = 0;
            float[] yawPitchRoll = new float[3];
            double[] yawPitchRoll_d = new double[3];
            StringBuilder str = new StringBuilder();

            LOG("order motors - get quaternion of orientation");
            //var pose = renderPose[(int)eye] = hmd.GetHmdPosePerEye(eye); 
            Quaternion Q = hmd.GetHmdPosePerEye((SharpOVR.EyeType)ieye).Orientation;

            LOG("order motors - get euler angles");
            Q.GetEulerAngles(out yawPitchRoll[0], out yawPitchRoll[1], out yawPitchRoll[2]);


            config.player.SetRotation(
                 yawPitchRoll[1] , // pitch
                 yawPitchRoll[0] , // yaw
                 yawPitchRoll[2]  // roll
                );


            for (q = 0; q < 3; q++)
            {
                yawPitchRoll_d[q] = C_Value.CONV_rad2deg(((double)yawPitchRoll[q]));
            }

            LOG("order motors - build message");
            HUD.AppendLine(string.Format("YawPitchRoll[deg] [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                yawPitchRoll_d[0], yawPitchRoll_d[1], yawPitchRoll_d[2]
                ));

            HUD.AppendLine("[!]alt|[^]ctrl|[+]shift|[#]super");
            HUD.AppendLine(string.Format("Motor: [^M]Control={0}|[+M]Read={1}", config.WRITE_dataToMotors, config.READ_dataFromMotors));

            Vector3 pos = config.player.GetPosition();
            HUD.AppendLine(string.Format("Player position [Fwd|Up|Right]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                pos[0],pos[1],pos[2]
                ));
            Vector3 rot = config.player.GetRotation();
            HUD.AppendLine(string.Format("Player rotation [Fwd|Up|Right]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                rot[0], rot[1], rot[2]
                ));

            //str.Append(string.Format("cam position:{0}|{1}, keyboardStateW={2}", cameraSurfaceX, cameraSurfaceY, keyboardState.IsKeyPressed(Keys.U)));


            if (config.WRITE_dataToMotors == true)
            {
                LOG("order motors - write data to motors");

                foreach (C_Motor mot in MainWindow.Ms)
                {
                    mot.angleWanted.Dec_FromDefault = yawPitchRoll_d[(int)mot.rotMotor];
                    mot.speedWanted.Dec = mot.speedWanted.DecMax;
                    //mot.speedWanted.Dec = C_DynVal.SET_MOV_SPEED_NOCONTROL;
                }
                MainWindow.Ms.SYNC_WRITE_moveSpeed();
                LOG("order motors - write data to motors end");
            }
            LOG("order motors - end");
        }

    }
}
