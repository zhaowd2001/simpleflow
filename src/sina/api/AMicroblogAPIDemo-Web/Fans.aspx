<%@ Page Title="" Language="C#" MasterPageFile="~/Skeleton.master" AutoEventWireup="true" CodeFile="Fans.aspx.cs" Inherits="Fans" %>

<asp:Content ID="Content2" ContentPlaceHolderID="content" Runat="Server">
    <p>
    <asp:DataList ID="dlUsers" runat="server">
            <ItemTemplate>
                <table style="width: 100%;">
                    <tr>
                        <td rowspan="2" style="width: 60px; vertical-align: top">
                            <div class="userNameStyle">
                                <image class="userIconStyle" src="<%# Eval("ProfileImageUrl") %>"></image>
                                <div class="followStyle">
                                    <div class="followBoxStyle">
                                        <b><%# Eval("ScreenName")%></b> (<%# Eval("Location")%>)

                                        <div style="margin-bottom:4px">Fans:<b><%# Eval("FollowersCount")%></b>&nbsp;Friends:<b><%# Eval("FriendsCount")%></b>&nbsp;Statuses:<b><%# Eval("StatusesCount")%></b></div>

                                        <div class="userDescStyle"><%# Eval("Description")%></div>

                                        <div style="position:absolute; bottom: 4px; left: 4px;">
                                            <span>
                                                <asp:LinkButton ID="LinkButton2" runat="server" Text="Follow" CommandArgument='<%# Eval("ID")%>' OnClick="HandleFollow" />
                                            </span>
                                            <span>
                                                <asp:LinkButton ID="LinkButton3" runat="server" Text="Unfollow" CommandArgument='<%# Eval("ID")%>' OnClick="HandleUnfollow" />
                                            </span>
                                       </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td>
                            <a href="#"><b>
                                <%# Eval("ScreenName") %></b></a>: &nbsp;(<%# Eval("Location") %>)<br />
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                           <%# Eval("Description") %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            
                        </td>
                    </tr>
                </table>

                <%--Separator--%>
                <div style="border-bottom: 1px dashed green">
                </div>
            </ItemTemplate>
        </asp:DataList>
</p>
</asp:Content>

