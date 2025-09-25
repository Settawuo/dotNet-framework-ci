using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
    public class QueryDBDProfileHandler : IQueryHandler<QueryDBDProfileQuery, QueryDBDProfileModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;

        public QueryDBDProfileHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
        }

        public QueryDBDProfileModel Handle(QueryDBDProfileQuery query)
        {

            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TAX_ID, "QueryDBDProfile", "QueryDBDProfileQuery", null, "FBB|", "");
            QueryDBDProfileModel result = new QueryDBDProfileModel();

            try
            {
                string UrlStr = "";

                UrlStr = (from z in _lov.Get()
                          where z.LOV_NAME == "QueryDBDProfile" && z.DISPLAY_VAL == "URL" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                          select z.LOV_VAL1).FirstOrDefault().ToSafeString();


                if (UrlStr != "" && query.TAX_ID != "")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        string jsonStr = JsonConvert.SerializeObject(query);
                        InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, jsonStr, query.TAX_ID, "QueryDBDProfileSub", "CheckPrivilegePointQuerySub", null, "FBB|", "");
                        var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        var resultData = client.PostAsync(UrlStr, content).Result;
                        if (resultData != null && resultData.IsSuccessStatusCode)
                        {

                            string resultContent = resultData.Content.ReadAsStringAsync().Result;
                            result = JsonConvert.DeserializeObject<QueryDBDProfileModel>(resultContent);
                            if (result != null)
                            {

                                if ((result.ResultCode == "0000" && result.ResultMessage == "Success") || result.ResultCode == "0001")
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                                }
                                else
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "", "");
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                                }
                            }
                            else
                            {

                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "Sevice No result", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }

                        }
                        else if (resultData != null)
                        {
                            string resultContent = resultData.Content.ReadAsStringAsync().Result;
                            result = JsonConvert.DeserializeObject<QueryDBDProfileModel>(resultContent);
                            if (result != null)
                            {

                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log2, "Failed", "Call Service not Success", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }
                            else
                            {

                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log2, "Failed", "Call Service not Success", "");
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "", "");
                            }
                        }
                        else
                        {

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log2, "Failed", "Call Service not Success", "");
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", "", "");
                        }

                    }
                }
                else
                {

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", "", "");
                }
            }
            catch (Exception ex)
            {

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.GetErrorMessage(), "");
            }
            return result;
        }
    }
}
