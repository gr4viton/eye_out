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
        List<StockItem> itemList = new List<StockItem>();
        List<SomeInfo> arrSomeInfo = new List<SomeInfo>();


        public MainWindow()
        {
            C_Motor mot1 = new C_Motor(1);
            InitializeComponent();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //create business data
            //var itemList = new List<StockItem>();
            itemList.Add(new StockItem { Name = "Many items", Quantity = 100, IsObsolete = false });
            itemList.Add(new StockItem { Name = "Enough items", Quantity = 10, IsObsolete = false });
            //...
            
            //link business data to CollectionViewSource
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = itemList;

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            //arrSomeInfo = new List<SomeInfo>;
            this.grdMailbag.ItemsSource = arrSomeInfo; //Didn't worked
            this.grdMailbag.DataContext = arrSomeInfo;  // Didn't worked
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
         /*   
    Persons persons = new Persons();
persons.Add(new Parson("New","Person");
dataGrid1.DataContext = persons;*/
        }


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

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            arrSomeInfo.Add(new SomeInfo { Name = "asdasd", Description = "asd" });
            //this.grdMailbag.ItemsSource = arrSomeInfo; //Didn't worked
            //this.grdMailbag.DataContext = arrSomeInfo;  // Didn't worked
        }

    }
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class Persons : List<Person>
    {
        // Parameterless constructor      
        public Persons()
        {
        }
        public new void Add(Person parson)
        {
            base.Add(parson);
        }
    }  

    public class Person
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }

        public Person(string lastName, string firstName)
        {
            LastName = lastName;
            FirstName = firstName;
        }
    }
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class StockItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public bool IsObsolete { get; set; }
    }
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class SomeInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ID { get; set; }
    }
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class LogMessageRow
    {
        public string time { get; set; }
        public string device { get; set; }
        public string msg { get; set; }
    }
}
