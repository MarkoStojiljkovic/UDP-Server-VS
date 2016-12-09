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
using System.Globalization;

namespace UDPServer
{
    public partial class FormMain : Form
    {
        public Server serverPtr = null;
        public Server_v2 serverPtr2 = null;
        delegate void SetTextCallback(string text);
        delegate void ChangeButtonStateCallback(bool status);
        public int serverIsDead = 1;
        string oracleDB = "DATA SOURCE=192.168.2.98:1521/M2CDatabase;PERSIST SECURITY INFO=True;USER ID=NTPM;PASSWORD=NeticoPassword123;";

        public FormMain()
        {
            InitializeComponent();
            button3.Enabled = false; // Disable button while program is trying to get data from database...

            UpdateDatabase();
            
            //HashtableAndDatabaseClass.AddToHashTable("00:1E:C0:9B:78:E5", "192.168.2.22"); // Manualy added my NTPM
            

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

        public void SetStatusText(string text) // Change text to label with any class
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

        public void SetDBStatusText(string text) // Change text to label with any class
        {
            if (this.labelDB.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetDBStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.labelDB.Text = text;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverPtr != null)
            {
                serverPtr.Kill = true;
            }
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
            //FormStorage fs = new FormStorage();
            //fs.Show();
            //Storage.StorageInit();
        }

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
        //    if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9.]"))
        //    {
        //        MessageBox.Show("Please enter valid IP.");
        //        textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
        //        textBox1.SelectionStart = textBox1.TextLength;
        //    }
        //}

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

            if (!ValidateInput()) // Check IP and port number formatting
            {
                return;
            }

            button2.Enabled = false;

            if (Server_v2.isActive)
            {
                Server_v2.Kill = true;
                Console.WriteLine("Phase Killing server \n");
                Thread.Sleep(100);
                //button2.Enabled = true;
                //return;
            }

            try
            {
                startUDPServer2();
            }
            catch (Exception)
            {

                SetStatusText("Status: ERROR");
                button3.Enabled = false;
            }

            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e) // Commands button
        {
            CommandsForm cf = new CommandsForm();
            cf.serverPtr2 = serverPtr2;
            cf.Show();
        }


        //private bool validateIP()
        //{
        //    byte[] toBytes = Encoding.ASCII.GetBytes(textBox1.Text);
        //    // Check if empty field
        //    if (toBytes.Length == 0) 
        //    {
        //        MessageBox.Show("You need to input something first");
        //        return false;
        //    }
        //    // Check if minimum length provided (including dots)
        //    if (toBytes.Length < 7)
        //    {
        //        MessageBox.Show("Wrong IP range");
        //        return false;
        //    }
        //    // Check does field contain 3 dots
        //    int numOfDots = 0;
        //    for (int i = 0; i < toBytes.Length; i++)
        //    {
        //        if (toBytes[i] == '.') numOfDots++;
        //    }
        //    if (numOfDots != 3)
        //    {
        //        MessageBox.Show("Wrong IP range");
        //        return false;
        //    }
        //    // Check that field does not start with dot or end with dot
        //    if (toBytes[0] == '.')
        //    {
        //        MessageBox.Show("Wrong IP range");
        //        return false;
        //    }
        //    if (toBytes[toBytes.Length - 1] == '.')
        //    {
        //        MessageBox.Show("Wrong IP range");
        //        return false;
        //    }

        //    // Consecutive dots check
        //    int dotFlag = 0;
        //    for (int i = 0; i < toBytes.Length; i++)
        //    {
        //        if (toBytes[i] == '.')
        //        {
        //            if (dotFlag == 1)
        //            {
        //                MessageBox.Show("Wrong IP range");
        //                return false;
        //            }
        //            dotFlag = 1;
        //        }
        //        else
        //        { // not a dot
        //            dotFlag = 0;
        //        }
        //    }

