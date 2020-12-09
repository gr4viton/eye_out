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

namespace cs_event_MVVM_moje
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // declare delegate
    public delegate void d_del1(string msg);


    public class C_controlLog
    {

    }
    // LOGGER obsluha
    public partial class MainWindow : Window
    {
        // instance of producer 
        C_logger logMot;
        // event declaration
        event d_del1 e_changeTextBox;

        public void INIT_LOG()
        {
            logMot = new C_logger();
        }
    }

    // consumer class
    public partial class MainWindow : Window
    {
        //TextBox txt;
        public MainWindow()
        {
            InitializeComponent();

            INIT_LOG();

            // hook event - register it
            // e_changeTextBox += new d_del1(h_handle);


            //txt = new TextBox();
            //instOfProd = new C_logger_producer(this.h_handle);
        }

        private void btnSendCmd_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(logMot.text);
            logMot.text = "ddd";
            MessageBox.Show(logMot.text);
            logMot.text = "ASD";
            MessageBox.Show(logMot.text);
        }
        // handle function
        public void h_handle(string msg)
        {
            tx.Text = msg;
        }
    }
}
