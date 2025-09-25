using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMeshParameterAppointmentQuery : IQuery<MeshParameterAppointmentModel>
    {
        public string nonMobile { get; set; }
        public string p_addressID { get; set; }
        public string Channel { get; set; }
        public string installDate { get; set; }
        public string promotionMain { get; set; }
        public string language { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string outServiceLevel { get; set; }
        public string optionMesh { get; set; }
        public string officerChannel { get; set; }
        public string flag_call { get; set; }
    }
}
