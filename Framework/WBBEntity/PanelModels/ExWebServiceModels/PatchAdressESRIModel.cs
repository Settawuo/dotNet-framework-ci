using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class PatchAdressESRIModel
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<PatchAdressESRIData> list_address { get; set; }
    }

    public class PatchAdressESRIData
    {
        public string P_SUB_DISTRICT { get; set; }
        public string P_DISTRICT { get; set; }
        public string P_PROVINCE { get; set; }
    }
}
