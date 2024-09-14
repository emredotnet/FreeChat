namespace FreeChat
{
    partial class ConnectForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectForm));
            IpTxt = new TextBox();
            connectButton = new Button();
            PortTxt = new TextBox();
            panel1 = new Panel();
            label3 = new Label();
            label1 = new Label();
            label2 = new Label();
            label4 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // IpTxt
            // 
            IpTxt.Font = new Font("Segoe UI", 12F);
            IpTxt.Location = new Point(12, 38);
            IpTxt.Name = "IpTxt";
            IpTxt.Size = new Size(164, 29);
            IpTxt.TabIndex = 0;
            // 
            // connectButton
            // 
            connectButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            connectButton.Location = new Point(12, 160);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(164, 50);
            connectButton.TabIndex = 1;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // PortTxt
            // 
            PortTxt.Font = new Font("Segoe UI", 12F);
            PortTxt.Location = new Point(12, 114);
            PortTxt.Name = "PortTxt";
            PortTxt.Size = new Size(164, 29);
            PortTxt.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Font = new Font("Segoe UI", 12F);
            panel1.Location = new Point(197, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(345, 198);
            panel1.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 17);
            label3.Name = "label3";
            label3.Size = new Size(186, 21);
            label3.TabIndex = 0;
            label3.Text = "Free Chat'e Hoş geldiniz! ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold | FontStyle.Italic);
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(29, 25);
            label1.TabIndex = 4;
            label1.Text = "IP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold | FontStyle.Italic);
            label2.Location = new Point(12, 86);
            label2.Name = "label2";
            label2.Size = new Size(60, 25);
            label2.TabIndex = 5;
            label2.Text = "PORT";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 48);
            label4.Name = "label4";
            label4.Size = new Size(167, 21);
            label4.TabIndex = 1;
            label4.Text = "Welcome to Free Chat!";
            // 
            // ConnectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(554, 222);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel1);
            Controls.Add(PortTxt);
            Controls.Add(connectButton);
            Controls.Add(IpTxt);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ConnectForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Free Chat";
            Load += ConnectForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox IpTxt;
        private Button connectButton;
        private TextBox PortTxt;
        private Panel panel1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
    }
}
