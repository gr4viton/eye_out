
using SharpDX;
using SharpDX.Direct3D11;

using SharpDX.Toolkit;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

using System.Threading.Tasks;

using System.Diagnostics; // StopWatch

namespace EyeOut_Telepresence
{
    using SharpDX.Toolkit.Graphics;
    /// <summary>
    /// FPS part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {


        private int frameCountDirectX;
        private string fpsText;
        private readonly Stopwatch fpsClock;

        void Constructor_FPS()
        {
            // Variable used for FPS
            fpsText = string.Empty;
        }

        void BeginDraw_FPS()
        {
            // Starts the FPS clock
            fpsClock.Start();
        }
    }
}
