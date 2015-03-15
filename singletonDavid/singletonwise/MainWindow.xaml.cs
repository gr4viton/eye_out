﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


//using System.Windows;
using System.Data; // datagrid

using System.Collections.ObjectModel; // ObservableCollection
using System.ComponentModel; // INotifyPropertyChanged
using System.Collections.Specialized; // NotifyCollectionChangedEventHandler


namespace singletonwise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<LogMessageRow> itemList = new ObservableCollection<LogMessageRow>();
        //List<SomeInfo> arrSomeInfo = new List<SomeInfo>();


        // dataGrid binding = http://www.codeproject.com/Articles/683429/Guide-to-WPF-DataGrid-formatting-using-bindings
        // trullyObservableCollection = http://stackoverflow.com/questions/17211462/wpf-bound-datagrid-does-not-update-items-properties
        public MainWindow()
        {
            C_Motor mot1 = new C_Motor(1);
            InitializeComponent();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //create business data
            //var itemList = new List<StockItem>();

            itemList.Add(new LogMessageRow { time = "now", src = "log", msg = "Logging system initialized" });
            //...
            
            // link business data to CollectionViewSource
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = itemList;

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            itemList.Add(new LogMessageRow { time = "hek", src = "log", msg = "new item" });
            //var data = new LogMessageRow { time = "Test1", device = "Test2", msg = "something happened" };
        }

        

    }

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class LogMessageRow
    {
        public string time { get; set; }
        public string src { get; set; }
        public string msg { get; set; }
    }
}
