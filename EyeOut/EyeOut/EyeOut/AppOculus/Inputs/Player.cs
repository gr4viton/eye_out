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
                
                //Orientation.GetEulerAngles(out yawPitchRoll[0], out yawPitchRoll[1], out yawPitchRoll[2]);
                //return Matrix.RotationYawPitchRoll( yawPitchRoll[1], yawPitchRoll[2], yawPitchRoll[0] );
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

        //public static Vector3 CONV_RotationMatrix_2_AngleXYZVector3(Matrix rotation)
        //{
        //    var element = new float[3];
        //    Quaternion.RotationMatrix(rotation).GetEulerAngles(out element[2], out element[0], out element[1]);
        //    return new Vector3(element);
        //}
    }

    public enum e_positionLock 
    {
        cameraSensor = 0,
        desk = 1
    }
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class Player : ICloneable // based on Rastertek Terrain Tutorials in SharpDX 
    {
        //#region Structures / Enums
        //private enum Movement
        //{
        //    Forward,
        //    Backward,
        //    Upward,
        //    Downward,
        //    SidestepLeft,
        //    SidestepRight,
        //    LeftTurn,
        //    RightTurn,
        //}
        //#endregion
        #region Properties / Variables
        public float FrameTime { get; set; }

        public PostureF scout; // where is the scout relative to body default position - when free moving around in scene
        public PostureF hmd; // where the user with hmd is looking at and standing relative to oculus camera head pose tracking
        public PostureF body; // default position and look in scene - for north setting 

        //public float[] hmdYawPitchRoll;
        public bool PositionLockActive = false; 
        // if true the scout position is always adjusted to the actualPosition updated from outside 
        // - also the internal control of scout position by user is disabled
        public e_positionLock PositionLock;

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
          
        public Matrix LookAtRH
        {
            get
            {
                var finalRotation = Rotation;
                var finalUp = finalRotation.Transform(Vector3.UnitY); 
                var finalForward = finalRotation.Transform(-Vector3.UnitZ);
                var shiftedEyePos = Position;
                return Matrix.LookAtRH(shiftedEyePos, shiftedEyePos + finalForward, finalUp);
            }
        }
        //public float posX { get { return playerPosition[0]; } set { playerPosition[0] = value; } }
        //public float posY { get { return playerPosition[1]; } set { playerPosition[1] = value; } }
        //public float posZ { get { return playerPosition[2]; } set { playerPosition[2] = value; } }

        //public float rotX { get { return playerRotation[0]; } set { playerRotation[0] = value; } }
        //public float rotY { get { return playerRotation[1]; } set { playerRotation[1] = value; } }
        //public float rotZ { get { return playerRotation[2]; } set { playerRotation[2] = value; } }


        private Matrix forward = Matrix.Translation(Vector3.UnitX);
        private Matrix rightward = Matrix.Translation(Vector3.UnitX);
        private Matrix upward = Matrix.Translation(Vector3.UnitY);

        private float forwardSpeed, backwardSpeed;
        public float upwardSpeed, downwardSpeed;

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
            body = new PostureF();
            //{
            //    Rotation = Matrix.RotationYawPitchRoll((float)Math.PI, 0, 0)
            //};
            body.Rotation = Matrix.RotationYawPitchRoll(0.0001f, 0, 0);

            hmd = new PostureF();
            scout = new PostureF();
            //hmdYawPitchRoll = new float[3];
        }


        public void ResetPositionAndBodyYaw()
        {
            ResetPosition();
            ResetBodyYaw();
        }
        

        public void ResetPosition()
        {
            scout.Position = new Vector3(0, 0, 0);
        }

        public void ResetBodyYaw()
        {
            body.Rotation = Matrix.RotationY(  - hmd.YawPitchRoll[0]);//Matrix.RotationY(hmd_angleY);
            //TelepresenceSystem.LOG(hmd.YawPitchRoll[0].ToString());
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



        public void Move(float speed, Vector3 vector)
        {
            MoveAbsolute(speed, Rotation.Transform(vector));
        }

        public void MoveAbsolute(float speed, Vector3 absoluteVector)
        {
            if (PositionLockActive == false)
            {
                scout.Position += speed * absoluteVector; // *new Vector3(1, 1, 1);
            }
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

            //scout.Position -= Rotation.Forward * forwardSpeed;
            Move(forwardSpeed, Vector3.ForwardRH);
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

            Move(backwardSpeed, Vector3.BackwardRH);
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
                Move(forwardSpeed, Vector3.Right);
            else
                Move(forwardSpeed, Vector3.Left);

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
            MoveAbsolute(upwardSpeed, Vector3.Up);
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
            MoveAbsolute(downwardSpeed, Vector3.Down);
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
