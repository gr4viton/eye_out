﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamController = Basler.Pylon.Controls.WPF.StreamController;
using ImageViewer = Basler.Pylon.Controls.WPF.ImageViewer;

using SharpDX;
using EyeOut;

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
    public class C_gazeMark
    {
        public bool Oculus = true;
        public bool MotorPostureSent = true;
        public bool MotorPostureSeen = true;
    }

    public class C_HUD
    {
        public bool time;
        public bool compas;
        public bool motorPosture;

        public bool helpMenu = true;
        public bool toolStrip = true;
        public C_gazeMark gazeMark;
        public C_HUD() { }
    }

    public class C_SceneDraw
    {
        public bool SkySurface = true;
        public bool RoboticArm = true;
    }


    // telepresence configurations
    public class TelepresenceSystemConfiguration
    {
        public C_HUD hud;
        public C_SceneDraw draw;
        public bool WRITE_dataToMotors = false;
        public bool READ_dataFromMotors = false;

        public bool ReadCameraStream = false;


        public bool motorSpeedControl;
        
        public Player player = new Player();

        public int[] cameraFrameQueueLengthList = new int[] { 1, 2, 5, 12, 20, 50 };
        public int cameraFrameQueueLength = 1;

        public BaslerCameraControl cameraControl;
        //public System.Windows.Threading.Dispatcher guiDispatcher;
        public TelepresenceSystemConfiguration() { }
    }
}
