namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    using AIRNETEntity.Models;
    using Oracle.ManagedDataAccess.Client;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.ExWebServices;
    using WBBContract.Queries.ExWebServices.FbbCpGw;
    using WBBData.DbIteration;
    using WBBData.Repository;
    using WBBEntity.Extensions;
    using WBBEntity.Models;
    using WBBEntity.PanelModels.ExWebServiceModels;

    public class GetSFFInternetProfileQueryHandler : IQueryHandler<GetSFFInternetProfileQuery, evOMServiceCheckChangePromotionModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _obj;
        private readonly IWBBUnitOfWork _uow;

        public GetSFFInternetProfileQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<string> obj,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lovService = lov;
            _sffLog = sffLog;
            _intfLog = intfLog;
            _obj = obj;
            _uow = uow;
        }

        public evOMServiceCheckChangePromotionModel Handle(GetSFFInternetProfileQuery query)
        {
            InterfaceLogCommand log = null;

            var checkChangePromotionModel = new evOMServiceCheckChangePromotionModel();

            //var result = new List<string>();

            try
            {
                var sffParamModel = new SFFInternetProfile();
                sffParamModel.IDCardNo = query.IDCardNo;
                sffParamModel.InternetNo = query.InternetNo;

                var request = new SFFServices.SffRequest();
                request.Event = "evOMServiceCheckChangeService";

                var paramList = new SFFServices.ParameterList();

                sffParamModel.ServiceCode = (from t in _lovService.Get()
                                             where t.LOV_TYPE == "PACK_AIS_WIFI"
                                                  && t.LOV_NAME == "SERVICE_CODE"
                                                  && t.ACTIVEFLAG == "Y"
                                             select t.LOV_VAL1).FirstOrDefault();

                var promotionCode = (from t in _lovService.Get()
                                     where t.LOV_TYPE == "PACK_AIS_WIFI"
                                        && t.LOV_NAME == "PROMOTION_CODE"
                                        && t.ACTIVEFLAG == "Y"
                                     select t.LOV_VAL1).FirstOrDefault();

                var sffPrms = (from t in _lovService.Get()
                               where t.LOV_TYPE == "SFF_PARAMETER"
                                    && t.LOV_NAME == "serviceCheckChangeService"
                                    && t.ACTIVEFLAG == "Y"
                                    && (string.IsNullOrEmpty(t.LOV_VAL4) || t.LOV_VAL4 != "L1")
                               orderby t.ORDER_BY
                               select t)
                                  .ToList()
                                  .Select(t => new SFFServices.Parameter
                                  {
                                      Name = t.DISPLAY_VAL.ToSafeString(),
                                      Value = string.IsNullOrEmpty(WBBExtensions.GetPropValue(sffParamModel, t.LOV_VAL2.ToSafeString()).ToSafeString()) ?
                                            t.LOV_VAL2.ToSafeString() :
                                            WBBExtensions.GetPropValue(sffParamModel, t.LOV_VAL2.ToSafeString()).ToSafeString()
                                  }).ToArray();

                //foreach (var prm in sffPrms)
                //{
                //    switch (prm.Name)
                //    {
                //        case "mobileNo": prm.Value = query.InternetNo; 
                //            break;
                //        case "serviceCode": prm.Value = serviceCode;
                //            break;
                //        case "IdCardNo": prm.Value = query.IDCardNo;
                //            break;
                //        default:
                //            break;
                //    }
                //}

                paramList.Parameter = sffPrms;
                request.ParameterList = paramList;

                //var paramArray = new SFFServices.Parameter[4];
                //var param0 = new SFFServices.Parameter();
                //var param1 = new SFFServices.Parameter();
                //var param2 = new SFFServices.Parameter();
                //var param3 = new SFFServices.Parameter();

                //param0.Name = "mobileNo";
                //param0.Value = query.InternetNo;

                //param1.Name = "serviceCode";
                //param1.Value = serviceCode;

                //param2.Name = "orderChannel";
                //param2.Value = "WEB";

                //param3.Name = "IdCardNo";
                //param3.Value = query.IDCardNo;

                //paramArray[0] = param0;
                //paramArray[1] = param1;
                //paramArray[2] = param2;
                //paramArray[3] = param3;

                //paramList.Parameter = paramArray;
                //request.ParameterList = paramList;

                //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog,
                //                                                    request, query.TransactionID,
                //                                                    "GetSFFInternetProfileQuery",
                //                                                    "evOMServiceCheckChangeService",
                //                                                    query.IDCardNo);
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.TransactionID, "GetSFFInternetProfileQuery", "evOMServiceCheckChangeService", query.IDCardNo, "FBB", "");

                SffServiceConseHelper.CheckChangePromotion(request, checkChangePromotionModel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var ret_order_no = new OracleParameter();
                ret_order_no.OracleDbType = OracleDbType.Varchar2;
                ret_order_no.Size = 2000;
                ret_order_no.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var executeResult = _obj.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_REGISTER_AISWIFI",
                    out paramOut,
                       new
                       {
                           p_internet_no = query.InternetNo,
                           p_id_card_no = query.IDCardNo,
                           p_package_code = promotionCode,
                           p_order_type = "Change Service",
                           p_sff_return_code = checkChangePromotionModel.ReturnCode,
                           p_sff_return_message = checkChangePromotionModel.ReturnMessage,
                           p_transaction_id = query.TransactionID,
                           p_user_name = "FBBMOB",
                           ret_code = ret_code,
                           ret_message = ret_message,
                           ret_order_no = ret_order_no,
                       }
                );

                checkChangePromotionModel.OrderNo = ret_order_no.Value.ToSafeString();

                //SffServiceConseHelper
                //    .EndInterfaceSffLog(_uow, _intfLog, checkChangePromotionModel, log,
                //                            "Success", checkChangePromotionModel.ReturnCode);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkChangePromotionModel, log, "Success", checkChangePromotionModel.ReturnCode, "");

                //result.Add(checkChangePromotionModel.ReturnCode);


            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    //SffServiceConseHelper
                    //    .EndInterfaceSffLog(_uow, _intfLog, checkChangePromotionModel,
                    //                            log, "Failed", ex.GetErrorMessage());
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkChangePromotionModel, log, "Failed", ex.GetErrorMessage(), "");
                }

                return checkChangePromotionModel;
            }

            return checkChangePromotionModel;
        }
    }

    public class evOMServiceCheckChangeServiceQueryHandler : IQueryHandler<evOMServiceCheckChangeServiceQuery, evOMServiceCheckChangePromotionModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _custProfile;
        private readonly IEntityRepository<string> _obj;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> _AIR_SFF_SERVICE_CODE;
        private readonly IQueryHandler<evOMQueryListServiceAndPromotionByPackageTypeQuery, evOMQueryListServiceAndPromotionByPackageTypeModel> _evOMQueryListServiceAndPromotionByPackageType;

        public evOMServiceCheckChangeServiceQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CUST_PROFILE> custProfile,
            IEntityRepository<string> obj,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<AIR_SFF_SERVICE_CODE> AIR_SFF_SERVICE_CODE,
            IQueryHandler<evOMQueryListServiceAndPromotionByPackageTypeQuery, evOMQueryListServiceAndPromotionByPackageTypeModel> evOMQueryListServiceAndPromotionByPackageType)
        {
            _logger = logger;
            _lovService = lov;
            _sffLog = sffLog;
            _intfLog = intfLog;
            _custProfile = custProfile;
            _obj = obj;
            _uow = uow;
            _AIR_SFF_SERVICE_CODE = AIR_SFF_SERVICE_CODE;
            _evOMQueryListServiceAndPromotionByPackageType = evOMQueryListServiceAndPromotionByPackageType;
        }

        public evOMServiceCheckChangePromotionModel Handle(evOMServiceCheckChangeServiceQuery query)
        {
            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;

            evOMServiceCheckChangePromotionModel result = new evOMServiceCheckChangePromotionModel();

            try
            {
                var sffParamModel = new SFFInternetProfile();
                sffParamModel.IDCardNo = query.IDCardNo;
                sffParamModel.InternetNo = query.InternetNo;

                var request = new SFFServices.SffRequest();
                request.Event = "evOMServiceCheckChangeService";

                var paramList = new SFFServices.ParameterList();
                /// V20.3
                //sffParamModel.ServiceCode = (from t in _custProfile.Get()
                //                             where t.CUST_NON_MOBILE == query.InternetNo
                //                             select t.SERVICE_CODE).FirstOrDefault();

                //R20.6
                var evOMQueryListServiceAndPromotionByPackageType = new evOMQueryListServiceAndPromotionByPackageTypeQuery() { mobileNo = query.InternetNo, idCard = query.IDCardNo, FullUrl = query.FullUrl };
                var servicelist = _evOMQueryListServiceAndPromotionByPackageType.Handle(evOMQueryListServiceAndPromotionByPackageType);

                string access_mode = servicelist.access_mode.ToSafeString();
                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, access_mode, query.TransactionID, "CheckAcessType", "CheckAcessType", query.IDCardNo, "FBB", "");
                var servicecode = (from z in _AIR_SFF_SERVICE_CODE.Get()
                                   where z.PRODUCT_NAME == access_mode
                                   select z.SERVICE_CODE).FirstOrDefault();
                sffParamModel.ServiceCode = servicecode.ToSafeString();
                //---End R20.6
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, servicecode, log, "Success", "", "");
                var promotionCode = (from t in _lovService.Get()
                                     where t.LOV_TYPE == "PACK_AIS_WIFI"
                                        && t.LOV_NAME == "PROMOTION_CODE"
                                        && t.ACTIVEFLAG == "Y"
                                     select t.LOV_VAL1).FirstOrDefault();

                var sffPrms = (from t in _lovService.Get()
                               where t.LOV_TYPE == "SFF_PARAMETER"
                                    && t.LOV_NAME == "serviceCheckChangeService"
                                    && t.ACTIVEFLAG == "Y"
                                    && (string.IsNullOrEmpty(t.LOV_VAL4) || t.LOV_VAL4 != "L1")
                               orderby t.ORDER_BY
                               select t)
                                  .ToList()
                                  .Select(t => new SFFServices.Parameter
                                  {
                                      Name = t.DISPLAY_VAL.ToSafeString(),
                                      Value = string.IsNullOrEmpty(WBBExtensions.GetPropValue(sffParamModel, t.LOV_VAL2.ToSafeString()).ToSafeString()) ?
                                            t.LOV_VAL2.ToSafeString() :
                                            WBBExtensions.GetPropValue(sffParamModel, t.LOV_VAL2.ToSafeString()).ToSafeString()
                                  }).ToArray();

                paramList.Parameter = sffPrms;
                request.ParameterList = paramList;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.TransactionID, "evOMServiceCheckChangeService", "evOMServiceCheckChangeServiceQueryHandler", query.IDCardNo, "FBB", "");
                SffServiceConseHelper.CheckChangePromotion(request, result);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", result.ReturnCode, "");

            }
            catch (Exception ex)
            {

                if (null != log)
                {

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.GetErrorMessage(), "");
                }

            }

            return result;
        }

    }
}