        //    /* So far we know that the IP is correctly formated xxx.xxx.xxx.xxx or xx.x.xxx.x , we need to check for number range */
        //    int currentPosition = 0;
        //    int ipSegment = 0; // 4 segments in valid IP number
        //    //int ipSegmentPosition = 0; // Number in segment

        //    StringBuilder sb = new StringBuilder();

        //    while(ipSegment < 4)
        //     {
        //        while (toBytes[currentPosition] != '.')
        //        {
        //            sb.Append(Convert.ToChar(toBytes[currentPosition]));
        //            currentPosition++;
        //            if (currentPosition >= toBytes.Length)
        //            {
        //                break;
        //            }
        //        }
        //        if (Int32.Parse(sb.ToString()) > 255)
        //        {
        //            MessageBox.Show("Wrong IP range");
        //            return false;
        //        }
        //        sb.Clear();
        //        ipSegment++;
        //        currentPosition++;
        //    }

        //    return true;
        //}

        private bool validatePORT(TextBox tb)
        {
            if (tb.Text == "")
            {
                return false;
            }
            int portNum = Int32.Parse(tb.Text);
            if (portNum < 0 || portNum > 65535)
            {
                return false;
            }
            return true;
        }

        //public void startUDPServer()
        //{

        //    Server server = new Server(textBox1.Text, Int32.Parse(textBox2.Text), Int32.Parse(textBox3.Text));
        //    server.formMain = this;
        //    this.serverPtr = server;
        //}

        public void startUDPServer2()
        {
            Server_v2 server = new Server_v2(Int32.Parse(textBox2.Text), Int32.Parse(textBox3.Text));
            Server_v2.formMain = this;
            this.serverPtr2 = server;
        }

        bool ValidateInput()
        {
            // Not used anymore, now it gets IP automaticly 
            //if (!validateIP())
            //{
            //    return false;
            //}

            if (!validatePORT(textBox2))
            {
                MessageBox.Show("Invalid port number (number must be between 0 and 65535)");
                return false;
            }

            if (!validatePORT(textBox3))
            {
                MessageBox.Show("Invalid port number (number must be between 0 and 65535)");
                return false;
            }
            return true;
        }

        public void ChangeButtonState(bool state)
        {
            if (this.button3.InvokeRequired)
            {
                ChangeButtonStateCallback d = new ChangeButtonStateCallback(ChangeButtonState);
                this.Invoke(d, new object[] { state });
            }
            else
            {
                this.button3.Enabled = state;
            }
        }

        private void updateDB_Click(object sender, EventArgs e)
        {
            updateDBChangeEnabledState("false"); // Update button enable status
            UpdateDatabase();
        }

        private void UpdateDatabase() 
        {
            Thread t = new Thread(() => // I want it to be async
            {

                try
                {
                    HashtableAndDatabaseClass.FetchIPFromDatabase(oracleDB);
                    DateTime localDate = DateTime.Now;
                    var culture = new CultureInfo("en-GB");
                    string formatedTime = localDate.ToString(culture);
                    SetDBStatusText("DB status: Last time updated > " + formatedTime);

                }
                catch (Exception)
                {
                    Console.WriteLine("Retrieve from database failed");
                }
                finally
                {
                    updateDBChangeEnabledState("true"); // Update button enable status
                }
            }
            ); // End lambda expression
             
            t.Start();



        }

        public void updateDBChangeEnabledState(string status)
        {
            if (this.updateDB.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(updateDBChangeEnabledState); // This delegatge is not mentioned for this, but it can be used for this too
                this.Invoke(d, new object[] { status });
            }
            else
            {
                if (status.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    this.updateDB.Enabled = true;
                }
                else if (status.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    this.updateDB.Enabled = false;
                }
            }
        }

        private void buttonAddIP_Click(object sender, EventArgs e)
        {
            FormManualAddIP f = new FormManualAddIP();
            f.serverPtr2 = serverPtr2;
            f.Show();
        }
    }
}
