using System;
using System.Collections.Generic;
using System.Linq; // Last
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
using System.Globalization; // cultureinfo tostring

namespace EyeOut
{
    /*
     * spi = Serial Peripheral Interface
     * gui = Graphical User Interface
     * mot = Motor
     * cam = Camera
     * TP = Telepresence
     * valConv = Value conversion
     */
    public enum e_LogMsgSource
    {
        cam, cam_err,
        oculus, oculus_err,
        packet, packInstruct, packStatus, 
        spi, spi_sent, spi_got, spi_err,
        gui, log, 
        mot, mot_yaw, mot_pitch, mot_roll,
        byteReg,
        valConv, debug, unimportant
    }
    public enum e_LogMsgType
    {
        info = 0, warning = 1, error = 2, godsWill = 42
    }


    // singleton
    internal class C_Logger
    {
        //private DataTable dataTable;
        private ObservableCollection<C_LogMsg> msgList { get; set; }

        //private List<C_LogMsg> msgListBuffer { get; set; } 
        private int trimTriggerCount = 50; // trim the log only after its count is logMsgCount + trimTriggerCount = to be easy on collection event handlers


        private static object msgList_locker = new object();

        private static byte errorAntiLoopCounter = 0;
        private const byte errorAntiLoopCounter_max = 10; 
        public long logMsgCount { get; private set; }

        private bool trimMsgBuffer = false;
        public bool TrimMsgBuffer 
        {
            get { return trimMsgBuffer; }
            set
            {
                if (logMsgCount > 0)
                    trimMsgBuffer = value;
                else
                    trimMsgBuffer = false;
            } 
        }

        public void START_trimming(long _logMsgCountMaximum)
        {
            logMsgCountMaximum = _logMsgCountMaximum;
            TrimMsgBuffer = true;
        }

        public long logMsgCountMaximum = 0;
        //C_LoggingTable _tasks = (C_LoggingTable)this.Resources["tasks"];

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // one datagrid for all tabs -> filtering by checkboxes and column [src]
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
            msgList = new ObservableCollection<C_LogMsg>();
            //msgListBuffer = new List<C_LogMsg>();
            //dataTable = new DataTable();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            //create business data
            //var itemList = new List<StockItem>();

            msgList.Add(new C_LogMsg { time = DateTime.UtcNow, src = e_LogMsgSource.log, 
                msg = "Logging system initialized! Time is [Ascending] == new messages on first row == ^^^^^^" });
            //...

        }

        // property
        public ObservableCollection<C_LogMsg> Data
        {
            get { return msgList; }
        }

        public void LOG(C_LogMsg _msg)
        {
            ADD_toBuffer(_msg);
        }

        public void LOG(e_LogMsgSource _src, string _msg)
        {
            ADD_toBuffer(new C_LogMsg { src = _src, msg = _msg, type = e_LogMsgType.info });
        }

        public void LOG_err(e_LogMsgSource _src, string _msg)
        {
            ADD_toBuffer(new C_LogMsg { src = _src, msg = _msg, type = e_LogMsgType.error });
        }

        public void LOG_type(e_LogMsgSource _src, string _msg, e_LogMsgType _type)
        {
            ADD_toBuffer(new C_LogMsg { src = _src, msg = _msg, type = _type });
        }

        //public string filePath = @"log.txt";
        public string filePath = @"C:\log_eyeOut.txt";
        private System.IO.StreamWriter file;
        private bool fileIsOpen = false;
        public void OPEN_file(string filePath)
        {
            file = new System.IO.StreamWriter(filePath,true);
            file.Close();
            file = new System.IO.StreamWriter(filePath, true);
            fileIsOpen = true;
        }
        public void ADD_bufferToList()
        {

        }
        public void ADD_toBuffer(C_LogMsg _logMsg)
        {
#if (!DEBUG)
            // don't log unimportant and log msgs when not debugging
            if ((_logMsg.src == e_LogMsgSource.unimportant)
                ||
                (_logMsg.src == e_LogMsgSource.debug))
            {
                return;
            }
#endif
            //if (fileIsOpen == false)
            //{
            //    OPEN_file(filePath);
            //}

            // loger must trigger its collection update after some reasonable intervals - otherwise the program stops due to too quick collection updates triggers and handlers

            lock (msgList_locker)
            {
                try
                {
                    if (msgList.Last().time == _logMsg.time) // multiple at the same time
                    {
                        _logMsg.queue = msgList.Last().queue + 1;
                        //_logMsg.queue = msgListBuffer.Last().queue + 1;
                    }

                    msgList.Add(_logMsg);
                    //msgListBuffer.Add(_logMsg);


                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
                    {
                        file.WriteLine(_logMsg.ToString());
                    }

                    if( trimMsgBuffer == true)
                    {
                        if (logMsgCount > logMsgCountMaximum + trimTriggerCount)
                        {
                            msgList = new ObservableCollection<C_LogMsg>(
                                msgList.Skip((int)(logMsgCountMaximum - logMsgCount))
                                );
                            logMsgCount = logMsgCountMaximum;
                        }
                        else if (logMsgCount == logMsgCountMaximum)
                        {
                            msgList.RemoveAt(0);
                        }
                        else
                        {
                            logMsgCount++;
                        }
                    }
                    else
                    {   
                        logMsgCount++;
                    }
                    errorAntiLoopCounter = 0;
                }
                catch (Exception e)
                {
                    string err_str = string.Format("Cannot add item to dataGrid:\n{0}\n{1}", e.Data, e.Message);
                    errorAntiLoopCounter++;
                    if (errorAntiLoopCounter < errorAntiLoopCounter_max)
                    {
                        // try to log it again
                        LOG_err(e_LogMsgSource.log, err_str);
                    }
                    else
                    {
                        Console.WriteLine(err_str);
                        errorAntiLoopCounter = 0;
                    }
                }
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }



    public class C_LogMsg
    {
        public DateTime time { get; set; }
        public int queue { get; set; } // if more messages arrive at the exact same time
        public e_LogMsgSource src { get; set; }
        public e_LogMsgType type { get; set; }
        public string msg { get; set; }
        public C_LogMsg()
        {
            time = DateTime.UtcNow.ToLocalTime();
            type = e_LogMsgType.info;
            queue = 0;
        }
        public override string ToString()
        {
            return string.Format("{0}\t|{1}\t|{2}\t|{3}\t|{4}", 
                time.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), 
                queue, type, src, msg);
        }
    }
}

