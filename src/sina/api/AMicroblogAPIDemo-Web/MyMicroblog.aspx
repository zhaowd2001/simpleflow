<%@ Page Title="" Language="C#" MasterPageFile="~/Skeleton.master" AutoEventWireup="true"
    CodeFile="MyMicroblog.aspx.cs" Inherits="MyMicroblog" %>

<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <asp:DataList ID="dlStatus" runat="server">
        <ItemTemplate>
            <table style="width: 100%;">
                <tr>
                    <td>
                        <a href="#"><b>
                            <%# Eval("User.ScreenName") %></b></a>: &nbsp;<%# Eval("Text") %><br />

                        <!--Pic viewer-->
                        <div id="pic<%# Eval("ID") %>" style="display: <%# Eval("DisplayPic") %>">
                            <div onclick="ShowPic('pic<%# Eval("ID") %>', '<%# Eval("MiddlePic") %>')">
                                <image class="picStyle" style="cursor: pointer;" src="<%# Eval("ThumbnailPic") %>" />
                            </div>
                            <div style="display: none" class="picViewerStyle">
                                <div>
                                    &nbsp;<span onclick="HidePic('pic<%# Eval("ID") %>')" style="text-decoration: underline;
                                        color: Green;">Hide Pic</span> &nbsp; <a href="<%# Eval("OriginalPic") %>" target="_blank">
                                            Orig Pic</a></div>
                                <image class="picStyle" onclick="HidePic('pic<%# Eval("ID") %>')" />
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="retweetedStatusStyle" style="display: <%# Eval("DisplayRetweetedStatus") %>">
                            <b>@<%# Eval("RetweetedStatus.User.ScreenName")%>:</b>
                            <%# Eval("RetweetedStatus.Text")%>

                            <!--retweetedPic viewer-->
                            <div id="retweetedPic<%# Eval("RetweetedStatus.ID") %>" style="display: <%# Eval("DisplayRetweetedStatusPic") %>">
                                <div onclick="ShowPic('retweetedPic<%# Eval("RetweetedStatus.ID") %>', '<%# Eval("RetweetedStatus.MiddlePic") %>')">
                                    <image class="picStyle" style="cursor: pointer;" src="<%# Eval("RetweetedStatus.ThumbnailPic") %>" />
                                </div>
                                <div style="display: none" class="picViewerStyle">
                                    <div>
                                        &nbsp;<span onclick="HidePic('retweetedPic<%# Eval("RetweetedStatus.ID") %>')" style="text-decoration: underline;
                                            color: Green;">Hide Pic</span> &nbsp; <a href="<%# Eval("RetweetedStatus.OriginalPic") %>"
                                                target="_blank">Orig Pic</a></div>
                                    <image class="picStyle" onclick="HidePic('retweetedPic<%# Eval("RetweetedStatus.ID") %>')" />
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div>
                            From:&nbsp;<a href="<%# Eval("Source.Content.Uri")%>"><%# Eval("Source.Content.Text")%></a>&nbsp;
                            At:&nbsp;
                            <%# Eval("CreatedAt")%>
                            <div style="float: right; right: 4px;">
                                <asp:LinkButton runat="server" Text="Delete" CommandArgument='<%# Eval("ID")%>' OnClick="HandleDelete" />
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
            <%--Separator--%>
            <div style="border-bottom: 1px dashed green">
            </div>
        </ItemTemplate>
    </asp:DataList>
</asp:Content>
