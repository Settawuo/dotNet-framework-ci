using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SavePackageLocationCommandHandler : ICommandHandler<SavePackageLocationCommand>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _objService;

        public SavePackageLocationCommandHandler(ILogger logger,
            IAirNetEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(SavePackageLocationCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_msg = new OracleParameter();
                o_return_msg.OracleDbType = OracleDbType.Varchar2;
                o_return_msg.Size = 2000;
                o_return_msg.Direction = ParameterDirection.Output;

                var packageRegionObjectModel = new PackageRegionObjectModel();
                packageRegionObjectModel.AIR_PACKAGE_REGION_ARRAY = command.airPackageRegionArray.Select(a => new Air_Package_Region_ArrayMapping
                {
                    service_option = a.SERVICE_OPTION.ToSafeString(),
                    region = a.REGION.ToSafeString(),
                    effective_dtm = a.EFFECTIVE_DTM.ToSafeString(),
                    expire_dtm = a.EXPIRE_DTM.ToSafeString()
                }).ToArray();

                var packageProvinceObjectModel = new PackageProvinceObjectModel();
                packageProvinceObjectModel.AIR_PACKAGE_PROVINCE_ARRAY = command.airPackageProvinceArray.Select(a => new Air_Package_Province_ArrayMapping
                {
                    service_option = a.SERVICE_OPTION.ToSafeString(),
                    region = a.REGION.ToSafeString(),
                    province = a.PROVINCE.ToSafeString(),
                    effective_dtm = a.EFFECTIVE_DTM.ToSafeString(),
                    expire_dtm = a.EXPIRE_DTM.ToSafeString()
                }).ToArray();

                var packageBuildingObjectModel = new PackageBuildingObjectModel();
                packageBuildingObjectModel.AIR_PACKAGE_BUILDING_ARRAY = command.airPackageBuildingArray.Select(a => new Air_Package_Building_ArrayMapping
                {
                    service_option = a.SERVICE_OPTION.ToSafeString(),
                    address_id = a.ADDRESS_ID.ToSafeString(),
                    address_type = a.ADDRESS_TYPE.ToSafeString(),
                    building_name = a.BUILDING_NAME.ToSafeString(),
                    building_no = a.BUILDING_NO.ToSafeString(),
                    building_name_e = a.BUILDING_NAME_E.ToSafeString(),
                    building_no_e = a.BUILDING_NO_E.ToSafeString(),
                    effective_dtm = a.EFFECTIVE_DTM.ToSafeString(),
                    expire_dtm = a.EXPIRE_DTM.ToSafeString()
                }).ToArray();

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var packageRegion = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_PACKAGE_REGION_ARRAY", "AIR_PACKAGE_REGION_ARRAY", packageRegionObjectModel);
                var packageProvince = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_PACKAGE_PROVINCE_ARRAY", "AIR_PACKAGE_PROVINCE_ARRAY", packageProvinceObjectModel);
                var packageBuilding = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_PACKAGE_BUILDING_ARRAY", "AIR_PACKAGE_BUILDING_ARRAY", packageBuildingObjectModel);

                var executeResult = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR012.save_package_location",
                out paramOut,
                   new
                   {
                       p_service_option = command.service_option.ToSafeString(),
                       p_package_code = command.package_code.ToSafeString(),
                       p_user = command.user.ToSafeString(),

                       /// array
                       p_air_package_region_array = packageRegion,
                       p_air_package_province_array = packageProvince,
                       p_air_package_building_array = packageBuilding,
                       /// Out
                       o_return_code = o_return_code,
                       o_return_msg = o_return_msg

                   });
                command.return_msg = o_return_msg.Value.ToSafeString();
                string return_code = o_return_code.Value.ToSafeString();
                command.return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.return_code = -1;
                command.return_msg = "Error call Save PackageLocation service " + ex.Message;
            }
        }

        #region Mapping AIR_PACKAGE_REGION_ARRAY Type Oracle

        public class PackageRegionObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Air_Package_Region_ArrayMapping[] AIR_PACKAGE_REGION_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageRegionObjectModel Null
            {
                get
                {
                    PackageRegionObjectModel obj = new PackageRegionObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AIR_PACKAGE_REGION_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AIR_PACKAGE_REGION_ARRAY = (Air_Package_Region_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_PACKAGE_REGION_RECORD")]
        public class Air_Package_RegionOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Air_Package_Region_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("AIR_PACKAGE_REGION_ARRAY")]
        public class AirPackageRegionObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageRegionObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Air_Package_Region_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Air_Package_Region_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SERVICE_OPTION")]
            public string service_option { get; set; }

            [OracleObjectMappingAttribute("REGION")]
            public string region { get; set; }

            [OracleObjectMappingAttribute("EFFECTIVE_DTM")]
            public string effective_dtm { get; set; }

            [OracleObjectMappingAttribute("EXPIRE_DTM")]
            public string expire_dtm { get; set; }

            #endregion Attribute Mapping

            public static Air_Package_Region_ArrayMapping Null
            {
                get
                {
                    Air_Package_Region_ArrayMapping obj = new Air_Package_Region_ArrayMapping();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, "SERVICE_OPTION", service_option);
                OracleUdt.SetValue(con, udt, "REGION", region);
                OracleUdt.SetValue(con, udt, "EFFECTIVE_DTM", effective_dtm);
                OracleUdt.SetValue(con, udt, "EXPIRE_DTM", expire_dtm);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  AIR_PACKAGE_REGION_ARRAY Type Oracle

        #region Mapping AIR_PACKAGE_PROVINCE_ARRAY Type Oracle

        public class PackageProvinceObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Air_Package_Province_ArrayMapping[] AIR_PACKAGE_PROVINCE_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageProvinceObjectModel Null
            {
                get
                {
                    PackageProvinceObjectModel obj = new PackageProvinceObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AIR_PACKAGE_PROVINCE_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AIR_PACKAGE_PROVINCE_ARRAY = (Air_Package_Province_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_PACKAGE_PROVINCE_RECORD")]
        public class Air_Package_ProvinceOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Air_Package_Province_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("AIR_PACKAGE_PROVINCE_ARRAY")]
        public class AirPackageProvinceObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageProvinceObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Air_Package_Province_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Air_Package_Province_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SERVICE_OPTION")]
            public string service_option { get; set; }

            [OracleObjectMappingAttribute("REGION")]
            public string region { get; set; }

            [OracleObjectMappingAttribute("PROVINCE")]
            public string province { get; set; }

            [OracleObjectMappingAttribute("EFFECTIVE_DTM")]
            public string effective_dtm { get; set; }

            [OracleObjectMappingAttribute("EXPIRE_DTM")]
            public string expire_dtm { get; set; }

            #endregion Attribute Mapping

            public static Air_Package_Province_ArrayMapping Null
            {
                get
                {
                    Air_Package_Province_ArrayMapping obj = new Air_Package_Province_ArrayMapping();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, "SERVICE_OPTION", service_option);
                OracleUdt.SetValue(con, udt, "REGION", region);
                OracleUdt.SetValue(con, udt, "PROVINCE", province);
                OracleUdt.SetValue(con, udt, "EFFECTIVE_DTM", effective_dtm);
                OracleUdt.SetValue(con, udt, "EXPIRE_DTM", expire_dtm);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  AIR_PACKAGE_PROVINCE_ARRAY Type Oracle

        #region Mapping AIR_PACKAGE_BUILDING_ARRAY Type Oracle

        public class PackageBuildingObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Air_Package_Building_ArrayMapping[] AIR_PACKAGE_BUILDING_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageBuildingObjectModel Null
            {
                get
                {
                    PackageBuildingObjectModel obj = new PackageBuildingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AIR_PACKAGE_BUILDING_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AIR_PACKAGE_BUILDING_ARRAY = (Air_Package_Building_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_PACKAGE_BUILDING_RECORD")]
        public class Air_Package_BuildingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Air_Package_Building_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("AIR_PACKAGE_BUILDING_ARRAY")]
        public class AirPackageBuildingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageBuildingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Air_Package_Building_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Air_Package_Building_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SERVICE_OPTION")]
            public string service_option { get; set; }

            [OracleObjectMappingAttribute("ADDRESS_ID")]
            public string address_id { get; set; }

            [OracleObjectMappingAttribute("ADDRESS_TYPE")]
            public string address_type { get; set; }

            [OracleObjectMappingAttribute("BUILDING_NAME")]
            public string building_name { get; set; }

            [OracleObjectMappingAttribute("BUILDING_NO")]
            public string building_no { get; set; }

            [OracleObjectMappingAttribute("BUILDING_NAME_E")]
            public string building_name_e { get; set; }

            [OracleObjectMappingAttribute("BUILDING_NO_E")]
            public string building_no_e { get; set; }

            [OracleObjectMappingAttribute("EFFECTIVE_DTM")]
            public string effective_dtm { get; set; }

            [OracleObjectMappingAttribute("EXPIRE_DTM")]
            public string expire_dtm { get; set; }

            #endregion Attribute Mapping

            public static Air_Package_Building_ArrayMapping Null
            {
                get
                {
                    Air_Package_Building_ArrayMapping obj = new Air_Package_Building_ArrayMapping();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, "SERVICE_OPTION", service_option);
                OracleUdt.SetValue(con, udt, "ADDRESS_ID", address_id);
                OracleUdt.SetValue(con, udt, "ADDRESS_TYPE", address_type);
                OracleUdt.SetValue(con, udt, "BUILDING_NAME", building_name);
                OracleUdt.SetValue(con, udt, "BUILDING_NO", building_no);
                OracleUdt.SetValue(con, udt, "BUILDING_NAME_E", building_name_e);
                OracleUdt.SetValue(con, udt, "BUILDING_NO_E", building_no_e);
                OracleUdt.SetValue(con, udt, "EFFECTIVE_DTM", effective_dtm);
                OracleUdt.SetValue(con, udt, "EXPIRE_DTM", expire_dtm);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  AIR_PACKAGE_BUILDING_ARRAY Type Oracle
    }
}
