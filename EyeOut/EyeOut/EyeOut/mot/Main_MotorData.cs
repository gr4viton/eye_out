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

//using System.IO.Ports;
using System.Windows.Input; // GUI eventArgs
using System.Windows.Threading; // dispatcherTimer

namespace EyeOut
{
    /// <summary>
    /// MotorData - gui
    /// </summary>
    public partial class MainWindow : Window
    {
        // dgMotorData
        public ObservableCollection<C_MotorDataRow> motorData;
        public object dgMotorData_lock;
        public DispatcherTimer timMotorDataRefresh;

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
            //timMotorDataRefresh = new DispatcherTimer();
            timMotorDataRefresh = new DispatcherTimer(
                DispatcherPriority.Background, Application.Current.Dispatcher);
            timMotorDataRefresh.Tick += new EventHandler(timMotorDataRefresh_Tick);
            timMotorDataRefresh.Interval = new TimeSpan(0, 0, 0, 100);
        }

        private void timMotorDataRefresh_Tick(object sender, EventArgs e)
        {
            foreach (C_MotorDataRow row in motorData)
            {
                row.REFRESH();
            }
        }
        private void INIT_dgMotorData_binding()
        {
            // binding
            CollectionViewSource ItemCollectionViewSource_motorData;
            ItemCollectionViewSource_motorData = (CollectionViewSource)(FindResource("ItemCollectionViewSource_motorData"));
            ItemCollectionViewSource_motorData.Source = motorData;

            // when binding is changing inner guts of dataGrid from different thread
            dgMotorData_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(motorData, dgMotorData_lock); // for multi-thread updating
        }

        private void tbtMotorDataRefresh_Checked(object sender, RoutedEventArgs e)
        {
            timMotorDataRefresh.Start();
        }

        private void tbtMotorDataRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            timMotorDataRefresh.Stop();
        }
}
}
