using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetOwnerProductByNoQuery : IQuery<DropdownModel>
    {
        public string No { get; set; }
        public string BA_ID { get; set; }
    }
}