using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.WebServices.FBSS;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBContract.Queries.WebServices.FBSS;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;

namespace FBBConfig.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class MasterDataController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<AlterChangedBuildingCommand> _AlterChangedBuildingCommand;

        public MasterDataController(ILogger logger, IQueryProcessor queryProcessor, ICommandHandler<AlterChangedBuildingCommand> AlterChangedBuildingCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _AlterChangedBuildingCommand = AlterChangedBuildingCommand;
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
                _Logger.Info(ex.GetErrorMessage());
                return new List<ZipCodeModel>();
            }
        }

        public List<ZipCodeModel> GetZipCodeAirList(int currentCulture, string regioncode)
        {
            try
            {
                var query = new GetZipCodeAirQuery
                {
                    CurrentCulture = currentCulture,
                    Regioncode = regioncode
                };

                var zipCodeList = _queryProcessor.Execute(query);

                return zipCodeList;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<ZipCodeModel>();
            }
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
                return lov;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public List<LovModel> GetLovByTypeAndLovVal5(string type, string lovval5)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = type,
                LOV_VAL5 = lovval5
            };
            var data = _queryProcessor.Execute(query);

            return data;
        }

        public JsonResult GetProvince(int currentCulture)
        {
            var provType = new List<DropdownModel>();
            try
            {
                provType = base.ZipCodeData(currentCulture) // 1 = th , 2 = en
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .ToList();
            }
            catch (Exception) { }

            return Json(provType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProvinceAir(int currentCulture, string regioncode)
        {
            var provType = new List<DropdownModel>();
            try
            {
                provType = base.ZipCodeDataAir(currentCulture, regioncode) // 1 = th , 2 = en
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .ToList();
            }
            catch (Exception) { }

            return Json(provType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAmphur(int currentCulture, string provinceFilter)
        {
            var amphType = new List<DropdownModel>();
            try
            {
                amphType = base.ZipCodeData(currentCulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter)))
                    .GroupBy(z => z.Amphur)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Amphur, Value = item.Amphur };
                    })
                    .ToList();
            }
            catch (Exception) { }

            return Json(amphType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTumbon(int currentCulture, string provinceFilter, string amphurFilter)
        {
            var tumbType = new List<DropdownModel>();
            try
            {
                tumbType = base.ZipCodeData(currentCulture)
                    .Where(z => (string.IsNullOrEmpty(z.Province) || z.Province.Equals(provinceFilter))
                                    && (string.IsNullOrEmpty(z.Amphur) || z.Amphur.Equals(amphurFilter)))
                    .GroupBy(z => z.Tumbon)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Tumbon, Value = item.Tumbon };
                    })
                    .ToList();
            }
            catch (Exception) { }

            return Json(tumbType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetZipCode(int currentCulture, string provinceFilter, string amphurFilter, string tumbonFilter)
        {
            //ZipCodeModel zipCodeMode = null;
            var zipCodeModelList = new List<DropdownModel>();
            try
            {
                //var amphurToFilter = "";
                //int index1 = amphurFilter.IndexOf('(');
                //if (index1 > 0)
                //{
                //    int index2 = amphurFilter.IndexOf(')');
                //    amphurToFilter = amphurFilter.Remove(index1, index2 - index1 + 1);
                //}
                //else
                //{
                //    amphurToFilter = amphurFilter;
                //}

                zipCodeModelList = base.ZipCodeData(currentCulture)
                    .Where(z => z.Province.Equals(provinceFilter)
                        && z.Amphur.Equals(amphurFilter)
                        && z.Tumbon.Equals(tumbonFilter))
                        .Select(z => new DropdownModel
                        {
                            Text = z.ZipCode,
                            Value = z.ZipCodeId,
                        })
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

        public JsonResult GetRegionCode(string type)
        {
            var regioncode = new List<DropdownModel>();
            try
            {
                regioncode = base.LovData
                    .Where(r => r.Type == type)
                    .OrderBy(o => o.OrderBy)
                    .Select(l => new DropdownModel
                    {
                        Text = l.Name,
                        Value = l.Name,
                    }).ToList();

            }
            catch (Exception) { }
            regioncode.Insert(0, new DropdownModel { Text = "กรุณาเลือก", Value = "" });
            return Json(regioncode, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetTermCon(string LovType)
        {
            var data = base.LovData
                .Where(l => l.Type.Equals(LovType));
            //.Select(l => l.LovValue1).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReadHistoryLog([DataSourceRequest] DataSourceRequest request, string application, string refKey, string refName, bool firstLoadFlag)
        {
            if (!string.IsNullOrEmpty(application))
            {
                var query = new SelectHistoryLogQuery
                {
                    Application = application,
                    Ref_Key = refKey,
                    Ref_Name = refName,
                    FirstLoad = firstLoadFlag
                };

                var result = _queryProcessor.Execute(query);

                return Json(result.ToDataSourceResult(request));
            }
            else
            {
                var result = new List<FBB_HISTORY_LOG>();
                return Json(result.ToDataSourceResult(request));
            }
        }

        #region read data via call
        public JsonResult SelectLov(string type, bool lovNameText = false)
        {
            var query = new SelectLovQuery
            {
                LOV_TYPE = type
            };
            var data = _queryProcessor.Execute(query);

            if (!lovNameText)
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "", LOV_VAL1 = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "กรุณาเลือก", LOV_VAL1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectLovNoIndex0DDL(string type, bool haveall = false)
        {
            var query = new SelectLovQuery
            {
                LOV_TYPE = type
            };
            var data = _queryProcessor.Execute(query);

            if (haveall)
            {
                data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectLovByTypeAndLovVal5(string type, string lovval5, bool haveall = false)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = type,
                LOV_VAL5 = lovval5
            };
            var data = _queryProcessor.Execute(query);

            if (haveall)
            {
                data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectLovDisplayDDL(string type, string display, string lovval5)
        {
            var query = new SelectLovDisplayQuery
            {
                LOV_TYPE = type,
                DISPLAY_VAL = display,
                LOV_VAL5 = lovval5
            };
            var data = _queryProcessor.Execute(query);

            if (!data.Any())
            {
                data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SelecRegion_ByRegion(string type, bool lovNameText = false)
        {
            var query = new SelectLovQuery
            {
                LOV_TYPE = type
            };
            var data = _queryProcessor.Execute(query);

            if (!lovNameText)
                data.Insert(0, new LovModel { DISPLAY_VAL = "All", LOV_NAME = "All", LOV_VAL1 = "All" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "All", LOV_NAME = "All", LOV_VAL1 = "All" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectLovDefaultValue(string type)
        {
            var query = new SelectLovDefaultValueQuery
            {
                LOV_TYPE = type
            };
            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public JsonResult SelectProvince(string regionCode = "", string langFlag = "N")
        {
            var query = new SelectProvinceQuery
            {
                REGION_CODE = regionCode,
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectAmphur(string regionCode = "", string province = "", string langFlag = "N", string fttx = "N")
        {
            var query = new SelectAmphurQuery
            {
                REGION_CODE = regionCode,
                PROVINCE = province,
                Lang_Flag = langFlag,
                FTTX = fttx

            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectTumbon(string regionCode = "", string province = "", string aumphur = "", string langFlag = "N")
        {
            var query = new SelectTumbonQuery
            {
                REGION_CODE = regionCode,
                PROVINCE = province,
                AUMPHUR = aumphur,
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public JsonResult Selecttower_th(string region = "", string province = "", string aumphur = "", string tumbon = "", string FaqDataOnloandData = "", string langFlag = "N")
        {
            var query = new selectDataTowerTh
            {
                REGION_CODE = region,
                PROVINCE = province,
                AUMPHUR = aumphur,
                TUMBON = tumbon,
                Lang_Flag = FaqDataOnloandData
            };
            var data = _queryProcessor.Execute(query);
            data = data.Where(p => p.LOV_NAME != null).ToList();

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public JsonResult Selecttower_th_Manager(string langFlag = "N")
        {
            var query = new selectDataTowerTh
            {

                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            if (langFlag == "N")
                data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            else
                data.Insert(0, new LovModel { DISPLAY_VAL = "Please select", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectZipCode(string province = "", string aumphur = "", string tumbon = "", string langFlag = "N")
        {
            var query = new SelectZipCodeQuery
            {
                Province = province,
                Aumphur = aumphur,
                Tumbon = tumbon,
                Language = langFlag
            };

            var data = _queryProcessor.Execute(query);
            List<ZipCodeModel> dd = new List<ZipCodeModel>();
            dd.Add(data);
            return Json(dd, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOwnerProduct(bool label = true)
        {
            var query = new SelectOwnerProductQuery
            {

            };
            var data = _queryProcessor.Execute(query);


            data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOwnerType()
        {
            var query = new SelectOwnerTypeQuery
            {

            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectModelNmae()
        {
            var query = new SelectModelNameQuery
            {

            };

            var data = _queryProcessor.Execute(query);
            data.Insert(0, new ModelNameCardModel { DISPLAY_VAL = "กรุณาเลือก", Band = "" });
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SelectModelNmaeDLSAM()
        {
            var query = new SelectModelNameQuery
            {
                DlasmBran = "DSALAM"
            };

            var data = _queryProcessor.Execute(query);
            data.Insert(0, new ModelNameCardModel { DISPLAY_VAL = "กรุณาเลือก", Band = "" });
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SelectModelCradModel()
        {

            var query = new SelectModelNameQuery
            {
                CardModel = "ModelNmae"
            };

            var data = _queryProcessor.Execute(query);


            data.Insert(0, new ModelNameCardModel { Model = "กรุณาเลือก", CardModelID = 0 });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectModelCradModelID(decimal CardModelID)
        {

            var query = new SelectModelNameQuery
            {

                CardModel = "ModelNmaeSearchID",
                CardModelidID = CardModelID

            };

            var data = _queryProcessor.Execute(query);
            ///    data.Insert(0, new ModelNameCardModel { DISPLAY_VAL = "กรุณาเลือก", Model = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SelectMaxCardInfoModel()
        {

            var query = new SelectModelNameQuery
            {

                CardModel = "ModelNmaeSearchID",


            };

            var data = _queryProcessor.Execute(query);
            data.Insert(0, new ModelNameCardModel { DISPLAY_VAL = "กรุณาเลือก", Model = "" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SetdataReserve()
        {
            var query = new SelectModelNameQuery
            {
                Reserve = "Reserve"
            };

            var data = _queryProcessor.Execute(query);
            data.Insert(0, new ModelNameCardModel { DISPLAY_VAL = "กรุณาเลือก", reserve = "" });
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SetdataCardModelCardPort()
        {
            var query = new SelectModelNameQuery
            {
                Reserve = "ModelDropdown"
            };

            var data = _queryProcessor.Execute(query);
            data.Insert(0, new ModelNameCardModel { Model = "กรุณาเลือก", reserve = "" });

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SelectHistoryLogRefName(string application)
        {
            if (!string.IsNullOrEmpty(application))
            {
                var query = new SelectHistoryLogQuery
                {
                    Application = application
                };

                var result = _queryProcessor.Execute(query);

                var data = (from h in result
                            where h.REF_NAME != null
                            orderby h.REF_NAME != ""
                            select new LovModel() { DISPLAY_VAL = h.REF_NAME, LOV_NAME = h.REF_NAME }).DistinctBy(a => a.DISPLAY_VAL).ToList();

                //data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        public JsonResult selectDataBuildingTieCardPort(decimal DSLAMID)
        {
            var query = new GetBulidingCardPortTie
            {
                DSALMID = DSLAMID
            };



            var data = _queryProcessor.Execute(query);
            data.Insert(0, new BuailTieQuery { building = "กรุณาเลือก", Dispay = "" });
            return Json(data, JsonRequestBehavior.AllowGet);

        }



        public JsonResult SelectFTTX()
        {
            var query = new SelectFTTXQuery();



            var data = _queryProcessor.Execute(query);
            /// data.Insert(0, new LovModel { DISPLAY_VAL = "กรุณาเลือก", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateChangeBuilding()
        {
            var data = QueryBuild();
            AlterChangeBuild(data);
            if (data.Count != 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        private List<FBSSChangedAddressInfo> QueryBuild()
        {
            var query = new GetFBSSChangedBuilding
            {
                //StartDate = DateTime.Parse("01/11/2014"),
                //EndDate = DateTime.Parse("07/11/2014")
            };

            var result = _queryProcessor.Execute(query);

            return result;

        }

        private void AlterChangeBuild(List<FBSSChangedAddressInfo> data)
        {
            //data = createTest();
            var command = new AlterChangedBuildingCommand
            {
                FBSSChangedAddressInfos = data,
                ActionBy = "Admin"
            };

            _AlterChangedBuildingCommand.Handle(command);

        }


        #region Redesign


        public JsonResult GetProblemType(string type)
        {
            var query = new SelectLovQuery
            {
                LOV_TYPE = type
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "All", LOV_NAME = "", LOV_VAL1 = "All" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private bool islang_flag_Province(string province) // for where province language
        {
            province = province.Replace(" ", "");
            bool _result_islang_flag = true;
            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(province, "^[a-zA-Z0-9]*$"))
                {
                    _result_islang_flag = false;
                }
                //var _ZipCodeData = ZipCodeData(2);
                //var _Result_ZipCodeData = _ZipCodeData.Where(p => p.IsThai = true && p.Province == province).GroupBy(z => z.Province);
                //var _Count_ZipCodeData = _Result_ZipCodeData.Count(); // Y = eng , N = th
                //if (_Count_ZipCodeData > 0) _result_islang_flag = true;// thai
                //else _result_islang_flag = _result_islang_flag = false;
            }
            catch (Exception ex)
            {

            }
            return _result_islang_flag;
        }

        //resen order
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public JsonResult GetAddress(string province, string amphur, string tumbon)
        {
            Dictionary<string, List<DropdownModel>> dict = new Dictionary<string, List<DropdownModel>>();

            try
            {
                bool _islang_flag_Province = islang_flag_Province(province);

                var provType = new List<DropdownModel>();
                provType = base.ZipCodeData(2)
                    .Where(z => z.IsThai == _islang_flag_Province)
                    .GroupBy(z => z.Province)
                    .Select(z =>
                    {
                        var item = z.First();
                        return new DropdownModel { Text = item.Province, Value = item.Province };
                    })
                    .OrderBy(o => o.Text)
                    .ToList();

                var amphType = new List<DropdownModel>();
                amphType = base.ZipCodeData(2)
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
                tumbType = base.ZipCodeData(2)
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
                if (amphur != null)
                {
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
                }

                var zipCodeList = new List<DropdownModel>();
                zipCodeList = base.ZipCodeData(2)
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
    }
}
