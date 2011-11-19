using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AMicroblogAPI;
using System.Collections.ObjectModel;

public partial class Friends : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (null != AMicroblogAPI.Environment.AccessToken)
            {
                var followers = AMicroblog.GetFriends(long.Parse(AMicroblogAPI.Environment.AccessToken.UserID));

                var users = new Collection<UserData>();
                foreach (var item in followers.Items)
                {
                    users.Add(DataConverter.ConvertFrom(item));
                }

                dlUsers.DataSource = users;
                dlUsers.DataBind();
            }
        }
    }

    protected void HandleUnfollow(object sender, EventArgs e)
    {
        if (null != AMicroblogAPI.Environment.AccessToken)
        {
            var target = sender as LinkButton;
            var userId = target.CommandArgument;
            var userIDLong = long.Parse(userId);
            if (AMicroblog.ExistsFriendship(long.Parse(AMicroblogAPI.Environment.AccessToken.UserID), userIDLong))
            {
                AMicroblog.Unfollow(userIDLong);
            }
        }

        Response.Redirect(Request.Path);
    }
}