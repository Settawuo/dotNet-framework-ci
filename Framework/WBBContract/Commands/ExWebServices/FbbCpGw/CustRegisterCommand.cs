using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Commands.ExWebServices.FbbCpGw
{
    public class CustRegisterCommand : CpGateWayCommandBase
    {
        public CustRegisterInfoModel CustRegisterInfoModel { get; set; }

        public CoveragePanelModel CoveragePanelModel { get; set; }

        public string PhoneFlag { get; set; }

        public string TimeSlot { get; set; }

        public string InstallCapacity { get; set; }

        //public decimal AddressID { get; set; }

        //public string AccessMode { get; set; }

        public decimal OrderRefId { get; set; }

        //public string ServiceCode { get; set; }

        public string Guid { get; set; }

        public string TimeSlotID { get; set; }


        // Update R16.1
        public string GIFT_VOUCHER { get; set; }
        public string SUB_LOCATION_ID { get; set; }
        public string SUB_CONTRACT_NAME { get; set; }
        public string INSTALL_STAFF_ID { get; set; }
        public string INSTALL_STAFF_NAME { get; set; }
    }
}