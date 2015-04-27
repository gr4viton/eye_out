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

namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;

    /// <summary>
    /// 
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        
        //private List<GeometricPrimitive> primitives;


        Basler.Pylon.IImage baslerImage;
        //ToolkitImage cameraImage;
        PixelFormat cameraPixelFormat = PixelFormat.R8G8B8A8.UNorm;
        //PixelFormat cameraPixelFormat = PixelFormat.R8;
        private BasicEffect cameraBasicEffect;
        private BasicEffect roboticArmEffect;
        private Texture2D cameraTexture;
        private GeometricPrimitive cameraSurface;

        private List<GeometricPrimitive> roboticArmParts;
        private Texture2D roboticArmTexture;
        
        //Basler.Pylon.PixelDataConverter pixelDataConverter;
        int width;
        int height;
        byte[] pixelData;


        public void CAPTURE_cameraImage()
        {
            if (config.firstPass == true)
            {
                config.streamController.StartStreaming();

                cameraPixelFormat = PixelFormat.B8G8R8X8.UNorm;

                //baslerImage.PixelTypeValue = Basler.Pylon.PixelType.BGR8packed;
                if (config.imageViewer != null)
                {
                    baslerImage = config.imageViewer.CaptureImage();
                    if(baslerImage != null)
                    {
                        pixelData = (byte[])baslerImage.PixelData;
                        width = config.imageViewer.CaptureImage().Width;
                        height = config.imageViewer.CaptureImage().Height;

                        cameraTexture = Texture2D.New(GraphicsDevice, width, height, cameraPixelFormat, pixelData, TextureFlags.ShaderResource, ResourceUsage.Dynamic);
                        config.firstPass = false;
                    }
                }
            }
            else
            {
                pixelData = (byte[])config.imageViewer.CaptureImage().PixelData;
                cameraTexture.SetData<byte>(pixelData);
            }
        }


        protected virtual void Draw_BaslerCamera(GameTime _gameTime)
        {
            cameraBasicEffect.Texture = cameraTexture;

            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            //cameraBasicEffect.Projection = 


            //Matrix mRot = Matrix.RotationY((float)Math.PI );

            // robotic arm dimensions
            // in [mm]
            float y_AB = 140f; // y_Desk2RollAxis
            float y_BC = 82.3f; // y_RollAxis2PitchAxis
            float y_CD = 74.5f; // y_PitchAxis2YawAxisTop
            float y_DE = 39f; // y_YawAxisTop2SensorMiddle

            //float y_HeadCenter2Desk = -335.8f; // sum of previous
            //float y_HeadCenter2Desk = -180f; // y_HeadCenter2Desk

            float z_DE = 30.526f; // z_YawAxis2SensorSurface
            float z_EF = -100; // z_Sensor2CameraTexture


            //Matrix translation_HeadCenter2Desk = Matrix.Translation(0, y_HeadCenter2Desk, 0);
            Matrix t_AB = Matrix.Translation(0, y_AB, 0);
            Matrix t_BC = Matrix.Translation(0, y_BC,0);
            Matrix t_CD = Matrix.Translation(0, y_CD, 0);
            Matrix t_DE = Matrix.Translation(0, y_DE, z_DE);
            Matrix t_EF = Matrix.Translation(0, 0, z_EF);

            Matrix t_BA = Matrix.Invert(t_AB);
            Matrix t_CB = Matrix.Invert(t_BC);
            Matrix t_DC = Matrix.Invert(t_CD);
            Matrix t_ED = Matrix.Invert(t_DE);
            Matrix t_FE = Matrix.Invert(t_EF);


            Matrix r_B = Matrix.RotationZ((float)MainWindow.Ms.Roll.angleSeen.RadFromDefault);
            Matrix r_C = Matrix.RotationX((float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault);
            Matrix r_D = Matrix.RotationY((float)MainWindow.Ms.Yaw.angleSeen.RadFromDefault);

            HUD.AppendLine(string.Format("READ YawPitchRoll[deg] [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                (float)MainWindow.Ms.Yaw.angleSeen.Dec_FromDefault,
                (float)MainWindow.Ms.Pitch.angleSeen.Dec_FromDefault,
                (float)MainWindow.Ms.Roll.angleSeen.Dec_FromDefault
                ));

            List<Matrix> roboticArmTransformations = new List<Matrix>()
                {
                    Matrix.Identity, // A
                    t_AB,   // B
                    r_B,    // B with rotation
                    t_BC,   // C
                    r_C,    // C with rotation
                    t_CD,   // D
                    r_D,    // D with rotation
                    t_DE,   // E - surfaceOfSensor
                    t_EF    // F - cameraImageSurface
                };

            //Matrix.Translation(0, y_Desk2RollAxis, 0);

            float scaling = 0.005f;
            //float scaling = 0.05f;
            roboticArmEffect.Projection = eyeProjection;
            roboticArmEffect.View = eyeView;
            roboticArmEffect.World = eyeWorld;


            int qmax = roboticArmTransformations.Count;
            for (int q = 0; q < qmax; q++)
            {
                float brightness = (float)q / (float)(qmax -1);
                roboticArmEffect.AmbientLightColor = brightness * (new Vector3(1, 1, 1));
                roboticArmEffect.LightingEnabled = true;

                roboticArmEffect.World = roboticArmTransformations[q] * roboticArmEffect.World;
                roboticArmParts[q].Draw(roboticArmEffect);
                //modelAirplane.Draw(GraphicsDevice, Matrix.Scaling(0.0001f / scaling) * roboticArmEffect.World, eyeView, eyeProjection);
            }

            cameraBasicEffect.Projection = eyeProjection;
            cameraBasicEffect.View = eyeView;
            cameraBasicEffect.World = roboticArmEffect.World;

            
            // Disable Cull only for the plane primitive, otherwise use standard culling
            //GraphicsDevice.SetRasterizerState(i == 0 ? GraphicsDevice.RasterizerStates.CullNone : GraphicsDevice.RasterizerStates.CullBack);



            modelAirplane.Draw(GraphicsDevice, Matrix.Scaling(0.0001f / scaling) * eyeWorld, eyeView, eyeProjection);

            // Draw the primitive using BasicEffect
            cameraSurface.Draw(cameraBasicEffect);
        }

        public void Constructor_BaslerCamera()
        {
            if (config.ReadCameraStream == true)
            {
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
            LoadContent_RoboticArm();
        }

        private void LoadContent_RoboticArm()
        {
            roboticArmEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));

            roboticArmEffect.AmbientLightColor = new Color3(255, 0, 255);
            roboticArmEffect.TextureEnabled = true;

            roboticArmParts = new List<GeometricPrimitive>();
            float sizeX = 50;
            float sizeY = 30;
            int qmax = 10;
            for (int q = 0; q < qmax; q++)
            {
                //roboticArmParts.Add(ToDisposeContent(GeometricPrimitive.Cylinder.New(GraphicsDevice, sizeX, sizeY)));
                roboticArmParts.Add(ToDisposeContent(GeometricPrimitive.Teapot.New(GraphicsDevice, sizeX)));
                //cameraSurface = ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice, sizeX, sizeY));
            }

            roboticArmTexture = Content.Load<Texture2D>("vut_grid");
            roboticArmEffect.Texture = roboticArmTexture;
        }
    }
}
