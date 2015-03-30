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
        public C_HUD()
        {
            time = false;
            compas = false;
        }
    }

    // telepresence configurations
    public class C_TP_config
    {
        public C_HUD hud;

        public C_TP_config()
        {
            hud = new C_HUD();
        }
    }


    public partial class MainWindow : Window
    {
        public static C_TP_config TP_config;
        public static C_Telepresence TP_program;

        public void INIT_TP()
        {
            TP_config = new C_TP_config();
            TP_config.hud.time = true;
        }

        public void START_TP()
        {
            C_Telepresence.LOG("Starting EyeOut telepresence\nby Daniel Davídek 2015");
            if (MessageBox.Show("I hereby confirm?\n"+
                "[x] There is enaugh free space around the servomotor-arm!\n"+
                "[x] No-one is standing in the dangerous distance!",
                "Safety warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                C_Telepresence.LOG("Starting canceled.");
                return;
            }
            else
            {
                HMDType hmdType = HMDType.DK1;
                // send active config
                using (TP_program = new C_Telepresence(C_Camera.actualId, hmdType))
                {
                    TP_program.Run();
                }
            }
        }

        public void STOP_TP()
        {
            if (TP_program.IsRunning)
            {
                TP_program.Exit();
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


        private void btnToggleTP_Click(object sender, RoutedEventArgs e)
        {
            if (btnToggleTP.IsChecked == true)
            {
                btnToggleTP.Content = "Stop Telepresence";
                btnToggleTP.Background = System.Windows.Media.Brushes.MediumPurple;
                START_TP();
            }
            else
            {
                STOP_TP();
                btnToggleTP.Content = "Start Telepresence";
                btnToggleTP.Background = System.Windows.Media.Brushes.GreenYellow;
            }
        }

    }
}
