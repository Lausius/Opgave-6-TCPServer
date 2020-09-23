using Newtonsoft.Json;
using Opgave1_Model_Klasse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FanOutputTCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }
    }

    class Server
    {
        public List<FanOutput> fanOutputReadings = new List<FanOutput>()
        {
            new FanOutput(1, "First Output", 15, 30),
            new FanOutput(2, "Second Output", 18, 41),
            new FanOutput(3, "Third Output", 24, 69),
            new FanOutput(4, "Fourth Output", 22, 49),
            new FanOutput(5, "Fifth Output", 20, 75),
            new FanOutput(6, "Sixth Output", 16, 50),
        };
        int clientsConnected = 0;
        public void Start()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Loopback, 4646);
            serverSocket.Start();
            while (true)
            {
                TcpClient connectionSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("Server Activated");
                clientsConnected++;
                Console.WriteLine("Client " + clientsConnected + " has connected to the server.");
                Task.Run(() => DoClient(connectionSocket));
            }
        }

        public void DoClient(TcpClient socket)
        {
            using (socket)
            {
                while (true)
                {
                    try
                    {
                        Stream ns = socket.GetStream();
                        StreamReader sr = new StreamReader(ns);
                        StreamWriter sw = new StreamWriter(ns);
                        sw.AutoFlush = true;
                        var message = sr.ReadLine();
                        switch (message)
                        {
                            case "HENT":
                                int n;
                                var id = sr.ReadLine();
                                if (int.TryParse(id, out n))
                                {
                                    FanOutput fanOutputItem = fanOutputReadings.Find(i => i.Id == n);
                                    if (fanOutputItem != null)
                                    {
                                        string fanOutputItemJson = JsonConvert.SerializeObject(fanOutputItem);
                                        sw.WriteLine(fanOutputItemJson);
                                    }
                                    else
                                    {
                                        sw.WriteLine("Denne måling eksisterer ikke");
                                        break;
                                    }
                                }

                                sw.WriteLine("ID skal være af typen 'int'");
                                break;

                            case "HENTALLE":
                                string fanOutputJsonString = JsonConvert.SerializeObject(fanOutputReadings);
                                sw.WriteLine(fanOutputJsonString);
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Connection ended unexpectedly");
                        socket.Close();
                    }

                    if (socket.Connected == false)
                    {
                        Console.WriteLine("Client: " + clientsConnected + " has left the server.");
                        clientsConnected--;
                        socket.Dispose();
                        break;
                    }
                }
            }
        }
    }
}
