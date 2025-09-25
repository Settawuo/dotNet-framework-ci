namespace WBBContract.Queries.SftpQueries
{
    public class UploadFileQuery : IQuery<UploadFileModel>
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Path { get; set; }
        public string TransectionId { get; set; }
        public string FileName { get; set; }
        public byte[] DataFile { get; set; }
        public string Key { get; set; }
        public string NasType { get; set; }
    }

    public class NasUploadFileQuery : IQuery<UploadFileModel>
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Path { get; set; }
        public string TransectionId { get; set; }
        public string FileName { get; set; }
        public byte[] DataFile { get; set; }
        public string Key { get; set; }
        public string NasType { get; set; }
    }
    public class UploadFileModel
    {
        public bool Upload { get; set; }
        public string Message { get; set; }
    }
}
