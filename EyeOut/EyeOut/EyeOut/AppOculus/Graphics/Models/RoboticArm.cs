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

    public enum e_valueType
    {
        wantedValue,
        sentValue,    
        seenValue   
    }

    public enum e_RoboticArmPart
    {
        // robotic arm dimensions
        // A = Black desk bottom
        // B = Roll axis
        // C = Pitch axis
        // D = Yaw axis top
        // E = Sensor surface middle
        // F = Camera texture
        t_A,    // A
        t_AB,   // B
        r_B_roll,    // B with rotation
        t_BC,   // C
        r_C_pitch,    // C with rotation
        t_CD,   // D
        r_D_yaw,    // D with rotation
        t_DE,   // E - surfaceOfSensor
        t_EF    // F - cameraImageSurface

    }
    class RoboticArmPart
    {
        public e_RoboticArmPart type;
        public GeometricPrimitive anchorBody;
        public BasicEffect effect;  // contains this part world matrix = transformation from root to this bone

        //Model model;
        public Matrix transformation; // one per part

        public bool draw = true;
        public RoboticArmPart(e_RoboticArmPart _type, Matrix _transformation)
        {
            type = _type;
            transformation = _transformation;
        }
        public void SETUP_effect(Color color, Texture2D texture = null)
        {
            byte[] col = color.ToArray();
            float[] colF = new float[3] { col[0] / 255f, col[1] / 255f, col[2] / 255f };
            SETUP_effect(new Vector3(colF), texture);
        }

        public void SETUP_effect(Vector3 color, Texture2D texture = null)
        {
            effect.AmbientLightColor = color;
            effect.LightingEnabled = true;

            if (texture == null)
            {
                effect.TextureEnabled = false;
            }
            else
            {
                effect.TextureEnabled = true;
                effect.Texture = texture;
            }
        }
        public void Draw(Matrix view, Matrix projection)
        {
            if (draw)
            {
                effect.View = view;
                effect.Projection = projection;
                anchorBody.Draw(effect);
            }
        }
    }

    class RoboticArm
    {
        public List<RoboticArmPart> parts;
        public Matrix baseProjection;
        public Matrix baseView;
        public Matrix baseWorld; // first node = root
        public Matrix cameraSurfaceWorld; // the last node = camera image surface

        public bool draw = true;

        // height in [mm]
        float y_AB = 140f; // y_Desk - RollAxis
        float y_BC = 82.3f; // y_RollAxis -PitchAxis
        float y_CD = 74.5f; // y_PitchAxis - YawAxisTop
        float y_DE = 39f; // y_YawAxisTop - SensorMiddle
        // width in [mm]
        float z_DE = 30.526f; // z_YawAxis - SensorSurface
        float z_EF = 100; //z_Sensor - CameraTextureSurface .. recalculated in constructor

        public e_valueType angleType = e_valueType.wantedValue;

        public RoboticArmPart this[int i] 
        {
            get{ return parts[i]; }
        }

        public RoboticArmPart this[e_RoboticArmPart type] 
        {
            get{ return parts[(int)type]; }
        }

        public float this[e_rot mot]
        {
            get 
            {
                return YawPitchRoll[(int)mot];
            }
            set 
            {
                YawPitchRoll[(int)mot] = value;
            }
        }

        public List<float> YawPitchRoll;
        public float YawRad { get { return this[e_rot.yaw]; } set { this[e_rot.yaw] = value; } }
        public float PitchRad { get { return this[e_rot.pitch]; } set { this[e_rot.pitch] = value; } }
        public float RollRad { get { return this[e_rot.roll]; } set { this[e_rot.roll] = value; } }

        //Matrix translation_HeadCenter2Desk = Matrix.Translation(0, y_HeadCenter2Desk, 0);
        public Matrix t_AB ;
        public Matrix t_BC ;
        public Matrix t_CD ;
        public Matrix t_DE ;
        public Matrix t_EF ;
        public Matrix overDeskTranslation;

        public RoboticArm(float camTextureSizeX)
        {
            z_EF = -camTextureSizeX * 0.5913242f; // = camTextureSizeX * tan(hFOV/2)/2 = 0.5913242

            t_AB = Matrix.Translation(0, y_AB, 0);
            t_BC = Matrix.Translation(0, y_BC, 0);
            t_CD = Matrix.Translation(0, y_CD, 0);
            t_DE = Matrix.Translation(0, y_DE, z_DE);
            t_EF = Matrix.Translation(0, 0, z_EF);

            float y_defaultView = y_AB + y_BC + y_CD + y_DE;
            overDeskTranslation = Matrix.Translation(0, y_defaultView, 0);

            parts = new List<RoboticArmPart>();
            e_RoboticArmPart partType = 0;
            parts.Add(new RoboticArmPart (partType++, Matrix.Identity ));
            parts.Add(new RoboticArmPart (partType++, t_AB ));
            parts.Add(new RoboticArmPart (partType++, Matrix.Identity )); // rB
            parts.Add(new RoboticArmPart (partType++, t_BC ));
            parts.Add(new RoboticArmPart (partType++, Matrix.Identity )); // rC
            parts.Add(new RoboticArmPart (partType++, t_CD ));
            parts.Add(new RoboticArmPart (partType++, Matrix.Identity )); // rD
            parts.Add(new RoboticArmPart (partType++, t_DE ));
            parts.Add(new RoboticArmPart (partType++, t_EF ));


            YawPitchRoll = new List<float>() { 0, 0, 0 };
        }


        public void UPDATE_PartRotation(float yawMotorRad, float pitchMotorRad, float rollMotorRad)
        {
            this[e_RoboticArmPart.r_D_yaw].transformation = Matrix.RotationY(yawMotorRad);
            this[e_RoboticArmPart.r_C_pitch].transformation = Matrix.RotationX(pitchMotorRad);
            this[e_RoboticArmPart.r_B_roll].transformation = Matrix.RotationZ(rollMotorRad);
        }

        public void CONV_hmdRotationToPartRotation(Matrix playerRotation)
        {
            // converts the head orientation to individual motor angles (Yaw, Pitch, Roll)
            // later inverse kinematics 
            Vector3 values = PostureF.CONV_RotationMatrix_2_YawPitchRollVector3(playerRotation);
            YawRad = -values[0];
            PitchRad = -values[1];
            RollRad = -values[2];
        }

        public void UPDATE_PartRotationAndWorldMatrix(float yawMotorRad, float pitchMotorRad, float rollMotorRad)
        {
            UPDATE_PartRotation(yawMotorRad, pitchMotorRad, rollMotorRad);

            Matrix root = baseWorld;
            foreach (RoboticArmPart p in parts)
            {
                root = p.effect.World = p.transformation * root;
            }
            cameraSurfaceWorld = root;
        }

        public void UPDATE_matrices(Matrix eyeProjection, Matrix eyeView, Matrix eyeWorld)
        {
            baseProjection = eyeProjection;
            baseView = eyeView;
            baseWorld = eyeWorld;
        }

        public void Draw()
        {
            if (draw)
            {
                foreach (RoboticArmPart p in parts)
                {
                    p.Draw(baseView, baseProjection);
                }
            }
            //float scaling = 0.005f;
            //modelAirplane.Draw(GraphicsDevice, Matrix.Scaling(0.0001f / scaling) * eyeWorld, eyeView, eyeProjection);
        }
    }

    /// <summary>
    /// Basler camera part
    /// </summary>
    public partial class TelepresenceSystem : Game
    {
        RoboticArm ra;
        public void Update_RobotArmWantedAnglesFromPlayer()
        {
            ra.CONV_hmdRotationToPartRotation(config.player.Rotation);
        }

        public static List<C_Value> GetAngleValuesFromMotors_Reference(e_valueType valueType)
        {
            switch (valueType)
            {
                case (e_valueType.wantedValue):
                    return new List<C_Value>() { MainWindow.Ms.Yaw.angleWanted, MainWindow.Ms.Pitch.angleWanted, MainWindow.Ms.Roll.angleWanted };
                case (e_valueType.sentValue):
                    return new List<C_Value>() { MainWindow.Ms.Yaw.angleSent, MainWindow.Ms.Pitch.angleSent, MainWindow.Ms.Roll.angleSent };
                case (e_valueType.seenValue):
                    return new List<C_Value>() { MainWindow.Ms.Yaw.angleSeen, MainWindow.Ms.Pitch.angleSeen, MainWindow.Ms.Roll.angleSeen };
                default:
                    return null;
            }
        }

        public static List<C_Value> GetAngleValuesFromMotors(e_valueType valueType)
        {
            switch (valueType)
            {
                case (e_valueType.wantedValue):
                    return new List<C_Value>() { 
                        new C_Value(MainWindow.Ms.Yaw.angleWanted), 
                        new C_Value(MainWindow.Ms.Pitch.angleWanted), 
                        new C_Value(MainWindow.Ms.Roll.angleWanted)
                    };
                case (e_valueType.sentValue):
                    return new List<C_Value>() { 
                        new C_Value(MainWindow.Ms.Yaw.angleSent), 
                        new C_Value(MainWindow.Ms.Pitch.angleSent), 
                        new C_Value(MainWindow.Ms.Roll.angleSent)
                    };
                case (e_valueType.seenValue):
                    return new List<C_Value>() { 
                        new C_Value(MainWindow.Ms.Yaw.angleSeen),
                        new C_Value(MainWindow.Ms.Pitch.angleSeen), 
                        new C_Value(MainWindow.Ms.Roll.angleSeen)
                    };
                default:
                    return null;
            }
        }

        public void Update_RoboticArmDrawnPosture()
        {
            ra.UPDATE_matrices(eyeProjection, eyeView, eyeWorld);

            List<C_Value> yawPitchRoll;

            if (config.roboticArmPostureOnCameraCapture == false)
            {
                yawPitchRoll = GetAngleValuesFromMotors(ra.angleType);
            }
            else
            {
                yawPitchRoll = config.cameraControl.YawPitchRollOnCapture;
            }


            ra.UPDATE_PartRotationAndWorldMatrix(
                (float)yawPitchRoll[0].Rad_FromDefault,
                (float)yawPitchRoll[1].Rad_FromDefault,
                (float)yawPitchRoll[2].Rad_FromDefault                
                );

            HUD.AppendLine(string.Format("READ YawPitchRoll[deg] [{0,7:0.00}|{1,7:0.00}|{2,7:0.00}]",
                (float)yawPitchRoll[0].Dec_FromDefault,
                (float)yawPitchRoll[1].Dec_FromDefault,
                (float)yawPitchRoll[2].Dec_FromDefault
                ));
        }
        public void Draw_RoboticArm()
        {
            ra.Draw();
        }

        private void LoadContent_RoboticArm(float camTextureSizeX)
        {
            ra = new RoboticArm(camTextureSizeX);
            ra.draw = config.draw.RoboticArm;

            float sizeX = 5;
            var roboticArmPartDefaultTexture = Content.Load<Texture2D>("vut_grid");
            Color[] cols = new Color[]
            {               
                Color.Black, Color.Blue, Color.Red, Color.Green, Color.Yellow, Color.Magenta, Color.Cyan, Color.White//, Color.Purple, Color.LimeGreen, Color.Aquamarine
            };

            int q = 0;
            foreach( RoboticArmPart part in ra.parts)
            {
                //roboticArmParts.Add(ToDisposeContent(GeometricPrimitive.Cylinder.New(GraphicsDevice, sizeX, sizeY)));
                part.anchorBody = ToDisposeContent(GeometricPrimitive.Teapot.New(GraphicsDevice, sizeX));
                part.effect = ToDisposeContent(new BasicEffect(GraphicsDevice));

                part.SETUP_effect( cols[q],
                    roboticArmPartDefaultTexture);
                q++;
                if (q >= cols.Length) { q = 0; }
            }
        }
    }
}
