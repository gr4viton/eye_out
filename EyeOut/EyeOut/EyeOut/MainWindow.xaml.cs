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

namespace EyeOut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            int[] brs = { 9600, 57600, 1000000 };
            foreach (int br in brs)
            {
                lsBaudRate.Items.Add(Convert.ToString(br));
            }
            lsBaudRate.SelectedIndex = 1;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            txMotLog.Text = C_controlMot.baudRate_str;
            //txMotLog.Text = selectedItem.Content.ToString();
        }

        private ListBoxItem selectedItem;
        public ListBoxItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                /*
                RaisePropertyChanged("SelectedItem");

                var thePanel = new StackPanel();
                thePanel = ((((((selectedItem as ListBoxItem).Parent as ListBox).Parent as StackPanel).Parent as Grid).Parent as StackPanel).Parent as MainWindow).thePanel;
                string message;
                message = selectedItem.ToString();

                if (message == "Data entries")
                {
                    var allEntries = new CategoryViewModel();

                    foreach (var category in (thePanel.DataContext as UserViewModel).Categories)
                        allEntries.Entries = new ObservableCollection<EntryViewModel>(category.Entries);

                    thePanel.Children.Clear();
                    thePanel.Children.Add(new EntriesUC(allEntries));
                }
                // implementation for all the other list items...
                 * */
            }
        }

    }
}
