<% @ page language="VB"  debug="true"%>
<% @ Import namespace="System" %>
<% @ Import namespace="System.IO" %>
<% @ Import namespace="System.Object" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>文件列表</title>


<style type="text/css">
<!--
.STYLE1 {font-size: 12px;
color:#7E7E7E;
line-height:20px;
}
a:link {
	color: #000000;
	text-decoration: none;
}
a:visited {
	text-decoration: none;
	color: #000000;
}
a:hover {
	text-decoration: underline;
	color: #000000;
}
a:active {
	text-decoration: none;
	color: #000000;
}
-->
</style>
</head>
<body onLoad="winSizer();">
    <form id="form1" runat="server" action="FileList.aspx">
        <div>
<!--        <table width="400" border="0" cellspacing="0" cellpadding="0">
  <tr>
    <td><input type="hidden" name="Action" value="ok" /><input type="submit" name="Del" value="删除三天前的数据" /></td>
  </tr>
</table>-->

            <%

Dim ls_Path  as string
Dim ls_FileNames as string()
Dim ls_split as String()
Dim ls_dir as String
Dim i as Long
Dim ls_Fpath as string
Dim ls_action As String

if request("action") ="ok" then
 Call Kill(Server.MapPath("~/Bigupload/" & request("FileName")))
response.Redirect("FileList.aspx")
end if


ls_Path = Server.MapPath("~/Bigupload")
ls_FileNames = Directory.GetFiles(ls_Path,"*.*")

'if request.Form("action") ="ok" then
'call tf_KillFile
'end if
response.write("<table width='400px' border=0 cellspacing=10 cellpadding=0 class=STYLE1>")
for i=0 to ls_FileNames.Length -1
	ls_split = split ( ls_FileNames(i), "Bigupload\" )
	ls_dir = ls_split(1)

	ls_fpath="http://" & System.Web.HttpContext.Current.Request.ServerVariables("HTTP_HOST")
	ls_fpath=ls_fpath & Component.Web.tf_GetAppSetting("MyAppPath") & "Bigupload/" &  ls_dir
    response.write("<tr><td width=""80%""><a href=""" & ls_fpath & """>" & ls_dir & " ( " & format(File.GetLastWriteTime(ls_FileNames(i)),"yyyy-MM-dd hh:mm")  &" )"& "</a></td>" & vbcrlf)
    response.write("<td><input type=button name=Submit value=复制链接 onclick=""window.clipboardData.setData('text','" & ls_fpath & "');alert('链接已复制到剪贴板！')"" /></td>" & vbcrlf)
    response.write("<td><input type=button name=Submit value=删除 onclick=""javascript:if (true == window.confirm('确定要删除 " & ls_dir & " 吗?')){ location.href = 'FileList.aspx?action=ok&FileName=" & Server.UrlEncode(ls_dir) & "';  }"" /></td>" & vbcrlf)
    response.write("</tr>" & vbcrlf & vbcrlf)

next
response.write("</table>")
%>
        </div>
    </form>
</body>
</html>
<SCRIPT LANGUAGE="javascript">
function winSizer(){

 window.resizeTo(520,540);
 xoffset=50;   
  yoffset=50;   
  window.moveTo(xoffset,yoffset);  
  }

</SCRIPT>
<script language="vb" runat="server">
Public sub  tf_KillFile ()
    Dim i as Long
    'Dim ls_Temp As String
    Dim ls_Date As String
    Dim ls_Path  as string
    Dim ls_FileNames as string()
    'Dim ls_split as String() 
    'Dim ls_dir as String

    ls_date=format(Now(),"yyyy-MM-dd hh:mm")
    ls_Path = Server.MapPath("~/Bigupload")
    ls_FileNames = Directory.GetFiles(ls_Path,"*.*")

    for i=0 to ls_FileNames.Length -1

        If DateDiff(DateInterval.Day,cdate(format(File.GetLastWriteTime(ls_FileNames(i)),"yyyy-MM-dd hh:mm")),cdate(ls_date)) >3 Then
            Call Kill(ls_FileNames(i))
        End If
        'MsgBox(DateDiff(DateInterval.Day,cdate(format(File.GetLastWriteTime(ls_FileNames(i)),"yyyy-MM-dd hh:mm")),cdate(ls_date)))
    Next

    'MsgBox("三天前的数据已删除")

    response.Redirect("FileList.aspx")
End Sub
	
	
</script>

