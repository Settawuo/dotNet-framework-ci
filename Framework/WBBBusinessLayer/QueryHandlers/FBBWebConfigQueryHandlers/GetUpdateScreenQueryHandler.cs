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
    public class GetUpdateScreenQueryHandler : IQueryHandler<UpdateScreenQuery, List<UpdateScreenList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<UpdateScreenList> _objService;

        public GetUpdateScreenQueryHandler(ILogger logger, IEntityRepository<UpdateScreenList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<UpdateScreenList> Handle(UpdateScreenQuery command)
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


                var inv_str = new OracleParameter();
                inv_str.OracleDbType = OracleDbType.Varchar2;
                inv_str.Size = 2000;
                inv_str.Direction = ParameterDirection.Output;

                //Nullable<DateTime> dateFrom = !string.IsNullOrEmpty(command.DateFrom) ? DateTime.Parse(command.DateFrom.ToSafeString()) : (DateTime?)null;
                //Nullable<DateTime> dateTo = !string.IsNullOrEmpty(command.DateTo) ? DateTime.Parse(command.DateTo.ToSafeString()) : (DateTime?)null; 

                //var dateFrom = !string.IsNullOrEmpty(command.DateFrom) ? command.DateFrom : "ALL";
                //var dateTo = !string.IsNullOrEmpty(command.DateTo) ? command.DateTo : "ALL";

                Nullable<DateTime> tempDateTo = DateTime.Now.AddYears(1);
                var dateFrom = !string.IsNullOrEmpty(command.DateFrom) ? command.DateFrom : "01/01/1999";
                var dateTo = !string.IsNullOrEmpty(command.DateTo) ? command.DateTo : tempDateTo.Value.Date.ToString("dd/MM/yyyy");

                List<UpdateScreenList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_UPDATEPAID_STD.p_get_paid_status",
                            new
                            {
                                p_internet_no = command.InternatNo,
                                p_po = command.PO,
                                p_invoice = command.Invoice,
                                p_inv_dt_from = dateFrom,//command.DateFrom,
                                p_inv_dt_to = dateTo,//command.DateTo,
                                ch_ind = command.CHKINDOOR,
                                ch_out = command.CHKOUTDOOR,
                                ch_ont = command.CHKONT,
                                //  return code
                                ret_code = ret_code,
                                inv_str = inv_str,
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
