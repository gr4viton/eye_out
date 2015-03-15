using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data; //DataGrid
using System.Collections.ObjectModel; // ObservableCollection

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
using System.Data; // datagrid
using System.Collections.ObjectModel; 

namespace singletonwise
{
    // singleton
    internal class C_Logger
    {
        //private DataTable dataTable;
        private ObservableCollection<LogMessageRow> itemList;

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

            itemList.Add(new LogMessageRow { time = "now", src = "log", msg = "Logging system initialized" });
            //...

        }

        // property
        public ObservableCollection<LogMessageRow> Data
        {
            get { return itemList; }
        }

        public void LOG_msg(string _msg)
        {
            // add time for msg
            itemList.Add(new LogMessageRow { time = "hek", src = "log", msg = _msg });
        }


        public void LOG_msg_mot(string _msg)
        {
            // add time for msg
            itemList.Add(new LogMessageRow { time = "hek", src = "mot", msg = _msg });
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public class LogMessageRow
        {
            public string time { get; set; }
            public string src { get; set; }
            public string msg { get; set; }
        }
    }
}

