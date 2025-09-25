using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class SubmitFOAGetErrorDescQuery : IQuery<SubmitFOAGetErrorDesc>
    {
        public string P_MATERIAL { get; set; }
        public string P_SERIAL { get; set; }
        public string P_LOCATION { get; set; }
        public string P_PLANT { get; set; }
        public string P_ERR_MSG { get; set; }
    }
}
