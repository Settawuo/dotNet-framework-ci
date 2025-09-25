using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.SftpQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.SftpHandlers
{
    public class NasDeleteFileQueryHandler : IQueryHandler<NasDeleteFileQuery, DeleteFileModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public NasDeleteFileQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public DeleteFileModel Handle(NasDeleteFileQuery query)
        {
            DeleteFileModel resp = new DeleteFileModel();
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            InterfaceLogPayGCommand logNasUpdate = new InterfaceLogPayGCommand();

            #region Check 
            log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, query, "", "FbbNasGetFileOwnerQuery", "FbbNasGetFileOwnerQueryHandler", null, "FBB", "");
            string file_owner = null;
            try
            {

                var ret_file_owner = new OracleParameter();
                ret_file_owner.OracleDbType = OracleDbType.Varchar2;
                ret_file_owner.ParameterName = "ret_file_owner";
                ret_file_owner.Size = 2000;
                ret_file_owner.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Int32;
                ret_code.ParameterName = "ret_code";
                ret_code.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _objService.ExecuteStoredProc("WBB.P_FBB_NAS_GET_FILEOWNER",
                    out paramOut,
                   new
                   {
                       //in 
                       p_FILE_NAME = query.FileName,

                       //out
                       ret_file_owner = ret_file_owner,
                       ret_code = ret_code
                   });

                if (ret_code.Value.ToString() == "0")
                {
                    file_owner = ret_file_owner.Value.ToSafeString();
                }

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ret_code, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log, "Error", ex.Message, "");
            }

            #endregion

            if (file_owner != null)
            {
                try
                {
                    log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, query, query.TransectionId, "DeleteFile", "DeleteFileQueryHandler", "", "DELETE", "FBBCONFIG");
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

                    var fileName = System.IO.Path.Combine(cfg.remotePath, query.FileName);

                    resp = FileHelper.NasRemoveFile(cfg.UserName
                                                 , cfg.Password
                                                 , cfg.Host
                                                 , fileName, out string msg);

                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, resp, log, msg, cfg.UserName, "FBBCONFIG");

                    if (resp.Delete)
                    {
                        try
                        {
                            var command = new FbbNasUpdateLogCommand
                            {
                                file_name = query.FileName,
                                file_owner = query.Username, // form current user
                                nas_path = cfg.remotePath,
                                action = "delete",
                            };

                            logNasUpdate = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, command, command.file_name, "FbbNasUpdateLogCommandHandler", "FbbNasUpdateLogCommandHandler", command.file_name, "FBB", "WEB");

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
                                resp.Message = "Log Command was created.";
                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ret_codeUpdate, logNasUpdate, "Success", "", "");
                            }
                            else
                            {
                                resp.Message = "Log Command failed.";
                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ret_codeUpdate, logNasUpdate, "Failed", ret_codeUpdate.Value.ToSafeString(), "");

                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message);

                            if (null != log)
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
            }
            else
            {
                resp.Message = "The file cannot be deleted. (not owner)";
            }
            return resp;
        }
    }
}
