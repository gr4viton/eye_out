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
        C_Motor actMot2;
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

        public void SET_sliderLimits(Slider sl, C_Value val)
        {
            sl.Maximum = val.DecMax;
            sl.Value = val.Dec;
            sl.Minimum = val.DecMin;
        }
        public void INIT_mot()
        {

            C_Value angleFull = new C_Value(0, 360, C_DynAdd.SET_GOAL_POS_MIN, C_DynAdd.SET_GOAL_POS_MAX * 4);
            //C_Value speedFull = new C_Value(0, 100, C_DynAdd.SET_MOV_SPEED_MIN, C_DynAdd.SET_MOV_SPEED_MAX, 20);
            C_Value speedFull = new C_Value(0, 101, C_DynAdd.SET_MOV_SPEED_NOCONTROL, C_DynAdd.SET_MOV_SPEED_MAX, 5); // no control as 0

            C_Value angleYaw = new C_Value(0, 360, C_DynAdd.SET_GOAL_POS_MIN, C_DynAdd.SET_GOAL_POS_MAX * 4);
            C_Value speedYaw = new C_Value(speedFull);

            C_Value anglePitch = new C_Value(0, 360, C_DynAdd.SET_GOAL_POS_MIN, C_DynAdd.SET_GOAL_POS_MAX * 4);
            C_Value speedPitch = new C_Value(speedFull);

            Ms = new List<C_Motor>();

            // Motor Yaw
            Ms.Add ( new C_Motor(
                1, 
                new C_Value(0, 360, C_DynAdd.SET_GOAL_POS_MIN, C_DynAdd.SET_GOAL_POS_MAX * 4, 0), // angle
                new C_Value(0, 101, C_DynAdd.SET_MOV_SPEED_NOCONTROL, C_DynAdd.SET_MOV_SPEED_MAX, 5) // speed
                ));
            // Motor Pitch
            Ms.Add(new C_Motor(
                1,
                new C_Value(0, 360, C_DynAdd.SET_GOAL_POS_MIN, C_DynAdd.SET_GOAL_POS_MAX * 4, 0), // angle
                new C_Value(0, 101, C_DynAdd.SET_MOV_SPEED_NOCONTROL, C_DynAdd.SET_MOV_SPEED_MAX, 5) // speed
                ));
            // Motor 
            Ms.Add(new C_Motor(
                1,
                new C_Value(0, 360, C_DynAdd.SET_GOAL_POS_MIN, C_DynAdd.SET_GOAL_POS_MAX * 4, 0), // angle
                new C_Value(0, 101, C_DynAdd.SET_MOV_SPEED_NOCONTROL, C_DynAdd.SET_MOV_SPEED_MAX, 5) // speed
                ));

            actMot = new C_Motor(1, angleYaw, speedYaw);
            actMot2 = new C_Motor(2, anglePitch, speedPitch);
            /*
            mot = this.Resources["motYawDataSource"] as C_Motor;
            mot.Angle = 20;*/
            foreach (string str in C_Motor.cmdinEx_str)
            {
                lsCmdEx.Items.Add(str);
            }

            SET_sliderLimits(slAngleYaw, angleYaw);
            SET_sliderLimits(slSpeedYaw, speedYaw);
            SET_sliderLimits(slAnglePitch, anglePitch);
            SET_sliderLimits(slSpeedPitch, speedPitch);

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
            actMot.angle.Dec = slAngleYaw.Value;
            actMot.speed.Dec = slSpeedYaw.Value;

            actMot2.angle.Dec = slAnglePitch.Value;
            actMot2.speed.Dec = slSpeedPitch.Value;

            if (cbSendValuesToMotorYaw.IsChecked == true)
            {
                actMot.ORDER_move();
            }
            if (cbSendValuesToMotorPitch.IsChecked == true)
            {
                //actMot2.ORDER_move();
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sl = sender as Slider;
            if (sl!=null)
            {
                sl.Value = Math.Round(e.NewValue, 2);
                UPDATE_angles();
            }
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
