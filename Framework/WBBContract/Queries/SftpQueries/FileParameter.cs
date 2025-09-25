namespace WBBContract.Queries.SftpQueries
{
    public class SftpFileParameter
    {
        public string Key { get; set; }
        public object Nas { get; set; }
        public string Lovtype { get; set; }
        public string LovnameAcc { get; set; }
        public string LovnamePath { get; set; }
        public string LovtypePower { get; set; }
    }
    public class FileParameter
    {
        public string remotePath { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string KeyFile { get; set; }
        public string UserName { get; set; }
        public string ConfigType { get; set; }
    }

    public class NasFileParameter
    {
        public string remotePath { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string ConfigType { get; set; }
    }
}
