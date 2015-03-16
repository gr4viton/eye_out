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

namespace singletonwise
{
    public enum e_LogMsgSource
    {
        spi = 0, gui, log, mot
    }
    // singleton



    // Requires using System.Collections.ObjectModel; 
    public class C_LoggingTable : ObservableCollection<C_LogMsg>
    {
        // Creating the Tasks collection in this way enables data binding from XAML.
        
    }

    internal class C_Logger
    {
        //private DataTable dataTable;
        private ObservableCollection<C_LogMsg> itemList { get; set; }

        //C_LoggingTable _tasks = (C_LoggingTable)this.Resources["tasks"];

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // one datagrid for all tabs -> filtering by checkboxes and column [src]
        // sorting filtering.. https://msdn.microsoft.com/en-us/library/ff407126(v=vs.110).aspx
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

            itemList.Add(new C_LogMsg { time = DateTime.UtcNow, src = e_LogMsgSource.log, msg = "Logging system initialized" });
            //...

        }

        // property
        public ObservableCollection<C_LogMsg> Data
        {
            get { return itemList; }
        }




        //public void LOG_motX(byte id, string _msg) { LOG(string.Format("mot{0}", id), _msg); }
        //public void LOG_motX_e(byte id, string _msg) { LOG_type(string.Format("mot{0}", id), _msg,"error"); }


        public void LOG_spi(string _msg) { LOG(e_LogMsgSource.spi, _msg); }
        public void LOG_gui(string _msg) { LOG(e_LogMsgSource.gui, _msg); }
        public void LOG_log(string _msg) { LOG(e_LogMsgSource.log, _msg); }
        public void LOG_mot(string _msg) { LOG(e_LogMsgSource.mot, _msg); }

        public void LOG(e_LogMsgSource _src, string _msg)
        {
            itemList.Add(new C_LogMsg { src = _src, msg = _msg });
//            ObservableCollection<LogMessageRow>.CollectionChanged +=;
        }

        public void LOG_type(e_LogMsgSource _src, string _msg, string _type)
        {
            itemList.Add(new C_LogMsg { src = _src, msg = _msg, type = _type });
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }



    public class C_LogMsg
    {
        public DateTime time { get; set; }
        public e_LogMsgSource src { get; set; }
        public string type { get; set; }
        public string msg { get; set; }
        public C_LogMsg()
        {
            time = DateTime.UtcNow;
            type = "info";
        }
    }
}

