using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Data.SqlClient;

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
                TrakerBD.FillingUser(clientName, "Подключен", loginTime);

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
                TrakerBD.FillingUser(clientName, "отключен", logoutTime);

                Console.WriteLine($"[{logoutTime}] Клиент [{clientName}] отключился.");
                Console.WriteLine($"Клиентов онлайн: {clients.Count}");
            }
        }
    }

    class TrakerBD
    {
        static string masterConn =
             @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";

        static string connString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrackerDB;Integrated Security=True";

        public static void CreateDatabase() //Метод для создания Базы данных :3
        {
            string masterConn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
            string createDbSql = "IF DB_ID('TrackerDB') IS NULL CREATE DATABASE TrackerDB;";
            using (var conn = new SqlConnection(masterConn))
            using (var cmd = new SqlCommand(createDbSql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("База данных создана");
        }
        public static void CreateTables() //Метод для создания табличек :3
        {
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                conn.Open();
                cmd.CommandText = @" 
        IF OBJECT_ID('Users') IS NULL 
        CREATE TABLE Users ( 
            Id INT PRIMARY KEY IDENTITY(1,1), 
           UserName NVARCHAR(90),
            Connect NVARCHAR(90),
            DateTime DATETIME,
            
                );  

        IF OBJECT_ID('TrackerData') IS NULL
        CREATE TABLE TrackerData(
            NameApplication NVARCHAR(120),
            TimeUse INT,
            LaunchDate DATETIME,
            ClosingDate DATETIME
                 )";

                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Таблицы созданы");
        }
        public static void FillingUser(string UserName, string Connect, string datetime) //Метод для сохранение данных в таблицу Users :3
        {
            string sql = "INSERT INTO Users (UserName, Connect,DateTime) VALUES  (@username, @connect, @datetime)";
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", UserName);
                cmd.Parameters.AddWithValue("@connect", Connect);
                cmd.Parameters.AddWithValue("@datetime", datetime);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Данные заполнены");
        }
    }

}

