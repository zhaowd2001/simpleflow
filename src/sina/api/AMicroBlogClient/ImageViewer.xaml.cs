using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace AMicroblogAPISample
{
    /// <summary>
    /// ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewer : WindowBase
    {
        private static ObservableCollection<string> ImagesViewed = new ObservableCollection<string>();

        private double? RequiredLocationX { get; set; }
        private double? RequiredLocationY { get; set; }

        public static bool IsShowing = false;
        public static readonly ImageViewer Instance = new ImageViewer();

        private ImageViewer()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(HandleImageViewerLoaded);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            IsShowing = false;
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        private void HandleImageViewerLoaded(object sender, RoutedEventArgs e)
        {
            if (RequiredLocationX.HasValue)
                this.Left = RequiredLocationX.Value;

            if (RequiredLocationY.HasValue)
                this.Top = RequiredLocationY.Value;            
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

        private void HandleNavPrivious(object sender, RoutedEventArgs e)
        {
            var idx = ImagesViewed.IndexOf(imgLoader.Source);
            if (idx > 0)
            {
                imgLoader.Source = ImagesViewed[idx - 1];
            }
        }

        private void HandleNavNext(object sender, RoutedEventArgs e)
        {
            var idx = ImagesViewed.IndexOf(imgLoader.Source);
            if (idx < ImagesViewed.Count - 1)
            {
                imgLoader.Source = ImagesViewed[idx + 1];
            }
        }

        public static void ShowImage(StatusData statusData, bool isShowForwardedPic)
        {
            var mainWin = Application.Current.MainWindow;
            var imgViewer = ImageViewer.Instance;

            if (!IsShowing)
            {
                imgViewer.Owner = mainWin;
                imgViewer.RequiredLocationX = mainWin.Left + mainWin.ActualWidth + 2;
                imgViewer.RequiredLocationY = mainWin.Top + 2;

                mainWin.LocationChanged += imgViewer.HandleMainWinLocationChanged;
            }

            var picToShow = string.Empty;

            if (isShowForwardedPic)
                picToShow = statusData.RetweetedStatus.OriginalPic;
            else
                picToShow = statusData.OriginalPic;

            if (!ImageHelper.ExistsImage(picToShow))
                imgViewer.imgLoader.Source = picToShow;
            else
                imgViewer.imgLoader.Source = ImageHelper.GetImage(picToShow);

            if (!ImagesViewed.Contains(imgViewer.imgLoader.Source))
                ImagesViewed.Add(imgViewer.imgLoader.Source);

            ImageViewer.Instance.Show();
            ImageViewer.Instance.Activate();

            IsShowing = true;
        }

        private void HandleMainWinLocationChanged(object sender, EventArgs e)
        {
            var mainWin = Application.Current.MainWindow;

            this.RequiredLocationX = mainWin.Left + mainWin.ActualWidth + 2;
            this.RequiredLocationY = mainWin.Top + 2;

            if (RequiredLocationX.HasValue)
                this.Left = RequiredLocationX.Value;

            if (RequiredLocationY.HasValue)
                this.Top = RequiredLocationY.Value;   
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                HandleNavPrivious(this, null);
            }
            else if (e.Key == Key.Right)
            {
                HandleNavNext(this, null);
            }
        }

        private void HandleSaveAs(object sender, RoutedEventArgs e)
        {
            var source = imgLoader.CurrentImageSource;
            if (null != source && !source.IsDownloading)
            {
                var targetFileLocation = ImageHelper.Save(source);

                var dialog = new ConfirmationDialog();
                dialog.Owner = this;
                dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                dialog.Message = "Image saved to: " + targetFileLocation + ". Open it now?";
                dialog.ConfirmButtonText = "Open";
                var diaResult = dialog.ShowDialog();
                if (diaResult.HasValue && diaResult.Value)
                {
                    Process.Start(targetFileLocation);
                }
            }
        }
    }
}
