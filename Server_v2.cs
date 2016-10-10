using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPServer
{
    public class Server_v2
    {
        public static int defaultUDPSendingPort = 4023;
        public static int defaultUDPListeningPort = 4023;
        public static string defaultIpAddress = "192.168.2.22";
        public static Thread UDPThread = null;
        public static FormMain formMain = null;
        public static bool Kill = false;
        public static bool noError = false;
        public static bool isActive = false;

        static IPAddress IPSendTo;
        static IPEndPoint ipep;
        static UdpClient socket;
        static IPEndPoint sender;

        public Server_v2(string s, int listeningPort, int sendingPort)
        {
            isActive = true;
            defaultUDPListeningPort = listeningPort;
            defaultUDPSendingPort = sendingPort;
            defaultIpAddress = s;
            
            IPSendTo = IPAddress.Parse(defaultIpAddress);
            ipep = new IPEndPoint(IPAddress.Any, defaultUDPListeningPort);
            Console.WriteLine("Phase 1 \n");
            //ipep = new IPEndPoint(IPSendTo, defaultUDPListeningPort);
            socket = new UdpClient(ipep);
            Console.WriteLine("Phase 2 \n");

            sender = new IPEndPoint(IPSendTo, defaultUDPSendingPort);
            Console.WriteLine("Phase 3 \n");

            UDPThread = new Thread(new ThreadStart(StartReceive));
            UDPThread.IsBackground = true;
            UDPThread.Start();

        } // End constructor

        public void StartReceive()
        {
            using (socket)
            {
                formMain.SetStatusText("Status: OK");
                while (true)
                {
                    byte[] data = new byte[1024];

                    if (socket.Available > 0)
                    {
                        Console.WriteLine("Phase Receive \n");
                        //data = socket.Receive(ref sender);
                        IPEndPoint tempasd = new IPEndPoint(IPAddress.Any, 0);
                        data = socket.Receive(ref tempasd);
                        Console.WriteLine("Phase Receive done \n");

                        byte[] finalData = AdjustBuffer(data);

                        // Extract information from message
                        MessageProcesser.ExtractMessageInformations(finalData);
                        // Form message to display
                        string decodedMessage = MessageProcesser.formMessage();
                        formMain.SetText(decodedMessage);

                        //Form message for text file
                        decodedMessage = MessageProcesser.FormMessageForFile();
                        Storage.UpdateFolderTimeStamp(); // Update file, in case it is new day
                        Storage.AppendTextToFile(decodedMessage);

                        //IsoletedStorage.WriteToStorage(decodedMessage); // For now disabled

                        // Send ACK
                        string packetStamp = MessageProcesser.GetPacketNumber();
                        data = Encoding.ASCII.GetBytes("ACK" + " " + packetStamp);
                        socket.Send(data, data.Length, sender);
                        Console.WriteLine("Phase Data sent \n");

                    }

                    if (Kill == true)
                    {
                        if (socket.Available > 0)
                        {
                            Console.WriteLine("More data needs to be read first \n");
                            continue;
                        }
                        Kill = false;
                        Console.WriteLine("Phase Exit \n");
                        isActive = false;
                        return;
                    }
                    Thread.Sleep(10);
                } // While
            } // End using(socket)

        } // StartReceive







        byte[] AdjustBuffer(byte[] data)
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


    }
}
