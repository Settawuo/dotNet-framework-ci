using System.Collections.Generic;
using WBBContract.Queries.Commons;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices.FBSS
{
    public class GetFBSSAppointment : IQuery<List<FBSSTimeSlot>>
    {
        public GetFBSSAppointment()
        {
            ASSIGN_CONDITION_LIST = new List<ASSIGN_CONDITION_ATTR>();
        }

        public string InstallationDate { get; set; }
        public string AccessMode { get; set; }
        public string ProductSpecCode { get; set; }
        public string ExtendingAttributes { get; set; }

        /// <summary>
        /// contain a list of address id
        /// </summary>
        public string AddressId { get; set; }
        public long Days { get; set; }

        /// <summary>
        /// Parameter for call wbb package add by Yeen
        /// </summary>
        public string Language { get; set; }
        public string Service_Code { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Postal_Code { get; set; }

        public LineType LineSelect { get; set; }

        // Update 17.2
        public string Transaction_Id { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }

        // Update 18.8 
        public string SubAccessMode { get; set; }

        public string TaskType { get; set; }

        //20.5 : ServiceLevel
        public List<ASSIGN_CONDITION_ATTR> ASSIGN_CONDITION_LIST { get; set; }

        public string AisAirNumber { get; set; }
        public string PlayBoxCountOld { get; set; }
        public string PlayBoxCountNew { get; set; }

        //R21.3
        public string TimeAdd { get; set; }
        public string ActionTimeSlot { get; set; }
        public string NumTimeSlot { get; set; }

        //R22.03 Topup Replace
        public string SymptonCodePBreplace { get; set; }
    }

    public enum LineType
    {
        Line1 = 1,
        line2 = 2
    }

    public class GetAppointmentChageProQuery : IQuery<List<FBSSTimeSlotChangePro>>
    {
        public string INSTALLATION_DATE { get; set; }
        public string ACCESS_MODE { get; set; }
        public string PROD_SPEC_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string DAYS { get; set; }
        public string SUBDISTRICT { get; set; }
        public string POSTCODE { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string TaskType { get; set; }
        public string FULL_URL { get; set; }
    }

    public class GetDataForAppointmentQuery : IQuery<List<DataForAppointment>>
    {
        public string NON_MOBILE { get; set; }
        public string ID_CARD { get; set; }
        public string BILL_CYCLE { get; set; }
        public string LANQUAGE_SCREEN { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string P_ADDRESS_ID { get; set; }
        public string INSTALL_ADDRESS_1 { get; set; }
        public string INSTALL_ADDRESS_2 { get; set; }
        public string INSTALL_ADDRESS_3 { get; set; }
        public string INSTALL_ADDRESS_4 { get; set; }
        public string INSTALL_ADDRESS_5 { get; set; }
        public string FULL_URL { get; set; }
    }

    public class GetDataForReserveTimeslotQuery : IQuery<List<DataForReserveTimeslot>>
    {
        public string NON_MOBILE { get; set; }
        public string ID_CARD { get; set; }
        public string APPOINTMENT_DATE { get; set; }
        public string TIME_SLOT { get; set; }
        public string LANQUAGE_SCREEN { get; set; }
        public string P_ADDRESS_ID { get; set; }
        public string INSTALL_ADDRESS_1 { get; set; }
        public string INSTALL_ADDRESS_2 { get; set; }
        public string INSTALL_ADDRESS_3 { get; set; }
        public string INSTALL_ADDRESS_4 { get; set; }
        public string INSTALL_ADDRESS_5 { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FULL_URL { get; set; }
    }

    public class CheckNoToPBQuery : IQuery<List<CheckNoToPB>>
    {
        public string p_sff_promocode_current { get; set; }
        public string p_sff_promocode_futurn { get; set; }
        public string P_status_option { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FULL_URL { get; set; }
    }

    public class CheckUnlockOrderQuery : IQuery<string>
    {
        public string p_sff_promocode_current { get; set; }
        public string p_sff_promocode_futurn { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FULL_URL { get; set; }
    }

    public class GetInputGetListPackageBySFFPROMOQuery : IQuery<List<GetInputGetListPackageBySFFPROMO>>
    {
        public string p_sff_promocode_current { get; set; }
        public string p_sff_promocode_futurn { get; set; }
        public string P_NON_MOBILE { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FULL_URL { get; set; }
    }

    public class GetPackageListBySFFPromoForChangProQuery : IQuery<List<PackageModel>>
    {
        public string P_SFF_PROMOCODE { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; }
        public string P_OWNER_PRODUCT { get; set; }
        public string EXISTING_REQ { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FULL_URL { get; set; }
    }

    public class GetDataDATAILChangeproQuery : IQuery<DataDATAILChangepro>
    {
        public string P_BILL_CYCLE { get; set; }
        public string P_APPOINTMENT_DATE { get; set; }
        public string P_TIME_SLOT { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FULL_URL { get; set; }
    }
}
