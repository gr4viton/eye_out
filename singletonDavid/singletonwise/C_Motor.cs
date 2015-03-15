using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.ComponentModel;

namespace singletonwise
{

    public class SEND_cmd_eventArgs : DoWorkEventArgs
    {
        public byte[] cmd;
        // nebo tady rovnou řešit to inner etc
        public SEND_cmd_eventArgs(byte id, byte[] innerCmd): base(null)
        {
            cmd = CREATE_cmdFromInner(id, innerCmd);
            //base((object)cmd);
        }

        private byte[] CREATE_cmdFromInner(byte id, byte[] innerCmd)
        {
            //..check, length, id etc
            return (innerCmd);
        }
    }

    internal class C_Motor
    {
        public byte id;

        public C_Motor(byte _id)
        {
            id = _id;
        }

        public void SEND_cmd(byte[] cmd)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.DoWork += worker_DoWork;

            SEND_cmd_eventArgs args = new SEND_cmd_eventArgs(id, cmd);
            worker.RunWorkerAsync(args);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //e.Result = ExecuteActions(input);
            //SEND_cmd_eventArgs ev = (SEND_cmd_eventArgs) e;
            e.Result = C_SPI.WriteData(((SEND_cmd_eventArgs)e).cmd);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // catch if response was A-OK
            if (e.Error != null)
            {
                //ie Helpers.HandleCOMException(e.Error);
            }
            else
            {
                //var results = e.Result as List<object>;
            }
        }
    }		
}
