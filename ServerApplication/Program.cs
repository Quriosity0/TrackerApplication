using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Data.SqlClient;
using System.IO;
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
            string password = "";
            DateTime loginTime = DateTime.Now;

            try
            {
                NetworkStream stream = client.GetStream();

                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Читаем режим (регистрация или вход)
                    string mode = reader.ReadLine();
                    if (mode == null)
                    {
                        Console.WriteLine("Клиент разорвал соединение при передаче режима.");
                        return;
                    }
                    mode = mode.Trim();

                    // Читаем логин
                    string loginLine = reader.ReadLine();
                    if (loginLine == null)
                    {
                        Console.WriteLine("Клиент разорвал соединение при передаче логина.");
                        return;
                    }
                    clientName = loginLine.Trim();
                    Console.WriteLine($"Логин: {clientName}");

                    // Читаем пароль
                    string passwordLine = reader.ReadLine();
                    if (passwordLine == null)
                    {
                        Console.WriteLine("Клиент разорвал соединение при передаче пароля.");
                        return;
                    }
                    password = passwordLine.Trim();
                    Console.WriteLine($"Пароль получен");

                    // обработка регистрации
                    if (mode == "register")
                    {
                        bool nameOk = RegistrationAndLogin.RegistrationAndLogin.RegistrName(clientName);
                        bool passOk = RegistrationAndLogin.RegistrationAndLogin.RegistrPassword(password);

                        if (nameOk && passOk)
                        {
                            TrakerBD.FillingUser(clientName, "Подключен", loginTime, password);
                            SendMessageToClient(client, "true");
                        }
                        else
                        {
                            SendMessageToClient(client, "false");
                        }

                        return;
                    }

                    // обработка входа (для любого другого mode)
                    bool loginOk = RegistrationAndLogin.RegistrationAndLogin.LoginName(clientName);
                    bool passwordOk = RegistrationAndLogin.RegistrationAndLogin.LoginPassword(password);

                    if (loginOk && passwordOk)
                    {
                        SendMessageToClient(client, "true");
                    }
                    else
                    {
                        SendMessageToClient(client, "false");
                        return;
                    }

                    loginTime = DateTime.Now;
                    TrakerBD.FillingUser(clientName, "Подключен", loginTime, password);
                    Console.WriteLine($"[{loginTime}] Клиент [{clientName}] подключился.");

                    // Дальнейший обмен сообщениями (если нужен)
                    byte[] buffer = new byte[1024];
                    while (client.Connected)
                    {
                        if (client.Client.Poll(10, SelectMode.SelectRead) && client.Client.Available > 0)
                        {
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                                Console.WriteLine($"Сообщение от [{clientName}]: {message}");
                                SendMessageToClient(client, $"Принято: {message}");
                            }
                        }
                        Thread.Sleep(500);
                    }
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