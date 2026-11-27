using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using DBNamespace;

namespace ServerApplication
{
    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>();
        static readonly object clientsLock = new object(); // Для безопасной работы со списком из разных потоков

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
                lock (clientsLock)
                {
                    clients.Add(client);
                }

                Thread clientThread = new Thread(HandleClient);
                clientThread.IsBackground = true;
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
                    stream.Flush();
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
            bool isAuthorized = false; // Флаг, что пользователь реально вошёл в аккаунт
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
                        return; // Клиент закрыл окно до отправки данных — штатный выход
                    }
                    mode = mode.Trim();

                    // Читаем логин
                    string loginLine = reader.ReadLine();
                    if (loginLine == null) return;
                    clientName = loginLine.Trim();
                    Console.WriteLine($"Логин: {clientName}");

                    // Читаем пароль
                    string passwordLine = reader.ReadLine();
                    if (passwordLine == null) return;
                    password = passwordLine.Trim();
                    Console.WriteLine("Пароль получен");

                    // Обработка регистрации
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

                    // Обработка входа
                    bool loginOk = RegistrationAndLogin.RegistrationAndLogin.LoginName(clientName);
                    bool passwordOk = RegistrationAndLogin.RegistrationAndLogin.LoginPassword(password);

                    if (loginOk && passwordOk)
                    {
                        SendMessageToClient(client, "true");
                        isAuthorized = true;
                    }
                    else
                    {
                        SendMessageToClient(client, "false");
                        return;
                    }

                    loginTime = DateTime.Now;
                    TrakerBD.FillingUser(clientName, "Подключен", loginTime, password);
                    Console.WriteLine($"[{loginTime}] Клиент [{clientName}] подключился.");

                    // Основной цикл сессии после входа
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        // Блокирующее чтение: ждёт данные или мгновенно возвращает 0 при закрытии Form2
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        // bytesRead == 0 означает, что клиент закрыл Form2 или вышел из аккаунта
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        if (message == "logout" || message == "exit")
                        {
                            break;
                        }

                        Console.WriteLine($"Сообщение от [{clientName}]: {message}");
                        SendMessageToClient(client, $"Принято: {message}");
                    }
                }
            }
            catch (IOException)
            {
                // Клиент принудительно закрыл приложение (например, через диспетчер задач или крестик) — не считаем это сбоем сервера
            }
            catch (SocketException)
            {
                // Штатный разрыв сокета
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка с клиентом {clientName}: {ex.Message}");
            }
             finally
            {
                int onlineCount;
                lock (clientsLock)
                {
                    clients.Remove(client);
                    onlineCount = clients.Count;
                }

                try { client.Close(); } catch { }

                // Срабатывает ТОЛЬКО если пользователь успешно вошёл в аккаунт
                if (isAuthorized)
                {
                    DateTime logoutTime = DateTime.Now;
                    TrakerBD.FillingUser(clientName, "Отключен", logoutTime, password);
                    Console.WriteLine($"[{logoutTime}] Клиент [{clientName}] отключился.");
                }

                Console.WriteLine($"Клиентов онлайн: {onlineCount}");
            }
        }
    }
}
