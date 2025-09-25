using System.Collections.Generic;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Commands
{
    public class CreateOrderChangePromotionCommand
    {
        public string OrderNo { get; set; }
        public string RelateMobile { get; set; }
        public string NonMobileNo { get; set; }
        public string ProjectName { get; set; }
        public List<PromotionAction> ListAction { get; set; }
        public string mobileNumberContact { get; set; }
        public string acTion { get; set; }
        public string oldRelateMobile { get; set; }

        public string VALIDATE_FLAG { get; set; }
        public string ERROR_MSG { get; set; }

        // Update 17.2
        public string client_ip { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }

        // Update 18.3
        public string locationCd { get; set; }
        public string ascCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }

        // Update 18.10
        public PromotionPlayBox promotionPlayBox { get; set; }
        public string servicCodeApp { get; set; }
        public string appointmentDate { get; set; }
        public string reservedId { get; set; }
        public string timeslot { get; set; }

        //AWARE_R20.02
        public string GOTO_TOPUP { get; set; }

        //R20.6
        public List<ListChangePackageModel> ListChangePackageModel { get; set; }
        public string BUNDLING_ACTION { get; set; }
        public string OLD_RELATE_MOBILE { get; set; }
        public string MOBILE_CONTACT { get; set; }
        public string new_project_name { get; set; }
        public string new_project_name_opt { get; set; }
        public string new_mobile_check_right { get; set; }
        public string new_mobile_check_right_opt { get; set; }
        public string new_mobile_get_benefit { get; set; }
        public string new_mobile_get_benefit_opt { get; set; }

        //R22.03 Topup Replace
        public string GOTO_TOPUP_REPLACE { get; set; }
    }

    public class PromotionAction
    {
        public string PromotionCode { get; set; }
        public string ActionStatus { get; set; }
        public string Overrule { get; set; }

        // R20.6 Add by Aware : Atipon
        public string SendSffFlag { get; set; }
    }

    public class PromotionPlayBox
    {
        public string serviceCode { get; set; }
        public string accessMode { get; set; }
        public string addressId { get; set; }
        public string appointmentDate { get; set; }
        public string contactMobilePhone { get; set; }
        public string contactName { get; set; }
        public string contentName { get; set; }
        public string installAddress1 { get; set; }
        public string installAddress2 { get; set; }
        public string installAddress3 { get; set; }
        public string installAddress4 { get; set; }
        public string installAddress5 { get; set; }
        public string relateMobile { get; set; }
        public string serialNumber { get; set; }
        public string timeSlot { get; set; }
        public string installedFlag { get; set; }
    }

    public class CreateOrderMeshPromotionCommand
    {
        public string NonMobileNo { get; set; }
        public GetOrderChangeServiceModel GetOrderChangeService { get; set; }
        public string client_ip { get; set; }
        public string FullUrl { get; set; }
        public string VALIDATE_FLAG { get; set; }
        public string ERROR_MSG { get; set; }
        public string sffOrderNo { get; set; }
    }
}
