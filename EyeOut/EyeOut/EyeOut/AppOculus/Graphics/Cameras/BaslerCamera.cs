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

using System.Collections;

using System.Threading;
using System.ComponentModel;

namespace EyeOut_Telepresence
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;


    //public class Texture2Denchanced : Texture2D
    //{
    //    public void SetData

    //}

    public enum e_textureConversionAlgorithm
    {
        safeConversion_forLoop = 0, // about 33 ms
        unsafeConversion_pointerForLoop = 1 // about 17 ms
    }

    /// <summary>
    /// Basler camera part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        private Queue<byte[]> queuePixelData = new Queue<byte[]>();
        private object queuePixelData_locker = new object();
        DateTime qAct ;
        Queue<DateTime> que = new Queue<DateTime>();

        //private Basler.Pylon.IImage baslerImage;
        //private PixelFormat cameraTexturePixelFormat = PixelFormat.R8G8B8A8.UNorm;
        private PixelFormat cameraTexturePixelFormat = PixelFormat.B8G8R8X8.UNorm;
        private BasicEffect cameraBasicEffect;



        public e_textureConversionAlgorithm textureConversionAlgorithm = e_textureConversionAlgorithm.unsafeConversion_pointerForLoop;

        BackgroundWorker UpdateTextureWorker;
        private object UpdateTextureWorker_locker = new object();

        private long cameraTextureByteCount;
        private object cameraTexture_locker = new object();

        private byte[] grabResultBufferRGB;
        private object grabResultBufferRGB_locker = new object();


        private byte[] textureSizedBuffer;
        private object textureSizedBuffer_locker = new object(); 

        private GeometricPrimitive cameraSurface;
        private Texture2D cameraTexture;

        private object locker_pixelData = new object();

        public void RealocateTexture(int width, int height, byte[] destinationBuffer)
        {
            lock (cameraTexture_locker)
            {
                if (cameraTexture != null)
                {
                    cameraTexture.Dispose();
                }
                cameraTexture = Texture2D.New(GraphicsDevice, width, height, cameraTexturePixelFormat, destinationBuffer,
                    TextureFlags.ShaderResource, ResourceUsage.Dynamic);
                cameraTextureByteCount = width * height * cameraTexturePixelFormat.SizeInBytes;
            }

        }

        public object initialize_locker = new object();
        public bool initialized = false;

        public void CAPTURE_cameraImage()
        {
            LOG("CAPTURE_cameraImage started");
            lock(initialize_locker)
            {
                if (initialized == false)
                {
                    lock (textureSizedBuffer_locker)
                    {
                        int width;
                        int height;
                        lock (grabResultBufferRGB_locker)
                        {
                            grabResultBufferRGB = new byte[config.cameraControl.grabResultBufferRGB_size];
                            //byte[] grabResultBuffer_RGB = config.cameraControl.ConvertStoredGrabResultToByteArray(ref ,out width, out height);

                            config.cameraControl.ConvertStoredGrabResultToByteArray(ref grabResultBufferRGB, out width, out height);

                            if ((grabResultBufferRGB == null) || (width == 0) || (height == 0))
                            {
                                return;
                            }
                            else
                            {
                                if (cameraTextureByteCount != grabResultBufferRGB.Length)
                                {
                                    cameraTextureByteCount = width * height * cameraTexturePixelFormat.SizeInBytes;
                                    textureSizedBuffer = new byte[cameraTextureByteCount];
                                    grabResultBufferRGB.CopyTo(textureSizedBuffer, 0);

                                    RealocateTexture(width, height, textureSizedBuffer);

                                }
                                else
                                {
                                    lock (cameraTexture_locker)
                                    {
                                        cameraTexture.SetData<byte>(grabResultBufferRGB);
                                    }
                                }
                                initialized = true;
                            }
                        }
                    }
                }
                else
                {
                    lock (UpdateTextureWorker_locker)
                    {
                        if (UpdateTextureWorker.IsBusy == false)
                        {
                            Thread.Sleep(1);
                            UpdateTextureWorker.RunWorkerAsync((object)config.player.hmd);
                        }
                    }

                }
                    
            }
            LOG("CAPTURE_cameraImage ended");
        }


        public unsafe void RGBtoRGBA_Safe(ref byte[] Source, ref byte[] Target)
        {
            int num = Source.Length / 3;

            int i_t = 0; // target index
            int i_s = 0; // source index
            for (int i = 0; i < num; i++)
            {
                Target[i_t] = Source[i_s];
                Target[i_t + 1] = Source[i_s + 1];
                Target[i_t + 2] = Source[i_s + 2];
                Target[i_t + 3] = 255;
                i_s += 3;
                i_t += 4;
            }

        }
        public unsafe void RGBtoRGBA_Unsafe(ref byte[] Source, ref byte[] Target)
        {
            int num = Source.Length / 3;

            fixed (byte* pSource = Source, pTarget = Target)
            {
                // Set the starting points in source and target for the copying. 
                byte* ps = pSource + 0;
                byte* pt = pTarget + 0;

                for (int i = 0; i < num; i++)
                {
                    *(pt) = *(ps);
                    *(pt + 1) = *(ps + 1);
                    *(pt + 2) = *(ps + 2);
                    *(pt + 3) = 255;
                    pt += 4;
                    ps += 3;
                }
            }
        }



        public void UpdateTexture_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (UpdateTextureWorker_locker)
            {
                if (config.cameraControl.StoredNewGrabResult == false)
                {
                    return;
                }
                Stopwatch stopwatch = Stopwatch.StartNew();
                lock (grabResultBufferRGB_locker)
                {
                    config.cameraControl.ConvertStoredGrabResultToByteArray(ref grabResultBufferRGB);
                    stopwatch.Stop();
                    LOG(string.Format("[Bayer BG8] to [RGB] byte array took [{0}]", stopwatch.Elapsed));

                    if (cameraTextureByteCount != grabResultBufferRGB.Length)
                    {
                        lock (textureSizedBuffer_locker)
                        {
                            LOG("byte[] textureSizeBuffer = new byte[cameraTextureByteCount];");
                            //textureSizedBuffer = new byte[cameraTextureByteCount];
                            //destinationBuffer.CopyTo(textureSizeBuffer, 0);

                            LOG("started recounting texture");

                            stopwatch = Stopwatch.StartNew();
                            string type = "undefined";

                            if (textureConversionAlgorithm == e_textureConversionAlgorithm.unsafeConversion_pointerForLoop)
                            {
                                RGBtoRGBA_Unsafe(ref grabResultBufferRGB, ref textureSizedBuffer);
                                type = "unsafe [pt+=4]";
                            }
                            else if (textureConversionAlgorithm == e_textureConversionAlgorithm.safeConversion_forLoop)
                            {
                                RGBtoRGBA_Safe(ref grabResultBufferRGB, ref textureSizedBuffer);
                                type = "safe [i_t+=4]";

                            }
                            stopwatch.Stop();
                            LOG(string.Format("RGB to RGBA texture conversion {1} took [{0}]", stopwatch.Elapsed, type));


                            if (config.cameraFrameQueueLength == config.cameraFrameQueueLengthList[0])
                            {
                                LOG("SetTextureData(textureSizedBuffer);");
                                SetTextureData(textureSizedBuffer);
                                LOG("SetTextureData(textureSizedBuffer); ended");
                                lock (queuePixelData_locker)
                                {
                                    if (queuePixelData.Count > 1)
                                    {
                                        queuePixelData.Clear();
                                    }
                                }
                            }
                            else
                            {
                                lock (queuePixelData_locker)
                                {
                                    LOG("SetTextureData(queued texture)");
                                    queuePixelData.Enqueue(textureSizedBuffer);
                                    que.Enqueue(DateTime.Now);

                                    if (queuePixelData.Count == config.cameraFrameQueueLength)
                                    {
                                        qAct = que.Dequeue();
                                        SetTextureData(queuePixelData.Dequeue());
                                    }
                                    else if (queuePixelData.Count > config.cameraFrameQueueLength)
                                    {
                                        // when cameraFrameQueueLength changed to smaller number
                                        while (queuePixelData.Count > config.cameraFrameQueueLength)
                                        {
                                            queuePixelData.Dequeue(); // skip them
                                        }
                                    }
                                    LOG("SetTextureData(queued texture) end");
                                }
                            }
                        }
                    }
                    else
                    {
                        SetTextureData(grabResultBufferRGB);
                    }
                    LOG("end UpdateTexture_DoWork");
                }
            }
        }

        private void SetTextureData(byte[] _textureSizedBuffer)
        {
            lock (cameraTexture_locker)
            {
                try
                {
                    cameraTexture.SetData<byte>(_textureSizedBuffer);
                }
                catch (Exception ex)
                {
                    LOG("Error while setting camera texture data : " + ex.Message);
                }
            }
        }
        protected virtual void Draw_BaslerCamera(GameTime _gameTime)
        {
            lock (cameraTexture_locker)
            {
                cameraBasicEffect.Texture = cameraTexture;
                var time = (float)gameTime.TotalGameTime.TotalSeconds;

                cameraBasicEffect.Projection = eyeProjection;
                cameraBasicEffect.View = eyeView;

                cameraBasicEffect.World = ra.cameraSurfaceWorld; //* Matrix.Translation(config.player.scout.Position);
                cameraBasicEffect.World.Invert();
                //cameraBasicEffect.World = Matrix.Identity ;
            
                // Draw the primitive using BasicEffect
                cameraSurface.Draw(cameraBasicEffect);
            }
        }

        public void START_streaming()
        {
            if (config.cameraControl.StartGrabbing())
            {
                timeStartedStreaming = DateTime.UtcNow.ToLocalTime();
                if (C_State.FURTHER(e_stateBaslerCam.initialized))
                {
                    C_State.SET_state(e_stateBaslerCam.streaming);
                }
            }
        }

        public void STOP_streaming()
        {
            if(config.cameraControl.StopGrabbing())
            {
                if(C_State.FURTHER(e_stateBaslerCam.initialized))
                {
                    C_State.SET_state(e_stateBaslerCam.notStreaming);
                }
            }
        }

        public void Constructor_BaslerCamera()
        {
            UpdateTextureWorker = new BackgroundWorker();
            UpdateTextureWorker.DoWork += UpdateTexture_DoWork;
            //UpdateTexture_worker.DoWork += UpdateTexture_DoWork_Unsafe;
            if (config.ReadCameraStream == true)
            {
                START_streaming();
            }
        }

        public float LoadContent_BaslerCamera()
        {
            //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            cameraBasicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));

            // size of imaginary camera picture plane [mm]
            float sizeX = 2040f; // [mm] - but pixels as it is auxilary - only depends on ratio 
            float sizeY = 2046f; // [mm]

            cameraSurface = ToDisposeContent(GeometricPrimitive.Plane.New(GraphicsDevice, sizeX, sizeY));
            
            // Load the texture
            //cameraTexture = Content.Load<Texture2D>("speaker");
            lock (cameraTexture_locker)
            {
                cameraTexture = Content.Load<Texture2D>("cameraDefault_2015-04-20_09-34-31");
                //cameraTexture = Texture2D.New(GraphicsDevice, width, height, PixelFormat.B8G8R8A8.UNorm);
                cameraBasicEffect.Texture = cameraTexture;
            
                cameraBasicEffect.TextureEnabled = true;
                cameraBasicEffect.LightingEnabled = false;
            }

            return sizeX;
        }

    }
}




















