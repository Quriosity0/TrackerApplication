using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Data.SqlClient;
using DBNamespace;

namespace ServerApplication
{
    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static void Main()
        {
            TrakerBD.CreateDatabase();
            TrakerBD.CreateTables();

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

        static void SendMessageToClient(TcpClient client, string message)
        {
            try
            {
                if (client?.Connected == true)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] msgBuffer = Encoding.UTF8.GetBytes(message);
                    stream.Write(msgBuffer, 0, msgBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения клиенту: {ex.Message}");
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            string clientName = "НН";
            string password = "пароль"; // здесь пароль
            DateTime loginTime = DateTime.Now;

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                // Получаем имя клиента
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                }

                // пароль
                buffer = new byte[1024];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string clientPassword = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    if (clientPassword != password)
                    {
                        byte[] Msg = Encoding.UTF8.GetBytes("Неверный пароль.");
                        stream.Write(Msg, 0, Msg.Length);
                        return; //неверный пароль
                    }
                }

                loginTime = DateTime.Now;
                TrakerBD.FillingUser(clientName, "Подключен", loginTime, password);
                Console.WriteLine($"[{loginTime}] Клиент [{clientName}] подключился.");

                while (client.Connected)
                {
                    if (client.Client.Poll(10, SelectMode.SelectRead) && client.Client.Available > 0)
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                            Console.WriteLine($"Сообщение от [{clientName}]: {message}");
                            //здесь  Обрабатываем сообщения
                            SendMessageToClient(client, $"Принято: {message}");
                        }
                        else
                        {
                            // Клиент отключился;
                        }
                    }
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
                DateTime logoutTime = DateTime.Now;
                TrakerBD.FillingUser(clientName, "Отключен", logoutTime, password);
                Console.WriteLine($"[{logoutTime}] Клиент [{clientName}] отключился.");
                Console.WriteLine($"Клиентов онлайн: {clients.Count}");
            }
        }
    }
}
