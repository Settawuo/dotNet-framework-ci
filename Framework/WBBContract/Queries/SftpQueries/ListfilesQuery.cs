using System.Collections.Generic;

namespace WBBContract.Queries.SftpQueries
{
    public class ListfilesQuery : IQuery<List<ListfilesModels>>
    {
        public string Path { get; set; }
        public string TransectionId { get; set; }
        public string NasType { get; set; }
        public string Key { get; set; }
    }

    public class ListfilesModels
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string DateModified { get; set; }
        public long FileSize { get; set; }
        public string msg { get; set; }
    }

    public class GetfilesAllQuery : IQuery<List<ListfilesModels>>
    {
        public string Path { get; set; }
        public string TransectionId { get; set; }
        public string NasType { get; set; }
        public string Key { get; set; }
    }
}
