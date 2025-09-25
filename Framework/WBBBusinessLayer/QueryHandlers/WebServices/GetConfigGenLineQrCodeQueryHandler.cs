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
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetConfigGenLineQrCodeQueryHandler : IQueryHandler<GetConfigGenLineQrCodeQuery, GetConfigGenLineQrCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<GetConfigGenLineQrCodeModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public GetConfigGenLineQrCodeQueryHandler(ILogger logger, IWBBUnitOfWork uow
            , IEntityRepository<GetConfigGenLineQrCodeModel> objService
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public GetConfigGenLineQrCodeModel Handle(GetConfigGenLineQrCodeQuery query)
        {
            InterfaceLogCommand log = null;
            GetConfigGenLineQrCodeModel result = new GetConfigGenLineQrCodeModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MobileNo, "GetConfigGenLineQrCode", "GetConfigGenLineQrCodeQueryHandler", "", "FBB|" + query.FullUrl, "WBB");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var payload_ch = new OracleParameter();
                payload_ch.ParameterName = "payload_ch";
                payload_ch.OracleDbType = OracleDbType.Varchar2;
                payload_ch.Size = 2000;
                payload_ch.Direction = ParameterDirection.Output;

                var payload_linetempid = new OracleParameter();
                payload_linetempid.ParameterName = "payload_linetempid";
                payload_linetempid.OracleDbType = OracleDbType.Varchar2;
                payload_linetempid.Size = 2000;
                payload_linetempid.Direction = ParameterDirection.Output;

                var verify_signature = new OracleParameter();
                verify_signature.ParameterName = "verify_signature";
                verify_signature.OracleDbType = OracleDbType.Varchar2;
                verify_signature.Size = 2000;
                verify_signature.Direction = ParameterDirection.Output;

                var url = new OracleParameter();
                url.ParameterName = "url";
                url.OracleDbType = OracleDbType.Varchar2;
                url.Size = 2000;
                url.Direction = ParameterDirection.Output;


                var executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBOR004.PROC_GEN_LINE_QR_CODE",
                    new
                    {
                        // return code
                        ret_code = ret_code,
                        ret_msg = ret_msg,
                        payload_ch = payload_ch,
                        payload_linetempid = payload_linetempid,
                        verify_signature = verify_signature,
                        url = url,
                    }).ToList();

                if (ret_code.Value.ToSafeString() == "0") // return 0 pass value to screen 
                {
                    result.ret_code = ret_code.Value.ToSafeString();
                    result.ret_msg = ret_msg.Value.ToSafeString();
                    result.payload_ch = payload_ch.Value.ToSafeString();
                    result.payload_linetempid = payload_linetempid.Value.ToSafeString();
                    result.verify_signature = verify_signature.Value.ToSafeString();
                    result.url = url.Value.ToSafeString();
                }
                else //return -1 error
                {
                    result.ret_code = ret_code.Value.ToSafeString();
                    result.ret_msg = ret_msg.Value.ToSafeString();
                    result.payload_ch = "";
                    result.payload_linetempid = "";
                    result.verify_signature = "";
                    result.url = "";
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                result.ret_code = "-1";
                result.ret_msg = ex.Message;
                result.payload_ch = "";
                result.payload_linetempid = "";
                result.verify_signature = "";
                result.url = "";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
            }

            return result;

        }
    }
}
