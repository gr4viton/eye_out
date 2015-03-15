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

using System.Windows;
using System.Data;

namespace singletonwise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<LogMessageRow> itemList = new List<LogMessageRow>();
        //List<SomeInfo> arrSomeInfo = new List<SomeInfo>();


        // dataGrid binding = http://www.codeproject.com/Articles/683429/Guide-to-WPF-DataGrid-formatting-using-bindings
        public MainWindow()
        {
            C_Motor mot1 = new C_Motor(1);
            InitializeComponent();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //create business data
            //var itemList = new List<StockItem>();
            itemList.Add(new LogMessageRow { time = "Many items", dev = 100, msg = false });
            itemList.Add(new LogMessageRow { time = "Enough items", dev = 10, msg = false });
            //...
            
            //link business data to CollectionViewSource
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = itemList;


        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            itemList.Add(new LogMessageRow { time = "hek", dev = 1, msg = true });
            //var data = new LogMessageRow { time = "Test1", device = "Test2", msg = "something happened" };
        }

    }

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class LogMessageRow
    {
        public string time { get; set; }
        public int dev { get; set; }
        public bool msg { get; set; }
    }
}
