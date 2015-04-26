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

        private float positionX, positionY, positionZ;
        private float rotationX, rotationY, rotationZ;

        private float bodyRotationX, bodyRotationY, bodyRotationZ; 

        private float forwardSpeed, backwardSpeed;
        private float upwardSpeed, downwardSpeed;


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
            //bodyRotationY = (float)Math.PI;
            bodyRotationY = (float)Math.PI;
        }

        public void SetPosition(float x, float y, float z)
        {
            positionX = x;
            positionY = y;
            positionZ = z;
        }
        public void SetPosition(Vector3 position)
        {
            SetPosition(position.X, position.Y, position.Z);
        }

        public void SetRotation(float x, float y, float z)
        {
            rotationX = x;
            rotationY = y;
            rotationZ = z;
        }
        public void SetRotation(Vector3 rotation)
        {
            SetRotation(rotation.X, rotation.Y, rotation.Z);
        }


        public Vector3 GetPosition()
        {
            return new Vector3(positionX, positionY, positionZ);
        }
        public void GetPosition(out float x, out float y, out float z)
        {
            x = positionX;
            y = positionY;
            z = positionZ;
        }

        public Vector3 GetRotation()
        {
            return new Vector3(rotationX, rotationY, rotationZ);
        }
        public void GetRotation(out float x, out float y, out float z)
        {
            x = rotationX;
            y = rotationY;
            z = rotationZ;
        }

        public float GetBodyRotationY()
        {
            return bodyRotationY;
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
            positionX += (float)Math.Sin(rotationY) * forwardSpeed;
            positionZ += (float)Math.Cos(rotationY) * forwardSpeed;
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
            positionX -= (float)Math.Sin(rotationY) * backwardSpeed;
            positionZ -= (float)Math.Cos(rotationY) * backwardSpeed;
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
            positionX -= (float)Math.Sin(rotationY + coef) * forwardSpeed;
            positionZ -= (float)Math.Cos(rotationY + coef) * forwardSpeed;
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
            positionY += upwardSpeed;
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
            positionY -= downwardSpeed;
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
