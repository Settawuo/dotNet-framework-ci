using System.Collections.Generic;
using WBBEntity.PanelModels.One2NetModels.InWebServices;

namespace WBBEntity.PanelModels.One2NetModels
{
    public class RegisterPanelModel : LoginRegisterModel
    {

        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string CARD_TYPE { get; set; }
        public string CARD_NO { get; set; }
        public string BirthDay { get; set; }

        public string HOME_NO { get; set; }
        public string MOO { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string PROVINCE { get; set; }
        public string ZIPCODE { get; set; }
        public string HOME_PHONE_NO { get; set; }
        public string HOME_PHONE_EXT_NO { get; set; }
        public string MOBILE_NO { get; set; }

        public string DORMITARY_NAME { get; set; }
        public string DORMITARY_ROOM { get; set; }
        public string DORMITARY_NO { get; set; }

        public string PackageCode { get; set; }
        public string MappingCode { get; set; }
        public string old_registerstatus { get; set; }
        public int CurrentCulture { get; set; }
        public List<PackageByServiceModel> Packlist_ALL { get; set; }
        public List<PackageByServiceModel> Packlist_DISPLAY { get; set; }
        public bool ChkStatusRegister { get; set; }

    }
}
