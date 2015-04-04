using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; // description
using System.Reflection; // fieldInfo  - description

namespace EyeOut
{
    // some helping functions

    class C_CounterDown
    {
        private int val;
        private int valDef;

        public int Val { get { return val; } }
        public int ValDef { get { return valDef; } }
        public C_CounterDown(int _val, int _valDef)
        {
            val = _val;
                valDef= _valDef;
        }
        public C_CounterDown(int _valDef) : this(_valDef, _valDef) { }
        
        public int Decrement()
        {
            if (val != 0)
            {
                return --val;
            }
            else
            {
                return 0;
            }
        }

        public int DecrementAndRestart()
        {
            if (val != 0)
            {
                return --val;
            }
            else
            {
                return Restart();
            }
        }

        public int Restart()
        {
            val = valDef;
            return val;
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
}
