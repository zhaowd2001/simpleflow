<%@Page  Language="VB"  EnableSessionState=False%> 
<% 
    Dim ls_XML As String
    
    ls_XML = wf_GetTransmissionInfo(Request.QueryString("ProgressID"))
    Call response.write("<?xml version=""1.0"" encoding=""utf-8""?>" & ls_XML)
%>

<script language=vbscript runat=server>

    Private Function wf_GetTransmissionInfo(ByVal as_ProgressID As String) As String
        Dim ls_RetValue As String
        
        ls_RetValue = UploadFile.TransmissionInfo.GetTransmissionInfo(as_ProgressID)
        
        Return ls_RetValue
    End Function

</script>