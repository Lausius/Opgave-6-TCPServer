using System;
using System.IO;
using System.Net.Sockets;

namespace FanOutputTCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client.Start();
        }
    }

    class Client
    {
        public static void Start()
        {
            TcpClient clientSocket = new TcpClient("localhost", 4646);

            using (clientSocket)
            {
                Console.WriteLine("Server found");
                Stream ns = clientSocket.GetStream();
                StreamReader sr = new StreamReader(ns);
                StreamWriter sw = new StreamWriter(ns);
                sw.AutoFlush = true;
                while (true)
                {
                    Console.WriteLine("Vælg en af følgende valgmuligheder: 'Hent' 'HentAlle' og 'Gem'");
                    string method = Console.ReadLine();
                    switch (method.ToUpper())
                    {
                        case "HENT":
                            Console.WriteLine("Hvilket element/ID vil du hente?");
                            string id = Console.ReadLine();
                            sw.WriteLine(method.ToUpper());
                            sw.WriteLine(id);
                            string receievedJsonString = sr.ReadLine();
                            Console.WriteLine(receievedJsonString);
                            break;

                        case "HENTALLE":
                            sw.WriteLine(method.ToUpper());
                            string receivedJsonString = sr.ReadLine();
                            Console.WriteLine(receivedJsonString);
                            break;

                        case "GEM":
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
