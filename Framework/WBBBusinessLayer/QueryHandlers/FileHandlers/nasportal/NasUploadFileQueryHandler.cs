using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.SftpQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.SftpHandlers
{
    public class NasUploadFileQueryHandler : IQueryHandler<NasUploadFileQuery, UploadFileModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public NasUploadFileQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public UploadFileModel Handle(NasUploadFileQuery query)
        {
            UploadFileModel resp = new UploadFileModel();
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            InterfaceLogPayGCommand logNasUpdate = new InterfaceLogPayGCommand();
            try
            {
                log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, query, query.TransectionId, "UploadFile", "UploadFileQueryHandler", "", "UPLOAD", "FBBCONFIG");

                SftpFileParameter configFile = new SftpFileParameter
                {
                    Key = query.Key,
                    Nas = query.NasType,
                    Lovtype = "FBB_NAS_PORTAL",
                    LovnameAcc = "NAS_PORTAL_ACCESS_TYPE",
                    LovnamePath = "NAS_PORTAL_PATH",
                    LovtypePower = "Config_DataPower"
                };
                var cfg = GetNasServiceHelper.GetParameter(_objService, configFile);

                var remotePath = Path.Combine(cfg.remotePath, query.FileName);
                resp = FileHelper.UploadFile(cfg.UserName
                                                          , cfg.Password
                                                          , cfg.Host
                                                          , remotePath
                                                          , query.DataFile, out string msg
                    );
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, resp, log, msg, cfg.UserName, "FBBCONFIG");
                resp.Message = msg;
                if (resp.Upload)
                {
                    try
                    {
                        var command = new FbbNasUpdateLogCommand
                        {
                            file_name = query.FileName,
                            file_owner = query.Username, // form current user
                            nas_path = query.Path,
                            action = "create",
                        };

                        logNasUpdate = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command,
                            query.TransectionId, "FbbNasUpdateLogCommand", "FbbNasUpdateLogCommandHandler",
                            command.file_name, "FBB", "WEB");

                        var ret_codeUpdate = new OracleParameter();
                        ret_codeUpdate.OracleDbType = OracleDbType.Int32;
                        ret_codeUpdate.ParameterName = "ret_code";
                        ret_codeUpdate.Direction = ParameterDirection.Output;

                        var outp = new List<object>();
                        var paramOut = outp.ToArray();

                        var executeResult = _objService.ExecuteStoredProc("WBB.P_FBB_NAS_UPDATE_LOG",
                        out paramOut,
                           new
                           {
                               //in 
                               p_FILE_NAME = command.file_name,
                               p_NAS_PATH = command.nas_path,
                               p_USERNAME = command.file_owner,
                               p_ACTION = command.action,
                               //out
                               ret_code = ret_codeUpdate,
                           });
                        if (ret_codeUpdate.Value.ToString() == "0")
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ret_codeUpdate, logNasUpdate, "Success", "", "");
                        else
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ret_codeUpdate, logNasUpdate, "Failed", ret_codeUpdate.Value.ToSafeString(), "");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message);

                        if (null != logNasUpdate)
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, logNasUpdate, "Failed", ex.GetErrorMessage(), "");

                        resp.Message = ex.Message.ToSafeString();
                    }
                }
                else
                {
                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", log, "Failed", "", "FBBCONFIG");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", log, ex.Message.ToString(), "", "FBBCONFIG");
                return null;
            }

            return resp;
        }
    }
}
