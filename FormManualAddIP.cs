using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UDPServer
{
    public partial class FormManualAddIP : Form
    {
        
        public Server_v2 serverPtr2 = null;

        public FormManualAddIP()
        {
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                if (serverPtr2 == null)
                {
                    MessageBox.Show("You didn't started server");
                    return;
                }
                // Its valid, update Hashtable, update sender IP, and clear texboxes
                HashtableAndDatabaseClass.AddToHashTable(textBox1.Text, textBox2.Text);
                serverPtr2.updateSender(textBox2.Text);
                textBox2.Text = "";
                textBox1.Text = "";
            }
            
        }


        private bool validateIP(TextBox tb)
        {
            byte[] toBytes = Encoding.ASCII.GetBytes(tb.Text);
            // Check if empty field
            if (toBytes.Length == 0)
            {
                MessageBox.Show("You need to input something first");
                return false;
            }
            // Check if minimum length provided (including dots)
            if (toBytes.Length < 7)
            {
                MessageBox.Show("Wrong IP range");
                return false;
            }
            // Check does field contain 3 dots
            int numOfDots = 0;
            for (int i = 0; i < toBytes.Length; i++)
            {
                if (toBytes[i] == '.') numOfDots++;
            }
            if (numOfDots != 3)
            {
                MessageBox.Show("Wrong IP range");
                return false;
            }
            // Check that field does not start with dot or end with dot
            if (toBytes[0] == '.')
            {
                MessageBox.Show("Wrong IP range");
                return false;
            }
            if (toBytes[toBytes.Length - 1] == '.')
            {
                MessageBox.Show("Wrong IP range");
                return false;
            }

            // Consecutive dots check
            int dotFlag = 0;
            for (int i = 0; i < toBytes.Length; i++)
            {
                if (toBytes[i] == '.')
                {
                    if (dotFlag == 1)
                    {
                        MessageBox.Show("Wrong IP range");
                        return false;
                    }
                    dotFlag = 1;
                }
                else
                { // not a dot
                    dotFlag = 0;
                }
            }

            /* So far we know that the IP is correctly formated xxx.xxx.xxx.xxx or xx.x.xxx.x , we need to check for number range */
            int currentPosition = 0;
            int ipSegment = 0; // 4 segments in valid IP number
            //int ipSegmentPosition = 0; // Number in segment

            StringBuilder sb = new StringBuilder();

            while (ipSegment < 4)
            {
                while (toBytes[currentPosition] != '.')
                {
                    sb.Append(Convert.ToChar(toBytes[currentPosition]));
                    currentPosition++;
                    if (currentPosition >= toBytes.Length)
                    {
                        break;
                    }
                }
                if (Int32.Parse(sb.ToString()) > 255)
                {
                    MessageBox.Show("Wrong IP range");
                    return false;
                }
                sb.Clear();
                ipSegment++;
                currentPosition++;
            }

            return true;
        }

        bool ValidateInput()
        {

            if (!validateIP(textBox2))
            {
                return false;
            }
            if (!validateMAC(textBox1))
            {
                return false;
            }
            

            return true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, "[^0-9.]"))
            {
                MessageBox.Show("Please enter valid IP.");
                textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
                textBox2.SelectionStart = textBox2.TextLength;
            }
        }

        private bool validateMAC(TextBox tb)
        {

            //HashtableAndDatabaseClass.AddToHashTable("00:1E:C0:9B:78:E5", "192.168.2.22"); // Manualy added my NTPM

            byte[] toBytes = Encoding.ASCII.GetBytes(tb.Text);
            // Check if empty field
            if (toBytes.Length == 0)
            {
                MessageBox.Show("You need to input something first");
                return false;
            }
            // Check if minimum length provided (including dots)
            if (toBytes.Length != 17)
            {
                MessageBox.Show("Wrong MAC format");
                return false;
            }
            // Check does field contain 3 dots
            int numOfDots = 0;
            for (int i = 0; i < toBytes.Length; i++)
            {
                if (toBytes[i] == ':') numOfDots++;
            }
            if (numOfDots != 5)
            {
                MessageBox.Show("Wrong MAC format");
                return false;
            }
            // Check that field does not start with dot or end with dot
            if (toBytes[0] == ':')
            {
                MessageBox.Show("Wrong MAC format");
                return false;
            }
            if (toBytes[toBytes.Length - 1] == ':')
            {
                MessageBox.Show("Wrong MAC format");
                return false;
            }

            // Consecutive dots check
            int dotFlag = 0;
            for (int i = 0; i < toBytes.Length; i++)
            {
                if (toBytes[i] == ':')
                {
                    if (dotFlag == 1)
                    {
                        MessageBox.Show("Wrong MAC format");
                        return false;
                    }
                    dotFlag = 1;
                }
                else
                { // not a dot
                    dotFlag = 0;
                }
            }

            /* So far we know that the MAC is correctly formated  xx:xx:xx:xx:xx:xx  , we need to check for number range */
            int currentPosition = 0;
            int macSegment = 0; // 6 segments in valid IP number
            //int ipSegmentPosition = 0; // Number in segment

            StringBuilder sb = new StringBuilder();

            while (macSegment < 6)
            {
                while (toBytes[currentPosition] != ':')
                {
                    sb.Append(Convert.ToChar(toBytes[currentPosition]));
                    currentPosition++;
                    if (currentPosition >= toBytes.Length)
                    {
                        break;
                    }
                }
                string hexValue = "0X" + sb.ToString();
                int decValue;
                try
                {
                    decValue = Convert.ToInt32(sb.ToString(), 16);
                }
                catch (Exception)
                {

                    MessageBox.Show("Wrong MAC number");
                    return false;
                }
                
                if (decValue > 255)
                {
                    MessageBox.Show("Wrong MAC range");
                    return false;
                }
                sb.Clear();
                macSegment++;
                currentPosition++;
            }

            return true;


        }


    } // End class FormManualAddIP
}
