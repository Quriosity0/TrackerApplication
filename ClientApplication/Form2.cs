using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace projecttrecer_client
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process[] procs = Process.GetProcesses();

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
