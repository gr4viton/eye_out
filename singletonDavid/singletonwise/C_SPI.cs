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

        public static void SETUP(string portName,int baudRate, Parity parity,int dataBits, StopBits stopBits)
        {
            spi = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

        }
        
        public static void INIT()
        {
            C_SPI.SETUP("COM4", 57600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        }

        public static void OPEN_connection()
        {
            
            spi.Open();
        }
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
            //Console.WriteLine(String.Format("{0}{1}{2}", data[0], data[1], data[2]));
            C_Logger.Instance.LOG_spi("SPI HAPPENED");
        }
    }
    
}
