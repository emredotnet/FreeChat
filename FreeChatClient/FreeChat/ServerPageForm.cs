using Microsoft.VisualBasic.Devices;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskBand;
using System.Net.Sockets;
using Microsoft.VisualBasic.Logging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics.Eventing.Reader;

namespace FreeChat
{
    public partial class ServerPageForm : Form
    {
        private readonly object receiveLock = new object();
        public Dictionary<string, string> infos { get; set; }
        private bool isReceiving = false;
        public string username;
        private NetworkStream stream;
        int inroom;
        RichTextBox lasttxt = null;
        Label lastonline = null;
        private List<System.Windows.Forms.Button> roomButtons = new List<System.Windows.Forms.Button>();
        private Dictionary<int, string> receivedParts = new Dictionary<int, string>();
        static bool disconnect = false;
        static bool appclose = false;

        public ServerPageForm(NetworkStream networkStream)
        {
            InitializeComponent();
            stream = networkStream;

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

        }

        private void ServerPageForm_Load(object sender, EventArgs e)
        {
            chattxt.MaxLength = 150;
            System.Windows.Forms.Button lastbutton = null;

            username = infos["Username"];
            lblname.Text = username;

            if (infos.ContainsKey("Rooms"))
            {
                var rooms = JsonConvert.DeserializeObject<List<dynamic>>(infos["Rooms"]);

                foreach (var room in rooms)
                {
                    int roomId = room.RoomID;
                    string roomName = room.RoomName;

                    System.Windows.Forms.Button rmbutton = new System.Windows.Forms.Button();
                    rmbutton.Name = "Room_" + roomId.ToString();
                    rmbutton.Text = roomName;
                    rmbutton.Size = new System.Drawing.Size(100, 27);

                    if (lastbutton == null)
                    {
                        rmbutton.Location = new Point(5, 10);
                    }
                    else
                    {
                        rmbutton.Location = new Point(5, lastbutton.Location.Y + lastbutton.Height + 5);
                    }

                    rmbutton.Click += (s, e) => RoomButton_Click(s, e, roomId);
                    roompanel.Controls.Add(rmbutton);
                    roomButtons.Add(rmbutton);
                    lastbutton = rmbutton;

                    if (roomId == 1) 
                    {
                        REQ(roomId);
                        rmbutton.Enabled = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("Room information not found.");
            }
        }

        private async void RoomButton_Click(object sender, EventArgs e, int roomId)
        {
            ClearChatPanel();
            REQ(roomId);
            System.Windows.Forms.Button clickedButton = sender as System.Windows.Forms.Button;

            if (clickedButton != null)
            {
                foreach (System.Windows.Forms.Button button in roomButtons)
                {
                    button.Enabled = false;
                }

                //await Task.Delay(500);

                foreach (System.Windows.Forms.Button button in roomButtons)
                {
                    if (button != clickedButton)
                    {
                        button.Enabled = true;
                    }
                }
            }
        }

        void REQ(int rmID)
        {
            inroom = rmID;
            var data = new { Type = "[REQ]", RoomID = $"{rmID}" };
            string jdata = JsonConvert.SerializeObject(data);
            Send(jdata);
        }

        private void ReceiveMessages()
        {

            byte[] buffer = new byte[1024 * 5100];
            int receivedBytes;

            while (true)
            {
                if (appclose)
                {
                    break;
                }

                try
                {
                    receivedBytes = stream.Read(buffer, 0, buffer.Length);
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
            if (!appclose)
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

        void ProcessFullMessage(JObject fullMessage)
        {
            JObject messageObject = fullMessage;
            string message = messageObject["Type"].ToString();
            JArray messagesArray = (JArray)messageObject["Messages"];

            if (message == "[ROOM]")
            {
                for (int i = 0; i < messagesArray.Count; i++)
                {
                    int id = Convert.ToInt32(messagesArray[i]["RoomID"]);
                    if (id == inroom) 
                    {

                        string username = messagesArray[i]["Username"].ToString();
                        string msgContent = messagesArray[i]["Content"].ToString();
                        string image = messagesArray[i]["Image"].ToString();
                        string date = messagesArray[i]["Date"].ToString();

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                                AddMsgPanel(username, msgContent, image, date)
                            ));
                        }
                        else
                        {
                            AddMsgPanel(username, msgContent, image, date);
                        }
                    }
                }

            }
            else if (message.StartsWith("[MSG]"))
            {
                for (int i = 0; i < messagesArray.Count; i++)
                {
                    int id = Convert.ToInt32(messagesArray[i]["RoomID"]);
                    if (id == inroom)
                    {
                        string username = messagesArray[i]["Username"].ToString();
                        string msgContent = messagesArray[i]["Content"].ToString();
                        string image = messagesArray[i]["Image"].ToString();
                        string date = messagesArray[i]["Date"].ToString();

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                                AddMsgPanel(username, msgContent, image, date)
                            ));
                        }
                        else
                        {
                            AddMsgPanel(username, msgContent, image, date);
                        }
                    }
                }

            }
            else if (message.StartsWith("[ONLINE]"))
            {
                ClearOnlinePanel();
                JArray onlineListArray = (JArray)messageObject["OnlinesList"];
                for (int i = 0; i < onlineListArray.Count; i++)
                {
                    string onlineUser = onlineListArray[i].ToString();
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                           AddOnlinePanel(onlineUser)
                        ));
                    }
                    else
                    {
                        AddOnlinePanel(onlineUser);
                    }
                }

            }
        }
        void AddOnlinePanel(string online)
        {
            Label txt = new Label();
            txt.Text = online;
            txt.AutoSize = true;
            txt.Font = onlinepanel.Font;

            if (lastonline == null)
            {
                txt.Location = new Point(10,10);
            }
            else
            {
                txt.Location = new Point(10, lastonline.Location.Y + lastonline.Height + 1);
            }
            onlinepanel.Controls.Add(txt);
            lastonline = txt;
        }

        void ClearOnlinePanel()
        {
            if (chatpanel != null && chatpanel.Controls.Count > 0)
            {
                onlinepanel.Controls.Clear();
                lastonline = null;
            }
        }

        void Send(string cmsg)
        {
            byte[] data = Encoding.UTF8.GetBytes(cmsg);
            stream.Write(data, 0, data.Length);
        }

        void ClearChatPanel()
        {
            if (chatpanel != null && chatpanel.Controls.Count > 0)
            {
                chatpanel.Controls.Clear();
                lasttxt = null;
            }
        }

        void CloseForm()
        {
            disconnect = true;
            ConnectForm cform = (ConnectForm)System.Windows.Forms.Application.OpenForms["ConnectForm"];
            if (cform != null) { cform.Show(); }
            this.Close();
        }

        void AddMsgPanel(string s, string m , string i , string t)
        {
            string message = m;
            string image = i;
            string time = t;
            string sender = s;

            string txttime = DateTime.Parse(time).ToString("HH:mm");

            if (!string.IsNullOrEmpty(message) && message != "NULL" && (string.IsNullOrEmpty(image) || image == "NULL")) // Mesaj null veya boş değilse
            {

                RichTextBox sendertxt = new RichTextBox();
                sendertxt.Text = $"{sender} :";
                sendertxt.AutoSize = true;
                sendertxt.ReadOnly = true;
                sendertxt.BackColor = chatpanel.BackColor;
                sendertxt.BorderStyle = BorderStyle.None;
                sendertxt.Cursor = Cursors.Hand;
                sendertxt.Font = new System.Drawing.Font("Segoe UI Semibold", 12, FontStyle.Bold | FontStyle.Italic);

                Size sendertextSize = TextRenderer.MeasureText(sendertxt.Text, sendertxt.Font);
                sendertxt.Width = sendertextSize.Width + 10;
                sendertxt.Height = sendertextSize.Height;

                if (lasttxt == null)
                {
                    sendertxt.Location = new Point(70, 10);
                }
                else
                {
                    sendertxt.Location = new Point(70, lasttxt.Location.Y + lasttxt.Height + 1);
                }

                chatpanel.Controls.Add(sendertxt);

                RichTextBox txt = new RichTextBox();
                txt.Text = $"{message}";
                txt.AutoSize = true;
                txt.ReadOnly = true;
                txt.BackColor = chatpanel.BackColor;
                txt.BorderStyle = BorderStyle.None;
                txt.Cursor = Cursors.Hand;
                txt.Font = chatpanel.Font;

                Size textSize = TextRenderer.MeasureText(txt.Text, txt.Font);
                txt.Width = textSize.Width + 10;
                txt.Height = textSize.Height;

                if (lasttxt == null)
                {
                    txt.Location = new Point(sendertxt.Location.X + sendertxt.Width, 10);
                }
                else
                {
                    txt.Location = new Point(sendertxt.Location.X + sendertxt.Width, lasttxt.Location.Y + lasttxt.Height + 1);
                }

                RichTextBox timetxt = new RichTextBox();
                timetxt.Text = $"{txttime}";
                timetxt.AutoSize = true;
                timetxt.ReadOnly = true;
                timetxt.BackColor = chatpanel.BackColor;
                timetxt.BorderStyle = BorderStyle.None;
                timetxt.Cursor = Cursors.Hand;
                timetxt.Font = new System.Drawing.Font("Segoe UI Semibold", 12, FontStyle.Bold);

                Size timetextSize = TextRenderer.MeasureText(timetxt.Text, timetxt.Font);
                timetxt.Width = timetextSize.Width + 10;
                timetxt.Height = timetextSize.Height;

                if (lasttxt == null)
                {
                    timetxt.Location = new Point(10, 10);
                }
                else
                {
                    timetxt.Location = new Point(10, lasttxt.Location.Y + lasttxt.Height + 1);
                }

                chatpanel.Controls.Add(timetxt);
                chatpanel.Controls.Add(txt);
                lasttxt = txt;
            }
            else if (!string.IsNullOrEmpty(image) && image != "NULL")
            {
                
                byte[] imageBytes = Convert.FromBase64String(image);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    int originalWidth = img.Width;
                    int originalHeight = img.Height;

                    int minSize = 200;
                    int maxSize = 600;

                    int newWidth = originalWidth;
                    int newHeight = originalHeight;

                    if (originalWidth > maxSize || originalHeight > maxSize)
                    {
                        double scaleFactor = Math.Min((double)maxSize / originalWidth, (double)maxSize / originalHeight);
                        newWidth = (int)(originalWidth * scaleFactor);
                        newHeight = (int)(originalHeight * scaleFactor);
                    }
                    else if (originalWidth < minSize || originalHeight < minSize)
                    {
                        double scaleFactor = Math.Max((double)minSize / originalWidth, (double)minSize / originalHeight);
                        newWidth = (int)(originalWidth * scaleFactor);
                        newHeight = (int)(originalHeight * scaleFactor);
                    }

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Image = System.Drawing.Image.FromStream(ms);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Size = new Size(newWidth, newHeight);

                    RichTextBox timetxt = new RichTextBox();
                    timetxt.Text = $"{txttime}";
                    timetxt.AutoSize = true;
                    timetxt.ReadOnly = true;
                    timetxt.BackColor = chatpanel.BackColor;
                    timetxt.BorderStyle = BorderStyle.None;
                    timetxt.Cursor = Cursors.Hand;
                    timetxt.Font = new System.Drawing.Font("Segoe UI Semibold", 12, FontStyle.Bold);

                    Size timetextSize = TextRenderer.MeasureText(timetxt.Text, timetxt.Font);
                    timetxt.Width = timetextSize.Width + 10;
                    timetxt.Height = timetextSize.Height;

                    if (lasttxt == null)
                    {
                        timetxt.Location = new Point(10, 10);
                    }
                    else
                    {
                        timetxt.Location = new Point(10, lasttxt.Location.Y + lasttxt.Height);
                    }

                    chatpanel.Controls.Add(timetxt);

                    RichTextBox sendertxt = new RichTextBox();
                    sendertxt.Text = $"{sender} : ";
                    sendertxt.AutoSize = true;
                    sendertxt.ReadOnly = true;
                    sendertxt.BackColor = chatpanel.BackColor;
                    sendertxt.BorderStyle = BorderStyle.None;
                    sendertxt.Cursor = Cursors.Hand;
                    sendertxt.Font = new System.Drawing.Font("Segoe UI Semibold", 12, FontStyle.Bold | FontStyle.Italic);

                    Size senderSize = TextRenderer.MeasureText(sendertxt.Text, sendertxt.Font);
                    sendertxt.Width = senderSize.Width + 10;
                    sendertxt.Height = senderSize.Height;

                    if (lasttxt == null)
                    {
                        sendertxt.Location = new Point(70, 10);
                    }
                    else
                    {
                        sendertxt.Location = new Point(70, lasttxt.Location.Y + lasttxt.Height);
                    }

                    chatpanel.Controls.Add(sendertxt);

                    pictureBox.Location = new Point(10, sendertxt.Location.Y + sendertxt.Height + 2);
                    chatpanel.Controls.Add(pictureBox);

                    RichTextBox txt = new RichTextBox();
                    txt.Text = "";
                    txt.ReadOnly = true;
                    txt.BackColor = chatpanel.BackColor;
                    txt.BorderStyle = BorderStyle.None;
                    txt.Cursor = Cursors.Hand;
                    txt.Font = chatpanel.Font;
                    txt.Location = new Point(10, pictureBox.Location.Y + pictureBox.Height);

                    Size txtSize = TextRenderer.MeasureText(txt.Text, txt.Font);
                    txt.Width = timetextSize.Width + 10;
                    txt.Height = timetextSize.Height;

                    chatpanel.Controls.Add(txt);
                    lasttxt = txt;
                }
            }

            chatpanel.ScrollControlIntoView(lasttxt);
        }

        private void SendMsgBtn_Click(object sender, EventArgs e)
        {
            SendMsg();
            chattxt.Text = "";
        }

        void SendMsg()
        {
            if (!string.IsNullOrEmpty(chattxt.Text))
            {
                var msg = new { Type = "[MSG]", Message = chattxt.Text, Image = "NULL", RoomID = inroom.ToString() };
                string jmsg = JsonConvert.SerializeObject(msg);
                Send(jmsg);
            }

        }

        private void chattxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(chattxt.Text))
                {
                    SendMsg();
                    chattxt.Text = "";
                }
                else chattxt.Text = "";
                e.Handled = true;
            }
        }

        private void SendImageBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select an Image File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    long fileSize = new FileInfo(filePath).Length;

                    
                    const long maxFileSize = 500 * 1024; 
                    if (fileSize > maxFileSize)
                    {
                        MessageBox.Show("Seçilen dosya 500KB'dan büyük. Lütfen daha küçük bir dosya seçin.", "Dosya Boyutu Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; 
                    }

                    byte[] imageBytes = File.ReadAllBytes(filePath);
                    string base64String = Convert.ToBase64String(imageBytes);
                    var image = new
                    {
                        Type = "[MSG]",
                        Message = "NULL",
                        Image = base64String,
                        RoomID = inroom.ToString()
                    };

                    string jimage = JsonConvert.SerializeObject(image);
                    Send(jimage);
                }
            }
        }

        private void ServerPageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!disconnect)
            {
                appclose = true;
                stream.Close();
                if (isReceiving)
                {
                    isReceiving = false; 
                }
                System.Windows.Forms.Application.Exit();
            }
            
        }

    }
}
