using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckCardByLaserQuery : IQuery<CheckCardByLaserMappingModel>
    {
        //Header
        public string xssborigin { get; set; }
        public string xssbserviceorigin { get; set; }
        public string xssbtransactionid { get; set; }
        public string xssborderchannel { get; set; }
        public string xssbversion { get; set; }
        public string ContentType { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string Lang { get; set; }

        //Body
        public CheckCardByLaserBody Body { get; set; }
    }

    public class CheckCardByLaserBody
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string idCardNo { get; set; }
        public string laserID { get; set; }
        public string birthday { get; set; }
    }
}
