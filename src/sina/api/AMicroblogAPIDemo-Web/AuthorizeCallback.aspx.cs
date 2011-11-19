using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AMicroblogAPI.Common;

public partial class AuthorizeCallback : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var verifer = Request.QueryString["oauth_verifier"];

        var reqToken = Session["RequestToken"] as OAuthRequestToken;
        var accessToken = AMicroblogAPI.AMicroblog.GetAccessToken(reqToken, verifer);

        Session["AccessToken"] = accessToken;

        if (null != Session["SaveCredential"])
        {
            var userName = "anyone";
            CredentialHelper.Save(userName, accessToken);
        }

        Response.Redirect("Default.aspx");
    }
}