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

        private BasicEffect skySurfaceEffect;
        private Texture2D skySurfaceTexture;
        private GeometricPrimitive skySurfacePrimitive;


        protected void Draw_SkySurface(GameTime _gameTime)
        {
            skySurfaceEffect.Projection = eyeProjection;
            skySurfaceEffect.View = eyeView;
            skySurfaceEffect.World = Matrix.Identity;

            skySurfacePrimitive.Draw(skySurfaceEffect);
        }
        public void Constructor_SkySurface()
        {
        }
        public void LoadContent_SkySurface()
        {
            // size of imaginary camera picture plane [mm]
            float sizeX = 100; // [mm]
            float sizeY = 100; // [mm]
            skySurfacePrimitive = ToDisposeContent(GeometricPrimitive.Cylinder.New(GraphicsDevice, sizeX, sizeY));

            // Load the texture
            //cameraTexture = Content.Load<Texture2D>("speaker");
            skySurfaceTexture = Content.Load<Texture2D>("grid");

            skySurfaceEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            skySurfaceEffect.Texture = skySurfaceTexture;
            skySurfaceEffect.TextureEnabled = true;
        }
        
    }
}
