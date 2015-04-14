using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    public enum e_regByteType
    {
        registerDefault = 0,
        sentValue = 1,
        seenValue = 2
    }
    public enum e_readWrite
    {
        registered = 0, // cannot read or write
        readOnly = 1,
        writeOnly = 2,
        readWrite = 3 // can read and write
    }
    public class C_RegByteValue
    {
        private byte val;
        private bool touched;
        private DateTime actualized;
        public e_readWrite rwShadow { get; private set;}
        public e_readWrite rwActual {get; private set;}

        public bool Touched
        {
            get { return touched; }
        }
        public DateTime Actualized
        {
            get
            {
                return actualized;
            }
        }

        public byte Val
        {
            get
            {
                if (C_RegByteValue.IS_readable(rwShadow) == true)
                {
                    return val;
                    //if (touched == true)
                    //{
                    //    return val;
                    //}
                    //else
                    //{
                    //    throw new Exception("Tried to get data from register byte which wasn't touched yet!");
                    //}
                }
                else
                {
                    throw new Exception(string.Format(
                        "Read operation not permited on {0} value", rwShadow));
                }
            }
            set
            {
                if (C_RegByteValue.IS_writable(rwShadow) == true)
                {
                    val = value;
                    touched = true;
                    actualized = DateTime.UtcNow;
                }
                else
                {
                    throw new Exception(string.Format(
                        "Read operation not permited on {0} value", rwShadow));
                }
            }
        }

        public C_RegByteValue(byte _value, e_readWrite _rwActual, e_readWrite _rwShadow)
        {
            rwShadow = _rwShadow;
            rwActual = _rwActual;
            touched = false;
            val = _value;
            actualized = DateTime.UtcNow;
        }

        public C_RegByteValue(byte _value, e_readWrite _rwActual, e_readWrite _rwShadow, bool _touched)
        {
            rwShadow = _rwShadow;
            rwActual = _rwActual;
            touched = _touched;
            val = _value;
            actualized = DateTime.UtcNow;
        }

        public bool IS_readableActual()
        {
            return IS_readable(rwActual);
        }
            
        public bool IS_writableActual()
        {
            return IS_writable(rwActual);
        }

        public static bool IS_readable(e_readWrite rw)
        {
            if ((rw == e_readWrite.readOnly) || (rw == e_readWrite.readWrite))
            {
                return true;
            }
            else // if ((rw == e_readWrite.writeOnly) || (rw == e_readWrite.registered))
            {
                return false;
            }
        }
        public static bool IS_writable(e_readWrite rw)
        {
            if ((rw == e_readWrite.writeOnly) || (rw == e_readWrite.readWrite))
            {
                return true;
            }
            else // if ((rw == e_readWrite.readOnly) || (rw == e_readWrite.registered))
            {
                return false;
            }
        }
    }

    public class C_RegByte
    {
        public C_RegByteValue def; // register default
        public C_RegByteValue sent; // last sent
        public C_RegByteValue seen; // last seen
        public byte byteAddress { get; private set; }

        private string name;
        private e_readWrite rwShadow;

        public string Name
        {
            get { return name; }
        }

        public e_readWrite RwShadow
        {
            get { return rwShadow; }
        }


        public void SET(byte _value, e_regByteType _type)
        {
            switch (_type)
            {
                case (e_regByteType.registerDefault): def.Val = _value; break;
                case (e_regByteType.sentValue): sent.Val = _value; break;
                case (e_regByteType.seenValue): seen.Val = _value; break;
            }
        }

        public C_RegByteValue GET(e_regByteType _type)
        {
            switch (_type)
            {
                case (e_regByteType.registerDefault): return def;   
                case (e_regByteType.sentValue): return sent; 
                case (e_regByteType.seenValue): return seen; 
            }
            throw new Exception(string.Format(
                "[{0}] is not a valid type of register byte!", _type));
        }

        public C_RegByte(string _name, e_readWrite _rwActual, e_readWrite _rwShadow, byte _address, byte _value)
            : base()
        {
            rwShadow = _rwShadow;
            name = _name;
            byteAddress = _address;
            def = new C_RegByteValue(_value, _rwActual, _rwShadow, true);
            sent = new C_RegByteValue(_value, _rwActual, _rwShadow);
            seen = new C_RegByteValue(_value, _rwActual, _rwShadow);
        }
    }

    // copy of the register stored in motor
    public class C_ByteRegister
    {
        object lock_reg;
        private List<C_RegByte> reg = new List<C_RegByte>();
        private int i_maxReg;
        

        public C_ByteRegister()
        {
            lock_reg = new object();
            INIT_byteRegisterDefaultValues();
            //BIND_bytes();
        }

        public void INIT_byteRegisterDefaultValues()
        {
            // load it from txt file

            string txt = Properties.Resources.ResourceManager.GetString("registerByteDefault");

            string[] lines = txt.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            char sep = '|';

            byte add = 0;
            foreach (string line in lines)
            {
                if (line.Length > 2)
                {
                    string[] splited = line.Split(sep);
                    // syntax of each line:
                    // XX A #num name
                    // legend:
                    // splited[0]= XX - hexa
                    // splited[1]= A - [X]registered [R]ead [B]oth ([W]rite)
                    // splited[2]= # - number symbol
                    // splited[2]= num - address in register - not needed
                    // splited[3]= name - address byte name string

                    e_readWrite rwShadow = e_readWrite.registered;
                    e_readWrite rwActual = e_readWrite.registered;
                    switch (splited[1])
                    {
                        //case ("X"): rw = e_readWrite.registered; break;
                        //case ("B"): rw = e_readWrite.readWrite; break;
                        //case ("R"): rw = e_readWrite.readOnly; break;
                        //case ("W"): rw = e_readWrite.writeOnly; break;
                        case ("X"): 
                            rwShadow = e_readWrite.registered;
                            rwActual = e_readWrite.registered;
                            break;
                        case ("B"): 
                            rwShadow = e_readWrite.readWrite;
                            rwActual = e_readWrite.readWrite;
                            break;
                        case ("R"): 
                            rwShadow = e_readWrite.readWrite;
                            rwActual = e_readWrite.readOnly; 
                            break;
                        case ("W"): 
                            rwShadow = e_readWrite.readWrite;
                            rwActual = e_readWrite.writeOnly; 
                            break;
                    }

                    reg.Add(new C_RegByte(
                        splited[3], rwActual, rwShadow, add, C_CONV.strHex2byte(splited[0])
                        ));
                    //reg[reg.Count-1].
                    add++;
                }
            }
            i_maxReg = reg.Count;
        }
        
        public void SET(int _add, byte _value, e_regByteType _type)
        {
            lock (lock_reg)
            {
                if (_add < i_maxReg)
                {
                    reg[_add].SET(_value, _type);
                }
                else
                {
                    throw new Exception(GET_outOfBoundsInfo(_add));
                }
            }
        }
        public bool IS_writableShadow(int _add)
        {
            if ((reg[_add].RwShadow == e_readWrite.registered)
                ||
                (reg[_add].RwShadow == e_readWrite.readOnly))
            {
                return false;
            }
            else if ((reg[_add].RwShadow == e_readWrite.readWrite)
                    ||
                    (reg[_add].RwShadow == e_readWrite.writeOnly))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public string GET_name(int _add)
        {
            return reg[_add].Name;
        }
        public C_RegByteValue GET(int _add, e_regByteType _type)
        {
            lock (lock_reg)
            {
                if (_add < i_maxReg)
                {
                    return reg[_add].GET(_type);
                }
                else
                {
                    throw new Exception(GET_outOfBoundsInfo(_add));
                }
            }
        }

        public List<byte> GET_byteAddresses(e_regByteType regByteType)
        {
            List<byte> bys = new List<byte>();
            //returns the list of with stated rw 
            foreach (C_RegByte regByte in reg)
            {
                // if seenValue -> return only those which can be read from motor
                if(regByteType == e_regByteType.seenValue)
                {
                    if (regByte.GET(regByteType).IS_readableActual() == true)
                    {
                        bys.Add(regByte.byteAddress);
                    }
                }
                // if sentValue -> return only those which can be written to motor 
                else if (regByteType == e_regByteType.sentValue)
                {
                    if (regByte.GET(regByteType).IS_writableActual() == true)
                    {
                        bys.Add(regByte.byteAddress);
                    }
                }
            }
            return bys;
        }

        //public void Add(C_RegByte item)
        //{
        //    reg.Add(item); 
        //    // setup binding switch
        //}

        public static void LOG(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.byteReg, _msg);
        }
        public static void LOG_err(string _msg)
        {
            C_Logger.Instance.LOG_err(e_LogMsgSource.byteReg, _msg);
        }
        private string GET_outOfBoundsInfo(int _add)
        {
            return string.Format(
                "Out of bounds: Cannot access byte[{0}] of the register. Max=[{1}]",
                _add, i_maxReg);
        }
    }
}
