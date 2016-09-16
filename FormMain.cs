using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Media;

namespace UDPServer
{
    public partial class FormMain : Form
    {
        public Server serverPtr = null;
        delegate void SetTextCallback(string text);
        public int serverIsDead = 1;

        public FormMain()
        {
            InitializeComponent();
            button1.Hide(); // Hide event log button
            IsoletedStorage.InitStorage();
        }

        public void SetText(string text)
        {
            if (this.textBoxUDP.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxUDP.Text += text;
                this.textBoxUDP.AppendText(Environment.NewLine);
                if (checkBox1.Checked)
                {
                    try
                    {
                        string[] messageSounds = { @"C:\visual studio projects\Sounds\burp2_x.wav", //0
                                               @"C:\visual studio projects\Sounds\whip.wav",    //1
                                               @"C:\visual studio projects\Sounds\pacman_dies_y.wav", //2
                                               @"C:\visual studio projects\Sounds\neon_light.wav", //3
                                               @"C:\visual studio projects\Sounds\modem1.wav", //4
                                               @"C:\visual studio projects\Sounds\floop2_x.wav", //5 
                                               @"C:\visual studio projects\Sounds\buzzer_x.wav", //6
                                               @"buzzer_x.wav"
                    };

                        //SystemSounds.Asterisk.Play();
                        //SoundPlayer simpleSound = new SoundPlayer(@"C:\visual studio projects\Sounds\burp2_x.wav");
                        //SoundPlayer simpleSound = new SoundPlayer(messageSounds[7]);
                        SoundPlayer simpleSound = new SoundPlayer(Properties.Resources.buzzer_x);
                        simpleSound.Play();
                    }
                    catch (Exception)
                    {
                        SystemSounds.Asterisk.Play();
                    }
                }


            }
        }

        public void SetStatusText(string text)
        {
            if (this.label4.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.label4.Text = text;


            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverPtr != null)
            {
                serverPtr.Kill = true;
            }
            

            /*
            while (server.UDPThread.IsAlive)
            {
              try
              {
                server.UDPThread.Interrupt();
                if (!server.UDPThread.Join(2000))
                {
                  server.UDPThread.Abort();
                }
              }
              catch (Exception ex)
              {
                MessageBox.Show(ex.Message.ToString());
              }
            }
            */
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxUDP.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.Hide();
            FormStorage fs = new FormStorage();
            fs.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9.]"))
            {
                MessageBox.Show("Please valid IP.");
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter number between 0 to 65535.");
                //textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
                textBox2.Text = textBox2.Text.Remove(0);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox3.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter number between 0 to 65535.");
                //textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
                textBox3.Text = textBox3.Text.Remove(0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (!validateIP())
            {
                return;
            }

            if (!validatePORT(textBox2))
            {
                MessageBox.Show("Invalid port number (number must be between 0 and 65535)");
                return;
            }

            if (!validatePORT(textBox3))
            {
                MessageBox.Show("Invalid port number (number must be between 0 and 65535)");
                return;
            }

            if (serverPtr != null)
            {
                serverPtr.Kill = true;
                Thread.Sleep(10);
                serverPtr.killMe();
                Thread.Sleep(10);
                serverPtr = null;
            }
            
            startUDPServer();
        }

        private bool validateIP()
        {
            byte[] toBytes = Encoding.ASCII.GetBytes(textBox1.Text);
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

            while(ipSegment < 4)
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

        private bool validatePORT(TextBox tb)
        {
            int portNum = Int32.Parse(tb.Text);
            if (portNum < 0 || portNum > 65535)
            {
                return false;
            }
            return true;
        }

        public void startUDPServer()
        {

            Server server = new Server(textBox1.Text, Int32.Parse(textBox2.Text), Int32.Parse(textBox3.Text));
            server.formMain = this;
            this.serverPtr = server;
        }

        
    }
}
