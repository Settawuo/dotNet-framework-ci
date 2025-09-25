using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class evAMQueryCustomerInfoQueryHandler : IQueryHandler<evAMQueryCustomerInfoQuery, evAMQueryCustomerInfoModel>
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

        public evAMQueryCustomerInfoQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
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

        public evAMQueryCustomerInfoModel Handle(evAMQueryCustomerInfoQuery query)
        {
            InterfaceLogCommand log = null;
            var model = new evAMQueryCustomerInfoModel();

            try
            {
                var request = new SFFServices.SffRequest();
                //request.Event = "evAMQueryCustomerInfo";

                var paramArray = new SFFServices.Parameter[6];
                var param0 = new SFFServices.Parameter();
                var param1 = new SFFServices.Parameter();
                var param2 = new SFFServices.Parameter();
                var param3 = new SFFServices.Parameter();
                var param4 = new SFFServices.Parameter();
                var param5 = new SFFServices.Parameter();

                //param0.Name = "idCardNum";
                //param0.Value = query.idCardNum;
                //param1.Name = "name";
                //param1.Value = query.name;
                //param2.Name = "accntNo";
                //param2.Value = query.accntNo;
                //param3.Name = "contactBirthDt";
                //param3.Value = query.contactBirthDt;
                //param4.Name = "minRowNum";
                //param4.Value = query.minRowNum;
                //param5.Name = "maxRowNum";
                //param5.Value = query.maxRowNum;

                param0.Name = "idCardNum";
                param0.Value = query.idCardNum;
                param1.Name = "name";
                param1.Value = query.name;
                param2.Name = "accntNo";
                param2.Value = query.accntNo;
                param3.Name = "contactBirthDt";
                param3.Value = query.contactBirthDt;
                param4.Name = "minRowNum";
                param4.Value = query.minRowNum;
                param5.Name = "maxRowNum";
                param5.Value = query.maxRowNum;

                paramArray[0] = param0;
                paramArray[1] = param1;
                paramArray[2] = param2;
                paramArray[3] = param3;
                paramArray[4] = param4;
                paramArray[5] = param5;

                //var paramList = new SFFServices.ParameterList();
                //paramList.Parameter = paramArray;
                //request.ParameterList = paramList;

                request = new SFFServices.SffRequest()
                {
                    Event = "evAMQueryCustomerInfo",
                    ParameterList = new SFFServices.ParameterList()
                    {
                        Parameter = paramArray
                    }

                };


                _logger.Info("Call evAMService SFF");
                _logger.Info("accntNo: " + query.accntNo + ", name: " + query.name);

                //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog, request, query.inMobileNo,
                //    "evESeServiceQueryMassCommonAccountInfoQuery", "evESeServiceQueryMassCommonAccountInfo", query.inCardNo);
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.accntNo + query.ClientIP, "evAMQueryCustomerInfoQuery", "evAMQueryCustomerInfo", query.accntNo, "FBB", "");

                //SffServiceConseHelper.GetQueryCustomerInfo(_logger, _uow, request, model);

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);

                    if (data != null)
                    {
                        if (data.ErrorMessage == null)
                        {
                            var response = new SFFServices.SffResponse();
                            var parmStatus = data.ParameterList.ParameterList1[0].Parameter.FirstOrDefault(item => item.Name == "resultStatus");
                            model.total = data.ParameterList.ParameterList1[0].Parameter[2].Value;
                            if (parmStatus != null && parmStatus.Value.ToUpper() == "SUCCESS")
                            {
                                //model.total = data.ParameterList.ParameterList1[0];
                                foreach (var itemData in data.ParameterList.ParameterList1[1].Parameter)
                                {
                                    if (itemData.Name == "rowNum") model.rowNum = itemData.Value;
                                    else if (itemData.Name == "rowId") model.rowId = itemData.Value;
                                    else if (itemData.Name == "accntNo") model.accntNo = itemData.Value;
                                    else if (itemData.Name == "accntClass") model.accntClass = itemData.Value;
                                    else if (itemData.Name == "accntTitle") model.accntTitle = itemData.Value;
                                    else if (itemData.Name == "name") model.name = itemData.Value;
                                    else if (itemData.Name == "idCardNum") model.idCardNum = itemData.Value;
                                    else if (itemData.Name == "idCardType") model.idCardType = itemData.Value;
                                    else if (itemData.Name == "contactBirthDt") model.contactBirthDt = itemData.Value;
                                    else if (itemData.Name == "statusCd") model.statusCd = itemData.Value;
                                    else if (itemData.Name == "accntCategory") model.accntCategory = itemData.Value;
                                    else if (itemData.Name == "accntSubCategory") model.accntSubCategory = itemData.Value;
                                    else if (itemData.Name == "mainPhone") model.mainPhone = itemData.Value;
                                    else if (itemData.Name == "mainMobile") model.mainMobile = itemData.Value;
                                    else if (itemData.Name == "legalFlg") model.legalFlg = itemData.Value;
                                    else if (itemData.Name == "houseNo") model.houseNo = itemData.Value;
                                    else if (itemData.Name == "buildingName") model.buildingName = itemData.Value;
                                    else if (itemData.Name == "floor") model.floor = itemData.Value;
                                    else if (itemData.Name == "room") model.room = itemData.Value;
                                    else if (itemData.Name == "moo") model.moo = itemData.Value;
                                    else if (itemData.Name == "mooban") model.mooban = itemData.Value;
                                    else if (itemData.Name == "streetName") model.streetName = itemData.Value;
                                    else if (itemData.Name == "soi") model.soi = itemData.Value;
                                    else if (itemData.Name == "zipCode") model.zipCode = itemData.Value;
                                    else if (itemData.Name == "tumbol") model.tumbol = itemData.Value;
                                    else if (itemData.Name == "amphur") model.amphur = itemData.Value;
                                    else if (itemData.Name == "country") model.country = itemData.Value;
                                    else if (itemData.Name == "vatName") model.vatName = itemData.Value;
                                    else if (itemData.Name == "vatRate") model.vatRate = itemData.Value;
                                    else if (itemData.Name == "vatAddress1") model.vatAddress1 = itemData.Value;
                                    else if (itemData.Name == "vatAddress2") model.vatAddress2 = itemData.Value;
                                    else if (itemData.Name == "vatAddress3") model.vatAddress3 = itemData.Value;
                                    else if (itemData.Name == "vatAddress4") model.vatAddress4 = itemData.Value;
                                    else if (itemData.Name == "vatAddress5") model.vatAddress5 = itemData.Value;
                                    else if (itemData.Name == "vatPostalCd") model.vatPostalCd = itemData.Value;
                                    else if (itemData.Name == "errorMessage") model.errorMessage = itemData.Value;

                                    /*
                                    PropertyInfo prop = model.GetType().GetProperty(itemData.Name, BindingFlags.Public | BindingFlags.Instance);
                                    if (null != prop && prop.CanWrite)
                                    {
                                        prop.SetValue(model, itemData.Value, null);
                                    }*/
                                }
                            }
                        }
                    }
                }

                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                //    "Success", model.errorMessage);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Success", model.errorMessage, "");
            }
            catch (Exception ex)
            {
                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, model, log,
                //    "Failed", model.errorMessage);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, model, log, "Failed", ex.GetErrorMessage(), "");
            }

            return model;
        }
    }
}
