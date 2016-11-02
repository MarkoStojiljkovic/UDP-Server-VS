using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPServer
{
    public partial class CommandsForm : Form
    {
        public Server_v2 serverPtr2 = null;

        public CommandsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) // Send stop command
        {
            serverPtr2.SendCommand("STOP");
        }

        private void button2_Click(object sender, EventArgs e) // Send discard command
        {
            serverPtr2.SendCommand("DSCD");
        }

        private void button3_Click(object sender, EventArgs e) // Send block
        {
            serverPtr2.SendCommand("BLK1");
        }

        private void button4_Click(object sender, EventArgs e) // Send unblock
        {
            serverPtr2.SendCommand("BLK0");
        }

        private void button5_Click(object sender, EventArgs e) // Send ACK
        {
            int packetNum;
            if (!Int32.TryParse(textBox1.Text, out packetNum))
            {
                MessageBox.Show("Field empty!");
                return;
            }

            if (packetNum > 255)
            {
                MessageBox.Show("Only numbers between 0 and 255 are valid");
                textBox1.Text = textBox1.Text.Remove(0);
                return;
            }
            


            
            string ack = "ACK" + " " +"p" + textBox1.Text;
            serverPtr2.SendCommand(ack);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9.]"))
            {
                MessageBox.Show("Only numbers between 0 and 255 are valid");
                textBox1.Text = textBox1.Text.Remove(0);
            }
        }

        private void button6_Click(object sender, EventArgs e) // Send Get Status
        {
            serverPtr2.SendCommand("STAT");
        }
    }
}
