using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class DetailCalliMTopupReplaceModel
    {
        public DetailCalliMTopupReplaceModel()
        {
            if (RETURN_PRICE_CURROR == null)
                RETURN_PRICE_CURROR = new List<DetailCalliMTopupReplace>();
        }

        public int RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<DetailCalliMTopupReplace> RETURN_PRICE_CURROR { get; set; }
    }

    public class DetailCalliMTopupReplace
    {
        public string AssetNo { get; set; }
        public string UserID { get; set; }
        public string InteractionType { get; set; }
        public string SocialAccountID { get; set; }
        public string SocialName { get; set; }
        public string CustomerName { get; set; }
        public string ConfirmFlag { get; set; }
        public string SymptomCode { get; set; }
        public string Symptom { get; set; }
        public string ContactNo { get; set; }
        public string Channel { get; set; }
        public string SymptomName { get; set; }
        public string Priority { get; set; }
        public string TopicName { get; set; }
        public string PlayboxAdditionalService { get; set; }
        public string SerialNo { get; set; }
        public string FixedLineNo { get; set; }
        public string TimeSelected { get; set; }
        public string DateSelected { get; set; }
        public string ReasonOver24Hr { get; set; }
        public string ContactFlag { get; set; }
        public string CommentFromCustomer { get; set; }
        public string AccessType { get; set; }
        public string OnlineStatus { get; set; }
        public string LocationAddress { get; set; }
        public string Package { get; set; }
        public string PackageOnTop { get; set; }
        public string StartedUsingDateTime { get; set; }
        public string IPAddress { get; set; }
        public string IPvSix { get; set; }
        public string IPType { get; set; }
        public string NodeName { get; set; }
        public string AddressID { get; set; }
        public string ReservedId { get; set; }
    }
}
