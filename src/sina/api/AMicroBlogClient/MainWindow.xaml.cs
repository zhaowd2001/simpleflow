using System;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using AMicroblogAPI;
using AMicroblogAPI.Common;
using AMicroblogAPI.DataContract;

namespace AMicroblogAPISample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        private MainViewModel viewModel = new MainViewModel();

        public MainWindow()
        {             
            InitializeComponent();

            var token = CredentialHelper.Get();
            if (null == token)
            {
                var logonWin = new Logon();
                var dialogResult = logonWin.ShowDialog();
                if (!dialogResult.HasValue || !dialogResult.Value)
                {
                    this.Close();
                }
            }

            this.DataContext = viewModel;
            poster.ViewModel = viewModel;
        }

        #region Event

        #region Retrieves Info

        private void HandleWindowLoaded(object sender, RoutedEventArgs e)
        {
            DoLoading();
        }

        private void DoLoading()
        {
            if (null != AMicroblogAPI.Environment.AccessToken)
            {
                try
                {
                    var userInfo = AMicroblog.VerifyCredential();
                    viewModel.CurrentUser = userInfo;

                    ShowMessage("Retrieving data, please standby...");

                    viewModel.RetrieveHomeStatuses();
                }
                catch (AMicroblogException aex)
                {
                    ShowMessage(aex.Message, false);
                    poster.IsEnabled = false;
                }
            }
            else
            {
                this.Close();
            }
        }

        #endregion

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.IsLoaded)
            {
                var confirmDialog = new ConfirmationDialog();
                confirmDialog.Message = "Are you sure to quit AMicroblogAPI demo app?";
                confirmDialog.ConfirmButtonText = "Quit";
                confirmDialog.Owner = this;
                confirmDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var isPicAttached = poster.IsPicAttached;
                poster.IsPicAttached = false;
                this.DoBlur();
                var dialogResult = confirmDialog.ShowDialog();
                if (dialogResult.HasValue)
                {
                    if (!dialogResult.Value)
                        e.Cancel = true;
                }

                this.UndoBlur();
                poster.IsPicAttached = isPicAttached;
            }

            base.OnClosing(e);
        }

        private void HandleCloseBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HandleSwitchWindowState(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void HandleSearch(object sender, RoutedEventArgs e)
        {
            var keyword = txbSearchKeyword.Text;
            if (!string.IsNullOrEmpty(keyword))
            {
                tbItemSearchResult.IsSelected = true;

                viewModel.DoSearch(keyword);
            }
            else
                txbSearchKeyword.Focus();
        }

        private void HandleSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                HandleSearch(null, null);
        }

        private void HandleSearchKeywordTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (null != btnSearch)
            {
                var keyword = txbSearchKeyword.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                    btnSearch.IsEnabled = true;
                else
                    btnSearch.IsEnabled = false;
            }
        }

        private void HandleWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                poster.CancelImageGen();
        }

        private void HandleOpenImgViewerForForwardedStatus(object sender, MouseButtonEventArgs e)
        {
            var target = sender as FrameworkElement;
            var data = target.Tag as StatusData;

            ImageViewer.ShowImage(data, true);
        }

        private void HanleOpenImageViewerForStatus(object sender, MouseButtonEventArgs e)
        {
            var target = sender as FrameworkElement;
            var data = target.Tag as StatusData;
            ImageViewer.ShowImage(data, false);
        }

        private void HandleVisitAMicroblogAPI(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://amicroblogapi.codeplex.com");
        }

        #region Operation Buttons Events

        private void HandleForward(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var statusInfo = target.Tag as StatusData;
            if (null != statusInfo)
            {
                var isPicAttached = poster.IsPicAttached;
                poster.IsPicAttached = false;
                DoBlur();
                var postWin = new PostWindow(viewModel, statusInfo, PosterMode.Forward);
                postWin.Owner = this;
                postWin.ShowDialog();
                UndoBlur();
                poster.IsPicAttached = isPicAttached;
            }
        }

        private void HandleComment(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var statusInfo = target.Tag as StatusData;
            if (null != statusInfo)
            {
                var isPicAttached = poster.IsPicAttached;
                poster.IsPicAttached = false;
                DoBlur();
                var postWin = new PostWindow(viewModel, statusInfo, PosterMode.Comment);
                postWin.Owner = this;
                postWin.ShowDialog();
                UndoBlur();
                poster.IsPicAttached = isPicAttached;
            }
        }

        private void HandleRepost(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var statusInfo = target.Tag as StatusData;
            if (null != statusInfo)
            {
                var isPicAttached = poster.IsPicAttached;
                poster.IsPicAttached = false; DoBlur();
                var postWin = new PostWindow(viewModel, statusInfo, PosterMode.Repost);
                postWin.Owner = this;
                postWin.ShowDialog();
                UndoBlur();
                poster.IsPicAttached = isPicAttached;
            }
        }

        private void HandleAddToFavourite(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var statusData = target.Tag as StatusData;

            if (!statusData.Favorited)
            {
                viewModel.AddToFavourite(statusData, target);
            }
            else
            {
                viewModel.RemoveFromFavourite(statusData, target);
            }
        }

        private void HandleDelete(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var pos = Mouse.GetPosition(this);
            var confirmDialog = new ConfirmationDialog(this.Left + pos.X, this.Top + pos.Y);
            confirmDialog.Owner = this;

            DoBlur();
            var dialogResult = confirmDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                var statusInfo = target.Tag as StatusData;
                var deletedStatus = AMicroblog.DeleteStatus(statusInfo.ID);

                viewModel.RemoveStatusFromView(deletedStatus);
            }
            UndoBlur();
        }

        private void HandleDeleteToMeComment(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var comment = target.Tag as CommentInfo;

            AMicroblog.DeleteComment(comment.ID);
            var exists = viewModel.CommentsToMe.FirstOrDefault((item) => comment.ID == item.ID);
            if (null != exists)
                viewModel.CommentsToMe.Remove(exists);

            ShowMessage("Comments deleted.");
        }

        private void HandleDeleteByMeComment(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var comment = target.Tag as CommentInfo;

            AMicroblog.DeleteComment(comment.ID);
            var exists = viewModel.CommentsByMe.FirstOrDefault((item) => comment.ID == item.ID);
            if(null != exists)
                viewModel.CommentsByMe.Remove(exists);

            ShowMessage("Comments deleted.");
        }

        private void HandleReplyComment(object sender, RoutedEventArgs e)
        {
            var target = sender as FrameworkElement;
            var comment = target.Tag as CommentInfo;

            if (null != comment)
            {
                var isPicAttached = poster.IsPicAttached;
                poster.IsPicAttached = false;
                DoBlur();
                var postWin = new PostWindow(viewModel, comment);
                postWin.Owner = this;
                postWin.ShowDialog();
                UndoBlur();
                poster.IsPicAttached = isPicAttached;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private void SetItemState(FrameworkElement target, bool val)
        {
            this.Dispatcher.Invoke(new Action(delegate()
            {
                target.IsEnabled = val;
            }));
        }

        private bool Validate()
        {
            return poster.Validate();
        }

        private void ClearMessage()
        {
            ShowMessage("Ready");
        }

        private void ShowMessage(string msg, params object[] parameters)
        {
            ShowMessage(msg, true, parameters);
        }

        private void ShowMessage(string msg, bool isSuccess, params object[] parameters)
        {
            viewModel.ShowMessage(msg, isSuccess, parameters);
        }

        #endregion

        private void HandleSwitchUserBtnClicked(object sender, RoutedEventArgs e)
        {
            this.DoBlur();

            var logonWin = new Logon();
            logonWin.Owner = this;
            logonWin.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            var dialogResult = logonWin.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                viewModel.AtMeStatuses.Clear();
                viewModel.CommentsByMe.Clear();
                viewModel.CommentsToMe.Clear();
                viewModel.Counters.Clear();
                viewModel.FavouriteStatuses.Clear();
                viewModel.Followers.Clear();
                viewModel.Followings.Clear();
                viewModel.MyHomeStatuses.Clear();
                viewModel.MyStatuses.Clear();

                DoLoading();
            }

            this.UndoBlur();
        }
    }
}
