using System;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class evESeServiceQueryMassCommonAccountInfoForAddBundlingQueryHandler : IQueryHandler<evESeServiceQueryMassCommonAccountInfoForAddBundlingQuery, evESeServiceQueryMassCommonAccountInfoModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CUST_PACKAGE> _custPackage;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _custProfile;
        private readonly IEntityRepository<FBB_REGISTER> _register;
        private readonly IEntityRepository<FBB_PACKAGE_TRAN> _packageTran;
        private readonly IWBBUnitOfWork _uow;

        public evESeServiceQueryMassCommonAccountInfoForAddBundlingQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog, IWBBUnitOfWork uow,
            IEntityRepository<FBB_CUST_PACKAGE> custPackage,
            IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_REGISTER> register,
            IEntityRepository<FBB_PACKAGE_TRAN> packageTran)
        {
            _logger = logger;
            _lovService = lovService;
            _sffLog = sffLog;
            _intfLog = intfLog;
            _uow = uow;
            _custPackage = custPackage;
            _custProfile = custProfile;
            _register = register;
            _packageTran = packageTran;
        }

        public evESeServiceQueryMassCommonAccountInfoModel Handle(evESeServiceQueryMassCommonAccountInfoForAddBundlingQuery query)
        {
            InterfaceLogCommand log = null;
            var model = new evESeServiceQueryMassCommonAccountInfoModel();

            try
            {
                var request = new SFFServices.SffRequest();
                request.Event = "evESeServiceQueryMassCommonAccountInfo";

                var paramArray = new SFFServices.Parameter[4];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();
                var param3 = new SFFServices.Parameter();

                param0.Name = "option";
                param0.Value = query.inOption;
                param1.Name = "inMobileNo";
                param1.Value = query.inMobileNo;
                param2.Name = "inIDCardNo";
                param2.Value = query.inCardNo;
                param3.Name = "inIDCardType";
                param3.Value = query.inCardType;

                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;
                paramArray[3] = param3;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                _logger.Info("Call evESeService SFF");
                _logger.Info("inMobileNo: " + query.inMobileNo + ", inIDCardNo: " + query.inCardNo + ", inIDCardType: " + query.inCardType);

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.inMobileNo, "evESeServiceQueryMassCommonAccountInfoForAddBundlingQuery", "evESeServiceQueryMassCommonAccountInfoForAddBundling", query.inCardNo, "FBB|" + query.FullUrl, "");

                SffServiceConseHelper.GetMassCommonAccountInfoForAddBundling(_logger, _uow, _lovService,
                                                                    _sffLog, query, request, model, _custPackage, _custProfile, _register, _packageTran);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.errorMessage, "");
            }
            catch (Exception)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", model.errorMessage, "");
            }

            return model;
        }
    }
}
