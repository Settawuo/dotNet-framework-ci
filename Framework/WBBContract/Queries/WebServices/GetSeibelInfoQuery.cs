using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSeibelInfoQuery : IQuery<SeibelResultModel>
    {
        public string LocationCode { get; set; }
        public string ASCCode { get; set; }
        public string Inparam1 { get; set; }

        public string errorMessage { get; set; }

        // Update 17.2
        public string Transaction_Id { get; set; }

        // Update 17.5
        public string FullURL { get; set; }
    }
}
