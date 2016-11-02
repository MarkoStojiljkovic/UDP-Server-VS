using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPServer
{
    public partial class FormStorage : Form
    {
        public FormStorage()
        {
            InitializeComponent();
            //textBox1.Text = IsoletedStorage.ReadFromStorage();
        }

        private void FormStorage_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FormMain nf = new FormMain();
            //nf.Show();
            this.Hide();

        }

        private void FormStorage_Enter(object sender, EventArgs e)
        {

        }
    }
}
