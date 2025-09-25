using System;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public class GetMassCommonAccountQueryHandler : IQueryHandler<GetMassCommonAccountQuery, evESeServiceQueryMassCommonAccountInfoModel>
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

        public GetMassCommonAccountQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CUST_PACKAGE> custPackage,
            IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<FBB_REGISTER> register,
            IEntityRepository<FBB_PACKAGE_TRAN> packageTran)
        {
            _logger = logger;
            _lovService = lov;
            _sffLog = sffLog;
            _intfLog = intfLog;
            _uow = uow;
            _custPackage = custPackage;
            _custProfile = custProfile;
            _register = register;
            _packageTran = packageTran;
        }

        public evESeServiceQueryMassCommonAccountInfoModel Handle(GetMassCommonAccountQuery query)
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

                //param0.Name = "option";
                //param0.Value = query.inOption;
                //param1.Name = "inMobileNo";
                //param1.Value = "8905005319";
                //param2.Name = "inIDCardNo";
                //param2.Value = "D201409100002";
                //param3.Name = "inIDCardType";
                //param3.Value = "ID_CARD";

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

                //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog, request,
                //                                                    query.inMobileNo,
                //                                                    "GetMassCommonAccountQuery",
                //                                                    "evESeServiceQueryMassCommonAccountInfo",
                //                                                    query.inCardNo);
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.inMobileNo, "GetMassCommonAccountQuery", "evESeServiceQueryMassCommonAccountInfo", query.inCardNo, "FBB", "");

                SffServiceConseHelper.GetMassCommonAccountInfo(_logger, _uow, _lovService,
                                                                    _sffLog, query, request, model, _custPackage, _custProfile, _register, _packageTran);



                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                //                                            "Success", model.errorMessage);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.errorMessage, "");
            }
            catch (Exception)
            {
                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                //    "Failed", model.errorMessage);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", model.errorMessage, "");
            }

            //_uow.Persist();

            //if (string.IsNullOrEmpty(model.errorMessage))
            //    return true;

            //return false;
            return model;
        }
    }
}