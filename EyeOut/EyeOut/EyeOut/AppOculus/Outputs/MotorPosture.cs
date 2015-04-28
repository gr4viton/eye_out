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
            // background worker - if it takes too long

            UpdateFromHmd(0);

            int q = 0;
            double[] yawPitchRoll_d = new double[3];
            for (q = 0; q < 3; q++)
            {
                yawPitchRoll_d[q] = C_Value.CONV_rad2deg(((double)config.player.hmd.YawPitchRoll[q]));
            }

            HUD.AppendLine(string.Format("YawPitchRoll[deg] [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                yawPitchRoll_d[0], yawPitchRoll_d[1], yawPitchRoll_d[2]
                ));

            HUD.AppendLine("[!]alt|[^]ctrl|[+]shift|[#]super");
            HUD.AppendLine(string.Format("Control: [^M]otor={0}|", config.WRITE_dataToMotors));
            HUD.AppendLine(string.Format("Read: [+M]otor={0}|[+C]amera={1}", config.READ_dataFromMotors, config.ReadCameraStream));
            HUD.AppendLine(string.Format("PositionLock: [Tab]={0}|[numbers]={1}:{2}", 
                config.player.PositionLockActive, 
                (int)config.player.PositionLock, config.player.PositionLock
                ));
            HUD.AppendLine("");

            Vector3 pos;
            Vector3 rot;

            pos = config.player.scout.Position;
            HUD.AppendLine(string.Format("scout [XYZ]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                pos[0], pos[1], pos[2]
                ));
            rot = config.player.body.YawPitchRoll;
            HUD.AppendLine(string.Format("body YawPitchRoll [Yaw|Pitch|Roll]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                rot[0], rot[1], rot[2]
                ));

            pos = config.player.hmd.Position;
            HUD.AppendLine(string.Format("hmd [XYZ]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                pos[0], pos[1], pos[2]
                ));
            rot = config.player.hmd.YawPitchRoll;
            HUD.AppendLine(string.Format("hmd [Yaw|Pitch|Roll]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                rot[0], rot[1], rot[2]
                ));

            pos = config.player.Position;
            HUD.AppendLine(string.Format("Player [XYZ]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                pos[0],pos[1],pos[2]
                ));
            rot = config.player.YawPitchRoll;
            HUD.AppendLine(string.Format("Player [Yaw|Pitch|Roll]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                rot[0], rot[1], rot[2]
                ));




            //str.Append(string.Format("cam position:{0}|{1}, keyboardStateW={2}", cameraSurfaceX, cameraSurfaceY, keyboardState.IsKeyPressed(Keys.U)));


            if (config.WRITE_dataToMotors == true)
            {

                foreach (C_Motor mot in MainWindow.Ms)
                {
                    mot.angleWanted.Dec_FromDefault = yawPitchRoll_d[(int)mot.rotMotor];
                    mot.speedWanted.Dec = mot.speedWanted.DecMax;
                    //mot.speedWanted.Dec = C_DynVal.SET_MOV_SPEED_NOCONTROL;
                }
                MainWindow.Ms.SYNC_WRITE_moveSpeed();
            }
        }

    }
}
