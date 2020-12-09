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
        // packetBytes
        public static byte GET_checkSum_fromDataBytes(byte[] _dataBytes)
        {
            // data should not contain the checksum byte neither the start packet bytes
            byte calc_check = 0x00;
            unchecked // Let overflow occur without exceptions
            {
                foreach (byte ch in _dataBytes)
                {
                    calc_check += ch;
                }
            }
            calc_check = (byte)~calc_check;

            return calc_check;
            //return (byte)(calc_check-2);
        }

        public static byte GET_checkSum_fromDataBytes(List<byte> _lsDataBytes)
        {
            return GET_checkSum_fromDataBytes(_lsDataBytes.ToArray());
        }

        public static byte GET_checkSum_fromWholePacket(List<byte> _lsPacketBytes)
        {
            int count = _lsPacketBytes.Count - (int)C_DynAdd.SIZEOF_PACKETSTART - (int)C_DynAdd.SIZEOF_CHECKSUM;

            return GET_checkSum_fromDataBytes(
                _lsPacketBytes.GetRange(C_DynAdd.INDEXOF_ID_IN_INSTRUCTIONPACKET, count).ToArray());

        }

        public static byte GET_checkSum_fromWholePacket(byte[] _packetBytes)
        {
            return GET_checkSum_fromWholePacket(new List<byte>(_packetBytes));
        }

        public static bool CHECK_checkSum(byte check1, byte check2)
        {
            return (byte)check1 == (byte)(check2);
        }
    }
}
