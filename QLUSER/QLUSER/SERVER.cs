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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace QLUSER
{
    public partial class SERVER : Form
    {
        private Thread serverThread;
        
        public SERVER()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            serverThread = new Thread(new ThreadStart(StartUnsafeThread));
            serverThread.SetApartmentState(ApartmentState.STA);
            serverThread.Start();
        }
        private void StartUnsafeThread()
        {
            try
            {   
                    Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint ipepServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
                    listenerSocket.Bind(ipepServer);
                    listenerSocket.Listen(-1);
                    MessageBox.Show("Server đang chạy và lắng nghe tại 127.0.0.1:8080");
                    while (true)
                    {
                        Socket clientSocket = listenerSocket.Accept(); 
                        this.Invoke((MethodInvoker)delegate { listView1.Items.Add(new ListViewItem("New client connected"));  });
                
                        Thread clientThread = new Thread(() => HandleClient(clientSocket));
                        clientThread.Start();
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi chạy server"+ex.Message);
            }

        }

        private void HandleClient(Socket clientSocket)
        {
            try
            {
                int bytesReceived;
                byte[] recv = new byte[1024];
                string text = "";

                while (clientSocket.Connected)
                {
                    bytesReceived = clientSocket.Receive(recv);
                    if (bytesReceived == 0) break;

                    text = Encoding.ASCII.GetString(recv, 0, bytesReceived);

                    if (text.Contains("DK"))
                    {
                        string[] str = text.Split('|');

                        int result = Dangky(str);
                        byte[] send = Encoding.ASCII.GetBytes(result.ToString());
                        clientSocket.Send(send);
                    }
                    else if (text == "quit\n")
                    {
                        break;
                    }
                    if(text.Contains("DN"))
                    {
                    string[] str = text.Split('|');
                    string[] result = Dangnhap(str);
                    string data = result[0] +"|"+ result[1]+"|" + result[2] +"|"+ result[3];
                   
                        byte[] send = Encoding.ASCII.GetBytes(data);
                    clientSocket.Send(send);
                    }
                }
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            } 
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi kết nối với client"+ex.Message);
            }
        }

        public int Dangky(string[] str)
        {
            ClassQLUSER data = new ClassQLUSER();
            SqlConnection connectionDB = data.ConnectToDatabase();
            if (connectionDB == null)
            {
                return 0;
            }
                string checkUsername = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                SqlCommand checkUsernameCmd = new SqlCommand(checkUsername, connectionDB);
                checkUsernameCmd.Parameters.AddWithValue("@username", str[1]);
                int user = (int)checkUsernameCmd.ExecuteScalar();
                if (user > 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại.");
                    return 0;
                }
                string CommandText = "SET DATEFORMAT dmy";
                SqlCommand CommandCmd = new SqlCommand(CommandText, connectionDB);
                CommandCmd.ExecuteNonQuery();
                string insert = "INSERT INTO Users (Username, Password, Email, FullName, Birthday) VALUES (@username, @password, @Email, @ten, @ngaysinh)";
                SqlCommand insertCmd = new SqlCommand(insert, connectionDB);
                insertCmd.Parameters.AddWithValue("@username", str[1]);
                insertCmd.Parameters.AddWithValue("@password", str[2]);
                insertCmd.Parameters.AddWithValue("@Email", str[3]);
                insertCmd.Parameters.AddWithValue("@ten", str[4]);
                insertCmd.Parameters.AddWithValue("@ngaysinh", str[5]);
                insertCmd.ExecuteNonQuery();
                return 1;
        }
        public string[] Dangnhap(string[] str)
        {
            string[] result =new string[4];
            result[0] = "0"; result[1] = "";
            result[2] = ""; result[3] = "";

            ClassQLUSER data = new ClassQLUSER();
            SqlConnection connectionDB = data.ConnectToDatabase();
            if (connectionDB == null)
            {
                return result;
            }
            string login = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
            SqlCommand loginCmd = new SqlCommand(login, connectionDB);
            loginCmd.Parameters.AddWithValue("@username", str[1]);
            loginCmd.Parameters.AddWithValue("@password", str[2]);
            int count = (int)loginCmd.ExecuteScalar();
            if (count == 1)
            {
                result[0] = "1";
                string strQuery = "SELECT Email, FullName, Birthday FROM Users WHERE Username = @username AND Password = @password";
                SqlCommand command = new SqlCommand(strQuery, connectionDB);
                command.Parameters.AddWithValue("@username", str[1]);
                command.Parameters.AddWithValue("@password", str[2]);
                DataTable dataTable = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    result[1] = row["Email"].ToString();
                    result[2] = row["FullName"].ToString();
                    DateTime birthday = DateTime.Parse(row["Birthday"].ToString());
                    result[3] = birthday.ToString("dd/MM/yyyy");
                    
                    
                }
                return result;
                                
            }
            else
            {
                
                return result;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            this.Close(); 
            
        }
    }
}
