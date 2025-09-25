using System;
using System.Collections.Generic;
using System.Web;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.SftpQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.SftpHandlers
{
    public class ListfilesQueryHandler : IQueryHandler<ListfilesQuery, List<ListfilesModels>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public ListfilesQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public List<ListfilesModels> Handle(ListfilesQuery query)
        {
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            try
            {
                log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, query, query.TransectionId, "GET FILENAME", "ListfilesQueryHandler", "", "LISTFILES", "FBBCONFIG");


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

                var result = sftp.Listfiles(cfg.UserName, keyfile, cfg.remotePath, cfg.ConfigType, out string msg);
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, msg, keyfile, "FBBCONFIG");
                return result;

            }
            catch (Exception ex)
            {
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex, log, ex.Message.ToString(), "", "FBBCONFIG");
                return null;
            }
        }
    }
}
