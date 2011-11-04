namespace TestUploader
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnSetWebUrl = new System.Windows.Forms.Button();
            this.btnList = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.txtNewName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(198, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 21);
            this.button1.TabIndex = 0;
            this.button1.Text = "&Browse...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(198, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 21);
            this.button2.TabIndex = 1;
            this.button2.Text = "&Upload";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(12, 29);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(180, 21);
            this.txtFileName.TabIndex = 2;
            this.txtFileName.Text = "e:\\1.tif";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(198, 81);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 21);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(198, 109);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 21);
            this.btnDownload.TabIndex = 4;
            this.btnDownload.Text = "&Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnSetWebUrl
            // 
            this.btnSetWebUrl.Location = new System.Drawing.Point(12, 109);
            this.btnSetWebUrl.Name = "btnSetWebUrl";
            this.btnSetWebUrl.Size = new System.Drawing.Size(75, 21);
            this.btnSetWebUrl.TabIndex = 5;
            this.btnSetWebUrl.Text = "&Web Url";
            this.btnSetWebUrl.UseVisualStyleBackColor = true;
            this.btnSetWebUrl.Click += new System.EventHandler(this.btnSetWebUrl_Click);
            // 
            // btnList
            // 
            this.btnList.Location = new System.Drawing.Point(198, 136);
            this.btnList.Name = "btnList";
            this.btnList.Size = new System.Drawing.Size(75, 21);
            this.btnList.TabIndex = 6;
            this.btnList.Text = "&List";
            this.btnList.UseVisualStyleBackColor = true;
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(198, 163);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(75, 21);
            this.btnMove.TabIndex = 7;
            this.btnMove.Text = "&Move";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // txtNewName
            // 
            this.txtNewName.Location = new System.Drawing.Point(12, 163);
            this.txtNewName.Name = "txtNewName";
            this.txtNewName.Size = new System.Drawing.Size(180, 21);
            this.txtNewName.TabIndex = 8;
            this.txtNewName.Text = "testdata\\test2.jpg";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 264);
            this.Controls.Add(this.txtNewName);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.btnList);
            this.Controls.Add(this.btnSetWebUrl);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Uploader Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnSetWebUrl;
        private System.Windows.Forms.Button btnList;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.TextBox txtNewName;
    }
}

