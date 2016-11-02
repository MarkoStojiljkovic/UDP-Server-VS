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
                formMain.SetStatusText("Status: Running");
                formMain.ChangeButtonState(true);
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

                        if (data.GetLength(0) == 4) // Is this get status response
                        {
                            MessageProcesser.ProcessStatusResponse(data);
                        }
                        else // For now it is only received Alarm 
                        {
                            byte[] finalMessage = MessageProcesser.ProcessAlarmResponse(data); // This will: decode, print text, save log and return ACK
                            socket.Send(finalMessage, finalMessage.Length, sender);
                            Console.WriteLine("Phase Data sent \n");
                        }
                        

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





        public void SendCommand(string s)
        {
            byte[] data = Encoding.ASCII.GetBytes(s);
            try
            {
                socket.Send(data, data.Length, sender);
                Console.WriteLine("Sending command: {0}  = success \n", s);
            }
            catch (Exception)
            {

                Console.WriteLine("Sending command: {0}  = fail \n", s);
            }
        }

        


    }
}
