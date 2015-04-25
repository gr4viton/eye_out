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

    public class C_CounterDown
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
    public partial class C_CONV
    {
        public static List<byte> listOfObjects2listOfBytes(List<object> L)
        {
            // creates byte array out of list of byte / byte arrays - concatenates them
            List<byte> liby = new List<byte>();
            if (L != null)
            {
                foreach (object o in L)
                {
                    if(o is byte)
                    {
                        liby.Add((byte)o);
                    }
                    else if (o is int)
                    {
                        liby.Add((byte)(int)o);
                    }
                    else if (o is byte[])
                    {
                        if (((byte[])o).Length > 0)
                        {
                            liby.AddRange((byte[])o);
                        }
                    }
                    else if (o is UInt16)
                    {
                        liby.AddRange(BitConverter.GetBytes((UInt16)o));
                    }
                    else if (o is List<byte>)
                    {
                        //throw new Exception("Cannot convert from
                        if ((o as List<byte>).Count() > 0)
                        {
                            liby.AddRange((o as List<byte>));
                        }
                    }
                }
            }
            else
            {
                return liby;
            }
            return liby;
        }

        public static byte[] stringOfBytes2arrayOfBytes(string str)
        {
            str.Replace(" ", ", 0x");
            string[] words = str.Split(' ');

            byte[] bytes = new byte[words.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public static List<byte> stringOfBytes2listOfBytes(string str)
        {
            return new List<byte>(stringOfBytes2arrayOfBytes(str));
        }
    }
}
