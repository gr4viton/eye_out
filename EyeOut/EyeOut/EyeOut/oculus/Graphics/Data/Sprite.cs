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


namespace EyeOut_TP
{
    using SharpDX.Toolkit.Graphics;
    /// <summary>
    /// Sprite part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {

        private PrimitiveBatch<VertexPositionColor> primitiveBatch;
        private BasicEffect primitiveBatchEffect;

        private SpriteBatch spriteBatch;

        void LoadContent_Sprite()
        {
            // Instantiate a SpriteBatch
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));
            colorTexture = ToDisposeContent(Texture2D.New(GraphicsDevice, 1, 1, PixelFormat.R8G8B8A8.UNorm, new[] { Color.White }));
        }

        void UnloadContent_Sprites()
        {
            Utilities.Dispose(ref spriteBatch);
            Utilities.Dispose(ref primitiveBatch);
            Utilities.Dispose(ref primitiveBatchEffect);

        }
    }
}
