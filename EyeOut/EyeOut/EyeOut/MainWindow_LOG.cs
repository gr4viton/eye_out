using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO.Ports;
using System.Threading;

namespace EyeOut
{
    /// <summary>
    /// Logging system
    /// </summary>
    public partial class MainWindow : Window
    {
        string log;
        string logSent;
        string logRec;
        public static bool logChanged = false;

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // EVENTinionation
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


        //Here is some code I want to be executed
        //when WANNA_LOG_msgAppendLine fires.
        public void h_LOG_msgAppendLine(string msg)
        {
            LOG_msgAppendLine(msg);
        }

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // LOG
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void LOG_cmdSent(Byte[] cmd)
        {
            LOG_cmd(cmd, e_cmd.sent);
        }
        private void LOG_cmdRec(Byte[] cmd)
        {
            LOG_cmd(cmd, e_cmd.received);
        }

        private void LOG_cmdRec_check(Byte[] cmd, Byte rec_checkSum)
        {
            // check for [checksum error] and cmd [error byte] sub-bites disambiguation
            Byte calc_checkSum = C_CheckSum.GET_checkSum(cmd);
            if (C_CheckSum.CHECK_checkSum(calc_checkSum, rec_checkSum))
            //if( calc_check == 0 )
            {
                //MessageBox.Show(string.Format("cmd[{0}] = {1}", i_cmdError, cmd[i_cmdError]));
                if (cmd[i_cmdError] == 0)
                    // no error
                    LOG_cmd(cmd, e_cmd.received);
                else
                {
                    LOG_cmd(cmd, e_cmd.receivedWithError);
                    LOG_cmdError(cmd[i_cmdId], cmd[i_cmdError]);
                }
            }
            else
            {
                LOG_cmd(cmd, e_cmd.receivedCheckNot);
                LOG_msgAppendLine(String.Format("CheckSumGot != CheckSumCounted :: {0} != {1}", (Byte)rec_checkSum, (Byte)calc_checkSum));
                //LOG_msgAppendLine(String.Format("CheckSumGot = {0} ", (Byte)calc_check));
            }
        }


        private void LOG_cmdError(Byte byId, Byte byError)
        {
            for (int b = 0; b < 7; b++)
                if (GET_bit(byError, b) == true)
                {
                    LOG_msgAppendLine(
                        string.Format("ID[{0}] error: {1}", byId, errStr[b])
                        );
                    logChanged = true;
                }
        }
        private void LOG_cmd(Byte[] cmd, e_cmd type)
        {
            string prefix = "";
            string hex = BitConverter.ToString(cmd).Replace("-", " ");
            string line = hex + "\r\n";

            switch (type)
            {
                case (e_cmd.received):
                    prefix = "Got : ";
                    logRec += line;
                    break;
                case (e_cmd.receivedCheckNot):
                    prefix = "! Got with wrong Checksum: ";
                    logRec += line;
                    break;
                case (e_cmd.receivedWithError):
                    prefix = "! Got with an Error: ";
                    logRec += line;
                    break;
                case (e_cmd.sent):
                    prefix = "Sent: ";
                    logSent += line;
                    break;
            }

            log += prefix + line;
            // raise event LOG_UPDATE_tx!!
            logChanged = true;
        }

        private void LOG_msgAppendLine(string msg)
        {
            LOG_msgAppend(msg + "\r\n");
        }

        private void LOG_msgAppend(string msg)
        {
            log += msg;
            logChanged = true;
            // raise event LOG_UPDATE_tx!!
            //txMotLog.AppendText(msg);
        }




        private void LOG_UPDATE_tx()
        {
            // make it through binding!
            logChanged = false;
            txMotLog.Text = log;
            txMotLog.AppendText(" ");

            
            //txSent.Text = logSent;
            //txSent.AppendText(" ");

            //txReceived.Text = logRec;
            //txReceived.AppendText(" ");

            //txReadBuff.Text = BitConverter.ToString(readBuff).Replace("-", " ");
            //txCurCmd.Text = BitConverter.ToString(curCmd).Replace("-", " ");
            
            
            //txReceived.Select(txReceived.Text.Length, 0);
            //txSent.Select(txSent.Text.Length, 0);
            //txMotLog.Select(txMotLog.Text.Length, 0);
        }
        private void LOG_clear()
        {
            log = "";
            logChanged = true;
        }

    }
}
