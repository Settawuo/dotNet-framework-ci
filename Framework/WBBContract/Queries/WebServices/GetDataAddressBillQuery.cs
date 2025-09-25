using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDataAddressBillQuery : IQuery<GetDataAddressBillModel>
    {
        public string transaction_Id { get; set; }
        public string fullUrl { get; set; }
        public string p_houseNo { get; set; }
        public string p_Moo { get; set; }
        public string p_Mooban { get; set; }
        public string p_Room { get; set; }
        public string p_Floor { get; set; }
        public string p_buildingName { get; set; }
        public string p_Soi { get; set; }
        public string p_streetName { get; set; }
        public string p_Tumbol { get; set; }
        public string p_Amphur { get; set; }
        public string p_provinceName { get; set; }
        public string p_zipCode { get; set; }
        public string p_language { get; set; }

    }
}
