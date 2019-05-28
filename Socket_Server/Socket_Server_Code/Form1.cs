using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Socket_Server_Code
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            //Control.CheckForIllegalCrossThreadCalls = false;            
        }
        
        void Send(Socket socket,string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            socket.Send(buffer);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (ClientConnectionItems.ContainsKey(comboBox1.SelectedItem.ToString()))
            {
                Send(ClientConnectionItems[comboBox1.SelectedItem.ToString()], richTextBox1.Text);                
                richTextBox1.Text = "";
            }
            else
            {
                listBox1.Items.Add("不包含此客户端");
            }
            
        }
        void ShowMsg(string msg)
        {
            listBox1.Items.Add(msg + "\r\n");
        }

        static Socket socketSend = null;
        static Dictionary<string, Socket> ClientConnectionItems = new Dictionary<string, Socket> ();
        static string Cid ;
        void Listen(Object o)
        {
            try
            {
                Socket socketWatch = o as Socket;
                while (true)
                {
                    socketWatch = socketSend.Accept();//等待客户端连接
                    byte[] inderc = new byte[1024 * 1024];
                    int ll = socketWatch.Receive(inderc);
                    Cid = Encoding.UTF8.GetString(inderc,0,ll);
                    ClientConnectionItems.Add(Cid, socketWatch);
                    ShowMsg(socketWatch.RemoteEndPoint.ToString() + ":" + "连接成功,当前连接数：" + ClientConnectionItems.Count);
                    comboBox1.Items.Add(Cid);
                    comboBox1.SelectedItem = Cid;                    
                    //开启一个新线程，执行接受消息方法
                    Thread r_thread = new Thread(Received);
                    r_thread.IsBackground = true;
                    r_thread.Start(socketWatch);
                }
            }
            catch (Exception e)
            {
                ShowMsg("服务器异常:" + e.Message + "异常方法:" + e.TargetSite);
                throw;
            }
        }
        void Received(object o)
        {
            try
            {
                Socket SendSocket = o as Socket;
                while (true)
                {
                    //客户端连接服务器成功后，服务器接受客户端发送的数据
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    //实际接受到的数据
                    int len = SendSocket.Receive(buffer);
                    if (len == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, len);
                    ShowMsg(SendSocket.RemoteEndPoint + ":" + str);
                }
            }
            catch (Exception e)
            {
                ShowMsg("服务器异常:"+e.Message +"异常方法:"+e.TargetSite);
                throw;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                //创建监听接口
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress iP = IPAddress.Parse(textBox1.Text);
                
                //创建对象端口
                IPEndPoint point = new IPEndPoint(iP, Convert.ToInt32(textBox2.Text));                
                socketSend.Bind(point);//绑定端口号
                ShowMsg("监听成功");
                socketSend.Listen(10);  //设置监听                
                //创建监听线程
                Thread thread = new Thread(Listen);
                thread.IsBackground = true;
                thread.Start(socketSend);
                button2.Enabled = false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
