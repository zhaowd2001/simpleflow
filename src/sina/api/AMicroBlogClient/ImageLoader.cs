using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AMicroblogAPISample
{
    /// <summary>
    /// Represents a cache-enabled remote image loader.
    /// </summary>
    public class ImageLoader : StackPanel
    {
        private static Dictionary<string, BitmapImage> cachedImaged = new Dictionary<string, BitmapImage>();

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImageLoader), new UIPropertyMetadata(Stretch.None));

        public double? PWidth
        {
            get { return (double?)GetValue(PWidthProperty); }
            set { SetValue(PWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PWidthProperty =
            DependencyProperty.Register("PWidth", typeof(double?), typeof(ImageLoader), new UIPropertyMetadata(null));

        public double? PHeight
        {
            get { return (double?)GetValue(PHeightProperty); }
            set { SetValue(PHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PHeightProperty =
            DependencyProperty.Register("PHeight", typeof(double?), typeof(ImageLoader), new UIPropertyMetadata(null));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(ImageLoader), new UIPropertyMetadata(new PropertyChangedCallback(HandleImageUriChanged)));

        private static void HandleImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var loader = d as ImageLoader;

            loader.PerformImageLoad();
        }


        public BitmapImage CurrentImageSource
        {
            get { return (BitmapImage)GetValue(CurrentImageSourceProperty); }
            set { SetValue(CurrentImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentImageSourceProperty =
            DependencyProperty.Register("CurrentImageSource", typeof(BitmapImage), typeof(ImageLoader), new UIPropertyMetadata(null));


        private void PerformImageLoad()
        {
            var newUri = Source;
            if (null != newUri && !string.IsNullOrEmpty(newUri.ToString()))
            {
                this.Children.Clear();
                BitmapImage source = null;
                // If an image is previously loaded, get it from the cache.
                if (!cachedImaged.ContainsKey(newUri))
                {
                    source = new BitmapImage(new Uri(newUri));
                    source.CacheOption = BitmapCacheOption.OnDemand;
                    source.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                    cachedImaged[newUri] = source;
                }
                else
                    source = cachedImaged[newUri];

                CurrentImageSource = source;

                var img = new Image() { Source = source, Stretch = Stretch };
                if (PWidth.HasValue)
                    img.Width = PWidth.Value;
                if (PHeight.HasValue)
                    img.Height = PHeight.Value;

                this.Children.Add(img);

                if (source.IsDownloading)
                {
                    var textBlock = new TextBlock();
                    textBlock.FontSize = 9;
                    textBlock.Text = "Loading...";
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;

                    source.DownloadCompleted += HandleSourceDownloadCompleted;

                    this.Children.Add(textBlock);
                }
            }
            else
            {
                this.Children.Clear();
            }
        }

        private void HandleSourceDownloadCompleted(object sender, EventArgs e)
        {
            foreach (var child in this.Children)
            {
                var txtBlock = child as TextBlock;
                if (null != txtBlock)
                    txtBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
