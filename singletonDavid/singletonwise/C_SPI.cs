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

        public static bool WriteData(byte[] data)
        {
            lock (locker)
            {
                //OpenConnection(null, null);
                WriteSerialPort(data);
                //responseBuffer = ReadSerialPort(8);
            }
            return true;
        }

        private static void WriteSerialPort(byte[] data)
        {
            spi.Write(data, 0, data.Length);
        }
    }
    
}
