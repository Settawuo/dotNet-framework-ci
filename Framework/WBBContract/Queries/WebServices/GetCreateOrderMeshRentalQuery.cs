using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCreateOrderMeshRentalQuery : IQuery<GetCreateOrderMeshRentalModel>
    {
        public string p_internet_no { get; set; }
        public string p_payment_order_id { get; set; }
        public string p_penalty_install { get; set; }
        //R22.11 Mesh with arpu
        public string p_point { get; set; }
        public string p_flag_option { get; set; }
        public string p_flag_mesh { get; set; }
    }
}
