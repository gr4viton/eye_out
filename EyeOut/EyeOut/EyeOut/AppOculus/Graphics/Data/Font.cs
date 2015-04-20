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

using System.Threading.Tasks;

namespace EyeOut_Telepresence
{
    using SharpDX.Toolkit.Graphics;
    /// <summary>
    /// Font part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {

        private SpriteFont arial13;
        private SpriteFont msSansSerif10;
        private SpriteFont arial16;
        private SpriteFont arial16ClearType;
        private SpriteFont defaultFont;
        private SpriteFont courrierNew10;
        private SpriteFont calibri64;
        private SpriteFont fontDefault;
        private Texture2D colorTexture;

        public string text;

        void LoadContent_Font()
        {
            // Load fonts
            arial13 = ToDisposeContent(Content.Load<SpriteFont>("Arial13"));
            msSansSerif10 = ToDisposeContent(Content.Load<SpriteFont>("MicrosoftSansSerif10"));
            arial16 = ToDisposeContent(Content.Load<SpriteFont>("Arial16"));
            arial16ClearType = ToDisposeContent(Content.Load<SpriteFont>("Arial16ClearType"));
            defaultFont = ToDisposeContent(Content.Load<SpriteFont>("Arial16Bold"));
            calibri64 = ToDisposeContent(Content.Load<SpriteFont>("Calibri64"));
            courrierNew10 = ToDisposeContent(Content.Load<SpriteFont>("CourierNew10"));
            fontDefault = ToDisposeContent(Content.Load<SpriteFont>("fontDefault"));
        }

        public void BeginDraw_Font()
        {
            ////            spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.FrontToBack,
            //    GraphicsDevice.BlendStates.NonPremultiplied);  // Use NonPremultiplied, as this sprite texture is not premultiplied
        }

        public void Draw_Font(int eye)
        {
            //MatrixOrthoSubProjection - no
            // GetTranslationMatrix 

            spriteBatch.Begin(SpriteSortMode.FrontToBack,
                GraphicsDevice.BlendStates.NonPremultiplied);  // Use NonPremultiplied, as this sprite texture is not premultiplied
            // Render the text
            //var text = String.Format("{0}{0}{0}", "Hello World - EyeOut is comming to town!\n");
            if (text == null)
            {
                text = String.Format("{0}{0}{0}", "Hello World - EyeOut is comming to town!\n");
            }

            //var dim = fontDefault.MeasureString(text);
            var dim = fontDefault.MeasureString(text);
            int diff = -452; // for DK1
            int x = 620 + eye * diff, y = 1020; // for DK1
            spriteBatch.Draw(colorTexture, new SharpDX.Rectangle(x, y, (int)dim.X, (int)dim.Y), SharpDX.Color.White);
            spriteBatch.DrawString(fontDefault, text, new SharpDX.Vector2(x, y), SharpDX.Color.Black);


            // Update the FPS text
            frameCount++;
            if (fpsClock.ElapsedMilliseconds > 1000.0f)
            {
                fpsText = string.Format("{0:F2} FPS", (float)frameCount * 1000 / fpsClock.ElapsedMilliseconds);
                frameCount = 0;
                fpsClock.Restart();
            }

            //spriteBatch.DrawString(courrierNew10, "Measured: " + dim, new SharpDX.Vector2(x, y + dim.Y + 5), SharpDX.Color.GreenYellow);


            //spriteBatch.DrawString(defaultFont, "  " + SpriteCount + "\nSprites", new Vector2(spriteSceneWidth - 32, spriteSceneHeight - 24), Color.White);
            spriteBatch.DrawString(fontDefault, fpsText, new Vector2(x, y + 200), Color.Black);

            //spriteBatch.DrawString(arial16, "text", new SharpDX.Vector2(470, 420), SharpDX.Color.White, 0.0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0.0f);

            //text = "Rendering test\nRotated On Center";
            //spriteBatch.DrawString(arial16, text, new SharpDX.Vector2(600, 120), SharpDX.Color.White, -(float)gameTime.TotalGameTime.TotalSeconds, new Vector2(dim.X / 2.0f, dim.Y / 2.0f), 1.0f, SpriteEffects.None, 0.0f);

            Draw_Geometrics();

            spriteBatch.End();
        }

        public void Draw_Geometrics()
        {

            //// Render each primitive
            //for (int i = 0; i < primitives.Count; i++)
            //{
            //    var primitive = primitives[i];

            //    // Calculate the translation
            //    float dx = ((i + 1) % 4);
            //    float dy = ((i + 1) / 4);

            //    float x = (dx - 1.5f) * 1.7f;
            //    float y = 1.0f - 2.0f * dy;

            //    var time = (float)gameTime.TotalGameTime.TotalSeconds + i;

            //    // Setup the World matrice for this primitive
            //    basicEffect.World = Matrix.Scaling((float)Math.Sin(time * 1.5f) * 0.2f + 1.0f) * Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f) * Matrix.Translation(x, y, 0);

            //    // Render the name of the primitive
            //    spriteBatch.DrawString(defaultFont, primitive.Name, new Vector2(GraphicsDevice.BackBuffer.Width * (0.08f + dx / 4.0f), GraphicsDevice.BackBuffer.Height * (0.47f + dy / 2.2f)), Color.White);

            //    // Disable Cull only for the plane primitive, otherwise use standard culling
            //    GraphicsDevice.SetRasterizerState(i == 0 ? GraphicsDevice.RasterizerStates.CullNone : GraphicsDevice.RasterizerStates.CullBack);

            //    // Draw the primitive using BasicEffect
            //    primitive.Draw(basicEffect);
            //}
        }
        public void EndDraw_Font()
        {
            //spriteBatch.End();
        }
    }

}
