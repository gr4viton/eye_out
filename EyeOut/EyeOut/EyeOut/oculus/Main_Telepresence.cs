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

namespace EyeOut
{
    /// <summary>
    /// Oculus - gui
    /// </summary>
    /// 
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
        public C_HUD() {}
    }

    public class C_DrawGazeMark
    {
        public bool Oculus = true;
        public bool MotorPostureSent = true;
        public bool MotorPostureSeen = true;
    }

    // telepresence configurations
    public class C_TP_config
    {
        public C_HUD hud;
        public bool WRITE_dataToMotors = false;
        public bool READ_dataFromMotors = false;
        public C_DrawGazeMark gazeMark;

        public C_TP_config() {}
    }


    public partial class MainWindow : Window
    {
        public static C_TP_config TP_config;
        public static C_Telepresence TP_program;

        public void INIT_TP()
        {
            TP_config = new C_TP_config()
            {
                WRITE_dataToMotors = (bool)cbWriteMotorData.IsChecked,
                READ_dataFromMotors = (bool)cbReadMotorData.IsChecked,
                gazeMark = new C_DrawGazeMark()
                {
                    Oculus = (bool)cbDrawOculusGaze.IsChecked,
                    MotorPostureSent = (bool)cbDrawMotorPostureSent.IsChecked,
                    MotorPostureSeen = (bool)cbDrawMotorPostureSeen.IsChecked,
                },
                hud = new C_HUD()
                {
                    time = (bool)cbHudTime.IsChecked,
                    compas = (bool)cbHudCompas.IsChecked,
                    motorPosture = (bool)cbHudMotorPosture.IsChecked
                }
            };
            TP_config.hud.time = true;

            KILL_allNotNeededGui();
        }

        public void KILL_allNotNeededGui()
        {
            timCam.Stop();
            timMotorDataRead.Stop();
            timSim.Stop();
        }
        public void START_TP_withCaution()
        {
            INIT_TP();
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

        public void START_TP(C_TP_config TP_config)
        {
            HMDType hmdType = HMDType.DK1;
            // send active config
            using (TP_program = new C_Telepresence(C_Camera.actualId, hmdType))
            {
                TP_program.config = TP_config;
                TP_program.Run();
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
            RiftGame.LOG("Starting Demo of Oculus w/SharpDX & SharpOVR libraries\nby Guy Godin 2014");
            using (var TP_Demo_program = new RiftGame())
            {
                TP_Demo_program.Run();
            }
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
