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

        C_Motor mot1;

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
        
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LOG_gui("SOMETHING HAPPENED");
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

        public void LOG_gui(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.gui, _msg);
        }
    }
}
