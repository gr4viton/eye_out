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
        public static Byte GET_checkSum(Byte[] cmd)
        {
            Byte calc_check = 0x00;
            unchecked // Let overflow occur without exceptions
            {
                foreach (Byte ch in cmd)
                {
                    calc_check += ch;
                }
            }
            calc_check = (Byte)~calc_check;
            return calc_check;
        }

        public static bool CHECK_checkSum(Byte check1, Byte check2)
        {
            return (Byte)check1 == (Byte)(check2);
        }
    }
}
