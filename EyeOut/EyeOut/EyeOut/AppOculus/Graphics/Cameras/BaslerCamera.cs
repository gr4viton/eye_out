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

    /// <summary>
    /// Basler camera part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        private Queue<byte[]> queuePixelData = new Queue<byte[]>();

        //private Basler.Pylon.IImage baslerImage;
        //private PixelFormat cameraTexturePixelFormat = PixelFormat.R8G8B8A8.UNorm;
        private PixelFormat cameraTexturePixelFormat = PixelFormat.B8G8R8X8.UNorm;
        private BasicEffect cameraBasicEffect;
        
        BackgroundWorker UpdateTexture_worker;

        private long cameraTextureByteCount;

        private object cameraTexture_locker = new object();

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
            lock(initialize_locker)
            //if(false)
            {
                if (initialized == false)
                {
                    int width;
                    int height;

                    byte[] destinationBuffer = config.camera.ConvertStoredGrabResultToByteArray(out width, out height);

                    if (destinationBuffer == null)
                    {
                        return;
                    }
                    else
                    {
                        if (cameraTextureByteCount != destinationBuffer.Length)
                        {
                            cameraTextureByteCount = width * height * cameraTexturePixelFormat.SizeInBytes;
                            byte[] textureSizeBuffer = new byte[cameraTextureByteCount];
                            destinationBuffer.CopyTo(textureSizeBuffer, 0);

                            RealocateTexture(width, height, textureSizeBuffer);
                            
                        }
                        else
                        {
                            lock (cameraTexture_locker)
                            {
                                cameraTexture.SetData<byte>(destinationBuffer);
                            }
                        }
                        initialized = true;
                    }
                }
                else
                {

                    if (UpdateTexture_worker.IsBusy == false)
                    {
                        //worker_SEND.RunWorkerAsync((object)echo);
                        UpdateTexture_worker.RunWorkerAsync((object)config.player.hmd);
                    }

                }
                    
            }
        }


        public void UpdateTexture_DoWork(object sender, DoWorkEventArgs e)
        {
            if(config.camera.StoredNewGrabResult == false)
            {
                return;
            }
            byte[] destinationBuffer = config.camera.ConvertStoredGrabResultToByteArray();

            //queuePixelData.Enqueue(destinationBuffer);
            //if (queuePixelData.Count == config.cameraFrameQueueLength)
            //{
            //    cameraTexture.SetData<byte>(queuePixelData.Dequeue());
            //}
            if (cameraTextureByteCount != destinationBuffer.Length)
            {
                LOG("byte[] textureSizeBuffer = new byte[cameraTextureByteCount];");
                byte[] textureSizeBuffer = new byte[cameraTextureByteCount];
                //destinationBuffer.CopyTo(textureSizeBuffer, 0);

                LOG("started recounting texture" + DateTime.UtcNow.ToString());

                Stopwatch stopwatch = Stopwatch.StartNew();
                // 30ms - safe version
                int num = destinationBuffer.Length / 3;
                int i;
                int i_t=0; // target index
                int i_s=0; // source index
                for (i = 0; i < num; i++)
                {
                    textureSizeBuffer[i_t++] = destinationBuffer[i_s++];
                    textureSizeBuffer[i_t++] = destinationBuffer[i_s++];
                    textureSizeBuffer[i_t++] = destinationBuffer[i_s++];
                    textureSizeBuffer[i_t++] = 255;
                }

                stopwatch.Stop();

                LOG(string.Format("Recounting texture safe [i_t++] took [{0}] ms", stopwatch.Elapsed));


                //textureSizeBuffer
                LOG("ended recounting texture");
                LOG("cameraTexture.SetData<byte>(textureSizeBuffer);");
                lock (cameraTexture_locker)
                {
                    try
                    {
                        cameraTexture.SetData<byte>(textureSizeBuffer);
                    }
                    catch (Exception ex)
                    {
                        LOG("Error while setting camera texture data : " + ex.Message);
                    }
                }
                LOG("ended" + DateTime.UtcNow.ToString());
            }
            else
            {
                lock (cameraTexture_locker)
                {
                    cameraTexture.SetData<byte>(destinationBuffer);
                }
            }
        }


        public unsafe void UpdateTexture_DoWork_Unsafe(object sender, DoWorkEventArgs e)
        {
            if (config.camera.StoredNewGrabResult == false)
            {
                return;
            }
            byte[] destinationBuffer = config.camera.ConvertStoredGrabResultToByteArray();

            //queuePixelData.Enqueue(destinationBuffer);
            //if (queuePixelData.Count == config.cameraFrameQueueLength)
            //{
            //    cameraTexture.SetData<byte>(queuePixelData.Dequeue());
            //}
            if (cameraTextureByteCount != destinationBuffer.Length)
            {
                LOG("byte[] textureSizeBuffer = new byte[cameraTextureByteCount];");
                byte[] textureSizeBuffer = new byte[cameraTextureByteCount];
                //destinationBuffer.CopyTo(textureSizeBuffer, 0);

                LOG("started recounting texture" + DateTime.UtcNow.ToString());

                Stopwatch stopwatch;

                stopwatch = Stopwatch.StartNew();

                int num = destinationBuffer.Length / 3;

                // The following fixed statement pins the location of the source and 
                // target objects in memory so that they will not be moved by garbage 
                // collection. 

                // 44 ms
                int a = 0;
                fixed (byte* pSource = destinationBuffer, pTarget = textureSizeBuffer)
                {
                    // Set the starting points in source and target for the copying. 
                    byte* ps = pSource + 0;
                    byte* pt = pTarget + 0;

                    // Copy the specified number of bytes from source to target. 
                    for (int i = 0; i < num; i++)
                    {
                        for (a = 0; a < 3; a++)
                        {
                            *pt = *ps;
                            pt++;
                            ps++;
                        }
                        *pt = 255;
                        pt++;
                    }
                }

                stopwatch.Stop();

                LOG(string.Format("Recounting texture unsafe [for(pt++)] took [{0}] ms", stopwatch.Elapsed));



                stopwatch = Stopwatch.StartNew();

                // The following fixed statement pins the location of the source and 
                // target objects in memory so that they will not be moved by garbage 
                // collection. 
                fixed (byte* pSource = destinationBuffer, pTarget = textureSizeBuffer)
                {
                    // Set the starting points in source and target for the copying. 
                    byte* ps = pSource + 0;
                    byte* pt = pTarget + 0;

                    // Copy the specified number of bytes from source to target. 
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
                stopwatch.Stop();

                LOG(string.Format("Recounting texture unsafe [pt+4] took [{0}] ms", stopwatch.Elapsed));

                //textureSizeBuffer
                LOG("ended recounting texture");
                LOG("cameraTexture.SetData<byte>(textureSizeBuffer);");
                lock (cameraTexture_locker)
                {
                    try
                    {
                        cameraTexture.SetData<byte>(textureSizeBuffer);
                    }
                    catch(Exception ex)
                    {
                        LOG("Error while setting camera texture data : " + ex.Message);
                    }
                }
                LOG("ended" + DateTime.UtcNow.ToString());
            }
            else
            {
                lock (cameraTexture_locker)
                {
                    cameraTexture.SetData<byte>(destinationBuffer);
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
                cameraBasicEffect.World = ra.cameraSurfaceWorld;
                //cameraBasicEffect.World = Matrix.Identity ;
            
                // Draw the primitive using BasicEffect
                cameraSurface.Draw(cameraBasicEffect);
            }
        }

        public void START_streaming()
        {
            if (config.camera.StartGrabbing())
            {
                if (C_State.FURTHER(e_stateBaslerCam.initialized))
                {
                    C_State.SET_state(e_stateBaslerCam.streaming);
                }
            }
        }

        public void STOP_streaming()
        {
            if(config.camera.StopGrabbing())
            {
                if(C_State.FURTHER(e_stateBaslerCam.initialized))
                {
                    C_State.SET_state(e_stateBaslerCam.notStreaming);
                }
            }
        }

        public void Constructor_BaslerCamera()
        {
            UpdateTexture_worker = new BackgroundWorker();
            //UpdateTexture_worker.DoWork += UpdateTexture_DoWork;
            UpdateTexture_worker.DoWork += UpdateTexture_DoWork_Unsafe;
            if (config.ReadCameraStream == true)
            {
                START_streaming();
            }
        }

        public void LoadContent_BaslerCamera()
        {
            //config.streamController.Camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            cameraBasicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));

            // size of imaginary camera picture plane [mm]
            float sizeX = 250; // [mm]
            float sizeY = 250; // [mm]
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
        }

    }
}
