namespace cardocr
{
    public class JobInfo : HandlerInfo
    {
        public string JobID;
        public string AppID;
        public string Version;
        public string RemoteFilePath;
        public string SessionFrom;
        public string SessionTo;
        //job result
        public int    Result;
        public string ResultRemoteFilePath;

        public void setNullFieldToEmpty()
        {
            setNullFieldToEmpty( ref JobID);
            setNullFieldToEmpty( ref AppID);
            setNullFieldToEmpty( ref Version);
            setNullFieldToEmpty( ref RemoteFilePath);
            setNullFieldToEmpty( ref SessionFrom);
            setNullFieldToEmpty( ref SessionTo);
            setNullFieldToEmpty( ref ResultRemoteFilePath);
        }

        void setNullFieldToEmpty(ref string s)
        {
            if (s == null)
                s = "";
        }

        public JobInfo()
        {
            Result = -1;
        }
    }
}
