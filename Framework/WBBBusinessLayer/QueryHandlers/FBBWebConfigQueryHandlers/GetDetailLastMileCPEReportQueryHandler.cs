using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetDetailLastMileCPEReportQueryHandler : IQueryHandler<DetailLastMileCPEQuery, List<DetailLastmileAndCPEReportList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DetailLastmileAndCPEReportList> _objService;

        public GetDetailLastMileCPEReportQueryHandler(ILogger logger, IEntityRepository<DetailLastmileAndCPEReportList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<DetailLastmileAndCPEReportList> Handle(DetailLastMileCPEQuery command)
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

                //var result = _objService.ExecuteStoredProcExecuteReader("WBB.PKG_FBBPAYG_DETAILLASTMILE.p_get_detail_lastmile_and_cpe",

                //                new object[]
                //            {
                //                p_vendor,
                //                p_phase,
                //                p_region,
                //                p_inv_dt_from,
                //                p_inv_dt_to ,
                //                p_product_name ,
                //                p_addrss_id  ,
                //                ret_code       ,
                //                cur

                //            });
                //var v1 = result.FirstOrDefault().Value;

                //returnData.str_pdf_html = executeResult.FirstOrDefault(x => x.Key == "str_pdf_html").Value.ToString();
                //returnData.return_code = decimal.Parse(executeResult.FirstOrDefault(x => x.Key == "return_code").Value.ToString());
                //returnData.return_message = executeResult.FirstOrDefault(x => x.Key == "return_message").Value.ToString();

                List<DetailLastmileAndCPEReportList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_DETAILLASTMILE.p_get_detail_lastmile_and_cpe",
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
}
