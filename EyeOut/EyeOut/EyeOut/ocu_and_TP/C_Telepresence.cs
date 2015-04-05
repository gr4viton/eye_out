using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Threading;
using System.ComponentModel; // backgroundWorker

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

//using SharpDX.Direct2D1; // text d3d10
//using SharpDX.DirectWrite; // text d3d10
using SharpDX.Direct3D9; // text d3d9

namespace EyeOut
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;

    public class C_CaptureData
    {
        //cv::Mat image;
        BitmapSource image;
        //OVR.posef pose;
        public C_CaptureData(BitmapSource _image)
        {
            image = _image;
        }

        public BitmapSource Image
        {
            get { return image; }
        }
    }
    
    public class C_CameraCaptureHandler
    {
        // instance of class interacting with camera
        private C_Camera cam;  // resp in fact I can use Capture & all the conversion would be defined here..
        //private Capture capture;        //takes images from camera as image frames
        //public static int actualId;

        private C_CaptureData captureData;

        private object captureData_locker = new object();

        private SharpOVR.HMD hmd; // for fetching the headpose
        bool isStopped;

        public C_CameraCaptureHandler(SharpOVR.HMD _hmd, int _camId)
        {
            // open the camera and set it up
            cam = new C_Camera(_camId);
            hmd = _hmd;
            isStopped = true;
        }

        public void startCapture()
        {
            isStopped = false;
            startCaptureLoop();
        }

        private void startCaptureLoop()
        {
            // better to create [Backgroundworker with the loop] inside C_CaptureDataHandler then in upper
            // because if I would have more cameras each would create its loop separately :)

            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += captureLoop_RunWorkerCompleted;
            worker.DoWork += captureLoop_DoWork;
            //worker.RunWorkerAsync((object)cmd);
            worker.RunWorkerAsync();
        }

        public void stopCapture()
        {
            isStopped = true;
        }

        bool newImgReady = true;

        // with a locker
        public C_CaptureData CaptureData // nullable
        {
            get
            {
                lock(captureData_locker)
                {
                    if (newImgReady)
                    {
                        // with act MOTOR pose
                        // act Cam img
                        return captureData;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                lock (captureData_locker)
                {
                    captureData = value;
                }
            }
        }


        private void captureLoop_DoWork(object sender, DoWorkEventArgs e)
        {

            while (isStopped == false)
            {
                captureData = new C_CaptureData(
                    cam.GET_txu()
                    //,MOT.GET_position();
                        );

                
            //// capture the headpose
            //S_CaptureData captured;
            //double captureTime = OVR.GetTimeInSeconds() - cam.CamLatency; // subtract a camera lag from pose
                
                // SDK can not predict in back time 
                // -> so tweak the sdk 
                // -> or make a abstract layer to store older positions
                // looks like this SDK through SharpDX cannot predict at all
                // so make predictions?

                //SharpOVR.TrackingState tracking = SharpOVR.TrackingCapabilities.Orientation(hmd, captureTime);

                //var pose = SharpOVR.TrackingCapabilities.Orientation;

                //captured.pose = SharpOVR.TrackingCapabilities.Position;

                //// capture the image
                //if(!videoCapture.grab() || !videoCapture.retrieve(captured.image))
                //{
                //    //Failed video capture
                //    LOG_err("Failed video capture");
                //}

                //cv::flip(captured.image.clone(), captured.image, 0); // opencv vs opengl ? vs directX
                //setResult(captured);

            }
            //e.Result = C_SPI.WriteData(e.Argument as byte[]);
        }

        private void captureLoop_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // catch if response was A-OK
            if (e.Error != null)
            {
                LOG_err(String.Format("Camera id#{2} had an error:\n{0}\n{1}", e.Error.Data, e.Error.Message, cam.id));
                //ie Helpers.HandleCOMException(e.Error);
            }
            else
            {
                //e.Result = "tocovrati writeData";
                //MyResultObject result = (MyResultObject)e.Result;

                //LOG("DATA SENT");
                //var results = e.Result as List<object>;
            }
        }


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.EyeOut_cam, _msg);
        }

        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.EyeOut_cam, _msg);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }
     
