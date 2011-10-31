Public Class UploadContext
    'Inherits Component.WebContextBase



    Public Shared Property UpFileName() As String
        Get
            Return ConextItem("UploadContext_UpFileName")
        End Get
        Set(ByVal value As String)
            ConextItem("UploadContext_UpFileName") = value
        End Set
    End Property

    Public Shared Property FileSize() As String
        Get
            Return ConextItem("UploadContext_FileSize")
        End Get
        Set(ByVal value As String)
            ConextItem("UploadContext_FileSize") = value
        End Set
    End Property

    Public Shared Property FileType() As String
        Get
            Return ConextItem("UploadContext_FileType")
        End Get
        Set(ByVal value As String)
            ConextItem("UploadContext_FileType") = value
        End Set
    End Property

    Public Shared Property FilePath() As String
        Get
            Return ConextItem("UploadContext_FilePath")
        End Get
        Set(ByVal value As String)
            ConextItem("UploadContext_FilePath") = value
        End Set
    End Property

    Public Shared Property ConextItem(ByVal as_Key As String) As String
        Get
            Dim ls_RetValue As String

            ls_RetValue = ""
            as_Key = as_Key

            If ls_RetValue = "" Then
                If Not System.Web.HttpContext.Current.Request.Cookies(as_Key) Is Nothing Then
                    ls_RetValue = System.Web.HttpContext.Current.Request.Cookies(as_Key).Value
                End If
            End If

            If ls_RetValue <> "" Then
                ls_RetValue = Component.WebContextBase.DecodePara(System.Web.HttpUtility.UrlDecode(ls_RetValue))
            End If
          

            Return ls_RetValue
        End Get
        Set(ByVal value As String)
            Dim ls_Temp As String
            Dim l_HttpCookie As System.Web.HttpCookie

            ls_Temp = ""
            as_Key = as_Key
            If value <> "" Then
                ls_Temp = Component.WebContextBase.EncodePara(value)
            End If


            '设定Cookie内容
            If System.Web.HttpContext.Current.Request.Cookies(as_Key) Is Nothing Then
                l_HttpCookie = New System.Web.HttpCookie(as_Key)
                Call System.Web.HttpContext.Current.Request.Cookies.Add(l_HttpCookie)
            End If
            System.Web.HttpContext.Current.Request.Cookies(as_Key).Value = ls_Temp

            If System.Web.HttpContext.Current.Response.Cookies(as_Key) Is Nothing Then
                l_HttpCookie = New System.Web.HttpCookie(as_Key)
                Call System.Web.HttpContext.Current.Response.Cookies.Add(l_HttpCookie)
            End If
            System.Web.HttpContext.Current.Response.Cookies(as_Key).Value = ls_Temp

        End Set
    End Property

   
End Class
