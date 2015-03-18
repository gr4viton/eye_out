using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Data; // datagrid

using System.Collections.ObjectModel; // ObservableCollection
using System.Windows; // Window
using System.Windows.Data; //CollectionViewSource
using System.Windows.Controls; // checkbox

using System.IO.Ports;

using System.Windows.Input; // GUI eventArgs

namespace EyeOut
{
    /// <summary>
    /// Motor - gui
    /// </summary>
    /// 
    public enum e_rotInd
    {
        roll = 0, pitch = 1, yaw = 2
    }
    public partial class MainWindow : Window
    {
        C_Motor actMot;
        public List<C_Motor> Ms;

        public static Byte nudId = 1;

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properies
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properies
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void INIT_mot()
        {
            actMot = new C_Motor(1);
            /*
            mot = this.Resources["motYawDataSource"] as C_Motor;
            mot.Angle = 20;*/
            foreach (string str in C_Motor.cmdinEx_str)
            {
                lsCmdEx.Items.Add(str);
            }

        }

        public void SEARCH_motors()
        {
            Ms = new List<C_Motor>();
            // send pings and get responses - add items to [Ms] motor list
            // - use local Search motor for pinging and changing of id..
            Byte id = C_DynAdd.ID_MIN;
            C_Motor srchM = new C_Motor(id);

            for (id = C_DynAdd.ID_MIN; id < C_DynAdd.ID_MAX; id++)
            {
                srchM.id = id;
                
                C_SPI.spi.DiscardInBuffer(); 
                
                srchM.ORDER_ping();

                if (C_SPI.READ_cmd()) 
                {
                    MessageBox.Show(id.ToString());
                }
                    
                    /*
                     * No ak to mas threadsafe, tak pred kazdou read/write dvojicou vycisti buffer pre istotu
                    Dal by som normalne port read do cyklu kolkto dat ocakavas
                        Nad to hodis try catch na timeoutexception
                    Ak mas response, metoda vrati true, ak exceptiom false
                     */
            }
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private void btnStartMotors_Click(object sender, RoutedEventArgs e)
        {
            // just trial

            // connect serial port and search for motors
            string strCmd = "FF0A11";
            string strCmd_delimited = "FF FE FD 00 01 0A";
            string strDelimiter = " ";

            //C_DynMot.CONV_strHex2byteArray(strCmd2);
            byte[] bys;

            bys = C_Motor.strHex2byteArray(strCmd_delimited, strDelimiter);
            C_Motor.PRINT_byteArray(bys);
            bys = C_Motor.strHex2byteArray(strCmd);
            C_Motor.PRINT_byteArray(bys);


            MessageBox.Show(actMot.angle.Dec.ToString());
        }

        private Byte ID_fromNUDid()
        {
            //return Convert.ToByte(nudID.Text);
            return nudId;
        }

        private void lsCmdEx_wannaSend(object sender, MouseButtonEventArgs e)
        {
            lsCmdEx_SEND_selected();
        }

        private void lsCmdEx_wannaSend(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Space))
                lsCmdEx_SEND_selected();
        }

        private void btnSendExample_wannaSend(object sender, RoutedEventArgs e)
        {
            actMot.SEND_example(3);
        }


        public void lsCmdEx_SEND_selected()
        {
            if (cbExampleDoubleClick.IsChecked == true)
            {
                actMot.SEND_example(lsCmdEx.SelectedIndex);
                //lsCmdEx_SEND_selected(); 
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Angle sliders
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void UPDATE_angles()
        {
            actMot.angle.Dec = slYaw.Value;
            //actMot.speed.Dec = slYawSpeed.Value;

            actMot.ORDER_move();
        }

        private void slYaw_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slYaw.Value = Math.Round(e.NewValue, 2);
            UPDATE_angles();
        }

        private void slPitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slPitch.Value = Math.Round(e.NewValue, 2);
            UPDATE_angles();
        }

        private void slYaw_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UPDATE_angles();
        }

        private void slPitch_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UPDATE_angles();
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Angle sliders
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private void btnSearch4motors_Click(object sender, RoutedEventArgs e)
        {
            SEARCH_motors();
        }


    }
}