/*
// takes internation between Oculus VR and 
    public class CameraApp : RiftApp 
    {
        private C_CameraCaptureHandler captureHandler;
        private S_CaptureData captureData;
        
        gl::ProgramPtr videoRenderProgram; // shader
        gl::GeometryPtr videoGemoetry; // geometry
        gl::Texture2Ptr texture; // a texture containing the image capture from the camera

        // inits in initGl
        // use in render scene

        public CameraApp(): captureHandler(_hmd)
        {
            captureHandler.startCapture();
        }

        virtual ~CameraApp()
        {
            captureHandler.stopCapture();
        }

        virtual void initGl(); // once after opengl context created
        virtual void update(); // optionaly - once per frame
        virtual void renderScene(); // twice for frame - once for each eye

        void initGl()
        {
            videoRenderProgram = glutils::getProgram( Resource::SHaders_textured_VS, Resource::SHaders_textured_FS);
            texture = gl::TexturePtr(new gl::Texture2d());

            texture->bind();

            texture->parameter(GL_TEXTURE_MAG_FILTER,GL_LINEAR_);
            texture->parameter(GL_TEXTURE_MIN_FILTER,GL_LINEAR_);
            gl::Texture2d::unbind();

            float halfFov = (camera_HFOV_DEGREES / 2.0f) * DEGREES_TO_RADIANS;
            float scale = tan(halfFov) * IMAGE_DSTANCE;
            videoGeometry = GlUtils::getQuadGeometry(Camera_ASPECT, scale * 2.0f); // create geometry of a rectangle with given aspect ratio and size of longer dimension

            // same field of view as camer
            // cam 70°
            // distance 10 m
            // xhalf = d*tan(delta) 
            // xfull = 2*xhalf;
        }

        void update()
        {
            if (captureHandler.getResult(captureData)) // is there new data?
            {
                // consume it by loading it into texture
                // openCV picture = GL texture format !
                // does openCV picture = DX texture format ????

                // format it in capture thread
                texture->bind();
                glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, captureData.image.cols,
                    captureData.image.rows, 0 , 
                    gl_BGR, //<< opengl vs directx
                    gl_unsigned_byte, 
                    captureData.image.data);
                gl::Texture2d::unbind();
            }
        }

        void renderScene()
        {
            glClear(GL_DEPTH_BUFFER_BIT);
            glutils::renderSkybox(Resource::Images_SKY_CITY_XNEG_PNG); // any vr profits
            gl::MatrixStack & mv = global::Stacks::modelView();

            mv.with_push(
                [&]{
                mv.identity(); // reset


                    
                    // between curent head pose and pose in which the image from the camera was captured
                    // delta transform betwen capture pose and current render pose
                    glm::quat eyePose = rift::fromOvr(getEyePose().orientation);
                    glm::quat cameraPose = rift::fromOvr(captureData.pose.orientation);
                    glm::mat4 webcamdelta = glm::mat4_cast(glm::inverse(eyePose) * webcampose);

                    mv.preMultiply(webcamdelta);

                    mv.translate(glm::vec3(0,0,-IMAGE_DISTANCE)); // move farer

                    texture->bind();
                GLutils::renderGeometry(videoGemoetry, videoRenderProgram);
                    gl::Texture2d::unbind();
                });

            // diagnostics
            std::string message = PlatformID::format(
                "OpenGL FPS: %0.2f\n" + 
                "Vidcap FPS: %0.2f\n",
                fps, captureHandler.getCapturesPerSecond());
            GlfwApp::renderstringAt(message, glm::vec(-0.5f,0.5f));
        }
    }
*/

    /// <summary>
    /// EyeOut telepresence using SharpDX.Toolkit
    /// </summary>
    public partial class C_Telepresence : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;

        private Matrix view;
        private Matrix projection;


        private HMD hmd;
        private Rect[] eyeRenderViewport;
        private D3D11TextureData[] eyeTexture;
        SharpDX.Direct3D11.Device device;

        private RenderTarget2D renderTarget;
        private RenderTargetView renderTargetView;
        private ShaderResourceView renderTargetSRView;

        private DepthStencilBuffer depthStencilBuffer;
        private EyeRenderDesc[] eyeRenderDesc;

        private PoseF[] renderPose = new PoseF[2];

        // default head position and rotation in scene
        private Vector3 headPos = new Vector3(0f, 0f, -5f);
        private float bodyYaw = 3.141592f;

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region drawable objects
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private Model model;
        private SharpDX.Toolkit.Graphics.Texture2D txuCam;
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion drawable objects
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private C_CameraCaptureHandler captureHandler;
        //private C_CaptureData captureData;

                
        /// <summary>
        /// Initializes a new instance of the <see cref="RiftGame" /> class.
        /// </summary>
        public C_Telepresence(int _camId, HMDType _hmdType)
        {
            INIT_toolkit(_hmdType);
            // later possible more cameras -> int[] _camIds
            INIT_captureHandler(_camId);
        }

        public void RESET_magneticCorrection()
        { 
            // not sure!
            hmd.ConfigureTracking(TrackingCapabilities.Orientation | TrackingCapabilities.Position | TrackingCapabilities.MagYawCorrection, TrackingCapabilities.None);
        }

        public void INIT_captureHandler(int _camId)
        {
            captureHandler = new C_CameraCaptureHandler(hmd, _camId);
            captureHandler.startCapture();
            
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculates the world and the view based on the model size
            view = Matrix.LookAtRH(new Vector3(0.0f, 0.0f, 7.0f), new Vector3(0, 0.0f, 0), Vector3.UnitY);
            projection = Matrix.PerspectiveFovRH(0.9f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
        }

        protected override bool BeginDraw()
        {
            if (!base.BeginDraw())
                return false;

            // Set Render Target and Viewport
            GraphicsDevice.SetRenderTargets(depthStencilBuffer, renderTarget);
            GraphicsDevice.SetViewport(0f, 0f, (float)renderTarget.Width, (float)renderTarget.Height);

            // Begin frame
            hmd.BeginFrame(0);
            return true;
        }
        static int maxRepeats = 10;
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // synchronous..
            for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
            {
                var eye = hmd.EyeRenderOrder[eyeIndex];
                var renderDesc = eyeRenderDesc[(int)eye];
                var renderViewport = eyeRenderViewport[(int)eye];

                // get position from hmd
                var pose = renderPose[(int)eye] = hmd.GetEyePose(eye);

                //hmd.GetTrackingState()


                // Calculate view matrix                
                var rollPitchYaw = Matrix.RotationY(bodyYaw);

                var finalRollPitchYaw = rollPitchYaw * pose.Orientation.GetMatrix();

                var finalUp = finalRollPitchYaw.Transform(Vector3.UnitY);
                var finalForward = finalRollPitchYaw.Transform(-Vector3.UnitZ);

                // position of eye = head position + [ transform to left/right + tracked head pose ]
                var shiftedEyePos = headPos + rollPitchYaw.Transform(pose.Position);

                view = Matrix.Translation(renderDesc.ViewAdjust) * Matrix.LookAtRH(shiftedEyePos, shiftedEyePos + finalForward, finalUp);

                // Calculate projection matrix
                projection = OVR.MatrixProjection(renderDesc.Fov, 0.001f, 1000.0f, true);
                projection.Transpose();

                // Set Viewport for our eye
                GraphicsDevice.SetViewport(renderViewport.ToViewportF());

                // Perform the actual drawing
                InternalDraw(gameTime);
            }
            ORDER_motors();

            /*
            RenderTarget2D.Clear(Color.White);
            RenderTarget2D.DrawText("Hello World using DirectWrite!", TextFormat, ClientRectangle, SceneColorBrush);
             */

        }

        public void ORDER_motors()
        {
            // background worker - if it takes too long
            //maxRepeats--;
            //if (maxRepeats < 1)
            {
                //maxRepeats = 10;
                int ieye = 0;

                Matrix ori = renderPose[ieye].Orientation.GetMatrix();
                Vector3 pos = renderPose[ieye].Position;
                float[] posArr = pos.ToArray();

                StringBuilder msg = new StringBuilder();
                int q = 0;
                /*
                for(q=0;q<3;q++)
                {
                    msg.AppendLine(string.Format("{0}\t{1}\t{2}", ori[q+0], ori[q+1], ori[q+2]));
                }*/

                //float yaw, pitch, roll;
                float[] yawPitchRoll = new float[3];

                hmd.GetEyePose((SharpOVR.EyeType)ieye).Orientation.GetEulerAngles(out yawPitchRoll[0], out yawPitchRoll[1], out yawPitchRoll[2]);
                q = 0;
                double[] yawPitchRoll_d = new double[3];
                for(q=0;q<3;q++)
                {
                    yawPitchRoll_d[q] = C_Value.CONV_rad2deg((double)yawPitchRoll[q]);
                }

                q = 0;
                msg.Append(string.Format("YAW={0,5:0.00}°\tPITCH={1,5:0.00}°\tROLL={2,5:0.00}°", yawPitchRoll_d[q + 0], yawPitchRoll_d[q + 1], yawPitchRoll_d[q + 2]));


                LOG(msg.ToString());


                foreach (C_Motor mot in MainWindow.Ms)
                {
                    mot.angle.Dec_FromDefault = yawPitchRoll_d[(int)mot.rotMotor];
                    mot.speed.Dec = mot.speed.DecMax;
                    mot.REGISTER_move();
                }
                C_Motor.ORDER_ActionToAll();


                MainWindow.Ms.Yaw.ORDER_getPosition();
                
                //foreach (C_Motor mot in MainWindow.Ms)
                //{
                //    mot.ORDER_getPosition();
                //}
            }
        }


        protected override void EndDraw()
        {
            // Cancel original EndDraw() as the Present call is made through hmd.EndFrame()
            hmd.EndFrame(renderPose, eyeTexture);
        }

        protected virtual void InternalDraw(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // ------------------------------------------------------------------------
            // Draw the 3d model
            // ------------------------------------------------------------------------
            var world = Matrix.Scaling(0.003f) *
                        Matrix.RotationY(time) *
                        Matrix.Translation(0, -1.5f, 2.0f);
            model.Draw(GraphicsDevice, world, view, projection);


            //txuCam.draw
            //captureHandler.CaptureData.Image;
            DRAW_txu();
            base.Draw(gameTime);
        }

        protected void DRAW_text()
        {
            // Make the text boucing on the screen limits
            if ((fontDimension.Right + xDir) > renderTarget.Width)
                xDir = -1;
            else if ((fontDimension.Left + xDir) <= 0)
                xDir = 1;

            if ((fontDimension.Bottom + yDir) > renderTarget.Height)
                yDir = -1;
            else if ((fontDimension.Top + yDir) <= 0)
                yDir = 1;

            fontDimension.Left += (int)xDir;
            fontDimension.Top += (int)yDir;
            fontDimension.Bottom += (int)yDir;
            fontDimension.Right += (int)xDir;

            // Draw the text
            font.DrawText(null, displayText, fontDimension, FontDrawFlags.Center | FontDrawFlags.VerticalCenter, Color.White);

        }
        protected void DRAW_txu()
        {
            GET_txu();
        }
        protected void GET_txu()
        {

            /*
            System.IO.Stream inputMemoryStream = new System.IO.MemoryStream(captureHandler.CaptureData.Image);
            //
            //var inputTex2D = Texture2D.FromStream<Texture2D>(device, inputMemoryStream, (int)inputMemoryStream.Length, new ImageLoadInformation()
            var inputTex2D = Texture2D.Load(graphicsDeviceManager, inputMemoryStream, (int)inputMemoryStream.Length, new ImageLoadInformation()
            {
                Depth = 1,
                FirstMipLevel = 0,
                MipLevels = 0,
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Filter = FilterFlags.None,
                MipFilter = FilterFlags.None,
            });
            */
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            LOG("Disposing");
            base.Dispose(disposeManagedResources);
            if (disposeManagedResources)
            {
                // Release the HMD
                LOG("Release the HMD");
                hmd.Dispose();

                // Shutdown the OVR Library
                LOG("Shutting-down the OVR Library");
                OVR.Shutdown();
            }
        }

        

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.EyeOut, _msg);
        }

        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.EyeOut, _msg);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


    }
}
