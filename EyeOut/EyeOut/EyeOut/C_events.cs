using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Threading;

namespace EyeOut
{


    public class C_events
    {

        
        public enum e_logger
        {
            logAll = 0, logMot, logMotGot, logMotSent,logCam, logOculus
        }
        public enum e_how
        {
            renew = 0, appendLine, append
        }

        
        /*
        public event d_LOG_msg_2logger event_LOG_msg_2logger;

        public event d_LOG_logger_2gui event_LOG_logger_2gui;

        public event d_SEND_bytes2serial event_SPI_bytes2serial_send;
        */
    }
}
