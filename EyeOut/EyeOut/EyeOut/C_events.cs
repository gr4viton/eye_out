using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Threading;

namespace EyeOut
{

    //This delegate can be used to point to methods
    //which return void and take a string.
    public delegate void h_LOG_String(string msg);
    public delegate void h_SEND_Bytes(Byte[] cmd);


    public class C_events
    {



    }
}
