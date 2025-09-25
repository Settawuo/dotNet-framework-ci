using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBEntity.PanelModels
{
    public class SummaryPanelModel : PanelModelBase
    {
        public SummaryPanelModel()
        {
            this.PackageModel = new PackageModel();
            this.PDFPackageModel = new PDFPackageModel();
            this.PackageModelList = new List<PackageModel>();
            //this.SaveorderPackagemodelList = new List<SaveorderPackagemodel>();
        }

        public string H_FBB004 { get; set; }
        public string L_CONDITION { get; set; }
        public string L_DOCUMENT { get; set; }
        public string L_AIRNET_EMAIL { get; set; }
        public string L_ACCEPTED { get; set; }
        public string B_SUBMIT { get; set; }
        public string L_INSTALL_DETAIL { get; set; }
        public string L_BILLING_DETAIL { get; set; }
        public string L_POP_WIRE { get; set; }
        public string L_POP_WIRELESS { get; set; }
        public bool L_SEND_EMAIL { get; set; }
        public bool L_SAVE { get; set; }
        public string VAS_FLAG { get; set; }
        public string TOPUP { get; set; }

        public string VOIP_FLAG { get; set; }

        //public List<SaveorderPackagemodel> SaveorderPackagemodelList { get; set; }
        public PackageModel PackageModel { get; set; }
        public List<PackageModel> PackageModelList { get; set; }
        public PDFPackageModel PDFPackageModel { get; set; }

        //16.8
        public string RESERVED_ID { get; set; }
        //22.03 TupupReplace
        public DetailCalliMTopupReplace DetailCalliMPBReplace { get; set; }
    }

    public class DetailCalliMTopupReplace
    {
        public string FIBRENET_ID { get; set; }
        public string CONTRACT_NO { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string SERIAL_NO { get; set; }
        public string RESERVED_ID { get; set; }
        public string TIME_SLOT { get; set; }
        public string DATE_TIME_SLOT { get; set; }
        public string ACCESS_MODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string COUNT_PB { get; set; }
    }
}
