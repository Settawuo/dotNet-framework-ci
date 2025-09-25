using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.PatchEquipment;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBBusinessLayer.QueryHandlers.FBBHVR
{
    public class GetLowUtilizeSaleReportHVRQueryHandler : IQueryHandler<GetLowUtilizeSaleReportHVRQuery, List<LowUtilizeSaleReportList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IFBBShareplexEntityRepository<LowUtilizeSaleReportList> _fbbShareplexRepository;
        private readonly IFBBHVREntityRepository<object> _fbbHVRRepository;

        public GetLowUtilizeSaleReportHVRQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBShareplexEntityRepository<LowUtilizeSaleReportList> fbbShareplexRepository,
            IFBBHVREntityRepository<object> fbbHVRRepository)
        {
            _logger = logger;
            _fbbShareplexRepository = fbbShareplexRepository;
            _fbbHVRRepository = fbbHVRRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public List<LowUtilizeSaleReportList> Handle(GetLowUtilizeSaleReportHVRQuery query)
        {
            InterfaceLogCommand log = null;

            var p_location_code = new NpgsqlParameter();
            p_location_code.ParameterName = "p_location_code";
            p_location_code.Size = 2000;
            p_location_code.NpgsqlDbType = NpgsqlDbType.Text;
            p_location_code.Direction = ParameterDirection.Input;
            p_location_code.Value = query.p_location_code.ToSafeString();

            var ret_code_cur = new NpgsqlParameter();
            ret_code_cur.ParameterName = "ret_code_cur";
            ret_code_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
            ret_code_cur.Direction = ParameterDirection.InputOutput;
            ret_code_cur.Value = "ret_code_cur";

            var list_low_utilize_rpt = new NpgsqlParameter();
            list_low_utilize_rpt.ParameterName = "list_low_utilize_rpt";
            list_low_utilize_rpt.NpgsqlDbType = NpgsqlDbType.Refcursor;
            list_low_utilize_rpt.Direction = ParameterDirection.InputOutput;
            list_low_utilize_rpt.Value = "list_low_utilize_rpt";

            List<LowUtilizeSaleReportList> result = new List<LowUtilizeSaleReportList>();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_LOW_UTILIZE_RPT.QUERY_LOW_UTILIZE_RPT");

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "QUERY_LOW_UTILIZE_RPT_HVR", "GetLowUtilizeSaleReport", query.p_location_code, "FBB", "LowUtilizeSaleReport.exe");

                var executeResults = _fbbHVRRepository.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_fbb_low_utilize_rpt_query_low_utilize_rpt",
                    new object[]
                    {
                        p_location_code,

                        //return code
                        ret_code_cur,
                        list_low_utilize_rpt

                    }).ToList();

                _logger.Info("End FBBADM.PKG_FBB_LOW_UTILIZE_RPT.QUERY_LOW_UTILIZE_RPT output msg: " + query.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");

                if (executeResults != null && executeResults.Count > 0)
                {
                    DataTable dt_ret_code = (DataTable)executeResults[0];
                    DataTable dt_list_low_utilize_rpt = (DataTable)executeResults[1];

                    List<LowUtilizeSaleReportList> response_ret_code = dt_ret_code.DataTableToList<LowUtilizeSaleReportList>();
                    List<LowUtilizeSaleReportList> response_list = dt_list_low_utilize_rpt.DataTableToList<LowUtilizeSaleReportList>();

                    var _first = response_ret_code.FirstOrDefault();

                    foreach (var item in response_list)
                    {
                        item.ret_code = _first.ret_code != null ? _first.ret_code.ToString() : "-1";
                        item.ret_message = _first.ret_message != null ? _first.ret_message.ToString() : "";
                    }

                    result = response_list;
                }
                else
                {
                    result = new List<LowUtilizeSaleReportList>();
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error call FBBADM.PKG_FBB_LOW_UTILIZE_RPT.QUERY_LOW_UTILIZE_RPT handles : " + ex.Message);
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return result;
        }
    }
}
