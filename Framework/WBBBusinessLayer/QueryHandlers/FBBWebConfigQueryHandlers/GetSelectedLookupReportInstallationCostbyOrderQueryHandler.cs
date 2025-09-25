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
    public class GetSelectedLookupReportInstallationCostbyOrderQueryHandler : IQueryHandler<GetSelectedLookupReportInstallationCostbyOrderQuery, SelectedLookupReportInstallationCostbyOrderReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SelectedLookupReportInstallationCostbyOrderReturn> _objService;

        public GetSelectedLookupReportInstallationCostbyOrderQueryHandler(ILogger logger, IEntityRepository<SelectedLookupReportInstallationCostbyOrderReturn> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public SelectedLookupReportInstallationCostbyOrderReturn Handle(GetSelectedLookupReportInstallationCostbyOrderQuery query)
        {
            var returnForm = new SelectedLookupReportInstallationCostbyOrderReturn();
            try
            {

                var p_LOOKUP_NAME = new OracleParameter();
                {
                    p_LOOKUP_NAME.ParameterName = "p_LOOKUP_NAME";
                    p_LOOKUP_NAME.Size = 2000;
                    p_LOOKUP_NAME.OracleDbType = OracleDbType.Varchar2;
                    p_LOOKUP_NAME.Direction = ParameterDirection.Input;
                    p_LOOKUP_NAME.Value = query.p_LOOKUP_NAME;
                };

                var p_FOA_SUBMIT_DATE = new OracleParameter();
                {
                    p_FOA_SUBMIT_DATE.ParameterName = "p_FOA_SUBMIT_DATE";
                    p_FOA_SUBMIT_DATE.Size = 2000;
                    p_FOA_SUBMIT_DATE.OracleDbType = OracleDbType.Varchar2;
                    p_FOA_SUBMIT_DATE.Direction = ParameterDirection.Input;
                    p_FOA_SUBMIT_DATE.Value = query.p_FOA_SUBMIT_DATE;
                };

                var ret_code = new OracleParameter();
                {
                    ret_code.ParameterName = "ret_code";
                    ret_code.Size = 2000;
                    ret_code.OracleDbType = OracleDbType.Int32;
                    ret_code.Direction = ParameterDirection.Output;
                };

                var ret_msg = new OracleParameter();
                {
                    ret_msg.ParameterName = "ret_msg";
                    ret_msg.Size = 2000;
                    ret_msg.OracleDbType = OracleDbType.Varchar2;
                    ret_msg.Direction = ParameterDirection.Output;
                };

                var result_lookup_id_cur = new OracleParameter();
                {
                    result_lookup_id_cur.ParameterName = "result_lookup_id_cur";
                    result_lookup_id_cur.OracleDbType = OracleDbType.RefCursor;
                    result_lookup_id_cur.Direction = ParameterDirection.Output;
                };


                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE");


                var executeResult = _objService.ExecuteStoredProcMultipleCursor(
                    "WBB.PKG_PAYG_INSTALL_COST_RPT.p_get_selected_lookup",
                    new object[]
                    {
                        p_LOOKUP_NAME,
                        p_FOA_SUBMIT_DATE,
                        ret_code,
                        ret_msg,
                        result_lookup_id_cur
                    }).ToList();

                DataTable resp = new DataTable();
                List<SelectedLookupCurReportInstallationCostbyOrderListModel> respList = new List<SelectedLookupCurReportInstallationCostbyOrderListModel>();
                if (executeResult[2] != null)
                {
                    resp = (DataTable)executeResult[2];
                    respList = resp.DataTableToList<SelectedLookupCurReportInstallationCostbyOrderListModel>();
                    returnForm.result_lookup_id_cur = respList;
                }


                //returnForm.ret_code = ret_code.Value.ToString(); // Assign RET_CODE
                //returnForm.ret_msg = ret_msg.Value.ToString();

                return returnForm;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FIXED_ASSET_LASTMILE handles : " + ex.Message);

                return null;
            }
        }
    }
}
