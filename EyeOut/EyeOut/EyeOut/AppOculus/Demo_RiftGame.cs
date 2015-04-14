//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//using SharpDX;
//using SharpDX.Direct3D11;
//using SharpOVR;

//using System.Windows.Forms;

//namespace EyeOut
//{
//    // Use these namespaces here to override SharpDX.Direct3D11
//    using SharpDX.Toolkit;
//    using SharpDX.Toolkit.Graphics;
//    using SharpDX.DXGI;

//    /// <summary>
//    /// Simple RiftGame game using SharpDX.Toolkit.
//    /// </summary>
//    public class RiftGame : Game
//    {
//        //private GraphicsDeviceManager graphicsDeviceManager;

//        //private Matrix view;
//        //private Matrix projection;

//        //private Model model;

//        //private HMD hmd;
//        //private Rect[] eyeRenderViewport;
//        //private D3D11TextureData[] eyeTexture;

//        //private RenderTarget2D renderTarget;
//        //private RenderTargetView renderTargetView;
//        //private ShaderResourceView renderTargetSRView;
//        //private DepthStencilBuffer depthStencilBuffer;
//        //private EyeRenderDesc[] eyeRenderDesc;
//        //private PoseF[] renderPose = new PoseF[2];

//        //private Vector3 headPos = new Vector3(0f, 0f, -5f);
//        //private float bodyYaw = 3.141592f;



//        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//        #region LOG
//        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//        public static void LOG(string _msg)
//        {
//            C_Logger.Instance.LOG(e_LogMsgSource.Demo_TP, _msg);
//        }

//        public static void LOG_err(string _msg)
//        {
//            C_Logger.Instance.LOG_err(e_LogMsgSource.Demo_TP, _msg);
//        }
//        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//        #endregion LOG
//        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


//        ///// <summary>
//        ///// Initializes a new instance of the <see cref="RiftGame" /> class.
//        ///// </summary>
//        public RiftGame()
//        {
            
//        //    // Creates a graphics manager. This is mandatory.
//        //    LOG("Creating Graphics Manager");
//        //    graphicsDeviceManager = new GraphicsDeviceManager(this);

//        //    // Setup the relative directory to the executable directory 
//        //    // for loading contents with the ContentManager
//        //    Content.RootDirectory = "Content\\Demo";

//        //    //Content.RootDirectory = "..\\..\\Content";
//        //    //Content.RootDirectory = "B:\\__DIP\\dev\\2015_03_28 - sharpovr only\\sharpOVR_wpf\\Content";


//        //    // Initialize OVR Library
//        //    LOG("Initializing OVR Library");
//        //    OVR.Initialize();

//        //    // Create our HMD
//        //    LOG("Creating HMD control");
//        //    hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(HMDType.DK1);

//        //    // Match back buffer size with HMD resolution
//        //    graphicsDeviceManager.PreferredBackBufferWidth = hmd.Resolution.Width;
//        //    graphicsDeviceManager.PreferredBackBufferHeight = hmd.Resolution.Height;
//        }

//        //protected override void Initialize()
//        //{
//        //    // Modify the title of the window
//        //    Window.Title = "RiftGame Demonstration of SharpDX & SharpOVR # Guy Godin 2014";

//        //    // Attach HMD to window
//        //    LOG("Attaching HMD to window");
//        //    var control = (System.Windows.Forms.Control)Window.NativeWindow;
//        //    hmd.AttachToWindow(control.Handle);

//        //    // Create our render target
//        //    LOG("Creating render target");
//        //    var renderTargetSize = hmd.GetDefaultRenderTargetSize(1.5f);
//        //    renderTarget = RenderTarget2D.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, new MipMapCount(1), PixelFormat.R8G8B8A8.UNorm, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
//        //    renderTargetView = (RenderTargetView)renderTarget;
//        //    renderTargetSRView = (ShaderResourceView)renderTarget;

//        //    // Create a depth stencil buffer for our render target
//        //    LOG("Creating a depth stencil buffer for target of rendering");
//        //    depthStencilBuffer = DepthStencilBuffer.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, DepthFormat.Depth32, true);

