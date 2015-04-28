﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel; // ObservableCollection
using System.Windows; // Window
using System.Windows.Data; //CollectionViewSource
using System.Windows.Controls; // checkbox

using System.IO.Ports;

using System.Windows.Input; // GUI eventArgs

//using SharpDX;
using SharpOVR; // hmdType

using TelepresenceSystem = EyeOut_Telepresence.TelepresenceSystem;

using TelepresenceSystemConfiguration = EyeOut_Telepresence.TelepresenceSystemConfiguration;
using EyeOut_Telepresence;

using StreamController = Basler.Pylon.Controls.WPF.StreamController;
using ImageViewer = Basler.Pylon.Controls.WPF.ImageViewer;


namespace EyeOut
{
    /// <summary>
    /// Oculus - gui
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public static TelepresenceSystemConfiguration TP_config;
        public static TelepresenceSystem TP_program;

        public void INIT_TelepresenceConfigAndPrepareGui()
        {
            lsLogSrcSelction.UnselectAll();
            LOG_filterIn(e_LogMsgSource.oculus);
            LOG_filterIn(e_LogMsgSource.oculus_err);

            TP_config = new TelepresenceSystemConfiguration()
            {
                ReadCameraStream = cbReadCameraStream.IsChecked.Value,

                WRITE_dataToMotors = cbWriteMotorData.IsChecked.Value,
                READ_dataFromMotors = cbReadMotorData.IsChecked.Value,
                hud = new C_HUD()
                {
                    time = cbHudTime.IsChecked.Value,
                    compas = cbHudCompas.IsChecked.Value,
                    motorPosture = cbHudMotorPosture.IsChecked.Value,

                    helpMenu = cbHudHelpMenu.IsChecked.Value,                    
                    toolStrip = cbHudToolStrip.IsChecked.Value,                    

                    gazeMark = new C_gazeMark()
                    {
                        Oculus = cbDrawOculusGaze.IsChecked.Value,
                        MotorPostureSent = cbDrawMotorPostureSent.IsChecked.Value,
                        MotorPostureSeen = cbDrawMotorPostureSeen.IsChecked.Value,
                    }
                },

                draw = new C_SceneDraw()
                {
                    SkySurface = cbTelepresence_SkySurface.IsChecked.Value,
                    RoboticArm = cbTelepresence_RoboticArm.IsChecked.Value
                },

                //hmdType = HMDType.DK1,

                streamController = guiStreamController,
                imageViewer = guiImageViewer,
                //imageViewer = new ImageViewer()
                //imageViewer = new Basler.Pylon.Controls.WPF.ImageViewer()
                guiDispatcher = guiImageViewer.Dispatcher
            };
            

            //guiStreamController.Dispose();

            //TP_config.imageViewer = new ImageViewer();
            //TP_config.streamController = new StreamController();

            TP_config.streamController.BeginInit();
            TP_config.imageViewer.BeginInit();
            if (guiStreamController.Camera != null)
            {
                // take selected camera
                TP_config.streamController.Camera = guiStreamController.Camera;
            }
            else
            {
                if (cameraListener.Camera != null)
                {
                    // take selected camera
                    TP_config.streamController.Camera = cameraListener.Camera;
                }
                else
                {
                    int res = 0;
                    foreach (var cameraModel in cameraListener.CameraList)
                    {
                        
                        if(res == 0)
                        {
                            TP_config.streamController.Camera = cameraModel.Camera;
                        }
                        res++;
                    }
                    if (TP_config.streamController.Camera != null)
                    {
                        // inform that this telepresence will use the first camera from connected camera list
                        if (cbSafe_NoCameraSelectedWarning.IsChecked.Value == true)
                        {
                            MessageBox.Show("No Basler camera selected in 'Basler camera' tab!\nThis telepresence session will use the first one from the camera list:\n"
                            + TP_config.streamController.Camera.CameraInfo.ToString());
                        }
                    }
                    if (res == 0) // no cameras found
                    {
                        // inform that the camera is not going to be assigned in this telepresence settings as the camera is not connected
                        MessageBox.Show("No Basler camera found!\nThere will be no streamed camera image in this telepresence session!\n"
                            + "Please connect Basler camera to some port (USB3.0 for Basler acA2040-90uc) and check whether it is found in 'Basler camera' tab!",
                            "No Basler camera found!", MessageBoxButton.OK);
                    }
                }
            }

            TP_config.imageViewer.Camera = TP_config.streamController.Camera;
            TP_config.streamController.ActiveViewer = TP_config.imageViewer;

            TP_config.streamController.Camera.Open();

            
            //Basler.Pylon.Controls.WPF.IRenderer renderer;
            //TP_config.imageViewer.Renderer = renderer;
            //TP_config.imageViewer.SnapsToDevicePixels = true;
                //Basler.Pylon.Controls.WPF.InteropBitmapRenderer


            TP_config.streamController.EndInit();
            TP_config.imageViewer.EndInit();

            TP_config.streamController.Camera.StreamGrabber.GrabStarted += grabStarted();

            TP_config.hud.time = true;

            KILL_allNotNeededGui();
        }

