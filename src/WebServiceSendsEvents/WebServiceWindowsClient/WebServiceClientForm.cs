
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WebServiceWindowsClient
{
  public partial class WebServiceClientForm : Form
  {
    #region local members
    private WebServiceWindowsClient.localhost.WebService m_service;
    private Guid m_sessionID;
    private int m_clientID;
    #endregion local members

    public WebServiceClientForm()
    {
      InitializeComponent();

      // Create proxy for WebService
      m_service = new WebServiceWindowsClient.localhost.WebService();

      // Subscribe for event
      m_service.GetActiveClientsCompleted += new WebServiceWindowsClient.localhost.GetActiveClientsCompletedEventHandler(m_service_GetActiveClientsCompleted);

      buttonStartSession.Enabled = true;
      buttonStopSession.Enabled = false;

      // Just for test purposes set m_sessionID and m_clientID to something random
      m_sessionID = Guid.NewGuid();
      m_clientID = Guid.NewGuid().ToByteArray()[0];
    }

    void m_service_GetActiveClientsCompleted(object sender, WebServiceWindowsClient.localhost.GetActiveClientsCompletedEventArgs e)
    {
      // Add current list of active clients to list box
      int[] clients = e.Result;
      string client_list = "";
      foreach (int client in clients) client_list += client + " ";
      listBoxEvents.Items.Add(string.Format("GetActiveClients completed with result: {0}", client_list));

      // This call reactivates GetActiveClients event listener
      m_service.GetActiveClientsAsync(m_sessionID);
    }

    private void buttonStartSession_Click(object sender, EventArgs e)
    {
      buttonStartSession.Enabled = false;
      buttonStopSession.Enabled = true;

      // Start session
      m_service.StartSession(m_sessionID, m_clientID);

      // Update listbox
      listBoxEvents.Items.Add(string.Format("New session for client {0} started", m_clientID));

      // This call activates GetActiveClients event listener
      m_service.GetActiveClientsAsync(m_sessionID);
    }

    private void WebServiceClientForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      // Stop session
      m_service.StopSession(m_sessionID);
    }

    private void buttonStopSession_Click(object sender, EventArgs e)
    {
      buttonStartSession.Enabled = true;
      buttonStopSession.Enabled = false;

      // Stop session
      m_service.StopSession(m_sessionID);

      // Update listbox
      listBoxEvents.Items.Add(string.Format("Session for client {0} stopped", m_clientID));
    }
  }
}