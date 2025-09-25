using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class CheckEmpCodeQueryHandler : IQueryHandler<CheckEmpCodeQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_EMPLOYEE> _FBB_EMPLOYEE;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public CheckEmpCodeQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_EMPLOYEE> fbb_EMPLOYEE,
            IEntityRepository<FBB_CFG_LOV> fbb_CFG_LOV)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _FBB_EMPLOYEE = fbb_EMPLOYEE;
            _FBB_CFG_LOV = fbb_CFG_LOV;
        }

        public string Handle(CheckEmpCodeQuery query)
        {
            InterfaceLogCommand log = null;
            //string URL = "";
            //string X_Token = "N=fbbon90day";
            //string urlParameters = "?Key1=";
            string result = "";

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.EMP_CODE.PadLeft(8, '0'), "CheckEmpCode", "CheckEmpCodeQueryHandler", "", "FBB", "");

                string EnableCallService = "";

                EnableCallService = (from r in _FBB_CFG_LOV.Get()
                                     where r.LOV_NAME == "OM_WS_GetEmployeeProfileByPIN" && r.ACTIVEFLAG == "Y" && r.DISPLAY_VAL == "EnableCallService"
                                     select r.LOV_VAL1).FirstOrDefault().ToSafeString();

                if (EnableCallService == "Y")
                {

                    using (var service = new OMWService.WS_OM_OMService())
                    {
                        string KeyData = "";
                        string userData = "";
                        string pData = "";
                        string dData = "";
                        string omCodeData = "";

                        service.Url = (from r in _FBB_CFG_LOV.Get()
                                       where r.LOV_NAME == "OMWSURL" && r.ACTIVEFLAG == "Y"
                                       select r.LOV_VAL1).FirstOrDefault().ToSafeString();

                        KeyData = (from z in _FBB_CFG_LOV.Get()
                                   where z.LOV_NAME == "sobs" && z.ACTIVEFLAG == "Y"
                                   select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                        userData = (from r in _FBB_CFG_LOV.Get()
                                    where r.LOV_NAME == "OM_WS_GetEmployeeProfileByPIN" && r.ACTIVEFLAG == "Y" && r.DISPLAY_VAL == "resu"
                                    select r.LOV_VAL1).FirstOrDefault().ToSafeString();

                        pData = (from r in _FBB_CFG_LOV.Get()
                                 where r.LOV_NAME == "OM_WS_GetEmployeeProfileByPIN" && r.ACTIVEFLAG == "Y" && r.DISPLAY_VAL == "drowssap"
                                 select r.LOV_VAL1).FirstOrDefault().ToSafeString();

                        dData = (from r in _FBB_CFG_LOV.Get()
                                 where r.LOV_NAME == "OM_WS_GetEmployeeProfileByPIN" && r.ACTIVEFLAG == "Y" && r.DISPLAY_VAL == "Domain"
                                 select r.LOV_VAL1).FirstOrDefault().ToSafeString();

                        omCodeData = (from r in _FBB_CFG_LOV.Get()
                                      where r.LOV_NAME == "OM_WS_GetEmployeeProfileByPIN" && r.ACTIVEFLAG == "Y" && r.DISPLAY_VAL == "OMCode"
                                      select r.LOV_VAL1).FirstOrDefault().ToSafeString();

                        service.Credentials = new System.Net.NetworkCredential(EncryptionUtility.Decrypt(userData, KeyData), EncryptionUtility.Decrypt(pData, KeyData), dData);

                        ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                        string datas = service.OM_WS_GetEmployeeProfileByPIN(omCodeData, query.EMP_CODE.PadLeft(8, '0'));
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(datas)));
                        if (ds != null)
                        {
                            DataTable dt1 = ds.Tables["Permission"];
                            if (dt1 != null)
                            {
                                if (dt1.Rows.Count > 0)
                                {
                                    DataRow dr = dt1.Rows[0];
                                    if (dr.ItemArray[0].ToSafeString() == "Success" && dr.ItemArray[1].ToSafeString() == "000")
                                    {
                                        DataTable dt2 = ds.Tables["Table"];
                                        if (dt2 != null && dt2.Rows.Count > 0)
                                        {
                                            DataRow dr2 = dt2.Rows[0];
                                            result = dr2.ItemArray[1].ToSafeString();
                                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, datas, log, "Success", "", "");
                                        }

                                    }
                                    else
                                    {
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", dr.ItemArray[0].ToSafeString() + " : " + dr.ItemArray[1].ToSafeString(), "");
                                    }
                                }
                                else
                                {
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, datas, log, "Failed", "", "");
                                }
                            }
                            else
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, datas, log, "Failed", "", "");
                            }
                        }
                        else
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, datas, log, "Failed", "", "");
                        }
                    }
                }
                else
                {
                    string EMP_CODE_TMP = query.EMP_CODE.TrimStart('0');
                    var resultData = (from r in _FBB_EMPLOYEE.Get()
                                      where r.EMP_PIN == EMP_CODE_TMP && r.ACTIVE_FLAG == "Y"
                                      select r).ToList();
                    if (resultData != null && resultData.Count > 0)
                    {
                        string userName = "";
                        userName = resultData.Select(t => t.EMP_USER_NAME).FirstOrDefault().ToSafeString();
                        result = userName;
                    }
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }

                //urlParameters = urlParameters + query.EMP_CODE;

                //var lovData = (from r in _FBB_CFG_LOV.Get()
                //               where r.LOV_NAME == "profilefbbon90day" && r.ACTIVEFLAG == "Y"
                //               select r).ToList();
                //if (lovData.Count > 0)
                //{
                //    var lovDataUrl = lovData.Where(t => t.DISPLAY_VAL == "URL");
                //    if (lovDataUrl.Count() > 0)
                //    {
                //        URL = lovDataUrl.FirstOrDefault().LOV_VAL1;
                //    }
                //    var lovDataXToken = lovData.Where(t => t.DISPLAY_VAL == "X-Token");
                //    if (lovDataXToken.Count() > 0)
                //    {
                //        X_Token = lovDataXToken.FirstOrDefault().LOV_VAL1;
                //    }
                //}

                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(URL);
                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-Token", X_Token);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //var content = new FormUrlEncodedContent(new[]
                //    {
                //        new KeyValuePair<string, string>("Key1", query.EMP_CODE.PadLeft(8, '0'))
                //    });


                //var resultData = client.PostAsync(urlParameters, content).Result;
                //if (resultData.IsSuccessStatusCode)
                //{
                //    string resultContent = resultData.Content.ReadAsStringAsync().Result;
                //    var resultObj = JsonConvert.DeserializeObject<MyObj>(resultContent);
                //    if (resultObj.output1 == "000")
                //    {
                //        result = "Y";
                //    }
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                //}
                //else
                //{
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", resultData.RequestMessage.ToString(), "");
                //    string EMP_CODE_TMP = query.EMP_CODE.TrimStart('0');
                //    var resultDataTmp = (from r in _FBB_EMPLOYEE.Get()
                //                         where r.EMP_PIN == EMP_CODE_TMP && r.ACTIVE_FLAG == "Y"
                //                         select r).ToList();
                //    if (resultDataTmp != null && resultDataTmp.Count > 0)
                //    {
                //        result = "Y";
                //    }
                //}

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message.ToString(), "");
            }

            return result;

        }
    }
}
