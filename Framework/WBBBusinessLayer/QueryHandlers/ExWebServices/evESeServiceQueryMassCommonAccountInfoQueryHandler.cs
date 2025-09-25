using System;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.QueryHandlers.Commons.ATN;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.ExWebServices.Contract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class evESeServiceQueryMassCommonAccountInfoQueryHandler : IQueryHandler<evESeServiceQueryMassCommonAccountInfoQuery, evESeServiceQueryMassCommonAccountInfoModel>
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

        public evESeServiceQueryMassCommonAccountInfoQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_INTERFACE_LOG_3BB> initfThreeBBlog,
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

        public evESeServiceQueryMassCommonAccountInfoModel Handle(evESeServiceQueryMassCommonAccountInfoQuery query)
        {
            InterfaceLogCommand log = null;
            var model = new evESeServiceQueryMassCommonAccountInfoModel();

            if (query.inMobileNo?.Length == 10)
            {
                #region SFF (AIS Fiber Account)
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

                    //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog, request, query.inMobileNo,
                    //    "evESeServiceQueryMassCommonAccountInfoQuery", "evESeServiceQueryMassCommonAccountInfo", query.inCardNo);
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.inMobileNo + query.ClientIP, "evESeServiceQueryMassCommonAccountInfoQuery", "evESeServiceQueryMassCommonAccountInfo", query.inCardNo, "FBB|" + query.FullUrl, "");

                    SffServiceConseHelper.GetMassCommonAccountInfo(_logger, _uow, _lovService,
                                                                        _sffLog, query, request, model, _custPackage, _custProfile, _register, _packageTran);

                    //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                    //    "Success", model.errorMessage);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.errorMessage, "");
                }
                catch (Exception e)
                {
                    //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                    //    "Failed", model.errorMessage);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", e.GetErrorMessage(), "");
                }
                #endregion

                #region SACF (3bb Fiber Account)
                if (string.IsNullOrEmpty(model?.outServiceMobileNo))//หาเลข 88 ใน AIS ไม่เจอ
                {
                    InterfaceValidate3BBProfile(log, query, model);
                }
                #endregion
            }
            else if (query.inMobileNo?.Length == 9) //if digit-9 use 3bb only
            {
                InterfaceValidate3BBProfile(log, query, model);
            }



            return model;
        }

        public void InterfaceValidate3BBProfile(InterfaceLogCommand log, evESeServiceQueryMassCommonAccountInfoQuery query, evESeServiceQueryMassCommonAccountInfoModel model)
        {
            try
            {
                _logger.Info("Call evESeService SACF");
                _logger.Info("inMobileNo: " + query.inMobileNo + ", inIDCardNo: " + query.inCardNo + ", inIDCardType: " + query.inCardType);

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query.inMobileNo, query.inMobileNo + query.ClientIP, "evESeServiceQueryMassCommonAccountInfoQuery", "evESeServiceQueryMassCommonAccountInfo", query.inCardNo, "FBB|" + query.FullUrl, "");

                if (SacfServiceConseHelper.GetUuidAndMobileByFbbId(_lovService, query, model))
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, string.IsNullOrEmpty(model.outIPCamera3bbErrorMessage) ? "Success" : "Failed", model.outIPCamera3bbErrorMessage, ""); //Success
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", ex.GetErrorMessage(), ""); //Failed
            }
        }
    }

    public class evOMCheckDeviceContractHandler : IQueryHandler<evOMCheckDeviceContractQuery, evOMCheckDeviceContractModel>
    {
        private const string CONF_ENABLE_APIMAPPINGSKY = "ENABLE_EVOMCHECKDEVICECONTRACT_TOBE_APIMAPPINGSKY";
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetQueryAgreementCheckDeviceContractFbbRequest, GetQueryAgreementCheckDeviceContractFbbResult> _qcCheckContract;

        public evOMCheckDeviceContractHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IQueryHandler<GetQueryAgreementCheckDeviceContractFbbRequest, GetQueryAgreementCheckDeviceContractFbbResult> qcCheckContract)
        {
            _logger = logger;
            _lovService = lovService;
            _intfLog = intfLog;
            _uow = uow;
            _qcCheckContract = qcCheckContract;
        }

        public evOMCheckDeviceContractModel Handle(evOMCheckDeviceContractQuery query)
        {
            var confApiMappingSky = _lovService.Get().Where(x => x.LOV_NAME == CONF_ENABLE_APIMAPPINGSKY).FirstOrDefault() ?? new FBB_CFG_LOV();
            if (confApiMappingSky?.ACTIVEFLAG == "Y")
            {
                return HandleBySky(query);
            }
            return HandleBySff(query);
        }

        public evOMCheckDeviceContractModel HandleBySky(evOMCheckDeviceContractQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new evOMCheckDeviceContractModel
            {
                contractFlagFbb = "",
                countContractFbb = "",
                fbbLimitContract = "",
                errorMessage = ""
            };
            var success = "";
            var remark = "";

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.inMobileNo, "evOMCheckDeviceContractQuery", "evOMCheckDeviceContract", query.inMobileNo, "FBB|" + query.FullUrl, "");

                var req = new GetQueryAgreementCheckDeviceContractFbbRequest
                {
                    fibrenetId = query.inMobileNo,
                    idCardNo = query.inCardNo,
                    idCardType = query.inCardType,
                    sourceSystem = ATNConstants.WEBRegister
                };
                var res = _qcCheckContract.Handle(req);
                if (!string.IsNullOrEmpty(res?.errorMessage))
                {
                    result.errorMessage = res?.errorMessage;
                    remark = result.errorMessage;
                }
                if (string.IsNullOrEmpty(res?.contractFlagFbb)
                    && string.IsNullOrEmpty(res?.countContractFbb)
                    && string.IsNullOrEmpty(res?.fbbLimitContract))
                {
                    result.errorMessage = "No data.";
                    remark = result.errorMessage;
                }

                result.contractFlagFbb = res?.contractFlagFbb;
                result.countContractFbb = res?.countContractFbb;
                result.fbbLimitContract = res?.fbbLimitContract;

                success = "Success";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error when call {GetType().Name} : {ex.ToSafeString()}");
                result.errorMessage = "Error Exception";
                success = "Failed";
                remark = ex.Message;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, success, remark, "");
            }
            return result;
        }


        public evOMCheckDeviceContractModel HandleBySff(evOMCheckDeviceContractQuery query)
        {
            InterfaceLogCommand log = null;
            evOMCheckDeviceContractModel model = new evOMCheckDeviceContractModel()
            {
                contractFlagFbb = "",
                countContractFbb = "",
                fbbLimitContract = "",
                errorMessage = "",
                countContract = "",
                contractExpireDt = "",
                contractFlag = "",
                sameNumber = "",
                returnCode = "",
                blackListFlag = "",
                limitContract = "",
                contractProfileCount = "",
                idCardNo = "",
                remainLimitMobile = "",
                contractExpireDtFbb = "",
                contractProfileCountFbb = "",
                sameFbbNumber = "",
            };

            try
            {
                var request = new SFFServices.SffRequest();
                request.Event = "evOMCheckDeviceContract";

                var paramArray = new SFFServices.Parameter[2];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();

                param0.Name = "idCardNo";
                param0.Value = query.inCardNo;
                param1.Name = "idCardType";
                param1.Value = query.inCardType;

                paramArray[0] = param0;
                paramArray[1] = param1;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.inMobileNo + query.ClientIP, "evOMCheckDeviceContractQuery", "evOMCheckDeviceContract", query.inCardNo, "FBB|" + query.FullUrl, "");

                SffServiceConseHelper.OMCheckDeviceContract(request, model);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.errorMessage, "");
            }
            catch (Exception e)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", e.GetErrorMessage(), "");
            }

            return model;
        }


    }

    public class evFBBGenerateFBBNoHandler : IQueryHandler<evFBBGenerateFBBNoQuery, evFBBGenerateFBBNoModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public evFBBGenerateFBBNoHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lovService = lovService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public evFBBGenerateFBBNoModel Handle(evFBBGenerateFBBNoQuery query)
        {
            InterfaceLogCommand log = null;
            evFBBGenerateFBBNoModel model = new evFBBGenerateFBBNoModel()
            {
                errorMessage = ""
            };

            List<FBB_CFG_LOV> loveList = _lovService.Get(lov => lov.LOV_TYPE == "FBB_GENERATE_FBBNO_CONFIG").ToList();
            if (loveList != null && loveList.Count == 3)
            {
                try
                {
                    var request = new SFFServices.SffRequest();
                    request.Event = "evFBBGenerateFBBNo";

                    var paramArray = new SFFServices.Parameter[3];
                    var param0 = new SFFServices.Parameter();
                    var param1 = new SFFServices.Parameter();
                    var param2 = new SFFServices.Parameter();

                    param0.Name = "genref";
                    param0.Value = loveList.FirstOrDefault(t => t.LOV_NAME == "genref").LOV_VAL1.ToSafeString();
                    param1.Name = "channel";
                    param1.Value = loveList.FirstOrDefault(t => t.LOV_NAME == "channel").LOV_VAL1.ToSafeString();
                    param2.Name = "userName";
                    param2.Value = loveList.FirstOrDefault(t => t.LOV_NAME == "userName").LOV_VAL1.ToSafeString();

                    paramArray[0] = param0;
                    paramArray[1] = param1;
                    paramArray[2] = param2;

                    var paramList = new SFFServices.ParameterList();
                    paramList.Parameter = paramArray;

                    request.ParameterList = paramList;

                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.inMobileNo, "evFBBGenerateFBBNoQuery", "evFBBGenerateFBBNo", query.inMobileNo, "FBB|" + query.FullUrl, "");

                    using (var service = new SFFServices.SFFServiceService())
                    {
                        SFFServices.SffResponse data = service.ExecuteService(request);

                        if (data != null)
                        {
                            string returnCode = "";
                            foreach (var a in data.ParameterList.Parameter)
                            {
                                if (a.Name == "FBBNo") model.FBBNo = a.Value.ToSafeString();
                            }
                            if (returnCode != "")
                            {
                                model.errorMessage = "returnCode : " + returnCode;
                            }
                        }
                        else
                        {
                            model.errorMessage = "No SffResponse data.";
                        }
                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.errorMessage, "");
                }
                catch (Exception e)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", e.GetErrorMessage(), "");
                }
            }
            else
            {
                model.errorMessage = "No config";
            }
            return model;
        }

    }

    public class evOMQueryContractHandler : IQueryHandler<evOMQueryContractQuery, evOMQueryContractModel>
    {
        private const string CONF_ENABLE_APIMAPPINGSKY = "ENABLE_EVOMQUERYCONTRACT_TOBE_APIMAPPINGSKY";
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IQueryHandler<GetQueryAgreementContractFbbRequest, List<GetQueryAgreementContractFbbResult>> _contractFbbHandler;

        public evOMQueryContractHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IQueryHandler<GetQueryAgreementContractFbbRequest, List<GetQueryAgreementContractFbbResult>> contractFbbHandler)
        {
            _logger = logger;
            _lovService = lovService;
            _intfLog = intfLog;
            _uow = uow;
            _contractFbbHandler = contractFbbHandler;
        }
        public evOMQueryContractModel Handle(evOMQueryContractQuery query)
        {
            var confApiMappingSky = _lovService.Get().Where(x => x.LOV_NAME == CONF_ENABLE_APIMAPPINGSKY).FirstOrDefault() ?? new FBB_CFG_LOV();
            if (confApiMappingSky?.ACTIVEFLAG == "Y")
            {
                return HandleBySky(query);
            }
            return HandleBySff(query);
        }

        public evOMQueryContractModel HandleBySky(evOMQueryContractQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new evOMQueryContractModel()
            {
                evOMQueryContractDatas = null,
                errorMessage = ""
            };
            var success = "";
            var remark = "";
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.inMobileNo, "evOMQueryContract", GetType().Name, query.inMobileNo, "FBB|" + query.FullUrl, "");

                var req = new GetQueryAgreementContractFbbRequest
                {
                    transactionId = query.inMobileNo,
                    option = "4",
                    fibrenetId = query.inMobileNo.ToSafeString(),
                    idCardNo = "",
                    profileType = "All",
                    contractNo = "",
                    sourceSystem = ATNConstants.WEBRegister
                };
                var response = _contractFbbHandler.Handle(req);
                var res = response.FirstOrDefault();
                if (!string.IsNullOrEmpty(res?.errorMessage))
                {
                    result.errorMessage = res?.errorMessage;
                    return result;
                }
                if (res?.profileTypeList == null || res?.profileTypeList?.Any() != true)
                {
                    result.errorMessage = "No data.";
                    return result;
                }

                result.evOMQueryContractDatas = new List<evOMQueryContractData>();
                foreach (var ptList in res?.profileTypeList ?? new List<QueryAgreementProfileTypeItem>())
                {
                    foreach (var ctList in ptList?.contractList ?? new List<QueryAgreementContractItem>())
                    {
                        foreach (var cdList in ctList?.contractDetailList ?? new List<QueryAgreementContractDetailItem>())
                        {
                            var data = new evOMQueryContractData
                            {
                                penalty = cdList.penalty,
                                tdmContractId = cdList.contractId,
                                contractNo = cdList.contractNo,
                                duration = cdList.duration
                            };
                            result.evOMQueryContractDatas.Add(data);
                        }
                    }
                }

                success = "Success";
            }
            catch (Exception ex)
            {
                _logger.Error($"Error when call {GetType().Name} : {ex.ToSafeString()}");
                result.errorMessage = "Error Exception";
                success = "Failed";
                remark = ex.Message;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, success, remark, "");
            }

            return result;
        }

        public evOMQueryContractModel HandleBySff(evOMQueryContractQuery query)
        {
            InterfaceLogCommand log = null;
            evOMQueryContractModel model = new evOMQueryContractModel()
            {
                evOMQueryContractDatas = new List<evOMQueryContractData>(),
                errorMessage = ""
            };


            try
            {
                var request = new SFFServices.SffRequest();
                request.Event = "evOMQueryContract";

                var paramArray = new SFFServices.Parameter[4];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();
                var param3 = new SFFServices.Parameter();

                param0.Name = "option";
                param0.Value = "4";
                param1.Name = "mobileNo";
                param1.Value = query.inMobileNo.ToSafeString();
                param2.Name = "profileType";
                param2.Value = "All";
                param3.Name = "contractNo";
                param3.Value = "";

                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;
                paramArray[3] = param3;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.inMobileNo, "evOMQueryContract", "evOMQueryContractHandler", query.inMobileNo, "FBB|" + query.FullUrl, "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    SFFServices.SffResponse data = service.ExecuteService(request);

                    if (data != null)
                    {
                        string returnCode = "";
                        foreach (var item in data.ParameterList.Parameter)
                        {
                            if (item.Name == "errorMessage") returnCode = item.Value.ToSafeString();
                        }
                        if (data.ParameterList.ParameterList1 != null && data.ParameterList.ParameterList1.Count() > 0
                            && data.ParameterList.ParameterList1[0].ParameterList1 != null
                            && data.ParameterList.ParameterList1[0].ParameterList1.Count() > 0)
                        {
                            foreach (var item1 in data.ParameterList.ParameterList1[0].ParameterList1[0].ParameterList1)
                            {
                                evOMQueryContractData evOMQueryContractDataItem = new evOMQueryContractData();
                                foreach (var a in item1.Parameter)
                                {
                                    if (a.Name == "penalty") evOMQueryContractDataItem.penalty = a.Value.ToSafeString();
                                    if (a.Name == "tdmContractId") evOMQueryContractDataItem.tdmContractId = a.Value.ToSafeString();
                                    if (a.Name == "contractNo") evOMQueryContractDataItem.contractNo = a.Value.ToSafeString();
                                    if (a.Name == "duration") evOMQueryContractDataItem.duration = a.Value.ToSafeString();
                                }
                                model.evOMQueryContractDatas.Add(evOMQueryContractDataItem);
                            }
                        }
                        else
                        {
                            model.errorMessage = "No data.";
                            model.evOMQueryContractDatas = null;
                        }

                        if (returnCode != "")
                        {
                            model.errorMessage = "returnCode : " + returnCode;
                            model.evOMQueryContractDatas = null;
                        }
                    }
                    else
                    {
                        model.errorMessage = "No SffResponse data.";
                        model.evOMQueryContractDatas = null;
                    }
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.errorMessage, "");
            }
            catch (Exception e)
            {
                model.errorMessage = "Error Exception";
                model.evOMQueryContractDatas = null;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", e.GetErrorMessage(), "");
            }

            return model;
        }

    }
}
