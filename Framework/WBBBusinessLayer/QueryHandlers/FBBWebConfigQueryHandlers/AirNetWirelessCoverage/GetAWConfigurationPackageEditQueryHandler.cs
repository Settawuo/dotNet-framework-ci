using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetAWConfigurationPackageDetailEditQueryHandler : IQueryHandler<GetAWConfigurationPackageDetailEditQuery, List<ConfigurationPackageDetail>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ConfigurationPackageDetail> _ConfigurationPackageDetail;

        public GetAWConfigurationPackageDetailEditQueryHandler(ILogger logger,
            IAirNetEntityRepository<ConfigurationPackageDetail> ConfigurationPackageDetail)
        {
            _logger = logger;
            _ConfigurationPackageDetail = ConfigurationPackageDetail;

        }

        public List<ConfigurationPackageDetail> Handle(GetAWConfigurationPackageDetailEditQuery query)
        {
            return Version1(query);
        }

        private List<ConfigurationPackageDetail> Version1(GetAWConfigurationPackageDetailEditQuery query)
        {

            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<ConfigurationPackageDetail> executeResult = _ConfigurationPackageDetail.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_NEW_PACKAGE_FOR_EDIT",
                            new
                            {
                                p_sff_product_code = query.PromotionCode,
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }

    public class GetAWConfigurationPackageProductTypeEditQueryHandler : IQueryHandler<GetAWConfigurationPackageProductTypeEditQuery, List<ProductTypePackage>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ProductTypePackage> _ProductTypePackage;

        public GetAWConfigurationPackageProductTypeEditQueryHandler(ILogger logger,
            IAirNetEntityRepository<ProductTypePackage> ProductTypePackage)
        {
            _logger = logger;
            _ProductTypePackage = ProductTypePackage;

        }

        public List<ProductTypePackage> Handle(GetAWConfigurationPackageProductTypeEditQuery query)
        {
            return Version1(query);
        }

        private List<ProductTypePackage> Version1(GetAWConfigurationPackageProductTypeEditQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults_detail = new OracleParameter();
                ioResults_detail.ParameterName = "ioResults_detail";
                ioResults_detail.OracleDbType = OracleDbType.RefCursor;
                ioResults_detail.Direction = ParameterDirection.Output;


                List<ProductTypePackage> executeResult = _ProductTypePackage.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_PACKAGE_DETAIL",
                            new
                            {
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,
                                ioResults_detail = ioResults_detail


                            }).ToList();

                var return_code = o_return_code.Value.ToString() != "null" ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }

    public class GetAWConfigurationPackageVendorEditQueryHandler : IQueryHandler<GetAWConfigurationPackageVendorEditQuery, List<VendorPartner>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<VendorPartner> _VendorPartner;

        public GetAWConfigurationPackageVendorEditQueryHandler(ILogger logger,
            IAirNetEntityRepository<VendorPartner> VendorPartner)
        {
            _logger = logger;
            _VendorPartner = VendorPartner;

        }

        public List<VendorPartner> Handle(GetAWConfigurationPackageVendorEditQuery query)
        {
            return Version1(query);
        }

        private List<VendorPartner> Version1(GetAWConfigurationPackageVendorEditQuery query)
        {

            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults_vendor = new OracleParameter();
                ioResults_vendor.ParameterName = "ioResults_vendor";
                ioResults_vendor.OracleDbType = OracleDbType.RefCursor;
                ioResults_vendor.Direction = ParameterDirection.Output;

                List<VendorPartner> executeResult = _VendorPartner.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_OWNER_PRODUCT",
                            new
                            {
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults_vendor = ioResults_vendor

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }

    public class GetAWBuildingByBuildingNameAndTechnologyQueryHandler : IQueryHandler<GetAWBuildingByBuildingNameAndTechnologyQuery, List<BuildingDetail>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<BuildingDetail> _BuildingDetail;

        public GetAWBuildingByBuildingNameAndTechnologyQueryHandler(ILogger logger,
            IEntityRepository<BuildingDetail> BuildingDetail)
        {
            _logger = logger;
            _BuildingDetail = BuildingDetail;
        }

        public List<BuildingDetail> Handle(GetAWBuildingByBuildingNameAndTechnologyQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<BuildingDetail> executeResult = _BuildingDetail.ExecuteReadStoredProc("WBB.PKG_FBBOR012.LIST_BUILDING_VILLAGE",
                            new
                            {
                                p_building = query.Building,
                                p_technology = query.Technology,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }

    public class GetAWBuildingByPackageCodeQueryHandler : IQueryHandler<GetAWBuildingByPackageCodeQuery, List<BuildingDetail>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<BuildingDetail> _BuildingDetail;

        public GetAWBuildingByPackageCodeQueryHandler(ILogger logger,
            IAirNetEntityRepository<BuildingDetail> BuildingDetail)
        {
            _logger = logger;
            _BuildingDetail = BuildingDetail;
        }

        public List<BuildingDetail> Handle(GetAWBuildingByPackageCodeQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<BuildingDetail> executeResult = _BuildingDetail.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_PACKAGE_LOCATION",
                            new
                            {
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class GetAWSummaryPackageMasterQueryHandler : IQueryHandler<GetAWSummaryPackageMasterQuery, List<SummaryPackageMaster>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<SummaryPackageMaster> _SummaryPackageMaster;

        public GetAWSummaryPackageMasterQueryHandler(ILogger logger,
            IAirNetEntityRepository<SummaryPackageMaster> SummaryPackageMaster)
        {
            _logger = logger;
            _SummaryPackageMaster = SummaryPackageMaster;
        }

        public List<SummaryPackageMaster> Handle(GetAWSummaryPackageMasterQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<SummaryPackageMaster> executeResult = _SummaryPackageMaster.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_SUMMARY_PACKAGE_MASTER",
                            new
                            {
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class GetAWSummaryPackageMappingQueryHandler : IQueryHandler<GetAWSummaryPackageMappingQuery, List<SummaryPackageMapping>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<SummaryPackageMapping> _SummaryPackageMapping;

        public GetAWSummaryPackageMappingQueryHandler(ILogger logger,
            IAirNetEntityRepository<SummaryPackageMapping> SummaryPackageMapping)
        {
            _logger = logger;
            _SummaryPackageMapping = SummaryPackageMapping;
        }

        public List<SummaryPackageMapping> Handle(GetAWSummaryPackageMappingQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<SummaryPackageMapping> executeResult = _SummaryPackageMapping.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_SUMMARY_PACKAGE_MAPPING",
                            new
                            {
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class GetAWSummaryPackageUserQueryHandler : IQueryHandler<GetAWSummaryPackageUserQuery, List<SummaryPackageUser>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<SummaryPackageUser> _SummaryPackageUser;

        public GetAWSummaryPackageUserQueryHandler(ILogger logger,
            IAirNetEntityRepository<SummaryPackageUser> SummaryPackageUser)
        {
            _logger = logger;
            _SummaryPackageUser = SummaryPackageUser;
        }

        public List<SummaryPackageUser> Handle(GetAWSummaryPackageUserQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<SummaryPackageUser> executeResult = _SummaryPackageUser.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_SUMMARY_PACKAGE_USER",
                            new
                            {
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class GetAWSummaryPackageLocQueryHandler : IQueryHandler<GetAWSummaryPackageLocQuery, List<SummaryPackageLoc>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<SummaryPackageLoc> _SummaryPackageLoc;

        public GetAWSummaryPackageLocQueryHandler(ILogger logger,
            IAirNetEntityRepository<SummaryPackageLoc> SummaryPackageLoc)
        {
            _logger = logger;
            _SummaryPackageLoc = SummaryPackageLoc;
        }

        public List<SummaryPackageLoc> Handle(GetAWSummaryPackageLocQuery query)
        {
            try
            {

                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "ioResults";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<SummaryPackageLoc> executeResult = _SummaryPackageLoc.ExecuteReadStoredProc("AIR_ADMIN.PKG_AIROR012.LIST_SUMMARY_PACKAGE_LOC",
                            new
                            {
                                p_package_code = query.PackageCode,

                                /// return //////
                                o_return_code = o_return_code,

                                ioResults = ioResults

                            }).ToList();

                var Return_Code = o_return_code.Value != null ? Convert.ToInt32(o_return_code.Value.ToSafeString()) : -1;

                return executeResult;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}


