using FBBConfig.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBConfig.Controllers
{
    public class SelectPackageController : FBBConfigController
    {
        private IQueryProcessor _queryProcessor;
        public SelectPackageController(ILogger logger, IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }


        public JsonResult GetACC_CATEGORY(string ACC_CATEGORY = "")
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "MAPPING_ACC_CATEGORY" && l.Type == "FBB_CONSTANT" && l.LovValue1 == ACC_CATEGORY).ToList();

            var acc_category = config.Select(i => i.LovValue2).FirstOrDefault();

            return Json(acc_category, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 30)]
        public JsonResult GetDescription(string Packgrop)
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "DESCRIPTION_BY_PACKAGE_GROUP" && Packgrop.Trim().Contains(l.Text)).ToList();
            List<LovScreenValueModel> screenValue;
            screenValue = config.Select(l => new LovScreenValueModel
            {
                Name = l.Name,
                PageCode = l.LovValue5,
                DisplayValue = l.LovValue1,
                LovValue3 = l.LovValue3,
                GroupByPDF = l.LovValue4,
                OrderByPDF = l.OrderBy,
                Type = l.Type,
                DefaultValue = l.DefaultValue
            }).ToList();

            return Json(screenValue, JsonRequestBehavior.AllowGet);

        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 30)]
        public JsonResult GetRemark(string Packgroup, string BuildingName)
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "REMARK_BY_PACKAGE_GROUP" && Packgroup.Trim().Contains(l.Text) && l.LovValue3 == BuildingName).ToList();
            if (config.Count < 1)
            {
                config = base.LovData.Where(l => l.Name == "REMARK_BY_PACKAGE_GROUP" && Packgroup.Trim().Contains(l.Text) && string.IsNullOrEmpty(l.LovValue3)).ToList();
            }

            List<LovScreenValueModel> screenValue;
            screenValue = config.Select(l => new LovScreenValueModel
            {
                Name = l.Name,
                PageCode = l.LovValue5,
                DisplayValue = l.LovValue1,
                LovValue3 = l.LovValue3,
                GroupByPDF = l.LovValue4,
                OrderByPDF = l.OrderBy,
                Type = l.Type,
                DefaultValue = l.DefaultValue
            }).ToList();

            return Json(screenValue, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 30)]
        public JsonResult GetAllowPopup(string Packgrop, string line)
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "SHOW_PAGE_SELECT_VAS_" + line && Packgrop.Trim().Contains(l.Text)).ToList();
            List<LovScreenValueModel> screenValue;

            screenValue = config.Select(l => new LovScreenValueModel
            {
                Name = l.Name,
                PageCode = l.LovValue5,
                DisplayValue = l.LovValue1,
                LovValue3 = l.LovValue3,
                GroupByPDF = l.LovValue4,
                OrderByPDF = l.OrderBy,
                Type = l.Type,
                DefaultValue = l.DefaultValue
            }).ToList();

            return Json(screenValue, JsonRequestBehavior.AllowGet);

        }

        [OutputCache(Location = OutputCacheLocation.Server, Duration = 3600)]
        public ActionResult GetListPackageService(string P_OWNER_PRODUCT = "", string P_PRODUCT_SUBTYPE = "", string P_NETWORK_TYPE = "", string P_SERVICE_DAY = "",
            string line = "", string P_PACKAGE_FOR = "", string P_PACKAGE_CODE = "", string isPartner = "", string partnerName = "", string accessMode = "",
            string interfaceId = "", string no = "", string topUp = "", string P_SFF_PROMOCODE = "", string p_location_code = "", string p_asc_code = "", string p_region = "",
            string p_province = "", string p_district = "", string p_sub_district = "", string p_address_type = "", string p_building_name = "", string p_building_no = "",
            string p_partner_type = "", string p_partner_subtype = "", string p_Serenade_Flag = "", string p_Customer_Type = "", string p_Address_Id = "", string p_InstallationType = "",
            string p_Plug_And_Play_Flag = "", string p_mobile = "", string p_rental_flag = "")
        {
            //// Load Package same old
            StringReader streamSer = null;
            XmlTextReader readerSer = null;
            DataSet xmlDSSer = new DataSet();
            var _query = new GetPackagebyServiceQuery()
            {
                P_IN_TRANSACTION_ID = p_mobile
            };
            var _FB_Interfce_log_byServiceModel = _queryProcessor.Execute(_query);

            streamSer = new StringReader(_FB_Interfce_log_byServiceModel.P_IN_XML_PARAM[0].IN_XML_PARAM);
            readerSer = new XmlTextReader(streamSer);
            xmlDSSer.ReadXml(readerSer);
            DataTable _GetListPackageByServiceQuery = xmlDSSer.Tables["GetListPackageByServiceQuery"];
            //string _GetListPackageByServiceQuery_Serenade_Flag = _GetListPackageByServiceQuery.Rows[0]["P_Serenade_Flag"].ToSafeString();
            //string _GetListPackageByServiceQuery_P_Plug_And_Play_Flag = _GetListPackageByServiceQuery.Rows[0]["P_Plug_And_Play_Flag"].ToSafeString();

            // protect if no tag XML na kup eiei
            string _GetListPackageByServiceQuery_P_PRODUCT_SUBTYPE = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_PRODUCT_SUBTYPE")) _GetListPackageByServiceQuery_P_PRODUCT_SUBTYPE = _GetListPackageByServiceQuery.Rows[0]["P_PRODUCT_SUBTYPE"].ToSafeString();
            string _GetListPackageByServiceQuery_P_NETWORK_TYPE = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_NETWORK_TYPE")) _GetListPackageByServiceQuery_P_NETWORK_TYPE = _GetListPackageByServiceQuery.Rows[0]["P_NETWORK_TYPE"].ToSafeString();
            string _GetListPackageByServiceQuery_P_SERVICE_DAY = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_SERVICE_DAY")) _GetListPackageByServiceQuery_P_SERVICE_DAY = _GetListPackageByServiceQuery.Rows[0]["P_SERVICE_DAY"].ToSafeString();
            string _GetListPackageByServiceQuery_P_PACKAGE_FOR = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_PACKAGE_FOR")) _GetListPackageByServiceQuery_P_PACKAGE_FOR = _GetListPackageByServiceQuery.Rows[0]["P_PACKAGE_FOR"].ToSafeString();
            string _GetListPackageByServiceQuery_P_PACKAGE_CODE = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_PACKAGE_CODE")) _GetListPackageByServiceQuery_P_PACKAGE_CODE = _GetListPackageByServiceQuery.Rows[0]["P_PACKAGE_CODE"].ToSafeString();
            string _GetListPackageByServiceQuery_TransactionID = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("TransactionID")) _GetListPackageByServiceQuery_TransactionID = _GetListPackageByServiceQuery.Rows[0]["TransactionID"].ToSafeString();
            //string _GetListPackageByServiceQuery_IsPartner = _GetListPackageByServiceQuery.Rows[0]["IsPartner"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Location_Code = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Location_Code")) _GetListPackageByServiceQuery_P_Location_Code = _GetListPackageByServiceQuery.Rows[0]["P_Location_Code"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Asc_Code = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Asc_Code")) _GetListPackageByServiceQuery_P_Asc_Code = _GetListPackageByServiceQuery.Rows[0]["P_Asc_Code"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Partner_Type = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Partner_Type")) _GetListPackageByServiceQuery_P_Partner_Type = _GetListPackageByServiceQuery.Rows[0]["P_Partner_Type"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Partner_SubType = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Partner_SubType")) _GetListPackageByServiceQuery_P_Partner_SubType = _GetListPackageByServiceQuery.Rows[0]["P_Partner_SubType"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Region = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Region")) _GetListPackageByServiceQuery_P_Region = _GetListPackageByServiceQuery.Rows[0]["P_Region"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Province = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Province")) _GetListPackageByServiceQuery_P_Province = _GetListPackageByServiceQuery.Rows[0]["P_Province"].ToSafeString();
            string _GetListPackageByServiceQuery_P_District = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_District")) _GetListPackageByServiceQuery_P_District = _GetListPackageByServiceQuery.Rows[0]["P_District"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Sub_District = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Sub_District")) _GetListPackageByServiceQuery_P_Sub_District = _GetListPackageByServiceQuery.Rows[0]["P_Sub_District"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Address_Type = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Address_Type")) _GetListPackageByServiceQuery_P_Address_Type = _GetListPackageByServiceQuery.Rows[0]["P_Address_Type"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Building_Name = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Building_Name")) _GetListPackageByServiceQuery_P_Building_Name = _GetListPackageByServiceQuery.Rows[0]["P_Building_Name"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Building_No = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Building_No")) _GetListPackageByServiceQuery_P_Building_No = _GetListPackageByServiceQuery.Rows[0]["P_Building_No"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Serenade_Flag = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Serenade_Flag")) _GetListPackageByServiceQuery_P_Serenade_Flag = _GetListPackageByServiceQuery.Rows[0]["P_Serenade_Flag"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Customer_Type = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Customer_Type")) _GetListPackageByServiceQuery_P_Customer_Type = _GetListPackageByServiceQuery.Rows[0]["P_Customer_Type"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Address_Id = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Address_Id")) _GetListPackageByServiceQuery_P_Address_Id = _GetListPackageByServiceQuery.Rows[0]["P_Address_Id"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Plug_And_Play_Flag = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Plug_And_Play_Flag")) _GetListPackageByServiceQuery_P_Plug_And_Play_Flag = _GetListPackageByServiceQuery.Rows[0]["P_Plug_And_Play_Flag"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Rental_Flag = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Rental_Flag")) _GetListPackageByServiceQuery_P_Rental_Flag = _GetListPackageByServiceQuery.Rows[0]["P_Rental_Flag"].ToSafeString();
            string _GetListPackageByServiceQuery_P_Customer_subtype = "";
            if (_GetListPackageByServiceQuery.Columns.Contains("P_Customer_subtype")) _GetListPackageByServiceQuery_P_Customer_subtype = _GetListPackageByServiceQuery.Rows[0]["P_Customer_subtype"].ToSafeString();

            //string _GetListPackageByServiceQuery_FullUrl = _GetListPackageByServiceQuery.Rows[0]["FullUrl"].ToSafeString();
            Session["FullUrl"] = this.Url.Action("OnSave", "ResendOrder", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
            {
                FullUrl = Session["FullUrl"].ToSafeString();
            }
            var jsonResult = new ServiceStackJsonResult();
            // ส่ง access mode list ที่ได้มาจาก GetFBSSFeasibilityCheckHandler

            var accessModeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FBSSAccessModeInfo>>(accessMode);
            //var accessModeModel = new JavaScriptSerializer().Deserialize<List<FBSSAccessModeInfo>>(accessMode);
            var owenerProduct = "";
            var productSubtype = P_PRODUCT_SUBTYPE;

            if (line == "2" || topUp == "1" || topUp == "4")
            {
                if (P_OWNER_PRODUCT == "")
                {
                    var q = new GetOwnerProductByNoQuery
                    {
                        No = no
                    };
                    var getCusData = _queryProcessor.Execute(q);
                    owenerProduct = getCusData.Value;
                }
                else
                    owenerProduct = P_OWNER_PRODUCT;
            }
            else
            {
                base._Logger.Info("Begin get Owner Product");

                base._Logger.Info("Prameter for list owner \r\n IsPartner:" + isPartner + "\r\n PartnerName:" + partnerName + "\r\n FBSSAccessModeInfo:" + accessModeModel);
                owenerProduct = _queryProcessor.Execute(new GetMappingSbnOwnerProd
                {
                    IsPartner = isPartner,
                    PartnerName = partnerName,
                    FBSSAccessModeInfo = accessModeModel

                    //FBSSAccessModeInfo = new List<string>().Select(t => new FBSSAccessModeInfo // access mode list ตรงนี้
                    //{
                    //    AccessMode = t,//.ParseEnum<FBSSAccessMode>(),
                    //}).ToList()
                });

                base._Logger.Info("Result Owner :" + owenerProduct);
                base._Logger.Info("End get Owner Product");
            }

            List<PackageModel> list;

            if (line == "2" || topUp == "1" || topUp == "4")
            {
                var query = new GetPackageListBySFFPromoQuery
                {
                    P_SFF_PROMOCODE = P_SFF_PROMOCODE,
                    P_OWNER_PRODUCT = owenerProduct,
                    P_PRODUCT_SUBTYPE = productSubtype,
                    TransactionID = interfaceId
                };
                list = _queryProcessor.Execute(query);
                //var c = _queryProcessor.Execute(query);
                //list= c.Where(a=>a.PRODUCT_SUBTYPE.Contains("PBOX")).ToList();

            }
            else
            {

                #region GetListPackageByServiceQuery_OLd
                //var query = new GetListPackageByServiceQuery
                //{
                //    P_OWNER_PRODUCT = owenerProduct, //P_OWNER_PRODUCT แทนที่ด้วย access mode mapping,
                //    P_PRODUCT_SUBTYPE = P_PRODUCT_SUBTYPE,
                //    P_NETWORK_TYPE = P_NETWORK_TYPE,
                //    P_SERVICE_DAY = P_SERVICE_DAY,
                //    P_PACKAGE_FOR = P_PACKAGE_FOR,
                //    P_PACKAGE_CODE = P_PACKAGE_CODE,
                //    TransactionID = interfaceId,
                //    P_Location_Code = p_location_code,
                //    P_Asc_Code = p_asc_code,
                //    P_Region = p_region,
                //    P_Province = p_province,
                //    P_District = p_district,
                //    P_Sub_District = p_sub_district,
                //    P_Address_Type = p_address_type,
                //    P_Building_Name = p_building_name,
                //    P_Building_No = p_building_no,
                //    P_Partner_Type = p_partner_type,
                //    P_Partner_SubType = p_partner_subtype,
                //    P_Serenade_Flag = _GetListPackageByServiceQuery_Serenade_Flag,//p_Serenade_Flag,
                //    P_Customer_Type = p_Customer_Type,
                //    P_Address_Id = p_Address_Id,
                //    P_Plug_And_Play_Flag = p_Plug_And_Play_Flag
                //};
                #endregion  

                var query = new GetListPackageByServiceQuery
                {
                    P_OWNER_PRODUCT = owenerProduct, //P_OWNER_PRODUCT แทนที่ด้วย access mode mapping,
                    P_PRODUCT_SUBTYPE = _GetListPackageByServiceQuery_P_PRODUCT_SUBTYPE,
                    P_NETWORK_TYPE = _GetListPackageByServiceQuery_P_NETWORK_TYPE,
                    P_SERVICE_DAY = _GetListPackageByServiceQuery_P_SERVICE_DAY,
                    P_PACKAGE_FOR = _GetListPackageByServiceQuery_P_PACKAGE_FOR,
                    P_PACKAGE_CODE = _GetListPackageByServiceQuery_P_PACKAGE_CODE,
                    TransactionID = _GetListPackageByServiceQuery_TransactionID,
                    P_Location_Code = _GetListPackageByServiceQuery_P_Location_Code,
                    P_Asc_Code = _GetListPackageByServiceQuery_P_Asc_Code,
                    P_Region = _GetListPackageByServiceQuery_P_Region,
                    P_Province = _GetListPackageByServiceQuery_P_Province,
                    P_District = _GetListPackageByServiceQuery_P_District,
                    P_Sub_District = _GetListPackageByServiceQuery_P_Sub_District,
                    P_Address_Type = _GetListPackageByServiceQuery_P_Address_Type,
                    P_Building_Name = _GetListPackageByServiceQuery_P_Building_Name,
                    P_Building_No = _GetListPackageByServiceQuery_P_Building_No,
                    P_Partner_Type = _GetListPackageByServiceQuery_P_Partner_Type,
                    P_Partner_SubType = _GetListPackageByServiceQuery_P_Partner_SubType,
                    P_Serenade_Flag = _GetListPackageByServiceQuery_P_Serenade_Flag,//p_Serenade_Flag,
                    P_Customer_Type = _GetListPackageByServiceQuery_P_Customer_Type,
                    P_Address_Id = _GetListPackageByServiceQuery_P_Address_Id,
                    P_Plug_And_Play_Flag = _GetListPackageByServiceQuery_P_Plug_And_Play_Flag,
                    FullUrl = FullUrl,
                    P_Rental_Flag = _GetListPackageByServiceQuery_P_Rental_Flag,
                    P_Customer_subtype = _GetListPackageByServiceQuery_P_Customer_subtype
                };
                list = _queryProcessor.Execute(query);
            }

            //int _P00531 = 0;
            //int _P00532 = 0;
            //for(int i = 0 ; i < list.Count ; i++)
            //{
            //    if (list[i].MAPPING_PRODUCT == "P00531")
            //    {
            //        _P00531++;
            //    }
            //    if (list[i].MAPPING_PRODUCT == "P00532")
            //    {
            //        _P00532++;
            //    }
            //}
            string ShowInstall = "";
            string TecnologyType = "";
            var listAuto = new List<PackageModel>();

            if (list != null && list.Count > 0)
            {
                //TecnologyType = list.Select(p => new { p.PRODUCT_SUBTYPE, p.PACKAGE_TYPE }).Where(p => p.PACKAGE_TYPE == "Main").FirstOrDefault().PRODUCT_SUBTYPE;

                listAuto = list.Where(p => p.PACKAGE_TYPE != "Main" && p.AUTO_MAPPING == "A").ToList();
                list = list.Where(p => p.AUTO_MAPPING != "A" || p.PACKAGE_TYPE == "Main").ToList();
                TecnologyType = list.Select(p => new { p.PRODUCT_SUBTYPE, p.PACKAGE_TYPE }).Where(p => p.PACKAGE_TYPE == "Main").FirstOrDefault().PRODUCT_SUBTYPE;
            }
            var LovConfig = GetLovConfigOptionInstallData();
            var LovConfigShowOptionInstall = GetLovConfigShowOptionInstall();

            if (LovConfig != null && LovConfig.Count > 0)
            {
                if (p_InstallationType == "0")
                {
                    var LovConfigStr = LovConfig.Where(p => p.Text == TecnologyType && p.DefaultValue == "1").ToList();
                    if (LovConfigStr != null && LovConfigStr.Count > 0)
                    {
                        var LovConfigStrTmp = LovConfigStr.FirstOrDefault();
                        if (LovConfigStrTmp != null)
                        {
                            string[] codesarray = new string[2];
                            codesarray[0] = LovConfigStrTmp.LovValue1;
                            codesarray[1] = LovConfigStrTmp.LovValue2;
                            if (LovConfigStrTmp.LovValue1 != "" && LovConfigStrTmp.LovValue2 != "")
                            {
                                list = list.Where(p => !codesarray.Contains(p.SFF_PROMOTION_CODE)).ToList();
                                ShowInstall = "True";
                            }
                        }
                    }
                }
                else
                {
                    var LovConfigStr = LovConfig.Where(p => p.Text == TecnologyType && p.DefaultValue == "0");
                    if (LovConfigStr != null)
                    {
                        var LovConfigStrTmp = LovConfigStr.FirstOrDefault();
                        if (LovConfigStrTmp != null)
                        {
                            string codes = LovConfigStrTmp.LovValue1 + "," + LovConfigStrTmp.LovValue2;
                            string[] codesarray = new string[2];
                            codesarray[0] = LovConfigStrTmp.LovValue1;
                            codesarray[1] = LovConfigStrTmp.LovValue2;
                            if (LovConfigStrTmp.LovValue1 != "" && LovConfigStrTmp.LovValue2 != "")
                            {
                                list = list.Where(p => !codesarray.Contains(p.SFF_PROMOTION_CODE)).ToList();
                                ShowInstall = "True";
                            }
                        }
                    }
                }
            }

            if (!list.Any())
            {

                base._Logger.Info("Package Model Is Null");
                //jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonResult.Data = new List<PackageModel>();

                return jsonResult;
            }

            if (line == "" || line == "3" || line == "6")
            {
                // list = list.Where(p => p.PACKAGE_TYPE != "Bundle" && !p.PACKAGE_GROUP.Contains("Bundle")).ToList();
            }
            else
            {
                int componentBundle = 0;

                if (base.CurrentUser.ComponentModel != null && base.CurrentUser.Groups != null)
                    componentBundle = base.CurrentUser.ComponentModel.Where(w => w.ComponentName == "FBBOR002_REGISTER_BUNDLE"
                    && base.CurrentUser.Groups.Contains(w.GroupID)).Count();

                if (componentBundle == 0)
                    list = list.Where(p => p.PACKAGE_TYPE != "Bundle" && !p.PACKAGE_GROUP.Contains("Bundle")).ToList();
            }


            //if (P_OWNER_PRODUCT != "WireBB")
            //{
            //var top = new List<string>();
            //var bottom = new List<string>();
            var listGroup = new List<SGroup>();

            var top = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && !p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });

            var bottom = list.Where(p => p.PACKAGE_GROUP != null && p.PACKAGE_GROUP != "" && p.PACKAGE_GROUP.Contains("Incredible")
                && p.PRODUCT_SUBTYPE != "VOIP" && p.PRODUCT_SUBTYPE != "Equipment" && p.PACKAGE_TYPE != "Ontop" && p.PRODUCT_SUBTYPE != "PBOX"
                && p.PACKAGE_TYPE != "Ontop Special")
                .GroupBy(g => new { g.PACKAGE_GROUP, g.PRODUCT_SUBTYPE, g.PACKAGE_TYPE }).Select(s => new { s.Key.PACKAGE_GROUP, s.Key.PRODUCT_SUBTYPE, s.Key.PACKAGE_TYPE });


            if (top.Any())
                foreach (var a in top)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            if (bottom.Any())
                foreach (var a in bottom)
                    listGroup.Add(new SGroup { PACKAGE_GROUP = a.PACKAGE_GROUP, PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE, PACKAGE_TYPE = a.PACKAGE_TYPE });

            list[0].S_GROUP = listGroup;
            list[0].TEMP_IA = ShowInstall;
            if (listAuto.Any() && listAuto.Count > 0)
            {
                List<PackageAutoModel> packageAutoDatas = new List<PackageAutoModel>();
                foreach (var item in listAuto)
                {
                    PackageAutoModel packageAutoData = new PackageAutoModel
                    {
                        ACCESS_MODE = item.ACCESS_MODE,
                        DISCOUNT_INITIATION_CHARGE = item.DISCOUNT_INITIATION_CHARGE,
                        DOWNLOAD_SPEED = item.DOWNLOAD_SPEED,
                        INITIATION_CHARGE = item.INITIATION_CHARGE,
                        MAPPING_CODE = item.MAPPING_CODE,
                        MAPPING_PRODUCT = item.MAPPING_PRODUCT,
                        OWNER_PRODUCT = item.OWNER_PRODUCT,
                        PACKAGE_CODE = item.PACKAGE_CODE,
                        PACKAGE_GROUP = item.PACKAGE_GROUP,
                        PACKAGE_NAME = item.PACKAGE_NAME,
                        PACKAGE_TYPE = item.PACKAGE_TYPE,
                        PRE_INITIATION_CHARGE = item.PRE_INITIATION_CHARGE,
                        PRE_RECURRING_CHARGE = item.PRE_RECURRING_CHARGE,
                        PRODUCT_SUBTYPE = item.PRODUCT_SUBTYPE,
                        PRODUCT_SUBTYPE3 = item.PRODUCT_SUBTYPE3,
                        RECURRING_CHARGE = item.RECURRING_CHARGE,
                        SERVICE_CODE = item.SERVICE_CODE,
                        SFF_PROMOTION_BILL_ENG = item.SFF_PROMOTION_BILL_ENG,
                        SFF_PROMOTION_BILL_THA = item.SFF_PROMOTION_BILL_THA,
                        SFF_PROMOTION_CODE = item.SFF_PROMOTION_CODE,
                        TECHNOLOGY = item.TECHNOLOGY,
                        UPLOAD_SPEED = item.UPLOAD_SPEED,
                        AUTO_MAPPING = item.AUTO_MAPPING,
                        DISPLAY_FLAG = item.DISPLAY_FLAG,
                        DISPLAY_SEQ = item.DISPLAY_SEQ,
                        DISCOUNT_TYPE = item.DISCOUNT_TYPE,
                        DISCOUNT_VALUE = item.DISCOUNT_VALUE,
                        DISCOUNT_DAY = item.DISCOUNT_DAY

                    };
                    packageAutoDatas.Add(packageAutoData);

                }
                list[0].PACKAGE_AUTO = packageAutoDatas;
            }
            //}

            //TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            ////Console.Out.WriteLine("GetListPackageService elasped time is " + elapsedTime);

            jsonResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonResult.Data = list;
            int aa = list.Count;
            return jsonResult;
        }

        public JsonResult GetpackageGroup(string Package)
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "VOUCHER_PACKAGE_GROUP" && l.Type == "FBB_CONSTANT" && l.LovValue1 == Package).ToList();

            var Name = config.Select(i => i.LovValue1).FirstOrDefault();

            return Json(Name, JsonRequestBehavior.AllowGet);
        }

        public List<LovValueModel> GetLovConfigOptionInstall(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "OPTION_INSTALL").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = base.LovData.Where(l => l.Type == "OPTION_INSTALL").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "OPTION_INSTALL")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }

                return config;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public List<LovValueModel> GetLovConfigOptionInstallData()
        {
            var screenData = GetLovConfigOptionInstall(WebConstants.LovConfigName.DisplayPackagePageCode);
            return screenData;
        }

        public List<LovValueModel> GetLovConfigShowOptionInstall()
        {
            try
            {
                List<LovValueModel> config = null;

                config = base.LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == "PACKAGE_GROUP")).ToList();

                return config;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

    }
}