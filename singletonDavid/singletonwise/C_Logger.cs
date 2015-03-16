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
    // singleton
    internal class C_Logger
    {
        //private DataTable dataTable;
        private ObservableCollection<LogMessageRow> itemList;
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
            itemList = new ObservableCollection<LogMessageRow>();
            //dataTable = new DataTable();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //create business data
            //var itemList = new List<StockItem>();

            itemList.Add(new LogMessageRow { time = DateTime.UtcNow, src = "log", msg = "Logging system initialized" });
            //...

        }

        // property
        public ObservableCollection<LogMessageRow> Data
        {
            get { return itemList; }
        }

        
        
        public void LOG_spi(string _msg) { LOG("spi", _msg); }
        public void LOG_gui(string _msg) { LOG("gui", _msg); }
        public void LOG_log(string _msg) { LOG("log", _msg); }
        public void LOG_mot(string _msg) { LOG("mot", _msg); }

        public void LOG_motX(byte id, string _msg) { LOG(string.Format("mot{0}", id), _msg); }

        public void LOG_motX_e(byte id, string _msg) { LOG_type(string.Format("mot{0}", id), _msg,"error"); }

        public void LOG(string _src, string _msg)
        {
            itemList.Add(new LogMessageRow { src = _src, msg = _msg });
        }

        public void LOG_type(string _src, string _msg, string _type)
        {
            itemList.Add(new LogMessageRow { src = _src, msg = _msg, type = _type });
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public class LogMessageRow
        {
            public DateTime time { get; set; }
            public string src { get; set; }
            public string type { get; set; }
            public string msg { get; set; }
            public LogMessageRow()
            {
                time = DateTime.UtcNow;
                type = "info";
            }
        }
    }
}

