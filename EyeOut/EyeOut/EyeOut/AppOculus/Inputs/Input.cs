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

        private readonly KeyboardManager keyboardManager; // we will process keyboard input here

        private KeyboardState keyboardState;
        
        void Constructor_Input()
        {
        }

        void Update_Input()
        {
            // update keyboard state
            keyboardState = keyboardManager.GetState();

            // if Esc is pressed - quit program
            if (keyboardState.IsKeyPressed(Keys.Escape))
            {
                Exit();
                return;
            }


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

            if (keyboardState.IsKeyPressed(Keys.T))
            {
                //cameraImage.Description.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                cameraImage.Description.Format = cameraImage.Description.Format + 1;
                return;
            }

            if (keyboardState.IsKeyPressed(Keys.Y))
            {
                //cameraImage.Description.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                cameraImage.Description.Format = cameraImage.Description.Format -1;
                return;
            }
            if (keyboardState.IsKeyPressed(Keys.U))
            {
                //cameraImage.Description.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                cameraImage.Description.Format = cameraImage.Description.Format + 3;
                return;
            }

            if (keyboardState.IsKeyPressed(Keys.I))
            {
                //cameraImage.Description.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                cameraImage.Description.Format = cameraImage.Description.Format -3;
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

}
