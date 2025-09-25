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
    public class SaveConfigPackagePage1CommandHandler : ICommandHandler<SaveConfigPackagePage1Command>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<string> _objService;

        public SaveConfigPackagePage1CommandHandler(ILogger logger,
            IAirNetEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(SaveConfigPackagePage1Command command)
        {
            InterfaceLogCommand log = null;
            try
            {
                var o_package_code = new OracleParameter();
                o_package_code.OracleDbType = OracleDbType.Varchar2;
                o_package_code.Size = 10;
                o_package_code.Direction = ParameterDirection.Output;

                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_msg = new OracleParameter();
                o_return_msg.OracleDbType = OracleDbType.Varchar2;
                o_return_msg.Size = 2000;
                o_return_msg.Direction = ParameterDirection.Output;

                var packageDetailObjectModel = new PackageDetailObjectModel();
                packageDetailObjectModel.AIR_PACKAGE_DETAIL_ARRAY = command.airPackageDetail.Select(a => new Air_Package_Detail_ArrayMapping
                {
                    service_option = a.service_option.ToSafeString(),
                    package_code = a.package_code.ToSafeString(),
                    product_type = a.product_type.ToSafeString(),
                    product_subtype = a.product_subtype.ToSafeString(),
                    product_subtype3 = a.product_subtype3.ToSafeString(),
                    package_group = a.package_group.ToSafeString(),
                    network_type = a.network_type.ToSafeString(),
                    service_day_stary = a.service_day_stary,
                    service_day_end = a.service_day_end
                }).ToArray();

                var packageVendorObjectModel = new PackageVendorObjectModel();
                packageVendorObjectModel.AIR_PACKAGE_VENDOR_ARRAY = command.airPackageVendor.Select(a => new Air_Package_Vendor_ArrayMapping
                {
                    service_option = a.service_option.ToSafeString(),
                    owner_product = a.owner_product.ToSafeString()
                }).ToArray();

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var packageDetail = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_PACKAGE_DETAIL_ARRAY", "AIR_PACKAGE_DETAIL_ARRAY", packageDetailObjectModel);
                var packageVendor = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_AIR_PACKAGE_VENDOR_ARRAY", "AIR_PACKAGE_VENDOR_ARRAY", packageVendorObjectModel);

                var executeResult = _objService.ExecuteStoredProc("AIR_ADMIN.PKG_AIROR012.save_package_master",
                out paramOut,
                   new
                   {
                       p_service_option = command.service_option.ToSafeString(),
                       p_user = command.user.ToSafeString(),
                       p_package_code = command.package_code.ToSafeString(),
                       p_package_type = command.package_type.ToSafeString(),
                       p_package_class = command.package_class.ToSafeString(),
                       p_sale_start_date = command.sale_start_date.ToSafeString(),
                       p_sale_end_date = command.sale_end_date.ToSafeString(),
                       p_pre_initiation_charge = command.pre_initiation_charge,
                       p_initiation_charge = command.initiation_charge,
                       p_pre_recurring_charge = command.pre_recurring_charge,
                       p_recurring_charge = command.recurring_charge,
                       p_package_name_tha = command.package_name_tha.ToSafeString(),
                       p_package_name_eng = command.package_name_eng.ToSafeString(),
                       p_sff_promotion_code = command.sff_promotion_code.ToSafeString(),
                       p_sff_promotion_bill_tha = command.sff_promotion_bill_tha.ToSafeString(),
                       p_sff_promotion_bill_eng = command.sff_promotion_bill_eng.ToSafeString(),
                       p_download_speed = command.download_speed.ToSafeString(),
                       p_upload_speed = command.upload_speed.ToSafeString(),
                       p_discount_type = command.discount_type.ToSafeString(),
                       p_discount_value = command.discount_value,
                       p_discount_day = command.discount_day,
                       p_vas_service = command.vas_service.ToSafeString(),
                       p_product_subtype2 = command.product_subtype2.ToSafeString(),
                       p_technology = command.technology.ToSafeString(),
                       /// array
                       p_air_package_detail_array = packageDetail,
                       p_air_package_vendor_array = packageVendor,
                       /// Out
                       o_package_code = o_package_code,
                       o_return_code = o_return_code,
                       o_return_msg = o_return_msg

                   });
                command.package_code = o_package_code.Value.ToSafeString();
                command.return_msg = o_return_msg.Value.ToSafeString();
                string return_code = o_return_code.Value.ToSafeString();
                command.return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.return_code = -1;
                command.return_msg = "Error call save profile customer service " + ex.Message;
            }
        }

        #region Mapping air_package_detail_array Type Oracle

        public class PackageDetailObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Air_Package_Detail_ArrayMapping[] AIR_PACKAGE_DETAIL_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageDetailObjectModel Null
            {
                get
                {
                    PackageDetailObjectModel obj = new PackageDetailObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AIR_PACKAGE_DETAIL_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AIR_PACKAGE_DETAIL_ARRAY = (Air_Package_Detail_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_PACKAGE_DETAIL_RECORD")]
        public class Air_Package_DetailOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Air_Package_Detail_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("AIR_PACKAGE_DETAIL_ARRAY")]
        public class AirPackageDetailObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageDetailObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Air_Package_Detail_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Air_Package_Detail_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SERVICE_OPTION")]
            public string service_option { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_CODE")]
            public string package_code { get; set; }

            [OracleObjectMappingAttribute("PRODUCT_TYPE")]
            public string product_type { get; set; }

            [OracleObjectMappingAttribute("PRODUCT_SUBTYPE")]
            public string product_subtype { get; set; }

            [OracleObjectMappingAttribute("PRODUCT_SUBTYPE3")]
            public string product_subtype3 { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_GROUP")]
            public string package_group { get; set; }

            [OracleObjectMappingAttribute("NETWORK_TYPE")]
            public string network_type { get; set; }

            [OracleObjectMappingAttribute("SERVICE_DAY_START")]
            public decimal service_day_stary { get; set; }

            [OracleObjectMappingAttribute("SERVICE_DAY_END")]
            public decimal service_day_end { get; set; }


            #endregion Attribute Mapping

            public static Air_Package_Detail_ArrayMapping Null
            {
                get
                {
                    Air_Package_Detail_ArrayMapping obj = new Air_Package_Detail_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "PACKAGE_CODE", package_code);
                OracleUdt.SetValue(con, udt, "PRODUCT_TYPE", product_type);
                OracleUdt.SetValue(con, udt, "PRODUCT_SUBTYPE", product_subtype);
                OracleUdt.SetValue(con, udt, "PRODUCT_SUBTYPE3", product_subtype3);
                OracleUdt.SetValue(con, udt, "PACKAGE_GROUP", package_group);
                OracleUdt.SetValue(con, udt, "NETWORK_TYPE", network_type);
                OracleUdt.SetValue(con, udt, "SERVICE_DAY_START", service_day_stary);
                OracleUdt.SetValue(con, udt, "SERVICE_DAY_END", service_day_end);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  Air_Package_Detail_Array Type Oracle

        #region Mapping air_package_vendor_array Type Oracle

        public class PackageVendorObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Air_Package_Vendor_ArrayMapping[] AIR_PACKAGE_VENDOR_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageVendorObjectModel Null
            {
                get
                {
                    PackageVendorObjectModel obj = new PackageVendorObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AIR_PACKAGE_VENDOR_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AIR_PACKAGE_VENDOR_ARRAY = (Air_Package_Vendor_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_PACKAGE_VENDOR_RECORD")]
        public class Air_Package_VendorOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Air_Package_Vendor_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("AIR_PACKAGE_VENDOR_ARRAY")]
        public class AirPackageVendorObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageVendorObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Air_Package_Vendor_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Air_Package_Vendor_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SERVICE_OPTION")]
            public string service_option { get; set; }

            [OracleObjectMappingAttribute("OWNER_PRODUCT")]
            public string owner_product { get; set; }

            #endregion Attribute Mapping

            public static Air_Package_Vendor_ArrayMapping Null
            {
                get
                {
                    Air_Package_Vendor_ArrayMapping obj = new Air_Package_Vendor_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "OWNER_PRODUCT", owner_product);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  air_package_vendor_array Type Oracle
    }
}
