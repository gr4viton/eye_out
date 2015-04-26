using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamController = Basler.Pylon.Controls.WPF.StreamController;
using ImageViewer = Basler.Pylon.Controls.WPF.ImageViewer;

using SharpDX;

namespace EyeOut_Telepresence
{
    /*
    public class C_HUD_item
    {
        bool visibility
        string value
        doubl value
    }
     */
    public class C_HUD
    {
        public bool time;
        public bool compas;
        public bool motorPosture;
        public C_HUD() { }
    }

    public class C_DrawGazeMark
    {
        public bool Oculus = true;
        public bool MotorPostureSent = true;
        public bool MotorPostureSeen = true;
    }

    public class C_Player
    {
        public Player position = new Player();
        //public Matrix position;
        //float positionStep = 0.05f;

        //public void forward()
        //{
        //    position *= positionStep * Matrix.Translation(new Vector3(0, 0, 1));
        //}

        //public void back()
        //{
        //    position *= positionStep * Matrix.Translation(new Vector3(0, 0, -1));

        //}

        //public void up()
        //{
        //    position *= positionStep * Matrix.Translation(new Vector3(0, 1, 0));
        //}

        //public void down()
        //{
        //    position *= positionStep * Matrix.Translation(new Vector3(0, -1, 0));

        //}

        //public void sidestepLeft()
        //{
        //    position *= positionStep * Matrix.Translation(new Vector3(1, 0, 0));

        //}
        
        //public void sidestepRight()
        //{
        //    position *= positionStep * Matrix.Translation(new Vector3(-1, 0, -0));

        //}

        //public C_Player()
        //{
        //}
    }

    // telepresence configurations
    public class TelepresenceSystemConfiguration
    {
        public C_HUD hud;
        public bool WRITE_dataToMotors = false;
        public bool READ_dataFromMotors = false;
        public bool SHOW_helpText = true;

        public bool ReadCameraStream = false;
        public C_DrawGazeMark gazeMark;

        
        public bool firstPass = true;

        public ImageViewer imageViewer;
        public StreamController streamController;

        public C_Player player = new C_Player();

        public TelepresenceSystemConfiguration() { }
    }
}
