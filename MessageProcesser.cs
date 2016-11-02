using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPServer
{
    class MessageProcesser
    {
        private static string[] alarmStrings = {
            "DOOR EVENT",           //0
            "REL1",                 //1
            "REL2",                 //2
            "NOT USED",             //3
            "NESTANAK NAPONA",      //4
            "POVISENA TEMPERATURA", //5
            "OTVORENO KOLO",        //6
            "NEUSPELO ISLJUCENJE",  //7
            "PREKORACENJE STRUJE",  //8
            "POTKORACENJE STRUJE"   //9
        };


        public static string[] extractedInfo = new string[5];

        private static void ExtractMessageInformations(byte[] msg)
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

        private static string formMessage()
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

        private static string FormMessageForFile()
        {
            string decodedMessage = extractedInfo[0] + " " + extractedInfo[1] + " " + extractedInfo[2] + " " + extractedInfo[3] + "  " + extractedInfo[4] + " "  + DateTime.Now + Environment.NewLine;
            return decodedMessage;
        }


        private static string TrimEverything(string s ,char x)
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

        private static string GetPacketNumber()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < extractedInfo[4].Length; i++) // Ignore first char
            {
                sb.Append(extractedInfo[4][i]);
            }
            return sb.ToString();
        }

        private static byte[] AdjustBuffer(byte[] data) // Replace /n with " "
        {
            byte[] temp_data = new byte[100];
            int y = 0;
            //for (int i = 0; i < data.Length; i++) // Replacing \n with \r\n
            //{
            //    if (data[i] == 0xA)
            //    {
            //        temp_data[y] = 0xD; // carriage return
            //        y++;
            //        temp_data[y] = 0xA; // new line
            //        y++;
            //    }
            //    else
            //    {
            //        temp_data[y] = data[i];
            //        y++;
            //    }
            //}

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0xA)
                {
                    temp_data[y] = 0x20; //  ' '
                    y++;
                }
                else
                {
                    temp_data[y] = data[i];
                    y++;
                }
            }

            // Process message
            int endPtr = 0;
            for (int i = 0; i < temp_data.Length; i++) // Find actual length
            {
                if (temp_data[i] == 0)
                {
                    endPtr = i;
                    break;
                }
            }

            byte[] temp_data2 = new byte[endPtr]; // Create new array with precise length

            for (int i = 0; i < temp_data2.Length; i++)  // And fill it
            {
                temp_data2[i] = temp_data[i];
            }

            return temp_data2;
        }


        public static void ProcessStatusResponse(byte[] s)
        {
            //s[1] = 0x3; // I used this just for testing purpose
            //s[2] = 0x4;
            //s[3] = 0x7;
            UInt32 alarms = 0;
            // Merge 4 bytes into single uint
            alarms = (UInt32) s[3];
            alarms = alarms << 8;
            alarms |= (UInt32)s[2];
            alarms = alarms << 8;
            alarms |= (UInt32)s[1];
            alarms = alarms << 8;
            alarms |= (UInt32)s[0];

            alarms = alarms ^ 0x00000001; // Door alarm is inverted

            StringBuilder sb = new StringBuilder();
            UInt32 flag;
            int cnt = 0;
            UInt32 i = 1;
            UInt32 skipMask = 0x00000008; // Skip alarm at bit position 3



            while (cnt < 10)
            {
                if ((skipMask & i) != 0) // Skip alarms at marked positions
                {
                    cnt++;
                    i = i << 1;
                    continue;
                }


                if ((i & alarms) != 0)
                {
                    flag = 1;
                }
                else
                {
                    flag = 0;
                }
                sb.Append(alarmStrings[cnt] + " je: " + flag.ToString() + Environment.NewLine);
                cnt++;
                i = i << 1;
            }

            //Console.WriteLine(sb.ToString());
            //System.Windows.Forms.MessageBox.Show(sb.ToString());

            Thread t = new Thread(() => MessageBox.Show(sb.ToString())); // I dont want blocking message box
            t.Start();

        }

        public static byte[] ProcessAlarmResponse(byte[] s)
        {
            byte[] finalData = AdjustBuffer(s);
            // Extract information from message
            MessageProcesser.ExtractMessageInformations(finalData);
            // Form message to display
            string decodedMessage = MessageProcesser.formMessage();
            Server_v2.formMain.SetText(decodedMessage);


            //Form message for text file
            decodedMessage = MessageProcesser.FormMessageForFile();
            Storage.UpdateFolderTimeStamp(); // Update file, in case it is new day
            Storage.AppendTextToFile(decodedMessage);

            //IsoletedStorage.WriteToStorage(decodedMessage); // For now disabled

            // Send ACK
            string packetStamp = MessageProcesser.GetPacketNumber();
            return Encoding.ASCII.GetBytes("ACK" + " " + packetStamp);
        }



    }
}
