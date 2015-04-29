using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

using SharpOVR; // PoseF
using EyeOut;

namespace EyeOut_Telepresence
{
    /// <summary>
    /// Graphics part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {

        protected override bool BeginDraw()
        {
            if (!base.BeginDraw())
                return false;

            BeginDraw_FPS();

            // Set Render Target and Viewport
            GraphicsDevice.SetRenderTargets(depthStencilBuffer, renderTarget);
            GraphicsDevice.SetViewport(0f, 0f, (float)renderTarget.Width, (float)renderTarget.Height);

            // Begin frame
            hmd.BeginFrame(frameIndex);
            frameIndex++;

            return true;
        }

        public void UpdateFromHmd(EyeType eye)
        {

            //TrackingState outTrack = hmd.GetTrackingState(0);

            //PoseF[] outEyePoses = new PoseF[2];

            //// hmdToEyeViewOffset[2] can be ovrEyeRenderDesc.HmdToEyeViewOffset returned
            ////     from ovrHmd_ConfigureRendering or ovrHmd_GetRenderDesc. 
            //FovPort fov = renderDesc.Fov;

            //Vector3 hmdToEyeViewOffset1 = hmd.GetRenderDesc(eye, fov).HmdToEyeViewOffset;
            //Vector3 hmdToEyeViewOffset2 = hmd.GetRenderDesc(eye, fov).HmdToEyeViewOffset;

            //Vector3[] hmdToEyeViewOffset = new Vector3[] { hmdToEyeViewOffset1, hmdToEyeViewOffset2 };

            //hmd.GetEyePoses(frameIndex, hmdToEyeViewOffset, outEyePoses, ref outTrack);
            //var pose = renderPose[(int)eye] = outTrack.CameraPose; 
            //hmd.GetHmdPosePerEye(eye); // obsolete in 0.4.4
            //hmd.GetEyePose(eye); // 0.4.1

            //var orientation = renderPose[(int)eye].Orientation;

            //OVR.MatrixProjection(renderDesc.Fov);

            config.player.hmd.PoseF = hmd.GetHmdPosePerEye(eye);
            //var pose = hmd.GetHmdPosePerEye(eye);
            //config.player.lastPose = pose;
            //config.player.UPDATE_hmdOrientation(pose.Orientation);

        }
        #region Draw

        private void SETUP_eyeRender(int eyeIndex)
        {
            GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.Default);
            EyeType eye = hmd.EyeRenderOrder[eyeIndex];
            EyeRenderDesc renderDesc = eyeRenderDesc[(int)eye];
            Rect renderViewport = eyeRenderViewport[(int)eye];

            UpdateFromHmd(eye); 

            renderPose[(int)eye] = config.player.hmd.PoseF;

            // Calculate view matrix                
            var finalRollPitchYaw = config.player.Rotation;

            var finalUp = finalRollPitchYaw.Transform(Vector3.UnitY);
            //var finalUp = Vector3.UnitY
            var finalForward = finalRollPitchYaw.Transform(-Vector3.UnitZ);

            var shiftedEyePos = config.player.Position;

            eyeView = Matrix.Translation(renderDesc.HmdToEyeViewOffset) * config.player.LookAtRH;

            // Calculate projection matrix
            eyeProjection = OVR.MatrixProjection(renderDesc.Fov, 0.001f, -1000.0f, true);
            eyeProjection.Transpose();

            eyeWorld = Matrix.Identity;

            // Set Viewport for our eye
            GraphicsDevice.SetViewport(renderViewport.ToViewportF());
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.Clear(new Color(41,51,86));
            GraphicsDevice.Clear(Color.White);

            for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
            {
                SETUP_eyeRender(eyeIndex);

                Draw_SkySurface(gameTime);
                Draw_Model(gameTime);
                Draw_BaslerCamera(gameTime);
                Draw_RoboticArm();
                //Draw_Font((int)eye);
                //Draw_SoundGraphicalEffects();
            }

            BeginDraw_Font();
            for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++) // need second loop as font drawing uses stencil buffer
            {
                SETUP_eyeRender(eyeIndex); // ask for the values from hmd again as they may have changed from the point where other models were drown

                Draw_Font(eyeIndex);
            }
            EndDraw_Font();

            CONTROL_motors();

            HUD.AppendLine(string.Format("hmd latency = {0,7:0.00000000}", hmd.GetMeasuredLatency()));
        }

        #endregion Draw

        protected override void EndDraw()
        {
            // Cancel original EndDraw() as the Present call is made through hmd.EndFrame()
            
            //base.EndDraw();

            //// waiing loop
            //if (config.wantedDelay != 0)
            //{
            //    System.Threading.Thread.Sleep(config.wantedDelay);
            //}
            hmd.EndFrame(renderPose, eyeTexture);
        }

        protected virtual void Draw_Model(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // ------------------------------------------------------------------------
            // Draw the 3d model
            // ------------------------------------------------------------------------

            Matrix mRot = Matrix.RotationY(time * 1);
            Matrix mTransl = Matrix.Translation(0, -1.5f, -10.0f);

            var world = Matrix.Scaling(0.003f) 
                        * Matrix.RotationY(time) 
                        * mTransl
                        * mRot
                        //* mTransl
                        ;



            modelAirplane.Draw(GraphicsDevice, world, eyeView, eyeProjection);
            //BasicEffect.EnableDefaultLighting(model, true);
            //GraphicsDevice.BackBuffer.Dispose();

            //Texture2D a = new Texture2D();
            //GraphicsDevice.DepthStencilBuffer.SetData(a);

            base.Draw(gameTime);
        }
    }
}
