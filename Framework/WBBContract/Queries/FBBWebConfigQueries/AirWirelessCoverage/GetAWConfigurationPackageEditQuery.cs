using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{

    public class GetAWConfigurationPackageDetailEditQuery : IQuery<List<ConfigurationPackageDetail>>
    {
        public string PromotionCode { get; set; }
        public string PackageCode { get; set; }
    }

    public class GetAWConfigurationPackageProductTypeEditQuery : IQuery<List<ProductTypePackage>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWConfigurationPackageVendorEditQuery : IQuery<List<VendorPartner>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWPackageForMappingQuery : IQuery<List<PackageTypeSearch>>
    {
        public string PackageCode { get; set; }
        public string ProductType { get; set; }
    }

    public class GetAWPackageForMappingUseQuery : IQuery<List<PackageTypeUse>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWChannelGroupQuery : IQuery<List<ChannelGroup>>
    {

    }

    public class GetAWPackageUserGroupQuery : IQuery<List<PackageUserGroup>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWZipCodeQuery : IQuery<List<ZipCode>>
    {

    }

    public class GetAWZipCodeProvinceQuery : IQuery<List<ZipCodeProvince>>
    {
        public string[] RegionNames { get; set; }
    }

    public class GetAWRegionQuery : IQuery<List<RegionTable>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWProvinceUseByPackageCodeQuery : IQuery<List<ProvinceTable>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWBuildingByBuildingNameAndTechnologyQuery : IQuery<List<BuildingDetail>>
    {
        public string Building { get; set; }
        public string Technology { get; set; }
    }

    public class GetAWBuildingByPackageCodeQuery : IQuery<List<BuildingDetail>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWSummaryPackageMasterQuery : IQuery<List<SummaryPackageMaster>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWSummaryPackageMappingQuery : IQuery<List<SummaryPackageMapping>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWSummaryPackageUserQuery : IQuery<List<SummaryPackageUser>>
    {
        public string PackageCode { get; set; }
    }

    public class GetAWSummaryPackageLocQuery : IQuery<List<SummaryPackageLoc>>
    {
        public string PackageCode { get; set; }
    }
}
