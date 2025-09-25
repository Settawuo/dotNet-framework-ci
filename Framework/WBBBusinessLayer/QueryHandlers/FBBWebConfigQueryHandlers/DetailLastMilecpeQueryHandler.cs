using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class DetailLastMilecpeQueryHandler : IQueryHandler<DetailLastMilecpeQuery, List<DetailLastmileAndCPEReportList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DetailLastmileAndCPEReportList> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;

        public DetailLastMilecpeQueryHandler(ILogger logger, IEntityRepository<DetailLastmileAndCPEReportList> objService, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _objService = objService;
            _lovService = lovService;
        }
        public List<DetailLastmileAndCPEReportList> Handle(DetailLastMilecpeQuery command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;


                var cur = new OracleParameter();
                cur.ParameterName = "cur";
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                var p_vendor = new OracleParameter();
                p_vendor.ParameterName = "p_vendor";
                p_vendor.OracleDbType = OracleDbType.Varchar2;
                p_vendor.Direction = ParameterDirection.Input;
                p_vendor.Value = command.oltbrand.ToSafeString();


                var p_phase = new OracleParameter();
                p_phase.ParameterName = "p_phase";
                p_phase.OracleDbType = OracleDbType.Varchar2;
                p_phase.Direction = ParameterDirection.Input;
                p_phase.Value = command.phase.ToSafeString();


                var p_region = new OracleParameter();
                p_region.ParameterName = "p_vendor";
                p_region.OracleDbType = OracleDbType.Varchar2;
                p_region.Direction = ParameterDirection.Input;
                p_region.Value = command.region.ToSafeString();


                var p_inv_dt_from = new OracleParameter();
                p_inv_dt_from.ParameterName = "p_inv_dt_from";
                p_inv_dt_from.OracleDbType = OracleDbType.Varchar2;
                p_inv_dt_from.Direction = ParameterDirection.Input;
                p_inv_dt_from.Value = command.dateFrom.ToSafeString();

                var p_inv_dt_to = new OracleParameter();
                p_inv_dt_to.ParameterName = "p_inv_dt_to";
                p_inv_dt_to.OracleDbType = OracleDbType.Varchar2;
                p_inv_dt_to.Direction = ParameterDirection.Input;
                p_inv_dt_to.Value = command.dateTo.ToSafeString();

                var p_product_name = new OracleParameter();
                p_product_name.ParameterName = "p_product_name";
                p_product_name.OracleDbType = OracleDbType.Varchar2;
                p_product_name.Direction = ParameterDirection.Input;
                p_product_name.Value = command.product.ToSafeString();


                var p_addrss_id = new OracleParameter();
                p_addrss_id.ParameterName = "p_addrss_id";
                p_addrss_id.OracleDbType = OracleDbType.Varchar2;
                p_addrss_id.Direction = ParameterDirection.Input;
                p_addrss_id.Value = command.addressid.ToSafeString();

                var P_PAGE_INDEX = new OracleParameter();
                P_PAGE_INDEX.ParameterName = "P_PAGE_INDEX";
                P_PAGE_INDEX.OracleDbType = OracleDbType.Decimal;
                P_PAGE_INDEX.Direction = ParameterDirection.Input;
                P_PAGE_INDEX.Value = command.P_PAGE_INDEX;


                var P_PAGE_SIZE = new OracleParameter();
                P_PAGE_SIZE.ParameterName = "P_PAGE_SIZE";
                P_PAGE_SIZE.OracleDbType = OracleDbType.Decimal;
                P_PAGE_SIZE.Direction = ParameterDirection.Input;
                P_PAGE_SIZE.Value = command.P_PAGE_SIZE;

                var Roolback = _lovService.Get()
                    .Where(w => w.LOV_TYPE == "DETAIL_LASTMILE" && w.LOV_NAME == "ROLLBACK")
                    .FirstOrDefault();
                var result = Roolback != null ? Roolback.ACTIVEFLAG : "N";


                if (result == "Y")
                {

                }
                else if (result == "N")
                {

                }
                //   List<DetailLastmileAndCPEReportList> 
                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_DETAILLASTMILE.p_get_detail_lastmile_and_cpe",
                            new
                            {
                                p_vendor = command.oltbrand.ToSafeString(),
                                p_phase = command.phase.ToSafeString(),
                                p_region = command.region.ToSafeString(),
                                p_inv_dt_from = command.dateFrom.ToSafeString(),
                                p_inv_dt_to = command.dateTo.ToSafeString(),
                                p_product_name = command.product.ToSafeString(),
                                p_addrss_id = command.addressid.ToSafeString(),
                                P_PAGE_INDEX = command.P_PAGE_INDEX.ToSafeString(),
                                P_PAGE_SIZE = command.P_PAGE_SIZE.ToSafeString(),
                                //  return code
                                ret_code = ret_code,
                                cur = cur

                            }).ToList();

                command.Return_Code = 1;
                command.Return_Desc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.Return_Code = -1;
                command.Return_Desc = "Error call save event service " + ex.Message;

                return null;
            }

        }
    }

    public class DetailLastMilecpeAddressListQueryHandler : IQueryHandler<DetailLastMilecpeAddressidListQuery, List<DetailLastmileAndCPEReportList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DetailLastmileAndCPEReportList> _objService;

        public DetailLastMilecpeAddressListQueryHandler(ILogger logger, IEntityRepository<DetailLastmileAndCPEReportList> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<DetailLastmileAndCPEReportList> Handle(DetailLastMilecpeAddressidListQuery command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Direction = ParameterDirection.Output;

                var cur = new OracleParameter();
                cur.ParameterName = "cur";
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                var p_vendor = new OracleParameter();
                p_vendor.ParameterName = "p_vendor";
                p_vendor.OracleDbType = OracleDbType.Varchar2;
                p_vendor.Direction = ParameterDirection.Input;
                p_vendor.Value = command.oltbrand.ToSafeString();


                var p_phase = new OracleParameter();
                p_phase.ParameterName = "p_phase";
                p_phase.OracleDbType = OracleDbType.Varchar2;
                p_phase.Direction = ParameterDirection.Input;
                p_phase.Value = command.phase.ToSafeString();


                var p_region = new OracleParameter();
                p_region.ParameterName = "p_vendor";
                p_region.OracleDbType = OracleDbType.Varchar2;
                p_region.Direction = ParameterDirection.Input;
                p_region.Value = command.region.ToSafeString();


                var p_inv_dt_from = new OracleParameter();
                p_inv_dt_from.ParameterName = "p_inv_dt_from";
                p_inv_dt_from.OracleDbType = OracleDbType.Varchar2;
                p_inv_dt_from.Direction = ParameterDirection.Input;
                p_inv_dt_from.Value = command.dateFrom.ToSafeString();

                var p_inv_dt_to = new OracleParameter();
                p_inv_dt_to.ParameterName = "p_inv_dt_to";
                p_inv_dt_to.OracleDbType = OracleDbType.Varchar2;
                p_inv_dt_to.Direction = ParameterDirection.Input;
                p_inv_dt_to.Value = command.dateTo.ToSafeString();

                var p_product_name = new OracleParameter();
                p_product_name.ParameterName = "p_product_name";
                p_product_name.OracleDbType = OracleDbType.Varchar2;
                p_product_name.Direction = ParameterDirection.Input;
                p_product_name.Value = command.product.ToSafeString();

                var P_PAGE_INDEX = new OracleParameter();
                P_PAGE_INDEX.ParameterName = "P_PAGE_INDEX";
                P_PAGE_INDEX.OracleDbType = OracleDbType.Decimal;
                P_PAGE_INDEX.Direction = ParameterDirection.Input;
                P_PAGE_INDEX.Value = command.P_PAGE_INDEX;


                var P_PAGE_SIZE = new OracleParameter();
                P_PAGE_SIZE.ParameterName = "P_PAGE_SIZE";
                P_PAGE_SIZE.OracleDbType = OracleDbType.Decimal;
                P_PAGE_SIZE.Direction = ParameterDirection.Input;
                P_PAGE_SIZE.Value = command.P_PAGE_SIZE;

                var p_addrss_id = new OracleParameter();
                p_addrss_id.ParameterName = "p_addrss_id";
                p_addrss_id.OracleDbType = OracleDbType.Varchar2;
                p_addrss_id.Direction = ParameterDirection.Input;
                p_addrss_id.Value = command.addressid.ToSafeString();

                var packageMapping = new OracleParameter();

                if (command.address != null)
                {
                    var packageMappingObjectModel = new PackageMappingObjectModel
                    {
                        AddressId = command.address.Select(a => new Address_Mapping
                        {
                            ADDRESS_ID = a
                        }).ToArray()
                    };

                    packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_addrss_id_list", "WBB.FBB_ADDRESS_ID_LIST", packageMappingObjectModel);

                }

                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_DETAILLASTMILE.p_get_detail_lastmile_and_cpe",
                  new
                  {
                      p_vendor,
                      p_phase,
                      p_region,
                      p_inv_dt_from,
                      p_inv_dt_to,
                      p_product_name,
                      p_addrss_id,
                      P_PAGE_INDEX,
                      P_PAGE_SIZE,
                      //Return
                      ret_code,
                      cur,


                      packageMapping
                  }).ToList();
                //List<DetailLastmileAndCPEReportList> retDLMs = new List<DetailLastmileAndCPEReportList>();

                //if (executeResult != null)
                //{
                //	//var results = (DataTable)executeResult[1];
                //	executeResult
                //	retDLMs = results.DataTableToList<DetailLastmileAndCPEReportList>();
                //}
                command.Return_Code = 1;
                command.Return_Desc = "Success";
                return executeResult;


            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.Return_Code = -1;
                command.Return_Desc = "Error call save event service " + ex.Message;

                return null;
            }

        }
        #region PackageMappingObjectModel
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Address_Mapping[] AddressId { get; set; }


            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModel Null
            {
                get
                {
                    var obj = new PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AddressId);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AddressId = (Address_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("WBB.FBB_ADDRESS_ID_REC")]
        public class Address_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Address_Mapping();
            }
        }

        [OracleCustomTypeMapping("WBB.FBB_ADDRESS_ID_LIST")]
        public class Address_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Address_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Address_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("ADDRESS_ID")]
            public string ADDRESS_ID { get; set; }
            #endregion Attribute Mapping

            public static Address_Mapping Null
            {
                get
                {
                    var obj = new Address_Mapping();
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
                OracleUdt.SetValue(con, udt, "ADDRESS_ID", ADDRESS_ID);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
