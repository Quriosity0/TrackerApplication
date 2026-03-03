using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace ServerApplication
{
    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                clients.Add(client);

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            string clientName = "НН";

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }

                string loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                SaveToDb(clientName, "Подключен", loginTime);

                Console.WriteLine($"[{loginTime}] Клиент [{clientName}] подключился.");

                while (client.Connected)
                {
                    if (client.Client.Poll(1, SelectMode.SelectRead) && client.Client.Available == 0)
                        break;

                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка с клиентом {clientName}: {ex.Message}");
            }
            finally
            {
                clients.Remove(client);
                client.Close();
                string logoutTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                SaveToDb(clientName, "", logoutTime);

                Console.WriteLine($"[{logoutTime}] Клиент [{clientName}] отключился.");
                Console.WriteLine($"Клиентов онлайн: {clients.Count}");
            }
        }

        // Имитация работы с БД
        static void SaveToDb(string name, string status, string timestamp)
        {
            Console.WriteLine($"[DB LOG] {name} | {status} | Время: {timestamp}");
        }
    }
}
