using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SharpDX;
using SharpDX.Direct3D11;

using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

using System.ComponentModel; // description
using System.Reflection; // fieldInfo  - description
using EyeOut;

using SharpOVR;

namespace EyeOut_Telepresence
{
    public class PostureF
    {
        private PoseF poseF;
        public PoseF PoseF
        {
            get{return poseF;}
            set{poseF = value;}
        }
        public Quaternion Orientation
        {
            get { return poseF.Orientation; }
            set { poseF.Orientation = value; }
        }
        public Vector3 Position
        {
            get { return poseF.Position; }
            set { poseF.Position = value; }
        }

        public Matrix Rotation
        {
            get
            {
                return Orientation.GetMatrix();
            }
            set
            {
                Orientation = Quaternion.RotationMatrix(value);
                //Orientation = value * Orientation ;
            }
        }

        public Vector3 YawPitchRoll
        {
            get
            {
                Orientation.GetEulerAngles(out yawPitchRoll[0], out yawPitchRoll[1], out yawPitchRoll[2]);
                return new Vector3(yawPitchRoll);
            }
            set
            {
                Rotation = Matrix.RotationYawPitchRoll( value[0], value[1], value[2] );
            }
        }

        private float[] yawPitchRoll;


        public PostureF()
        {
            yawPitchRoll = new float[3];
        }

        public static Vector3 CONV_RotationMatrix_2_YawPitchRollVector3(Matrix rotation)
        {
            var yawPitchRoll = new float[3];
            Quaternion.RotationMatrix(rotation).GetEulerAngles(out yawPitchRoll[0], out yawPitchRoll[1], out yawPitchRoll[2]);
            return new Vector3(yawPitchRoll);
        }
    }

    public class Player : ICloneable // based on Rastertek Terrain Tutorials in SharpDX 
    {
        #region Structures / Enums
        private enum Movement
        {
            Forward,
            Backward,
            Upward,
            Downward,
            SidestepLeft,
            SidestepRight,
            LeftTurn,
            RightTurn,
        }
        #endregion
        #region Properties / Variables
        public float FrameTime { get; set; }

        public PostureF scout; // where is the scout relative to body default position - when free moving around in scene
        public PostureF hmd; // where the user with hmd is looking at and standing relative to oculus camera head pose tracking
        public PostureF body; // default position and look in scene - for north setting 

        //public float[] hmdYawPitchRoll;

        public Matrix Rotation
        {
            get
            {
                return body.Rotation * hmd.Rotation;
            }
        }

        public Vector3 YawPitchRoll
        {
            get
            {
                return PostureF.CONV_RotationMatrix_2_YawPitchRollVector3(Rotation);
            }
        }

        public Vector3 Position
        {
            get
            {
                return body.Position + hmd.Position + scout.Position;
            }
        }

        //public float posX { get { return playerPosition[0]; } set { playerPosition[0] = value; } }
        //public float posY { get { return playerPosition[1]; } set { playerPosition[1] = value; } }
        //public float posZ { get { return playerPosition[2]; } set { playerPosition[2] = value; } }

        //public float rotX { get { return playerRotation[0]; } set { playerRotation[0] = value; } }
        //public float rotY { get { return playerRotation[1]; } set { playerRotation[1] = value; } }
        //public float rotZ { get { return playerRotation[2]; } set { playerRotation[2] = value; } }


        private Matrix forward = Matrix.Translation(Vector3.UnitZ);
        private Matrix rightward = Matrix.Translation(Vector3.UnitX);
        private Matrix upward = Matrix.Translation(Vector3.UnitY);

        private float forwardSpeed, backwardSpeed;
        private float upwardSpeed, downwardSpeed;

        public float hmd_angleX { get { return hmd.YawPitchRoll[1]; } }
        public float hmd_angleY { get { return hmd.YawPitchRoll[0]; } }
        public float hmd_angleZ { get { return hmd.YawPitchRoll[2]; } }

        

        public float acceleration = 0.5f;
        public float accelerationNegative = 0.7f;
        public float actualVelocityMax = 0.03f;
        public float velocityMaxSlow = 0.001f;
        public float velocityMaxNormal = 0.03f;
        public float velocityMaxFast = 0.3f;
        #endregion

        #region Public Methods

