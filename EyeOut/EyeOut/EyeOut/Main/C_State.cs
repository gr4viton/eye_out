using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace EyeOut
{
    public enum e_state
    {
        started = 0, initializing, initialized, running, closing, closed
    }
    public enum e_stateMotors
    {
        initializing = 0, ready = 1
    }

    public class C_State
    {
        public static e_state prog;
        public static e_stateMotors mot;

        public C_State()
        {
            prog = e_state.started;
            mot = e_stateMotors.initializing;
        }

        public static void CLOSE_program()
        {
            prog = e_state.closing;

            // Kill serial port
            Thread dexter = new Thread(new ThreadStart(C_SPI.CLOSE_connection)); // the serial (port) killer
            int q = 10; // try to stop it X-times
            while (q > 0)
            {
                dexter.Start();
                System.Threading.Thread.Sleep(500);
                if (C_SPI.spi.IsOpen == false)
                {
                    break;
                }
            }
        }

        public static bool FURTHER(e_stateMotors _comparedState)
        {
            return FURTHER((object)_comparedState, (object)mot);
        }

        public static bool FURTHER(e_state _comparedState)
        {
            return FURTHER((object)_comparedState, (object)prog);
        }

        public static bool FURTHER(object _comparedState, object _actualState)
        {
            return ((int)_actualState >= (int)_comparedState);
        }
        

    }
}
