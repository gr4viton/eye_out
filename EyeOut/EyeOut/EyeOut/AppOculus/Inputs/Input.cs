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
    
    public enum e_stateSPI
    {
        [Description("No port avalible")]
        noPortAvailible,
        [Description("Not connected")]
        disconnected = 0,
        [Description("Connecting")]
        connecting,
        [Description("Connected")]
        connected //,sending, recieving
    }
    public enum e_ModifierFunction
    {
        
        [Description("")] none = 0,
        [Description("Alt")] alt = 1,
        [Description("Ctrl")] control = 2,
        [Description("Shft")] shift = 3,
        [Description("Supr")] super = 4
    }
    public class ModifierAndKeyFunction
    {
        public e_ModifierFunction modifier;
        private Keys key;
        public string name;
        public object function; // function pointer --> returns object -> using toString
        public object val; // changed everytime when calling function -> function returns bool / object

        private ModifierAndKeyFunction()
        {

        }
    }

    
    public class GroupedKeyControl
    {
        private Keys key;
        public string name;
        private ModifierAndKeyFunction[] modificatorFunctions;

        public ModifierAndKeyFunction none;
        public ModifierAndKeyFunction alt;
        public ModifierAndKeyFunction control;
        public ModifierAndKeyFunction shift;
        

        public const string modifierChars = "nothing|[!]alt|[^]ctrl|[+]shift"; //|[#]super";
                

        public GroupedKeyControl(Keys _key, string _name)
        {
            name = _name;
            key = _key;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(string.Format("[{0}]{1}: ", key, name));
            foreach(ModifierAndKeyFunction modFu in modificatorFunctions)
            {
                str.Append(string.Format("[{0}]{1}={2}|", 
                    EnumGetDescription.GetDescription(modFu.modifier), 
                    modFu.name, 
                    modFu.val.ToString()
                    ));
            }

            return str.ToString();
        }
    }
    /// <summary>
    /// Input part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {

        private KeyboardManager keyboardManager; // we will process keyboard input here
        //private readonly KeyboardManager keyboardManager; // we will process keyboard input here

        private KeyboardState keyboardState;
        
        void Constructor_Input()
        {
        }

        void Update_Input()
        {
            // update keyboard state
            //keyboardState = new KeyboardState();
            keyboardManager.Update(gameTime);
            //keyboardManager = new KeyboardManager(this);
            keyboardState = keyboardManager.GetState();

            // if Esc is pressed - quit program
            if (keyboardState.IsKeyPressed(Keys.Escape))
            {
                Exit();
                return;
            }

            HUD.AppendLine( string.Format(
                "W{0}|A{1}|S{2}", 
                keyboardState.IsKeyDown(Keys.W), 
                keyboardState.IsKeyDown(Keys.A), 
                keyboardState.IsKeyDown(Keys.S)
                ));

            config.player.position.FrameTime = gameTime.ElapsedGameTime.Milliseconds;

            config.player.position.MoveForward(keyboardState.IsKeyDown(Keys.W));
            config.player.position.TurnLeft(keyboardState.IsKeyDown(Keys.A));
            config.player.position.MoveBackward(keyboardState.IsKeyDown(Keys.S));
            config.player.position.TurnRight(keyboardState.IsKeyDown(Keys.D));
            config.player.position.MoveUpward(keyboardState.IsKeyDown(Keys.Q));
            config.player.position.MoveDownward(keyboardState.IsKeyDown(Keys.E));

            config.player.position.SetupSpeed(keyboardState.IsKeyDown(Keys.Shift));

            //if (keyboardState.IsKeyPressed(Keys.W)) { config.player.position.MoveForward(true); }
            //if (keyboardState.IsKeyPressed(Keys.A)) { config.player.position.TurnLeft(true); }
            //if (keyboardState.IsKeyPressed(Keys.S)) { config.player.position.MoveBackward(true); }
            //if (keyboardState.IsKeyPressed(Keys.D)) { config.player.position.TurnRight(true); }
            //if (keyboardState.IsKeyPressed(Keys.Q)) { config.player.position.MoveUpward(true); }
            //if (keyboardState.IsKeyPressed(Keys.E)) { config.player.position.MoveDownward(true); }

            if (keyboardState.IsKeyPressed(Keys.K))
            {
                tiles[3].PlayDelegate();
                return;
            }


            if (keyboardState.IsKeyPressed(Keys.J))
            {
                tiles[3].StopDelegate();
                return;
            }

            if (keyboardState.IsKeyPressed(Keys.Q))
            {
                tiles[4].PlayDelegate();
                return;
            }


            if (keyboardState.IsKeyPressed(Keys.P))
            {
                tiles[4].StopDelegate();
                return;
            }


            if (keyboardState.IsKeyPressed(Keys.H) && keyboardState.IsKeyDown(Keys.Control))
            {
                //config.READ_dataFromMotors ^= true; // toggle
                config.SHOW_helpText = !config.SHOW_helpText;
                return;
            }

            if (keyboardState.IsKeyPressed(Keys.M) && keyboardState.IsKeyDown(Keys.Control))
            {
                //config.READ_dataFromMotors ^= true; // toggle
                config.WRITE_dataToMotors = !config.WRITE_dataToMotors;
                return;
            }

            if (keyboardState.IsKeyPressed(Keys.M) && keyboardState.IsKeyDown(Keys.Shift))
            {
                //config.READ_dataFromMotors ^= true; // toggle
                config.READ_dataFromMotors = !config.READ_dataFromMotors;
                return;
            }

            List<Keys> keys = new List<Keys>();
            keyboardState.GetDownKeys(keys);
            //foreach (var key in keys)
            //    sb.AppendFormat("Key: {0}, Code: {1}\n", key, (int)key);

            // numer keys (NOT numpad ones) have name like D0, D1, etc...
            // associate available modes each with its key
            //for (int i = 0; i < availableModes.Count; i++)
            //{
            //    var key = (Keys)Enum.Parse(typeof(Keys), "D" + i);
            //    if (keyboardState.IsKeyPressed(key))
            //    {
            //        ApplyMode(availableModes[i]);
            //        return;
            //    }
            //}
        }
    }


    public class Player : ICloneable // Rastertek Terrain Tutorials in SharpDX 
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

        private float forwardSpeed, backwardSpeed;
        private float upwardSpeed, downwardSpeed;
        private float leftTurnSpeed, rightTurnSpeed;
        private float lookUpSpeed, lookDownSpeed;

            
        public float acceleration = 0.001f;
        public float accelerationNegative = 0.07f;
        public float actualVelocityMax = 0.003f;
        public float velocityMaxSlow = 0.003f;
        public float velocityMaxFast = 0.03f;
        #endregion

        #region Public Methods
        public void SetupSpeed(bool keydown)
        {
            if(keydown)
                actualVelocityMax = velocityMaxFast;
            else
                actualVelocityMax = velocityMaxSlow;
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

            // Convert degrees to radians.
            var radians = rotationY * 0.0174532925f;

            // Update the position.
            positionX += (float)Math.Sin(radians) * forwardSpeed;
            positionZ += (float)Math.Cos(radians) * forwardSpeed;
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

            // Convert degrees to radians.
            var radians = rotationY * 0.0174532925f;

            // Update the position.
            positionX -= (float)Math.Sin(radians) * backwardSpeed;
            positionZ -= (float)Math.Cos(radians) * backwardSpeed;
        }

        public void MoveUpward(bool keydown)
        {
            // Update the upward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                upwardSpeed += FrameTime * 0.003f;
                if (upwardSpeed > FrameTime * actualVelocityMax)
                    upwardSpeed = FrameTime * actualVelocityMax;
            }
            else
            {
                upwardSpeed -= FrameTime * 0.0002f;
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
                downwardSpeed += FrameTime * 0.003f;
                if (downwardSpeed > FrameTime * actualVelocityMax)
                    downwardSpeed = FrameTime * actualVelocityMax;
            }
            else
            {
                downwardSpeed -= FrameTime * 0.0002f;
                if (downwardSpeed < 0)
                    downwardSpeed = 0;
            }

            // Update the height position.
            positionY -= downwardSpeed;
        }

        public void TurnLeft(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera turns left. If not slow down the turn speed.
            if (keydown)
            {
                leftTurnSpeed += FrameTime * 0.01f;
                if (leftTurnSpeed > FrameTime * 0.15f)
                    leftTurnSpeed = FrameTime * 0.15f;
            }
            else
            {
                leftTurnSpeed -= FrameTime * 0.005f;
                if (leftTurnSpeed < 0)
                    leftTurnSpeed = 0;
            }

            // Update the rotation using the turning speed.
            rotationY -= leftTurnSpeed;

            // Keep the rotation in the 0 to 360
            if (rotationY < 0)
                rotationY += 360;
        }

        public void TurnRight(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera turns right. If not slow down the turn speed.
            if (keydown)
            {
                rightTurnSpeed += FrameTime * 0.01f;
                if (rightTurnSpeed > FrameTime * 0.15)
                    rightTurnSpeed = FrameTime * 0.15f;
            }
            else
            {
                rightTurnSpeed -= FrameTime * 0.005f;
                if (rightTurnSpeed < 0)
                    rightTurnSpeed = 0;
            }

            // Update the rotation using the turning speed.
            rotationY += rightTurnSpeed;

            // Keep the rotation in the range 0 to 360 range.
            if (rotationY > 360)
                rotationY -= 360;
        }

        public void LookUpward(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera turns up. If not slow down the turn speed.
            if (keydown)
            {
                lookUpSpeed += FrameTime * 0.01f;
                if (lookUpSpeed > FrameTime * 0.15)
                    lookUpSpeed = FrameTime * 0.15f;
            }
            else
            {
                lookUpSpeed -= FrameTime * 0.005f;
                if (lookUpSpeed < 0)
                    lookUpSpeed = 0;
            }

            // Update the rotation using the turning speed.
            rotationX -= lookUpSpeed;

            // Keep the rotation maximum 90 degrees.
            if (rotationX > 90)
                rotationX = 90;
        }

        public void LookDownward(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera turns down. If not slow down the turn speed.
            if (keydown)
            {
                lookDownSpeed += FrameTime * 0.01f;
                if (lookDownSpeed > FrameTime * 0.15)
                    lookDownSpeed = FrameTime * 0.15f;
            }
            else
            {
                lookDownSpeed -= FrameTime * 0.005f;
                if (lookDownSpeed < 0)
                    lookDownSpeed = 0;
            }

            // Update the rotation using the turning speed.
            rotationX += lookDownSpeed;

            // Keep the rotation maximum 90 degrees
            if (rotationX < -90)
                rotationX = -90;
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
