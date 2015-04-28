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

using StreamController = Basler.Pylon.Controls.WPF.StreamController;
using ImageViewer = Basler.Pylon.Controls.WPF.ImageViewer;

namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;

    /// <summary>
    /// Basler camera part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        
        //private List<GeometricPrimitive> primitives;
        StreamController streamController;
        ImageViewer imageViewer;

        Basler.Pylon.IImage baslerImage;
        //ToolkitImage cameraImage;
        PixelFormat cameraPixelFormat = PixelFormat.R8G8B8A8.UNorm;
        //PixelFormat cameraPixelFormat = PixelFormat.R8;
        private BasicEffect cameraBasicEffect;

        private Texture2D cameraTexture;
        private GeometricPrimitive cameraSurface;

        
        //Basler.Pylon.PixelDataConverter pixelDataConverter;
        int width;
        int height;
        byte[] pixelData;



        public void CAPTURE_cameraImage()
        {
            if (C_State.FURTHER(e_stateBaslerCam.initialized))
            {
                START_streaming();
                //baslerImage = config.streamController.Camera.StreamGrabber();
                baslerImage = config.imageViewer.CaptureImage();

                if (baslerImage != null)
                {
                    pixelData = (byte[])baslerImage.PixelData;
                    if (pixelData.Length != cameraTexture.Width * cameraTexture.Height * 4)
                    {
                        SETUP_BaslerCamera();
                    }
                    else
                    {
                        cameraTexture.SetData<byte>(pixelData);
                    }
                }
            }
            else
            {
                SETUP_BaslerCamera();
            }
        }


        protected virtual void Draw_BaslerCamera(GameTime _gameTime)
        {
            cameraBasicEffect.Texture = cameraTexture;

            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            cameraBasicEffect.Projection = eyeProjection;
            cameraBasicEffect.View = eyeView;
            cameraBasicEffect.World = ra.cameraSurfaceWorld;
            
            // Draw the primitive using BasicEffect
            cameraSurface.Draw(cameraBasicEffect);
        }

        public void START_streaming()
        {
            if (config.streamController.Camera.StreamGrabber.IsGrabbing == false)
            {
                config.streamController.StartStreaming();
                SETUP_BaslerCamera();
            }
            if(C_State.FURTHER(e_stateBaslerCam.initialized))
            {
                C_State.SET_state( e_stateBaslerCam.streaming );
            }
        }

        public void STOP_streaming()
        {
            config.streamController.StopStreaming();
            if(C_State.FURTHER(e_stateBaslerCam.initialized))
            {
                C_State.SET_state(e_stateBaslerCam.notStreaming);
            }
        }

        public void SETUP_BaslerCamera()
        {
            START_streaming();
            cameraPixelFormat = PixelFormat.B8G8R8X8.UNorm;

            //baslerImage.PixelTypeValue = Basler.Pylon.PixelType.BGR8packed;
            if (config.imageViewer != null)
            {
                baslerImage = config.imageViewer.CaptureImage();
                if (baslerImage != null)
                {
                    pixelData = (byte[])baslerImage.PixelData;
                    width = config.imageViewer.CaptureImage().Width;
                    height = config.imageViewer.CaptureImage().Height;

                    cameraTexture = Texture2D.New(GraphicsDevice, width, height, cameraPixelFormat, pixelData, TextureFlags.ShaderResource, ResourceUsage.Dynamic);

                    cameraTexture.SetData<byte>(pixelData);

                    if (C_State.FURTHER(e_stateBaslerCam.initialized) == false)
                    {
                        C_State.SET_state(e_stateBaslerCam.initialized);
                    }
                }
            }
        }
        public void Constructor_BaslerCamera()
        {
            //streamController = new StreamController();
            //imageViewer = new ImageViewer();


            //streamController.

            //config.streamController = streamController;
            //config.imageViewer = imageViewer;

            if (config.streamController.Camera != null)
            {
                C_State.SET_state(e_stateBaslerCam.initializing);
            }
            if (config.ReadCameraStream == true)
            {
                SETUP_BaslerCamera();
                //config.streamController.StartStreaming();
                //if (config.streamController.Camera == null)
                //{
                //    // add camear
                //    config.streamController.
                //}
                //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
                //config.imageViewer.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            }
        }

        public void LoadContent_BaslerCamera()
        {
            //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            cameraBasicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));

            // size of imaginary camera picture plane [mm]
            float sizeX = 100; // [mm]
            float sizeY = 100; // [mm]
            cameraSurface = ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice, sizeX, sizeY));
            
            // Load the texture
            //cameraTexture = Content.Load<Texture2D>("speaker");
            cameraTexture = Content.Load<Texture2D>("cameraDefault_2015-04-20_09-34-31");
            
            //cameraTexture = Texture2D.New(GraphicsDevice, width, height, PixelFormat.B8G8R8A8.UNorm);
            cameraBasicEffect.Texture = cameraTexture;
            cameraBasicEffect.TextureEnabled = true;
        }

    }
}
