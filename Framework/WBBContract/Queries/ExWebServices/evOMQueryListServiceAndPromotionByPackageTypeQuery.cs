using System.Collections.Generic;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class evOMQueryListServiceAndPromotionByPackageTypeQuery : IQuery<evOMQueryListServiceAndPromotionByPackageTypeModel>
    {
        //InputParameter 
        public string mobileNo { get; set; }
        public string idCard { get; set; }
        //public string promotionType { get; set; }
        public string serviceType { get; set; }
        public string FullUrl { get; set; }
    }

    public class evESQueryPersonalInformationQuery : IQuery<List<evESQueryPersonalInformationModel>>
    {
        //InputParameter
        /// <summary>
        /// Mobile No
        /// </summary>
        public string mobileNo { get; set; }
        /// <summary>
        /// ‘1’ = query Indentification
        /// ‘2’ = query promotion
        /// ‘3’ = query service 
        /// ‘4’ = query Billing Profile (serviceQueryBillingProfile)
        /// ‘5’ = query Billing Address (serviceQueryBillingAddressProfile)  
        /// ‘6’ = query Prepaid Profile 
        /// ‘7’ = query Prepaid Iden
        /// </summary>
        public string option { get; set; }
        public string FullUrl { get; set; }

        public string sourceSystem { get; set; }
    }

    public class IpcameraSuccess
    {
        public string uuid { get; set; }
        public string transactionID { get; set; }
        public string email { get; set; }
    }

    public class IpcameraFail
    {
        public string status { get; set; }
        public string errorMessage { get; set; }
    }


    public class ResponseIpcameraSuccess
    {
        public string result { get; set; }
        public string code { get; set; }
        public IpcameraSuccess payload { get; set; }
    }

    public class ResponseIpcameraFail
    {
        public string result { get; set; }
        public string code { get; set; }
        public string errorMessage { get; set; }
        public IpcameraFail payload { get; set; }
    }
}
