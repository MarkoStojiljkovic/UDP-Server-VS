using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomTypes;

namespace UDPServer
{
    public class Server_v2
    {
        public static int defaultUDPSendingPort = 4023;
        public static int defaultUDPListeningPort = 4023;
        public static string defaultIpAddress = "192.168.2.254";
        public static Thread UDPThread = null;
        public static FormMain formMain = null;
        public static bool Kill = false;
        public static bool noError = false;
        public static bool isActive = false;

        static IPAddress IPSendTo;
        static IPEndPoint ipep;
        static UdpClient socket;
        static IPEndPoint sender;

        public Server_v2(int listeningPort, int sendingPort)
        {
            isActive = true;
            defaultUDPListeningPort = listeningPort;
            defaultUDPSendingPort = sendingPort;
            
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
                            string xmlData = System.Text.Encoding.Default.GetString(data);
                            senderInfo si = new senderInfo();
                            // Assign default value
                            si.errorCode = "none";

                            si = MessageProcesserXML.ProcessAlarmResponseXML(xmlData);
                            //byte[] finalMessage = MessageProcesserXML.ProcessAlarmResponseXML(xmlData); // This will: decode, and fill struct
                            //if (System.Text.Encoding.Default.GetString(finalMessage).Equals("error", StringComparison.OrdinalIgnoreCase))
                            if (si.error)
                            {
                                continue; // Something went wrong, abort
                            }

                            //string sIP = MessageProcesserXML.
                            si.trueIPAddress = HashtableAndDatabaseClass.FetchIPFromMac(si.mac);
                            if (si.trueIPAddress == null)
                            {
                                continue; // Hashtable doesn't countain that MAC, abort everything (writing in log and display too)
                            }

                            // Write log and to display
                            WriteOnDisplayAndStorage(si); // Will use information stored on si structure


                            // Form responce
                            byte[] finalMessage = Encoding.ASCII.GetBytes("ACK " + si.seq);
                            
                            sender = new IPEndPoint(IPAddress.Parse(si.trueIPAddress), defaultUDPSendingPort);
                            
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
                        formMain.SetStatusText("Status: Stopped");
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

        private void WriteOnDisplayAndStorage(senderInfo si)
        {
            string MAC = si.mac;
            string alarmName = si.name;
            string status = si.value.ToString();
            string timeStamp = si.time;
            string packetNum = si.seq;

            string messageToAppend = MAC + " " + alarmName + " " + status + " " + timeStamp + " " + packetNum + Environment.NewLine;
            formMain.SetText(messageToAppend);

            Storage.UpdateFolderTimeStamp(); // Update file, in case it is new day
            Storage.AppendTextToFile(messageToAppend);

        }

        public void updateSender(string ip) // This is called to update sender for manualy added devices so you can send commands and not wait for them to send first
        {
            try
            {
                IPAddress tempIPSendTo = IPAddress.Parse(ip);
                sender = new IPEndPoint(tempIPSendTo, defaultUDPSendingPort); // Update current ip with default port

            }
            catch (Exception)
            {
                return;
            }
        }

    }
}
