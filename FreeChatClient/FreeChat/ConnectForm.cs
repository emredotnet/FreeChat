using System;
using System.Net.Sockets;
using System.Text;
namespace FreeChat
{
    public partial class ConnectForm : Form
    {
        int PORT = -1;
        public ConnectForm()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(IpTxt.Text))
            {
                string ip = IpTxt.Text;
                if (!string.IsNullOrEmpty(PortTxt.Text))
                {
                    try
                    {
                        PORT = Convert.ToInt32(PortTxt.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid PORT!");
                        PORT = -1;
                    }

                    if (!(PORT > 65535) && !(PORT < 1))
                    {
                        try
                        {
                            TcpClient client = new TcpClient(ip, PORT);
                            NetworkStream stream = client.GetStream();
                            LoginForm LgnFrm = new LoginForm(stream);
                            LgnFrm.Show();
                            this.Hide();
                        }
                        catch
                        {
                            MessageBox.Show("Invalid Server Address!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid PORT!");
                    }
                }
                else MessageBox.Show("Invalid PORT!");
            }
            else MessageBox.Show("Invalid IP!");
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            //IpTxt.Text = "127.0.0.1";
            //PortTxt.Text = "8080";
        }

    }
}
