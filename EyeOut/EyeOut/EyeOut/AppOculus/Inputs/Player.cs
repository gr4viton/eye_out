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

    public class Player : ICloneable // based on Rastertek Terrain Tutorials in SharpDX 
    {
        #region Structures / Enums
        private enum Movement
        {
            Forward,
            Backward,
            Upward,
            Downward,
            LeftTurn,
            RightTurn,
            LookUp,
            LookDown
        }
        #endregion
        #region Properties / Variables
        public float FrameTime { get; set; }

        private Vector3 playerPosition;
        private Vector3 playerRotation;
        private Vector3 bodyRotation;

        public Vector3 Position
        {
            get
            {
                return playerPosition;
            }
            set
            {
                playerPosition = value;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return playerRotation + bodyRotation;
            }
            set
            {
                playerRotation = value ;
            }
        }
        public float posX { get { return playerPosition[0]; } set { playerPosition[0] = value; } }
        public float posY { get { return playerPosition[1]; } set { playerPosition[1] = value; } }
        public float posZ { get { return playerPosition[2]; } set { playerPosition[2] = value; } }

        public float rotX { get { return playerRotation[0]; } set { playerRotation[0] = value; } }
        public float rotY { get { return playerRotation[1]; } set { playerRotation[1] = value; } }
        public float rotZ { get { return playerRotation[2]; } set { playerRotation[2] = value; } }


        private float forwardSpeed, backwardSpeed;
        private float upwardSpeed, downwardSpeed;

        public PoseF lastPose;
        public float angleX { get { return hmdYawPitchRoll[1]; } }
        public float angleY { get { return hmdYawPitchRoll[0]; } }
        public float angleZ { get { return hmdYawPitchRoll[2]; } }
        public float[] hmdYawPitchRoll;

        

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
            bodyRotation = new Vector3(0 , (float)Math.PI , 0);
            playerPosition = new Vector3(0, 0, 0);
            playerRotation = new Vector3(0, 0, 0);
            //bodyRotationY = 0f;
            hmdYawPitchRoll = new float[3];
        }

        public void ResetPosition()
        {
            playerPosition = new Vector3(0, 0, 0);
        }

        public void ResetBodyRotationY()
        {
            bodyRotation[1] = angleY + (float)Math.PI;
        }

        public void UPDATE_hmdOrientation(Quaternion Q)
        {
            Q.GetEulerAngles(out hmdYawPitchRoll[0], out hmdYawPitchRoll[1], out hmdYawPitchRoll[2]);
            Rotation = new Vector3(
                 hmdYawPitchRoll[1], // pitch
                 hmdYawPitchRoll[0], // yaw
                 hmdYawPitchRoll[2]  // roll
                );
        }
        
        
        public float GetBodyRotationY()
        {
            return bodyRotation[1];
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

            // Update the position.
            posX += (float)Math.Sin(rotY) * forwardSpeed;
            posZ += (float)Math.Cos(rotY) * forwardSpeed;
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


            // Update the position.
            posX -= (float)Math.Sin(rotY) * backwardSpeed;
            posZ -= (float)Math.Cos(rotY) * backwardSpeed;
        }

        public void MoveSideStep(bool keydown, bool right)
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

            float coef;
            if (right)
                coef = +(float)Math.PI/2;
            else
                coef = -(float)Math.PI/2;

            // Update the position.
            posX -= (float)Math.Sin(rotY + coef) * forwardSpeed;
            posZ -= (float)Math.Cos(rotY + coef) * forwardSpeed;
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
            posY += upwardSpeed;
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
            posY -= downwardSpeed;
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
