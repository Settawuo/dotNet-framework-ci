using System;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.SFFServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class EvOmPlayboxExtensionInfoQueryHandler : IQueryHandler<EvOmPlayboxExtensionInfoQuery, EvOmPlayboxExtensionInfoModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLovRepository;

        public EvOmPlayboxExtensionInfoQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<FBB_CFG_LOV> cfgLovRepository)
        {
            _uow = uow;
            _intfLog = intfLog;
            _cfgLovRepository = cfgLovRepository;
        }

        public EvOmPlayboxExtensionInfoModel Handle(EvOmPlayboxExtensionInfoQuery query)
        {
            var evOmPlayboxExtensionInfoModel = new EvOmPlayboxExtensionInfoModel();
            InterfaceLogCommand log = null;
            try
            {
                var objReqParam = new[]
                {
                    new Parameter {Name = "FBBID", Value = query.FbbId.ToSafeString()}
                };

                var request = new SffRequest
                {
                    Event = "evOMPlayboxExtensionInfo",
                    ParameterList = new ParameterList
                    {
                        Parameter = objReqParam.ToArray(),
                    }
                };

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.TransactionId, "evOMPlayboxExtensionInfo", "SFFService", null, "FBB", "");

                using (var service = new SFFServiceService())
                {
                    var data = service.ExecuteService(request);


                    if (data != null)
                    {
                        if (data.ErrorMessage == null)
                        {
                            var errorMessage = data.ParameterList.Parameter.FirstOrDefault(item => item.Name == "errorMessage");
                            if (errorMessage != null)
                            {
                                evOmPlayboxExtensionInfoModel.ErrorMessage = errorMessage.Value.ToSafeString();
                            }

                            var countPlayBox = data.ParameterList.Parameter.FirstOrDefault(item => item.Name == "countPlayBox");
                            if (countPlayBox != null)
                            {
                                evOmPlayboxExtensionInfoModel.CountPlaybox = countPlayBox.Value.ToSafeString();
                            }

                            //R22.04 Issue Playbox
                            var mainPlayboxCode = data.ParameterList.Parameter.FirstOrDefault(item => item.Name == "mainPlayboxCode");
                            if (mainPlayboxCode != null)
                            {
                                evOmPlayboxExtensionInfoModel.MainPlayboxCode = mainPlayboxCode.Value.ToSafeString();
                            }

                            var mainPlayboxName = data.ParameterList.Parameter.FirstOrDefault(item => item.Name == "mainPlayboxName");
                            if (mainPlayboxName != null)
                            {
                                evOmPlayboxExtensionInfoModel.MainPlayboxName = mainPlayboxName.Value.ToSafeString();
                            }

                            var mainPlayboxExists = data.ParameterList.Parameter.FirstOrDefault(item => item.Name == "mainPlayboxExists");
                            if (mainPlayboxExists != null)
                            {
                                evOmPlayboxExtensionInfoModel.MainPlayboxExists = mainPlayboxExists.Value.ToSafeString();
                            }
                            //-----------

                            var usePlayboxList = data.ParameterList.ParameterList1.FirstOrDefault(item => item != null && item.ParameterType == "usePlayboxList");
                            if (usePlayboxList != null)
                            {

                                evOmPlayboxExtensionInfoModel.UsePlayboxList = new List<UsePlaybox>();
                                foreach (var parameterList in usePlayboxList.ParameterList1)
                                {
                                    var usePlaybox = new UsePlaybox();
                                    var serviceCode = parameterList.Parameter.FirstOrDefault(item => item.Name == "serviceCode");
                                    if (serviceCode != null)
                                    {
                                        usePlaybox.ServiceCode = serviceCode.Value.ToSafeString();
                                    }
                                    var serviceName = parameterList.Parameter.FirstOrDefault(item => item.Name == "serviceName");
                                    if (serviceName != null)
                                    {
                                        usePlaybox.ServiceName = serviceName.Value.ToSafeString();
                                    }
                                    var serial = parameterList.Parameter.FirstOrDefault(item => item.Name == "serial");
                                    if (serial != null)
                                    {
                                        usePlaybox.Serial = serial.Value.ToSafeString();
                                    }
                                    evOmPlayboxExtensionInfoModel.UsePlayboxList.Add(usePlaybox);
                                }
                            }

                            var availPlayboxtype = data.ParameterList.ParameterList1.FirstOrDefault(item => item != null && item.ParameterType == "availPlayboxList");
                            if (availPlayboxtype != null)
                            {
                                var playboxList = new List<AvailPlaybox>();
                                foreach (var parameterList in availPlayboxtype.ParameterList1)
                                {
                                    var availPlaybox = new AvailPlaybox();
                                    var productCode = parameterList.Parameter.FirstOrDefault(item => item.Name == "productCode");
                                    if (productCode != null)
                                    {
                                        availPlaybox.ProductCode = productCode.Value.ToSafeString();
                                    }
                                    var productName = parameterList.Parameter.FirstOrDefault(item => item.Name == "productName");
                                    if (productName != null)
                                    {
                                        availPlaybox.ProductName = productName.Value.ToSafeString();
                                    }

                                    //Mapping Playbox Ext Service 
                                    var result = (from lov in _cfgLovRepository.Get()
                                                 where lov.LOV_TYPE == "MULTIPLE_PLAYBOX"
                                                 && lov.LOV_NAME == "SERVICE_CODE"
                                                 && lov.DISPLAY_VAL == availPlaybox.ProductCode
                                                 select lov).ToList();

                                    if (result.FirstOrDefault() == null)
                                    {
                                        continue;
                                    }

                                    var lovExt = result.FirstOrDefault() ?? new FBB_CFG_LOV();
                                    availPlaybox.PlayBoxExt = Convert.ToInt16(lovExt.ORDER_BY);
                                    playboxList.Add(availPlaybox);
                                }

                                //return Max install playbox
                                var resultMax = (from lov in _cfgLovRepository.Get()
                                                where lov.LOV_NAME == "L_NUMBER_OF_PLAYBOX"
                                                select lov).ToList();
                                var lovMax = resultMax.FirstOrDefault() ?? new FBB_CFG_LOV();

                                var pbMax = Convert.ToInt16(lovMax.LOV_VAL1);
                                int[] i = { 1 };
                                evOmPlayboxExtensionInfoModel.AvailPlayboxList = new List<AvailPlaybox>();
                                foreach (var availPlaybox in playboxList.OrderBy(item => item.PlayBoxExt).Where(availPlaybox => i[0] <= pbMax))
                                {
                                    evOmPlayboxExtensionInfoModel.AvailPlayboxList.Add(availPlaybox);
                                    i[0] += 1;
                                }
                            }

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                        }
                        else
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Error", data.ErrorMessage, "");
                        }
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SffResponse(), log, "Error", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SffResponse(), log, "Error", ex.Message, "");
            }

            return evOmPlayboxExtensionInfoModel;
        }
    }
}
