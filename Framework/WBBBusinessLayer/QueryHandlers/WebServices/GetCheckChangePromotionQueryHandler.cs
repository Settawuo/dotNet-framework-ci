using System;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetCheckChangePromotionQueryHandler : IQueryHandler<GetCheckChangePromotionQuery, CheckChangePromotionModelLine4>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetCheckChangePromotionQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,

            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
        }

        public CheckChangePromotionModelLine4 Handle(GetCheckChangePromotionQuery query)
        {
            InterfaceLogCommand log = null;
            var checkChangePromotionModel = new CheckChangePromotionModelLine4();

            try
            {
                var request = new SFFServices.SffRequest();
                request.Event = "evOMServiceCheckChangePromotion";

                var paramArray = new SFFServices.Parameter[4];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();
                var param3 = new SFFServices.Parameter();


                param0.Name = "mobileNo";
                param0.Value = query.mobileNo;
                param1.Name = "promotionType";
                param1.Value = "CHECK";
                param2.Name = "promotionCd";
                param2.Value = query.promotionCd;
                param3.Name = "orderChannel";
                param3.Value = query.orderChannel;

                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;
                paramArray[3] = param3;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;
                request.ParameterList = paramList;


                //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog,
                //                                                   request, "",
                //                                                   "GetCheckChangePromotionQuery",
                //                                                   "evOMServiceCheckChangeService",
                //                                                   query.mobileNo);
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.mobileNo, "GetCheckChangePromotionQuery", "evOMServiceCheckChangePromotion", "", "FBB", "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);
                    if (data != null)
                    {
                        foreach (var a in data.ParameterList.Parameter)
                        {
                            if (a.Name == "returnCode")
                            {
                                checkChangePromotionModel.returnCode = a.Value;
                                if (a.Value != "001")
                                {
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if (a.Name == "existFlag") checkChangePromotionModel.existFlag = a.Value;
                            else if (a.Name == "productCd") checkChangePromotionModel.productCd = a.Value;
                            else if (a.Name == "firstActDate") checkChangePromotionModel.firstActDate = a.Value;
                            else if (a.Name == "startDate") checkChangePromotionModel.startDate = a.Value;
                            else if (a.Name == "endDate") checkChangePromotionModel.endDate = a.Value;
                            else if (a.Name == "countFN") checkChangePromotionModel.countFN = a.Value;

                        }
                    }
                    if (checkChangePromotionModel.returnCode == null)
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", "No Data.", "");
                    }
                }

                //SffServiceConseHelper
                //  .EndInterfaceSffLog(_uow, _intfLog, checkChangePromotionModel, log,
                //                          "Success", checkChangePromotionModel.returnCode);
                if (checkChangePromotionModel.returnCode != null)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkChangePromotionModel, log, "Success", checkChangePromotionModel.returnCode, "");
                }
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
}
