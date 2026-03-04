using System;
using System.Data.SqlClient;

namespace DBNamespace
{
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
        public static void FillingUser(string UserName, string Connect, DateTime datetime) //Метод для сохранение данных в таблицу Users :3
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
