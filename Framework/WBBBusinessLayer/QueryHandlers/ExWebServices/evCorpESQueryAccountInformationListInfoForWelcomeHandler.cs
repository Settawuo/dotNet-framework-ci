using AIRNETEntity.Extensions;
using System;
using WBBBusinessLayer.SFFServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class evCorpESQueryAccountInformationListInfoForWelcomeHandler : IQueryHandler<evCorpESQueryAccountInformationListInfoForWelcomeQuery, evCorpESQueryAccountInformationListInfoForWelcomeModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow; // insert log
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        public evCorpESQueryAccountInformationListInfoForWelcomeHandler(
            ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }


        public evCorpESQueryAccountInformationListInfoForWelcomeModel Handle(evCorpESQueryAccountInformationListInfoForWelcomeQuery query)
        {
            InterfaceLogCommand log = null;
            evCorpESQueryAccountInformationListInfoForWelcomeModel modelreturn = new evCorpESQueryAccountInformationListInfoForWelcomeModel();
            try
            {

                var request = new SFFServices.SffRequest();
                request.Event = "evCorpESQueryAccountInformationListInfo";
                var paramArray = new SFFServices.Parameter[1];
                var param0 = new SFFServices.Parameter();

                param0.Name = "inAccountNo";
                param0.Value = query.inAccountNo;


                paramArray[0] = param0;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.inAccountNo, "evCorpESQueryAccountInformationListInfo", "evCorpESQueryAccountInformationListInfoForWelcomeHandler", null, "SFF", "");

                using (var service = new SFFServiceService())
                {
                    var data = service.ExecuteService(request);

                    if (data != null)
                    {

                        if (data.ErrorMessage == null)
                        {
                            modelreturn.errorCode = data.ErrorMessage.ToSafeString();
                            modelreturn.errorMessage = data.ErrorMessage.ToSafeString();
                        }
                        if (data.ParameterList.ParameterList1 != null)
                        {
                            if (data.ParameterList.ParameterList1[0].Parameter != null)
                            {
                                foreach (var listData in data.ParameterList.ParameterList1)
                                {

                                    foreach (var item in listData.Parameter)
                                    {
                                        if (item.Name == "accountId")
                                            modelreturn.accountId = item.Value;
                                        else if (item.Name == "accountTitle")
                                            modelreturn.accountTitle = item.Value;
                                        else if (item.Name == "accountName")
                                            modelreturn.accountName = item.Value;
                                        else if (item.Name == "accountNumber")
                                            modelreturn.accountNumber = item.Value;
                                        else if (item.Name == "accountClass")
                                            modelreturn.accountClass = item.Value;
                                        else if (item.Name == "accountStatus")
                                            modelreturn.accountStatus = item.Value;
                                        else if (item.Name == "accountBillCycle")
                                            modelreturn.accountBillCycle = item.Value;
                                        else if (item.Name == "accountHouseNo")
                                            modelreturn.accountHouseNo = item.Value;
                                        else if (item.Name == "accountSoi")
                                            modelreturn.accountSoi = item.Value;
                                        else if (item.Name == "accountMoo")
                                            modelreturn.accountMoo = item.Value;
                                        else if (item.Name == "accountMooban")
                                            modelreturn.accountMooban = item.Value;
                                        else if (item.Name == "accountBuildingName")
                                            modelreturn.accountBuildingName = item.Value;
                                        else if (item.Name == "accountFloor")
                                            modelreturn.accountFloor = item.Value;
                                        else if (item.Name == "accountRoom")
                                            modelreturn.accountRoom = item.Value;
                                        else if (item.Name == "accountStreetName")
                                            modelreturn.accountStreetName = item.Value;
                                        else if (item.Name == "accountTambol")
                                            modelreturn.accountTambol = item.Value;
                                        else if (item.Name == "accountAmphur")
                                            modelreturn.accountAmphur = item.Value;
                                        else if (item.Name == "accountProvince")
                                            modelreturn.accountProvince = item.Value;
                                        else if (item.Name == "accountZipCode")
                                            modelreturn.accountZipCode = item.Value;
                                        else if (item.Name == "fullAddress")
                                            modelreturn.fullAddress = item.Value;
                                        else if (item.Name == "accountEngFlg")
                                            modelreturn.accountEngFlg = item.Value;
                                        else if (item.Name == "accountCategory")
                                            modelreturn.accountCategory = item.Value;
                                        else if (item.Name == "accountSubCategory")
                                            modelreturn.accountSubCategory = item.Value;
                                        else if (item.Name == "accountMailGroupFlg")
                                            modelreturn.accountMailGroupFlg = item.Value;
                                        else if (item.Name == "accountMailGroupAccntId")
                                            modelreturn.accountMailGroupAccntId = item.Value;
                                        else if (item.Name == "itemize")
                                            modelreturn.itemize = item.Value;
                                        else if (item.Name == "valueSegment")
                                            modelreturn.valueSegment = item.Value;
                                        else if (item.Name == "billLanguage")
                                            modelreturn.billLanguage = item.Value;
                                        else if (item.Name == "billName")
                                            modelreturn.billName = item.Value;
                                        else if (item.Name == "billStyle")
                                            modelreturn.billStyle = item.Value;
                                        else if (item.Name == "invoicingCompany")
                                            modelreturn.invoicingCompany = item.Value;
                                        else if (item.Name == "taxPaperFlag")
                                            modelreturn.taxPaperFlag = item.Value;

                                    }
                                }
                            }
                        }
                        if (modelreturn.errorCode == "" || modelreturn.errorCode == null)
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                        }

                        else// service return flag N
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", modelreturn.errorMessage.ToSafeString(), "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, modelreturn, log, "Failed", ex.ToSafeString(), "");
            }

            return modelreturn;

        }



    }
}
