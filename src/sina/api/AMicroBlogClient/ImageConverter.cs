using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace AMicroblogAPISample
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value || string.IsNullOrEmpty(value.ToString()) || value.ToString() == "EmptyPic")
            {
                return new BitmapImage();
            }
            else
            {
                ImageSource source = new BitmapImage(new Uri(value.ToString()));
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
