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
        public MainWindow()
        {
            C_Motor mot1 = new C_Motor(1);
            InitializeComponent();


            //create business data
            //var itemList = new List<StockItem>();
            itemList.Add(new StockItem { Name = "Many items", Quantity = 100, IsObsolete = false });
            itemList.Add(new StockItem { Name = "Enough items", Quantity = 10, IsObsolete = false });
            //...

            //link business data to CollectionViewSource
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = itemList;

        }

        List<StockItem> itemList = new List<StockItem>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            itemList.Add(new StockItem { Name = "hek", Quantity = 1, IsObsolete = true });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // dataGrid binding = http://www.codeproject.com/Articles/683429/Guide-to-WPF-DataGrid-formatting-using-bindings
            var data = new LogMessageRow { time = "Test1", device = "Test2", msg = "something happened" };

            dgLogMot.Items.Add(data);
        }

    }
    public class StockItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public bool IsObsolete { get; set; }
    } 

    public class LogMessageRow
    {
        public string time { get; set; }
        public string device { get; set; }
        public string msg { get; set; }
    }
}
