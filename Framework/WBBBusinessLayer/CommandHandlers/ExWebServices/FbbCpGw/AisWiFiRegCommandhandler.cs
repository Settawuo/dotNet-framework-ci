using System;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw
{
    public class AisWiFiRegCommandhandler : ICommandHandler<AisWiFiRegCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_SFF_CHKPROFILE_LOG> _sffLog;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public AisWiFiRegCommandhandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_SFF_CHKPROFILE_LOG> sffLog,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lovService = lov;
            _sffLog = sffLog;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(AisWiFiRegCommand command)
        {
            var confirmChangePromotionModel = new evOMServiceConfirmChangePromotionModel();

            InterfaceLogCommand log = null;

            try
            {

                var request = new SFFServices.SffRequest();
                request.Event = "evOMServiceConfirmChangeService";

                var paramList = new SFFServices.ParameterList();

                command.ServiceCode = (from t in _lovService.Get()
                                       where t.LOV_TYPE == "PACK_AIS_WIFI"
                                            && t.LOV_NAME == "SERVICE_CODE"
                                            && t.ACTIVEFLAG == "Y"
                                       select t.LOV_VAL1).FirstOrDefault();

                command.PromotionCode = (from t in _lovService.Get()
                                         where t.LOV_TYPE == "PACK_AIS_WIFI"
                                            && t.LOV_NAME == "PROMOTION_CODE"
                                            && t.ACTIVEFLAG == "Y"
                                         select t.LOV_VAL1).FirstOrDefault();

                var sffPrms = (from t in _lovService.Get()
                               where t.LOV_TYPE == "SFF_PARAMETER"
                                && t.LOV_NAME == "serviceConfirmChangeService"
                                       && t.ACTIVEFLAG == "Y"
                                && (string.IsNullOrEmpty(t.LOV_VAL4) || t.LOV_VAL4 != "L1")
                               orderby t.ORDER_BY
                               select t)
                                .ToList()
                               .Select(t => new SFFServices.Parameter
                               {
                                   Name = t.DISPLAY_VAL.ToSafeString(),
                                   Value = string.IsNullOrEmpty(WBBExtensions.GetPropValue(command, t.LOV_VAL2.ToSafeString()).ToSafeString()) ?
                                            t.LOV_VAL2.ToSafeString() :
                                            WBBExtensions.GetPropValue(command, t.LOV_VAL2.ToSafeString()).ToSafeString()
                               })
                               .ToArray();

                var paramList1 = (from t in _lovService.Get()
                                  where t.LOV_TYPE == "SFF_PARAMETER"
                                    && t.LOV_NAME == "serviceConfirmChangeService"
                                    && t.LOV_VAL4 == "L1"
                                    //&& t.DISPLAY_VAL == "ServiceItem" || t.DISPLAY_VAL == "PromotionItem"
                                    && t.ACTIVEFLAG == "Y"
                                  group t by new { t.DISPLAY_VAL, t.ORDER_BY } into g
                                  orderby g.Key.ORDER_BY
                                  select g)
                                  .ToList()
                                  .Select(g => new SFFServices.ParameterList
                                  {
                                      ParameterType = g.Key.DISPLAY_VAL.ToSafeString(),
                                      Parameter = CreateSFFParameter(g.Key.DISPLAY_VAL.ToSafeString(), command),
                                  })
                                  .ToArray();

                //var paramArray = new SFFServices.Parameter[17];
                //var param0 = new SFFServices.Parameter();
                //var param1 = new SFFServices.Parameter();
                //var param2 = new SFFServices.Parameter();
                //var param3 = new SFFServices.Parameter();
                //var param4 = new SFFServices.Parameter();
                //var param5 = new SFFServices.Parameter();
                //var param6 = new SFFServices.Parameter();
                //var param7 = new SFFServices.Parameter();
                //var param8 = new SFFServices.Parameter();
                //var param9 = new SFFServices.Parameter();
                //var param10 = new SFFServices.Parameter();
                //var param11 = new SFFServices.Parameter();
                //var param12 = new SFFServices.Parameter();
                //var param13 = new SFFServices.Parameter();
                //var param14 = new SFFServices.Parameter();
                //var param15 = new SFFServices.Parameter();
                //var param16 = new SFFServices.Parameter();

                //param0.Name = "orderType";
                //param0.Value = "Change Service";
                //param1.Name = "mobileNo";
                //param1.Value = command.InternetNo;
                //param2.Name = "orderReason";
                //param2.Value = "";
                //param3.Name = "orderChannel";
                //param3.Value = "WEB";
                //param4.Name = "userName";
                //param4.Value = "FBBMOB";
                //param5.Name = "ascCode";
                //param5.Value = "";
                //param6.Name = "locationCd";
                //param6.Value = "";
                //param7.Name = "club900Mobile";
                //param7.Value = "";
                //param8.Name = "referenceNo";
                //param8.Value = command.CheckChangePromotionModel.OrderNo;
                //param9.Name = "actionStatus(Service)";
                //param9.Value = "Add";
                //param10.Name = "serviceCode";
                //param10.Value = serviceCode;
                //param11.Name = "actionStatus(Promoiton)";
                //param11.Value = "Add";
                //param12.Name = "promotionCode";
                //param12.Value = promotionCode;
                //param13.Name = "overRuleStartDate";
                //param13.Value = "I";
                //param14.Name = "productSeq";
                //param14.Value = "";
                //param15.Name = "waiveFlag(Promotion)";
                //param15.Value = "";
                //param16.Name = "chargeNode";
                //param16.Value = "";

                //paramArray[0] = param0;
                //paramArray[1] = param1;
                //paramArray[2] = param2;
                //paramArray[3] = param3;
                //paramArray[4] = param4;
                //paramArray[5] = param5;
                //paramArray[6] = param6;
                //paramArray[7] = param7;
                //paramArray[8] = param8;
                //paramArray[9] = param9;
                //paramArray[10] = param10;
                //paramArray[11] = param11;
                //paramArray[12] = param12;
                //paramArray[13] = param13;
                //paramArray[14] = param14;
                //paramArray[15] = param15;
                //paramArray[16] = param16;

                paramList.Parameter = sffPrms;
                paramList.ParameterList1 = paramList1;
                request.ParameterList = paramList;

                //log = SffServiceConseHelper
                //                .StartInterfaceSffLog(_uow, _intfLog, request,
                //                                        command.TransactionID,
                //                                        "ConfirmChangeService",
                //                                        "evOMServiceConfirmChangeService",
                //                                        command.IDCardNo);
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, command.TransactionID, "ConfirmChangeService", "evOMServiceConfirmChangeService", command.IDCardNo, "FBB", "");

                SffServiceConseHelper
                    .ConfirmChangePromotion(request, confirmChangePromotionModel);

                //SffServiceConseHelper
                //        .EndInterfaceSffLog(_uow, _intfLog,
                //                                confirmChangePromotionModel, log,
                //                                "Success", confirmChangePromotionModel.SuccessFlag);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, confirmChangePromotionModel, log, "Success", confirmChangePromotionModel.SuccessFlag, "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    //SffServiceConseHelper
                    //    .EndInterfaceSffLog(_uow, _intfLog,
                    //                            confirmChangePromotionModel, log,
                    //                            "Failed", ex.GetErrorMessage());
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, confirmChangePromotionModel, log, "Failed", ex.GetErrorMessage(), "");
                }
            }
        }

        private SFFServices.Parameter[] CreateSFFParameter(string paramName, AisWiFiRegCommand command)
        {
            var sffPrms = (from t in _lovService.Get()
                           where t.LOV_TYPE == "SFF_PARAMETER"
                            && t.LOV_NAME == "serviceConfirmChangeService"
                            && t.ACTIVEFLAG == "Y"
                            && t.DISPLAY_VAL == paramName
                           select t)
                           .ToList()
                           .Select(t => new SFFServices.Parameter
                           {
                               Name = t.LOV_VAL1.ToSafeString(),
                               Value = string.IsNullOrEmpty(WBBExtensions.GetPropValue(command, t.LOV_VAL2.ToSafeString()).ToSafeString()) ?
                                            t.LOV_VAL2.ToSafeString() :
                                            WBBExtensions.GetPropValue(command, t.LOV_VAL2.ToSafeString()).ToSafeString()
                           }).ToArray();


            return sffPrms;
        }
    }
}