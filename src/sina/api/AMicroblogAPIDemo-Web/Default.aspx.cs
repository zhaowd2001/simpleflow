using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AMicroblogAPI;
using AMicroblogAPI.Common;
using System.Collections.ObjectModel;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (null != AMicroblogAPI.Environment.AccessToken)
            {
                try
                {
                    var homeStatuses = AMicroblog.GetFriendsStatuses();

                    var statuses = new Collection<StatusData>();
                    foreach (var item in homeStatuses.Items)
                    {
                        statuses.Add(DataConverter.ConvertFrom(item));
                    }

                    dlHomeStatus.DataSource = statuses;
                    dlHomeStatus.DataBind();
                }
                catch (AMicroblogException ex)
                {
                    Session.Clear();
                    AMicroblogAPI.Environment.AccessToken = null;
                    CredentialHelper.DeleteCachedToken();
                    Response.Redirect("Default.aspx");
                    Response.End();
                }
            }
        }
    }

    protected void HandleFollow(object sender, EventArgs e)
    {
        if (null != AMicroblogAPI.Environment.AccessToken)
        {
            var target = sender as LinkButton;
            var userId = target.CommandArgument;
            var userIDLong = long.Parse(userId);
            if (!AMicroblog.ExistsFriendship(long.Parse(AMicroblogAPI.Environment.AccessToken.UserID), userIDLong))
            {
                AMicroblog.Follow(userIDLong);
            }
        }

        Response.Redirect(Request.Path);
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