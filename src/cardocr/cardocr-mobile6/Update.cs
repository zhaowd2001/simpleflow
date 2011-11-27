using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.Xml;
using System.Text;
using Corrigo.CorrigoNet.CorrigoNetToGo;

namespace Updater
{
	/// <summary>
	/// Main form for the Updater applet
	/// </summary>
	public class frmUpdate : System.Windows.Forms.Form
	{
		#region Form Controls
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button btnCheck;
		private System.Windows.Forms.ProgressBar pbProgress;
		private System.Windows.Forms.Button btnUpdate;
		#endregion

		#region Private Data
		// Form size
		private Size realSize;
		// Http request/response
		private HttpWebRequest m_req;
		private HttpWebResponse m_resp;
		private FileStream m_fs;

		// Data buffer for stream operations
		private byte[] dataBuffer;
		private const int DataBlockSize = 65536;
		
		// Configuration
		private XmlDocument xmlConfig;
		
		private string m_Status;
		private string UpdateUrl;
		private int pbVal, maxVal;
		#endregion
	
		#region Construction/Destruction
		/// <summary>
		///  Constructor
		/// </summary>
		public frmUpdate()
		{
			InitializeComponent();
			realSize = new Size(224, 144);
			this.GotFocus += new EventHandler(frmUpdate_GotFocus);
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pbProgress = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.btnCheck = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			// 
			// pbProgress
			// 
			this.pbProgress.Location = new System.Drawing.Point(16, 16);
			this.pbProgress.Size = new System.Drawing.Size(184, 20);
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(16, 48);
			this.lblStatus.Size = new System.Drawing.Size(184, 20);
			// 
			// btnCheck
			// 
			this.btnCheck.Location = new System.Drawing.Point(16, 88);
			this.btnCheck.Text = "Check";
			this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
			// 
			// btnUpdate
			// 
			this.btnUpdate.Enabled = false;
			this.btnUpdate.Location = new System.Drawing.Point(128, 88);
			this.btnUpdate.Text = "Update";
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			// 
			// frmUpdate
			// 
			this.ClientSize = new System.Drawing.Size(218, 119);
			this.ControlBox = false;
			this.Controls.Add(this.btnCheck);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.pbProgress);
			this.Controls.Add(this.btnUpdate);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Text = "Mobile Updater";
			this.Load += new System.EventHandler(this.Form_Load);

		}
		#endregion

        public string WebServiceUrl_;

		/// <summary>
		/// Delegate for updating the file download progress
		/// </summary>
		public void UpdateProgressValue(object sender, EventArgs e)
		{
			pbProgress.Value = pbVal;
			Application.DoEvents();
		}
		
		/// <summary>
		/// Delegate for setting the progress bar size
		/// </summary>
		public void SetProgressMax(object sender, EventArgs e)
		{
			pbProgress.Maximum = maxVal;
			Application.DoEvents();
		}

		/// <summary>
		/// Form load event handler. Here we read configuration
		/// </summary>
		private void Form_Load(object sender, System.EventArgs e)
		{
			ResizeForm();
			xmlConfig = new XmlDocument();
			try
			{
				xmlConfig.Load(GetCurrentDirectory() + @"\updatecfg.xml");
			}
			catch(Exception ex)
			{
				ShowMessageBox("Failed to read updater configuration: " + ex.ToString());
				this.Close();
			}
		}

		// Helper procedure
		private DialogResult ShowMessageBox(string msg)
		{
			return ShowMessageBox(msg, "", MessageBoxButtons.OK);
		}

		// Helper procedure
		private DialogResult ShowMessageBox(string msg, string caption)
		{
			return ShowMessageBox(msg, caption, MessageBoxButtons.OK);
		}

