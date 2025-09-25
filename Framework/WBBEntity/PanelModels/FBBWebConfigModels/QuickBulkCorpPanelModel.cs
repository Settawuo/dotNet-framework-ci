namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class QuickBulkCorpPanelModel : PanelModelBase
    {
        public class SeibelResultModel
        {
            public string outASCPatnerName { get; set; }
            public string outBusinessType { get; set; }
            public string outCharacteristic { get; set; }
            public string outErrorMessage { get; set; }
            public string outFullAddress { get; set; }
            public string outLocationCode { get; set; }
            public string outMainFax { get; set; }
            public string outMainPhone { get; set; }
            public string outMobileNo { get; set; }
            public string outOperatorClass { get; set; }

            public string outPartnerName { get; set; }
            public string outProvince { get; set; }
            public string outRegion { get; set; }
            public string outStatus { get; set; }
            public string outSubRegion { get; set; }
            public string outSubType { get; set; }
            public string outTitle { get; set; }
            public string outType { get; set; }
            public string outWTName { get; set; }

        }

        public RegisterBulkCorpInstallModel RegisterBulkCorpInstallModel { get; set; }
        //Workflow
        public DetailWorkflow DetailWorkflow { get; set; }
        public airregistpkgList airregistpkgList { get; set; }
        //SFF
        public DetailSFF DetailSFF { get; set; }
        public listServiceVdsl listServiceVdsl { get; set; }
        public listServiceVdslRouter listServiceVdslRouter { get; set; }
        public PromoMainlist PromoMainlist { get; set; }
        public PromoOntoplist PromoOntoplist { get; set; }
        public InstanceList InstanceList { get; set; }

    }
}

