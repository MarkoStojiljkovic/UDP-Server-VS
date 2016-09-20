using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDPServer
{


    public class Server
    {
        public int defaultUDPSendingPort = 4023;
        public int defaultUDPListeningPort = 4023;
        public string defaultIpAddress = "192.168.2.22";
        public Thread UDPThread = null;
        public FormMain formMain = null;
        public bool Kill = false;
        public bool KillRdy = false;

        public Server(string s, int listeningPort, int sendingPort)
        {
            defaultUDPListeningPort = listeningPort;
            defaultUDPSendingPort = sendingPort;
            defaultIpAddress = s;
            try
            {
                //Starting the UDP Server thread.
                UDPThread = new Thread(new ThreadStart(StartReceive));
                UDPThread.IsBackground = true;
                UDPThread.Start();
                Console.WriteLine("Started SampleTcpUdpServer's UDP Receiver Thread!\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("An UDP Exception has occurred!" + e.ToString());
                //UDPThread.Abort();
            }
        }

        public void StartReceive()
        {
            byte[] data = new byte[1024];
            IPAddress IPSendTo = IPAddress.Parse(defaultIpAddress);

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, defaultUDPListeningPort);
            UdpClient socket;
            try
            {
                socket = new UdpClient(ipep);
            }
            catch (SocketException)
            {
                formMain.SetStatusText("Status: ERROR");
                Console.WriteLine("Couldn't create socket :( \n");
                //UDPThread.Abort();
                //Thread.Sleep(10);
                return; // Abort all
            }

            Console.WriteLine("Waiting for a client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            /*
            data = socket.Receive(ref sender);

            Console.WriteLine("Message received from {0}:", sender.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            socket.Send(data, data.Length, sender);
            */
            Console.WriteLine("TESTING LINE \n" + Thread.); // Print thread ID
            formMain.SetStatusText("Status: OK");
            while (true)
            {
                if (socket.Available > 0)
                {
                    data = socket.Receive(ref sender); // Crashing here
                    //if (sender.Address.Equals(IPAdressToCompare)) // Is this IP we listen to 
                    //{
                    byte[] temp_data = new byte[100];
                    int y = 0;
                    for (int i = 0; i < data.Length; i++) // Replacing \n with \r\n
                    {
                        if (data[i] == 0xA)
                        {
                            temp_data[y] = 0xD; // carriage return
                            y++;
                            temp_data[y] = 0xA; // new line
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

                    // Extract information from message
                    MessageProcesser.ExtractMessageInformations(temp_data2);
                    // Form message to display
                    string decodedMessage = MessageProcesser.formMessage();
                    formMain.SetText(decodedMessage);

                    //Form message for text file
                    decodedMessage = MessageProcesser.FormMessageForFile();
                    Storage.UpdateFolderTimeStamp(); // Update file, in case it is new day
                    Storage.AppendTextToFile(decodedMessage);

                    //IsoletedStorage.WriteToStorage(decodedMessage); // For now disabled

                    // Send ACK
                    data = Encoding.ASCII.GetBytes("ACK");
                    sender.Port = defaultUDPSendingPort;
                    sender.Address = IPSendTo;
                    socket.Send(data, data.Length, sender);
                    //} // end if
                } // end if
                if (Kill == true)
                {
                    //if (socket.Available > 0)
                    //{
                    //    continue;
                    //}
                    KillRdy = true;
                    socket.Close();
                    Thread.Sleep(200);
                    return;
                }
                Thread.Sleep(10);
            } // while
        } // StartReceive()


        public void killMe()    // :(
        {
            //if (Thread.)
            //{

            //}
            //UDPThread.Abort();
        }


        public string ReadTo(string s, char c)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    return sb.ToString();
                }
                else
                {
                    sb.Append(s[i]);
                }
            }

            return null;
        }


    }//end of class
}//end of namespace