        public EventHandler<EventArgs> grabStarted()
        {
            return null;
        }


        public void KILL_allNotNeededGui()
        {
            if (C_State.FURTHER(e_stateWebCam.ready))
            {
                timCam.Stop();
            }
            timMotorDataRead.Stop();
            timSim.Stop();

            // unload Basler camera gui elements not to interfere with the telepresence ones
            //guiImageViewer.Visibility = System.Windows.Visibility.Hidden;
            guiImageViewer.IsEnabled = false;
            guiStreamController.IsEnabled = false;
        }
        public void START_TP_withCaution()
        {
            INIT_TelepresenceConfigAndPrepareGui();


            TelepresenceSystem.LOG("Starting EyeOut telepresence\nby Daniel Davídek 2015");
            if (cbSafe_Warning.IsChecked == true)
            {
                if (MessageBox.Show("I hereby confirm?\n" +
                    "[x] There is enaugh free space around the servomotor-arm!\n" +
                    "[x] No-one is standing in the dangerous distance!",
                    "Safety warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    TelepresenceSystem.LOG("Starting canceled.");
                    tbtToggleTP.IsChecked = false;
                    return;
                }
                else
                {
                    START_TP(TP_config);
                }
            }
            else
            {
                START_TP(TP_config);
            }
        }

        public void START_TP(TelepresenceSystemConfiguration TP_config)
        {
            using (TP_program = new TelepresenceSystem(TP_config))
            {
                TP_program.Run();
                END_TPsettings();
            }
        }

        public void STOP_TP()
        {
            if (TP_program != null)
            {
                if (TP_program.IsRunning)
                {
                    TP_program.Exit();
                    END_TPsettings();
                    return;
                }
            }
            TelepresenceSystem.LOG_err("The Telepresence session is not running");
        }

        public void END_TPsettings()
        {
            System.Windows.Forms.Cursor.Show(); // not working
            TelepresenceSystem.LOG("The Telepresence session is stopped");
            tbtToggleTP.IsChecked = false;
        }
        
        private void tbtToggleTP_Click(object sender, RoutedEventArgs e)
        {
            if (tbtToggleTP.IsChecked == true)
            {
                START_TP_withCaution();
            }
            else
            {
                STOP_TP();
                tbtToggleTP.Content = "Start Telepresence";
                tbtToggleTP.Background = System.Windows.Media.Brushes.GreenYellow;
                System.Windows.Forms.Cursor.Show();
            }
        }

        private void tbtToggleTP_ValueChanged(object sender, RoutedEventArgs e)
        {

            if (tbtToggleTP.IsChecked == true)
            {
                tbtToggleTP.Content = "Stop Telepresence";
                tbtToggleTP.Background = System.Windows.Media.Brushes.MediumPurple;
            }
            else
            {
                tbtToggleTP.Content = "Start Telepresence";
                tbtToggleTP.Background = System.Windows.Media.Brushes.GreenYellow;
            }
        }

    }
}
