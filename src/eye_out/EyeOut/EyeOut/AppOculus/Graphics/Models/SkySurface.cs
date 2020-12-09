using System;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using SharpOVR;
using System.Diagnostics; // StopWatch

using System.Collections.Generic; // list

using SharpDX.Toolkit;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

using System.Threading.Tasks;

using EyeOut;

using System.Runtime.InteropServices; // marshal

using BaslerImage = Basler.Pylon.IImage;
using ToolkitImage = SharpDX.Toolkit.Graphics.Image;
//using ToolkitTexture = SharpDX.Toolkit.Graphics.Texture2D;
//using SharpDX.Toolkit.Graphics;

namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;

    /// <summary>
    /// Sky surface
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        private Model modelSky;

        protected void Draw_SkySurface(GameTime _gameTime)
        {

            if (config.draw.SkySurface == true)
            {
                var world = Matrix.Scaling(4000f)
                            * Matrix.RotationY(0)
                            * Matrix.Translation(config.player.Position)
                            ;

                modelSky.Draw(GraphicsDevice, world, eyeView, eyeProjection);
            }
        }

        public void LoadContent_SkySurface()
        {
            try
            {
                modelSky = Content.Load<Model>("skybox/untitled");
            }
            catch
            {
                LOG_err("Could not load 'skybox/untitled'");
            }
            
            // Enable default lighting on model.
            BasicEffect.EnableDefaultLighting(modelSky, true);
        }
    }
}
