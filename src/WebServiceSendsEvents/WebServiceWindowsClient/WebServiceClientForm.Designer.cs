namespace WebServiceWindowsClient
{
  partial class MessageBusForm
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
            this.listBoxEvents = new System.Windows.Forms.ListBox();
            this.buttonStartSession = new System.Windows.Forms.Button();
            this.buttonStopSession = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblTo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.listBoxClients = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxEvents
            // 
            this.listBoxEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxEvents.FormattingEnabled = true;
            this.listBoxEvents.ItemHeight = 12;
            this.listBoxEvents.Location = new System.Drawing.Point(153, 49);
            this.listBoxEvents.Name = "listBoxEvents";
            this.listBoxEvents.Size = new System.Drawing.Size(513, 196);
            this.listBoxEvents.TabIndex = 3;
            // 
            // buttonStartSession
            // 
            this.buttonStartSession.Location = new System.Drawing.Point(12, 11);
            this.buttonStartSession.Name = "buttonStartSession";
            this.buttonStartSession.Size = new System.Drawing.Size(118, 21);
            this.buttonStartSession.TabIndex = 2;
            this.buttonStartSession.Text = "&Start Session";
            this.buttonStartSession.UseVisualStyleBackColor = true;
            this.buttonStartSession.Click += new System.EventHandler(this.buttonStartSession_Click);
            // 
            // buttonStopSession
            // 
            this.buttonStopSession.Location = new System.Drawing.Point(523, 10);
            this.buttonStopSession.Name = "buttonStopSession";
            this.buttonStopSession.Size = new System.Drawing.Size(143, 21);
            this.buttonStopSession.TabIndex = 4;
            this.buttonStopSession.Text = "S&top Session";
            this.buttonStopSession.UseVisualStyleBackColor = true;
            this.buttonStopSession.Click += new System.EventHandler(this.buttonStopSession_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(241, 8);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(100, 21);
            this.txtMessage.TabIndex = 5;
            this.txtMessage.TextChanged += new System.EventHandler(this.txtMessage_TextChanged);
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(153, 10);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(53, 21);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "S&end";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(212, 10);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(29, 12);
            this.lblTo.TabIndex = 7;
            this.lblTo.Text = "Say:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(347, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "To:";
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(376, 8);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(100, 21);
            this.txtTo.TabIndex = 8;
            // 
            // listBoxClients
            // 
            this.listBoxClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxClients.FormattingEnabled = true;
            this.listBoxClients.ItemHeight = 12;
            this.listBoxClients.Location = new System.Drawing.Point(12, 49);
            this.listBoxClients.Name = "listBoxClients";
            this.listBoxClients.Size = new System.Drawing.Size(118, 196);
            this.listBoxClients.TabIndex = 10;
            this.listBoxClients.DoubleClick += new System.EventHandler(this.listBoxClients_DoubleClick);
            // 
            // MessageBusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 254);
            this.Controls.Add(this.listBoxClients);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTo);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.buttonStopSession);
            this.Controls.Add(this.listBoxEvents);
            this.Controls.Add(this.buttonStartSession);
            this.Name = "MessageBusForm";
            this.Text = "Test Web Service";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebServiceClientForm_FormClosing);
            this.Load += new System.EventHandler(this.MessageBusForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox listBoxEvents;
    private System.Windows.Forms.Button buttonStartSession;
    private System.Windows.Forms.Button buttonStopSession;
    private System.Windows.Forms.TextBox txtMessage;
    private System.Windows.Forms.Button btnSend;
    private System.Windows.Forms.Label lblTo;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtTo;
    private System.Windows.Forms.ListBox listBoxClients;
  }
}

