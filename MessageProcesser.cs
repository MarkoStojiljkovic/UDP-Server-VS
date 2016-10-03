using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    class MessageProcesser
    {
        public static string[] extractedInfo = new string[4];

        public static void ExtractMessageInformations(byte[] msg)
        {
            var str = System.Text.Encoding.Default.GetString(msg);
            string[] tempMsg = str.Split();
            StringBuilder sb = new StringBuilder();
            foreach (var tmp in tempMsg)
            {
                if (tmp != "" & tmp != "\0")
                {
                    sb.Append(tmp);
                    sb.Append(';');
                }
            }
            string[] finalStr = sb.ToString().Split(';');
            if (finalStr.Length < 6)
            {
                extractedInfo[0] = finalStr[0];
                extractedInfo[1] = finalStr[1][4].ToString();
                extractedInfo[2] = finalStr[2];
                extractedInfo[3] = finalStr[3];
            }
            else
            {
                extractedInfo[0] = finalStr[0] + " " + finalStr[1];
                extractedInfo[1] = finalStr[2][4].ToString();
                extractedInfo[2] = finalStr[3];
                extractedInfo[3] = finalStr[4];
            }

        } // End ExtractMessageInformations

        public static string formMessage()
        {
            string alarmType = extractedInfo[0];
            string alarmStatus = extractedInfo[1];
            string alarmDateStamp = extractedInfo[2];
            string alarmTimeStamp = extractedInfo[3];


            string decodedMessage =
                        alarmType
                        + " JE:" + " " + alarmStatus
                        + Environment.NewLine
                        + alarmDateStamp
                        + Environment.NewLine
                        + alarmTimeStamp
                        + Environment.NewLine + Environment.NewLine;

            return decodedMessage;
        }

        public static string FormMessageForFile()
        {
            string decodedMessage = extractedInfo[0] + " " + extractedInfo[1] + " " + extractedInfo[2] + " " + extractedInfo[3] + "  " + DateTime.Now + Environment.NewLine;
            return decodedMessage;
        }

        


    }
}