		// We'd like to keep the window smaller than full-screen
		// Unfortunately, after getting focus back from modal dialog (message box) it gets automatically resized 
		// by the shell. To prevent this we resize it again
		private DialogResult ShowMessageBox(string msg, string caption, MessageBoxButtons buttons)
		{

			DialogResult res = MessageBox.Show(msg, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
			ResizeForm();
			return res;
		}

		// Workaround to keep window non-fullscreen
		private void ResizeForm()
		{
			int x, y;
			x = ( Screen.PrimaryScreen.WorkingArea.Width - realSize.Width ) / 2;
			y = ( Screen.PrimaryScreen.WorkingArea.Height - realSize.Height ) / 2 + Screen.PrimaryScreen.WorkingArea.Top;
			this.Capture = true;
			IntPtr hWnd = GetCapture();
			this.Capture = false;
			SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) | WS_CAPTION);
			MoveWindow(hWnd, x, y, realSize.Width, realSize.Height, true);
		}

		private void frmUpdate_GotFocus(object sender, EventArgs e)
		{
			ResizeForm();
		}

		// Helper procedure
		private string GetCurrentDirectory()
		{
			return Path.GetDirectoryName( Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName );
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			// Create asynchronous web request
			m_req = (HttpWebRequest)HttpWebRequest.Create(UpdateUrl);
			m_req.BeginGetResponse(new AsyncCallback(ResponseReceived), null);
			btnCheck.Enabled = false;
			Cursor.Current = Cursors.WaitCursor;
		}

		// Delegate for setting status line
		private void SetStatus(object sender, System.EventArgs e)
		{
			lblStatus.Text = m_Status;
			Application.DoEvents();
		}

		// Setting status via Control.Invoke. We need this when changing status text from 
		// inside async call callback
		private void SetStatus(string Status)
		{
			m_Status = Status;
			lblStatus.Invoke(new EventHandler(SetStatus));
		}
		
		// Get the assembly version for the specified assembly (config file)
		// Use web service to query update availability
		private void btnCheck_Click(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			SetStatus("Checking for updates");
			Assembly a = null;
			AssemblyName name = new AssemblyName();
			
			// If assembly does not exists, presume the version to be 0.0.0
			name.Version = new Version("0.0.0");
			
			// Try obtaining assembly version
			string sPath = GetCurrentDirectory() + @"\" + xmlConfig["updateinfo"]["checkassembly"].GetAttribute("name");
			try
			{
				if ( File.Exists(sPath) )
				{
					a = Assembly.LoadFrom(sPath);
					name = a.GetName();
				}
			}
			catch(Exception)
			{
			}
			
			// Release assembly, so that its file is not locked anymore
			a = null;
			
			// Use web service to inquire as of update availability
            cardocr_mobile6.uploaderWS.FileUploader agent = new cardocr_mobile6.uploaderWS.FileUploader();
            agent.Url = WebServiceUrl_;// xmlConfig["updateinfo"]["service"].GetAttribute("url");
			string platform = Utils.GetPlatformType();
			string arch = Utils.GetInstructionSet();
			string appName = xmlConfig["updateinfo"]["remoteapp"].GetAttribute("name").ToUpper();
            cardocr_mobile6.uploaderWS.
            UpdateInfo info = agent.GetUpdateInfo(appName, platform, arch, name.Version.Major, name.Version.Minor, name.Version.Build);

			Cursor.Current = Cursors.Default;
			SetStatus("");

			// If there is an updated version allow user to proceed with update
			if ( info.IsAvailable )
			{
				ShowMessageBox("Update is available");
				btnUpdate.Enabled = true;
				UpdateUrl = info.Url;
			}
			else
			{
				ShowMessageBox("There are no updates available");
				this.Close();
			}
		}

		// When cab download is finished, launch it. This will cause
		// wceload.exe to initiate installation process
		private void AllDone(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.Default;
			
			string docname = GetCurrentDirectory() +  @"\download.cab";
			int nSize = docname.Length * 2 + 2;
			IntPtr pData = LocalAlloc(0x40, nSize);
			Marshal.Copy(Encoding.Unicode.GetBytes(docname), 0, pData, nSize - 2);
			SHELLEXECUTEEX see = new SHELLEXECUTEEX();
			see.cbSize = 60;
			see.dwHotKey = 0;
			see.fMask = 0;
			see.hIcon = IntPtr.Zero;
			see.hInstApp = IntPtr.Zero;
			see.hProcess = IntPtr.Zero;;
			see.lpClass = IntPtr.Zero;
			see.lpDirectory = IntPtr.Zero;
			see.lpIDList = IntPtr.Zero;
			see.lpParameters = IntPtr.Zero;
			see.lpVerb = IntPtr.Zero;
			see.nShow = 1;
			see.lpFile = pData;
			ShellExecuteEx(see);
			LocalFree(pData);

			Close();
		}

		// Asynchronous routine to process the http web request
		void ResponseReceived(IAsyncResult res)
		{
			// Try getting the web response. If there was an error (404 or other),
			// web exception will be thrown hete
			try
			{
				m_resp = (HttpWebResponse)m_req.EndGetResponse(res);
			}
			catch(WebException /*ex*/)
			{
				return;
			}
			dataBuffer = new byte[ DataBlockSize ];
			// Prepare the propgres bar
			maxVal = (int)m_resp.ContentLength;
			pbProgress.Invoke(new EventHandler(SetProgressMax));
			m_fs = new FileStream(GetCurrentDirectory() +  @"\download.cab", FileMode.Create);
			
			// Start reading from network stream asynchronously
			m_resp.GetResponseStream().BeginRead(dataBuffer, 0, DataBlockSize, new AsyncCallback(OnDataRead), this);
		}

		// Asynchronous network stream reading
		void OnDataRead(IAsyncResult res)
		{
			// Get bytecount of the received chunk
			int nBytes = m_resp.GetResponseStream().EndRead(res);
			// Dump it to the output stream
			m_fs.Write(dataBuffer, 0, nBytes);
			// Update prgress bar
			pbVal += nBytes;
			pbProgress.Invoke(new EventHandler(UpdateProgressValue));
			if ( nBytes > 0 )
			{
				// If read was successful, continue reading asynchronously as there is more data
				m_resp.GetResponseStream().BeginRead(dataBuffer, 0, DataBlockSize, new AsyncCallback(OnDataRead), this);
			}
			else
			{
				// Otherwise close the stream and proceed with installation
				m_fs.Close();
				m_fs = null;
				this.Invoke(new EventHandler(this.AllDone));
			}
		}

		#region PInvoke defintions
		class SHELLEXECUTEEX
		{
			public UInt32 cbSize; 
			public UInt32 fMask; 
			public IntPtr hwnd; 
			public IntPtr lpVerb; 
			public IntPtr lpFile; 
			public IntPtr lpParameters; 
			public IntPtr lpDirectory; 
			public int nShow; 
			public IntPtr hInstApp; 

			// Optional members 
			public IntPtr lpIDList; 
			public IntPtr lpClass; 
			public IntPtr hkeyClass; 
			public UInt32 dwHotKey; 
			public IntPtr hIcon; 
			public IntPtr hProcess; 
		}

		[DllImport("coredll")]
		extern static int ShellExecuteEx( SHELLEXECUTEEX ex );

		[DllImport("coredll")]
		extern static IntPtr LocalAlloc( int flags, int size );

		[DllImport("coredll")]
		extern static void LocalFree( IntPtr ptr );

		[DllImport("coredll")]
		public static extern IntPtr GetCapture();

		[DllImport("coredll")]
		public static extern IntPtr MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

		[DllImport("coredll")]
		public static extern int GetWindowLong(IntPtr hWnd, int nItem);

		[DllImport("coredll")]
		public static extern void SetWindowLong(IntPtr hWnd, int nItem, int nValue);
		
		public const int GWL_STYLE     =      (-16);
		public const int GWL_EXSTYLE   =      (-20);
		public const int GWL_USERDATA  =      (-21);
		public const int GWL_ID        =      (-12);

		public const int WS_BORDER     =      0x00800000;
		public const int WS_CAPTION     =     0x00C00000;

		#endregion

	}
}
