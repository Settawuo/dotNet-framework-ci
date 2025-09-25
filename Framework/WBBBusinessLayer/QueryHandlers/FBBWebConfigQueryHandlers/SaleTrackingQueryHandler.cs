using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class SaleTrackingQueryHandler : IQueryHandler<SaleTrackingQuery, List<SaleTrackingList>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<SaleTrackingList> _objService;

        public SaleTrackingQueryHandler(ILogger logger, IAirNetEntityRepository<SaleTrackingList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<SaleTrackingList> Handle(SaleTrackingQuery query)
        {
            try
            {
                _logger.Info("SaleTrackingQueryHandler Start");
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "ret_data";
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();


                List<SaleTrackingList> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_LIST_ORD",
                    new
                    {
                        p_id_card = query.P_Id_Card,
                        p_first_name = query.P_First_Name,
                        p_last_name = query.P_Last_Name,
                        p_location_code = query.P_Location_Code,
                        p_asc_code = query.P_Asc_Code,
                        p_date_from = query.P_Date_From,
                        p_date_to = query.P_Date_To,
                        p_cust_name = query.P_Cust_Name,
                        p_user = query.P_User,

                        // return code
                        ret_code = ret_code,
                        ret_msg = ret_msg,
                        ret_data = ret_data

                    }).ToList();

                // 0 = Success , -1 = Fail
                query.ret_code = 0;
                query.ret_msg = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service AIR_ADMIN.PKG_FBBOR005.QueryOrder" + ex.Message);
                query.ret_code = -1;
                query.ret_msg = "Error";

                return null;
            }

        }
    }
}
