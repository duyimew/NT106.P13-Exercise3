using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace QLUSER
{
    public partial class Dangnhap : Form
    {
        TcpClient tcpClient = new TcpClient();
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        private IPAddress ipAddress;
        public Dangnhap()
        {
            InitializeComponent();
            IPAddress[] IPs = Dns.GetHostAddresses("LAPTOP-GIAHIEU");
            ipAddress = IPs.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
        private void bt_DK_Click(object sender, EventArgs e)
        {
            Dangky dk = new Dangky(this);
            dk.Show();
            this.Hide();
        }
        private void bt_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void bt_DN_Click(object sender, EventArgs e)
        {
            string username = tb_username.Text;
            string password = tb_pwd.Text;
            ClassQLUSER data = new ClassQLUSER();
            string hashpwd = data.HashPassword(password);
            if (hashpwd == null)
            {
                MessageBox.Show("Mã hóa thất bại");
                return;
            }
            if (ketnoi(username, hashpwd))
            {
                string[] serverResponse = ketqua();
                if (serverResponse[0] == "1")
                {
                    string token = serverResponse[1];
                    string path = data.GetProjectRootDirectory();
                    FileStream fs = new FileStream(path +"\\token.txt", FileMode.Create);
                    fs.Close();
                    string filepath = data.find("token.txt");
                    File.WriteAllText(filepath, username+"|"+ token);
                    MessageBox.Show("Đăng nhập thành công");
                    Formuser mainForm = new Formuser(username,token, this);
                    mainForm.Show();
                    this.Hide();
                }
                else MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng");
            }
            else
            {
                MessageBox.Show("Kết nối đến server thất bại.");
            }

        }
        private bool ketnoi(string username, string hashpwd)
        {
          try
            {
                tcpClient = new TcpClient();
                MessageBox.Show(ipAddress.ToString());
                tcpClient.Connect(ipAddress, 8080);
                MessageBox.Show("Kết nối thành công!");
                NetworkStream ns = tcpClient.GetStream();
                string TEXT = "DN|" + username + "|" + hashpwd;
                byte[] data1 = Encoding.ASCII.GetBytes(TEXT);
                ns.Write(data1, 0, data1.Length);
                ns.Flush();
                return true;
            }
          catch (Exception ex)
            {
             MessageBox.Show("Lỗi kết nối đến server"+ex.Message);
             return false;
            }
        }

        private string[] ketqua()
        {
            NetworkStream ns = tcpClient.GetStream();
            byte[] data2 = new byte[1024];

            try
            {
                int bytesRead = ns.Read(data2, 0, data2.Length);
                string text = Encoding.ASCII.GetString(data2, 0, bytesRead);
                string[] result = text.Split('|');
                byte[] quit = Encoding.ASCII.GetBytes("quit");
                ns.Write(quit, 0, quit.Length);
                ns.Close();
                tcpClient.Close();
                return result;
            }
            catch (Exception)
            {
                return new string[] { "Lỗi nhận dữ liệu từ server" };
            }
        }
    }
}
