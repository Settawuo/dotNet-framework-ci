using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetMobileDataUsageQuery : IQuery<MobileDataUsageModel>
    {
        /// <summary>
        /// เบอร์โทรศัพท์
        /// </summary>
        public string MOBILE_NO { get; set; }
        /// <summary>
        /// ประเภท POSTPAID/PERPAID
        /// </summary>
        public string networkType { get; set; }
    }
}
