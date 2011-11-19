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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Reflection;
using Microsoft.Win32;
using AMicroblogAPI.DataContract;
using AMicroblogAPI;
using AMicroblogAPI.Common;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AMicroblogAPISample
{
    /// <summary>
    /// Poster.xaml 的交互逻辑
    /// </summary>
    public partial class Poster : UserControl, INotifyPropertyChanged
    {
        public Poster()
        {
            InitializeComponent();

            container.DataContext = this;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event EventHandler<PostingEventArgs> Posting;
        public event EventHandler Posted;

        private MainViewModel viewModel;
        public MainViewModel ViewModel 
        {
            get 
            {
                return viewModel;
            }
            set
            {
                viewModel = value;
                if (null != viewModel)
                    lbSuggestion.DataContext = viewModel;
            }
        }

        public PosterMode Mode { get; set; }

        public void CancelImageGen()
        {
            if (IsPostAsPic)
            {
                IsPostAsPic = false;
            }
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(txbStatus.Text.Trim()))
            {
                ShowMessage("Weibo content not specified.", false);
                txbStatus.Focus();
                return false;
            }

            return true;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            if (Mode != PosterMode.Standard && Mode != PosterMode.Repost)
            {
                optionBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #region Post Pic
        
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (value != text)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        private bool isPostAsPic;
        public bool IsPostAsPic
        {
            get
            {
                return isPostAsPic;
            }
            set
            {
                if (value != isPostAsPic)
                {
                    isPostAsPic = value;
                    OnPropertyChanged("IsPostAsPic");
                }
            }
        }

        private string genAttachedPicLocation;
        public string GenAttachedPicLocation
        {
            get
            {
                return genAttachedPicLocation;
            }
            set
            {
                if (value != genAttachedPicLocation)
                {
                    genAttachedPicLocation = value;
                    OnPropertyChanged("GenAttachedPicLocation");
                }
            }
        }

        private bool isPicAttached;
        public bool IsPicAttached
        {
            get
            {
                return isPicAttached;
            }
            set
            {
                if (value != isPicAttached)
                {
                    isPicAttached = value;
                    OnPropertyChanged("IsPicAttached");
                }
            }
        }

        private string attachedPicLocation;
        public string AttachedPicLocation
        {
            get
            {
                return attachedPicLocation;
            }
            set
            {
                if (value != attachedPicLocation)
                {
                    attachedPicLocation = value;
                    OnPropertyChanged("AttachedPicLocation");
                    OnPropertyChanged("AttachedPicName");
                }
            }
        }

        public string AttachedPicName
        {
            get
            {
                if (!string.IsNullOrEmpty(attachedPicLocation))
                    return System.IO.Path.GetFileName(attachedPicLocation);
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Post Related

        private void HandlePostBtnClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var availableLen = GetAvailableStatusText();
                if (availableLen < 0)
                {
                    ShowMessage("Exceeds the status length limit. Try the post-as-pic option.", false);

                    IsPostAsPic = true;
                    return;
                }

                ClearMessage();

                if (!Validate())
                    return;

                if (null != Posting)
                {
                    var args = new PostingEventArgs();
                    args.PostingText = Text;

                    Posting(this, args);

                    if (!args.IsContinue)
                        return;
                }

                ShowMessage("Posting...");

                if (!this.IsPicAttached)
                {
                    var updateStatusInfo = new UpdateStatusInfo() { Status = Text, Latitude = 31.22f, Longitude = 121.48f };

                    AMicroblog.PostStatusAsync(
                        delegate(AsyncCallResult<StatusInfo> result)
                        {
                            if (result.Success)
                            {
                                var statusInfo = result.Data;
                                AddStatusToView(statusInfo);

                                ShowMessage("Message posted successfully at {0}", statusInfo.CreatedAt);
                            }
                            else
                                ShowMessage("Failed to post message due to: {0}", false, result.Exception.Message);


                            if (null != Posted)
                                Posted(this, null);

                        }, updateStatusInfo);
                }
                else
                {
                    try
                    {
                        var updStatusData = new UpdateStatusWithPicInfo();
                        updStatusData.Status = Text;
                        updStatusData.Pic = this.AttachedPicLocation;

                        var postCallback = new AsyncCallback<StatusInfo>(delegate(AsyncCallResult<StatusInfo> result)
                        {
                            if (result.Success)
                            {
                                var statusInfo = result.Data;

                                AddStatusToView(statusInfo);

                                this.AttachedPicLocation = string.Empty;

                                ShowMessage("Message posted successfully at {0}", statusInfo.CreatedAt);
                            }
                            else
                            {
                                this.IsPicAttached = true;
                                ShowMessage("Failed to post message due to: {0}", false, result.Exception.Message);
                            }
                        });

                        AMicroblog.PostStatusWithPicAsync(postCallback, updStatusData);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message, false);
                    }
                }

                this.IsPicAttached = false;
                txbStatus.Clear();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
        }

        private void HandleBrowserBtnClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "All files|*.*";
            dialog.Title = "Locates the picture file to post";
            var result = dialog.ShowDialog(Application.Current.MainWindow);
            if (result.HasValue && result.Value)
            {
                IsPostAsPic = false;

                this.AttachedPicLocation = dialog.FileName;
                this.IsPicAttached = true;

                SetPostButtonState();
            }
        }

        private void HandleDiscardPic(object sender, RoutedEventArgs e)
        {
            this.IsPicAttached = false;
            this.AttachedPicLocation = string.Empty;

            SetPostButtonState();
        }

        #endregion

        #region Suggestion Related

        private void HandStatusTextChanged(object sender, TextChangedEventArgs e)
        {
            SetPostButtonState();

            var availableChar = GetAvailableStatusText();

            txbStatusAvailableCharsMsg.Text = string.Format("{0} chars available", availableChar);
            if (availableChar < 0)
                txbStatusAvailableCharsMsg.Foreground = new SolidColorBrush(Colors.Red);
            else
                txbStatusAvailableCharsMsg.Foreground = new SolidColorBrush(Colors.Gray);

            PerformSuggestion();
        }

        private void PerformSuggestion()
        {
            if (suspendChangeHandling)
                return;

            string statusText = txbStatus.Text;
            var match = Regex.Match(statusText, @"@([^\s@]+?)$");
            if (match.Success)
            {
                var location = txbStatus.GetRectFromCharacterIndex(txbStatus.CaretIndex);

                var screenNameKeyword = match.Groups[1].Value.ToLowerInvariant();
                var usersMatched = new ObservableCollection<UserData>();
                foreach (var following in ViewModel.Followings)
                {
                    if (following.ScreenName.ToLowerInvariant().Contains(screenNameKeyword) && !following.ScreenName.ToLowerInvariant().Equals(screenNameKeyword, StringComparison.InvariantCultureIgnoreCase))
                    {
                        usersMatched.Add(following);
                    }
                }

                foreach (var follower in ViewModel.Followers)
                {
                    if (follower.ScreenName.ToLowerInvariant().Contains(screenNameKeyword) && !follower.ScreenName.ToLowerInvariant().Equals(screenNameKeyword, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (null == usersMatched.FirstOrDefault((item) => item.ScreenName == follower.ScreenName))
                        {
                            usersMatched.Add(follower);
                        }
                    }
                }

                if (usersMatched.Count > 0)
                {
                    ViewModel.UserSuggestions = usersMatched;

                    lbSuggestion.SelectedIndex = 0;
                    suggestionPop.HorizontalOffset = location.Left;
                    suggestionPop.VerticalOffset = location.Bottom;
                    suggestionPop.IsOpen = true;
                }
                else
                    suggestionPop.IsOpen = false;
            }
            else
                suggestionPop.IsOpen = false;
        }

        private bool suspendChangeHandling;
        private void HandleSelectSuggestion(object sender, MouseButtonEventArgs e)
        {
            var target = sender as FrameworkElement;
            var userInfo = target.Tag as UserInfo;

            ProcessSuggestionSelected(userInfo);
        }

        private void ProcessSuggestionSelected(UserInfo userInfo)
        {
            if (null == userInfo)
                return;

            suggestionPop.IsOpen = false;

            var statusText = txbStatus.Text;
            suspendChangeHandling = true;
            txbStatus.Text = Regex.Replace(statusText, @"@([^\s@]+?)$", "@" + userInfo.ScreenName + " ");
            txbStatus.CaretIndex = txbStatus.Text.Length;
            suspendChangeHandling = false;
        }

        private void HandleStatusTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (suggestionPop.IsOpen)
            {
                if (e.Key == Key.Down)
                {
                    var idx = lbSuggestion.SelectedIndex;
                    if (idx < lbSuggestion.Items.Count)
                        lbSuggestion.SelectedIndex++;

                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    var idx = lbSuggestion.SelectedIndex;
                    if (idx > 0)
                        lbSuggestion.SelectedIndex--;
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter || e.Key == Key.Space)
                {
                    var userInfo = lbSuggestion.SelectedItem as UserInfo;
                    ProcessSuggestionSelected(userInfo);
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    suggestionPop.IsOpen = false;
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Enter && ModifierKeys.Control == Keyboard.Modifiers)
            {
                HandlePostBtnClicked(this, null);
            }
        }

        private void HandleSuggestionPopClosed(object sender, EventArgs e)
        {
            txbStatus.Focus();
        }

        #endregion

        #region Post-as-pic Events

        private const string TempImgFileName = "temp.png";
        private void HandleGenerateImg(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)imgGen.ActualWidth + 1, (int)imgGen.ActualHeight + 1, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(imgGen);

            var dir = ImageHelper.StartupLocation;

            using (var stream = File.Open(dir + TempImgFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
            }

            IsPostAsPic = false;
            this.AttachedPicLocation = dir + TempImgFileName;

            // Clears the text before the img popup is shown.
            if (GetAvailableStatusText() <= 0)
            {
                txbStatus.Text = txbStatus.Text.Substring(0, 130) + "...";
            }
            SetPostButtonState();

            this.IsPicAttached = true;
        }

        private void HandleCancelGenerateImg(object sender, RoutedEventArgs e)
        {
            this.IsPostAsPic = false;
        }

        private void HandleIncFontSize(object sender, RoutedEventArgs e)
        {
            var fs = imgBlock.FontSize;
            if (fs < 30)
                imgBlock.FontSize = fs + 1;
        }

        private void HandleDecFontSize(object sender, RoutedEventArgs e)
        {
            var fs = imgBlock.FontSize;
            if (fs > 8)
                imgBlock.FontSize = fs - 1;
        }

        private void HandlePostAsImgPopOpened(object sender, EventArgs e)
        {
            this.IsPicAttached = false;

            SetPostButtonState();
        }

        private void HandlePostAsImgPopClosed(object sender, EventArgs e)
        {
            SetPostButtonState();

            if(!string.IsNullOrEmpty(this.AttachedPicLocation))
                this.IsPicAttached = true;
        }

        #endregion

        #region Private Message

        private void SetItemState(FrameworkElement target, bool val)
        {
            this.Dispatcher.Invoke(new Action(delegate()
            {
                target.IsEnabled = val;
            }));
        }

        private void AddStatusToView(StatusInfo status)
        {
            this.Dispatcher.Invoke(new Action<StatusInfo>(delegate(StatusInfo data)
            {
                ViewModel.MyHomeStatuses.Insert(0, DataConverter.ConvertFrom(data));
                ViewModel.MyStatuses.Insert(0, DataConverter.ConvertFrom(data));

            }), status);
        }

        private void SetPostButtonState()
        {
            if (!IsPostAsPic)
                btnPost.IsEnabled = !string.IsNullOrEmpty(txbStatus.Text.Trim());
            else
                btnPost.IsEnabled = false;
        }

        private int GetAvailableStatusText()
        {
            var text = txbStatus.Text.Trim();

            var length = 0;
            if (!string.IsNullOrEmpty(text))
            {
                var data = Encoding.GetEncoding("gb2312").GetBytes(text);
                length = data.Length;
            }

            return 280 - length;
        }

        private void ShowMessage(string msg, params object[] parameters)
        {
            ViewModel.ShowMessage(msg, true, parameters);
        }

        private void ShowMessage(string msg, bool isSuccess, params object[] parameters)
        {
            ViewModel.ShowMessage(msg, isSuccess, parameters);
        }

        private void ClearMessage()
        {
            ShowMessage("Ready");
        }

        #endregion
    }

    public enum PosterMode
    { 
        Standard,
        Forward,
        Comment,
        ReplyComment,
        Repost
    }

    public class PostingEventArgs : EventArgs
    {
        public bool IsContinue { get; set; }

        public string PostingText { get; set; }
    }
}
