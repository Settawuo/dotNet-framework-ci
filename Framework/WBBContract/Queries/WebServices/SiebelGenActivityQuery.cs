using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class SiebelGenActivityQuery : IQuery<SiebelGenActivityQueryModel>
    {
        public string SRNUMBER { get; set; }
        public string ORDERID { get; set; }
        public string SERIALNUMBER { get; set; }
        public string DONE { get; set; }
        public string STATUS { get; set; }
        public string PLANNED { get; set; }
        public string CAMPAIGNID { get; set; }
        public string PRIMARYOWNERID { get; set; }
        public string COMMENT { get; set; }
        public string NOSOONERTHANDATE { get; set; }
        public string ACTIVITYCATEGORY { get; set; }
        public string REASON { get; set; }
        public string TYPE { get; set; }
        public string STARTED { get; set; }
        public string DOCUMENTNO { get; set; }
        public string MOREINFO { get; set; }
        public string CHECKORDERID { get; set; }
        public string ASSETID { get; set; }
        public string ACCOUNTID { get; set; }
        public string SUBSTATUS { get; set; }
        public string CONTACTID { get; set; }
        public string ACTIVITYSUBCATEGORY { get; set; }
        public string PRIORITY { get; set; }
        public string PRIMARYPRODUCTID { get; set; }
        public string OWNERNAME { get; set; }

        public string FullURL { get; set; }
        public string ID_CARD_NO { get; set; }

    }
}
