using System.Collections.Generic;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class evOMNewRegisMultiInstanceQuery : IQuery<evOMNewRegisMultiInstanceModel>
    {
        public evOMNewRegisMultiInstanceQuery()
        {
            if (bulkvdsl == null)
                bulkvdsl = new List<DetailBulkCorpListServiceVdsl>();
            if (bulkvdslrouter == null)
                bulkvdslrouter = new List<DetailBulkCorpListServiceVdslRouter>();
            if (bulkappoint == null)
                bulkappoint = new List<DetailBulkCorpListServiceAppoint>();//17.7
            if (bulksffpromotioncur == null)
                bulksffpromotioncur = new List<DetailBulkCorpSffPromotionCur>();
            if (bulksffpromotionontopcur == null)
                bulksffpromotionontopcur = new List<DetailBulkCorpSffPromotionOntopCur>();
            if (bulklistinstancecur == null)
                bulklistinstancecur = new List<DetailBulkCorpListInstanceCur>();
        }
        public string referenceNo { get; set; }
        public string accountCat { get; set; }
        public string accountSubCat { get; set; }
        public string idCardType { get; set; }
        public string idCardNo { get; set; }
        public string titleName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string saName { get; set; }
        public string baName { get; set; }
        public string caNumber { get; set; }
        public string baNumber { get; set; }
        public string saNumber { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public string billName { get; set; }
        public string billCycle { get; set; }
        public string billLanguage { get; set; }
        public string engFlag { get; set; }
        public string accHomeNo { get; set; }
        public string accBuildingName { get; set; }
        public string accFloor { get; set; }
        public string accRoom { get; set; }
        public string accMoo { get; set; }
        public string accMooBan { get; set; }
        public string accSoi { get; set; }
        public string accStreet { get; set; }
        public string accTumbol { get; set; }
        public string accAmphur { get; set; }
        public string accProvince { get; set; }
        public string accZipCode { get; set; }
        public string billHomeNo { get; set; }
        public string billBuildingName { get; set; }
        public string billFloor { get; set; }
        public string billRoom { get; set; }
        public string billMoo { get; set; }
        public string billMooBan { get; set; }
        public string billSoi { get; set; }
        public string billStreet { get; set; }
        public string billTumbol { get; set; }
        public string billAmphur { get; set; }
        public string billProvince { get; set; }
        public string billZipCode { get; set; }
        public string userId { get; set; }
        public string dealerLocationCode { get; set; }
        public string ascCode { get; set; }
        public string orderReason { get; set; }
        public string remark { get; set; }
        public string saVatName { get; set; }
        public string saVatAddress1 { get; set; }
        public string saVatAddress2 { get; set; }
        public string saVatAddress3 { get; set; }
        public string saVatAddress4 { get; set; }
        public string saVatAddress5 { get; set; }
        public string saVatAddress6 { get; set; }
        public string contactFirstName { get; set; }
        public string contactLastName { get; set; }
        public string contactTitle { get; set; }
        public string mobileNumberContact { get; set; }
        public string phoneNumberContact { get; set; }
        public string emailAddress { get; set; }
        public string saHomeNo { get; set; }
        public string saBuildingName { get; set; }
        public string saFloor { get; set; }
        public string saRoom { get; set; }
        public string saMoo { get; set; }
        public string saMooBan { get; set; }
        public string saSoi { get; set; }
        public string saStreet { get; set; }
        public string saTumbol { get; set; }
        public string saAmphur { get; set; }
        public string saProvince { get; set; }
        public string saZipCode { get; set; }
        public string orderType { get; set; }
        public string channel { get; set; }
        public string projectName { get; set; }
        public string caBranchNo { get; set; }
        public string saBranchNo { get; set; }
        public string chargeType { get; set; }
        public string sourceSystem { get; set; }
        public string subcontractor { get; set; }
        public string installStaffID { get; set; }
        public string employeeID { get; set; }
        public string billMedia { get; set; }

        public string productInstance { get; set; }
        public string mobileNo { get; set; }
        public string simSerialNo { get; set; }
        public string provinceCode { get; set; }

        public string serviceCode { get; set; }

        public string dpName { get; set; }
        public string dpPort { get; set; }
        public string dslamName { get; set; }
        public string dslamPort { get; set; }
        public string ia { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string networkProvider { get; set; }
        public string orderNo { get; set; }
        public string password { get; set; }
        public string relateNumber { get; set; }
        public string type { get; set; }
        public string contactName { get; set; }
        public string contactMobilePhone { get; set; }
        public string addressId { get; set; }
        public string flowFlag { get; set; }

        public string accessType { get; set; }
        public string brand { get; set; }
        public string macAddress { get; set; }
        public string meterialCode { get; set; }
        public string model { get; set; }
        public string serialNo { get; set; }
        public string subContractor { get; set; }

        public string promotionMain { get; set; }
        public string promotionOntop { get; set; }

        public List<DetailBulkCorpListServiceVdsl> bulkvdsl { get; set; }
        public List<DetailBulkCorpListServiceVdslRouter> bulkvdslrouter { get; set; }
        public List<DetailBulkCorpListServiceAppoint> bulkappoint { get; set; }//17.7
        public List<DetailBulkCorpSffPromotionCur> bulksffpromotioncur { get; set; }
        public List<DetailBulkCorpSffPromotionOntopCur> bulksffpromotionontopcur { get; set; }
        public List<DetailBulkCorpListInstanceCur> bulklistinstancecur { get; set; }
    }
}
