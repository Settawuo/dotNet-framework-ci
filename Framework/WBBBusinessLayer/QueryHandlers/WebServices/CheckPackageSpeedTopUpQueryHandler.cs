using AIRNETEntity.Models;
using System;
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
    public class CheckPackageSpeedTopUpQueryHandler :
        IQueryHandler<CheckPackageSpeedTopUpPlayboxQuery, CheckPackageDownloadSpeedModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLovRepository;
        private readonly IAirNetEntityRepository<AIR_NEW_PACKAGE_MASTER> _objAirService;

        public CheckPackageSpeedTopUpQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<FBB_CFG_LOV> cfgLovRepository
            , IAirNetEntityRepository<AIR_NEW_PACKAGE_MASTER> objAirService)
        {
            _uow = uow;
            _intfLog = intfLog;
            _cfgLovRepository = cfgLovRepository;
            _objAirService = objAirService;
        }

        public CheckPackageDownloadSpeedModel Handle(CheckPackageSpeedTopUpPlayboxQuery query)
        {
            var checkPackageDownloadSpeedModel = new CheckPackageDownloadSpeedModel();
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "CheckPackageSpeedTopUp", "FBBService", null, "FBB", "");

                //Get config min speed
                var resultlov = from lov in _cfgLovRepository.Get()
                                where lov.LOV_TYPE == "TOPUP_PLAYBOX"
                                && lov.LOV_NAME == "MIN_DOWNLOAD_SPEED"
                                select lov;
                var lovspeed = resultlov.FirstOrDefault() ?? new FBB_CFG_LOV();
                var configspeed = string.IsNullOrEmpty(lovspeed.DISPLAY_VAL) ? 0 : Convert.ToInt16(lovspeed.DISPLAY_VAL);

                //get package speed
                var resultpackage = from pm in _objAirService.Get()
                                    where pm.SFF_PROMOTION_CODE == query.ProductCode
                                    select new
                                    {
                                        //donwloadSpeed = pm.DOWNLOAD_SPEED.Substring(1,pm.DOWNLOAD_SPEED.IndexOf("M", StringComparison.Ordinal)-1)
                                        donwloadSpeed = pm.DOWNLOAD_SPEED
                                    };
                var firstOrDefault = resultpackage.FirstOrDefault();
                var packspeed = firstOrDefault == null ? "0" : firstOrDefault.donwloadSpeed.ToSafeString().Substring(0, firstOrDefault.donwloadSpeed.ToSafeString().IndexOf("M", StringComparison.Ordinal) - 1);
                var ipackspeed = Convert.ToInt16(packspeed);

                //Validate speed
                if (ipackspeed != 0 && configspeed != 0 && ipackspeed < configspeed)
                    checkPackageDownloadSpeedModel.IsStatus = true;
                else
                    checkPackageDownloadSpeedModel.IsStatus = false;

                checkPackageDownloadSpeedModel.ReturnCode = "0";

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkPackageDownloadSpeedModel, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                checkPackageDownloadSpeedModel.ReturnCode = "-1";
                checkPackageDownloadSpeedModel.ReturnMessage = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkPackageDownloadSpeedModel, log, "Error", ex.Message, "");
            }

            return checkPackageDownloadSpeedModel;
        }
    }
}