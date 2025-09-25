using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebApi;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class MasterDataController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateLoadConfigLovCommand> _UpdateLoadConfigLovCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;

        public MasterDataController(
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateLoadConfigLovCommand> UpdateLoadConfigLovCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _UpdateLoadConfigLovCommand = UpdateLoadConfigLovCommand;
            _mailLogCommand = mailLogCommand;
            base.Logger = logger;
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public JsonResult TestGetListWebsiteConfigurationQuery(
            string LOV_TYPE,
            string LOV_NAME,
            string PACKAGE_CODE)
        {
            var model = new GetListWebsiteConfigurationQuery();
            model.TransactionId = Guid.NewGuid().ToString();
            model.ColumnName = LOV_NAME;
            model.LovType = LOV_TYPE;
            model.LovName = PACKAGE_CODE;

            var result = _queryProcessor.Execute(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                if (lov == null || lov.Count == 0)
                {
                    Logger.Info("Get value db is null.");
                }
                return lov;
            }
            catch (Exception ex)
            {
                Logger.Info("Get value db is null.");
                Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public LoadConfigLovModel GetLoadConfigLov(string eventName)
        {
            try
            {

                var query = new GetLoadConfigLovQuery
                {
                    EVENT_NAME = eventName
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new LoadConfigLovModel();
            }
        }

        public void SetLoadConfigLov(string eventName, string no, string ip)
        {
            try
            {
                var command = new UpdateLoadConfigLovCommand
                {
                    EVENT_NAME = eventName,
                    FLAG_NUMBER = no,
                    IP = ip
                };

                _UpdateLoadConfigLovCommand.Handle(command);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
            }
        }

        public JsonResult GetCustomerNat()
        {
            //var dropDown = base.LovData
            //    .Where(l => l.Type.Equals(WebConstants.LovConfigName.Nationality))
            //    .Select(l => new DropdownModel
            //    {
            //        Text = l.LovValue1,
            //        Value = l.Name,
            //        DefaultValue = l.DefaultValue,
            //    }).ToList();

            //var dropDown = new List<DropdownModel>();

            //var query = new GetNationalityQuery
            //{
            //    CurrentCulture = SiteSession.CurrentUICulture,
            //};

            //dropDown = _queryProcessor.Execute(query).Select(n => new DropdownModel
            //{
            //    Text = n.Nationality,
            //    Value = n.InterfaceSFF,
            //}).ToList();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public JsonResult GetCustomerCardType(string customerType)
        {
            var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
               WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            if (customerType == "All")
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                    .Select(l => new DropdownModel
                    {
                        Text = (langFlg == WebConstants.LovConfigName.TitleCodeTh ? l.LovValue1 : l.LovValue2),
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    }).ToList();
                return Json(dropDown, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType) && l.LovValue3.Equals(customerType))
                    .Select(l => new DropdownModel
                    {
                        Text = (langFlg == WebConstants.LovConfigName.TitleCodeTh ? l.LovValue1 : l.LovValue2),
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    }).ToList();
                return Json(dropDown, JsonRequestBehavior.AllowGet);
            }

        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public List<DropdownModel> GetCustomerCardTypeDropdownModel(string customerType)
        {
            var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
               WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            if (customerType == "All")
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType))
                    .Select(l => new DropdownModel
                    {
                        Text = (langFlg == WebConstants.LovConfigName.TitleCodeTh ? l.LovValue1 : l.LovValue2),
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    }).ToList();
                return dropDown;
            }
            else
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals(WebConstants.LovConfigName.CardType) && l.LovValue3.Equals(customerType))
                    .Select(l => new DropdownModel
                    {
                        Text = (langFlg == WebConstants.LovConfigName.TitleCodeTh ? l.LovValue1 : l.LovValue2),
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    }).ToList();
                return dropDown;
            }

        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetCustomerGender()
        {
            var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
                            WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            var dropDown = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.Gender))
                .Select(l => new DropdownModel
                {
                    Text = (langFlg == WebConstants.LovConfigName.TitleCodeTh ? l.LovValue1 : l.LovValue2),
                    Value = l.Name,
                    DefaultValue = l.DefaultValue,
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetContactTime()
        {
            var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
                            WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            var dropDown = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.ContactTime))
                .OrderBy(l => l.OrderBy)
                .Select(l => new DropdownModel
                {
                    Text = (langFlg == WebConstants.LovConfigName.TitleCodeTh ? l.LovValue1 : l.LovValue2),
                    Value = l.Name,
                    DefaultValue = l.DefaultValue,
                })
                .ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetTermCon(string LovType)
        {
            var data = base.LovData
                .Where(l => l.Type.Equals(LovType) && l.ActiveFlag == "Y");

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetTermConEapp()
        {
            var data = base.LovData
                .Where(l => l.Type == "TERM_AND_CONDITION_WIRE_EAPP")
                .OrderBy(d => d.OrderBy)
                .ToList();

            data.ForEach(d =>
            {
                if (d.OrderBy != 0)
                {
                    d.LovValue1 = $"<tr>{d.LovValue1}</tr>";
                    d.LovValue2 = $"<tr>{d.LovValue2}</tr>";
                }
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //18.1 FTTB Sell Router
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetConditionLov(string lovType = "", string lovName = "")
        {
            if (string.IsNullOrEmpty(lovName))
            {
                var data = base.LovData
                .Where(l => l.Type.Equals(lovType))
                .OrderBy(o => o.OrderBy);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = base.LovData
                .Where(l => l.Type.Equals(lovType) && l.Name.Equals(lovName))
                .OrderBy(o => o.OrderBy);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult WatchingList(string aisAirNo)
        {
            try
            {
                var data = base.LovData
                            .Where(l =>
                                l.Type.Equals("WATCH_LIST")
                                && l.LovValue1 == aisAirNo).Any();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetDocumentMessage(string LovName)
        {
            var data = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.Document)
                        && l.Name.Equals(LovName));

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetLastPopupMessage(string LovName, string dataType)
        {
            var data = base.LovData
                    .Where(l => l.Type.Equals(WebConstants.LovConfigName.Screen)
                    && l.Name.Equals(LovName));

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public List<CoverageValueModel> GetCoverrageValueModel()
        {
            var query = new GetCoverageAreaQuery
            {
                CurrentCulture = SiteSession.CurrentUICulture,
                // todo : wait for implement
                ZipCodeId = "",
            };

            var result = _queryProcessor.Execute(query);
            return result;
        }

        public List<CoverageRelValueModel> GetCoverrageRelValueModel(string coverageNodeName)
        {
            var query = new GetCoverageAreaRelQuery
            {
                CurrentCulture = SiteSession.CurrentUICulture,
                NodeName = coverageNodeName,
            };

            var result = _queryProcessor.Execute(query);
            return result;
        }

        private List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
        {
            var data = base.LovData
               .Where(l => l.Type.Equals(fbbConstType))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }

        public List<FbbConstantModel> GetConstants()
        {
            return GetFbbConstantModel(WebConstants.LovConfigName.FbbConstant);
        }

        public List<FbbConstantModel> GetExceptionIdCard()
        {
            return GetFbbConstantModel(WebConstants.LovConfigName.FbbExceptionIdCard);
        }

        // web services
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetCustomerTitle(string customerCardType)
        {
            var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
                WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            var query = new GetCustomerTitleQuery
            {
                CurrentCulture = SiteSession.CurrentUICulture,
                CustomerType = customerCardType,
            };

            var dropDown = _queryProcessor.Execute(query)
                .Select(t => new DropdownModel
                {
                    Text = t.Title,
                    Value = t.TitleCode,
                    DefaultValue = t.DefaultValue,
                }).ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 1800, Location = OutputCacheLocation.Server, VaryByParam = "currentCulture", NoStore = true)]
        public List<ZipCodeModel> GetZipCodeList(int currentCulture)
        {
            try
            {
                var query = new GetZipCodeQuery
                {
                    CurrentCulture = currentCulture,
                };

                var zipCodeList = _queryProcessor.Execute(query);

                return zipCodeList;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<ZipCodeModel>();
            }
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetProvince()
        {
            var provType = new List<DropdownModel>();
            try
            {
                provType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .OrderBy(t => t.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(provType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAmphur(string provinceFilter)
        {
            var amphType = new List<DropdownModel>();
            try
            {
                amphType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter)) && (!z.Amphur.Contains("ปณ") && !z.Amphur.Contains("PO")))
                    .GroupBy(z => z.Amphur)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Amphur, Value = item.Amphur };
                    })
                    .OrderBy(t => t.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(amphType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTumbon(string provinceFilter, string amphurFilter)
        {
            var tumbType = new List<DropdownModel>();
            try
            {


                tumbType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter))
                                    && (string.IsNullOrEmpty(z.Amphur) || z.Amphur.Equals(amphurFilter)))
                    .GroupBy(z => z.Tumbon)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Tumbon, Value = item.Tumbon };
                    })
                    .OrderBy(t => t.Text)
                    .ToList();
            }
            catch (Exception) { }

            return Json(tumbType, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetZipCode(string provinceFilter, string amphurFilter, string tumbonFilter)
        {
            //ZipCodeModel zipCodeMode = null;
            var zipCodeModelList = new List<DropdownModel>();
            try
            {
                var amphurToFilter = "";
                int index1 = amphurFilter.IndexOf('(');
                if (index1 > 0)
                {
                    int index2 = amphurFilter.IndexOf(')');
                    amphurToFilter = amphurFilter.Remove(index1, index2 - index1 + 1);
                }
                else
                {
                    amphurToFilter = amphurFilter;
                }

                zipCodeModelList = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.Equals(provinceFilter))
                        && (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.Contains(amphurToFilter))
                        && (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.Equals(tumbonFilter)))
                    .Select(z => new DropdownModel { Text = z.ZipCode, Value = z.ZipCodeId, })
                    .ToList();

            }
            catch (Exception) { }

            //if (null == zipCodeModelList)
            //{
            //    return Json(new { ZipCode = "", ZipCodeId = 0m }, JsonRequestBehavior.AllowGet);
            //}

            //return Json(new { zipCodeMode.ZipCode, zipCodeMode.ZipCodeId }, JsonRequestBehavior.AllowGet);
            return Json(zipCodeModelList, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetLovInTemp()
        {
            var lovs = LovData.ToList();
            return Json(lovs, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetLovInDB()
        {
            var lovs = _queryProcessor.Execute(new GetLovQuery());
            return Json(lovs, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetAddress(string province, string amphur, string tumbon)
        {
            Dictionary<string, List<DropdownModel>> dict = new Dictionary<string, List<DropdownModel>>();
            try
            {
                var provType = new List<DropdownModel>();
                provType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                var amphType = new List<DropdownModel>();
                amphType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(province)))
                    .GroupBy(z => z.Amphur)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Amphur, Value = item.Amphur };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                var tumbType = new List<DropdownModel>();
                tumbType = base.ZipCodeData(SiteSession.CurrentUICulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(province))
                                    && (string.IsNullOrEmpty(z.Amphur) || z.Amphur.Equals(amphur)))
                    .GroupBy(z => z.Tumbon)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Tumbon, Value = item.Tumbon };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                var amphurToFilter = "";
                int index1 = amphur.IndexOf('(');
                if (index1 > 0)
                {
                    int index2 = amphur.IndexOf(')');
                    amphurToFilter = amphur.Remove(index1, index2 - index1 + 1);
                }
                else
                {
                    amphurToFilter = amphur;
                }

                var zipCodeList = new List<DropdownModel>();
                zipCodeList = base.ZipCodeData(SiteSession.CurrentUICulture)
                   .Where(z => (!string.IsNullOrEmpty(z.Province) && z.Province.Equals(province))
                       && (!string.IsNullOrEmpty(z.Amphur) && z.Amphur.Contains(amphurToFilter))
                       && (!string.IsNullOrEmpty(z.Tumbon) && z.Tumbon.Equals(tumbon)))
                   .Select(z => new DropdownModel { Text = z.ZipCode, Value = z.ZipCodeId, })
                   .ToList();

                dict.Add("province", provType);
                dict.Add("amphur", amphType);
                dict.Add("tumbon", tumbType);
                dict.Add("zipcode", zipCodeList);
            }
            catch (Exception)
            { }

            return Json(dict, JsonRequestBehavior.AllowGet);
        }

        public GssoSsoResponseModel ConfirmOneTimePW(string msisdn, string pwd, string transactionID)
        {
            GssoSsoResponseModel result = new GssoSsoResponseModel();
            try
            {
                var query = new ConfirmOneTimePWQuery()
                {
                    msisdn = msisdn,
                    pwd = pwd,
                    transactionID = transactionID
                };
                result = new GssoSsoResponseModel();
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                return null;
            }

            return result;
        }

        public GssoSsoResponseModel SendOneTimePW(string msisdn, string accountType)
        {
            GssoSsoResponseModel result = new GssoSsoResponseModel();
            try
            {
                var query = new SendOneTimePWQuery()
                {
                    msisdn = msisdn,
                    accountType = accountType
                };
                result = new GssoSsoResponseModel();
                result = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                return null;
            }

            return result;
        }

        // for test
        public SaveOrderResp Test(GetSaveOrderRespQuery query)
        {
            var result = _queryProcessor.Execute(query);

            return result;
        }

        public void TestMailLog()
        {
            var query = new MailLogCommand
            {
                CustomerId = "F69EABDFADCE6525E04365A0FC0A7563",
            };

            _mailLogCommand.Handle(query);

        }

        public JsonResult WriteLogFromView(string msg)
        {
            Logger.Info(msg);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetListISP()
        {

            //var dropDown = base.LovData
            //    .Where(l => l.Type.Equals("FBB_CONSTANT") && l.Name.Equals("LIST_ISP")
            //    && l.LovValue5.Equals("FBBOR003"))
            //.Select(l => new DropdownModel
            //{
            //    Text = l.LovValue1,
            //    Value = l.LovValue1,
            //    //DefaultValue = "",
            //}).ToList();

            var langFlg = (SiteSession.CurrentUICulture.IsThaiCulture() ?
                         WebConstants.LovConfigName.TitleCodeTh : WebConstants.LovConfigName.TitleCodeEn);

            var dropDown = base.LovData
                .Where(l => l.Type == "FBB_CONSTANT" && l.Name == "LIST_ISP" && l.LovValue5 == "FBBOR003")
                .OrderBy(l => l.OrderBy)
                .Select(l => new DropdownModel
                {
                    Text = (langFlg == WebConstants.LovConfigName.TitleCodeTh ? l.LovValue1 : l.LovValue1),
                    Value = l.LovValue1,
                    DefaultValue = l.DefaultValue,
                })
                .ToList();

            return Json(dropDown, JsonRequestBehavior.AllowGet);
        }

        //redesign
        public int GetSessionLoginStatus(string custInternetNum, string sessionId)
        {
            int result = -1;
            var requst = new GetSessionLoginStatusQuery
            {
                CustInternetNum = custInternetNum,
                SessionId = sessionId
            };
            var resultQuery = _queryProcessor.Execute(requst);
            if (resultQuery != null)
            {
                result = resultQuery.ReturnStatus;
            }
            return result;
        }

        public JsonResult GetLovListJson(string type, string name = "")
        {
            var query = new GetLovQuery
            {
                LovType = type,
                LovName = name
            };

            var lov = _queryProcessor.Execute(query);
            return Json(lov, JsonRequestBehavior.AllowGet);
        }

    }
}
