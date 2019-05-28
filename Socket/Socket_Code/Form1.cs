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
using System.Net;
using System.Threading;

namespace Socket_Code
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static byte[] result = new byte[1024];
        IPAddress ip = null;
        Socket clientSocket = null;
        void MsgShow(string str)
        {
            listBox1.Items.Add(str + "\r\n");
        }

        void Received()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];

                    int len = clientSocket.Receive(buffer);
                    if (len == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, len);
                    MsgShow(clientSocket.RemoteEndPoint + ":" + str);
                }
                catch (Exception e)
                {
                    MsgShow("客户端异常：" + e.Message + "异常方法:" + e.TargetSite);
                    throw;
                }
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            ip = IPAddress.Parse(textBox1.Text);
            
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, Convert.ToInt32(textBox2.Text)));                

                byte[] sendMesg = Encoding.UTF8.GetBytes(textBox3.Text);
                clientSocket.Send(sendMesg);

                MsgShow("服务器连接成功！");

                Thread c_thread = new Thread(Received);
                c_thread.IsBackground = true;
                c_thread.Start();

                button2.Visible = true;
                button1.Enabled = false;
            }
            catch (Exception ex)
            {
                MsgShow("连接失败！:" + ex.Message);
                throw;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = richTextBox1.Text.Trim();
                byte[] buffer = new byte[1024 * 1024 * 3];
                buffer = Encoding.UTF8.GetBytes(msg);
                clientSocket.Send(buffer);
                richTextBox1.Text = "";
            }
            catch (Exception ex)
            {
                MsgShow("发送失败！：" + ex.Message);
                throw;
            }
        }
    }
}
