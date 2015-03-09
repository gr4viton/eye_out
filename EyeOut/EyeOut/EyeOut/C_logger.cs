using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{

    public class C_controlLog
    {
        C_logger logMot;

        public event d_LOG_logger2gui event_LOG_msg2logger;


        public C_controlLog()
        {
            logMot = new C_logger(C_events.e_logger.logMot);
            
        }

        public void h_LOG_msg2logger(C_events.e_logger logger, C_events.e_how how, string msg)
        {
            switch (logger)
            {
                case (C_events.e_logger.logMot):
                    logMot.UPDATE_text(how, msg);
                    break;
            }
        }

    }
    public class C_logger
    {
        public string text;
        C_events.e_logger logger;

        string logSent;
        string logRec;
        public static bool logChanged = false;


        public event d_LOG_logger2gui event_LOG_logger2gui;

        public C_logger(C_events.e_logger _logger)
        {
            text = "";
            logger = _logger;
        }

        // use onlythis function to change text 
        public void UPDATE_text(C_events.e_how how, string msg)
        {
            switch (how)
            {
                case (C_events.e_how.renew):
                    text = msg;
                    break;
                case (C_events.e_how.appendLine):
                    text += msg + "\r\n";
                    break;
                case (C_events.e_how.append):
                    text += msg;
                    break;
            }
            event_LOG_logger2gui(logger, how, msg);
        }

        private void LOG_msg2logger(C_events.e_how how, string msg)
        {
            UPDATE_text(how, msg);
        }

        public void CLEAR()
        {
            UPDATE_text( C_events.e_how.renew, "");
            
        }

        



    }

    public class C_loggerGot : C_logger
    {
        public C_loggerGot()
            : base(C_events.e_logger.logMotGot)
        {
        }
    }
}
