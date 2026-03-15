using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Net.Sockets;

namespace projecttrecer_client
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private const int EM_SETCUEBANNER = 0x1501;

        private TcpClient client;
        private NetworkStream stream;

        private Label labelError;      // Сообщение об ошибке на вкладке "Вход"
        private Label labelRegError;   // Сообщение об ошибке на вкладке "Регистрация"

        public Form1()
        {
            InitializeComponent();

            // Создаем элементы для вывода сообщений об ошибках
            labelError = new Label() { Visible = false };
            labelError.ForeColor = System.Drawing.Color.Red;
            tabPage1.Controls.Add(labelError);
            labelError.Top = textBox2.Bottom + 10;
            labelError.Left = textBox2.Left;

            labelRegError = new Label() { Visible = false };
            labelRegError.ForeColor = System.Drawing.Color.Red;
            tabPage2.Controls.Add(labelRegError);
            labelRegError.Top = textBox5.Bottom + 10;
            labelRegError.Left = textBox5.Left;

            // Устанавливаем плацебохолдеры
            SetPlaceholder(textBox1, "Никнейм");
            SetPlaceholder(textBox2, "Пароль");
            SetPlaceholder(textBox3, "Никнейм");
            SetPlaceholder(textBox4, "Пароль");
            SetPlaceholder(textBox5, "Подтверждение пароля");

            // Скрываем символы пароля
            textBox2.UseSystemPasswordChar = true;
            textBox4.UseSystemPasswordChar = true;
            textBox5.UseSystemPasswordChar = true;

            // Подключаемся к серверу
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();
            }
            catch (SocketException se)
            {
                ShowError(labelError, $"Ошибка сети: {se.Message}");
            }
            catch (Exception ex)
            {
                ShowError(labelError, $"Общая ошибка: {ex.Message}");
            }
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, placeholder);
        }

        // Метод для отображения ошибок
        private void ShowError(Label errorLabel, string message)
        {
            errorLabel.Text = message;
            errorLabel.Visible = true;
        }

        // Метод для сброса состояния полей и ошибок
        private void ResetRegistration()
        {
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            labelRegError.Visible = false;
        }

        // Логин (Кнопка "Вход")
        private void button1_Click_1(object sender, EventArgs e)
        {
            string un = textBox1.Text.Trim();
            string pd = textBox2.Text;
            byte[] data0 = Encoding.UTF8.GetBytes("Vhod");
            stream.Write(data0, 0, data0.Length);
            stream.Flush();

            if (string.IsNullOrEmpty(un) || string.IsNullOrEmpty(pd))
            {
                ShowError(labelError, "Необходимо ввести имя пользователя и пароль.");
                return;
            }

            if (stream == null || !client.Connected)
            {
                ShowError(labelError, "Нет подключения к серверу.");
                return;
            }

            try
            {

                byte[] data = Encoding.UTF8.GetBytes(un);
               stream.Write(data, 0, data.Length);
                stream .Flush();

                byte[] data1 = Encoding.UTF8.GetBytes(pd);
                stream.Write(data1, 0, data1.Length);
                stream.Flush();

                byte[] responseBuffer = new byte[256];
                int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                string r = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                bool rb;
                if (bool.TryParse(r, out rb))
                {
                    // result теперь равен true
                }
                else
                {
                    // Ошибка преобразования
                }

                if (rb == true)
                {
                    Form1 form1 = new Form1();
                    form1.Close();

                    Form2 form2 = new Form2();
                    form2.Show();
                }
                else
                {
                    ShowError(labelError, "Неверное имя пользователя или пароль.");
                }
            }
            catch (Exception ex)
            {
                ShowError(labelError, $"Ошибка связи с сервером: {ex.Message}");
            }
        }

        // Регистрация (Кнопка "Зарегистрироваться")
        private void button3_Click_1(object sender, EventArgs e)
        {
            string un = textBox3.Text.Trim();
            string pd = textBox4.Text;
            string cp = textBox5.Text;
            byte[] data0 = Encoding.UTF8.GetBytes("register");
            stream.Write(data0, 0, data0.Length);
            stream.Flush();


            if (string.IsNullOrEmpty(un) || string.IsNullOrEmpty(pd) || string.IsNullOrEmpty(cp))
            {
                ShowError(labelRegError, "Все поля обязательны для заполнения.");
                return;
            }

            if (pd != cp)
            {
                ShowError(labelRegError, "Пароли не совпадают.");
                return;
            }

            if (un.Length < 3 || pd.Length < 6)
            {
                ShowError(labelRegError, "Имя пользователя должно быть длиной >= 3 символов, пароль — >= 6 символов.");
                return;
            }

            if (stream == null || !client.Connected)
            {
                ShowError(labelRegError, "Нет подключения к серверу.");
                return;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(un);
                stream.Write(data, 0, data.Length);
                stream.Flush();

                byte[] data1 = Encoding.UTF8.GetBytes(pd);
                stream.Write(data1, 0, data1.Length);
                stream.Flush();

                byte[] responseBuffer = new byte[256];
                int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                string r = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                bool rb;
                if (bool.TryParse(r, out rb))
                {
                    // result теперь равен true
                }
                else
                {
                    // Ошибка преобразования
                }

                if (rb == true)
                {
                    // Регистрация успешна
                    ResetRegistration();
                    tabControl1.SelectedTab = tabPage1;
                    MessageBox.Show("Вы зарегистрированы успешно! Перейдите на вкладку 'Вход'.", "Успешная регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ShowError(labelRegError, "Данный никнейм уже занят.");
                }
            }
            catch (Exception ex)
            {
                ShowError(labelRegError, $"Ошибка при попытке зарегистрироваться: {ex.Message}");
            }
        }

        // Кнопка "Назад"
        private void button4_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }
    private void textBox2_TextChanged(object sender, EventArgs e)
    {
    }
    private void textBox1_TextChanged(object sender, EventArgs e)
    {
    }
    private void button2_Click_1(object sender, EventArgs e)
    {
            tabControl1.SelectedTab = tabPage2;
        }
    private void textBox3_TextChanged(object sender, EventArgs e)
    {
    }
    private void textBox4_TextChanged(object sender, EventArgs e)
    {
    }
    private void textBox5_TextChanged(object sender, EventArgs e)
    {
    }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        } 
    }
}
       