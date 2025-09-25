using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class evAMQueryCustomerInfoQuery : IQuery<evAMQueryCustomerInfoModel>
    {
        public string idCardNum { get; set; }
        public string name { get; set; }
        public string accntNo { get; set; }
        public string contactBirthDt { get; set; }
        public string minRowNum { get; set; }
        public string maxRowNum { get; set; }
        public string ClientIP { get; set; }

    }
}
