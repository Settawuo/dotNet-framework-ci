using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.CommandHandlers
{
    public class GeneratePdfCommandHandler : ICommandHandler<GeneratePdfCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GeneratePdfCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _lov = lov;
        }

        public void Handle(GeneratePdfCommand command)
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

                var displayType_ = command.Model.CustomerRegisterPanelModel.CateType;
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

                var lovData = (from t in _lov.Get() select t);

                var config = lovData.Where(l => l.LOV_VAL4 != null)
                                    .OrderBy(l => l.ORDER_BY)
                                    .ThenBy(n => n.LOV_ID).ToList();

                var screenValue = new List<LovScreenValueModel>();

                if (command.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.LOV_NAME,
                        PageCode = l.LOV_VAL5,
                        DisplayValue = l.LOV_VAL1,
                        GroupByPDF = l.LOV_VAL4,
                        OrderByPDF = l.ORDER_BY,
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.LOV_NAME,
                        PageCode = l.LOV_VAL5,
                        DisplayValue = l.LOV_VAL2,
                        GroupByPDF = l.LOV_VAL4,
                        OrderByPDF = l.ORDER_BY,
                    }).ToList();
                }

                var ListEntity = screenValue.Where(l => l.GroupByPDF.Contains("0")).ToList();

                string item = "";

                var items = lovData.Where(x1 => x1.LOV_TYPE.Equals("SCREEN") && x1.LOV_NAME.Equals("H_FBB004"));
                if (command.CurrentUICulture.IsThaiCulture())
                { item = items.Select(n => n.LOV_VAL1).FirstOrDefault(); }
                else
                { item = items.Select(n => n.LOV_VAL2).FirstOrDefault(); }

                table.Rows.Add(numberx, item, " ", " ", " ", "", ""); numberx += 1;
                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;
                table.Rows.Add(numberx, " ", " ", " ", " ", "", ""); numberx += 1;

                var chkColumn = true;
                var column1 = "";
                var column2 = "";
                var name1 = "";
                var Value1 = "";
                var Value2 = "";
                var type = "";
                var groupPDF = "";
                var currenttable = 1;

                // set CateType
                var type_ = command.Model.CustomerRegisterPanelModel.CateType;
                if (type_ != null)
                {
                    type = type_;

                    var units = lovData.Where(n => !string.IsNullOrEmpty(n.LOV_NAME) && n.LOV_NAME.Equals("L_BAHT"));
                    string unit = "";

                    if (command.CurrentUICulture.IsThaiCulture())
                        unit = units.Select(n => n.LOV_VAL1).FirstOrDefault();
                    else
                        unit = units.Select(n => n.LOV_VAL2).FirstOrDefault();


                    foreach (var l in ListEntity)
                    {
                        _logger.Info("DisplayValue" + l.DisplayValue);
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
                                if (command.Model.CoveragePanelModel.PRODUCT_SUBTYPE.ToSafeString().Trim() == "SWiFi")
                                {
                                    ProSubType = "TERM_AND_CONDITION_WIRELESS";
                                }
                                else
                                {
                                    ProSubType = "TERM_AND_CONDITION_WIRE";
                                }

                                var data = lovData.Where(z => z.LOV_TYPE.Equals(ProSubType));
                                int num = 1;
                                if (command.CurrentUICulture.IsThaiCulture())
                                {
                                    foreach (var x in data.ToList())
                                    {
                                        // table.Rows.Add(numberx, num.ToString() + ". " + x.LovValue1, " ", " ", " ", "", ""); num++; numberx += 1;
                                        table.Rows.Add(numberx, num.ToString() + ". ", x.LOV_VAL1, " ", " ", "", ""); num++; numberx += 1;
                                    }
                                }
                                else
                                {
                                    foreach (var x in data.ToList())
                                    {
                                        //table.Rows.Add(numberx, num.ToString() + ". " + x.LovValue2, " ", " ", " ", "", ""); num++; numberx += 1;
                                        table.Rows.Add(numberx, num.ToString() + ". ", x.LOV_VAL2, " ", " ", "", ""); num++; numberx += 1;
                                    }
                                }

                                EndCon = data.ToList().Count().ToString();
                            }
                            #endregion

                            #region document
                            else if (l.Name.Equals("L_DOCUMENT"))// add detail
                            {

                                var data = lovData.Where(n => n.LOV_TYPE.Equals("DOCUMENT") && n.LOV_NAME.Equals(command.Model.CustomerRegisterPanelModel.DocType));
                                if (command.CurrentUICulture.IsThaiCulture())
                                { val = data.Select(n => n.LOV_VAL1).FirstOrDefault(); }
                                else
                                { val = data.Select(n => n.LOV_VAL2).FirstOrDefault(); }

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
                                        var Value3 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
                                        if (Value3.ToSafeString() == "") Value3 = "-";
                                        table.Rows.Add(numberx, coln1, Value1, coln2, Value2, x.DisplayValue, Value3); numberx += 1;
                                        currentcol = 1;
                                    }
                                    else if (x.Name.Equals("L_ROAD") || x.Name.Equals("L_AMPHUR") || x.Name.Equals("L_ZIPCODE") || x.Name.Equals("ZIPCODE_ID"))
                                    {
                                        if (x.Name.Equals("ZIPCODE_ID")) x.Name = "L_ZIPCODE";
                                        Value1 = "";
                                        Value1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, name1);
                                        Value2 = "";
                                        Value2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
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
                                        Value1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, name1);

                                        Value2 = "";
                                        Value2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSendDoc, x.Name);
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
                                            var Value3 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
                                            if (Value3.ToSafeString() == "") Value3 = "-";
                                            table.Rows.Add(numberx, coln1, Value1, coln2, Value2, x.DisplayValue, Value3); numberx += 1;
                                            currentcol = 1;
                                        }
                                        else if (x.Name.Equals("L_ROAD") || x.Name.Equals("L_AMPHUR") || x.Name.Equals("L_ZIPCODE") || x.Name.Equals("ZIPCODE_ID"))
                                        {
                                            if (x.Name.Equals("ZIPCODE_ID")) x.Name = "L_ZIPCODE";
                                            Value1 = "";
                                            Value1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelVat, name1);
                                            Value2 = "";
                                            Value2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
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
                                            Value1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelVat, name1);

                                            Value2 = "";
                                            Value2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelVat, x.Name);
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

                            else if (l.Name.Equals("L_MAIN_PACK"))//แพกเกจหลัก
                            {
                                var val = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_MAIN_PACKAGE;
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
                                var val = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_ONTOPLIST;

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
                                if (command.Model.CoveragePanelModel.PRODUCT_SUBTYPE == "WireBB")
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
                                if (command.Model.CoveragePanelModel.PRODUCT_SUBTYPE == "SWiFi")
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
                                if (command.Model.CoveragePanelModel.PRODUCT_SUBTYPE == "FTTx")
                                {
                                    table.Rows.Add(numberx, "", l.DisplayValue, "", "", "", ""); numberx += 1;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL5")))//รายการที่ต้องชำระครั้งเดียวในรอบบิลแรก
                            {
                                Value1 = "";
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL6")))//
                            {
                                var tidtun = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_LABEL_TIDTUN;
                                var tidtunval = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_TIDTUN;

                                table.Rows.Add(numberx, "", tidtun, tidtunval, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL7")))//
                            {
                                var discount = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_DISCOUNT;
                                table.Rows.Add(numberx, "", l.DisplayValue, discount, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL8")))//
                            {
                                var sum1 = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM1;

                                table.Rows.Add(numberx, "", l.DisplayValue, sum1, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL9")))//รายการที่ต้องชำระรายเดือน
                            {
                                table.Rows.Add(numberx, l.DisplayValue, "", "", "", "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL10")))//
                            {
                                var pakagename = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_LABEL_PAKAGENAME;
                                var pakagenameval = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_PAKAGE;

                                table.Rows.Add(numberx, "", l.DisplayValue + " " + pakagename, pakagenameval, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL11")))//
                            {
                                var sum2 = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_SUM2;
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
                                var val4day = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_VALUE_DETAIL_4DAY;
                                table.Rows.Add(numberx, "", l.DisplayValue, val4day, unit, "", ""); numberx += 1;
                            }
                            else if ((l.Name.Equals("L_SUMM_DETAIL15")))
                            {
                                var firsbillsum = command.Model.SummaryPanelModel.PDFPackageModel.PDF_L_LABEL_DETAIL_ALLFIRSTBILL;
                                table.Rows.Add(numberx, "", l.DisplayValue, firsbillsum, unit, "", ""); numberx += 1;
                            }
                            else if (l.Name.Equals("L_SUM_MAINTEN_RATE") || l.Name.Equals("L_SUM_INSTALL_RATE"))
                            {
                                continue;
                            }

                            #endregion

                            else
                            {
                                if (chkColumn == true && l.Name.Equals("L_INSTALLATION_DATE") && (type.ToUpper().Trim() == "B" || type.ToUpper().Trim() == "G"))
                                {
                                    var installdateb = command.Model.CustomerRegisterPanelModel.L_INSTALL_DATE;
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
                                            var val1 = command.Model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            var val2 = command.Model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            var val3 = command.Model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
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
                                            var a1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                            var a2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
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
                                            Value2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
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
                                            Value1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            if (Value1.ToSafeString() == "") Value1 = "-";
                                            continue;
                                        }
                                        else if (l.Name.Equals("L_ROAD") || l.Name.Equals("L_AMPHUR") || l.Name.Equals("L_ZIPCODE") || l.Name.Equals("ZIPCODE_ID"))
                                        {
                                            if (l.Name.Equals("ZIPCODE_ID")) l.Name = "L_ZIPCODE";
                                            var a1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                            var a2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            if (a1.ToSafeString() == "") a1 = "-";
                                            if (a2.ToSafeString() == "") a2 = "-";
                                            table.Rows.Add(numberx, column1, a1, "", "", l.DisplayValue, a2); numberx += 1;

                                            if ((l.Name.Equals("L_ZIPCODE")) && (command.Model.CustomerRegisterPanelModel.DocType == "NON_RES"))
                                            {
                                                table.Rows.Add(numberx, "", "", "", "", "", ""); numberx += 1;
                                            }
                                            continue;
                                        }

                                        #endregion

                                        column2 = l.DisplayValue;

                                        Value1 = "";
                                        Value1 = GetModelValue(command.Model.CoveragePanelModel, name1);
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(command.Model.CustomerRegisterPanelModel, name1);
                                        }
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(command.Model.DisplayPackagePanelModel, name1);
                                        }
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(command.Model.SummaryPanelModel, name1);
                                        }
                                        if (string.IsNullOrEmpty(Value1))
                                        {
                                            Value1 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, name1);
                                        }

                                        if (Value1.ToSafeString() == "" && name1 != "") Value1 = "-";

                                        if (name1.Equals("L_CONTACT_PHONE"))
                                        {
                                            var val1 = command.Model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            var val2 = command.Model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            var val3 = command.Model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            Value1 = val1.ToString();
                                            if ((Value1.Trim() != "") && (val2.ToString().Trim() != "")) { Value1 += ", "; }
                                            Value1 += val2.ToString();
                                            if ((Value1.Trim() != "") && (val3.ToString().Trim() != "")) { Value1 += ", "; }
                                            Value1 += val3.ToString();
                                        }

                                        Value2 = "";

                                        if ((l.Name.Equals("L_NAME")))
                                        {
                                            var val1 = command.Model.CustomerRegisterPanelModel.L_TITLE; if (val1 == null) { val1 = ""; }
                                            var val2 = command.Model.CustomerRegisterPanelModel.L_FIRST_NAME; if (val2 == null) { val2 = ""; }
                                            var val3 = command.Model.CustomerRegisterPanelModel.L_LAST_NAME; if (val3 == null) { val3 = ""; }

                                            Value2 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();

                                        }
                                        else if ((l.Name.Equals("L_GOVERNMENT_NAME")))
                                        {
                                            var val1 = command.Model.CustomerRegisterPanelModel.L_TITLE; if (val1 == null) { val1 = ""; }
                                            var val2 = command.Model.CustomerRegisterPanelModel.L_GOVERNMENT_NAME; if (val2 == null) { val2 = ""; }

                                            Value2 = val1.ToString() + " " + val2.ToString();

                                        }

                                        else if (l.Name.Equals("L_CONTACT_PHONE"))
                                        {
                                            var val1 = command.Model.CustomerRegisterPanelModel.L_HOME_PHONE; if (val1 == null) { val1 = ""; }
                                            var val2 = command.Model.CustomerRegisterPanelModel.L_MOBILE; if (val2 == null) { val2 = ""; }
                                            var val3 = command.Model.CustomerRegisterPanelModel.L_OR; if (val3 == null) { val3 = ""; }
                                            Value2 = val1.ToString() + " " + val2.ToString() + " " + val3.ToString();
                                            Value2 = val1.ToString();
                                            if ((Value2.Trim() != "") && (val2.ToString().Trim() != "")) { Value2 += ", "; }
                                            Value2 += val2.ToString();
                                            if ((Value2.Trim() != "") && (val3.ToString().Trim() != "")) { Value2 += ", "; }
                                            Value2 += val3.ToString();
                                        }

                                        else
                                        {
                                            Value2 = GetModelValue(command.Model.CoveragePanelModel, l.Name);
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(command.Model.CustomerRegisterPanelModel, l.Name);
                                            }
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(command.Model.DisplayPackagePanelModel, l.Name);
                                            }
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(command.Model.SummaryPanelModel, l.Name);
                                            }
                                            if (string.IsNullOrEmpty(Value2))
                                            {
                                                Value2 = GetModelValue(command.Model.CustomerRegisterPanelModel.AddressPanelModelSetup, l.Name);
                                            }
                                        }

                                        if (Value2.ToSafeString() == "") Value2 = "-";
                                        if (l.Name.Equals("L_INSTALLATION_DATE"))
                                        {
                                            Value2 = command.Model.CustomerRegisterPanelModel.L_INSTALL_DATE;
                                            if (Value2.ToSafeString() == "") Value2 = "-";
                                        }
                                        //if (l.Name.Equals("L_EMAIL"))
                                        //{
                                        //    Value2 = model.CustomerRegisterPanelModel.L_EMAIL;
                                        //    if (Value2.ToSafeString() == "") Value2 = "-";
                                        //}
                                        //else if (l.Name.Equals("L_EMAIL"))
                                        //{
                                        //    Value2 = model.CustomerRegisterPanelModel.L_EMAIL;
                                        //}


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
                #endregion

                #region Create PDF

                var borderSize = 0.7f;
                var borderCoolor = BaseColor.GREEN;
                using (MemoryStream myMemoryStream = new MemoryStream())
                {

                    var fixedHeight = 12.5f;

                    bool chkend2 = true; bool chkendDoc = false;
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(document, myMemoryStream);
                    //PdfWriter.GetInstance(document, new FileStream(Request.PhysicalApplicationPath + "\\Chap0101.pdf", FileMode.Create));
                    document.Open();

                    for (int x = 1; x <= 2; x++)
                    {
                        if (x == 1)
                        {
                            table = new System.Data.DataTable();
                            table = table1;
                        }
                        else
                        {
                            table = new System.Data.DataTable(); table = table2;
                        }

                        document.NewPage();

                        BaseFont bf = BaseFont.CreateFont(Path.GetFullPath(@command.FontFolderPath + @"tahoma.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        // สร้าง Font จาก BaseFont 
                        iTextSharp.text.Font fnt = new iTextSharp.text.Font(bf, 6.7f);
                        iTextSharp.text.Font fnt1 = new iTextSharp.text.Font(bf, 6f);
                        iTextSharp.text.Font fntgreen = new iTextSharp.text.Font(bf, 7f, Font.NORMAL, BaseColor.GREEN);

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
                            //PdfTable.SetWidths(new float[] { 15f, 25f, 10f, 10f, 10f, 10f });
                            string id_ = table.Rows[row].ItemArray[0].ToString();

                            if (displayType_ == "R")
                            {
                                if ((id_ == "2") || (id_ == "3") || (id_ == "28") || (id_ == "36") ||
                                     (id_ == "10") || (id_ == "45") ||
                                     (id_ == "44") || (id_ == "46"))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((id_ == "2") || (id_ == "10") || (id_ == "28") || (id_ == "36") || (id_ == "45") || (id_ == "53") ||
                                    (id_ == "3") || (id_ == "37")
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

                            PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
                            if (id_ == "8")
                            {
                                if (val_3.Split(',').Length > 1) { PdfPCell.FixedHeight = 20f; }
                            }

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
                            }
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "15") || (id_ == "17") || (id_ == "21") || (id_ == "24")))
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
                            else if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "1")))
                            {
                                PdfPCell.Border = PdfPCell.NO_BORDER;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(val_0.Trim()) && ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8")))
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
                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[1].ToString(), fnt))); //PdfPCell.FixedHeight = fixedHeight;

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

                                PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); //PdfPCell.FixedHeight = fixedHeight;
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
                                    if ((id_ == "13") || (id_ == "14") || (id_ == "16") || (id_ == EndPack) || ((Convert.ToInt32(id_) > startDoc)))
                                    {
                                        if ((id_ == "16") || (id_ == EndPack))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }

                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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
                                    else if ((id_ == "5") || (id_ == "6") || (id_ == "7") || (id_ == "8") || (id_ == EndInfo) || (id_ == "17") || (id_ == "18") || (id_ == "19") || (id_ == "20") || (id_ == "22") || (id_ == "23") || (id_ == "25") || (id_ == "26") || (id_ == "27"))
                                    {

                                        if ((id_ == "17") || (id_ == "18") || (id_ == "19") || (id_ == "20") || (id_ == "22") || (id_ == "23") || (id_ == "25") || (id_ == "26") || (id_ == "27"))
                                        {
                                            PdfTable.SetWidths(new float[] { 5f, 35f, 10f, 10f, 10f, 10f });
                                        }

                                        if (id_ == EndInfo)
                                        {
                                            PdfTable.SetWidths(new float[] { 15f, 25f, 0f, 15f, 15f, 0f });
                                        }

                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[2].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

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
                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[3].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;

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

                                        PdfPCell = new PdfPCell(new Phrase(new Chunk(table.Rows[row].ItemArray[4].ToString(), fnt))); PdfPCell.FixedHeight = fixedHeight;
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

                        // Add PdfTable to pdfDoc
                        //document.Add(PdfTable);

                        ////start => Add image
                        // background
                        iTextSharp.text.Image pdfbg = iTextSharp.text.Image.GetInstance(@command.ImageFolderPath + @"BG_base1.jpg");
                        //pdfbg.ScaleToFit(3400, 900);
                        pdfbg.ScaleToFit(3200, 850);
                        pdfbg.Alignment = iTextSharp.text.Image.UNDERLYING;
                        pdfbg.SetAbsolutePosition(0, 0);
                        document.Add(pdfbg);

                        // logo
                        iTextSharp.text.Image pdflogo = iTextSharp.text.Image.GetInstance(@command.ImageFolderPath + @"img-logo.png");
                        pdflogo.ScaleToFit(200, 100);
                        pdflogo.Alignment = iTextSharp.text.Image.UNDERLYING; pdflogo.SetAbsolutePosition(380, 712);
                        document.Add(pdflogo);
                        // status
                        string imgStatus = "";
                        if (command.CurrentUICulture.IsThaiCulture())
                        { imgStatus = @command.ImageFolderPath + @"status-4.png"; }
                        else
                        { imgStatus = @command.ImageFolderPath + @"status-4-eng.png"; }

                        iTextSharp.text.Image pdfstatus = iTextSharp.text.Image.GetInstance(@imgStatus);

                        pdfstatus.ScaleToFit(300, 150);
                        pdfstatus.Alignment = iTextSharp.text.Image.UNDERLYING; pdfstatus.SetAbsolutePosition(80, 712);
                        document.Add(pdfstatus);

                        iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                        iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
                        //hw.Parse(new StringReader(HTML));

                    }

                    #endregion

                    document.Close();
                    byte[] content = myMemoryStream.ToArray();
                    //Session["PFDBYTE"] = content;

                    using (FileStream f = new FileStream(command.DirectoryPath + "\\" + command.FileName + ".pdf", System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        f.Write(content, 0, content.Length);
                        f.Close();
                    }


                    System.IO.FileStream _FileStream = new System.IO.FileStream(command.DirectoryPath
                                                                                    + "\\" + command.FileName
                                                                                    + ".pdf", System.IO.FileMode.Create, System.IO.FileAccess.Write);

                    _FileStream.Write(content, 0, content.Length);
                    _FileStream.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }

            //return directoryTempPath + "\\" + fileName + ".pdf";
        }

        private string GetModelValue(object model, string propertyName)
        {

            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                if (propertyInfo.Name.Equals(propertyName))
                {
                    var value_ = propertyInfo.GetValue(model, null);
                    return value_.ToSafeString();
                }
            }

            return string.Empty;
        }
    }
}
