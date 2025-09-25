using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Linq;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public partial class ProcessController : WBBController
    {
        public string GeneratePDF_newVas2(QuickWinPanelModel model, string directoryPath,
           string directoryTempPath, string fileName)
        {
            try
            {
                #region setborder

                string EndAddressSetup = "29";
                string EndAddressVat = "";
                string EndAddressBill = "37";
                string EndDoc = "0";
                string EndCon = "5";
                string EndPack = "26";
                string EndInfo = "8";
                int startDoc = 40;
                int starInfo = 4;
                int startPack = 12;
                int startAddressSetup = 24;
                int startAddressBill = 32;
                int startAddressVat = 0;

                var displayType_ = model.CustomerRegisterPanelModel.CateType;
                if (displayType_ == "R")
                {
                    EndAddressSetup = "28";
                    EndAddressVat = "";
                    EndAddressBill = "32";
                    EndPack = "24";
                    EndInfo = "9";
                    startDoc = 51;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 27;
                    startAddressBill = 31;
                    startAddressVat = 0;
                }
                else  //// G/B
                {
                    EndAddressSetup = "28";
                    EndAddressVat = "36";
                    EndAddressBill = "32";
                    EndPack = "24";
                    EndInfo = "9";
                    startDoc = 66;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 27;
                    startAddressBill = 31;
                    startAddressVat = 35;
                }

                #endregion setborder

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
                var type_ = model.outcardType;
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

                        #endregion กำหนดช่องว่างระหว่าง group

                        #region set value to datatable

                        // Check group header
                        if (l.GroupByPDF.Substring(2, 1).Equals("H"))
                        {
                            string val = "";

                            #region condition

                            if (l.Name.Equals("L_CONDITION"))// add detail
                            {
                                continue;
                            }
                            //if (l.Name.Equals("L_CONDITION"))// add detail
                            //{
                            //    table = new System.Data.DataTable();
                            //    table.Columns.Add("ID", typeof(int));
                            //    table.Columns.Add("1", typeof(string));
                            //    table.Columns.Add("2", typeof(string));
                            //    table.Columns.Add("3", typeof(string));
                            //    table.Columns.Add("4", typeof(string));
                            //    table.Columns.Add("5", typeof(string));
                            //    table.Columns.Add("6", typeof(string));

                            //    //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                            //    //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                            //    currenttable = 2;

                            //    string ProSubType = "";
                            //    if (model.CoveragePanelModel.PRODUCT_SUBTYPE.ToString().Trim() == "SWiFi")
                            //    {
                            //        ProSubType = "TERM_AND_CONDITION_WIRELESS";
                            //    }
                            //    else
                            //    {
                            //        ProSubType = "TERM_AND_CONDITION_WIRE";
                            //    }
                            //    var data = base.LovData.Where(z => z.Type.Equals(ProSubType));
                            //    int num = 1;
                            //    if (SiteSession.CurrentUICulture.IsThaiCulture())
                            //    {
                            //        foreach (var x in data.ToList())
                            //        {
                            //            // table.Rows.Add(numberx, num.ToString() + ". " + x.LovValue1, " ", " ", " ", "", ""); num++; numberx += 1;
                            //            table.Rows.Add(numberx, num.ToString() + ". ", x.LovValue1, " ", " ", "", ""); num++; numberx += 1;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        foreach (var x in data.ToList())
                            //        {
                            //            //table.Rows.Add(numberx, num.ToString() + ". " + x.LovValue2, " ", " ", " ", "", ""); num++; numberx += 1;
                            //            table.Rows.Add(numberx, num.ToString() + ". ", x.LovValue2, " ", " ", "", ""); num++; numberx += 1;
                            //        }
                            //    }
                            //    EndCon = data.ToList().Count().ToString();

                            //}

                            #endregion condition

                            #region document

                            if (l.Name.Equals("L_DOCUMENT"))
                            {
                                continue;
                            }

                            #endregion document

                            #region address billing

                            else if (l.Name.Equals("L_BILLING_DETAIL"))// add detail
                            {
                                var value = "-";
                                if (model.CustomerRegisterPanelModel.outFullAddress.ToSafeString() != "")
                                {
                                    value = model.CustomerRegisterPanelModel.outFullAddress.ToSafeString();
                                }
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, value, "", "", "", "", ""); numberx += 1;
                                //string coln1 = ""; string coln2 = ""; int currentcol = 1;
                                //foreach (var x in ListEntity.Where(t => t.GroupByPDF.Equals("03D")))
                                //{
                                //    if (x.Name.Equals("L_MOOBAN") || x.Name.Equals("L_ROOM"))
                                //    {
                                //        var Value3 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                //        if (Value3.ToSafeString() == "") Value3 = "-";
                                //        table.Rows.Add(numberx, coln1, Value1, coln2, Value2, x.DisplayValue, Value3); numberx += 1;
                                //        currentcol = 1;
                                //    }
                                //    else if (x.Name.Equals("L_ROAD") || x.Name.Equals("L_AMPHUR") || x.Name.Equals("L_ZIPCODE") || x.Name.Equals("ZIPCODE_ID"))
                                //    {
                                //        if (x.Name.Equals("ZIPCODE_ID")) x.Name = "L_ZIPCODE";
                                //        Value1 = "";
                                //        Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, name1);
                                //        Value2 = "";
                                //        Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                //        if (Value1.ToSafeString() == "") Value1 = "-";
                                //        if (Value2.ToSafeString() == "") Value2 = "-";
                                //        table.Rows.Add(numberx, coln1, Value1, "", "", x.DisplayValue, Value2); numberx += 1;
                                //        currentcol = 1;
                                //    }
                                //    //if (l.Name.Equals("L_MOO") || l.Name.Equals("L_FLOOR"))
                                //    //{
                                //    //    continue;
                                //    //}
                                //    else if (currentcol == 1)
                                //    {
                                //        coln1 = "";
                                //        coln2 = "";
                                //        coln1 = x.DisplayValue;
                                //        currentcol += 1;
                                //        name1 = "";
                                //        name1 = x.Name;
                                //    }
                                //    else
                                //    {
                                //        #region get value
                                //        Value1 = "";
                                //        Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, name1);

                                //        Value2 = "";
                                //        Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                //        if (Value1.ToSafeString() == "") Value1 = "-";
                                //        if (Value2.ToSafeString() == "") Value2 = "-";
                                //        #endregion

                                //        coln2 = x.DisplayValue;
                                //        currentcol = 1;
                                //    }

                                //}
                                //if (model.CustomerRegisterPanelModel.DocType == "NON_RES")
                                //{
                                //    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                //}
                            }

                            #endregion address billing

                            #region address vat

                            else if ((l.Name.Equals("L_VAT_DETAIL")))// add detail
                            {
                                if (type != "R")
                                {
                                    var value = "-";
                                    if (model.CustomerRegisterPanelModel.outFullAddress.ToSafeString() != "")
                                    {
                                        value = model.CustomerRegisterPanelModel.vatAddressFull.ToSafeString();
                                        if (string.IsNullOrEmpty(value.Trim()))
                                        {
                                            value = "-";
                                        }
                                    }
                                    table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                    table.Rows.Add(numberx, value, "", "", "", "", ""); numberx += 1;
                                    //string coln1 = ""; string coln2 = ""; int currentcol = 1;
                                    //foreach (var x in ListEntity.Where(t => t.GroupByPDF.Equals("03D")))
                                    //{
                                    //    if (x.Name.Equals("L_MOOBAN") || x.Name.Equals("L_ROOM"))
                                    //    {
                                    //        var Value3 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
                                    //        if (Value3.ToSafeString() == "") Value3 = "-";
                                    //        table.Rows.Add(numberx, coln1, Value1, coln2, Value2, x.DisplayValue, Value3); numberx += 1;
                                    //        currentcol = 1;
                                    //    }
                                    //    else if (x.Name.Equals("L_ROAD") || x.Name.Equals("L_AMPHUR") || x.Name.Equals("L_ZIPCODE") || x.Name.Equals("ZIPCODE_ID"))
                                    //    {
                                    //        if (x.Name.Equals("ZIPCODE_ID")) x.Name = "L_ZIPCODE";
                                    //        Value1 = "";
                                    //        Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, name1);
                                    //        Value2 = "";
                                    //        Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
                                    //        if (Value1.ToSafeString() == "") Value1 = "-";
                                    //        if (Value2.ToSafeString() == "") Value2 = "-";
                                    //        table.Rows.Add(numberx, coln1, Value1, "", "", x.DisplayValue, Value2); numberx += 1;
                                    //        currentcol = 1;
                                    //    }
                                    //    //if (l.Name.Equals("L_MOO") || l.Name.Equals("L_FLOOR"))
                                    //    //{
                                    //    //    continue;
                                    //    //}
                                    //    else if (currentcol == 1)
                                    //    {
                                    //        coln1 = "";
                                    //        coln2 = "";
                                    //        coln1 = x.DisplayValue;
                                    //        currentcol += 1;
                                    //        name1 = "";
                                    //        name1 = x.Name;
                                    //    }
                                    //    else
                                    //    {
                                    //        #region get value
                                    //        Value1 = "";
                                    //        Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, name1);

                                    //        Value2 = "";
                                    //        Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                    //        if (Value1.ToSafeString() == "") Value1 = "-";
                                    //        if (Value2.ToSafeString() == "") Value2 = "-";
                                    //        #endregion

                                    //        coln2 = x.DisplayValue;
                                    //        currentcol = 1;
                                    //    }

                                    //}
                                    //if (model.CustomerRegisterPanelModel.DocType == "NON_RES")
                                    //{
                                    //    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                    //}
                                }
                            }

                            #endregion address vat

                            #region header group

                            else
                            {
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;

                                if (l.Name.Equals("L_INSTALL_DETAIL"))
                                {
                                    var value = "-";
                                    if (model.CustomerRegisterPanelModel.v_installAddress.ToSafeString() != "")
                                    {
                                        value = model.CustomerRegisterPanelModel.v_installAddress.ToSafeString();
                                    }
                                    table.Rows.Add(numberx, value, "", "", "", "", ""); numberx += 1;
                                }
                            }

                            #endregion header group
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

                            if (l.Name.Equals("L_MAIN_PACK"))
                            {
                                continue;
                            }
                            else if (l.Name.Equals("L_ONTOP_PLAYBOX_PRE_INITIATION"))
                            {
                                continue;
                            }
                            else if (l.Name.Equals("L_ONTOP_PLAYBOX_DISCOUNT_INITIATION"))
                            {
                                continue;
                            }
                            else if ((l.Name.Equals("L_ONTOP_PACK")))//แพกเกจเสริม
                            {
                                var val = model.SummaryPanelModel.PDFPackageModel.PDF_L_ONTOPLIST;

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
                            else if (l.Name.Equals("L_SUMM_DETAIL_WireBB") || l.Name.Equals("L_SUMM_DETAIL_FTTx") || l.Name.Equals("L_SUMM_DETAIL_SWiFi"))
                            {
                                continue;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL5")))//รายการที่ต้องชำระครั้งเดวในรอบบิลแรก
                            {
                                continue;
                                //table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_MULTI_PLAYBOX_INITIATIONS")))//รายการที่ต้องชำระครั้งเดวในรอบบิลแรก
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if (l.Name.Equals("L_SUMM_DETAIL6") || l.Name.Equals("L_SUMM_DETAIL7") || l.Name.Equals("L_SUMM_DETAIL10"))
                            {
                                continue;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL8")))
                            {
                                var sum1 = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM1;

                                table.Rows.Add(numberx, "", l.DisplayValue, sum1, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL11")))
                            {
                                var sum2 = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM2;
                                table.Rows.Add(numberx, "", l.DisplayValue, sum2, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUM_INSTALL_RATE")))//
                            {
                                var suminstall = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_BASEPHONE;
                                table.Rows.Add(numberx, "", l.DisplayValue, suminstall, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUM_DISCOUNT_INSTALL_RATE")))
                            {
                                var disbasephone = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNTBASEPHONE;
                                table.Rows.Add(numberx, "", l.DisplayValue, disbasephone, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL9")))//รายการที่ต้องชำระรายเดือน
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUM_MAINTEN_RATE")))//
                            {
                                var summaintain = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_MAINTAINPHONE;

                                table.Rows.Add(numberx, "", l.DisplayValue, summaintain, unit, "", ""); numberx += 1;
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
                            else if ((l.Name.Equals("L_SUMM_DETAIL15")))//
                            {
                                var firsbillsum = model.SummaryPanelModel.PDFPackageModel.PDF_L_LABEL_DETAIL_ALLFIRSTBILL;

                                table.Rows.Add(numberx, "", l.DisplayValue, firsbillsum, unit, "", ""); numberx += 1;
                            }
                            else if (l.Name.Equals("L_BUNDLING_DISCOUNT_DTL")
                                    || l.Name.Equals("L_DISCOUNT_DTL")
                                    )
                            {
                                continue;
                            }

                            #endregion Package

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
                                            //var val1 = model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            //var val2 = model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            //var val3 = model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            //Value1 = val1.ToString();
                                            //if ((Value1.Trim() != "") && (val2.ToString().Trim() != "")) { Value1 += ", "; } Value1 += val2.ToString();
                                            //if ((Value1.Trim() != "") && (val3.ToString().Trim() != "")) { Value1 += ", "; } Value1 += val3.ToString();
                                            Value1 = model.CustomerRegisterPanelModel.L_CONTACT_PHONE;//val1.ToString() + " " + val2.ToString() + " " + val3.ToString();
                                            if (Value2.ToSafeString() == "") Value2 = "-";
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
                                            //var a1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                            //var a2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            //if (a1.ToSafeString() == "") a1 = "-";
                                            //if (a2.ToSafeString() == "") a2 = "-";
                                            //if (Value2.ToSafeString() == "") Value2 = "-";
                                            //table.Rows.Add(numberx, column1, a1, column2, Value2, l.DisplayValue, a2); numberx += 1;
                                            continue;
                                        }
                                        else if ((l.Name.Equals("L_MOO")) || (l.Name.Equals("L_FLOOR")))
                                        {
                                            //column2 = "";
                                            //column2 = l.DisplayValue;
                                            //Value2 = "";
                                            //Value2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            continue;
                                        }
                                        else if ((l.Name.Equals("L_HOME_NUMBER_2")) || (l.Name.Equals("L_PROVINCE")) || (l.Name.Equals("L_TUMBOL")) || (l.Name.Equals("L_SOI")) || (l.Name.Equals("L_BUILD_NAME")))
                                        {
                                            //column1 = "";
                                            //column1 = l.DisplayValue;
                                            //name1 = "";
                                            //name1 = l.Name;
                                            //Value1 = "";
                                            //Value1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            continue;
                                        }
                                        else if (l.Name.Equals("L_ROAD") || l.Name.Equals("L_AMPHUR") || l.Name.Equals("L_ZIPCODE") || l.Name.Equals("ZIPCODE_ID"))
                                        {
                                            //if (l.Name.Equals("ZIPCODE_ID")) l.Name = "L_ZIPCODE";
                                            //var a1 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                            //var a2 = GetModelValue(model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            //if (a1.ToSafeString() == "") a1 = "-";
                                            //if (a2.ToSafeString() == "") a2 = "-";
                                            //table.Rows.Add(numberx, column1, a1, "", "", l.DisplayValue, a2); numberx += 1;

                                            //if ((l.Name.Equals("L_ZIPCODE")) && (model.CustomerRegisterPanelModel.DocType == "NON_RES"))
                                            //{
                                            //    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                            //}
                                            continue;
                                        }

                                        #endregion address setup

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
                                            //var val1 = model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            //var val2 = model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            //var val3 = model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            //Value1 = val1.ToString();
                                            //if ((Value1.Trim() != "") && (val2.ToString().Trim() != "")) { Value1 += ", "; } Value1 += val2.ToString();
                                            //if ((Value1.Trim() != "") && (val3.ToString().Trim() != "")) { Value1 += ", "; } Value1 += val3.ToString();
                                            Value1 = model.CustomerRegisterPanelModel.L_CONTACT_PHONE;
                                            if (Value1.ToSafeString() == "") Value1 = "-";
                                        }

                                        Value2 = "";

                                        if ((l.Name.Equals("L_NAME")))
                                        {
                                            var val1 = model.CustomerRegisterPanelModel.L_TITLE; if (val1 == null) { val1 = ""; }
                                            //if (val1 == "127") { val1 = "คุณ"; }
                                            var val2 = model.CustomerRegisterPanelModel.L_FIRST_NAME; if (val2 == null) { val2 = ""; }
                                            var val3 = model.CustomerRegisterPanelModel.L_LAST_NAME; if (val3 == null) { val3 = ""; }

                                            Value2 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();
                                        }
                                        else if (l.Name.Equals("L_CONTACT_PHONE"))
                                        {
                                            //var val1 = model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            //var val2 = model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            //var val3 = model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            //Value2 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();
                                            //Value2 = val1.ToString();
                                            //if ((Value2.Trim() != "") && (val2.ToString().Trim() != "")) { Value2 += ", "; } Value2 += val2.ToString();
                                            //if ((Value2.Trim() != "") && (val3.ToString().Trim() != "")) { Value2 += ", "; } Value2 += val3.ToString();
                                            Value2 = model.CustomerRegisterPanelModel.L_CONTACT_PHONE;
                                            if (Value2.ToSafeString() == "") Value2 = "-";
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
                                            Value2 = ""; /// แก้ให้ไม่แสดงวันที่ติดตั้ง
                                            if (Value2.ToSafeString() == "") Value2 = "-";
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

                        #endregion set value to datatable
                    }
                }

                #endregion Format PDF

                #region Create PDF

                var pdfIndexTemp = GetTermCon();

                LovValueModel[] pdfIndex = pdfIndexTemp.ToArray();

                var borderSize = 0.7f;
                var borderCoolor = BaseColor.GREEN;
                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    var fixedHeight = 13f;

                    bool chkend2 = true; bool chkendDoc = false;
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, myMemoryStream);
                    //PdfWriter.GetInstance(document, new FileStream(Request.PhysicalApplicationPath + "\\Chap0101.pdf", FileMode.Create));
                    document.Open();
                    PdfPTable PdfTable = new PdfPTable(6);
                    PdfTable.TotalWidth = 415;

                    BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/fonts/tahoma.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // สร้าง Font จาก BaseFont
                    iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 6.7f);
                    iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6f);
                    iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 7f, Font.NORMAL, BaseColor.GREEN);
                    iTextSharp.text.Font fntbold = new iTextSharp.text.Font(bf, 6.7f, Font.BOLD);

                    PdfPCell PdfPCell = null;
                    for (int x = 1; x <= 1; x++)
                    {
                        if (x == 1)
                        {
                            table = new System.Data.DataTable();
                            table = table1;
                            document.NewPage();

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
                        }
                        //else
                        //{
                        //    table = new System.Data.DataTable(); table = table2;
                        //    document.NewPage();

                        //}

                        //document.NewPage();

                        //BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/fonts/tahoma.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        //// สร้าง Font จาก BaseFont
                        //iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 7f);
                        //iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6.5f);
                        //iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 8f, Font.NORMAL, BaseColor.GREEN);

                        // Add PdfTable to pdfDoc
                        //document.Add(PdfTable);

                        ////start => Add image
                        // background
                        iTextSharp.text.Image pdfbg = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/BG_base1.jpg"));
                        //pdfbg.ScaleToFit(3400, 900);
                        pdfbg.ScaleToFit(3200, 850);
                        pdfbg.Alignment = iTextSharp.text.Image.UNDERLYING;
                        pdfbg.SetAbsolutePosition(0, 0);
                        document.Add(pdfbg);

                        // logo
                        iTextSharp.text.Image pdflogo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/img-logo.png"));
                        pdflogo.ScaleToFit(200, 100);
                        pdflogo.Alignment = iTextSharp.text.Image.UNDERLYING; pdflogo.SetAbsolutePosition(380, 712);
                        document.Add(pdflogo);
                        // status
                        string imgStatus = "";
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        { imgStatus = "~/Content/images/vas/statusTH-03.png"; }
                        else
                        { imgStatus = "~/Content/images/vas/statusEN-03.png"; }
                        iTextSharp.text.Image pdfstatus = iTextSharp.text.Image.GetInstance(Server.MapPath(imgStatus));
                        pdfstatus.ScaleToFit(300, 150);
                        pdfstatus.Alignment = iTextSharp.text.Image.UNDERLYING; pdfstatus.SetAbsolutePosition(80, 712);
                        document.Add(pdfstatus);

                        iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                        iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                        // Write Data ที่ PdfCell ใน PdfTable
                        for (int row = 0; row < table.Rows.Count; row++)
                        {
                            PdfTable = new PdfPTable(table.Columns.Count - 1);
                            PdfTable.TotalWidth = 415;
                            //PdfTable.SetWidths(new float[] { 15f, 25f, 10f, 10f, 10f, 10f });
                            string id_ = table.Rows[row].ItemArray[0].ToString();

                            if (displayType_ == "R") /// space
                            {
                                if ((id_ == "2") || (id_ == "3") || (id_ == "25") ||
                                     (id_ == "10") || (id_ == "29") || (id_ == "34") || (id_ == "35") || (id_ == "36") || (id_ == "37") || (id_ == "38") || (id_ == "39") || (id_ == "40") ||
                                     (id_ == "42") || (id_ == "44") || (id_ == "45") || (id_ == "46") || (id_ == "47") || (id_ == "48"))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((id_ == "2") || (id_ == "3") || (id_ == "10") || (id_ == "25") || (id_ == "29") || (id_ == "33") || (id_ == "42")
                                   || (id_ == "38") || (id_ == "39") || (id_ == "40") || (id_ == "41") || (id_ == "50") || (id_ == "51") || (id_ == "52") || (id_ == "53") || (id_ == "54"))
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

                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

                            if ((id_ == EndInfo) || (id_ == EndPack) || (id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
                            {
                                if (id_ == EndInfo)
                                {
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt1))); PdfPCell.FixedHeight = fixedHeight;
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

                                if (id_ == EndAddressSetup || (id_ == EndAddressBill) || (id_ == EndAddressVat))
                                {
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt1))); PdfPCell.FixedHeight = fixedHeight;
                                    PdfPCell.Colspan = 6;
                                    //PdfTable.SetWidths(new float[] { f, 15f, 0f, 15f, 15f, 0f });

                                    PdfPCell.BorderColorRight = borderCoolor;
                                    PdfPCell.BorderColorTop = borderCoolor;
                                    PdfPCell.BorderColorLeft = borderCoolor;
                                    PdfPCell.BorderColorBottom = borderCoolor;
                                    PdfPCell.BorderWidthLeft = borderSize;
                                    PdfPCell.BorderWidthRight = borderSize;
                                    PdfPCell.BorderWidthTop = 0;
                                    PdfPCell.BorderWidthBottom = borderSize;
                                }
                            }
                            ////// record ที่เป็นหัวรายการของแต่ละอัน
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "14") || (id_ == "18") || (id_ == "21")))
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
                            ///// ตัวแรกของ แต่ละกลุ่ม
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == starInfo.ToString()) || (id_ == startPack.ToString()) || (id_ == startAddressSetup.ToString()) || (id_ == startAddressBill.ToString()) || (id_ == startAddressVat.ToString()) || (id_ == startDoc.ToString())))
                            {
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fntgreen))); PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = borderSize;
                                PdfPCell.BorderWidthBottom = 0;
                            }
                            /// หัวตัวแรกสุด ข้อมูลผู้สมัคร
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "1")))
                            {
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8")))
                                {
                                    if (id_ == "8")
                                    {
                                        if (val_2.Split(',').Length > 1)
                                        {
                                            PdfPCell.FixedHeight = 20f;
                                        }
                                    }

                                    if (displayType_ == "R")
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 15f, 0f, 15f, 25f, 0f });
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

                            if ((x == 2) || ((string.IsNullOrEmpty(val_1.Trim())) && (string.IsNullOrEmpty(val_2.Trim())) && (string.IsNullOrEmpty(val_3.Trim()) && (string.IsNullOrEmpty(val_4.Trim())) && (string.IsNullOrEmpty(val_5.Trim())))))
                            {
                                if (string.IsNullOrEmpty(val_0.Trim()))
                                {
                                    if ((x == 1) && ((Convert.ToInt32(id_) > startDoc)))
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
                                    else { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                }
                                if ((!string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())) && (x == 2))
                                {
                                    if (chkend2)
                                    {
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(val_0.Trim(), fntgreen))); PdfPCell.FixedHeight = fixedHeight;
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
                                else if ((!string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())) && (x == 1) && ((Convert.ToInt32(id_) > startDoc)))
                                {
                                    PdfPCell.BorderColorBottom = borderCoolor;
                                    PdfPCell.BorderColorRight = borderCoolor;
                                    PdfPCell.BorderColorTop = borderCoolor;
                                    PdfPCell.BorderColorLeft = borderCoolor;
                                    PdfPCell.BorderWidthLeft = borderSize;
                                    PdfPCell.BorderWidthBottom = 0;
                                    PdfPCell.BorderWidthTop = 0;
                                    PdfPCell.BorderWidthRight = borderSize;
                                    if (val_0.Contains("@")) { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                }
                                if ((string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())))
                                {
                                    PdfPCell.Border = PdfPCell.NO_BORDER;
                                }

                                if (val_0.Contains("@")) { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                PdfPCell.Colspan = 6;
                            }
                            else if (((Convert.ToInt32(id_) > startDoc)))
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

                            if ((!string.IsNullOrEmpty(val_0.Trim())) && (!string.IsNullOrEmpty(val_1.Trim())) && (x == 2))
                            {
                                PdfTable.SetWidths(new float[] { 3f, 37f, 10f, 10f, 10f, 10f });
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(pdfIndex[int.Parse(table.Rows[row].ItemArray[1].ToString().Replace(".", "")) - 1].Text, fnt))); //PdfPCell.FixedHeight = fixedHeight;

                                //PdfPCell.Width = 30;

                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthRight = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                { PdfPCell.BorderWidthBottom = borderSize; }
                                else
                                { PdfPCell.BorderWidthBottom = 0; }

                                PdfTable.AddCell(PdfPCell);

                                string tempArrayData2 = table.Rows[row].ItemArray[2].ToString();
                                while (tempArrayData2.IndexOf('<') >= 0 && tempArrayData2.IndexOf('>') >= 0)
                                {

                                    tempArrayData2 = tempArrayData2.Replace(tempArrayData2.Substring(tempArrayData2.IndexOf('<'), tempArrayData2.IndexOf('>') - tempArrayData2.IndexOf('<') + 1), "");
                                }

                                PdfPCell = new PdfPCell(new Phrase(new Chunk(tempArrayData2, fnt))); //PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = 0;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                { PdfPCell.BorderWidthBottom = borderSize; }
                                else
                                { PdfPCell.BorderWidthBottom = 0; }
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
                                if ((!string.IsNullOrEmpty(val_1.Trim())) || (!string.IsNullOrEmpty(val_2.Trim())) || (!string.IsNullOrEmpty(val_3.Trim())) || (!string.IsNullOrEmpty(val_4.Trim())) || (!string.IsNullOrEmpty(val_5.Trim())))
                                {
                                    /// record ที่มี ค่าเดียว ใน package
                                    if ((id_ == "14") || (id_ == "18") | (id_ == "21") || (id_ == EndPack) || ((Convert.ToInt32(id_) > startDoc)))
                                    {
                                        if ((id_ == EndPack))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }
                                        if (val_1 != "hasBorder")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

                                            if (id_ == EndPack)
                                            {
                                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                            }
                                        }
                                        //PdfPCell.Border = PdfPCell.NO_BORDER;
                                        PdfPCell.Colspan = 5;

                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;
                                        PdfPCell.BorderWidthBottom = 0;

                                        if ((id_ == EndPack) || (id_ == (startDoc + Convert.ToInt16(EndDoc) - 1).ToString()))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else if ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8") || (id_ == EndInfo) || (id_ == "15") || (id_ == "16") || (id_ == "17") || (id_ == "19") || (id_ == "20") || (id_ == "22") || (id_ == "23"))
                                    {
                                        if ((id_ == "15") || (id_ == "16") || (id_ == "17") || (id_ == "19") || (id_ == "20") || (id_ == "22") || (id_ == "23"))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }
                                        if ((id_ == EndInfo))
                                        {
                                            PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                        }

                                        if (id_ == "23")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfPCell.Colspan = 2;
                                        PdfTable.AddCell(PdfPCell);
                                        if (id_ == "23")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);

                                        if (id_ == "23")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.BorderWidthBottom = 0; }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 30f, 5f, 5f, 10f, 20f });
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[5].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[6].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        //PdfPCell.Border = PdfPCell.NO_BORDER;

                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthBottom = 0;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;

                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                }
                            }

                            document.Add(PdfTable);
                        }
                    }

                    #endregion Create PDF

                    document.Close();
                    byte[] content = myMemoryStream.ToArray();

                    //SET PDF PASSWORD
                    var pdfbyte = PdfSecurity.SetPasswordPdf(content, model.CustomerRegisterPanelModel.L_CARD_NO);
                    Session["PFDBYTE"] = pdfbyte;

                    //Write file to NAS
                    var pathfile = directoryPath + "\\" + fileName + ".pdf";
                    PdfSecurity.WriteFile(pathfile, pdfbyte);
                }

                return directoryTempPath + "\\" + fileName + ".pdf";
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }

        public string GeneratePDF_TriplePlay(QuickWinPanelModel model, string directoryPath,
           string directoryTempPath, string fileName)
        {
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
                int numOntop = model.SummaryPanelModel.PDFPackageModel.PDF_L_ONTOPLIST.Count;

                var displayType_ = model.CustomerRegisterPanelModel.CateType;
                if (displayType_ == "R")
                {
                    EndAddressSetup = "38";
                    EndAddressBill = "46";
                    EndAddressVat = "";
                    EndPack = "30";
                    EndInfo = "9";
                    startDoc = 54;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 33;
                    startAddressBill = 41;
                    startAddressVat = 0;
                }
                else  //// G/B
                {
                    EndAddressSetup = "38";
                    EndAddressBill = "47";
                    EndAddressVat = "55";
                    EndPack = "30";
                    EndInfo = "9";
                    startDoc = 61;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 33;
                    startAddressBill = 42;
                    startAddressVat = 50;
                }

                #endregion setborder

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

                        #endregion กำหนดช่องว่างระหว่าง group

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

                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;

                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                                //table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;

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

                            #endregion condition

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

                            #endregion document

                            #region address billing

                            else if (l.Name.Equals("L_BILLING_DETAIL"))// add detail
                            {
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                string coln1 = ""; string coln2 = ""; int currentcol = 1;
                                foreach (var x in ListEntity.Where(t => t.GroupByPDF.Equals("03D")))
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

                                        #endregion get value

                                        coln2 = x.DisplayValue;
                                        currentcol = 1;
                                    }
                                }
                                //if (model.CustomerRegisterPanelModel.DocType == "NON_RES")
                                //{
                                //    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                //}
                            }

                            #endregion address billing

                            #region address vat

                            else if ((l.Name.Equals("L_VAT_DETAIL")))// add detail
                            {
                                if (type != "R")
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                    string coln1 = ""; string coln2 = ""; int currentcol = 1;
                                    foreach (var x in ListEntity.Where(t => t.GroupByPDF.Equals("03D")))
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

                                            #endregion get value

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

                            #endregion address vat

                            #region header group

                            else
                            {
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                            }

                            #endregion header group
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

                            else if (l.Name.Equals("L_MAIN_PACK"))//แพกเกจหลัก
                            {
                                var val = model.SummaryPanelModel.PDFPackageModel.PDF_L_MAIN_PACKAGE;
                                if (val != null)
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, val.ToString(), "", "", "", ""); numberx += 1;
                                }
                                else
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, "-", "", "", "", ""); numberx += 1;
                                }
                            }
                            else if ((l.Name.Equals("L_ONTOP_PACK")))//แพกเกจเสริม
                            {
                                var val = model.SummaryPanelModel.PDFPackageModel.PDF_L_ONTOPLIST;

                                if (val.Count > 0)
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
                                    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                }

                                if (val.Count == 1)
                                {
                                    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                }
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL1")))//รายการที่ต้องชำระในวันที่ติดตั้ง
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if (l.Name.Equals("L_SUMM_DETAIL_WireBB"))
                            {
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
                            //else if ((l.Name.Equals("L_SUMM_DETAIL2")))//
                            //{
                            //    Value1 = "";
                            //    Value1 += l.DisplayValue;

                            //}
                            //else if ((l.Name.Equals("L_SUMM_DETAIL3")))//
                            //{
                            //    Value1 += l.DisplayValue;

                            //}
                            //else if ((l.Name.Equals("L_SUMM_DETAIL4")))//
                            //{
                            //    Value1 += l.DisplayValue;
                            //    table.Rows.Add(numberx, "", Value1, "", "", "", ""); numberx += 1;
                            //}
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
                            //else if ((l.Name.Equals("L_SUM_INSTALL_RATE")))//
                            //{
                            //    var basephone = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_BASEPHONE;
                            //    table.Rows.Add(numberx, "", l.DisplayValue, basephone, unit, "", ""); numberx += 1;

                            //}
                            else if ((l.Name.Equals("L_SUMM_DETAIL7")))//ลดค่าติดตั้ง
                            {
                                var discount = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNT;
                                table.Rows.Add(numberx, "", l.DisplayValue, discount, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL8")))//
                            {
                                var sum1 = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM1;

                                table.Rows.Add(numberx, "", l.DisplayValue, sum1, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_ONTOP_PLAYBOX_PRE_INITIATION")))// playbox ค่าติดตั้ง
                            {
                                var sum1 = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_PLAYBOXINSTALL;

                                table.Rows.Add(numberx, "", l.DisplayValue, sum1, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_ONTOP_PLAYBOX_DISCOUNT_INITIATION")))// playbox ส่วนลดค่าติดตั้ง
                            {
                                var sum1 = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNTPLAYBOX;

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
                            else if ((l.Name.Equals("L_SUM_MAINTEN_RATE")))//
                            {
                                continue;
                            }
                            else if ((l.Name.Equals("L_SUM_DISCOUNT_INSTALL_RATE")))//
                            {
                                continue;
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
                            else if ((l.Name.Equals("L_SUMM_DETAIL15")))//
                            {
                                var firsbillsum = model.SummaryPanelModel.PDFPackageModel.PDF_L_LABEL_DETAIL_ALLFIRSTBILL;

                                table.Rows.Add(numberx, "", l.DisplayValue, firsbillsum, unit, "", ""); numberx += 1;
                            }
                            else if (l.Name.Equals("L_BUNDLING_DISCOUNT_DTL")
                                    || l.Name.Equals("L_DISCOUNT_DTL")
                                    )
                            {
                                continue;
                            }

                            #endregion Package

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
                                            //Value1 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();
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

                                        #endregion address setup

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

                        #endregion set value to datatable
                    }
                }

                #endregion Format PDF

                #region Create PDF

                var pdfIndexTemp = GetTermCon();

                LovValueModel[] pdfIndex = pdfIndexTemp.ToArray();

                var borderSize = 0.7f;
                var borderCoolor = BaseColor.GREEN;
                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    var fixedHeight = 13f;

                    bool chkend2 = true; bool chkendDoc = false;
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, myMemoryStream);
                    //PdfWriter.GetInstance(document, new FileStream(Request.PhysicalApplicationPath + "\\Chap0101.pdf", FileMode.Create));
                    document.Open();
                    PdfPTable PdfTable = new PdfPTable(6);
                    PdfTable.TotalWidth = 415;

                    BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/fonts/tahoma.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // สร้าง Font จาก BaseFont
                    iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 6.7f);
                    iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6f);
                    iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 7f, Font.NORMAL, BaseColor.GREEN);
                    iTextSharp.text.Font fntbold = new iTextSharp.text.Font(bf, 6.7f, Font.BOLD);

                    PdfPCell PdfPCell = null;
                    for (int x = 1; x <= 2; x++)
                    {
                        if (x == 1)
                        {
                            table = new System.Data.DataTable();
                            table = table1;
                            document.NewPage();

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
                        }
                        else
                        {
                            table = new System.Data.DataTable(); table = table2;
                            //if (displayType_ == "R")
                            //{
                            //    document.NewPage();
                            //}
                        }

                        //document.NewPage();

                        //BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/fonts/tahoma.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        //// สร้าง Font จาก BaseFont
                        //iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 7f);
                        //iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6.5f);
                        //iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 8f, Font.NORMAL, BaseColor.GREEN);

                        // Add PdfTable to pdfDoc
                        //document.Add(PdfTable);

                        ////start => Add image
                        // background
                        iTextSharp.text.Image pdfbg = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/BG_base1.jpg"));
                        //pdfbg.ScaleToFit(3400, 900);
                        pdfbg.ScaleToFit(3200, 850);
                        pdfbg.Alignment = iTextSharp.text.Image.UNDERLYING;
                        pdfbg.SetAbsolutePosition(0, 0);
                        document.Add(pdfbg);

                        // logo
                        iTextSharp.text.Image pdflogo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/img-logo.png"));
                        pdflogo.ScaleToFit(200, 100);
                        pdflogo.Alignment = iTextSharp.text.Image.UNDERLYING; pdflogo.SetAbsolutePosition(380, 712);
                        document.Add(pdflogo);
                        // status
                        string imgStatus = "";
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        { imgStatus = "~/Content/src/thai_5.png"; }
                        else
                        { imgStatus = "~/Content/src/eng_5.png"; }
                        iTextSharp.text.Image pdfstatus = iTextSharp.text.Image.GetInstance(Server.MapPath(imgStatus));
                        pdfstatus.ScaleToFit(300, 150);
                        pdfstatus.Alignment = iTextSharp.text.Image.UNDERLYING; pdfstatus.SetAbsolutePosition(80, 712);
                        document.Add(pdfstatus);

                        iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                        iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                        // Write Data ที่ PdfCell ใน PdfTable
                        for (int row = 0; row < table.Rows.Count; row++)
                        {
                            PdfTable = new PdfPTable(table.Columns.Count - 1);
                            PdfTable.TotalWidth = 415;
                            //PdfTable.SetWidths(new float[] { 15f, 25f, 10f, 10f, 10f, 10f });
                            string id_ = table.Rows[row].ItemArray[0].ToString();

                            if (Convert.ToInt32(id_) == 47 && displayType_ == "R")
                            {
                                document.NewPage();
                                document.Add(pdfbg);
                                document.Add(pdflogo);
                                document.Add(pdfstatus);
                                PdfTable = new PdfPTable(table.Columns.Count - 1);
                                PdfTable.TotalWidth = 415;
                                styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                                hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                                for (int row2 = 1; row2 < 6; row2++)
                                {
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt))); PdfPCell.FixedHeight = fixedHeight;
                                    PdfPCell.Border = PdfPCell.NO_BORDER;
                                    PdfPCell.Colspan = 6;
                                    PdfTable.AddCell(PdfPCell);
                                }
                                document.Add(PdfTable);
                            }
                            else if (Convert.ToInt32(id_) == 58 && displayType_ != "R")
                            {
                                document.NewPage();
                                document.Add(pdfbg);
                                document.Add(pdflogo);
                                document.Add(pdfstatus);
                                PdfTable = new PdfPTable(table.Columns.Count - 1);
                                PdfTable.TotalWidth = 415;
                                styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                                hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                                for (int row2 = 1; row2 < 6; row2++)
                                {
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt))); PdfPCell.FixedHeight = fixedHeight;
                                    PdfPCell.Border = PdfPCell.NO_BORDER;
                                    PdfPCell.Colspan = 6;
                                    PdfTable.AddCell(PdfPCell);
                                }
                                document.Add(PdfTable);
                            }

                            if (displayType_ == "R") /// space
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx" && (id_ == "17"))
                                {
                                    continue;
                                }

                                if ((id_ == "2") || (id_ == "3") || (id_ == "10") || (id_ == "31") || (id_ == "39") || (id_ == "47"))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx" && (id_ == "17"))
                                {
                                    continue;
                                }

                                if ((id_ == "2") || (id_ == "3") || (id_ == "10") || (id_ == "31") || (id_ == "39") || (id_ == "40") || (id_ == "48") || (id_ == "56") || (id_ == "57"))
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

                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

                            if ((id_ == EndInfo) || (id_ == EndPack) || (id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
                            {
                                if (id_ == EndInfo)
                                {
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt1))); PdfPCell.FixedHeight = fixedHeight;
                                    PdfTable.SetWidths(new float[] { 15f, 15f, 0f, 15f, 15f, 0f });
                                }

                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = 0;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthBottom = borderSize;//(id_ == "16")(id_ == "23")
                            }
                            ////// record ที่เป็นหัวรายการของแต่ละอัน
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "16") || (id_ == "18") || (id_ == "24") || (id_ == "27")))
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
                            ///// ตัวแรกของ แต่ละกลุ่ม
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == starInfo.ToString()) || (id_ == startPack.ToString()) || (id_ == startAddressSetup.ToString()) || (id_ == startAddressBill.ToString()) || (id_ == startAddressVat.ToString()) || (id_ == startDoc.ToString())))
                            {
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fntgreen))); PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = borderSize;
                                PdfPCell.BorderWidthBottom = 0;
                            }
                            /// หัวตัวแรกสุด ข้อมูลผู้สมัคร
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "1")))
                            {
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8")))
                                {
                                    if (id_ == "8")
                                    {
                                        if (val_2.Split(',').Length > 1)
                                        {
                                            PdfPCell.FixedHeight = 20f;
                                        }
                                    }

                                    if (displayType_ == "R")
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 15f, 0f, 15f, 25f, 0f });
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

                            if ((x == 2) || ((string.IsNullOrEmpty(val_1.Trim())) && (string.IsNullOrEmpty(val_2.Trim())) && (string.IsNullOrEmpty(val_3.Trim()) && (string.IsNullOrEmpty(val_4.Trim())) && (string.IsNullOrEmpty(val_5.Trim())))))
                            {
                                if (string.IsNullOrEmpty(val_0.Trim()))
                                {
                                    if ((x == 1) && ((Convert.ToInt32(id_) > startDoc)))
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
                                    else { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                }
                                if ((!string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())) && (x == 2))
                                {
                                    if (chkend2)
                                    {
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(val_0.Trim(), fntgreen))); PdfPCell.FixedHeight = fixedHeight;
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
                                else if ((!string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())) && (x == 1) && ((Convert.ToInt32(id_) > startDoc)))
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
                                if ((string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())))
                                {
                                    PdfPCell.Border = PdfPCell.NO_BORDER;
                                }
                                if (id_ == "15")
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
                                PdfPCell.Colspan = 6;
                            }
                            else if (((Convert.ToInt32(id_) > startDoc)))
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

                            if ((!string.IsNullOrEmpty(val_0.Trim())) && (!string.IsNullOrEmpty(val_1.Trim())) && (x == 2))
                            {
                                PdfTable.SetWidths(new float[] { 3f, 37f, 10f, 10f, 10f, 10f });
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(pdfIndex[int.Parse(table.Rows[row].ItemArray[1].ToString().Replace(".", "")) - 1].Text, fnt))); //PdfPCell.FixedHeight = fixedHeight;

                                //PdfPCell.Width = 30;

                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthRight = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                { PdfPCell.BorderWidthBottom = borderSize; }
                                else
                                { PdfPCell.BorderWidthBottom = 0; }

                                PdfTable.AddCell(PdfPCell);

                                string tempArrayData2 = table.Rows[row].ItemArray[2].ToString();
                                while (tempArrayData2.IndexOf('<') >= 0 && tempArrayData2.IndexOf('>') >= 0)
                                {

                                    tempArrayData2 = tempArrayData2.Replace(tempArrayData2.Substring(tempArrayData2.IndexOf('<'), tempArrayData2.IndexOf('>') - tempArrayData2.IndexOf('<') + 1), "");
                                }

                                PdfPCell = new PdfPCell(new Phrase(new Chunk(tempArrayData2, fnt))); //PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = 0;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                { PdfPCell.BorderWidthBottom = borderSize; }
                                else
                                { PdfPCell.BorderWidthBottom = 0; }
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
                                if ((!string.IsNullOrEmpty(val_1.Trim())) || (!string.IsNullOrEmpty(val_2.Trim())) || (!string.IsNullOrEmpty(val_3.Trim())) || (!string.IsNullOrEmpty(val_4.Trim())) || (!string.IsNullOrEmpty(val_5.Trim())))
                                {
                                    /// record ที่มี ค่าเดียว ใน package
                                    if ((id_ == "13") || (id_ == "14") | (id_ == "15") || (id_ == "17") || (id_ == EndPack) || ((Convert.ToInt32(id_) > startDoc)))
                                    {
                                        if ((id_ == "20") || (id_ == EndPack))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 45f, 10f, 10f, 10f, 10f });
                                        }
                                        if (val_1 != "hasBorder")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

                                            if (id_ == EndPack)
                                            {
                                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                            }
                                        }
                                        //if (id_ == "16" || id_ == "17" || id_ == "18")
                                        //{
                                        //    PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        //}
                                        //else
                                        //{
                                        //    PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        //}
                                        //PdfPCell.Border = PdfPCell.NO_BORDER;
                                        PdfPCell.Colspan = 5;

                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;
                                        PdfPCell.BorderWidthBottom = 0;

                                        if ((id_ == EndPack) || (id_ == (startDoc + Convert.ToInt16(EndDoc) - 1).ToString()))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else if ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8") || (id_ == EndInfo) || (id_ == "19") || (id_ == "20") || (id_ == "21") || (id_ == "22") || (id_ == "23") || (id_ == "25") || (id_ == "26") || (id_ == "28") || (id_ == "29"))
                                    {
                                        if ((id_ == "19") || (id_ == "20") || (id_ == "21") || (id_ == "22") || (id_ == "23") || (id_ == "25") || (id_ == "26") || (id_ == "28") || (id_ == "29"))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }
                                        if ((id_ == EndInfo))
                                        {
                                            PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                        }

                                        if (id_ == "29")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfPCell.Colspan = 2;
                                        PdfTable.AddCell(PdfPCell);
                                        if (id_ == "29")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);

                                        if (id_ == "29")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.BorderWidthBottom = 0; }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 30f, 5f, 5f, 10f, 20f });
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[5].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[6].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        //PdfPCell.Border = PdfPCell.NO_BORDER;

                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthBottom = 0;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;

                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                }
                            }

                            document.Add(PdfTable);
                        }
                    }

                    #endregion Create PDF

                    document.Close();
                    byte[] content = myMemoryStream.ToArray();

                    //SET PDF PASSWORD
                    var pdfbyte = PdfSecurity.SetPasswordPdf(content, model.CustomerRegisterPanelModel.L_CARD_NO);
                    Session["PFDBYTE"] = pdfbyte;

                    //Write file to NAS
                    var pathfile = directoryPath + "\\" + fileName + ".pdf";
                    PdfSecurity.WriteFile(pathfile, pdfbyte);
                }

                return directoryTempPath + "\\" + fileName + ".pdf";
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }

        public string GeneratePDF_oldvas(QuickWinPanelModel model, string directoryPath,
           string directoryTempPath, string fileName, string pdfSignature, string pdfSignature2)
        {
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
                int numOntop = model.SummaryPanelModel.PDFPackageModel.PDF_L_ONTOPLIST.Count;

                int recurringContentExtend = 0;

                var displayType_ = model.CustomerRegisterPanelModel.CateType;
                if (displayType_ == "R")
                {
                    EndAddressSetup = "41";
                    EndAddressVat = "";
                    EndAddressBill = "49";
                    EndPack = "33";
                    EndInfo = "9";
                    startDoc = 52;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 36;
                    startAddressBill = 44;
                    startAddressVat = 0;
                }
                else  //// G/B
                {
                    EndAddressSetup = "39";
                    EndAddressVat = "56";
                    EndAddressBill = "48";
                    EndPack = "31";
                    EndInfo = "9";
                    startDoc = 58;
                    starInfo = 4;
                    startPack = 12;
                    startAddressSetup = 34;
                    startAddressBill = 43;
                    startAddressVat = 51;
                }

                #endregion setborder

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

                        #endregion กำหนดช่องว่างระหว่าง group

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

                            #endregion condition

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

                            #endregion document

                            #region address billing

                            else if (l.Name.Equals("L_BILLING_DETAIL"))// add detail
                            {
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                string coln1 = ""; string coln2 = ""; int currentcol = 1;
                                foreach (var x in ListEntity.Where(t => t.GroupByPDF.Equals("03D")))
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

                                        #endregion get value

                                        coln2 = x.DisplayValue;
                                        currentcol = 1;
                                    }
                                }
                                //if (model.CustomerRegisterPanelModel.DocType == "NON_RES")
                                //{
                                //    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                //}
                            }

                            #endregion address billing

                            #region address vat

                            else if ((l.Name.Equals("L_VAT_DETAIL")))// add detail
                            {
                                if (type != "R")
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                                    string coln1 = ""; string coln2 = ""; int currentcol = 1;
                                    foreach (var x in ListEntity.Where(t => t.GroupByPDF.Equals("03D")))
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

                                            #endregion get value

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

                            #endregion address vat

                            #region header group

                            else
                            {
                                table.Rows.Add(numberx, l.DisplayValue, " ", " ", " ", "", ""); numberx += 1;
                            }

                            #endregion header group
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
                                }
                                else
                                {
                                    table.Rows.Add(numberx, l.DisplayValue, "-", "", "", "", ""); numberx += 1;
                                }
                            }
                            else if ((l.Name.Equals("L_ONTOP_PACK")))//แพกเกจเสริม
                            {
                                var val = model.SummaryPanelModel.PDFPackageModel.PDF_L_ONTOPLIST;

                                if (val.Count > 0)
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
                                    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                }

                                if (val.Count == 1)
                                {
                                    table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                }
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL1")))//รายการที่ต้องชำระในวันที่ติดตั้ง
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if (l.Name.Equals("L_SUMM_DETAIL_WireBB"))
                            {
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
                            //else if ((l.Name.Equals("L_SUMM_DETAIL2")))//
                            //{
                            //    Value1 = "";
                            //    Value1 += l.DisplayValue;

                            //}
                            //else if ((l.Name.Equals("L_SUMM_DETAIL3")))//
                            //{
                            //    Value1 += l.DisplayValue;

                            //}
                            //else if ((l.Name.Equals("L_SUMM_DETAIL4")))//
                            //{
                            //    Value1 += l.DisplayValue;
                            //    table.Rows.Add(numberx, "", Value1, "", "", "", ""); numberx += 1;
                            //}
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
                            else if ((l.Name.Equals("L_SUM_INSTALL_RATE")))//
                            {
                                //var items_ = null;//model.SummaryPanelModel.PackageModel;
                                //Value1 = "";
                                //if (items_ != null)
                                //{
                                //    var l0 = items_.; if (l0 == null) { l0 = ""; }
                                //    var l1 = items_.INITIATION_CHARGE; if (l1 == null) { l1 = 0; }
                                //    var l2 = items_.DISCOUNT_INITIATION_ONTOP; if (l2 == null) { l2 = "0"; }
                                //    var l3 = items_.INITIATION_CHARGE_ONTOP; if (l3 == null) { l3 = "0"; }
                                //    var inOntop = (Convert.ToDecimal(l3) - Convert.ToDecimal(l2));
                                //    table.Rows.Add(numberx, "", l0, String.Format("{0:0,0}", l1 + Convert.ToDecimal(inOntop)), unit, "", ""); numberx += 1;
                                //}
                                //else
                                //{
                                var basephone = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_BASEPHONE;
                                table.Rows.Add(numberx, "", l.DisplayValue, basephone, unit, "", ""); numberx += 1;
                                //}
                            }
                            else if ((l.Name.Equals("L_SUM_DISCOUNT_INSTALL_RATE")))
                            {
                                var disbasephone = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNTBASEPHONE;
                                table.Rows.Add(numberx, "", l.DisplayValue, disbasephone, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL7")))//ลดค่าติดตั้ง
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
                            else if ((l.Name.Equals("L_SUM_MAINTEN_RATE")))//
                            {
                                var summaintain = model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_MAINTAINPHONE;

                                //Value1 = "";
                                //if (items_ != null)
                                //{
                                //    var l1 = items_.TECHNOLOGY; if (l1 == null) { l1 = ""; }

                                //    var l2 = items_.RECURRING_CHARGE; if (l2 == null) { l2 = 0; }
                                //    table.Rows.Add(numberx, "", l.DisplayValue + l1, String.Format("{0:0,0}", l2), unit, "", ""); numberx += 1;
                                //}
                                //else
                                //{
                                table.Rows.Add(numberx, "", l.DisplayValue, summaintain, unit, "", ""); numberx += 1;
                                //}
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
                            else if ((l.Name.Equals("L_SUMM_DETAIL15")))//
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

                            #endregion Package

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
                                            //Value1 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();
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

                                        #endregion address setup

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

                        #endregion set value to datatable
                    }
                }

                #endregion Format PDF

                #region Create PDF

                var pdfIndexTemp = GetTermCon();

                LovValueModel[] pdfIndex = pdfIndexTemp.ToArray();

                var borderSize = 0.7f;
                var borderCoolor = BaseColor.GREEN;
                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    var fixedHeight = 13f;

                    bool chkend2 = true; bool chkendDoc = false;
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, myMemoryStream);
                    //PdfWriter.GetInstance(document, new FileStream(Request.PhysicalApplicationPath + "\\Chap0101.pdf", FileMode.Create));
                    document.Open();
                    PdfPTable PdfTable = new PdfPTable(6);
                    PdfTable.TotalWidth = 415;

                    BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/fonts/tahoma.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // สร้าง Font จาก BaseFont
                    iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 6.7f);
                    iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6f);
                    iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 7f, Font.NORMAL, BaseColor.GREEN);
                    iTextSharp.text.Font fntbold = new iTextSharp.text.Font(bf, 6.7f, Font.BOLD);

                    PdfPCell PdfPCell = null;
                    for (int x = 1; x <= 2; x++)
                    {
                        if (x == 1)
                        {
                            table = new System.Data.DataTable();
                            table = table1;
                            document.NewPage();

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
                        }
                        else
                        {
                            table = new System.Data.DataTable(); table = table2;
                            if (displayType_ == "R")
                            {
                                document.NewPage();
                            }
                        }

                        //document.NewPage();

                        //BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/fonts/tahoma.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        //// สร้าง Font จาก BaseFont
                        //iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 7f);
                        //iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6.5f);
                        //iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 8f, Font.NORMAL, BaseColor.GREEN);

                        // Add PdfTable to pdfDoc
                        //document.Add(PdfTable);

                        ////start => Add image
                        // background
                        iTextSharp.text.Image pdfbg = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/BG_base1.jpg"));
                        //pdfbg.ScaleToFit(3400, 900);
                        pdfbg.ScaleToFit(3200, 850);
                        pdfbg.Alignment = iTextSharp.text.Image.UNDERLYING;
                        pdfbg.SetAbsolutePosition(0, 0);
                        document.Add(pdfbg);

                        // logo
                        iTextSharp.text.Image pdflogo;

                        // status
                        string imgStatus = "";
                        if (SiteSession.CurrentUICulture.IsThaiCulture())
                        { imgStatus = "~/Content/src/thai_5.png"; }
                        else
                        { imgStatus = "~/Content/src/eng_5.png"; }
                        iTextSharp.text.Image pdfstatus = iTextSharp.text.Image.GetInstance(Server.MapPath(imgStatus));
                        pdfstatus.ScaleToFit(300, 150);
                        pdfstatus.Alignment = iTextSharp.text.Image.UNDERLYING; pdfstatus.SetAbsolutePosition(80, 712);

                        if (model.PlugAndPlayFlow != "Y")
                        {
                            pdflogo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/src/img-logo.png"));
                            pdflogo.ScaleToFit(200, 100);
                            pdflogo.Alignment = iTextSharp.text.Image.UNDERLYING; pdflogo.SetAbsolutePosition(380, 712);
                            document.Add(pdflogo);
                            document.Add(pdfstatus);
                        }
                        else
                        {
                            pdflogo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/Header_appform.jpg"));
                            pdflogo.ScaleToFit(480, 120);
                            pdflogo.Alignment = iTextSharp.text.Image.UNDERLYING;
                            pdflogo.SetAbsolutePosition(58, 712);
                            document.Add(pdflogo);
                        }

                        iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                        iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                        // Write Data ที่ PdfCell ใน PdfTable
                        for (int row = 0; row < table.Rows.Count; row++)
                        {
                            PdfTable = new PdfPTable(table.Columns.Count - 1);
                            PdfTable.TotalWidth = 415;
                            //PdfTable.SetWidths(new float[] { 15f, 25f, 10f, 10f, 10f, 10f });
                            string id_ = table.Rows[row].ItemArray[0].ToString();

                            if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "SWiFi")
                            {
                                if (Convert.ToInt32(id_) == 59 && displayType_ != "R")
                                {
                                    document.NewPage();
                                    document.Add(pdfbg);
                                    document.Add(pdflogo);
                                    if (model.PlugAndPlayFlow != "Y")
                                        document.Add(pdfstatus);
                                    PdfTable = new PdfPTable(table.Columns.Count - 1);
                                    PdfTable.TotalWidth = 415;
                                    styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                                    hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                                    for (int row2 = 1; row2 < 6; row2++)
                                    {
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        PdfPCell.Border = PdfPCell.NO_BORDER;
                                        PdfPCell.Colspan = 6;
                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    document.Add(PdfTable);
                                }
                            }
                            else
                            {
                                if (Convert.ToInt32(id_) == 57 && displayType_ != "R")
                                {
                                    document.NewPage();
                                    document.Add(pdfbg);
                                    document.Add(pdflogo);
                                    if (model.PlugAndPlayFlow != "Y")
                                        document.Add(pdfstatus);
                                    PdfTable = new PdfPTable(table.Columns.Count - 1);
                                    PdfTable.TotalWidth = 415;
                                    styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                                    hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                                    for (int row2 = 1; row2 < 6; row2++)
                                    {
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(" ", fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        PdfPCell.Border = PdfPCell.NO_BORDER;
                                        PdfPCell.Colspan = 6;
                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    document.Add(PdfTable);
                                }
                            }

                            if (displayType_ == "R") /// space
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx" && (id_ == "17"))
                                {
                                    continue;
                                }

                                if ((id_ == "2") || (id_ == "3") ||
                                     (id_ == "10") || (id_ == "34") ||
                                     (id_ == "42") || (id_ == "50"))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx" && (id_ == "17"))
                                {
                                    continue;
                                }

                                if (model.CoveragePanelModel.PRODUCT_SUBTYPE == "SWiFi")
                                {
                                    if ((id_ == "2") || (id_ == "3") || (id_ == "10") || (id_ == "32") || (id_ == "40") || (id_ == "41") || (id_ == "49") ||
                                        (id_ == "57") || (id_ == "63") || (id_ == "64") || (id_ == "65") || (id_ == "66") || (id_ == "67") || (id_ == "68") || (id_ == "69") || (id_ == "70")
                                        || (id_ == "71") || (id_ == "72") || (id_ == "73"))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if ((id_ == "2") || (id_ == "3") || (id_ == "10") || (id_ == "32") || (id_ == "40") || (id_ == "41") || (id_ == "49") ||
                                       (id_ == "57") || (id_ == "63") || (id_ == "64") || (id_ == "65") || (id_ == "66") || (id_ == "67") || (id_ == "68") || (id_ == "69"))
                                    {
                                        continue;
                                    }
                                }
                            }

                            string val_0 = table.Rows[row].ItemArray[1].ToString();
                            string val_1 = table.Rows[row].ItemArray[2].ToString();
                            string val_2 = table.Rows[row].ItemArray[3].ToString();
                            string val_3 = table.Rows[row].ItemArray[4].ToString();
                            string val_4 = table.Rows[row].ItemArray[5].ToString();
                            string val_5 = table.Rows[row].ItemArray[6].ToString();

                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

                            if ((id_ == EndInfo) || (id_ == EndPack) || (id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
                            {
                                if (id_ == EndInfo)
                                {
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt1))); PdfPCell.FixedHeight = fixedHeight;
                                    PdfTable.SetWidths(new float[] { 15f, 15f, 0f, 15f, 15f, 0f });
                                }

                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = 0;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthBottom = borderSize;//(id_ == "16")(id_ == "23")
                            }
                            ////// record ที่เป็นหัวรายการของแต่ละอัน
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "16") || (id_ == "18") || (id_ == "25") || (id_ == "30")))
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
                            ///// ตัวแรกของ แต่ละกลุ่ม
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == starInfo.ToString()) || (id_ == startPack.ToString()) || (id_ == startAddressSetup.ToString()) || (id_ == startAddressBill.ToString()) || (id_ == startAddressVat.ToString()) || (id_ == startDoc.ToString())))
                            {
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fntgreen))); PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = borderSize;
                                PdfPCell.BorderWidthBottom = 0;
                            }
                            /// หัวตัวแรกสุด ข้อมูลผู้สมัคร
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "1")))
                            {
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8")))
                                {
                                    if (id_ == "8")
                                    {
                                        if (val_2.Split(',').Length > 1)
                                        {
                                            PdfPCell.FixedHeight = 20f;
                                        }
                                    }

                                    if (displayType_ == "R")
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 15f, 0f, 15f, 25f, 0f });
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

                            if ((x == 2) || ((string.IsNullOrEmpty(val_1.Trim())) && (string.IsNullOrEmpty(val_2.Trim())) && (string.IsNullOrEmpty(val_3.Trim()) && (string.IsNullOrEmpty(val_4.Trim())) && (string.IsNullOrEmpty(val_5.Trim())))))
                            {
                                if (string.IsNullOrEmpty(val_0.Trim()))
                                {
                                    if ((x == 1) && ((Convert.ToInt32(id_) > startDoc)))
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
                                    else { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                }
                                if ((!string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())) && (x == 2))
                                {
                                    if (chkend2)
                                    {
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(val_0.Trim(), fntgreen))); PdfPCell.FixedHeight = fixedHeight;
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
                                else if ((!string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())) && (x == 1) && ((Convert.ToInt32(id_) > startDoc)))
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
                                if ((string.IsNullOrEmpty(val_0.Trim())) && (string.IsNullOrEmpty(val_1.Trim())))
                                {
                                    PdfPCell.Border = PdfPCell.NO_BORDER;
                                }
                                if (id_ == "15")
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
                                PdfPCell.Colspan = 6;
                            }
                            else if (((Convert.ToInt32(id_) > startDoc)))
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

                            if ((!string.IsNullOrEmpty(val_0.Trim())) && (!string.IsNullOrEmpty(val_1.Trim())) && (x == 2))
                            {
                                PdfTable.SetWidths(new float[] { 3f, 37f, 10f, 10f, 10f, 10f });
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(pdfIndex[int.Parse(table.Rows[row].ItemArray[1].ToString().Replace(".", "")) - 1].Text, fnt))); //PdfPCell.FixedHeight = fixedHeight;

                                //PdfPCell.Width = 30;

                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderWidthLeft = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                PdfPCell.BorderWidthRight = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                { PdfPCell.BorderWidthBottom = borderSize; }
                                else
                                { PdfPCell.BorderWidthBottom = 0; }

                                PdfTable.AddCell(PdfPCell);

                                string tempArrayData2 = table.Rows[row].ItemArray[2].ToString();
                                while (tempArrayData2.IndexOf('<') >= 0 && tempArrayData2.IndexOf('>') >= 0)
                                {

                                    tempArrayData2 = tempArrayData2.Replace(tempArrayData2.Substring(tempArrayData2.IndexOf('<'), tempArrayData2.IndexOf('>') - tempArrayData2.IndexOf('<') + 1), "");
                                }

                                PdfPCell = new PdfPCell(new Phrase(new Chunk(tempArrayData2, fnt))); //PdfPCell.FixedHeight = fixedHeight;
                                PdfPCell.BorderColorBottom = borderCoolor;
                                PdfPCell.BorderColorLeft = borderCoolor;
                                PdfPCell.BorderColorRight = borderCoolor;
                                PdfPCell.BorderColorTop = borderCoolor;
                                PdfPCell.BorderWidthLeft = 0;
                                PdfPCell.BorderWidthRight = borderSize;
                                PdfPCell.BorderWidthTop = 0;
                                if (val_0.Trim().Replace(".", "").ToString() == EndCon.ToString())
                                { PdfPCell.BorderWidthBottom = borderSize; }
                                else
                                { PdfPCell.BorderWidthBottom = 0; }
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
                                if ((!string.IsNullOrEmpty(val_1.Trim())) || (!string.IsNullOrEmpty(val_2.Trim())) || (!string.IsNullOrEmpty(val_3.Trim())) || (!string.IsNullOrEmpty(val_4.Trim())) || (!string.IsNullOrEmpty(val_5.Trim())))
                                {
                                    /// record ที่มี ค่าเดียว ใน package
                                    if ((id_ == "13") || (id_ == "14") | (id_ == "15") || (id_ == "17") || (id_ == EndPack) || ((Convert.ToInt32(id_) > startDoc)))
                                    {
                                        if ((id_ == "17") || (id_ == EndPack))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 45f, 10f, 10f, 10f, 10f });
                                        }
                                        if (val_1 != "hasBorder")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

                                            if (id_ == EndPack)
                                            {
                                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                            }
                                        }
                                        //PdfPCell.Border = PdfPCell.NO_BORDER;
                                        PdfPCell.Colspan = 5;

                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;
                                        PdfPCell.BorderWidthBottom = 0;

                                        if ((id_ == EndPack) || (id_ == (startDoc + Convert.ToInt16(EndDoc) - 1).ToString()))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else if ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8") || (id_ == EndInfo) || (id_ == "19") || (id_ == "20") || (id_ == "21") || (id_ == "22") || (id_ == "23") || (id_ == "24") || (id_ == "26") || (id_ == "27") || (id_ == "28") || (id_ == "29") || (id_ == "30") || (id_ == "31") || (id_ == "32"))
                                    {
                                        if ((id_ == "19") || (id_ == "20") || (id_ == "21") || (id_ == "22") || (id_ == "23") || (id_ == "24") || (id_ == "25") || (id_ == "26") || (id_ == "27") || (id_ == "28") || (id_ == "29") || (id_ == "31") || (id_ == "32"))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }
                                        if ((id_ == EndInfo))
                                        {
                                            PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                        }

                                        if (id_ == "30")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfPCell.Colspan = 2;
                                        PdfTable.AddCell(PdfPCell);
                                        if (id_ == "30")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);

                                        if (id_ == "30")
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fntbold))); PdfPCell.FixedHeight = fixedHeight;
                                        }
                                        else
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                        { PdfPCell.BorderWidthBottom = 0; }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                    else
                                    {
                                        PdfTable.SetWidths(new float[] { 15f, 30f, 5f, 5f, 10f, 20f });
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[5].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
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
                                        { PdfPCell.Border = PdfPCell.NO_BORDER; }
                                        PdfTable.AddCell(PdfPCell);
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[6].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                                        //PdfPCell.Border = PdfPCell.NO_BORDER;

                                        PdfPCell.BorderColorTop = borderCoolor;
                                        PdfPCell.BorderColorLeft = borderCoolor;
                                        PdfPCell.BorderColorRight = borderCoolor;
                                        PdfPCell.BorderColorBottom = borderCoolor;
                                        PdfPCell.BorderWidthRight = borderSize;
                                        PdfPCell.BorderWidthBottom = 0;
                                        PdfPCell.BorderWidthLeft = 0;
                                        PdfPCell.BorderWidthTop = 0;

                                        if ((id_ == EndAddressSetup) || (id_ == EndAddressBill) || (id_ == EndAddressVat))
                                        {
                                            PdfPCell.BorderWidthBottom = borderSize;
                                        }

                                        PdfTable.AddCell(PdfPCell);
                                    }
                                }
                            }

                            document.Add(PdfTable);
                        }
                    }
                    if (model.PlugAndPlayFlow == "Y")
                    {

                        var listPlugAndPlay = GetPDFPlugAndPlay();

                        // สร้าง Font จาก BaseFont 
                        fntbold = new iTextSharp.text.Font(bf, 7f, Font.BOLD);
                        int j = 0;
                        for (int i = 0; i < 6; i++)
                        {
                            PdfPTable PlugAndPlaySignatureTable;
                            if (i == 1)
                            {
                                PlugAndPlaySignatureTable = new PdfPTable(11);
                                PlugAndPlaySignatureTable.SetWidths(new float[] { 5f, 10f, 15f, 5f, 10f, 15f, 5f, 10f, 15f, 5f, 10f });
                            }
                            else if (i == 2 || i == 3)
                            {
                                PlugAndPlaySignatureTable = new PdfPTable(3);
                                PlugAndPlaySignatureTable.SetWidths(new float[] { 5f, 5f, 95f });
                            }
                            else if (i == 5)
                            {
                                PlugAndPlaySignatureTable = new PdfPTable(7);
                                PlugAndPlaySignatureTable.SetWidths(new float[] { 5f, 15f, 10f, 10f, 5f, 15f, 10f });
                            }
                            else
                            {
                                if (i != 0)
                                {
                                    PlugAndPlaySignatureTable = new PdfPTable(2);
                                    PlugAndPlaySignatureTable.SetWidths(new float[] { 3f, 102f });
                                }
                                else
                                    PlugAndPlaySignatureTable = new PdfPTable(1);
                            }

                            PlugAndPlaySignatureTable.TotalWidth = 415;

                            PdfPCell SignatureCell;

                            if (i == 1)
                            {
                                iTextSharp.text.Image chk_box1 =
                                iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/chk_box1.jpg"));
                                chk_box1.ScaleToFit(18, 20);
                                SignatureCell = new PdfPCell(chk_box1);
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);
                                PlugAndPlaySignatureTable.WriteSelectedRows(1, 1, 1, 1, writer.DirectContent);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk("", fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                iTextSharp.text.Image chk_box2 =
                                iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/chk_box2.jpg"));
                                chk_box2.ScaleToFit(18, 20);
                                SignatureCell = new PdfPCell(chk_box2);
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);
                                PlugAndPlaySignatureTable.WriteSelectedRows(1, 1, 1, 1, writer.DirectContent);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk("", fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                iTextSharp.text.Image chk_box3 =
                                iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/chk_box2.jpg"));
                                chk_box3.ScaleToFit(18, 20);
                                SignatureCell = new PdfPCell(chk_box3);
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk("", fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                iTextSharp.text.Image chk_box4 =
                                iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/chk_box2.jpg"));
                                chk_box4.ScaleToFit(18, 20);
                                SignatureCell = new PdfPCell(chk_box4);
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);
                            }
                            else if (i == 2 || i == 3)
                            {
                                SignatureCell = new PdfPCell(new Phrase(new Chunk("", fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                iTextSharp.text.Image chk_box5;
                                if (i == 2)
                                    chk_box5 = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/chk_box3.jpg"));
                                else
                                    chk_box5 = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/chk_box4.jpg"));
                                chk_box5.ScaleToFit(18, 20);
                                chk_box5.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
                                SignatureCell = new PdfPCell(chk_box5);
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fnt)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);
                            }
                            else if (i == 5)
                            {

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fnt)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk("", fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fnt)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk("", fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fnt)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk("", fntbold)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j].DisplayValue, fnt)));
                                SignatureCell.Colspan = 1;
                                SignatureCell.Border = 0;
                                PlugAndPlaySignatureTable.AddCell(SignatureCell);

                            }
                            else
                            {
                                if (i != 0)
                                {
                                    SignatureCell = new PdfPCell(new Phrase(new Chunk("", fnt)));
                                    SignatureCell.Colspan = 1;
                                    SignatureCell.Border = 0;
                                    PlugAndPlaySignatureTable.AddCell(SignatureCell);

                                    SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fnt)));
                                    SignatureCell.Colspan = 1;
                                    SignatureCell.Border = 0;
                                    PlugAndPlaySignatureTable.AddCell(SignatureCell);
                                }
                                else
                                {
                                    SignatureCell = new PdfPCell(new Phrase(new Chunk(listPlugAndPlay[j++].DisplayValue, fntbold)));
                                    SignatureCell.Colspan = 1;
                                    SignatureCell.Border = 0;
                                    PlugAndPlaySignatureTable.AddCell(SignatureCell);
                                }

                            }

                            //PlugAndPlaySignatureTable.WriteSelectedRows(1, 1, 1, 1, writer.DirectContent);

                            document.Add(PlugAndPlaySignatureTable);
                        }

                        //iTextSharp.text.Image SignaturePic;

                        //if (pdfSignature != null)
                        //{
                        //    int pos = pdfSignature.IndexOf("base64,");
                        //    byte[] newBytes = Convert.FromBase64String(pdfSignature.Substring(pos + 7));
                        //    SignaturePic = iTextSharp.text.Image.GetInstance(newBytes);
                        //}
                        //else
                        //{
                        //    SignaturePic = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Images/bottom_appform.jpg"));
                        //}
                        //SignaturePic.ScaleToFit(450, 400);
                        //SignaturePic.Alignment = iTextSharp.text.Image.UNDERLYING;

                        //PdfPCell SignatureCell = new PdfPCell(SignaturePic);
                        //SignatureCell.Border = 0;
                        //PlugAndPlaySignatureTable.AddCell(SignatureCell);
                        //PlugAndPlaySignatureTable.WriteSelectedRows(1, 1, 1, 1, writer.DirectContent);
                    }

                    #endregion Create PDF

                    document.Close();
                    byte[] content = myMemoryStream.ToArray();

                    var pdfbyte = content;
                    //SET PDF PASSWORD
                    if (model.PlugAndPlayFlow != "Y")
                        pdfbyte = PdfSecurity.SetPasswordPdf(content, model.CustomerRegisterPanelModel.L_CARD_NO);
                    Session["PFDBYTE"] = pdfbyte;

                    if (model.PlugAndPlayFlow == "Y")
                    {
                        var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate").SingleOrDefault();
                        var imagepathimer = @ImpersonateVar.LovValue4;
                        string user = ImpersonateVar.LovValue1;
                        string pass = ImpersonateVar.LovValue2;
                        string ip = ImpersonateVar.LovValue3;
                        string yearweek = (DateTime.Now.Year.ToString());
                        string monthyear = (DateTime.Now.Month.ToString("00"));

                        var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));

                        imagepathimer = imagepathimerTemp;

                        Logger.Info("Start Impersonate:");

                        using (var impersonator = new Impersonator(user, ip, pass, false))
                        {
                            var pathfileImpesontae = imagepathimer + "\\" + fileName + ".pdf";
                            PdfSecurity.WriteFile(pathfileImpesontae, pdfbyte);
                            UpdateFileName(model.CustomerRegisterPanelModel.OrderNo, pathfileImpesontae, model.CoveragePanelModel.L_CONTACT_PHONE);
                        }
                    }

                    //Write file to NAS
                    var pathfile = directoryPath + "\\" + fileName + ".pdf";
                    PdfSecurity.WriteFile(pathfile, pdfbyte);
                }

                return directoryTempPath + "\\" + fileName + ".pdf";
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }
    }
}