﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOut
{
    // LOG
    public partial class C_Packet
    {
        // make it into HASHTABLE
        static string[] errStr = {     
                                     "Input Voltage Error"
                                  , "Angle Limit Error"
                                  , "Overheating Error"
                                  , "Range Error"
                                  , "Checksum Error"
                                  , "Overload Error"
                                  , "Instruction Error"
                              };
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #region LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        public static void LOG_sent(C_Packet packet)
        {
            StringBuilder str = new StringBuilder();
            str.Append(string.Format(
                "Sent Instruction packet!\tMotId[{0}]\tEcho[{1}]\tReturnPacket[{2}]",
                packet.IdByte, packet.packetEcho, packet.packetReturn
                ));
            LOG_packInstruct(str.ToString());
        }

        public static void LOG_echo(C_Packet packet)
        {
            StringBuilder str = new StringBuilder();
            str.Append(string.Format(
                "Got Status packet!\tMotId[{0}]\tEcho[{1}]\tReturnPacket[{2}]",
                packet.IdByte, packet.packetEcho, packet.packetReturn
                ));
            LOG_packInstruct(str.ToString());
        }

        public static void LOG_ex(C_Packet packet, Exception ex)
        {
            if (packet.IsConsistent == false)
            {
                LOG_packet(string.Format(
                        "Inconsistent packet:\n{0}\n{1}",
                        GET_packetInfo(packet),
                        GET_exceptionInfo(ex)
                    ));
            }
        }

        public static void LOG_errorByte(C_Packet packet)
        {
            LOG_err(packet,
                GET_errorByteInfo(packet)
                );
        }

        public static void LOG_err(C_Packet packet, string _msg)
        {
            LOG_packet(string.Format(
                        "Error packet:\n{0}\n{1}",
                        GET_packetInfo(packet),
                        _msg
                    ));
        }


        public static void LOG_packet(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.packet, _msg);
        }

        public static void LOG_packInstruct(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.packInstruct, _msg);
        }

        public static void LOG_packStatus(string _msg)
        {
            C_Logger.Instance.LOG(e_LogMsgSource.packStatus, _msg);
        }

        public static string GET_exceptionInfo(Exception ex)
        {
            return ("Catched exception: " + ex.Message);
        }

        public static string GET_packetInfo(C_Packet packet)
        {
            return string.Format(
                "MotId[{0}]\tMot[{1}]\tEcho[{2}]\tReturn[{3}]\tBytes[{4}]",
                packet.IdByte, GET_rotMotorInfo(packet),
                packet.packetEcho, packet.packetReturn,
                C_CONV.byteArray2strHex_space(packet.PacketBytes.ToArray())
                    );
        }

        public static string GET_errorByteInfo(C_Packet packet)
        {
            StringBuilder str = new StringBuilder();

            for (int b = 0; b < 7; b++)
                if (C_CONV.GET_bit(packet.instructionOrErrorByte, b) == true)
                {
                    str.AppendLine(string.Format("ID[{0}] error: {1}", packet.idByte, errStr[b]));
                }
            return str.ToString();
        }

        public static string GET_rotMotorInfo(C_Packet packet)
        {
            if (packet.idByte == C_DynAdd.ID_BROADCAST)
            {
                return "Broadcast";
            }
            else
            {
                return packet.rotMotor.ToString();
            }

        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        #endregion LOG
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }
}
