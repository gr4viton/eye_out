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
        public DispatcherTimer timMotorDataRefresh;
        //public event
        public ObservableCollection<C_MotorDataRow> motorData;
        
        //public static event EventHandler motorDataChanged;

        public object dgMotorData_lock;

        private void INIT_dgMotorData()
        {
            motorData = new ObservableCollection<C_MotorDataRow>();

            // add all
            foreach (e_motorDataType _type in Enum.GetValues(typeof(e_motorDataType)))
            {
                motorData.Add(new C_MotorDataRow(_type));
            }

            INIT_dgMotorData_binding();
            INIT_timMotorDataRefresh();
        }

        public void INIT_timMotorDataRefresh()
        {
            timMotorDataRefresh = new DispatcherTimer();
            timMotorDataRefresh.Tick += new EventHandler(timMotorDataRefresh_Tick);
            timMotorDataRefresh.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timMotorDataRefresh.Start();
        }
        
        CollectionViewSource ItemCollectionViewSource_motorData;
        private void timMotorDataRefresh_Tick(object sender, EventArgs e)
        {
            if(C_State.FURTHER(e_stateProg.initialized))
            {
                if (tbtReadPresentPosition.IsChecked  == true)
                {
                    //foreach (C_Motor mot in Ms)
                    {
                        //mot.ORDER_getPosition();
                        //mot.READ(C_DynAdd.LED_ENABLE, 1);
                    }
                    //Ms.Yaw.READ_position
                    Ms.Yaw.READ_position();
                }
                foreach (C_MotorDataRow row in motorData)
                {
                    row.REFRESH();
                }

                // it does not change the datagrid if the motorData is not recreated
                ItemCollectionViewSource_motorData.Source = new ObservableCollection<C_MotorDataRow>(motorData);
                
                
                //EventHandler handler = motorDataChanged;
                //if (handler != null)
                //    handler(null, EventArgs.Empty);
            }
        }
        private void INIT_dgMotorData_binding()
        {
            // binding
            //CollectionViewSource ItemCollectionViewSource_motorData;
            ItemCollectionViewSource_motorData = (CollectionViewSource)(FindResource("ItemCollectionViewSource_motorData"));
            ItemCollectionViewSource_motorData.Source = motorData;

            
            // when binding is changing inner guts of dataGrid from different thread
            dgMotorData_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(motorData, dgMotorData_lock); // for multi-thread updating 
        }

        private void tbtMotorDataRefresh_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (C_State.FURTHER(e_stateProg.initialized))
            {
                timMotorDataRefresh.IsEnabled = (bool)tbtMotorDataRefresh.IsChecked;
            }
        }

        private void tbtMotorDataRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            timMotorDataRefresh.Stop();
        }
}
}
