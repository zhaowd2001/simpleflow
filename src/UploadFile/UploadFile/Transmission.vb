
Public Enum TransmissionReadyState
    uninitialized = 1
    loading = 2
    loaded = 3
    interactive = 4
    complete = 5
End Enum

Public Class Transmission
    Implements System.Web.IHttpModule

    Private ob_FileBinary() As Byte
    Private o_TransmissionInfo As TransmissionInfo
    Private o_TransmissionFile As TransmissionFile
    Private ol_TotalBytes As Long
    Private os_BigUploadDir As String = "Bigupload\"

    Public Property BigUploadDir() As String
        Get
            Return os_BigUploadDir
        End Get
        Set(ByVal value As String)
            os_BigUploadDir = value
        End Set
    End Property

    Public ReadOnly Property TransmissionFile() As TransmissionFile
        Get
            Return o_TransmissionFile
        End Get
    End Property

    Public ReadOnly Property TransmissionInfo() As TransmissionInfo
        Get
            Return o_TransmissionInfo
        End Get
    End Property

    Public Property TotalBytes() As Long
        Get
            Return ol_TotalBytes
        End Get
        Set(ByVal value As Long)
            ol_TotalBytes = value
            o_TransmissionInfo.TotalBytes = value
        End Set
    End Property

    Public ReadOnly Property MaxTotalBytes() As Long
        Get
            Return 200 * 1024 * 1024    ' 最大100MB
        End Get
    End Property

    Public ReadOnly Property Version() As Long
        Get
            Return "3.0"    ' 版本
        End Get
    End Property

    Public ReadOnly Property ChunkReadSize() As Long
        Get
            Return 8192
        End Get
    End Property

    Public Function tf_InstrB(ByRef a_Byte1() As Byte, ByRef a_Byte2() As Byte) As Long
        Return tf_InstrB_Start(0, a_Byte1, a_Byte2)
    End Function

    Public Function tf_InstrB_Start(ByVal al_Start As Long, ByRef a_Byte1() As Byte, ByRef a_Byte2() As Byte) As Long
        Dim i As Integer
        Dim j As Integer
        Dim ll_RetValue As Integer

        al_Start = al_Start
        ll_RetValue = -1
        For i = al_Start To UBound(a_Byte1) - UBound(a_Byte2)
            If a_Byte1(i) = a_Byte2(0) Then
                For j = 0 To UBound(a_Byte2)
                    If a_Byte1(j + i) <> a_Byte2(j) Then
                        Exit For
                    End If
                Next
                If j > UBound(a_Byte2) Then
                    ll_RetValue = i
                    Exit For
                End If
            End If
        Next

        Return ll_RetValue
    End Function

    Public Function tf_MidB(ByRef a_Byte() As Byte, ByVal al_Start As Long, ByVal al_Length As Long) As Byte()
        Dim lb_RetValue() As Byte
        Dim i As Long

        ReDim lb_RetValue(al_Length - 1)
        For i = 0 To al_Length - 1
            lb_RetValue(i) = a_Byte(i + al_Start)
        Next

        Return lb_RetValue
    End Function

    Private Sub wf_PharseUploadFileInfo(ByRef ab_FileBinary As Byte())
        Dim ll_BoundaryStart As Long
        Dim ll_BoundaryEnd As Long
        Dim ll_PosEndOfHeader As Long
        Dim lb_IsBoundaryEnd As Boolean
        Dim ls_Header As String
        Dim TwoCharsAfterEndBoundary As String
        Dim ls_Boundary As String
        Dim lb_BinaryBoundary() As Byte

        lb_IsBoundaryEnd = False
        ls_Boundary = Component.Web.tf_GetBinaryBoundary()
        lb_BinaryBoundary = Component.LsConvert.StringToBinary(ls_Boundary)
        ll_BoundaryStart = tf_InstrB(ob_FileBinary, lb_BinaryBoundary)
        ll_BoundaryEnd = tf_InstrB_Start(CType(ll_BoundaryStart + UBound(lb_BinaryBoundary) + 1, Long), ob_FileBinary, lb_BinaryBoundary)
        Do While (ll_BoundaryStart >= 0 And ll_BoundaryEnd >= 0 And Not lb_IsBoundaryEnd)
            ' 获取表单头的结束位置
            ll_PosEndOfHeader = tf_InstrB_Start(ll_BoundaryStart + (UBound(lb_BinaryBoundary) + 1), _
                                                ob_FileBinary, _
                                                Component.LsConvert.StringToBinary(vbCrLf + vbCrLf))

            ' 分离表单头信息，类似于：
            ' Content-Disposition: form-data; name="file1"; filename="G:\homepage.txt"
            ' Content-Type: text/plain
            ls_Header = Component.LsConvert.BinaryToString(tf_MidB(ob_FileBinary, _
                                                                ll_BoundaryStart + (UBound(lb_BinaryBoundary) + 1) + 2, _
                                                                ll_PosEndOfHeader - ll_BoundaryStart - (UBound(lb_BinaryBoundary) + 1) - 2))

            ' 分离表单内容
            ab_FileBinary = tf_MidB(ob_FileBinary, (ll_PosEndOfHeader + 4), ll_BoundaryEnd - (ll_PosEndOfHeader + 4) - 2)

            ' 如果是附件
            If InStr(ls_Header, "filename=""") > 0 Then
                Call o_TransmissionFile.FillFileInfo(ls_Header, UBound(ab_FileBinary) + 1)
            End If

            ' 是否结束位置
            TwoCharsAfterEndBoundary = Component.LsConvert.BinaryToString(tf_MidB(ob_FileBinary, ll_BoundaryStart + UBound(lb_BinaryBoundary) + 1, 2))
            If TwoCharsAfterEndBoundary = "--" Then
                lb_IsBoundaryEnd = True
            End If

            If Not lb_IsBoundaryEnd Then ' 如果不是结尾, 继续读取下一块
                ll_BoundaryStart = ll_BoundaryEnd
                ll_BoundaryEnd = tf_InstrB_Start(ll_BoundaryStart + (UBound(lb_BinaryBoundary) + 1), ob_FileBinary, lb_BinaryBoundary)
            End If
        Loop

        ' 解析文件结束后更新进度信息
        o_TransmissionInfo.UploadedBytes = Me.TotalBytes
        o_TransmissionInfo.ReadyState = TransmissionReadyState.interactive '解析文件结束
        Call o_TransmissionInfo.UpdateToApplicationContext()

        'Call System.Web.HttpContext.Current.Response.Write("<hr>")
        'Call System.Web.HttpContext.Current.Response.Write(Component.LsConvert.BinaryToString(ob_FileBinary) & "<br><hr>")
        'Call System.Web.HttpContext.Current.Response.Write("ls_Boundary=" & ls_Boundary & "<br><hr>")
        'Call System.Web.HttpContext.Current.Response.Write("ll_BoundaryStart=" & ll_BoundaryStart & "<br><hr>")
        'Call System.Web.HttpContext.Current.Response.Write("ll_BoundaryEnd=" & ll_BoundaryEnd & "<br><hr>")
    End Sub

    ' 分析上传的数据，并保存到相应集合中
    Public Function Upload(ByVal as_ProgressID As String, ByRef a_HttpWorkerRequest As System.Web.HttpWorkerRequest) As String
        Dim ls_RetValue As String
        Dim lb_FileBinary() As Byte

        ls_RetValue = ""
        If TotalBytes < 1 Then
            ls_RetValue = "无数据传入"
        End If
        If TotalBytes > MaxTotalBytes Then
            ls_RetValue = "您当前上传大小为" & TotalBytes / 1000 & " K，最大允许为" & MaxTotalBytes / 1024 & "K"
        End If

        If ls_RetValue = "" Then
            o_TransmissionInfo.ProgressID = as_ProgressID

            '开始上传，读取客户端的二进制文件到内存
            Call wf_StartUploading(a_HttpWorkerRequest)
            '提取文件描述信息
            lb_FileBinary = Nothing
            Call wf_PharseUploadFileInfo(lb_FileBinary)
            '保存到硬盘
            Call Save(lb_FileBinary)
        End If

        Return ls_RetValue
    End Function

    Public Sub Save(ByRef ab_FileBinary() As Byte)
        Dim ls_FileName As String
        Dim ls_FileType As String
        Dim l_ZipFile As Component.ZipFile

        ls_FileName = System.Web.HttpContext.Current.Server.MapPath(BigUploadDir + Me.o_TransmissionFile.FileName)
        Call Component.File.WriteToFile(ls_FileName, ab_FileBinary)

        ls_FileType = LCase(Right(ls_FileName, 3))
        If ls_FileType <> "zip" And ls_FileType <> "rar" Then
            l_ZipFile = New Component.ZipFile
            Call l_ZipFile.Create(ls_FileName + ".zip")
            Call l_ZipFile.Entries.Add(ls_FileName)

            Call Component.File.DeleteFile(ls_FileName)
        End If
    End Sub

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

    Private Sub wf_StartUploading(ByRef a_HttpWorkerRequest As System.Web.HttpWorkerRequest)
        Dim ll_BytesRead As Long
        Dim lb_DataPart() As Byte
        Dim ll_PartSize As Long
        Dim ll_ReadedSize As Long
        Dim lb_PreloadedBufferData() As Byte
        Dim i As Long

        'HttpWorkerRequest 
        o_TransmissionInfo.ReadyState = TransmissionReadyState.loading '开始上传
        Call o_TransmissionInfo.UpdateToApplicationContext()

        ll_BytesRead = 0
        ReDim ob_FileBinary(TotalBytes - 1)

        '读取已经加载的内容
        lb_PreloadedBufferData = a_HttpWorkerRequest.GetPreloadedEntityBody()
        If Not lb_PreloadedBufferData Is Nothing Then
            For i = 0 To UBound(lb_PreloadedBufferData)
                ob_FileBinary(i + ll_BytesRead) = lb_PreloadedBufferData(i)
            Next
            ll_BytesRead = ll_BytesRead + UBound(lb_PreloadedBufferData) + 1
        End If

        '循环分块读取
        ll_PartSize = ChunkReadSize
        ll_ReadedSize = Long.MaxValue
        Do While ll_ReadedSize > 0 And ll_BytesRead < TotalBytes
            ReDim lb_DataPart(ll_PartSize - 1)
            ll_ReadedSize = a_HttpWorkerRequest.ReadEntityBody(lb_DataPart, ll_PartSize)

            '分块读取
            For i = 0 To ll_ReadedSize - 1
                ob_FileBinary(i + ll_BytesRead) = lb_DataPart(i)
            Next
            ll_BytesRead = ll_BytesRead + ll_ReadedSize

            o_TransmissionInfo.UploadedBytes = ll_BytesRead
            o_TransmissionInfo.LastActivity = Now()

            ' 更新进度信息
            Call o_TransmissionInfo.UpdateToApplicationContext()
        Loop

        ' 上传结束后更新进度信息
        o_TransmissionInfo.ReadyState = TransmissionReadyState.loaded '上传结束
        Call o_TransmissionInfo.UpdateToApplicationContext()

        'Call System.Web.HttpContext.Current.Response.Write(Component.LsConvert.BinaryToString(ob_FileBinary) & "<br><hr>")
        'Call System.Web.HttpContext.Current.Response.Write("ll_BytesRead=" & ll_BytesRead & "<br><hr>")
        'Call System.Web.HttpContext.Current.Response.Write("TotalBytes=" & TotalBytes & "<br><hr>")
    End Sub

    Public Sub New()
        o_TransmissionInfo = New TransmissionInfo
        o_TransmissionFile = New TransmissionFile
    End Sub

    Public Sub Dispose() Implements System.Web.IHttpModule.Dispose
        'do nothing
    End Sub

    Public Sub Init(ByVal context As System.Web.HttpApplication) Implements System.Web.IHttpModule.Init
        AddHandler context.BeginRequest, AddressOf context_BeginRequest
    End Sub

    Public Sub context_BeginRequest(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim lb_Temp As Boolean
        Dim ls_ProgressID As String
        Dim ls_RawUrl As String
        Dim l_HttpContext As System.Web.HttpContext
        Dim l_HttpWorkerRequest As System.Web.HttpWorkerRequest
        Dim l_Temp As Object
        Dim ll_ContentLength As Long
        'Dim ls_GetFileType As String
        'Dim ls_GetFileTypeX As String

        lb_Temp = of_CheckNeedProcess()
        If lb_Temp = True Then
            ls_ProgressID = System.Web.HttpContext.Current.Request.Item("ProgressID")
            'ls_GetFileType = System.Web.HttpContext.Current.Request.QueryString("filefield")

            'ls_GetFileType = LCase(Right(ls_GetFileType, 3))
            'ls_GetFileTypeX = LCase(Right(ls_GetFileType, 4))
            'If ls_GetFileType <> "zip" And ls_GetFileType <> "rar" And ls_GetFileType <> "doc" And ls_GetFileType <> "xls" _
            '    And ls_GetFileType <> "ppt" And ls_GetFileTypeX <> "docx" And ls_GetFileTypeX <> "xlsx" _
            '    And ls_GetFileTypeX <> "pptx" Then

            '    Call System.Web.HttpContext.Current.Response.Write("只能上传ZIP、RAR压缩文件")
            '    Call System.Web.HttpContext.Current.Response.End()
            'End If

            l_HttpContext = CType(sender, System.Web.HttpApplication).Context
            l_Temp = l_HttpContext.GetType().GetProperty("WorkerRequest", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
            l_HttpWorkerRequest = CType(l_Temp.GetValue(l_HttpContext, Nothing), System.Web.HttpWorkerRequest)
            ll_ContentLength = Long.Parse((l_HttpWorkerRequest.GetKnownRequestHeader(System.Web.HttpWorkerRequest.HeaderContentLength)))

            Me.TotalBytes = ll_ContentLength
            Call Me.Upload(ls_ProgressID, l_HttpWorkerRequest)
            ls_RawUrl = Component.Web.tf_GetAppSetting("MyAppPath") + "Transmission.aspx"
            Call l_HttpContext.Response.Redirect(ls_RawUrl)
        End If
    End Sub

    Private Function of_CheckNeedProcess() As Boolean
        Dim lb_RetValue As Boolean
        Dim ls_FileField As String
        Dim ls_ProgressID As String
        Dim ls_ConfirmKey As String

        lb_RetValue = False
        ls_FileField = System.Web.HttpContext.Current.Request.Item("filefield")
        ls_ProgressID = System.Web.HttpContext.Current.Request.Item("ProgressID")
        ls_ConfirmKey = System.Web.HttpContext.Current.Request.Item("ConfirmKey")   '这个参数用来判断是否是指定页面
        If ls_FileField <> "" And ls_ProgressID <> "" And ls_ConfirmKey = "uploadfilewith3wfocus" Then
            '执行指定的处理代码
            lb_RetValue = True
        End If

        Return lb_RetValue
    End Function



End Class

