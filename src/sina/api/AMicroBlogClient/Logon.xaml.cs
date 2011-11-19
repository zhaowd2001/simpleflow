using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using AMicroblogAPI;
using AMicroblogAPI.Common;
using System.Windows.Media;

namespace AMicroblogAPISample
{
    /// <summary>
    /// Interaction logic for Logon.xaml
    /// </summary>
    public partial class Logon : Window
    {
        public Logon()
        {
            InitializeComponent();
        }

        private void btnLogon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gridMain.IsEnabled = false;
                lbMsg.Text = string.Empty;

                var name = txbUserName.Text.Trim();
                var pwd = txbPwd.Password;

                if (string.IsNullOrEmpty(name))
                {
                    ShowMessage("Name not provided.");
                    return;
                }

                if (string.IsNullOrEmpty(pwd))
                {
                    ShowMessage("Password not provided.");
                    return;
                }

                var callback = new AsyncCallback<OAuthAccessToken>(delegate(AsyncCallResult<OAuthAccessToken> result)
                {
                    if (result.Success)
                    {
                        Token = result.Data;

                        this.Dispatcher.Invoke(new Action(delegate()
                        {
                            if (ckbSavePwd.IsChecked.HasValue && ckbSavePwd.IsChecked.Value)
                                CredentialHelper.Save(name, Token);
                            gridMain.IsEnabled = true;
                            this.DialogResult = true;
                        }));
                    }
                    else
                    {
                       this.Dispatcher.Invoke(new Action(delegate()
                       {
                           gridMain.IsEnabled = true;
                       }));

                       ShowMessage("Failed to login due to: {0}.", result.Exception.Message);
                    }
                });

                lbMsg.Text = "Login...";
                AMicroblog.LoginAsync(callback, name, pwd);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        public OAuthAccessToken Token { get; private set; }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txbUserName);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ShowMessage(string message, params string[] args)
        {
            this.Dispatcher.Invoke(new Action(delegate()
            {
                lbMsg.Foreground = new SolidColorBrush(Colors.Red);
                lbMsg.Text = string.Format(message, args);
            }));
        }
    }
}
