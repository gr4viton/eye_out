using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EyeOut
{
    public class C_controlMot
    {
        public static string baudRate_str;
        private int baudRate_val;


        //raises exception
        public string _BaudRate
        {
            get { return baudRate_str; }
            set
            {
                baudRate_str = value;
                //this.baudRate_str = value;
                /*
                if (!this.CHECK_level_and_numOfTickets(this.level, value))
                {
                    throw new ApplicationException(
                        "Máš moc malej level na to abys mohl mít tolik vstupenek!");
                }
                if (this.numOfTickets <= 0)
                {
                    throw new ApplicationException(
                        "Kup si něco na sebe! aspoň jednu vstupenku.. vogo");
                }*/
            }
        }
    }
}
