using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class DelectInterfaceLogARCModel
    {
        public DelectInterfaceLogARCModel()
        {
            if (P_EMAIL == null)
                P_EMAIL = new List<DataSendEmailDelectInterface>();
        }

        public string RET_CODE { get; set; }
        public string RET_MSG { get; set; }
        public List<DataSendEmailDelectInterface> P_EMAIL { get; set; }
    }

    public class DataSendEmailDelectInterface
    {
        public string P_SUBJECT { get; set; }
        public string P_BODY_H { get; set; }
        public string P_BODY_RESULT { get; set; }
        public string P_BODY_SUMMARY { get; set; }
        public string P_BODY_SIGNATURE { get; set; }
    }
}
