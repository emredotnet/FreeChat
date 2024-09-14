namespace FreeChat
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            btn1 = new Button();
            usernametxt = new TextBox();
            passtxt = new TextBox();
            emailtxt = new TextBox();
            regbtn = new Button();
            label1 = new Label();
            label2 = new Label();
            lblemail = new Label();
            panel1 = new Panel();
            label3 = new Label();
            SuspendLayout();
            // 
            // btn1
            // 
            btn1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            btn1.Location = new Point(63, 140);
            btn1.Name = "btn1";
            btn1.Size = new Size(103, 50);
            btn1.TabIndex = 3;
            btn1.Text = "Login";
            btn1.UseVisualStyleBackColor = true;
            btn1.Click += btn1_Click;
            // 
            // usernametxt
            // 
            usernametxt.Font = new Font("Segoe UI", 12F);
            usernametxt.Location = new Point(12, 35);
            usernametxt.Name = "usernametxt";
            usernametxt.Size = new Size(154, 29);
            usernametxt.TabIndex = 1;
            // 
            // passtxt
            // 
            passtxt.Font = new Font("Segoe UI", 12F);
            passtxt.Location = new Point(12, 97);
            passtxt.Name = "passtxt";
            passtxt.Size = new Size(154, 29);
            passtxt.TabIndex = 2;
            // 
            // emailtxt
            // 
            emailtxt.Font = new Font("Segoe UI", 12F);
            emailtxt.Location = new Point(12, 225);
            emailtxt.Name = "emailtxt";
            emailtxt.Size = new Size(154, 29);
            emailtxt.TabIndex = 5;
            emailtxt.Visible = false;
            // 
            // regbtn
            // 
            regbtn.BackgroundImage = (Image)resources.GetObject("regbtn.BackgroundImage");
            regbtn.BackgroundImageLayout = ImageLayout.Center;
            regbtn.Font = new Font("Segoe UI Emoji", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            regbtn.Location = new Point(12, 140);
            regbtn.Name = "regbtn";
            regbtn.Size = new Size(45, 50);
            regbtn.TabIndex = 4;
            regbtn.UseVisualStyleBackColor = true;
            regbtn.Click += regbtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label1.Location = new Point(12, 11);
            label1.Name = "label1";
            label1.Size = new Size(81, 21);
            label1.TabIndex = 5;
            label1.Text = "Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label2.Location = new Point(12, 73);
            label2.Name = "label2";
            label2.Size = new Size(76, 21);
            label2.TabIndex = 6;
            label2.Text = "Password";
            // 
            // lblemail
            // 
            lblemail.AutoSize = true;
            lblemail.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblemail.Location = new Point(12, 201);
            lblemail.Name = "lblemail";
            lblemail.Size = new Size(48, 21);
            lblemail.TabIndex = 7;
            lblemail.Text = "Email";
            lblemail.Visible = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Font = new Font("Segoe UI", 15.75F, FontStyle.Italic, GraphicsUnit.Point, 162);
            panel1.Location = new Point(181, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(447, 347);
            panel1.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 162);
            label3.Location = new Point(28, 327);
            label3.Name = "label3";
            label3.Size = new Size(111, 30);
            label3.TabIndex = 9;
            label3.Text = "Free Chat!";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(635, 369);
            Controls.Add(label3);
            Controls.Add(panel1);
            Controls.Add(lblemail);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(regbtn);
            Controls.Add(emailtxt);
            Controls.Add(passtxt);
            Controls.Add(usernametxt);
            Controls.Add(btn1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            FormClosing += LoginForm_FormClosing;
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn1;
        private TextBox usernametxt;
        private TextBox passtxt;
        private TextBox emailtxt;
        private Button regbtn;
        private Label label1;
        private Label label2;
        private Label lblemail;
        private Panel panel1;
        private Label label3;
    }
}