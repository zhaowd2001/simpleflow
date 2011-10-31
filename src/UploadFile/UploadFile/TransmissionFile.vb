

'---------------------------------------------------
' 文件信息 类
'---------------------------------------------------
Public Class TransmissionFile

    Private os_FormName As String
    Private os_FileName As String
    Private os_FilePath As String
    Private ol_FileSize As Long
    Private os_FileType As String
    Private os_FileExt As String
    Private os_NewFileName As String

    Public Property NewFileName() As String
        Get
            Return os_NewFileName
        End Get
        Set(ByVal value As String)
            os_NewFileName = value
        End Set
    End Property

    Public Property FileExt() As String
        Get
            Return os_FileExt
        End Get
        Set(ByVal value As String)
            os_FileExt = value
        End Set
    End Property

    Public Property FileType() As String
        Get
            Return os_FileType
        End Get
        Set(ByVal value As String)
            os_FileType = value
        End Set
    End Property

    Public Property FileSize() As Long
        Get
            Return ol_FileSize
        End Get
        Set(ByVal value As Long)
            ol_FileSize = value
        End Set
    End Property

    Public Property FilePath() As String
        Get
            Return os_FilePath
        End Get
        Set(ByVal value As String)
            os_FilePath = value
        End Set
    End Property

    Public Property FileName() As String
        Get
            Return os_FileName
        End Get
        Set(ByVal value As String)
            os_FileName = value
        End Set
    End Property

    Public Property FormName() As String
        Get
            Return os_FormName
        End Get
        Set(ByVal value As String)
            os_FormName = value
        End Set
    End Property

    Public Sub New()
        FileName = ""       ' 文件名
        FilePath = ""           ' 客户端路径
        FileSize = 0            ' 文件大小
        FormName = ""   ' 表单名
        FileType = ""       ' 文件Content Type
        FileExt = ""            ' 文件扩展名
        NewFileName = ""    '上传后文件名
    End Sub

    'Public Sub Save(ByRef FileBinary As Byte())
    '    SaveAs(FileName, DoteyUpload_SourceData)
    'End Sub

    '' 保存文件
    'Public Sub SaveAs(ByVal fullpath, ByRef DoteyUpload_SourceData)
    '    Dim dr

    '    'SaveAs = False
    '    If Trim(fullpath) = "" Or FileStart = 0 Or FileName = "" Or Right(fullpath, 1) = "/" Then
    '        Exit Sub
    '    End If

    '    NewFileName = GetFileNameByPath(fullpath)
    '    dr = CreateObject("Adodb.Stream")
    '    dr.Mode = 3
    '    dr.Type = 1
    '    dr.Open()
    '    DoteyUpload_SourceData.position = FileStart
    '    DoteyUpload_SourceData.copyto(dr, FileSize)
    '    dr.SaveToFile(MapPath(fullpath), 2)
    '    dr.Close()
    '    dr = Nothing
    'End Sub

    ' 取服务器端路径
    Private Function MapPath(ByVal Path As String) As String
        If InStr(1, Path, ":") > 0 Or Left(Path, 2) = "\\" Then
            MapPath = Path
        Else
            MapPath = System.Web.HttpContext.Current.Server.MapPath(Path)
        End If
    End Function

    Public Sub FillFileInfo(ByVal as_Header As String, ByVal al_FileSize As Long)
        ' 获取文件相关信息
        Dim ls_ClientPath As String

        ls_ClientPath = GetFileName(as_Header)
        Me.FileName = GetFileNameByPath(ls_ClientPath)
        Me.FileExt = GetFileExt(ls_ClientPath)
        Me.FilePath = ls_ClientPath
        Me.FileType = GetFileType(as_Header)
        Me.FileSize = al_FileSize
        Me.FormName = GetFieldName(as_Header)

        '保存到 UploadContext 中
        UploadContext.UpFileName = FileName
        UploadContext.FileSize = Component.tf_保留两位小数(FileSize / 1024)
        UploadContext.FileType = FileType
        UploadContext.FilePath = FilePath
    End Sub

    '返回表单名
    Private Function GetFieldName(ByVal infoStr As String) As String
        Dim sPos As Integer
        Dim EndPos As Integer

        sPos = InStr(infoStr, "name=")
        EndPos = InStr(sPos + 6, infoStr, Chr(34) & ";")
        If EndPos = 0 Then
            EndPos = InStr(sPos + 6, infoStr, Chr(34))
        End If
        GetFieldName = Mid(infoStr, sPos + 6, EndPos - (sPos + 6))
    End Function

    '返回文件名
    Private Function GetFileName(ByVal infoStr As String) As String
        Dim sPos As Integer
        Dim EndPos As Integer

        sPos = InStr(infoStr, "filename=")
        EndPos = InStr(infoStr, Chr(34) & vbCrLf)
        GetFileName = Mid(infoStr, sPos + 10, EndPos - (sPos + 10))
    End Function

    '返回文件的 MIME type
    Private Function GetFileType(ByVal infoStr As String) As String
        Dim sPos As Integer

        sPos = InStr(infoStr, "Content-Type: ")
        GetFileType = Mid(infoStr, sPos + 14)
    End Function

    '根据路径获取文件名
    Private Function GetFileNameByPath(ByVal FullPath As String) As String
        Dim pos As Integer
        Dim ls_RetValue As String

        pos = 0
        ls_RetValue = ""
        FullPath = Replace(FullPath, "/", "\")
        pos = InStrRev(FullPath, "\") + 1
        If (pos > 0) Then
            ls_RetValue = Mid(FullPath, pos)
        Else
            ls_RetValue = FullPath
        End If

        Return ls_RetValue
    End Function

    '根据路径获取扩展名
    Private Function GetFileExt(ByVal FullPath As String) As String
        Dim pos As Integer
        Dim ls_RetValue As String

        ls_RetValue = ""
        pos = InStrRev(FullPath, ".")
        If pos > 0 Then
            ls_RetValue = Mid(FullPath, pos)
        End If

        Return ls_RetValue
    End Function
End Class
