namespace cardocr_mobile6
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.btnUpload = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnStartSession = new System.Windows.Forms.Button();
            this.btnStopSession = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnCamera = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(19, 116);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(101, 31);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "&Upload";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point(20, 14);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(220, 65);
            this.lblInfo.Text = "lblInfo";
            // 
            // btnStartSession
            // 
            this.btnStartSession.Location = new System.Drawing.Point(20, 153);
            this.btnStartSession.Name = "btnStartSession";
            this.btnStartSession.Size = new System.Drawing.Size(100, 30);
            this.btnStartSession.TabIndex = 2;
            this.btnStartSession.Text = "&Start Session";
            this.btnStartSession.Click += new System.EventHandler(this.btnStartSession_Click);
            // 
            // btnStopSession
            // 
            this.btnStopSession.Location = new System.Drawing.Point(21, 223);
            this.btnStopSession.Name = "btnStopSession";
            this.btnStopSession.Size = new System.Drawing.Size(100, 30);
            this.btnStopSession.TabIndex = 4;
            this.btnStopSession.Text = "S&top Session";
            this.btnStopSession.Click += new System.EventHandler(this.btnStopSession_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(19, 189);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(101, 31);
            this.btnSendMessage.TabIndex = 3;
            this.btnSendMessage.Text = "Sen&d Message";
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(127, 223);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(110, 30);
            this.btnQuit.TabIndex = 5;
            this.btnQuit.Text = "&Quit";
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnCamera
            // 
            this.btnCamera.Location = new System.Drawing.Point(19, 82);
            this.btnCamera.Name = "btnCamera";
            this.btnCamera.Size = new System.Drawing.Size(101, 31);
            this.btnCamera.TabIndex = 0;
            this.btnCamera.Text = "&Camera";
            this.btnCamera.Click += new System.EventHandler(this.btnCamera_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.btnCamera);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.btnStopSession);
            this.Controls.Add(this.btnStartSession);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnUpload);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnStartSession;
        private System.Windows.Forms.Button btnStopSession;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnCamera;
    }
}

