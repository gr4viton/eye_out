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
using System.Windows.Controls.Primitives; // ischedcked

//using System.IO.Ports;
using System.Windows.Input; // GUI eventArgs
using System.Windows.Threading; // dispatcherTimer


using System.Runtime.InteropServices;

namespace EyeOut
{
    /// <summary>
    /// MotorData - gui
    /// </summary>
    public partial class MainWindow : Window
    {
        // dgMotorData
        public DispatcherTimer timMotorDataRead;
        CollectionViewSource ItemCollectionViewSource_motorData;

        public object dgMotorData_lock;
        public static ObservableCollection<C_MotorDataRow> motorData;
        
                
        private void INIT_dgMotorData()
        {
            motorData = new ObservableCollection<C_MotorDataRow>();

            // add all defined values
            foreach (e_motorDataType _type in Enum.GetValues(typeof(e_motorDataType)))
            {
                if (_type != e_motorDataType.regByteValue)
                {
                    motorData.Add(new C_MotorDataRow(_type));
                }
            }

            // add whole shadow register (Readable & Writable)
            foreach (e_regByteType regByteType in Enum.GetValues(typeof(e_regByteType)))
            {
                foreach (byte address in Ms.Yaw.Reg.GET_byteAddresses(regByteType))
                {
                    motorData.Add(new C_MotorDataRow(address, regByteType));
                }
            }

            INIT_dgMotorData_binding();
            INIT_timMotorDataRefresh();
        }

        public void INIT_timMotorDataRefresh()
        {
            timMotorDataRead = new DispatcherTimer();
            timMotorDataRead.Tick += new EventHandler(timMotorDataRefresh_Tick);
            timMotorDataRead.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timMotorDataRead.Start();
        }

        private void btnReadLEDvalue_Click(object sender, RoutedEventArgs e)
        {
            Ms.Yaw.READ(C_DynAdd.LED_ENABLE, 1);
        }

        public static void REFRESH_motorData()
        {
            foreach (C_MotorDataRow row in MainWindow.motorData)
            {
                row.REFRESH();
            }
        }
        private void timMotorDataRefresh_Tick(object sender, EventArgs e)
        {
            READ_selectedData();
        }

        private void lsReadMotorDataPart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            READ_selectedData();
        }

        private void READ_selectedData()
        {
            if (C_State.FURTHER(e_stateProg.initialized))
            {
                foreach (int index in lsReadPresentPositionMotors.SelectedItems)
                {
                    if (lsiWholeRegister.IsSelected == true)
                    {
                        Ms[index].READ_wholeRegister();
                    }
                    if (lsiActualPostionEtc.IsSelected == true)
                    {
                        Ms[index].READ_positionSpeedLoadVoltageTemperatureRegisteredInstructionMoving();
                    }
                }
            }
        }
        private void INIT_dgMotorData_binding()
        {
            // binding
            ItemCollectionViewSource_motorData = (CollectionViewSource)(FindResource("ItemCollectionViewSource_motorData"));
            ItemCollectionViewSource_motorData.Source = motorData;

            // when binding is changing inner guts of dataGrid from different thread
            dgMotorData_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(motorData, dgMotorData_lock); // for multi-thread updating 
        }

        private void tbtActiveReadPresentPosition_Click(object sender, RoutedEventArgs e)
        {
            if (C_State.FURTHER(e_stateProg.initialized))
            {
                timMotorDataRead.IsEnabled = (bool)tbtActiveReadPresentPosition.IsChecked;
            }
        }

        private void btnRefreshMotorData_Click(object sender, RoutedEventArgs e)
        {
            REFRESH_motorData();
        }


        private void btnReadPresentPostionYaw_Click(object sender, RoutedEventArgs e)
        {
            Ms.Yaw.READ_position();
        }
        private void btnReadPresentPostionPitch_Click(object sender, RoutedEventArgs e)
        {
            Ms.Pitch.READ_position();
        }

        private void slActiveReadingTimerInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(C_State.FURTHER(e_stateProg.initialized))
            {
                timMotorDataRead.Interval = new TimeSpan(0, 0, 0, 0, (int)slActiveReadingTimerInterval.Value);
            }
        }

    }
}
