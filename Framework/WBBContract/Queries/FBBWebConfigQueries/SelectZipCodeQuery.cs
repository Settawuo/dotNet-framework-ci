using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectZipCodeQuery : IQuery<ZipCodeModel>
    {
        public string Province { get; set; }

        public string Aumphur { get; set; }

        public string Tumbon { get; set; }

        public string PostalCode { get; set; }

        public string Language { get; set; }
    }
}