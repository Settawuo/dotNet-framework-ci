using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetAWCEditQuery : IQuery<AWCinformation>
    {
        public string site_id { get; set; }
        public AWCModel oldmodelpage1 { get; set; }

    }
}
