using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public partial class ProcessController : WBBController
    {
        System.Data.DataTable tempTable1;
        System.Data.DataTable tempTable2;

        public string GeneratePDF_old(QuickWinPanelModel model, string directoryPath,
           string directoryTempPath, string fileName, string pdfSignature, string pdfSignature2)
        {
            InterfaceLogCommand log = null;
            try
            {
                #region setborder
                string EndAddressSetup = "34";
                string EndAddressVat = "";
                string EndAddressBill = "42";
                string EndDoc = "0";
                string EndCon = "5";
                string EndPack = "26";
                string EndInfo = "8";
                int startDoc = 47;
                int starInfo = 4;
                int startPack = 11;
                int startAddressSetup = 29;
                int startAddressBill = 37;
                int startAddressVat = 0;
                int mainPackContentExtend = 0;
                //int ontopPackContentExtend = 0;
                int recurringContentExtend = 0;

                var displayType_ = model.CustomerRegisterPanelModel.CateType;
                if (displayType_ == "R")
                {
                    EndAddressSetup = "35";
                    EndAddressVat = "";
                    EndAddressBill = "43";
                    EndPack = "27";
                    EndInfo = "9";
                    startDoc = 48;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 30;
                    startAddressBill = 38;
                    startAddressVat = 0;
                }
                else  //// G/B
                {
                    EndAddressSetup = "35";
                    EndAddressVat = "52";
                    EndAddressBill = "44";
                    EndPack = "27";
                    EndInfo = "9";
                    startDoc = 55;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 30;
                    startAddressBill = 39;
                    startAddressVat = 47;
                }

                #endregion

                #region Format PDF

                int numberx = 1;

                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("1", typeof(string));
                table.Columns.Add("2", typeof(string));
                table.Columns.Add("3", typeof(string));
                table.Columns.Add("4", typeof(string));
                table.Columns.Add("5", typeof(string));
                table.Columns.Add("6", typeof(string));

                System.Data.DataTable table1 = new System.Data.DataTable();
                System.Data.DataTable table2 = new System.Data.DataTable();

                GetPDFLOV();

                var ListEntity = GetPDFConfig().Where(l => l.GroupByPDF.Contains("0")).ToList();
                string item = "";

                var items = base.LovData.Where(x1 => x1.Type.Equals(WebConstants.LovConfigName.Screen) && x1.Name.Equals("H_FBB004"));
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                { item = items.Select(n => n.LovValue1).FirstOrDefault(); }
                else
                { item = items.Select(n => n.LovValue2).FirstOrDefault(); }

                table.Rows.Add(numberx, item, " ", " ", " ", "", ""); numberx += 1;
                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;

                bool chkColumn = true;
                string column1 = "";
                string column2 = "";
                string name1 = "";
                string Value1 = ""; string Value2 = ""; string type = "";
                string groupPDF = "";
                int currenttable = 1;

                // set CateType
                var type_ = model.CustomerRegisterPanelModel.CateType;
                if (type_ != null)
                {
                    type = type_;

                    var units = base.LovData.Where(n => !string.IsNullOrEmpty(n.Name) && n.Name.Equals("L_BAHT"));
                    string unit = "";
                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                    { unit = units.Select(n => n.LovValue1).FirstOrDefault(); }
                    else
                    { unit = units.Select(n => n.LovValue2).FirstOrDefault(); }


                    foreach (var l in ListEntity)
                    {

                        #region กำหนดช่องว่างระหว่าง group
                        // Check group PDF
                        if ((!string.IsNullOrEmpty(groupPDF)) && (groupPDF != l.GroupByPDF.Substring(0, 2)))
                        {
                            chkColumn = true;
                            table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            groupPDF = l.GroupByPDF.Substring(0, 2);
                        }
                        else if (string.IsNullOrEmpty(groupPDF))
                        {
                            groupPDF = l.GroupByPDF.Substring(0, 2);
                        }

                        #endregion

                        #region set value to datatable
                        // Check group header
                        if (l.GroupByPDF.Substring(2, 1).Equals("H"))
                        {

                            string val = "";

                            #region condition
                            if (l.Name.Equals("L_CONDITION"))// add detail
                            {

                                table = new System.Data.DataTable();
                                table.Columns.Add("ID", typeof(int));
                                table.Columns.Add("1", typeof(string));
                                table.Columns.Add("2", typeof(string));
                                table.Columns.Add("3", typeof(string));
                                table.Columns.Add("4", typeof(string));
                                table.Columns.Add("5", typeof(string));
                                table.Columns.Add("6", typeof(string));

                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                currenttable = 2;

                                string ProSubType = "";
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE.ToString().Trim() == "SWiFi")
                                {
                                    ProSubType = "TERM_AND_CONDITION_WIRELESS";
                                }
                                else
                                {
                                    ProSubType = "TERM_AND_CONDITION_WIRE";
                                }
                                var data = base.LovData.Where(z => z.Type.Equals(ProSubType));
                                int num = 1;
                                if (SiteSession.CurrentUICulture.IsThaiCulture())
                                {

                                    foreach (var x in data.ToList())
                                    {
                                        // table.Rows.Add(numberx, num.ToString() + ". " + x.LovValue1, " ", " ", " ", "", ""); num++; numberx += 1;
                                        table.Rows.Add(numberx, num.ToString() + ". ", x.LovValue1, " ", " ", "", ""); num++; numberx += 1;
                                    }
                                }
                                else
                                {
                                    foreach (var x in data.ToList())
                                    {
                                        //table.Rows.Add(numberx, num.ToString() + ". " + x.LovValue2, " ", " ", " ", "", ""); num++; numberx += 1;
                                        table.Rows.Add(numberx, num.ToString() + ". ", x.LovValue2, " ", " ", "", ""); num++; numberx += 1;
                                    }
                                }
                                EndCon = data.ToList().Count().ToString();

                            }
                            #endregion

                            #region document
                            else if (l.Name.Equals("L_DOCUMENT"))// add detail
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "SWiFi")
                                {
                                    var data = base.LovData.Where(n => n.Type.Equals(WebConstants.LovConfigName.Document) && n.Name.Equals(model.CustomerRegisterPanelModel.DocType));
                                    if (SiteSession.CurrentUICulture.IsThaiCulture())
                                    { val = data.Select(n => n.LovValue1).FirstOrDefault(); }
                                    else
                                    { val = data.Select(n => n.LovValue2).FirstOrDefault(); }

                                    val = val.Replace("<br/>", "|"); val = val.Replace("<br />", "|");
                                    val = val.Replace("<b>", ""); val = val.Replace("</b>", "");
                                    string[] words = val.Split('|');
                                    table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                    int n1_ = 1; int numdoc = 1;
                                    foreach (var x in words)
                                    {
                                        string[] items2 = x.Split('.');
                                        if (items2.Length > 1)
                                        {
                                            table.Rows.Add(numberx, n1_.ToString(), items2[1].ToString(), " ", " ", "", ""); numberx += 1;
                                        }
                                        else
                                        {
                                            table.Rows.Add(numberx, items2[0].ToString(), " ", " ", " ", "", ""); numberx += 1;
                                            n1_ = 1;
                                        }
                                        n1_++;
                                        numdoc++;
                                    }
                                    EndDoc = numdoc.ToString();
                                }
                            }
                            #endregion

                            #region address billing
                            else if (l.Name.Equals("L_BILLING_DETAIL"))// add detail
                            {
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                string coln1 = ""; string coln2 = ""; int currentcol = 1;

                                var d = from c in ListEntity
                                        where c.GroupByPDF == "03D"
                                        select c;

                                foreach (var x in d)
                                {
                                    if (x.Name.Equals("L_MOOBAN") || x.Name.Equals("L_ROOM"))
                                    {
                                        var Value3 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                        if (Value3.ToSafeString() == "") Value3 = "-";
                                        table.Rows.Add(numberx, coln1, Value1, coln2, Value2, x.DisplayValue, Value3); numberx += 1;
                                        currentcol = 1;
                                    }
                                    else if (x.Name.Equals("L_ROAD") || x.Name.Equals("L_AMPHUR") || x.Name.Equals("L_ZIPCODE") || x.Name.Equals("ZIPCODE_ID"))
                                    {
                                        if (x.Name.Equals("ZIPCODE_ID")) x.Name = "L_ZIPCODE";
                                        Value1 = "";
                                        Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, name1);
                                        Value2 = "";
                                        Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                        if (Value1.ToSafeString() == "") Value1 = "-";
                                        if (Value2.ToSafeString() == "") Value2 = "-";
                                        table.Rows.Add(numberx, coln1, Value1, "", "", x.DisplayValue, Value2); numberx += 1;
                                        currentcol = 1;
                                    }
                                    //if (l.Name.Equals("L_MOO") || l.Name.Equals("L_FLOOR"))
                                    //{
                                    //    continue;
                                    //}
                                    else if (currentcol == 1)
                                    {
                                        coln1 = "";
                                        coln2 = "";
                                        coln1 = x.DisplayValue;
                                        currentcol += 1;
                                        name1 = "";
                                        name1 = x.Name;
                                    }
                                    else
                                    {
                                        #region get value
                                        Value1 = "";
                                        Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, name1);

                                        Value2 = "";
                                        Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                        if (Value1.ToSafeString() == "") Value1 = "-";
                                        if (Value2.ToSafeString() == "") Value2 = "-";
                                        #endregion

                                        coln2 = x.DisplayValue;
                                        currentcol = 1;
                                    }

                                }
                                //if (model.CustomerRegisterPanelModel.DocType == "NON_RES")
                                //{
                                //    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                //}
                            }
                            #endregion

                            #region address vat
                            else if ((l.Name.Equals("L_VAT_DETAIL")))// add detail
                            {
                                if (type != "R")
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                    string coln1 = ""; string coln2 = ""; int currentcol = 1;
                                    var d = from c in ListEntity
                                            where c.GroupByPDF == "03D"
                                            select c;
                                    foreach (var x in d)
                                    {
                                        if (x.Name.Equals("L_MOOBAN") || x.Name.Equals("L_ROOM"))
                                        {
                                            var Value3 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
                                            if (Value3.ToSafeString() == "") Value3 = "-";
                                            table.Rows.Add(numberx, coln1, Value1, coln2, Value2, x.DisplayValue, Value3); numberx += 1;
                                            currentcol = 1;
                                        }
                                        else if (x.Name.Equals("L_ROAD") || x.Name.Equals("L_AMPHUR") || x.Name.Equals("L_ZIPCODE") || x.Name.Equals("ZIPCODE_ID"))
                                        {
                                            if (x.Name.Equals("ZIPCODE_ID")) x.Name = "L_ZIPCODE";
                                            Value1 = "";
                                            Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, name1);
                                            Value2 = "";
                                            Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
                                            if (Value1.ToSafeString() == "") Value1 = "-";
                                            if (Value2.ToSafeString() == "") Value2 = "-";
                                            table.Rows.Add(numberx, coln1, Value1, "", "", x.DisplayValue, Value2); numberx += 1;
                                            currentcol = 1;
                                        }
                                        //if (l.Name.Equals("L_MOO") || l.Name.Equals("L_FLOOR"))
                                        //{
                                        //    continue;
                                        //}
                                        else if (currentcol == 1)
                                        {
                                            coln1 = "";
                                            coln2 = "";
                                            coln1 = x.DisplayValue;
                                            currentcol += 1;
                                            name1 = "";
                                            name1 = x.Name;
                                        }
                                        else
                                        {
                                            #region get value
                                            Value1 = "";
                                            Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, name1);

                                            Value2 = "";
                                            Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
                                            if (Value1.ToSafeString() == "") Value1 = "-";
                                            if (Value2.ToSafeString() == "") Value2 = "-";
                                            #endregion

                                            coln2 = x.DisplayValue;
                                            currentcol = 1;
                                        }

                                    }
                                    //if (model.CustomerRegisterPanelModel.DocType == "NON_RES")
                                    //{
                                    //    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                    //}
                                }

                            }
                            #endregion

                            #region header group
                            else
                            {
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                            }
                            #endregion

                        }
                        else
                        {

                            if ((type == "R") && ((l.Name.Equals("L_GOVERNMENT_NAME")) || (l.Name.Equals("L_CONTACT_PERSON"))))
                            {
                                continue;
                            }

                            else if ((type != "R") && ((l.Name.Equals("L_NAME")) || (l.Name.Equals("L_GENDER")) || (l.Name.Equals("L_BIRTHDAY")) || (l.Name.Equals("L_HOME_PHONE"))))
                            {
                                continue;
                            }

                            #region Package

                            else if (l.Name.Equals("L_ONTOP_PLAYBOX_PRE_INITIATION"))
                            {
                                continue;

                            }
                            else if (l.Name.Equals("L_ONTOP_PLAYBOX_DISCOUNT_INITIATION"))
                            {
                                continue;

                            }
                            else if (l.Name.Equals("L_MAIN_PACK"))//แพกเกจหลัก
                            {
                                var val = model.SummaryPanelModel.PDFPackageModel.PDF_L_MAIN_PACKAGE;
                                if (val != null)
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, val.ToString(), "", "", "", ""); numberx += 1;
                                    if (val.ToString().ToUpper().Contains("GREETING"))
                                    {
                                        var REMARK_GREETING = base.LovData.Where(x1 => x1.Type.Equals(WebConstants.LovConfigName.Screen) && x1.Name.Equals("L_REMARK_GREETING_1"));
                                        var strREMARK_GREETING_1 = "";
                                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                                        { strREMARK_GREETING_1 = REMARK_GREETING.Select(n => n.LovValue1).FirstOrDefault(); }
                                        else
                                        { strREMARK_GREETING_1 = REMARK_GREETING.Select(n => n.LovValue2).FirstOrDefault(); }
                                        table.Rows.Add(numberx, "", strREMARK_GREETING_1.ToString(), "", "", "", ""); numberx += 1;
                                        mainPackContentExtend++;

                                        REMARK_GREETING = base.LovData.Where(x1 => x1.Type.Equals(WebConstants.LovConfigName.Screen) && x1.Name.Equals("L_REMARK_GREETING_2"));
                                        var strREMARK_GREETING_2 = "";
                                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                                        { strREMARK_GREETING_2 = REMARK_GREETING.Select(n => n.LovValue1).FirstOrDefault(); }
                                        else
                                        { strREMARK_GREETING_2 = REMARK_GREETING.Select(n => n.LovValue2).FirstOrDefault(); }
                                        table.Rows.Add(numberx, "", strREMARK_GREETING_2.ToString(), "", "", "", ""); numberx += 1;
                                        mainPackContentExtend++;
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, "-", "", "", "", ""); numberx += 1;
                                }

                            }
                            else if ((l.Name.Equals("L_ONTOP_PACK")))//แพกเกจเสริม
                            {
                                var val = model.SummaryPanelModel.PDFPackageModel.PDF_L_ONTOPLIST;

                                if (!string.IsNullOrEmpty(model.CoveragePanelModel.BundlingSpecialFlag))
                                {
                                    mainPackContentExtend += 1;
                                }

                                if (val == null)
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, "-", "", "", "", ""); numberx += 1;
                                }
                                else if (val.Count > 0)
                                {
                                    for (var ii = 0; ii < val.Count; ii++)
                                    {
                                        if (ii == 0)
                                        {
                                            table.Rows.Add(numberx, l.DisplayValue, val[ii], "", "", "", ""); numberx += 1;
                                        }
                                        else
                                        {
                                            table.Rows.Add(numberx, "", val[ii], "", "", "", ""); numberx += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, "-", "", "", "", ""); numberx += 1;
                                }
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL1")))//รายการที่ต้องชำระในวันที่ติดตั้ง
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            //else if (model.CoveragePanelModel.PRODUCT_TYPE != "")
                            //{
                            //    if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "WireBB")
                            //    {
                            //        if (l.Name.Equals("L_SUMM_DETAIL_WireBB"))
                            //        {
                            //            Value1 = "";
                            //            table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            //        }
                            //    }
                            //}
                            else if (l.Name.Equals("L_SUMM_DETAIL_WireBB"))
                            {
                                //Value1 = l.DisplayValue;
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "WireBB")
                                {
                                    table.Rows.Add(numberx, "", l.DisplayValue, "", "", "", ""); numberx += 1;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if (l.Name.Equals("L_SUMM_DETAIL_SWiFi"))
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "SWiFi")
                                {
                                    table.Rows.Add(numberx, "", l.DisplayValue, "", "", "", ""); numberx += 1;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if (l.Name.Equals("L_SUMM_DETAIL_FTTx"))
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx" || model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTR")
                                {
                                    if (l.DisplayValue == " ")
                                        l.DisplayValue = "hasBorder";
                                    table.Rows.Add(numberx, "", l.DisplayValue, "", "", "", ""); numberx += 1;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL5")))//รายการที่ต้องชำระครั้งเดวในรอบบิลแรก
                            {
                                Value1 = "";
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL6")))//
                            {
                                var tidtun = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_LABEL_TIDTUN;
                                var tidtunval = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_TIDTUN;
                                table.Rows.Add(numberx, "", tidtun, tidtunval, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL7")))//
                            {
                                var discount = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNT;
                                table.Rows.Add(numberx, "", l.DisplayValue, discount, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL8")))//
                            {
                                var sum1 = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM1;

                                table.Rows.Add(numberx, "", l.DisplayValue, sum1, unit, "", ""); numberx += 1;


                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL9")))//รายการที่ต้องชำระรายเดือน
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL10")))//
                            {
                                var pakagename = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_LABEL_PAKAGENAME;
                                var pakagenameval = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_PAKAGE;

                                table.Rows.Add(numberx, "", l.DisplayValue + pakagename, pakagenameval, unit, "", ""); numberx += 1;

                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL11")))//
                            {
                                var sum2 = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM2;
                                table.Rows.Add(numberx, "", l.DisplayValue, sum2, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL12")))//
                            {
                                table.Rows.Add(numberx, "", l.DisplayValue, "", "", "", ""); numberx += 1;
                            }

                            else if ((l.Name.Equals("L_SUMM_DETAIL13")))//ค่่าบริการเฉลี่ยรายวัน(pro-rate)
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL14")))//
                            {
                                var val4day = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_4DAY;
                                table.Rows.Add(numberx, "", l.DisplayValue, val4day, unit, "", ""); numberx += 1;

                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL15")))
                            {
                                var firsbillsum = model.SummaryPanelModel.PDFPackageModel.PDF_L_LABEL_DETAIL_ALLFIRSTBILL;

                                table.Rows.Add(numberx, "", l.DisplayValue, firsbillsum, unit, "", ""); numberx += 1;

                            }
                            else if (l.Name.Equals("L_BUNDLING_DISCOUNT_DTL"))
                            {
                                if (string.IsNullOrEmpty(model.CoveragePanelModel.BundlingSpecialFlag))
                                {
                                    continue;
                                }
                                if (model.CoveragePanelModel.BundlingSpecialFlag.ToYesNoFlgBoolean())
                                {
                                    var bundlingdcval = model.DisplayPackagePanelModel.L_SPECIAL_BUNDLING_DISCOUNT;
                                    table.Rows.Add(numberx, "", l.DisplayValue, bundlingdcval, unit, "", ""); numberx += 1;
                                    recurringContentExtend++;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if (l.Name.Equals("L_DISCOUNT_DTL"))
                            {
                                if (string.IsNullOrEmpty(model.CoveragePanelModel.BundlingSpecialFlag))
                                {
                                    continue;
                                }
                                if (!model.CoveragePanelModel.BundlingSpecialFlag.ToYesNoFlgBoolean())
                                {
                                    var dcval = model.DisplayPackagePanelModel.L_SPECIAL_DISCOUNT;
                                    table.Rows.Add(numberx, "", l.DisplayValue, dcval, unit, "", ""); numberx += 1;
                                    recurringContentExtend++;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if (l.Name.Equals("L_SUM_MAINTEN_RATE")
                                    || l.Name.Equals("L_SUM_INSTALL_RATE")
                                    )
                            {
                                continue;
                            }

                            #endregion

                            else
                            {

                                if (chkColumn == true && l.Name.Equals("L_INSTALLATION_DATE") && (type.ToUpper().Trim() == "B" || type.ToUpper().Trim() == "G"))
                                {
                                    var installdateb = model.CustomerRegisterPanelModel.L_INSTALL_DATE;
                                    if (installdateb.ToSafeString() == "") installdateb = "-";
                                    table.Rows.Add(numberx, l.DisplayValue, installdateb, "", "", "", ""); numberx += 1;
                                }
                                else
                                {
                                    if (chkColumn)
                                    {



                                        if ((l.Name.Equals("L_BUSINESS")) || (l.Name.Equals("L_RESIDENTIAL")) || (l.Name.Equals("L_GOVERNMENT")))
                                        {
                                            if ((type.ToUpper().Trim() == "B") && (l.Name.Equals("L_BUSINESS")))
                                            {
                                                column1 = l.DisplayValue; chkColumn = false; continue;
                                            }
                                            else if ((type.ToUpper().Trim() == "R") && (l.Name.Equals("L_RESIDENTIAL")))
                                            {
                                                column1 = l.DisplayValue; chkColumn = true; continue;
                                            }
                                            else if ((type.ToUpper().Trim() == "G") && (l.Name.Equals("L_GOVERNMENT")))
                                            {
                                                column1 = l.DisplayValue; chkColumn = true; continue;
                                            }
                                            else
                                            {
                                                if ((l.Name.Equals("L_BUSINESS")))
                                                {
                                                    chkColumn = false; continue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            name1 = "";
                                            column1 = "";
                                            column2 = "";
                                            column1 = l.DisplayValue;
                                            name1 = l.Name;
                                            chkColumn = false;
                                        }

                                        if (l.Name.Equals("L_CONTACT_PHONE"))
                                        {
                                            Value1 = "";
                                            var val1 = model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            var val2 = model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            var val3 = model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            Value1 = val1.ToString();
                                            if ((Value1.Trim() != "") && (val2.ToString().Trim() != "")) { Value1 += ", "; }
                                            Value1 += val2.ToString();
                                            if ((Value1.Trim() != "") && (val3.ToString().Trim() != "")) { Value1 += ", "; }
                                            Value1 += val3.ToString();
                                        }

                                    }
                                    else
                                    {

                                        if ((l.Name.Equals("L_RESIDENTIAL")))
                                        {
                                            chkColumn = false;
                                            continue;
                                        }

                                        else if ((l.Name.Equals("L_GOVERNMENT")))
                                        {
                                            chkColumn = false;
                                            continue;
                                        }
                                        else if ((l.Name.Equals("L_BUSINESS")))
                                        {
                                            chkColumn = false;
                                            continue;
                                        }

                                        #region address setup
                                        if (l.Name.Equals("L_MOOBAN") || l.Name.Equals("L_ROOM"))
                                        {
                                            var a1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                            var a2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            if (a1.ToSafeString() == "") a1 = "-";
                                            if (a2.ToSafeString() == "") a2 = "-";
                                            table.Rows.Add(numberx, column1, a1, column2, Value2, l.DisplayValue, a2); numberx += 1;
                                            continue;
                                        }
                                        else if ((l.Name.Equals("L_MOO")) || (l.Name.Equals("L_FLOOR")))
                                        {
                                            column2 = "";
                                            column2 = l.DisplayValue;
                                            Value2 = "";
                                            Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            if (Value2.ToSafeString() == "") Value2 = "-";
                                            continue;
                                        }
                                        else if ((l.Name.Equals("L_HOME_NUMBER_2")) || (l.Name.Equals("L_PROVINCE")) || (l.Name.Equals("L_TUMBOL")) || (l.Name.Equals("L_SOI")) || (l.Name.Equals("L_BUILD_NAME")))
                                        {
                                            column1 = "";
                                            column1 = l.DisplayValue;
                                            name1 = "";
                                            name1 = l.Name;
                                            Value1 = "";
                                            Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            if (Value1.ToSafeString() == "") Value1 = "-";
                                            continue;
                                        }
                                        else if (l.Name.Equals("L_ROAD") || l.Name.Equals("L_AMPHUR") || l.Name.Equals("L_ZIPCODE") || l.Name.Equals("ZIPCODE_ID"))
                                        {
                                            if (l.Name.Equals("ZIPCODE_ID")) l.Name = "L_ZIPCODE";
                                            var a1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                            var a2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            if (a1.ToSafeString() == "") a1 = "-";
                                            if (a2.ToSafeString() == "") a2 = "-";
                                            table.Rows.Add(numberx, column1, a1, "", "", l.DisplayValue, a2); numberx += 1;

                                            if ((l.Name.Equals("L_ZIPCODE")) && (type == "B" || type == "G"))
                                            {
                                                table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                            }
                                            continue;
                                        }

                                        #endregion

                                        column2 = l.DisplayValue;

                                        Value1 = "";
                                        Value1 = GetModelValue(model.CoveragePanelModel, name1);
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(model.CustomerRegisterPanelModel, name1);
                                        }
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(model.DisplayPackagePanelModel, name1);
                                        }
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(model.SummaryPanelModel, name1);
                                        }
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                        }

                                        if (Value1.ToSafeString() == "" && name1 != "") Value1 = "-";

                                        if (name1.Equals("L_CONTACT_PHONE"))
                                        {
                                            var val1 = model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            var val2 = model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            var val3 = model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            Value1 = val1.ToString();
                                            if ((Value1.Trim() != "") && (val2.ToString().Trim() != "")) { Value1 += ", "; }
                                            Value1 += val2.ToString();
                                            if ((Value1.Trim() != "") && (val3.ToString().Trim() != "")) { Value1 += ", "; }
                                            Value1 += val3.ToString();
                                        }

                                        Value2 = "";

                                        if ((l.Name.Equals("L_NAME")))
                                        {
                                            var val1 = model.CustomerRegisterPanelModel.L_TITLE; if (val1 == null) { val1 = ""; }
                                            var val2 = model.CustomerRegisterPanelModel.L_FIRST_NAME; if (val2 == null) { val2 = ""; }
                                            var val3 = model.CustomerRegisterPanelModel.L_LAST_NAME; if (val3 == null) { val3 = ""; }

                                            Value2 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();

                                        }
                                        else if ((l.Name.Equals("L_GOVERNMENT_NAME")))
                                        {
                                            var val1 = model.CustomerRegisterPanelModel.L_TITLE; if (val1 == null) { val1 = ""; }
                                            var val2 = model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME; if (val2 == null) { val2 = ""; }

                                            Value2 = val1.ToString() + " " + val2.ToString();

                                        }

                                        else if (l.Name.Equals("L_CONTACT_PHONE"))
                                        {
                                            var val1 = model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            var val2 = model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            var val3 = model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            Value2 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();
                                            Value2 = val1.ToString();
                                            if ((Value2.Trim() != "") && (val2.ToString().Trim() != "")) { Value2 += ", "; }
                                            Value2 += val2.ToString();
                                            if ((Value2.Trim() != "") && (val3.ToString().Trim() != "")) { Value2 += ", "; }
                                            Value2 += val3.ToString();
                                        }

                                        else
                                        {
                                            Value2 = GetModelValue(model.CoveragePanelModel, l.Name);
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(model.CustomerRegisterPanelModel, l.Name);
                                            }
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(model.DisplayPackagePanelModel, l.Name);
                                            }
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(model.SummaryPanelModel, l.Name);
                                            }
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            }
                                        }

                                        if (Value2.ToSafeString() == "") Value2 = "-";
                                        if (l.Name.Equals("L_INSTALLATION_DATE"))
                                        {
                                            Value2 = model.CustomerRegisterPanelModel.L_INSTALL_DATE;
                                            if (Value2.ToSafeString() == "") Value2 = "-";
                                            if (model.PlugAndPlayFlow == "Y") Value2 = "-";
                                        }

                                        column1 = column1.Replace("*", "");
                                        column2 = column2.Replace("*", "");
                                        column2 = column2.Replace("*", "");
                                        column2 = column2.Replace("วว/ดด/ปปปป(พ.ศ.)", "");
                                        column2 = column2.Replace("dd/mm/yyyy", "");
                                        table.Rows.Add(numberx, column1, Value1, column2, Value2, "", ""); numberx += 1;

                                        chkColumn = true;

                                    }
                                }


                            }
                        }

                        if (currenttable == 1)
                        {
                            table1 = table;
                        }
                        else
                        {

                            table2 = table;
                        }

                        #endregion
                    }
                }

                tempTable1 = table1;
                tempTable2 = table2;

                #endregion

                #region Create PDF

                var pdfIndexTemp = GetTermCon();

                LovValueModel[] pdfIndex = pdfIndexTemp.ToArray();

                var borderSize = 0.7f;
                var borderCoolor = BaseColor.GREEN;
                using (MemoryStream myMemoryStream = new MemoryStream())
                {

                    var fixedHeight = 12.5f;

                    bool chkend2 = true;
                    bool chkendDoc = false;
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, myMemoryStream);
                    //PdfWriter.GetInstance(document, new FileStream(Request.PhysicalApplicationPath + "\\Chap0101.pdf", FileMode.Create));
                    document.Open();

                    var extendedContentRow = mainPackContentExtend + recurringContentExtend;
                    EndPack = (EndPack.ToSafeInteger() + extendedContentRow).ToSafeString();
                    startAddressSetup = startAddressSetup + extendedContentRow;
                    EndAddressSetup = (EndAddressSetup.ToSafeInteger() + extendedContentRow).ToString();
                    startAddressBill = startAddressBill + extendedContentRow;
                    EndAddressBill = (EndAddressBill.ToSafeInteger() + extendedContentRow).ToString();

                    if (displayType_ != "R")
                    {
                        startAddressVat = startAddressVat + extendedContentRow;
                        EndAddressVat = (EndAddressVat.ToSafeInteger() + extendedContentRow).ToString();
                    }

                    startDoc = startDoc + extendedContentRow;
                    EndDoc = (EndDoc.ToSafeInteger() + extendedContentRow).ToSafeString();



                    for (int x = 1; x <= 2; x++)
                    {
                        if (x == 1)
                        {
                            table = new System.Data.DataTable();
                            table = table1;
                            int removeLength = displayType_ != "R" ? int.Parse(EndAddressVat) : int.Parse(EndAddressBill);
                            var AISsubtypePDF = LovData.FirstOrDefault(
                            itemPDF => itemPDF.Name == "AIS_SHOP_SUB_TYPE" && itemPDF.LovValue5 == "FBBOR004").LovValue1;

                            if (model.TopUp != "5" || model.CustomerRegisterPanelModel.outSubType == AISsubtypePDF)
                            {
                                for (int i = startAddressSetup - 1; i < removeLength; i++)
                                {
                                    table.Rows.RemoveAt(startAddressSetup - 1);
                                }
                            }
                            else if (displayType_ != "R")
                            {
                                for (int i = startAddressVat - 1; i < int.Parse(EndAddressVat); i++)
                                {
                                    table.Rows.RemoveAt(startAddressVat - 1);
                                }
                            }


                        }
                        else
                        {
                            table = new System.Data.DataTable();
                            table = table2;

                        }
                        document.NewPage();
                        BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/fonts/tahoma.ttf"),
                            BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        // สร้าง Font จาก BaseFont 
                        iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 6.7f);
                        iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6f);
                        iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 7f, Font.NORMAL, BaseColor.GREEN);
                        iTextSharp.text.Font fntbold = new iTextSharp.text.Font(bf, 6.7f, Font.BOLD);


                        // สร้าง PdfTable จาก GridView Control
                        PdfPTable PdfTable = new PdfPTable(table.Columns.Count - 1);
                        PdfTable.TotalWidth = 415;

                        //PdfTable.TotalWidth = 100f;
                        //PdfTable.LockedWidth = true;

                        PdfPCell PdfPCell = null;

                        // Write Data ที่ PdfCell ใน PdfTable
                        if (x == 1)
                        {
                            for (int row = 1; row < 12; row++)
                            {
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt)));
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                                PdfTable.AddCell(PdfPCell);
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt)));
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                                PdfTable.AddCell(PdfPCell);
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt)));
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                                PdfTable.AddCell(PdfPCell);
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt)));
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                                PdfTable.AddCell(PdfPCell);
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt)));
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                                PdfTable.AddCell(PdfPCell);
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt)));
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                                PdfTable.AddCell(PdfPCell);
                            }

                            document.Add(PdfTable);
                        }

                        // Write Data ที่ PdfCell ใน PdfTable
                        for (int row = 0; row < table.Rows.Count; row++)
                        {
                            PdfTable = new PdfPTable(table.Columns.Count - 1);
                            PdfTable.TotalWidth = 415;

                            string id_ = table.Rows[row].ItemArray[0].ToString();

                            if (displayType_ == "R")
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx"
                                    && (id_ == (16 + mainPackContentExtend).ToSafeString())
                                    )
                                {
                                    continue;
                                }

                                if ((id_ == "2")
                                    || (id_ == "3")
                                    || (id_ == "10")
                                    || (id_ == (28 + extendedContentRow).ToSafeString())
                                    || (id_ == (36 + extendedContentRow).ToSafeString())
                                    || (id_ == (44 + extendedContentRow).ToSafeString())
                                    || (id_ == (45 + extendedContentRow).ToSafeString())
                                    || (id_ == (46 + extendedContentRow).ToSafeString())
                                    )
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx"
                                    && (id_ == (16 + mainPackContentExtend).ToSafeString())
                                    )
                                {
                                    continue;
                                }

                                if ((id_ == "2")
                                    || (id_ == "3")
                                    || (id_ == "10")
                                    || (id_ == (28 + extendedContentRow).ToSafeString())
                                    || (id_ == (36 + extendedContentRow).ToSafeString())
                                    || (id_ == (37 + extendedContentRow).ToSafeString())
                                    || (id_ == (45 + extendedContentRow).ToSafeString())
                                    || (id_ == (53 + extendedContentRow).ToSafeString())
                                    )
                                {
                                    continue;
                                }
                            }

                            string val_0 = table.Rows[row].ItemArray[1].ToString();
                            string val_1 = table.Rows[row].ItemArray[2].ToString();
                            string val_2 = table.Rows[row].ItemArray[3].ToString();
                            string val_3 = table.Rows[row].ItemArray[4].ToString();
                            string val_4 = table.Rows[row].ItemArray[5].ToString();
                            string val_5 = table.Rows[row].ItemArray[6].ToString();

                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt)));
                            PdfPCell.FixedHeight = fixedHeight;
                            if (id_ == "8")
                            {
                                if (val_3.Split(',').Length > 1)
                                {
                                    PdfPCell.FixedHeight = 20f;
                                }
                            }

                            if ((id_ == EndInfo)
                                || (id_ == EndPack)
                                || (id_ == EndAddressSetup)
                                || (id_ == EndAddressBill)
                                || (id_ == EndAddressVat))
                            {

                                if (id_ == EndInfo)
                                {
                                    PdfPCell =
                                        new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt1)));
                                    PdfPCell.FixedHeight = fixedHeight;
                                    PdfTable.SetWidths(new float[] { 15f, 15f, 0f, 15f, 15f, 0f });
                                }

                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = 0;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthBottom = borderSize;
                            }
                            else if (!string.IsNullOrEmpty(val_0.Trim())
                                    && string.IsNullOrEmpty(val_1.Trim())
                                    && string.IsNullOrEmpty(val_2.Trim())
                                    && string.IsNullOrEmpty(val_3.Trim())
                                        && (startPack < id_.ToSafeInteger()
                                            && id_.ToSafeInteger() < EndPack.ToSafeInteger()
                                            )
                                    )
                            {
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthBottom = 0;
                            }
                            else if (!string.IsNullOrEmpty(val_0.Trim())
                                    && ((id_ == starInfo.ToString())
                                        || (id_ == startPack.ToString())
                                        || (id_ == startAddressSetup.ToString())
                                        || (id_ == startAddressBill.ToString())
                                        || (id_ == startAddressVat.ToString())
                                        || (id_ == startDoc.ToString())
                                    )
                                )
                            {
                                PdfPCell =
                                    new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fntgreen)));
                                PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = borderSize;
                                PdfPCell.BorderWidthBottom = 0;
                            }
                            else if (!string.IsNullOrEmpty(val_0.Trim())
                                    && ((id_ == "1"))
                                )
                            {
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(val_0.Trim())
                                    && ((id_ == "5")
                                        || (id_ == "6")
                                        || (id_ == "7")
                                        || (id_ == "8")
                                        )
                                    )
                                {
                                    if (displayType_ == "R")
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 17f, 0f, 15f, 25f, 0f });
                                    }
                                }

                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthRight = 0;
                                PdfPCell.BorderWidthBottom = 0;
                            }

                            if ((x == 2)
                                || ((string.IsNullOrEmpty(val_1.Trim()))
                                    && (string.IsNullOrEmpty(val_2.Trim()))
                                    && (string.IsNullOrEmpty(val_3.Trim())
                                    && (string.IsNullOrEmpty(val_4.Trim()))
                                    && (string.IsNullOrEmpty(val_5.Trim())))
                                    )
                                )
                            {
                                if (string.IsNullOrEmpty(val_0.Trim()))
                                {
                                    if ((x == 1)
                                        && ((Convert.ToInt32(id_) > startDoc)))
                                    {
                                        if (!chkendDoc)
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });

                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = borderSize;
                                            PdfPCell.BorderWidthRight = 0;
                                            PdfPCell.BorderWidthBottom = 0;
                                            chkendDoc = true;
                                        }
                                    }
                                    else
                                    {
                                        PdfPCell.Border = PdfPCell.NO_BORDER;
                                    }

                                }
                                if ((!string.IsNullOrEmpty(val_0.Trim()))
                                    && (string.IsNullOrEmpty(val_1.Trim()))
                                    && (x == 2)
                                    )
                                {
                                    if (chkend2)
                                    {
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(val_0.Trim(), fntgreen)));
                                        PdfPCell.FixedHeight = fixedHeight;
                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderWidthLeft = borderSize;
                                        PdfPCell.BorderWidthBottom = 0;
                                        PdfPCell.BorderWidthTop = borderSize;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        chkend2 = false;
                                    }
                                    else
                                    {
                                        PdfPCell.Border = PdfPCell.NO_BORDER;
                                    }

                                }
                                else if ((!string.IsNullOrEmpty(val_0.Trim()))
                                        && (string.IsNullOrEmpty(val_1.Trim()))
                                        && (x == 1)
                                        && ((Convert.ToInt32(id_) > startDoc))
                                    )
                                {
                                    PdfPCell.BorderColorBottom = borderCoolor;
                                    PdfPCell.BorderColorRight = borderCoolor;
                                    PdfPCell.BorderColorTop = borderCoolor;
                                    PdfPCell.BorderColorLeft = borderCoolor;
                                    PdfPCell.BorderWidthLeft = borderSize;
                                    PdfPCell.BorderWidthBottom = 0;
                                    PdfPCell.BorderWidthTop = 0;
                                    PdfPCell.BorderWidthRight = borderSize;
                                }
                                if ((string.IsNullOrEmpty(val_0.Trim()))
                                    && (string.IsNullOrEmpty(val_1.Trim()))
                                    )
                                {
                                    PdfPCell.Border = PdfPCell.NO_BORDER;
                                }

                                PdfPCell.Colspan = 6;
                            }
                            else if (id_.ToSafeInteger() > startDoc)
                            {
                                if (!chkendDoc)
                                {
                                    PdfTable.SetWidths(new float[] { 3f, 37f, 10f, 10f, 10f, 10f });
                                    PdfPCell.BorderColorBottom = borderCoolor;
                                    PdfPCell.BorderColorRight = borderCoolor;
                                    PdfPCell.BorderColorTop = borderCoolor;
                                    PdfPCell.BorderColorLeft = borderCoolor;
                                    PdfPCell.BorderWidthLeft = borderSize;
                                    PdfPCell.BorderWidthTop = 0;
                                    PdfPCell.BorderWidthRight = 0;
                                    PdfPCell.BorderWidthBottom = 0;

                                    if (id_ == (startDoc + Convert.ToInt16(EndDoc) - 1).ToString())
                                    {
                                        PdfPCell.BorderWidthBottom = borderSize;
                                    }
                                    else
                                    {
                                        PdfPCell.BorderWidthBottom = 0;
                                    }
                                }
                            }

                            if ((!string.IsNullOrEmpty(val_0.Trim()))
                                && (!string.IsNullOrEmpty(val_1.Trim()))
                                && (x == 2))
                            {
                                PdfTable.SetWidths(new float[] { 3f, 37f, 10f, 10f, 10f, 10f });
                                PdfPCell =
                                    new PdfPCell(new Phrase(new Chunk(pdfIndex[int.Parse(table.Rows[row].ItemArray[1].ToString().Replace(".", "")) - 1].Text, fnt)));
                                //PdfPCell.FixedHeight = fixedHeight;

                                //PdfPCell.Width = 30;

                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthRight = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                {
                                    PdfPCell.BorderWidthBottom = borderSize;
                                }
                                else
                                {
                                    PdfPCell.BorderWidthBottom = 0;
                                }

                                PdfTable.AddCell(PdfPCell);

                                string tempArrayData2 = table.Rows[row].ItemArray[2].ToString();
                                while (tempArrayData2.IndexOf('<') >= 0 && tempArrayData2.IndexOf('>') >= 0)
                                {

                                    tempArrayData2 = tempArrayData2.Replace(tempArrayData2.Substring(tempArrayData2.IndexOf('<'), tempArrayData2.IndexOf('>') - tempArrayData2.IndexOf('<') + 1), "");
                                }

                                PdfPCell =
                                    new PdfPCell(new Phrase(new Chunk(tempArrayData2, fnt)));
                                //PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = 0;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                {
                                    PdfPCell.BorderWidthBottom = borderSize;
                                }
                                else
                                {
                                    PdfPCell.BorderWidthBottom = 0;
                                }

                                PdfPCell.Colspan = 5;
                                PdfTable.AddCell(PdfPCell);
                                document.Add(PdfTable);
                                continue;
                            }
                            else
                            {
                                PdfTable.AddCell(PdfPCell);
                            }

                            if (x == 1)
                            {
                                if ((!string.IsNullOrEmpty(val_1.Trim()))
                                    || (!string.IsNullOrEmpty(val_2.Trim()))
                                    || (!string.IsNullOrEmpty(val_3.Trim()))
                                    || (!string.IsNullOrEmpty(val_4.Trim()))
                                    || (!string.IsNullOrEmpty(val_5.Trim()))
                                    )
                                {
                                    if ((id_ == "13")
                                        || ((mainPackContentExtend > 0 && id_ == "14"))
                                        || ((mainPackContentExtend > 0 && id_ == "15"))
                                        || ((mainPackContentExtend > 0 && id_ == "16"))
                                        || (id_ == (14 + mainPackContentExtend).ToSafeString())
                                        || (id_ == (16 + mainPackContentExtend).ToSafeString())
                                        || (id_ == EndPack)
                                        || id_.ToSafeInteger() > startDoc
                                        )
                                    {
                                        if ((id_ == (16 + mainPackContentExtend).ToSafeString())
                                            || (id_ == EndPack))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }

                                        if (val_1 != "hasBorder")
                                        {
                                            PdfPCell =
                                                new PdfPCell(
                                                    new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt)));
                                            PdfPCell.FixedHeight = fixedHeight;
                                            if (id_ == EndPack)
                                            {
                                                PdfPCell =
                                                    new PdfPCell(
                                                        new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(),
                                                            fntbold)));
                                                PdfPCell.FixedHeight = fixedHeight;
                                            }
                                        }

                                        PdfPCell.Colspan = 5;

                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;
                                        PdfPCell.BorderWidthBottom = 0;

                                        if ((id_ == EndPack)
                                            || (id_ == (startDoc + Convert.ToInt16(EndDoc) - 1).ToString()))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else if ((id_ == "5")
                                            || (id_ == "6")
                                            || (id_ == "7")
                                            || (id_ == "8")
                                            || (id_ == EndInfo)
                                            || (id_ == (17 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (18 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (19 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (20 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (22 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (23 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (24 + extendedContentRow).ToSafeString())
                                            || (id_ == (25 + extendedContentRow).ToSafeString())
                                            || (id_ == (26 + extendedContentRow).ToSafeString())
                                            || (id_ == (27 + extendedContentRow).ToSafeString())
                                            || ((extendedContentRow > 0 && id_ == "25"))
                                            || ((extendedContentRow > 0 && id_ == "26"))
                                            || ((extendedContentRow > 0 && id_ == "27"))
                                        )
                                    {
                                        if ((id_ == (17 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (18 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (19 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (20 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (22 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (23 + mainPackContentExtend).ToSafeString())
                                            || (id_ == (24 + extendedContentRow).ToSafeString())
                                            || (id_ == (25 + extendedContentRow).ToSafeString())
                                            || (id_ == (26 + extendedContentRow).ToSafeString())
                                            || (id_ == (27 + extendedContentRow).ToSafeString())
                                            || ((extendedContentRow > 0 && id_ == "25"))
                                            || ((extendedContentRow > 0 && id_ == "26"))
                                            || ((extendedContentRow > 0 && id_ == "27"))
                                            )
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }

                                        if (id_ == EndInfo)
                                        {
                                            PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                        }

                                        if ((id_ == (26 + extendedContentRow).ToSafeString())
                                            || (id_ == (27 + extendedContentRow).ToSafeString()))
                                        {
                                            PdfPCell =
                                                new PdfPCell(
                                                    new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(),
                                                        fntbold)));
                                            PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell =
                                                new PdfPCell(
                                                    new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt)));
                                            PdfPCell.FixedHeight = fixedHeight;
                                        }

                                        if (id_ == EndInfo)
                                        {
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderWidthBottom = borderSize;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = 0;
                                            PdfPCell.BorderWidthRight = 0;
                                        }
                                        else
                                        {
                                            PdfPCell.Border = PdfPCell.NO_BORDER;
                                        }

                                        PdfPCell.Colspan = 2;
                                        PdfTable.AddCell(PdfPCell);

                                        if ((id_ == (26 + extendedContentRow).ToSafeString())
                                            || (id_ == (27 + extendedContentRow).ToSafeString()))
                                        {
                                            PdfPCell =
                                                new PdfPCell(
                                                    new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(),
                                                        fntbold)));
                                            PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell =
                                                new PdfPCell(
                                                    new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt)));
                                            PdfPCell.FixedHeight = fixedHeight;
                                        }

                                        if (id_ == EndInfo)
                                        {
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderWidthBottom = borderSize;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = 0;
                                            PdfPCell.BorderWidthRight = 0;
                                        }
                                        else
                                        {
                                            PdfPCell.Border = PdfPCell.NO_BORDER;
                                        }

                                        PdfTable.AddCell(PdfPCell);

                                        if ((id_ == (26 + extendedContentRow).ToSafeString())
                                            || (id_ == (27 + extendedContentRow).ToSafeString()))
                                        {
                                            PdfPCell =
                                                new PdfPCell(
                                                    new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(),
                                                        fntbold)));
                                            PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell =
                                                new PdfPCell(
                                                    new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt)));
                                            PdfPCell.FixedHeight = fixedHeight;
                                        }

                                        //PdfPCell.Border = PdfPCell.NO_BORDER;
                                        PdfPCell.Colspan = 2;

                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;
                                        PdfPCell.BorderWidthBottom = 0;

                                        if (id_ == EndInfo)
                                        {
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderWidthBottom = borderSize;
                                            PdfPCell.BorderWidthRight = borderSize;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = 0;

                                        }
                                        else
                                        {
                                            PdfPCell.BorderWidthBottom = 0;
                                        }
                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 30f, 5f, 5f, 10f, 20f });
                                        PdfPCell =
                                            new PdfPCell(
                                                new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt)));
                                        PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup)
                                            || (id_ == EndAddressBill)
                                            || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderWidthBottom = borderSize;
                                            PdfPCell.BorderWidthRight = 0;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = 0;
                                        }
                                        else
                                        {
                                            PdfPCell.Border = PdfPCell.NO_BORDER;
                                        }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell =
                                            new PdfPCell(
                                                new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt)));
                                        PdfPCell.FixedHeight = fixedHeight;

                                        if ((id_ == EndAddressSetup)
                                            || (id_ == EndAddressBill)
                                            || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderWidthBottom = borderSize;
                                            PdfPCell.BorderWidthRight = 0;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = 0;
                                        }
                                        else
                                        {
                                            PdfPCell.Border = PdfPCell.NO_BORDER;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell =
                                            new PdfPCell(
                                                new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt)));
                                        PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup)
                                            || (id_ == EndAddressBill)
                                            || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderWidthBottom = borderSize;
                                            PdfPCell.BorderWidthRight = 0;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = 0;
                                        }
                                        else
                                        {
                                            PdfPCell.Border = PdfPCell.NO_BORDER;
                                        }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell =
                                            new PdfPCell(
                                                new Phrase(new Chunk(table.Rows[row].ItemArray[5].ToString(), fnt)));
                                        PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup)
                                            || (id_ == EndAddressBill)
                                            || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderColorTop = borderCoolor;
                                            PdfPCell.BorderColorLeft = borderCoolor;
                                            PdfPCell.BorderColorRight = borderCoolor;
                                            PdfPCell.BorderColorBottom = borderCoolor;
                                            PdfPCell.BorderWidthBottom = borderSize;
                                            PdfPCell.BorderWidthRight = 0;
                                            PdfPCell.BorderWidthLeft = 0;
                                            PdfPCell.BorderWidthTop = 0;
                                        }
                                        else
                                        {
                                            PdfPCell.Border = PdfPCell.NO_BORDER;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell =
                                            new PdfPCell(
                                                new Phrase(new Chunk(table.Rows[row].ItemArray[6].ToString(), fnt)));
                                        PdfPCell.FixedHeight = fixedHeight;

                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthBottom = 0;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;

                                        if ((id_ == EndAddressSetup)
                                            || (id_ == EndAddressBill)
                                            || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                }
                            }

                            document.Add(PdfTable);
                        }


                        // background
                        iTextSharp.text.Image pdfbg =
                            iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/BG_base1.jpg"));
                        pdfbg.ScaleToFit(3200, 850);
                        pdfbg.Alignment = iTextSharp.text.Image.UNDERLYING;
                        pdfbg.SetAbsolutePosition(0, 0);
                        document.Add(pdfbg);

                        // logo
                        iTextSharp.text.Image pdflogo =
                            iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/img-logo.png"));
                        pdflogo.ScaleToFit(200, 100);
                        pdflogo.Alignment = iTextSharp.text.Image.UNDERLYING;
                        pdflogo.SetAbsolutePosition(380, 712);
                        document.Add(pdflogo);
                        // status
                        string imgStatus = "";
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        {
                            imgStatus = "~/Content/src/thai_5.png";
                        }
                        else
                        {
                            imgStatus = "~/Content/src/eng_5.png";
                        }

                        iTextSharp.text.Image pdfstatus = iTextSharp.text.Image.GetInstance(Server.MapPath(imgStatus));
                        pdfstatus.ScaleToFit(300, 150);
                        pdfstatus.Alignment = iTextSharp.text.Image.UNDERLYING;
                        pdfstatus.SetAbsolutePosition(80, 712);
                        document.Add(pdfstatus);


                        iTextSharp.text.html.simpleparser.StyleSheet styles =
                            new iTextSharp.text.html.simpleparser.StyleSheet();
                        iTextSharp.text.html.simpleparser.HTMLWorker hw =
                            new iTextSharp.text.html.simpleparser.HTMLWorker(document);
                    }

                    #endregion

                    document.Close();
                    byte[] content = myMemoryStream.ToArray();

                    //SET PDF PASSWORD
                    var pdfbyte = content;

                    pdfbyte = PdfSecurity.SetPasswordPdf(content, model.CustomerRegisterPanelModel.L_CARD_NO);

                    Session["PFDBYTE"] = pdfbyte;
                    Session["PDFDATA"] = setPDFdata();

                    log = StartInterface("directoryPath:" + directoryPath + "\r\n" + "fileName:" + fileName, "Generate PDF", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, model.CustomerRegisterPanelModel.L_CARD_NO, "WEB");

                    //Write file to NAS
                    var pathfile = directoryPath + "\\" + fileName + ".pdf";
                    PdfSecurity.WriteFile(pathfile, pdfbyte);

                    EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");
                }

                return directoryTempPath + "\\" + fileName + ".pdf";
            }
            catch (Exception ex)
            {
                EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());
                return "";
                throw ex;
            }
        }

        //Gen eApp 
        public string GeneratePDFApp(string CardNo, string orderNo, string Language, string contactNo, QuickWinPanelModel model)
        {
            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;
            InterfaceLogCommand log3 = null;
            try
            {
                log = StartInterface(model, "Generate PDF APP", orderNo, CardNo, "WEB");
                var html = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                var pathfileImpesontae = string.Empty;
                Session["OrderNoAPP"] = orderNo;
                Session["LanguageAPP"] = Language;
                Byte[] bytes;
                PDFDataQuery query = new PDFDataQuery();
                if (!string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(Language))
                {
                    query.orderNo = orderNo;
                    query.Language = Language;
                    query.isEApp = true;
                }
                else
                {
                    throw new Exception("QueryGeneratePDF parameter is null or empty");
                }
                //var htmlFromPackage = QueryGeneratePDFNew(query, model); //gen pdf new on code
                var htmlFromPackage = QueryGeneratePDF(query);

                if (!string.IsNullOrEmpty(htmlFromPackage))
                {
                    log2 = StartInterface(model, "GeneratePDFAPP_callStore", htmlFromPackage, CardNo, "WEB");
                    html = html + htmlFromPackage;
                    PicturePDF picturePDF = new PicturePDF();
                    html = html.Replace("{Sign}", "<img id='signature-pad' style='width: 100%; height:80px' src='" + Session["SIGNATURE1"].ToSafeString() + "'>");
                    html = html.Replace("{aisFibre}", "data:image/png;base64," + picturePDF.AisFibre);
                    Logger.Info("HTML :" + html);

                    bytes = htmlToPDF(html);
                    log3 = StartInterface(bytes, "GenPDF_EAPP_Page_download", "", CardNo, "WEB");
                    bytes = PdfSecurity.SetPasswordPdf(bytes, CardNo);

                    var queryName = new GetFormatFileNameEAPPQuery
                    {
                        ID_CardNo = CardNo,
                    };

                    var result = _queryProcessor.Execute(queryName);
                    if (result != null)
                    {
                        string fileName = result.file_name;
                        var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT_WEB" && l.Name == "Impersonate").SingleOrDefault();
                        var imagepathimer = @ImpersonateVar.LovValue4;
                        string user = ImpersonateVar.LovValue1;
                        string pass = ImpersonateVar.LovValue2;
                        string ip = ImpersonateVar.LovValue3;
                        string yearweek = (DateTime.Now.Year.ToString());
                        string monthyear = (DateTime.Now.Month.ToString("00"));
                        var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));
                        //imagepathimer = imagepathimerTemp;
                        Logger.Info("Start Impersonate:");
                        EndInterface("", log2, orderNo, "SUCCESS", "");


                        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass) && !string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(imagepathimer))
                        {
                            using (var impersonator = new Impersonator(user, ip, pass, false))
                            {
                                System.IO.Directory.CreateDirectory(imagepathimerTemp);
                                pathfileImpesontae = Path.Combine(imagepathimerTemp, fileName + ".pdf");
                                PdfSecurity.WriteFile(pathfileImpesontae, bytes);
                                UpdateFileName(orderNo, pathfileImpesontae, contactNo);
                            }

                            //DATA TEST LOCAL
                            //string test1 = imagepathimer.Substring(2, imagepathimer.Length - 3);
                            //string test2 = "D:\\PDF_EAPP";
                            //imagepathimerTemp = Path.Combine(test2, test1);
                            //System.IO.Directory.CreateDirectory(imagepathimerTemp);
                            //pathfileImpesontae = Path.Combine(imagepathimerTemp, fileName + ".pdf");
                            //PdfSecurity.WriteFile(pathfileImpesontae, bytes);

                            //pathfileImpesontae = Path.Combine(imagepathimerTemp, fileName + ".pdf");
                            //PdfSecurity.WriteFile(pathfileImpesontae, bytes);
                            //UpdateFileName(orderNo, pathfileImpesontae, contactNo);
                            EndInterface(pathfileImpesontae, log, orderNo, "Success", "");
                        }
                        else
                        {
                            throw new Exception("Can not write PDF file Impersonator");
                        }

                    }
                    else
                    {
                        throw new Exception("Can not call stored procedure GetFormatFileNameEAPPQuery");
                    }
                }
                else
                {
                    throw new Exception("Can not call stored procedure QueryGeneratePDF");
                }

                return pathfileImpesontae;
            }
            catch (Exception ex)
            {
                EndInterface("", log, orderNo, "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());
                return "";
            }
        }

        //Gen PDF
        public string GeneratePDF_HTML(QuickWinPanelModel model, string directoryPath,
            string directoryTempPath, string fileName, string isShop)
        {
            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;
            InterfaceLogCommand log3 = null;
            string orderNo = model.CustomerRegisterPanelModel.OrderNo;
            string CardNo = model.CustomerRegisterPanelModel.L_CARD_NO;
            string Language = "";
            var html = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            var pathfileImpesontae = string.Empty;
            string[] htmlPage = new string[2];
            Byte[] bytes;
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                Language = "T";
            }
            else
            {
                Language = "E";
            }
            try
            {
                log = StartInterface("", "Generate PDF", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, CardNo, "WEB");



                PDFDataQuery query = new PDFDataQuery();
                if (!string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(Language) && !string.IsNullOrEmpty(isShop))
                {
                    query.orderNo = orderNo;
                    query.Language = Language;
                    query.isEApp = false;
                    query.isShop = isShop;
                    query.pageNo = 1;
                }
                else
                {
                    throw new Exception("QueryGeneratePDFNew parameter is null or empty");
                }

                var htmlFromPackage = QueryGeneratePDFNew(query, model);
                if (!string.IsNullOrEmpty(htmlFromPackage))
                {
                    log2 = StartInterface(model, "GeneratePDFSumary_callStore", htmlFromPackage, CardNo, "WEB");
                    htmlPage[0] = html + htmlFromPackage;
                    if (isShop == "IP_CAMERA")
                    {
                        bytes = htmlToPDF(htmlPage[0]);
                    }
                    else if (isShop != "TOPUP")
                    {
                        query.pageNo = 2;
                        htmlFromPackage = QueryGeneratePDFNew(query, model);
                        if (!string.IsNullOrEmpty(htmlFromPackage))
                        {
                            htmlPage[1] = html + htmlFromPackage;
                            InterfaceLogCommand logTest = StartInterface<string[]>(htmlPage, "htmlToPDF", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, CardNo, "WEB");
                            bytes = htmlToPDF(htmlPage);
                        }
                        else
                        {
                            throw new Exception("Can not write PDF file QueryGeneratePDFNew2");
                        }
                    }
                    else
                    {
                        bytes = htmlToPDF(htmlPage[0]);
                    }
                    log3 = StartInterface(bytes, "GenPDF_Page_download", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, CardNo, "WEB");
                    bytes = PdfSecurity.SetPasswordPdf(bytes, CardNo);
                    Session["PFDBYTE"] = bytes;

                    var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT_WEB" && l.Name == "Impersonate").SingleOrDefault();
                    string imagepathimer = ImpersonateVar.LovValue4;
                    string user = ImpersonateVar.LovValue1;
                    string pass = ImpersonateVar.LovValue2;
                    string ip = ImpersonateVar.LovValue3;
                    string yearweek = (DateTime.Now.Year.ToString());
                    string monthyear = (DateTime.Now.Month.ToString("00"));
                    var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));
                    Logger.Info("Start Impersonate:");
                    EndInterface("", log2, orderNo, "SUCCESS", "");

                    if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass) && !string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(imagepathimer))
                    {
                        using (var impersonator = new Impersonator(user, ip, pass, false))
                        {
                            System.IO.Directory.CreateDirectory(imagepathimerTemp);
                            pathfileImpesontae = Path.Combine(imagepathimerTemp, fileName + ".pdf");
                            PdfSecurity.WriteFile(pathfileImpesontae, bytes);
                            UpdateFileName(orderNo, pathfileImpesontae, "");
                        }
                        //DATA TEST LOCAL

                        //string test1 = imagepathimer.Substring(2,imagepathimer.Length-3);
                        //string test2 = "D:\\PDF_EAPP";
                        //imagepathimerTemp = Path.Combine(test2, test1);
                        //System.IO.Directory.CreateDirectory(imagepathimerTemp);
                        //pathfileImpesontae = Path.Combine(imagepathimerTemp, fileName + ".pdf");
                        //PdfSecurity.WriteFile(pathfileImpesontae, bytes);
                        //pathfileImpesontae = Path.Combine(imagepathimerTemp, fileName + ".pdf");
                        EndInterface(pathfileImpesontae, log, orderNo, "Success", "");
                    }
                    else
                    {
                        throw new Exception("Can not write PDF file Impersonator");
                    }
                }
                else
                {
                    throw new Exception("Can not call stored procedure QueryGeneratePDFNew");
                }
                return pathfileImpesontae;
            }
            catch (Exception ex)
            {
                EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage: " + ex.RenderExceptionMessage());
                return "";
            }
        }

        //Convert HTML to PDF (Use TOPUP and eApp)
        private Byte[] htmlToPDF(string html)
        {
            InterfaceLogCommand log = null;
            log = StartInterface(html, "htmlToPDFEAPP", "", "", "WEB");
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var doc = new Document())
                    {

                        doc.SetMargins(doc.LeftMargin / 4, doc.RightMargin / 4, doc.TopMargin, doc.BottomMargin);

                        using (var writer = PdfWriter.GetInstance(doc, ms))
                        {
                            doc.Open();
                            doc.NewPage();//doc.NewPage();
                            using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                            {
                                using (var sr = new StringReader(html))
                                {

                                    Logger.Info("Get Font");
                                    //Path to our font
                                    //string arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
                                    string fonttttts = System.Web.HttpContext.Current.Server.MapPath("~/Content/fonts/tahoma.ttf");//Server.MapPath("~/Content/fonts/tahoma.ttf");
                                                                                                                                   //Register the font with iTextSharp
                                                                                                                                   ////iTextSharp.text.FontFactory.Register(fonttttts);
                                                                                                                                   ////iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);
                                    FontFactory.Register(fonttttts);
                                    FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);
                                    Logger.Info("StyleSheet");

                                    //Create a new stylesheet          
                                    StyleSheet ST = new StyleSheet(); ////iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();         
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FACE, "Tahoma");//Set the default body font to our registered font's internal name                       
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.ENCODING, BaseFont.IDENTITY_H); //Set the default encoding to support Unicode characters
                                    ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONTSIZE, "4");

                                    Logger.Info("Image Providers");
                                    Dictionary<string, object> providers = new Dictionary<string, object>();
                                    providers.Add(HTMLWorker.IMG_PROVIDER, new ImageThing(doc));
                                    Logger.Info("HTML Parser to List");
                                    List<IElement> list = HTMLWorker.ParseToList(sr, ST, providers);////Parse our HTML using the stylesheet created above

                                    foreach (var element in list)//htmlWorker.Parse(sr);
                                    {
                                        doc.Add(element);
                                    }
                                    Logger.Info("Finished HTML Parse");
                                    EndInterface(doc, log, "", "SUCCESS", "");
                                }
                            }
                            doc.Close();
                        }
                    }
                    return ms.ToArray();
                }

            }
            catch (Exception ex)
            {
                EndInterface("", log, "", "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage ConvertPDF EAPP: " + ex.RenderExceptionMessage());
                Byte[] error = null;
                return error;
            }
        }

        //Convert HTML to PDF (Use New Register)
        private Byte[] htmlToPDF(string[] html)
        {
            InterfaceLogCommand log = null;
            log = StartInterface(html, "htmlToPDFSumary", "", "", "WEB");
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var doc = new Document())
                    {

                        doc.SetMargins(doc.LeftMargin / 2, doc.RightMargin / 2, doc.TopMargin, doc.BottomMargin);

                        using (var writer = PdfWriter.GetInstance(doc, ms))
                        {

                            doc.Open();

                            using (var htmlWorker = new HTMLWorker(doc))
                            //using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                            {
                                for (int i = 0; i < html.Count(); i++)
                                {
                                    using (var sr = new StringReader(html[i]))
                                    {
                                        string fonttttts = Server.MapPath("~/Content/fonts/tahoma.ttf");
                                        FontFactory.Register(fonttttts);
                                        FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);////iTextSharp.text.FontFactory.Register(fonttttts);
                                                                                            ////iTextSharp.text.FontFactory.GetFont(fonttttts, 6f, BaseColor.BLACK);

                                        //Create a new stylesheet                             
                                        StyleSheet ST = new StyleSheet();////iTextSharp.text.html.simpleparser.StyleSheet ST = new iTextSharp.text.html.simpleparser.StyleSheet();
                                        ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FACE, "Tahoma");
                                        ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.ENCODING, BaseFont.IDENTITY_H);
                                        ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONTSIZE, "4");

                                        Logger.Info("Image Providers Page1");
                                        Dictionary<string, object> providers = new Dictionary<string, object>();
                                        providers.Add(HTMLWorker.IMG_PROVIDER, new ImageThing(doc));

                                        Logger.Info("HTML Parser to List Page1");
                                        List<IElement> list = HTMLWorker.ParseToList(sr, ST, providers);
                                        foreach (var element in list)
                                        {
                                            doc.Add(element);
                                        }
                                        EndInterface(doc, log, "", "SUCCESS", "");
                                        Logger.Info("Finished HTML Parse Page1");
                                    }
                                    if (i != html.Count() - 1)
                                        doc.NewPage();
                                }
                            }
                            doc.Close();
                        }
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                EndInterface("", log, "", "ERROR", "ErrorMessage: " + ex.GetErrorMessage() + "\r\n" + "RenderExceptionMessage ConvertPDF Summary: " + ex.RenderExceptionMessage());
                Byte[] error = null;
                return error;
            }
        }

        private void GetPDFLOV()
        {
            if (!base.LovData.Any())
            {
                var query = new GetLovQuery
                {
                    LovType = "",
                    LovName = ""
                };

                base.LovData = _queryProcessor.Execute(query);
            }
        }

        private List<LovScreenValueModel> GetPDFConfig()
        {
            try
            {
                List<LovValueModel> config = null;
                config = base.LovData.Where(l => l.LovValue4 != null && (l.Type != "FBB_CONSTANT" && (l.Name != "Impersonate" || l.Name != "UploadImageFile" || l.Name != "LOAD_SUBCONTRACTOR_TIMESLOT"))).OrderBy(l => l.OrderBy).ThenBy(n => n.Id).ToList();

                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
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

        private List<LovScreenValueModel> GetPDFPlugAndPlay()
        {
            try
            {
                List<LovValueModel> config = null;
                config = base.LovData.Where(l => l.Type == "TERM_AND_CONDITION_PLUGANDPLAY").ToList();

                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue1,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue2,
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

        private List<LovScreenValueModel> SetPDFLOVView()
        {
            try
            {
                List<LovValueModel> config = null;
                config = base.LovData.Where(l => l.LovValue5 == "PDF001").OrderBy(t => t.OrderBy).ToList();

                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue1,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        DisplayValue = l.LovValue2,
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

        public string setPDFdata()
        {
            System.Data.DataTable dtAll = tempTable1.Copy();
            dtAll.Merge(tempTable2);
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(dtAll);

            return JSONString;
        }

        public JsonResult getPDFdata()
        {
            string JSONString = string.Empty;
            if (Session["PDFDATA"] != null)
            {
                JSONString = (string)Session["PDFDATA"];
            }


            return Json(JSONString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPDFPlugAndPlay()
        {
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(GetPDFPlugAndPlay());
            return Json(JSONString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPDFSignature()
        {
            string JSONString = string.Empty;
            string signature1 = string.Empty;
            string signature2 = string.Empty;
            Dictionary<string, string> dicSignature = new Dictionary<string, string>();

            if (Session["SIGNATURE1"] != null)
            {
                signature1 = (string)Session["SIGNATURE1"];
                dicSignature.Add("SIGNATURE1", signature1);
            }
            if (Session["SIGNATURE2"] != null)
            {
                signature2 = (string)Session["SIGNATURE2"];
                dicSignature.Add("SIGNATURE2", signature2);
            }
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(dicSignature);

            return Json(JSONString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPDFLOVView()
        {
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(SetPDFLOVView());
            return Json(JSONString, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 10)]
        public IEnumerable<LovValueModel> GetTermCon()
        {
            var data = base.LovData
                .Where(l => l.Type.Equals("TERM_AND_CONDITION_WIRE"));

            return data;
        }

        //Create HTML eApp
        public string QueryGeneratePDF(PDFDataQuery query)
        {
            InterfaceLogCommand log = null;
            var pdfData = new PDFData();

            try
            {
                log = StartInterface<PDFDataQuery>(query, "QueryGeneratePDF", query.orderNo, "", "");

                pdfData = _queryProcessor.Execute(query);

                EndInterface<PDFData>(pdfData, log, query.orderNo, "Success", "");

                return pdfData.str_pdf_html;

            }
            catch (System.Exception ex)
            {

                EndInterface<PDFDataQuery>(query, log, query.orderNo,
                   ex.Message, ex.GetErrorMessage());

                return ex.Message.ToString();
            }
        }

        //Create HTML PDF
        public string QueryGeneratePDFNew(PDFDataQuery query, QuickWinPanelModel model)
        {
            var getFlagPromotion = GetScreenConfig("98");
            var flagPromotion = getFlagPromotion.Any(c => c.Name == "SCREEN_PRICE_MONTH_FLAG") ? getFlagPromotion.FirstOrDefault(c => c.Name == "SCREEN_PRICE_MONTH_FLAG").DisplayValue : "";
            InterfaceLogCommand log = null;
            var pdfData = new PDFData();

            string Language = "";
            if (SiteSession.CurrentUICulture.IsThaiCulture())
                Language = "T";
            else
                Language = "E";

            var url_web = "";
            url_web = System.Web.HttpContext.Current.Request.Url.ToSafeString().Split('?')[0]; //get url current web

            PdfOnWebQuery pdfOnWebQuery = new PdfOnWebQuery();
            PdfOnWebModel check_gen_pdfonweb = new PdfOnWebModel();
            PicturePDF picturePDF = new PicturePDF();
            try
            {
                log = StartInterface<PdfOnWebQuery>(pdfOnWebQuery, "QueryGeneratePDFNew", query.orderNo, "", "");
                log = StartInterface<QuickWinPanelModel>(model, "QueryGeneratePDFNew_Model", query.orderNo, "", "");

                var str_pdf_html = "";


                // page 1 (Register , Existing)
                if (query.pageNo == 1)
                {
                    if (!string.IsNullOrEmpty(query.orderNo) && !string.IsNullOrEmpty(query.Language) && !string.IsNullOrEmpty(query.isShop))
                    {
                        #region service pdfOnWebQuery
                        pdfOnWebQuery.orderNo = query.orderNo;
                        pdfOnWebQuery.Language = query.Language;
                        pdfOnWebQuery.isShop = query.isShop;

                    }
                    else
                    {
                        throw new Exception("Can not call stored procedure PdfOnWebQuery Summary");
                    }
                    check_gen_pdfonweb = _queryProcessor.Execute(pdfOnWebQuery);
                    #endregion
                    if (check_gen_pdfonweb != null)
                    {

                        //ipcamera
                        if (query.isShop == "IP_CAMERA")
                        {
                            //Existing HTML Thai
                            if (Language == "T")
                            {
                                //START_HTML
                                str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryTable'>";
                                str_pdf_html += "<tbody>";

                                //HEADER
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='8'  align='right'><img src='data:image/png;base64," + picturePDF.AddServiceTH + "' border=0 height=161 width=700 /></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='2'  align='left'><img src='data:image/png;base64," + picturePDF.Img_logo1 + "' border=0 height=138 width=198 /></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //SUMMARY
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top;' colspan='10'>";
                                str_pdf_html += "<b>สรุปข้อมูล</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //PERSONAL_INFO
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>ข้อมูลผู้สมัคร</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INDIVIDUAL_INFO
                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>บุคคลธรรมดา</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_TITLE + " " + model.CustomerRegisterPanelModel.L_FIRST_NAME + " " + model.CustomerRegisterPanelModel.L_LAST_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>หน่วยงานราชการ/รัฐวิสาหกิจ</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อหน่วยงานราชการ/ชื่อบริษัท</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CARD_TYPE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ประเภทบัตร</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_TYPE + "</td>";
                                //CARD_NO
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>เลขที่บัตร</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_NO + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    //GENDER
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>เพศ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GENDER + "</td>";
                                    //BIRTHDAY
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>วันเกิด</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_BIRTHDAY + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType != "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ชื่อ-นามสกุล ผู้เป็นตัวแทนติดต่อ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CONTACT_PERSON + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTACT_MOBILE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>หมายเลขที่ใช้ในการติดต่อ</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_MOBILE + "</td>";
                                //EMAIL
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Email</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_EMAIL + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CONTACT_TIME
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='3'>เวลาที่สะดวกให้ติดต่อกลับ</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_SPECIFIC_TIME + "</td>";
                                //INSTALL_DATE
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>วันที่ให้เข้าติดตั้ง</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_INSTALL_DATE + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //YOUR_PACKAGE
                                //str_pdf_html += "<tr>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                //str_pdf_html += "<b>แพ็กเกจที่เลือก</b>";
                                //str_pdf_html += "</td>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "</tr>";

                                //ONTOP_FIXEDLINE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_FIXEDLINE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>แพ็กเกจเสริม :</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>โทรศัพท์พื้นฐาน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //HORIZON_LINE
                                //str_pdf_html += "<tr>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='3'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "</tr>";

                                ////EQUIPMENT_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].EQUIPMENT_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>ค่าติดตั้ง</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_1
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_1_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //EQUIPMENT_FEE_MULTI_PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE_FLAG != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                    str_pdf_html += "<b>ค่าติดตั้ง (***ชำระให้แก่ช่างโดยตรง ณ วันติดตั้ง)</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //INSTALL_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าติดตั้งโทรศัพท์พื้นฐานพร้อมอุปกรณ์</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //DISCOUNT_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ส่วนลดค่าติดตั้งโทรศัพท์พื้นฐานพร้อมอุปกรณ์</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //DISCOUNT_ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_NAME_RECURRING_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_NAME_RECURRUNG_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                ////TOTAL_1
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != null && check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MONTHLY_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                str_pdf_html += "<b>ค่าบริการรายเดือน</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //MA_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าบำรุงรักษาคู่สายโทรศัพท์บ้าน</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_1_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //SPOT_1_INSTALL_CONTENT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_CONTENT_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                // ONTOP_IP_CAMERA
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //SPOT_2_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //TOTAL_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PRO_RATE_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                str_pdf_html += "<b>ค่าบริการเฉลี่ยรายวัน (เรียกเก็บในบิลแรกเท่านั้น)</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CAL_PER_DAY
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>คิดเฉลี่ยต่อ 1 วัน</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //TOTAL_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                    str_pdf_html += "<b>รวมยอดโดยประมาณที่ต้องชำระ</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //EXCLUDE_VAT
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='6' allign='left'>ราคานี้ยังไม่รวมภาษีมูลค่าเพิ่ม 7%</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='1' allign='left'></td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='1'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INSTALL_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>ข้อมูลสถานที่ติดตั้ง</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //BILLING_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>ข้อมูลทีอยู่ในการจัดส่งเอกสาร</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td></tr><tr><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //END_HTML
                                str_pdf_html += "</tbody>";
                                str_pdf_html += "</table>";
                            }
                            //Existing HTML English
                            else
                            {
                                //START_HTML
                                str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryTable'>";
                                str_pdf_html += "<tbody>";

                                //HEADER
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='8'  align='right'><img src='data:image/png;base64," + picturePDF.AddServiceEN + "' border=0 height=161 width=700 /></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='2'  align='left'><img src='data:image/png;base64," + picturePDF.Img_logo1 + "' border=0 height=138 width=198 /></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //SUMMARY
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top;' colspan='10'>";
                                str_pdf_html += "<b>Summary</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //PERSONAL_INFO
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>Your Information</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INDIVIDUAL_INFO
                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>Residential</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Name</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_TITLE + " " + model.CustomerRegisterPanelModel.L_FIRST_NAME + " " + model.CustomerRegisterPanelModel.L_LAST_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>Government/State Enterprise</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Name of Gevernment/Business</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CARD_TYPE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Card Type</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_TYPE + "</td>";
                                //CARD_NO
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ID card no</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_NO + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    //GENDER
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Gender</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GENDER + "</td>";
                                    //BIRTHDAY
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Birthday</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_BIRTHDAY + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType != "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Contact Person</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CONTACT_PERSON + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTACT_MOBILE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Contact Number</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_MOBILE + "</td>";
                                //EMAIL
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Email</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_EMAIL + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CONTACT_TIME
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Specific Contact Time</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_SPECIFIC_TIME + "</td>";
                                //INSTALL_DATE
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Req. install date</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_INSTALL_DATE + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //YOUR_PACKAGE
                                //str_pdf_html += "<tr>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                //str_pdf_html += "<b>Your Package</b>";
                                //str_pdf_html += "</td>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "</tr>";

                                //ONTOP_FIXEDLINE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_FIXEDLINE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>On top package :</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>Fixed Line</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ONTOP_HOOQ
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Free! HOOQ Package for " + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ + " months</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //HORIZON_LINE
                                //str_pdf_html += "<tr>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='3'></td>";
                                //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //str_pdf_html += "</tr>";

                                //EQUIPMENT_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].EQUIPMENT_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>Installation fee</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //IP_CAMERA_SPOT_1
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_1_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //EQUIPMENT_FEE_MULTI_PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE_FLAG != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                    str_pdf_html += "<b>Installation fee (***pay to installer on installation date)</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //INSTALL_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Fixed Line Installation fee & equipment</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //DISCOUNT_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Discount Fixed Line Installation fee & equipment</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //DISCOUNT_ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_NAME_RECURRING_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_NAME_RECURRUNG_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //TOTAL_1
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != null && check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MONTHLY_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                str_pdf_html += "<b>Monthly service fee</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //MA_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Fixed Line maintenance fee</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //ONTOP_IP_CAMERA
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //SPOT_1_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_1_INSTALL_CONTENT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_CONTENT_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_2_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //TOTAL_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PRO_RATE_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                str_pdf_html += "<b>Average daily (charged on the first bill only)</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CAL_PER_DAY
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Calculated per one day</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //TOTAL_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                    str_pdf_html += "<b>Total estimated to be paid</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //EXCLUDE_VAT
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='6' allign='left'>All service charge and installation fee are excluding VAT 7%</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='1' allign='left'></td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='1'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INSTALL_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>Installation Address</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //BILLING_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>Billing Address</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td></tr><tr><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //END_HTML
                                str_pdf_html += "</tbody>";
                                str_pdf_html += "</table>";
                            }
                        }
                        //Register
                        else if (query.isShop != "TOPUP")
                        {
                            //14.05.23 e-app first edit image base 64
                            //Register HTML Thai
                            if (Language == "T")
                            {
                                str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryTable'>";
                                str_pdf_html += "<tbody>";

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='8'  align='right'>";

                                str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Thai_5 + " ' border=0 height=161 width=700 />";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='2'  align='left'>";

                                str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Img_logo1 + " ' border=0 height=138 width=198 />";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top;' colspan='10'>";
                                str_pdf_html += "<b>สรุปข้อมูล</b>";
                                str_pdf_html += "</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>ข้อมูลผู้สมัคร</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //บุคคลธรรมดา
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<b>บุคคลธรรมดา</b>";
                                }
                                else
                                {
                                    str_pdf_html += "<b>หน่วยงานราชการ/รัฐวิสาหกิจ</b>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_TITLE + " " + model.CustomerRegisterPanelModel.L_FIRST_NAME + " " + model.CustomerRegisterPanelModel.L_LAST_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อหน่วยงานราชการ/ชื่อบริษัท</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ประเภทบัตร</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CARD_TYPE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>เลขที่บัตร</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CARD_NO + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>เพศ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_GENDER + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>วันเกิด</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_BIRTHDAY + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType != "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ชื่อ-นามสกุล ผู้เป็นตัวแทนติดต่อ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CONTACT_PERSON + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>หมายเลขที่ใช้ในการติดต่อ</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_MOBILE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Email</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_EMAIL + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='3'>เวลาที่สะดวกให้ติดต่อกลับ</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_SPECIFIC_TIME + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>วันที่ให้เข้าติดตั้ง</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_INSTALL_DATE + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>สรุปรายการแพ็กเกจที่เลือก</b>";
                                str_pdf_html += "</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>แพ็กเกจหลัก :</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'> " + check_gen_pdfonweb.LIST_PDF_CUR[0].MAIN_PACKAGE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_WIFI_ROUTER == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>แพ็กเกจเสริม :</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>ค่าติดตั้งอินเทอร์เน็ตพร้อมอุปกรณ์รับส่งสัญญาณ (WiFi router)</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                //ONTOP_MESH
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH != null) //ฟรี! ค่าเปิดใช้บริการ AIS PLAYBOX มูลค่า 2,500 บาท
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                //MESH_SPOT_1_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ONTOP_IP_CAMERA
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA + " " + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_PRICE_7DAY + " " + "บาท" + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                //IP_CAMERA_SPOT_1_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_INSTALL_PLAYBOX != null)//AIS PLAYBOX Monthly Service Fee 100 THB for spot No. 1
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_INSTALL_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX != null) //ฟรี! ค่าเปิดใช้บริการ AIS PLAYBOX มูลค่า 2,500 บาท
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>ฟรี! ค่าบริการ HOOQ " + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ + " " + "เดือน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_BUNDLING == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>พิเศษ! ส่วนลดค่าแพ็กเกจ AIS Fibre 10% สำหรับลูกค้า AIS</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_CONTENT_PLAYBOX_FREE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //ONTOP_CONTENT_PLAYBOX_FREE_2
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_CONTENT_PLAYBOX_FREE_2 + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_CONTENT_PLAYBOX_SALE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_FIXEDLINE == "true")
                                {

                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>โทรศัพท์พื้นฐาน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_BOOST_SPEED + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_WIFI_LOG_FREE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Business WiFi Log ฟรีถึง 31 ธ.ค.63 จากนั้น ค่าบริการรายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_WIFI_LOG_SALE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Business WiFi Log ค่าบริการายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //if(check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_DISCOUNT != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_DISCOUNT != "0.00")
                                // " + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_DISCOUNT + "บาท
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_DISCOUNT != "false") // old ONTOP_SPECIAL_DISCOUNT
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>ส่วนลดค่าบริการรายเดือน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 37.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='8'>";
                                    str_pdf_html += "<b>ค่าใช้จ่ายที่ต้องชำระในวันที่ติดตั้ง</b>";
                                    str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ENTRY_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าแรกเข้า</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ENTRY_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PAY_IN_ADVANCE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>แพคเกจค่าบริการล่วงหน้า</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PAY_IN_ADVANCE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าอุปกรณ์ Wi-Fi router</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIRING_FEE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 37.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='8'>";
                                    str_pdf_html += "<b>ค่าเดินสายที่ต้องชำระในวันติดตั้ง (ถ้ามี)</b>";
                                    str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIRING_DETAIL != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].WIRING_DETAIL + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }



                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].EQUIPMENT_FEE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>ค่าติดตั้งและอุปกรณ์</b>";
                                    str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE_FLAG == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าติดตั้งอินเทอร์เน็ตพร้อมอุปกรณ์รับส่งสัญญาณ (WiFi router)</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>4,800.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //Edit Condition sammary report 25/09/23
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_WIFI_ROUTER_FEE != "false" && !string.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_WIFI_ROUTER_FEE))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ส่วนลดค่าติดตั้ง</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_WIFI_ROUTER_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }



                                //MESH_SPOT_1
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_1_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_2
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_2_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_3
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_3_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_1
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_1_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //if(check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != "false") //ค่าเปิดใช้บริการ AIS PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_WORDDING_INSTALL != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_WORDDING_INSTALL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_PRICE_INSTALL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //if(check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != "false") //ส่วนลดค่าเปิดใช้บริการ AIS PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX != null) //ส่วนลดค่าเปิดใช้บริการ AIS PLAYBOX
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_PRICE_INSTALL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าติดตั้งโทรศัพท์พื้นฐานพร้อมอุปกรณ์</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //DISCOUNT_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ส่วนลดค่าติดตั้งโทรศัพท์พื้นฐานพร้อมอุปกรณ์</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //Edit Condition sammary report 25/09/23
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != "false" && !string.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }



                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                str_pdf_html += "<b>ค่าบริการรายเดือน</b>";
                                str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //PACKAGE_NAME / RECURRING_CHARGE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PACKAGE_NAME + "</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].RECURRING_CHARGE + "</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //SPECIAL_BUNDLING_DISCOUNT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_BUNDLING_DISCOUNT != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ส่วนลดค่าแพ็กเกจ AIS Fibre 10% สำหรับลูกค้า AIS</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_BUNDLING_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                //mesh arpu
                                if (flagPromotion == "Y")
                                {
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_1_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_1_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_1_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }

                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_DISCOUNT_NAME))
                                    {

                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_DISCOUNT_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_DISCOUNT_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_DISCOUNT_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_DISCOUNT_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_DISCOUNT_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH != null) //ฟรี! ค่าเปิดใช้บริการ AIS PLAYBOX มูลค่า 2,500 บาท
                                {
                                    if (flagPromotion != "Y" && flagPromotion == "N")
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH_CHARGE + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }

                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA != null)
                                {
                                    if (flagPromotion != "Y" && flagPromotion == "N")
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA + " " + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_PRICE_7DAY + " " + "บาท" + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_PRICE_7DAY + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }

                                }

                                //MA_PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_PLAYBOX != "false") //AIS Playbox Monthly Service Fee //ONTOP_AIS_INSTALL_PLAYBOX
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_INSTALL_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTENT_PLAYBOXNAME_FREE , CONTENT_PLAYBOX_FREE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE != null) //CONTENT_RECURRING_CHARGE_FREE = CONTENT_PLAYBOX_FREE --->CONTENT_PLAYBOXNAME_FREE
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_FREE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //CONTENT_PLAYBOXNAME_FREE_2 , CONTENT_PLAYBOX_FREE_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE_2 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_FREE_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTENT_PLAYBOX_NAME_SALE / CONTENT_RECURRING
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_NAME_SALE != null) //CONTENT_PLAYBOX_NAME_SALE/ CONTENT_RECURRING
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_NAME_SALE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MA_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าบำรุงรักษาคู่สายโทรศัพท์บ้าน</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ONTOP_BOOST_SPEED_NAME / BOOST_SPEED_RECURRING_CHARGE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_BOOST_SPEED_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_BOOST_SPEED_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BOOST_SPEED_RECURRING_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //TRIAL_WIFI_LOG
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TRIAL_WIFI_LOG == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Business WiFi Log ฟรีถึง 31 ธ.ค.63 จากนั้น ค่าบริการรายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //WIFI_LOG
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_LOG == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Business WiFi Log ค่าบริการายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //TOTAL_2
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}


                                ////TOTAL_2_DISCOUNT
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2_DISCOUNT != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2_DISCOUNT + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //Wait FIX PDF
                                //SPECIAL_DISCOUNT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_DISCOUNT != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ส่วนลดค่าบริการรายเดือน</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //R23.08 Billing Consolidate Change TOTAL_2 to BILL_SUM_TOTAL 
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_TOTAL != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_TOTAL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                str_pdf_html += "<b>ค่าบริการเฉลี่ยรายวัน (เรียกเก็บในบิลแรกเท่านั้น)</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //CAL_PER_DAY
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>คิดเฉลี่ยต่อ 1 วัน</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}
                                //Bill Consolidate R23.08_20230817
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>คิดเฉลี่ย " + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG_DAY + " วัน(" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_AVG_PER_DAY + "บาท ต่อ 1 วัน)</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CAL_PER_DAY_DISCOUNT
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY_DISCOUNT != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>คิดเฉลี่ยต่อ 1 วัน</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY_DISCOUNT + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}
                                //Bill Consolidate R23.08_20230817
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY_DISCOUNT != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>คิดเฉลี่ย " + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG_DAY + " วัน(" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_AVG_PER_DAY + "บาท ต่อ 1 วัน)</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }



                                ////TOTAL_3
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                //    str_pdf_html += "<b>รวมยอดโดยประมาณที่ต้องชำระ</b>";
                                //    str_pdf_html += "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}


                                ////TOTAL_3_DISCOUNT
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3_DISCOUNT != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                //    str_pdf_html += "<b>รวมยอดโดยประมาณที่ต้องชำระ</b>";
                                //    str_pdf_html += "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3_DISCOUNT + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //Bill Consolidate R23.08_20230817
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_TOTAL != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                    str_pdf_html += "<b>รวมยอดโดยประมาณที่ต้องชำระ</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_TOTAL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='6' allign='left'>ราคานี้ยังไม่รวมภาษีมูลค่าเพิ่ม 7%</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='1' allign='left'></td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='1'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //START Officer
                                if (query.isShop == "N")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>ข้อมูลสถานที่ติดตั้ง</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_MOO

                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>เลขที่</td><td style='width: 12.5%; vertical-align: top;' colspan='1'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2 + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='1'>หมู่ " + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOO + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อหมู่บ้าน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOOBAN + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ชื่อคอนโด/อาคารสำนักงาน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ห้อง</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROOM + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ตรอก/ซอย</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_SOI + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ถนน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROAD + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>แขวง/ตำบล</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_TUMBOL + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>เขต/อำเภอ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_AMPHUR + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;' colspan='3'>จังหวัด</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_PROVINCE + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>รหัสไปรษณีย์</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>ข้อมูลทีอยู่ในการจัดส่งเอกสาร</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>เลขที่</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='1'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_HOME_NUMBER_2 + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='1'>หมู่ " + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOO + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อหมู่บ้าน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOOBAN + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    //str_pdf_html += "<tr>";
                                    //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>เลขที่</td>";
                                    //str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='1'>{HouseNo}</td>";
                                    //str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อหมู่บ้าน</td>";
                                    //str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>{Village}</td>";
                                    //str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    //str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ชื่อคอนโด/อาคารสำนักงาน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ห้อง</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROOM + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ตรอก/ซอย</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_SOI + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ถนน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROAD + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>แขวง/ตำบล</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_TUMBOL + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>เขต/อำเภอ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_AMPHUR + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";


                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;' colspan='3'>จังหวัด</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_PROVINCE + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>รหัสไปรษณีย์</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE + "</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }
                                //END Officer

                                str_pdf_html += "</tbody>";
                                str_pdf_html += "</table>";
                            }
                            //Register HTML Eng
                            else
                            {
                                str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryTable'>";
                                str_pdf_html += "<tbody>";

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='8'  align='right'>";

                                str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Thai_5 + " ' border=0 height=161 width=700 />";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='2'  align='left'>";

                                str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Img_logo1 + "' border=0 height=138 width=198 />";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top;' colspan='10'>";
                                str_pdf_html += "<b>Summary</b>";
                                str_pdf_html += "</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>Your Information</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //บุคคลธรรมดา
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<b>Residential</b>";
                                }
                                else
                                {
                                    str_pdf_html += "<b>Government/State Enterprise</b>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Name</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_TITLE + " " + model.CustomerRegisterPanelModel.L_FIRST_NAME + " " + model.CustomerRegisterPanelModel.L_LAST_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Name of Gevernment/Business</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Card Type</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CARD_TYPE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ID card no</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CARD_NO + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Gender</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_GENDER + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Birthday</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'> " + model.CustomerRegisterPanelModel.L_BIRTHDAY + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType != "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Contact Person</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CONTACT_PERSON + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Contact Number</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_MOBILE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Email</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_EMAIL + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Specific Contact Time</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_SPECIFIC_TIME + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Req. install date</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_INSTALL_DATE + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>Your Package</b>";
                                str_pdf_html += "</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>Main package :</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'> " + check_gen_pdfonweb.LIST_PDF_CUR[0].MAIN_PACKAGE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_WIFI_ROUTER == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>On top package :</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>Installation fee with WiFi router</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                //ONTOP_MESH
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH != null) //ฟรี! ค่าเปิดใช้บริการ AIS PLAYBOX มูลค่า 2,500 บาท
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                //MESH_SPOT_1_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ONTOP_IP_CAMERA
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA + " " + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_PRICE_7DAY + " " + "Baht" + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                //IP_CAMERA_SPOT_1_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != null && check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_INSTALL_PLAYBOX != null)//AIS PLAYBOX Monthly Service Fee 100 THB for spot No. 1
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_INSTALL_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX != null) //ฟรี! ค่าเปิดใช้บริการ AIS PLAYBOX มูลค่า 2,500 บาท
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Free! HOOQ Package for " + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ + " " + "months</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_BUNDLING == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Special! Discount 10% on AIS Fibre Package for AIS Customers</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_CONTENT_PLAYBOX_FREE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //ONTOP_CONTENT_PLAYBOX_FREE_2
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_CONTENT_PLAYBOX_FREE_2 + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_CONTENT_PLAYBOX_SALE + "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_FIXEDLINE == "true")
                                {

                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Fixed Line</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }

                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_BOOST_SPEED + "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_WIFI_LOG_FREE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Business WiFi Log ฟรีถึง 31 ธ.ค.63 จากนั้น ค่าบริการรายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_WIFI_LOG_SALE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Business WiFi Log ค่าบริการายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //if(check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_DISCOUNT != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_DISCOUNT != "0.00")
                                // " + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_SPECIAL_DISCOUNT + "บาท
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_DISCOUNT != "false") // old ONTOP_SPECIAL_DISCOUNT
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Special! Promotion discount</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 37.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='8'>";
                                    str_pdf_html += "<b>Installation fee (Charge on installation date)</b>";
                                    str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ENTRY_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Entry Fee</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ENTRY_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PAY_IN_ADVANCE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Pay in advance</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PAY_IN_ADVANCE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Wi-Fi Router Equipment Fee</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIRING_FEE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 37.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='8'>";
                                    str_pdf_html += "<b>The wiring to be paid on the installation (if any)</b>";
                                    str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIRING_DETAIL != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].WIRING_DETAIL + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].EQUIPMENT_FEE == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>Installation and equipment fee</b>";
                                    str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE_FLAG == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Installation fee with WiFi router</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>4,800.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //Edit Condition sammary report 25/09/23
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_WIFI_ROUTER_FEE != "false" && !string.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_WIFI_ROUTER_FEE))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Promotional discount</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_WIFI_ROUTER_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //MESH_SPOT_1
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_1_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_2
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_2_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_3
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MESH_SPOT_3_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_1
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_1_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_1_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_2_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_3_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_4_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_4_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //IP_CAMERA_SPOT_5_DISCOUNT_NAME
                                if ((check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != null) && (check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME != "false"))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_SPOT_5_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //if(check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != "false") //ค่าเปิดใช้บริการ AIS PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_WORDDING_INSTALL != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_WORDDING_INSTALL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_PRICE_INSTALL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //if(check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != "false") //ส่วนลดค่าเปิดใช้บริการ AIS PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX != null) //ส่วนลดค่าเปิดใช้บริการ AIS PLAYBOX
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_PB_PRICE_INSTALL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Fixed Line Installation fee & equipment</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //DISCOUNT_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Discount Fixed Line Installation fee & equipment</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //Edit Condition sammary report 25/09/23 TOTAL_1
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != "false" && !string.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1))
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MONTHLY_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                str_pdf_html += "<b>Monthly service fee</b>";
                                str_pdf_html += "</td><td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //PACKAGE_NAME / RECURRING_CHARGE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PACKAGE_NAME + "</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].RECURRING_CHARGE + "</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //SPECIAL_BUNDLING_DISCOUNT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_BUNDLING_DISCOUNT != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Discount 10% on AIS Fibre Package for AIS Customers</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_BUNDLING_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //mesh arpu
                                if (flagPromotion == "Y")
                                {
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_1_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_1_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_1_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }

                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_DISCOUNT_NAME))
                                    {

                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_DISCOUNT_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_2_DISCOUNT_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                    if (!String.IsNullOrEmpty(check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_DISCOUNT_NAME))
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_DISCOUNT_NAME + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MESH_WIFI_SPOT_3_DISCOUNT_REC + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH != null) //ฟรี! ค่าเปิดใช้บริการ AIS PLAYBOX มูลค่า 2,500 บาท
                                {
                                    if (flagPromotion != "Y" && flagPromotion == "N")
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_MESH_CHARGE + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }
                                }

                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA != null)
                                {
                                    if (flagPromotion != "Y" && flagPromotion == "N")
                                    {
                                        str_pdf_html += "<tr>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_IP_CAMERA + " " + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_PRICE_7DAY + " " + "Baht" + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].IP_CAMERA_PRICE_7DAY + "</td>";
                                        str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                        str_pdf_html += "</tr>";
                                    }

                                }

                                //MA_PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_PLAYBOX != "false") //AIS Playbox Monthly Service Fee //ONTOP_AIS_INSTALL_PLAYBOX
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_AIS_INSTALL_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_PLAYBOX + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTENT_PLAYBOXNAME_FREE / CONTENT_PLAYBOX_FREE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE != null) //CONTENT_RECURRING_CHARGE_FREE = CONTENT_PLAYBOX_FREE --->CONTENT_PLAYBOXNAME_FREE
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_FREE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTENT_PLAYBOXNAME_FREE_2 , CONTENT_PLAYBOX_FREE_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE_2 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOXNAME_FREE_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_FREE_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTENT_PLAYBOX_NAME_SALE / CONTENT_RECURRING
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_NAME_SALE != null) //CONTENT_PLAYBOX_NAME_SALE/ CONTENT_RECURRING
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_PLAYBOX_NAME_SALE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CONTENT_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MA_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Fixed Line maintenance fee</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ONTOP_BOOST_SPEED_NAME / BOOST_SPEED_RECURRING_CHARGE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_BOOST_SPEED_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_BOOST_SPEED_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BOOST_SPEED_RECURRING_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //TRIAL_WIFI_LOG
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TRIAL_WIFI_LOG == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Business WiFi Log ฟรีถึง 31 ธ.ค.63 จากนั้น ค่าบริการรายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //WIFI_LOG
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_LOG == "true")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Business WiFi Log ค่าบริการายเดือน 199 บาท</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //TOTAL_2
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}


                                //TOTAL_2_DISCOUNT
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2_DISCOUNT != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2_DISCOUNT + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //SPECIAL_DISCOUNT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_DISCOUNT != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Promotion discount</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPECIAL_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //R23.08 Billing Consolidate Change TOTAL_2 to BILL_SUM_TOTAL 
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_TOTAL != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_TOTAL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                str_pdf_html += "<b>Average daily (charged on the first bill only)</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //CAL_PER_DAY
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Calculated per one day</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //R23.08 Billing Consolidate
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Calculated average per " + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG_DAY + " days (" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_AVG_PER_DAY + " Baht per 1 day)</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //CAL_PER_DAY_DISCOUNT
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY_DISCOUNT != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Calculated per one day</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY_DISCOUNT + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}


                                //R23.08 Billing Consolidate
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY_DISCOUNT != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Calculated average per " + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG_DAY + " days (" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_AVG_PER_DAY + " Baht per 1 day)</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_SUM_AVG + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }



                                //TOTAL_3
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                //    str_pdf_html += "<b>Total estimated to be paid</b>";
                                //    str_pdf_html += "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}


                                //TOTAL_3_DISCOUNT
                                //if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3_DISCOUNT != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                //    str_pdf_html += "<b>Total estimated to be paid</b>";
                                //    str_pdf_html += "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3_DISCOUNT + "</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}


                                //R23.08 Billing Consolidate Change TOTAL_3 to BILL_TOTAL
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_TOTAL != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                    str_pdf_html += "<b>Total estimated to be paid</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILL_TOTAL + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='6' allign='left'>All service charge and installation fee are excluding VAT 7%</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='1' allign='left'></td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='1'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";


                                //START Officer
                                if (query.isShop == "N")
                                {
                                    //INSTALL_ADDRESS
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>Installation Address</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //CustomerRegisterPanelModel.AddressPanelModelSendDocIDCard.L_MOO

                                    //HOME_NO ,MOO ,VILLAGE_NAME
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Home No.</td><td style='width: 12.5%; vertical-align: top;' colspan='1'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_HOME_NUMBER_2 + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='1'>Moo " + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOO + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Village Name</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_MOOBAN + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //BUILDING_NAME ,ROOM
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Condominium/Office Building Name</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_BUILD_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Room</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROOM + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //SOI ,STREET
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Soi</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_SOI + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Road</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ROAD + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //SUB_DISTRICT ,DISTRICT
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Sub-District</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_TUMBOL + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>District</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_AMPHUR + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //PROVINCE ,POSTCODE
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;' colspan='3'>Province</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_PROVINCE + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Zip Code</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSetup.L_ZIPCODE + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //BILLING_ADDRESS
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>Billing Address</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //HOME_NO ,MOO ,VILLAGE_NAME
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Home No.</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='1'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_HOME_NUMBER_2 + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='1'>Moo " + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOO + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Village Name</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_MOOBAN + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //BUILDING_NAME ,ROOM
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Condominium/Office Building Name</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_BUILD_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Room</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROOM + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //SOI ,STREET
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Soi</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_SOI + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Road</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ROAD + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //SUB_DISTRICT ,DISTRICT
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Sub-District</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_TUMBOL + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>District</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_AMPHUR + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                    //PROVINCE ,POSTCODE
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;' colspan='3'>Province</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_PROVINCE + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Zip Code</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.AddressPanelModelSendDoc.L_ZIPCODE + "</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";

                                }
                                //END Officer

                                str_pdf_html += "</tbody>";
                                str_pdf_html += "</table>";
                            }

                        }
                        //Existing
                        else
                        {
                            //Existing HTML Thai
                            if (Language == "T")
                            {
                                //START_HTML
                                str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryTable'>";
                                str_pdf_html += "<tbody>";

                                //HEADER
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='8'  align='right'><img src='data:image/png;base64," + picturePDF.AddServiceTH + "' border=0 height=161 width=700 /></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='2'  align='left'><img src='data:image/png;base64," + picturePDF.Img_logo1 + "' border=0 height=138 width=198 /></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //SUMMARY
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top;' colspan='10'>";
                                str_pdf_html += "<b>สรุปข้อมูล</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //PERSONAL_INFO
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>ข้อมูลผู้สมัคร</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INDIVIDUAL_INFO
                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>บุคคลธรรมดา</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_TITLE + " " + model.CustomerRegisterPanelModel.L_FIRST_NAME + " " + model.CustomerRegisterPanelModel.L_LAST_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>หน่วยงานราชการ/รัฐวิสาหกิจ</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ชื่อหน่วยงานราชการ/ชื่อบริษัท</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CARD_TYPE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ประเภทบัตร</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_TYPE + "</td>";
                                //CARD_NO
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>เลขที่บัตร</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_NO + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    //GENDER
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>เพศ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GENDER + "</td>";
                                    //BIRTHDAY
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>วันเกิด</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_BIRTHDAY + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType != "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>ชื่อ-นามสกุล ผู้เป็นตัวแทนติดต่อ</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CONTACT_PERSON + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTACT_MOBILE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>หมายเลขที่ใช้ในการติดต่อ</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_MOBILE + "</td>";
                                //EMAIL
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Email</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_EMAIL + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CONTACT_TIME
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='3'>เวลาที่สะดวกให้ติดต่อกลับ</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_SPECIFIC_TIME + "</td>";
                                //INSTALL_DATE
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>วันที่ให้เข้าติดตั้ง</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_INSTALL_DATE + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //YOUR_PACKAGE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>แพ็กเกจที่เลือก</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //ONTOP_FIXEDLINE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_FIXEDLINE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>แพ็กเกจเสริม :</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>โทรศัพท์พื้นฐาน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ONTOP_AIS_PLAYBOX
                                //if (aisPLAYBOX != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>แพ็กเกจเสริม :</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>โปรโมชั่นพิเศษ<br/>";
                                //    str_pdf_html += "ฟรี! ส่วนลดค่าเปิดใช้บริการ AIS PLAYBOX มูลค่า 2,500 บาท";
                                //    str_pdf_html += "</td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //ONTOP_HOOQ
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>ฟรี! ค่าบริการ HOOQ " + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ + " เดือน</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //HORIZON_LINE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //EQUIPMENT_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].EQUIPMENT_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>ค่าติดตั้ง</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //EQUIPMENT_FEE_MULTI_PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE_FLAG != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                    str_pdf_html += "<b>ค่าติดตั้ง (***ชำระให้แก่ช่างโดยตรง ณ วันติดตั้ง)</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //INSTALL_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าติดตั้งโทรศัพท์พื้นฐานพร้อมอุปกรณ์</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //DISCOUNT_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ส่วนลดค่าติดตั้งโทรศัพท์พื้นฐานพร้อมอุปกรณ์</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //DISCOUNT_ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_NAME_RECURRING_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_NAME_RECURRUNG_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2
                                //if (ContentPlaybox != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าติดตั้ง AIS PLAYBOX จุดที่ 2</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>850.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //PLAYBOX_FEE_SPOT_3
                                //if (contentPlayboxSale != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าติดตั้ง AIS PLAYBOX จุดที่ 3</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>850.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //TOTAL_1
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MONTHLY_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                str_pdf_html += "<b>ค่าบริการรายเดือน</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //MA_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าบำรุงรักษาคู่สายโทรศัพท์บ้าน</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_1_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_1_INSTALL_CONTENT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_CONTENT_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_2_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_MONTHLY_FEE_SPOT_1
                                //if (ContentPlaybox_NF != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าบริการรายเดือน AIS PLAYBOX จุดที่ 1</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>100.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //PLAYBOX_MONTHLY_FEE_SPOT_2
                                //if (playboxMonthFeeSpot2 != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าบริการรายเดือน AIS PLAYBOX จุดที่ 2</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>100.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //PLAYBOX_MONTHLY_FEE_SPOT_3
                                //if (packageName != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>ค่าบริการรายเดือน AIS PLAYBOX จุดที่ 3</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>100.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //TOTAL_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>รวมรายการที่ต้องชำระ</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PRO_RATE_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                str_pdf_html += "<b>ค่าบริการเฉลี่ยรายวัน (เรียกเก็บในบิลแรกเท่านั้น)</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CAL_PER_DAY
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>คิดเฉลี่ยต่อ 1 วัน</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //TOTAL_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                    str_pdf_html += "<b>รวมยอดโดยประมาณที่ต้องชำระ</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>บาท</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //EXCLUDE_VAT
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='6' allign='left'>ราคานี้ยังไม่รวมภาษีมูลค่าเพิ่ม 7%</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='1' allign='left'></td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='1'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INSTALL_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>ข้อมูลสถานที่ติดตั้ง</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //BILLING_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>ข้อมูลทีอยู่ในการจัดส่งเอกสาร</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td></tr><tr><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //END_HTML
                                str_pdf_html += "</tbody>";
                                str_pdf_html += "</table>";
                            }
                            //Existing HTML English
                            else
                            {
                                //START_HTML
                                str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryTable'>";
                                str_pdf_html += "<tbody>";

                                //HEADER
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='8'  align='right'><img src='data:image/png;base64," + picturePDF.AddServiceEN + "' border=0 height=161 width=700 /></td>";
                                str_pdf_html += "<td style='width: 25%;' colspan='2'  align='left'><img src='data:image/png;base64," + picturePDF.Img_logo1 + "' border=0 height=138 width=198 /></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //SUMMARY
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top;' colspan='10'>";
                                str_pdf_html += "<b>Summary</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //PERSONAL_INFO
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>Your Information</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INDIVIDUAL_INFO
                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>Residential</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Name</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_TITLE + " " + model.CustomerRegisterPanelModel.L_FIRST_NAME + " " + model.CustomerRegisterPanelModel.L_LAST_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>Government/State Enterprise</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Name of Gevernment/Business</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CARD_TYPE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Card Type</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_TYPE + "</td>";
                                //CARD_NO
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>ID card no</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_CARD_NO + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                if (model.CustomerRegisterPanelModel.CateType == "R")
                                {
                                    //GENDER
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Gender</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_GENDER + "</td>";
                                    //BIRTHDAY
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Birthday</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_BIRTHDAY + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                if (model.CustomerRegisterPanelModel.CateType != "R")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Contact Person</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'> " + model.CustomerRegisterPanelModel.L_CONTACT_PERSON + "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //CONTACT_MOBILE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>Contact Number</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'>" + model.CustomerRegisterPanelModel.L_MOBILE + "</td>";
                                //EMAIL
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'>Email</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_EMAIL + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CONTACT_TIME
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Specific Contact Time</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_SPECIFIC_TIME + "</td>";
                                //INSTALL_DATE
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='3'>Req. install date</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'>" + model.CustomerRegisterPanelModel.L_INSTALL_DATE + "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //YOUR_PACKAGE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                str_pdf_html += "<b>Your Package</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //ONTOP_FIXEDLINE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_FIXEDLINE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>On top package :</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>Fixed Line</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ONTOP_AIS_PLAYBOX
                                //if (aisPLAYBOX != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'>On top package :</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top; ' colspan='5'>Special Promotion<br/>";
                                //    str_pdf_html += "Free! Discount for AIS PLAYBOX activation fee 2,500 Baht";
                                //    str_pdf_html += "</td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //ONTOP_HOOQ
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='5'>Free! HOOQ Package for " + check_gen_pdfonweb.LIST_PDF_CUR[0].ONTOP_HOOQ + " months</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //HORIZON_LINE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3' align='center'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-top: thin solid; border-color: #A4A4A4;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //EQUIPMENT_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].EQUIPMENT_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                    str_pdf_html += "<b>Installation fee</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //EQUIPMENT_FEE_MULTI_PLAYBOX
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].WIFI_ROUTER_FEE_FLAG != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                    str_pdf_html += "<b>Installation fee (***pay to installer on installation date)</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //INSTALL_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Fixed Line Installation fee & equipment</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //DISCOUNT_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td><td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Discount Fixed Line Installation fee & equipment</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>3,500.00</td><td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }


                                //DISCOUNT_ACTIVATE_PLAYBOX_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].DISCOUNT_ACTIVATE_PLAYBOX_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].ACTIVATE_PLAYBOX_FEE_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_NAME_RECURRING_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_2_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_2_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_NAME_RECURRUNG_CHARGE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_DISCOUNT_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_DISCOUNT_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].PLAYBOX_FEE_SPOT_3_DISCOUNT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_FEE_SPOT_2
                                //if (ContentPlaybox != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>AIS PLAYBOX monthly fee for 2nd spot</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>850.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //PLAYBOX_FEE_SPOT_3
                                //if (contentPlayboxSale != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>AIS PLAYBOX monthly fee for 3rd spot</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>850.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //TOTAL_1
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_1 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }
                                else
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>0.00</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //MONTHLY_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='3'>";
                                str_pdf_html += "<b>Monthly service fee</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //MA_FIXEDLINE_FEE
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Fixed Line maintenance fee</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].MA_FIXEDLINE_FEE + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_1_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != "false" && check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_1_INSTALL_CONTENT
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_INSTALL_CONTENT + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_1_CONTENT_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_2_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_2_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //SPOT_3_INSTALL_NAME
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME != null)
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_NAME + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='right'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].SPOT_3_INSTALL_RECURRING + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PLAYBOX_MONTHLY_FEE_SPOT_1
                                //if (ContentPlaybox_NF != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>AIS PLAYBOX monthly fee for 1st spot</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>100.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //PLAYBOX_MONTHLY_FEE_SPOT_2
                                //if (playboxMonthFeeSpot2 != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td><td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>AIS PLAYBOX monthly fee for 2nd spot</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>100.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //PLAYBOX_MONTHLY_FEE_SPOT_3
                                //if (packageName != "false")
                                //{
                                //    str_pdf_html += "<tr>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>AIS PLAYBOX monthly fee for 3rd spot</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>100.00</td>";
                                //    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                //    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                //    str_pdf_html += "</tr>";
                                //}

                                //TOTAL_2
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Total</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_2 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //PRO_RATE_FEE
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='5'>";
                                str_pdf_html += "<b>Average daily (charged on the first bill only)</b>";
                                str_pdf_html += "</td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top;' colspan='3'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //CAL_PER_DAY
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>Calculated per one day</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].CAL_PER_DAY + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //TOTAL_3
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='6' allign='left'>";
                                    str_pdf_html += "<b>Total estimated to be paid</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top;' colspan='1' allign='left'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].TOTAL_3 + "</td>";
                                    str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-right: thin solid; border-color: #088A08;' colspan='1'>Baht</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //EXCLUDE_VAT
                                str_pdf_html += "<tr>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 12.5%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='2'></td>";
                                str_pdf_html += "<td style='width: 25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='6' allign='left'>All service charge and installation fee are excluding VAT 7%</td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08;' colspan='1' allign='left'></td>";
                                str_pdf_html += "<td style='width: 6.25%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='1'></td><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                str_pdf_html += "</tr>";

                                //INSTALL_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>Installation Address</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].INSTALL_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //BILLING_ADDRESS
                                if (check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS != "false")
                                {
                                    str_pdf_html += "<tr>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>";
                                    str_pdf_html += "<b>Billing Address</b>";
                                    str_pdf_html += "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td></tr><tr><td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "<td style='width: 50%; vertical-align: top; border-bottom: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;'  colspan='10'>" + check_gen_pdfonweb.LIST_PDF_CUR[0].BILLING_ADDRESS + "</td>";
                                    str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                                    str_pdf_html += "</tr>";
                                }

                                //END_HTML
                                str_pdf_html += "</tbody>";
                                str_pdf_html += "</table>";
                            }

                        }
                    }
                    else
                    {
                        throw new Exception("Can not call stored procedure PdfOnWebQuery");
                    }

                }
                // page 2-3
                else
                {

                    if (Language.ToString() == "T")
                    {
                        str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryP2Table'>";
                        str_pdf_html += "<tbody>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 25%;' colspan='8' align='right'>";
                        str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Thai_5 + "' border=0 height=161 width=700 />";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%;' colspan='2' align='left'>";
                        str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Img_logo1 + "' border=0 height=138 width=198 />";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>เงื่อนไขในการให้บริการ</b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;' colspan='10'>AWN: บริษัท แอดวานซ์ ไวร์เลส เน็ทเวอร์ค จำกัด<br>&nbsp;&nbsp;&nbsp;บริการตามสัญญานี้: บริการอินเทอร์เน็ตบรอดแบนด์ (AIS Fibre), บริการโทรศัพท์เคลื่อนที่ (Moblie), บริการโทรศัพท์ประจำที่ (AIS Fixed Line) ภายใต้ใบอนุญาตของ AWN และ/หรือบริการโทรทัศน์แบบบอกรับสมาชิก (AIS PLAYBOX) ภายใต้ใบอนุญาตของบริษัท ซุปเปอร์ บรอดแบนด์ เน็ทเวอร์ค จำกัด (SBN) ซึ่งมีข้อตกลงการให้บริการกับ AWN และบริการเสริมอื่นๆ ที่ผู้ใช้บริการสมัครใช้บริการจาก AWN ตามสัญญานี้ หรือบริการอื่นๆ ที่ผู้ใช้บริการตกลงทำสัญญาใช้บริการกับ AWN ในอนาคต<br>&nbsp;&nbsp;&nbsp;บริการ AIS Convergence: บริการอินเทอร์เน็ตบรอดแบนด์ (AIS Fibre), บริการโทรศัพท์เคลื่อนที่ (Moblie), บริการโทรศัพท์ประจำที่ (AIS Fixed Line) ภายใต้ใบอนุญาตของ AWN และ/หรือบริการโทรทัศน์แบบบอกรับสมาชิก (AIS PLAYBOX) ภายใต้ใบอนุญาตของ SBN ซึ่งมีข้อตกลงให้บริการกับ AWN และบริการเสริมอื่นๆ ที่ผู้ใช้บริการสมัครใช้บริการจาก AWN ตามสัญญานี้ หรือบริการอื่นๆ ที่ผู้ใช้บริการตกลงทำสัญญาใช้บริการกับ AWN ในอนาคต ซึ่งให้บริการรวมกันในรูปแบบแพ็กเกจ</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>1. <b>มาตรฐานการให้บริการ:</b></b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;1.1 AWN จะให้บริการตามมาตรฐานและคุณภาพการให้บริการตามที่ได้โฆษณาไว้ หรือแจ้งให้ผู้ใช้บริการทราบ โดยมาตรฐานและคุณภาพการให้บริการดังกล่าวจะไม่ต่ำกว่าหลักเกณฑ์ที่หน่วยงาน กำกับดูแลกำหนด </td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;1.2 กรณี เกิดเหตุขัดข้องกับการให้บริการอันเนื่องมาจากมาตรฐานหรือคุณภาพการให้ บริการจนเป็นเหตุให้ผู้ใช้บริการไม่สามารถใช้บริการได้ตามปกติ AWN มีหน้าที่ดำเนินการแก้ไขเพื่อให้ผู้ใช้บริการสามารถใช้บริการได้ และ AWN จะไม่เรียกเก็บเงินค่าบริการในช่วงเวลาดังกล่าวจากผู้ใช้บริการ เว้นแต่พิสูจน์ได้ว่าเหตุขัดข้องดังกล่าวเกิดขึ้นจากผู้ใช้บริการ</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;1.3 เพื่อป้องกันและหลีกเลี่ยงผลกระทบต่อการใช้งานของผู้ใช้บริการรายอื่น AWN ขอสงวนสิทธิ์ในการกำหนดเงื่อนไขการให้บริการเพื่อการให้บริการอย่างเท่า เทียม โดยคำนึงถึงประโยชน์ของผู้ใช้บริการส่วนรวมเป็นสำคัญ ทั้งนี้ บริการ AIS Fibre เป็นบริการแบบใช้ความเร็วรับ-ส่งข้อมูลร่วมกัน คุณภาพและความเร็วในการใช้งานขึ้นอยู่กับปริมาณหรือจำนวนผู้ใช้งานในขณะนั้น โดยระยะทางระหว่างโครงข่ายอินเทอร์เน็ตกับสถานที่ของผู้ใช้บริการ คุณภาพและประสิทธิภาพของอุปกรณ์ที่ใช้เชื่อมต่อ รวมถึงเซิร์ฟเวอร์ของเว็บไซต์ที่เข้าใช้งาน อาจส่งผลให้ความเร็วเชื่อมต่อต่ำกว่าที่กำหนดไว้ในแพ็กเกจที่ผู้ใช้บริการ สมัครใช้บริการ</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>2. <b>หน้าที่และความรับผิดชอบของผู้ให้บริการและผู้ใช้บริการ</b></b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.1 ผู้ใช้บริการจะโอนสิทธิการใช้บริการตามสัญญาให้แก่บุคคลอื่นมิได้ เว้นแต่ได้รับความยินยอมจาก AWN ทั้งนี้ ผู้ใช้บริการไม่สามารถยกเหตุการโอนสิทธิมาปฏิเสธความรับผิดชอบชำระค่า ธรรมเนียมหรือค่าบริการที่เกิดขึ้นได้ ในกรณีที่ผู้ใช้บริการโอนสิทธิการใช้บริการให้กับบุคคลอื่นแล้ว ถือว่าสิทธิการใช้บริการตามรายการส่งเสริมการขายนั้นเป็นอันสิ้นสุดลงทันที ในกรณีนี้ผู้รับโอนต้องดำเนินการสมัคร หรือยื่นคำขอใช้บริการใหม่กับ AWN ต่อไป</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.2 ผู้ใช้บริการยินยอมให้ AWN นำข้อมูลส่วนบุคคลของผู้ใช้บริการไปประมวลผลเพื่อประโยชน์ในการดำเนินกิจการ โทรคมนาคมและโทรทัศน์แบบบอกรับสมาชิกตามหลักเกณฑ์ที่หน่วยงานกำกับดูแลกำหนด เกี่ยวกับมาตรการคุ้มครองสิทธิของผู้ใช้บริการโทรคมนาคมเกี่ยวกับข้อมูลส่วน บุคคล</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.3 สำหรับ แพ็กเกจที่ AWN เป็นผู้จัดหาและ/หรือติดตั้งอุปกรณ์การให้บริการ AIS Fibre และ/หรือ AIS PLAYBOX ให้แก่ผู้ใช้บริการ ให้ถือว่าอุปกรณ์หรือสิ่งอื่นใดอันเกี่ยวข้องกับการให้บริการดังกล่าวเป็น กรรมสิทธิ์ของ AWN และ/หรือ SBN ผู้ใช้บริการมีหน้าที่ต้องใช้ความระมัดระวังในการดูแลรักษาอุปกรณ์เช่น วิญญูชนพึงกระทำและไม่นำอุปกรณ์ไปใช้ผิดวัตถุประสงค์หรือหน้าที่ของอุปกรณ์ นั้น ๆ และเมื่อการใช้บริการสิ้นสุดลงไม่ว่าด้วยเหตุใด ผู้ใช้บริการจะต้องส่งมอบอุปกรณ์การให้บริการในสภาพสมบูรณ์และสามารถใช้งาน ได้ตามปกติคืนให้แก่ AWN ภายใน 15 วันนับแต่วันสิ้นสุดการใช้บริการ หรือยินยอมให้ AWN หรือตัวแทนดำเนินการเก็บอุปกรณ์คืน กรณีผู้ใช้บริการไม่คืนอุปกรณ์ หรือไม่ยินยอมให้ AWN หรือตัวแทนเก็บอุปกรณ์คืน ไม่ว่าทั้งหมดหรือบางส่วน AWN มีสิทธิเรียกค่าอุปกรณ์จากผู้ใช้บริการ ตามอัตราและวิธีการที่ AWN กำหนด</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.4 ผู้ใช้บริการยินยอมและอำนวยความสะดวกให้แก่ AWN ลูกจ้าง และ/หรือผู้ที่ AWN มอบหมายในการเข้าไปในสถานที่ติดตั้ง เพื่อทำการซ่อมแซม ตรวจสอบ บำรุงรักษาอุปกรณ์การให้บริการ หรือเก็บคืนอุปกรณ์ตามข้อ 2.3 ได้ตลอดเวลา โดย AWN จะแจ้งให้ผู้ใช้บริการทราบล่วงหน้าก่อนการเข้าไปในสถานที่ติดตั้งดังกล่าว</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.5 ผู้ใช้บริการตกลงว่าจะไม่ทำการเคลื่อนย้าย หรือเปลี่ยนแปลงสถานที่ติดตั้งอุปกรณ์การให้บริการโดย ไม่ได้รับอนุญาตเป็นหนังสือจาก AWN ก่อน หากผู้ใช้บริการกระทำการฝ่าฝืนข้อตกลงตามข้อนี้ ผู้ใช้บริการยินยอมชำระค่าใช้จ่ายหรือค่าดำเนินการจากการฝ่าฝืนดังกล่าวแก่ AWN</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.6 กรณี ผู้ใช้บริการมีข้อเดือดร้อนเสียหายอันเนื่องมาจากการให้บริการของ AWN ผู้ใช้บริการมีสิทธิยื่นข้อร้องเรียนได้ตามหลักเกณฑ์การรับเรื่องร้องเรียน และการแก้ไขปัญหาเรื่องร้องเรียนของผู้ใช้บริการที่ AWN ได้กำหนดและประกาศแจ้งให้ทราบแล้ว</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.7 AWN สงวนสิทธิในการเปลี่ยนแปลง แก้ไข เพิ่มเติมสัญญาให้บริการนี้ รวมถึงการแก้ไขเปลี่ยนแปลงอัตราค่าบริการตามที่ได้รับความเห็นชอบจากหน่วย งานกำกับดูแล</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>3. <b>ค่าบริการและค่าธรรมเนียมต่างๆ</b></b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.1 ผู้ใช้บริการตกลงชำระค่าบริการทั้งหมดให้แก่ AWN ตามอัตราค่าบริการและวิธีการที่ AWN กำหนดภายในระยะเวลาที่ถึงกำหนดชำระตามที่ระบุ ไว้ในใบแจ้งรายการใช้บริการ โดย AWN จะส่งใบแจ้งรายการใช้บริการในแต่ละรอบให้แก่ผู้ใช้บริการทราบเป็นการล่วง หน้าไม่น้อยกว่า 15 วันก่อนวันครบกำหนดชำระ หากผู้ใช้บริการผิดนัดชำระค่าบริการ ผู้ใช้บริการต้องชำระดอกเบี้ยในอัตราร้อยละ 15 ต่อปีให้แก่ AWN นับแต่วันที่ผู้ใช้บริการผิดนัดจนกว่าจะชำระเสร็จสิ้น ทั้งนี้ AWN รับรองว่า จะไม่เรียกเก็บค่าธรรมเนียม หรือค่าบริการนอกเหนือจากที่กำหนดไว้ในสัญญานี้</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.2 ผู้ใช้บริการมีสิทธิขอเปลี่ยนแปลงรายการส่งเสริมการขายและการใช้บริการได้ตาม รายการส่งเสริมการขายที่ AWN เปิดให้สมัครใช้ได้อยู่ ณ เวลานั้นและตามเงื่อนไขที่ AWN กำหนด กรณีที่ผู้ใช้บริการโอนสิทธิใช้บริการตามข้อ 2.1 หรือโอนเลขหมายไปให้ผู้อื่นไม่ว่าด้วยเหตุใดๆ หรือถูกผู้ให้บริการบอกเลิก การให้บริการสิทธิประโยชน์ของรายการส่งเสริมการขายการใช้บริการนั้นเป็นอัน สิ้นสุดลงทันที ทั้งนี้ หากผู้ใช้บริการยกเลิกหรือสิ้นสุดการใช้บริการตามแพ็กเกจสำหรับบริการใด บริการหนึ่งหรือหลายบริการรวมกัน ให้ถือว่าการใช้บริการตามแพ็กเกจเป็นอันสิ้นสุดลงทันที</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.3 บรรดาหนังสือ จดหมาย คำบอกกล่าวใดๆ ของ AWN ที่ส่งไปยังผู้ใช้บริการ ณ สถานที่ส่งใบแจ้งหนี้ค่าใช้บริการ ให้ถือว่าผู้ใช้บริการได้รับโดยชอบแล้ว และผู้ใช้บริการยินยอมให้ AWN แจ้งเตือนหนี้ค่าบริการที่ค้างชำระทางระบบโทรศัพท์อัตโนมัติ, ไปรษณีย์, SMS, E-mail และช่องทางอื่นๆ เพื่อประโยชน์ของผู้ใช้บริการในกรณีที่มีค่าบริการที่ค้างชำระ ทั้งนี้ เท่าที่ไม่ขัดกับกฎหมายที่เกี่ยวข้องกำหนด</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.4 กรณีที่ผู้ใช้บริการเห็นว่า AWN เรียกเก็บค่าบริการสูงกว่าอัตราที่กำหนดไว้หรือสูงกว่าที่เรียกเก็บจากผู้ ใช้บริการรายอื่นที่ใช้บริการโทรคมนาคมหรือโทรทัศน์แบบบอกรับสมาชิกในลักษณะ หรือประเภทเดียวกัน หรือเห็นว่า AWN เรียกเก็บค่าบริการไม่ถูกต้อง ผู้ใช้บริการมีสิทธิยื่นคำขอตรวจสอบข้อมูลการใช้บริการได้ โดยยื่นคำขอเป็นหนังสือ หรือทางโทรศัพท์ หรือเครื่องมือสื่อสารอย่างอื่น หรือโดยวิธีการอื่นใดมายังสำนักงานบริการ สำนักงานสาขา หรือตัวแทนศูนย์บริการ หรือศูนย์บริการรับข้อร้องเรียน และ AWN จะดำเนินการตรวจสอบข้อเท็จจริงและแจ้งผลการตรวจสอบข้อมูลให้ผู้ใช้บริการ ทราบโดยเร็ว แต่ทั้งนี้ต้องไม่เกินกว่า 30 วันนับแต่วันที่ผู้ใช้บริการมีคำขอ หาก AWN ไม่ดำเนินการภายในระยะเวลาที่กำหนด ให้ถือว่า AWN สิ้นสิทธิในการเรียกเก็บค่าธรรมเนียมหรือค่าบริการในจำนวนที่ผู้ใช้บริการ ได้โต้แย้งนั้น</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.5 กรณีข้อเท็จจริงปรากฏว่า AWN เรียกเก็บค่าบริการเกินกว่าจำนวนที่เกิดขึ้นจากการใช้บริการจริง AWN จะคืนเงินส่วนต่างของค่าบริการให้แก่ผู้ใช้บริการภายใน 30 วันนับแต่วันที่ข้อเท็จจริงยุติ และ AWN จะชำระดอกเบี้ยในส่วนต่างในอัตราเท่ากับที่ได้กำหนดว่าจะเรียกเก็บจากผู้ใช้ บริการในกรณีที่ผู้ใช้บริการผิดนัด โดย AWN จะจ่ายคืนเงินให้ตามวิธีการที่ผู้ใช้บริการแจ้งความประสงค์ไว้</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.6 AWN และ/หรือ SBN สงวนสิทธิ์การให้บริการ AIS PLAYBOX เฉพาะภายในที่พักอาศัยหรือสถานที่ติดตั้งบริการของผู้ใช้บริการเท่านั้น หากมีการใช้บริการหรือเผยแพร่ส่วนใดส่วนหนึ่งของบริการไปในที่สาธารณะ ผู้ใช้บริการจะต้องรับผิดชอบต่อความเสียหายอันเกิดจากการเผยแพร่ดังกล่าวเอง ทั้งสิ้น</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.7 AWN และ/หรือ SBN สงวนสิทธิ์ในการหยุดหรือระงับการให้บริการ Free TV บน AIS PLAYBOX ไม่ว่าทั้งหมดหรือแต่บางส่วน</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>4.</b> <b>บริการเสริม</b>";
                        str_pdf_html += "<br />หากผู้ใช้บริการประสงค์จะขอใช้บริการเสริม ผู้ใช้บริการตกลงจะสมัครใช้บริการเสริมตามรูปแบบและวิธีการที่ AWN กำหนด โดยให้ถือว่าใบสมัครหรือคำขอใช้บริการเสริมเป็นส่วนหนึ่งของการให้บริการนี้ และหากมีการยกเลิกหรือระงับการใช้บริการไม่ว่าด้วยเหตุใด ผู้ใช้บริการยอมรับว่าการให้บริการเสริมจะถูกยกเลิกหรือระงับไปโดยอัตโนมัติ<br />";
                        str_pdf_html += "<br />ผู้ใช้บริการสามารถดูรายละเอียดเพิ่มเติมที่เกี่ยวกับการใช้บริการ รายละเอียดช่องรายการ และอุปกรณ์การให้บริการ";
                        str_pdf_html += "<br />ตามสัญญานี้ ผ่านเว็บไซต์ www.ais.co.th/fibre และ www.ais.co.th/playbox และแผ่นพับของบริการ";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "</tbody>";
                        str_pdf_html += "</table>";
                    }
                    else
                    {
                        str_pdf_html += "<table style='width: 100%; font-size: 6px; border-collapse: collapse;' id='SummaryP2Table'>";
                        str_pdf_html += "<tbody>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 25%;' colspan='8' align='right'>";
                        str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Thai_5 + "' border=0 height=161 width=700 />";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%;' colspan='2' align='left'>";
                        str_pdf_html += "<img src='data:image/png;base64," + picturePDF.Img_logo1 + "' border=0 height=138 width=198 />";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-top: thin solid; border-color: #088A08; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>Terms and Conditions</b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08;  border-right: thin solid; border-color: #088A08;' colspan='10'>AWN: Advanced Wireless Network Company Limited<br>&nbsp;&nbsp;&nbsp;Fixed Broadband Service (AIS Fibre), Mobile Phone Service (Mobile), Fixed Line Service (AIS Fixed Line) under license of AWN and/or Subscription Broadcasting Service (AIS PLAYBOX) under license of Super Broadband Network Company Limited (SBN) which has service providing agreement with AWN, and Value Added Service which the Subscriber applied with AWN relating to this Terms and Conditions or any types of service which the Subscriber applied or shall apply with AWN in the future.<br>&nbsp;&nbsp;&nbsp;AIS Convergence Services: Fixed Broadband Service (AIS Fibre), Mobile Phone Service (Mobile), Fixed Line Service (AIS Fixed Line) under license of AWN and/or Subscription Broadcasting Service (AIS PLAYBOX) under license of Super Broadband Network Company Limited (SBN) which has service providing agreement with AWN, and Value Added Service which the Subscriber applied with AWN relating to this Terms and Conditions or any types of service which the Subscriber applied or shall apply with AWN in the future which altogether provided in package.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>1. <b>The standard of the Services:</b></b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;1.1 AWN agrees to provide the Services with the standard and quality not lower than the criteria designated by the regulator for such kind of service under the announcement or the notification to the Subscriber. </td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;1.2 In the event that there is the interruption of the Services due to the standard and quality of such service which prevents the Subscriber from using the Services, AWN shall be obliged to immediately remedy in order to enable Subscriber to use the Services and AWN shall not collect the service charge from the Subscriber during such period unless AWN can prove that such interruption caused by the Subscriber.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;1.3 For the prevention and avoidance of effect on other subscriber using of the Services, AWN reserves the right to specify the conditions prioritizes the collective interest of general subscribers. Furthermore, AIS Fibre is a co-speed of data usage service for both the receiving and sending of data. Therefore the quality and speed of internet depend on quantity, amount of subscribers, the distance between the internet network and subscribers’ premises, quality and efficiency of connecting devices including the server of website which the subscribers visit and may affect the speed of internet to be lower than specified for the package selected by the Subscriber.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>2. <b>The right and obligations of AWN and Subscriber</b></b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.1 The Subscriber shall not assign or transfer the right of using the Services under this Terms and Conditions to any person without the consent of AWN. In order that the Subscriber shall not raise the reason for such assignment as an excuse for refusing to make payment of any fees or service charge. However, if AWN agrees to consent of such assignment by the Subscriber, it shall be deemed that the right of using former Services under promotional campaign of Subscriber shall be immediately terminated. In this regard, the assignee shall apply for the Services and submit the new application form with AWN.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.2 The Subscriber hereby agree and accept to collect, use and disclose personal data in which AWN shall collect, store, use of Subscriber’s personal data for the purpose of service in which the Subscriber has subscribed and in accordance with the law.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.3 For the package which AWN provide and install the equipment to the Subscriber for using the Services, such equipment and other relevant accessories shall be deemed own by AWN. The Subscriber shall be obliged to take care and keep the equipment with such care as a person of ordinary prudence would take of its own property and shall not misuse or misappropriate for the purpose or usability of such equipment. Consequently, upon termination of service for any reason, the Subscriber agrees to return the equipment in complete and good working condition within 15 days from the termination date of the service or allows AWN or its agent or representative to collect the equipment. In case the Subscriber will not, in the whole or part, return or allow AWN or its agent or representative to collect the equipment, AWN has the right to claim the damages of the equipment according to the rate and process specified by AWN which shall not be higher than the criteria designated by the regulator. Furthermore, for the package which Subscriber provide and/or install the equipment such as modem or router by itself, the Subscriber shall take all responsibility for any damage or claim arising from such equipment providing and/or installation.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.4 The Subscriber agrees to facilitate AWN’s agent or its representative to access into the premise to repair, inspect and maintain or retrieve the equipment of the Services as specified at section 2.3 at all-time with the prior notification provided by AWN.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.5 The Subscriber agrees that it shall make no movement or modification to the equipment settled place without prior written consent of AWN. If the Subscriber violates this section, the Subscriber shall indemnify AWN for all damages.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.6 In case that the Subscriber has the trouble and damage due to the provision of the Services from AWN, the Subscriber has the right to submit the complaint according to the principle for the receiving of the complaint and solving the problem for the complaint of the Services specified and announced by AWN.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;2.7 AWN reserves the right to change, amend, add this Term and Conditions under the approval of the regulator or as announced and specified by the regulator.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>3. <b>Service charges and fees</b></b>";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.1 The Subscriber shall pay service charge and/or fees according to the rate and process specified by AWN within the period stated in the invoice. AWN shall deliver the invoice in advance at least 15 days prior to the due date. If the Subscriber fails to make payment of service charge, the Subscriber shall pay the interest to AWN at the rate of 15% per annum from the defaulting date until the full amount shall be received by AWN. AWN represents that it does not collect the fees or service charge other than specified under this Terms and Conditions.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.2 The Subscriber has the right to change the promotional campaign and the use of Services under the promotional campaign provided by AWN at that moment. If the Subscriber transfers the right to use the Services to the other person for whatever reason or AWN cancels the benefits of the promotional campaign, the use of such service shall be terminated forthwith.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.3 Any AWN’s letters or notices which delivered to the Subscriber’s billing address shall be deemed duly received by the Subscriber. The Subscriber allows AWN to notice the outstanding Services fee via automatic telephone system, postal, SMS, E-mail and other channels as it is not contrary to relevant law.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.4 In case the Subscriber found that AWN collects service charge exceeding than the specified rate or exceeding than the amount which collects from other subscribers who use the same kind and type of Services or found that AWN collects service charge incorrectly, the Subscriber has the right to submit the request for verification of the usage details by submitting the request in writing or by telephone or other communication devices or by other means to AWN’ service office, branch office or service center representative or complaints handling center. AWN shall immediately verify the facts and notify the result of such verification to the Subscriber which not more than 30 days from the request date. If AWN does not perform upon request of the Subscriber during specified period, it shall be deemed that AWN deprives of the right to collect fees or service charge for such disputed amount.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.5 In case the fact that AWN has collected the service charge more than the actual usage from the Subscriber, AWN shall refund such excess amount within 30 days from the fact end date and AWN shall pay the interest of such excess amount at the rate which AWN determines that it shall collect from the Subscriber in case that the Subscriber fails to make payment of service charge. AWN may refund by means as requested by the Subscriber.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.6 The Subscriber has the right to use AIS PLAYBOX only within the residence or installation premise of itself. If there is any use or broadcasting of AIS PLAYBOX in the public, the Subscriber shall be responsible for all damages occurred from such use or broadcasting.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-right: thin solid; border-left: thin solid; border-color: #088A08;' colspan='10'>&nbsp;&nbsp;&nbsp;3.7 AWN and/or SBN reserve the right, in the whole or part, to cease or restrain Free TV service on AIS PLAYBOX.</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "<tr>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "<td style='width: 50%; vertical-align: top; border-left: thin solid; border-color: #088A08; border-right: thin solid; border-color: #088A08; border-bottom: thin solid; border-color: #088A08;' colspan='10'>";
                        str_pdf_html += "<b>4.</b> <b>Value Added Service</b>";
                        str_pdf_html += "<br />In case that the Subscriber requests for Value Added Service, the Subscriber shall apply for Value Added Service under the process and type specified by AWN and such Value Added Service application shall be deemed attached with this agreement.If there is the Services ceasing or termination for whatever reason, the Subscriber accepts that such Valued Added Service shall be canceled or terminated forthwith.< br /> ";
                        str_pdf_html += "<br />The Subscriber can see more details regarding using the Services as well as AIS PLAYBOX channels and the Services";
                        str_pdf_html += "<br />equipment via www.ais.co.th/fibre, www.ais.co.th/playbox and Services’ leaflet.";
                        str_pdf_html += "</td>";
                        str_pdf_html += "<td style='width: 25%; vertical-align: top;' colspan='2'></td>";
                        str_pdf_html += "</tr>";
                        str_pdf_html += "</tbody>";
                        str_pdf_html += "</table>";
                    }

                }


                pdfData.str_pdf_html = str_pdf_html;

                EndInterface<string>(pdfData.str_pdf_html, log, query.orderNo, "Success", "");

                return pdfData.str_pdf_html;

            }
            catch (System.Exception ex)
            {

                EndInterface<PdfOnWebQuery>(pdfOnWebQuery, log, query.orderNo,
                   ex.Message, ex.GetErrorMessage());

                return ex.Message.ToString();
            }
        }

        //Not Use
        public JsonResult QueryGeneratePDFApp(QuickWinPanelModel model)
        {
            PDFDataQuery query = new PDFDataQuery();

            if (Session["OrderNoAPP"] != null)
                query.orderNo = Session["OrderNoAPP"].ToSafeString();
            if (Session["LanguageAPP"] != null)
                query.Language = Session["LanguageAPP"].ToSafeString();
            query.isEApp = true;
            //var htmlFromPackage = QueryGeneratePDF(query);
            var htmlFromPackage = QueryGeneratePDFNew(query, model);
            //Session["OrderNoAPP"] = "";
            return Json(htmlFromPackage, JsonRequestBehavior.AllowGet);
        }

    }

    /// <summary>
    /// ImageThing for HTML with image to PDF
    /// </summary>
    public class ImageThing : IImageProvider
    {
        //Store a reference to the main document so that we can access the page size and margins
        private Document MainDoc = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc"></param>
        public ImageThing(Document doc)
        {
            this.MainDoc = doc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="attrs"></param>
        /// <param name="chain"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        Image IImageProvider.GetImage(string src, IDictionary<string, string> attrs, ChainedProperties chain, IDocListener doc)
        {
            //Prepend the src tag with our path. NOTE, when using HTMLWorker.IMG_PROVIDER, HTMLWorker.IMG_BASEURL gets ignored unless you choose to implement it on your own

            // Local image file
            //src = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + src;

            //Get the image. NOTE, this will attempt to download/copy the image, you'd really want to sanity check here
            Image img = null;
            if (src.IndexOf("data:image/png;base64,") < 0)
            {
                img = Image.GetInstance(src);
                //Make sure we got something
                if (img == null)
                    return null;
                img.ScalePercent(40);
            }
            else
            {
                Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(src.Replace("data:image/png;base64,", "")));
                img = Image.GetInstance(bitmapData);
                //Make sure we got something
                if (img == null)
                    return null;
                img.ScalePercent(40);
            }
            //If the downloaded image is bigger than either width and/or height then shrink it
            //if (img.Width > usableW || img.Height > usableH)
            //{
            //    img.ScaleToFit(usableW, usableH);
            //    img.ScaleAbsolute(img.Width, img.Height);
            //}

            //return our image
            return img;
        }

        public string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }
    }

}
