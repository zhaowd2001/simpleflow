using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AMicroblogAPI;
using AMicroblogAPI.Common;
using AMicroblogAPI.DataContract;
using System.Configuration;

public partial class Skeleton : System.Web.UI.MasterPage
{
    protected override void OnInit(EventArgs e)
    {
        var accessToken = Session["AccessToken"] as OAuthAccessToken;
        if (null != accessToken)
        {
            AMicroblogAPI.Environment.AccessToken = accessToken;
        }
        else
        {
            var lastUserToken = CredentialHelper.Get();
            if (null != lastUserToken)
            {
                Session["AccessToken"] = lastUserToken;
                AMicroblogAPI.Environment.AccessToken = lastUserToken;
            }
        }

        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (null != AMicroblogAPI.Environment.AccessToken)
            {
                divMsg.Visible = false;

                var userInfo = AMicroblog.VerifyCredential();

                lbStatusCounter.Text = userInfo.StatusesCount.ToString();
                lbFriendsCounter.Text = userInfo.FriendsCount.ToString();
                lbFansCounter.Text = userInfo.FollowersCount.ToString();

                lbUserName.Text = userInfo.ScreenName;
            }
            else
                tbContent.Visible = false;
        }
    }

    protected void HandleLogout(object sender, EventArgs e)
    {
        if (null != AMicroblogAPI.Environment.AccessToken)
        {
            AMicroblog.EndSession();
        }

        Session.Clear();
        AMicroblogAPI.Environment.AccessToken = null;

        Response.Redirect("Default.aspx");
    }

    protected void HandleLogonClicked(object sender, EventArgs e)
    {
        var requestToken = AMicroblog.GetRequestToken();

        Session["RequestToken"] = requestToken;

        if(saveCredential.Checked)
            Session["SaveCredential"] = saveCredential.Checked;

        //TODO: Gets the baseSiteUri.

        var callback = ConfigurationManager.AppSettings["callbackUrl"];
        var callbackUrl = HttpUtility.UrlEncode(callback);

        Response.Redirect(string.Format("{0}?oauth_token={1}&oauth_callback={2}", APIUri.Authorize, requestToken.Token, callbackUrl));
    }

    protected void HandlePostClicked(object sender, EventArgs e)
    {
        var status = txbStatus.Text;

        var statusInfo = new UpdateStatusInfo();
        statusInfo.Status = status;
        AMicroblog.PostStatus(statusInfo);

        Response.Redirect(Request.Path);
    }

    protected void HandleDeleteCachedCredential(object sender, EventArgs e)
    {
        if (null != AMicroblogAPI.Environment.AccessToken)
        {
            AMicroblog.EndSession();
        }

        Session.Clear();
        AMicroblogAPI.Environment.AccessToken = null;
        CredentialHelper.DeleteCachedToken();

        Response.Redirect("Default.aspx");
    }
}
