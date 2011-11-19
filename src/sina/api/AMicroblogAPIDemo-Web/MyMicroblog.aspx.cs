using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AMicroblogAPI.Common;
using AMicroblogAPI;
using System.Collections.ObjectModel;

public partial class MyMicroblog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (null != AMicroblogAPI.Environment.AccessToken)
            {
                var myStatuses = AMicroblog.GetUserStatuses();

                var statuses = new Collection<StatusData>();
                foreach (var item in myStatuses.Items)
                {
                    statuses.Add(DataConverter.ConvertFrom(item));
                }

                dlStatus.DataSource = statuses;
                dlStatus.DataBind();
            }
        }
    }

    protected void HandleDelete(object sender, EventArgs e)
    {
        if (null != AMicroblogAPI.Environment.AccessToken)
        {
            var target = sender as LinkButton;
            var statusId = target.CommandArgument;
            var statusIdLong = long.Parse(statusId);

            AMicroblog.DeleteStatus(statusIdLong);
        }

        Response.Redirect(Request.Path);
    }
}