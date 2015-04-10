using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{

    public partial class C_CONV
    {
        
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
        #region strHex 2 byte
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static byte strHex2byte(string strHex)
        {
            byte by = strHex2byteArray(strHex)[0];
            return by;
        }

        public static byte[] strHex2byteArray(string strHex, string delimiter)
        {
            string[] strHexDoubles = strHex.Split(' ');
            return strHexDoubles2byteArray(strHexDoubles);
        }
        public static byte[] strHexDoubles2byteArray(string[] strHexDoubles)
        {
            byte[] by = new byte[strHexDoubles.Length];
            int i = 0;
            foreach (String hex in strHexDoubles)
            {
                by[i] = (byte)Convert.ToInt32(hex, 16);
                //Console.WriteLine("int value = {0} ", by[i]);
                i++;
            }
            return by;
        }
        public static byte[] strHex2byteArray(string strHex_concatenated)
        {
            int numOfDoubles = strHex_concatenated.Length / 2;
            string[] strHexDoubles = new string[numOfDoubles];

            for (int q = 0; q < numOfDoubles; q++)
            {
                strHexDoubles[q] = strHex_concatenated.Substring(q * 2, 2);
            }
            return strHexDoubles2byteArray(strHexDoubles);
        }

        
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion strHex 2 byte
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
        #region byte 2 strHex
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static string byteArray2strHex_hyphen(byte[] bys)
        {
            return BitConverter.ToString(bys);
        }

        public static string byteArray2strHex_space(byte[] bys)
        {
            return BitConverter.ToString(bys).Replace("-", " ");
        }

        public static string byteArray2strHex_space(List<byte> lsBys)
        {
            return BitConverter.ToString(lsBys.ToArray()).Replace("-", " ");
        }

        public static string byteArray2strHex_delimiter(byte[] bys, string del)
        {
            return BitConverter.ToString(bys).Replace("-", del);
        }

                
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion byte 2 strHex
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
        #region CONV
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        
            /*
        private byte[] CONV_ang_deg2by(double deg)
        {
            // by = 0 to 1023 (0x3FF)
            // ang = 0 to 300
            //(byte) 1023*
            double min = 0;
            double max = 360;
            double maxHex = 1023;
            e_bounds e = NOTIN_bounds(deg, min, max);
            switch (e)
            {
                case (e_bounds.bigger):
                    LOG(String.Format(
                        "Tried to calculate angle bigger then boundary {0} > [max{1}] deg. Used the maximum value.",
                        deg, max));
                    break;
                case (e_bounds.smaller):
                    LOG(String.Format(
                        "Tried to calculate angle lower then boundary {0} < [min{1}] deg. Used the minimum value.",
                        deg, min));
                    break;
            }

            UInt16 degconv = Convert.ToUInt16(maxHex * GET_bounded(deg, min, max) / max * 4);

            byte H = (byte)(degconv >> 8);
            byte L = (byte)(degconv & 0xff);
            return new byte[] { L, H };
           
        }
        private byte[] CONV_speed_deg2by(double deg)
        {

        }*/
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion CONV
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
        
        public static void PRINT_byteArray(byte[] bys)
        {
            foreach (byte by in bys)
                Console.WriteLine("dec {0}\t= 0x{0:X}", by);
        }

        public static bool GET_bit(byte by, int bitNumber)
        {
            return (by & (1 << bitNumber)) != 0;
        }

    }

}
