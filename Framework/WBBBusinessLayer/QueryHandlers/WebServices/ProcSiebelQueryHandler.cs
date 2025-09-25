using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class ProcSiebelQueryHandler : IQueryHandler<ProcSiebelQuery, ProcSiebelModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<ProcSiebelModel> _objService;

        public ProcSiebelQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<ProcSiebelModel> objService)
        {
            _intfLog = intfLog;
            _uow = uow;
            _objService = objService;
        }

        public ProcSiebelModel Handle(ProcSiebelQuery query)
        {
            var OUTPUT_RETURN_CODE = new OracleParameter();
            OUTPUT_RETURN_CODE.ParameterName = "OUTPUT_RETURN_CODE";
            OUTPUT_RETURN_CODE.OracleDbType = OracleDbType.Decimal;
            OUTPUT_RETURN_CODE.Direction = ParameterDirection.Output;

            var OUTPUT_RETURN_MESSAGE = new OracleParameter();
            OUTPUT_RETURN_MESSAGE.ParameterName = "OUTPUT_RETURN_MESSAGE";
            OUTPUT_RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
            OUTPUT_RETURN_MESSAGE.Size = 2000;
            OUTPUT_RETURN_MESSAGE.Direction = ParameterDirection.Output;

            var P_SIEBEL_XML = new OracleParameter();
            P_SIEBEL_XML.ParameterName = "P_SIEBEL_XML";
            P_SIEBEL_XML.OracleDbType = OracleDbType.RefCursor;
            P_SIEBEL_XML.Direction = ParameterDirection.Output;

            ProcSiebelModel result = new ProcSiebelModel();

            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_INTERNET_ID + query.client_ip, "ProcSiebelQuery", "WBB.PKG_PLAYBOX_REPLACE.PROC_SIEBEL", null, "FBB|" + query.FullUrl, "FBBOR016");

            try
            {
                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_PLAYBOX_REPLACE.PROC_SIEBEL",
                            new
                            {
                                P_INTERNET_ID = query.P_INTERNET_ID,
                                P_CONTACT_MOBILE = query.P_CONTACT_MOBILE,
                                //  return code
                                OUTPUT_RETURN_CODE = OUTPUT_RETURN_CODE,
                                OUTPUT_RETURN_MESSAGE = OUTPUT_RETURN_MESSAGE,
                                P_SIEBEL_XML = P_SIEBEL_XML
                            });
                if (OUTPUT_RETURN_CODE.Value.ToSafeString() == "0" || OUTPUT_RETURN_CODE.Value == null)
                {
                    result = executeResult.FirstOrDefault();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", OUTPUT_RETURN_MESSAGE.Value.ToSafeString(), "");
                }
                result.OUTPUT_RETURN_CODE = OUTPUT_RETURN_CODE.Value.ToSafeString();
                result.OUTPUT_RETURN_MESSAGE = OUTPUT_RETURN_MESSAGE.Value.ToSafeString();

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.Message, "");
            }

            return result;

        }
    }
}
