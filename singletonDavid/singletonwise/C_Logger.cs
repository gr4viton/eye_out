using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace singletonwise
{
    // singleton
    internal class C_Logger
    {
        private DataTable dataTable;
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
            dataTable = new DataTable();
        }

        // property
        public DataTable Data
        {
            get { return dataTable; }
        }
    }
}

