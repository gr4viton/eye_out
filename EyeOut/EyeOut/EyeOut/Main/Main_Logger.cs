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

namespace EyeOut
{


    /// <summary>
    /// Logger - GUI
    /// </summary>
    public partial class MainWindow : Window
    {

        private object daraGrid_lock; // lock for datagrid

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void INIT_logger()
        {
            // base multithread binding, sorting, filtering.. https://msdn.microsoft.com/en-us/library/ff407126(v=vs.110).aspx
            // dataGrid binding = http://www.codeproject.com/Articles/683429/Guide-to-WPF-DataGrid-formatting-using-bindings
            // trullyObservableCollection = http://stackoverflow.com/questions/17211462/wpf-bound-datagrid-does-not-update-items-properties

            // binding
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = C_Logger.Instance.Data;

            // when binding is changing inner guts of dataGrid from different thread
            daraGrid_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(C_Logger.Instance.Data, daraGrid_lock); // for multi-thread updating

            // init filter
            LogFilterSetAll(true);
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Filtering Checkboxes
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void LogFilterSetAll(bool val)
        {
            if (spLogFilterCheckboxes.IsInitialized)
            {
                foreach (CheckBox cb in spLogFilterCheckboxes.Children)
                {
                    cb.IsChecked = val;
                }
            }
        }

        private void LogFilterCheckAll(object sender, RoutedEventArgs e)
        {
            LogFilterSetAll(true);
        }

        private void LogFilterUncheckAll(object sender, RoutedEventArgs e)
        {
            LogFilterSetAll(false);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Filtering Checkboxes
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Filter Event
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void CompleteFilter_Changed(object sender, RoutedEventArgs e)
        {
            // Refresh the view to apply filters.
            CollectionViewSource.GetDefaultView(dgLog.ItemsSource).Refresh();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            C_LogMsg t = e.Item as C_LogMsg;
            if (t != null)
            // If filter is turned on, filter completed items.
            {
                if (
                    (this.cbLogShowSpi.IsChecked == false && t.src == e_LogMsgSource.spi) ||
                    (this.cbLogShowGui.IsChecked == false && t.src == e_LogMsgSource.gui) ||
                    (this.cbLogShowMot.IsChecked == false && t.src == e_LogMsgSource.mot)
                    )
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }

        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Filter Events
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        public static void LOG_logger(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.log, _msg);
        }
    }
}
