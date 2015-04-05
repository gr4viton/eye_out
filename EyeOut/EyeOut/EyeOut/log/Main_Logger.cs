﻿using System;
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
            
            lsLogSrcSelction.SelectedItems.Remove(e_LogMsgSource.spi);
            lsLogSrcSelction.SelectedItems.Remove(e_LogMsgSource.spi_got);
            lsLogSrcSelction.SelectedItems.Remove(e_LogMsgSource.spi_sent);
            lsLogSrcSelction.SelectedItems.Remove(e_LogMsgSource.mot);
            lsLogSrcSelction.SelectedItems.Remove(e_LogMsgSource.mot_yaw);
            lsLogSrcSelction.SelectedItems.Remove(e_LogMsgSource.mot_pitch);
            lsLogSrcSelction.SelectedItems.Remove(e_LogMsgSource.mot_roll);
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Filtering Checkboxes
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private void LogFilterCheckAll(object sender, RoutedEventArgs e)
        {
            lsLogSrcSelction.SelectAll();    
        }

        private void LogFilterUncheckAll(object sender, RoutedEventArgs e)
        {
            lsLogSrcSelction.UnselectAll();
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