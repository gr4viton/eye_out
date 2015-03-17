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

namespace EyeOut
{
    /// <summary>
    /// SPI - gui
    /// </summary>
    public partial class MainWindow : Window
    {
        private void INIT_spi()
        {
            SPI_rescanPorts();
            INIT_GUI_lsBaudRate();

            /*
            SPI_UPDATE_portName();
            SPI_UPDATE_baudRate();*/
        }

        private void INIT_GUI_lsBaudRate()
        {
            int[] brs = { 9600, 57600, 1000000 };
            foreach (int br in brs)
            {
                lsBaudRate.Items.Add(Convert.ToString(br));
            }
            lsBaudRate.SelectedIndex = 1;
        }
        private void SPI_rescanPorts()
        {
            string[] allPorts;
            try
            {
                /*
                mainState = E_GUI_All_MainState.port_ScanningPorts;
                GUI.SC.portState = E_PortState.notConnected;
                GUI_SC_UpdateToolStrips();
                */

                cbPort.Items.Clear();
                allPorts = SerialPort.GetPortNames();
                

                foreach (string port in allPorts)
                {
                    //if(port[port.Length] != 'o')
                    cbPort.Items.Add(port);
                }

                //cbPort.SelectedIndex = 0;
                cbPort.SelectedIndex = cbPort.Items.Count - 1;
            }
            catch (Exception)//Win32Exception)
            {/*
                MessageBox.Show(fStringResources.msg_port_NoCOMPortFound.Text);
                GUI.SC.portState = E_PortState.noPortsFound;
                GUI_SC_UpdateToolStrips();
              */
            }

        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {            /*
            if (C_SPI.act_con_status != e_con.port_opened)
                WANNA_SPI_OpenConnection();
            else
                WANNA_SPI_CloseConnection();*/
        }
        private void btnRescanPort_Click(object sender, RoutedEventArgs e)
        {
            SPI_rescanPorts();
        }


        /*
        public void SPI_UPDATE_baudRate()
        {
            C_SPI.spi.BaudRate = Convert.ToInt32(txBaudRate.Text); //COM Port Sp
        }
        public void SPI_UPDATE_portName()
        {
            //SPI.PortName = "COM4"; //Com Port Name     
            if (cbPort.SelectedIndex != -1)
            {
                string port = cbPort.SelectedValue.ToString();
                if (string.IsNullOrEmpty(port) == false)
                    C_SPI.spi.PortName = port; //Com Port Name                
            }
        }
        */


    }
}
