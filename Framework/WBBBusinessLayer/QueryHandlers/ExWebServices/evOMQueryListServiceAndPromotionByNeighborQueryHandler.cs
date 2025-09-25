using AIRNETEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WBBBusinessLayer.SFFServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class evOMQueryListServiceAndPromotionByNeighborQueryHandler : IQueryHandler<evOMQueryListServiceAndPromotionByNeighborQuery, evOMQueryListServiceAndPromotionByNeighborModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow; // insert log
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        public evOMQueryListServiceAndPromotionByNeighborQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public evOMQueryListServiceAndPromotionByNeighborModel Handle(evOMQueryListServiceAndPromotionByNeighborQuery query)
        {
            InterfaceLogCommand log = null;
            List<AIR_SFF_SERVICE_CODE> servicecodelist = new List<AIR_SFF_SERVICE_CODE>();
            evOMQueryListServiceAndPromotionByNeighborModel modelreturn = new evOMQueryListServiceAndPromotionByNeighborModel();
            List<promotionModel> promotionlist = new List<promotionModel>();
            promotionModel promotionmodel = new promotionModel();

            //modelreturn.productCDContent = "";
            try
            {
                /* get promotion type and service type*/
                var inputparam = (from z in _FBB_CFG_LOV.Get()
                                  where z.DISPLAY_VAL == "evOMQueryListServiceAndPromotionByPackageType" && z.ACTIVEFLAG == "Y" && z.LOV_VAL5 == "FBBOR001"
                                  select z);
                /*Get content*/
                //var ContentList = (from z in _FBB_CFG_LOV.Get()
                //                   where z.LOV_NAME == "PRODUCT_CD" && z.LOV_TYPE == "FBB_CONSTANT"
                //                   select z.LOV_VAL1).ToList();
                var promotiontype = inputparam.Where(a => a.LOV_NAME == "promotionType").FirstOrDefault();
                var servicetype = inputparam.Where(a => a.LOV_NAME == "serviceType").FirstOrDefault();

                var request = new SFFServices.SffRequest();
                request.Event = "evOMQueryListServiceAndPromotionByPackageType";
                var paramArray = new SFFServices.Parameter[3];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();


                param0.Name = "mobileNo";
                param0.Value = query.mobileNo;
                param1.Name = promotiontype.LOV_NAME;
                param1.Value = promotiontype.LOV_VAL1;
                param2.Name = servicetype.LOV_NAME;
                param2.Value = servicetype.LOV_VAL1;


                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                _logger.Info("Call evESeService SFF");
                _logger.Info("mobileNo: " + query.mobileNo + ", promotionType: " + promotiontype.LOV_VAL1 + ", serviceType: " + servicetype.LOV_VAL1);

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.mobileNo, "evOMQueryListServiceAndPromotionByPackageType", "evOMQueryListServiceAndPromotionByPackageType", "", "FBB|" + query.FullUrl, "");


                using (var service = new SFFServiceService())
                {
                    var data = service.ExecuteService(request);

                    if (data != null)
                    {
                        _logger.Info(data.ErrorMessage);
                        if (data.ErrorMessage != "" || data.ErrorMessage != null)
                        {
                            modelreturn.ErrorMessage = data.ErrorMessage;
                            var errSp = data.ErrorMessage.Trim().Split(':');
                            modelreturn.ErrorMessage = errSp[0];
                        }

                        foreach (var a in data.ParameterList.Parameter)
                        {
                            if (a.Name == "resultFlag")
                                modelreturn.resultFlag = a.Value;
                        }
                        if (modelreturn.resultFlag == "Y")
                        {
                            //ถ้า retrun resultFlag=="Y"--> Fail
                            string productType = "";
                            Boolean IsproductCD = false;
                            string productclass = "";

                            foreach (var a in data.ParameterList.ParameterList1)
                            {
                                productType = "";
                                IsproductCD = false;

                                promotionmodel = new promotionModel();
                                foreach (var b in a.Parameter)
                                {

                                    if (b.Name == "productCD")
                                    {
                                        IsproductCD = servicecodelist.Exists(x => x.SERVICE_CODE == b.Value);
                                        promotionmodel.productCD = b.Value;


                                        modelreturn.productCDContent.Add(b.Value);

                                    }
                                }
                                promotionlist.Add(promotionmodel);
                            }


                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                        }

                        else// service return flag N
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", modelreturn.ErrorMessage.ToSafeString(), "");
                        }
                    }
                }

                return modelreturn;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, modelreturn, log, "Failed", ex.ToSafeString(), "");
                return modelreturn;
            }

        }
        private string Breakword(string input)
        {
            var templist = (from z in _FBB_CFG_LOV.Get()
                            where z.LOV_NAME == "REPLACE_INSTALL_ADDRESS"
                            select z.LOV_VAL1).FirstOrDefault();

            string regex = @templist;
            string output = Regex.Replace(input, regex, "");

            return output;
        }
    }
}
