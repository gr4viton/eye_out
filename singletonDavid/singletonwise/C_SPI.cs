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

        
        public static void INIT(string portName,int baudRate, Parity parity,int dataBits, StopBits stopBits)
        {
            spi = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        }

        public static void OPEN_connection()
        {
            spi.Open();
        }
        public static bool WriteData(byte[] data)
        {
            lock (locker)
            {
                if (spi.IsOpen)
                {
                    WriteSerialPort(data);
                    //responseBuffer = ReadSerialPort(8);
                }
                else
                {
                    OPEN_connection();
                }
                
            }
            return true;
        }

        private static void WriteSerialPort(byte[] data)
        {
            //spi.Write(data, 0, data.Length);
            //Console.WriteLine(String.Format("{0}{1}{2}", data[0], data[1], data[2]));
            C_Logger.Instance.LOG_spi("SPI HAPPENED");
        }
    }
    
}
