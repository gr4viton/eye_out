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

        //public event d_LOG_logger_2gui event_LOG_logger_2gui;
        //public event d_LOG_msg_2logger event_LOG_msg_2logger;


        public C_controlLog(d_LOG_logger_2gui _h_LOG_logger_2gui)
        {
            logMot = new C_logger(e_logger.logMot, _h_LOG_logger_2gui);
            //event_LOG_logger_2gui 
            //this.event_LOG_msg_2logger += new d_LOG_msg_2logger(h_LOG_msg_2logger);
            
        }

        public void h_LOG_msg_2logger(e_logger logger, e_how how, string msg)
        {
            switch (logger)
            {
                case (e_logger.logMot):
                    logMot.UPDATE_text(how, msg);
                    break;
            }
        }

    }
    public class C_logger
    {
        public string text;
        e_logger logger;

        string logSent;
        string logRec;
        public static bool logChanged = false;


        public event d_LOG_logger_2gui event_LOG_logger_2gui;

        public C_logger(e_logger _logger, d_LOG_logger_2gui _h_LOG_logger_2gui)
        {
            text = "";
            logger = _logger;
            event_LOG_logger_2gui += new d_LOG_logger_2gui(_h_LOG_logger_2gui);
        }


        // use onlythis function to change text 
        public void UPDATE_text(e_how how, string msg)
        {
            switch (how)
            {
                case (e_how.renew):
                    text = msg;
                    break;
                case (e_how.appendLine):
                    text += msg + "\r\n";
                    break;
                case (e_how.append):
                    text += msg;
                    break;
            }
            event_LOG_logger_2gui(logger, how, msg);
        }

        private void LOG_msg_2logger(e_how how, string msg)
        {
            UPDATE_text(how, msg);
        }

        public void CLEAR()
        {
            UPDATE_text( e_how.renew, "");
            
        }

        



    }
    /*
    public class C_loggerGot : C_logger
    {
        public C_loggerGot()
            : base(e_logger.logMotGot)
        {
        }
    }*/
}
