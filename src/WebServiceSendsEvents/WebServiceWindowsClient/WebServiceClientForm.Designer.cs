namespace WebServiceWindowsClient
{
  partial class WebServiceClientForm
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
      this.SuspendLayout();
      // 
      // listBoxEvents
      // 
      this.listBoxEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listBoxEvents.FormattingEnabled = true;
      this.listBoxEvents.Location = new System.Drawing.Point(12, 53);
      this.listBoxEvents.Name = "listBoxEvents";
      this.listBoxEvents.Size = new System.Drawing.Size(527, 212);
      this.listBoxEvents.TabIndex = 3;
      // 
      // buttonStartSession
      // 
      this.buttonStartSession.Location = new System.Drawing.Point(12, 12);
      this.buttonStartSession.Name = "buttonStartSession";
      this.buttonStartSession.Size = new System.Drawing.Size(143, 23);
      this.buttonStartSession.TabIndex = 2;
      this.buttonStartSession.Text = "Start Session";
      this.buttonStartSession.UseVisualStyleBackColor = true;
      this.buttonStartSession.Click += new System.EventHandler(this.buttonStartSession_Click);
      // 
      // buttonStopSession
      // 
      this.buttonStopSession.Location = new System.Drawing.Point(161, 12);
      this.buttonStopSession.Name = "buttonStopSession";
      this.buttonStopSession.Size = new System.Drawing.Size(143, 23);
      this.buttonStopSession.TabIndex = 4;
      this.buttonStopSession.Text = "Stop Session";
      this.buttonStopSession.UseVisualStyleBackColor = true;
      this.buttonStopSession.Click += new System.EventHandler(this.buttonStopSession_Click);
      // 
      // WebServiceClientForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(551, 275);
      this.Controls.Add(this.buttonStopSession);
      this.Controls.Add(this.listBoxEvents);
      this.Controls.Add(this.buttonStartSession);
      this.Name = "WebServiceClientForm";
      this.Text = "Test Web Service";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebServiceClientForm_FormClosing);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox listBoxEvents;
    private System.Windows.Forms.Button buttonStartSession;
    private System.Windows.Forms.Button buttonStopSession;
  }
}

