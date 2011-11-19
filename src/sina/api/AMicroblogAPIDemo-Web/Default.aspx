<%@ Page Title="" Language="C#" MasterPageFile="~/Skeleton.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ContentPlaceHolderID="content" runat="Server">
    <div>
        <asp:DataList ID="dlHomeStatus" runat="server" EnableViewState="true">
            <ItemTemplate>
                <table style="width: 100%;">
                    <tr>
                        <td rowspan="2" style="width: 60px; vertical-align: top">
                            <div class="userNameStyle">
                                <image class="userIconStyle" src="<%# Eval("User.ProfileImageUrl") %>"></image>
                                <div class="followStyle">
                                    <div class="followBoxStyle">
                                        <b><%# Eval("User.ScreenName")%></b> (<%# Eval("User.Location")%>)

                                        <div style="margin-bottom:4px">Fans:<b><%# Eval("User.FollowersCount")%></b>&nbsp;Friends:<b><%# Eval("User.FriendsCount")%></b>&nbsp;Statuses:<b><%# Eval("User.StatusesCount")%></b></div>

                                        <div class="userDescStyle"><%# Eval("User.Description")%></div>

                                        <div style="position:absolute; bottom: 4px; left: 4px;">
                                            <span style="display:<%# Eval("DisplayUserFollow")%>">
                                                <asp:LinkButton ID="LinkButton2" runat="server" Text="Follow" CommandArgument='<%# Eval("User.ID")%>' OnClick="HandleFollow" />
                                            </span>
                                            <span style="display:<%# Eval("DisplayUserUnfollow")%>">
                                                <asp:LinkButton ID="LinkButton3" runat="server" Text="Unfollow" CommandArgument='<%# Eval("User.ID")%>' OnClick="HandleUnfollow" />
                                            </span>
                                       </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td>
                            <div class="userNameStyle">
                                <a href="#">
                                    <b>
                                        <%# Eval("User.ScreenName") %>
                                    </b>
                                 </a>
                             </div>
                             :&nbsp;<%# Eval("Text") %><br />

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
                            &nbsp;
                        </td>
                        <td>
                            <div>
                                From:&nbsp;<a href="<%# Eval("Source.Content.Uri")%>"><%# Eval("Source.Content.Text")%></a>&nbsp;
                                At:&nbsp;
                                <%# Eval("CreatedAt")%>
                            </div>
                        </td>
                    </tr>
                </table>

                <%--Separator--%>
                <div style="border-bottom: 1px dashed green">
                </div>
            </ItemTemplate>
        </asp:DataList>
        <p>
            
            <br />
        </p>
    </div>
</asp:Content>
