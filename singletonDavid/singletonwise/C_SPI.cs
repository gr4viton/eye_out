using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace singletonwise
{
    internal class C_SPI
    {
        private static object locker = new object();
        private static SerialPort spi;

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static void SETUP(string portName,int baudRate, Parity parity,int dataBits, StopBits stopBits)
        {
            spi = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

        }
        
        public static void INIT()
        {
            C_SPI.SETUP("COM4", 57600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Initialization
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Open close
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void OPEN_connection()
        {
            
            spi.Open();
        }


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Open close
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Writing
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static bool WriteData(byte[] data)
        {
            lock (locker)
            {
                int q = 10; // try q-times
                while (q>0)
                {
                    if (spi.IsOpen)
                    {
                        WriteSerialPort(data);
                        return true;
                        //responseBuffer = ReadSerialPort(8);
                    }
                    else
                    {
                        OPEN_connection();
                    }
                    q--;
                }
            }
            return false; // should never run as far as to this line
        }

        private static void WriteSerialPort(byte[] data)
        {
            //spi.Write(data, 0, data.Length);
            LOG(
                String.Format("SENT Bytes:{0}.{1}.{2}", data[0], data[1], data[2])
                );
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Writing
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region Reading
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        //..

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion Reading
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.spi, _msg);
        }
    }
    
}
