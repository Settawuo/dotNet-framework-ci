using System;
using System.Collections.Generic;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.SftpQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.SftpHandlers
{
    public class GetfilesAllQueryHandler : IQueryHandler<GetfilesAllQuery, List<ListfilesModels>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;

        public GetfilesAllQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public List<ListfilesModels> Handle(GetfilesAllQuery query)
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
                var cfg = GetNasServiceHelper.GetParameter(_objService, configFile); //test  1234 10.0.4.79 "//10.0.4.79/Users/test/Desktop/SAP/SAB_FBB_IN"

                var result = FileHelper.GetAllFileNas(cfg.UserName
                                                          , cfg.Password
                                                          , cfg.Host
                                                          , cfg.remotePath, out string msg);

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
