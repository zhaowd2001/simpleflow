using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMicroblogAPI.DataContract;
using System.Text.RegularExpressions;

/// <summary>
///DataConverter 的摘要说明
/// </summary>
public static class DataConverter
{
    public static StatusData ConvertFrom(StatusInfo statusInfo)
    {
        var data = new StatusData();
        data.CreatedAt = statusInfo.CreatedAt;
        data.ID = statusInfo.ID;
        data.Text = ProcessStatusText(statusInfo.Text);
        data.Source = statusInfo.Source;
        data.Favorited = statusInfo.Favorited;
        data.Truncated = statusInfo.Truncated;
        data.ReplyTo = statusInfo.ReplyTo;
        data.ReplyToUserId = statusInfo.ReplyToUserId;
        data.ReplyToUserScreenName = statusInfo.ReplyToUserScreenName;
        data.ThumbnailPic = statusInfo.ThumbnailPic;
        data.MiddlePic = statusInfo.MiddlePic;
        data.OriginalPic = statusInfo.OriginalPic;
        data.Mid = statusInfo.Mid;
        if (null != statusInfo.User)
            data.User = ConvertFrom(statusInfo.User);
        data.Geo = statusInfo.Geo;

        if (null != statusInfo.RetweetedStatus)
            data.RetweetedStatus = ConvertFrom(statusInfo.RetweetedStatus);

        return data;
    }

    public static UserData ConvertFrom(UserInfo userInfo)
    {
        var data = new UserData();
        data.ID = userInfo.ID;
        data.ScreenName = userInfo.ScreenName;
        data.Name = userInfo.Name;
        data.DefineAs = userInfo.DefineAs;
        data.Province = userInfo.Province;
        data.City = userInfo.City;
        data.Location = userInfo.Location;
        data.Description = userInfo.Description;
        data.Url = userInfo.Url;
        data.ProfileImageUrl = userInfo.ProfileImageUrl;
        data.Domain = userInfo.Domain;
        data.Gender = userInfo.Gender;
        data.FollowersCount = userInfo.FollowersCount;
        data.FriendsCount = userInfo.FriendsCount;
        data.StatusesCount = userInfo.StatusesCount;
        data.FavouritesCount = userInfo.FavouritesCount;
        data.CreatedAt = userInfo.CreatedAt;
        data.GeoEnabled = userInfo.GeoEnabled;
        data.AllowAllActMsg = userInfo.AllowAllActMsg;
        data.Following = userInfo.Following;
        data.Verified = userInfo.Verified;

        if (null != userInfo.LatestStatus)
            data.LatestStatus = ConvertFrom(userInfo.LatestStatus);

        return data;
    }

    private static string ProcessStatusText(string text)
    {
        var result = text;

        result = Regex.Replace(text, @"(http://([^\s]+)\b)", "<a target=\"_blank\" href=\"$1\">$1</a>", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"#(.+?)#", "<a target=\"_blank\" href=\"SearchTopic.aspx?topic=$1\">#$1#</a>", RegexOptions.IgnoreCase);

        result = Regex.Replace(result, @"@([^\s@]+)\b", "<a target=\"_blank\" href=\"SearchPeople.aspx?name=$1\">@$1</a>", RegexOptions.IgnoreCase);

        return result;
    }
}