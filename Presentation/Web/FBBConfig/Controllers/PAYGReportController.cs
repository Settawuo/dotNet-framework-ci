using Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
//using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class PAYGReportController : FBBConfigController
    {
        //
        // GET: /PAYGReport/

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendMailLastMileNotificationCommand> _sendMail;
        private readonly ICommandHandler<UpdatePaidStatusDataCommand> _UpdatePaidStatusCommand;
        private readonly ICommandHandler<ImportPaidStatusCommand> _ImportPaidStatusCommand;
        private readonly ICommandHandler<UpdateOLTStatusCommand> _UpdateOLTStatusCommand;

        private string rptName = string.Empty;
        private string rptCriteria = string.Empty;
        private string rptDate = string.Empty;
        private List<string> rptCriterias = new List<string>();
        private bool showInDoor = true;
        private bool showOutDoor = true;
        private bool showONT = true;

        static List<DetailLastmileAndCPEReportList> result = new List<DetailLastmileAndCPEReportList>();
        static string flagAfter = "";
        static string activeflag = "";
        static LastmileAndCPEReportModel searchModel = new LastmileAndCPEReportModel();

        public PAYGReportController(ILogger logger,
              IQueryProcessor queryProcessor,
              ICommandHandler<UpdatePaidStatusDataCommand> updatePaidStatusCommand,
              ICommandHandler<ImportPaidStatusCommand> importPaidStatusCommand,
              ICommandHandler<SendMailLastMileNotificationCommand> sendMail,
              ICommandHandler<UpdateOLTStatusCommand> updateOLTStatusCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _UpdatePaidStatusCommand = updatePaidStatusCommand;
            _ImportPaidStatusCommand = importPaidStatusCommand;
            _sendMail = sendMail;
            _UpdateOLTStatusCommand = updateOLTStatusCommand;

        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /PAYGReport/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /PAYGReport/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PAYGReport/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /PAYGReport/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /PAYGReport/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /PAYGReport/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /PAYGReport/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private void SetViewBagLovV2(string screenType, string LovValue5)
        {
            var query = new GetLovV2Query()
            {
                LovType = screenType,
                LovVal5 = LovValue5

            };
            var LovDataScreen = _queryProcessor.Execute(query).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        public void SetColumnForExcel(string lovVal5, ref DataTable table, ref List<string> groupHeaderList)
        {
            string[] groupHeader = null;
            string[] colHeader = null;
            string[] typeCol = null;

            int index = 0;
            var colHeaderList = new List<string>();
            List<LovValueModel> tmpVal;
            var colVal = string.Empty;
            var dupVal = string.Empty;

            var lovDataScreen = base.LovData.Where(p => p.Type == "FBBPAYG_SCREEN" && p.LovValue5 == lovVal5 && p.Name.Contains("R_HEADER_"))
                                .OrderBy(p => p.OrderBy).ToList();

            switch (lovVal5)
            {
                case "SUMLASTMILE":

                    groupHeader = new string[] { "R_HEADER_no_of_reg", "R_HEADER_paid_last", "R_HEADER_notpaid_last", "R_HEADER_rem_last", "R_HEADER_product", "R_HEADER_addresid" };

                    colHeader = new string[] { "R_HEADER_olt_vendor", "R_HEADER_phase", "R_HEADER_region", "R_HEADER_district", "R_HEADER_sub_district",
                                               "R_HEADER_olt_name", "R_HEADER_register_dt", "R_HEADER_approve", "R_HEADER_tob_approve", "R_HEADER_indoor",
                                               "R_HEADER_outdoor", "R_HEADER_indoor", "R_HEADER_outdoor", "R_HEADER_indoor", "R_HEADER_outdoor",
                                               "R_HEADER_paid_ont", "R_HEADER_notpaid_pnt", "R_HEADER_rem_ont"};

                    typeCol = new string[] { "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.DateTime", "System.Int32", "System.Int32", "System.Int32",
                                             "System.Int32", "System.Int32", "System.Int32", "System.Int32", "System.Int32",
                                             "System.Int32", "System.Int32", "System.Int32" };

                    break;
                case "DETAILLASTMILE":
                    //R_HEADER_paid_last_indoor
                    groupHeader = new string[] { "R_HEADER_paid_last_indoor", "R_HEADER_paid_last_outdoor", "R_HEADER_paid_ont", "R_HEADER_product", "R_HEADER_addresid" };

                    colHeader = new string[] { "R_HEADER_customer_req_dt", "R_HEADER_installation_Complete_dt", "R_HEADER_cs_approve_dt",
                                               "R_HEADER_new_reg_status", "R_HEADER_Internet_no", "R_HEADER_cust_name", "R_HEADER_cust_status",
                                               "R_HEADER_cust_status_dt", "R_HEADER_package_name", "R_HEADER_Change_Package_dt", "R_HEADER_address_id", "R_HEADER_building_name",
                                               "R_HEADER_business_model", "R_HEADER_product", "R_HEADER_service", "R_HEADER_Sub_contract_name_ins", "R_HEADER_region",
                                               "R_HEADER_phase", "R_HEADER_olt_vendor", "R_HEADER_olt_name", "R_HEADER_district", "R_HEADER_sub_district",
                                               "R_HEADER_site_name", "R_HEADER_site_no", "R_HEADER_spl1", "R_HEADER_spl2", "R_HEADER_invoice",
                                               "R_HEADER_invoice_dt", "R_HEADER_po", "R_HEADER_status", "R_HEADER_invoice", "R_HEADER_invoice_dt",
                                               "R_HEADER_po", "R_HEADER_status", "R_HEADER_subcont_indoor", 
                                               //"R_HEADER_ont_vendor", "R_HEADER_ont_model","R_HEADER_ont_serial", 
                                               "R_HEADER_ont_CPE_SN","R_HEADER_ont_Wifi_Router_SN","R_HEADER_ont_ATA_SN","R_HEADER_ont_STB_SN1","R_HEADER_ont_STB_SN2","R_HEADER_ont_STB_SN3",
                                               "R_HEADER_invoice", "R_HEADER_invoice_dt", "R_HEADER_po", "R_HEADER_status","CNT"};

                    typeCol = new string[] { "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", 
                                             //"System.String", "System.String", "System.String", 
                                             "System.String", "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String"};

                    break;
                case "PAIDSTATUS":

                    groupHeader = new string[] { "R_HEADER_paid_last_in", "R_HEADER_paid_last_out", "R_HEADER_paid_ont" };

                    colHeader = new string[] { "R_HEADER_register_dt", "R_HEADER_new_resg_status", "R_HEADER_internet_no", "R_HEADER_cust_name", "R_HEADER_phase",
                                               "R_HEADER_olt_vendor", "R_HEADER_district", "R_HEADER_sub_district", "R_HEADER_paid_last_in", "R_HEADER_paid_last_out",
                                               "R_HEADER_invoice", "R_HEADER_invoice_dt", "R_HEADER_po", "R_HEADER_status",
                                               "R_HEADER_invoice", "R_HEADER_invoice_dt", "R_HEADER_po", "R_HEADER_status", "R_HEADER_ont_vendor",
                                               "R_HEADER_invoice", "R_HEADER_invoice_dt", "R_HEADER_po", "R_HEADER_status"};

                    typeCol = new string[] { "System.DateTime", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.DateTime", "System.String", "System.String",
                                             "System.String", "System.DateTime", "System.String", "System.String", "System.String",
                                             "System.String", "System.DateTime", "System.String", "System.String"};
                    break;
                case "REPORTOLT":

                    groupHeader = new string[] { "R_HEADER_olt_transfer", "R_HEADER_active_number" };

                    colHeader = new string[] { "R_HEADER_olt_brand", "R_HEADER_phase", "R_HEADER_region", "R_HEADER_sub_district", "R_HEADER_district",
                                               "R_HEADER_olt_name", "R_HEADER_register_dt", "R_HEADER_day_used", "R_HEADER_port_number", "R_HEADER_status",
                                               "R_HEADER_max", "R_HEADER_max_as_of", "R_HEADER_last_period", "R_HEADER_curr_period", "R_HEADER_diff_number" };

                    typeCol = new string[] { "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.DateTime", "System.Int32", "System.Int32", "System.String",
                                             "System.Int32", "System.DateTime", "System.Int32", "System.Int32", "System.Int32"};
                    break;
                case "REPORTOSP":

                    colHeader = new string[] { "R_HEADER_SPT1_NO", "R_HEADER_SPT2_NO", "R_HEADER_invoice", "R_HEADER_invoice_dt", "R_HEADER_po", "R_HEADER_paid_osp",
                                               "R_HEADER_region", "R_HEADER_phase", "R_HEADER_olt_vendor", "R_HEADER_olt_name", "R_HEADER_site_name", "R_HEADER_sub_district",
                                               "R_HEADER_district", "R_HEADER_node_code", "R_HEADER_cust_no", "R_HEADER_cust_use_dt", "R_HEADER_order_status", "R_HEADER_cust_status",
                                               "R_HEADER_cust_state_dt"};

                    typeCol = new string[] { "System.String", "System.String", "System.String", "System.DateTime", "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.DateTime", "System.String", "System.String", "System.DateTime"  };

                    break;
            }

            if (groupHeader != null && groupHeader.Length != 0)
            {
                for (index = 0; index < groupHeader.Length; index++)
                {
                    tmpVal = lovDataScreen.Where(p => p.Name == groupHeader[index]).ToList();
                    if (tmpVal.Any())
                    {
                        groupHeaderList.Add(tmpVal.FirstOrDefault().LovValue1.ToSafeString());
                    }
                    else
                    {
                        groupHeaderList.Add("Group " + index.ToSafeString());
                    }
                }
            }

            if (colHeader != null && colHeader.Length != 0)
            {
                for (index = 0; index < colHeader.Length; index++)
                {
                    tmpVal = lovDataScreen.Where(p => p.Name == colHeader[index]).ToList();
                    if (tmpVal.Any())
                    {
                        colHeaderList.Add(tmpVal.FirstOrDefault().LovValue1.ToSafeString());
                    }
                    else
                    {
                        colHeaderList.Add(colHeader[index].ToSafeString());
                    }
                }
            }


            if (typeCol != null && typeCol.Length != 0)
            {
                for (index = 0; index < colHeaderList.Count; index++)
                {
                    colVal = colHeaderList[index].ToSafeString();
                    if (!table.Columns.Contains(colVal))
                    {
                        table.Columns.Add(colVal, System.Type.GetType(typeCol[index].ToSafeString()));
                    }
                    else
                    {
                        dupVal += " ";
                        table.Columns.Add(colVal + dupVal, System.Type.GetType(typeCol[index].ToSafeString()));
                    }
                }
            }
        }


        public byte[] GeneratePAYGEntitytoExcel<T>(List<T> data, string fileName, string lovVal5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            //int groupHeader = 0;
            //int colHeader = 0;
            //int colHeaderIndex = 0;
            //bool addColHeader = false;

            var groupHeaderList = new List<string>();
            var colHeaderList = new List<string>();
            int index = 0;
            string tmp = string.Empty;

            var criterList = new List<LovValueModel>();

            SetColumnForExcel(lovVal5, ref table, ref groupHeaderList);

            if (string.Compare(lovVal5, "REPORTOLT", true) == 0 || string.Compare(lovVal5, "REPORTOSP", true) == 0)
            {
                criterList = base.LovData.Where(p => p.Type == "FBBPAYG_SCREEN" && p.LovValue5 == lovVal5 && p.Name == "LABEL_report_name")
                                .OrderBy(p => p.OrderBy).ToList();
            }

            //Replace Value.
            object[] values = new object[props.Count];
            if (string.Compare(lovVal5, "PAIDSTATUS", true) == 0)
            {
                #region PAIDSTATUS

                var statusTopup = base.LovData
                    .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "PAIDSTATUS" && r.Name.Contains("TOPUP_"))
                    .OrderBy(o => o.OrderBy).ToList();

                var reMarkTxt = " :Remark: ";
                foreach (T item in data)
                {
                    var temp = item as UpdateScreenList;

                    //PAID_ST_IN 
                    switch (temp.PAID_ST_IN)
                    {
                        case "W":
                            temp.PAID_ST_IN = statusTopup[0].LovValue1.ToSafeString();
                            break;
                        case "Y":
                            temp.PAID_ST_IN = statusTopup[1].LovValue1.ToSafeString();
                            break;
                        case "N":
                            temp.PAID_ST_IN = statusTopup[2].LovValue1.ToSafeString() + reMarkTxt + temp.REMARK_IN.ToSafeString();
                            break;
                        default:
                            temp.PAID_ST_IN = temp.PAID_ST_IN.ToSafeString();
                            break;

                    }

                    //PAID_ST_OUT 
                    switch (temp.PAID_ST_OUT)
                    {
                        case "W":
                            temp.PAID_ST_OUT = statusTopup[0].LovValue1.ToSafeString();
                            break;
                        case "Y":
                            temp.PAID_ST_OUT = statusTopup[1].LovValue1.ToSafeString();
                            break;
                        case "N":
                            temp.PAID_ST_OUT = statusTopup[2].LovValue1.ToSafeString() + reMarkTxt + temp.REMARK_OUT.ToSafeString();
                            break;
                        default:
                            temp.PAID_ST_OUT = temp.PAID_ST_OUT.ToSafeString();
                            break;

                    }

                    //PAID_ST_ONT
                    switch (temp.PAID_ST_ONT)
                    {
                        case "W":
                            temp.PAID_ST_ONT = statusTopup[0].LovValue1.ToSafeString();
                            break;
                        case "Y":
                            temp.PAID_ST_ONT = statusTopup[1].LovValue1.ToSafeString();
                            break;
                        case "N":
                            temp.PAID_ST_ONT = statusTopup[2].LovValue1.ToSafeString() + reMarkTxt + temp.REMARK_ONT.ToSafeString();
                            break;
                        default:
                            temp.PAID_ST_ONT = temp.PAID_ST_ONT.ToSafeString();
                            break;

                    }

                    var list = new List<int> { 14, 19, 25, 26, 27, 28 };

                    values = new object[props.Count - list.Count];
                    index = 0;
                    for (int i = 0; i < props.Count; i++)
                    {
                        if (!list.Contains(i))
                        {
                            values[index] = props[i].GetValue(temp);
                            index++;
                        }
                    }

                    table.Rows.Add(values);
                }
                #endregion
            }
            else if (string.Compare(lovVal5, "DETAILLASTMILE", true) == 0)
            {
                #region DETAILLASTMILE

                var statusTopup = base.LovData
                    .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "DETAILLASTMILE" && r.Name.Contains("TOPUP_"))
                    .OrderBy(o => o.OrderBy).ToList();

                foreach (T item in data)
                {
                    var temp = item as DetailLastmileAndCPEReportList;
                    //INDOOR_ONT_STATUS 
                    switch (temp.INDOOR_ONT_STATUS)
                    {
                        case "W":
                            temp.INDOOR_ONT_STATUS = statusTopup[0].LovValue1.ToSafeString();
                            break;
                        case "Y":
                            temp.INDOOR_ONT_STATUS = statusTopup[1].LovValue1.ToSafeString();
                            break;
                        case "N":
                            temp.INDOOR_ONT_STATUS = statusTopup[2].LovValue1.ToSafeString();
                            break;
                        default:
                            temp.INDOOR_ONT_STATUS = temp.INDOOR_ONT_STATUS.ToSafeString();
                            break;

                    }

                    //OUTDOO_STATUS 
                    switch (temp.OUTDOO_STATUS)
                    {
                        case "W":
                            temp.OUTDOO_STATUS = statusTopup[0].LovValue1.ToSafeString();
                            break;
                        case "Y":
                            temp.OUTDOO_STATUS = statusTopup[1].LovValue1.ToSafeString();
                            break;
                        case "N":
                            temp.OUTDOO_STATUS = statusTopup[2].LovValue1.ToSafeString();
                            break;
                        default:
                            temp.OUTDOO_STATUS = temp.OUTDOO_STATUS.ToSafeString();
                            break;

                    }

                    //ONT_STATUS
                    switch (temp.ONT_STATUS)
                    {
                        case "W":
                            temp.ONT_STATUS = statusTopup[0].LovValue1.ToSafeString();
                            break;
                        case "Y":
                            temp.ONT_STATUS = statusTopup[1].LovValue1.ToSafeString();
                            break;
                        case "N":
                            temp.ONT_STATUS = statusTopup[2].LovValue1.ToSafeString();
                            break;
                        default:
                            temp.ONT_STATUS = temp.ONT_STATUS.ToSafeString();
                            break;

                    }


                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(temp);
                    }
                    table.Rows.Add(values);
                }
                //if (table.Columns.Contains("ONT  Vendor")) table.Columns.Remove("ONT  Vendor");
                //if (table.Columns.Contains("ONT Model")) table.Columns.Remove("ONT Model");
                //if (table.Columns.Contains("R_HEADER_ont_vendor")) table.Columns.Remove("R_HEADER_ont_vendor");
                //if (table.Columns.Contains("R_HEADER_ont_model")) table.Columns.Remove("R_HEADER_ont_model");
                #endregion
            }
            else if (string.Compare(lovVal5, "REPORTOSP", true) == 0)
            {
                #region REPORTOSP

                var statusTopup = base.LovData
                    .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "REPORTOSP" && r.Name.Contains("TOPUP_"))
                    .OrderBy(o => o.OrderBy).ToList();

                foreach (T item in data)
                {
                    var temp = item as OSPList;

                    //paid_st 
                    switch (temp.paid_st)
                    {
                        case "W":
                            temp.paid_st = statusTopup[0].LovValue1.ToSafeString();
                            break;
                        case "Y":
                            temp.paid_st = statusTopup[1].LovValue1.ToSafeString();
                            break;
                        case "N":
                            temp.paid_st = statusTopup[2].LovValue1.ToSafeString();
                            break;
                        default:
                            temp.paid_st = temp.paid_st.ToSafeString();
                            break;

                    }

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(temp);
                    }

                    table.Rows.Add(values);
                }

                #endregion
            }
            else
            {
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
            }

            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateExcelForPAYG(table, "WorkSheet", tempPath, fileName, lovVal5, groupHeaderList, criterList);
            return data_;
        }

        private byte[] GenerateExcelForPAYG(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string lovVal5, List<string> headerList, List<LovValueModel> criterList)
        {
            if (dataToExcel.Columns.Contains("CNT"))
            {
                dataToExcel.Columns.Remove("CNT");
            }

            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            //fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
            finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", directoryPath, fileName);

            //Delete existing file with same file name.
            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }


            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range1 = null;
            ExcelRange range2 = null;
            ExcelRange range3 = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strHeader;
            string strMergeRow = string.Empty;
            string strColumn1 = string.Empty;
            string strColumn2 = string.Empty;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                //worksheet 
                if (string.Compare(lovVal5, "SUMLASTMILE", true) == 0)
                {
                    #region SUMLASTMILE

                    //Fixed line for Report detail.
                    worksheet.Cells["A2:G2"].Merge = true;
                    worksheet.Cells["A2,G2"].LoadFromText(rptName);
                    worksheet.Cells["A3:I3"].Merge = true;
                    worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                    worksheet.Cells["A4:I4"].Merge = true;
                    worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                    range1 = worksheet.SelectedRange[2, 1, 4, 4];
                    range1.Style.Fill.PatternType = ExcelFillStyle.None;
                    range1.Style.Font.Bold = true;
                    range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                    //start line column.
                    iRow = 7;
                    iHeaderRow = iRow + 1;
                    strRow = iRow.ToSafeString();
                    strHeader = iHeaderRow.ToSafeString();

                    range2 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 7];
                    range2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                    range3 = worksheet.SelectedRange[iRow, 16, iHeaderRow, 18];
                    range3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range3.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                    range3 = worksheet.SelectedRange[iRow, 7, dataToExcel.Rows.Count + iHeaderRow, 7];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy";

                    strMergeRow = string.Format("H{0}:I{0}", strRow);
                    strColumn1 = string.Format("H{0},I{0}", strRow);
                    strColumn2 = string.Format("H{0},I{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[0]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#4cffff"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));

                    strMergeRow = string.Format("J{0}:K{0}", strRow);
                    strColumn1 = string.Format("J{0},K{0}", strRow);
                    strColumn2 = string.Format("J{0},K{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells["J7,K7"].LoadFromText(headerList[1]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ee8823"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F4B77B"));

                    strMergeRow = string.Format("L{0}:M{0}", strRow);
                    strColumn1 = string.Format("L{0},M{0}", strRow);
                    strColumn2 = string.Format("L{0},M{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#8899a9"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C3CCD4"));

                    strMergeRow = string.Format("N{0}:O{0}", strRow);
                    strColumn1 = string.Format("N{0},O{0}", strRow);
                    strColumn2 = string.Format("N{0},O{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[3]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C5919D"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#eddee1"));

                    iHeaderRow++;
                    worksheet.View.FreezePanes(iHeaderRow, 1);

                    worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                    //Add sum value
                    int iTotalRow = dataToExcel.Rows.Count + 10;
                    int iCountRow = dataToExcel.Rows.Count + iHeaderRow;
                    string[] colName = { "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R" };

                    strMergeRow = string.Format("F{0}", iTotalRow.ToSafeString()); // show Total
                    worksheet.Cells[strMergeRow].LoadFromText("Total");
                    worksheet.Cells[strMergeRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strMergeRow].Style.Font.Bold = true;


                    for (int i = 0; i < colName.Length; i++)
                    {
                        strColumn1 = string.Format("{0}{1}", colName[i], iTotalRow.ToSafeString()); //total value col
                        strColumn2 = string.Format("=SUM({0}{1}:{0}{2})", colName[i], iHeaderRow.ToSafeString(), iCountRow); //formula
                        worksheet.Cells[strColumn1].Formula = strColumn2;
                    }

                    #endregion
                }
                else if (string.Compare(lovVal5, "DETAILLASTMILE", true) == 0)
                {
                    #region DETAILLASTMILE
                    //Fixed line for Report detail.
                    worksheet.Cells["A2:G2"].Merge = true;
                    worksheet.Cells["A2,G2"].LoadFromText(rptName);
                    worksheet.Cells["A3:I3"].Merge = true;
                    worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                    worksheet.Cells["A4:I4"].Merge = true;
                    worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                    range1 = worksheet.SelectedRange[2, 1, 4, 4];
                    range1.Style.Fill.PatternType = ExcelFillStyle.None;
                    range1.Style.Font.Bold = true;
                    range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                    //start line column.
                    iRow = 7;
                    iHeaderRow = iRow + 1;
                    strRow = iRow.ToSafeString();
                    strHeader = iHeaderRow.ToSafeString();

                    range2 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 26];
                    range2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                    range3 = worksheet.SelectedRange[iRow, 35, iHeaderRow, 41];
                    range3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range3.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                    //change format date.
                    range3 = worksheet.SelectedRange[iRow, 1, dataToExcel.Rows.Count + iHeaderRow, 1];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    range3 = worksheet.SelectedRange[iRow, 2, dataToExcel.Rows.Count + iHeaderRow, 2];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    range3 = worksheet.SelectedRange[iRow, 3, dataToExcel.Rows.Count + iHeaderRow, 3];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                    range3 = worksheet.SelectedRange[iRow, 8, dataToExcel.Rows.Count + iHeaderRow, 8];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    range3 = worksheet.SelectedRange[iRow, 10, dataToExcel.Rows.Count + iHeaderRow, 10];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    range3 = worksheet.SelectedRange[iRow, 28, dataToExcel.Rows.Count + iHeaderRow, 28];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    range3 = worksheet.SelectedRange[iRow, 32, dataToExcel.Rows.Count + iHeaderRow, 32];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    range3 = worksheet.SelectedRange[iRow, 40, dataToExcel.Rows.Count + iHeaderRow, 40];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                    strMergeRow = string.Format("AA{0}:AD{0}", strRow);
                    strColumn1 = string.Format("AA{0},AD{0}", strRow);
                    strColumn2 = string.Format("AA{0},AB{0},AC{0},AD{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[0]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#4cffff"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));

                    strMergeRow = string.Format("AE{0}:AH{0}", strRow);
                    strColumn1 = string.Format("AE{0},AH{0}", strRow);
                    strColumn2 = string.Format("AE{0},AF{0},AG{0},AH{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[1]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ee8823"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F4B77B"));

                    strMergeRow = string.Format("AP{0}:AS{0}", strRow);
                    strColumn1 = string.Format("AP{0},AS{0}", strRow);
                    strColumn2 = string.Format("AP{0},AQ{0},AR{0},AS{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#8899a9"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C3CCD4"));

                    strColumn1 = string.Format("J{0},J{0}", strRow);
                    worksheet.Cells[strColumn1].Style.Hidden = true;

                    range3 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 45];
                    range3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    iHeaderRow++;
                    worksheet.View.FreezePanes(iHeaderRow, 1);

                    worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                    #endregion
                }
                else if (string.Compare(lovVal5, "PAIDSTATUS", true) == 0)
                {
                    #region PAIDSTATUS

                    //Fixed line for Report detail.
                    worksheet.Cells["A2:G2"].Merge = true;
                    worksheet.Cells["A2,G2"].LoadFromText(rptName);
                    worksheet.Cells["A3:I3"].Merge = true;
                    worksheet.Cells["A3,I3"].LoadFromText(rptDate);

                    worksheet.Cells["A5:I5"].Merge = true;
                    worksheet.Cells["A5,I5"].LoadFromText(rptCriteria);

                    //start line column.
                    iRow = 6;
                    strRow = iRow.ToSafeString();

                    for (int i = 0; i < rptCriterias.Count; i++)
                    {
                        worksheet.Cells[string.Format("A{0}:I{0}", strRow)].Merge = true;
                        worksheet.Cells[string.Format("A{0},I{0}", strRow)].LoadFromText(rptCriterias[i]);
                        iRow++;
                        strRow = iRow.ToSafeString();
                    }

                    iRow++;
                    iHeaderRow = iRow + 1;
                    strRow = iRow.ToSafeString();
                    strHeader = iHeaderRow.ToSafeString();

                    range1 = worksheet.SelectedRange[2, 1, iRow, 10];
                    range1.Style.Fill.PatternType = ExcelFillStyle.None;
                    range1.Style.Font.Bold = true;
                    range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                    range2 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 16];
                    range2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                    range3 = worksheet.SelectedRange[iRow, 17, iHeaderRow, 19];
                    range3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range3.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                    //change format date.
                    range3 = worksheet.SelectedRange[iRow, 1, dataToExcel.Rows.Count + iHeaderRow, 1];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy";

                    strMergeRow = string.Format("K{0}:N{0}", strRow);
                    strColumn1 = string.Format("K{0},N{0}", strRow);
                    strColumn2 = string.Format("K{0},L{0},M{0},N{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    //worksheet.Cells[strMergeRow].Style.Hidden = showInDoor;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[1]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#4cffff"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));

                    if (!showInDoor)
                    {
                        worksheet.Column(11).Hidden = true;
                        worksheet.Column(12).Hidden = true;
                        worksheet.Column(13).Hidden = true;
                        worksheet.Column(14).Hidden = true;
                    }

                    strMergeRow = string.Format("O{0}:R{0}", strRow);
                    strColumn1 = string.Format("O{0},R{0}", strRow);
                    strColumn2 = string.Format("O{0},P{0},Q{0},R{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ee8823"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F4B77B"));
                    if (!showOutDoor)
                    {
                        worksheet.Column(15).Hidden = true;
                        worksheet.Column(16).Hidden = true;
                        worksheet.Column(17).Hidden = true;
                        worksheet.Column(18).Hidden = true;
                    }

                    strMergeRow = string.Format("T{0}:W{0}", strRow);
                    strColumn1 = string.Format("T{0},W{0}", strRow);
                    strColumn2 = string.Format("S{0},T{0},U{0},V{0},W{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#8899a9"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C3CCD4"));
                    if (!showONT)
                    {
                        worksheet.Column(19).Hidden = true;
                        worksheet.Column(20).Hidden = true;
                        worksheet.Column(21).Hidden = true;
                        worksheet.Column(22).Hidden = true;
                        worksheet.Column(23).Hidden = true;
                    }

                    iHeaderRow++;
                    worksheet.View.FreezePanes(iHeaderRow, 1);

                    worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                    #endregion
                }
                else if (string.Compare(lovVal5, "REPORTOLT", true) == 0)
                {
                    #region REPORTOLT

                    var lovRemark = base.LovData.Where(p => p.Type == "FBBPAYG_SCREEN" && p.LovValue5 == lovVal5 && p.Name == "LABEL_remark")
                               .OrderBy(p => p.OrderBy).ToList();

                    worksheet.Cells["A2:G2"].Merge = true;
                    worksheet.Cells["A2,G2"].LoadFromText(rptName);
                    worksheet.Cells["A3:I3"].Merge = true;
                    worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                    worksheet.Cells["A4:I4"].Merge = true;
                    worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                    worksheet.Cells["A6:L6"].Merge = true;
                    worksheet.Cells["A6,L6"].LoadFromText(lovRemark.FirstOrDefault().LovValue1.ToSafeString());

                    range1 = worksheet.SelectedRange[2, 1, 4, 4];
                    range1.Style.Fill.PatternType = ExcelFillStyle.None;
                    range1.Style.Font.Bold = true;
                    range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                    //Fixed line for Report detail.
                    /*worksheet.Cells["A2:D2"].Merge = true;
                    worksheet.Cells["A2,D2"].LoadFromText(criterList.FirstOrDefault().LovValue1.ToSafeString());
                    worksheet.Cells["A2,D2"].Style.Font.Bold = true;
                    worksheet.Cells["A2,D2"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));*/

                    //start line column.
                    iRow = 8;
                    iHeaderRow = iRow + 1;
                    strRow = iRow.ToSafeString();
                    strHeader = iHeaderRow.ToSafeString();

                    range1 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 9];
                    range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range1.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                    range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    range2 = worksheet.SelectedRange[iRow, 7, dataToExcel.Rows.Count + iHeaderRow, 7];
                    range2.Style.Numberformat.Format = "dd/MM/yyyy";

                    range3 = worksheet.SelectedRange[iRow, 12, dataToExcel.Rows.Count + iHeaderRow, 12];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy";

                    strColumn1 = string.Format("J{0}", strRow);
                    strColumn2 = string.Format("J{0}", strHeader);
                    worksheet.Cells[strColumn1].LoadFromText(headerList[0]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ffcc11"));
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ff3333"));

                    strMergeRow = string.Format("K{0}:O{0}", strRow);
                    strColumn1 = string.Format("K{0},O{0}", strRow);
                    strColumn2 = string.Format("K{0},L{0},M{0},N{0},O{0}", strHeader);
                    worksheet.Cells[strMergeRow].Merge = true;
                    worksheet.Cells[strColumn1].LoadFromText(headerList[1]);
                    worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#bbdd00"));
                    worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#bbdd00"));

                    iHeaderRow++;
                    worksheet.View.FreezePanes(iHeaderRow, 1);

                    worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                    #endregion
                }
                else if (string.Compare(lovVal5, "REPORTOSP", true) == 0)
                {
                    #region REPORTOSP

                    worksheet.Cells["A2:G2"].Merge = true;
                    worksheet.Cells["A2,G2"].LoadFromText(rptName);
                    worksheet.Cells["A3:I3"].Merge = true;
                    worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                    worksheet.Cells["A4:I4"].Merge = true;
                    worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                    range1 = worksheet.SelectedRange[2, 1, 4, 4];
                    range1.Style.Fill.PatternType = ExcelFillStyle.None;
                    range1.Style.Font.Bold = true;
                    range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));


                    /*worksheet.Cells["A2:D2"].Merge = true;
                    worksheet.Cells["A2,D2"].LoadFromText(criterList.FirstOrDefault().LovValue1.ToSafeString());
                    worksheet.Cells["A2,D2"].Style.Font.Bold = true;
                    worksheet.Cells["A2,D2"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));*/

                    //start line column.
                    iRow = 7;
                    iHeaderRow = iRow + 1;
                    strRow = iRow.ToSafeString();

                    range1 = worksheet.SelectedRange[iRow, 1, iRow, 20];
                    range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range1.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                    range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    range2 = worksheet.SelectedRange[iRow, 4, dataToExcel.Rows.Count + iHeaderRow, 4];
                    range2.Style.Numberformat.Format = "dd/MM/yyyy";

                    range3 = worksheet.SelectedRange[iRow, 16, dataToExcel.Rows.Count + iHeaderRow, 16];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy";

                    range3 = worksheet.SelectedRange[iRow, 19, dataToExcel.Rows.Count + iHeaderRow, 19];
                    range3.Style.Numberformat.Format = "dd/MM/yyyy";


                    worksheet.View.FreezePanes(iHeaderRow, 1);

                    //Step 3 : Start loading datatable form A1 cell of worksheet.
                    strColumn1 = string.Format("A{0}", strRow);
                    worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                    #endregion
                }

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }
        }

        public FileStreamResult SetFileLog()
        {
            string filename = GetExcelName("ImportData");

            var errorMsg = Session["ImportData"].ToSafeString();

            errorMsg = errorMsg.Replace("<br/>", "\r\n");

            var byteArray = Encoding.ASCII.GetBytes(errorMsg);
            var stream = new MemoryStream(byteArray);

            return File(stream, "text/plain", filename + ".txt");

        }
        public class DelailLastMileSendEmail
        {
            public static byte[] FileData;
            public static MemoryStream msExcel;
        }

        #region ExportByEmail
        public async Task ExportByEmail([DataSourceRequest] DataSourceRequest request, string dataS = "", string ctr = "", string Email = "")
        {
            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                var searchorpt1Model = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);


                rptCriteria = ctr;
                searchorpt1Model.P_PAGE_SIZE = 9999999;
                var listall = new List<DetailLastmileAndCPEReportList>();

                //var Rollback = base.LovData.Where(s => s.Type == "DETAIL_LASTMILE" && s.Name == "ROLLBACK").FirstOrDefault();
                //var activeflag = Rollback != null ? Rollback.ActiveFlag : "N";
                if (activeflag == "Y")
                {
                    listall = GetDataRpt2SearchModel(searchorpt1Model);
                }
                else
                {
                    listall = GetDataRpt2SearchModelNew(searchorpt1Model);
                }


                if (listall.Count > 0)
                {
                    //GenFileName
                    string filename = GetExcelName("DetailLastmileAndCPE") + CurrentUser.UserName.ToSafeString();
                    //GenReport
                    string x = await GenerateFilePAYGEntitytoExcel<DetailLastmileAndCPEReportList>(listall, filename, "DETAILLASTMILE");
                    DetailLastMileSendtoEmail(filename, Email, rptCriteria);
                    //SendEmail
                }

            }
            catch (Exception Ex)
            {
                Ex.Message.ToSafeString();

                // return null;
            }
        }
        public async Task<string> GenerateFilePAYGEntitytoExcel<T>(List<T> data, string fileName, string lovVal5)
        {

            try
            {
                System.ComponentModel.PropertyDescriptorCollection props =
           System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
                DataTable table = new DataTable();

                //int groupHeader = 0;
                //int colHeader = 0;
                //int colHeaderIndex = 0;
                //bool addColHeader = false;

                var groupHeaderList = new List<string>();
                var colHeaderList = new List<string>();
                int index = 0;
                string tmp = string.Empty;

                var criterList = new List<LovValueModel>();

                SetColumnForExcel(lovVal5, ref table, ref groupHeaderList);

                if (string.Compare(lovVal5, "REPORTOLT", true) == 0 || string.Compare(lovVal5, "REPORTOSP", true) == 0)
                {
                    criterList = base.LovData.Where(p => p.Type == "FBBPAYG_SCREEN" && p.LovValue5 == lovVal5 && p.Name == "LABEL_report_name")
                                    .OrderBy(p => p.OrderBy).ToList();
                }

                //Replace Value.
                object[] values = new object[props.Count];
                if (string.Compare(lovVal5, "PAIDSTATUS", true) == 0)
                {
                    #region PAIDSTATUS

                    var statusTopup = base.LovData
                        .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "PAIDSTATUS" && r.Name.Contains("TOPUP_"))
                        .OrderBy(o => o.OrderBy).ToList();

                    var reMarkTxt = " :Remark: ";
                    foreach (T item in data)
                    {
                        var temp = item as UpdateScreenList;

                        //PAID_ST_IN 
                        switch (temp.PAID_ST_IN)
                        {
                            case "W":
                                temp.PAID_ST_IN = statusTopup[0].LovValue1.ToSafeString();
                                break;
                            case "Y":
                                temp.PAID_ST_IN = statusTopup[1].LovValue1.ToSafeString();
                                break;
                            case "N":
                                temp.PAID_ST_IN = statusTopup[2].LovValue1.ToSafeString() + reMarkTxt + temp.REMARK_IN.ToSafeString();
                                break;
                            default:
                                temp.PAID_ST_IN = temp.PAID_ST_IN.ToSafeString();
                                break;

                        }

                        //PAID_ST_OUT 
                        switch (temp.PAID_ST_OUT)
                        {
                            case "W":
                                temp.PAID_ST_OUT = statusTopup[0].LovValue1.ToSafeString();
                                break;
                            case "Y":
                                temp.PAID_ST_OUT = statusTopup[1].LovValue1.ToSafeString();
                                break;
                            case "N":
                                temp.PAID_ST_OUT = statusTopup[2].LovValue1.ToSafeString() + reMarkTxt + temp.REMARK_OUT.ToSafeString();
                                break;
                            default:
                                temp.PAID_ST_OUT = temp.PAID_ST_OUT.ToSafeString();
                                break;

                        }

                        //PAID_ST_ONT
                        switch (temp.PAID_ST_ONT)
                        {
                            case "W":
                                temp.PAID_ST_ONT = statusTopup[0].LovValue1.ToSafeString();
                                break;
                            case "Y":
                                temp.PAID_ST_ONT = statusTopup[1].LovValue1.ToSafeString();
                                break;
                            case "N":
                                temp.PAID_ST_ONT = statusTopup[2].LovValue1.ToSafeString() + reMarkTxt + temp.REMARK_ONT.ToSafeString();
                                break;
                            default:
                                temp.PAID_ST_ONT = temp.PAID_ST_ONT.ToSafeString();
                                break;

                        }

                        var list = new List<int> { 14, 19, 25, 26, 27, 28 };

                        values = new object[props.Count - list.Count];
                        index = 0;
                        for (int i = 0; i < props.Count; i++)
                        {
                            if (!list.Contains(i))
                            {
                                values[index] = props[i].GetValue(temp);
                                index++;
                            }
                        }

                        table.Rows.Add(values);
                    }
                    #endregion
                }
                else if (string.Compare(lovVal5, "DETAILLASTMILE", true) == 0)
                {
                    #region DETAILLASTMILE

                    var statusTopup = base.LovData
                        .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "DETAILLASTMILE" && r.Name.Contains("TOPUP_"))
                        .OrderBy(o => o.OrderBy).ToList();

                    foreach (T item in data)
                    {
                        var temp = item as DetailLastmileAndCPEReportList;
                        //INDOOR_ONT_STATUS 
                        switch (temp.INDOOR_ONT_STATUS)
                        {
                            case "W":
                                temp.INDOOR_ONT_STATUS = statusTopup[0].LovValue1.ToSafeString();
                                break;
                            case "Y":
                                temp.INDOOR_ONT_STATUS = statusTopup[1].LovValue1.ToSafeString();
                                break;
                            case "N":
                                temp.INDOOR_ONT_STATUS = statusTopup[2].LovValue1.ToSafeString();
                                break;
                            default:
                                temp.INDOOR_ONT_STATUS = temp.INDOOR_ONT_STATUS.ToSafeString();
                                break;

                        }

                        //OUTDOO_STATUS 
                        switch (temp.OUTDOO_STATUS)
                        {
                            case "W":
                                temp.OUTDOO_STATUS = statusTopup[0].LovValue1.ToSafeString();
                                break;
                            case "Y":
                                temp.OUTDOO_STATUS = statusTopup[1].LovValue1.ToSafeString();
                                break;
                            case "N":
                                temp.OUTDOO_STATUS = statusTopup[2].LovValue1.ToSafeString();
                                break;
                            default:
                                temp.OUTDOO_STATUS = temp.OUTDOO_STATUS.ToSafeString();
                                break;

                        }

                        //ONT_STATUS
                        switch (temp.ONT_STATUS)
                        {
                            case "W":
                                temp.ONT_STATUS = statusTopup[0].LovValue1.ToSafeString();
                                break;
                            case "Y":
                                temp.ONT_STATUS = statusTopup[1].LovValue1.ToSafeString();
                                break;
                            case "N":
                                temp.ONT_STATUS = statusTopup[2].LovValue1.ToSafeString();
                                break;
                            default:
                                temp.ONT_STATUS = temp.ONT_STATUS.ToSafeString();
                                break;

                        }


                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(temp);
                        }
                        table.Rows.Add(values);
                    }
                    //if (table.Columns.Contains("ONT  Vendor")) table.Columns.Remove("ONT  Vendor");
                    //if (table.Columns.Contains("ONT Model")) table.Columns.Remove("ONT Model");
                    //if (table.Columns.Contains("R_HEADER_ont_vendor")) table.Columns.Remove("R_HEADER_ont_vendor");
                    //if (table.Columns.Contains("R_HEADER_ont_model")) table.Columns.Remove("R_HEADER_ont_model");
                    #endregion
                }
                else if (string.Compare(lovVal5, "REPORTOSP", true) == 0)
                {
                    #region REPORTOSP

                    var statusTopup = base.LovData
                        .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "REPORTOSP" && r.Name.Contains("TOPUP_"))
                        .OrderBy(o => o.OrderBy).ToList();

                    foreach (T item in data)
                    {
                        var temp = item as OSPList;

                        //paid_st 
                        switch (temp.paid_st)
                        {
                            case "W":
                                temp.paid_st = statusTopup[0].LovValue1.ToSafeString();
                                break;
                            case "Y":
                                temp.paid_st = statusTopup[1].LovValue1.ToSafeString();
                                break;
                            case "N":
                                temp.paid_st = statusTopup[2].LovValue1.ToSafeString();
                                break;
                            default:
                                temp.paid_st = temp.paid_st.ToSafeString();
                                break;

                        }

                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(temp);
                        }

                        table.Rows.Add(values);
                    }

                    #endregion
                }
                else
                {
                    foreach (T item in data)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }
                        table.Rows.Add(values);
                    }
                }

                string tempPath = System.IO.Path.GetTempPath();


                var dataToExcel = table;
                string excelSheetName = "WorkSheet";
                string directoryPath = tempPath;
                //string lovVal5, 
                //   
                List<string> headerList = groupHeaderList;
                // List<LovValueModel> criterList = criterList;
                if (dataToExcel.Columns.Contains("CNT"))
                {
                    dataToExcel.Columns.Remove("CNT");
                }

                if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
                { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }

                //string currentDirectorypath = Environment.CurrentDirectory;
                string finalFileNameWithPath = string.Empty;

                //fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
                finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", directoryPath, fileName);

                //Delete existing file with same file name.
                if (System.IO.File.Exists(finalFileNameWithPath))
                { System.IO.File.Delete(finalFileNameWithPath); }


                var newFile = new FileInfo(finalFileNameWithPath);
                ExcelRange range1 = null;
                ExcelRange range2 = null;
                ExcelRange range3 = null;

                int iRow;
                int iHeaderRow;
                string strRow;
                string strHeader;
                string strMergeRow = string.Empty;
                string strColumn1 = string.Empty;
                string strColumn2 = string.Empty;

                //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
                using (var package = new ExcelPackage(newFile))
                {
                    //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                    //worksheet 
                    if (string.Compare(lovVal5, "SUMLASTMILE", true) == 0)
                    {
                        #region SUMLASTMILE

                        //Fixed line for Report detail.
                        //worksheet.Cells["A2:G2"].Merge = true;
                        //worksheet.Cells["A2,G2"].LoadFromText(rptName);
                        //worksheet.Cells["A3:I3"].Merge = true;
                        //worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                        //worksheet.Cells["A4:I4"].Merge = true;
                        //worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                        range1 = worksheet.SelectedRange[2, 1, 4, 4];
                        range1.Style.Fill.PatternType = ExcelFillStyle.None;
                        range1.Style.Font.Bold = true;
                        range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                        //start line column.
                        iRow = 2;
                        iHeaderRow = iRow + 1;
                        strRow = iRow.ToSafeString();
                        strHeader = iHeaderRow.ToSafeString();

                        range2 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 7];
                        range2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                        range3 = worksheet.SelectedRange[iRow, 16, iHeaderRow, 18];
                        range3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range3.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                        range3 = worksheet.SelectedRange[iRow, 7, dataToExcel.Rows.Count + iHeaderRow, 7];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy";

                        strMergeRow = string.Format("H{0}:I{0}", strRow);
                        strColumn1 = string.Format("H{0},I{0}", strRow);
                        strColumn2 = string.Format("H{0},I{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[0]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#4cffff"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));

                        strMergeRow = string.Format("J{0}:K{0}", strRow);
                        strColumn1 = string.Format("J{0},K{0}", strRow);
                        strColumn2 = string.Format("J{0},K{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells["J7,K7"].LoadFromText(headerList[1]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ee8823"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F4B77B"));

                        strMergeRow = string.Format("L{0}:M{0}", strRow);
                        strColumn1 = string.Format("L{0},M{0}", strRow);
                        strColumn2 = string.Format("L{0},M{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#8899a9"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C3CCD4"));

                        strMergeRow = string.Format("N{0}:O{0}", strRow);
                        strColumn1 = string.Format("N{0},O{0}", strRow);
                        strColumn2 = string.Format("N{0},O{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[3]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C5919D"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#eddee1"));

                        iHeaderRow++;
                        worksheet.View.FreezePanes(iHeaderRow, 1);

                        worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                        //Add sum value
                        int iTotalRow = dataToExcel.Rows.Count + 10;
                        int iCountRow = dataToExcel.Rows.Count + iHeaderRow;
                        string[] colName = { "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R" };

                        strMergeRow = string.Format("F{0}", iTotalRow.ToSafeString()); // show Total
                        worksheet.Cells[strMergeRow].LoadFromText("Total");
                        worksheet.Cells[strMergeRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strMergeRow].Style.Font.Bold = true;


                        for (int i = 0; i < colName.Length; i++)
                        {
                            strColumn1 = string.Format("{0}{1}", colName[i], iTotalRow.ToSafeString()); //total value col
                            strColumn2 = string.Format("=SUM({0}{1}:{0}{2})", colName[i], iHeaderRow.ToSafeString(), iCountRow); //formula
                            worksheet.Cells[strColumn1].Formula = strColumn2;
                        }

                        #endregion
                    }
                    else if (string.Compare(lovVal5, "DETAILLASTMILE", true) == 0)
                    {
                        #region DETAILLASTMILE
                        //Fixed line for Report detail.
                        //worksheet.Cells["A2:G2"].Merge = true;
                        //worksheet.Cells["A2,G2"].LoadFromText(rptName);
                        //worksheet.Cells["A3:I3"].Merge = true;
                        //worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                        //worksheet.Cells["A4:I4"].Merge = true;
                        //worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                        range1 = worksheet.SelectedRange[2, 1, 4, 4];
                        range1.Style.Fill.PatternType = ExcelFillStyle.None;
                        range1.Style.Font.Bold = true;
                        range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                        //start line column.
                        iRow = 2;
                        iHeaderRow = iRow + 1;
                        strRow = iRow.ToSafeString();
                        strHeader = iHeaderRow.ToSafeString();

                        range2 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 26];
                        range2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                        range3 = worksheet.SelectedRange[iRow, 35, iHeaderRow, 41];
                        range3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range3.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                        //change format date.
                        range3 = worksheet.SelectedRange[iRow, 1, dataToExcel.Rows.Count + iHeaderRow, 1];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                        range3 = worksheet.SelectedRange[iRow, 2, dataToExcel.Rows.Count + iHeaderRow, 2];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                        range3 = worksheet.SelectedRange[iRow, 3, dataToExcel.Rows.Count + iHeaderRow, 3];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                        range3 = worksheet.SelectedRange[iRow, 8, dataToExcel.Rows.Count + iHeaderRow, 8];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                        range3 = worksheet.SelectedRange[iRow, 10, dataToExcel.Rows.Count + iHeaderRow, 10];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                        range3 = worksheet.SelectedRange[iRow, 28, dataToExcel.Rows.Count + iHeaderRow, 28];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                        range3 = worksheet.SelectedRange[iRow, 32, dataToExcel.Rows.Count + iHeaderRow, 32];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                        range3 = worksheet.SelectedRange[iRow, 40, dataToExcel.Rows.Count + iHeaderRow, 40];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                        strMergeRow = string.Format("AA{0}:AD{0}", strRow);
                        strColumn1 = string.Format("AA{0},AD{0}", strRow);
                        strColumn2 = string.Format("AA{0},AB{0},AC{0},AD{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[0]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#4cffff"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));

                        strMergeRow = string.Format("AE{0}:AH{0}", strRow);
                        strColumn1 = string.Format("AE{0},AH{0}", strRow);
                        strColumn2 = string.Format("AE{0},AF{0},AG{0},AH{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[1]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ee8823"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F4B77B"));

                        strMergeRow = string.Format("AP{0}:AS{0}", strRow);
                        strColumn1 = string.Format("AP{0},AS{0}", strRow);
                        strColumn2 = string.Format("AP{0},AQ{0},AR{0},AS{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#8899a9"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C3CCD4"));

                        strColumn1 = string.Format("J{0},J{0}", strRow);
                        worksheet.Cells[strColumn1].Style.Hidden = true;

                        range3 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 45];
                        range3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        iHeaderRow++;
                        worksheet.View.FreezePanes(iHeaderRow, 1);

                        worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                        #endregion
                    }
                    else if (string.Compare(lovVal5, "PAIDSTATUS", true) == 0)
                    {
                        #region PAIDSTATUS

                        //Fixed line for Report detail.
                        //worksheet.Cells["A2:G2"].Merge = true;
                        //worksheet.Cells["A2,G2"].LoadFromText(rptName);
                        //worksheet.Cells["A3:I3"].Merge = true;
                        //worksheet.Cells["A3,I3"].LoadFromText(rptDate);

                        //worksheet.Cells["A5:I5"].Merge = true;
                        //worksheet.Cells["A5,I5"].LoadFromText(rptCriteria);

                        //start line column.
                        iRow = 2;
                        strRow = iRow.ToSafeString();

                        for (int i = 0; i < rptCriterias.Count; i++)
                        {
                            worksheet.Cells[string.Format("A{0}:I{0}", strRow)].Merge = true;
                            worksheet.Cells[string.Format("A{0},I{0}", strRow)].LoadFromText(rptCriterias[i]);
                            iRow++;
                            strRow = iRow.ToSafeString();
                        }

                        iRow++;
                        iHeaderRow = iRow + 1;
                        strRow = iRow.ToSafeString();
                        strHeader = iHeaderRow.ToSafeString();

                        range1 = worksheet.SelectedRange[2, 1, iRow, 10];
                        range1.Style.Fill.PatternType = ExcelFillStyle.None;
                        range1.Style.Font.Bold = true;
                        range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                        range2 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 16];
                        range2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range2.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                        range3 = worksheet.SelectedRange[iRow, 17, iHeaderRow, 19];
                        range3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range3.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));

                        //change format date.
                        range3 = worksheet.SelectedRange[iRow, 1, dataToExcel.Rows.Count + iHeaderRow, 1];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy";

                        strMergeRow = string.Format("K{0}:N{0}", strRow);
                        strColumn1 = string.Format("K{0},N{0}", strRow);
                        strColumn2 = string.Format("K{0},L{0},M{0},N{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        //worksheet.Cells[strMergeRow].Style.Hidden = showInDoor;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[1]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#4cffff"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));

                        if (!showInDoor)
                        {
                            worksheet.Column(11).Hidden = true;
                            worksheet.Column(12).Hidden = true;
                            worksheet.Column(13).Hidden = true;
                            worksheet.Column(14).Hidden = true;
                        }

                        strMergeRow = string.Format("O{0}:R{0}", strRow);
                        strColumn1 = string.Format("O{0},R{0}", strRow);
                        strColumn2 = string.Format("O{0},P{0},Q{0},R{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ee8823"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#F4B77B"));
                        if (!showOutDoor)
                        {
                            worksheet.Column(15).Hidden = true;
                            worksheet.Column(16).Hidden = true;
                            worksheet.Column(17).Hidden = true;
                            worksheet.Column(18).Hidden = true;
                        }

                        strMergeRow = string.Format("T{0}:W{0}", strRow);
                        strColumn1 = string.Format("T{0},W{0}", strRow);
                        strColumn2 = string.Format("S{0},T{0},U{0},V{0},W{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[2]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#8899a9"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#C3CCD4"));
                        if (!showONT)
                        {
                            worksheet.Column(19).Hidden = true;
                            worksheet.Column(20).Hidden = true;
                            worksheet.Column(21).Hidden = true;
                            worksheet.Column(22).Hidden = true;
                            worksheet.Column(23).Hidden = true;
                        }

                        iHeaderRow++;
                        worksheet.View.FreezePanes(iHeaderRow, 1);

                        worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                        #endregion
                    }
                    else if (string.Compare(lovVal5, "REPORTOLT", true) == 0)
                    {
                        #region REPORTOLT

                        var lovRemark = base.LovData.Where(p => p.Type == "FBBPAYG_SCREEN" && p.LovValue5 == lovVal5 && p.Name == "LABEL_remark")
                                   .OrderBy(p => p.OrderBy).ToList();

                        //worksheet.Cells["A2:G2"].Merge = true;
                        //worksheet.Cells["A2,G2"].LoadFromText(rptName);
                        //worksheet.Cells["A3:I3"].Merge = true;
                        //worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                        //worksheet.Cells["A4:I4"].Merge = true;
                        //worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                        worksheet.Cells["A2:L2"].Merge = true;
                        worksheet.Cells["A2,L2"].LoadFromText(lovRemark.FirstOrDefault().LovValue1.ToSafeString());

                        range1 = worksheet.SelectedRange[2, 1, 4, 4];
                        range1.Style.Fill.PatternType = ExcelFillStyle.None;
                        range1.Style.Font.Bold = true;
                        range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                        //Fixed line for Report detail.
                        /*worksheet.Cells["A2:D2"].Merge = true;
                        worksheet.Cells["A2,D2"].LoadFromText(criterList.FirstOrDefault().LovValue1.ToSafeString());
                        worksheet.Cells["A2,D2"].Style.Font.Bold = true;
                        worksheet.Cells["A2,D2"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));*/

                        //start line column.
                        iRow = 3;
                        iHeaderRow = iRow + 1;
                        strRow = iRow.ToSafeString();
                        strHeader = iHeaderRow.ToSafeString();

                        range1 = worksheet.SelectedRange[iRow, 1, iHeaderRow, 9];
                        range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range1.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                        range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        range2 = worksheet.SelectedRange[iRow, 7, dataToExcel.Rows.Count + iHeaderRow, 7];
                        range2.Style.Numberformat.Format = "dd/MM/yyyy";

                        range3 = worksheet.SelectedRange[iRow, 12, dataToExcel.Rows.Count + iHeaderRow, 12];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy";

                        strColumn1 = string.Format("J{0}", strRow);
                        strColumn2 = string.Format("J{0}", strHeader);
                        worksheet.Cells[strColumn1].LoadFromText(headerList[0]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ffcc11"));
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ff3333"));

                        strMergeRow = string.Format("K{0}:O{0}", strRow);
                        strColumn1 = string.Format("K{0},O{0}", strRow);
                        strColumn2 = string.Format("K{0},L{0},M{0},N{0},O{0}", strHeader);
                        worksheet.Cells[strMergeRow].Merge = true;
                        worksheet.Cells[strColumn1].LoadFromText(headerList[1]);
                        worksheet.Cells[strColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[strColumn1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#bbdd00"));
                        worksheet.Cells[strColumn2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[strColumn2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#bbdd00"));

                        iHeaderRow++;
                        worksheet.View.FreezePanes(iHeaderRow, 1);

                        worksheet.Cells[string.Format("A{0}", strHeader)].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                        #endregion
                    }
                    else if (string.Compare(lovVal5, "REPORTOSP", true) == 0)
                    {
                        #region REPORTOSP

                        //worksheet.Cells["A2:G2"].Merge = true;
                        //worksheet.Cells["A2,G2"].LoadFromText(rptName);
                        //worksheet.Cells["A3:I3"].Merge = true;
                        //worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                        //worksheet.Cells["A4:I4"].Merge = true;
                        //worksheet.Cells["A4,I4"].LoadFromText(rptDate);

                        range1 = worksheet.SelectedRange[2, 1, 4, 4];
                        range1.Style.Fill.PatternType = ExcelFillStyle.None;
                        range1.Style.Font.Bold = true;
                        range1.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));


                        /*worksheet.Cells["A2:D2"].Merge = true;
                        worksheet.Cells["A2,D2"].LoadFromText(criterList.FirstOrDefault().LovValue1.ToSafeString());
                        worksheet.Cells["A2,D2"].Style.Font.Bold = true;
                        worksheet.Cells["A2,D2"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));*/

                        //start line column.
                        iRow = 2;
                        iHeaderRow = iRow + 1;
                        strRow = iRow.ToSafeString();

                        range1 = worksheet.SelectedRange[iRow, 1, iRow, 20];
                        range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range1.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                        range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        range2 = worksheet.SelectedRange[iRow, 4, dataToExcel.Rows.Count + iHeaderRow, 4];
                        range2.Style.Numberformat.Format = "dd/MM/yyyy";

                        range3 = worksheet.SelectedRange[iRow, 16, dataToExcel.Rows.Count + iHeaderRow, 16];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy";

                        range3 = worksheet.SelectedRange[iRow, 19, dataToExcel.Rows.Count + iHeaderRow, 19];
                        range3.Style.Numberformat.Format = "dd/MM/yyyy";


                        worksheet.View.FreezePanes(iHeaderRow, 1);

                        //Step 3 : Start loading datatable form A1 cell of worksheet.
                        strColumn1 = string.Format("A{0}", strRow);
                        worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                        #endregion
                    }

                    //Step 4 : (Optional) Set the file properties like title, author and subject
                    package.Workbook.Properties.Title = @"FBB Config";
                    package.Workbook.Properties.Author = "FBB";
                    package.Workbook.Properties.Subject = @"" + excelSheetName;

                    //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                    //package.Save();
                    DelailLastMileSendEmail.FileData = package.GetAsByteArray();
                    //byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                    // return data;
                }
                DelailLastMileSendEmail.msExcel = new MemoryStream(DelailLastMileSendEmail.FileData);
                //// _Logger.Info("EndSplitSheet" + DateNow.ToSafeString());
                ////_Logger.Info("StartFileStream" + DateNow.ToSafeString());
                using (FileStream fs = new FileStream(tempPath.Trim() + fileName.Trim(), FileMode.OpenOrCreate))
                {
                    new MemoryStream(DelailLastMileSendEmail.FileData).CopyTo(fs);
                    fs.Flush();

                }
            }
            catch (Exception Ex)
            {
                Ex.Message.ToSafeString();
            }
            //   
            return "Success";
        }
        private async Task DetailLastMileSendtoEmail(string fileName, string Email, string rptCriteria)
        {

            _Logger.Info("Start SendEmail");
            _Logger.Info("Start SendEmail");
            string result = "";
            string body = "";
            string sendto = string.Empty;
            string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            if (Email != "" || Email != null)
            {
                sendto = Email;
            }
            else
            {
                sendto = CurrentUser.Email.ToSafeString();
            }
            var command = new SendMailLastMileNotificationCommand
            {
                ProcessName = "SEND_EMAIL_DETAIL_LASTMILE",
                Subject = "Export DETAIL_LAST_MILE:" + DateNow,
                Body = EmailTemplate(fileName, rptCriteria),
                // msAttachFiles = files,
                SendTo = sendto
                // SendTo = CurrentUser.Email.ToSafeString()
            };
            _sendMail.Handle(command);

            _Logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
            if (command.ReturnMessage == "Success.")
            {
                result = "Success";
            }
            else
            {
                result = command.ReturnMessage;
            }

            //  return result;
        }
        public string EmailTemplate(string fileName, string rptCriteria)
        {

            try
            {
                string tempPath = Path.GetTempPath();
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                StringBuilder tempBody = new StringBuilder();
                CultureInfo ThaiCulture = new CultureInfo("th-TH");
                CultureInfo UsaCulture = new CultureInfo("en-US");


                #region tempBody

                tempBody.Append("<p style='font-weight:bolder;'>เรียน..." + CurrentUser.UserFullNameInThai + "</p>");
                tempBody.Append("<br/>");


                tempBody.Append("<span> ExportDate:" + DateNow.ToSafeString());
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<span>File Export DetailLastMile available for Download :" + fileName);
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                tempBody.Append("<a style='font-weight:bolder;'> Search Criteria By</a>");
                tempBody.Append("<br/>");
                tempBody.Append("<span>" + rptCriteria.ToSafeString());
                tempBody.Append("<br/>");
                tempBody.Append("<a style='font-weight:bolder;'> To DownloadFile here By</a>");
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                // tempBody.Append(tempPath.ToSafeString());
                var url = $"{Request.Url.GetLeftPart(UriPartial.Authority)}{Url.Content("~/")}";
                string urlName = url + "PAYGReport/DownloadFileExport?fileName=" + fileName;
                tempBody.Append("<a href='" + urlName + "'>Download Here</a>");

                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                tempBody.Append("<span>Thanks.");
                tempBody.Append("</span>");


                #endregion
                string body = "";
                body = tempBody.ToSafeString();

                return body;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error ExportFile: " + ex.GetErrorMessage());
                return ex.GetErrorMessage();
            }
        }
        public ActionResult DownloadFileExport(string fileName)
        {
            try
            {
                //   var fileName = $"ReadMe_2.1.txt";
                string tempPath = Path.GetTempPath();
                var filepath = tempPath + fileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                return File(fileBytes, "application/octet-stream", fileName + ".xlsx");
            }
            catch (Exception Ex)
            {
                _Logger.Info("DownLoad: " + Ex.GetErrorMessage());
                return null;
            }
        }

        #endregion
        #region PAYG Report01
        /*******************************************************************************************************************
         * Begin Report Summary Last mile and CPE Transactions (PAYG_Report1)                                              *
         *******************************************************************************************************************/

        public ActionResult LastmileAndCPETransactions(string oltbrand = "", string phase = "", string region = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            SetViewBagLovV2("FBBPAYG_SCREEN", "SUMLASTMILE");

            var rptModel = new LastmileAndCPEReportModel();
            rptModel.OLTBrand = oltbrand;
            rptModel.Phase = phase;
            rptModel.Region = region;

            return View(rptModel);
        }

        public ActionResult ReadReport01Search([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchorpt1Model = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);
                var result = GetDataRpt1SearchModel(searchorpt1Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public JsonResult p_get_vendor_brand()
        {
            var query = new FBBPAYG_SUMLASTMILEQuery
            {
                ProcName = "p_get_vendor_brand"
                ,
                Region = ""
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "All", VALUE = "All" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult p_get_olt_phase()
        {
            var query = new FBBPAYG_SUMLASTMILEQuery
            {
                ProcName = "p_get_olt_phase"
                ,
                Region = ""
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "All", VALUE = "All" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult p_get_region()
        {
            var query = new FBBPAYG_SUMLASTMILEQuery
            {
                ProcName = "p_get_region"
                ,
                Region = ""
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "All", VALUE = "All" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult p_get_product()
        {
            var query = new FBBPAYG_SUMLASTMILEQuery
            {
                ProcName = "p_get_product"
                ,
                Region = ""
            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new FBBPAYG_Dropdown { LOV_NAME = "All", VALUE = "All" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult p_get_address_id_Model([DataSourceRequest] DataSourceRequest request, string Region)
        {
            var query = new FBBPAYG_SUMLASTMILEQuery
            {
                ProcName = "p_get_address_id"
                ,
                Region = Region
            };
            var result = _queryProcessor.Execute(query);

            Session["p_get_address_id_Model"] = result;
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Data_address_id_List(string check = "", string value = "", string from = "", string flag = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            List<FBBPAYG_Dropdown> list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model"];
            int fullLenght = list.Count();
            if (from == "pager")
            {
                if (check == "false" || check == "")
                {
                    if ((List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"] == null)
                    {
                        list = new List<FBBPAYG_Dropdown>();
                    }
                    else
                    {
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                        List<FBBPAYG_Dropdown> lenght = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model"];

                        List<FBBPAYG_Dropdown> temp = new List<FBBPAYG_Dropdown>();
                        List<FBBPAYG_Dropdown> temps = new List<FBBPAYG_Dropdown>();
                        var selected_val = JsonConvert.DeserializeObject<List<string>>(value);
                        if (value != "[]")
                        {
                            for (int i = 0; i < selected_val.Count; i++)
                            {
                                var tee = list.Where(x => x.VALUE.Contains(selected_val[i])).ToList();
                                var te = lenght.Where(x => x.VALUE.Contains(selected_val[i])).ToList();
                                temp.AddRange(tee);
                                temps.AddRange(te);
                            }

                            var text = temp.Where(x => x.VALUE != value).ToList();
                            fullLenght = temps.Count;
                            Session["p_get_address_id_Model_temp"] = temp;
                            list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                        }
                    }
                }
                else if (check == "true")
                {
                    if ((List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"] == null)
                    {
                        list = new List<FBBPAYG_Dropdown>();
                    }
                    else
                    {
                        var lenght = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model"];
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                        List<FBBPAYG_Dropdown> tempp = new List<FBBPAYG_Dropdown>();
                        var selected_val = JsonConvert.DeserializeObject<List<string>>(value);
                        if (value != "[]")
                        {
                            for (int i = 0; i < selected_val.Count; i++)
                            {
                                var tee = lenght.Where(x => x.VALUE.Contains(selected_val[i])).ToList();
                                tempp.AddRange(tee);
                            }

                            var text = tempp.Where(x => x.VALUE != value).ToList();
                            fullLenght = text.Count;
                            Session["p_get_address_id_Model_temp"] = list;
                            list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                        }
                    }
                }
            }
            else if (from == "checkall")
            {
                if (check == "false")
                {
                    if (from == "checkbox" && flag == "false")
                    {
                        var text = list.Where(x => x.VALUE != value).ToList();
                        Session["p_get_address_id_Model"] = text;
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model"];
                    }
                    else if (from == "checkbox" && flag == "true")
                    {
                        var text = list.Where(x => x.VALUE == value).ToList();
                        list = text;
                    }
                    else
                    {
                        list = new List<FBBPAYG_Dropdown>();
                        Session["p_get_address_id_Model_temp"] = list;
                    }
                }
                else
                {
                    if (value != "[]")
                    {
                        List<FBBPAYG_Dropdown> temp = new List<FBBPAYG_Dropdown>();
                        var selected_val = JsonConvert.DeserializeObject<List<string>>(value);

                        for (int i = 0; i < selected_val.Count; i++)
                        {
                            var text = list.Where(x => x.VALUE.Contains(selected_val[i])).ToList();

                            temp.AddRange(text);
                        }
                        list = temp;
                        Session["p_get_address_id_Model_temp"] = list;
                        fullLenght = temp.Count;
                    }
                    else
                    {
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model"];
                        Session["p_get_address_id_Model_temp"] = list;
                    }
                }
            }
            else if (from == "checkbox")
            {
                if (check == "false")
                {
                    List<FBBPAYG_Dropdown> session_temp = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                    if (from == "checkbox" && flag == "false")
                    {
                        List<FBBPAYG_Dropdown> temp = new List<FBBPAYG_Dropdown>();
                        //var selected_val = JsonConvert.DeserializeObject<List<string>>(value);
                        if (value != "")
                        {
                            //for (int i = 0; i < selected_val.Count; i++)
                            //{
                            //    var tee = list.Where(x => x.VALUE.Contains(selected_val[i])).ToList();
                            //    temp.AddRange(tee);
                            //}

                            var text = session_temp.Where(x => x.VALUE != value).ToList();
                            Session["p_get_address_id_Model_temp"] = text;
                            list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                        }
                        else
                        {
                            var text = session_temp.Where(x => x.VALUE != value).ToList();
                            Session["p_get_address_id_Model_temp"] = text;
                            list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                        }
                    }
                    else if (from == "checkbox" && flag == "true")
                    {
                        var text = list.Where(x => x.VALUE == value).ToList();
                        if ((List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"] == null)
                        {
                            session_temp = new List<FBBPAYG_Dropdown>();
                        }
                        session_temp.AddRange(text);
                        Session["p_get_address_id_Model_temp"] = session_temp;
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                    }
                }
                else
                {
                    List<FBBPAYG_Dropdown> session_temp = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                    if (from == "checkbox" && flag == "false")
                    {
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                        fullLenght = list.Count;
                        var text = list.Where(x => x.VALUE != value).ToList();
                        Session["p_get_address_id_Model_temp"] = text;
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                    }
                    else if (from == "checkbox" && flag == "true")
                    {
                        var text = list.Where(x => x.VALUE == value).ToList();
                        if ((List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"] == null)
                        {
                            session_temp = new List<FBBPAYG_Dropdown>();
                        }
                        session_temp.AddRange(text);
                        Session["p_get_address_id_Model_temp"] = session_temp;
                        list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
                    }
                }
            }
            return Json(new { Data = list, isFull = (fullLenght - list.Count() == 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get_Session_Data_address_id_List(string check = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            List<FBBPAYG_Dropdown> list = (List<FBBPAYG_Dropdown>)Session["p_get_address_id_Model_temp"];
            if (list == null)
            {
                list = new List<FBBPAYG_Dropdown>();

            }
            return Json(new { Data = list.OrderBy(x => x.VALUE) }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Clear_Data_address_id_List(string check = "")
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            Session["p_get_address_id_Model_temp"] = new List<FBBPAYG_Dropdown>();

            return Json(new { Data = "x" }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult p_get_address_id_Model([DataSourceRequest] DataSourceRequest request, string Region)
        //{
        //    if (dataS != null && dataS != "")
        //    {
        //        var searchorpt1Model = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);
        //        var result = GetDataRpt1SearchModel(searchorpt1Model);

        //        return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public JsonResult p_get_address_id(string Region)
        {
            var query = new FBBPAYG_SUMLASTMILEQuery
            {
                ProcName = "p_get_address_id"
                ,
                Region = Region
            };
            var data = _queryProcessor.Execute(query);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        private List<LovModel> SelectFbbCfgLov(string lov_type, string lov_val5)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = lov_type,
                LOV_VAL5 = lov_val5
            };
            return _queryProcessor.Execute(query);
        }
        public List<LastmileAndCPEReportList> GetRpt1SearchReqCurStageQueryData(LastmileAndCPETransactionsQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public List<LastmileAndCPEReportList> GetDataRpt1SearchModel(LastmileAndCPEReportModel searchrptModel)
        {
            try
            {
                var dateFrom = !string.IsNullOrEmpty(searchrptModel.DateFrom.ToSafeString()) ? searchrptModel.DateFrom.Value.ToDisplayText("ddMMyyyy") : "";
                var dateTo = !string.IsNullOrEmpty(searchrptModel.DateTo.ToSafeString()) ? searchrptModel.DateTo.Value.ToDisplayText("ddMMyyyy") : "";

                var query = new LastmileAndCPETransactionsQuery()
                {
                    oltbrand = searchrptModel.OLTBrand,
                    phase = searchrptModel.Phase,
                    region = searchrptModel.Region,
                    product = searchrptModel.product,
                    addressid = searchrptModel.addressid,
                    dateFrom = dateFrom,
                    dateTo = dateTo,
                };

                return GetRpt1SearchReqCurStageQueryData(query);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LastmileAndCPEReportList>();
            }
        }

        public ActionResult ExportSumLastMileReport(string dataS, string criteria)
        {
            List<LastmileAndCPEReportList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);
            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            rptCriteria = criteriaModel.CRITERIA;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;

            listall = GetDataRpt1SearchModel(searchModel);


            string filename = GetExcelName("SumLastmileAndCPE");

            var bytes = GeneratePAYGEntitytoExcel<LastmileAndCPEReportList>(listall, filename, "SUMLASTMILE");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public ActionResult ReadSearchSumLastMileReport(string dataS = "")
        {
            //var result = GetDataRpt1SearchModel();

            var searchModel = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);
            var result = GetDataRpt1SearchModel(searchModel);

            string item = "0";

            if (result != null && result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*******************************************************************************************************************
         * End Report Summary Last mile and CPE Transactions (PAYG_Report1)                                                * 
         *******************************************************************************************************************/
        #endregion

        #region PAYG Report02
        /*******************************************************************************************************************
         * Begin Report Detail Last mile and CPE Transactions (PAYG_Report2)                                               *
         *******************************************************************************************************************/

        public ActionResult DetailLastmileAndCPETransactions(string oltbrand = "", string phase = "", string region = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            var query = new GetLovQuery
            {
                LovType = "DETAIL_LASTMILE",
                LovName = "ROLLBACK"
            };

            var Rollback = _queryProcessor.Execute(query).FirstOrDefault();

            // var Rollback = base.LovData.Where(s => s.Type == "DETAIL_LASTMILE" && s.Name == "ROLLBACK").FirstOrDefault();
            activeflag = Rollback != null ? Rollback.ActiveFlag : "N";

            result = null;
            searchModel = null;

            SetViewBagLovV2("FBBPAYG_SCREEN", "DETAILLASTMILE");
            var rptModel = new LastmileAndCPEReportModel();
            rptModel.OLTBrand = oltbrand;
            rptModel.Phase = phase;
            rptModel.Region = region;

            return View(rptModel);
        }

        public ActionResult ReadReport02Search([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchorpt2Model = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);
                var statusTopup = base.LovData
               .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "DETAILLASTMILE" && r.Name.Contains("TOPUP_"))
               .OrderBy(o => o.OrderBy).ToList();

                //------------Rollback Y => ใช้ของเดิม -------------------
                //------------Rollback N => ใช้ของใหม่ -------------------
                //------------Rollback Null => ต้องใช้ของใหม่ -------------

                var checksearch = false;
                if (searchModel == null)
                {
                    searchModel = searchorpt2Model;
                }
                else
                {
                    if (searchModel.OLTBrand != searchorpt2Model.OLTBrand ||
                        searchModel.Phase != searchorpt2Model.Phase ||
                        searchModel.Region != searchorpt2Model.Region ||
                        searchModel.product != searchorpt2Model.product ||
                        searchModel.addressid != searchorpt2Model.addressid ||
                        searchModel.DateFrom.ToString() != searchorpt2Model.DateFrom.ToString() ||
                        searchModel.DateTo.ToString() != searchorpt2Model.DateTo.ToString())
                    {
                        checksearch = true;
                    }
                    if (checksearch)
                    {
                        searchModel = searchorpt2Model;
                        result = null;
                    }
                }
                if (activeflag == "Y")  // Rollback.ActiveFlag => Y
                {
                    result = GetDataRpt2SearchModel(searchorpt2Model); //create date : 01/10/2021	
                    var dataDetailLastmileforGrid = SetDetailLastmileforGrid(result, statusTopup);

                    if (result.Count > 0)
                    {
                        return Json(new
                        {
                            Data = dataDetailLastmileforGrid,
                            Total = result[0].CNT
                        });
                    }
                }
                else // Rollback.ActiveFlag => N
                {
                    if (result != null)
                    {
                        if (result.Count > 0 && !checksearch)
                        {
                            var data = SetDetailLastmileforGrid(result, statusTopup);
                            return Json(new
                            {
                                Data = data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToArray(),
                                Total = result.Count
                            });

                        }
                        else
                        {
                            result = GetDataRpt2SearchModelNew(searchorpt2Model);
                            var data = SetDetailLastmileforGrid(result, statusTopup);
                            return Json(new
                            {
                                Data = data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize),
                                Total = result.Count
                            });
                        }
                    }
                    else  //first loop 
                    {
                        result = GetDataRpt2SearchModelNew(searchorpt2Model);
                        var data = SetDetailLastmileforGrid(result, statusTopup);
                        var total = 0;
                        if (result.Count != 0)
                        {
                            total = result.Count;
                        }
                        return Json(new
                        {
                            Data = data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize),
                            Total = total
                        });
                    }

                }

                return null;
                //return Json(result2Show.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<DetailLastmileAndCPEReportGridList> SetDetailLastmileforGrid(List<DetailLastmileAndCPEReportList> result, List<LovValueModel> statusTopup)
        {
            return (from temp in result
                    select new DetailLastmileAndCPEReportGridList
                    {
                        INDOOR_ONT_STATUS = (temp.INDOOR_ONT_STATUS == "W") ? statusTopup[0].LovValue1.ToSafeString() : (temp.INDOOR_ONT_STATUS == "Y") ? statusTopup[1].LovValue1.ToSafeString() : (temp.INDOOR_ONT_STATUS == "N") ? statusTopup[2].LovValue1.ToSafeString() : temp.INDOOR_ONT_STATUS.ToSafeString(),
                        OUTDOO_STATUS = (temp.OUTDOO_STATUS == "W") ? statusTopup[0].LovValue1.ToSafeString() : (temp.OUTDOO_STATUS == "Y") ? statusTopup[1].LovValue1.ToSafeString() : (temp.OUTDOO_STATUS == "N") ? statusTopup[2].LovValue1.ToSafeString() : temp.OUTDOO_STATUS.ToSafeString(),
                        ONT_STATUS = (temp.ONT_STATUS == "W") ? statusTopup[0].LovValue1.ToSafeString() : (temp.ONT_STATUS == "Y") ? statusTopup[1].LovValue1.ToSafeString() : (temp.ONT_STATUS == "N") ? statusTopup[2].LovValue1.ToSafeString() : temp.ONT_STATUS.ToSafeString(),

                        CUST_REQUST_DT = temp.CUST_REQUST_DT.ToSafeString(), //ToDisplayText()
                        CUST_REGISTER_DT = temp.CUST_REGISTER_DT.ToSafeString(), //.ToDisplayText(),
                        CS_APPROVE_DT = temp.CS_APPROVE_DT.ToSafeString(), //.ToDisplayText(),
                                                                           //CS_APPROVE_DT = temp.CS_APPROVE_DT.ToDateDisplayText(),
                        ACTIVITY = temp.ACTIVITY,
                        FIBRENET_ID = temp.FIBRENET_ID,
                        CUST_NAME = temp.CUST_NAME,
                        CUST_STATUS = temp.CUST_STATUS,
                        CUST_STATUS_DT = temp.CUST_STATUS_DT.ToSafeString(), //.ToDisplayText(),
                        PACKAGE_NAME = temp.PACKAGE_NAME,
                        PACKAGE_CHANGE_DT = temp.PACKAGE_CHANGE_DT.ToSafeString(), //.ToDisplayText(),//(temp.PACKAGE_CHANGE_DT == null) ? (DateTime?)null : DateTime.ParseExact(temp.PACKAGE_CHANGE_DT.ToDisplayText(), "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None),
                        ADDRESS_ID = temp.ADDRESS_ID,
                        BUILDING_NAME = temp.BUILDING_NAME,
                        BUSINESS_MODEL = temp.BUSINESS_MODEL,
                        PRODUCT = temp.PRODUCT,
                        SERVICE = temp.SERVICE,
                        SUBCONTRACTOR_NAME = temp.SUBCONTRACTOR_NAME,
                        REGION = temp.REGION,
                        PHASE = temp.PHASE,
                        OLT_VENDOR = temp.OLT_VENDOR,
                        OLT_NAME = temp.OLT_NAME,
                        DISTRICT = temp.DISTRICT,
                        SUB_DISTRICT = temp.SUB_DISTRICT,
                        SITE_NAME = temp.SITE_NAME,
                        SITE_NO = temp.SITE_NO,
                        SPLITTER1 = temp.SPLITTER1,
                        SPLITTER2 = temp.SPLITTER2,
                        INDOOR_INVOICE = temp.INDOOR_INVOICE,
                        INDOOR_ONT_INVOICE_DATE = temp.INDOOR_ONT_INVOICE_DATE.ToSafeString(), //.ToDisplayText(),
                        INDOOR_ONT_PO = temp.INDOOR_ONT_PO,
                        OUTDOOR_INVOICE = temp.OUTDOOR_INVOICE,
                        OUTDOO_INVOICE_DATE = temp.OUTDOO_INVOICE_DATE.ToSafeString(), //.ToDisplayText(),
                        OUTDOO_PO = temp.OUTDOO_PO,
                        SUBCONTRACT_IN = temp.SUBCONTRACT_IN,
                        //ONT_VENDOR = temp.ONT_VENDOR,
                        //ONT_MODEL = temp.ONT_MODEL,
                        //ONT_SERIAL_NO = temp.ONT_SERIAL_NO,
                        CPE_SN = temp.CPE_SN,
                        WIFI_ROUTER_SN = temp.WIFI_ROUTER_SN,
                        ATA_SN = temp.ATA_SN,
                        STB_SN1 = temp.STB_SN1,
                        STB_SN2 = temp.STB_SN2,
                        STB_SN3 = temp.STB_SN3,
                        ONT_INVOICE = temp.ONT_INVOICE,
                        ONT_INVOICE_DATE = temp.ONT_INVOICE_DATE.ToSafeString(), //.ToDisplayText(),
                        ONT_PO = temp.ONT_PO

                    }).ToList();

        }

        public List<DetailLastmileAndCPEReportList> GetRpt2SearchReqCurStageQueryData(DetailLastMilecpeAddressidListQuery query)
        {
            var result = _queryProcessor.Execute(query);
            return result;
        }

        public List<DetailLastmileAndCPEReportList> GetDataRpt2SearchModel(LastmileAndCPEReportModel searchrptModel)
        {
            try
            {
                var dateFrom = !string.IsNullOrEmpty(searchrptModel.DateFrom.ToSafeString()) ? searchrptModel.DateFrom.Value.ToDisplayText("ddMMyyyy") : "";
                var dateTo = !string.IsNullOrEmpty(searchrptModel.DateTo.ToSafeString()) ? searchrptModel.DateTo.Value.ToDisplayText("ddMMyyyy") : "";

                var ltaddress = new List<string>();
                ltaddress.Add("");

                var query = new DetailLastMilecpeAddressidListQuery()
                {
                    oltbrand = searchrptModel.OLTBrand,
                    phase = searchrptModel.Phase,
                    region = searchrptModel.Region,
                    product = searchrptModel.product,
                    addressid = searchrptModel.addressid,
                    dateFrom = dateFrom,
                    dateTo = dateTo,
                    address = ltaddress,
                    P_PAGE_INDEX = searchrptModel.P_PAGE_INDEX,
                    P_PAGE_SIZE = searchrptModel.P_PAGE_SIZE
                };

                var result = GetRpt2SearchReqCurStageQueryData(query);
                if (result.Count > 0 || result != null)
                {
                    _Logger.Info("DetailLastmileReturn:" + result.Count);
                }
                else
                {
                    _Logger.Info("DetailLastmileReturn null");
                }
                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info("DetailLastmile" + ex.GetErrorMessage());
                return new List<DetailLastmileAndCPEReportList>();
            }
        }

        public List<DetailLastmileAndCPEReportList> GetDataRpt2SearchModelNew(LastmileAndCPEReportModel searchrptModel)
        {
            try
            {
                var dateFrom = !string.IsNullOrEmpty(searchrptModel.DateFrom.ToSafeString()) ? searchrptModel.DateFrom.Value.ToDisplayText("ddMMyyyy") : "";
                var dateTo = !string.IsNullOrEmpty(searchrptModel.DateTo.ToSafeString()) ? searchrptModel.DateTo.Value.ToDisplayText("ddMMyyyy") : "";
                var temp = new List<AddressidListQueryModel>();

                var ltaddress = (from item in searchrptModel.addressid.Split(',')
                                 select item).ToList();
                var tmpaddressid = "ALL";
                if (ltaddress[0].Contains("ALL"))
                    ltaddress[0] = "";
                else
                    tmpaddressid = null;

                var query = new DetailLastMilecpeAddressidListQuery()
                {
                    oltbrand = searchrptModel.OLTBrand,
                    phase = searchrptModel.Phase,
                    region = searchrptModel.Region,
                    product = searchrptModel.product,
                    addressid = tmpaddressid,
                    address = ltaddress,
                    dateFrom = dateFrom,
                    dateTo = dateTo,
                    P_PAGE_INDEX = 1,
                    P_PAGE_SIZE = 1000
                };

                var result = GetRpt2SearchReqCurStageQueryData(query);
                if (result != null)
                {
                    _Logger.Info("DetailLastmileReturn:" + result.Count);
                }
                else
                {
                    _Logger.Info("DetailLastmileReturn null");
                }
                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info("DetailLastmile" + ex.GetErrorMessage());
                return new List<DetailLastmileAndCPEReportList>();
            }
        }

        public ActionResult ExportDetailLastMileReport(string dataS, string criteria)
        {
            List<DetailLastmileAndCPEReportList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);

            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            rptCriteria = criteriaModel.CRITERIA;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;

            listall = GetDataRpt2SearchModel(searchModel);


            string filename = GetExcelName("DetailLastmileAndCPE");

            var bytes = GeneratePAYGEntitytoExcel<DetailLastmileAndCPEReportList>(listall, filename, "DETAILLASTMILE");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public ActionResult ReadSearchDetailLastMileReport(string dataS = "")
        {
            //var result = GetDataRpt1SearchModel();

            var searchModel = new JavaScriptSerializer().Deserialize<LastmileAndCPEReportModel>(dataS);
            var result = GetDataRpt2SearchModel(searchModel);

            string item = "0";

            if (result != null && result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }
        /*******************************************************************************************************************
         * End Report Detail Last mile and CPE Transactions (PAYG_Report2)                                                 *      
         *******************************************************************************************************************/
        #endregion

        #region PAYG Report03
        /*******************************************************************************************************************
         * Begin Report Input File Load (PAYG_Report3)                                                                     *
         *******************************************************************************************************************/

        public ActionResult InputFileLoad()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;


            return View();
        }

        /*******************************************************************************************************************
         * End Report Input File Load (PAYG_Report3)                                                                       *
         *******************************************************************************************************************/
        #endregion

        #region PAYG Report04
        /*******************************************************************************************************************
         * Begin Report Screen  Update Paid Status (PAYG_Report4)                                                          *
         *******************************************************************************************************************/

        public ActionResult UpdatePaidStatus()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            SetViewBagLov("FBBPAYG_SCREEN", "PAIDSTATUS");

            return View();
        }

        public ActionResult ReadReport04Search([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt4Model = new JavaScriptSerializer().Deserialize<UpDateScreenModel>(dataS);
                var result = GetDataRpt04SearchModel(searchrpt4Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var searchoawcModel = new JavaScriptSerializer().Deserialize<AWCModel>(dataS);
                //var result = GetDataSearchModel(searchoawcModel);
                //return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                return null;
            }
        }

        public List<UpdateScreenList> GetDataRpt04SearchModel(UpDateScreenModel searchrptModel)
        {
            /*DateTime dttmp;
            Nullable<DateTime> dtstartdate = DateTime.TryParseExact(searchrptModel.DateFrom.ToDateTimeDisplayText(), Constants.DisplayFormats.DateTimeFormatNoSecond, CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp : (DateTime?)null;
            Nullable<DateTime> dtexpireddate = DateTime.TryParseExact(searchrptModel.DateTo.ToDateTimeDisplayText(), Constants.DisplayFormats.DateTimeFormatNoSecond, CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp : (DateTime?)null;*/

            var dateFrom = !string.IsNullOrEmpty(searchrptModel.DateFrom.ToSafeString()) ? searchrptModel.DateFrom.ToSafeString() : "";
            var dateTo = !string.IsNullOrEmpty(searchrptModel.DateTo.ToSafeString()) ? searchrptModel.DateTo.ToSafeString() : "";

            var query = new UpdateScreenQuery()
            {
                InternatNo = searchrptModel.InternatNo,
                PO = searchrptModel.PO,
                Invoice = searchrptModel.Invoice,
                DateFrom = dateFrom,
                DateTo = dateTo,
                CHKINDOOR = searchrptModel.CHKINDOOR,
                CHKOUTDOOR = searchrptModel.CHKOUTDOOR,
                CHKONT = searchrptModel.CHKONT

            };
            return GetRpt04SearchReqCurStageQueryData(query);
        }

        public List<UpdateScreenList> GetRpt04SearchReqCurStageQueryData(UpdateScreenQuery query)
        {
            //return _queryProcessor.Execute(query);
            List<UpdateScreenList> result;
            result = _queryProcessor.Execute(query);
            if (result != null && result.Count > 0)
            {
                int i = 0;
                foreach (var item in result)
                {
                    result[i].PAID_ST_IN_OLD = item.PAID_ST_IN;
                    result[i].PAID_ST_OUT_OLD = item.PAID_ST_OUT;
                    result[i].PAID_ST_ONT_OLD = item.PAID_ST_ONT;
                    i++;
                }

            }
            return result;
        }

        public ActionResult GetExportScreenToExcel(string dataS, string criteria)
        {
            List<UpdateScreenList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<UpDateScreenModel>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            listall = GetExportScreen(searchModel);

            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaScreenModel>(criteria);

            rptCriteria = criteriaModel.PAIDFOR;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;
            rptCriterias.Add("Internet No : " + searchModel.InternatNo);
            rptCriterias.Add("PO : " + searchModel.PO);
            rptCriterias.Add("Invoice : " + searchModel.Invoice);
            rptCriterias.Add("Invoice Date From : " + searchModel.DateFrom + "   To :   " + searchModel.DateTo);

            showInDoor = searchModel.CHKINDOOR == 1;
            showOutDoor = searchModel.CHKOUTDOOR == 1;
            showONT = searchModel.CHKONT == 1;

            string filename = GetExcelName("UpdateScreen");

            var bytes = GeneratePAYGEntitytoExcel<UpdateScreenList>(listall, filename, "PAIDSTATUS");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public List<UpdateScreenList> GetExportScreen(UpDateScreenModel searchModel)
        {

            try
            {
                var dateFrom = !string.IsNullOrEmpty(searchModel.DateFrom.ToSafeString()) ? searchModel.DateFrom.ToSafeString() : "";
                var dateTo = !string.IsNullOrEmpty(searchModel.DateTo.ToSafeString()) ? searchModel.DateTo.ToSafeString() : "";

                var query = new UpdateScreenQuery
                {
                    InternatNo = searchModel.InternatNo,
                    PO = searchModel.PO,
                    Invoice = searchModel.Invoice,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CHKINDOOR = searchModel.CHKINDOOR,
                    CHKOUTDOOR = searchModel.CHKOUTDOOR,
                    CHKONT = searchModel.CHKONT
                };

                return _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<UpdateScreenList>();
            }

        }

        public ActionResult ReadSearchScreen(string dataS = "")
        {

            var searchoawcModel = new JavaScriptSerializer().Deserialize<UpDateScreenModel>(dataS);
            var result = GetDataRpt04SearchModel(searchoawcModel);

            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            ViewBag.ActiveIndoor = (searchoawcModel.InternatNo != null) ? searchoawcModel.CHKINDOOR == 1 : true;
            ViewBag.ActiveOutdoor = (searchoawcModel.InternatNo != null) ? searchoawcModel.CHKOUTDOOR == 1 : true;
            ViewBag.ActiveONT = (searchoawcModel.InternatNo != null) ? searchoawcModel.CHKONT == 1 : true;

            SetViewBagLov("FBBPAYG_SCREEN", "PAIDSTATUS");
            return PartialView("~/Views/PAYGReport/_GridReport.cshtml", searchoawcModel);
        }

        public JsonResult GetStatusTopUp()
        {
            var statusTopup = new List<DropdownModel>();
            try
            {
                statusTopup = base.LovData
                    .Where(r => r.Type == "FBBPAYG_SCREEN" && r.LovValue5 == "PAIDSTATUS" && r.Name.Contains("TOPUP_"))
                    .OrderBy(o => o.OrderBy)
                    .Select(l => new DropdownModel
                    {
                        Text = l.LovValue1,
                        Value = l.Text,
                    }).ToList();
            }
            catch (Exception) { }
            return Json(statusTopup, JsonRequestBehavior.AllowGet);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //[HttpPost]
        public ActionResult UpdatePaidStatusData(List<UpdatePaidStatusDataModel> saveModels)
        {
            string item = "0";
            bool result = true;

            if (saveModels != null)
            {
                //var saveData;
                foreach (var saveModel in saveModels)
                {
                    if (saveModel.CHECKED_IN)
                    {
                        if (saveModel.PAID_ST_IN != null)
                            result = UpdateData(saveModel.FIBRENET_ID, saveModel.INVOICE_NO_IN, saveModel.PO_NO_IN, "LAST MILE INDOOR", saveModel.PAID_ST_IN, saveModel.REMARK_IN);

                        if (!result)
                            break;
                    }

                    if (saveModel.CHECKED_OUT)
                    {
                        if (saveModel.PAID_ST_OUT != null)
                            result = UpdateData(saveModel.FIBRENET_ID, saveModel.INVOICE_NO_OUT, saveModel.PO_NO_OUT, "LAST MILE OUTDOOR", saveModel.PAID_ST_OUT, saveModel.REMARK_OUT);

                        if (!result)
                            break;
                    }

                    if (saveModel.CHECKED_ONT)
                    {
                        if (saveModel.PAID_ST_ONT != null)
                            result = UpdateData(saveModel.FIBRENET_ID, saveModel.INVOICE_NO_ONT, saveModel.PO_NO_ONT, "ONT", saveModel.PAID_ST_ONT, saveModel.REMARK_ONT);

                        if (!result)
                            break;
                    }

                    if (!result)
                        break;
                }

                if (result)
                    item = "1";
            }

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        public bool UpdateData(string fibreID, string invoiceNo, string poNo, string deviceType, string paidST, string remark)
        {
            var command = new UpdatePaidStatusDataCommand()
            {
                FIBRENET_ID = fibreID,
                INVOICE_NO = invoiceNo,
                PO_NO = poNo,
                DEVICE_TYPE = deviceType,
                PAID_ST = paidST,
                REMARK = remark
            };

            _UpdatePaidStatusCommand.Handle(command);
            return true;
        }

        public ActionResult UpdatePaidStatusDataByStatus(List<UpdatePaidStatusDataModel> saveModels, int saveOption)
        {
            string item = "0";
            bool result = true;

            if (saveModels != null)
            {
                //var saveData;
                foreach (var saveModel in saveModels)
                {
                    switch (saveOption)
                    {
                        case 1:
                            if (saveModel.PAID_ST_IN != null)
                                result = UpdateData(saveModel.FIBRENET_ID, saveModel.INVOICE_NO_IN, saveModel.PO_NO_IN, "LAST MILE INDOOR", saveModel.PAID_ST_IN, saveModel.REMARK_IN);

                            break;

                        case 2:
                            if (saveModel.PAID_ST_OUT != null)
                                result = UpdateData(saveModel.FIBRENET_ID, saveModel.INVOICE_NO_OUT, saveModel.PO_NO_OUT, "LAST MILE OUTDOOR", saveModel.PAID_ST_OUT, saveModel.REMARK_OUT);

                            break;

                        case 3:
                            if (saveModel.PAID_ST_ONT != null)
                                result = UpdateData(saveModel.FIBRENET_ID, saveModel.INVOICE_NO_ONT, saveModel.PO_NO_ONT, "ONT", saveModel.PAID_ST_ONT, saveModel.REMARK_ONT);

                            break;
                        default:
                            break;
                    }

                    if (!result)
                        break;
                }

                if (result)
                    item = "1";
            }

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*public ActionResult ImportFile(HttpPostedFileBase fileName)
        {
            string strResult = string.Empty;


            //HttpPostedFileBase importFile1 = fileName;

            //FileInfo importFile = new FileInfo(fileName);
            var name = Path.GetFileName(fileName.FileName);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var destinationPath = Path.Combine(Server.MapPath("~/App_Data"), name);
            fileName.SaveAs(destinationPath);
            //importFile.CopyTo(destinationPath, true);
            FileInfo fInfo = new FileInfo(destinationPath);
            long size = fInfo.Length;

            var result = new List<ImportPaidStatusCommand>();
            string tempError = "Upload Status: {0}<br/><br/>Import date : {1}<br/>File name : {2}<br/>Number of record : {3}<br/>Success = {4}<br/>Failed = {5} <br/><br/>{6} ";
            string tempMsgError = string.Empty;
            int total = 0;
            int success = 0;
            int error = 0;
            string errorMsg = string.Empty;

            if (size > 10000000)
            {
                var modelResponse2 = new { status = false, message = "File's exceeded", filename = fileName };
                return Json(modelResponse2, "text/plain");
            }
            else
            {
                DataTable table = ConvertCSVtoDataTable(destinationPath);
                table = RemoveAllNullRowsFromDataTable(table);

                string[] formats = { "dd/MM/yyyy" };

                var date = new DateTime();

                foreach (DataRow dRow in table.Rows)
                {
                    var data = DateTime.TryParseExact(dRow["Invoice date"].ToString(), Constants.DisplayFormats.DateTimeFormatNoSecond,
                                Constants.DisplayFormats.DefaultCultureInfo,
                                System.Globalization.DateTimeStyles.None, out date);

                    var command = new ImportPaidStatusCommand()
                    {
                        FIBRENET_ID = dRow["Internet No."].ToString(),
                        INVOICE_NO = dRow["Invoice No."].ToString(),
                        INVOICE_DT = date,//DateTime.Parse(dRow["Invoice date"].ToString()),
                        PO_NO = dRow["PO Number"].ToString(),
                        DEVICE_TYPE = dRow["Device type"].ToString().ToUpper(),
                        PAID_ST = dRow["Paid Status"].ToString(),
                        REMARK = dRow["Remark"].ToString()

                    };

                    _ImportPaidStatusCommand.Handle(command);

                    total++;

                    if (string.Compare(command.Return_Code, "1") == 0)
                    {
                        success++;
                    }
                    else
                    {
                        error++;
                        tempMsgError += "<br/>" + command.Return_Message; 
                    }


                }

                tempError = string.Format(tempError, "", DateTime.Now.ToDateDisplayText(), name, total.ToSafeString(), success.ToSafeString(), error.ToSafeString(), tempMsgError);

                //Session["ERRORIMPORT"] = null;
                Session.Remove("ImportData");
                Session["ImportData"] = tempError;

                stopwatch.Stop();
                var sds = stopwatch.Elapsed;
                var modelResponse = new { status = true, message = tempError, filename = fileName };
                return Json(modelResponse, "text/plain");
            }
        }*/

        public DataTable ConvertCSVtoDataTable(string path)
        {
            DataTable csvData = new DataTable();
            StreamReader sr = new StreamReader(path);
            try
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    csvData.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = csvData.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    csvData.Rows.Add(dr);
                }

                sr.Close();
            }
            catch (Exception ex)
            {
                sr.Close();
            }

            return csvData;
        }

        public DataTable ConvertExcelToDataSet(string path)
        {
            // Get the Excel file and convert to dataset 
            DataTable res = null;
            DataSet dataSet = new DataSet();
            IExcelDataReader iExcelDataReader = null;
            FileStream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read);
            if (path.EndsWith("xls"))
                iExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
            if (path.EndsWith("xlsx"))
                iExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            stream.Dispose();
            if (iExcelDataReader != null)
            {
                iExcelDataReader.IsFirstRowAsColumnNames = true;

                dataSet = iExcelDataReader.AsDataSet();

                iExcelDataReader.Close();
            }

            if (dataSet != null && dataSet.Tables.Count > 0)
            {

                res = dataSet.Tables[0];

            }
            return res;
        }

        public static DataTable RemoveAllNullRowsFromDataTable(DataTable dt)
        {
            int columnCount = dt.Columns.Count;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                bool allNull = true;
                for (int j = 0; j < columnCount; j++)
                {
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        allNull = false;
                    }
                }
                if (allNull)
                {
                    dt.Rows[i].Delete();
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        public ActionResult clearSession()
        {
            var filename = Session["filename"];
            if (filename != null)
                Session.Remove("filename");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                string tempError = "Upload Status: {0}<br/><br/>Import date : {1}<br/>File name : {2}<br/>Number of record : {3}<br/>Success = {4}<br/>Failed = {5} <br/><br/>{6} ";
                string tempMsgError = string.Empty;
                int total = 0;
                int success = 0;
                int error = 0;
                string errorMsg = string.Empty;
                try
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path. We only care about the file name.
                        var fileName = Path.GetFileName(file.FileName);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        if (fileName.EndsWith("csv") || fileName.EndsWith("xls") || fileName.EndsWith("xlsx"))
                        {
                            var destinationPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                            file.SaveAs(destinationPath);
                            FileInfo fInfo = new FileInfo(destinationPath);
                            long size = fInfo.Length;
                            if (size > 10000000)
                            {
                                var modelResponse2 = new { status = false, message = "File's exceeded", filename = fileName };
                                return Json(modelResponse2, "text/plain");
                            }
                            else
                            {
                                DataTable table;

                                if (fileName.EndsWith("csv"))
                                    table = ConvertCSVtoDataTable(destinationPath);
                                else
                                    table = ConvertExcelToDataSet(destinationPath);

                                table = RemoveAllNullRowsFromDataTable(table);

                                var date = new DateTime();
                                DateTime? convertDate = null;

                                foreach (DataRow dRow in table.Rows)
                                {
                                    if (fileName.EndsWith("xls"))
                                    {
                                        if (dRow[2].GetType() == typeof(string))
                                        {
                                            var tempp = dRow[2].ToString().Split(' ');
                                            var data = true;

                                            if (tempp[0].Length >= 10)
                                                data = DateTime.TryParseExact(tempp[0].ToString(), Constants.DisplayFormats.DateFormat,
                                                    Constants.DisplayFormats.DefaultCultureInfo,
                                                    System.Globalization.DateTimeStyles.None, out date);
                                            else if (tempp[0].Length != 0)
                                                data = DateTime.TryParseExact(tempp[0].ToString(), "d/M/yyyy",
                                                    Constants.DisplayFormats.DefaultCultureInfo,
                                                    System.Globalization.DateTimeStyles.None, out date);

                                            if (tempp[0].Length != 0)
                                                convertDate = date;
                                        }
                                        else
                                        {
                                            convertDate = (dRow[2].ToString().Length != 0) ? DateTime.FromOADate(Convert.ToDouble(dRow[2].ToSafeString())) : (DateTime?)null;
                                        }
                                    }
                                    else if (fileName.EndsWith("xlsx"))
                                    {
                                        convertDate = (dRow[2].ToString().Length != 0) ? (DateTime)dRow[2] : (DateTime?)null;
                                    }
                                    else //csv
                                    {
                                        var tempp = dRow[2].ToString().Split(' ');
                                        var data = true;

                                        if (tempp[0].Length >= 10)
                                            data = DateTime.TryParseExact(tempp[0].ToString(), Constants.DisplayFormats.DateFormat,
                                                Constants.DisplayFormats.DefaultCultureInfo,
                                                System.Globalization.DateTimeStyles.None, out date);
                                        else if (tempp[0].Length != 0)
                                            data = DateTime.TryParseExact(tempp[0].ToString(), "d/M/yyyy",
                                                Constants.DisplayFormats.DefaultCultureInfo,
                                                System.Globalization.DateTimeStyles.None, out date);

                                        if (tempp[0].Length != 0)
                                            convertDate = date;
                                    }

                                    var command = new ImportPaidStatusCommand()
                                    {
                                        FIBRENET_ID = dRow[0].ToString(),
                                        INVOICE_NO = dRow[1].ToString(),
                                        INVOICE_DT = convertDate,
                                        PO_NO = dRow[3].ToString(),
                                        DEVICE_TYPE = dRow[5].ToString().ToUpper(),
                                        PAID_ST = dRow[4].ToString(),
                                        REMARK = dRow[6].ToString()

                                    };

                                    _ImportPaidStatusCommand.Handle(command);

                                    total++;

                                    if (string.Compare(command.Return_Code, "0") == 0)
                                    {
                                        success++;
                                    }
                                    else
                                    {
                                        error++;
                                        tempMsgError += "<br/>" + command.Return_Message;
                                    }

                                }

                                tempError = string.Format(tempError, "", DateTime.Now.ToDateDisplayText(), fileName, total.ToSafeString(), success.ToSafeString(), error.ToSafeString(), tempMsgError);

                                Session.Remove("ImportData");
                                Session["ImportData"] = tempError;

                                stopwatch.Stop();
                                var sds = stopwatch.Elapsed;
                                var modelResponse = new { status = true, message = tempError, filename = fileName };
                                return Json(modelResponse, "text/plain");
                            }

                        }
                        else
                        {
                            var modelResponse = new { status = false, message = "File Format type Error", filename = fileName };
                            return Json(modelResponse, "text/plain");
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("does not belong to table"))
                    {
                        var modelResponse = new { status = false, message = "This file has missing field format", filename = "" };
                        return Json(modelResponse, "text/plain");
                    }
                    var modelResponse2 = new { status = false, message = e.Message, filename = "" };
                    return Json(modelResponse2, "text/plain");
                }

            }


            return Content("");
        }

        /*******************************************************************************************************************
         * End Report Screen  Update Paid Status (PAYG_Report4)                                                            *
         *******************************************************************************************************************/
        #endregion

        #region PAYG Report05
        /*******************************************************************************************************************
         * Begin Report OLT (PAYG_Report5)                                                                                 *
         *******************************************************************************************************************/

        public ActionResult OLTReport()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLovV2("FBBPAYG_SCREEN", "REPORTOLT");

            return View();
        }

        public ActionResult ReadReport05Search([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt5Model = new JavaScriptSerializer().Deserialize<PAYGSearchDateModel>(dataS);
                var result = GetDataRpt5SearchModel(searchrpt5Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public List<OLTList> GetDataRpt5SearchModel(PAYGSearchDateModel searchrptModel)
        {
            try
            {
                var dateFrom = !string.IsNullOrEmpty(searchrptModel.DateFrom.ToSafeString()) ? searchrptModel.DateFrom.Value.ToDisplayText("ddMMyyyy") : "";
                var dateTo = !string.IsNullOrEmpty(searchrptModel.DateTo.ToSafeString()) ? searchrptModel.DateTo.Value.ToDisplayText("ddMMyyyy") : "";

                var query = new OLTQuery()
                {
                    dateFrom = dateFrom,
                    dateTo = dateTo
                };

                return GetRpt5SearchReqCurStageQueryData(query);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<OLTList>();
            }
        }

        public List<OLTList> GetRpt5SearchReqCurStageQueryData(OLTQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportOLTReport(string dataS, string criteria)
        {
            List<OLTList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<PAYGSearchDateModel>(dataS);
            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            rptCriteria = criteriaModel.CRITERIA;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;

            listall = GetDataRpt5SearchModel(searchModel);


            string filename = GetExcelName("OLTReport");

            var bytes = GeneratePAYGEntitytoExcel<OLTList>(listall, filename, "REPORTOLT");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public ActionResult ReadSearchOLTReport(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<PAYGSearchDateModel>(dataS);
            var result = GetDataRpt5SearchModel(searchModel);

            string item = "0";

            if (result != null && result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateOLTStatusData(List<UpdateOLTStatusModel> saveModels)
        {
            string item = "0";
            //bool result = true;

            if (saveModels != null)
            {
                //var saveData;
                foreach (var saveModel in saveModels)
                {
                    if (saveModel.OLT_STATUS != null)
                    {
                        var command = new UpdateOLTStatusCommand()
                        {
                            OLT_NAME = saveModel.OLT_NAME,
                            OLT_STATUS = saveModel.OLT_STATUS
                        };

                        _UpdateOLTStatusCommand.Handle(command);

                        //result = command.Return_Code;
                    }
                }

                //if (result)
                item = "1";
            }

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*******************************************************************************************************************
         * End Report OLT (PAYG_Report5)                                                                                   *                                                     
         *******************************************************************************************************************/
        #endregion

        #region PAYG Report06
        /*******************************************************************************************************************
         * Begin Report OSP (PAYG_Report6)                                                                                 *
         *******************************************************************************************************************/

        public ActionResult OSPReport()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLovV2("FBBPAYG_SCREEN", "REPORTOSP");

            return View();
        }

        public ActionResult ReadReport06Search([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt6Model = new JavaScriptSerializer().Deserialize<PAYGSearchDateModel>(dataS);
                var result = GetDataRpt6SearchModel(searchrpt6Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }

        public List<OSPList> GetDataRpt6SearchModel(PAYGSearchDateModel searchrptModel)
        {
            try
            {
                var dateFrom = !string.IsNullOrEmpty(searchrptModel.DateFrom.ToSafeString()) ? searchrptModel.DateFrom.Value.ToDisplayText("ddMMyyyy") : "";
                var dateTo = !string.IsNullOrEmpty(searchrptModel.DateTo.ToSafeString()) ? searchrptModel.DateTo.Value.ToDisplayText("ddMMyyyy") : "";

                var query = new OSPQuery()
                {
                    dateFrom = dateFrom,
                    dateTo = dateTo
                };

                return GetRpt6SearchReqCurStageQueryData(query);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<OSPList>();
            }
        }

        public List<OSPList> GetRpt6SearchReqCurStageQueryData(OSPQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportOSPReport(string dataS, string criteria)
        {
            List<OSPList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<PAYGSearchDateModel>(dataS);
            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            rptCriteria = criteriaModel.CRITERIA;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;

            listall = GetDataRpt6SearchModel(searchModel);


            string filename = GetExcelName("OSPReport");

            var bytes = GeneratePAYGEntitytoExcel<OSPList>(listall, filename, "REPORTOSP");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public ActionResult ReadSearchOSPReport(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<PAYGSearchDateModel>(dataS);
            var result = GetDataRpt6SearchModel(searchModel);

            string item = "0";

            if (result != null && result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*******************************************************************************************************************
         * End Report OSP (PAYG_Report6)                                                                                   *
         *******************************************************************************************************************/
        #endregion


        #region PAYG Report07
        /*******************************************************************************************************************
         * Begin Report StandardFullConnection (PAYG_Report7)                                                                                 *
         *******************************************************************************************************************/

        public ActionResult StandardFullConnection()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBPAYG_SCREEN", "REPORTFULLCON");

            return View();
        }

        public ActionResult ReadReport07Search([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt7Model = new JavaScriptSerializer().Deserialize<StandardFullConModel>(dataS);
                var result = GetDataRpt7SearchModel(searchrpt7Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }

        public List<StandardFullConList> GetDataRpt7SearchModel(StandardFullConModel searchrptModel)
        {
            try
            {
                var dateFrom = !string.IsNullOrEmpty(searchrptModel.DateFrom.ToSafeString()) ? searchrptModel.DateFrom.Value.ToDisplayText("ddMMyyyy") : "";
                var dateTo = !string.IsNullOrEmpty(searchrptModel.DateTo.ToSafeString()) ? searchrptModel.DateTo.Value.ToDisplayText("ddMMyyyy") : "";

                var query = new StandardFullConQuery()
                {
                    region = searchrptModel.Region,
                    dateFrom = dateFrom,
                    dateTo = dateTo
                };

                return GetRpt7SearchReqCurStageQueryData(query);
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<StandardFullConList>();
            }
        }

        public List<StandardFullConList> GetRpt7SearchReqCurStageQueryData(StandardFullConQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportReadSearchStandFullConReportReport(string dataS, string criteria)
        {
            List<StandardFullConList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<StandardFullConModel>(dataS);
            var criteriaModel = new JavaScriptSerializer().Deserialize<CriteriaModel>(criteria);

            rptCriteria = criteriaModel.CRITERIA;
            rptName = criteriaModel.REPORT;
            rptDate = criteriaModel.REPORT_DATE;

            listall = GetDataRpt7SearchModel(searchModel);


            string filename = GetExcelName("StandardFullConnectionReport");

            var bytes = GeneratePAYGEntitytoExcel<StandardFullConList>(listall, filename, "REPORTFULLCON");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public ActionResult ReadSearchStandFullConReport(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<StandardFullConModel>(dataS);
            var result = GetDataRpt7SearchModel(searchModel);

            string item = "0";

            if (result != null && result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*******************************************************************************************************************
         * End Report StandardFullConnection (PAYG_Report7)                                                                                   *
         *******************************************************************************************************************/
        #endregion

    }
}
