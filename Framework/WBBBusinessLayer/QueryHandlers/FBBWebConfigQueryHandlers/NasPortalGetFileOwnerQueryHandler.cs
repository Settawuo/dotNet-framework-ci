using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class NasPortalGetFileOwnerQueryHandler : IQueryHandler<NasPortalGetFileOwnerQuery, NasPortalFileOwnerModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<FBSS_HISTORY_LOG> _fbsshistoryLog;
        private readonly IWBBUnitOfWork _uow;
        public NasPortalGetFileOwnerQueryHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IEntityRepository<FBSS_HISTORY_LOG> fbsshistoryLog,
            IWBBUnitOfWork uow
            )
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _fbsshistoryLog = fbsshistoryLog;
            _uow = uow;

        }
        public NasPortalFileOwnerModel Handle(NasPortalGetFileOwnerQuery query)
        {
            var historyLog = new FBB_HISTORY_LOG();
            var fbsshistoryLog = new FBSS_HISTORY_LOG();

            NasPortalFileOwnerModel result = new NasPortalFileOwnerModel();
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_file_owner = new OracleParameter();
                ret_file_owner.OracleDbType = OracleDbType.Varchar2;
                ret_file_owner.Size = 2000;
                ret_file_owner.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();


                var executeResult = _objService.ExecuteStoredProc("wbb.p_fbb_nas_get_fileowner",
                    out paramOut,
                 new
                 {
                     p_file_name = query.p_file_name,
                     // out 
                     ret_file_owner,
                     ret_code,
                 });
                result.ret_code = (ret_code.Value.ToSafeString() != null) ? int.Parse(ret_code.Value.ToSafeString()) : -1;
                result.ret_file_owner = ret_file_owner.Value.ToSafeString() != "null" ? ret_file_owner.Value.ToSafeString() : null;

                decimal idmax = 0;
                //  var MaxTranId = (from c in _fbsshistoryLog.Get() select c);
                // string idMaxTranId = Maxidmax.Max(c => c.IN_TRANSACTION_ID);

                DateTime dt = DateTime.UtcNow;
                string TranId = dt.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                idmax = GetId();

                fbsshistoryLog.INTERFACE_ID = (long)idmax;
                fbsshistoryLog.IN_TRANSACTION_ID = TranId;
                fbsshistoryLog.METHOD_NAME = "p_fbb_nas_get_fileowner";
                fbsshistoryLog.CREATED_BY = "WBB_APP";
                fbsshistoryLog.REQUEST_STATUS = "Success";
                fbsshistoryLog.CREATED_DATE = DateTime.UtcNow;
                fbsshistoryLog.IN_XML_PARAM = "";
                fbsshistoryLog.SERVICE_NAME = "NasPortalGetFileOwnerQueryHandler";
                fbsshistoryLog.URL_LINE = "NasPortal/Index";
                fbsshistoryLog.INTERFACE_NODE = "";
                _fbsshistoryLog.Create(fbsshistoryLog);
                //_uow.Persist();

                // --------------------insert log error 

                idmax = GetId();
                fbsshistoryLog.INTERFACE_ID = (long)idmax;
                fbsshistoryLog.IN_TRANSACTION_ID = TranId;
                fbsshistoryLog.METHOD_NAME = "p_fbb_nas_get_fileowner";
                fbsshistoryLog.CREATED_BY = "WBB_APP";
                fbsshistoryLog.REQUEST_STATUS = "Error";
                fbsshistoryLog.CREATED_DATE = DateTime.UtcNow;
                fbsshistoryLog.IN_XML_PARAM = "";
                fbsshistoryLog.SERVICE_NAME = "NasPortalGetFileOwnerQueryHandler";
                fbsshistoryLog.URL_LINE = "NasPortal/Index";
                fbsshistoryLog.INTERFACE_NODE = "";
                _fbsshistoryLog.Create(fbsshistoryLog);

            }
            catch (Exception ex)
            {
                result.ret_code = -1;

                _logger.Error("Error WBB.p_fbb_nas_get_fileowner " + ex.GetErrorMessage());
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "NAS_PORTAL";
                historyLog.CREATED_BY = "WBB_APP";
                historyLog.CREATED_DATE = DateTime.UtcNow;
                historyLog.DESCRIPTION = "Error WBB.p_fbb_nas_get_fileowner " + ex.GetErrorMessage();
                historyLog.REF_KEY = "p_fbb_nas_get_fileowner";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
            }



            return result;
        }
        public decimal GetId()
        {
            var historyLog = new FBB_HISTORY_LOG();
            decimal idmax = 0;
            try
            {
                var Maxidmax = (from c in _fbsshistoryLog.Get() select c);
                var CheckData = Maxidmax.ToList();

                if (CheckData.Count > 0)
                {
                    idmax = Maxidmax.Max(c => c.INTERFACE_ID);
                }
                idmax += 1;
                return idmax;

            }
            catch (Exception ex)
            {

                _logger.Error("Error  GetId fro interface_id" + ex.GetErrorMessage());
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "NAS_PORTAL";
                historyLog.CREATED_BY = "WBB_APP";
                historyLog.CREATED_DATE = DateTime.UtcNow;
                historyLog.DESCRIPTION = "Error  GetId fro interface_id " + ex.GetErrorMessage();
                historyLog.REF_KEY = "fbb_nas_update_log";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                return 0;
            }

        }
    }
}
