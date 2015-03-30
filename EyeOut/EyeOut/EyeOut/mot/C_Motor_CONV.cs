using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    public enum e_bounds
    {
        in_bounds = 0
        ,
        bigger = 1
            , smaller = 2
    }

    public partial class C_Motor
    {
        
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
        #region strHex 2 byte
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static Byte strHex2byte(string strHex)
        {
            byte by = strHex2byteArray(strHex)[0];
            return by;
        }

        public static Byte[] strHex2byteArray(string strHex, string delimiter)
        {
            string[] strHexDoubles = strHex.Split(' ');
            return strHexDoubles2byteArray(strHexDoubles);
        }
        public static Byte[] strHexDoubles2byteArray(string[] strHexDoubles)
        {
            Byte[] by = new Byte[strHexDoubles.Length];
            int i = 0;
            foreach (String hex in strHexDoubles)
            {
                by[i] = (Byte)Convert.ToInt32(hex, 16);
                //Console.WriteLine("int value = {0} ", by[i]);
                i++;
            }
            return by;
        }
        public static Byte[] strHex2byteArray(string strHex_concatenated)
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
        public static string byteArray2strHex_hyphen(Byte[] bys)
        {
            return BitConverter.ToString(bys);
        }

        public static string byteArray2strHex_space(Byte[] bys)
        {
            return BitConverter.ToString(bys).Replace("-", " ");
        }

        public static string byteArray2strHex_delimiter(Byte[] bys, string del)
        {
            return BitConverter.ToString(bys).Replace("-", del);
        }

                
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion byte 2 strHex
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
        #region CONV
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        
            /*
        private Byte[] CONV_ang_deg2by(double deg)
        {
            // by = 0 to 1023 (0x3FF)
            // ang = 0 to 300
            //(Byte) 1023*
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

            Byte H = (byte)(degconv >> 8);
            Byte L = (byte)(degconv & 0xff);
            return new Byte[] { L, H };
           
        }
        private Byte[] CONV_speed_deg2by(double deg)
        {

        }*/
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion CONV
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
        
        public static void PRINT_byteArray(Byte[] bys)
        {
            foreach (byte by in bys)
                Console.WriteLine("dec {0}\t= 0x{0:X}", by);
        }

        public static bool GET_bit(Byte by, int bitNumber)
        {
            return (by & (1 << bitNumber)) != 0;
        }

    }

    public class C_Value
    {
        private const double pi = Math.PI;
        private const double piHalf = Math.PI/2;
        private double dec;
        private double decMin;
        private double decMax;

        private double decLimitMin;
        private double decLimitMax;

        private byte[] hex;
        private UInt16 hexMin;
        private UInt16 hexMax;

        private double decLast;
        
        public C_Value() // because of search motor
        {
            decMin = 0;
            decMax = 1;
            decLimitMin = decMin;
            decLimitMax = decMax;
            hexMin = 0;
            hexMax = 0xffff;
            Dec = 0; // must be zero because of ORDER_moveBrisk
        }
        public C_Value(C_Value _val, double _decLimitMin, double _decLimitMax)
        {
            decLimitMin = _decLimitMin;
            decLimitMax = _decLimitMax;
            decMin = _val.decMin;
            decMax = _val.decMax;
            hexMin = _val.hexMin;
            hexMax = _val.hexMax;
            Dec = _val.Dec;
        }

        public C_Value(C_Value _val, double _decLimitMin, double _decLimitMax, double _dec)
        {
            decLimitMin = _decLimitMin;
            decLimitMax = _decLimitMax;
            decMin = _val.decMin;
            decMax = _val.decMax;
            hexMin = _val.hexMin;
            hexMax = _val.hexMax;
            Dec = _dec;
        }

        public C_Value(C_Value _val) // because of search motor
        {
            decLimitMin = _val.decLimitMin;
            decLimitMax = _val.decLimitMax;
            decMin = _val.decMin;
            decMax = _val.decMax;
            hexMin = _val.hexMin;
            hexMax = _val.hexMax;
            Dec = _val.Dec;
        }

        public C_Value(double _decMin, double _decMax, UInt16 _hexMin, UInt16 _hexMax)
        {
            decMin = _decMin;
            decMax = _decMax;
            decLimitMin = decMin;
            decLimitMax = decMax;
            hexMin = _hexMin;
            hexMax = _hexMax;
            Dec = 0;
        }

        public C_Value(double _decLimitMin, double _decLimitMax, double _decMin, double _decMax, UInt16 _hexMin, UInt16 _hexMax)
        {
            decMin = _decMin;
            decMax = _decMax;
            decLimitMin = decMin;
            decLimitMax = decMax;
            hexMin = _hexMin;
            hexMax = _hexMax;
            Dec = 0;
        }

        public C_Value(double _decMin, double _decMax, UInt16 _hexMin, UInt16 _hexMax, double _dec)
        {
            decMin = _decMin;
            decMax = _decMax;
            decLimitMin = decMin;
            decLimitMax = decMax;
            hexMin = _hexMin;
            hexMax = _hexMax;
            Dec = _dec;
        }

        public C_Value(double _decLimitMin, double _decLimitMax, double _decMin, double _decMax, UInt16 _hexMin, UInt16 _hexMax, double _dec)
        {
            decMin = _decMin;
            decMax = _decMax;
            decLimitMin = _decLimitMin;
            decLimitMax = _decLimitMax;
            hexMin = _hexMin;
            hexMax = _hexMax;
            Dec = _dec;
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public double Dec // <decLimitMin; decLimitMax>
        {
            get
            {
                return dec;
            }
            set
            {
                dec = (double)GET_bounded(value, decLimitMin, decLimitMax);
                hex = dec2hex(dec);
            }
        }

        public double Dec_interval01 // <0;1>
        {
            get
            {
                return CONV_intervalMinMax_to_interval01(dec,decMin,decMax);
            }
            set
            {
                dec = (double)CONV_interval01_to_intervalMinMax(value, decLimitMin, decLimitMax);
                hex = dec2hex(dec);
            }
        }

        public double Dec_interval_11 // <-1;1>
        {
            get
            {
                return CONV_interval01_to_intervalMinMax(CONV_intervalMinMax_to_interval01(dec, decMin, decMax),
                    -1,1);
            }
            set
            {
                dec = (double)CONV_interval01_to_intervalMinMax(
                    CONV_intervalMinMax_to_interval01(value,-1,1), decLimitMin, decLimitMax);
                hex = dec2hex(dec);
            }
        }

        public double Dec_interval_piPi // <-pi;pi>
        {
            get
            {
                return CONV_interval01_to_intervalMinMax(CONV_intervalMinMax_to_interval01(dec, decMin, decMax),
                    -pi, pi);
            }
            set
            {
                dec = (double)CONV_interval01_to_intervalMinMax(
                    CONV_intervalMinMax_to_interval01(value, -Math.PI, Math.PI), decLimitMin, decLimitMax);
                hex = dec2hex(dec);
            }
        }

        public double Dec_interval_piHalfPiHalf // <-pi/2;pi/2>
        {
            get
            {
                return CONV_interval01_to_intervalMinMax(CONV_intervalMinMax_to_interval01(dec, decMin, decMax),
                    -piHalf, piHalf);
            }
            set
            {
                dec = (double)CONV_interval01_to_intervalMinMax(
                    CONV_intervalMinMax_to_interval01(value, -Math.PI, Math.PI), decLimitMin, decLimitMax);
                hex = dec2hex(dec);
            }
        }

        public byte[] Hex
        {
            get
            {
                return hex;
            }
            set
            {
                hex = value;
                dec = hex2dec(value);
            }
        }

        public double DecMin { get { return decMin; } }
        public double DecMax { get { return decMax; } }

        public double DecLast { get { return decLast; } }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region conv
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public byte[] dec2hex(double _dec)
        {
            dec = GET_bounded(_dec, decLimitMin, decLimitMax); // number in interval <decLimitMin, decLimitMax>
            double decOne = (double)CONV_intervalMinMax_to_interval01(dec, decMin, decMax); // get number in interval <0,1> ~ in scale of <decMin,decMax>
            UInt16 hexUInt16 = (UInt16)CONV_interval01_to_intervalMinMax(decOne, hexMin, hexMax); // number in interval <hexMin, hexMax>

            Byte H = (byte)(hexUInt16 >> 8); // higher byte part
            Byte L = (byte)(hexUInt16 & 0xFF); // lower byte part

            return new Byte[] { L, H };
        }

        public double hex2dec(byte[] hex)
        {
            UInt32 hexUInt32 = ((UInt32)hex[1] >> 8) + (UInt32)hex[0];
            UInt16 hexOne = (UInt16)GET_bounded(hexUInt32, hexMin, hexMax); // number in interval <hexMin, hexMax>
            double dec = (double)CONV_intervalMinMax_to_interval01(hexOne, hexMin, hexMax); // number in interval <0,1>
            dec = (UInt16)CONV_interval01_to_intervalMinMax(dec, decMin, decMax); // get number in interval <0,1> ~ in scale of <decMin,decMax>
            dec = GET_bounded(dec, decLimitMin, decLimitMax); // number in interval <decLimitMin, decLimitMax>
            return dec;
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion conv
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region bounds
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        /*
        private e_bounds NOTIN_bounds(double num, double min, double max)
        {
            if (num > max)
                return e_bounds.bigger;
            else if (num < min)
                return e_bounds.smaller;
            else
                return e_bounds.in_bounds;
        }*/

        // make generic type!
        private double CONV_intervalMinMax_to_interval01(double val, double min, double max)
        {
            return  (val - min) / (max - min) ;
        }
        private byte CONV_intervalMinMax_to_interval01(byte val, byte min, byte max)
        {
            return (byte)CONV_intervalMinMax_to_interval01((double)val, (double)min, (double)max);
        }

        /*
        private byte CONV_interval2one(byte val, byte min, byte max)
        {
            return (byte)CONV_interval2one((double)val, (double)min, (double)max);
        }

        private double CONV_interval2one(object val, object min, object max)
        {
            return CONV_interval2one((double)val, (double)min, (double)max);
        }*/

        private double CONV_interval01_to_intervalMinMax(double val, double min, double max)
        {
            return val * (max - min) + min;
        }
        private byte CONV_interval01_to_intervalMinMax(byte val, byte min, byte max)
        {
            return (byte)CONV_interval01_to_intervalMinMax((double)val, (double)min, (double)max);
        }

        private double GET_bounded(double val, double min, double max)
        {
            if (val > max)
            {
                LOG(String.Format(
                    "Value out of bounds: bigger then boundary {0} > [max{1}]",
                    val, max));
                return max;
            }
            else if (val < min)
            {
                LOG(String.Format(
                    "Value out of bounds: lower then boundary {0} < [min{1}]",
                    val, min));
                return min;
            }
            else
            {
                return val;
            }
        }

        private byte GET_bounded(byte val, byte min, byte max)
        {
            if (val > max)
            {
                LOG(String.Format(
                    "Value out of bounds: bigger then boundary {0} > [max{1}]",
                    val, max));
                return max;
            }
            else if (val < min)
            {
                LOG(String.Format(
                    "Value out of bounds: lower then boundary {0} < [min{1}]",
                    val, min));
                return min;
            }
            else
            {
                return val;
            }
        }
        /*
        private void LOG_NOTIN_bounds(double num, double min, double max)
        {
            e_bounds e = NOTIN_bounds(num, min, max);
            switch (e)
            {
                case (e_bounds.bigger):
                    break;
                case (e_bounds.smaller):
                    break;
            }
        }*/
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion bounds
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private void LOG(string msg)
        {
            C_Logger.Instance.LOG_type(e_LogMsgSource.valConv, msg, e_LogMsgType.warning);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region UPDATE
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void UPDATE_lastSent()
        {
            decLast = dec;
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion UPDATE
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }

}
