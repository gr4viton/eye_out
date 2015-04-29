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
using ToolkitTexture = SharpDX.Toolkit.Graphics.Texture2D;
//using SharpDX.Toolkit.Graphics;

namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;

    /// <summary>
    /// EyeOut telepresence using SharpDX.Toolkit
    /// 
    /// Individual methods:
    /// - Initialize	Called after the Game and GraphicsDevice are created, but before LoadContent. Reference page contains code sample. 
    /// - LoadContent	Loads the content.
    /// - Run	        Call this method to initialize the game, begin running the game loop, and start processing events for the game.
    ///     - BeginRun	    Called after all components are initialized but before the first update in the game loop. 
    ///     - Tick	        Updates the game's clock and calls Update and Draw. 
    ///         - Update    Called before BeginDraw - may contain pre-draw calculations
    ///         - BeginDraw	    Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw. 
    ///         - Draw          The drawing methods itself
    ///         - EndDraw	    Ends the drawing of a frame. This method is preceeded by calls to Draw and BeginDraw. 
    /// - EndRun	    Called after the game loop has stopped running before exiting. 
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;

        private Matrix eyeView;
        private Matrix eyeProjection;
        private Matrix eyeWorld;


        private Model modelAirplane;

        private HMD hmd;
        private Rect[] eyeRenderViewport;
        private D3D11TextureData[] eyeTexture;

        private RenderTarget2D renderTarget;
        private RenderTargetView renderTargetView;
        private ShaderResourceView renderTargetSRView;
        private DepthStencilBuffer depthStencilBuffer;
        private EyeRenderDesc[] eyeRenderDesc;
        private PoseF[] renderPose = new PoseF[2];

        
        //private Vector3 headPos = new Vector3(0f, 0f, 0f);
        //private float bodyYaw = 3.141592f;

        uint frameIndex = 0;

        private BasicEffect basicEffect;
        //private Texture2D primitiveTexture;

        public TelepresenceSystemConfiguration config;


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        /// <summary>
        /// Initializes a new instance of the <see cref="TelepresenceSystem" /> class.
        /// </summary>
        public TelepresenceSystem(TelepresenceSystemConfiguration _configuration)
        {
            // configuration
            config = _configuration;
            Constructor_BaslerCamera();
            
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Force no vsync and use real timestep to print actual FPS
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            
            Constructor_FPS();
            fpsClock = new Stopwatch(); // readonly


            // Input Constructor
            // all initial components should be created in game constructor
            keyboardManager = new KeyboardManager(this); // readonly
            Constructor_Input();

            Constructor_Sound();

            // Setup the relative directory to the executable directory 
            // for loading contents with the ContentManager
            Content.RootDirectory = @"Content\Demo";

            // Initialize OVR Library
            OVR.Initialize();

            // Create our HMD
            hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(HMDType.DK1);

            // Match back buffer size with HMD resolution
            graphicsDeviceManager.PreferredBackBufferWidth = hmd.Resolution.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = hmd.Resolution.Height;

        }


        
        protected override void Initialize()
        {
            //            INIT_TP_window();
            //            INIT_TP_renderTarget();
            //            INIT_TP_eyeTextureRendering();
            //            INIT_TP_d3d11();
            //            INIT_TP_hmd();

            //            base.Initialize();

            //            INIT_txu();
            //            //INIT_TP_text();

            // Modify the title of the window
            Window.Title = "RiftGame";

            // Attach HMD to window
            var control = (System.Windows.Forms.Control)Window.NativeWindow;
            hmd.AttachToWindow(control.Handle);

            // Create our render target
            var renderTargetSize = hmd.GetDefaultRenderTargetSize(1.5f);
            renderTarget = RenderTarget2D.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, 
                new MipMapCount(1), PixelFormat.B8G8R8X8.UNorm, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            //    new MipMapCount(1), PixelFormat.R8G8B8A8.UNorm, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            renderTargetView = (RenderTargetView)renderTarget;
            renderTargetSRView = (ShaderResourceView)renderTarget;

            // Create a depth stencil buffer for our render target
            depthStencilBuffer = DepthStencilBuffer.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, DepthFormat.Depth32, true);

            // Adjust render target size if there were any hardware limitations
            renderTargetSize.Width = renderTarget.Width;
            renderTargetSize.Height = renderTarget.Height;

            // The viewport sizes are re-computed in case renderTargetSize changed
            eyeRenderViewport = new Rect[2];
            eyeRenderViewport[0] = new Rect(0, 0, renderTargetSize.Width / 2, renderTargetSize.Height);
            eyeRenderViewport[1] = new Rect((renderTargetSize.Width + 1) / 2, 0, eyeRenderViewport[0].Width, eyeRenderViewport[0].Height);

            // Create our eye texture data
            eyeTexture = new D3D11TextureData[2];
            eyeTexture[0].Header.API = RenderAPIType.D3D11;
            eyeTexture[0].Header.TextureSize = renderTargetSize;
            eyeTexture[0].Header.RenderViewport = eyeRenderViewport[0];
            eyeTexture[0].pTexture = ((SharpDX.Direct3D11.Texture2D)renderTarget).NativePointer;
            eyeTexture[0].pSRView = renderTargetSRView.NativePointer;

            // Right eye uses the same texture, but different rendering viewport
            eyeTexture[1] = eyeTexture[0];
            eyeTexture[1].Header.RenderViewport = eyeRenderViewport[1];

            // Configure d3d11
            var device = (SharpDX.Direct3D11.Device)GraphicsDevice;
            D3D11ConfigData d3d11cfg = new D3D11ConfigData();
            d3d11cfg.Header.API = RenderAPIType.D3D11;
            d3d11cfg.Header.BackBufferSize = hmd.Resolution;
            d3d11cfg.Header.Multisample = 1;
            d3d11cfg.pDevice = device.NativePointer;
            d3d11cfg.pDeviceContext = device.ImmediateContext.NativePointer;
            d3d11cfg.pBackBufferRT = ((RenderTargetView)GraphicsDevice.BackBuffer).NativePointer;
            d3d11cfg.pSwapChain = ((SharpDX.DXGI.SwapChain)GraphicsDevice.Presenter.NativePresenter).NativePointer;

            // Configure rendering
            eyeRenderDesc = new EyeRenderDesc[2];
            if (!hmd.ConfigureRendering(d3d11cfg, DistortionCapabilities.Chromatic | DistortionCapabilities.TimeWarp, hmd.DefaultEyeFov, eyeRenderDesc))
            //if (!hmd.ConfigureRendering(d3d11cfg, DistortionCapabilities.None, hmd.DefaultEyeFov, eyeRenderDesc))
            //if (!hmd.ConfigureRendering(d3d11cfg, DistortionCapabilities.TimeWarp, hmd.DefaultEyeFov, eyeRenderDesc))
            {
                throw new Exception("Failed to configure rendering");
            }

            // Set enabled capabilities
            hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;

            // Configure tracking
            hmd.ConfigureTracking(TrackingCapabilities.Orientation | TrackingCapabilities.Position | TrackingCapabilities.MagYawCorrection, TrackingCapabilities.None);

            // Dismiss the Heatlh and Safety Window
            hmd.DismissHSWDisplay();

            // Get HMD output
            var adapter = (Adapter)GraphicsDevice.Adapter;
            var hmdOutput = adapter.Outputs.FirstOrDefault(o => hmd.DeviceName.StartsWith(o.Description.DeviceName, StringComparison.OrdinalIgnoreCase));
            if (hmdOutput != null)
            {
                // Set game to fullscreen on rift
                var swapChain = (SwapChain)GraphicsDevice.Presenter.NativePresenter;
                var description = swapChain.Description.ModeDescription;
                swapChain.ResizeTarget(ref description);
                swapChain.SetFullscreenState(true, hmdOutput);
            }

            //GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullBack);
            GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullFront);


            base.Initialize();
        }
        #region LoadContent
        protected override void LoadContent()
        {
            LoadContent_Sprite();
            LoadContent_Airplane();
            LoadContent_Font();
            LoadContent_Sound();
            LoadContent_BaslerCamera();
            LoadContent_RoboticArm();
            LoadContent_SkySurface();

            base.LoadContent();

            tiles[4].PlayDelegate();
        }

        void LoadContent_Airplane()
        {
            // Load a 3D model
            // The [Ship.fbx] file is defined with the build action [ToolkitModel] in the project
            modelAirplane = Content.Load<Model>("Ship");

            // Enable default lighting on model.
            BasicEffect.EnableDefaultLighting(modelAirplane, true);
        }

        #endregion LoadContent

        #region Update

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculates the world and the view based on the model size
            eyeView = Matrix.LookAtRH(new Vector3(0.0f, 0.0f, 7.0f), new Vector3(0, 0.0f, 0), Vector3.UnitY);
            eyeProjection = Matrix.PerspectiveFovRH(0.9f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);
            eyeWorld = Matrix.Identity;

            Update_Sound();
            Update_Input();

            Update_ScoutPosition();
            SETUP_eyeRender(0);
            //Update_PlayerFromHmd(0);
            
            Update_RobotArmWantedAnglesFromPlayer();
            Update_MotorWantedAnglesFromRobotArmWantedAngles();
            Update_RoboticArmDrawnPosture();
            

            Update_helpText();
        }
        #endregion Update


        public void Update_ScoutPosition()
        {
            if (config.player.PositionLockActive)
            {
                if (config.player.PositionLock == e_positionLock.cameraSensor)
                {
                    config.player.scout.Position = ra[e_RoboticArmPart.t_DE].effect.World.TranslationVector;
                }
            }
        }

        public void Update_helpText()
        {

            HUD.AppendLine("[!]alt|[^]ctrl|[+]shift|[#]super");
            HUD.AppendLine(string.Format("Control: [^M]otor={0}|", config.WRITE_dataToMotors));
            HUD.AppendLine(string.Format("Read: [+M]otor={0}|[+C]amera={1}", config.READ_dataFromMotors, config.ReadCameraStream));
            HUD.AppendLine(string.Format("PositionLock: [Tab]={0}|[numbers]={1}:{2}",
                config.player.PositionLockActive,
                (int)config.player.PositionLock, config.player.PositionLock
                ));
            HUD.AppendLine(string.Format("RoboticArmUpdatingFromAngle: [F5,F6,F7]={0}",
                ra.angleType
                ));
            HUD.AppendLine("");
            HUD.AppendLine(string.Format("Status: [Camera={0}][Motors={1}][SPI={2}]",
                C_State.baslerCam,
                C_State.mot,
                C_State.Spi
                ));
            HUD.AppendLine(string.Format("MotorSpeedControl: [M]{0}", config.motorSpeedControl)); 
            

            // may be in config constructor
            string name = "No camera connected - please connect the camera and restart telepresence!";
            if(C_State.FURTHER(e_stateBaslerCam.initializing))
            {
                name = config.streamController.Camera.CameraInfo[Basler.Pylon.CameraInfoKey.DeviceType] + " " +
                    config.streamController.Camera.CameraInfo[Basler.Pylon.CameraInfoKey.ModelName]
                    ;
                HUD.AppendLine("Camera: " + name);
                HUD.AppendLine(string.Format(
                    "StreamControler: [Init={0}]",
                    config.streamController.IsInitialized
                    ));
                HUD.AppendLine(string.Format(
                    "ImageViewer: [Init={0}]",
                    config.ImageViewer.IsInitialized
                    ));
                HUD.AppendLine(string.Format(
                    "Camera: [Connected={0}], [Open={1}], [Grabbing={2}]",
                    config.ImageViewer.Camera.IsConnected, config.ImageViewer.Camera.IsOpen, 
                    config.ImageViewer.Camera.StreamGrabber.IsGrabbing
                    ));
            }

            HUD.AppendLine("");
            Vector3 pos;
            Vector3 rot;

            pos = config.player.scout.Position;
            HUD.AppendLine(string.Format("scout [XYZ]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                pos[0], pos[1], pos[2]
                ));
            rot = config.player.body.YawPitchRoll;
            HUD.AppendLine(string.Format("body YawPitchRoll [Yaw|Pitch|Roll]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                rot[0], rot[1], rot[2]
                ));

            pos = config.player.hmd.Position;
            HUD.AppendLine(string.Format("hmd [XYZ]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                pos[0], pos[1], pos[2]
                ));
            rot = config.player.hmd.YawPitchRoll;
            HUD.AppendLine(string.Format("hmd [Yaw|Pitch|Roll]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                rot[0], rot[1], rot[2]
                ));

            pos = config.player.Position;
            HUD.AppendLine(string.Format("Player [XYZ]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                pos[0], pos[1], pos[2]
                ));
            rot = config.player.YawPitchRoll;
            HUD.AppendLine(string.Format("Player [Yaw|Pitch|Roll]: [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                rot[0], rot[1], rot[2]
                ));
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);
            if (disposeManagedResources)
            {
                // Release the HMD
                hmd.Dispose();

                // Shutdown the OVR Library
                OVR.Shutdown();
            }            
        }
        
        protected override void UnloadContent()
        {
            UnloadContent_Sprites();
            base.UnloadContent();
        }




        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.oculus, _msg);
        }

        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.oculus_err, _msg);
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }
}




        //public void INIT_toolkit(HMDType _hmdType) // called from constructor of class C_Telepresence
        //{
        //    // Creates a graphics manager. This is mandatory.
        //    LOG("Creating Graphics Manager");
        //    graphicsDeviceManager = new GraphicsDeviceManager(this);

        //    // Setup the relative directory to the executable directory 
        //    // for loading contents with the ContentManager
        //    Content.RootDirectory = "Content\\Demo";

        //    // Initialize OVR Library
        //    LOG("Initializing OVR Library");
        //    OVR.Initialize();

        //    // Create our HMD
        //    LOG("Creating HMD control");
        //    hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(_hmdType);

        //    // Match back buffer size with HMD resolution
        //    graphicsDeviceManager.PreferredBackBufferWidth = hmd.Resolution.Width;
        //    graphicsDeviceManager.PreferredBackBufferHeight = hmd.Resolution.Height;
        //}

        //protected void INIT_TP_window()
        //{
        //    // Modify the title of the window
        //    Window.Title = "EyeOut Telepresence";

        //    // Attach HMD to window
        //    LOG("Attaching HMD to window");
        //    var control = (System.Windows.Forms.Control)Window.NativeWindow;
        //    hmd.AttachToWindow(control.Handle);
        //}

        //Size2 renderTargetSize;
        //protected void INIT_TP_renderTarget()
        //{
        //    // Create our render target
        //    LOG("Creating render target");
        //    renderTargetSize = hmd.GetDefaultRenderTargetSize(1.5f);
        //    renderTarget = RenderTarget2D.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, new MipMapCount(1), PixelFormat.R8G8B8A8.UNorm, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
        //    renderTargetView = (RenderTargetView)renderTarget;
        //    renderTargetSRView = (ShaderResourceView)renderTarget;

        //    // Create a depth stencil buffer for our render target
        //    LOG("Creating a depth stencil buffer for target of rendering");
        //    depthStencilBuffer = DepthStencilBuffer.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, DepthFormat.Depth32, true);

        //    // Adjust render target size if there were any hardware limitations
        //    renderTargetSize.Width = renderTarget.Width;
        //    renderTargetSize.Height = renderTarget.Height;
        //}

        //protected void INIT_TP_eyeTextureRendering()
        //{
        //    // The viewport sizes are re-computed in case renderTargetSize changed
        //    eyeRenderViewport = new Rect[2];
        //    eyeRenderViewport[0] = new Rect(0, 0, renderTargetSize.Width / 2, renderTargetSize.Height);
        //    eyeRenderViewport[1] = new Rect((renderTargetSize.Width + 1) / 2, 0, eyeRenderViewport[0].Width, eyeRenderViewport[0].Height);

        //    // Create our eye texture data
        //    LOG("Creating eye texture data");
        //    eyeTexture = new D3D11TextureData[2];
        //    eyeTexture[0].Header.API = RenderAPIType.D3D11;
        //    eyeTexture[0].Header.TextureSize = renderTargetSize;
        //    eyeTexture[0].Header.RenderViewport = eyeRenderViewport[0];
        //    eyeTexture[0].pTexture = ((SharpDX.Direct3D11.Texture2D)renderTarget).NativePointer;
        //    eyeTexture[0].pSRView = renderTargetSRView.NativePointer;

        //    // Right eye uses the same texture, but different rendering viewport
        //    eyeTexture[1] = eyeTexture[0];
        //    eyeTexture[1].Header.RenderViewport = eyeRenderViewport[1];
        //}

        //protected void INIT_TP_d3d11()
        //{
        //    // Configure d3d11
        //    LOG("Configuring d3d11");
        //    device = (SharpDX.Direct3D11.Device)GraphicsDevice;
        //    D3D11ConfigData d3d11cfg = new D3D11ConfigData();
        //    d3d11cfg.Header.API = RenderAPIType.D3D11;
        //    d3d11cfg.Header.RTSize = hmd.Resolution;
        //    d3d11cfg.Header.Multisample = 1;
        //    d3d11cfg.pDevice = device.NativePointer;
        //    d3d11cfg.pDeviceContext = device.ImmediateContext.NativePointer;
        //    d3d11cfg.pBackBufferRT = ((RenderTargetView)GraphicsDevice.BackBuffer).NativePointer;
        //    d3d11cfg.pSwapChain = ((SharpDX.DXGI.SwapChain)GraphicsDevice.Presenter.NativePresenter).NativePointer;

        //    // Configure rendering
        //    LOG("Configuring rendering");
        //    eyeRenderDesc = new EyeRenderDesc[2];
        //    if (!hmd.ConfigureRendering(d3d11cfg, DistortionCapabilities.Chromatic | DistortionCapabilities.TimeWarp, hmd.DefaultEyeFov, eyeRenderDesc))
        //    {
        //        LOG_err("Failed to configure rendering");
        //        throw new Exception("Failed to configure rendering");
        //    }
        //}
        //protected void INIT_TP_hmd()
        //{
        //    // Set enabled capabilities
        //    hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;

        //    // Configure tracking
        //    hmd.ConfigureTracking(TrackingCapabilities.Orientation | TrackingCapabilities.Position | TrackingCapabilities.MagYawCorrection, TrackingCapabilities.None);

        //    // Dismiss the Heatlh and Safety Window
        //    hmd.DismissHSWDisplay();

        //    // Get HMD output
        //    LOG("Getting HMD output");
        //    var adapter = (Adapter)GraphicsDevice.Adapter;
        //    var hmdOutput = adapter.Outputs.FirstOrDefault(o => hmd.DeviceName.StartsWith(o.Description.DeviceName, StringComparison.OrdinalIgnoreCase));
        //    if (hmdOutput != null)
        //    {
        //        // Set game to fullscreen on rift
        //        var swapChain = (SwapChain)GraphicsDevice.Presenter.NativePresenter;
        //        var description = swapChain.Description.ModeDescription;
        //        swapChain.ResizeTarget(ref description);
        //        swapChain.SetFullscreenState(true, hmdOutput);
        //    }
        //}