//        //    // Adjust render target size if there were any hardware limitations
//        //    renderTargetSize.Width = renderTarget.Width;
//        //    renderTargetSize.Height = renderTarget.Height;

//        //    // The viewport sizes are re-computed in case renderTargetSize changed
//        //    eyeRenderViewport = new Rect[2];
//        //    eyeRenderViewport[0] = new Rect(0, 0, renderTargetSize.Width / 2, renderTargetSize.Height);
//        //    eyeRenderViewport[1] = new Rect((renderTargetSize.Width + 1) / 2, 0, eyeRenderViewport[0].Width, eyeRenderViewport[0].Height);

//        //    // Create our eye texture data
//        //    LOG("Creating eye texture data");
//        //    eyeTexture = new D3D11TextureData[2];
//        //    eyeTexture[0].Header.API = RenderAPIType.D3D11;
//        //    eyeTexture[0].Header.TextureSize = renderTargetSize;
//        //    eyeTexture[0].Header.RenderViewport = eyeRenderViewport[0];
//        //    eyeTexture[0].pTexture = ((SharpDX.Direct3D11.Texture2D)renderTarget).NativePointer;
//        //    eyeTexture[0].pSRView = renderTargetSRView.NativePointer;

//        //    // Right eye uses the same texture, but different rendering viewport
//        //    eyeTexture[1] = eyeTexture[0];
//        //    eyeTexture[1].Header.RenderViewport = eyeRenderViewport[1];

//        //    // Configure d3d11
//        //    LOG("Configuring d3d11");
//        //    var device = (SharpDX.Direct3D11.Device)GraphicsDevice;
//        //    D3D11ConfigData d3d11cfg = new D3D11ConfigData();
//        //    d3d11cfg.Header.API = RenderAPIType.D3D11;
//        //    d3d11cfg.Header.RTSize = hmd.Resolution;
//        //    d3d11cfg.Header.Multisample = 1;
//        //    d3d11cfg.pDevice = device.NativePointer;
//        //    d3d11cfg.pDeviceContext = device.ImmediateContext.NativePointer;
//        //    d3d11cfg.pBackBufferRT = ((RenderTargetView)GraphicsDevice.BackBuffer).NativePointer;
//        //    d3d11cfg.pSwapChain = ((SharpDX.DXGI.SwapChain)GraphicsDevice.Presenter.NativePresenter).NativePointer;

//        //    // Configure rendering
//        //    LOG("Configuring rendering");
//        //    eyeRenderDesc = new EyeRenderDesc[2];
//        //    if (!hmd.ConfigureRendering(d3d11cfg, DistortionCapabilities.Chromatic | DistortionCapabilities.TimeWarp, hmd.DefaultEyeFov, eyeRenderDesc))
//        //    {
//        //        LOG_err("Failed to configure rendering");
//        //        throw new Exception("Failed to configure rendering");
//        //    }

//        //    // Set enabled capabilities
//        //    hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;

//        //    // Configure tracking
//        //    hmd.ConfigureTracking(TrackingCapabilities.Orientation | TrackingCapabilities.Position | TrackingCapabilities.MagYawCorrection, TrackingCapabilities.None);

//        //    // Dismiss the Heatlh and Safety Window
//        //    hmd.DismissHSWDisplay();

//        //    // Get HMD output
//        //    LOG("Getting HMD output");
//        //    var adapter = (Adapter)GraphicsDevice.Adapter;
//        //    var hmdOutput = adapter.Outputs.FirstOrDefault(o => hmd.DeviceName.StartsWith(o.Description.DeviceName, StringComparison.OrdinalIgnoreCase));
//        //    if (hmdOutput != null)
//        //    {
//        //        // Set game to fullscreen on rift
//        //        var swapChain = (SwapChain)GraphicsDevice.Presenter.NativePresenter;
//        //        var description = swapChain.Description.ModeDescription;
//        //        swapChain.ResizeTarget(ref description);
//        //        swapChain.SetFullscreenState(true, hmdOutput);
//        //    }

//        //    base.Initialize();
//        //}

