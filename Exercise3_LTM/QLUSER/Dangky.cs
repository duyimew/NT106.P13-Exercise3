using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net.Sockets;
using System.Net.Http;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Threading;

namespace QLUSER
{
    public partial class Dangky : Form
    {
        private Dangnhap DN;
        TcpClient tcpClient = new TcpClient();
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        private IPAddress ipAddress;

        public Dangky(Dangnhap dn)
        {
            InitializeComponent();
            DN = dn;
            IPAddress[] IPs = Dns.GetHostAddresses("localhost");
            ipAddress = IPs.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
        private void bt_thoat_Click(object sender, EventArgs e)
        {
            DN.Show();
            this.Close();
        }
        private void bt_DK_Click(object sender, EventArgs e)
        {
            string username = tb_username.Text;
            string password = tb_pwd.Text;
            string confirmPassword = tb_cfpwd.Text;
            string email = tb_email.Text;
            string ten = textBox1.Text;
            string ngaysinh = textBox2.Text;
            ClassQLUSER data = new ClassQLUSER();

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không trùng khớp.");
                return;
            }

            if (!data.IsValidEmail(email))
            {
                MessageBox.Show("Email không đúng định dạng.");
                return;
            }

            string hashpwd = data.HashPassword(password);
            if (hashpwd == null)
            {
                MessageBox.Show("Mã hóa thất bại.");
                return;
            }

            if (ketnoi(username, hashpwd, email,ten,ngaysinh))
            {
                string serverResponse = ketqua();
                if (serverResponse == "1") MessageBox.Show("Đăng ký thành công");
                else MessageBox.Show("Đăng ký thất bại");
            }
            else
            {
                MessageBox.Show("Kết nối đến server thất bại.");
            }
        }

        private bool ketnoi(string username, string hashpwd, string email,string ten, string ngaysinh)
        {
            try
            {
                tcpClient = new TcpClient();
                MessageBox.Show(ipAddress.ToString());
                tcpClient.Connect(ipAddress, 8080);
                MessageBox.Show("Kết nối thành công!");
                NetworkStream ns = tcpClient.GetStream();
                string TEXT = "DK|" + username + "|" + hashpwd + "|" + email + "|" + ten + "|" +ngaysinh;
                byte[] data1 = Encoding.ASCII.GetBytes(TEXT);
                ns.Write(data1, 0, data1.Length);
                ns.Flush();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối đến server"+ ex.Message);
                return false;
            }
        }

        private string ketqua()
        {
            NetworkStream ns = tcpClient.GetStream();
            byte[] data2 = new byte[1024];

            try
            {
                int bytesRead = ns.Read(data2, 0, data2.Length);
                string result = Encoding.ASCII.GetString(data2, 0, bytesRead);
                byte[] quit = Encoding.ASCII.GetBytes("quit");
                ns.Write(quit, 0, quit.Length);
                ns.Close();
                tcpClient.Close();
                return result.Trim();
            }
            catch (Exception)
            {
                return "Lỗi nhận dữ liệu từ server";
            }
        }


    }
}




