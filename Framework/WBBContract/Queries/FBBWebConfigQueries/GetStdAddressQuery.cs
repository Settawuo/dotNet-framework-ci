using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{

    public class STDFullConQuery : IQuery<StdAddressFullConListResult>
    {
        //public string OltNo { get; set; }
        //public string Region { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
}
