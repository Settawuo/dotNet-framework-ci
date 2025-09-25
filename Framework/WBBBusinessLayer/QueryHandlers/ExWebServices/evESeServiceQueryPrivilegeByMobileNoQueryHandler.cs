using System;
using System.Collections.Generic;
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
    public class evESeServiceQueryPrivilegeByMobileNoQueryHandler : IQueryHandler<evESeServiceQueryPrivilegeByMobileNoQuery, evESeServiceQueryPrivilegeByMobileNoModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow; // insert log
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        public evESeServiceQueryPrivilegeByMobileNoQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }
        public evESeServiceQueryPrivilegeByMobileNoModel Handle(evESeServiceQueryPrivilegeByMobileNoQuery query)
        {
            InterfaceLogCommand log = null;
            evESeServiceQueryPrivilegeByMobileNoModel modelreturn = new evESeServiceQueryPrivilegeByMobileNoModel();
            List<PrivilegePromotionModel> assetPromotionItemList = new List<PrivilegePromotionModel>();
            List<PrivilegeServiceModel> assetServiceItemList = new List<PrivilegeServiceModel>();
            try
            {
                var request = new SFFServices.SffRequest();
                request.Event = "evESeServiceQueryPrivilegeByMobileNo";
                var paramArray = new SFFServices.Parameter[1];
                var param0 = new SFFServices.Parameter();


                param0.Name = "mobileNo";
                param0.Value = query.mobileNo;



                paramArray[0] = param0;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                _logger.Info("Call evESeService SFF");
                _logger.Info("mobileNo: " + query.mobileNo + ",");

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.mobileNo, " evESeServiceQueryPrivilegeByMobileNo", " evESeServiceQueryPrivilegeByMobileNo", "", "SFF", "");


                using (var service = new SFFServiceService())
                {
                    var data = service.ExecuteService(request);

                    if (data != null)
                    {
                        //_logger.Info(data.ErrorMessage);
                        //if (data.ErrorMessage != "" || data.ErrorMessage != null)
                        //{
                        //    modelreturn.errorMessage = data.ErrorMessage;
                        //    var errSp = data.ErrorMessage.Trim().Split(':');
                        //    modelreturn.errorMessage = errSp[0];
                        //}

                        foreach (var a in data.ParameterList.Parameter)
                        {
                            if (a.Name == "result")
                                modelreturn.result = a.Value;
                            else if (a.Name == "errorMessage")
                                modelreturn.errorMessage = a.Value;
                            //else if (a.Name == "assetServiceItemList")
                            //{
                            //    foreach (var item in data.ParameterList.ParameterList1)
                            //    {
                            //        PrivilegeServiceModel s = new PrivilegeServiceModel();
                            //        if (a.Name == "accountId")
                            //        s.accountId = item.Parameter[0].Value;
                            //        else if (a.Name == "accountNumber")
                            //        s.accountNumber = item.Parameter[0].Value;
                            //        else if (a.Name == "accountName")
                            //        s.accountName = item.Parameter[0].Value;
                            //        else if (a.Name == "billingAccountId")
                            //        s.billingAccountId = item.Parameter[0].Value;
                            //        else if (a.Name == "billingAccountNumber")
                            //        s.billingAccountNumber = item.Parameter[0].Value;
                            //        else if (a.Name == "billingAccountName")
                            //        s.billingAccountName = item.Parameter[0].Value;
                            //        else if (a.Name == "mobileNo")
                            //        s.mobileNo = item.Parameter[0].Value;
                            //        else if (a.Name == "integrationId")
                            //        s.integrationId = item.Parameter[0].Value;
                            //        else if (a.Name == "productId")
                            //        s.productId = item.Parameter[0].Value;
                            //        else if (a.Name == "productCode")
                            //        s.productCode = item.Parameter[0].Value;
                            //        else if (a.Name == "productName")
                            //        s.productName = item.Parameter[0].Value;
                            //        else if (a.Name == "integrationName")
                            //        s.integrationName = item.Parameter[0].Value;
                            //        else if (a.Name == "serviceIntegrationId")
                            //        s.serviceIntegrationId = item.Parameter[0].Value;
                            //        else if (a.Name == "serviceStartDt")
                            //        s.serviceStartDt = item.Parameter[0].Value;
                            //        else if (a.Name == "serviceEndDt")
                            //        s.serviceEndDt = item.Parameter[0].Value;
                            //        else if (a.Name == "serviceStatusCd")
                            //        s.serviceStatusCd = item.Parameter[0].Value;
                            //        else if (a.Name == "effectiveDt")
                            //        s.effectiveDt = item.Parameter[0].Value;
                            //        else if (a.Name == "expiredt")
                            //        s.expiredt = item.Parameter[0].Value;
                            //        else if (a.Name == "endDt")
                            //        s.endDt = item.Parameter[0].Value;
                            //        else if (a.Name == "remark")
                            //        s.remark = item.Parameter[0].Value;
                            //        else if (a.Name == "reasonCode")
                            //        s.reasonCode = item.Parameter[0].Value;
                            //        else if (a.Name == "orderNo")
                            //        s.orderNo = item.Parameter[0].Value;
                            //        else if (a.Name == "projectCd")
                            //        s.projectCd = item.Parameter[0].Value;

                            //        assetServiceItemList.Add(s);

                            //    }
                            //}
                            //modelreturn.assetServiceItemList = assetServiceItemList;
                        }
                        if (data.ParameterList.ParameterList1 != null)
                        {
                            if (data.ParameterList.ParameterList1[0].ParameterList1[0].Parameter != null)
                            {
                                foreach (var list in data.ParameterList.ParameterList1[0].ParameterList1)
                                {
                                    PrivilegePromotionModel p = new PrivilegePromotionModel();
                                    foreach (var item in list.Parameter)
                                    {
                                        if (item.Name == "accountId")
                                            p.accountId = item.Value;
                                        else if (item.Name == "accountNumber")
                                            p.accountNumber = item.Value;
                                        else if (item.Name == "accountName")
                                            p.accountName = item.Value;
                                        else if (item.Name == "billingAccountId")
                                            p.billingAccountId = item.Value;
                                        else if (item.Name == "billingAccountNumber")
                                            p.billingAccountNumber = item.Value;
                                        else if (item.Name == "billingAccountName")
                                            p.billingAccountName = item.Value;
                                        else if (item.Name == "mobileNo")
                                            p.mobileNo = item.Value;
                                        else if (item.Name == "integrationId")
                                            p.integrationId = item.Value;
                                        else if (item.Name == "productId")
                                            p.productId = item.Value;
                                        else if (item.Name == "productCode")
                                            p.productCode = item.Value;
                                        else if (item.Name == "productName")
                                            p.productName = item.Value;
                                        else if (item.Name == "integrationName")
                                            p.integrationName = item.Value;
                                        else if (item.Name == "promotionIntegrationId")
                                            p.promotionIntegrationId = item.Value;
                                        else if (item.Name == "promotionStartDt")
                                            p.promotionStartDt = item.Value;
                                        else if (item.Name == "promotionEndDt")
                                            p.promotionEndDt = item.Value;
                                        else if (item.Name == "promotionStatusCd")
                                            p.promotionStatusCd = item.Value;
                                        else if (item.Name == "effectiveDt")
                                            p.effectiveDt = item.Value;
                                        else if (item.Name == "Expired")
                                            p.Expired = item.Value;
                                        else if (item.Name == "endDt")
                                            p.endDt = item.Value;
                                        else if (item.Name == "remark")
                                            p.remark = item.Value;
                                        else if (item.Name == "reasonCode")
                                            p.reasonCode = item.Value;
                                        else if (item.Name == "orderNo")
                                            p.orderNo = item.Value;
                                        else if (item.Name == "projectCd")
                                            p.projectCd = item.Value;
                                    }
                                    assetPromotionItemList.Add(p);
                                }

                                modelreturn.assetPromotionItemList = assetPromotionItemList;
                            }
                        }
                        if (modelreturn.result == "Success")
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                        }

                        else// service return flag N
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", modelreturn.errorMessage.ToSafeString(), "");
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
    }
}
