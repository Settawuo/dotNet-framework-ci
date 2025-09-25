using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBContract.WebService;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public class ChangePromotionController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<CreateOrderChangePromotionCommand> _crateOrderChangePromotionCommand;

        public ChangePromotionController(IQueryProcessor queryProcessor,
             ICommandHandler<InterfaceLogCommand> intfLogCommand,
             ICommandHandler<CreateOrderChangePromotionCommand> crateOrderChangePromotionCommand,
             ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _crateOrderChangePromotionCommand = crateOrderChangePromotionCommand;
            base.Logger = logger;
        }

        public ActionResult Index()
        {
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            return View();
        }

        public List<LovScreenValueModel> GetChangePromotionScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.ChangePromotionPageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = base.LovData.Where(l => l.Type == "SCREEN").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }
                //config = config.Where(a => a.Name == "L_DETAIL_DISCOUNT_SINGLE_BILL_1").ToList();
                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
                    }).ToList();
                }

                return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
            }
        }

        public string CheckChangePackageOntop(string non_mobile_no, string air_change_old_package_array, string air_change_new_package_array)
        {
            List<AirChangePackage> air_old_list = new List<AirChangePackage>();
            var js_old = new JavaScriptSerializer();
            var deserializedAirOldItems = (object[])js_old.DeserializeObject(air_change_old_package_array);
            if (deserializedAirOldItems != null)
            {
                foreach (Dictionary<string, object> oldItem in deserializedAirOldItems)
                {
                    SerializeAirchangePromotionJSonModel stmpOld = new SerializeAirchangePromotionJSonModel(oldItem);
                    AirChangePackage air_old = new AirChangePackage()
                    {
                        sff_promotion_code = stmpOld.SFF_PROMOTION_CODE,
                        startdt = stmpOld.startDt,
                        enddt = stmpOld.endDt,
                        product_seq = stmpOld.PRODUCT_SEQ
                    };
                    air_old_list.Add(air_old);
                }
            }

            List<AirChangePackage> air_new_list = new List<AirChangePackage>();
            var js_new = new JavaScriptSerializer();
            var deserializedAirNewItems = (object[])js_new.DeserializeObject(air_change_new_package_array);
            if (deserializedAirNewItems != null)
            {
                foreach (Dictionary<string, object> newItem in deserializedAirNewItems)
                {
                    SerializeAirchangePromotionJSonModel stmp_new = new SerializeAirchangePromotionJSonModel(newItem);
                    AirChangePackage air_new = new AirChangePackage()
                    {
                        sff_promotion_code = stmp_new.SFF_PROMOTION_CODE,
                        startdt = stmp_new.startDt,
                        enddt = stmp_new.endDt,
                        product_seq = ""
                    };
                    air_new_list.Add(air_new);
                }
            }

            try
            {
                var query = new CheckChangePackageOntopQuery()
                {
                    p_non_mobile_no = non_mobile_no,
                    AirChangeOldPackageArray = air_old_list,
                    AirChangeNewPackageArray = air_new_list
                };

                var result = _queryProcessor.Execute(query);

                if (result.o_result == "Success")
                {
                    return string.Format("Success");
                }
                else
                {
                    return string.Format("Fail");
                }

            }
            catch (Exception ex)

            {
                Logger.Error("Error when call CheckChangePackageOntop in ChangePromotionController : " + ex.Message);
                return string.Format("Fail");
            }

        }

        public JsonResult GetTablePersonalPackageInfo(string non_mobile_no, string relate_mobile, string owner_product, string serenate_flag, string promocodeCurrent, string checkHavePlayBox, string air_change_package_array)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var config = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "AIS_PLAY_BOX").Select(t => t.LovValue1).ToList();
            List<ListPackageDisplayModel> result = new List<ListPackageDisplayModel>();
            List<ServicePlayboxModel> listServicePB = new List<ServicePlayboxModel>();
            int subseq = 1;

            var js = new JavaScriptSerializer();
            var deserializedAirItems = (object[])js.DeserializeObject(air_change_package_array);
            if (deserializedAirItems != null)
            {
                evOMQueryListServiceAndPromotionByPackageTypeQuery listServiceAndPromotionByPackageTypeQuery = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                {
                    mobileNo = non_mobile_no,
                    idCard = "",
                    FullUrl = FullUrl
                };
                evOMQueryListServiceAndPromotionByPackageTypeModel listServiceAndPromotionByPackageTypeData = new evOMQueryListServiceAndPromotionByPackageTypeModel();

                listServiceAndPromotionByPackageTypeData = _queryProcessor.Execute(listServiceAndPromotionByPackageTypeQuery);

                if (listServiceAndPromotionByPackageTypeData != null
                    && listServiceAndPromotionByPackageTypeData.gridId.ToSafeString() != ""
                    && listServiceAndPromotionByPackageTypeData.access_mode.ToSafeString() == "WTTx") //R22.04 WTTx
                {
                    listServiceAndPromotionByPackageTypeData.addressId = listServiceAndPromotionByPackageTypeData.gridId.ToSafeString();
                }

                if (listServiceAndPromotionByPackageTypeData != null
                    && listServiceAndPromotionByPackageTypeData.addressId.ToSafeString() != ""
                    && listServiceAndPromotionByPackageTypeData.access_mode.ToSafeString() != "")
                {
                    //R23.1                            
                    var noDispTextList = LovData.Where(item => item.Name == "W_CHANGE_PRO_NODISP" && item.LovValue5 == "FBBOR016").Select(s => s.LovValue1.ToUpper()).ToList();

                    foreach (Dictionary<string, object> newItem in deserializedAirItems)
                    {
                        // Update 17.3
                        GetPackageSequenceModel seqResult = new GetPackageSequenceModel();
                        seqResult.PACKAGE_SEQ = 0;
                        SerializeAirPackageDisplayJSonModel stmp = new SerializeAirPackageDisplayJSonModel(newItem);
                        if (stmp.SFF_PROMOTION_CODE.ToSafeString() != "")
                        {
                            GetPackageSequenceQuery query = new GetPackageSequenceQuery()
                            {
                                P_FIBRENET_ID = non_mobile_no,
                                P_ADDRESS_ID = listServiceAndPromotionByPackageTypeData.addressId,
                                P_ACCESS_MODE = listServiceAndPromotionByPackageTypeData.access_mode,
                                P_PROMOTION_CODE = stmp.SFF_PROMOTION_CODE
                            };
                            seqResult = _queryProcessor.Execute(query);
                        }
                        if (seqResult.PACKAGE_SEQ == 0)
                        {
                            seqResult.PACKAGE_SEQ = 4;
                            seqResult.PACKAGE_SUBSEQ = subseq.ToSafeString();
                            subseq++;
                        }



                        if (stmp.promotionName != null && stmp.promotionName != "")
                        {
                            //R23.1     
                            var promotionName = stmp.promotionName.ToUpper();


                            var checkNoDisp = noDispTextList.Where(stringToCheck => promotionName.Contains(stringToCheck)).ToList();

                            if (noDispTextList.Count > 0 && checkNoDisp.Count > 0 && seqResult.PACKAGE_TYPE_DESC?.ToUpper() != "MAIN")
                            {
                                continue;
                            }


                            ListPackageDisplayModel displayPackage = new ListPackageDisplayModel()
                            {
                                non_mobile_no = non_mobile_no,
                                ref_row_id = null,
                                package_seq = seqResult.PACKAGE_SEQ,
                                package_subseq = seqResult.PACKAGE_SUBSEQ,
                                font_type = "B",
                                line_seq = "".ToSafeDecimal(),

                                //package_display_th = stmp.promotionName + " (" + stmp.startDt + " - " + stmp.endDt + ")",
                                //package_display_en = stmp.promotionName + " (" + stmp.startDt + " - " + stmp.endDt + ")",
                                //R23.1 
                                package_display_th = "(" + stmp.startDt + " - " + stmp.endDt + ")",
                                //R23.1 
                                package_display_en = "(" + stmp.startDt + " - " + stmp.endDt + ")",

                                sff_promotion_code = stmp.SFF_PROMOTION_CODE,
                                startdt = stmp.startDt,
                                enddt = stmp.endDt,
                                package_description_th = stmp.descThai.ToSafeString(),
                                package_description_en = stmp.descEng.ToSafeString(),
                                package_type_desc = seqResult.PACKAGE_TYPE_DESC,
                                product_subtype = seqResult.PRODUCT_SUBTYPE,
                                product_subtype1 = seqResult.PRODUCT_SUBTYPE1,
                                sub_seq = seqResult.SUB_SEQ
                            };
                            result.Add(displayPackage);
                        }
                    }
                }
            }

            result = result.Distinct().ToList();
            result.Sort((a, b) =>
            {
                // compare b to a to get descending order
                int r = a.package_seq.CompareTo(b.package_seq);
                if (r == 0)
                {
                    // if categories are the same, sort by product
                    r = a.package_subseq.CompareTo(b.package_subseq);
                    if (r == 0)
                    {
                        if (a.startdt == "" && a.enddt == "" && b.startdt == "" && b.enddt == "")
                        {
                            r = a.sub_seq.ToSafeDecimal().CompareTo(b.sub_seq.ToSafeDecimal());
                        }
                        else if (a.startdt == "" && a.enddt == "")
                        {
                            a.package_display_th = "";
                            a.package_display_en = "";
                            a.package_description_th = "";
                            a.package_description_en = "";
                        }
                        else if (b.startdt == "" && b.enddt == "")
                        {
                            b.package_display_th = "";
                            b.package_display_en = "";
                            b.package_description_th = "";
                            b.package_description_en = "";
                        }
                        else if (a.startdt.ToDateTime() > b.startdt.ToDateTime())
                        {
                            r = 1;
                        }
                        else if (a.startdt.ToDateTime() < b.startdt.ToDateTime())
                        {
                            r = -1;
                        }
                    }
                }
                return r;
            });

            int PackageSeq4 = 1;
            foreach (var item in result.Where(x => x.package_seq == 4).OrderBy(x => x.sff_promotion_code))
            {
                string PackageSeq4Str = "4" + PackageSeq4;
                item.sub_seq = int.Parse(PackageSeq4Str);
                item.package_subseq = "41";
                PackageSeq4++;
            }

            var orderByResult = result.OrderBy(c => c.package_subseq).ThenBy(c => c.package_seq).ToList();
            result = orderByResult;

            // call add mesh service
            evOMQueryContractModel tevOMQueryContractModel = new evOMQueryContractModel();
            evOMQueryContractQuery tevOMQueryContractQuery = new evOMQueryContractQuery()
            {
                inMobileNo = non_mobile_no,
                FullUrl = FullUrl,
                ClientIP = ""
            };

            tevOMQueryContractModel = _queryProcessor.Execute(tevOMQueryContractQuery);


            if (tevOMQueryContractModel != null && tevOMQueryContractModel.evOMQueryContractDatas != null
                && tevOMQueryContractModel.evOMQueryContractDatas.Count > 0)
            {
                GetDisplayContractModel displayContractModel = new GetDisplayContractModel();

                foreach (var item in tevOMQueryContractModel.evOMQueryContractDatas)
                {
                    int penaltyLength = item.penalty.Length;
                    if (penaltyLength > 3)
                    {
                        item.penalty = item.penalty.Substring(0, penaltyLength - 3);
                    }
                    else
                    {
                        item.penalty = "0";
                    }
                }

                List<FbbDisplayData> FbbDisplayDatas = tevOMQueryContractModel.evOMQueryContractDatas.Select(p => new FbbDisplayData()
                {
                    CONTRACTNAME = "ContractData",
                    CONTRACTNO = p.contractNo.ToSafeString(),
                    DURATION = p.duration.ToSafeString(),
                    PENALTY = p.penalty.ToSafeString(),
                    TDMCONTRACTID = p.tdmContractId.ToSafeString()
                }).ToList();

                string P_LANGUAGE = "E";
                if (GetCurrentCulture().IsThaiCulture())
                {
                    P_LANGUAGE = "T";
                }

                GetDisplayContractQuery getDisplayContractQuery = new GetDisplayContractQuery()
                {
                    P_FIBRENET_ID = non_mobile_no,
                    P_LANGUAGE = P_LANGUAGE,
                    FbbDisplayDatas = FbbDisplayDatas,
                    FullUrl = FullUrl
                };

                displayContractModel = _queryProcessor.Execute(getDisplayContractQuery);
                if (displayContractModel != null && displayContractModel.DisplayContractDatas != null
                    && displayContractModel.DisplayContractDatas.Count > 0)
                {
                    foreach (var item in displayContractModel.DisplayContractDatas)
                    {
                        ListPackageDisplayModel displayPackage = new ListPackageDisplayModel()
                        {
                            non_mobile_no = item.FIBRENETID.ToSafeString(),
                            ref_row_id = "5",
                            package_seq = 5,
                            package_subseq = "51",
                            font_type = "",
                            line_seq = 0,
                            package_display_th = "",
                            package_display_en = "",
                            sff_promotion_code = "",
                            startdt = "",
                            enddt = "",
                            package_description_th = item.PACKAGE_DISPLAY,
                            package_description_en = item.PACKAGE_DISPLAY,
                            package_type_desc = "",
                            product_subtype = "",
                            product_subtype1 = "",
                            sub_seq = int.Parse(item.DISPLAY_SEQ.ToSafeString() != "" ? item.DISPLAY_SEQ.ToSafeString() : "0")
                        };
                        result.Add(displayPackage);
                    }
                }
            }

            // end call add pbox service

            StringBuilder rowcontent = new StringBuilder();
            var tblHtml = "<table class='border_bottom_gray' border='0' cellpadding='0' cellspacing='0' width='100%'>" +
                                "<tbody>{0}</tbody></table>";
            try
            {
                #region Get IP Address Interface Log (Update 17.2)

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                #endregion

                if (result != null)
                {
                    if (result.Any())
                    {
                        var grpHeader = result.Select(x => x.package_seq).Distinct().ToList();
                        string ref_row_id = result.Select(x => x.ref_row_id).Distinct().FirstOrDefault();
                        tblHtml += "<input type='hidden' id='ref_row_id' value ='" + ref_row_id + "' />";

                        foreach (var header in grpHeader)
                        {
                            bool isFirstRow = true;
                            var subGrp = result.Where(x => x.package_seq == header).OrderBy(x => x.package_subseq).Select(x => x.package_subseq).Distinct().ToList();
                            int subGrpRowSpan = subGrp.Count();
                            foreach (var tmp in subGrp)
                            {
                                string rowtxt = "";
                                if (header == 99)
                                {
                                    rowtxt = "<tr id='PersonalOntop' style='display: none'>{0}{1}</tr>";
                                    string cellheadertxt = "";

                                    string celltxt = "<td>{0}</td>";
                                    string htmltxt = "";
                                    foreach (var detail in result.Where(x => x.package_seq == header).OrderBy(x => x.line_seq))
                                    {
                                        string txtcontent;
                                        if (GetCurrentCulture().IsThaiCulture()) txtcontent = detail.package_display_th;
                                        else txtcontent = detail.package_display_en;

                                        htmltxt += string.Format("<div>{0}</div>", txtcontent);
                                    }
                                    celltxt = string.Format(celltxt, htmltxt);
                                    rowcontent.Append(string.Format(rowtxt, cellheadertxt, celltxt));
                                }
                                else
                                {
                                    rowtxt = "<tr>{0}{1}</tr>";
                                    string cellheadertxt = "";
                                    if (isFirstRow)
                                    {
                                        cellheadertxt = "<td rowspan='{0}' class='border_bottom_gray border_right_gray' align='center' width='15%'>" +
                                            "<h3 class='font_green font18'>{1}</h3></td>";
                                        isFirstRow = false;

                                        string lblheader = "";
                                        var lovherder = base.LovData.Where(x => x.Name == "L_PROFILE_PACKAGE_GROUP" && x.LovValue5 == header.ToString());
                                        if (lovherder.Any())
                                        {
                                            if (GetCurrentCulture().IsThaiCulture()) lblheader = lovherder.FirstOrDefault().LovValue1;
                                            else lblheader = lovherder.FirstOrDefault().LovValue2;
                                        }

                                        cellheadertxt = string.Format(cellheadertxt, subGrpRowSpan.ToString(), lblheader);
                                    }
                                    string celltxt = "<td class='border_bottom_gray'><div class='box_padding15 font_14'>{0}</div></td>";
                                    string htmltxt = "";
                                    foreach (var detail in result.Where(x => x.package_subseq == tmp.ToString()).OrderBy(x => x.sub_seq))
                                    {
                                        string txtcontent;
                                        if (GetCurrentCulture().IsThaiCulture()) txtcontent = detail.package_display_th;
                                        else txtcontent = detail.package_display_en;

                                        string txtformat = "";


                                        if (detail.package_description_th != "" || detail.package_description_en != "")
                                        {
                                            string txtcontent2;
                                            if (GetCurrentCulture().IsThaiCulture()) txtcontent2 = detail.package_description_th;
                                            else txtcontent2 = detail.package_description_en;

                                            htmltxt += string.Format("<h4 class='{0}'>{1}</h4>", "", txtcontent2.Replace("|", "<br>"));
                                        }

                                        //R23.1s
                                        if (detail.font_type == "B") txtformat = "font_green";

                                        htmltxt += string.Format("<h4 class='{0}'>{1}</h4>", txtformat, txtcontent.Replace("|", "<br>"));
                                    }
                                    celltxt = string.Format(celltxt, htmltxt);
                                    //R23.1
                                    rowcontent.Append(string.Format(rowtxt, cellheadertxt, celltxt));
                                }
                            }
                        }
                    }
                }
                tblHtml = string.Format(tblHtml, rowcontent.ToString());


                return Json(new { tblHtml = tblHtml.ToString(), result = result }, JsonRequestBehavior.AllowGet);
                //return tblHtml;
            }
            catch (Exception ex)
            {
                Logger.Error("Error when call GetPersonalDisplay in ChangePromotionController : " + ex.InnerException);
            }

            return Json(new { tblHtml = string.Format(tblHtml, rowcontent.ToString()), result = "" }, JsonRequestBehavior.AllowGet);
            //return string.Format(tblHtml, rowcontent.ToString());
        }

        public List<ListPackageDisplayModel> getPlayboxDiscount(string SFF_PROMOTION_CODE, string PRODUCT_SUBTYPE, string OWNER_PRODUCT,
            string NON_MOBILE_NO, string ADDRESS_ID, string ACCESS_MODE)
        {
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var config = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "AIS_PLAY_BOX").Select(t => t.LovValue1).ToList();
            List<PackageModel> PackageListBySFFPromos = new List<PackageModel>();
            List<ListPackageDisplayModel> result = new List<ListPackageDisplayModel>();
            PackageListBySFFPromos = GetPackageListBySFFPromoForChangPro(SFF_PROMOTION_CODE.ToSafeString(),
                        PRODUCT_SUBTYPE.ToSafeString(), OWNER_PRODUCT.ToSafeString(),
                        "", NON_MOBILE_NO);

            var PackageListBySFFPromo = PackageListBySFFPromos.Where(t => config.Contains(t.SERVICE_CODE.ToSafeString()) && t.PRODUCT_SUBTYPE3 == "PBOX").ToList();

            if (PackageListBySFFPromo != null && PackageListBySFFPromo.Count > 0)
            {

                foreach (PackageModel serviceCodeList in PackageListBySFFPromo)
                {
                    //var servicePlayboxResult = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "AIS_PLAY_BOX_CHANGE_PRO" && l.LovValue1 == SFF_PROMOTION_CODE).Select(t => t.LovValue1).FirstOrDefault();
                    var query2 = new GetServicePlayboxQuery()
                    {
                        SERVICE_CODE = serviceCodeList.SERVICE_CODE,
                        SFF_PROMOTION_CODE = SFF_PROMOTION_CODE
                    };
                    var servicePlayboxResult = _queryProcessor.Execute(query2);

                    var query3 = new GetPackageSequenceQuery()
                    {
                        P_FIBRENET_ID = NON_MOBILE_NO,
                        P_ADDRESS_ID = ADDRESS_ID,
                        P_ACCESS_MODE = ACCESS_MODE,
                        P_PROMOTION_CODE = servicePlayboxResult.SFF_PROMOTION_CODE
                    };
                    var seqResultPB = _queryProcessor.Execute(query3);
                    if (seqResultPB.PACKAGE_SEQ != 0)
                    {
                        ListPackageDisplayModel displayPackagePB = new ListPackageDisplayModel()
                        {
                            non_mobile_no = NON_MOBILE_NO,
                            ref_row_id = null,
                            package_seq = seqResultPB.PACKAGE_SEQ,
                            package_subseq = seqResultPB.PACKAGE_SUBSEQ,
                            font_type = "B",
                            line_seq = "".ToSafeDecimal(),
                            package_display_th = seqResultPB.PACKAGE_DISPLAY_THA,
                            package_display_en = seqResultPB.PACKAGE_DISPLAY_ENG,
                            sff_promotion_code = servicePlayboxResult.SFF_PROMOTION_CODE,
                            startdt = "",
                            enddt = "",
                            package_description_th = seqResultPB.DESCTHAI.ToSafeString(),
                            package_description_en = seqResultPB.DESCENG.ToSafeString(),
                            package_type_desc = seqResultPB.PACKAGE_TYPE_DESC,
                            product_subtype = seqResultPB.PRODUCT_SUBTYPE,
                            sub_seq = seqResultPB.SUB_SEQ
                        };

                        result.Add(displayPackagePB);
                    }
                }

            }

            return result;
        }
        public string GetTableChangePackageInfo(string non_mobile_no, string relate_mobile, string ref_row_id, string owner_product, string serenate_flag, string checkHavePlayBox, string oldPackage, string air_change_package_array, string ShowPlaybox)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var config = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "AIS_PLAY_BOX").Select(t => t.LovValue1).ToList();
            List<ListPackageDisplayModel> result = new List<ListPackageDisplayModel>();

            evOMQueryListServiceAndPromotionByPackageTypeQuery listServiceAndPromotionByPackageTypeQuery = new evOMQueryListServiceAndPromotionByPackageTypeQuery
            {
                mobileNo = non_mobile_no,
                idCard = "",
                FullUrl = FullUrl
            };
            evOMQueryListServiceAndPromotionByPackageTypeModel listServiceAndPromotionByPackageTypeData = new evOMQueryListServiceAndPromotionByPackageTypeModel();

            listServiceAndPromotionByPackageTypeData = _queryProcessor.Execute(listServiceAndPromotionByPackageTypeQuery);

            if (listServiceAndPromotionByPackageTypeData != null
                    && listServiceAndPromotionByPackageTypeData.gridId.ToSafeString() != ""
                    && listServiceAndPromotionByPackageTypeData.access_mode.ToSafeString() == "WTTx") //R22.04 WTTx
            {
                listServiceAndPromotionByPackageTypeData.addressId = listServiceAndPromotionByPackageTypeData.gridId.ToSafeString();
            }

            if (listServiceAndPromotionByPackageTypeData != null
                    && listServiceAndPromotionByPackageTypeData.addressId.ToSafeString() != ""
                    && listServiceAndPromotionByPackageTypeData.access_mode.ToSafeString() != "")
            {
                var js = new JavaScriptSerializer();
                var deserializedAirItems = (object[])js.DeserializeObject(air_change_package_array);
                if (deserializedAirItems != null)
                {
                    foreach (Dictionary<string, object> newItem in deserializedAirItems)
                    {
                        // Update 17.3
                        SerializeAirPackageDisplayJSonModel stmp = new SerializeAirPackageDisplayJSonModel(newItem);
                        if (stmp.SFF_PROMOTION_CODE != null && stmp.SFF_PROMOTION_CODE != "")
                        {

                            var query = new GetPackageSequenceQuery()
                            {
                                P_FIBRENET_ID = non_mobile_no,
                                P_ADDRESS_ID = listServiceAndPromotionByPackageTypeData.addressId,
                                P_ACCESS_MODE = listServiceAndPromotionByPackageTypeData.access_mode,
                                P_PROMOTION_CODE = stmp.SFF_PROMOTION_CODE
                            };
                            var seqResult = _queryProcessor.Execute(query);
                            if (seqResult.PACKAGE_SEQ != 0)
                            {
                                //if (config != null && config.Count > 0 && checkHavePlayBox == "Y")
                                if (config != null && config.Count > 0)
                                {
                                    List<ListPackageDisplayModel> playboxDiscounts = getPlayboxDiscount(stmp.SFF_PROMOTION_CODE, seqResult.PRODUCT_SUBTYPE, owner_product, non_mobile_no,
                                                                                                        listServiceAndPromotionByPackageTypeData.addressId,
                                                                                                        listServiceAndPromotionByPackageTypeData.access_mode);

                                    foreach (ListPackageDisplayModel playboxDiscount in playboxDiscounts)
                                    {
                                        result.Add(playboxDiscount);
                                    }
                                }
                                ListPackageDisplayModel displayPackage = new ListPackageDisplayModel()
                                {
                                    non_mobile_no = non_mobile_no,
                                    ref_row_id = null,
                                    package_seq = seqResult.PACKAGE_SEQ,
                                    package_subseq = seqResult.PACKAGE_SUBSEQ,
                                    font_type = "B",
                                    line_seq = "".ToSafeDecimal(),
                                    package_display_th = stmp.promotionName,
                                    package_display_en = stmp.promotionName,
                                    sff_promotion_code = stmp.SFF_PROMOTION_CODE,
                                    startdt = stmp.startDt,
                                    enddt = stmp.endDt,
                                    package_description_th = stmp.descThai.ToSafeString(),
                                    package_description_en = stmp.descEng.ToSafeString(),
                                    package_type_desc = seqResult.PACKAGE_TYPE_DESC,
                                    product_subtype = seqResult.PRODUCT_SUBTYPE,
                                    product_subtype1 = seqResult.PRODUCT_SUBTYPE1
                                };
                                result.Add(displayPackage);
                            }
                        }



                    }
                }
            }

            result.Sort((a, b) =>
            {
                // compare b to a to get descending order
                int r = a.package_seq.CompareTo(b.package_seq);
                if (r == 0)
                {
                    // if categories are the same, sort by product
                    r = a.package_subseq.CompareTo(b.package_subseq);
                }
                return r;
            });

            StringBuilder rowcontent = new StringBuilder();
            try
            {
                #region Get IP Address Interface Log (Update 17.2)

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                #endregion


                if (result != null)
                {
                    if (result.Any())
                    {
                        if (ShowPlaybox == "Y")
                        {
                            ListPackageDisplayModel tmp = new ListPackageDisplayModel()
                            {
                                package_seq = 2,
                                package_subseq = "pb"
                            };
                            result.Add(tmp);
                        }
                        var grpHeader = result.Select(x => x.package_seq).Distinct().ToList();
                        foreach (var header in grpHeader)
                        {
                            bool isFirstRow = true;
                            var subGrp = result.Where(x => x.package_seq == header).Select(x => x.package_subseq).Distinct().ToList();
                            int subGrpRowSpan = subGrp.Count();
                            foreach (var tmp in subGrp)
                            {
                                string rowtxt = "";
                                if (header == 99)
                                {
                                    rowtxt = "<tr id='PersonalOntopNew' style='display: none'>{0}{1}</tr>";
                                    string cellheadertxt = "";

                                    string celltxt = "<td>{0}</td>";
                                    string htmltxt = "";
                                    foreach (var detail in result.Where(x => x.package_seq == header).OrderBy(x => x.line_seq))
                                    {
                                        string txtcontent;
                                        if (GetCurrentCulture().IsThaiCulture()) txtcontent = detail.package_display_th;
                                        else txtcontent = detail.package_display_en;

                                        htmltxt += string.Format("<div>{0}</div>", txtcontent);
                                    }
                                    celltxt = string.Format(celltxt, htmltxt);
                                    rowcontent.Append(string.Format(rowtxt, cellheadertxt, celltxt));
                                }
                                else
                                {

                                    rowtxt = "<tr>{0}{1}</tr>";
                                    string cellheadertxt = "";
                                    if (isFirstRow)
                                    {
                                        cellheadertxt = "<td rowspan='{0}' class='border_bottom_gray border_right_gray' align='center' width='15%'>" +
                                            "<h3 class='font_green font18'>{1}</h3></td>";
                                        isFirstRow = false;

                                        string lblheader = "";
                                        var lovherder = base.LovData.Where(x => x.Name == "L_PROFILE_PACKAGE_GROUP" && x.LovValue5 == header.ToString());
                                        if (lovherder.Any())
                                        {
                                            if (GetCurrentCulture().IsThaiCulture()) lblheader = lovherder.FirstOrDefault().LovValue1;
                                            else lblheader = lovherder.FirstOrDefault().LovValue2;
                                        }

                                        cellheadertxt = string.Format(cellheadertxt, subGrpRowSpan.ToString(), lblheader);
                                    }
                                    string celltxt = "<td class='border_bottom_gray'><div class='box_padding15 font_14'>{0}</div></td>";
                                    string htmltxt = "";
                                    if (ShowPlaybox == "Y" && tmp.ToString() == "pb")
                                    {
                                        string lbldetail = "";
                                        var lovdetail = base.LovData.Where(x => x.Name == "WORDING_1");
                                        if (lovdetail.Any())
                                        {
                                            if (GetCurrentCulture().IsThaiCulture()) lbldetail = lovdetail.FirstOrDefault().LovValue1;
                                            else lbldetail = lovdetail.FirstOrDefault().LovValue2;
                                        }
                                        string lbldetail1 = "";
                                        var lovdetail1 = base.LovData.Where(x => x.Name == "WORDING_3");
                                        if (lovdetail1.Any())
                                        {
                                            if (GetCurrentCulture().IsThaiCulture()) lbldetail1 = lovdetail1.FirstOrDefault().LovValue1;
                                            else lbldetail1 = lovdetail1.FirstOrDefault().LovValue2;
                                        }

                                        htmltxt += string.Format("<h4 class='{0}'>{1}</h4>", "", lbldetail1 + "<br>" + lbldetail);
                                        celltxt = string.Format(celltxt, htmltxt);
                                    }
                                    else
                                    {
                                        foreach (var detail in result.Where(x => x.package_subseq == tmp.ToString() && !string.IsNullOrEmpty(x.package_subseq)).OrderBy(x => x.line_seq))
                                        {
                                            string txtcontent;
                                            if (GetCurrentCulture().IsThaiCulture()) txtcontent = detail.package_display_th;
                                            else txtcontent = detail.package_display_en;

                                            string txtformat = "";
                                            if (detail.font_type == "B") txtformat = "font_green";

                                            htmltxt += string.Format("<h4 class='{0}'>{1}</h4>", txtformat, txtcontent.Replace("|", "<br>"));

                                            if (detail.package_description_th != "" || detail.package_description_en != "")
                                            {
                                                string txtcontent2;
                                                if (GetCurrentCulture().IsThaiCulture()) txtcontent2 = detail.package_description_th;
                                                else txtcontent2 = detail.package_description_en;

                                                htmltxt += string.Format("<h4 class='{0}'>{1}</h4>", "", txtcontent2.Replace("|", "<br>"));
                                            }
                                        }
                                        celltxt = string.Format(celltxt, htmltxt);
                                    }
                                    rowcontent.Append(string.Format(rowtxt, cellheadertxt, celltxt));
                                }
                            }
                        }
                    }
                }
                return rowcontent.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Error when call GetPersonalDisplay in ChangePromotionController : " + ex.InnerException);
            }

            return rowcontent.ToString();
        }

        private string getPackageDescription(string lov_val)
        {
            var tmp = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals("DESCRIPTION_BY_PACKAGE_GROUP") && l.Text.Equals(lov_val));
            if (tmp.Any())
            {
                if (base.GetCurrentCulture().IsThaiCulture())
                {
                    return tmp.FirstOrDefault().LovValue1;
                }
                else
                {
                    return tmp.FirstOrDefault().LovValue2;
                }
            }
            else
            {
                return "";
            }
        }

        private string GetLovByName(string LovName)
        {
            var tmp = base.LovData.Where(l => !string.IsNullOrEmpty(l.Name) && l.Name.Equals(LovName));
            if (tmp.Any())
            {
                if (base.GetCurrentCulture().IsThaiCulture())
                {
                    return tmp.FirstOrDefault().LovValue1;
                }
                else
                {
                    return tmp.FirstOrDefault().LovValue2;
                }
            }
            else
            {
                return "";
            }
        }


        [HttpPost]
        public JsonResult GetTablePackageListByChange(string sale_channel, string owner_product, string customer_type,
                                                  string partner_type, string partner_subtype, string location_code, string asc_code, string employee_id,
                                                  string province, string district, string sub_district, string address_id, string seranede_flag, string non_mobile_no,
                                                  string penalty_flag, string package_main, string product_subtype, string language,
                                                  string fmpa_flag = "", string cvm_flag = "", string fmc_special_flag = "", string existing_mobile_flag = "",
                                                  string outAccountNumber = "", string subNetworkType = "", string serenade_flag = "", string relate_mobile = "", string old_relate_mobile = "",
                                                  string mobile_checkright = "", string mobile_get_benefit = "", string distribution_channel = "", string channel_sales_group = "", string shop_segment = "",
                                                  string location_Province = "")
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            /// Update PRODUCT_SUBTYPE R.2103
            if (product_subtype == "")
            {
                var checkTech = new GetCheckTechnologyQuery
                {
                    P_OWNER_PRODUCT = owner_product,
                    P_ADDRESS_ID = address_id
                };

                var modelCheckTech = _queryProcessor.Execute(checkTech);

                if (modelCheckTech != null && modelCheckTech.PRODUCT_SUBTYPE.ToSafeString() != "")
                {
                    product_subtype = modelCheckTech.PRODUCT_SUBTYPE;
                }
            }

            var config = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "AIS_PLAY_BOX").Select(t => t.LovValue1).ToList();

            //R20.6 ChangePromotionCheckRight
            //string fmc_specail_flag = "";

            #region Column Header
            string L_PROFILE_HEADER_PACKAGE_1 = "";
            string L_PROFILE_HEADER_PACKAGE_2 = "";
            string L_PROFILE_HEADER_PACKAGE_21 = "";
            string L_PROFILE_HEADER_PACKAGE_22 = "";
            string L_PROFILE_HEADER_PACKAGE_3 = "";
            string L_PROFILE_HEADER_PACKAGE_4 = "";
            string L_PROFILE_HEADER_PACKAGE_5 = "";
            string L_PROFILE_HEADER_PACKAGE_6 = "";
            string L_PROFILE_HEADER_PACKAGE_7 = "";
            string L_PACKAGE_REMARK = "";
            string L_BAHT = "";

            L_PROFILE_HEADER_PACKAGE_1 = GetLovByName("L_PROFILE_HEADER_PACKAGE_1");
            L_PROFILE_HEADER_PACKAGE_2 = GetLovByName("L_PROFILE_HEADER_PACKAGE_2");
            L_PROFILE_HEADER_PACKAGE_21 = GetLovByName("L_PROFILE_HEADER_PACKAGE_21");
            L_PROFILE_HEADER_PACKAGE_22 = GetLovByName("L_PROFILE_HEADER_PACKAGE_22");
            L_PROFILE_HEADER_PACKAGE_3 = GetLovByName("L_PROFILE_HEADER_PACKAGE_3");
            L_PROFILE_HEADER_PACKAGE_4 = GetLovByName("L_PROFILE_HEADER_PACKAGE_4");
            L_PROFILE_HEADER_PACKAGE_5 = GetLovByName("L_PROFILE_HEADER_PACKAGE_5");
            L_PROFILE_HEADER_PACKAGE_6 = GetLovByName("L_PROFILE_HEADER_PACKAGE_6");
            L_PROFILE_HEADER_PACKAGE_7 = GetLovByName("L_PROFILE_HEADER_PACKAGE_7");
            L_PACKAGE_REMARK = GetLovByName("L_PACKAGE_REMARK");
            L_BAHT = GetLovByName("L_BAHT");

            #endregion

            string package_for = "PUBLIC";
            StringBuilder resulthtml = new StringBuilder();
            try
            {
                #region Get IP Address Interface Log (Update 17.2)

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                #endregion

                //R20.6 ChangePromotionCheckRight
                //var projectCondFlagArray = GetProjectCondFlagArray(ipAddress, FullUrl, non_mobile_no, relate_mobile, outAccountNumber, subNetworkType, serenade_flag, mobile_checkright, mobile_get_benefit, out fmc_specail_flag, out serenade_flag);
                var projectCondFlagArray = GetProjectCondFlagArrayNew(serenade_flag, fmpa_flag, cvm_flag, fmc_special_flag, existing_mobile_flag);
                var query = new GetListPackageChangeQuery()
                {
                    sale_channel = sale_channel,
                    owner_product = owner_product,
                    package_for = package_for,
                    customer_type = customer_type,
                    partner_type = partner_type,
                    partner_subtype = partner_subtype,
                    location_code = location_code,
                    asc_code = asc_code,
                    employee_id = employee_id,
                    region = "",
                    province = province,
                    district = district,
                    sub_district = sub_district,
                    address_id = address_id,
                    serenade_flag = serenade_flag,
                    penalty_flag = penalty_flag,
                    package_main = package_main,
                    product_subtype = product_subtype,
                    //15.02.21
                    distribution_channel = distribution_channel,
                    channel_sales_group = channel_sales_group,
                    shop_segment = shop_segment,

                    //flag_no_playbox = statusOption + "|" + promocodeCurrent,
                    non_mobile_no = non_mobile_no,
                    client_ip = ipAddress,
                    FullUrl = FullUrl,

                    //R20.6 ChangePromotionCheckRight
                    ProjectCondFlagArray = projectCondFlagArray,
                    location_Province = location_Province
                };

                var result = _queryProcessor.Execute(query);

                if (result != null)
                {
                    if (result.Any())
                    {
                        var pkggrp = result.Where(x => x.package_type == "1" && x.package_type_desc == "Main").Select(x => x.package_group).Distinct().ToList();
                        int table_i = 0;
                        int promotion_i = 0;

                        foreach (string pkggrp_name in pkggrp)
                        {
                            #region PackageName & Description
                            string lblPkgGrp = string.Format("<h2><strong>{0}</strong></h2>", pkggrp_name);
                            string pkgDesc = getPackageDescription(pkggrp_name);
                            if (!string.IsNullOrEmpty(pkgDesc))
                            {
                                lblPkgGrp = lblPkgGrp + string.Format("<h4 style='margin-bottom: 0px;'>{0}</h4>", pkgDesc);
                            }
                            //R21.11 add wording package desc and remark
                            string lblPkgGrpDesc = "";
                            string lblPkgRemark = "";
                            #endregion
                            string htmlTable = "<div class='panel panel-info'><div class='table-responsive'><table class='table table-hover'>{0}{1}</table></div></div>";

                            bool hasDiscount = false;
                            #region TableBody
                            string tableBody = "<tbody class='text-center promotionDetail2' style='font-size:18px;' id='dynamicRow" + table_i.ToString() + "'>{0}</tbody>";
                            StringBuilder tblBodyContent = new StringBuilder();
                            foreach (var detail in result.Where(x => x.package_group == pkggrp_name))
                            {
                                string detailTemp = "";
                                string packageName = "";
                                if (language == "1")
                                    packageName = detail.sff_promotion_bill_tha.ToSafeString();
                                else
                                    packageName = detail.sff_promotion_bill_eng.ToSafeString();

                                //R21.11 add wording package desc and remark
                                if (GetCurrentCulture().IsThaiCulture())
                                {
                                    if (detail.package_group_desc_tha != null)
                                    {
                                        lblPkgGrpDesc = string.Format("<h4 style='margin-top: -10px;margin-bottom: 10px;'>{0}</h4>", detail.package_group_desc_tha.ToSafeString());
                                    }
                                    if (detail.package_remark_tha != null)
                                    {
                                        lblPkgRemark = string.Format("<h4 style='margin-top: -5px;margin-bottom: 20px;'>{0}</h4>", detail.package_remark_tha.ToSafeString());
                                    }
                                }
                                else
                                {
                                    if (detail.package_group_desc_eng != null)
                                    {
                                        lblPkgGrpDesc = string.Format("<h4 style='margin-top: -10px;margin-bottom: 10px;'>{0}</h4>", detail.package_group_desc_eng.ToSafeString());
                                    }
                                    if (detail.package_remark_eng != null)
                                    {
                                        lblPkgRemark = string.Format("<h4 style='margin-top: -5px;margin-bottom: 20px;'>{0}</h4>", detail.package_remark_eng.ToSafeString());
                                    }
                                }

                                detailTemp = packageName;
                                //string detailTemp = detail.technology.Replace("|", "<br>");
                                var tmpOntopPBOXT = result.Where(x => x.package_type_desc == "Ontop" && (x.product_subtype == "PBOXT" || x.product_subtype == "WireBBT" || x.product_subtype == "FTTxT") && x.auto_mapping_code == detail.auto_mapping_code && x.display_flag == "Y");
                                if (tmpOntopPBOXT != null && tmpOntopPBOXT.Count() > 0)
                                {
                                    tmpOntopPBOXT = tmpOntopPBOXT.OrderBy(p => p.display_seq);
                                    foreach (var item in tmpOntopPBOXT)
                                    {
                                        if (language == "1")
                                            detailTemp = detailTemp + "<br>" + item.sff_promotion_bill_tha;
                                        else
                                            detailTemp = detailTemp + "<br>" + item.sff_promotion_bill_eng;
                                        //detailTemp = detailTemp + "<br>" + item.technology.Replace("|", "<br>");
                                    }
                                }

                                //R20.6 ChangePromotionCheckRight
                                var tmpDisplayBuilder = new StringBuilder();
                                var tmpDisplay = result.Where(x => !string.IsNullOrEmpty(x.auto_mapping_code) && x.auto_mapping_code == detail.sff_promotion_code && x.mapping_code == detail.mapping_code && x.display_flag == "Y");
                                if (tmpDisplay != null && tmpDisplay.Count() > 0)
                                {
                                    tmpDisplay = tmpDisplay.OrderBy(p => p.display_seq);
                                    foreach (var dis in tmpDisplay)
                                    {
                                        var tmpDisplayStr = "";
                                        if (language == "1")
                                        {
                                            tmpDisplayStr = "|" + dis.package_display_tha;
                                            detailTemp = detailTemp + "|" + dis.package_display_tha;
                                        }
                                        else
                                        {
                                            tmpDisplayStr = "|" + dis.package_display_eng;
                                            detailTemp = detailTemp + "|" + dis.package_display_eng;
                                        }
                                        tmpDisplayBuilder.Append(tmpDisplayStr);
                                    }
                                }

                                string checkShowPlaybox = "N";
                                //string accessModePlaybox = "";

                                //List<PackageModel> PackageListBySFFPromos = new List<PackageModel>();

                                //if (statusOption == "N")
                                //{
                                //    checkShowPlaybox = CheckNoToPB(promocodeCurrent, detail.sff_promotion_code, statusOption, non_mobile_no);
                                //    if (checkShowPlaybox == "Y")
                                //    {
                                //        List<GetInputGetListPackageBySFFPROMO> inputDatas = GetInputGetListPackageBySFFPROMO(promocodeCurrent, detail.sff_promotion_code, non_mobile_no);
                                //        if (inputDatas != null && inputDatas.Count > 0)
                                //        {
                                //            GetInputGetListPackageBySFFPROMO inputData = inputDatas.FirstOrDefault();
                                //            if (config != null && config.Count > 0)
                                //            {
                                //                PackageListBySFFPromos = GetPackageListBySFFPromoForChangPro(inputData.p_sff_promocode.ToSafeString(),
                                //                    inputData.p_product_subtype.ToSafeString(), inputData.p_owner_product.ToSafeString(),
                                //                    inputData.p_vas_service.ToSafeString(), non_mobile_no);

                                //                var PackageListBySFFPromo = PackageListBySFFPromos.FirstOrDefault(t => config.Contains(t.SERVICE_CODE.ToSafeString()) && t.PRODUCT_SUBTYPE3 == "PBOX");

                                //                if (PackageListBySFFPromo != null)
                                //                {
                                //                    accessModePlaybox = PackageListBySFFPromo.ACCESS_MODE;
                                //                }
                                //                else
                                //                {
                                //                    checkShowPlaybox = "N";
                                //                }
                                //            }
                                //            else
                                //            {
                                //                checkShowPlaybox = "N";
                                //            }
                                //        }
                                //    }
                                //    else
                                //    {
                                //        checkShowPlaybox = "N";
                                //    }
                                //}

                                string tblRow = "<tr style='cursor: pointer;' class='promotionDetail-table' onclick='onBindingSelectPack(" + promotion_i.ToString() + ");'>{0}</tr>";
                                string tblCell = "<th>" +
                                    "<input name='SelectPackage' id='SelectPackage" + promotion_i.ToString() + "' checked='' value='" + promotion_i.ToString() + "' type='radio' onclick='onBindingSelectPack(" + promotion_i.ToString() + ");'>" +
                                    "<input id='MAPPING_CODE" + promotion_i.ToString() + "' value='" + detail.mapping_code + "' type='hidden'>" +
                                    "<input id='PACKAGE_CODE" + promotion_i.ToString() + "' value='" + detail.sff_promotion_code + "' type='hidden'>" +
                                    "<input id='PACKAGE_NAME" + promotion_i.ToString() + "' value='" + packageName + "' type='hidden'>" +
                                    "<input id='PACKAGE_GROUP" + promotion_i.ToString() + "' value='" + detail.package_group + "' type='hidden'>" +
                                    "<input id='RECURRING_CHARGE" + promotion_i.ToString() + "' value='" + detail.recurring_charge + "' type='hidden'>" +
                                    "<input id='PRE_RECURRING_CHARGE" + promotion_i.ToString() + "' value='" + detail.pre_recurring_charge + "' type='hidden'>" +
                                    "<input id='SFF_PROMOTION_BILL_THA" + promotion_i.ToString() + "' value='" + detail.sff_promotion_bill_tha + "' type='hidden'>" +
                                    "<input id='SFF_PROMOTION_BILL_ENG" + promotion_i.ToString() + "' value='" + detail.sff_promotion_bill_eng + "' type='hidden'>" +
                                    "<input id='TECHNOLOGY" + promotion_i.ToString() + "' value='" + packageName + "' type='hidden'>" +
                                    "<input id='DOWNLOAD_SPEED" + promotion_i.ToString() + "' value='" + detail.download_speed + "' type='hidden'>" +
                                    "<input id='UPLOAD_SPEED" + promotion_i.ToString() + "' value='" + detail.upload_speed + "' type='hidden'>" +
                                    "<input id='INITIATION_CHARGE" + promotion_i.ToString() + "' value='" + detail.initiation_charge + "' type='hidden'>" +
                                    "<input id='PRE_INITIATION_CHARGE" + promotion_i.ToString() + "' value='" + detail.pre_initiation_charge + "' type='hidden'>" +
                                    "<input id='PACKAGE_TYPE" + promotion_i.ToString() + "' value='" + detail.package_type + "' type='hidden'>" +
                                    "<input id='PACKAGE_TYPE_DESC" + promotion_i.ToString() + "' value='" + detail.package_type_desc + "' type='hidden'>" +
                                    "<input id='PRODUCT_SUBTYPE" + promotion_i.ToString() + "' value='" + detail.product_subtype + "' type='hidden'>" +
                                    "<input id='OWNER_PRODUCT" + promotion_i.ToString() + "' value='" + detail.owner_product + "' type='hidden'>" +
                                    "<input id='SFF_PROMOTION_CODE" + promotion_i.ToString() + "' value='" + detail.sff_promotion_code + "' type='hidden'>" +
                                    "<input id='CHECK_SHOW_PLAYBOX" + promotion_i.ToString() + "' value='" + checkShowPlaybox + "' type='hidden'>" +
                                    //"<input id='ACCESS_MODE_PLAYBOX" + promotion_i.ToString() + "' value='" + accessModePlaybox + "' type='hidden'>" +
                                    //"<input id='SEQ" + promotion_i.ToString() + "' value='" + detail.seq + "' type='hidden'>" +
                                    "<input id='SFF_DISCOUNT_DISPLAY_TEXT" + promotion_i.ToString() + "' value='" + string.Join("", tmpDisplayBuilder) + "' type='hidden'>" +
                                    "</th>" +
                                    "<td class='text-left'>" + detailTemp.ToSafeString().Replace("|", "<br>") + "</td>" +
                                    "<td>" + detail.download_speed + "</td>" +
                                    "<td>" + detail.upload_speed + "</td>";

                                if (detail.pre_recurring_charge != 0 || (detail.pre_recurring_charge == 0 && !string.IsNullOrEmpty(detail.auto_mapping_code)))
                                {
                                    hasDiscount = true;
                                    string additionalCell = "<td>{0}</td><td>{1}</td>";
                                    string discountVal = "";
                                    string RemarkVal = "";

                                    if (detail.pre_recurring_charge == 0)
                                    {
                                        var tmp = result.Where(x => x.package_type == "Ontop Special" && x.auto_mapping_code == detail.auto_mapping_code);
                                        if (tmp != null && tmp.Any())
                                        {

                                            var ontop_pkg = tmp.FirstOrDefault();
                                            if (ontop_pkg.discount_type == "PCT")
                                            {
                                                decimal? _discountVal = detail.recurring_charge - ((ontop_pkg.discount_value / 100) * detail.recurring_charge);
                                                decimal ddiscountVal = 0;
                                                decimal.TryParse(_discountVal.ToString(), out ddiscountVal);
                                                discountVal = ddiscountVal.ToString("#,###.00");
                                                RemarkVal = L_PACKAGE_REMARK.Replace("{%1}", ontop_pkg.discount_value.ToString() + "%").Replace("{%2}", detail.recurring_charge.ToString());
                                            }
                                            else if (ontop_pkg.discount_type == "AMT")
                                            {
                                                decimal? _discountVal = detail.recurring_charge - ontop_pkg.discount_value;
                                                decimal ddiscountVal = 0;
                                                decimal.TryParse(_discountVal.ToString(), out ddiscountVal);
                                                discountVal = ddiscountVal.ToString("#,###.00");
                                                RemarkVal = L_PACKAGE_REMARK.Replace("{%1}", ontop_pkg.discount_value.ToString() + " " + L_BAHT).Replace("{%2}", detail.recurring_charge.ToString());
                                            }
                                            tblCell += string.Format(additionalCell, discountVal, RemarkVal);
                                        }
                                        else
                                        {
                                            hasDiscount = false;
                                            string additionalCell2 = "<td>{0}</td>";
                                            decimal _chargeVal = 0;
                                            decimal.TryParse(detail.recurring_charge.ToString(), out _chargeVal);
                                            tblCell += string.Format(additionalCell2, _chargeVal.ToString("#,###.00"));
                                        }
                                    }
                                    else
                                    {
                                        decimal? _discountVal = detail.recurring_charge;
                                        decimal ddiscountVal = 0;
                                        decimal.TryParse(_discountVal.ToString(), out ddiscountVal);
                                        discountVal = ddiscountVal.ToString("#,###.00");
                                        RemarkVal = L_PACKAGE_REMARK.Replace("{%1}", (detail.pre_recurring_charge - detail.recurring_charge).ToString() + " " + L_BAHT).Replace("{%2}", detail.pre_recurring_charge.ToString());
                                        tblCell += string.Format(additionalCell, discountVal, RemarkVal);
                                    }
                                }
                                else
                                {
                                    hasDiscount = false;
                                    string additionalCell = "<td>{0}</td>";
                                    decimal _chargeVal = 0;
                                    decimal.TryParse(detail.recurring_charge.ToString(), out _chargeVal);
                                    tblCell += string.Format(additionalCell, _chargeVal.ToString("#,###.00"));
                                }

                                tblRow = string.Format(tblRow, tblCell);
                                tblBodyContent.Append(tblRow);
                                promotion_i++;
                            }
                            tableBody = string.Format(tableBody, tblBodyContent);
                            #endregion

                            #region Condition
                            string lblCondition = "";
                            var conditionDesc = base.LovData.Where(x => x.Name == "L_PACKAGE_CONDITION_DESC" && x.LovValue5 == pkggrp_name);
                            if (conditionDesc != null)
                            {
                                if (conditionDesc.Any())
                                {
                                    var LovTMP = base.LovData.Where(x => x.Name == "L_PACKAGE_CONDITION").FirstOrDefault();
                                    string L_PACKAGE_CONDITION = LovTMP != null ? base.GetCurrentCulture().IsThaiCulture() ? LovTMP.LovValue1 : LovTMP.LovValue2 : "";
                                    lblCondition = "<div class='panel-group'><div class='panel panel-default'>" +
                                        "<div class='panel-heading'>" +
                                        "<h4 class='panel-title'>" +
                                        "<a data-toggle='collapse' data-parent='#accordion' href='#collapse" + table_i.ToString() + "'>{0}</a>" +
                                        "</h4>" +
                                        "</div>" +
                                        "<div id='collapse" + table_i.ToString() + "' class='panel-collapse collapse'>" +
                                        "<div class='panel-body' style='font-size:16px;'>{1}</div>" +
                                        "</div>" +
                                        "</div></div>";
                                    StringBuilder pkgConditionDesc = new StringBuilder();
                                    foreach (var condDesctmp in conditionDesc)
                                    {
                                        string descTmp = "";
                                        if (base.GetCurrentCulture().IsThaiCulture())
                                        {
                                            descTmp = condDesctmp.LovValue1;
                                        }
                                        else
                                        {
                                            descTmp = condDesctmp.LovValue2;
                                        }
                                        pkgConditionDesc.Append(descTmp + "<br />");
                                    }
                                    lblCondition = string.Format(lblCondition, L_PACKAGE_CONDITION, pkgConditionDesc);
                                }
                            }
                            #endregion

                            #region ColumnHeader
                            string tableHeader = "<thead class='text-center promotionTitle'>" +
                                "<tr style='background-image: linear-gradient(to bottom, #F5F5F5 0px, #E8E8E8 100%); background-repeat: repeat-x; border-top-left-radius: 3px; border-top-right-radius: 3px;'>" +
                                "<th class='text-center'><h4 style='margin-top:15px;'> &nbsp;</h4></th>" +
                                "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_1 + "</strong></h4></th>" +
                                "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_21 + "</strong></h4></th>" +
                                "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_22 + "</strong></h4></th>" +
                                "{0}" +
                                "</tr></thead>";
                            string additionalColumn = "";
                            if (hasDiscount)
                            {
                                if (serenade_flag == "Y")
                                {
                                    additionalColumn = "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_6 + "</strong></h4></th>" +
                                        "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_5 + "</strong></h4></th>";
                                }
                                else if (serenade_flag == "N")
                                {
                                    additionalColumn = "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_4 + "</strong></h4></th>" +
                                       "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_5 + "</strong></h4></th>";
                                }
                                else
                                {
                                    additionalColumn = "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_7 + "</strong></h4></th>" +
                                       "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_5 + "</strong></h4></th>";
                                }
                            }
                            else
                            {
                                additionalColumn = "<th class='text-center'><h4><strong>" + L_PROFILE_HEADER_PACKAGE_3 + "</strong></h4></th>";
                            }
                            tableHeader = string.Format(tableHeader, additionalColumn);
                            #endregion

                            table_i++;

                            resulthtml.Append(lblPkgGrp + lblPkgGrpDesc + string.Format(htmlTable, tableHeader, tableBody) + lblCondition + lblPkgRemark + "<p class='clearfix'></p>");
                        }

                        string hiddenTable = "<table id='OntopPkg' style='display:none'><tbody>{0}</tbody></table>";
                        StringBuilder ontopTableBody = new StringBuilder();
                        var pkgontop = result.Where(x => x.package_type == "4");
                        foreach (var pkgtmp in pkgontop)
                        {
                            string tmpTechnology = language == "1" ? pkgtmp.sff_promotion_bill_tha : pkgtmp.sff_promotion_bill_eng;
                            string tblRow = "<tr name='" + pkgtmp.mapping_code + "'>{0}</tr>";
                            string tblCell = "<th>" +
                                "<input name='PACKAGE_CODE' value='" + pkgtmp.sff_promotion_code + "' type='hidden'>" +
                                "<input name='PACKAGE_NAME' value='" + tmpTechnology + "' type='hidden'>" +
                                "<input name='PACKAGE_GROUP' value='" + pkgtmp.package_group + "' type='hidden'>" +
                                "<input name='RECURRING_CHARGE' value='" + pkgtmp.recurring_charge + "' type='hidden'>" +
                                "<input name='PRE_RECURRING_CHARGE' value='" + pkgtmp.pre_recurring_charge + "' type='hidden'>" +
                                "<input name='SFF_PROMOTION_BILL_THA' value='" + pkgtmp.sff_promotion_bill_tha + "' type='hidden'>" +
                                "<input name='SFF_PROMOTION_BILL_ENG' value='" + pkgtmp.sff_promotion_bill_eng + "' type='hidden'>" +
                                "<input name='TECHNOLOGY' value='" + tmpTechnology + "' type='hidden'>" +
                                "<input name='DOWNLOAD_SPEED' value='" + pkgtmp.download_speed + "' type='hidden'>" +
                                "<input name='UPLOAD_SPEED' value='" + pkgtmp.upload_speed + "' type='hidden'>" +
                                "<input name='INITIATION_CHARGE' value='" + pkgtmp.initiation_charge + "' type='hidden'>" +
                                "<input name='PRE_INITIATION_CHARGE' value='" + pkgtmp.pre_initiation_charge + "' type='hidden'>" +
                                "<input name='PACKAGE_TYPE' value='" + pkgtmp.package_type + "' type='hidden'>" +
                                "<input name='PACKAGE_TYPE_DESC' value='" + pkgtmp.package_type_desc + "' type='hidden'>" +
                                "<input name='PRODUCT_SUBTYPE' value='" + pkgtmp.product_subtype + "' type='hidden'>" +
                                "<input name='OWNER_PRODUCT' value='" + pkgtmp.owner_product + "' type='hidden'>" +
                                "<input name='SFF_PROMOTION_CODE' value='" + pkgtmp.sff_promotion_code + "' type='hidden'>" +
                                //"<input name='SEQ' value='" + pkgtmp.seq + "' type='hidden'>" +
                                "</th>";
                            tblRow = string.Format(tblRow, tblCell);
                            ontopTableBody.Append(tblRow);
                        }
                        hiddenTable = string.Format(hiddenTable, ontopTableBody);
                        resulthtml.Append(hiddenTable);

                        string hiddenTable2 = "<table id='OntopPBOXTPkg' style='display:none'><tbody>{0}</tbody></table>";
                        StringBuilder ontopPBOXTTableBody = new StringBuilder();
                        var pkgontopPBOXTs = result.Where(x => x.package_type == "8");
                        foreach (var pkgontopPBOXT in pkgontopPBOXTs)
                        {
                            string tmpTechnology = language == "1" ? pkgontopPBOXT.sff_promotion_bill_tha : pkgontopPBOXT.sff_promotion_bill_eng;
                            string tblRow = "<tr name='" + pkgontopPBOXT.mapping_code + "'>{0}</tr>";
                            string tblCell = "<th>" +
                                "<input name='PACKAGE_CODE' value='" + pkgontopPBOXT.sff_promotion_code + "' type='hidden'>" +
                                "<input name='PACKAGE_NAME' value='" + tmpTechnology + "' type='hidden'>" +
                                "<input name='PACKAGE_GROUP' value='" + pkgontopPBOXT.package_group + "' type='hidden'>" +
                                "<input name='RECURRING_CHARGE' value='" + pkgontopPBOXT.recurring_charge + "' type='hidden'>" +
                                "<input name='PRE_RECURRING_CHARGE' value='" + pkgontopPBOXT.pre_recurring_charge + "' type='hidden'>" +
                                "<input name='SFF_PROMOTION_BILL_THA' value='" + pkgontopPBOXT.sff_promotion_bill_tha + "' type='hidden'>" +
                                "<input name='SFF_PROMOTION_BILL_ENG' value='" + pkgontopPBOXT.sff_promotion_bill_eng + "' type='hidden'>" +
                                "<input name='TECHNOLOGY' value='" + tmpTechnology + "' type='hidden'>" +
                                "<input name='DOWNLOAD_SPEED' value='" + pkgontopPBOXT.download_speed + "' type='hidden'>" +
                                "<input name='UPLOAD_SPEED' value='" + pkgontopPBOXT.upload_speed + "' type='hidden'>" +
                                "<input name='INITIATION_CHARGE' value='" + pkgontopPBOXT.initiation_charge + "' type='hidden'>" +
                                "<input name='PRE_INITIATION_CHARGE' value='" + pkgontopPBOXT.pre_initiation_charge + "' type='hidden'>" +
                                "<input name='PACKAGE_TYPE' value='" + pkgontopPBOXT.package_type + "' type='hidden'>" +
                                "<input name='PACKAGE_TYPE_DESC' value='" + pkgontopPBOXT.package_type_desc + "' type='hidden'>" +
                                "<input name='PRODUCT_SUBTYPE' value='" + pkgontopPBOXT.product_subtype + "' type='hidden'>" +
                                "<input name='OWNER_PRODUCT' value='" + pkgontopPBOXT.owner_product + "' type='hidden'>" +
                                "<input name='SFF_PROMOTION_CODE' value='" + pkgontopPBOXT.sff_promotion_code + "' type='hidden'>" +
                                //"<input name='SEQ' value='" + pkgontopPBOXT.seq + "' type='hidden'>" +
                                "</th>";
                            tblRow = string.Format(tblRow, tblCell);
                            ontopPBOXTTableBody.Append(tblRow);
                        }
                        hiddenTable2 = string.Format(hiddenTable2, ontopPBOXTTableBody);
                        resulthtml.Append(hiddenTable2);

                        // fff


                    }
                }

                return Json(new { resulthtml = resulthtml.ToString(), fmc_special_flag, existing_mobile_flag = existing_mobile_flag, serenade_flag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.RenderExceptionMessage());
                return Json(new { resulthtml = resulthtml.ToString(), fmc_special_flag, existing_mobile_flag = existing_mobile_flag, serenade_flag }, JsonRequestBehavior.AllowGet);
            }

        }

        [AjaxValidateAntiForgeryToken]
        public JsonResult ConfirmChange(
            string non_mobile_no,
            string relate_mobile,
            string old_relate_mobile,
            string project_name,
            string network_type,
            string old_package,
            string new_package,
            string isBundle,
            string mobileNumberContact,
            string relateMobileAction,
            string overrule,
            string locationCd,
            string ascCode,
            string employee_id,
            string accessMode = "",
            string playBoxCode = "",
            string appointmentDate = "",
            string timeSlot = "",
            string RESERVED_ID = "",
            string employee_name = "",
            string checkHavePlayBox = "",
            string changepro_pk = "")
        {
            try
            {
                // 17.6 Interface Log Add Url
                Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "ConfirmChange", null, this.Request.Url.Scheme);
                string FullUrl = "";
                if (Session["FullUrl"] != null)
                    FullUrl = Session["FullUrl"].ToSafeString();

                var air_old_list = new List<AirChangePromotionCode>();
                var js = new JavaScriptSerializer();
                var deserializedAirItems = (object[])js.DeserializeObject(old_package);
                if (deserializedAirItems != null)
                {
                    foreach (Dictionary<string, object> newItem in deserializedAirItems)
                    {
                        var stmp = new SerializeAirchangePromotionJSonModel(newItem);
                        var air = new AirChangePromotionCode()
                        {
                            SFF_PROMOTION_CODE = stmp.SFF_PROMOTION_CODE,
                            startDt = stmp.startDt,
                            endDt = stmp.endDt,
                            PRODUCT_SEQ = stmp.PRODUCT_SEQ
                        };
                        air_old_list.Add(air);
                    }
                }

                var air_new_list = new List<AirChangePromotionCode>();
                deserializedAirItems = (object[])js.DeserializeObject(new_package);
                if (deserializedAirItems != null)
                {
                    foreach (Dictionary<string, object> newItem in deserializedAirItems)
                    {
                        var stmp = new SerializeAirchangePromotionJSonModel(newItem);
                        var air = new AirChangePromotionCode()
                        {
                            SFF_PROMOTION_CODE = stmp.SFF_PROMOTION_CODE,
                            startDt = stmp.startDt,
                            endDt = stmp.endDt,
                            PRODUCT_SEQ = stmp.PRODUCT_SEQ
                        };
                        air_new_list.Add(air);
                    }
                }

                #region Get IP Address Interface Log (Update 17.2)

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                #endregion

                //var query = new GetListChangePackageQuery()
                //{
                //    non_mobile_no = non_mobile_no,
                //    relate_mobile = relate_mobile,
                //    current_project_name = project_name,
                //    network_type = network_type,
                //    AirChangePromotionCodeOldArray = air_old_list,
                //    AirChangePromotionCodeNewArray = air_new_list,
                //    AirChangePlayBoxPromotionCodeNewArray = air_playbox_list,
                //    oldRelateMobile = old_relate_mobile,
                //    acTion = relateMobileAction,
                //    mobileNumberContact = mobileNumberContact,
                //    client_ip = ipAddress,
                //    FullUrl = FullUrl,
                //};

                ////AWARE_R20.02 Set ไม่ให้ submit สำหรับอ่าน code
                ////TODO : Mockup
                //#region
                //var aware_R20_20_Mocking = true;
                //if (aware_R20_20_Mocking)
                //{
                //    var gotoTopup = "N";
                //    var newSffPromotionCode = "";
                //    if (air_new_list != null && air_new_list.Any())
                //    {
                //        newSffPromotionCode = air_new_list.FirstOrDefault().SFF_PROMOTION_CODE;
                //    }
                //    var queryMapping = new GetServicePlayboxMappingCodeQuery
                //    {
                //        INTERNET_NO = non_mobile_no,
                //        SFF_PROMOTION_CODE = newSffPromotionCode,
                //        HAVE_PLAY_FLAG = checkHavePlayBox,
                //        FULL_URL = FullUrl
                //    };
                //    var linqMapping = _queryProcessor.Execute(queryMapping);
                //    if (linqMapping != null)
                //    {
                //        gotoTopup = linqMapping.GOTO_TOPUP;
                //        gotoTopup = "Y"; //VALIDATE_FLAG="Y" , GOTO_TOPUP="Y" จะ redirect ไป Topup Playbox

                //        if (gotoTopup == "Y")
                //        {
                //            var configDelay = base.LovData.FirstOrDefault(l => l.Type == "CONFIG" && l.Name == "CHANGE_PROMOTION_DELAY");
                //            if (configDelay != null)
                //            {
                //                var valDelay = configDelay.LovValue1.ToSafeInteger();
                //                if (valDelay > 0)
                //                {
                //                    var interval = new TimeSpan(0, 0, valDelay);
                //                    Thread.Sleep(interval);
                //                }
                //            }
                //        }
                //    }
                //    return Json(new CreateOrderChangePromotionCommand() { VALIDATE_FLAG = "Y", ERROR_MSG = "ERROR", GOTO_TOPUP = gotoTopup }, JsonRequestBehavior.AllowGet);
                //}
                //#endregion

                //var result = _queryProcessor.Execute(query);

                //R20.6 
                if (string.IsNullOrEmpty(changepro_pk))
                {
                    return Json(new CreateOrderChangePromotionCommand() { VALIDATE_FLAG = "N", ERROR_MSG = "GetListChangePackageFailed" }, JsonRequestBehavior.AllowGet);
                }
                var resultSession = Session["ListChangePackageModel"];
                if (resultSession == null)
                {
                    var errorTimeout = "ท่านเข้าใช้งานในระบบนานเกินเวลาที่กำหนด (Session Expired)"; //Default Msg
                    if (LovData != null)
                    {
                        var timeOutMsg = LovData.FirstOrDefault(f => f.Name == "L_SESSION_TIMEOUT_WARNING");
                        if (timeOutMsg != null)
                        {
                            errorTimeout = timeOutMsg.LovValue1.ToSafeString();
                        }
                    }

                    return Json(new CreateOrderChangePromotionCommand() { VALIDATE_FLAG = "TIMEOUT", ERROR_MSG = errorTimeout }, JsonRequestBehavior.AllowGet);
                }

                var dataPackage = Session["ListChangePackageModel"] as Dictionary<string, List<ListChangePackageModel>>;
                var result = dataPackage[changepro_pk];
                if (result == null)
                {
                    return Json(new CreateOrderChangePromotionCommand() { VALIDATE_FLAG = "N", ERROR_MSG = "GetListChangePackageNoData" }, JsonRequestBehavior.AllowGet);
                }
                //end

                string orderNo = result.Select(x => x.order_no).FirstOrDefault();
                non_mobile_no = result.Select(x => x.non_mobile_no).FirstOrDefault();
                relate_mobile = result.Select(x => x.relate_mobile).FirstOrDefault();
                relateMobileAction = result.Select(x => x.bundling_mobile_action).FirstOrDefault();

                //R20.6
                string new_project_name = result.Select(x => x.new_project_name).FirstOrDefault();
                var new_project_name_opt = result.Select(x => x.new_project_name_opt).FirstOrDefault();
                var new_mobile_check_right = result.Select(x => x.new_mobile_check_right).FirstOrDefault();
                var new_mobile_check_right_opt = result.Select(x => x.new_mobile_check_right_opt).FirstOrDefault();
                var new_mobile_get_benefit = result.Select(x => x.new_mobile_get_benefit).FirstOrDefault();
                var new_mobile_get_benefit_opt = result.Select(x => x.new_mobile_get_benefit_opt).FirstOrDefault();
                var old_relate_mobile_db = result.Select(x => x.old_relate_mobile).FirstOrDefault();

                var listAction = new List<PromotionAction>();
                foreach (var tmp in result)
                {
                    listAction.Add(
                                new PromotionAction()
                                {
                                    PromotionCode = tmp.sff_promotion_code,
                                    ActionStatus = tmp.action_status,
                                    Overrule = overrule,

                                    // R20.6 Add by Aware : Atipon
                                    SendSffFlag = tmp.send_sff_flag
                                });
                }

                PromotionPlayBox promotionPlayBox = null;
                if (playBoxCode != "")
                {
                    try
                    {
                        var listServiceAndPromotionByPackageTypeQuery = new evOMQueryListServiceAndPromotionByPackageTypeQuery
                        {
                            mobileNo = non_mobile_no,
                            idCard = "",
                            FullUrl = FullUrl
                        };
                        evOMQueryListServiceAndPromotionByPackageTypeModel listServiceAndPromotionByPackageTypeData = new evOMQueryListServiceAndPromotionByPackageTypeModel();

                        listServiceAndPromotionByPackageTypeData = _queryProcessor.Execute(listServiceAndPromotionByPackageTypeQuery);

                        promotionPlayBox = new PromotionPlayBox()
                        {
                            serviceCode = playBoxCode,
                            accessMode = accessMode,
                            addressId = listServiceAndPromotionByPackageTypeData.addressId,
                            appointmentDate = appointmentDate.Replace("/", "-"),
                            contactMobilePhone = listServiceAndPromotionByPackageTypeData.contactMobilePhone,
                            contactName = listServiceAndPromotionByPackageTypeData.contactName,
                            contentName = "",
                            installAddress1 = listServiceAndPromotionByPackageTypeData.v_installAddress1,
                            installAddress2 = listServiceAndPromotionByPackageTypeData.v_installAddress2,
                            installAddress3 = listServiceAndPromotionByPackageTypeData.v_installAddress3,
                            installAddress4 = listServiceAndPromotionByPackageTypeData.v_installAddress4,
                            installAddress5 = listServiceAndPromotionByPackageTypeData.v_installAddress5,
                            relateMobile = relate_mobile,
                            serialNumber = null,
                            timeSlot = timeSlot,
                            installedFlag = "N",
                        };
                    }
                    catch { }
                }

                string servicCodeAp = "";
                var config = base.LovData.FirstOrDefault(l => l.Type == "SCREEN" && l.Name == "APPOINTMENT");
                if (config != null)
                {
                    servicCodeAp = config.LovValue1;
                }

                var command = new CreateOrderChangePromotionCommand
                {
                    NonMobileNo = non_mobile_no,
                    RelateMobile = relate_mobile,
                    ProjectName = new_project_name,
                    OrderNo = orderNo,
                    ListAction = listAction,
                    oldRelateMobile = old_relate_mobile_db,
                    acTion = relateMobileAction,
                    mobileNumberContact = mobileNumberContact,
                    client_ip = ipAddress,
                    FullUrl = FullUrl,
                    locationCd = locationCd,
                    ascCode = ascCode,
                    EmployeeID = employee_id,
                    promotionPlayBox = promotionPlayBox,
                    servicCodeApp = servicCodeAp,
                    appointmentDate = appointmentDate.Replace("/", "-"),
                    reservedId = RESERVED_ID,
                    timeslot = timeSlot,
                    EmployeeName = employee_name,

                    //R20.6
                    ListChangePackageModel = result,
                    BUNDLING_ACTION = relateMobileAction,
                    OLD_RELATE_MOBILE = old_relate_mobile,
                    MOBILE_CONTACT = mobileNumberContact,
                    new_project_name = new_project_name,
                    new_project_name_opt = new_project_name_opt,
                    new_mobile_check_right = new_mobile_check_right,
                    new_mobile_check_right_opt = new_mobile_check_right_opt,
                    new_mobile_get_benefit = new_mobile_get_benefit,
                    new_mobile_get_benefit_opt = new_mobile_get_benefit_opt
                };
                _crateOrderChangePromotionCommand.Handle(command);

                //AWARE_R20.02
                if (command.VALIDATE_FLAG == "Y")
                {
                    var gotoTopup = "N";
                    var gotoTopupReplace = "N";//R22.03 Topup Replace

                    var newSffPromotionCode = "";
                    if (air_new_list != null && air_new_list.Any())
                    {
                        newSffPromotionCode = air_new_list.FirstOrDefault().SFF_PROMOTION_CODE;
                    }

                    if (checkHavePlayBox == "N")
                    { //Topup
                        var queryMapping = new GetServicePlayboxMappingCodeQuery
                        {
                            INTERNET_NO = non_mobile_no,
                            SFF_PROMOTION_CODE = newSffPromotionCode,
                            HAVE_PLAY_FLAG = checkHavePlayBox,
                            FULL_URL = FullUrl
                        };
                        var linqMapping = _queryProcessor.Execute(queryMapping);
                        if (linqMapping != null)
                        {
                            gotoTopup = linqMapping.GOTO_TOPUP;
                        }
                        command.GOTO_TOPUP = gotoTopup;

                        if (command.GOTO_TOPUP == "Y")
                        {
                            var configDelay = base.LovData.FirstOrDefault(l => l.Type == "CONFIG" && l.Name == "CHANGE_PROMOTION_DELAY");
                            if (configDelay != null)
                            {
                                var valDelay = configDelay.LovValue1.ToSafeInteger();
                                if (valDelay > 0)
                                {
                                    var interval = new TimeSpan(0, 0, valDelay);
                                    Thread.Sleep(interval);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Topup Replace //R22.03 Topup Replace
                        gotoTopupReplace = CheckGotoTopupReplace(non_mobile_no, "T", newSffPromotionCode, FullUrl);
                        command.GOTO_TOPUP_REPLACE = gotoTopupReplace;

                        if (command.GOTO_TOPUP_REPLACE == "Y")
                        {
                            var configDelay = base.LovData.FirstOrDefault(l => l.Type == "CONFIG" && l.Name == "CHANGE_PROMOTION_DELAY");
                            if (configDelay != null)
                            {
                                var valDelay = configDelay.LovValue1.ToSafeInteger();
                                if (valDelay > 0)
                                {
                                    var interval = new TimeSpan(0, 0, valDelay);
                                    Thread.Sleep(interval);
                                }
                            }
                        }
                    }
                }

                return Json(command, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new CreateOrderChangePromotionCommand() { VALIDATE_FLAG = "N", ERROR_MSG = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string CheckGotoTopupReplace(string AisAirNumber, string Lang, string SffPromotionCodeMain, string FullUrl)
        {
            #region Get IP Address Interface Log

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            //Step1: Check PROMOTION_NOT_ALLOW
            List<LovValueModel> lovPROMOTION_NOT_ALLOW = base.LovData.Where(l => l.Name == "PROMOTION_NOT_ALLOW"
                                                                            && l.Type == "SCREEN"
                                                                            && l.LovValue5 == "FBBOR050"
                                                                            && l.LovValue1 == SffPromotionCodeMain).ToList();
            if (lovPROMOTION_NOT_ALLOW.Any())
            {
                var checkPromoNotAllow = lovPROMOTION_NOT_ALLOW.Select(i => i.LovValue1).FirstOrDefault();
                if (checkPromoNotAllow != null) return "N"; //else Goto Step 2.
            }

            //Step2: Get Check PKG_FBBOR050.CHECK_PROMOTION
            var query = new GetCheckPromotionTopupReplaceQuery
            {
                P_FLAG_LANG = Lang,
                TransactionId = transactionId,
                FullUrl = FullUrl
            };
            query.SffPromotionCodeList = new List<ContractDeviceArrayModel>();
            query.SffPromotionCodeList.Add(new ContractDeviceArrayModel { PROMOTION_CODE = SffPromotionCodeMain });

            var resultdata = _queryProcessor.Execute(query);

            if (resultdata != null && resultdata.RETURN_ERROR_FLAG == "N")
            {
                //Step3: Get FBSSQueryCPEPenalty
                var query2 = new GetFBSSQueryCPEPenaltyQuery
                {
                    OPTION = "1",
                    FIBRENET_ID = AisAirNumber,
                    SERIAL_NO = "",
                    STATUS = "4",
                    MAC_ADDRESS = "",
                    TransactionId = transactionId,
                    FullUrl = FullUrl
                };

                List<FBSSQueryCPEPenaltyModel> dataCPEPenalty = _queryProcessor.Execute(query2);

                CheckATVTopupReplaceModel dataCheckATV = null;

                if (dataCPEPenalty != null)
                {
                    //Step4: Get Check PKG_FBBOR050.CHECK_ATV
                    var query3 = new GetCheckATVTopupReplaceQuery
                    {
                        P_FIBRENET_ID = AisAirNumber,
                        P_FLAG_LANG = Lang,
                        TransactionId = transactionId,
                        FullUrl = FullUrl
                    };
                    query3.Fbbor050PlayboxList = new List<Fbbor050PlayboxArrayModel>();

                    foreach (var i in dataCPEPenalty)
                    {
                        Fbbor050PlayboxArrayModel fbbor050PlayboxArrayModel = new Fbbor050PlayboxArrayModel
                        {
                            CPE_TYPE = i.CPE_TYPE,
                            CPE_MODEL_NAME = i.CPE_MODEL_NAME,
                            STATUS_DESC = i.STATUS_DESC,
                            CPE_BRAND_NAME = i.CPE_BRAND_NAME,
                            CPE_MODEL_ID = i.CPE_MODEL_ID,
                            CPE_GROUP_TYPE = i.CPE_GROUP_TYPE,
                            SN_PATTERN = i.SN_PATTERN,
                            SERIAL_NO = i.SERIAL_NO,
                            STATUS = i.STATUS
                        };
                        query3.Fbbor050PlayboxList.Add(fbbor050PlayboxArrayModel);
                    }

                    dataCheckATV = _queryProcessor.Execute(query3);

                    if (dataCheckATV != null && dataCheckATV.RETURN_SERIAL_CURROR != null
                        && dataCheckATV.RETURN_SERIAL_CURROR.Count() > 0)
                    {
                        return "Y";
                    }
                }
            }

            return "N";
        }

        private List<ProjectCondFlag> GetProjectCondFlagArray(string ipAddress, string fullUrl, string non_mobile_no, string relate_mobile, string internetAccountNumber, string subNetworkType, string serenade_flag_old, string mobile_checkright, string mobile_get_benefit, out string fmc_specail_flag, out string serenade_flag)
        {
            var result = new List<ProjectCondFlag>();
            serenade_flag = serenade_flag_old;
            fmc_specail_flag = null;
            var p_projectname = "";
            var resultMass2 = new evESeServiceQueryMassCommonAccountInfoModel();

            //2.1 Call Mass Option 4 input mobile (relate_mobile)
            var massQuery = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "4",
                inMobileNo = relate_mobile,
                inCardNo = "",
                inCardType = "",
                Page = "ChangePromotion/ConfirmChange",
                Username = non_mobile_no,
                FullUrl = fullUrl,
                ClientIP = "ipAddress"
            };
            var massResult = _queryProcessor.Execute(massQuery);

            if (massResult != null && massResult.errorMessage == "")
            {
                p_projectname = massResult.projectName;
                if (massResult.outInstanceNameFBBrowType == "mainCause" && massResult.outInstanceNameFBBnonMobileNumber == non_mobile_no)
                {
                    //2.2.1 rowtype = 'mainCause' and mobileNo = 88 login
                    if (GetChangePromotionCaNoCompare(ipAddress, fullUrl, non_mobile_no, relate_mobile, internetAccountNumber, out resultMass2))
                    {
                        //LOGIC GetChangePromotionFmcSpecailFlag
                        fmc_specail_flag = GetChangePromotionFmcSpecailFlag(fullUrl, subNetworkType, relate_mobile, p_projectname, mobile_checkright, mobile_get_benefit, resultMass2);
                    }
                    else
                    {
                        fmc_specail_flag = null;
                    }
                }
                else if (massResult.outInstanceNameFBBrowType == "mainCause" && massResult.outInstanceNameFBBnonMobileNumber != non_mobile_no)
                {
                    //2.2.2 rowtype = 'mainCause' and mobileNo = 88 login
                    serenade_flag = null;
                    fmc_specail_flag = null;
                }
                else
                {
                    //2.2.3 rowtype != 'mainCause'
                    if (GetChangePromotionCaNoCompare(ipAddress, fullUrl, non_mobile_no, relate_mobile, internetAccountNumber, out resultMass2))
                    {
                        //LOGIC GetChangePromotionFmcSpecailFlag
                        fmc_specail_flag = GetChangePromotionFmcSpecailFlag(fullUrl, subNetworkType, relate_mobile, p_projectname, mobile_checkright, mobile_get_benefit, resultMass2);
                    }
                    else
                    {
                        fmc_specail_flag = null;
                    }
                }
            }
            else
            {
                fmc_specail_flag = null;
            }

            result.Add(new ProjectCondFlag { projectCondFlag = "SERENADE_FLAG", projectCondValue = serenade_flag });
            result.Add(new ProjectCondFlag { projectCondFlag = "FMPA_FLAG", projectCondValue = "" });
            result.Add(new ProjectCondFlag { projectCondFlag = "CVM_FLAG", projectCondValue = "" });
            result.Add(new ProjectCondFlag { projectCondFlag = "FMC_SPECIAL_FLAG", projectCondValue = fmc_specail_flag });
            result.Add(new ProjectCondFlag { projectCondFlag = "NON_RES_FLAG", projectCondValue = "" });

            return result;
        }

        private bool GetChangePromotionCaNoCompare(string ipAddress, string fullUrl, string non_mobile_no, string relate_mobile, string internetAccountNumber, out evESeServiceQueryMassCommonAccountInfoModel resultMass2)
        {
            var result = false;
            resultMass2 = new evESeServiceQueryMassCommonAccountInfoModel();

            var mobileAccountNumber = "";
            var massOp2Query = new evESeServiceQueryMassCommonAccountInfoQuery
            {
                inOption = "2",
                inMobileNo = relate_mobile,
                inCardNo = "",
                inCardType = "",
                Page = "ChangePromotion/ConfirmChange",
                Username = non_mobile_no,
                FullUrl = fullUrl,
                ClientIP = ipAddress
            };
            var massOp2Result = _queryProcessor.Execute(massOp2Query);
            if (massOp2Result != null && massOp2Result.errorMessage == "")
            {
                mobileAccountNumber = massOp2Result.outAccountNumber;
                resultMass2 = massOp2Result;
            }
            if (!string.IsNullOrEmpty(internetAccountNumber) && !string.IsNullOrEmpty(mobileAccountNumber) && internetAccountNumber == mobileAccountNumber)
            {
                result = true;
            }

            return result;
        }

        private string GetChangePromotionFmcSpecailFlag(string FullUrl, string subNetworkType, string AisAirNumber, string p_projectname, string mobile_checkright, string mobile_get_benefit, evESeServiceQueryMassCommonAccountInfoModel resultMass2)
        {
            string p_fmc_special_flag = null;
            string priceExclVatData = "";
            string serviceYearData = "";
            string accountCategory = "";
            string sffServiceYear = "";

            if (resultMass2 != null)
            {
                accountCategory = resultMass2.outAccountCategory;
                sffServiceYear = resultMass2.outDayOfServiceYear;
            }

            if ((subNetworkType == "POSTPAID") && (accountCategory == "Residential"))
            {
                p_fmc_special_flag = "N";

                var fmcSpecialDateFr = 0;
                var fmcSpecialDateTo = 0;
                var fmcSpecialPrice = 0;
                var fmcSpecialCfg = LovData.Where(
                    l => (!string.IsNullOrEmpty(l.Type) && l.Type == "FMC_SPECIAL_CONSTANT")).ToList();

                if (fmcSpecialCfg.Any())
                {
                    var fmcSpecialDateCfg = fmcSpecialCfg.FirstOrDefault(n => n.Name == "SERVICE_BETWEEN_DATE");
                    if (fmcSpecialDateCfg != null)
                    {
                        fmcSpecialDateFr = fmcSpecialDateCfg.LovValue1.ToSafeInteger();
                        fmcSpecialDateTo = fmcSpecialDateCfg.LovValue2.ToSafeInteger();
                    }

                    var fmcSpecialPriceCfg = fmcSpecialCfg.FirstOrDefault(n => n.Name == "PRICE_EXCLUDE_VAT");
                    if (fmcSpecialPriceCfg != null)
                    {
                        fmcSpecialPrice = fmcSpecialPriceCfg.LovValue1.ToSafeInteger();
                    }
                }

                var psInfoRequest = new evESQueryPersonalInformationQuery
                {
                    mobileNo = AisAirNumber.ToSafeString(),
                    option = "2",
                    FullUrl = FullUrl
                };
                var psInfoResult = _queryProcessor.Execute(psInfoRequest);

                if (psInfoResult.Any())
                {
                    decimal priceExclVat = 0;
                    decimal serviceYear = sffServiceYear.ToSafeInteger();
                    serviceYearData = sffServiceYear.ToSafeString();

                    //19.2 Set FMPA Flag By Package Main
                    var psInfoRow = psInfoResult.FirstOrDefault(item =>
                        item.productClass == "Main" &&
                        item.startDt.ToDateNotTime() <= DateTime.Now.Date &&
                        item.endDt.ToDateNotTime() >= DateTime.Now.Date);
                    if (psInfoRow != null)
                    {
                        priceExclVat = psInfoRow.priceExclVat;
                        priceExclVatData = priceExclVat.ToSafeString();
                    }
                    if ((priceExclVat >= fmcSpecialPrice) &&
                        (serviceYear >= fmcSpecialDateFr && serviceYear <= fmcSpecialDateTo) &&
                        (string.IsNullOrEmpty(p_projectname)))
                    {
                        p_fmc_special_flag = "Y";
                    }
                    else if ((priceExclVat >= fmcSpecialPrice) &&
                        (serviceYear >= fmcSpecialDateFr && serviceYear <= fmcSpecialDateTo))
                    {
                        //R20.7 ChangePromotionCheckRight (non change relate mobile)
                        if (AisAirNumber.ToSafeString() == mobile_checkright.ToSafeString() || AisAirNumber.ToSafeString() == mobile_get_benefit.ToSafeString())
                        {
                            var configFMC = base.LovData.FirstOrDefault(l => l.Type == "CONFIG" && l.Name == "CONFIG_CP_FMC_SAME_MOBILE" && l.LovValue1 == p_projectname);
                            if (configFMC != null)
                            {
                                p_fmc_special_flag = configFMC.LovValue2.ToSafeString();
                            }
                        }
                    }
                }
            }

            return p_fmc_special_flag;
        }

        public JsonResult GetPersonalPromotion(string non_mobile_no)
        {
            StringBuilder htmlResult = new StringBuilder();
            StringBuilder htmlResult2 = new StringBuilder();
            List<evESQueryPersonalInformationModel> PersonalInformation2 = evESQueryPersonalInformation(non_mobile_no, "2");
            List<evESQueryPersonalInformationModel> PersonalInformation3 = evESQueryPersonalInformation(non_mobile_no, "3");
            List<evESQueryPersonalInformationModel> PersonalInformation4 = evESQueryPersonalInformation(non_mobile_no, "4");

            foreach (var tmp in PersonalInformation2)
            {
                string grpdiv = "<div id={0}>" +
                    "<input type='hidden' name='productCd' value='{1}' />" +
                    "<input type='hidden' name='promotionName' value='{2}' />" +
                    "<input type='hidden' name='productClass' value='{3}' />" +
                    "<input type='hidden' name='productGroup' value='{4}' />" +
                    "<input type='hidden' name='productPkg' value='{5}' />" +
                    "<input type='hidden' name='descThai' value='{6}' />" +
                    "<input type='hidden' name='descEng' value='{7}' />" +
                    "<input type='hidden' name='inStatementThai' value='{8}' />" +
                    "<input type='hidden' name='inStatementEng' value='{9}' />" +
                    "<input type='hidden' name='startDt' value='{10}' />" +
                    "<input type='hidden' name='endDt' value='{11}' />" +
                    "<input type='hidden' name='productSeq' value='{12}' />" +
                    "</div>";

                htmlResult.Append(string.Format(grpdiv, tmp.productCd, tmp.productCd, tmp.promotionName, tmp.productClass, tmp.produuctGroup, tmp.productPkg, tmp.descThai, tmp.descEng, tmp.inStatementThai, tmp.inStatementEng, tmp.startDt, tmp.endDt, tmp.productSeq));
            }

            string productCd = "";
            string billCycle = "";
            string checkHavePlayBox = "N";
            string checkHave2Main = "N";
            string ServiceCodePBOX = "";
            string usePackDIV =
                    "<input type='hidden' id='productCdUse' value='{0}' />" +
                    "<input type='hidden' id='billCycle' value='{1}' />" +
                    "<input type='hidden' id='checkHavePlayBox' value='{2}' />" +
                    "<input type='hidden' id='checkHave2Main' value='{3}' />" +
                    "<input type='hidden' id='ServiceCodePBOX' value='{4}' />";

            if (PersonalInformation2 != null && PersonalInformation2.Count > 0)
            {
                var PersonalInformationTMP = PersonalInformation2.Where(t => t.productClass == "Main").ToList();

                if (PersonalInformationTMP != null && PersonalInformationTMP.Count > 1)
                {
                    string MainCurrent = "";
                    string MainFuture = "";
                    int tmpMainCount = 1;
                    //var tmpMain = PersonalInformationTMP.OrderBy(t => t.startDt).ToList();


                    PersonalInformationTMP.Sort((a, b) =>
                    {

                        DateTime startA = a.startDt.ToDateTime() ?? DateTime.Now;
                        DateTime startB = b.startDt.ToDateTime() ?? DateTime.Now;
                        // compare b to a to get descending order
                        int r = startA.CompareTo(startB);

                        return r;
                    });

                    foreach (var item in PersonalInformationTMP)
                    {
                        if (tmpMainCount == 1)
                            MainCurrent = item.productCd;

                        if (tmpMainCount == 2)
                            MainFuture = item.productCd;

                        tmpMainCount++;
                    }

                    //if (CheckUnlockOrder(MainCurrent, MainFuture, non_mobile_no) != "Y")
                    checkHave2Main = "Y";
                }

                productCd = PersonalInformationTMP.Select(t => t.productCd).FirstOrDefault().ToSafeString();

                if (PersonalInformation3 != null && PersonalInformation3.Count > 0)
                {
                    var config = base.LovData.Where(l => l.Type == "SCREEN" && l.Name == "AIS_PLAY_BOX").Select(t => t.LovValue1).ToList();

                    if (config != null && config.Count > 0)
                    {
                        var PersonalInformation3HavePlayBox = PersonalInformation3.Where(t => config.Contains(t.productCd)).ToList();
                        if (PersonalInformation3HavePlayBox != null && PersonalInformation3HavePlayBox.Count > 0)
                        {
                            checkHavePlayBox = "Y";
                        }
                        else
                        {
                            ServiceCodePBOX = config.FirstOrDefault();
                        }
                    }
                }

                if (PersonalInformation4 != null && PersonalInformation4.Count > 0)
                {
                    billCycle = PersonalInformation4.Where(t => t.billCycle != null && t.billCycle != "").Select(t => t.billCycle).FirstOrDefault().ToSafeString();
                }
            }

            htmlResult2.Append(string.Format(usePackDIV, productCd, billCycle, checkHavePlayBox, checkHave2Main, ServiceCodePBOX));

            return Json(new { result1 = htmlResult.ToString(), result2 = htmlResult2.ToString() }, JsonRequestBehavior.AllowGet);
        }

        //AWARE_R20.02
        public JsonResult getDataTimeSlotByBillCycle(string non_mobile_no)
        {
            var PersonalInformation2 = evESQueryPersonalInformation(non_mobile_no, "2");

            var billCycle = "";
            var mainCurrent = "";
            var mainFuture = "";
            var billCycleHave2MainFlag = "N";
            var billCycleTimeSlotStartDate = "";
            var billCycleTimeSlotDayAdd = "";
            var billCycleTimeSlotDtpLenght = "";
            var billCycleTimeSlotDayLenght = "";

            if (PersonalInformation2 != null && PersonalInformation2.Count > 0)
            {
                var PersonalInformationTMP = PersonalInformation2.Where(t => t.productClass == "Main").ToList();

                if (PersonalInformationTMP != null && PersonalInformationTMP.Count > 1)
                {
                    int tmpMainCount = 1;

                    PersonalInformationTMP.Sort((a, b) =>
                    {
                        DateTime startA = a.startDt.ToDateTime() ?? DateTime.Now;
                        DateTime startB = b.startDt.ToDateTime() ?? DateTime.Now;
                        int r = startA.CompareTo(startB);
                        return r;
                    });

                    foreach (var item in PersonalInformationTMP)
                    {
                        if (tmpMainCount == 1)
                        {
                            mainCurrent = item.productCd;
                        }
                        if (tmpMainCount == 2)
                        {
                            mainFuture = item.productCd;
                        }
                        tmpMainCount++;

                        billCycleHave2MainFlag = "Y";
                    }
                }
            }

            if (billCycleHave2MainFlag == "Y")
            {
                var PersonalInformation4 = evESQueryPersonalInformation(non_mobile_no, "4");
                if (PersonalInformation4 != null && PersonalInformation4.Count > 0)
                {
                    billCycle = PersonalInformation4.Where(t => t.billCycle != null && t.billCycle != "").Select(t => t.billCycle).FirstOrDefault().ToSafeString();
                }

                var dLenght = "";
                var mLenght = "";
                var pLenght = "";
                var configDate = base.LovData.Where(l => l.Type == "CONFIG" && l.Name == "TIME_SLOT_BILL_CYCLE").ToList();
                if (configDate.Any())
                {
                    var rowDate = configDate.FirstOrDefault();
                    if (rowDate != null)
                    {
                        dLenght = rowDate.LovValue1;
                        mLenght = rowDate.LovValue2;
                        pLenght = rowDate.LovValue3;
                    }
                }

                var dateCycle = "";
                var config = base.LovData.Where(l => l.Type == "BILL_CYCLE" && !string.IsNullOrEmpty(l.Text)).ToList();
                if (config.Any())
                {
                    var rowBill = config.FirstOrDefault(f => f.Text == billCycle);
                    if (rowBill != null)
                    {
                        dateCycle = rowBill.LovValue1;
                    }
                }

                if (!string.IsNullOrEmpty(dLenght) && !string.IsNullOrEmpty(dLenght) && !string.IsNullOrEmpty(dateCycle))
                {
                    int monthLenght = mLenght.ToSafeInteger();
                    int cycleDay = dateCycle.ToSafeInteger();

                    int currentDay = 0;
                    int currentMonth = 0;
                    int currentYear = 0;

                    if (cycleDay < DateTime.Now.Day)
                    {
                        var current1 = DateTime.Now.AddMonths(monthLenght);
                        currentDay = cycleDay;
                        currentMonth = current1.Month;
                        currentYear = current1.Year;
                    }
                    else
                    {
                        var current2 = DateTime.Now;
                        currentDay = cycleDay;
                        currentMonth = current2.Month;
                        currentYear = current2.Year;
                    }

                    var billCycleStartDate = new DateTime(currentYear, currentMonth, currentDay);
                    billCycleTimeSlotStartDate = billCycleStartDate.ToString("dd/MM/yyyy");
                    billCycleTimeSlotDayAdd = ((billCycleStartDate - DateTime.Now).Days + 1).ToString();
                    billCycleTimeSlotDtpLenght = ((billCycleStartDate - DateTime.Now).Days + pLenght.ToSafeInteger()).ToSafeString();
                    billCycleTimeSlotDayLenght = dLenght.ToSafeInteger().ToSafeString();
                }
            }

            return Json(new
            {
                BillCycle = billCycle,
                MainCurrent = mainCurrent,
                MainFuture = mainFuture,
                BillCycleHave2MainFlag = billCycleHave2MainFlag,
                BillCycleTimeSlotStartDate = billCycleTimeSlotStartDate,
                BillCycleTimeSlotDayAdd = billCycleTimeSlotDayAdd,
                BillCycleTimeSlotDtpLenght = billCycleTimeSlotDtpLenght,
                BillCycleTimeSlotDayLenght = billCycleTimeSlotDayLenght
            }, JsonRequestBehavior.AllowGet);
        }

        public List<evESQueryPersonalInformationModel> evESQueryPersonalInformation(string mobileNo, string option)
        {
            // 17.6 Interface Log Add Url
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            List<evESQueryPersonalInformationModel> result = new List<evESQueryPersonalInformationModel>();
            try
            {
                var query = new evESQueryPersonalInformationQuery()
                {
                    mobileNo = mobileNo,
                    option = option,
                    FullUrl = FullUrl
                };
                result = _queryProcessor.Execute(query);
            }
            catch { }

            return result;
        }

        public string CheckNoToPB(string promocodeCurrent, string promocodeFuturn, string statusOption, string non_mobile_no)
        {
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            CheckNoToPBQuery query = new CheckNoToPBQuery
            {
                p_sff_promocode_current = promocodeCurrent,
                p_sff_promocode_futurn = promocodeFuturn,
                P_status_option = statusOption,
                TRANSACTION_ID = non_mobile_no,
                FULL_URL = FullUrl
            };

            string result = "";
            List<CheckNoToPB> results = new List<CheckNoToPB>();
            try
            {
                results = _queryProcessor.Execute(query);
                if (results != null && results.Count > 0)
                {
                    result = results.FirstOrDefault().CHECK_NO_TO_PB.ToSafeString();
                }
            }
            catch { }

            return result;
        }

        public string CheckUnlockOrder(string promocodeCurrent, string promocodeFuturn, string non_mobile_no)
        {
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            CheckUnlockOrderQuery query = new CheckUnlockOrderQuery
            {
                p_sff_promocode_current = promocodeCurrent,
                p_sff_promocode_futurn = promocodeFuturn,
                TRANSACTION_ID = non_mobile_no,
                FULL_URL = FullUrl
            };

            string result = "";
            try
            {
                result = _queryProcessor.Execute(query);
            }
            catch { }

            return result;
        }

        public List<GetInputGetListPackageBySFFPROMO> GetInputGetListPackageBySFFPROMO(string promocodeCurrent, string promocodeFuturn, string non_mobile_no)
        {
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            GetInputGetListPackageBySFFPROMOQuery query = new GetInputGetListPackageBySFFPROMOQuery
            {
                p_sff_promocode_current = promocodeCurrent,
                p_sff_promocode_futurn = promocodeFuturn,
                P_NON_MOBILE = non_mobile_no,
                TRANSACTION_ID = non_mobile_no,
                FULL_URL = FullUrl
            };
            List<GetInputGetListPackageBySFFPROMO> results = new List<GetInputGetListPackageBySFFPROMO>();
            try
            {
                results = _queryProcessor.Execute(query);
            }
            catch { }

            return results;
        }

        public List<PackageModel> GetPackageListBySFFPromoForChangPro(string P_SFF_PROMOCODE, string P_PRODUCT_SUBTYPE, string P_OWNER_PRODUCT, string EXISTING_REQ, string non_mobile_no)
        {
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            GetPackageListBySFFPromoForChangProQuery query = new GetPackageListBySFFPromoForChangProQuery
            {
                P_SFF_PROMOCODE = P_SFF_PROMOCODE,
                P_PRODUCT_SUBTYPE = P_PRODUCT_SUBTYPE,
                P_OWNER_PRODUCT = P_OWNER_PRODUCT,
                EXISTING_REQ = EXISTING_REQ,
                TRANSACTION_ID = non_mobile_no,
                FULL_URL = FullUrl
            };

            List<PackageModel> results = new List<PackageModel>();
            try
            {
                results = _queryProcessor.Execute(query);
            }
            catch { }

            return results;
        }

        [HttpPost]
        public JsonResult GetAppointment(string NON_MOBILE, string ID_CARD, string BILL_CYCLE, string LANQUAGE_SCREEN, string timeSlotId, string P_ADDRESS_ID,
            string INSTALL_ADDRESS_1, string INSTALL_ADDRESS_2, string INSTALL_ADDRESS_3, string INSTALL_ADDRESS_4, string INSTALL_ADDRESS_5)
        {

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();



            GetDataForAppointmentQuery getDataForAppointmentQuery = new GetDataForAppointmentQuery
            {
                NON_MOBILE = NON_MOBILE,
                ID_CARD = ID_CARD,
                BILL_CYCLE = BILL_CYCLE,
                LANQUAGE_SCREEN = LANQUAGE_SCREEN,
                P_ADDRESS_ID = P_ADDRESS_ID,
                INSTALL_ADDRESS_1 = INSTALL_ADDRESS_1,
                INSTALL_ADDRESS_2 = INSTALL_ADDRESS_2,
                INSTALL_ADDRESS_3 = INSTALL_ADDRESS_3,
                INSTALL_ADDRESS_4 = INSTALL_ADDRESS_4,
                INSTALL_ADDRESS_5 = INSTALL_ADDRESS_5,
                TRANSACTION_ID = NON_MOBILE,
                FULL_URL = FullUrl
            };

            List<DataForAppointment> dataForAppointments = new List<DataForAppointment>();
            try
            {
                dataForAppointments = _queryProcessor.Execute(getDataForAppointmentQuery);
            }
            catch { }

            List<FBSSTimeSlotChangePro> data = new List<FBSSTimeSlotChangePro>();
            DataForAppointment dataForAppointment = new DataForAppointment();
            GetAppointmentChageProQuery getAppointmentChageProQuery = new GetAppointmentChageProQuery();
            if (dataForAppointments != null && dataForAppointments.Count > 0)
            {
                dataForAppointment = dataForAppointments.FirstOrDefault();
                if (dataForAppointment != null)
                {
                    getAppointmentChageProQuery = new GetAppointmentChageProQuery
                    {
                        INSTALLATION_DATE = dataForAppointment.INSTALLATION_DATE.ToSafeString(),
                        ACCESS_MODE = dataForAppointment.ACCESS_MODE.ToSafeString(),
                        PROD_SPEC_CODE = dataForAppointment.PROD_SPEC_CODE.ToSafeString(),
                        ADDRESS_ID = dataForAppointment.ADDRESS_ID.ToSafeString(),
                        DAYS = dataForAppointment.DAYS.ToSafeString(),
                        SUBDISTRICT = dataForAppointment.SUBDISTRICT.ToSafeString(),
                        POSTCODE = dataForAppointment.POSTCODE.ToSafeString(),
                        SUB_ACCESS_MODE = dataForAppointment.SUB_ACCESS_MODE.ToSafeString(),
                        TRANSACTION_ID = NON_MOBILE,
                        FULL_URL = FullUrl
                    };

                    try
                    {
                        data = _queryProcessor.Execute(getAppointmentChageProQuery);
                    }
                    catch { }
                }
            }

            var strBuilder = new StringBuilder();
            var strRemarkBuilder = new StringBuilder();
            var installationDate = new DateTime();
            var strFBSSTimeSlot = "";
            bool isThai = false;
            isThai = LANQUAGE_SCREEN == "T" ? true : false;

            CultureInfo usDtfi = new CultureInfo("en-GB", false);
            installationDate = DateTime.ParseExact(dataForAppointment.INSTALLATION_DATE.ToSafeString(), "yyyy-MM-dd", usDtfi, DateTimeStyles.None);

            if (data != null)
            {
                if (data.Count > 0)
                {
                    #region Time column
                    var listDay = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                        p.Name.Contains("L_INSTALL_WEEK_OF_DAY")).OrderBy(p => p.OrderBy).ToList();

                    var listMonth = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                        p.Name.Contains("L_INSTALL_MONTH_OF_YEAR")).OrderBy(p => p.OrderBy).ToList();

                    var lblTime = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue1 : base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue2;

                    var oldDate = data[0].AppointmentDate;
                    int remarkTop = 0;


                    //L_INSTALL_TIME
                    strBuilder.Append("<div style=\"float:left;\">");
                    strBuilder.Append("<div class=\"select-header-show\">");
                    strBuilder.Append("</div>");
                    strBuilder.Append("<div class=\"time-header\">");
                    strBuilder.Append("<label>" + lblTime + "</label> ");
                    strBuilder.Append("</div>");

                    for (int i = 0; i < data.Count; i++)
                    {
                        if (oldDate == data[i].AppointmentDate && data[i].TimeSlot != "08:00-10:00")
                        {
                            strBuilder.Append("<div class=\"time-item\">");
                            strBuilder.Append("<label>" + data[i].TimeSlot + "</label>");
                            strBuilder.Append("</div>");
                            remarkTop++;
                        }
                    }

                    strBuilder.Append("<div class=\"time-footer\">");
                    strBuilder.Append("</div>");
                    strBuilder.Append("</div>");

                    #endregion

                    #region Day column
                    int LIMIT_TIME_FOR_FAST_JOB = 0;
                    var tmp_limittime = base.LovData.Where(p => p.Name == "LIMIT_TIME_FOR_FAST_JOB").OrderBy(p => p.OrderBy).ToList();
                    if (tmp_limittime != null)
                        if (tmp_limittime.Any())
                            LIMIT_TIME_FOR_FAST_JOB = int.Parse(tmp_limittime.FirstOrDefault().LovValue1);
                    DateTime Curr_dt = DateTime.Now;

                    for (int i = 0; i < data.Count; i++)
                    {
                        DateTime AppointmentDateTMP;
                        AppointmentDateTMP = DateTime.ParseExact(data[i].AppointmentDate.ToSafeString(), "yyyy-MM-dd", usDtfi, DateTimeStyles.None);

                        if (oldDate != data[i].AppointmentDate || i == 0)
                        {
                            oldDate = data[i].AppointmentDate;

                            if (i != 0)
                            {
                                strBuilder.Append("<div class=\"day-footer-show\">");
                                strBuilder.Append("<div class=\"day-footer\">");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                            }

                            var dayNum = AppointmentDateTMP.Day.ToString();
                            var dayName = isThai ? listDay[(int)AppointmentDateTMP.DayOfWeek].LovValue1 : listDay[(int)AppointmentDateTMP.DayOfWeek].LovValue2;
                            var monthName = isThai ? listMonth[AppointmentDateTMP.Month - 1].LovValue1 : listMonth[AppointmentDateTMP.Month - 1].LovValue2;

                            strBuilder.Append("<div style=\"float:left;\">");
                            strBuilder.Append("<div class=\"select-header-show\">");
                            strBuilder.Append("<div class=\"select-header\">");
                            strBuilder.Append("<span style=\"color:white;\">" + dayName + "</span>");
                            strBuilder.Append("</div>");
                            strBuilder.Append("</div>");
                            strBuilder.Append("<div class=\"day-header\">");
                            strBuilder.Append("<div class=\"header-label-playbox\">");
                            strBuilder.Append("<span>" + dayName + "</span>");
                            strBuilder.Append("<p style=\"margin: 0 0 2px;\"></p>");
                            strBuilder.Append("<span>" + dayNum + " " + monthName + "</span>");
                            strBuilder.Append("</div>");
                            strBuilder.Append("<div class=\"header-label-playbox-hidden\">");
                            strBuilder.Append("<span>" + dayNum + "</span>");
                            strBuilder.Append("<p style=\"margin: 0 0 1px;\"></p>");
                            strBuilder.Append("<span>" + monthName + "</span>");
                            strBuilder.Append("</div>");
                            strBuilder.Append("</div>");
                        }

                        if (oldDate == data[i].AppointmentDate && data[i].TimeSlot != "08:00-10:00")
                        {
                            var slot = data[i].InstallationCapacity.Split('/');

                            strBuilder.Append("<div class=\"day-item\" onclick=\"onDayItemClick(this,'" + timeSlotId + "');\">");
                            strBuilder.Append("<div class=\"day-item-circle\">");

                            if (Convert.ToInt32(slot[0]) <= 0)
                                strBuilder.Append("<i class=\"fa fa-circle color-red\"></i>");
                            else
                            {
                                DateTime TimeslotPeriod = new DateTime(AppointmentDateTMP.Year,
                                    AppointmentDateTMP.Month,
                                    AppointmentDateTMP.Day,
                                    int.Parse(data[i].TimeSlot.Substring(0, 2)),
                                    int.Parse(data[i].TimeSlot.Substring(3, 2)),
                                    0);
                                if (TimeslotPeriod > Curr_dt.AddMinutes(LIMIT_TIME_FOR_FAST_JOB))
                                    strBuilder.Append("<i class=\"fa fa-circle color-gray\"></i>");
                                else
                                    strBuilder.Append("<i class=\"fa fa-circle color-red\"></i>");
                            }

                            strBuilder.Append("<div style=\"display:none;\">");
                            strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"" + data[i].TimeSlotId + "\"/>");
                            strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + AppointmentDateTMP.ToString("yyyy/MM/dd") + "\"/>");
                            strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + data[i].TimeSlot + "\"/>");
                            strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"" + data[i].InstallationCapacity + "\"/>");
                            strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + AppointmentDateTMP.ToString("dd/MM/yyyy") + "\"/>");
                            strBuilder.Append("</div>");

                            strBuilder.Append("</div>");
                            strBuilder.Append("</div>");
                        }
                    }

                    strBuilder.Append("<div class=\"day-footer-show\">");
                    strBuilder.Append("<div class=\"day-footer\">");
                    strBuilder.Append("</div>");
                    strBuilder.Append("</div>");
                    strBuilder.Append("</div>");
                    strBuilder.Append("<div style=\"clear: both;\"></div>");
                    #endregion

                    #region Remark
                    string insSelected = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_SELECTED").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_SELECTED").Select(p => p.LovValue2).FirstOrDefault();
                    string insAvailable = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_AVAILABLE").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_AVAILABLE").Select(p => p.LovValue2).FirstOrDefault();
                    string insNotAvailable = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_NOT_ AVAILABLE").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_NOT_ AVAILABLE").Select(p => p.LovValue2).FirstOrDefault();

                    strRemarkBuilder.Append("<fieldset class=\"timeslot-remark center-block\">");
                    strRemarkBuilder.Append("<div style=\"margin-top:3px;\">");
                    strRemarkBuilder.Append("<i class=\"fa fa-circle color-green-remark\"></i>");
                    strRemarkBuilder.Append("&nbsp&nbsp<label>" + insSelected + "</label>");
                    strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-gray-remark\"></i>");
                    strRemarkBuilder.Append("&nbsp&nbsp<label>" + insAvailable + "</label>");
                    strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-red-remark\"></i>");
                    strRemarkBuilder.Append("&nbsp&nbsp<label>" + insNotAvailable + "</label>");
                    strRemarkBuilder.Append("</div>");
                    strRemarkBuilder.Append("</fieldset>");

                    #endregion
                }
                else
                {
                    strBuilder.Append("<div style=\"display:none;\"> List data = 0");
                    strBuilder.Append(", installation_Date = " + getAppointmentChageProQuery.INSTALLATION_DATE);
                    strBuilder.Append(", access_Mode = " + getAppointmentChageProQuery.ACCESS_MODE);
                    strBuilder.Append(", address_Id = " + getAppointmentChageProQuery.ADDRESS_ID);
                    strBuilder.Append(", days = " + getAppointmentChageProQuery.DAYS);
                    strBuilder.Append(", productSpecCode =" + getAppointmentChageProQuery.PROD_SPEC_CODE);
                    strBuilder.Append(", ExtendingAttributes = ");
                    strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("dd/MM/yyyy") + "\"/>");
                    strBuilder.Append("</div>");
                }
            }
            else
            {
                strBuilder.Append("<div style=\"display:none;\"> Data null");
                strBuilder.Append(", installation_Date = " + getAppointmentChageProQuery.INSTALLATION_DATE);
                strBuilder.Append(", access_Mode = " + getAppointmentChageProQuery.ACCESS_MODE);
                strBuilder.Append(", address_Id = " + getAppointmentChageProQuery.ADDRESS_ID);
                strBuilder.Append(", days = " + getAppointmentChageProQuery.DAYS);
                strBuilder.Append(", productSpecCode =" + getAppointmentChageProQuery.PROD_SPEC_CODE);
                strBuilder.Append(", ExtendingAttributes = ");
                strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("dd/MM/yyyy") + "\"/>");
                strBuilder.Append("</div>");
            }

            return Json(
                new { timeSlotData = strBuilder.ToSafeString(), timeSlotRemark = strRemarkBuilder.ToSafeString(), },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ReserveTimeslot(string NON_MOBILE, string ID_CARD, string APPOINTMENT_DATE, string TIME_SLOT, string LANQUAGE_SCREEN, string P_ADDRESS_ID,
            string INSTALL_ADDRESS_1, string INSTALL_ADDRESS_2, string INSTALL_ADDRESS_3, string INSTALL_ADDRESS_4, string INSTALL_ADDRESS_5)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            GetDataForReserveTimeslotQuery getDataForReserveTimeslotQuery = new GetDataForReserveTimeslotQuery()
            {
                NON_MOBILE = NON_MOBILE,
                ID_CARD = ID_CARD,
                APPOINTMENT_DATE = APPOINTMENT_DATE,
                TIME_SLOT = TIME_SLOT,
                LANQUAGE_SCREEN = LANQUAGE_SCREEN,
                P_ADDRESS_ID = P_ADDRESS_ID,
                INSTALL_ADDRESS_1 = INSTALL_ADDRESS_1,
                INSTALL_ADDRESS_2 = INSTALL_ADDRESS_2,
                INSTALL_ADDRESS_3 = INSTALL_ADDRESS_3,
                INSTALL_ADDRESS_4 = INSTALL_ADDRESS_4,
                INSTALL_ADDRESS_5 = INSTALL_ADDRESS_5,
                TRANSACTION_ID = NON_MOBILE,
                FULL_URL = FullUrl
            };

            List<DataForReserveTimeslot> dataForReserveTimeslots = new List<DataForReserveTimeslot>();
            try
            {
                dataForReserveTimeslots = _queryProcessor.Execute(getDataForReserveTimeslotQuery);
            }
            catch { }

            ZTEReservedTimeslotRespModel zte_result = new ZTEReservedTimeslotRespModel();

            if (dataForReserveTimeslots != null && dataForReserveTimeslots.Count > 0)
            {
                var dataForReserveTimeslot = dataForReserveTimeslots.FirstOrDefault();
                ZTEReservedTimeslotQuery query = new ZTEReservedTimeslotQuery()
                {
                    ACCESS_MODE = dataForReserveTimeslot.ACCESS_MODE,
                    ADDRESS_ID = dataForReserveTimeslot.ADDRESS_ID,
                    APPOINTMENT_DATE = dataForReserveTimeslot.APPOINTMENT_DATE.Replace("/", "-"),
                    LOCATION_CODE = dataForReserveTimeslot.LOCATION_CODE,
                    PROD_SPEC_CODE = dataForReserveTimeslot.PROD_SPEC_CODE,
                    TIME_SLOT = dataForReserveTimeslot.TIME_SLOT,
                    SUBDISTRICT = dataForReserveTimeslot.SUBDISTRICT,
                    POSTAL_CODE = dataForReserveTimeslot.POSTCODE,
                    ASSIGN_RULE = dataForReserveTimeslot.ASSIGN_RULE,
                    SUB_ACCESS_MODE = dataForReserveTimeslot.SUB_ACCESS_MODE,
                    TRANSACTION_ID = NON_MOBILE,
                    FullUrl = FullUrl
                };
                zte_result = _queryProcessor.Execute(query);
            }
            else
            {
                zte_result.RESERVED_ID = "";
                zte_result.RESULT = "-1";
                zte_result.RESULT_DESC = "ReservedTimeslot fail";
            }

            return Json(zte_result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DATAILChangepro(string NON_MOBILE, string BILL_CYCLE, string APPOINTMENT_DATE, string TIME_SLOT)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            GetDataDATAILChangeproQuery getDataDATAILChangeproQuery = new GetDataDATAILChangeproQuery()
            {
                P_BILL_CYCLE = BILL_CYCLE,
                P_APPOINTMENT_DATE = APPOINTMENT_DATE,
                P_TIME_SLOT = TIME_SLOT,
                TRANSACTION_ID = NON_MOBILE,
                FULL_URL = FullUrl
            };

            DataDATAILChangepro dataDATAILChangepro = new DataDATAILChangepro();
            try
            {
                dataDATAILChangepro = _queryProcessor.Execute(getDataDATAILChangeproQuery);
            }
            catch { }

            return Json(dataDATAILChangepro, JsonRequestBehavior.AllowGet);
        }

        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo, string INTERFACE_NODE)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = query.GetType().Name,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = INTERFACE_NODE,
                CREATED_BY = "FBBWEB",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd,
            string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogCommand.Handle(dbIntfCmd);
        }

        [HttpPost]
        public JsonResult GetInstallAddress(string NON_MOBILE)
        {
            Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "Process", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var listServiceAndPromotionByPackageTypeQuery = new evOMQueryListServiceAndPromotionByPackageTypeQuery
            {
                mobileNo = NON_MOBILE,
                idCard = "",
                FullUrl = FullUrl
            };
            evOMQueryListServiceAndPromotionByPackageTypeModel listServiceAndPromotionByPackageTypeData = new evOMQueryListServiceAndPromotionByPackageTypeModel();

            listServiceAndPromotionByPackageTypeData = _queryProcessor.Execute(listServiceAndPromotionByPackageTypeQuery);

            return Json(listServiceAndPromotionByPackageTypeData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetListChangePackage(string non_mobile_no,
            string relate_mobile,
            string old_relate_mobile,
            string project_name,
            string network_type,
            string old_package,
            string new_package,
            string mobileNumberContact,
            string relateMobileAction,
            string project_name_opt,
            string mobileCheckRight,
            string mobileCheckRight_opt,
            string mobileGetbenefit,
            string mobileGetbenefit_opt,
            string existingMobileFlag,
            string playBoxCode = "",
            string language = "")
        {

            var response = new List<ListChangePackageModel>();
            var GUIDKEY = string.Empty;
            var messageSummary = string.Empty;
            try
            {
                Session["FullUrl"] = this.Url.Action("ChangePackagePromotion", "GetListChangePackage", null, this.Request.Url.Scheme);
                string FullUrl = "";
                if (Session["FullUrl"] != null)
                    FullUrl = Session["FullUrl"].ToSafeString();
                var air_old_list = new List<AirChangePromotionCode>();
                var js = new JavaScriptSerializer();
                var deserializedAirItems = (object[])js.DeserializeObject(old_package);
                if (deserializedAirItems != null)
                {
                    foreach (Dictionary<string, object> newItem in deserializedAirItems)
                    {
                        var stmp = new SerializeAirchangePromotionJSonModel(newItem);
                        var air = new AirChangePromotionCode()
                        {
                            SFF_PROMOTION_CODE = stmp.SFF_PROMOTION_CODE,
                            startDt = stmp.startDt,
                            endDt = stmp.endDt,
                            PRODUCT_SEQ = stmp.PRODUCT_SEQ
                        };
                        air_old_list.Add(air);
                    }
                }

                var air_new_list = new List<AirChangePromotionCode>();
                deserializedAirItems = (object[])js.DeserializeObject(new_package);
                if (deserializedAirItems != null)
                {
                    foreach (Dictionary<string, object> newItem in deserializedAirItems)
                    {
                        var stmp = new SerializeAirchangePromotionJSonModel(newItem);
                        var air = new AirChangePromotionCode()
                        {
                            SFF_PROMOTION_CODE = stmp.SFF_PROMOTION_CODE,
                            startDt = stmp.startDt,
                            endDt = stmp.endDt,
                            PRODUCT_SEQ = stmp.PRODUCT_SEQ
                        };
                        air_new_list.Add(air);
                    }
                }

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (network_type == "PREPAID")
                {
                    network_type = "Pre-paid";
                }
                else if (network_type == "POSTPAID")
                {
                    network_type = "Post-paid";
                }
                else
                {
                    network_type = null;
                }

                List<AirChangePromotionCode> air_playbox_list = null;
                if (playBoxCode != "")
                {
                    air_playbox_list = new List<AirChangePromotionCode>();
                    var air = new AirChangePromotionCode()
                    {
                        SFF_PROMOTION_CODE = playBoxCode,
                        startDt = null,
                        endDt = null,
                        PRODUCT_SEQ = ""
                    };
                    air_playbox_list.Add(air);
                }

                var query = new GetListChangePackageQuery()
                {
                    non_mobile_no = non_mobile_no,
                    relate_mobile = relate_mobile,
                    current_project_name = project_name,
                    network_type = network_type,
                    AirChangePromotionCodeOldArray = air_old_list,
                    AirChangePromotionCodeNewArray = air_new_list,
                    AirChangePlayBoxPromotionCodeNewArray = air_playbox_list,
                    oldRelateMobile = old_relate_mobile,
                    acTion = relateMobileAction,
                    mobileNumberContact = mobileNumberContact,
                    current_project_name_opt = project_name_opt,
                    current_mobile_chk_right = mobileCheckRight,
                    current_mobile_chk_right_opt = mobileCheckRight_opt,
                    current_mobile_get_benefit = mobileGetbenefit,
                    current_mobile_get_benefit_opt = mobileGetbenefit_opt,
                    existing_mobile_flag = existingMobileFlag,
                    client_ip = ipAddress,
                    FullUrl = FullUrl,
                    Language = language
                };

                MappingNewInputMobile(query);

                response = _queryProcessor.Execute(query);

                var dataPackage = new Dictionary<string, List<ListChangePackageModel>>();
                GUIDKEY = Guid.NewGuid().ToSafeString();
                if (response != null && response.Any())
                {
                    messageSummary = response.FirstOrDefault().wordingChangePromotionSummary;
                }

                dataPackage[GUIDKEY] = response;
                Session["ListChangePackageModel"] = dataPackage;

            }
            catch (Exception)
            {
                messageSummary = "-";
                return Json(new { response, GUIDKEY, messageSummary }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { response, GUIDKEY, messageSummary }, JsonRequestBehavior.AllowGet);
        }

        private void MappingNewInputMobile(GetListChangePackageQuery query)
        {
            var _mobileCheckRight = string.Empty;
            var _mobileGetbenefit = string.Empty;

            try
            {
                switch (string.IsNullOrEmpty(query.existing_mobile_flag) ? "" : query.existing_mobile_flag)
                {
                    case "":
                        if (string.IsNullOrEmpty(query.current_project_name))
                        {
                            _mobileCheckRight = query.relate_mobile;
                            return;
                        }

                        if (!string.IsNullOrEmpty(query.current_project_name))
                        {
                            _mobileCheckRight = query.current_mobile_chk_right;
                            _mobileGetbenefit = query.current_mobile_get_benefit;
                            return;
                        }

                        break;

                    case "N":
                        _mobileGetbenefit = query.current_mobile_get_benefit;
                        break;
                    case "Y":
                        if (string.IsNullOrEmpty(query.current_project_name))
                        {
                            _mobileCheckRight = query.relate_mobile;
                            _mobileGetbenefit = query.relate_mobile;
                            return;
                        }

                        if (!string.IsNullOrEmpty(query.current_project_name))
                        {
                            _mobileCheckRight = query.relate_mobile;
                            _mobileGetbenefit = query.current_mobile_get_benefit;

                            return;
                        }

                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {

            }
            finally
            {
                query.new_mobile_chk_right = _mobileCheckRight;
                query.new_mobile_get_benefit = _mobileGetbenefit;
            }
        }

        //R23.05 Max E-app term & condition
        [HttpPost]
        public JsonResult InsertFbbConsentLog(InsertFbbConsentLogQuery query)
        {
            if (string.IsNullOrEmpty(query.ip_address))
            {
                query.ip_address = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(query.ip_address))
            {
                query.ip_address = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //end R23.05 Max E-app term & condition

        //R24.03 Max DisplayChangePro
        public JsonResult DisplayChangePro(string non_mobile_no, string addressId, string access_mode, string SFF_PROMOTION_CODE)
        {
            GetPackageSequenceQuery query = new GetPackageSequenceQuery()
            {
                P_FIBRENET_ID = non_mobile_no,
                P_ADDRESS_ID = addressId,
                P_ACCESS_MODE = access_mode,
                P_PROMOTION_CODE = SFF_PROMOTION_CODE
            };
            var result = _queryProcessor.Execute(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //end R24.03 Max DisplayChangePro

        //R25.04 Kong GetProjectCondFlagArrayNew
        private List<ProjectCondFlag> GetProjectCondFlagArrayNew(string serenade_flag, string fmpa_flag, string cvm_flag, string fmc_specail_flag, string existing_mobile_flag)
        {
            var projectCondFlagArray = new List<ProjectCondFlag>();
            projectCondFlagArray.Add(new ProjectCondFlag { projectCondFlag = "SERENADE_FLAG", projectCondValue = serenade_flag });
            projectCondFlagArray.Add(new ProjectCondFlag { projectCondFlag = "FMPA_FLAG", projectCondValue = fmpa_flag });
            projectCondFlagArray.Add(new ProjectCondFlag { projectCondFlag = "CVM_FLAG", projectCondValue = cvm_flag });
            projectCondFlagArray.Add(new ProjectCondFlag { projectCondFlag = "FMC_SPECIAL_FLAG", projectCondValue = fmc_specail_flag });
            projectCondFlagArray.Add(new ProjectCondFlag { projectCondFlag = "NON_RES_FLAG", projectCondValue = "" });

            return projectCondFlagArray;
        }
    }

    public class SerializeAirchangePromotionJSonModel
    {
        public SerializeAirchangePromotionJSonModel(Dictionary<string, object> newFeature)
        {
            if (newFeature.ContainsKey("SFF_PROMOTION_CODE"))
            {
                SFF_PROMOTION_CODE = (string)newFeature["SFF_PROMOTION_CODE"];
            }
            if (newFeature.ContainsKey("startDt"))
            {
                startDt = (string)newFeature["startDt"];
            }
            if (newFeature.ContainsKey("endDt"))
            {
                endDt = (string)newFeature["endDt"];
            }
            if (newFeature.ContainsKey("PRODUCT_SEQ"))
            {
                PRODUCT_SEQ = (string)newFeature["PRODUCT_SEQ"];
            }
        }

        public string SFF_PROMOTION_CODE { get; set; }
        public string startDt { get; set; }
        public string endDt { get; set; }
        public string PRODUCT_SEQ { get; set; }
    }
    public class ServicePlayboxModel
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string SERVICE_PLAYBOX { get; set; }
    }
    public class SerializeAirPackageDisplayJSonModel
    {
        public SerializeAirPackageDisplayJSonModel(Dictionary<string, object> newFeature)
        {
            if (newFeature.ContainsKey("SFF_PROMOTION_CODE"))
            {
                SFF_PROMOTION_CODE = (string)newFeature["SFF_PROMOTION_CODE"];
            }
            if (newFeature.ContainsKey("promotionName"))
            {
                promotionName = (string)newFeature["promotionName"];
            }
            if (newFeature.ContainsKey("productClass"))
            {
                productClass = (string)newFeature["productClass"];
            }
            if (newFeature.ContainsKey("productGroup"))
            {
                productGroup = (string)newFeature["productGroup"];
            }
            if (newFeature.ContainsKey("productPkg"))
            {
                productPkg = (string)newFeature["productPkg"];
            }
            if (newFeature.ContainsKey("descThai"))
            {
                descThai = (string)newFeature["descThai"];
            }
            if (newFeature.ContainsKey("descEng"))
            {
                descEng = (string)newFeature["descEng"];
            }
            if (newFeature.ContainsKey("inStatementThai"))
            {
                inStatementThai = (string)newFeature["inStatementThai"];
            }
            if (newFeature.ContainsKey("inStatementEng"))
            {
                inStatementEng = (string)newFeature["inStatementEng"];
            }
            if (newFeature.ContainsKey("startDt"))
            {
                startDt = (string)newFeature["startDt"];
            }
            if (newFeature.ContainsKey("endDt"))
            {
                endDt = (string)newFeature["endDt"];
            }
            if (newFeature.ContainsKey("PRODUCT_SEQ"))
            {
                PRODUCT_SEQ = (string)newFeature["PRODUCT_SEQ"];
            }

        }

        public string SFF_PROMOTION_CODE { get; set; }
        public string promotionName { get; set; }
        public string productClass { get; set; }
        public string productGroup { get; set; }
        public string productPkg { get; set; }
        public string descThai { get; set; }
        public string descEng { get; set; }
        public string inStatementThai { get; set; }
        public string inStatementEng { get; set; }
        public string startDt { get; set; }
        public string endDt { get; set; }
        public string PRODUCT_SEQ { get; set; }
    }
}
