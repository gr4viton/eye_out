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
    //using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    //using SharpDX.DXGI;
    /// <summary>
    /// Sound part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        //private SpriteBatch spriteBatch;
        //private SpriteFont arial16BMFont;
        //private PrimitiveBatch<VertexPositionColor> primitiveBatch;
        //private BasicEffect primitiveBatchEffect;
        private BasicEffect geometryEffect;
        private GeometricPrimitive cube;
        private Texture2D listenerTexture;
        private Texture2D emitterTexture;
        private Random random;


        private AudioManager audioManager;

        private SoundEffect ergonWave;
        private SoundEffectInstance ergonWaveInstance;

        private SoundEffect beep;
        private SoundEffectInstance beepInstance;

        private WaveBank waveBank;
        //private WaveBank waveBankXbox; //does not play correctly
        Matrix listener;
        Vector3 listenerVelocity;
        Matrix emitter;
        Vector3 emitterVelocity;
        SoundEffectInstance audio3DEffectInstance;

        private List<SoundTile> tiles;
        private bool play3D;
        private bool isMusicPlaying;



        void Constructor_Sound()
        {
            audioManager = new AudioManager(this);
            audioManager.EnableMasterVolumeLimiter();
        }

        void LoadContent_Sound()
        {

            beep = Content.Load<SoundEffect>("button-42");
            beepInstance = beep.Create();

            // Load Sounds
            ergonWave = Content.Load<SoundEffect>("ergon.adpcm");
            ergonWaveInstance = ergonWave.Create();
            ergonWaveInstance.IsLooped = true;
            waveBank = Content.Load<WaveBank>("TestBank");

            // setup tests
            tiles = new List<SoundTile>();
            Rectangle border = new Rectangle();
            border.X = SoundTile.Padding.X;
            border.Y = SoundTile.Padding.Y;

            AddTile(ref border, "Click to play looped SoundEffectInstance of " + ergonWave.Name, PlayMusic, PauseMusic);
            AddTile(ref border, "Click to play 'PewPew' wave bank entry", () => waveBank.Play("PewPew"));
            AddTile(ref border, "Click to play 'PewPew' wave bank entry with random pitch and pan", () => waveBank.Play("PewPew", 1, random.NextFloat(-1, 1), random.NextFloat(-1, 1)));
            AddTile(ref border, "Click to play 'PewPew' with 3D audio", PlayAudio3D, StopAudio3D);

            AddTile(ref border, "Click to play 'Button-42' ", beepInstance.Play, beepInstance.Stop);

            // setup 3D
            geometryEffect = ToDisposeContent(new BasicEffect(GraphicsDevice)
            {
                View = Matrix.LookAtRH(new Vector3(0, 10, 20), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            });

            cube = ToDisposeContent(GeometricPrimitive.Cube.New(GraphicsDevice));

            // Load the texture
            listenerTexture = Content.Load<Texture2D>("listen");
            emitterTexture = Content.Load<Texture2D>("speaker");
            geometryEffect.TextureEnabled = true;

        }

        void Update_Sound()
        {
            if (play3D)
                UpdateAudio3D(gameTime);

            //foreach (var tile in tiles)
            //    tile.BorderColor = Color.White;

            //if (pointerState.Points.Count > 0)
            //{
            //    var point = pointerState.Points[0];
            //    if (point.EventType == PointerEventType.Pressed)
            //    {
            //        var viewport = GraphicsDevice.Presenter.DefaultViewport;

            //        var pointerPosition = new Vector2(point.Position.X * viewport.Width, point.Position.Y * viewport.Height);

            //        foreach (var tile in tiles)
            //        {
            //            if (tile.Border.Contains(pointerPosition))
            //            {
            //                if (point.IsLeftButtonPressed)
            //                {
            //                    tile.BorderColor = Color.Green;
            //                    tile.Toggle();
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void AddTile(ref Rectangle border, string label, Action playDelegate, Action stopDelegate = null)
        {
            Vector2 labelSize;
            labelSize = defaultFont.MeasureString(label);
            border.Width = (int)(labelSize.X + SoundTile.Padding.X * 2);
            border.Height = (int)(labelSize.Y + SoundTile.Padding.Y * 2);
            tiles.Add(new SoundTile { Border = border, Label = label, PlayDelegate = playDelegate, StopDelegate = stopDelegate });
            border.Y = border.Bottom + SoundTile.Padding.Y;
        }



        private void PlayMusic()
        {
            if (isMusicPlaying)
                ergonWaveInstance.Resume();
            else
            {
                ergonWaveInstance.Play();
                isMusicPlaying = true;
            }
        }

        private void PauseMusic()
        {
            ergonWaveInstance.Pause();
        }

        private void EnableSpatialAudioWithReverb()
        {
            audioManager.EnableSpatialAudio();
            audioManager.EnableReverbEffect();
            audioManager.SetReverbEffectParameters(ReverbPresets.BathRoom);
            SoundEffect.DistanceScale = 10f; //needs a higher distance scale to hear reverb.
            audioManager.EnableReverbFilter();
        }

        private void PlayAudio3D()
        {
            if (play3D)
                return;

            EnableSpatialAudioWithReverb();

            listener = Matrix.LookAtRH(Vector3.Zero, new Vector3(0, 0, 8), Vector3.Up);
            listenerVelocity = Vector3.Zero;

            emitter = Matrix.LookAtRH(new Vector3(0, 0, 8), Vector3.Zero, Vector3.Up);
            emitterVelocity = Vector3.Zero;

            audio3DEffectInstance = waveBank.Create("PewPew");
            audio3DEffectInstance.IsLooped = true;
            audio3DEffectInstance.Apply3D(listener, listenerVelocity, emitter, emitterVelocity);
            audio3DEffectInstance.Play();
            play3D = true;
        }

        private void StopAudio3D()
        {
            if (!play3D) return;

            audio3DEffectInstance.Stop(true);
            audio3DEffectInstance.Dispose();
            audio3DEffectInstance = null;

            play3D = false;
        }

        private void UpdateAudio3D(GameTime gameTime)
        {
            var rotation = (float)gameTime.TotalGameTime.TotalSeconds * 2.0f;

            emitter = Matrix.LookAtRH(new Vector3(0, 0, 8), Vector3.Zero, Vector3.Up) * Matrix.RotationY(rotation);
            audio3DEffectInstance.Apply3D(listener, listenerVelocity, emitter, emitterVelocity);
        }


        class SoundTile
        {
            private bool isPlaying;

            public static Point Padding = new Point(4, 4);

            public Rectangle Border { get; set; }
            public Color BorderColor { get; set; }
            public string Label { get; set; }
            public Action PlayDelegate { get; set; }
            public Action StopDelegate { get; set; }

            public SoundTile()
            {
                BorderColor = Color.White;
            }

            public void Toggle()
            {
                if (isPlaying)
                {
                    if (StopDelegate != null)
                        StopDelegate();
                    isPlaying = false;
                }
                else
                {
                    if (PlayDelegate != null)
                        PlayDelegate();
                    isPlaying = StopDelegate != null;
                }
            }
        }
    }
}
