using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
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

    public class GetDataAddressInstallQueryHandler : IQueryHandler<GetDataAddressInstallQuery, GetDataAddressInstallModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetDataAddressInstallModel> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetDataAddressInstallQueryHandler(ILogger logger, IEntityRepository<GetDataAddressInstallModel> objService, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public GetDataAddressInstallModel Handle(GetDataAddressInstallQuery query)
        {
            InterfaceLogCommand log = null;
            GetDataAddressInstallModel result = new GetDataAddressInstallModel();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_Id, "GetDataAddressInstallQuery", "GetDataAddressInstallQueryHandler", "", "FBB", "");

                var p_fibrenet_id = new OracleParameter();
                p_fibrenet_id.ParameterName = "p_fibrenet_id";
                p_fibrenet_id.OracleDbType = OracleDbType.Varchar2;
                p_fibrenet_id.Direction = ParameterDirection.Input;
                p_fibrenet_id.Value = query.p_fibrenet_id;

                var return_code = new OracleParameter();
                return_code.OracleDbType = OracleDbType.Int32;
                return_code.Direction = ParameterDirection.Output;
                return_code.ParameterName = "RETURN_CODE";

                var return_message = new OracleParameter();
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Direction = ParameterDirection.Output;
                return_message.ParameterName = "RETURN_MESSAGE";

                var return_install_curror = new OracleParameter();
                return_install_curror.OracleDbType = OracleDbType.RefCursor;
                return_install_curror.Direction = ParameterDirection.Output;
                return_install_curror.ParameterName = "RETURN_INSTALL_CURROR";

                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR051.QUERY_DATA_ADDRESS_INSTALL",
                        new object[]
                        {
                            p_fibrenet_id,
                            //out
                            return_code,
                            return_message,
                            return_install_curror

                        });

                DataTable data1 = (DataTable)executeResult[2];
                result.install_curror = data1.DataTableToList<InstallAddressList>();

                result.ret_code = executeResult[0] != null ? executeResult[0].ToSafeString() : "-1";
                result.ret_message = executeResult[1] != null ? executeResult[1].ToSafeString() : "error";

                if (result.ret_code != "-1")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", result.ret_message, "");
                }

            }
            catch (Exception ex)
            {

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                result.ret_code = "-1";
                result.ret_message = "error GetDataAddressInstallQueryHandler " + ex.Message;

            }

            return result;
        }
    }
}
