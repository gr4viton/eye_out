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

namespace cs_event
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
         // declare delegate
    public delegate void d_del1(string msg);

    // producer class
    class C_logger_producer
    {
        // event declaration
        event d_del1 e_changeTextBox;
        // constructor    
        public C_logger_producer(d_del1 h_handle)
        {
            // hook event - register it
            this.e_changeTextBox += new d_del1(h_handle);

            // invoke event (withou thread safety etc..)
            e_changeTextBox("NEWTEXT");
        }
    }

    // consumer class
    public partial class MainWindow : Window
    {

        // instance of producer 
        C_logger_producer instOfProd;

        
        TextBox txt;
        public MainWindow()
        {
            InitializeComponent();
            
            txt = new TextBox();
            instOfProd = new C_logger_producer(this.h_handle);
        }

        private void btnSendCmd_Click(object sender, RoutedEventArgs e)
        {

        }
        // handle function
        public void h_handle(string msg)
        {
            tx.Text = msg;
        }
    }
}
