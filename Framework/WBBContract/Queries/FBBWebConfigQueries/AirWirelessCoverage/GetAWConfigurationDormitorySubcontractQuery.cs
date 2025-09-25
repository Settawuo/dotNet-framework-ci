using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetAWConfigurationDormitorySubcontractQuery : IQuery<List<ConfigurationDormitorySubcontract>>
    {
        public string Region { get; set; }
        public string DormitoryProvince { get; set; }
        public string DormitoryName { get; set; }
        public string User { get; set; }

        public string SubContractFlag { get; set; }
    }

    public class GetAWConfigurationAddressIDQuery : IQuery<List<ConfigurationAddressID>>
    {
        public string Region { get; set; }
        public string DormitoryProvince { get; set; }
        public string DormitoryName { get; set; }
        public string User { get; set; }

        public string FibrenetIDFlag { get; set; }
    }
    public class GetAWConfigurationOnOffServicetQuery : IQuery<List<ConfigurationOnOffServices>>
    {
        public string Region { get; set; }
        public string User { get; set; }
        public string DormitoryName { get; set; }
        public string BuildingNo { get; set; }
        public string DormitoryProvince { get; set; }

    }

    public class GetAWConfigurationDormitorySubcontractByIDQuery : IQuery<ConfigurationDormitorySubcontract>
    {
        public string p_dormitory_id { get; set; }
    }
}
