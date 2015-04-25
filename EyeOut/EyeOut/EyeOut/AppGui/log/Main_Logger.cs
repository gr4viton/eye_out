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

using System.Windows.Input;// mouse doubleClick

using System.Text.RegularExpressions;

namespace EyeOut
{
    /// <summary>
    /// Logger - GUI
    /// </summary>
    public partial class MainWindow : Window
    {

        private object dgLog_lock; // lock for datagrid

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void INIT_logger()
        {
            // base multithread binding, sorting, filtering.. https://msdn.microsoft.com/en-us/library/ff407126(v=vs.110).aspx
            // dataGrid binding = http://www.codeproject.com/Articles/683429/Guide-to-WPF-DataGrid-formatting-using-bindings
            // trullyObservableCollection = http://stackoverflow.com/questions/17211462/wpf-bound-datagrid-does-not-update-items-properties

            // binding
            CollectionViewSource ItemCollectionViewSource_log;
            ItemCollectionViewSource_log = (CollectionViewSource)(FindResource("ItemCollectionViewSource_log"));
            ItemCollectionViewSource_log.Source = C_Logger.Instance.Data;

            // when binding is changing inner guts of dataGrid from different thread
            dgLog_lock = new object(); // lock for datagrid
            BindingOperations.EnableCollectionSynchronization(C_Logger.Instance.Data, dgLog_lock); // for multi-thread updating

            // init filter
            lsLogSrcSelction.SelectAll();

            LOG_filterOut(e_LogMsgSource.spi);
            LOG_filterOut(e_LogMsgSource.spi_got);
            LOG_filterOut(e_LogMsgSource.spi_sent);
            LOG_filterOut(e_LogMsgSource.mot);
            LOG_filterOut(e_LogMsgSource.mot_yaw);
            LOG_filterOut(e_LogMsgSource.mot_pitch);
            LOG_filterOut(e_LogMsgSource.mot_roll);

            LOG_filterOut(e_LogMsgSource.unimportant);

            C_Logger.Instance.START_trimming(Convert.ToInt64(txLogBufferCount.Text));
        }
        public void LOG_filter(e_LogMsgSource src, bool visible)
        {
            if (visible == true)
                LOG_filterIn(src);
            else
                LOG_filterOut(src);
        }

        public void LOG_filterOut(e_LogMsgSource src)
        {
            lsLogSrcSelction.SelectedItems.Remove(src);
        }
        public void LOG_filterIn(e_LogMsgSource src)
        {
            lsLogSrcSelction.SelectedItems.Add(src);
        }

        private void txLogBufferCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumerical(e.Text);
        }


        private static bool IsTextNumerical(string text)
        {
            //Regex regex = new Regex("[^0-9.-]+"); 
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void cbLogSetMaximum_ValueChanged(object sender, RoutedEventArgs e)
        {
            if(C_State.FURTHER(e_stateProg.initialized))
            {
                if (cbLogSetMaximum.IsChecked == true)
                {
                    C_Logger.Instance.START_trimming(Convert.ToInt64(txLogBufferCount.Text));
                }
                else
                {
                    C_Logger.Instance.TrimMsgBuffer = false;
                }
            }
        }
        //private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        //{
        //    if (e.DataObject.GetDataPresent(typeof(String)))
        //    {
        //        String text = (String)e.DataObject.GetData(typeof(String));
        //        if (!IsTextNumerical(text))
        //        {
        //            e.CancelCommand();
        //        }
        //    }
        //    else
        //    {
        //        e.CancelCommand();
        //    }
        //}

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Filtering Checkboxes
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void lsLogSrcSelction_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    lsLogSrcSelction.SelectAll();
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    lsLogSrcSelction.UnselectAll();
                }
            }
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
            {
                e.Accepted = false;
                foreach (e_LogMsgSource src in lsLogSrcSelction.SelectedItems)
                {
                    if (src == t.src)
                    {
                        e.Accepted = true; // If filter is turned on, filter completed items.
                    }
                }
            }

        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Filter Events
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        private void lbLogCount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lbLogCount.Content = C_Logger.Instance.logMsgCount;
        }


        public static void LOG_logger(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.log, _msg);
        }
        public static void LOG_gui(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.gui, _msg);
        }
    }
}
