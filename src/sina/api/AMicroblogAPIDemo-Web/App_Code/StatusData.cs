using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMicroblogAPI.DataContract;

/// <summary>
///StatusData 的摘要说明
/// </summary>
public class StatusData : StatusInfo
{
	public StatusData()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public string DisplayPic
    {
        get
        {
            return string.IsNullOrEmpty(base.ThumbnailPic) ? "none" : "inherit";
        }
    }

    public string DisplayRetweetedStatus
    {
        get
        {
            return null == base.RetweetedStatus ? "none" : "inherit";
        }
    }

    public string DisplayRetweetedStatusPic
    {
        get
        {
            return null != base.RetweetedStatus && !string.IsNullOrEmpty(base.RetweetedStatus.ThumbnailPic) ? "inherit" : "none";
        }
    }

    public string DisplayUserFollow
    {
        get
        {
            return base.User.Following ? "inherit" : "none";
        }
    }

    public string DisplayUserUnfollow
    {
        get
        {
            return base.User.Following ? "none" : "inherit";
        }
    }
}