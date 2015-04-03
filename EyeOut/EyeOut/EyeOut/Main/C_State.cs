using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.ComponentModel; // description
using System.Reflection; // fieldInfo  - description
using System.Windows.Data; // IValueConverter
using System.Globalization; // CultureInfo


namespace EyeOut
{
    public enum e_stateProg
    {
        started = 0, initializing, initialized, running, closing, closed
    }
    public enum e_stateMotor
    {
        initializing = 0, ready = 1
    }

    public enum e_stateSPI
    {
        [Description("No port avalible")]
        noPortAvailible,
        [Description("Not connected")]
        disconnected = 0,
        [Description("Connecting")]
        connecting,
        [Description("Connected")]
        connected //,sending, recieving
    }


    public class C_State
    {
        //public static event EventHandler SpiChanged;
        
        public static e_stateProg prog;
        public static e_stateMotor mot;
        private static e_stateSPI spi;

        public static event EventHandler SpiChanged;

        public static e_stateSPI Spi
        {
            get { return spi; }
            set {
                spi = value;
                //tslConnected
                EventHandler handler = SpiChanged;
                if (handler != null)
                    handler(null, EventArgs.Empty);
            }
        }

        public C_State()
        {
            prog = e_stateProg.started;
            mot = e_stateMotor.initializing;
            Spi = e_stateSPI.disconnected;
        }

        public static void CLOSE_program()
        {
            prog = e_stateProg.closing;

            // Kill serial port
            Thread dexter = new Thread(new ThreadStart(C_SPI.CLOSE_connectionAndAbortThread)); // the serial (port) killer
            int q = 10; // try to stop it X-times
            while (q > 0)
            {
                dexter.Start();
                System.Threading.Thread.Sleep(500);
                if (C_SPI.spi.IsOpen == false)
                {
                    break;
                }
            }
        }

        public static bool FURTHER(e_stateSPI _comparedState)
        {
            return FURTHER((object)_comparedState, (object)Spi);
        }

        public static bool FURTHER(e_stateMotor _comparedState)
        {
            return FURTHER((object)_comparedState, (object)mot);
        }

        public static bool FURTHER(e_stateProg _comparedState)
        {
            return FURTHER((object)_comparedState, (object)prog);
        }


        public static bool FURTHER(object _comparedState, object _actualState)
        {
            return ((int)_actualState >= (int)_comparedState);
        }
        

    }


    public static class EnumGetDescription
    {
        public static string GetDescription(this Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }
    }

    public class VCMyEnumToString : IValueConverter
    {
        #region IValueConverter Members
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))  // This is for databinding
                return e_stateSPI.disconnected;
            return (StringToEnum<e_stateSPI>(value.ToString())).GetDescription(); // <-- The extention method
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString())) // This is for databinding
                return e_stateSPI.disconnected;
            return StringToEnum<e_stateSPI>(value.ToString());
        }

        public static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        #endregion
    }

    // converter for enum -> string
    public class CONV_e_rot_ToString : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))  // This is for databinding
                return e_stateSPI.disconnected;
            return (StringToEnum<e_rot>(value.ToString())).GetDescription(); // <-- The extention method
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString())) // This is for databinding
                return e_stateSPI.disconnected;
            return StringToEnum<e_rot>(value.ToString());
        }

        public static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        #endregion
    }
}
