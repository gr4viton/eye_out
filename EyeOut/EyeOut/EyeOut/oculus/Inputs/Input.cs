using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace EyeOut_TP
{
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

            if (keyboardState.IsKeyPressed(Keys.A))
            {
                tiles[3].PlayDelegate();
                return;
            }


            if (keyboardState.IsKeyPressed(Keys.S))
            {
                tiles[3].StopDelegate();
                return;
            }

            if (keyboardState.IsKeyPressed(Keys.Q))
            {
                tiles[4].PlayDelegate();
                return;
            }


            if (keyboardState.IsKeyPressed(Keys.W))
            {
                tiles[4].StopDelegate();
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
