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
//using MainWindow.Ms;

namespace EyeOut
{
    /// <summary>
    /// Motor - gui
    /// </summary>
    public partial class MainWindow : Window
    {
        public static C_MotorControl Ms;

        public static Byte nudId = 1;
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properies
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properies
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void INIT_allMotors()
        {
            Ms = new C_MotorControl();
            //Ms.INIT_individualMotors();
            
            // set slider limits
            SET_allSlidersLimits();
            
            // Examples
            INIT_examples();

            // update position
            UPDATE_slidersFromMotors();

            lsChosenMotor.SelectedIndex = 0;
            lsCmdEx.SelectedIndex = 0;

            INIT_dgMotorData();
            C_State.mot = e_stateMotor.ready;
            //UPDATE_motorsFromSliders();
            //UPDATE_slidersFromMotors();
        }
        public void INIT_examples()
        {
            foreach (string str in C_Motor.cmdEx_str)
            {
                lsCmdEx.Items.Add(str);
            }
        }


        public void SEARCH_motors()
        {
            // should be in C_Motor
            // should use background worker

            // send pings and get responses - add items to [Ms] motor list
            // - use local Search motor for pinging and changing of id..
            Byte id = C_DynAdd.ID_MIN;
            C_Motor srchM = new C_Motor(id);
            C_Packet querryPacket;

            for (id = C_DynAdd.ID_MIN; id < C_DynAdd.ID_MAX; id++)
            {
                srchM.id = id;
                
                C_SPI.spi.DiscardInBuffer(); 
                
                srchM.ORDER_ping();

                querryPacket = new C_Packet(srchM, C_DynAdd.INS_PING);
                C_Packet.SEND_packet(querryPacket);

                /*
                if (querryPacket.returnProcessed)
                {

                }*/
                //if (C_SPI.READ_packet(querryPacket) 
                //{
                //    MessageBox.Show(id.ToString());
                //}
                    
            }
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion INIT
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private void btnStartMotors_Click(object sender, RoutedEventArgs e)
        {
            foreach (C_Motor m in Ms)
            {
                UPDATE_motorFromSlider(m.rotMotor);
                //m.angle.Dec = 160;
                m.REGISTER_move();
            }
            C_Motor.ORDER_ActionToAll();
        }


        private void btnResetMotors_Click(object sender, RoutedEventArgs e)
        {
            foreach (C_Motor m in Ms)
            {
                m.angle.RESET_toDefault();
                m.speed.RESET_toDefault();
                //UPDATE_sliderFromMotor(m.rotationMotor);
                m.REGISTER_move();
            }
            C_Motor.ORDER_ActionToAll();
            /*
            foreach (C_Motor m in Ms)
            {
                UPDATE_sliderFromMotor(m.rotationMotor);
            }*/
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region examples
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private void lsChosenMotor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsChosenMotor.SelectedIndex == -1)
            {
                lsChosenMotor.SelectedIndex = 0;
            }
            Ms.actMrot = (e_rot)lsChosenMotor.SelectedIndex;
        }