//        //protected override void LoadContent()
//        //{
//        //    LOG("Loading Content models etc.");
//        //    // Load a 3D model
//        //    // The [Ship.fbx] file is defined with the build action [ToolkitModel] in the project
//        //    model = Content.Load<Model>("Ship");

//        //    // Enable default lighting on model.
//        //    BasicEffect.EnableDefaultLighting(model, true);

//        //    base.LoadContent();
//        //}

//        //protected override void Update(GameTime gameTime)
//        //{
//        //    base.Update(gameTime);

//        //    // Calculates the world and the view based on the model size
//        //    view = Matrix.LookAtRH(new Vector3(0.0f, 0.0f, 7.0f), new Vector3(0, 0.0f, 0), Vector3.UnitY);
//        //    projection = Matrix.PerspectiveFovRH(0.9f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
//        //}

//        //protected override bool BeginDraw()
//        //{
//        //    if (!base.BeginDraw())
//        //        return false;

//        //    // Set Render Target and Viewport
//        //    GraphicsDevice.SetRenderTargets(depthStencilBuffer, renderTarget);
//        //    GraphicsDevice.SetViewport(0f, 0f, (float)renderTarget.Width, (float)renderTarget.Height);

//        //    // Begin frame
//        //    hmd.BeginFrame(0);
//        //    return true;
//        //}

//        //protected override void Draw(GameTime gameTime)
//        //{
//        //    // Clear the screen
//        //    GraphicsDevice.Clear(Color.CornflowerBlue);

//        //    for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
//        //    {
//        //        var eye = hmd.EyeRenderOrder[eyeIndex];
//        //        var renderDesc = eyeRenderDesc[(int)eye];
//        //        var renderViewport = eyeRenderViewport[(int)eye];
//        //        var pose = renderPose[(int)eye] = hmd.GetEyePose(eye);

//        //        // Calculate view matrix                
//        //        var rollPitchYaw = Matrix.RotationY(bodyYaw);
//        //        var finalRollPitchYaw = rollPitchYaw * pose.Orientation.GetMatrix();
//        //        var finalUp = finalRollPitchYaw.Transform(Vector3.UnitY);
//        //        var finalForward = finalRollPitchYaw.Transform(-Vector3.UnitZ);
//        //        var shiftedEyePos = headPos + rollPitchYaw.Transform(pose.Position);
//        //        view = Matrix.Translation(renderDesc.ViewAdjust) * Matrix.LookAtRH(shiftedEyePos, shiftedEyePos + finalForward, finalUp);

//        //        // Calculate projection matrix
//        //        projection = OVR.MatrixProjection(renderDesc.Fov, 0.001f, 1000.0f, true);
//        //        projection.Transpose();

//        //        // Set Viewport for our eye
//        //        GraphicsDevice.SetViewport(renderViewport.ToViewportF());

//        //        // Perform the actual drawing
//        //        InternalDraw(gameTime);
//        //    }
//        //}

//        //protected override void EndDraw()
//        //{
//        //    // Cancel original EndDraw() as the Present call is made through hmd.EndFrame()
//        //    hmd.EndFrame(renderPose, eyeTexture);
//        //}

//        //protected virtual void InternalDraw(GameTime gameTime)
//        //{
//        //    // Use time in seconds directly
//        //    var time = (float)gameTime.TotalGameTime.TotalSeconds;

//        //    // ------------------------------------------------------------------------
//        //    // Draw the 3d model
//        //    // ------------------------------------------------------------------------
//        //    var world = Matrix.Scaling(0.003f) *
//        //                Matrix.RotationY(time) *
//        //                Matrix.Translation(0, -1.5f, 2.0f);
//        //    model.Draw(GraphicsDevice, world, view, projection);

//        //    base.Draw(gameTime);
//        //}

//        //protected override void Dispose(bool disposeManagedResources)
//        //{
//        //    LOG("Disposing");
//        //    base.Dispose(disposeManagedResources);
//        //    if (disposeManagedResources)
//        //    {
//        //        // Release the HMD
//        //        LOG("Release the HMD");
//        //        hmd.Dispose();

//        //        // Shutdown the OVR Library
//        //        LOG("Shutting-down the OVR Library");
//        //        OVR.Shutdown();
//        //    }
//        //}
//    }
//}
