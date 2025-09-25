using System;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.SftpQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.SftpHandlers
{
    public class NasDownloadFileQueryHandler : IQueryHandler<NasDownloadFileQuery, DownloadFileModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public NasDownloadFileQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public DownloadFileModel Handle(NasDownloadFileQuery query)
        {
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            try
            {
                log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, query, query.TransectionId, "DownloadFile", "DownloadFileQueryHandler", "", "DOWNLOAD", "FBBCONFIG");


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

                var FileName = System.IO.Path.Combine(cfg.remotePath, query.FileName);
                var result = FileHelper.DownloadFile(cfg.UserName,
                                                    cfg.Password,
                                                    cfg.Host,
                                                    FileName, out string msg);

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, msg, cfg.UserName, "FBBCONFIG");
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