        private void lsCmdEx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cbExampleDoubleClick.IsChecked == true)
            {
                SEND_selectedCmdEx();
            }
        }

        private void lsCmdEx_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Space))
            {   
                if (cbExampleDoubleClick.IsChecked == true)
                {
                    SEND_selectedCmdEx();
                }
            }
        }

        private void btnSendExample_Click(object sender, RoutedEventArgs e)
        {
            SEND_selectedCmdEx();
        }

        public void SEND_selectedCmdEx()
        {
            if (lsCmdEx.SelectedIndex == -1)
            {
                LOG_gui("No command example selected!");
            }
            else
            {
                Ms.ActualMotor.SEND_example(lsCmdEx.SelectedIndex);
            }
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion examples
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Angle & Speed sliders
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public void SET_sliderLimits(Slider sl, C_Value val)
        {
            sl.Maximum = val.DecMax;
            sl.Value = val.Dec;
            sl.Minimum = val.DecMin;
        }
        public void SET_allSlidersLimits()
        {
            foreach (e_rot rot in Enum.GetValues(typeof(e_rot)))
            {
                SET_sliderLimits(GET_slSpeed(rot), Ms.GET_M(rot).speed);
                SET_sliderLimits(GET_slAngle(rot), Ms.GET_M(rot).angle);
            }
        }
        //private void UPDATE_motorsFromSliders()
        //{
        //    //foreach(e_rot
        //}
        private void UPDATE_motorFromSlider(e_rot rot) //nn
        {
            if (C_State.FURTHER(e_stateMotor.ready))
            {
                Ms.GET_M(rot).angle.Dec = GET_slAngle(rot).Value;
                Ms.GET_M(rot).speed.Dec = GET_slSpeed(rot).Value;
            }
        }

        private void UPDATE_slidersFromMotors()
        {
            foreach (e_rot rot in Enum.GetValues(typeof(e_rot)))
            {
                UPDATE_slidersFromMotor(rot);
            }
        }

        private void UPDATE_slidersFromMotor(e_rot rot)  //nn
        {
            if (C_State.FURTHER(e_stateMotor.ready))
            { 
                GET_slSpeed(rot).Value = Ms.GET_M(rot).speed.Dec;
                GET_slAngle(rot).Value = Ms.GET_M(rot).angle.Dec;
                //LOG_logger(string.Format("{0} = angle[{1}], speed[{2}]", rot, M(rot).angle.Dec, M(rot).speed.Dec));
            }
        }

        private void ORDER_moveIfChecked(e_rot rot)
        {
            if (GET_cbSendValuesToMotor(rot).IsChecked == true)
            {
                Ms.GET_M(rot).ORDER_move();
                Ms.GET_M(rot).ORDER_getPosition();
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (C_State.FURTHER(e_stateMotor.ready))
            {
                Slider sl = sender as Slider;
                if (sl != null)
                {
                    sl.Value = Math.Round(e.NewValue, 2);
                    e_rot rot = GET_rot(sl);
                    UPDATE_motorFromSlider(rot);
                    UPDATE_slidersFromMotor(rot);
                    ORDER_moveIfChecked(rot);
                }
            }
        }


        private void cbSetValues_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (C_State.FURTHER(e_stateMotor.ready))
            {
                CheckBox cb = sender as CheckBox;
                if (cb == cbSendValuesToMotorYaw)
                {
                    ORDER_moveIfChecked(e_rot.yaw);
                }
                else if (cb == cbSendValuesToMotorPitch)
                {
                    ORDER_moveIfChecked(e_rot.pitch);
                }
                else if (cb == cbSendValuesToMotorRoll)
                {
                    ORDER_moveIfChecked(e_rot.roll);
                }
            }
        }

        private e_rot GET_rot(Slider sl)
        {
            if ((sl == slAngleYaw) || (sl == slSpeedYaw))
            {
                return e_rot.yaw;
            }
            else if ((sl == slAnglePitch) || (sl == slSpeedPitch))
            {
                return e_rot.pitch;
            }
            else if ((sl == slAngleRoll) || (sl == slSpeedRoll))
            {
                return e_rot.roll;
            }
            return e_rot.yaw;
        }


        public Slider GET_slSpeed(e_rot rot)
        {
            switch (rot)
            {
                case (e_rot.yaw):
                    return slSpeedYaw;
                case (e_rot.pitch):
                    return slSpeedPitch;
                case (e_rot.roll):
                    return slSpeedRoll;
            }
            return null;
        }
        public Slider GET_slAngle(e_rot rot)
        {
            switch (rot)
            {
                case (e_rot.yaw):
                    return slAngleYaw;
                case (e_rot.pitch):
                    return slAnglePitch;
                case (e_rot.roll):
                    return slAngleRoll;
            }
            return null;
        }
        public CheckBox GET_cbSendValuesToMotor(e_rot rot)
        {
            switch (rot)
            {
                case (e_rot.yaw):
                    return cbSendValuesToMotorYaw;
                case (e_rot.pitch):
                    return cbSendValuesToMotorPitch;
                case (e_rot.roll):
                    return cbSendValuesToMotorRoll;
            }
            return null;
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
