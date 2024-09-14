namespace FreeChat
{
    partial class ServerPageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerPageForm));
            roompanel = new Panel();
            chatpanel = new Panel();
            onlinepanel = new Panel();
            chattxt = new RichTextBox();
            SendMsgBtn = new Button();
            lblname = new Label();
            onlinelbl = new Label();
            SendImageBtn = new Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // roompanel
            // 
            roompanel.AutoScroll = true;
            roompanel.BackColor = Color.White;
            roompanel.Location = new Point(9, 32);
            roompanel.Name = "roompanel";
            roompanel.Size = new Size(114, 566);
            roompanel.TabIndex = 0;
            // 
            // chatpanel
            // 
            chatpanel.AutoScroll = true;
            chatpanel.BackColor = Color.White;
            chatpanel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            chatpanel.Location = new Point(129, 32);
            chatpanel.Name = "chatpanel";
            chatpanel.Size = new Size(1006, 566);
            chatpanel.TabIndex = 1;
            // 
            // onlinepanel
            // 
            onlinepanel.AutoScroll = true;
            onlinepanel.BackColor = Color.White;
            onlinepanel.Font = new Font("Segoe UI", 14.25F);
            onlinepanel.Location = new Point(1141, 32);
            onlinepanel.Name = "onlinepanel";
            onlinepanel.Size = new Size(130, 566);
            onlinepanel.TabIndex = 1;
            // 
            // chattxt
            // 
            chattxt.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            chattxt.Location = new Point(249, 604);
            chattxt.Name = "chattxt";
            chattxt.Size = new Size(886, 64);
            chattxt.TabIndex = 2;
            chattxt.Text = "";
            chattxt.KeyPress += chattxt_KeyPress;
            // 
            // SendMsgBtn
            // 
            SendMsgBtn.Font = new Font("Arial", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            SendMsgBtn.Location = new Point(1141, 604);
            SendMsgBtn.Name = "SendMsgBtn";
            SendMsgBtn.Size = new Size(130, 64);
            SendMsgBtn.TabIndex = 3;
            SendMsgBtn.Text = "Send";
            SendMsgBtn.UseVisualStyleBackColor = true;
            SendMsgBtn.Click += SendMsgBtn_Click;
            // 
            // lblname
            // 
            lblname.AutoSize = true;
            lblname.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblname.Location = new Point(12, 8);
            lblname.Name = "lblname";
            lblname.Size = new Size(81, 21);
            lblname.TabIndex = 6;
            lblname.Text = "Username";
            // 
            // onlinelbl
            // 
            onlinelbl.AutoSize = true;
            onlinelbl.Font = new Font("Segoe UI", 12F);
            onlinelbl.Location = new Point(1208, 9);
            onlinelbl.Name = "onlinelbl";
            onlinelbl.Size = new Size(63, 21);
            onlinelbl.TabIndex = 7;
            onlinelbl.Text = "Onlines";
            // 
            // SendImageBtn
            // 
            SendImageBtn.Font = new Font("Arial", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            SendImageBtn.Location = new Point(129, 604);
            SendImageBtn.Name = "SendImageBtn";
            SendImageBtn.Size = new Size(114, 64);
            SendImageBtn.TabIndex = 8;
            SendImageBtn.Text = "Image";
            SendImageBtn.UseVisualStyleBackColor = true;
            SendImageBtn.Click += SendImageBtn_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 604);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(111, 64);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // ServerPageForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1283, 680);
            Controls.Add(pictureBox1);
            Controls.Add(SendImageBtn);
            Controls.Add(onlinelbl);
            Controls.Add(lblname);
            Controls.Add(SendMsgBtn);
            Controls.Add(chattxt);
            Controls.Add(onlinepanel);
            Controls.Add(chatpanel);
            Controls.Add(roompanel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ServerPageForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ServerForm";
            FormClosing += ServerPageForm_FormClosing;
            Load += ServerPageForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel roompanel;
        private Panel chatpanel;
        private Panel onlinepanel;
        private RichTextBox chattxt;
        private Button SendMsgBtn;
        private Label lblname;
        private Label onlinelbl;
        private Button SendImageBtn;
        private PictureBox pictureBox1;
    }
}