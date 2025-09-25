using System.Collections.Generic;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.QueryModels.WebServices
{
    public class GetPackageDetailByNonMobileQueryModel
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<WelcomePackageMain> PACKAGE_MAIN { get; set; }
        public List<WelcomePackageDiscount> PACKAGE_DISCOUNT { get; set; }
        public List<WelcomePackageContent> PACKAGE_CONTENT { get; set; }
        public List<WelcomePackageVas> PACKAGE_VAS { get; set; }
        public List<WelcomePackageInstall> PACKAGE_INSTALL { get; set; }

        public List<evESQueryPersonalInformationModel> PACKAGE_ALL_OPTION2 { get; set; }
        public List<evESQueryPersonalInformationModel> PACKAGE_ALL_OPTION3 { get; set; }
        public string discountblob { get; set; }
        public string contentpiblob { get; set; }
        public string blobmeshpic { get; set; }
        public string blobplaybox { get; set; }

    }

    public class WelcomePackageMain
    {
        public string FIBRENET_ID { get; set; }
        public string PROMOTION_NAME { get; set; }
        public string PROMOTION_DISPLAY { get; set; }
        public string PROMOTION_EXPIRE { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
    }

    public class WelcomePackageDiscount
    {
        public string FIBRENET_ID { get; set; }
        public string PROMOTION_DISPLAY { get; set; }
        public string PROMOTION_DETALIL { get; set; }
        public string PROMOTION_PIC { get; set; }
    }

    public class WelcomePackageContent
    {
        public string FIBRENET_ID { get; set; }
        public string PROMOTION_DISPLAY { get; set; }
        public string PROMOTION_DETALIL { get; set; }
        public string PROMOTION_PIC { get; set; }
    }

    public class WelcomePackageVas
    {
        public string FIBRENET_ID { get; set; }
        public string PROMOTION_DISPLAY { get; set; }
        public string PROMOTION_DETALIL { get; set; }
        public string PROMOTION_PIC { get; set; }
    }

    public class WelcomePackageInstall
    {
        public string FIBRENET_ID { get; set; }
        public string PROMOTION_DISPLAY { get; set; }
        public string PROMOTION_DETALIL { get; set; }
    }


}
