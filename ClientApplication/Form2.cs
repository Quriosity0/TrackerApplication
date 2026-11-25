using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace projecttrecer_client
{
    public partial class Form2 : Form
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _username;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(TcpClient client, NetworkStream stream, string username) : this()
        {
            _client = client;
            _stream = stream;
            _username = username;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_username))
            {
                this.Text = $"Профиль: {_username}";
                label1.Text = $"Добро пожаловать, {_username}!";
            }
        }

        // Кнопка "Выйти из аккаунта" (привяжите её в дизайнере к этому методу)
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            // Просто вызываем Close() — дальше сработает общий обработчик OnFormClosing
            this.Close();
        }

        // Единый обработчик: сработает И при нажатии на крестик, И при нажатии на кнопку
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show(
                    "Выйти из аккаунта и вернуться на экран входа?",
                    "Выход из аккаунта",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Отменяем закрытие
                    return;
                }
            }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process[] procs = Process.GetProcesses();

            // Закрываем текущую сессию с сервером
            LogoutFromServer();

            base.OnFormClosing(e);
        }

         private void LogoutFromServer()
        {
            try
            {
                if (_client != null && _client.Connected)
                {
                    // Посылаем штатный сигнал завершения (TCP FIN вместо аварийного RST)
                    _client.Client.Shutdown(SocketShutdown.Send);
                }
            }
            catch
            {
                // Игнорируем, если сокет уже был закрыт
            }
            finally
            {
                _stream?.Close();
                _client?.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
                StringBuilder sb = new StringBuilder();

                foreach (Process proc in procs)
                {
                    try
                    {
                        sb.AppendLine(proc.ProcessName);
                    }
                    catch
                    {
                    }
                }

                string sendi = sb.ToString();

                using (TcpClient client = new TcpClient("000.0.0.0", 5000))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(sendi);
                    writer.Flush();
                }

            }
            catch
            {

            }
        }
    }
}
