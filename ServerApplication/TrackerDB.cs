using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DBNamespace
{
    public class TrakerBD
    {
        static string masterConn =
             @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";

        static string connString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrackerDB;Integrated Security=True";

        private static List<string> DataUser = new List<string>();
        private static List<string> DataTracker = new List<string>();

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
            UserPassword NVARCHAR(8),
            Connect NVARCHAR(90),
            DateTime DATETIME,
            
                );  

        IF OBJECT_ID('TrackerData') IS NULL
        CREATE TABLE TrackerData(
            IdUser INT,
            FOREIGN KEY (IdUser) REFERENCES Users(Id),
            NameApplication NVARCHAR(120),
            TimeUse INT,
            LaunchDate DATETIME,
            ClosingDate DATETIME
                 )";

                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Таблицы созданы");
        }
        public static void FillingUser(string UserName, string Connect, DateTime datetime, string Password) //Метод для сохранение данных в таблицу Users :3
        {
            string sql = "INSERT INTO Users (UserName, Connect,DateTime,UserPassword) VALUES  (@username, @connect, @datetime,@userpassword)";
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", UserName);
                cmd.Parameters.AddWithValue("@connect", Connect);
                cmd.Parameters.AddWithValue("@datetime", datetime);
                cmd.Parameters.AddWithValue("@userpassword", Password);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Данные заполнены");
        }
        public static void FillingTrackerData(int Id, string NameApplication, int TimeUse, DateTime LaunchDate, DateTime ClosingDate) //Метод для сохранение данных в таблицу TrackerData :3
        {
            string sql = "INSERT INTO TrackerData (NameApplication, TimeUse, LaunchDate, ClosingDate,IdUser) VALUES  (@nameapplication, @timeuse, @launchdate, @closingdate,@iduser)";
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@nameapplication", NameApplication);
                cmd.Parameters.AddWithValue("@timeuse", TimeUse);
                cmd.Parameters.AddWithValue("@launchdate", LaunchDate);
                cmd.Parameters.AddWithValue("@closingdate", ClosingDate);
                cmd.Parameters.AddWithValue("@iduser", Id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Данные заполнены");
        }
        public static List<string> GetDataUsers(int Id) // Выводит данные таб. Usres
        {
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        string UserName = reader["UserName"].ToString();
                        string UserPassword = reader["UserPassword"].ToString();
                        string Connect = reader["Connect"].ToString();
                        string DateTime = reader["DateTime"].ToString();

                        DataUser.Add(UserName);
                        DataUser.Add(UserPassword);
                        DataUser.Add(Connect);
                        DataUser.Add(DateTime);
                    }
                    else
                    {
                        Console.WriteLine("Запись не найдена.");
                    }
                    return DataUser;
                }
            }
        }
        public static List<string> GetTrackerData(int id)
        {
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT * FROM TrackerData WHERE IdUser = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string Name = reader["NameApplication"].ToString();
                        string Time = reader["TimeUse"].ToString();
                        string Launch = reader["LaunchDate"].ToString();
                        string Closing = reader["ClosingDate"].ToString();
                        DataTracker.Add(Name);
                        DataTracker.Add(Time);
                        DataTracker.Add(Launch);
                        DataTracker.Add(Closing);
                    }
                    else
                    {
                        Console.WriteLine("Запись не найдена.");
                    }
                    return DataTracker;
                }
            }
        }
    }

}