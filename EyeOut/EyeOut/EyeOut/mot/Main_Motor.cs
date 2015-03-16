using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Data; // datagrid

using System.Collections.ObjectModel; // ObservableCollection
using System.Windows; // Window
using System.Windows.Data; //CollectionViewSource
using System.Windows.Controls; // checkbox

using System.IO.Ports;

using System.Windows.Input; // GUI eventArgs

namespace EyeOut
{
    /// <summary>
    /// Motor - gui
    /// </summary>
    public partial class MainWindow : Window
    {
        private void btnStartMotors_Click(object sender, RoutedEventArgs e)
        {
            // just trial

            // connect serial port and search for motors
            string strCmd = "FF0A11";
            string strCmd_delimited = "FF FE FD 00 01 0A";
            string strDelimiter = " ";

            //C_DynMot.CONV_strHex2byteArray(strCmd2);
            byte[] bys;

            bys = C_CONV.strHex2byteArray(strCmd_delimited, strDelimiter);
            C_CONV.PRINT_byteArray(bys);
            bys = C_CONV.strHex2byteArray(strCmd);
            C_CONV.PRINT_byteArray(bys);
        }

        private Byte ID_fromNUDid()
        {
            //return Convert.ToByte(nudID.Text);
            return nudId;
        }

        private void lsCmdEx_wannaSend(object sender, MouseButtonEventArgs e)
        {
            lsCmdEx_SEND_selected();
        }

        private void lsCmdEx_wannaSend(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Space))
                lsCmdEx_SEND_selected();
        }

        private void btnSendExample_wannaSend(object sender, RoutedEventArgs e)
        {
            actMot.SEND_example(3);
        }


        public void lsCmdEx_SEND_selected()
        {

            if (cbExampleDoubleClick.IsChecked == true)
            {
                actMot.SEND_example(lsCmdEx.SelectedIndex);
                //lsCmdEx_SEND_selected();            
            }
        }
    }
}
