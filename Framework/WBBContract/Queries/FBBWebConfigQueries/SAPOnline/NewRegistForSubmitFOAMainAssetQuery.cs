using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class NewRegistForSubmitFOAMainAssetQuery : IQuery<NewRegistForSubmitFOAMainAssetModel>
    {
        public string p_ORDER_NO { get; set; }
        public string p_INTERNET_NO { get; set; }
        public string p_COM_CODE_OLD { get; set; }
        public string p_COM_CODE_NEW { get; set; }
        public string p_ASSET_CLASS { get; set; }
        public string p_COSTCENTER { get; set; }
        public string p_PRODUCT { get; set; }
        public string p_USER_CODE { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SUBCONTRACT_NAME { get; set; }
        public string FOA_SUBMIT_DATE { get; set; }
        public string UserName { get; set; }

    }
}
