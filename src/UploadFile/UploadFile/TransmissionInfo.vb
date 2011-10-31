
Public Class TransmissionInfo
    Private o_ReadyState As TransmissionReadyState
    Private os_ProgressID As String
    Private ol_UploadedBytes As Long
    Private od_LastActivity As Date
    Private od_StartTime As Date
    Private ol_TotalBytes As Long

    Public Property StartTime() As Date
        Get
            Return od_StartTime
        End Get
        Set(ByVal value As Date)
            od_StartTime = value
        End Set
    End Property

    Public Property TotalBytes() As Long
        Get
            Return ol_TotalBytes
        End Get
        Set(ByVal value As Long)
            ol_TotalBytes = value
        End Set
    End Property

    Public Property ReadyState() As TransmissionReadyState
        Get
            Return o_ReadyState
        End Get
        Set(ByVal value As TransmissionReadyState)
            o_ReadyState = value
        End Set
    End Property

    Public Property ProgressID() As String
        Get
            Return os_ProgressID
        End Get
        Set(ByVal value As String)
            os_ProgressID = value
        End Set
    End Property

    Public Property UploadedBytes() As Long
        Get
            Return ol_UploadedBytes
        End Get
        Set(ByVal value As Long)
            ol_UploadedBytes = value
        End Set
    End Property

    Public Property LastActivity() As Date
        Get
            Return od_LastActivity
        End Get
        Set(ByVal value As Date)
            od_LastActivity = value
        End Set
    End Property

    Private Function of_GetDescXML() As String
        Dim ls_RetValue As String
        Dim l_XmlDocument As New System.Xml.XmlDocument
        Dim l_XmlElement As System.Xml.XmlElement
        Dim l_ProgressInfo As System.Xml.XmlNode

        '构造XML格式
        l_XmlElement = l_XmlDocument.CreateElement("ProgressInfo")
        Call l_XmlDocument.AppendChild(l_XmlElement)
        l_ProgressInfo = l_XmlDocument.SelectSingleNode("ProgressInfo")

        '填写数据
        l_XmlElement = l_XmlDocument.CreateElement("TotalBytes")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.TotalBytes

        l_XmlElement = l_XmlDocument.CreateElement("UploadedBytes")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.UploadedBytes

        l_XmlElement = l_XmlDocument.CreateElement("StartTime")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.StartTime

        l_XmlElement = l_XmlDocument.CreateElement("LastActivity")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.LastActivity

        l_XmlElement = l_XmlDocument.CreateElement("ReadyState")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.ReadyState.ToString()

        l_XmlElement = l_XmlDocument.CreateElement("TotalSize")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.TotalSize

        l_XmlElement = l_XmlDocument.CreateElement("SizeCompleted")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.SizeCompleted

        l_XmlElement = l_XmlDocument.CreateElement("ElapsedTime")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.ElapsedTime

        l_XmlElement = l_XmlDocument.CreateElement("TransferRate")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.TransferRate

        l_XmlElement = l_XmlDocument.CreateElement("Percentage")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.Percentage

        l_XmlElement = l_XmlDocument.CreateElement("TimeLeft")
        Call l_ProgressInfo.AppendChild(l_XmlElement)
        l_XmlElement.InnerText = Me.TimeLeft

        ls_RetValue = l_XmlDocument.OuterXml

        Return ls_RetValue
    End Function

    Public Sub UpdateToApplicationContext()
        Dim ls_XML As String

        ls_XML = of_GetDescXML()

        '保存到 Application 变量
        Call System.Web.HttpContext.Current.Application.UnLock()
        System.Web.HttpContext.Current.Application.Item(ProgressID) = ls_XML
        Call System.Web.HttpContext.Current.Application.UnLock()
    End Sub

    Public Shared Sub RemoveTransmissionInfo(ByVal as_ProgressID As String)
        Call System.Web.HttpContext.Current.Application.UnLock()
        Call System.Web.HttpContext.Current.Application.Contents.Remove(as_ProgressID)
        Call System.Web.HttpContext.Current.Application.UnLock()
    End Sub

    Public Shared Function GetTransmissionInfo(ByVal as_ProgressID As String) As String
        Dim ls_RetValue As String
        Dim l_TransmissionInfo As TransmissionInfo

        ls_RetValue = ""
        If Not System.Web.HttpContext.Current.Application(as_ProgressID) Is Nothing Then
            Call System.Web.HttpContext.Current.Application.UnLock()
            ls_RetValue = System.Web.HttpContext.Current.Application.Item(as_ProgressID).ToString
            Call System.Web.HttpContext.Current.Application.UnLock()
        Else
            l_TransmissionInfo = New TransmissionInfo
            ls_RetValue = l_TransmissionInfo.of_GetDescXML()
        End If

        Return ls_RetValue
    End Function

    Public Sub New()
        UploadedBytes = 0   ' 已上传大小
        StartTime = Now()   ' 开始时间
        LastActivity = Now()     ' 最后更新时间
        'ReadyState = "uninitialized"    ' uninitialized,loading,loaded,interactive,complete
        ReadyState = TransmissionReadyState.uninitialized ' uninitialized,loading,loaded,interactive,complete
        'ErrorMessage = ""
    End Sub

    ' 总大小
    Public ReadOnly Property TotalSize() As String
        Get
            TotalSize = FormatNumber(TotalBytes / 1024, 0, 0, 0, -1) & " K"
        End Get
    End Property

    ' 已上传大小
    Public ReadOnly Property SizeCompleted() As String
        Get
            SizeCompleted = FormatNumber(UploadedBytes / 1024, 0, 0, 0, -1) & " K"
        End Get
    End Property

    ' 已上传秒数
    Public ReadOnly Property ElapsedSeconds() As Long
        Get
            ElapsedSeconds = DateDiff("s", StartTime, Now())
        End Get
    End Property

    ' 已上传时间
    Public ReadOnly Property ElapsedTime() As String
        Get
            If ElapsedSeconds > 3600 Then
                ElapsedTime = ElapsedSeconds \ 3600 & " 时 " & (ElapsedSeconds Mod 3600) \ 60 & " 分 " & ElapsedSeconds Mod 60 & " 秒"
            ElseIf ElapsedSeconds > 60 Then
                ElapsedTime = ElapsedSeconds \ 60 & " 分 " & ElapsedSeconds Mod 60 & " 秒"
            Else
                ElapsedTime = ElapsedSeconds Mod 60 & " 秒"
            End If
        End Get
    End Property

    ' 传输速率
    Public ReadOnly Property TransferRate() As String
        Get
            If ElapsedSeconds > 0 Then
                TransferRate = FormatNumber(UploadedBytes / 1024 / ElapsedSeconds, 2, 0, 0, -1) & " K/秒"
            Else
                TransferRate = "0 K/秒"
            End If
        End Get
    End Property

    ' 完成百分比
    Public ReadOnly Property Percentage() As String
        Get
            If TotalBytes > 0 Then
                Percentage = Fix(UploadedBytes / TotalBytes * 100) & "%"
            Else
                Percentage = "0%"
            End If
        End Get
    End Property

    ' 估计剩余时间
    Public ReadOnly Property TimeLeft() As String
        Get
            Dim SecondsLeft

            If UploadedBytes > 0 Then
                SecondsLeft = Fix(ElapsedSeconds * (TotalBytes / UploadedBytes - 1))
                If SecondsLeft > 3600 Then
                    TimeLeft = SecondsLeft \ 3600 & " 时 " & (SecondsLeft Mod 3600) \ 60 & " 分 " & SecondsLeft Mod 60 & " 秒"
                ElseIf SecondsLeft > 60 Then
                    TimeLeft = SecondsLeft \ 60 & " 分 " & SecondsLeft Mod 60 & " 秒"
                Else
                    TimeLeft = SecondsLeft Mod 60 & " 秒"
                End If
            Else
                TimeLeft = "未知"
            End If
        End Get
    End Property
End Class
