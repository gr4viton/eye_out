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

    // telepresence configurations
    public class TelepresenceSystemConfiguration
    {
        public C_HUD hud;
        public bool WRITE_dataToMotors = false;
        public bool READ_dataFromMotors = false;
        public bool SHOW_helpText = true;
        public bool drawSkySurface = true;

        public bool ReadCameraStream = false;
        public C_DrawGazeMark gazeMark;

        
        public ImageViewer imageViewer;
        public StreamController streamController;

        public Player player = new Player();

        public TelepresenceSystemConfiguration() { }
    }
}
