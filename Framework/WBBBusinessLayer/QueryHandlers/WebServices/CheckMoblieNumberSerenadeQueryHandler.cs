using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckMoblieNumberSerenadeQueryHandler : IQueryHandler<CheckMobileNumberSerenadeQuery, CheckMobileNumberSerenadeModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public CheckMoblieNumberSerenadeQueryHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lov, IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _lov = lov;
            _intfLog = intfLog;
        }

        public CheckMobileNumberSerenadeModel Handle(CheckMobileNumberSerenadeQuery query)
        {
            InterfaceLogCommand log = null;
            var moblieNumberSerenadeModel = new CheckMobileNumberSerenadeModel();
            var productCd = string.Empty;
            var resultFlag = "N";
            var errorMessage = string.Empty;
            try
            {
                /* get promotion type and service type*/
                var inputparam = (from z in _lov.Get()
                                  where
                                      z.DISPLAY_VAL == "evOMQueryListServiceAndPromotionByPackageType" && z.ACTIVEFLAG == "Y" &&
                                      z.LOV_VAL5 == "FBBOR001"
                                  select z);

                var promotiontype = inputparam.FirstOrDefault(a => a.LOV_NAME == "promotionType") ?? new FBB_CFG_LOV();
                var servicetype = inputparam.FirstOrDefault(a => a.LOV_NAME == "serviceType") ?? new FBB_CFG_LOV();

                var param0 = new SFFServices.Parameter { Name = "mobileNo", Value = query.MobileNo };
                var param1 = new SFFServices.Parameter { Name = promotiontype.LOV_NAME, Value = promotiontype.LOV_VAL1 };
                var param2 = new SFFServices.Parameter { Name = servicetype.LOV_NAME, Value = servicetype.LOV_VAL1 };

                var paramArray = new[] { param0, param1, param2 };
                var paramList = new SFFServices.ParameterList { Parameter = paramArray };

                var request = new SFFServices.SffRequest
                {
                    Event = "evOMQueryListServiceAndPromotionByPackageType",
                    ParameterList = paramList
                };

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.MobileNo, "evOMQueryListServiceAndPromotionByPackageType", "evOMQueryListServiceAndPromotionByPackageType", "", "FBB|" + query.FullUrl, "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);
                    if (data != null && string.IsNullOrEmpty(data.ErrorMessage))
                    {
                        var result =
                            data.ParameterList.Parameter.FirstOrDefault(
                                item => item.Name == "resultFlag" && item.Value == "Y");
                        if (result != null)
                        {
                            foreach (var parameterList in data.ParameterList.ParameterList1)
                            {
                                var parameterPackMain =
                                    parameterList.Parameter.Where(
                                        parameter => parameter.Name == "productClass" && parameter.Value == "Main");
                                if (!parameterPackMain.Any()) continue;
                                var parameterProductCd = parameterList.Parameter.FirstOrDefault(parameter => parameter.Name == "productCD");
                                if (parameterProductCd != null)
                                {
                                    resultFlag = "Y";
                                    productCd = parameterProductCd.Value;
                                }
                                break;
                            }

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                        }
                        else
                        {
                            var resulterror =
                                data.ParameterList.Parameter.FirstOrDefault(
                                    item => item.Name == "resultFlag" && item.Value != "Y");
                            errorMessage = resulterror != null ? resulterror.Value : string.Empty;

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", errorMessage, "");
                        }
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", data != null ? data.ErrorMessage : string.Empty, "");
                    }
                }

                // check product no recieve promotion
                if (resultFlag == "Y")
                {

                    var resultlov = (from z in _lov.Get()
                                     where z.LOV_TYPE == "PACKAGE_NO_PROMOTION" && z.ACTIVEFLAG == "Y" && z.LOV_NAME == productCd
                                     select z);
                    moblieNumberSerenadeModel.IsStatus = resultlov.Any();
                    moblieNumberSerenadeModel.ResultFlag = resultFlag;
                }
                else
                {
                    moblieNumberSerenadeModel.ResultFlag = resultFlag;
                    moblieNumberSerenadeModel.ErrorMessage = errorMessage;
                }

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SFFServices.SffResponse(), log, "Failed", ex.Message, "");

                moblieNumberSerenadeModel.ResultFlag = resultFlag;
                moblieNumberSerenadeModel.ErrorMessage = ex.Message;
            }

            return moblieNumberSerenadeModel;
        }
    }
}
