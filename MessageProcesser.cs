using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    class MessageProcesser
    {
        public static string[] extractedInfo = new string[5];

        public static void ExtractMessageInformations(byte[] msg)
        {
            var str = System.Text.Encoding.Default.GetString(msg);
            string tempMsg = str.Trim(' ');
            tempMsg = TrimEverything(tempMsg, ' ');
            string[] tempMsg2 = tempMsg.Split();

            if (tempMsg2.Length < 6)
            {
                extractedInfo[0] = tempMsg2[0];
                extractedInfo[1] = tempMsg2[1][4].ToString();
                extractedInfo[2] = tempMsg2[2];
                extractedInfo[3] = tempMsg2[3];
                extractedInfo[4] = tempMsg2[4];
            }
            else
            {
                extractedInfo[0] = tempMsg2[0] + " " + tempMsg2[1];
                extractedInfo[1] = tempMsg2[2][4].ToString();
                extractedInfo[2] = tempMsg2[3];
                extractedInfo[3] = tempMsg2[4];
                extractedInfo[4] = tempMsg2[5];
            }

        } // End ExtractMessageInformations

        public static string formMessage()
        {
            string alarmType = extractedInfo[0];
            string alarmStatus = extractedInfo[1];
            string alarmDateStamp = extractedInfo[2];
            string alarmTimeStamp = extractedInfo[3];
            string alarmPacketStamp = extractedInfo[4];


            string decodedMessage =
                        alarmType
                        + " JE:" + " " + alarmStatus
                        + Environment.NewLine
                        + alarmDateStamp
                        + Environment.NewLine
                        + alarmTimeStamp
                        + Environment.NewLine  
                        + alarmPacketStamp +
                        Environment.NewLine + Environment.NewLine;

            return decodedMessage;
        }

        public static string FormMessageForFile()
        {
            string decodedMessage = extractedInfo[0] + " " + extractedInfo[1] + " " + extractedInfo[2] + " " + extractedInfo[3] + "  " + extractedInfo[4] + " "  + DateTime.Now + Environment.NewLine;
            return decodedMessage;
        }


        public static string TrimEverything(string s ,char x)
        {
            int repeatFlag = 0;
            StringBuilder sb = new StringBuilder(100);
            foreach (char curChar in s)
            {
                if (curChar == x)
                {
                    if (repeatFlag == 1)
                    {
                        continue; // Skip appending
                    }
                    else
                    {
                        sb.Append(curChar);
                        repeatFlag = 1;
                    }

                }
                else
                {
                    repeatFlag = 0;
                    sb.Append(curChar);
                }
                
            }

            return sb.ToString();
        }

        public static string GetPacketNumber()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < extractedInfo[4].Length; i++) // Ignore first char
            {
                sb.Append(extractedInfo[4][i]);
            }
            return sb.ToString();
        } 

        


    }
}
