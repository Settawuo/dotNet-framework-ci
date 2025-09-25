using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class CheckdupBuildingQuery : IQuery<ConfigurationDormitoryBuildingData>
    {
        public string p_dormitory_id { get; set; }
        public string p_building_th { get; set; }
        public string p_building_en { get; set; }
        public string p_room_amount { get; set; }
        public decimal p_return_code { get; set; }

    }
}
