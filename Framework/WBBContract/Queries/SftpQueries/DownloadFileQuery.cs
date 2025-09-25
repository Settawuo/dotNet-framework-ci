namespace WBBContract.Queries.SftpQueries
{
    public class DownloadFileQuery : IQuery<DownloadFileModel>
    {
        public string TransectionId { get; set; }
        public string FileName { get; set; }
        public string Key { get; set; }
        public object NasType { get; set; }
    }

    public class NasDownloadFileQuery : IQuery<DownloadFileModel>
    {
        public string TransectionId { get; set; }
        public string FileName { get; set; }
        public string Key { get; set; }
        public object NasType { get; set; }
    }
    public class DownloadFileModel
    {
        public byte[] Download { get; set; }
        public string msg { get; set; }
    }
}
