using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBWeb.Controllers
{
    public class FbbsaleportalUploadFileExcelController : WBBController
    {
        #region Propreties

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InsertImportExcelLeaveMessageCommand> _insertImportExcelLeaveMessageCommand;
        private readonly ICommandHandler<DeleteFileNameLeaveMessageCommand> _deleteFileNameLeaveMessageCommand;

        #endregion
        // GET: /Leavemessage Upload File Excel/

        #region Constructor

        public FbbsaleportalUploadFileExcelController(IQueryProcessor queryProcessor
            , ICommandHandler<InsertImportExcelLeaveMessageCommand> insertImportExcelLeaveMessageCommand
            , ICommandHandler<DeleteFileNameLeaveMessageCommand> deleteFileNameLeaveMessageCommand
            , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _insertImportExcelLeaveMessageCommand = insertImportExcelLeaveMessageCommand;
            _deleteFileNameLeaveMessageCommand = deleteFileNameLeaveMessageCommand;
            base.Logger = logger;
        }

        #endregion

        #region ActionResult

        public ActionResult UploadFileExcelIndex()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            ViewBag.User = CurrentUser;
            SetViewBagLov("FBBOR021");

            return View();
        }

        public ActionResult CheckValidateFileName(string file_name)
        {
            string user_name = base.CurrentUser.UserName.ToSafeString();

            try
            {
                var query = new GetCheckFileNameLeaveMessageQuery
                {
                    p_file_name = file_name,
                    p_user_name = user_name
                };

                var result = _queryProcessor.Execute(query);

                return Json(new { code = result.return_code, message = result.return_message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, message = SetMsgLov("MESSAGE_ERROR_IMPORT_FILE") }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string msgCountColumn = "";
            string msgResultUpload = "";
            int codeResultUpload = 0;
            string user_name = base.CurrentUser.UserName.ToSafeString();
            var lovDataCol = base.LovData.Where(p => p.LovValue5 == "FBBOR021" && p.Name.StartsWith("C_COLUMN_")).OrderBy(p => p.OrderBy).ToList();

            try
            {
                if (file.ContentLength > 0)
                {
                    //string _FileName = Path.GetFileName(file.FileName);
                    //string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    //file.SaveAs(_path);
                    using (var excel = new ExcelPackage(file.InputStream))
                    {
                        var tbl = new DataTable();
                        var ws = excel.Workbook.Worksheets.First();
                        var hasHeader = true;  // adjust accordingly
                        // add DataColumns to DataTable
                        foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                        {
                            if (hasHeader)
                            {
                                tbl.Columns.Add(firstRowCell.Text);
                            }
                            else
                            {
                                tbl.Columns.Add(String.Format("Column {0}", firstRowCell.Start.Column));
                            }
                        }

                        // add DataRows to DataTable
                        int startRow = hasHeader ? 2 : 1;
                        for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                            var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                            DataRow row = tbl.NewRow();
                            foreach (var cell in wsRow)
                            {
                                row[cell.Start.Column - 1] = cell.Text;
                            }
                            tbl.Rows.Add(row);
                        }

                        //Check Count Column
                        msgCountColumn = CheckCountColumn(tbl, lovDataCol, file.FileName, user_name);
                        if (msgCountColumn != "Complete")
                        {
                            msgResultUpload = msgCountColumn;
                            codeResultUpload = -1;
                        }
                        else
                        {
                            //Insert to Database
                            msgResultUpload = InsertImportExcelLeaveMessage(file.FileName, user_name, tbl, lovDataCol);
                            codeResultUpload = 0;
                        }
                    }

                    return Json(new { code = codeResultUpload, message = msgResultUpload }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = -1, message = SetMsgLov("MESSAGE_ERROR_IMPORT_FILE") }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { code = -1, message = SetMsgLov("MESSAGE_ERROR_IMPORT_FILE") }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Private Method

        // Set Lov ViewBag
        private void SetViewBagLov(string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        private string SetMsgLov(string LovName)
        {
            string LovValue5 = "FBBOR021";

            List<LovValueModel> lovData = base.LovData.Where(p => p.LovValue5 == LovValue5 && p.Name == LovName).ToList();

            return lovData[0].LovValue1.ToSafeString();
        }

        private string CheckCountColumn(DataTable tbl, List<LovValueModel> lovDataCol, string file_name, string username)
        {
            int numRecord = Convert.ToInt32(SetMsgLov("EXCEL_RECORD"));

            if (tbl.Rows.Count > 0 && tbl.Rows.Count <= numRecord && tbl.Columns.Count == lovDataCol.Count + 1)
            {
                return "Complete";
            }
            else
            {
                var Command = new DeleteFileNameLeaveMessageCommand
                {
                    p_file_name = file_name,
                    p_username = username
                };

                _deleteFileNameLeaveMessageCommand.Handle(Command);

                if (Command.return_code == 0)
                {
                    if (tbl.Rows.Count <= numRecord)
                        return SetMsgLov("MESSAGE_DELETED_FILE");
                    else
                        return SetMsgLov("EM_DETAIL_9");
                }
                else
                {
                    return SetMsgLov("MESSAGE_ERROR_IMPORT_FILE");
                }
            }
        }

        private string InsertImportExcelLeaveMessage(string file_name, string username, DataTable tbl, List<LovValueModel> lovDataCol)
        {
            int iComplete = 0;
            int iFail = 0;
            string msgResult = "";

            if (tbl.Rows.Count > 0)
            {

                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    var Command = new InsertImportExcelLeaveMessageCommand
                    {
                        p_file_name = file_name,
                        p_username = username,
                        p_service_speed = tbl.Rows[i][lovDataCol[0].LovValue1.ToSafeString()].ToString(),
                        p_cust_name = tbl.Rows[i][lovDataCol[1].LovValue1.ToSafeString()].ToString(),
                        p_cust_surname = tbl.Rows[i][lovDataCol[2].LovValue1.ToSafeString()].ToString(),
                        p_contact_mobile_no = tbl.Rows[i][lovDataCol[3].LovValue1.ToSafeString()].ToString(),
                        p_is_ais_mobile = tbl.Rows[i][lovDataCol[4].LovValue1.ToSafeString()].ToString(),
                        p_contact_time = tbl.Rows[i][lovDataCol[5].LovValue1.ToSafeString()].ToString(),
                        p_contact_email = tbl.Rows[i][lovDataCol[6].LovValue1.ToSafeString()].ToString(),
                        p_address_type = tbl.Rows[i][lovDataCol[7].LovValue1.ToSafeString()].ToString(),
                        p_building_name = tbl.Rows[i][lovDataCol[8].LovValue1.ToSafeString()].ToString(),
                        p_village_name = tbl.Rows[i][lovDataCol[9].LovValue1.ToSafeString()].ToString(),
                        p_house_no = tbl.Rows[i][lovDataCol[10].LovValue1.ToSafeString()].ToString(),
                        p_soi = tbl.Rows[i][lovDataCol[11].LovValue1.ToSafeString()].ToString(),
                        p_road = tbl.Rows[i][lovDataCol[12].LovValue1.ToSafeString()].ToString(),
                        p_sub_district = tbl.Rows[i][lovDataCol[13].LovValue1.ToSafeString()].ToString(),
                        p_district = tbl.Rows[i][lovDataCol[14].LovValue1.ToSafeString()].ToString(),
                        p_province = tbl.Rows[i][lovDataCol[15].LovValue1.ToSafeString()].ToString(),
                        p_postal_code = tbl.Rows[i][lovDataCol[16].LovValue1.ToSafeString()].ToString(),
                        p_campaign_project_name = tbl.Rows[i][lovDataCol[17].LovValue1.ToSafeString()].ToString()

                    };

                    _insertImportExcelLeaveMessageCommand.Handle(Command);

                    if (Command.return_code == 0)
                    {
                        iComplete += 1;
                    }
                    else
                    {
                        iFail += 1;
                    }
                }

                if (iComplete > 0)
                    msgResult = msgResult + SetMsgLov("EM_DETAIL_1") + " " + iComplete + " " + SetMsgLov("EM_DETAIL_3");
                if (iComplete > 0 && iFail > 0)
                    msgResult = msgResult + "<br />";
                if (iFail > 0)
                    msgResult = msgResult + SetMsgLov("EM_DETAIL_2") + " " + iFail + " " + SetMsgLov("EM_DETAIL_3");

                if (iComplete == 0 && iFail == 0)
                    msgResult = SetMsgLov("MESSAGE_ERROR_IMPORT_FILE");

                return msgResult;
            }
            else
            {
                return SetMsgLov("MESSAGE_ERROR_IMPORT_FILE");
            }
        }

        #endregion
    }
}