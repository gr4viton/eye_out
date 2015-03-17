using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace EyeOut
{
    public enum e_state{
        running = 0, closing
    }

    public class C_State
    {
        public static e_state prog;

        public C_State()
        {
            prog = e_state.running;
        }

        public static void CLOSE_program()
        {
            prog = e_state.closing;

            Thread dexter = new Thread(new ThreadStart(C_SPI.CLOSE_connection)); // the serial_killer
            int q = 10; // try to stop it X-times
            while (q > 0)
            {
                dexter.Start();
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