        public Player()
        {
            body = new PostureF()
            {
                Rotation = Matrix.RotationYawPitchRoll((float)Math.PI, 0, 0)
            };

            hmd = new PostureF();
            scout = new PostureF();
            //hmdYawPitchRoll = new float[3];
        }


        public void ResetPosition()
        {
            scout.Position = new Vector3(0, 0, 0);
        }

        public void ResetBodyRotationY()
        {
            scout.Rotation = Matrix.RotationY(hmd_angleY);
        }
        


        public void UPDATE_hmdPosture(PoseF hmdPoseF)
        {
            UPDATE_hmdPosition(hmdPoseF.Position);
            UPDATE_hmdOrientation(hmdPoseF.Orientation);
        }

        public void UPDATE_hmdPosition(Vector3 hmdPosition)
        {
            hmd.Position = hmdPosition;
        }
        
        public void UPDATE_hmdOrientation(Quaternion Q)
        {
            hmd.Orientation = Q;
            //Q.GetEulerAngles(out hmdYawPitchRoll[0], out hmdYawPitchRoll[1], out hmdYawPitchRoll[2]);
            //hmd.Rotation = Matrix.RotationYawPitchRoll(
            //     hmdYawPitchRoll[0], 
            //     hmdYawPitchRoll[1], 
            //     hmdYawPitchRoll[2]  
            //    );
        }


        public void SetupSpeed(bool speedKeyDown, bool slowKeyDown)
        {
            if (speedKeyDown)
                actualVelocityMax = velocityMaxFast;
            else if (slowKeyDown)
                actualVelocityMax = velocityMaxSlow;
            else
                actualVelocityMax = velocityMaxNormal;
        }

        public void Move(float speed, Matrix directionFromHmdView )
        {
            scout.Position += (Rotation * directionFromHmdView * speed).TranslationVector;
        }
        public void MoveForward(bool keydown)
        {
            // Update the forward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                forwardSpeed += FrameTime * acceleration;
                if (forwardSpeed > FrameTime * actualVelocityMax)
                    forwardSpeed = FrameTime * actualVelocityMax;
            }
            else
            {
                forwardSpeed -= FrameTime * accelerationNegative;
                if (forwardSpeed < 0)
                    forwardSpeed = 0;
            }

            Move(forwardSpeed, forward);
        }

        public void MoveBackward(bool keydown)
        {
            // Update the backward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                backwardSpeed += FrameTime * acceleration;
                if (backwardSpeed > FrameTime * actualVelocityMax)
                    backwardSpeed = FrameTime * actualVelocityMax;
            }
            else
            {
                backwardSpeed -= FrameTime * accelerationNegative;
                if (backwardSpeed < 0)
                    backwardSpeed = 0;
            }

            Move(backwardSpeed, -forward);
        }

        public void MoveSideStep(bool keydown, bool _right)
        {
            // Update the forward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                forwardSpeed += FrameTime * acceleration;
                if (forwardSpeed > FrameTime * actualVelocityMax)
                    forwardSpeed = FrameTime * actualVelocityMax;
            }
            else
            {
                forwardSpeed -= FrameTime * accelerationNegative;
                if (forwardSpeed < 0)
                    forwardSpeed = 0;
            }

            if (_right)
                Move(forwardSpeed, rightward);
            else
                Move(forwardSpeed, -rightward);

        }

        public void MoveUpward(bool keydown)
        {
            // Update the upward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                upwardSpeed += FrameTime * acceleration;
                if (upwardSpeed > FrameTime * actualVelocityMax)
                    upwardSpeed = FrameTime * actualVelocityMax;
            }
            else
            {
                upwardSpeed -= FrameTime * accelerationNegative;
                if (upwardSpeed < 0)
                    upwardSpeed = 0;
            }

            // Update the height position.
            scout.Position += new Vector3(0,0,upwardSpeed);
        }

        public void MoveDownward(bool keydown)
        {
            // Update the upward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                downwardSpeed += FrameTime * acceleration;
                if (downwardSpeed > FrameTime * actualVelocityMax)
                    downwardSpeed = FrameTime * actualVelocityMax;
            }
            else
            {
                downwardSpeed -= FrameTime * accelerationNegative;
                if (downwardSpeed < 0)
                    downwardSpeed = 0;
            }

            // Update the height position.
            scout.Position -= new Vector3(0, 0, upwardSpeed);
        }


        #endregion

        #region Override Methods
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
