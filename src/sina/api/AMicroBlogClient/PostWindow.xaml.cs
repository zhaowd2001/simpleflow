using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AMicroblogAPI.DataContract;
using AMicroblogAPI;
using AMicroblogAPI.Common;
using System.Windows.Threading;

namespace AMicroblogAPISample
{
    /// <summary>
    /// PostWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PostWindow : WindowBase
    {
        private StatusData targetStatus;
        private CommentInfo targetComment;
        private MainViewModel viewModel;
        private PosterMode mode;

        public PostWindow(MainViewModel viewModel, CommentInfo targetComment)
        {
            InitializeComponent();

            this.poster.ViewModel = this.viewModel = viewModel;
            this.poster.Mode = this.mode = PosterMode.ReplyComment;
            this.targetStatusContainer.DataContext = this.targetComment = targetComment;
        }

        public PostWindow(MainViewModel viewModel, StatusData targetStatus, PosterMode mode)
        {
            InitializeComponent();

            this.targetStatusContainer.DataContext = this.targetStatus = targetStatus;
            this.poster.ViewModel = this.viewModel = viewModel;
            this.poster.Mode = this.mode = mode;

            this.Loaded += new RoutedEventHandler(HandlePostWindowLoaded);
        }

        private void HandlePostWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (mode == PosterMode.Forward)
            {
                tbTitle.Text = "Forward Status";
                ckbOption.Content = "Comments it too";
            }
            else if (mode == PosterMode.Comment)
            {
                tbTitle.Text = "Comment Status";
                ckbOption.Content = "Forwards it too";

                var comments = AMicroblog.GetComments(targetStatus.ID);
                if (comments.Items.Count > 0)
                {
                    lbComments.ItemsSource = comments.Items;
                    lbComments.Visibility = Visibility.Visible;
                }
            }
            else if (mode == PosterMode.Repost)
            {
                tbTitle.Text = "Repost Status";
                poster.Text = string.Format(" //@{0}:{1}", targetStatus.User.ScreenName, targetStatus.Text);
                if (!string.IsNullOrEmpty(targetStatus.OriginalPic))
                {
                    ckbOption.Content = "Combines the text and pic";
                    var localPath = ImageHelper.GetImage(targetStatus.OriginalPic);
                    poster.AttachedPicLocation = localPath;

                    DispatcherTimer disTimer = new DispatcherTimer(DispatcherPriority.Render, this.Dispatcher);
                    disTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                    disTimer.Tick += new EventHandler(disTimer_Tick);
                    disTimer.Start();
                }
                else
                    ckbOption.Visibility = Visibility.Collapsed;
            }
            else if (mode == PosterMode.ReplyComment)
            {
                ckbOption.Visibility = Visibility.Collapsed;

            }
        }

        private void disTimer_Tick(object sender, EventArgs e)
        {
            poster.IsPicAttached = true;
            DispatcherTimer disTimer = sender as DispatcherTimer;
            disTimer.Stop();
        }

        private void HandleOptionChecked(object sender, RoutedEventArgs e)
        {
            if (mode == PosterMode.Repost)
            {
                poster.IsPostAsPic = true;
                if (!string.IsNullOrEmpty(targetStatus.OriginalPic))
                {
                    poster.GenAttachedPicLocation = ImageHelper.GetImage(targetStatus.OriginalPic);
                }
            }
        }

        private void HandleOptionUnchecked(object sender, RoutedEventArgs e)
        {
            if (mode == PosterMode.Repost)
            {
                poster.IsPostAsPic = false;

                poster.GenAttachedPicLocation = string.Empty;
            }
        }

        private void HandlePosting(object sender, PostingEventArgs e)
        {
            if (mode == PosterMode.Forward)
            {
                DoForward(e.PostingText);
                
                if (ckbOption.IsChecked.HasValue && ckbOption.IsChecked.Value)
                    DoComment(e.PostingText);
                e.IsContinue = false;
            }
            else if (mode == PosterMode.Comment)
            {
                DoComment(e.PostingText);
                if (ckbOption.IsChecked.HasValue && ckbOption.IsChecked.Value)
                    DoForward(e.PostingText);
                e.IsContinue = false;
            }
            else if (mode == PosterMode.ReplyComment)
            {
                AMicroblog.ReplyComment(targetComment.ID, poster.Text, targetComment.Status.ID);
                viewModel.ShowMessage("Comments replied.");
            }
            else if (mode == PosterMode.Repost)
            {
                e.IsContinue = true;
            }
            
            this.Close();
        }

        private void DoForward(string postingText)
        {
            var callback = new AsyncCallback<StatusInfo>(delegate(AsyncCallResult<StatusInfo> result)
            {
                if (result.Success)
                {
                    targetStatus.Forwards ++;

                    AddStatusToView(result.Data);

                    viewModel.ShowMessage("Forward successfully.");
                }
                else
                {
                    viewModel.ShowMessage("Failed to forward the status due to: {0}.", false, result.Exception.Message);
                }
            });

            viewModel.ShowMessage("Forward...");
            AMicroblog.ForwardAsync(callback, targetStatus.ID, postingText);
        }

        private void DoComment(string postingText)
        {
            var callback = new AsyncCallback<CommentInfo>(delegate(AsyncCallResult<CommentInfo> result)
            {
                if (result.Success)
                {
                    targetStatus.Comments++;

                    viewModel.ShowMessage("Comments posted successfully.");
                }
                else
                {
                    viewModel.ShowMessage("Failed to comment for the status due to: {0}.", false, result.Exception.Message);
                }
            });

            viewModel.ShowMessage("Posting comment...");

            AMicroblog.CommentAsync(callback, targetStatus.ID, postingText);
        }

        private void AddStatusToView(StatusInfo status)
        {
            this.Dispatcher.Invoke(new Action<StatusInfo>(delegate(StatusInfo data)
            {
                viewModel.MyHomeStatuses.Insert(0, DataConverter.ConvertFrom(data));
                viewModel.MyStatuses.Insert(0, DataConverter.ConvertFrom(data));

            }), status);
        }

        private void HandleCloseBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
