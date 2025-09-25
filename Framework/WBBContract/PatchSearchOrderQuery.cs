using System.Collections.Generic;

namespace WBBContract
{
    public class PatchSearchOrderQuery : IQuery<List<RetPatchSearchOrders>>
    {
        public List<string> SerialNo { get; set; }
    }
    public class PatchSearchOrderHVRQuery : IQuery<List<PatchSearchOrdersHVR>>
    {
        public List<string> SerialNo { get; set; }
        public List<PatchSearchOrdersHVR> SerialNo2 { get; set; }
    }

    public class SerialNumber
    {
        public string SerialNo { get; set; }
    }


    public class RetPatchSearchOrders
    {
        public int NO { get; set; }
        public string SN { get; set; }
        public string INTERNET_NO { get; set; }
        public string ORDER_NO { get; set; }
        public string TECHNOLOGY { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SYMPTOM_GROUP { get; set; }
        public string ORDER_DATE { get; set; }
        public string STATUS { get; set; }
        public string LAST_UPDATE_DATE { get; set; }

    }

    public class PatchSearchOrdersHVR
    {
        public int NO { get; set; }
        public string SN { get; set; }
        public string INTERNET_NO { get; set; }
        public string ORDER_NO { get; set; }
        public string TECHNOLOGY { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SYMPTOM_GROUP { get; set; }
        public string ORDER_DATE { get; set; }
        public string STATUS { get; set; }
        public string LAST_UPDATE_DATE { get; set; }
    }
}
