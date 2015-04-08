using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
//using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using SharpDX.Windows;
//using SharpOVR;

using System.Windows.Forms;


//using SharpDX.Direct2D1; // text
//using SharpDX.DirectWrite; // text
//using SharpDX.DXGI;


//using AlphaMode = SharpDX.Direct2D1.AlphaMode;
//using Factory = SharpDX.Direct2D1.Factory;

namespace EyeOut
{
    /// <summary>
    /// EyeOut telepresence using SharpDX.Toolkit - text handling
    /// </summary>
    public partial class C_Telepresence : SharpDX.Toolkit.Game
    {

        /// <summary>
        /// Return the Handle to display to.
        /// </summary>
        protected IntPtr DisplayHandle
        {
            get
            {
                return ((System.Windows.Forms.Control)Window.NativeWindow).Handle;
            }
        }

        Rectangle fontDimension;
        Font font;

        int xDir = 1;
        int yDir = 1;
        string displayText;

        public void INIT_TP_text()
        {

            //// Initialize the Font
            //FontDescription fontDescription = new FontDescription()
            //{
            //    Height = 72,
            //    Italic = false,
            //    CharacterSet = FontCharacterSet.Ansi,
            //    FaceName = "Arial",
            //    MipLevels = 0,
            //    OutputPrecision = FontPrecision.TrueType,
            //    PitchAndFamily = FontPitchAndFamily.Default,
            //    Quality = FontQuality.ClearType,
            //    Weight = FontWeight.Bold
            //};
            
            ////font = new Font(device, fontDescription);

            // displayText = "Direct3D9 Text!";

            //// Measure the text to display
            //fontDimension = font.MeasureText(null, displayText, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), 
            //    FontDrawFlags.Center | FontDrawFlags.VerticalCenter);

        }
    }
}
