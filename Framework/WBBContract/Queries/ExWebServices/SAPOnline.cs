using System.Collections.Generic;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class NewRegistForSubmitFOAQuery : NewRegistFOA, IQuery<NewRegistForSubmitFOAResponse>
    {
        //private List<NewRegistFOAProductList> _productListMappingArray;
        //public List<NewRegistFOAProductList> productListMappingArray
        //{
        //    get { return _productListMappingArray; }
        //    set { _productListMappingArray = value; }
        //}
    }

    public class NewRegistForSubmitFOA4HANAQuery : NewRegistFOA, IQuery<NewRegistForSubmitFOAS4HANAResponse>
    {
    }

    public class NewRegisterQuery : Register, IQuery<NewRegisterResponse>
    {
    }

    public class JoinOrderQuery : JoinOrder, IQuery<JoinOrderResponse>
    {
    }

    public class InstallationCostQuery : InstallationCost, IQuery<InstallationCostResponse>
    {
    }

    public class TerminateServiceQuery : TerminateService, IQuery<TerminateServiceResponse>
    {
    }

    public class RenewServiceQuery : RenewService, IQuery<RenewServiceResponse>
    {
    }
    public class lostTranQuery : IQuery<List<lostTranQueryResponse>>
    {
    }
    public class NewRegistForSubmitFOARevaluePendingQuery : IQuery<NewRegistForSubmitFOAResponse>
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string ACTION { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUB_NUMBER { get; set; }
        public string COM_CODE { get; set; }
        public string DOC_DATE { get; set; }
        public string ERR_CODE { get; set; }
        public string ERR_MSG { get; set; }
        public string STATUS { get; set; }
        public string TRANS_ID { get; set; }
        public string ITEM_TEXT { get; set; }
    }
}
