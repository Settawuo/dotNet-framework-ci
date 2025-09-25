using System;
using System.Linq;
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
    public class GetConfirmChangePromotionQueryHandler : IQueryHandler<GetConfirmChangePromotionQuery, ConfirmChangePromotionModelLine4>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetConfirmChangePromotionQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,

            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
        }

        public ConfirmChangePromotionModelLine4 Handle(GetConfirmChangePromotionQuery query)
        {
            InterfaceLogCommand log = null;
            var ConfirmChangePromotionModel = new ConfirmChangePromotionModelLine4();

            try
            {
                #region evOMServiceConfirmChangePromotion
                if (query.FlagCallService_evOMServiceConfirmChangePromotion == "Y")
                {
                    var request = new SFFServices.SffRequest();
                    request.Event = "evOMServiceConfirmChangePromotion";

                    var paramArray = new SFFServices.Parameter[9];
                    var param0 = new SFFServices.Parameter();
                    var param1 = new SFFServices.Parameter();
                    var param2 = new SFFServices.Parameter();
                    var param3 = new SFFServices.Parameter();

                    var param4 = new SFFServices.Parameter();
                    var param5 = new SFFServices.Parameter();

                    //R18.4
                    var paramAscCode = new SFFServices.Parameter();
                    var paramLocationCd = new SFFServices.Parameter();
                    var paramEmployeeID = new SFFServices.Parameter();

                    param0.Name = "orderType";
                    param0.Value = "Change Promotion";
                    param1.Name = "mobileNo";
                    param1.Value = query.mobileNo;
                    param2.Name = "orderReson";
                    param2.Value = "956";
                    param3.Name = "orderChannel";
                    param3.Value = "WEB";

                    param4.Name = "userName";
                    param4.Value = "SBN_AWN";
                    param5.Name = "chargeFeeFlag";
                    param5.Value = "N";

                    //R18.4
                    paramAscCode.Name = "ascCode";
                    paramAscCode.Value = query.ascCode.ToSafeString();
                    paramLocationCd.Name = "locationCd";
                    paramLocationCd.Value = query.locationCd.ToSafeString();
                    paramEmployeeID.Name = "employeeID";
                    paramEmployeeID.Value = query.employeeID.ToSafeString();
                    ///////////////////////////////////////////////////////////

                    string[] sumcode = query.promotionCode.ToSafeString().Split(',');
                    var param6 = new SFFServices.ParameterList[sumcode.Count()];

                    for (var i = 0; i < sumcode.Count(); i++)
                    {
                        var orderStatus = "Add";
                        if (sumcode[i] == query.promotionCdOldContent)
                        {
                            orderStatus = "Delete";
                        }

                        var templist = new SFFServices.ParameterList();
                        var paramArray2 = new SFFServices.Parameter[11];
                        var param6_1 = new SFFServices.Parameter();
                        var param6_2 = new SFFServices.Parameter();
                        var param6_3 = new SFFServices.Parameter();
                        var param6_4 = new SFFServices.Parameter();
                        var param6_5 = new SFFServices.Parameter();
                        var param6_6 = new SFFServices.Parameter();
                        var param6_7 = new SFFServices.Parameter();
                        var param6_8 = new SFFServices.Parameter();
                        var param6_9 = new SFFServices.Parameter();
                        var param6_10 = new SFFServices.Parameter();
                        var param6_11 = new SFFServices.Parameter();

                        //////////////////////////////////////////////
                        param6_1.Name = "promotionCode";
                        param6_1.Value = sumcode[i];
                        param6_2.Name = "actionStatus";
                        param6_2.Value = orderStatus;
                        param6_3.Name = "productSeq";
                        param6_3.Value = "";
                        param6_4.Name = "promotionStartDt";
                        param6_4.Value = "";
                        param6_5.Name = "overRuleStartDate";
                        param6_5.Value = "I";
                        param6_6.Name = "attributeSeq";
                        param6_6.Value = "";
                        param6_7.Name = "attributeValue";
                        param6_7.Value = "";
                        param6_8.Name = "attributeAction";
                        param6_8.Value = "";
                        param6_9.Name = "validateAttrFlag";
                        param6_9.Value = "";
                        param6_10.Name = "waiveFlag";
                        param6_10.Value = "";
                        param6_11.Name = "vatAmt";
                        param6_11.Value = "";
                        /////////////////////////////////////////////
                        paramArray[0] = param0;
                        paramArray[1] = param1;
                        paramArray[2] = param2;
                        paramArray[3] = param3;
                        paramArray[4] = param4;
                        paramArray[5] = param5;
                        //R18.4
                        paramArray[6] = paramAscCode;
                        paramArray[7] = paramLocationCd;
                        paramArray[8] = paramEmployeeID;

                        paramArray2[0] = param6_1;
                        paramArray2[1] = param6_2;
                        paramArray2[2] = param6_3;
                        paramArray2[3] = param6_4;
                        paramArray2[4] = param6_5;
                        paramArray2[5] = param6_6;
                        paramArray2[6] = param6_7;
                        paramArray2[7] = param6_8;
                        paramArray2[8] = param6_9;
                        paramArray2[9] = param6_10;
                        paramArray2[10] = param6_11;
                        templist.Parameter = paramArray2;
                        param6[i] = templist;
                    }


                    var paramList = new SFFServices.ParameterList();
                    paramList.Parameter = paramArray;
                    paramList.ParameterList1 = param6;
                    request.ParameterList = paramList;


                    //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog,
                    //                                                   request, "",
                    //                                                   "GetConfirmChangePromotionQuery",
                    //                                                   "evOMServiceConfirmChangePromotion",
                    //                                                   query.mobileNo);
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, "", "GetConfirmChangePromotionQuery", "evOMServiceConfirmChangePromotion", query.mobileNo, "FBB", "");

                    using (var service = new SFFServices.SFFServiceService())
                    {
                        var data = service.ExecuteService(request);
                        if (data != null)
                        {
                            foreach (var a in data.ParameterList.Parameter)
                            {
                                if (a.Name == "successFlag")
                                {
                                    ConfirmChangePromotionModel.SuccessFlag = a.Value;

                                }
                                //else if (a.Name == "existFlag") ConfirmChangePromotionModel.existFlag = a.Value;
                                //else if (a.Name == "productCd") ConfirmChangePromotionModel.productCd = a.Value;
                                //else if (a.Name == "firstActDate") ConfirmChangePromotionModel.firstActDate = a.Value;
                                //else if (a.Name == "startDate") ConfirmChangePromotionModel.startDate = a.Value;
                                //else if (a.Name == "endDate") ConfirmChangePromotionModel.endDate = a.Value;
                                //else if (a.Name == "countFN") ConfirmChangePromotionModel.countFN = a.Value;

                            }
                        }
                    }
                }
                #endregion

                #region evOMCreateOrderChangePromotion

                else if (query.FlagCallService_evOMCreateOrderChangePromotion == "Y") /// new service evOMCreateOrderChangePromotion
                {
                    var request = new SFFServices.SffRequest();
                    request.Event = "evOMCreateOrderChangePromotion";

                    var paramArray = new SFFServices.Parameter[10];
                    var param0 = new SFFServices.Parameter();
                    var param1 = new SFFServices.Parameter();
                    var param2 = new SFFServices.Parameter();
                    var param3 = new SFFServices.Parameter();

                    var param4 = new SFFServices.Parameter();
                    var param5 = new SFFServices.Parameter();

                    //R18.4
                    var paramAscCode = new SFFServices.Parameter();
                    var paramLocationCd = new SFFServices.Parameter();
                    var paramEmployeeID = new SFFServices.Parameter();
                    var paramEmployeeName = new SFFServices.Parameter();

                    param0.Name = "orderType";
                    param0.Value = "Change Promotion";
                    param1.Name = "mobileNo";
                    param1.Value = query.mobileNo;
                    param2.Name = "orderReson";
                    param2.Value = "956";
                    param3.Name = "orderChannel";
                    param3.Value = "WEB";

                    param4.Name = "userName";
                    param4.Value = "SBN_AWN";
                    param5.Name = "chargeFeeFlag";
                    param5.Value = "N";

                    //R18.4
                    paramAscCode.Name = "ascCode";
                    paramAscCode.Value = query.ascCode.ToSafeString();
                    paramLocationCd.Name = "locationCd";
                    paramLocationCd.Value = query.locationCd.ToSafeString();
                    paramEmployeeID.Name = "employeeID";
                    paramEmployeeID.Value = query.employeeID.ToSafeString();
                    paramEmployeeName.Name = "saleStaffName";
                    paramEmployeeName.Value = query.employeeName.ToSafeString();
                    ///////////////////////////////////////////////////////////

                    string[] sumcode = query.promotionCode.ToSafeString().Split(',');
                    var param6 = new SFFServices.ParameterList[sumcode.Count()];

                    for (var i = 0; i < sumcode.Count(); i++)
                    {
                        var orderStatus = "Add";
                        if (sumcode[i] == query.promotionCdOldContent)
                        {
                            orderStatus = "Delete";
                        }

                        var templist = new SFFServices.ParameterList();
                        var paramArray2 = new SFFServices.Parameter[11];
                        var param6_1 = new SFFServices.Parameter();
                        var param6_2 = new SFFServices.Parameter();
                        var param6_3 = new SFFServices.Parameter();
                        var param6_4 = new SFFServices.Parameter();
                        var param6_5 = new SFFServices.Parameter();
                        var param6_6 = new SFFServices.Parameter();
                        var param6_7 = new SFFServices.Parameter();
                        var param6_8 = new SFFServices.Parameter();
                        var param6_9 = new SFFServices.Parameter();
                        var param6_10 = new SFFServices.Parameter();
                        var param6_11 = new SFFServices.Parameter();

                        //////////////////////////////////////////////
                        param6_1.Name = "promotionCode";
                        param6_1.Value = sumcode[i];
                        param6_2.Name = "actionStatus";
                        param6_2.Value = orderStatus;
                        param6_3.Name = "productSeq";
                        param6_3.Value = "";
                        param6_4.Name = "promotionStartDt";
                        param6_4.Value = "";
                        param6_5.Name = "overRuleStartDate";
                        param6_5.Value = "I";
                        param6_6.Name = "attributeSeq";
                        param6_6.Value = "";
                        param6_7.Name = "attributeValue";
                        param6_7.Value = "";
                        param6_8.Name = "attributeAction";
                        param6_8.Value = "";
                        param6_9.Name = "validateAttrFlag";
                        param6_9.Value = "";
                        param6_10.Name = "waiveFlag";
                        param6_10.Value = "";
                        param6_11.Name = "vatAmt";
                        param6_11.Value = "";
                        /////////////////////////////////////////////
                        paramArray[0] = param0;
                        paramArray[1] = param1;
                        paramArray[2] = param2;
                        paramArray[3] = param3;
                        paramArray[4] = param4;
                        paramArray[5] = param5;
                        //R18.4
                        paramArray[6] = paramAscCode;
                        paramArray[7] = paramLocationCd;
                        //R18.11
                        paramArray[8] = paramEmployeeID;
                        paramArray[9] = paramEmployeeName;

                        paramArray2[0] = param6_1;
                        paramArray2[1] = param6_2;
                        paramArray2[2] = param6_3;
                        paramArray2[3] = param6_4;
                        paramArray2[4] = param6_5;
                        paramArray2[5] = param6_6;
                        paramArray2[6] = param6_7;
                        paramArray2[7] = param6_8;
                        paramArray2[8] = param6_9;
                        paramArray2[9] = param6_10;
                        paramArray2[10] = param6_11;
                        templist.Parameter = paramArray2;
                        param6[i] = templist;
                    }


                    var paramList = new SFFServices.ParameterList();
                    paramList.Parameter = paramArray;
                    paramList.ParameterList1 = param6;
                    request.ParameterList = paramList;


                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.mobileNo, "GetConfirmChangePromotionQuery", "evOMCreateOrderChangePromotion", "", "FBB", "");

                    ConfirmChangePromotionModel.ReturnMessage = "";
                    using (var service = new SFFServices.SFFServiceService())
                    {
                        var data = service.ExecuteService(request);
                        if (data != null)
                        {
                            foreach (var a in data.ParameterList.Parameter)
                            {

                                if (a.Name == "VALIDATE_FLAG")
                                    ConfirmChangePromotionModel.SuccessFlag = a.Value;
                                else if (a.Name == "ORDER_NO") ConfirmChangePromotionModel.ReturnCode = a.Value;
                            }


                            if (data.ParameterList.ParameterList1 != null)
                            {
                                foreach (var b in data.ParameterList.ParameterList1)
                                {
                                    foreach (var d in b.Parameter)
                                    {
                                        if (d.Name == "ERROR_MASSAGE")
                                            ConfirmChangePromotionModel.ReturnMessage =
                                                ConfirmChangePromotionModel.ReturnMessage + " " + d.Value;
                                    }

                                }
                                ConfirmChangePromotionModel.ReturnMessage =
                                    ConfirmChangePromotionModel.ReturnMessage.Trim();
                            }

                        }
                    }
                }

                #endregion

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ConfirmChangePromotionModel, log, "Success", "Success", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    //SffServiceConseHelper
                    //    .EndInterfaceSffLog(_uow, _intfLog, ConfirmChangePromotionModel,
                    //                            log, "Failed", ex.GetErrorMessage());
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ConfirmChangePromotionModel, log, "Failed", ex.GetErrorMessage(), "");
                }

                return ConfirmChangePromotionModel;
            }

            return ConfirmChangePromotionModel;
        }

    }

    public class CheckNewRegisProspectQueryHandler : IQueryHandler<CheckNewRegisProspectQuery, CheckNewRegisProspectQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public CheckNewRegisProspectQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,

            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
        }

        public CheckNewRegisProspectQueryModel Handle(CheckNewRegisProspectQuery query)
        {
            InterfaceLogCommand log = null;
            CheckNewRegisProspectQueryModel checkNewRegisProspectQueryModel = new CheckNewRegisProspectQueryModel();

            try
            {

                var request = new SFFServices.SffRequest();
                request.Event = "evFBBServiceQueryCheckNewRegisProspect";

                var paramArray = new SFFServices.Parameter[3];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();

                param0.Name = "idCardNo";
                param0.Value = query.idCardNo.ToSafeString();
                param1.Name = "locationCd";
                param1.Value = query.locationCd.ToSafeString();
                param2.Name = "ascCd";
                param2.Value = query.ascCd.ToSafeString();

                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;
                request.ParameterList = paramList;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.TRANSACTION_ID, "CheckNewRegisProspectQuery", "evOMserviceQueryCheckNewRegisProspect", "", "FBB", "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);
                    if (data != null)
                    {
                        foreach (var a in data.ParameterList.Parameter)
                        {

                            if (a.Name == "firstName")
                                checkNewRegisProspectQueryModel.firstName = a.Value;

                            if (a.Name == "lastName")
                                checkNewRegisProspectQueryModel.lastName = a.Value;

                            if (a.Name == "blackListFlag")
                                checkNewRegisProspectQueryModel.blackListFlag = a.Value;

                            if (a.Name == "locationFlag")
                                checkNewRegisProspectQueryModel.locationFlag = a.Value;

                            if (a.Name == "ascFlag")
                                checkNewRegisProspectQueryModel.ascFlag = a.Value;

                            if (a.Name == "errorCode")
                                checkNewRegisProspectQueryModel.errorCode = a.Value;

                        }

                    }
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkNewRegisProspectQueryModel, log, "Success", "Success", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    checkNewRegisProspectQueryModel.errorCode = ex.Message;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, checkNewRegisProspectQueryModel, log, "Failed", ex.GetErrorMessage(), "");
                }

                return checkNewRegisProspectQueryModel;
            }

            return checkNewRegisProspectQueryModel;
        }

    }
}
