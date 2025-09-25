using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetCreateOrderMeshRentalModel
    {
        public string sffOrderNo { get; set; }
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public List<OrdServiceRentalData> list_ord_service_rental { get; set; }
        public List<OrdServiceData> list_ord_service { get; set; }
        public List<OrdServiceAppointAttributeData> list_ord_appoint_attribute { get; set; }
        public List<OrdServiceRentalAttributeData> list_ord_rental_attribute { get; set; }
        public List<OrdderfeeData> list_ordderfee { get; set; }
    }

    public class OrdServiceRentalData
    {
        public string mobileNo { get; set; }
        public string orderChannel { get; set; }
        public string locationCd { get; set; }
        public string ascCode { get; set; }
        public string orderType { get; set; }
        public string userName { get; set; }
        public string referenceNo { get; set; }
        public string employeeID { get; set; }
        public string actionStatus { get; set; }

    }

    public class OrdServiceData
    {
        public string serviceCode { get; set; }
        public string curAttriService { get; set; }

    }

    public class OrdServiceAppointAttributeData
    {
        public string serviceAttributeName { get; set; }
        public string serviceAttributeValue { get; set; }

    }

    public class OrdServiceRentalAttributeData
    {
        public string serviceAttributeName { get; set; }
        public string serviceAttributeValue { get; set; }

    }

    public class OrdderfeeData
    {
        public string parameterType { get; set; }
        public string parameterName { get; set; }
        public string parameterValue { get; set; }
    }
}
