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
using System.Data; //DataGrid
using System.Collections.ObjectModel; // ObservableCollection

namespace EyeOut
{
    public enum e_LogMsgSource
    {
        spi, spi_sent, spi_got, gui, log, mot
    }
    public enum e_LogMsgType
    {
        info = 0, error = 1, godsWill = 42
    }


    // singleton
    internal class C_Logger
    {
        //private DataTable dataTable;
        private ObservableCollection<C_LogMsg> itemList { get; set; }

        //C_LoggingTable _tasks = (C_LoggingTable)this.Resources["tasks"];

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // one datagrid for all tabs -> filtering by checkboxes and column [src]
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        private static C_Logger instance;


        // singleton
        public static C_Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new C_Logger();
                }
                return instance;
            }
        }

        private C_Logger()
        {
            itemList = new ObservableCollection<C_LogMsg>();
            //dataTable = new DataTable();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //create business data
            //var itemList = new List<StockItem>();

            itemList.Add(new C_LogMsg { time = DateTime.UtcNow, src = e_LogMsgSource.log, msg = "Logging system initialized! Time is [Ascending] == new messages on first row == ^^^^^^" });
            //...

        }

        // property
        public ObservableCollection<C_LogMsg> Data
        {
            get { return itemList; }
        }

        public void LOG(C_LogMsg _msg)
        {
            itemList.Add(_msg);
        }

        public void LOG(e_LogMsgSource _src, string _msg)
        {
            itemList.Add(new C_LogMsg { src = _src, msg = _msg, type = e_LogMsgType.info });
        }

        public void LOG_err(e_LogMsgSource _src, string _msg)
        {
            itemList.Add(new C_LogMsg { src = _src, msg = _msg, type = e_LogMsgType.error });
        }

        public void LOG_type(e_LogMsgSource _src, string _msg, e_LogMsgType _type)
        {
            itemList.Add(new C_LogMsg { src = _src, msg = _msg, type = _type });
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }



    public class C_LogMsg
    {
        public DateTime time { get; set; }
        public e_LogMsgSource src { get; set; }
        public e_LogMsgType type { get; set; }
        public string msg { get; set; }
        public C_LogMsg()
        {
            time = DateTime.UtcNow;
            type = e_LogMsgType.info;
        }
    }
}

