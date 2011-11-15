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
            this.txtTo = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxEvents
            // 
            this.listBoxEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxEvents.FormattingEnabled = true;
            this.listBoxEvents.ItemHeight = 12;
            this.listBoxEvents.Location = new System.Drawing.Point(12, 49);
            this.listBoxEvents.Name = "listBoxEvents";
            this.listBoxEvents.Size = new System.Drawing.Size(654, 196);
            this.listBoxEvents.TabIndex = 3;
            // 
            // buttonStartSession
            // 
            this.buttonStartSession.Location = new System.Drawing.Point(12, 11);
            this.buttonStartSession.Name = "buttonStartSession";
            this.buttonStartSession.Size = new System.Drawing.Size(118, 21);
            this.buttonStartSession.TabIndex = 2;
            this.buttonStartSession.Text = "Start Session";
            this.buttonStartSession.UseVisualStyleBackColor = true;
            this.buttonStartSession.Click += new System.EventHandler(this.buttonStartSession_Click);
            // 
            // buttonStopSession
            // 
            this.buttonStopSession.Location = new System.Drawing.Point(523, 10);
            this.buttonStopSession.Name = "buttonStopSession";
            this.buttonStopSession.Size = new System.Drawing.Size(143, 21);
            this.buttonStopSession.TabIndex = 4;
            this.buttonStopSession.Text = "Stop Session";
            this.buttonStopSession.UseVisualStyleBackColor = true;
            this.buttonStopSession.Click += new System.EventHandler(this.buttonStopSession_Click);
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(256, 13);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(100, 21);
            this.txtTo.TabIndex = 5;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(161, 12);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(89, 21);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // MessageBusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 254);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtTo);
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
    private System.Windows.Forms.TextBox txtTo;
    private System.Windows.Forms.Button btnSend;
  }
}

