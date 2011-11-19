using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMicroblogAPI.DataContract;

/// <summary>
///UserData 的摘要说明
/// </summary>
public class UserData : UserInfo
{
	public UserData()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public string IsFollowing_Display
    {
        get
        {
            return base.Following ? "inherit" : "none";
        }
    }

    public string IsNotFollowing_Display
    {
        get
        {
            return base.Following ? "none" : "inherit";
        }
    }
}