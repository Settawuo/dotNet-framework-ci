namespace WBBContract.Queries.SftpQueries
{
    public class DeleteFileQuery : IQuery<DeleteFileModel>
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Path { get; set; }
        public string TransectionId { get; set; }
        public string FileName { get; set; }
        public string Key { get; set; }
        public string NasType { get; set; }
    }
    public class NasDeleteFileQuery : IQuery<DeleteFileModel>
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Path { get; set; }
        public string TransectionId { get; set; }
        public string FileName { get; set; }
        public string Key { get; set; }
        public string NasType { get; set; }
    }
    public class DeleteFileModel
    {
        public bool Delete { get; set; }
        public string Message { get; set; }
    }
}
