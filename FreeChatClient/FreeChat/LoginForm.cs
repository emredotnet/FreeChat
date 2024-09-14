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
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Newtonsoft.Json.Linq;

namespace FreeChat
{
    public partial class LoginForm : Form
    {
        public NetworkStream stream;
        public static NetworkStream strm;
        bool rORl = true;
        bool formclosed = false;
        bool login = false;
        private Dictionary<int, string> receivedParts = new Dictionary<int, string>();
        
        public LoginForm(NetworkStream networkStream)
        {
            InitializeComponent();
            stream = networkStream;
            strm = stream;

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();
        }

        void OpenServer(Dictionary<string, string> msg)
        {
            formclosed = true;
            ServerPageForm ServerPage = new ServerPageForm(strm);
            ServerPage.infos = msg;
            ServerPage.Show();
            this.Close();
        }

        void Send(string jmsg)
        {
            byte[] data = Encoding.UTF8.GetBytes(jmsg);
            stream.Write(data, 0, data.Length);
        }

        void ProcessFullMessage(JObject fullMessage)
        {
            JObject messageObject = fullMessage;
            string message = messageObject["Type"].ToString();
            JArray messagesArray = (JArray)messageObject["Messages"];

            if (message.StartsWith("[LGN1]"))
            {
                Dictionary<string, string> infos = new Dictionary<string, string>{

                        { "Username", messageObject["Username"].ToString() },
                        { "Rooms", messageObject["Rooms"].ToString() }};


                login = true;
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => OpenServer(infos)));
                }
                else
                {
                    OpenServer(infos);
                }
            }
            else if (message.StartsWith("[LGN2]") || message.StartsWith("[LGN3]") || message.StartsWith("[REG]"))
            {
                string msg = messageObject["Message"].ToString();
                MessageBox.Show(msg);
            }
            else if (message.StartsWith("[ANN]"))
            {
                string msg = messageObject["Message"].ToString();
                UpdatePanel(msg);
            }
        }

        void ReceiveMessages()
        {
            byte[] buffer = new byte[65536];
            int receivedBytes;

            while (!formclosed)
            {
                try
                {
                    receivedBytes = strm.Read(buffer, 0, buffer.Length);
                    if (receivedBytes == 0) break;

                    string text = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    JObject jobj = JObject.Parse(text);
                    ProcessFullMessage(jobj);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            
            if (!login)
            {
                MessageBox.Show("Server disconnected!!!");
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(CloseForm));
                }
                else
                {
                    CloseForm();
                }
            }
        }

        private void UpdatePanel(string parts)
        {
            if (panel1.InvokeRequired)
            {
                panel1.Invoke(new Action(() => UpdatePanel(parts)));
            }
            else
            {
                panel1.Controls.Clear();
                RichTextBox lastTextBox = null;
                string[] partArray = parts.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < partArray.Length; i++)
                {
                    RichTextBox textBox = new RichTextBox();
                    textBox.Text = partArray[i].Trim();
                    textBox.ReadOnly = true;
                    textBox.BackColor = panel1.BackColor;
                    textBox.BorderStyle = BorderStyle.None;
                    textBox.AutoSize = true;
                    textBox.Cursor = Cursors.Hand;
                    textBox.Font = panel1.Font;

                    Size textSize = TextRenderer.MeasureText(partArray[i], textBox.Font);
                    textBox.Width = textSize.Width + 10;
                    textBox.Height = textSize.Height; 

                    if (i == 0)
                    {
                        textBox.Location = new Point(10, 10);
                    }
                    else
                    {
                        textBox.Location = new Point(10, lastTextBox.Location.Y + lastTextBox.Height + 5);
                    }

                    panel1.Controls.Add(textBox);
                    lastTextBox = textBox;
                }
            }
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            if (rORl)
            {
                if (!string.IsNullOrEmpty(usernametxt.Text) && !string.IsNullOrEmpty(passtxt.Text))
                {

                    var login = new
                    {
                        Type = "[LOGIN]",
                        Username = usernametxt.Text,
                        Password = passtxt.Text,
                    };

                    string jmsg = JsonConvert.SerializeObject(login);
                    Send(jmsg);
                }
                else MessageBox.Show("Invalid Username or Password!");
            }

            else
            {
                if (!string.IsNullOrEmpty(usernametxt.Text) && !string.IsNullOrEmpty(passtxt.Text) && !string.IsNullOrEmpty(emailtxt.Text))
                {
                    var reg = new
                    {
                        Type = "[REGISTER]",
                        Username = usernametxt.Text,
                        Password = passtxt.Text,
                        Email = emailtxt.Text,
                    };

                    string jmsg = JsonConvert.SerializeObject(reg);
                    Send(jmsg);
                }
                else MessageBox.Show("Invalid Username/Password or Email!");
            }

        }

        private void regbtn_Click(object sender, EventArgs e)
        {
            if (rORl)
            {
                rORl = false;
                btn1.Text = "Register";
                emailtxt.Visible = true;
                lblemail.Visible = true;
            }
            else
            {
                rORl = true;
                btn1.Text = "Login";
                emailtxt.Text = " ";
                emailtxt.Visible = false;
                lblemail.Visible = false;
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            passtxt.PasswordChar = '*';
            passtxt.MaxLength = 20;
            usernametxt.MaxLength = 20;
            emailtxt.MaxLength = 100;
        }

        void CloseForm()
        {
            ConnectForm cform = (ConnectForm)Application.OpenForms["ConnectForm"];
            if (cform != null) { cform.Show(); }
            this.Close();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (login == false) 
            {
                formclosed = true;
                ConnectForm cform = (ConnectForm)Application.OpenForms["ConnectForm"];
                if (cform != null) { cform.Show(); }
            }
            
        }
    }
}
