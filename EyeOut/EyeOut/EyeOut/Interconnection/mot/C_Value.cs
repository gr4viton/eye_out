using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    public enum e_bounds
    {
        in_bounds = 0,
        bigger = 1,
        smaller = 2
    }

    public class C_Value
    {
        public static C_Value angleFull = new C_Value(0, 360, 200, C_DynVal.SET_GOAL_POS_MIN, C_DynVal.SET_GOAL_POS_MAX);
        //C_Value speedFull = new C_Value(0, 100, C_DynVal.SET_MOV_SPEED_MIN, C_DynVal.SET_MOV_SPEED_MAX, 20);
        public static C_Value speedFull = new C_Value(0, 101, 1,C_DynVal.SET_MOV_SPEED_NOCONTROL, C_DynVal.SET_MOV_SPEED_MAX); // no control as 0

        // angle constants
        public const double pi = Math.PI;
        public const double piHalf = Math.PI / 2;
        public const double deg2rad = pi / (double)180;
        public const double rad2deg = (double)180 / pi;

        // speed constants
        // 100 should be [decLimitMax - decLimitMin - 1] 
        // so these values should be recalculated whenever decLimit changes
        private const double dec2RPM = 0.111 / 100 * C_DynVal.SET_MOV_SPEED_MAX;
        private const double RPM2dec = 1 / dec2RPM;

        private double decMin;
        private double decMax;
        private double decDefault;

        private double decLimitMin;
        private double decLimitMax;

        private UInt16 hexMin;
        private UInt16 hexMax;

        private double decLast; //?

        public double zeroAddition = 0;
        public double zeroMultiplication = 1;

        private object lock_dec = new object();
        private object lock_hex = new object();

        private double dec_treasure;
        private byte[] hex_treasure;

        private byte[] hex
        {
            get { return hex_treasure; }
            set
            {
                lock (lock_hex)
                {
                    hex_treasure = value;
                }
            }
        }
        private double dec
        {
            get { return dec_treasure; }
            set
            {
                lock (lock_dec)
                {
                    dec_treasure = value;
                }
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region constructors
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public C_Value() // because of search motor
        {
            decMin = 0;
            decMax = 1;
            decLimitMin = decMin;
            decLimitMax = decMax;
            hexMin = 0;
            hexMax = 0xffff;
            decDefault = 0;
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
            zeroAddition = _val.zeroAddition;
            zeroMultiplication = _val.zeroMultiplication;
            Dec = _val.Dec;
            decDefault = Dec;
        }

        public C_Value(C_Value _val, double _decLimitMin, double _decLimitMax, double _dec)
        {
            decLimitMin = _decLimitMin;
            decLimitMax = _decLimitMax;
            decMin = _val.decMin;
            decMax = _val.decMax;
            hexMin = _val.hexMin;
            hexMax = _val.hexMax;
            zeroAddition = _val.zeroAddition;
            zeroMultiplication = _val.zeroMultiplication;
            Dec = _dec;
            decDefault = Dec;
            
        }

        public C_Value(C_Value _val) // because of search motor
        {
            decDefault = _val.decDefault;
            decLimitMin = _val.decLimitMin;
            decLimitMax = _val.decLimitMax;
            decMin = _val.decMin;
            decMax = _val.decMax;
            hexMin = _val.hexMin;
            hexMax = _val.hexMax;
            zeroAddition = _val.zeroAddition;
            zeroMultiplication = _val.zeroMultiplication;
            Dec = _val.Dec;
            decDefault = Dec;
        }

        public C_Value(double _decMin, double _decMax, double _decDefault, UInt16 _hexMin, UInt16 _hexMax)
        {
            decDefault = _decDefault; 
            decMin = _decMin;
            decMax = _decMax;
            decLimitMin = decMin;
            decLimitMax = decMax;
            hexMin = _hexMin;
            hexMax = _hexMax;
            Dec = 0;
            decDefault = Dec;
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
            decDefault = Dec;
        }

        public C_Value(double _decMin, double _decMax, double _decDefault, UInt16 _hexMin, UInt16 _hexMax, double _dec)
        {
            decDefault = _decDefault; 
            decMin = _decMin;
            decMax = _decMax;
            decLimitMin = decMin;
            decLimitMax = decMax;
            hexMin = _hexMin;
            hexMax = _hexMax;
            Dec = _dec;
            decDefault = Dec;
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
            decDefault = Dec;
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion constructors
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
                dec = GET_bounded_decLimits(value);
                hex = dec2hex(dec);
            }
        }
        public double Dec_inRPM // Dec * constant
        {
            get
            {
                return Dec * dec2RPM;
            }
            set
            {
                Dec = value * RPM2dec;
            }
        }
        public double Dec_interval01 // <0;1>
        {
            get
            {
                return CONV_intervalMinMax_to_interval01(dec, decMin, decMax);
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
                    -1, 1);
            }
            set
            {
                dec = (double)CONV_interval01_to_intervalMinMax(
                    CONV_intervalMinMax_to_interval01(value, -1, 1), decLimitMin, decLimitMax);
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

        public double RadFromDefault // <-pi;pi>
        {
            get
            {
                return CONV_deg2rad(Dec_FromDefault);
            }
            set
            {
                Dec_FromDefault = CONV_deg2rad(value);
            }
        }

        public double RadFromDefaultZero // <-pi;pi>
        {
            get
            {
                return CONV_deg2rad(Dec_FromDefaultZero);
            }
            set
            {
                Dec_FromDefaultZero = CONV_deg2rad(value);
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

        public double Dec_interval_piHalfPiHalf_isDecInDecLimits // <-pi/2;pi/2>
        {
            get
            {
                return CONV_interval01_to_intervalMinMax(CONV_intervalMinMax_to_interval01(dec, decLimitMin, decLimitMax),
                    -piHalf, piHalf);
            }
            set
            {
                dec = (double)CONV_interval01_to_intervalMinMax(
                    CONV_intervalMinMax_to_interval01(value, -Math.PI, Math.PI), decLimitMax, decLimitMax);
                hex = dec2hex(dec);
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public double DecDefault // <decLimitMin; decLimitMax>
        {
            get
            {
                return decDefault;
            }
            set
            {
                decDefault = GET_bounded_decLimits(value);
            }
        }

        public byte[] HexDefault 
        {
            get
            {
                return dec2hex(decDefault);
            }
            //set
            //{
            //    decDefault = GET_bounded_decLimits(value);
            //}
        }

        public double Dec_FromDefault // <decLimitMin; decLimitMax>
        {
            get
            {
                return dec - decDefault;
            }
            set
            {
                Dec = GET_bounded_decLimits(value + decDefault);
            }
        }

        public double Dec_FromDefaultZero // <decLimitMin; decLimitMax>
        {
            get
            {
                return AngleIn_180to180((dec + zeroAddition) * zeroMultiplication);
            }
            set
            {
                Dec = GET_bounded_decLimits( (value/zeroMultiplication) - zeroAddition + decDefault);
            }
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public byte[] Hex // hex[0] = LOW, hex[1] = HIGH
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

        public string ToString()
        {
            return string.Format("{0:0.00}", Dec);
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion properties
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region conv
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public byte[] dec2hex(double _dec) // hex[0] = LOW, hex[1] = HIGH
        {
            dec = GET_bounded_decLimits(_dec); // number in interval <decLimitMin, decLimitMax>
            double decOne = (double)CONV_intervalMinMax_to_interval01(dec, decMin, decMax); // get number in interval <0,1> ~ in scale of <decMin,decMax>
            UInt16 hexUInt16 = (UInt16)CONV_interval01_to_intervalMinMax(decOne, hexMin, hexMax); // number in interval <hexMin, hexMax>

            byte H = (byte)(hexUInt16 >> 8); // higher byte part
            byte L = (byte)(hexUInt16 & 0xFF); // lower byte part

            return new byte[] { L, H };
        }

        public double hex2dec(byte[] hex) // hex[0] = LOW, hex[1] = HIGH
        {
            UInt32 hexUInt32 = ((UInt32)hex[1] << 8) + (UInt32)hex[0];
            UInt16 hexOne = (UInt16)GET_bounded(hexUInt32, hexMin, hexMax); // number in interval <hexMin, hexMax>
            double dec = (double)CONV_intervalMinMax_to_interval01(hexOne, hexMin, hexMax); // number in interval <0,1>
            dec = CONV_interval01_to_intervalMinMax(dec, decMin, decMax); // get number in interval <0,1> ~ in scale of <decMin,decMax>
            dec = GET_bounded_decLimits(dec); // number in interval <decLimitMin, decLimitMax>
            return dec;
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion conv
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region default
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public void RESET_toDefault()
        {
            Dec = decDefault;
        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion default
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region bounds
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        private double AngleIn0to360(double angle)
        {
            if (angle > 360)
                return angle % 360;
            if (angle < 0)
                return angle % 360 + 360;
            return angle;
        }

        private double AngleIn_180to180(double angle)
        {
            angle = AngleIn0to360(angle);
            if (angle > 180)
                return angle -360;
            return angle;
        }
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
            return (val - min) / (max - min);
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

        private double GET_bounded_decLimits(double val)
        {
            return GET_bounded(val, decLimitMin, decLimitMax);
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

        public static double CONV_deg2rad(double deg)
        {
            return deg * deg2rad;
        }

        public static double CONV_rad2deg(double rad)
        {
            return rad * rad2deg;
        }

        public static float CONV_deg2rad(float deg)
        {
            return deg * (float)deg2rad;
        }

        public static float CONV_rad2deg(float rad)
        {
            return rad * (float)rad2deg;
        }
    }

}
