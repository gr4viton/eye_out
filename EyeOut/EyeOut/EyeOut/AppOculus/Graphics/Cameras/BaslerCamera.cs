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
        private Texture2D cameraTexture;
        private GeometricPrimitive cameraSurface;

        
        //Basler.Pylon.PixelDataConverter pixelDataConverter;
        int width;
        int height;
        //long rgbLen;
        //long rgbaLen;
        //bool first2 = true;
        byte[] pixelData;
        //private byte[] pixelData;
        //private byte[] rgbPixelData;
        //private bool notGrabbedYet = true;

        public float cameraSurfaceX;
        public float cameraSurfaceY;

        public C_CounterDown everyFrame = new C_CounterDown(10);
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
            //GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullBack);

            //GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullNone);
            //cameraBasicEffect.VertexColorEnabled = true;
            cameraBasicEffect.TextureEnabled = true;
            cameraBasicEffect.Texture = cameraTexture;

            // Calculate the translation

            //float gameTime = _gameTime.ElapsedGameTime.Milliseconds/100;

            // Setup the World matrice for this primitive
            //cameraBasicEffect.World =
            //    Matrix.Scaling(1f)
            //    * Matrix.Translation(cameraSurfaceX, cameraSurfaceY, 0);
                //* Matrix.RotationX((float)MainWindow.Ms.Pitch.angleSent.Dec_interval_piPi);
                //* Matrix.RotationY((float) MainWindow.Ms.Yaw.angleSent.Dec_interval_piPi)
                //* Matrix.RotationZ((float) MainWindow.Ms.Roll.angleSent.Dec_interval_piPi);

            //cameraBasicEffect.View =
            //    Matrix.Scaling(1f)
            //    * Matrix.Translation(cameraSurfaceX, cameraSurfaceY, 0);
            //* Matrix.RotationX((float)MainWindow.Ms.Pitch.angleSent.Dec_interval_piPi);
            //* Matrix.RotationY((float) MainWindow.Ms.Yaw.angleSent.Dec_interval_piPi)
            //* Matrix.RotationZ((float) MainWindow.Ms.Roll.angleSent.Dec_interval_piPi);

            //            * Matrix.RotationYawPitchRoll(
            //                (float)MainWindow.Ms.Yaw.angleWanted.Dec_interval_piPi,
            //                (float)MainWindow.Ms.Pitch.angleWanted.Dec_interval_piPi,
            //                (float)MainWindow.Ms.Roll.angleWanted.Dec_interval_piPi
            //                )
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
            float y_HeadCenter2Desk = -250f;

            float z_YawAxis2SensorSurface = -30.526f;
            float z_Sensor2CameraTexture = 100;


            Matrix translation_HeadCenter2Desk = Matrix.Translation(0, y_HeadCenter2Desk, 0);
            Matrix translation_Desk2RollAxis = Matrix.Translation(0, y_Desk2RollAxis, 0);
            Matrix translation_RollAxis2PitchAxis = Matrix.Translation(0,y_RollAxis2PitchAxis,0);
            Matrix translation_PitchAxis2YawAxisTop = Matrix.Translation(0, y_PitchAxis2YawAxisTop, 0);
            Matrix translation_YawAxisTop2Sensor = Matrix.Translation(0, y_YawAxisTop2SensorMiddle, z_YawAxis2SensorSurface);
            Matrix translation_Sensor2CameraTexture = Matrix.Translation(0, 0, z_Sensor2CameraTexture);

            

            //Matrix mRot = Matrix.RotationYawPitchRoll(
            //                (float)MainWindow.Ms.Yaw.angleSeen.RadFromDefault + (float)Math.PI,
            //                (float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault,
            //                (float)MainWindow.Ms.Roll.angleSeen.RadFromDefault
            //                );

            Matrix HeadCenter2CameraTexture = Matrix.Identity
                * translation_HeadCenter2Desk
                * translation_Desk2RollAxis * Matrix.RotationZ((float)MainWindow.Ms.Roll.angleSeen.RadFromDefault)
                * translation_RollAxis2PitchAxis * Matrix.RotationX((float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault)
                * translation_PitchAxis2YawAxisTop * Matrix.RotationY((float)MainWindow.Ms.Yaw.angleSeen.RadFromDefault + (float)Math.PI)
                * translation_YawAxisTop2Sensor * translation_Sensor2CameraTexture
                ;
                
                //Matrix.RotationYawPitchRoll(
                //            (float)MainWindow.Ms.Yaw.angleSeen.RadFromDefault + (float)Math.PI,
                //            (float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault,
                //            (float)MainWindow.Ms.Roll.angleSeen.RadFromDefault
                //            );

            text = text + string.Format("\n[{0}] == {1}", (float)MainWindow.Ms.Pitch.angleSeen.RadFromDefault, (float)MainWindow.Ms.Pitch.angleSeen.Dec_FromDefaultZero);
            //Matrix mRot = Matrix.RotationYawPitchRoll(
            //                (float)MainWindow.Ms.Yaw.angleWanted.RadFromDefault + (float)Math.PI,
            //                (float)MainWindow.Ms.Pitch.angleWanted.RadFromDefault,
            //                (float)MainWindow.Ms.Roll.angleWanted.RadFromDefault
            //                );

            //cameraBasicEffect.World = Matrix.Identity
            //            * Matrix.Scaling(1f)
            //    //* Matrix.RotationX(time * 4)
            //            * mTransl
            //            * mRot
            //            //* Matrix.RotationZ(time * 4)
            //            //* Matrix.RotationAxis(
            //            //* -mTransl
            //            ;

            float scaling = 0.001f;
            var world = Matrix.Identity
                        //* Matrix.RotationY(time)
                        //* mTransl
                        * HeadCenter2CameraTexture
                        * Matrix.Scaling(scaling)
                //* mTransl
                        ;

            cameraBasicEffect.Projection = projection;
            cameraBasicEffect.View = view;
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
            cameraSurfaceX = 0.0f;
            cameraSurfaceY = 0.0f;
        }

        public void LoadContent_BaslerCamera()
        {
            //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            cameraBasicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            cameraSurface = ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice));

            // Load the texture
            //cameraTexture = Content.Load<Texture2D>("speaker");
            cameraTexture = Content.Load<Texture2D>("cameraDefault_2015-04-20_09-34-31");
            
            //cameraTexture = Texture2D.New(GraphicsDevice, width, height, PixelFormat.B8G8R8A8.UNorm);
            cameraBasicEffect.Texture = cameraTexture;
            cameraBasicEffect.TextureEnabled = true;
        }
    }
}
