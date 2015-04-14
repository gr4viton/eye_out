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

namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;

    /// <summary>
    /// Simple RiftGame game using SharpDX.Toolkit.
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;

        private Matrix view;
        private Matrix projection;

        private Model model;

        private HMD hmd;
        private Rect[] eyeRenderViewport;
        private D3D11TextureData[] eyeTexture;

        private RenderTarget2D renderTarget;
        private RenderTargetView renderTargetView;
        private ShaderResourceView renderTargetSRView;
        private DepthStencilBuffer depthStencilBuffer;
        private EyeRenderDesc[] eyeRenderDesc;
        private PoseF[] renderPose = new PoseF[2];

        
        private Vector3 headPos = new Vector3(0f, 0f, -5f);
        private float bodyYaw = 3.141592f;

        uint frameIndex = 0;

        private BasicEffect basicEffect;
        private Texture2D texture;
        private List<GeometricPrimitive> primitives;

        public TelepresenceSystemConfiguration config;


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        /// <summary>
        /// Initializes a new instance of the <see cref="TelepresenceSystem" /> class.
        /// </summary>
        public TelepresenceSystem(TelepresenceSystemConfiguration _configuration)
        {
            config = _configuration;
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

            base.Initialize();
        }
        #region LoadContent
        protected override void LoadContent()
        {
            LoadContent_Sprite();
            LoadContent_Models();
            LoadContent_Font();
            LoadContent_Sound();

            base.LoadContent();

            tiles[4].PlayDelegate();
        }

        void LoadContent_Models()
        {
            // Load a 3D model
            // The [Ship.fbx] file is defined with the build action [ToolkitModel] in the project
            model = Content.Load<Model>("Ship");

            // Enable default lighting on model.
            BasicEffect.EnableDefaultLighting(model, true);



            // Creates a basic effect
            basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice)
            {
                View = Matrix.LookAtRH(new Vector3(0, 0, 5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            });

            basicEffect.PreferPerPixelLighting = true;
            basicEffect.EnableDefaultLighting();

            // Creates all primitives
            primitives = new List<GeometricPrimitive>
                             {
                                 ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice)),
                                 ToDisposeContent(GeometricPrimitive.Cube.New(GraphicsDevice)),
                                 ToDisposeContent(GeometricPrimitive.Sphere.New(GraphicsDevice)),
                                 ToDisposeContent(GeometricPrimitive.GeoSphere.New(GraphicsDevice)),
                                 ToDisposeContent(GeometricPrimitive.Torus.New(GraphicsDevice)),
                                 ToDisposeContent(GeometricPrimitive.Cylinder.New(GraphicsDevice)),
                                 ToDisposeContent(GeometricPrimitive.Teapot.New(GraphicsDevice))
                             };

            // Load the texture
            texture = Content.Load<Texture2D>("speaker");
            basicEffect.Texture = texture;
            basicEffect.TextureEnabled = true;
        }

        #endregion LoadContent

        #region Update
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculates the world and the view based on the model size
            view = Matrix.LookAtRH(new Vector3(0.0f, 0.0f, 7.0f), new Vector3(0, 0.0f, 0), Vector3.UnitY);
            projection = Matrix.PerspectiveFovRH(0.9f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);

            Update_Sound();
            Update_Input();
        }

        
        #endregion Update


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