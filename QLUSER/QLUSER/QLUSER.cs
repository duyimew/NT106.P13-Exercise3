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

namespace QLUSER
{
    public partial class QLUSER : Form
    {
       
        public QLUSER()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SERVER SERVER = new SERVER();
            SERVER.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dangnhap dangnhap = new Dangnhap();
            dangnhap.Show();
        }
    }
}
