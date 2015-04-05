using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace EyeOut
{
    public class C_CheckSum
    {
        public static byte GET_checkSum(byte[] _data)
        {
            // data should not contain the checksum byte
            byte calc_check = 0x00;
            unchecked // Let overflow occur without exceptions
            {
                foreach (byte ch in _data)
                {
                    calc_check += ch;
                }
            }
            calc_check = (byte)~calc_check;

            return calc_check;
            //return (byte)(calc_check-2);
        }

        public static bool CHECK_checkSum(byte check1, byte check2)
        {
            return (byte)check1 == (byte)(check2);
        }
    }
}
