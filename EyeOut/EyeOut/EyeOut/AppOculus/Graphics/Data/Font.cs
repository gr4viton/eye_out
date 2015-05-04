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
using System.Globalization; // cultureinfo time tostring


namespace EyeOut_Telepresence
{

    using SharpDX.Toolkit.Graphics;


    public class HUD
    {
        private StringBuilder str;

        public byte backgroundAlpha = 128;

        public string text
        {
            get{
                return str.ToString();
            }
        }

        public void Append(string msg)
        {
            str.Append(msg);
        }
        public void AppendLine(string msg)
        {
            str.AppendLine(msg);
        }

        public void Clear()
        {
            str = new StringBuilder();
        }
        public HUD()
        {
            str = new StringBuilder();
        }
    }


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

        private DateTime timeStartedStreaming = DateTime.Now;

        float fpsDirectX = 0;
        float fpsBaslerCamera = 0;

        //public string text;

        public HUD HUD;

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


            HUD = new HUD();
        }

        public void BeginDraw_Font()
        {
            ////            spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.FrontToBack,
            //    GraphicsDevice.BlendStates.NonPremultiplied);  // Use NonPremultiplied, as this sprite texture is not premultiplied
        }

        public void Draw_Font(int eye)
        {
            fontSpriteBatch.Begin(SpriteSortMode.FrontToBack,
                GraphicsDevice.BlendStates.NonPremultiplied);  // Use NonPremultiplied, as this sprite texture is not premultiplied
            // Render the text

            var dim = fontDefault.MeasureString(HUD.text);
            //int diff = -452; // for DK1
            int diff = 0*152; // for DK2
            int x = 620 + eye * diff, y = 820; // for DK1

            if (config.hud.helpMenu == true)
            {
                fontSpriteBatch.Draw(colorTexture, new SharpDX.Rectangle(x, y, (int)dim.X, (int)dim.Y), SharpDX.Color.White);

                fontSpriteBatch.DrawString(fontDefault, HUD.text, new SharpDX.Vector2(x, y), SharpDX.Color.Black);
            }

            // Update the FPS text
            frameCountDirectX++;
            if (fpsClock.ElapsedMilliseconds > 1000.0f)
            {
                fpsDirectX = (float)frameCountDirectX * 1000 / fpsClock.ElapsedMilliseconds;
                fpsBaslerCamera = (float)config.cameraControl.frameCount * 1000 / fpsClock.ElapsedMilliseconds;

                frameCountDirectX = 0;
                config.cameraControl.frameCount = 0;
                fpsClock.Restart();
            }


            TimeSpan timeInterval = DateTime.Now.Subtract( timeStartedStreaming );
                //DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            fpsText = string.Format("FPS:[DirectX|BaslerCamera]:[{0:F1}|{1:F1}] Hz\nCaptureTime[{2}]|LocalTime[{3}]",
                    fpsDirectX, fpsBaslerCamera,
                    timeInterval.ToString(),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    );

            if (config.hud.toolStrip == true)
            {
                string toolStripText;
                if (config.cameraArtificialDelay == true)
                {
                    toolStripText = string.Format("ArtificialDelay: Frames[{0}]={1,9:F4} [ms] | timeFromXkeyPressed[{2}]", 
                        config.cameraFrameQueueLength, qAct.TotalMilliseconds,
                        DateTime.Now - measurementStart
                        );
                }
                else
                {
                    toolStripText = string.Format("ArtificialDelay: None | timeFromXkeyPressed[{0}]",
                        DateTime.Now - measurementStart
                        );
                }
                fontSpriteBatch.DrawString(fontDefault, toolStripText, new Vector2(x, y - 50), Color.White);
            }
            if (config.hud.timeStrip == true)
            {
                fontSpriteBatch.DrawString(fontDefault, fpsText, new Vector2(x, y - 100), Color.White);
            }

            // toolstrip info - zkratky vysvětleni ikon
            // keybindings
            // compas
            // statuses
            
            fontSpriteBatch.End();
        }

        public void EndDraw_Font()
        {
            //spriteBatch.End();
            HUD.Clear();
        }
    }

}
