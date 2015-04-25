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
        ToolkitImage cameraImage;
        PixelFormat cameraPixelFormat = PixelFormat.R8G8B8A8.UNorm;
        //PixelFormat cameraPixelFormat = PixelFormat.R8;
        private BasicEffect cameraBasicEffect;
        private BasicEffect roboticArmEffect;
        private Texture2D cameraTexture;
        private GeometricPrimitive cameraSurface;

        private List<GeometricPrimitive> roboticArmParts;
        
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
            float y_Desk2RollAxis = 140f;
            float y_RollAxis2PitchAxis = 82.3f;
            float y_PitchAxis2YawAxisTop = 74.5f;
            float y_YawAxisTop2SensorMiddle = 39f;

            //float y_HeadCenter2Desk = -335.8f; // sum of previous
            float y_HeadCenter2Desk = -180f;

            float z_YawAxis2SensorSurface = -30.526f;
            float z_Sensor2CameraTexture = 10;


            Matrix translation_HeadCenter2Desk = Matrix.Translation(0, y_HeadCenter2Desk, 0);
            Matrix translation_Desk2RollAxis = Matrix.Translation(0, y_Desk2RollAxis, 0);
            Matrix translation_RollAxis2PitchAxis = Matrix.Translation(0,y_RollAxis2PitchAxis,0);
            Matrix translation_PitchAxis2YawAxisTop = Matrix.Translation(0, y_PitchAxis2YawAxisTop, 0);
            Matrix translation_YawAxisTop2Sensor = Matrix.Translation(0, y_YawAxisTop2SensorMiddle, z_YawAxis2SensorSurface);
            Matrix translation_Sensor2CameraTexture = Matrix.Translation(0, 0, z_Sensor2CameraTexture);

            //Matrix HeadCenter2CameraTexture = Matrix.Identity
            //    * translation_HeadCenter2Desk
            //    * translation_Desk2RollAxis * Matrix.RotationZ((float)MainWindow.Ms.Roll.angleSeen.RadFromDefault)
            //    * translation_RollAxis2PitchAxis * Matrix.RotationX((float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault)
            //    * translation_PitchAxis2YawAxisTop * Matrix.RotationY((float)MainWindow.Ms.Yaw.angleSeen.RadFromDefault + (float)Math.PI)
            //    * translation_YawAxisTop2Sensor * translation_Sensor2CameraTexture
            //    ;


            text = text + string.Format("\nREAD YawPitchRoll[deg] [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                (float)MainWindow.Ms.Yaw.angleSeen.RadFromDefaultZero,
                (float)MainWindow.Ms.Pitch.angleSeen.Dec_FromDefaultZero,
                (float)MainWindow.Ms.Roll.angleSeen.Dec_FromDefaultZero
                );

            List<Matrix> roboticArmTransformations = new List<Matrix>()
            {
                Matrix.Identity,


                translation_Sensor2CameraTexture,
                translation_YawAxisTop2Sensor,
                Matrix.RotationY((float)MainWindow.Ms.Yaw.angleSeen.RadFromDefaultZero + (float)Math.PI),
                translation_PitchAxis2YawAxisTop,
                Matrix.RotationX((float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault),
                translation_RollAxis2PitchAxis,
                Matrix.RotationZ((float)MainWindow.Ms.Roll.angleSeen.RadFromDefaultZero),
                translation_Desk2RollAxis,
                translation_HeadCenter2Desk
            };

            float scaling = 0.005f;
            roboticArmEffect.Projection = projection;
            roboticArmEffect.View = view;

            cameraBasicEffect.Projection = projection;
            cameraBasicEffect.View = view;
            Matrix world = Matrix.Identity;

            int qmax = roboticArmTransformations.Count;
            for (int q = 0; q < qmax; q++)
            {
                world *= roboticArmTransformations[q];
                roboticArmEffect.SpecularColor = new Vector3(255 * q / qmax, 0, 0);
                roboticArmEffect.World = world * Matrix.Scaling(scaling);
                roboticArmParts[q].Draw(roboticArmEffect);
            }

            cameraBasicEffect.World = roboticArmEffect.World;
            //Matrix HeadCenter2CameraTexture = Matrix.Identity
            //    * translation_Sensor2CameraTexture
            //    * translation_YawAxisTop2Sensor
            //    * Matrix.RotationY((float)MainWindow.Ms.Yaw.angleSeen.RadFromDefaultZero + (float)Math.PI)
            //    * translation_PitchAxis2YawAxisTop
            //    * Matrix.RotationX((float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault)
            //    * translation_RollAxis2PitchAxis
            //    * Matrix.RotationZ((float)MainWindow.Ms.Roll.angleSeen.RadFromDefaultZero)
            //    * translation_Desk2RollAxis 
            //    * translation_HeadCenter2Desk
            //    ;
                
            //var world = Matrix.Identity
            //            * HeadCenter2CameraTexture
            //            * Matrix.Scaling(scaling)
                //* mTransl
                        ;
            cameraBasicEffect.World = world;

            
            // Disable Cull only for the plane primitive, otherwise use standard culling
            //GraphicsDevice.SetRasterizerState(i == 0 ? GraphicsDevice.RasterizerStates.CullNone : GraphicsDevice.RasterizerStates.CullBack);



            model.Draw(GraphicsDevice, Matrix.Scaling(0.0001f / scaling) * world, view, projection);

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
            roboticArmEffect.TextureEnabled = false;

            roboticArmParts = new List<GeometricPrimitive>();
            int qmax = 10;
            for (int q = 0; q < qmax; q++)
            {
                roboticArmParts.Add(ToDisposeContent(GeometricPrimitive.Cylinder.New(GraphicsDevice, 10, 10)));
                //cameraSurface = ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice, sizeX, sizeY));
            }

        }
    }
}
