using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

using SharpDX.Toolkit;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;


namespace EyeOut_Telepresence
{
    using SharpDX.Toolkit.Graphics;
    /// <summary>
    /// Sprite part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {

        //private PrimitiveBatch<VertexPositionColor> primitiveBatch;
        //private BasicEffect primitiveBatchEffect;

        private SpriteBatch fontSpriteBatch;

        void LoadContent_Sprite()
        {
            // Instantiate a SpriteBatch
            fontSpriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            //colorTexture = ToDisposeContent(Texture2D.New(GraphicsDevice, 1, 1, PixelFormat.R8G8B8A8.UNorm, new[] { Color.White}));
            colorTexture = ToDisposeContent(Texture2D.New(GraphicsDevice, 1, 1, PixelFormat.R8G8B8A8.UNorm, new[] { new Color(255,255,255,128) }));
        }

        void UnloadContent_Sprites()
        {
            Utilities.Dispose(ref fontSpriteBatch);
            //Utilities.Dispose(ref primitiveBatch);
            //Utilities.Dispose(ref primitiveBatchEffect);

        }
    }
}
