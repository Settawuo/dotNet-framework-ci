using System;
using System.Linq;
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
    public class evAMQueryCustomerProfileByTaxIdHandler : IQueryHandler<evAMQueryCustomerProfileByTaxIdQuery, evAMQueryCustomerProfileByTaxIdModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow; // insert log
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        public evAMQueryCustomerProfileByTaxIdHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }
        public evAMQueryCustomerProfileByTaxIdModel Handle(evAMQueryCustomerProfileByTaxIdQuery query)
        {
            InterfaceLogCommand log = null;
            evAMQueryCustomerProfileByTaxIdModel modelreturn = new evAMQueryCustomerProfileByTaxIdModel();
            try
            {
                string option = "1";
                string cardType = "";

                option = (from z in _FBB_CFG_LOV.Get()
                          where z.LOV_NAME == "evAMQueryCustomerProfileByTaxId" && z.DISPLAY_VAL == "option" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                          select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                cardType = (from z in _FBB_CFG_LOV.Get()
                            where z.LOV_NAME == "evAMQueryCustomerProfileByTaxId" && z.DISPLAY_VAL == "cardType" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                var request = new SFFServices.SffRequest();
                request.Event = "evAMQueryCustomerProfileByTaxId";
                var paramArray = new SFFServices.Parameter[3];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();

                param0.Name = "option";
                param0.Value = option.ToSafeString();
                param1.Name = "taxId";
                param1.Value = query.TAX_ID;
                param2.Name = "cardType";
                param2.Value = cardType.ToSafeString();

                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;

                var paramList = new SFFServices.ParameterList();
                paramList.Parameter = paramArray;

                request.ParameterList = paramList;

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.TAX_ID, "evAMQueryCustomerProfileByTaxId", "evAMQueryCustomerProfileByTaxIdHandler", null, "SFF", "");

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
                                        if (item.Name == "idCardNo")
                                            modelreturn.idCardNo = item.Value;
                                        else if (item.Name == "AccountTitle")
                                            modelreturn.AccountTitle = item.Value;
                                        else if (item.Name == "customerName")
                                            modelreturn.customerName = item.Value;
                                        else if (item.Name == "customerStatus")
                                            modelreturn.customerStatus = item.Value;
                                        else if (item.Name == "activeDt")
                                            modelreturn.activeDt = item.Value;
                                        else if (item.Name == "birthDay")
                                            modelreturn.birthDay = item.Value;
                                        else if (item.Name == "houseTelNo")
                                            modelreturn.houseTelNo = item.Value;
                                        else if (item.Name == "officeTelNo")
                                            modelreturn.officeTelNo = item.Value;
                                        else if (item.Name == "caAccountNo")
                                            modelreturn.caAccountNo = item.Value;
                                        else if (item.Name == "accountCat")
                                            modelreturn.accountCat = item.Value;
                                        else if (item.Name == "accountSubCat")
                                            modelreturn.accountSubCat = item.Value;
                                        else if (item.Name == "accountClass")
                                            modelreturn.accountClass = item.Value;
                                        else if (item.Name == "accountStatus")
                                            modelreturn.accountStatus = item.Value;
                                        else if (item.Name == "AccountHouseNo")
                                            modelreturn.AccountHouseNo = item.Value;
                                        else if (item.Name == "AccountSoi")
                                            modelreturn.AccountSoi = item.Value;
                                        else if (item.Name == "AccountMoo")
                                            modelreturn.AccountMoo = item.Value;
                                        else if (item.Name == "AccountMooban")
                                            modelreturn.AccountMooban = item.Value;
                                        else if (item.Name == "AccountBuildingName")
                                            modelreturn.AccountBuildingName = item.Value;
                                        else if (item.Name == "AccountFloor")
                                            modelreturn.AccountFloor = item.Value;
                                        else if (item.Name == "AccountRoom")
                                            modelreturn.AccountRoom = item.Value;
                                        else if (item.Name == "AccountStreetName")
                                            modelreturn.AccountStreetName = item.Value;
                                        else if (item.Name == "AccountTumbol")
                                            modelreturn.AccountTumbol = item.Value;
                                        else if (item.Name == "AccountAmphur")
                                            modelreturn.AccountAmphur = item.Value;
                                        else if (item.Name == "AccountProvince")
                                            modelreturn.AccountProvince = item.Value;
                                        else if (item.Name == "AccountZipCode")
                                            modelreturn.AccountZipCode = item.Value;
                                    }
                                }
                            }
                        }
                        if (modelreturn.errorCode == "")
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
