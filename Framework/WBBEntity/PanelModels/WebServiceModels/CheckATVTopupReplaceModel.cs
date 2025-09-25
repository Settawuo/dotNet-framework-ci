using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class CheckATVTopupReplaceModel
    {
        public CheckATVTopupReplaceModel()
        {
            if (RETURN_SERIAL_CURROR == null)
                RETURN_SERIAL_CURROR = new List<SerialATV>();
        }

        public int RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string RETURN_ERROR_FLAG { get; set; }
        public string RETURN_ERROR_MESSAGE { get; set; }
        public int RETURN_DROPDOWN { get; set; }
        public string RETURN_SYMPTON_CODE { get; set; }
        public List<SerialATV> RETURN_SERIAL_CURROR { get; set; }
    }

    public class SerialATV
    {
        public string FIBRENETID { get; set; }
        public string SERIAL_NO { get; set; }
    }
}
