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

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Draw_SoundGraphicalEffects();


            for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
            {
                EyeType eye = hmd.EyeRenderOrder[eyeIndex];
                EyeRenderDesc renderDesc = eyeRenderDesc[(int)eye];
                Rect renderViewport = eyeRenderViewport[(int)eye];


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
                var pose = renderPose[(int)eye] = hmd.GetHmdPosePerEye(eye);
                //hmd.GetHmdPosePerEye(eye); // obsolete in 0.4.4
                //hmd.GetEyePose(eye); // 0.4.1

                //var orientation = renderPose[(int)eye].Orientation;

                //OVR.MatrixProjection(renderDesc.Fov);

                // Calculate view matrix                
                var rollPitchYaw = Matrix.RotationY(bodyYaw);
                var finalRollPitchYaw = rollPitchYaw * pose.Orientation.GetMatrix();
                var finalUp = finalRollPitchYaw.Transform(Vector3.UnitY);
                var finalForward = finalRollPitchYaw.Transform(-Vector3.UnitZ);
                var shiftedEyePos = headPos + rollPitchYaw.Transform(pose.Position);
                view = Matrix.Translation(renderDesc.HmdToEyeViewOffset)
                    //.ViewAdjust 
                     * Matrix.LookAtRH(shiftedEyePos, shiftedEyePos + finalForward, finalUp);

                // Calculate projection matrix
                projection = OVR.MatrixProjection(renderDesc.Fov, 0.001f, -1000.0f, true);
                projection.Transpose();

                // Set Viewport for our eye
                GraphicsDevice.SetViewport(renderViewport.ToViewportF());

                // Perform the actual drawing
                Draw_Model(gameTime);
                Draw_BaslerCamera(gameTime);
                //DrawFonts((int)eye);
            }

            BeginDraw_Font();
            for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
            {


                EyeType eye = hmd.EyeRenderOrder[eyeIndex];
                EyeRenderDesc renderDesc = eyeRenderDesc[(int)eye];
                Rect renderViewport = eyeRenderViewport[(int)eye];
                var pose = renderPose[(int)eye] = hmd.GetHmdPosePerEye(eye);

                // Calculate view matrix                
                var rollPitchYaw = Matrix.RotationY(bodyYaw);
                var finalRollPitchYaw = rollPitchYaw * pose.Orientation.GetMatrix();
                var finalUp = finalRollPitchYaw.Transform(Vector3.UnitY);
                var finalForward = finalRollPitchYaw.Transform(-Vector3.UnitZ);
                var shiftedEyePos = headPos + rollPitchYaw.Transform(pose.Position);
                view = Matrix.Translation(renderDesc.HmdToEyeViewOffset)
                    //.ViewAdjust 
                     * Matrix.LookAtRH(shiftedEyePos, shiftedEyePos + finalForward, finalUp);

                // Calculate projection matrix
                projection = OVR.MatrixProjection(renderDesc.Fov, 0.001f, -1000.0f, true);
                projection.Transpose();

                // Set Viewport for our eye
                GraphicsDevice.SetViewport(renderViewport.ToViewportF());

                Draw_Font((int)eye);
            }
            //DrawFonts(0);
            //DrawFonts(1);
            EndDraw_Font();


            CONTROL_motors();
        }

        #endregion Draw

        protected override void EndDraw()
        {
            // Cancel original EndDraw() as the Present call is made through hmd.EndFrame()
            hmd.EndFrame(renderPose, eyeTexture);
        }

        protected virtual void Draw_Model(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // ------------------------------------------------------------------------
            // Draw the 3d model
            // ------------------------------------------------------------------------
            var world = Matrix.Scaling(0.003f) *
                        Matrix.RotationY(time) *
                        Matrix.Translation(0, -1.5f, -10.0f);


            model.Draw(GraphicsDevice, world, view, projection);

            //BasicEffect.EnableDefaultLighting(model, true);
            //GraphicsDevice.BackBuffer.Dispose();

            //Texture2D a = new Texture2D();
            //GraphicsDevice.DepthStencilBuffer.SetData(a);

            base.Draw(gameTime);
        }
    }
}
