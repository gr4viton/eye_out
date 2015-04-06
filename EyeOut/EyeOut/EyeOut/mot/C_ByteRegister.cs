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
        lastReceived = 2
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
        private e_readWrite rw;

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
                if (C_RegByteValue.CAN_read(rw) == true)
                {
                    if (touched == true)
                    {
                        return val;
                    }
                    else
                    {
                        throw new Exception("Tried to get data from register byte which wasn't touched yet!");
                    }
                }
                else
                {
                    throw new Exception(string.Format(
                        "Read operation not permited on {0} value", rw));
                }
            }
            set
            {
                if (C_RegByteValue.CAN_write(rw) == true)
                {
                    val = value;
                    touched = true;
                    actualized = DateTime.UtcNow;
                }
                else
                {
                    throw new Exception(string.Format(
                        "Read operation not permited on {0} value", rw));
                }
            }
        }

        public C_RegByteValue( byte _value, e_readWrite _rw)
        {
            rw = _rw;
            touched = false;
            val = _value;
            actualized = DateTime.UtcNow;
        }

        public C_RegByteValue(byte _value, e_readWrite _rw, bool _touched)
        {
            rw = _rw;
            touched = _touched;
            val = _value;
            actualized = DateTime.UtcNow;
        }

        public static bool CAN_read(e_readWrite rw)
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
        public static bool CAN_write(e_readWrite rw)
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

        private string name;
        private e_readWrite rw;

        public string Name
        {
            get { return name; }
        }

        public e_readWrite Rw
        {
            get { return rw; }
        }


        public void SET(byte _value, e_regByteType _type)
        {
            switch (_type)
            {
                case (e_regByteType.registerDefault): def.Val = _value; break;
                case (e_regByteType.sentValue): sent.Val = _value; break;
                case (e_regByteType.lastReceived): seen.Val = _value; break;
            }
        }

        public C_RegByteValue GET(e_regByteType _type)
        {
            switch (_type)
            {
                case (e_regByteType.registerDefault): return def;   
                case (e_regByteType.sentValue): return sent; 
                case (e_regByteType.lastReceived): return seen; 
            }
            throw new Exception(string.Format(
                "[{0}] is not a valid type of register byte!", _type));
        }

        public C_RegByte(string _name, e_readWrite _rw, byte _value)
            : base()
        {
            rw = _rw;
            name = _name;
            def = new C_RegByteValue(_value, _rw, true);
            sent = new C_RegByteValue(_value, _rw);
            seen = new C_RegByteValue(_value, _rw);
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
            char sep = ' ';

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

                    e_readWrite rw = e_readWrite.registered;
                    switch (splited[1])
                    {
                        case ("X"): rw = e_readWrite.registered; break;
                        case ("B"): rw = e_readWrite.readWrite; break;
                        case ("R"): rw = e_readWrite.readOnly; break;
                        case ("W"): rw = e_readWrite.writeOnly; break;
                    }

                    reg.Add(new C_RegByte(
                        splited[3], rw, C_CONV.strHex2byte(splited[0])
                        ));
                    //reg[reg.Count-1].
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
