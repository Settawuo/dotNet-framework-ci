using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetAWConfigurationEventQuery : IQuery<List<ConfigurationEventData>>
    {
        public string EventCode { get; set; }
    }

    public class GetAWConfigurationEventSearchQuery : IQuery<List<ConfigurationEventSearchData>>
    {
        public string EventCode { get; set; }

    }

    public class GetCapabilityQuery : IQuery<List<CapabilityData>>
    {
        public string Technology { get; set; }
        public string Lang_Flag { get; set; }
        public string Post_Code { get; set; }
        public string Sub_District { get; set; }
        public string Event_Start_Date { get; set; }
        public string Event_End_Date { get; set; }
    }

    public class ReserveCapabilityQuery : IQuery<string>
    {
        public string EventCode { get; set; }
        public string Service_Option { get; set; }
        public ReserveSubcontract ReserveSubcontract { get; set; }

    }

    public class ReserveSubcontract
    {
        public string Subcontract_Location_Code { get; set; }
        public string Subcontract_Team_Id { get; set; }
        public string Event_Date_From { get; set; }
        public string Event_Date_To { get; set; }
        public string Capacity_Amount { get; set; }
        public string Sub_District { get; set; }
        public string Post_Code { get; set; }
    }

    public class ValidateEngineerQuery : IQuery<List<ValidateStaff>>
    {
        public string Install_Staff_Id { get; set; }
        public string Install_Staff_Name { get; set; }
        public string Event_Start_Date { get; set; }
        public string Event_End_Date { get; set; }
    }
}
