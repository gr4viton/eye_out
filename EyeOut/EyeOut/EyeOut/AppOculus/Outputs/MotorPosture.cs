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

        public void CONTROL_motors()
        {
            ORDER_motors();
            if (config.READ_dataFromMotors == true)
            {
                //MainWindow.Ms.Yaw.READ_position();

                //foreach (C_Motor mot in MainWindow.Ms)
                //{
                //    mot.ORDER_getPosition();
                //}
            }
        }
        public void ORDER_motors()
        {
            // background worker - if it takes too long
            //maxRepeats--;
            //if (maxRepeats < 1)
            {
                //maxRepeats = 10;
                int ieye = 0;

                //Matrix ori = renderPose[ieye].Orientation.GetMatrix();
                //Matrix ori = renderPose[ieye].Orientation.Axis;

                //Vector3 pos = renderPose[ieye].Position;
                //float[] posArr = pos.ToArray();

                StringBuilder msg = new StringBuilder();
                int q = 0;
                /*
                for(q=0;q<3;q++)
                {
                    msg.AppendLine(string.Format("{0}\t{1}\t{2}", ori[q+0], ori[q+1], ori[q+2]));
                }*/

                //float yaw, pitch, roll;
                float[] yawPitchRoll = new float[3];

                //var pose = renderPose[(int)eye] = hmd.GetHmdPosePerEye(eye); 
                Quaternion Q = hmd.GetHmdPosePerEye((SharpOVR.EyeType)ieye).Orientation; // .GetEulerAngles(out yawPitchRoll[0], out yawPitchRoll[1], out yawPitchRoll[2]);
                    //Quaternion a;
                //a.Angle


                

                StringBuilder str = new StringBuilder();
                Q.GetEulerAngles(out yawPitchRoll[0], out yawPitchRoll[1], out yawPitchRoll[2]);

                double[] yawPitchRoll_d = new double[3];
                for (q = 0; q < 3; q++)
                {
                    yawPitchRoll_d[q] = C_Value.CONV_rad2deg(((double)yawPitchRoll[q]));
                    //yawPitchRoll_d[q] = (double)yawPitchRoll[q] * C_Value.rad2deg;
                    yawPitchRoll_d[q] = (double)yawPitchRoll[q] * 57.0;
                }


                str.AppendLine(string.Format("X[{0}]Y[{1}]Z[{2}]W[{3}]", Q.X, Q.Y, Q.Z, Q.W));
                str.AppendLine(string.Format("Angle[{0}] Axis[{1}]", Q.Angle, Q.Axis));
                str.AppendLine(string.Format("YawPitchRoll[rad] [{0,7:0.0000}|{1,7:0.0000}|{2,7:0.0000}]",
                    yawPitchRoll[0], yawPitchRoll[1], yawPitchRoll[2]
                    ));
                //str.AppendLine(string.Format("YawPitchRoll[rad] [{0,7:0.0000}|{1,7:0.0000}|{2,7:0.0000}]",
                //    (double)(yawPitchRoll[0]), (double)(yawPitchRoll[1]), (double)(yawPitchRoll[2])
                //    ));

                //str.AppendLine(string.Format("YawPitchRoll[deg] [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                //    (double)(yawPitchRoll[0]) * 57, (double)(yawPitchRoll[1]) * C_Value.deg2rad, (double)(yawPitchRoll[2]) * (double)57
                //    ));
                //float pif = 3.141592f;
                //double pid = (double)pif;
                //str.AppendLine(string.Format("{0}f == {1}d", pif,pid ));
                str.AppendLine(string.Format("YawPitchRoll[deg] [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                    yawPitchRoll_d[0], yawPitchRoll_d[1], yawPitchRoll_d[2]
                    ));
                //str.AppendLine(string.Format("{0}|{1}", C_Value.rad2deg, C_Value.deg2rad   ));
                

                text = str.ToString();
                //q = 0;
                //msg.Append(string.Format("YAW={0,5:0.00}°\tPITCH={1,5:0.00}°\tROLL={2,5:0.00}°", yawPitchRoll_d[q + 0], yawPitchRoll_d[q + 1], yawPitchRoll_d[q + 2]));


                //LOG(msg.ToString());


                if (config.WRITE_dataToMotors == true)
                {
                    foreach (C_Motor mot in MainWindow.Ms)
                    {
                        mot.angleWanted.Dec_FromDefault = yawPitchRoll_d[(int)mot.rotMotor];
                        //mot.speedWanted.Dec = mot.speedWanted.DecMax;
                        mot.speedWanted.Dec = C_DynVal.SET_MOV_SPEED_NOCONTROL;
                        mot.REGISTER_move();
                    }
                    C_Motor.ORDER_ActionToAll();
                }

            }
        }

    }
}
