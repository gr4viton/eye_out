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
        private C_CounterDown skySurfaceTextureIndex;

        private GeometricPrimitive skySurfacePrimitive;
        private List<Texture2D> skySurfaceTextureBatch;

        private Model modelSkyDome;

        protected void Draw_SkySurface(GameTime _gameTime)
        {
            skySurfaceEffect.Projection = eyeProjection;
            skySurfaceEffect.View = eyeView;
            skySurfaceEffect.World = Matrix.Identity;

            //GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullFront);
            //skySurfacePrimitive.Draw(skySurfaceEffect);
            //GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullBack);


            var world = Matrix.Scaling(300f)
                        * Matrix.RotationY(0)
                        ;

            modelSkyDome.Draw(GraphicsDevice, world, eyeView, eyeProjection);
        }
        //public void Constructor_SkySurface()
        //{
        //}

        public int SkySurface_SetNextSurface()
        {
            // returns looping index of next surface
            skySurfaceEffect.Texture = skySurfaceTextureBatch[ skySurfaceTextureIndex.Val ];
            return skySurfaceTextureIndex.DecrementAndRestart();
        }
        public void LoadContent_SkySurface()
        {
            // size of skySurface [mm]
            float sizeX = 1000; // [mm]
            //float sizeY = 1000; // [mm]
            //skySurfacePrimitive = ToDisposeContent(GeometricPrimitive.Cylinder.New(GraphicsDevice, sizeX, sizeY));
            skySurfacePrimitive = ToDisposeContent(GeometricPrimitive.Cube.New(GraphicsDevice, sizeX));

            // Load the texture
            //cameraTexture = Content.Load<Texture2D>("speaker");
            skySurfaceTextureBatch = new List<Texture2D>();
            string path = "skybox";
            skySurfaceTextureIndex = new C_CounterDown(4);
            for (int q = 0; q <= skySurfaceTextureIndex.ValDef; q++)
            {
                path = string.Format("skybox/skybox{0}", q);
                try
                {
                    skySurfaceTextureBatch.Add(ToDisposeContent(Content.Load<Texture2D>(path)));
                }
                catch
                {
                    LOG_err("Could not load '" + path + "'");
                }
            }
            

            
            skySurfaceEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            skySurfaceEffect.Texture = skySurfaceTextureBatch[skySurfaceTextureIndex.Val];
            skySurfaceEffect.TextureEnabled = true;
            //skySurfaceEffect.
            //    // Disable Cull only for the plane primitive, otherwise use standard culling
            //    GraphicsDevice.SetRasterizerState(i == 0 ? GraphicsDevice.RasterizerStates.CullNone : GraphicsDevice.RasterizerStates.CullBack);


            // Load a 3D model
            //modelSkyDome = Content.Load<Model>("skybox/skyboxBlend");
            modelSkyDome = Content.Load<Model>("skybox/untitled");

            // Enable default lighting on model.
            BasicEffect.EnableDefaultLighting(modelSkyDome, true);

        }
    }
}
