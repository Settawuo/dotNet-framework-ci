namespace FBBConfig.Models
{
    public class NasPortalInterface
    {
        public string Path { get; set; }
        public string NasConnection { get; set; }
        public string NasDisplayVal { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 22;
        public string Username { get; set; }
        public string Password { get; set; }
        public string TextSearch { get; set; }
    }
}