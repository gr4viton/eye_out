using System;
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
//using System.ComponentModel; // INotifyPropertyChanged
//using System.Collections.Specialized; // NotifyCollectionChangedEventHandler

//using System.Threading.Tasks;


namespace singletonwise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List<SomeInfo> arrSomeInfo = new List<SomeInfo>();

        //dataGrid
        private object daraGrid_lock = new object(); // lock for datagrid
        C_Motor mot1;

        // dataGrid binding = http://www.codeproject.com/Articles/683429/Guide-to-WPF-DataGrid-formatting-using-bindings
        // trullyObservableCollection = http://stackoverflow.com/questions/17211462/wpf-bound-datagrid-does-not-update-items-properties
        public MainWindow()
        {
            mot1 = new C_Motor(1);
            InitializeComponent();


            INIT_logger();
            INIT_spi();
        }

        public void INIT_spi()
        {
            C_SPI.INIT();
        }
        private void INIT_logger()
        {
            // link business data to CollectionViewSource
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = C_Logger.Instance.Data;
            BindingOperations.EnableCollectionSynchronization(C_Logger.Instance.Data, daraGrid_lock); // for multi-thread updating

            //BindingOperations.EnableCollectionSynchronization(itemCollectionViewSource, 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            C_Logger.Instance.LOG_gui("SOMETHING HAPPENED");
            //var data = new LogMessageRow { time = "Test1", device = "Test2", msg = "something happened" };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            byte[] b = new byte[3];
            b[0] = 1;
            b[1] = 2;
            b[2] = 3;
            mot1.SEND_cmd(b);
        }

        private void CompleteFilter_Changed(object sender, RoutedEventArgs e)
        {
            // Refresh the view to apply filters.
            CollectionViewSource.GetDefaultView(dgLog.ItemsSource).Refresh();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            /*
            Task t = e.Item as Task;
            if (t != null)
            // If filter is turned on, filter completed items.
            {
                if (this.cbCompleteFilter.IsChecked == true && t.Completed== true)
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }
             */ 
        }

    }
}
