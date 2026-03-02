using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static void Main()
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 5000);

            NetworkStream stream = client.GetStream();

         Console.Write("Введите имя: ");//временное тестирвание
            string name = Console.ReadLine();

            byte[] nameData = Encoding.UTF8.GetBytes(name);
            stream.Write(nameData, 0, nameData.Length);

            Thread receiveThread = new Thread(() =>
            {
                byte[] buffer = new byte[1024];

                while (true)
                {
                    try
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;

                        string message =
                            Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        Console.Write(message);
                    }
                    catch
                    {
                        break;
                    }
                }
            });

            receiveThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
    }
}