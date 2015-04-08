using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;
using SharpOVR;

using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

using System.Windows.Media.Imaging; // BitmapSource 
using System.Runtime.InteropServices;
using System.Windows.Threading; // dispatcherTimer

// minitri
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Windows;
namespace EyeOut
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;
    // minitri
    using Buffer = SharpDX.Direct3D11.Buffer;
    using Device = SharpDX.Direct3D11.Device;


    /// <summary>
    /// EyeOut telepresence using SharpDX.Toolkit - the init part
    /// </summary>
    public partial class C_Telepresence : Game
    {

        public C_TP_config config;

        public void INIT_toolkit(HMDType _hmdType) // called from constructor of class C_Telepresence
        {
            // Creates a graphics manager. This is mandatory.
            LOG("Creating Graphics Manager");
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory 
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content\\Demo";

            // Initialize OVR Library
            LOG("Initializing OVR Library");
            OVR.Initialize();

            // Create our HMD
            LOG("Creating HMD control");
            hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(_hmdType);

            // Match back buffer size with HMD resolution
            graphicsDeviceManager.PreferredBackBufferWidth = hmd.Resolution.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = hmd.Resolution.Height;
        }

        protected void INIT_TP_window()
        {
            // Modify the title of the window
            Window.Title = "EyeOut Telepresence";

            // Attach HMD to window
            LOG("Attaching HMD to window");
            var control = (System.Windows.Forms.Control)Window.NativeWindow;
            hmd.AttachToWindow(control.Handle);
        }

        Size2 renderTargetSize;
        protected void INIT_TP_renderTarget()
        {
            // Create our render target
            LOG("Creating render target");
            renderTargetSize = hmd.GetDefaultRenderTargetSize(1.5f);
            renderTarget = RenderTarget2D.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, new MipMapCount(1), PixelFormat.R8G8B8A8.UNorm, TextureFlags.RenderTarget | TextureFlags.ShaderResource);
            renderTargetView = (RenderTargetView)renderTarget;
            renderTargetSRView = (ShaderResourceView)renderTarget;

            // Create a depth stencil buffer for our render target
            LOG("Creating a depth stencil buffer for target of rendering");
            depthStencilBuffer = DepthStencilBuffer.New(GraphicsDevice, renderTargetSize.Width, renderTargetSize.Height, DepthFormat.Depth32, true);

            // Adjust render target size if there were any hardware limitations
            renderTargetSize.Width = renderTarget.Width;
            renderTargetSize.Height = renderTarget.Height;
        }

        protected void INIT_TP_eyeTextureRendering()
        {
            // The viewport sizes are re-computed in case renderTargetSize changed
            eyeRenderViewport = new Rect[2];
            eyeRenderViewport[0] = new Rect(0, 0, renderTargetSize.Width / 2, renderTargetSize.Height);
            eyeRenderViewport[1] = new Rect((renderTargetSize.Width + 1) / 2, 0, eyeRenderViewport[0].Width, eyeRenderViewport[0].Height);

            // Create our eye texture data
            LOG("Creating eye texture data");
            eyeTexture = new D3D11TextureData[2];
            eyeTexture[0].Header.API = RenderAPIType.D3D11;
            eyeTexture[0].Header.TextureSize = renderTargetSize;
            eyeTexture[0].Header.RenderViewport = eyeRenderViewport[0];
            eyeTexture[0].pTexture = ((SharpDX.Direct3D11.Texture2D)renderTarget).NativePointer;
            eyeTexture[0].pSRView = renderTargetSRView.NativePointer;

            // Right eye uses the same texture, but different rendering viewport
            eyeTexture[1] = eyeTexture[0];
            eyeTexture[1].Header.RenderViewport = eyeRenderViewport[1];
        }

        protected void INIT_TP_d3d11()
        {
            // Configure d3d11
            LOG("Configuring d3d11");
            device = (SharpDX.Direct3D11.Device)GraphicsDevice;
            D3D11ConfigData d3d11cfg = new D3D11ConfigData();
            d3d11cfg.Header.API = RenderAPIType.D3D11;
            d3d11cfg.Header.RTSize = hmd.Resolution;
            d3d11cfg.Header.Multisample = 1;
            d3d11cfg.pDevice = device.NativePointer;
            d3d11cfg.pDeviceContext = device.ImmediateContext.NativePointer;
            d3d11cfg.pBackBufferRT = ((RenderTargetView)GraphicsDevice.BackBuffer).NativePointer;
            d3d11cfg.pSwapChain = ((SharpDX.DXGI.SwapChain)GraphicsDevice.Presenter.NativePresenter).NativePointer;

            // Configure rendering
            LOG("Configuring rendering");
            eyeRenderDesc = new EyeRenderDesc[2];
            if (!hmd.ConfigureRendering(d3d11cfg, DistortionCapabilities.Chromatic | DistortionCapabilities.TimeWarp, hmd.DefaultEyeFov, eyeRenderDesc))
            {
                LOG_err("Failed to configure rendering");
                throw new Exception("Failed to configure rendering");
            }
        }
        protected void INIT_TP_hmd()
        {
            // Set enabled capabilities
            hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;

            // Configure tracking
            hmd.ConfigureTracking(TrackingCapabilities.Orientation | TrackingCapabilities.Position | TrackingCapabilities.MagYawCorrection, TrackingCapabilities.None);

            // Dismiss the Heatlh and Safety Window
            hmd.DismissHSWDisplay();

            // Get HMD output
            LOG("Getting HMD output");
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
        }
        protected override void Initialize()
        {
            INIT_TP_window();
            INIT_TP_renderTarget();
            INIT_TP_eyeTextureRendering();
            INIT_TP_d3d11();
            INIT_TP_hmd();

            base.Initialize();

            INIT_txu();
            INIT_TP_text();
        }

        protected void INIT_txu()
        {
            string fname;
            //fname = @"Content\Demo\MiniTri.fx";
            //fname = "MiniTri.fx";
            fname = @"B:\__DIP\dev\_main_dev\EyeOut\EyeOut\EyeOut\Content\Demo\MiniTri.fx";
            // Compile Vertex and Pixel shaders
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(fname, "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            var vertexShader = new VertexShader(device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(fname, "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            var pixelShader = new PixelShader(device, pixelShaderByteCode);

            // Layout from VertexShader input signature
            var layout = new InputLayout(
                device,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            // Instantiate Vertex buiffer from vertex data
            var vertices = Buffer.Create(device, BindFlags.VertexBuffer, new[]
                                  {
                                      new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                                  });
        }

        protected override void LoadContent()
        {
            LOG("Loading Content models etc.");
            // Load a 3D model
            // The [Ship.fbx] file is defined with the build action [ToolkitModel] in the project
            model = Content.Load<Model>("Ship");

            // Enable default lighting on model.
            BasicEffect.EnableDefaultLighting(model, true);

            base.LoadContent();
        }
    }
}
