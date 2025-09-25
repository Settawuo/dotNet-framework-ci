using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMeshParameterReserveQuery : IQuery<MeshParameterReserveModel>
    {
        public string nonMobile { get; set; }
        public string p_addressID { get; set; }
        public string promotionMain { get; set; }
        public string appointmentDate { get; set; }
        public string timeSlot { get; set; }
        public string language { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string outServiceLevel { get; set; }
        public string optionMesh { get; set; }
    }
}
