using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.SftpQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.SftpHandlers
{
    public class UploadFileQueryHandler : IQueryHandler<UploadFileQuery, UploadFileModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public UploadFileQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public UploadFileModel Handle(UploadFileQuery query)
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
                var cfg = GetConfigSftpServiceHelper.GetParameter(_objService, configFile);


                SftpService sftp = new SftpService() { Host = cfg.Host, Port = cfg.Port };

                string keyfile = cfg.ConfigType == "DataPower" ? HttpContext.Current.Server.MapPath($"~{cfg.KeyFile}") : cfg.KeyFile;

                resp = sftp.Upload(cfg.UserName, keyfile, cfg.remotePath, query.FileName, query.DataFile, cfg.ConfigType, out string msg);
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, resp, log, msg, keyfile, "FBBCONFIG");
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
                        {
                            //resp.Message = "Log Command was created.";
                            resp.Message = "";
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ret_codeUpdate, logNasUpdate, "Success", "", "");
                        }
                        else
                        {
                            resp.Message = "";
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ret_codeUpdate, logNasUpdate, "Failed", ret_codeUpdate.Value.ToSafeString(), "");

                        }
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
