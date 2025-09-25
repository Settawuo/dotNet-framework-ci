using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class CheckAROrderQueryHandler : IQueryHandler<CheckAROrderQuery, List<CheckAROrderModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public CheckAROrderQueryHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lov, IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _lov = lov;
            _intfLog = intfLog;
        }

        public List<CheckAROrderModel> Handle(CheckAROrderQuery query)
        {
            InterfaceLogCommand log = null;
            var AROrderModel = new CheckAROrderModel();
            List<CheckAROrderModel> result = new List<CheckAROrderModel>();
            try
            {
                var param0 = new SFFServices.Parameter { Name = "broadbandId", Value = query.BroadbandId };
                var param1 = new SFFServices.Parameter { Name = "option", Value = query.Option };

                var paramArray = new[] { param0, param1 };
                var paramList = new SFFServices.ParameterList { Parameter = paramArray };

                var request = new SFFServices.SffRequest
                {
                    Event = "evFBBQueryArOrder",
                    ParameterList = paramList
                };

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, request, query.BroadbandId, "evFBBQueryArOrder", "evFBBQueryArOrder", "", "FBB|" + query.FullUrl, "");

                using (var service = new SFFServices.SFFServiceService())
                {
                    var data = service.ExecuteService(request);
                    if (data != null && string.IsNullOrEmpty(data.ErrorMessage))
                    {
                        var errorMessage = data.ParameterList.Parameter.FirstOrDefault(item => item.Name == "errorMessage") ?? new Parameter();
                        if (errorMessage.Value.ToSafeString().IndexOf("EB0024") < 0)
                        {

                            foreach (var itemParamList in data.ParameterList.ParameterList1)
                            {

                                CheckAROrderModel model = new CheckAROrderModel();
                                foreach (var itemParam in itemParamList.Parameter)
                                {
                                    PropertyInfo prop = model.GetType().GetProperty(itemParam.Name, BindingFlags.Public | BindingFlags.Instance);
                                    if (null != prop && prop.CanWrite)
                                    {
                                        prop.SetValue(model, itemParam.Value.ToSafeString(), null);
                                    }
                                }
                                result.Add(model);
                            }
                        }


                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Failed", data != null ? data.ErrorMessage : string.Empty, "");
                    }
                }

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SFFServices.SffResponse(), log, "Failed", ex.Message, "");
            }

            return result;
        }
    }
}
