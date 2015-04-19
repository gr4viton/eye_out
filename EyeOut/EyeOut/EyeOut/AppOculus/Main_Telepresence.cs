using System;
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

namespace EyeOut
{
    /// <summary>
    /// Oculus - gui
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public static TelepresenceSystemConfiguration TP_config;
        public static C_Telepresence TP_program;

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
                gazeMark = new C_DrawGazeMark()
                {
                    Oculus = (bool)cbDrawOculusGaze.IsChecked,
                    MotorPostureSent = cbDrawMotorPostureSent.IsChecked.Value,
                    MotorPostureSeen = cbDrawMotorPostureSeen.IsChecked.Value,
                },
                hud = new C_HUD()
                {
                    time = cbHudTime.IsChecked.Value,
                    compas = cbHudCompas.IsChecked.Value,
                    motorPosture = cbHudMotorPosture.IsChecked.Value
                },

                streamController = guiStreamController
            };
            TP_config.hud.time = true;

            KILL_allNotNeededGui();
        }

        public void KILL_allNotNeededGui()
        {
            if (C_State.FURTHER(e_stateWebCam.ready))
            {
                timCam.Stop();
            }
            timMotorDataRead.Stop();
            timSim.Stop();
        }
        public void START_TP_withCaution()
        {
            INIT_TelepresenceConfigAndPrepareGui();


            C_Telepresence.LOG("Starting EyeOut telepresence\nby Daniel Davídek 2015");
            if (cbSafe_Warning.IsChecked == true)
            {
                if (MessageBox.Show("I hereby confirm?\n" +
                    "[x] There is enaugh free space around the servomotor-arm!\n" +
                    "[x] No-one is standing in the dangerous distance!",
                    "Safety warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    C_Telepresence.LOG("Starting canceled.");
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
            HMDType hmdType = HMDType.DK1;
            // send active config
            //using (TP_program = new C_Telepresence(C_Camera.actualId, hmdType))
            //{
            //    //TP_program.config = TP_config;
            //    TP_program.Run();
            //}

            using (var program = new TelepresenceSystem(TP_config))
            {
                program.Run();
            }
        }

        public void STOP_TP()
        {
            if (TP_program.IsRunning)
            {
                TP_program.Exit();
                System.Windows.Forms.Cursor.Show(); // not working
                C_Telepresence.LOG("The Telepresence session is stopped");
            }
            else
            {
                C_Telepresence.LOG_err("The Telepresence session is not running");
            }
        }

        public void START_TP_Demo()
        {
            //RiftGame.LOG("Starting Demo of Oculus w/SharpDX & SharpOVR libraries\nby Guy Godin 2014");
            //using (var TP_Demo_program = new RiftGame())
            //{
            //    TP_Demo_program.Run();
            //}
        }

        private void btnStartOculusDemo_Click(object sender, RoutedEventArgs e)
        {
            START_TP_Demo();
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
