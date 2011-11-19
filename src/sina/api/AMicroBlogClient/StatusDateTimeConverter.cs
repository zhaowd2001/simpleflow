using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Globalization;

namespace AMicroblogAPISample
{
    public class StatusDateTimeConverter : IValueConverter
    {
        private const string Pattern = @"^(\w+?)\s(\w+?)\s(\d+?)\s(.+?)\s(.+?)\s(\d+?)$";
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return string.Empty;

            DateTime dateTime = new DateTime();
            var nowDateTime = DateTime.Now;
            var dateTimeStr = value.ToString();
            var parsedTimeStr = string.Empty;

            var match = Regex.Match(dateTimeStr, Pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                parsedTimeStr = match.Groups[4].Value;
                var parsedTime = TimeSpan.Parse(parsedTimeStr, CultureInfo.InvariantCulture);
                var year = int.Parse(match.Groups[6].Value);
                var monthStr = match.Groups[2].Value;
                var day = int.Parse(match.Groups[3].Value);
                dateTime = new DateTime(year, GetMonth(monthStr), day);
                dateTime = dateTime.Add(parsedTime);
            }

            var format = "{0} {1}";

            var datePart = string.Empty;
            var timePart = string.Empty;
            if (dateTime.Date == nowDateTime.Date)
            {
                datePart = "Today";

                var timespan = nowDateTime.Subtract(dateTime);
                if (timespan.Hours != 0)
                    timePart = string.Format("{0} hours ago",timespan.Hours);
                else if (timespan.Minutes != 0)
                    timePart = string.Format("{0} minutes ago", timespan.Minutes);
                else if (timespan.Seconds != 0)
                    timePart = string.Format("{0} seconds ago", timespan.Seconds);
                else
                    timePart = parsedTimeStr;

            }
            else if (nowDateTime.Date.Subtract(dateTime.Date).Days == 1)
            {
                datePart = "Yesterday";
                timePart = parsedTimeStr;
            }
            else
            {
                datePart = dateTime.ToShortDateString();
                timePart = parsedTimeStr;
            }

            return string.Format(format, datePart, timePart);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private int GetMonth(string monthStr)
        {
            switch (monthStr)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    return 1;
            }
        }
    }
}
