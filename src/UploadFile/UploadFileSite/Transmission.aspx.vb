
Partial Class Forms_LsControl_UploadFile_Transmission
    Inherits Component.Web.uWebFormBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim l_Transmission As UploadFile.Transmission


        'Dim ls_Temp As String

        Server.ScriptTimeout = 9999

        'l_Transmission = New UploadFile.Transmission

        'ls_GetFileType = Request.QueryString("filefield")
        'ls_ProgressID = Request.QueryString("ProgressID")

        'ls_GetFileType = LCase(Right(ls_GetFileType, 3))
        'If ls_GetFileType <> "zip" And ls_GetFileType <> "rar" Then
        '    Call Response.Write("只能上传ZIP、RAR压缩文件")
        '    Response.End()
        'End If

        'l_Transmission.SaveTo(Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings("MyAppPath") & "\UploadFiles\")) '上传演示，不保存到硬盘
        'ls_Temp = l_Transmission.Upload(ls_ProgressID)

        'If ls_Temp <> "" Then
        '    Call Response.Write(ls_Temp)
        '    Call Response.End()
        'Else
        Call Response.Write("<p class =STYLE1> 文件名: " & UploadFile.UploadContext.UpFileName & "<br>")
        Call Response.Write("文件大小: " & UploadFile.UploadContext.FileSize & " KB<br>")
        Call Response.Write("文件类型: " & UploadFile.UploadContext.FileType & "<br>")
        Call Response.Write("客户端路径: " & UploadFile.UploadContext.FilePath & "<br>")
        Call Response.Write("<br>")
        Call Response.Write("<br></p>")
        'End If
    End Sub
End Class
