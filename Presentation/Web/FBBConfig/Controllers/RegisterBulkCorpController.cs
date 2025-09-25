using Excel;
using FBBConfig.Extensions;
using FBBConfig.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;


namespace FBBConfig.Controllers
{
    public enum ResizeOptions
    {
        // Use fixed width & height without keeping the proportions
        ExactWidthAndHeight,

        // Use maximum width (as defined) and keeping the proportions
        MaxWidth,

        // Use maximum height (as defined) and keeping the proportions
        MaxHeight,

        // Use maximum width or height (the biggest) and keeping the proportions
        MaxWidthAndHeight
    }

    public partial class RegisterBulkCorpController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public RegisterBulkCorpController(ILogger logger,
                IQueryProcessor queryProcessor,
                ICommandHandler<InterfaceLogCommand> intfLogCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;

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
                CREATED_BY = "FBBCONFIGWEB",
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

            //intfLogCommand.Handle(dbIntfCmd);
        }


        #region Screen Config Register Bulk Corp

        private void SetViewBagLov(string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
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

        #endregion

        #region Login Check Seibel
        public JsonResult CheckSeibel(string LocationCode = "", string ASCCode = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            string errorMsg = "";
            if (LocationCode != "")
            {
                if (ASCCode != "")
                {
                    var query = new GetSeibelInfoQuery()
                    {
                        ASCCode = ASCCode,
                        Inparam1 = "IVR",
                        Transaction_Id = ASCCode,
                        FullURL = FullUrl
                    };

                    var result = _queryProcessor.Execute(query);
                    errorMsg = result.outErrorMessage;
                    if (result.outLocationCode.ToSafeString() != "")
                    {
                        if (result.outLocationCode.ToSafeString() == LocationCode)
                        {
                            var query2 = new GetSeibelInfoQuery()
                            {
                                LocationCode = result.outLocationCode.ToSafeString(),
                                Inparam1 = "",
                                Transaction_Id = LocationCode,
                                FullURL = FullUrl
                            };
                            var result2 = _queryProcessor.Execute(query2);
                            return Json(result2, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            errorMsg = "LocationCode from ASCCode not match your LocationCode";
                        }
                    }
                }
                else
                {
                    var query = new GetSeibelInfoQuery()
                    {
                        LocationCode = LocationCode,
                        Transaction_Id = LocationCode,
                        FullURL = FullUrl
                    };

                    var result = _queryProcessor.Execute(query);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (LocationCode == "" && ASCCode != "")
            {
                var query = new GetSeibelInfoQuery()
                {
                    ASCCode = ASCCode,
                    Inparam1 = "IVR",
                    Transaction_Id = ASCCode,
                    FullURL = FullUrl
                };

                var result = _queryProcessor.Execute(query);
                errorMsg = result.outErrorMessage;
                if (result.outLocationCode.ToSafeString() != "")
                {
                    var query2 = new GetSeibelInfoQuery()
                    {
                        LocationCode = result.outLocationCode.ToSafeString(),
                        Inparam1 = "",
                        Transaction_Id = LocationCode,
                        FullURL = FullUrl
                    };
                    var result2 = _queryProcessor.Execute(query2);
                    return Json(result2, JsonRequestBehavior.AllowGet);
                }
            }

            var errormodel = new SeibelResultModel();
            errormodel.outStatus = "Error";
            errormodel.outErrorMessage = errorMsg;
            return Json(errormodel, JsonRequestBehavior.AllowGet);

        }

        #endregion Login Check Seibel

        public ActionResult Login()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBBULK001");

            return View();
        }

        #region Index Main Page

        public ActionResult Index(string SaveStatus, string BulkNoSuccess)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            ViewBag.LinkStatus = "Y";
            ViewBag.SaveStatus = SaveStatus;
            ViewBag.BulkNoSuccess = BulkNoSuccess;
            SetViewBagLov("FBBBULK001");
            return View();
        }

        #endregion Index Main Page

        #region Register new Bulk Corp


        public ActionResult NewCustomer()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            ViewBag.LinkStatus = "Y";
            SetViewBagLov("FBBBULK001");

            return View();

        }

        public ActionResult Existing()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            ViewBag.LinkStatus = "Y";
            SetViewBagLov("FBBBULK001");

            return View();

        }

        public JsonResult GetBulkNumberForExisting()
        {
            var LoginUser = base.CurrentUser;
            var queryBulk = new GetBulkNumberforExistingQuery()
            {
                p_user = LoginUser.UserName
            };
            var data = _queryProcessor.Execute(queryBulk);

            if (null != data)
            {
                Session["Bulk_number"] = data.output_bulk_no;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["Bulk_number"] = "";
                var modelResponse = new { status = false, message = "ไม่สามารถสร้าง Bulk Number ได้กรุณาตรวจสอบ UserName", filename = "" };
                return Json(modelResponse, "text/plain");
            }
        }

        public retInsertBulkCorpExisting GetInsertExisting(string AscCd, string EmpId, string LocatCd, string BulkNo)
        {
            var LoginUser = base.CurrentUser;
            var queryIns = new InsertBulkCorpExistingQuery()
            {
                p_user = LoginUser.UserName,
                p_asc_code = AscCd,
                p_employee_id = EmpId,
                p_location_code = LocatCd,
                p_bulk_number = BulkNo
            };
            var dataIns = _queryProcessor.Execute(queryIns);

            if (null != dataIns)
            {
                return dataIns;
            }
            else
            {
                return null;
            }
        }

        public JsonResult CheckInputMobileList(string lMobile = "")
        {
            bool data = true;

            try
            {
                var query = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "INPUT_MOBILE"
                };

                var result = _queryProcessor.Execute(query);
                var chkResult = result.Where(t => t.LovValue1 == lMobile);
                if (chkResult.Count() > 0)
                {
                    data = true;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    data = false;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

        }

        [HttpPost]
        public ActionResult GetDatabySearchSFF(string CANo = "", string CAName = "", string SANo = "", string SAName = "", string BANo = "", string BAName = "",
            string AscCd = "", string EmpId = "", string LocatCd = "", string BulkNo = "")
        {
            var LoginUser = base.CurrentUser;
            retInsertBulkCorpExisting dataIns = new retInsertBulkCorpExisting();
            evAMQueryCustomerInfoModel querycacustinfo;
            evAMQueryCustomerInfoModel querysacustinfo;
            evAMQueryCustomerInfoModel querybacustinfo;

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #endregion

            if (string.IsNullOrEmpty(CANo) && string.IsNullOrEmpty(CAName) && string.IsNullOrEmpty(SANo) && string.IsNullOrEmpty(SAName)
                && string.IsNullOrEmpty(BANo) && string.IsNullOrEmpty(BAName)) //All is null
            {
                dataIns.output_bulk_no = "";
                dataIns.output_return_code = "-1";
                dataIns.output_return_message = "กรุณาระบุข้อมูลที่ต้องการค้นหา";

                return Json(dataIns, JsonRequestBehavior.AllowGet);
            }

            else if ((!string.IsNullOrEmpty(CANo) || !string.IsNullOrEmpty(CAName)) && (!string.IsNullOrEmpty(SANo) || !string.IsNullOrEmpty(SAName)) &&
                (!string.IsNullOrEmpty(BANo) || !string.IsNullOrEmpty(BAName)))
            {
                //CA First SA Second BA Third
                querycacustinfo = SearchDatafromSFF(CANo, CAName, ipAddress);
                if (null != querycacustinfo)
                {

                    if (querycacustinfo.statusCd == "Active" && !string.IsNullOrEmpty(querycacustinfo.idCardNum) && !string.IsNullOrEmpty(querycacustinfo.idCardType) && querycacustinfo.accntClass == "Customer")
                    {
                        var dataregisCA = SendDatafromSFFToProc(querycacustinfo, BulkNo, LoginUser.UserName);

                        querysacustinfo = SearchDatafromSFF(SANo, SAName, ipAddress);
                        if (null != querysacustinfo)
                        {
                            if (querysacustinfo.statusCd == "Active" && querysacustinfo.accntClass == "Service")
                            {
                                var dataregisSA = SendDatafromSFFToProc(querysacustinfo, BulkNo, LoginUser.UserName);

                                querybacustinfo = SearchDatafromSFF(BANo, BAName, ipAddress);
                                if (null != querybacustinfo)
                                {
                                    if (querybacustinfo.statusCd == "Active" && querybacustinfo.accntClass == "Billing")
                                    {
                                        var dataregisBA = SendDatafromSFFToProc(querybacustinfo, BulkNo, LoginUser.UserName);
                                        if (null != dataregisBA)
                                        {
                                            dataIns = GetInsertExisting(AscCd, EmpId, LocatCd, BulkNo);
                                            dataIns.o_id_card_no = querycacustinfo.idCardNum.ToSafeString();
                                            dataIns.o_id_card_type = querycacustinfo.idCardType.ToSafeString();
                                            dataIns.o_mobile_no = "0000000000";
                                            return Json(dataIns, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            dataIns.output_bulk_no = "";
                                            dataIns.output_return_code = "-1";
                                            dataIns.output_return_message = "เกิดข้อผิดพลาดในการเตรียมข้อมูลกรุณาดำเนินการใหม่อีกครั้ง";

                                            return Json(dataIns, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(BANo) && (!string.IsNullOrEmpty(BAName)))
                                        {
                                            dataIns.output_bulk_no = "";
                                            dataIns.output_return_code = "-1";
                                            dataIns.output_return_message = "BA No: " + BANo + " , BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                                        }
                                        else if (!string.IsNullOrEmpty(BANo))
                                        {
                                            dataIns.output_bulk_no = "";
                                            dataIns.output_return_code = "-1";
                                            dataIns.output_return_message = "BA No: " + BANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                            return Json(dataIns, JsonRequestBehavior.AllowGet);
                                        }
                                        else //if (null != BAName)
                                        {
                                            dataIns.output_bulk_no = "";
                                            dataIns.output_return_code = "-1";
                                            dataIns.output_return_message = "BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                                        }
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(BANo) && (!string.IsNullOrEmpty(BAName)))
                                    {
                                        dataIns.output_bulk_no = "";
                                        dataIns.output_return_code = "-1";
                                        dataIns.output_return_message = "BA No: " + BANo + " , BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                        return Json(dataIns, JsonRequestBehavior.AllowGet);
                                    }
                                    else if (!string.IsNullOrEmpty(BANo))
                                    {
                                        dataIns.output_bulk_no = "";
                                        dataIns.output_return_code = "-1";
                                        dataIns.output_return_message = "BA No: " + BANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                        return Json(dataIns, JsonRequestBehavior.AllowGet);
                                    }
                                    else //if (null != BAName)
                                    {
                                        dataIns.output_bulk_no = "";
                                        dataIns.output_return_code = "-1";
                                        dataIns.output_return_message = "BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                        return Json(dataIns, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(SANo) && !string.IsNullOrEmpty(SAName))
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "SA No: " + SANo + " , SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                                else if (!string.IsNullOrEmpty(SANo))
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "SA No: " + SANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                                else //if (null != SAName)
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                            }

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(SANo) && !string.IsNullOrEmpty(SAName))
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "SA No: " + SANo + " , SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                            else if (!string.IsNullOrEmpty(SANo))
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "SA No: " + SANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                            else //if (null != SAName)
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    else //return data form SFF is null or throw exception
                    {
                        if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(CANo))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else //if (null != CAName)
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                    }


                }
                else //return data form SFF is null or throw exception
                {
                    if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else if (!string.IsNullOrEmpty(CANo))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else //if (null != CAName)
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                }
            }

            else if ((!string.IsNullOrEmpty(CANo) || !string.IsNullOrEmpty(CAName)) && (!string.IsNullOrEmpty(SANo) || !string.IsNullOrEmpty(SAName))
                && (string.IsNullOrEmpty(BANo) && string.IsNullOrEmpty(BAName)))
            {
                //CA First SA Second
                querycacustinfo = SearchDatafromSFF(CANo, CAName, ipAddress);
                if (null != querycacustinfo)
                {

                    if (querycacustinfo.statusCd == "Active" && !string.IsNullOrEmpty(querycacustinfo.idCardNum) && !string.IsNullOrEmpty(querycacustinfo.idCardType)
                        && querycacustinfo.accntClass == "Customer")
                    {
                        var dataregisCA = SendDatafromSFFToProc(querycacustinfo, BulkNo, LoginUser.UserName);

                        querysacustinfo = SearchDatafromSFF(SANo, SAName, ipAddress);
                        if (null != querysacustinfo)
                        {
                            if (querysacustinfo.statusCd == "Active" && querysacustinfo.accntClass == "Service")
                            {
                                var dataregisSA = SendDatafromSFFToProc(querysacustinfo, BulkNo, LoginUser.UserName);

                                if (null != dataregisSA)
                                {
                                    dataIns = GetInsertExisting(AscCd, EmpId, LocatCd, BulkNo);
                                    dataIns.o_id_card_no = querycacustinfo.idCardNum.ToSafeString();
                                    dataIns.o_id_card_type = querycacustinfo.idCardType.ToSafeString();
                                    dataIns.o_mobile_no = "0000000000";
                                    return Json(dataIns, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "เกิดข้อผิดพลาดในการเตรียมข้อมูลกรุณาดำเนินการใหม่อีกครั้ง";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(SANo) && !string.IsNullOrEmpty(SAName))
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "SA No: " + SANo + " , SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                                else if (!string.IsNullOrEmpty(SANo))
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "SA No: " + SANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                                else //if (null != SAName)
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                            }

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(SANo) && !string.IsNullOrEmpty(SAName))
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "SA No: " + SANo + " , SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                            else if (!string.IsNullOrEmpty(SANo))
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "SA No: " + SANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                            else //if (null != SAName)
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "SA Name: " + SAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(CANo))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else //if (null != CAName)
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                    }

                }
                else //return data form SFF is null or throw exception
                {
                    if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else if (!string.IsNullOrEmpty(CANo))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else //if (null != CAName)
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                }
            }

            else if ((!string.IsNullOrEmpty(CANo) || !string.IsNullOrEmpty(CAName)) && (string.IsNullOrEmpty(SANo) && string.IsNullOrEmpty(SAName)) && (!string.IsNullOrEmpty(BANo) || !string.IsNullOrEmpty(BAName)))
            {
                //CA First BA Second
                querycacustinfo = SearchDatafromSFF(CANo, CAName, ipAddress);
                if (null != querycacustinfo)
                {

                    if (querycacustinfo.statusCd == "Active" && !string.IsNullOrEmpty(querycacustinfo.idCardNum) && !string.IsNullOrEmpty(querycacustinfo.idCardType))
                    {
                        var dataregisCA = SendDatafromSFFToProc(querycacustinfo, BulkNo, LoginUser.UserName);

                        querybacustinfo = SearchDatafromSFF(BANo, BAName, ipAddress);
                        if (null != querybacustinfo)
                        {
                            if (querybacustinfo.statusCd == "Active" && querybacustinfo.accntClass == "Billing")
                            {
                                var dataregisBA = SendDatafromSFFToProc(querybacustinfo, BulkNo, LoginUser.UserName);
                                if (null != dataregisBA)
                                {
                                    dataIns = GetInsertExisting(AscCd, EmpId, LocatCd, BulkNo);
                                    dataIns.o_id_card_no = querycacustinfo.idCardNum.ToSafeString();
                                    dataIns.o_id_card_type = querycacustinfo.idCardType.ToSafeString();
                                    dataIns.o_mobile_no = "0000000000";
                                    return Json(dataIns, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "เกิดข้อผิดพลาดในการเตรียมข้อมูลกรุณาดำเนินการใหม่อีกครั้ง";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(BANo) && !string.IsNullOrEmpty(BAName))
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "BA No: " + BANo + " , BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                                else if (!string.IsNullOrEmpty(BANo))
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "BA No: " + BANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }
                                else //if (null != CAName)
                                {
                                    dataIns.output_bulk_no = "";
                                    dataIns.output_return_code = "-1";
                                    dataIns.output_return_message = "BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                    return Json(dataIns, JsonRequestBehavior.AllowGet);

                                }

                            }

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(BANo) && !string.IsNullOrEmpty(BAName))
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "BA No: " + BANo + " , BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                            else if (!string.IsNullOrEmpty(BANo))
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "BA No: " + BANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                            else //if (null != CAName)
                            {
                                dataIns.output_bulk_no = "";
                                dataIns.output_return_code = "-1";
                                dataIns.output_return_message = "BA Name: " + BAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                                return Json(dataIns, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(CANo))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else //if (null != CAName)
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                    }

                }
                else //return data form SFF is null or throw exception
                {
                    if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else if (!string.IsNullOrEmpty(CANo))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else //if (null != CAName)
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                }

            }

            else if ((!string.IsNullOrEmpty(CANo) || !string.IsNullOrEmpty(CAName)) && (string.IsNullOrEmpty(SANo) && string.IsNullOrEmpty(SAName))
                && (string.IsNullOrEmpty(BANo) && string.IsNullOrEmpty(BAName))) // Have CA
            {
                //CA Only
                querycacustinfo = SearchDatafromSFF(CANo, CAName, ipAddress);
                if (null != querycacustinfo)
                {

                    if (querycacustinfo.statusCd == "Active" && !string.IsNullOrEmpty(querycacustinfo.idCardNum) && !string.IsNullOrEmpty(querycacustinfo.idCardType))
                    {
                        var dataregis = SendDatafromSFFToProc(querycacustinfo, BulkNo, LoginUser.UserName);
                        if (null != dataregis)
                        {
                            dataIns = GetInsertExisting(AscCd, EmpId, LocatCd, BulkNo);
                            dataIns.o_id_card_no = querycacustinfo.idCardNum.ToSafeString();
                            dataIns.o_id_card_type = querycacustinfo.idCardType.ToSafeString();
                            dataIns.o_mobile_no = "0000000000";
                            return Json(dataIns, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "เกิดข้อผิดพลาดในการเตรียมข้อมูลกรุณาดำเนินการใหม่อีกครั้ง";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else if (!string.IsNullOrEmpty(CANo))
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                        else //if (null != CAName)
                        {
                            dataIns.output_bulk_no = "";
                            dataIns.output_return_code = "-1";
                            dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                            return Json(dataIns, JsonRequestBehavior.AllowGet);

                        }
                    }

                }
                else //return data from SFF is null or throw exception
                {
                    if (!string.IsNullOrEmpty(CANo) && !string.IsNullOrEmpty(CAName))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " , CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else if (!string.IsNullOrEmpty(CANo))
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA No: " + CANo + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                    else //if (null != CAName)
                    {
                        dataIns.output_bulk_no = "";
                        dataIns.output_return_code = "-1";
                        dataIns.output_return_message = "CA Name: " + CAName + " สถานะไม่พร้อมใช้งานกรุณาทำให้ Active ก่อน Register";

                        return Json(dataIns, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            else
            {
                dataIns.output_bulk_no = "";
                dataIns.output_return_code = "-1";
                dataIns.output_return_message = "เกิดข้อผิดพลาดในการเตรียมข้อมูลกรุณาดำเนินการใหม่อีกครั้ง";

                return Json(dataIns, JsonRequestBehavior.AllowGet);
            }
        }

        public evAMQueryCustomerInfoModel SearchDatafromSFF(string SearchNo, string SearchName, string ipAdd)
        {
            try
            {
                var queryca = new evAMQueryCustomerInfoQuery()
                {
                    idCardNum = "",
                    name = SearchName,
                    accntNo = SearchNo,
                    contactBirthDt = "",
                    minRowNum = "",
                    maxRowNum = "",
                    ClientIP = ipAdd
                };
                var querycustinfo = _queryProcessor.Execute(queryca);
                _Logger.Info("Search Bulk Corp Existing Account Data: " + "\r\n" +
                     "rowNum : " + querycustinfo.rowNum + "\r\n" +
                     "rowId : " + querycustinfo.rowId + "\r\n" +
                     "accntNo : " + querycustinfo.accntNo + "\r\n" +
                     "accntClass : " + querycustinfo.accntClass + "\r\n" +
                     "accntTitle : " + querycustinfo.accntTitle + "\r\n" +
                     "name : " + querycustinfo.name + "\r\n" +
                     "idCardNum : " + querycustinfo.idCardNum + "\r\n" +
                     "idCardType : " + querycustinfo.idCardType + "\r\n" +
                     "contactBirthDt : " + querycustinfo.contactBirthDt + "\r\n" +
                     "statusCd : " + querycustinfo.statusCd + "\r\n" +
                     "accntCategory : " + querycustinfo.accntCategory + "\r\n" +
                     "accntSubCategory : " + querycustinfo.accntSubCategory + "\r\n" +
                     "mainPhone : " + querycustinfo.mainPhone + "\r\n" +
                     "mainMobile : " + querycustinfo.mainMobile + "\r\n" +
                     "legalFlg : " + querycustinfo.legalFlg + "\r\n" +
                     "houseNo : " + querycustinfo.houseNo + "\r\n" +
                     "buildingName : " + querycustinfo.buildingName + "\r\n" +
                     "floor : " + querycustinfo.floor + "\r\n" +
                     "room : " + querycustinfo.room + "\r\n" +
                     "moo : " + querycustinfo.moo + "\r\n" +
                     "mooban : " + querycustinfo.mooban + "\r\n" +
                     "streetName : " + querycustinfo.streetName + "\r\n" +
                     "soi : " + querycustinfo.soi + "\r\n" +
                     "zipCode : " + querycustinfo.zipCode + "\r\n" +
                     "tumbol : " + querycustinfo.tumbol + "\r\n" +
                     "amphur : " + querycustinfo.amphur + "\r\n" +
                     "provinceName : " + querycustinfo.provinceName + "\r\n" +
                     "country : " + querycustinfo.country + "\r\n" +
                     "vatName : " + querycustinfo.vatName + "\r\n" +
                     "vatRate : " + querycustinfo.vatRate + "\r\n" +
                     "vatAddress1 : " + querycustinfo.vatAddress1 + "\r\n" +
                     "vatAddress2 : " + querycustinfo.vatAddress2 + "\r\n" +
                     "vatAddress3 : " + querycustinfo.vatAddress3 + "\r\n" +
                     "vatAddress4 : " + querycustinfo.vatAddress4 + "\r\n" +
                     "vatAddress5 : " + querycustinfo.vatAddress5 + "\r\n" +
                     "vatPostalCd : " + querycustinfo.vatPostalCd + "\r\n" +
                     "errorMessage : " + querycustinfo.errorMessage + "\r\n" +
                     "total : " + querycustinfo.total + "\r\n");

                return querycustinfo;

            }
            catch (Exception ex)
            {
                _Logger.Info("Error When Call evAMQueryCustomerInfoQuery " + ex.GetErrorMessage());
                _Logger.Info("Error When Call evAMQueryCustomerInfoQuery SearchNo: " + SearchNo + "SearchName: " + SearchName);

                return null;
            }

        }


        public returnExistRegister SendDatafromSFFToProc(evAMQueryCustomerInfoModel querycacustinfo, string BulkNo, string UserName)
        {

            var queryexist = new ExistingRegisterBulkCorpQuery()
            {
                p_user = UserName,
                p_bulk_number = BulkNo,
                p_errormessage = querycacustinfo.errorMessage,
                p_total = querycacustinfo.total,
                p_rownum = querycacustinfo.rowNum,
                p_rowid = querycacustinfo.rowId,
                p_accntno = querycacustinfo.accntNo,
                p_accntclass = querycacustinfo.accntClass,
                p_name = querycacustinfo.name,
                p_idcardnum = querycacustinfo.idCardNum,
                p_idcardtype = querycacustinfo.idCardType,
                p_contactbirthdt = querycacustinfo.contactBirthDt,
                p_statuscd = querycacustinfo.statusCd,
                p_accntcategory = querycacustinfo.accntCategory,
                p_accntsubcategory = querycacustinfo.accntSubCategory,
                p_mainphone = querycacustinfo.mainPhone,
                p_mainmobile = querycacustinfo.mainMobile,
                p_legalflg = querycacustinfo.legalFlg,
                p_houseno = querycacustinfo.houseNo,
                p_buildingname = querycacustinfo.buildingName,
                p_floor = querycacustinfo.floor,
                p_room = querycacustinfo.room,
                p_moo = querycacustinfo.moo,
                p_mooban = querycacustinfo.mooban,
                p_streetname = querycacustinfo.streetName,
                p_soi = querycacustinfo.soi,
                p_zipcode = querycacustinfo.zipCode,
                p_tumbol = querycacustinfo.tumbol,
                p_amphur = querycacustinfo.amphur,
                p_provincename = querycacustinfo.provinceName,
                p_country = querycacustinfo.country,
                p_vatname = querycacustinfo.vatName,
                p_vatrate = querycacustinfo.vatRate,
                p_vataddress1 = querycacustinfo.vatAddress1,
                p_vataddress2 = querycacustinfo.vatAddress2,
                p_vataddress3 = querycacustinfo.vatAddress3,
                p_vataddress4 = querycacustinfo.vatAddress4,
                p_vataddress5 = querycacustinfo.vatAddress5,
                p_vatpostalcd = querycacustinfo.vatPostalCd,
                p_accounttitle = querycacustinfo.accntTitle

            };

            var dataex = _queryProcessor.Execute(queryexist);
            return dataex;
        }

        public JsonResult CheckBlackList(string idcardNo = "")
        {

            var query = new evOMServiceIVRCheckBlackListQuery
            {
                inCardNo = idcardNo
            };
            var a = _queryProcessor.Execute(query);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectAccountCat(string type = "")
        {
            var query = new SelectLovDisplayQuery
            {
                LOV_TYPE = type,
                DISPLAY_VAL = "",
                LOV_VAL5 = "FBBBULK001"
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectAccountSubCat(string type = "", string AccountCat = "")
        {
            var query = new SelectLovDisplayQuery
            {
                LOV_TYPE = type,
                DISPLAY_VAL = AccountCat,
                LOV_VAL5 = "FBBBULK001"
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectAccountTitle(string type = "", string AccountCat = "")
        {
            var query = new SelectLovDisplayQuery
            {
                LOV_TYPE = type,
                DISPLAY_VAL = AccountCat,
                LOV_VAL5 = "FBBBULK001"
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectIDCardType(string type = "", string AccountSubCat = "")
        {
            var query = new SelectLovDisplayQuery
            {
                LOV_TYPE = type,
                DISPLAY_VAL = AccountSubCat,
                LOV_VAL5 = "FBBBULK001"
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectBillCycle(string type = "")
        {
            var query = new SelectLovDisplayQuery
            {
                LOV_TYPE = type,
                DISPLAY_VAL = "",
                LOV_VAL5 = "FBBBULK001"
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectInstallType(string type = "")
        {
            var query = new SelectLovDisplayQuery
            {
                LOV_TYPE = type,
                DISPLAY_VAL = "",
                LOV_VAL5 = "FBBBULK001"
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "", LOV_NAME = "", LOV_VAL1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchMainPkg(string Accnt_cat = "", string Tech_no = "", string Promo_Cd = "")
        {
            var retpkg = new List<ReturnPackageList>();
            Session["Select_Main_Package"] = new ReturnPackageList();

            try
            {
                var query = new GetBulkCorpMainPackageQuery
                {
                    AccntCat = Accnt_cat,
                    Techno = Tech_no,
                    PackCode = Promo_Cd
                };
                retpkg = _queryProcessor.Execute(query);

                if (null == retpkg || !retpkg.Any())
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session["Select_Main_Package"] = retpkg;

                    return Json(retpkg, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                _Logger.Info("Error When call SelectMainPackage " + ex.GetErrorMessage());
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SearchOntopDisPkg(string Accnt_cat = "", string Tech_no = "", string Promo_Cd = "", string MainPromo_Cd = "")
        {
            List<ReturnPackageList> retpkg = new List<ReturnPackageList>();
            Session["Select_OntopDis_Package"] = new ReturnPackageList();

            try
            {
                var query = new GetBulkCorpOntopDiscountPackageQuery
                {
                    AccntCat = Accnt_cat,
                    Techno = Tech_no,
                    PackCode = Promo_Cd,
                    MainPackCode = MainPromo_Cd
                };
                retpkg = _queryProcessor.Execute(query);

                if (null == retpkg || !retpkg.Any())
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session["Select_OntopDis_Package"] = retpkg;

                    return Json(retpkg, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                _Logger.Info("Error When call SelectMainPackage " + ex.GetErrorMessage());
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SelectMainPackage(string Accnt_cat = "", string sffPromoCd = "")//, string Pkgname = "")
        {
            var retpkg = new List<ReturnPackageList>();
            var retpkgsave = new List<ReturnPackageList>();
            Session["Select_Main_Package"] = new ReturnPackageList();
            try
            {
                var query = new GetBulkCorpMainPackageQuery
                {
                    AccntCat = Accnt_cat
                };
                retpkg = _queryProcessor.Execute(query);

                if (null == retpkg || !retpkg.Any())
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    /*
                     
                    if ((null == sffPromoCd || "" == sffPromoCd) )//&& (null != Pkgname || "" != Pkgname))
                    {

                        retpkgsave = retpkg.Where(pkg=>pkg.package_name_tha == Pkgname)
                            .ToList();

                    }
                    else if ((sffPromoCd != "" || null != sffPromoCd) && (null == Pkgname || "" == Pkgname))
                    {

                        retpkgsave = retpkg.Where(pkg => pkg.sff_promotion_code == sffPromoCd)
                            .ToList();

                    }
                    else
                    {
                        retpkgsave = retpkg.Where(pkg => pkg.sff_promotion_code == sffPromoCd )//&& pkg.package_name_tha == Pkgname)
                            .ToList();

                    }

                    if (null != retpkgsave)
                    {
                        Session["Select_Main_Package"] = retpkgsave;
                    }

                    */

                    return Json(retpkg, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info("Error When call SelectMainPackage " + ex.GetErrorMessage());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetMainPackageSffPromoCd(string Accnt_cat = "", string Pkgname = "")
        {
            var retpkg = new List<ReturnPackageList>();
            var pkgsffPromoCd = new List<DropdownModel>();
            var tempData = new List<DropdownModel>();

            try
            {

                if ("" == Accnt_cat || null == Accnt_cat)//|| "all" == Accnt_cat
                {
                    tempData.Insert(0, new DropdownModel { Text = "" });
                    tempData.Insert(1, new DropdownModel { Text = "" });
                    return Json(tempData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = new GetBulkCorpMainPackageQuery
                    {
                        AccntCat = Accnt_cat
                    };
                    retpkg = _queryProcessor.Execute(query);
                }

                if (null == retpkg)
                {
                    //DropdownModel tempData = new DropdownModel();
                    //tempData.Text = "";
                    tempData.Insert(0, new DropdownModel { Text = "" });
                    tempData.Insert(1, new DropdownModel { Text = "" });
                    return Json(tempData, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    if ((Accnt_cat != "" || null != Accnt_cat) && (null == Pkgname || "" == Pkgname))
                    {
                        pkgsffPromoCd.Insert(0, new DropdownModel { Text = "" });
                        pkgsffPromoCd.Insert(1, new DropdownModel { Text = "" });
                    }
                    else
                    {
                        pkgsffPromoCd = retpkg
                            .Where(pkg => pkg.package_name_tha == Pkgname)
                            .Select(pkg => new DropdownModel()
                            {
                                Text = pkg.sff_promotion_code
                            })
                            .OrderBy(o => o.Text)
                            .ToList();

                        pkgsffPromoCd.Insert(0, new DropdownModel { Text = "" });
                    }

                    return Json(pkgsffPromoCd, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info("Error When call GetMainPackagebySffPromoCd " + ex.GetErrorMessage());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetMainPackagebyPackageNameTH(string Accnt_cat = "")
        {
            Dictionary<string, List<DropdownModel>> dict = new Dictionary<string, List<DropdownModel>>();
            List<DropdownModel> data = new List<DropdownModel>();
            List<ReturnPackageList> retpkg = new List<ReturnPackageList>();

            try
            {

                var PackageNameth = new List<DropdownModel>();

                if ("" == Accnt_cat || "all" == Accnt_cat || null == Accnt_cat)
                {
                    var query = new GetBulkCorpMainPackageQuery
                    {
                        AccntCat = "all"
                    };
                    retpkg = _queryProcessor.Execute(query);
                }
                else
                {
                    var query = new GetBulkCorpMainPackageQuery
                    {
                        AccntCat = Accnt_cat
                    };
                    retpkg = _queryProcessor.Execute(query);
                }

                if (null == retpkg)
                {
                    data = new List<DropdownModel>();
                    DropdownModel tempData = new DropdownModel();
                    tempData.Value = "";
                    tempData.Text = "";
                    data.Add(tempData);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    if (Accnt_cat != "" || null != Accnt_cat)
                    {

                        PackageNameth = retpkg
                        .Select(pkg => new DropdownModel()
                        {
                            Text = pkg.package_name_tha,
                            Value = pkg.package_name_tha
                        }).ToList();

                        PackageNameth.Insert(0, new DropdownModel { Text = "", Value = "" });

                    }

                    dict.Add("PackageNameth", PackageNameth);

                    return Json(dict.Values.ElementAt(0), JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                _Logger.Info("Error When call GetMainPackagebyPackageNameTH " + ex.GetErrorMessage());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetDescriptMainPackage(string Accnt_cat = "", string sffPromoCd = "", string Pkgname = "")
        {
            var DespcriptMain = new List<DropdownModel>();
            var retpkg = new List<ReturnPackageList>();

            try
            {
                if ("" == Accnt_cat || "all" == Accnt_cat || null == Accnt_cat)
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = new GetBulkCorpMainPackageQuery
                    {
                        AccntCat = Accnt_cat
                    };
                    retpkg = _queryProcessor.Execute(query);
                }

                if (null == retpkg)
                {
                    DropdownModel tempData = new DropdownModel();
                    tempData.Text = "";

                    return Json(tempData, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    if ((null == sffPromoCd || "" == sffPromoCd) && (null == Pkgname || "" == Pkgname))
                    {

                        DespcriptMain.Insert(0, new DropdownModel { Text = "" });

                    }
                    else if ((Accnt_cat != "" || null != Accnt_cat) && (null != sffPromoCd || "" != sffPromoCd) && (null == Pkgname || "" == Pkgname))
                    {
                        DespcriptMain = retpkg.Where(pkg => pkg.sff_promotion_code == sffPromoCd)
                        .Select(pkg => new DropdownModel()
                        {
                            Text = pkg.sff_promotion_bill_tha
                        }).ToList();

                        DespcriptMain.Insert(0, new DropdownModel { Text = "" });

                    }
                    else if ((Accnt_cat != "" || null != Accnt_cat) && (null != sffPromoCd || "" != sffPromoCd) && (null != Pkgname || "" != Pkgname))
                    {
                        DespcriptMain = retpkg.Where(pkg => pkg.sff_promotion_code == sffPromoCd && pkg.package_name_tha == Pkgname)
                        .Select(pkg => new DropdownModel()
                        {
                            Text = pkg.sff_promotion_bill_tha
                        }).ToList();

                        DespcriptMain.Insert(0, new DropdownModel { Text = "" });

                    }
                    else
                    {
                        DespcriptMain.Insert(0, new DropdownModel { Text = "" });
                    }

                    return Json(DespcriptMain, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                _Logger.Info("Error When call GetDescriptMainPackage " + ex.GetErrorMessage());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SelectOntopPackage1(string Accnt_cat = "")
        {
            List<ReturnOntopPackageList> dataOntop1;
            Session["dataOntop1"] = new ReturnOntopPackageList();
            var queryOntop1 = new GetBulkCorpOntopPackage1Query
            {
                AccntCat = Accnt_cat
            };
            dataOntop1 = _queryProcessor.Execute(queryOntop1);

            if (null != dataOntop1)
                Session["dataOntop1"] = dataOntop1;

            return Json(dataOntop1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOntopPackage2(string Accnt_cat = "")
        {
            List<ReturnOntopPackageList> dataOntop2;
            Session["dataOntop2"] = new ReturnOntopPackageList();
            var queryOntop2 = new GetBulkCorpOntopPackage2Query
            {
                AccntCat = Accnt_cat
            };
            dataOntop2 = _queryProcessor.Execute(queryOntop2);

            if (null != dataOntop2)
                Session["dataOntop2"] = dataOntop2;


            return Json(dataOntop2, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectServicePackage1(string Accnt_cat = "")
        {
            List<ReturnServicePackageList> dataService1;
            Session["dataService1"] = new List<ReturnServicePackageList>();

            var queryService1 = new GetBulkCorpServicePackage1Query
            {
                AccntCat = Accnt_cat
            };
            dataService1 = _queryProcessor.Execute(queryService1);

            if (null != dataService1)
                Session["dataService1"] = dataService1;

            return Json(dataService1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectServicePackage2(string Accnt_cat = "")
        {
            List<ReturnServicePackageList> dataService2;
            Session["dataService2"] = new List<ReturnServicePackageList>();

            var queryService2 = new GetBulkCorpServicePackage2Query
            {
                AccntCat = Accnt_cat
            };
            dataService2 = _queryProcessor.Execute(queryService2);

            if (null != dataService2)
                Session["dataService2"] = dataService2;

            return Json(dataService2, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectServicePackage3(string Accnt_cat = "")
        {
            List<ReturnServicePackageList> dataService3;
            Session["dataService3"] = new List<ReturnServicePackageList>();

            var queryService3 = new GetBulkCorpServicePackage3Query
            {
                AccntCat = Accnt_cat
            };
            dataService3 = _queryProcessor.Execute(queryService3);

            if (null != dataService3)
                Session["dataService3"] = dataService3;

            return Json(dataService3, JsonRequestBehavior.AllowGet);
        }

        #endregion Register new Bulk Corp

        #region Excel Register new Bulk Corp
        public ActionResult clearSession()
        {
            var addedit = Session["Bulktempupload"] as List<BulkExcelData>;
            if (addedit != null)
                Session.Remove("Bulktempupload");

            var filename = Session["Bulkfilename"];
            if (filename != null)
                Session.Remove("Bulkfilename");

            var MainPkg = Session["Select_Main_Package"];
            if (MainPkg != null)
                Session.Remove("Select_Main_Package");

            var dataOntop1 = Session["dataOntop1"];
            if (dataOntop1 != null)
                Session.Remove("dataOntop1");

            var dataOntop2 = Session["dataOntop2"];
            if (dataOntop2 != null)
                Session.Remove("dataOntop2");

            var dataService1 = Session["dataService1"];
            if (dataService1 != null)
                Session.Remove("dataService1");

            var dataService2 = Session["dataService2"];
            if (dataService2 != null)
                Session.Remove("dataService2");

            var dataService3 = Session["dataService3"];
            if (dataService3 != null)
                Session.Remove("dataService3");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult total()
        {
            string ttr = string.Empty;
            var tempupload = Session["Bulktempupload"] as List<BulkExcelData>;
            ttr = tempupload.Count.ToString();

            return Json(new { ttr = ttr }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveExcel(IEnumerable<HttpPostedFileBase> files, string cateType, string cardNo,
            string cardType, string register_dv, string MobileNumber, string Bulk_No)
        {

            if (null != Session["totalrow"])
            {
                Session["totalrow"] = "";
            }

            if (null != Session["totalsize"])
            {
                Session["totalsize"] = "";
            }

            var tempupload = Session["Bulktempupload"] as List<BulkExcelData>;
            if (tempupload == null)
                tempupload = new List<BulkExcelData>();

            if (files != null)
            {
                var result = new List<BulkExcelData>();
                try
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path. We only care about the file name.
                        var fileName = Path.GetFileName(file.FileName);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        if (fileName.EndsWith("xls") || fileName.EndsWith("xlsx"))
                        {
                            var checkdup = true;
                            if (checkdup)
                            {

                                var destinationPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                                file.SaveAs(destinationPath);
                                FileInfo fInfo = new FileInfo(destinationPath);
                                long size = fInfo.Length;
                                Session["totalsize"] = size.ToSafeString();

                                if (size > 10000000)
                                {
                                    var modelResponse = new { status = false, message = "File's exceeded", filename = fileName };
                                    return Json(modelResponse, "text/plain");
                                }
                                else
                                {

                                    DataTable table = getDataSet(destinationPath);

                                    table = RemoveAllNullRowsFromDataTable(table);
                                    var inttotalrow = table.Rows.Count;
                                    Session["totalrow"] = inttotalrow.ToSafeString();

                                    var pathimper = UploadExcelBulk(cateType, cardNo, cardType, register_dv, MobileNumber, Bulk_No, table);

                                    foreach (DataRow dRow in table.Rows)
                                    {
                                        //i++;
                                        if (null == dRow["No"].ToString())
                                        {
                                            break;
                                        }

                                        if (dRow["No"].ToString().Equals("")
                                            || dRow["installAddress1"].ToString().Equals("")
                                            || dRow["installAddress2"].ToString().Equals("")
                                            || dRow["installAddress3"].ToString().Equals("")
                                            || dRow["installAddress4"].ToString().Equals("")
                                            || dRow["installAddress5"].ToString().Equals("")
                                            || dRow["latitude"].ToString().Equals("")
                                            || dRow["longitude"].ToString().Equals("")
                                            || dRow["install_date"].ToString().Equals(""))
                                        {
                                            break;
                                        }

                                    }

                                    if (table.Rows.Count == 0)
                                    {
                                        var modelResponse = new { status = false, message = fileName + " does not complete, please check require field in file.", filename = fileName };
                                        return Json(modelResponse, "text/plain");
                                    }

                                    stopwatch.Stop();
                                    var sds = stopwatch.Elapsed;
                                    Session["filename"] = pathimper.ExcelBulk.FileExcelBulk;

                                }
                            }
                            else
                            {
                                var modelResponse = new { status = false, message = fileName + "is already exist.", filename = fileName };
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
                    else
                    {
                        var modelResponse = new { status = false, message = e.Message, filename = "" };
                        return Json(modelResponse, "text/plain");
                    }
                }

            }

            return Content("");
        }

        public JsonResult GetAddressIDRegisterBulkCorp(string Addr_ID, string Event_CD)
        {
            RetGetAddrID dataAddr = new RetGetAddrID();

            try
            {
                _Logger.Info("Start GetAddressIDRegisterBulkCorpQuery");
                var query = new GetAddressIDRegisterBulkCorpQuery
                {
                    P_ADDRESS_ID = Addr_ID,
                    P_EVENT_CODE = Event_CD
                };

                dataAddr = _queryProcessor.Execute(query);

                _Logger.Info("End GetAddressIDRegisterBulkCorpQuery");
                return Json(dataAddr, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _Logger.Info("Error " + ex.GetErrorMessage());
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }

        //public ActionResult SaveExcel(IEnumerable<HttpPostedFileBase> files, string cateType, string cardNo, 
        //    string cardType, string register_dv, string MobileNumber, string Bulk_No)
        //{ 

        //    //ExcelWorksheet wc6 = p.Workbook.Worksheets[2];
        //    if (null != Session["Bulktempupload"])
        //    {
        //        Session["Bulktempupload"] = new List<BulkExcelData>();
        //    }

        //    if (null != Session["totalrow"])
        //    {
        //        Session["totalrow"] = "";
        //    }

        //    if (null != Session["totalsize"])
        //    {
        //        Session["totalsize"] = "";
        //    }

        //    var tempupload = Session["Bulktempupload"] as List<BulkExcelData>;
        //    if (tempupload == null)
        //        tempupload = new List<BulkExcelData>();

        //    if (files != null)
        //    {

        //        var result = new List<BulkExcelData>();
        //        try
        //        {
        //            foreach (var file in files)
        //            {
        //                // Some browsers send file names with full path. We only care about the file name.
        //                var fileName = Path.GetFileName(file.FileName);
        //                Stopwatch stopwatch = new Stopwatch();
        //                stopwatch.Start();
        //                if (fileName.EndsWith("xls") || fileName.EndsWith("xlsx"))
        //                {
        //                    var checkdup = true;
        //                    if (checkdup)
        //                    {

        //                        var destinationPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
        //                        file.SaveAs(destinationPath);
        //                        FileInfo fInfo = new FileInfo(destinationPath);
        //                        long size = fInfo.Length;
        //                        Session["totalsize"] = size;

        //                        if (size > 10000000)
        //                        {
        //                            var modelResponse = new { status = false, message = "File's exceeded", filename = fileName };
        //                            return Json(modelResponse, "text/plain");
        //                        }
        //                        else
        //                        {
        //                            //**********  **********//

        //                            DataTable table = getDataSet(destinationPath);

        //                            table = RemoveAllNullRowsFromDataTable(table);
        //                            var inttotalrow = table.Rows.Count;
        //                            Session["totalrow"] = inttotalrow;

        //                            var pathimper = UploadExcelBulk(cateType, cardNo, cardType, register_dv, MobileNumber, Bulk_No,table);

        //                            bool checkrequire = false;
        //                            bool checkheadercol = false;
        //                            foreach (DataRow dRow in table.Rows)
        //                            {
        //                                //i++;
        //                                if (null == dRow["No"].ToString())
        //                                {
        //                                    checkheadercol = true;
        //                                    break;
        //                                }

        //                                if (dRow["No"].ToString().Equals("")
        //                                    || dRow["installAddress1"].ToString().Equals("")
        //                                    || dRow["installAddress2"].ToString().Equals("")
        //                                    || dRow["latitude"].ToString().Equals("")
        //                                    || dRow["longitude"].ToString().Equals("")
        //                                    || dRow["ia"].ToString().Equals("")
        //                                    || dRow["password"].ToString().Equals(""))
        //                                {
        //                                    checkrequire = true;
        //                                    break;
        //                                }
        //                                var temp = new BulkExcelData();
        //                                temp.No = dRow["No"].ToString();
        //                                temp.installAddress1 = dRow["installAddress1"].ToString();
        //                                temp.installAddress2 = dRow["installAddress2"].ToString();
        //                                temp.installAddress3 = dRow["installAddress3"].ToString();
        //                                temp.installAddress4 = dRow["installAddress4"].ToString();
        //                                temp.installAddress5 = dRow["installAddress5"].ToString();
        //                                temp.latitude = dRow["latitude"].ToString();
        //                                temp.longitude = dRow["longitude"].ToString();
        //                                temp.dpName = dRow["dpName"].ToString();
        //                                temp.installationCapacity = dRow["installationCapacity"].ToString();
        //                                temp.ia = dRow["ia"].ToString();
        //                                temp.password = dRow["password"].ToString();

        //                                result.Add(temp);
        //                            }

        //                            if (checkheadercol == true )
        //                            {
        //                                var modelResponse = new { status = false, message = fileName + " does not complete, please check the first record in file must have value.", filename = fileName };
        //                                return Json(modelResponse, "text/plain");
        //                            }

        //                            if (checkrequire == true || table.Rows.Count == 0)
        //                            {
        //                                var modelResponse = new { status = false, message = fileName + " does not complete, please check require field in file.", filename = fileName };
        //                                return Json(modelResponse, "text/plain");
        //                            }

        //                            stopwatch.Stop();
        //                            var sds = stopwatch.Elapsed;
        //                            Session["Bulktempupload"] = result;
        //                            Session["filename"] = pathimper.ExcelBulk.FileExcelBulk;

        //                            /*
        //                            var modelResponse = new { status = true, message = "", filename = fileName };
        //                            return Json(modelResponse, "text/plain");
        //                             */
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var modelResponse = new { status = false, message = fileName + "is already exist.", filename = fileName };
        //                        return Json(modelResponse, "text/plain");
        //                    }
        //                }
        //                else
        //                {
        //                    var modelResponse = new { status = false, message = "File Format type Error", filename = fileName };
        //                    return Json(modelResponse, "text/plain");
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            if (e.Message.Contains("does not belong to table"))
        //            {
        //                var modelResponse = new { status = false, message = "This file has missing field format", filename = "" };
        //                return Json(modelResponse, "text/plain");
        //            }
        //            else
        //            {
        //                var modelResponse = new { status = false, message = e.Message, filename = "" };
        //                return Json(modelResponse, "text/plain");
        //            }
        //        }

        //    }

        //    return Content("");
        //}

        public ActionResult SaveRegisterPkg(
            string tech_install = "", string address_id = ""/*,string install_date = ""*/, string event_code = "", string first_name = "",
            string last_name = "", string phone = "", string mobile = "", string email = "",
            string MainPkg_sffpromocd = "", string MainPkg_pkg_class = "", string MainPkg_sffpromobillth = "",
            string MainPkg_sffpromobilleng = "", string MainPkg_pkgnameth = "", string MainPkg_pkgnameeng = "",
            string MainPkg_recur_charge = "", string MainPkg_pre_init_charge = "", string MainPkg_init_charge = "",
            string MainPkg_down_speed = "", string MainPkg_up_speed = "", string MainPkg_product_type = "", string MainPkg_owner_product = "",
            string MainPkg_product_subtype = "", string MainPkg_product_subtype2 = "", string MainPkg_tech = "",
            string MainPkg_pkg_grp = "", string MainPkg_pkg_cd = "", string MainPkg_network_type = "", string MainPkg_cust_type = "",
            string OntopPkg1_sffpromocd = "", string OntopPkg1_pkg_class = "", string OntopPkg1_sffpromobillth = "",
            string OntopPkg1_sffpromobilleng = "", string OntopPkg1_pkgnameth = "", string OntopPkg1_pkgnameeng = "",
            string OntopPkg1_recur_charge = "", string OntopPkg1_pre_init_charge = "", string OntopPkg1_init_charge = "",
            string OntopPkg1_down_speed = "", string OntopPkg1_up_speed = "", string OntopPkg1_product_type = "",
            string OntopPkg1_owner_product = "", string OntopPkg1_product_subtype = "", string OntopPkg1_product_subtype2 = "",
            string OntopPkg1_tech = "", string OntopPkg1_pkg_grp = "", string OntopPkg1_pkg_cd = "", string OntopPkg1_cust_type = "",
            string OntopPkg2_sffpromocd = "", string OntopPkg2_pkg_class = "", string OntopPkg2_sffpromobillth = "",
            string OntopPkg2_sffpromobilleng = "", string OntopPkg2_pkgnameth = "", string OntopPkg2_pkgnameeng = "",
            string OntopPkg2_recur_charge = "", string OntopPkg2_pre_init_charge = "", string OntopPkg2_init_charge = "",
            string OntopPkg2_down_speed = "", string OntopPkg2_up_speed = "", string OntopPkg2_product_type = "",
            string OntopPkg2_owner_product = "", string OntopPkg2_product_subtype = "", string OntopPkg2_product_subtype2 = "",
            string OntopPkg2_tech = "", string OntopPkg2_pkg_grp = "", string OntopPkg2_pkg_cd = "", string OntopPkg2_cust_type = "",
            string ServicePkg1_servicecd = "", string ServicePkg1_productname = "",
            string ServicePkg2_servicecd = "", string ServicePkg2_productname = "",
            string ServicePkg3_servicecd = "", string ServicePkg3_productname = "",//new
            string BulkNo = "", string OntopDisPkg_sffpromocd = "", string OntopDisPkg_pkg_class = "", string OntopDisPkg_sffpromobillth = "",
            string OntopDisPkg_sffpromobilleng = "", string OntopDisPkg_pkgnameth = "", string OntopDisPkg_pkgnameeng = "",
            string OntopDisPkg_recur_charge = "", string OntopDisPkg_pre_init_charge = "", string OntopDisPkg_init_charge = "",
            string OntopDisPkg_down_speed = "", string OntopDisPkg_up_speed = "", string OntopDisPkg_product_type = "", string OntopDisPkg_owner_product = "",
            string OntopDisPkg_product_subtype = "", string OntopDisPkg_product_subtype2 = "", string OntopDisPkg_tech = "",
            string OntopDisPkg_pkg_grp = "", string OntopDisPkg_pkg_cd = "", string OntopDisPkg_network_type = "", string OntopDisPkg_cust_type = ""
            )
        {

            var LoginUser = base.CurrentUser;
            if (null == base.CurrentUser)
                return Json("Sesson Time Out", JsonRequestBehavior.AllowGet);

            var returnexcel = new List<ReturnInsertExcelData>();
            ReturnInsertExcelData retexcel = new ReturnInsertExcelData();

            var tempSelectMainPkg = Session["Select_Main_Package"] as List<ReturnPackageList>;
            if (tempSelectMainPkg == null)
                tempSelectMainPkg = new List<ReturnPackageList>();

            var filename = Session["filename"] as String;
            if (string.IsNullOrEmpty(filename))
            {
                retexcel.output_bulk_no = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR ListExcelData is null";
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }

            var filesize = Session["totalsize"] as String;
            if (string.IsNullOrEmpty(filesize))
            {
                retexcel.output_bulk_no = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR total size of ListExcelData is null";
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }

            var filetotalrow = Session["totalrow"] as String;
            if (string.IsNullOrEmpty(filetotalrow))
            {
                retexcel.output_bulk_no = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR total row of ListExcelData is null";
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }

            #region Read Excel from NAS
            List<BulkExcelData> ListExcelData = new List<BulkExcelData>();
            byte[] excelFiles;
            IExcelDataReader iExcelDataReader = null;
            DataTable res = null;
            DataSet dataSet = new DataSet();
            FileStream stream = null;
            try
            {
                var ExcelName = filename;
                if (ExcelName.EndsWith("xls") || ExcelName.EndsWith("xlsx"))
                {
                    //////
                    try
                    {
                        var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate").SingleOrDefault();
                        var UploadImageFile = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "UploadImageFile").SingleOrDefault();

                        var imagePath = UploadImageFile.LovValue1;
                        var imagepathimer = @ImpersonateVar.LovValue4;
                        string user = ImpersonateVar.LovValue1;
                        string pass = ImpersonateVar.LovValue2;
                        string ip = ImpersonateVar.LovValue3;


                        string yearweek = (DateTime.Now.Year.ToString());
                        string monthyear = (DateTime.Now.Month.ToString("00"));

                        var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));

                        imagepathimer = imagepathimerTemp;
                        _Logger.Info("Start Impersonate: ");

                        using (var impersonator = new Impersonator(user, ip, pass, false))
                        {
                            excelFiles = System.IO.File.ReadAllBytes(filename);
                            iExcelDataReader = null;
                            stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read);
                        }
                    }
                    catch (Exception ex)
                    {
                        _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                        _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                    }
                    //////

                    var checkexcel = true;
                    if (checkexcel)
                    {
                        if (filename.EndsWith("xls"))
                        {
                            iExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }

                        if (filename.EndsWith("xlsx"))
                        {
                            iExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }

                        iExcelDataReader.IsFirstRowAsColumnNames = true;

                        dataSet = iExcelDataReader.AsDataSet();

                        iExcelDataReader.Close();

                        if (dataSet != null && dataSet.Tables.Count > 0)
                        {
                            res = dataSet.Tables[0];
                        }

                        DataTable table = res;
                        var inttotalrow = table.Rows.Count;
                        if (filetotalrow == inttotalrow.ToSafeString())
                        {

                            foreach (DataRow dRow in table.Rows)
                            {
                                //i++;
                                if (null == dRow["No"].ToString() ||
                                    null == dRow["installAddress1"].ToString() ||
                                    null == dRow["installAddress2"].ToString() ||
                                    null == dRow["installAddress3"].ToString() ||
                                    null == dRow["installAddress4"].ToString() ||
                                    null == dRow["installAddress5"].ToString() ||
                                    null == dRow["latitude"].ToString() ||
                                    null == dRow["longitude"].ToString() ||
                                    null == dRow["install_date"].ToString()
                                    )
                                {
                                    break;
                                }

                                if (dRow["No"].ToString().Equals("")
                                    || dRow["installAddress1"].ToString().Equals("")
                                    || dRow["installAddress2"].ToString().Equals("")
                                    || dRow["latitude"].ToString().Equals("")
                                    || dRow["longitude"].ToString().Equals("")
                                    || dRow["install_date"].ToString().Equals("")
                                    )
                                {
                                    break;
                                }

                                var temp = new BulkExcelData();
                                temp.No = dRow["No"].ToString();
                                temp.installAddress1 = dRow["installAddress1"].ToString();
                                temp.installAddress2 = dRow["installAddress2"].ToString();
                                temp.installAddress3 = dRow["installAddress3"].ToString();
                                temp.installAddress4 = dRow["installAddress4"].ToString();
                                temp.installAddress5 = dRow["installAddress5"].ToString();
                                temp.latitude = dRow["latitude"].ToString();
                                temp.longitude = dRow["longitude"].ToString();
                                temp.install_date = dRow["install_date"].ToString();

                                ListExcelData.Add(temp);
                            }
                        }
                        stream.Dispose();
                    }
                }

            }
            catch (Exception e)
            {
                if (e.Message.Contains("does not belong to table"))
                {
                    _Logger.Info("Error excel file was missing field format");
                    var modelResponse = new { status = false, message = "This file was missing field format", filename = "" };
                    return Json(modelResponse, "text/plain");
                }
                else
                {
                    _Logger.Info("Error read excel file " + e.GetErrorMessage());
                }

            }
            #endregion

            if (null != ListExcelData)
            {
                var result = new List<BulkInsertExcel>();

                // We only care about the file name.
                var fileName = Session["filename"];
                var file_size = Session["totalsize"];

                var tableexcel = ListExcelData;
                var inttotalrow = Session["totalrow"];

                foreach (var DataRow in tableexcel)
                {
                    var temp = new BulkInsertExcel();
                    temp.No = DataRow.No.ToSafeString();
                    temp.installAddress1 = DataRow.installAddress1.ToSafeString();
                    temp.installAddress2 = DataRow.installAddress2.ToSafeString();
                    temp.installAddress3 = DataRow.installAddress3.ToSafeString();
                    temp.installAddress4 = DataRow.installAddress4.ToSafeString();
                    temp.installAddress5 = DataRow.installAddress5.ToSafeString();
                    temp.latitude = DataRow.latitude.ToSafeString();
                    temp.longitude = DataRow.longitude.ToSafeString();
                    temp.p_install_date = DataRow.install_date.ToSafeString();

                    temp.p_user = LoginUser.UserName.ToSafeString();
                    temp.p_file_name = fileName.ToSafeString();
                    temp.p_file_size = Int32.Parse(file_size.ToString());
                    temp.p_total_row = Int32.Parse(inttotalrow.ToString());
                    temp.p_bulk_no = BulkNo.ToSafeString();
                    temp.p_technology_install = tech_install.ToSafeString();
                    temp.p_address_id = address_id.ToSafeString();

                    temp.p_event_code = event_code.ToSafeString();
                    temp.p_contact_first_name = first_name.ToSafeString();
                    temp.p_contact_last_name = last_name.ToSafeString();
                    temp.p_contact_phone = phone.ToSafeString();
                    temp.p_contact_mobile = mobile.ToSafeString();
                    temp.p_contact_email = email.ToSafeString();

                    temp.pm_sff_promotion_code = MainPkg_sffpromocd.ToSafeString();
                    temp.pm_package_class = MainPkg_pkg_class.ToSafeString();
                    temp.pm_sff_promotion_bill_tha = MainPkg_sffpromobillth.ToSafeString();
                    temp.pm_sff_promotion_bill_eng = MainPkg_sffpromobilleng.ToSafeString();
                    temp.pm_package_name_tha = MainPkg_pkgnameth.ToSafeString();
                    temp.pm_recurring_charge = MainPkg_recur_charge.ToSafeDecimal();
                    temp.pm_pre_initiation_charge = MainPkg_pre_init_charge.ToSafeDecimal();
                    temp.pm_initiation_charge = MainPkg_init_charge.ToSafeDecimal();
                    temp.pm_download_speed = MainPkg_down_speed.ToSafeString();
                    temp.pm_upload_speed = MainPkg_up_speed.ToSafeString();
                    temp.pm_product_type = MainPkg_product_type.ToSafeString();
                    temp.pm_owner_product = MainPkg_owner_product.ToSafeString();
                    temp.pm_product_subtype = MainPkg_product_subtype.ToSafeString();
                    temp.pm_product_subtype2 = MainPkg_product_subtype2.ToSafeString();
                    temp.pm_technology = MainPkg_tech.ToSafeString();
                    temp.pm_package_group = MainPkg_pkg_grp.ToSafeString();
                    temp.pm_package_code = MainPkg_pkg_cd.ToSafeString();

                    temp.pi_sff_promotion_code = OntopPkg1_sffpromocd.ToSafeString();
                    temp.pi_package_class = OntopPkg1_pkg_class.ToSafeString();
                    temp.pi_sff_promotion_bill_tha = OntopPkg1_sffpromobillth.ToSafeString();
                    temp.pi_sff_promotion_bill_eng = OntopPkg1_sffpromobilleng.ToSafeString();
                    temp.pi_package_name_tha = OntopPkg1_pkgnameth.ToSafeString();
                    temp.pi_recurring_charge = OntopPkg1_recur_charge.ToSafeDecimal();
                    temp.pi_pre_initiation_charge = OntopPkg1_pre_init_charge.ToSafeDecimal();
                    temp.pi_initiation_charge = OntopPkg1_init_charge.ToSafeDecimal();
                    temp.pi_download_speed = OntopPkg1_down_speed.ToSafeString();
                    temp.pi_upload_speed = OntopPkg1_up_speed.ToSafeString();
                    temp.pi_product_type = OntopPkg1_product_type.ToSafeString();
                    temp.pi_owner_product = OntopPkg1_owner_product.ToSafeString();
                    temp.pi_product_subtype = OntopPkg1_product_subtype.ToSafeString();
                    temp.pi_product_subtype2 = OntopPkg1_product_subtype2.ToSafeString();
                    temp.pi_technology = OntopPkg1_tech.ToSafeString();
                    temp.pi_package_group = OntopPkg1_pkg_grp.ToSafeString();
                    temp.pi_package_code = OntopPkg1_pkg_cd.ToSafeString();

                    temp.pv_sff_promotion_code = OntopPkg2_sffpromocd.ToSafeString();
                    temp.pv_package_class = OntopPkg2_pkg_class.ToSafeString();
                    temp.pv_sff_promotion_bill_tha = OntopPkg2_sffpromobillth.ToSafeString();
                    temp.pv_sff_promotion_bill_eng = OntopPkg2_sffpromobilleng.ToSafeString();
                    temp.pv_package_name_tha = OntopPkg2_pkgnameth.ToSafeString();
                    temp.pv_recurring_charge = OntopPkg2_recur_charge.ToSafeDecimal();
                    temp.pv_pre_initiation_charge = OntopPkg2_pre_init_charge.ToSafeDecimal();
                    temp.pv_initiation_charge = OntopPkg2_init_charge.ToSafeDecimal();
                    temp.pv_download_speed = OntopPkg2_down_speed.ToSafeString();
                    temp.pv_upload_speed = OntopPkg2_up_speed.ToSafeString();
                    temp.pv_product_type = OntopPkg2_product_type.ToSafeString();
                    temp.pv_owner_product = OntopPkg2_owner_product.ToSafeString();
                    temp.pv_product_subtype = OntopPkg2_product_subtype.ToSafeString();
                    temp.pv_product_subtype2 = OntopPkg2_product_subtype2.ToSafeString();
                    temp.pv_technology = OntopPkg2_tech.ToSafeString();
                    temp.pv_package_group = OntopPkg2_pkg_grp.ToSafeString();
                    temp.pv_package_code = OntopPkg2_pkg_cd.ToSafeString();

                    temp.s1_service_code = ServicePkg1_servicecd.ToSafeString();
                    temp.s1_product_name = ServicePkg1_productname.ToSafeString();
                    temp.s2_service_code = ServicePkg2_servicecd.ToSafeString();
                    temp.s2_product_name = ServicePkg2_productname.ToSafeString();
                    temp.s3_service_code = ServicePkg3_servicecd.ToSafeString();
                    temp.s3_product_name = ServicePkg3_productname.ToSafeString();

                    temp.pod_sff_promotion_code = OntopDisPkg_sffpromocd.ToSafeString();
                    temp.pod_package_class = OntopDisPkg_pkg_class.ToSafeString();
                    temp.pod_sff_promotion_bill_tha = OntopDisPkg_sffpromobillth.ToSafeString();
                    temp.pod_sff_promotion_bill_eng = OntopDisPkg_sffpromobilleng.ToSafeString();
                    temp.pod_package_name_tha = OntopDisPkg_pkgnameth.ToSafeString();
                    temp.pod_recurring_charge = OntopDisPkg_recur_charge.ToSafeDecimal();
                    temp.pod_pre_initiation_charge = OntopDisPkg_pre_init_charge.ToSafeDecimal();
                    temp.pod_initiation_charge = OntopDisPkg_init_charge.ToSafeDecimal();
                    temp.pod_download_speed = OntopDisPkg_down_speed.ToSafeString();
                    temp.pod_upload_speed = OntopDisPkg_up_speed.ToSafeString();
                    temp.pod_product_type = OntopDisPkg_product_type.ToSafeString();
                    temp.pod_owner_product = OntopDisPkg_owner_product.ToSafeString();
                    temp.pod_product_subtype = OntopDisPkg_product_subtype.ToSafeString();
                    temp.pod_product_subtype2 = OntopDisPkg_product_subtype2.ToSafeString();
                    temp.pod_technology = OntopDisPkg_tech.ToSafeString();
                    temp.pod_package_group = OntopDisPkg_pkg_grp.ToSafeString();
                    temp.pod_package_code = OntopDisPkg_pkg_cd.ToSafeString();

                    result.Add(temp);

                    try
                    {
                        var query = new RegisterBulkCorpInsertExcelQuery()
                        {
                            p_no = temp.No,
                            p_installaddress1 = temp.installAddress1,
                            p_installaddress2 = temp.installAddress2,
                            p_installaddress3 = temp.installAddress3,
                            p_installaddress4 = temp.installAddress4,
                            p_installaddress5 = temp.installAddress5,
                            p_latitude = temp.latitude,
                            p_longitude = temp.longitude,

                            p_user = temp.p_user,
                            p_file_name = temp.p_file_name,
                            p_file_size = temp.p_file_size,
                            p_total_row = temp.p_total_row,
                            p_bulk_no = temp.p_bulk_no,
                            p_technology_install = temp.p_technology_install,
                            p_address_id = temp.p_address_id,
                            p_install_date = temp.p_install_date,
                            p_event_code = temp.p_event_code,
                            p_contact_first_name = temp.p_contact_first_name,
                            p_contact_last_name = temp.p_contact_last_name,
                            p_contact_phone = temp.p_contact_phone,
                            p_contact_mobile = temp.p_contact_mobile,
                            p_contact_email = temp.p_contact_email,

                            pm_sff_promotion_code = temp.pm_sff_promotion_code,
                            pm_package_class = temp.pm_package_class,
                            pm_sff_promotion_bill_tha = temp.pm_sff_promotion_bill_tha,
                            pm_sff_promotion_bill_eng = temp.pm_sff_promotion_bill_eng,
                            pm_package_name_tha = temp.pm_package_name_tha,
                            pm_recurring_charge = temp.pm_recurring_charge,
                            pm_pre_initiation_charge = temp.pm_pre_initiation_charge,
                            pm_initiation_charge = temp.pm_initiation_charge,
                            pm_download_speed = temp.pm_download_speed,
                            pm_upload_speed = temp.pm_upload_speed,
                            pm_product_type = temp.pm_product_type,
                            pm_owner_product = temp.pm_owner_product,
                            pm_product_subtype = temp.pm_product_subtype,
                            pm_product_subtype2 = temp.pm_product_subtype2,
                            pm_technology = temp.pm_technology,
                            pm_package_group = temp.pm_package_group,
                            pm_package_code = temp.pm_package_code,

                            pi_sff_promotion_code = temp.pi_sff_promotion_code,
                            pi_package_class = temp.pi_package_class,
                            pi_sff_promotion_bill_tha = temp.pi_sff_promotion_bill_tha,
                            pi_sff_promotion_bill_eng = temp.pi_sff_promotion_bill_eng,
                            pi_package_name_tha = temp.pi_package_name_tha,
                            pi_recurring_charge = temp.pi_recurring_charge,
                            pi_pre_initiation_charge = temp.pi_pre_initiation_charge,
                            pi_initiation_charge = temp.pi_initiation_charge,
                            pi_download_speed = temp.pi_download_speed,
                            pi_upload_speed = temp.pi_upload_speed,
                            pi_product_type = temp.pi_product_type,
                            pi_owner_product = temp.pi_owner_product,
                            pi_product_subtype = temp.pi_product_subtype,
                            pi_product_subtype2 = temp.pi_product_subtype2,
                            pi_technology = temp.pi_technology,
                            pi_package_group = temp.pi_package_group,
                            pi_package_code = temp.pi_package_code,

                            pv_sff_promotion_code = temp.pv_sff_promotion_code,
                            pv_package_class = temp.pv_package_class,
                            pv_sff_promotion_bill_tha = temp.pv_sff_promotion_bill_tha,
                            pv_sff_promotion_bill_eng = temp.pv_sff_promotion_bill_eng,
                            pv_package_name_tha = temp.pv_package_name_tha,
                            pv_recurring_charge = temp.pv_recurring_charge,
                            pv_pre_initiation_charge = temp.pv_pre_initiation_charge,
                            pv_initiation_charge = temp.pv_initiation_charge,
                            pv_download_speed = temp.pv_download_speed,
                            pv_upload_speed = temp.pv_upload_speed,
                            pv_product_type = temp.pv_product_type,
                            pv_owner_product = temp.pv_owner_product,
                            pv_product_subtype = temp.pv_product_subtype,
                            pv_product_subtype2 = temp.pv_product_subtype2,
                            pv_technology = temp.pv_technology,
                            pv_package_group = temp.pv_package_group,
                            pv_package_code = temp.pv_package_code,

                            s1_service_code = temp.s1_service_code,
                            s1_product_name = temp.s1_product_name,
                            s2_service_code = temp.s2_service_code,
                            s2_product_name = temp.s2_product_name,
                            s3_service_code = temp.s3_service_code,//new
                            s3_product_name = temp.s3_product_name,//new

                            pod_sff_promotion_code = temp.pod_sff_promotion_code,
                            pod_package_class = temp.pod_package_class,
                            pod_sff_promotion_bill_tha = temp.pod_sff_promotion_bill_tha,
                            pod_sff_promotion_bill_eng = temp.pod_sff_promotion_bill_eng,
                            pod_package_name_tha = temp.pod_package_name_tha,
                            pod_recurring_charge = temp.pod_recurring_charge,
                            pod_pre_initiation_charge = temp.pod_pre_initiation_charge,
                            pod_initiation_charge = temp.pod_initiation_charge,
                            pod_download_speed = temp.pod_download_speed,
                            pod_upload_speed = temp.pod_upload_speed,
                            pod_product_type = temp.pod_product_type,
                            pod_owner_product = temp.pod_owner_product,
                            pod_product_subtype = temp.pod_product_subtype,
                            pod_product_subtype2 = temp.pod_product_subtype2,
                            pod_technology = temp.pod_technology,
                            pod_package_group = temp.pod_package_group,
                            pod_package_code = temp.pod_package_code
                        };

                        var returnexceltemp = _queryProcessor.Execute(query);

                        retexcel.output_bulk_no = returnexceltemp.output_bulk_no;
                        retexcel.output_return_code = returnexceltemp.output_return_code;
                        retexcel.output_return_message = returnexceltemp.output_return_message;

                        returnexcel.Add(retexcel);

                    }

                    catch (Exception ex)
                    {
                        _Logger.Info("Error when call RegisterBulkCorpInsertExcelQuery in SaveRegisterPkg " + ex.GetErrorMessage());
                        retexcel.output_bulk_no = "";
                        retexcel.output_return_code = "-1";
                        retexcel.output_return_message = "ERROR " + ex.GetErrorMessage();

                        returnexcel.Add(retexcel);
                        return Json(returnexcel, JsonRequestBehavior.AllowGet);
                    }

                }//End foreach


                try
                {
                    var queryp = new SaveUploadFileBulkCorpQuery()
                    {
                        p_bulk_number = BulkNo.ToSafeString(),
                        p_file_name = fileName.ToSafeString()
                    };
                    var datap = _queryProcessor.Execute(queryp);

                }
                catch (Exception ex)
                {
                    _Logger.Info("Error When Call SaveUploadFileBulkCorpQuery for Excel File " + ex.GetErrorMessage());

                    retexcel.output_bulk_no = "";
                    retexcel.output_return_code = "-1";
                    retexcel.output_return_message = "ERROR ListExcelData is null";
                    return Json(retexcel, JsonRequestBehavior.AllowGet);
                }

                return Json(returnexcel, JsonRequestBehavior.AllowGet);

            }
            else
            {
                _Logger.Info("Error when get value from Session in SaveRegisterPkg ");
                retexcel.output_bulk_no = "";
                retexcel.output_return_code = "-1";
                retexcel.output_return_message = "ERROR ListExcelData is null";

                //var modelResponse = new { status = false, message = "This file has missing field format", filename = "" };
                //return Json(modelResponse, "text/plain");
                return Json(retexcel, JsonRequestBehavior.AllowGet);
            }


        }

        //public ActionResult SaveRegisterPkg(
        //   string tech_install = "", string address_id = "",string install_date = "", string event_code = "", string first_name = "",
        //   string last_name = "", string phone = "", string mobile = "", string email = "",
        //   string MainPkg_sffpromocd = "", string MainPkg_pkg_class = "", string MainPkg_sffpromobillth = "",
        //   string MainPkg_sffpromobilleng = "", string MainPkg_pkgnameth = "", string MainPkg_pkgnameeng = "",
        //   string MainPkg_recur_charge = "", string MainPkg_pre_init_charge = "", string MainPkg_init_charge = "",
        //   string MainPkg_down_speed = "", string MainPkg_up_speed = "", string MainPkg_product_type = "", string MainPkg_owner_product = "",
        //   string MainPkg_product_subtype = "", string MainPkg_product_subtype2 = "", string MainPkg_tech = "",
        //   string MainPkg_pkg_grp = "", string MainPkg_pkg_cd = "", string MainPkg_network_type = "", string MainPkg_cust_type = "",
        //   string OntopPkg1_sffpromocd = "", string OntopPkg1_pkg_class = "", string OntopPkg1_sffpromobillth = "",
        //   string OntopPkg1_sffpromobilleng = "", string OntopPkg1_pkgnameth = "", string OntopPkg1_pkgnameeng = "",
        //   string OntopPkg1_recur_charge = "", string OntopPkg1_pre_init_charge = "", string OntopPkg1_init_charge = "",
        //   string OntopPkg1_down_speed = "", string OntopPkg1_up_speed = "", string OntopPkg1_product_type = "",
        //   string OntopPkg1_owner_product = "", string OntopPkg1_product_subtype = "", string OntopPkg1_product_subtype2 = "",
        //   string OntopPkg1_tech = "", string OntopPkg1_pkg_grp = "", string OntopPkg1_pkg_cd = "", string OntopPkg1_cust_type = "",
        //   string OntopPkg2_sffpromocd = "", string OntopPkg2_pkg_class = "", string OntopPkg2_sffpromobillth = "",
        //   string OntopPkg2_sffpromobilleng = "", string OntopPkg2_pkgnameth = "", string OntopPkg2_pkgnameeng = "",
        //   string OntopPkg2_recur_charge = "", string OntopPkg2_pre_init_charge = "", string OntopPkg2_init_charge = "",
        //   string OntopPkg2_down_speed = "", string OntopPkg2_up_speed = "", string OntopPkg2_product_type = "",
        //   string OntopPkg2_owner_product = "", string OntopPkg2_product_subtype = "", string OntopPkg2_product_subtype2 = "",
        //   string OntopPkg2_tech = "", string OntopPkg2_pkg_grp = "", string OntopPkg2_pkg_cd = "", string OntopPkg2_cust_type = "",
        //   string ServicePkg1_servicecd = "", string ServicePkg1_productname = "",
        //   string ServicePkg2_servicecd = "", string ServicePkg2_productname = "", string BulkNo = ""
        //   )
        //{

        //    var LoginUser = base.CurrentUser;
        //    if (null == base.CurrentUser)
        //        return Json("Sesson Time Out", JsonRequestBehavior.AllowGet);

        //    var returnexcel = new List<ReturnInsertExcelData>();
        //    ReturnInsertExcelData retexcel = new ReturnInsertExcelData();

        //    //List<BulkExcelData> ListExcelData = Session["Bulktempupload"] as List<BulkExcelData>;
        //    List<BulkExcelData> ListExcelData = new List<BulkExcelData>();

        //    var tempSelectMainPkg = Session["Select_Main_Package"] as List<ReturnPackageList>;
        //    if (tempSelectMainPkg == null)
        //        tempSelectMainPkg = new List<ReturnPackageList>();


        //    var tempOntopPkg1 = Session["dataOntop1"] as ReturnOntopPackageList;
        //    if (tempOntopPkg1 == null)
        //        tempOntopPkg1 = new ReturnOntopPackageList();

        //    var tempOntopPkg2 = Session["dataOntop2"] as ReturnOntopPackageList;
        //    if (tempOntopPkg2 == null)
        //        tempOntopPkg2 = new ReturnOntopPackageList();

        //    var tempServicePkg1 = Session["dataService1"] as ReturnServicePackageList;
        //    if (tempServicePkg1 == null)
        //        tempServicePkg1 = new ReturnServicePackageList();

        //    var tempServicePkg2 = Session["dataService2"] as ReturnServicePackageList;
        //    if (tempServicePkg2 == null)
        //       tempServicePkg2 = new ReturnServicePackageList();


        //    var filename = Session["filename"] as String;
        //    if (string.IsNullOrEmpty(filename))
        //    {
        //        retexcel.output_bulk_no = "";
        //        retexcel.output_return_code = "-1";
        //        retexcel.output_return_message = "ERROR ListExcelData is null";
        //        return Json(retexcel, JsonRequestBehavior.AllowGet);
        //    }

        //    var filesize = Session["totalsize"] as String;
        //    if (string.IsNullOrEmpty(filesize))
        //    {
        //        retexcel.output_bulk_no = "";
        //        retexcel.output_return_code = "-1";
        //        retexcel.output_return_message = "ERROR ListExcelData is null";
        //        return Json(retexcel, JsonRequestBehavior.AllowGet);
        //    }


        //    if (null != ListExcelData)
        //    {
        //        var result = new List<BulkInsertExcel>();

        //        // We only care about the file name.
        //        var fileName = Session["filename"];
        //        var file_size = Session["totalsize"];

        //        var table = ListExcelData;
        //        var inttotalrow = Session["totalrow"];

        //        foreach (var DataRow in table)
        //        {
        //            var temp = new BulkInsertExcel();
        //            temp.No = DataRow.No.ToSafeString();
        //            temp.installAddress1 = DataRow.installAddress1.ToSafeString();
        //            temp.installAddress2 = DataRow.installAddress2.ToSafeString();
        //            temp.installAddress3 = DataRow.installAddress3.ToSafeString();
        //            temp.installAddress4 = DataRow.installAddress4.ToSafeString();
        //            temp.installAddress5 = DataRow.installAddress5.ToSafeString();
        //            temp.latitude = DataRow.latitude.ToSafeString();
        //            temp.longitude = DataRow.longitude.ToSafeString();
        //            temp.dpName = DataRow.dpName.ToSafeString();
        //            temp.installationCapacity = DataRow.installationCapacity.ToSafeString();
        //            temp.ia = DataRow.ia.ToSafeString();
        //            temp.password = DataRow.password.ToSafeString();
        //            temp.p_user = LoginUser.UserName.ToSafeString();
        //            temp.p_file_name = fileName.ToSafeString();
        //            temp.p_file_size = Int32.Parse(file_size.ToString());
        //            temp.p_total_row = Int32.Parse(inttotalrow.ToString());
        //            temp.p_bulk_no = BulkNo.ToSafeString();
        //            temp.p_technology_install = tech_install.ToSafeString();
        //            temp.p_address_id = address_id.ToSafeString();
        //            temp.p_install_date = install_date.ToSafeString();
        //            temp.p_event_code = event_code.ToSafeString();
        //            temp.p_contact_first_name = first_name.ToSafeString();
        //            temp.p_contact_last_name = last_name.ToSafeString();
        //            temp.p_contact_phone = phone.ToSafeString();
        //            temp.p_contact_mobile = mobile.ToSafeString();
        //            temp.p_contact_email = email.ToSafeString();

        //            temp.pm_sff_promotion_code = MainPkg_sffpromocd.ToSafeString();
        //            temp.pm_package_class = MainPkg_pkg_class.ToSafeString();
        //            temp.pm_sff_promotion_bill_tha = MainPkg_sffpromobillth.ToSafeString();
        //            temp.pm_sff_promotion_bill_eng = MainPkg_sffpromobilleng.ToSafeString();
        //            temp.pm_package_name_tha = MainPkg_pkgnameth.ToSafeString();
        //            temp.pm_recurring_charge = MainPkg_recur_charge.ToSafeDecimal();
        //            temp.pm_pre_initiation_charge = MainPkg_pre_init_charge.ToSafeDecimal();
        //            temp.pm_initiation_charge = MainPkg_init_charge.ToSafeDecimal();
        //            temp.pm_download_speed = MainPkg_down_speed.ToSafeString();
        //            temp.pm_upload_speed = MainPkg_up_speed.ToSafeString();
        //            temp.pm_product_type = MainPkg_product_type.ToSafeString();
        //            temp.pm_owner_product = MainPkg_owner_product.ToSafeString();
        //            temp.pm_product_subtype = MainPkg_product_subtype.ToSafeString();
        //            temp.pm_product_subtype2 = MainPkg_product_subtype2.ToSafeString();
        //            temp.pm_technology = MainPkg_tech.ToSafeString();
        //            temp.pm_package_group = MainPkg_pkg_grp.ToSafeString();
        //            temp.pm_package_code = MainPkg_pkg_cd.ToSafeString();

        //            temp.pi_sff_promotion_code = OntopPkg1_sffpromocd.ToSafeString();
        //            temp.pi_package_class = OntopPkg1_pkg_class.ToSafeString();
        //            temp.pi_sff_promotion_bill_tha = OntopPkg1_sffpromobillth.ToSafeString();
        //            temp.pi_sff_promotion_bill_eng = OntopPkg1_sffpromobilleng.ToSafeString();
        //            temp.pi_package_name_tha = OntopPkg1_pkgnameth.ToSafeString();
        //            temp.pi_recurring_charge = OntopPkg1_recur_charge.ToSafeDecimal();
        //            temp.pi_pre_initiation_charge = OntopPkg1_pre_init_charge.ToSafeDecimal();
        //            temp.pi_initiation_charge = OntopPkg1_init_charge.ToSafeDecimal();
        //            temp.pi_download_speed = OntopPkg1_down_speed.ToSafeString();
        //            temp.pi_upload_speed = OntopPkg1_up_speed.ToSafeString();
        //            temp.pi_product_type = OntopPkg1_product_type.ToSafeString();
        //            temp.pi_owner_product = OntopPkg1_owner_product.ToSafeString();
        //            temp.pi_product_subtype = OntopPkg1_product_subtype.ToSafeString();
        //            temp.pi_product_subtype2 = OntopPkg1_product_subtype2.ToSafeString();
        //            temp.pi_technology = OntopPkg1_tech.ToSafeString();
        //            temp.pi_package_group = OntopPkg1_pkg_grp.ToSafeString();
        //            temp.pi_package_code = OntopPkg1_pkg_cd.ToSafeString();

        //            temp.pv_sff_promotion_code = OntopPkg2_sffpromocd.ToSafeString();
        //            temp.pv_package_class = OntopPkg2_pkg_class.ToSafeString();
        //            temp.pv_sff_promotion_bill_tha = OntopPkg2_sffpromobillth.ToSafeString();
        //            temp.pv_sff_promotion_bill_eng = OntopPkg2_sffpromobilleng.ToSafeString();
        //            temp.pv_package_name_tha = OntopPkg2_pkgnameth.ToSafeString();
        //            temp.pv_recurring_charge = OntopPkg2_recur_charge.ToSafeDecimal();
        //            temp.pv_pre_initiation_charge = OntopPkg2_pre_init_charge.ToSafeDecimal();
        //            temp.pv_initiation_charge = OntopPkg2_init_charge.ToSafeDecimal();
        //            temp.pv_download_speed = OntopPkg2_down_speed.ToSafeString();
        //            temp.pv_upload_speed = OntopPkg2_up_speed.ToSafeString();
        //            temp.pv_product_type = OntopPkg2_product_type.ToSafeString();
        //            temp.pv_owner_product = OntopPkg2_owner_product.ToSafeString();
        //            temp.pv_product_subtype = OntopPkg2_product_subtype.ToSafeString();
        //            temp.pv_product_subtype2 = OntopPkg2_product_subtype2.ToSafeString();
        //            temp.pv_technology = OntopPkg2_tech.ToSafeString();
        //            temp.pv_package_group = OntopPkg2_pkg_grp.ToSafeString();
        //            temp.pv_package_code = OntopPkg2_pkg_cd.ToSafeString();

        //            temp.s1_service_code = ServicePkg1_servicecd.ToSafeString();
        //            temp.s1_product_name = ServicePkg1_productname.ToSafeString();
        //            temp.s2_service_code = ServicePkg2_servicecd.ToSafeString();
        //            temp.s2_product_name = ServicePkg2_productname.ToSafeString();

        //            result.Add(temp);

        //            try
        //            {
        //                var query = new RegisterBulkCorpInsertExcelQuery()
        //                {
        //                    p_no = temp.No,
        //                    p_installaddress1 = temp.installAddress1,
        //                    p_installaddress2 = temp.installAddress2,
        //                    p_installaddress3 = temp.installAddress3,
        //                    p_installaddress4 = temp.installAddress4,
        //                    p_installaddress5 = temp.installAddress5,
        //                    p_latitude = temp.latitude,
        //                    p_longitude = temp.longitude,
        //                    p_dpname = temp.dpName,
        //                    p_installationcapacity = temp.installationCapacity,
        //                    p_ia = temp.ia,
        //                    p_password = temp.password,

        //                    p_user = temp.p_user,
        //                    p_file_name = temp.p_file_name,
        //                    p_file_size = temp.p_file_size,
        //                    p_total_row = temp.p_total_row,
        //                    p_bulk_no = temp.p_bulk_no,
        //                    p_technology_install = temp.p_technology_install,
        //                    p_address_id = temp.p_address_id,
        //                    p_install_date = temp.p_install_date,
        //                    p_event_code = temp.p_event_code,
        //                    p_contact_first_name = temp.p_contact_first_name,
        //                    p_contact_last_name = temp.p_contact_last_name,
        //                    p_contact_phone = temp.p_contact_phone,
        //                    p_contact_mobile = temp.p_contact_mobile,
        //                    p_contact_email = temp.p_contact_email,

        //                    pm_sff_promotion_code = temp.pm_sff_promotion_code,
        //                    pm_package_class = temp.pm_package_class,
        //                    pm_sff_promotion_bill_tha = temp.pm_sff_promotion_bill_tha,
        //                    pm_sff_promotion_bill_eng = temp.pm_sff_promotion_bill_eng,
        //                    pm_package_name_tha = temp.pm_package_name_tha,
        //                    pm_recurring_charge = temp.pm_recurring_charge,
        //                    pm_pre_initiation_charge = temp.pm_pre_initiation_charge,
        //                    pm_initiation_charge = temp.pm_initiation_charge,
        //                    pm_download_speed = temp.pm_download_speed,
        //                    pm_upload_speed = temp.pm_upload_speed,
        //                    pm_product_type = temp.pm_product_type,
        //                    pm_owner_product = temp.pm_owner_product,
        //                    pm_product_subtype = temp.pm_product_subtype,
        //                    pm_product_subtype2 = temp.pm_product_subtype2,
        //                    pm_technology = temp.pm_technology,
        //                    pm_package_group = temp.pm_package_group,
        //                    pm_package_code = temp.pm_package_code,

        //                    pi_sff_promotion_code = temp.pi_sff_promotion_code,
        //                    pi_package_class = temp.pi_package_class,
        //                    pi_sff_promotion_bill_tha = temp.pi_sff_promotion_bill_tha,
        //                    pi_sff_promotion_bill_eng = temp.pi_sff_promotion_bill_eng,
        //                    pi_package_name_tha = temp.pi_package_name_tha,
        //                    pi_recurring_charge = temp.pi_recurring_charge,
        //                    pi_pre_initiation_charge = temp.pi_pre_initiation_charge,
        //                    pi_initiation_charge = temp.pi_initiation_charge,
        //                    pi_download_speed = temp.pi_download_speed,
        //                    pi_upload_speed = temp.pi_upload_speed,
        //                    pi_product_type = temp.pi_product_type,
        //                    pi_owner_product = temp.pi_owner_product,
        //                    pi_product_subtype = temp.pi_product_subtype,
        //                    pi_product_subtype2 = temp.pi_product_subtype2,
        //                    pi_technology = temp.pi_technology,
        //                    pi_package_group = temp.pi_package_group,
        //                    pi_package_code = temp.pi_package_code,

        //                    pv_sff_promotion_code = temp.pv_sff_promotion_code,
        //                    pv_package_class = temp.pv_package_class,
        //                    pv_sff_promotion_bill_tha = temp.pv_sff_promotion_bill_tha,
        //                    pv_sff_promotion_bill_eng = temp.pv_sff_promotion_bill_eng,
        //                    pv_package_name_tha = temp.pv_package_name_tha,
        //                    pv_recurring_charge = temp.pv_recurring_charge,
        //                    pv_pre_initiation_charge = temp.pv_pre_initiation_charge,
        //                    pv_initiation_charge = temp.pv_initiation_charge,
        //                    pv_download_speed = temp.pv_download_speed,
        //                    pv_upload_speed = temp.pv_upload_speed,
        //                    pv_product_type = temp.pv_product_type,
        //                    pv_owner_product = temp.pv_owner_product,
        //                    pv_product_subtype = temp.pv_product_subtype,
        //                    pv_product_subtype2 = temp.pv_product_subtype2,
        //                    pv_technology = temp.pv_technology,
        //                    pv_package_group = temp.pv_package_group,
        //                    pv_package_code = temp.pv_package_code,

        //                    s1_service_code = temp.s1_service_code,
        //                    s1_product_name = temp.s1_product_name,
        //                    s2_service_code = temp.s2_service_code,
        //                    s2_product_name = temp.s2_product_name

        //                };

        //                var returnexceltemp = _queryProcessor.Execute(query);

        //                retexcel.output_bulk_no = returnexceltemp.output_bulk_no;
        //                retexcel.output_return_code = returnexceltemp.output_return_code;
        //                retexcel.output_return_message = returnexceltemp.output_return_message;

        //                returnexcel.Add(retexcel);

        //            }

        //            catch (Exception ex)
        //            {
        //                _Logger.Info("Error when call RegisterBulkCorpInsertExcelQuery in SaveRegisterPkg " + ex.GetErrorMessage());
        //                retexcel.output_bulk_no = "";
        //                retexcel.output_return_code = "-1";
        //                retexcel.output_return_message = "ERROR " + ex.GetErrorMessage();

        //                returnexcel.Add(retexcel);
        //                return Json(returnexcel, JsonRequestBehavior.AllowGet);
        //            }

        //        }//End foreach


        //        try
        //        {
        //            var queryp = new SaveUploadFileBulkCorpQuery()
        //            {
        //                p_bulk_number = BulkNo.ToSafeString(),
        //                p_file_name = fileName.ToSafeString()
        //            };
        //            var datap = _queryProcessor.Execute(queryp);

        //        }
        //        catch (Exception ex)
        //        {
        //            _Logger.Info("Error When Call SaveUploadFileBulkCorpQuery for Excel File " + ex.GetErrorMessage());

        //            retexcel.output_bulk_no = "";
        //            retexcel.output_return_code = "-1";
        //            retexcel.output_return_message = "ERROR ListExcelData is null";
        //            return Json(retexcel, JsonRequestBehavior.AllowGet);
        //        }

        //        return Json(returnexcel, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {

        //        _Logger.Info("Error when get value from Session in SaveRegisterPkg ");
        //        retexcel.output_bulk_no = "";
        //        retexcel.output_return_code = "-1";
        //        retexcel.output_return_message = "ERROR ListExcelData is null";

        //        //var modelResponse = new { status = false, message = "This file has missing field format", filename = "" };
        //        //return Json(modelResponse, "text/plain");
        //        return Json(retexcel, JsonRequestBehavior.AllowGet);
        //    }


        //}

        public JsonResult SaveImageFilenametoStore(string FileName, string BulkNo)
        {
            try
            {
                var queryp = new SaveUploadFileBulkCorpQuery()
                {
                    p_bulk_number = BulkNo,
                    p_file_name = FileName.ToSafeString()
                };

                var dataret = _queryProcessor.Execute(queryp);

                return Json(dataret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Logger.Info("Error When Call SaveUploadFileBulkCorpQuery for Excel File " + ex.GetErrorMessage());

                var modelResponse = new { status = false, message = "Cannot save upload file name", filename = "" };
                return Json(modelResponse, "text/plain");
            }
        }


        public DataTable getDataSet(string path)
        {
            // Get the Excel file and convert to dataset 
            DataTable res = null;
            DataSet dataSet;
            IExcelDataReader iExcelDataReader = null;
            FileStream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read);

            if (path.EndsWith("xls"))
            {
                iExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }

            if (path.EndsWith("xlsx"))
            {
                iExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            stream.Dispose();
            if (iExcelDataReader != null)
            {
                iExcelDataReader.IsFirstRowAsColumnNames = true;

                dataSet = iExcelDataReader.AsDataSet();

                iExcelDataReader.Close();

                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    res = dataSet.Tables[0];
                }
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

        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var i = 0;
            foreach (DataRow drow in dTable.Rows)
            {
                i++;
                if (hTable.Contains(drow[colName]))
                {
                    duplicateList.Add(drow);
                    //dupinfile.Add(i);
                }
                else
                {
                    hTable.Add(drow[colName], string.Empty);
                }
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            stopwatch.Stop();
            var sds = stopwatch.Elapsed;
            return dTable;
        }

        public JsonResult DocSummaryPkg(string BulkNo)
        {
            ReturnDocumentSum DocSummary = new ReturnDocumentSum();
            try
            {
                _Logger.Info("Start RegisterBulkCorpDocSumQuery");
                var query = new RegisterBulkCorpDocSumQuery()
                {
                    Bulk_No = BulkNo
                };

                DocSummary = _queryProcessor.Execute(query);

                if (null != DocSummary)
                    return Json(DocSummary, JsonRequestBehavior.AllowGet);

                else
                {
                    return Json(DocSummary, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _Logger.Info("Error " + ex.GetErrorMessage());
                return Json(DocSummary, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Gettogrid([DataSourceRequest] DataSourceRequest request)
        {
            var tempupload = Session["Bulktempupload"] as List<BulkExcelData>;
            if (tempupload == null)
                tempupload = new List<BulkExcelData>();

            return Json(tempupload.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Upload Image files

        [HttpPost]
        public ActionResult UploadImageBulk(string cateType, string cardNo, string cardType, string register_dv, string MobileNumber, string Bulk_No)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    #region Get IP Address Interface Log (Update 17.2)

                    // Get IP Address
                    string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    #endregion

                    List<string> Arr_files = new List<string>();
                    RegisterBulkCorpModel model = new RegisterBulkCorpModel();
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase[] filesPosted = new HttpPostedFileBase[files.Count];

                    for (int i = 0; i < files.Count; i++)
                    {
                        filesPosted[i] = files[i];
                    }

                    model.Register_device = register_dv;
                    model.account_category = cateType;
                    model.id_card_no = cardNo;
                    model.id_card_type = cardType;

                    model.ca_main_mobile = MobileNumber;
                    model.ClientIP = ipAddress;
                    model.out_bulk_number = Bulk_No;

                    filesPostedRegisterBulkTempStep = filesPosted;
                    model = SaveFileImage(filesPosted, model);
                    if (model.ListImageFileBulk.Any())
                        return Json(model.ListImageFileBulk);
                    else
                        throw new Exception("Null ListImageFile");
                }
                catch (Exception ex)
                {

                    _Logger.Info("Error Upload Image RegisterBulkCorp:" + ex.GetErrorMessage());
                    _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                    return Json(false);
                }
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost]
        public RegisterBulkCorpInstallModel UploadExcelBulk(string cateType, string cardNo, string cardType, string register_dv, string MobileNumber,
            string Bulk_No, DataTable dt)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    #region Get IP Address Interface Log (Update 17.2)

                    // Get IP Address
                    string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    #endregion

                    List<string> Arr_files = new List<string>();
                    RegisterBulkCorpInstallModel model = new RegisterBulkCorpInstallModel();
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase[] filesPosted = new HttpPostedFileBase[files.Count];

                    for (int i = 0; i < files.Count; i++)
                    {
                        filesPosted[i] = files[i];
                    }

                    model.Register_device = register_dv;
                    model.account_category = cateType;
                    model.id_card_no = cardNo;
                    model.id_card_type = cardType;

                    model.ca_main_mobile = MobileNumber;
                    model.ClientIP = ipAddress;
                    model.output_bulk_no = Bulk_No;

                    filesPostedRegisterExcelBulkTempStep = filesPosted;
                    model = SaveFileExcel(filesPosted, model, dt);
                    if (null != model.ExcelBulk)
                        //return Json(model.ExcelBulk);
                        return model;
                    else
                        throw new Exception("Null ListExcelFile");
                }
                catch (Exception ex)
                {

                    _Logger.Info("Error Upload Excel RegisterBulkCorp:" + ex.GetErrorMessage());
                    _Logger.Info("Error Upload Excel With Stack Trace : " + ex.RenderExceptionMessage());
                    return new RegisterBulkCorpInstallModel();
                    //return Json(false);
                }
            }
            else
            {
                return new RegisterBulkCorpInstallModel();
                //return Json(false);
            }
        }

        private RegisterBulkCorpInstallModel SaveFileExcel(HttpPostedFileBase[] files, RegisterBulkCorpInstallModel model, DataTable dt)
        {
            string cardNo = model.id_card_no;

            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;

            string transactionId = (model.ca_main_mobile + model.ClientIP).ToSafeString();

            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "SaveFileExcel", transactionId, cardNo, "WEB");

            try
            {
                var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate").SingleOrDefault();
                var UploadImageFile = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "UploadImageFile").SingleOrDefault();

                var imagePath = UploadImageFile.LovValue1;
                var imagepathimer = @ImpersonateVar.LovValue4;
                string user = ImpersonateVar.LovValue1;
                string pass = ImpersonateVar.LovValue2;
                string ip = ImpersonateVar.LovValue3;


                string yearweek = (DateTime.Now.Year.ToString());
                string monthyear = (DateTime.Now.Month.ToString("00"));

                var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));

                log2 = StartInterface("IdcardNo:" + cardNo + "Path : " + imagepathimerTemp + "\r\n", "Directory Check", transactionId, cardNo, "WEB");
                //if (Directory.Exists(imagepathimerTemp))
                EndInterface("", log2, transactionId, "Success", "");
                //else
                //{
                //    EndInterface("", log2, cardNo, "ERROR", "Directory Not Found : " + imagepathimerTemp + "\r\n" + "DirectoryExists: " + Directory.Exists(imagepathimerTemp) + "\r\n" + "imagepathimer: " + imagepathimer);
                //    imagepathimerTemp = imagepathimer;
                //}

                imagepathimer = imagepathimerTemp;
                _Logger.Info("Start Impersonate:");

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    if (string.IsNullOrEmpty(model.Register_device))
                    {
                        model.ExcelBulk
                                = ConvertHttpPostedFileBaseToUploadExcel(files, model, imagepathimer, dt);

                        if (null == model.ExcelBulk)
                        {
                            var base64photoDict = Session["ExcelBulk"] as Dictionary<string, string>;
                            model.ExcelBulk
                                = ConvertBase64PhotoToUploadExcel(base64photoDict, model, imagepathimer);
                        }
                    }
                    else if (model.Register_device == "MOBILE APP")
                    {
                        var base64photoDict = Session["ExcelBulk"] as Dictionary<string, string>;
                        model.ExcelBulk
                            = ConvertBase64PhotoToUploadExcel(base64photoDict, model, imagepathimer);
                    }
                    else
                    {
                        model.ExcelBulk
                            = ConvertHttpPostedFileBaseToUploadExcel(files, model, imagepathimer, dt);
                    }

                    _Logger.Info("End Impersonate:");
                    Session["ExcelBulk"] = null;

                    EndInterface("", log, transactionId, "Success", "");

                    return model;

                }
            }
            catch (Exception ex)
            {
                EndInterface("", log, transactionId, "ERROR", ex.GetErrorMessage());

                _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return model;
            }
        }

        private HttpPostedFileBase[] filesPostedRegisterExcelBulkTempStep;
        private HttpPostedFileBase[] filesPostedRegisterBulkTempStep;

        private UploadExcelBulk ConvertBase64PhotoToUploadExcel(
            Dictionary<string, string> excelDict,
            RegisterBulkCorpInstallModel model,
            string imagepathimer)
        {
            if (null == excelDict)
            {
                _Logger.Info("excel not contain valid files");
                return new UploadExcelBulk();
            }

            foreach (var item in excelDict)
            {
                var uploadExcelBulk = new UploadExcelBulk();
                uploadExcelBulk.FileExcelBulk = item.Key + Guid.NewGuid().ToString() + ".xlsx";
                model.ExcelBulk = uploadExcelBulk;
            }

            var path = "";
            var resultFormat = GetFormatExcelFile(model);
            var tempfile = new UploadExcelBulk();

            var fileIndex = 0;
            foreach (var item in excelDict)
            {
                path = Path.Combine(imagepathimer, resultFormat[fileIndex].file_name_bulk);
                var uploadImageWithGeneratedName = new UploadExcelBulk();
                uploadImageWithGeneratedName.FileExcelBulk = path;
                tempfile = uploadImageWithGeneratedName;

                var imgBytes = Convert.FromBase64String(excelDict[item.Key]);
                System.IO.File.WriteAllBytes(path, imgBytes);
                fileIndex++;
            }

            return tempfile;
        }

        private List<UploadImageBulk> ConvertHttpPostedFileBaseToUploadImage(
            HttpPostedFileBase[] files,
            RegisterBulkCorpModel model,
            string imagepathimer)
        {
            string cardNo = model.id_card_no;
            model.ListImageFileBulk = new List<UploadImageBulk>();

            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertHttpPostedFileBaseToUploadImage", model.ca_main_mobile + model.ClientIP, cardNo, "WEB");
            try
            {
                var width = 750;
                var height = 562;

                if (files.Count() <= 0)
                {
                    _Logger.Info("base64photo not contain valid files");
                    return new List<UploadImageBulk>();
                }

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            var varfileName = Path.GetFileName(files[i].FileName);
                            var p = new UploadImageBulk();
                            p.FileNameBulk = varfileName;
                            model.ListImageFileBulk.Add(p);
                        }
                    }
                }

                var resultFormat = GetFormatFile(model);
                var tempfile = new List<UploadImageBulk>();
                var j = 0;

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        var path = Path.Combine(imagepathimer, resultFormat[j].file_name_bulk);
                        var type = resultFormat[j].file_name_bulk.Substring(resultFormat[j].file_name_bulk.IndexOf(".") + 1).ToLower();
                        var p2 = new UploadImageBulk();


                        p2.FileNameBulk = path;
                        tempfile.Add(p2);

                        if (files[i].ContentLength > 204800 && type != "pdf")
                        {
                            using (System.Drawing.Bitmap img
                                = ResizeImage(new System.Drawing.Bitmap(files[i].InputStream),
                                                width, height, ResizeOptions.MaxWidth))
                            {
                                img.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }

                        }
                        else
                        {
                            files[i].SaveAs(path);

                        }

                        j++;
                    }
                }
                EndInterface("", log, model.ca_main_mobile + model.ClientIP, "Success", "");
                return tempfile;
            }
            catch (Exception ex)
            {
                EndInterface("", log, model.ca_main_mobile + model.ClientIP, "ERROR", ex.GetErrorMessage());

                _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new List<UploadImageBulk>();
            }


        }

        private RegisterBulkCorpModel SaveFileImage(HttpPostedFileBase[] files, RegisterBulkCorpModel model)
        {
            string cardNo = model.id_card_no;

            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;

            string transactionId = (model.ca_main_mobile + model.ClientIP).ToSafeString();

            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "SaveFileImage", transactionId, cardNo, "WEB");

            try
            {
                var ImpersonateVar = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "Impersonate").SingleOrDefault();
                var UploadImageFile = base.LovData.Where(l => l.Type == "FBB_CONSTANT" && l.Name == "UploadImageFile").SingleOrDefault();

                var imagePath = UploadImageFile.LovValue1;
                var imagepathimer = @ImpersonateVar.LovValue4;
                string user = ImpersonateVar.LovValue1;
                string pass = ImpersonateVar.LovValue2;
                string ip = ImpersonateVar.LovValue3;


                string yearweek = (DateTime.Now.Year.ToString());
                string monthyear = (DateTime.Now.Month.ToString("00"));

                var imagepathimerTemp = Path.Combine(imagepathimer, (yearweek + monthyear));

                log2 = StartInterface("IdcardNo:" + cardNo + "Path : " + imagepathimerTemp + "\r\n", "Directory Check", transactionId, cardNo, "WEB");
                //if (Directory.Exists(imagepathimerTemp))
                EndInterface("", log2, transactionId, "Success", "");
                //else
                //{
                //    EndInterface("", log2, cardNo, "ERROR", "Directory Not Found : " + imagepathimerTemp + "\r\n" + "DirectoryExists: " + Directory.Exists(imagepathimerTemp) + "\r\n" + "imagepathimer: " + imagepathimer);
                //    imagepathimerTemp = imagepathimer;
                //}

                imagepathimer = imagepathimerTemp;
                _Logger.Info("Start Impersonate:");

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    if (string.IsNullOrEmpty(model.Register_device))
                    {
                        model.ListImageFileBulk
                                = ConvertHttpPostedFileBaseToUploadImage(files, model, imagepathimer);

                        if (!model.ListImageFileBulk.Any())
                        {
                            var base64photoDict = Session["base64photo"] as Dictionary<string, string>;
                            model.ListImageFileBulk
                                = ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer);
                        }
                    }
                    else if (model.Register_device == "MOBILE APP")
                    {
                        var base64photoDict = Session["base64photo"] as Dictionary<string, string>;
                        model.ListImageFileBulk
                            = ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer);
                    }
                    else
                    {
                        model.ListImageFileBulk
                            = ConvertHttpPostedFileBaseToUploadImage(files, model, imagepathimer);
                    }

                    _Logger.Info("End Impersonate:");
                    Session["base64photo"] = null;

                    EndInterface("", log, transactionId, "Success", "");

                    return model;

                }
            }
            catch (Exception ex)
            {
                EndInterface("", log, transactionId, "ERROR", ex.GetErrorMessage());

                _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return model;
            }
        }

        private List<UploadImageBulk> ConvertBase64PhotoToUploadImage(
            Dictionary<string, string> base64photoDict,
            RegisterBulkCorpModel model,
            string imagepathimer)
        {
            if (null == base64photoDict)
            {
                _Logger.Info("base64photo not contain valid files");
                return new List<UploadImageBulk>();
            }

            foreach (var item in base64photoDict)
            {
                var uploadImageBulk = new UploadImageBulk();
                uploadImageBulk.FileNameBulk = item.Key + Guid.NewGuid().ToString() + ".jpg";
                model.ListImageFileBulk.Add(uploadImageBulk);
            }

            var path = "";
            var resultFormat = GetFormatFile(model);
            var tempfile = new List<UploadImageBulk>();

            var fileIndex = 0;
            foreach (var item in base64photoDict)
            {
                path = Path.Combine(imagepathimer, resultFormat[fileIndex].file_name_bulk);
                var uploadImageWithGeneratedName = new UploadImageBulk();
                uploadImageWithGeneratedName.FileNameBulk = path;
                tempfile.Add(uploadImageWithGeneratedName);

                var imgBytes = Convert.FromBase64String(base64photoDict[item.Key]);
                System.IO.File.WriteAllBytes(path, imgBytes);
                fileIndex++;
            }

            return tempfile;
        }

        private UploadExcelBulk ConvertHttpPostedFileBaseToUploadExcel(
            HttpPostedFileBase[] files,
            RegisterBulkCorpInstallModel model,
            string imagepathimer, DataTable dt)
        {
            string cardNo = model.id_card_no;
            model.ExcelBulk = new UploadExcelBulk();

            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertHttpPostedFileBaseToUploadExcel", model.ca_main_mobile + model.ClientIP, cardNo, "WEB");
            try
            {

                if (files.Count() <= 0)
                {
                    _Logger.Info("upload excel file not contain valid excel files");

                    return new UploadExcelBulk();
                }

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            var varfileName = Path.GetFileName(files[i].FileName);
                            var p = new UploadExcelBulk();
                            p.FileExcelBulk = varfileName;
                            model.ExcelBulk = p;
                        }
                    }
                }

                var resultFormat = GetFormatExcelFile(model);
                var tempfile = new UploadExcelBulk();
                var j = 0;

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        var path = Path.Combine(imagepathimer, resultFormat[j].file_name_bulk);
                        var type = resultFormat[j].file_name_bulk.Substring(resultFormat[j].file_name_bulk.IndexOf(".") + 1).ToLower();
                        var p2 = new UploadExcelBulk();


                        p2.FileExcelBulk = path;
                        tempfile = p2;
                        /////////////////////////////////////////////////////////////////////////////////////////

                        //files[i].SaveAs(path);
                        FileInfo excelInfo = new FileInfo(path);
                        using (ExcelPackage pck = new ExcelPackage(excelInfo))
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("RegisterBulk");
                            ws.Cells["A1"].LoadFromDataTable(dt, true);
                            pck.Save();
                        }

                        j++;
                    }
                }
                EndInterface("", log, model.ca_main_mobile + model.ClientIP, "Success", "");
                return tempfile;
            }
            catch (Exception ex)
            {
                EndInterface("", log, model.ca_main_mobile + model.ClientIP, "ERROR", ex.GetErrorMessage());

                _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new UploadExcelBulk();
            }


        }

        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Bitmap image, int width, int height, ResizeOptions resizeOptions)
        {
            float f_width;
            float f_height;
            float dim;
            switch (resizeOptions)
            {
                case ResizeOptions.ExactWidthAndHeight:
                    return DoResize(image, width, height);

                case ResizeOptions.MaxHeight:
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_height <= height)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;
                    width = (int)((float)(height) * dim);
                    return DoResize(image, width, height);

                case ResizeOptions.MaxWidth:
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_width <= width)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;
                    height = (int)((float)(width) / dim);
                    return DoResize(image, width, height);

                case ResizeOptions.MaxWidthAndHeight:
                    int tmpHeight = height;
                    int tmpWidth = width;
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_width <= width && f_height <= height)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;

                    if (f_width < width)
                        width = (int)f_width;
                    height = (int)((float)(width) / dim);

                    if (height > tmpHeight)
                    {
                        if (f_height < tmpHeight)
                            height = (int)f_height;
                        else
                            height = tmpHeight;
                        width = (int)((float)(height) * dim);
                    }
                    return DoResize(image, width, height);

                default:
                    return image;
            }
        }

        public static System.Drawing.Bitmap DoResize(System.Drawing.Bitmap originalImg, int widthInPixels, int heightInPixels)
        {
            System.Drawing.Bitmap bitmap;
            try
            {
                bitmap = new System.Drawing.Bitmap(widthInPixels, heightInPixels);
                using (System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(bitmap))
                {
                    graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    graphic.DrawImage(originalImg, 0, 0, widthInPixels, heightInPixels);
                    return bitmap;
                }
            }
            finally
            {
                if (originalImg != null)
                {
                    originalImg.Dispose();
                }
            }
        }

        private List<FileFormatModelBulk> GetFormatFile(RegisterBulkCorpModel model)
        {
            var lang = (SiteSession.CurrentUICulture.IsThaiCulture() ? "THAI" : "ENG");

            string lovdatatext = base.LovData.Where(l => l.Type == "ID_CARD_TYPE" && l.Name == model.id_card_type)
                .Select(l => l.Text).FirstOrDefault();

            string resultstr = string.Empty;
            List<FileFormatModelBulk> result = new List<FileFormatModelBulk>();

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmssffffff");
            string dateNow = currDateTime.ToString("yyyyMMdd");
            string idcardstr = model.id_card_no.Substring(model.id_card_no.Length - 4);
            string fileformatt = ".jpg";

            for (var ii = 0; ii < 4; ii++)
            {
                var iistr = ii.ToString("D2");
                resultstr = string.Format("FBB_{0}_{1}_{2}{3}{4}{5}", lovdatatext, idcardstr, dateNow, timeNow, iistr, fileformatt);
                FileFormatModelBulk resulttemp = new FileFormatModelBulk();
                resulttemp.file_name_bulk = resultstr;
                result.Add(resulttemp);
            }

            return result;
        }

        private List<FileFormatModelBulk> GetFormatExcelFile(RegisterBulkCorpInstallModel model)
        {
            var lang = (SiteSession.CurrentUICulture.IsThaiCulture() ? "THAI" : "ENG");

            string lovdatatext = base.LovData.Where(l => l.Type == "ID_CARD_TYPE" && l.Name == model.id_card_type)
                .Select(l => l.Text).FirstOrDefault();

            string resultstr = string.Empty;
            List<FileFormatModelBulk> result = new List<FileFormatModelBulk>();

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmssffffff");
            string dateNow = currDateTime.ToString("yyyyMMdd");
            string idcardstr = model.id_card_no.Substring(model.id_card_no.Length - 4);
            string fileformatt = ".xlsx";


            var iistr = "01";
            resultstr = string.Format("FBBExcel_{0}_{1}_{2}{3}{4}{5}", lovdatatext, idcardstr, dateNow, timeNow, iistr, fileformatt);
            FileFormatModelBulk resulttemp = new FileFormatModelBulk();
            resulttemp.file_name_bulk = resultstr;
            result.Add(resulttemp);


            return result;
        }

        public ActionResult Remove_list_photo(string type)
        {
            var fileList = Session["base64photo"] as Dictionary<string, string>;
            if (fileList != null)
            {
                if (type == "R")
                {
                    List<File_Remove> File_Remove_R = new List<File_Remove>()
                {
                  new File_Remove {file_name="fileG1TakePhoto"},
                  new File_Remove {file_name="fileG2TakePhoto"},
                  new File_Remove {file_name="fileG3TakePhoto"},
                  new File_Remove {file_name="fileB1TakePhoto"},
                  new File_Remove {file_name="fileB2TakePhoto"},
                  new File_Remove {file_name="fileB3TakePhoto"},
                  new File_Remove {file_name="fileB4TakePhoto"}
                };

                    foreach (var item_file in File_Remove_R)
                    {
                        foreach (var item in fileList.Where(fl => fl.Key == item_file.file_name).ToList())
                        {
                            fileList.Remove(item.Key);
                        }
                    }
                }
                else if (type == "G")
                {
                    List<File_Remove> File_Remove_G = new List<File_Remove>()
                {
                  new File_Remove {file_name="fileTakePhoto"},
                  new File_Remove {file_name="fileBillTakePhoto"},
                  new File_Remove {file_name="fileB1TakePhoto"},
                  new File_Remove {file_name="fileB2TakePhoto"},
                  new File_Remove {file_name="fileB3TakePhoto"},
                  new File_Remove {file_name="fileB4TakePhoto"}
                };

                    foreach (var item_file in File_Remove_G)
                    {
                        foreach (var item in fileList.Where(fl => fl.Key == item_file.file_name).ToList())
                        {
                            fileList.Remove(item.Key);
                        }
                    }
                }
                else if (type == "B")
                {
                    List<File_Remove> File_Remove_B = new List<File_Remove>()
                {
                  new File_Remove {file_name="fileTakePhoto"},
                  new File_Remove {file_name="fileBillTakePhoto"},
                  new File_Remove {file_name="fileG1TakePhoto"},
                  new File_Remove {file_name="fileG2TakePhoto"},
                  new File_Remove {file_name="fileG3TakePhoto"}
                };

                    foreach (var item_file in File_Remove_B)
                    {
                        foreach (var item in fileList.Where(fl => fl.Key == item_file.file_name).ToList())
                        {
                            fileList.Remove(item.Key);
                        }
                    }
                }
                else
                {
                    //to do
                }
            }

            return Json(new { result = true, }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemovePhoto(string control_id)
        {
            if (null != Session["base64photo"])
            {
                var fileList = Session["base64photo"] as Dictionary<string, string>;

                foreach (var item in fileList.Where(fl => fl.Key == control_id).ToList())
                {
                    fileList.Remove(item.Key);
                }
            }

            return Json(new { result = true, }, JsonRequestBehavior.AllowGet);
        }

        public class File_Remove
        {
            public string file_name { get; set; }
        }

        #endregion Upload Image Files

        #region Build Data Save

        public JsonResult SaveRegistrationInfo(string p_user = "", string asc_code = "", string employee_id = "", string location_code = "",
                                                string id_card_no = "", string account_category = "", string account_sub_category = "",
                                                string id_card_type = "", string account_title = "", string account_name = "", string ca_house_no = "",
                                                string ca_moo = "", string ca_mooban = "", string ca_building_name = "", string ca_floor = "",
                                                string ca_room = "", string ca_soi = "", string ca_street = "", string ca_sub_district = "",
                                                string ca_district = "", string ca_province = "", string ca_postcode = "", string ca_phone = "",
                                                string ca_main_mobile = "", string ba_language = "", string ba_bill_name = "", string ba_bill_cycle = "",
                                                string ba_house_no = "", string ba_moo = "", string ba_mooban = "", string ba_building_name = "",
                                                string ba_floor = "", string ba_room = "", string ba_soi = "", string ba_street = "", string ba_sub_district = "",
                                                string ba_district = "", string ba_province = "", string ba_postcode = "", string ba_phone = "",
                                                string ba_main_mobile = "")
        {

            if (null == base.CurrentUser)
                return Json("Sesson Time Out", JsonRequestBehavior.AllowGet);
            var LoginUser = base.CurrentUser;

            try
            {
                Session["Bulk_Number"] = "";
                if (null != location_code)
                {
                    if (null == asc_code || "null" == asc_code)
                    {
                        asc_code = "";
                    }
                    var query = new RegisterBulkCorpQuery()
                    {
                        p_user = LoginUser.UserName,
                        p_asc_code = asc_code,
                        p_employee_id = employee_id.ToSafeString(),
                        p_location_code = location_code.ToSafeString(),
                        p_id_card_no = id_card_no.ToSafeString(),
                        p_account_category = account_category.ToSafeString(),
                        p_account_sub_category = account_sub_category.ToSafeString(),
                        p_id_card_type = id_card_type.ToSafeString(),
                        p_account_title = account_title.ToSafeString(),
                        p_account_name = account_name.ToSafeString(),
                        p_ca_house_no = ca_house_no.ToSafeString(),
                        p_ca_moo = ca_moo.ToSafeString(),
                        p_ca_mooban = ca_mooban.ToSafeString(),
                        p_ca_building_name = ca_building_name.ToSafeString(),
                        p_ca_floor = ca_floor.ToSafeString(),
                        p_ca_room = ca_room.ToSafeString(),
                        p_ca_soi = ca_soi.ToSafeString(),
                        p_ca_street = ca_street.ToSafeString(),
                        p_ca_sub_district = ca_sub_district.ToSafeString(),
                        p_ca_district = ca_district.ToSafeString(),
                        p_ca_province = ca_province.ToSafeString(),
                        p_ca_postcode = ca_postcode.ToSafeString(),
                        p_ca_phone = ca_phone.ToSafeString(),
                        p_ca_main_mobile = ca_main_mobile.ToSafeString(),
                        p_ba_language = ba_language.ToSafeString(),
                        p_ba_bill_name = ba_bill_name.ToSafeString(),
                        p_ba_bill_cycle = ba_bill_cycle.ToSafeString(),
                        p_ba_house_no = ba_house_no.ToSafeString(),
                        p_ba_moo = ba_moo.ToSafeString(),
                        p_ba_mooban = ba_mooban.ToSafeString(),
                        p_ba_building_name = ba_building_name.ToSafeString(),
                        p_ba_floor = ba_floor.ToSafeString(),
                        p_ba_room = ba_room.ToSafeString(),
                        p_ba_soi = ba_soi.ToSafeString(),
                        p_ba_street = ba_street.ToSafeString(),
                        p_ba_sub_district = ba_sub_district.ToSafeString(),
                        p_ba_district = ba_district.ToSafeString(),
                        p_ba_province = ba_province.ToSafeString(),
                        p_ba_postcode = ba_postcode.ToSafeString(),
                        p_ba_phone = ba_phone.ToSafeString(),
                        p_ba_main_mobile = ba_main_mobile.ToSafeString()

                    };
                    var data = _queryProcessor.Execute(query);

                    if (data.output_bulk_number != "" || null != data.output_bulk_number)
                    {
                        Session["Bulk_number"] = "";
                        Session["Account_cat"] = "";
                        _Logger.Info("Start Add Session Bulk Number");
                        Session["Bulk_number"] = data.output_bulk_number;
                        Session["Account_cat"] = account_category;

                        _Logger.Info("Register Bulk Corp New Account Data: " + data.output_bulk_number + "\r\n" +
                                       "p_user : " + p_user + "\r\n" +
                                       "p_asc_code : " + asc_code + "\r\n" +
                                       "p_employee_id : " + employee_id + "\r\n" +
                                       "p_location_code : " + location_code + "\r\n" +
                                       "p_id_card_no : " + id_card_no + "\r\n" +
                                       "p_account_category : " + account_category + "\r\n" +
                                       "p_account_sub_category : " + account_sub_category + "\r\n" +
                                       "p_id_card_type : " + id_card_type + "\r\n" +
                                       "p_account_title : " + account_title + "\r\n" +
                                       "p_account_name : " + account_name + "\r\n" +
                                       "p_ca_house_no : " + ca_house_no + "\r\n" +
                                       "p_ca_moo : " + ca_moo + "\r\n" +
                                       "p_ca_mooban : " + ca_mooban + "\r\n" +
                                       "p_ca_building_name : " + ca_building_name + "\r\n" +
                                       "p_ca_floor : " + ca_floor + "\r\n" +
                                       "p_ca_room : " + ca_room + "\r\n" +
                                       "p_ca_soi : " + ca_soi + "\r\n" +
                                       "p_ca_street : " + ca_street + "\r\n" +
                                       "p_ca_sub_district : " + ca_sub_district + "\r\n" +
                                       "p_ca_district : " + ca_district + "\r\n" +
                                       "p_ca_province : " + ca_province + "\r\n" +
                                       "p_ca_postcode : " + ca_postcode + "\r\n" +
                                       "p_ca_phone : " + ca_phone + "\r\n" +
                                       "p_ca_main_mobile : " + ca_main_mobile + "\r\n" +
                                       "p_ba_language : " + ba_language + "\r\n" +
                                       "p_ba_bill_name : " + ba_bill_name + "\r\n" +
                                       "p_ba_bill_cycle : " + ba_bill_cycle + "\r\n" +
                                       "p_ba_house_no : " + ba_house_no + "\r\n" +
                                       "p_ba_moo : " + ba_moo + "\r\n" +
                                       "p_ba_mooban : " + ba_mooban + "\r\n" +
                                       "p_ba_building_name : " + ba_building_name + "\r\n" +
                                       "p_ba_floor : " + ba_floor + "\r\n" +
                                       "p_ba_room : " + ba_room + "\r\n" +
                                       "p_ba_soi : " + ba_soi + "\r\n" +
                                       "p_ba_street : " + ba_street + "\r\n" +
                                       "p_ba_sub_district : " + ba_sub_district + "\r\n" +
                                       "p_ba_district : " + ba_district + "\r\n" +
                                       "p_ba_province : " + ba_province + "\r\n" +
                                       "p_ba_postcode : " + ba_postcode + "\r\n" +
                                       "p_ba_phone : " + ba_phone + "\r\n" +
                                       "p_ba_main_mobile : " + ba_main_mobile + "\r\n");


                        _Logger.Info("End Add Session Bulk Number");

                        //return Json(new { result = Session["Bulk_number"], }, JsonRequestBehavior.AllowGet);
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        _Logger.Info("Error SaveRegistrationInfo do not return the proper data");
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    _Logger.Info("Error SaveRegistrationInfo location code was lost.");
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exc)
            {
                _Logger.Info("Error SaveRegistrationInfo " + exc.Message);
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult OnSubmitRegisterBulkCorp(string Bulk_No)
        {

            BatchBulkCorpModel modelwf = new BatchBulkCorpModel();
            BatchBulkCorpSFFModel modelsff = new BatchBulkCorpSFFModel();
            var ret_codecom = "";
            var ret_msgcom = "";


            //Workflow
            try
            {
                _Logger.Info("Start GetScreenBulkCorpQuery");
                var querysnd = new GetScreenBulkCorpQuery()
                {
                    P_BULK_NUMBER = Bulk_No
                };

                var dataret = _queryProcessor.Execute(querysnd);
                _Logger.Info("End GetScreenBulkCorpQuery");
                ret_codecom = dataret.OUTPUT_RETURN_CODE;
                ret_msgcom = dataret.OUTPUT_RETURN_MESSAGE;
                _Logger.Info("GetScreenBulk Workflow " + dataret.OUTPUT_RETURN_CODE + " " + dataret.OUTPUT_RETURN_MESSAGE);
                var datascrwf = SendToCompleteWorkflow(Bulk_No, ret_codecom, ret_msgcom);

                if (null != dataret && dataret.OUTPUT_RETURN_CODE != "-1")
                {
                    modelwf = dataret;
                    _Logger.Info("EXPECT_INSTALL_DATE: " + modelwf.P_CALL_WORKFLOW[0].EXPECT_INSTALL_DATE);
                    try
                    {
                        var querysndsff = new GetScreenBulkCorpSFFQuery()
                        {
                            P_BULK_NUMBER = Bulk_No
                        };

                        var dataretsff = _queryProcessor.Execute(querysndsff);

                        ret_codecom = dataretsff.OUTPUT_return_code;
                        ret_msgcom = dataretsff.OUTPUT_return_message;
                        _Logger.Info("GetScreenBulk SFF " + dataretsff.OUTPUT_return_code + " " + dataretsff.OUTPUT_return_message);
                        var datascrsff = SendToCompleteSFF(Bulk_No, ret_codecom, ret_msgcom);

                        if (null != dataretsff && dataret.OUTPUT_RETURN_CODE != "-1")
                        {
                            modelsff = dataretsff;
                            var querywf = new GetSaveBulkCorpOrderNewQuery()
                            {
                                GetBulkCorpRegister = modelwf
                            };

                            var retwfbatch = _queryProcessor.Execute(querywf);

                            if (null != retwfbatch && retwfbatch.RETURN_CODE.ToSafeString() != "-1")
                            {
                                ret_codecom = retwfbatch.RETURN_CODE.ToString();
                                ret_msgcom = retwfbatch.RETURN_MESSAGE;
                                _Logger.Info("SaveOrderNew Workflow " + retwfbatch.RETURN_CODE + " " + retwfbatch.RETURN_MESSAGE);
                                var datacomplwf = SendToCompleteWorkflow(Bulk_No, ret_codecom, ret_msgcom);

                                if (null != datacomplwf && datacomplwf.output_return_code != "-1")
                                {
                                    try
                                    {
                                        var querysff = new evOMNewRegisMultiInstanceQuery()
                                        {

                                            referenceNo = modelsff.P_CALL_SFF.First().p_referenceNo,
                                            accountCat = modelsff.P_CALL_SFF.First().P_accountCat,
                                            accountSubCat = modelsff.P_CALL_SFF.First().P_accountSubCat,
                                            idCardType = modelsff.P_CALL_SFF.First().P_idCardType,
                                            idCardNo = modelsff.P_CALL_SFF.First().P_idCardNo,
                                            titleName = modelsff.P_CALL_SFF.First().P_titleName,
                                            firstName = modelsff.P_CALL_SFF.First().P_firstName,
                                            lastName = modelsff.P_CALL_SFF.First().P_lastName,
                                            saName = modelsff.P_CALL_SFF.First().P_saName,
                                            baName = modelsff.P_CALL_SFF.First().P_baName,
                                            caNumber = modelsff.P_CALL_SFF.First().P_caNumber,
                                            baNumber = modelsff.P_CALL_SFF.First().P_baNumber,
                                            saNumber = modelsff.P_CALL_SFF.First().P_saNumber,
                                            birthdate = modelsff.P_CALL_SFF.First().P_birthdate,
                                            gender = modelsff.P_CALL_SFF.First().P_gender,
                                            billName = modelsff.P_CALL_SFF.First().P_billName,
                                            billCycle = modelsff.P_CALL_SFF.First().P_billCycle,
                                            billLanguage = modelsff.P_CALL_SFF.First().P_billLanguage,
                                            engFlag = modelsff.P_CALL_SFF.First().P_engFlag,
                                            accHomeNo = modelsff.P_CALL_SFF.First().P_accHomeNo,
                                            accBuildingName = modelsff.P_CALL_SFF.First().P_accBuildingName,
                                            accFloor = modelsff.P_CALL_SFF.First().P_accFloor,
                                            accRoom = modelsff.P_CALL_SFF.First().P_accRoom,
                                            accMoo = modelsff.P_CALL_SFF.First().P_accMoo,
                                            accMooBan = modelsff.P_CALL_SFF.First().P_accMooBan,
                                            accSoi = modelsff.P_CALL_SFF.First().P_accSoi,
                                            accStreet = modelsff.P_CALL_SFF.First().P_accStreet,
                                            accTumbol = modelsff.P_CALL_SFF.First().P_accTumbol,
                                            accAmphur = modelsff.P_CALL_SFF.First().P_accAmphur,
                                            accProvince = modelsff.P_CALL_SFF.First().P_accProvince,
                                            accZipCode = modelsff.P_CALL_SFF.First().P_accZipCode,
                                            billHomeNo = modelsff.P_CALL_SFF.First().P_billHomeNo,
                                            billBuildingName = modelsff.P_CALL_SFF.First().P_billBuildingName,
                                            billFloor = modelsff.P_CALL_SFF.First().P_billFloor,
                                            billRoom = modelsff.P_CALL_SFF.First().P_billRoom,
                                            billMoo = modelsff.P_CALL_SFF.First().P_billMoo,
                                            billMooBan = modelsff.P_CALL_SFF.First().P_billMooBan,
                                            billSoi = modelsff.P_CALL_SFF.First().P_billSoi,
                                            billStreet = modelsff.P_CALL_SFF.First().P_billStreet,
                                            billTumbol = modelsff.P_CALL_SFF.First().P_billTumbol,
                                            billAmphur = modelsff.P_CALL_SFF.First().P_billAmphur,
                                            billProvince = modelsff.P_CALL_SFF.First().P_billProvince,
                                            billZipCode = modelsff.P_CALL_SFF.First().P_billZipCode,
                                            userId = modelsff.P_CALL_SFF.First().P_userId,
                                            dealerLocationCode = modelsff.P_CALL_SFF.First().P_dealerLocationCode,
                                            ascCode = modelsff.P_CALL_SFF.First().P_ascCode,
                                            orderReason = modelsff.P_CALL_SFF.First().P_orderReason,
                                            remark = modelsff.P_CALL_SFF.First().P_remark,
                                            saVatName = modelsff.P_CALL_SFF.First().P_saVatName,
                                            saVatAddress1 = modelsff.P_CALL_SFF.First().P_saVatAddress1,
                                            saVatAddress2 = modelsff.P_CALL_SFF.First().P_saVatAddress2,
                                            saVatAddress3 = modelsff.P_CALL_SFF.First().P_saVatAddress3,
                                            saVatAddress4 = modelsff.P_CALL_SFF.First().P_saVatAddress4,
                                            saVatAddress5 = modelsff.P_CALL_SFF.First().P_saVatAddress5,
                                            saVatAddress6 = modelsff.P_CALL_SFF.First().P_saVatAddress6,
                                            contactFirstName = modelsff.P_CALL_SFF.First().P_contactFirstName,
                                            contactLastName = modelsff.P_CALL_SFF.First().P_contactLastName,
                                            contactTitle = modelsff.P_CALL_SFF.First().P_contactTitle,
                                            mobileNumberContact = modelsff.P_CALL_SFF.First().P_mobileNumberContact,
                                            phoneNumberContact = modelsff.P_CALL_SFF.First().P_phoneNumberContact,
                                            emailAddress = modelsff.P_CALL_SFF.First().P_emailAddress,
                                            saHomeNo = modelsff.P_CALL_SFF.First().P_saHomeNo,
                                            saBuildingName = modelsff.P_CALL_SFF.First().P_saBuildingName,
                                            saFloor = modelsff.P_CALL_SFF.First().P_saFloor,
                                            saRoom = modelsff.P_CALL_SFF.First().P_saRoom,
                                            saMoo = modelsff.P_CALL_SFF.First().P_saMoo,
                                            saMooBan = modelsff.P_CALL_SFF.First().P_saMooBan,
                                            saSoi = modelsff.P_CALL_SFF.First().P_saSoi,
                                            saStreet = modelsff.P_CALL_SFF.First().P_saStreet,
                                            saTumbol = modelsff.P_CALL_SFF.First().P_saTumbol,
                                            saAmphur = modelsff.P_CALL_SFF.First().P_saAmphur,
                                            saProvince = modelsff.P_CALL_SFF.First().P_saProvince,
                                            saZipCode = modelsff.P_CALL_SFF.First().P_saZipCode,
                                            orderType = modelsff.P_CALL_SFF.First().P_orderType,
                                            channel = modelsff.P_CALL_SFF.First().P_channel,
                                            projectName = modelsff.P_CALL_SFF.First().P_projectName,
                                            caBranchNo = modelsff.P_CALL_SFF.First().P_caBranchNo,
                                            saBranchNo = modelsff.P_CALL_SFF.First().P_saBranchNo,
                                            chargeType = modelsff.P_CALL_SFF.First().P_chargeType,
                                            sourceSystem = modelsff.P_CALL_SFF.First().P_sourceSystem,
                                            subcontractor = modelsff.P_CALL_SFF.First().P_subcontractor,
                                            installStaffID = modelsff.P_CALL_SFF.First().P_installStaffID,
                                            employeeID = modelsff.P_CALL_SFF.First().P_employeeID,
                                            billMedia = modelsff.P_CALL_SFF.First().P_billmedia,


                                            productInstance = modelsff.P_LIST_INSTANCE_CUR.First().p_productInstance,
                                            mobileNo = modelsff.P_LIST_INSTANCE_CUR.First().p_mobileNo,
                                            simSerialNo = modelsff.P_LIST_INSTANCE_CUR.First().p_simSerialNo,
                                            provinceCode = modelsff.P_LIST_INSTANCE_CUR.First().p_provinceCode,

                                            accessType = modelsff.P_LIST_SERVICE_VDSL_ROUTER.First().p_accessType,
                                            brand = modelsff.P_LIST_SERVICE_VDSL_ROUTER.First().p_brand,
                                            macAddress = modelsff.P_LIST_SERVICE_VDSL_ROUTER.First().p_macAddress,
                                            meterialCode = modelsff.P_LIST_SERVICE_VDSL_ROUTER.First().p_meterialCode,
                                            model = modelsff.P_LIST_SERVICE_VDSL_ROUTER.First().p_model,
                                            serialNo = modelsff.P_LIST_SERVICE_VDSL_ROUTER.First().p_serialNo,
                                            subContractor = modelsff.P_LIST_SERVICE_VDSL_ROUTER.First().p_subContractor,

                                            promotionMain = modelsff.P_SFF_PROMOTION_CUR.First().p_sff_product_cd,
                                            promotionOntop = modelsff.P_SFF_PROMOTION_ONTOP_CUR.First().p_sff_product_cd,

                                            bulkvdsl = modelsff.P_LIST_SERVICE_VDSL,
                                            bulkvdslrouter = modelsff.P_LIST_SERVICE_VDSL_ROUTER,
                                            bulkappoint = modelsff.P_LIST_SERVICE_APPOINT,
                                            bulksffpromotioncur = modelsff.P_SFF_PROMOTION_CUR,
                                            bulksffpromotionontopcur = modelsff.P_SFF_PROMOTION_ONTOP_CUR,
                                            bulklistinstancecur = modelsff.P_LIST_INSTANCE_CUR

                                        };

                                        var retsffbatch = _queryProcessor.Execute(querysff);

                                        if (null != retsffbatch && retsffbatch.ReturnCode != "-1")
                                        {
                                            ret_codecom = retsffbatch.ReturnCode;
                                            ret_msgcom = retsffbatch.ReturnMessage;
                                            _Logger.Info("Batch SFF " + retsffbatch.ReturnCode + " " + retsffbatch.ReturnMessage);
                                            var databatchsff = SendToCompleteSFF(Bulk_No, ret_codecom, ret_msgcom);

                                            if (null != databatchsff.output_bulk_no)
                                            {
                                                _Logger.Info("Register Success!");
                                                //var modelResponse = new { status = true, message ="FBB BULK: "+ Bulk_No, filename = Bulk_No };
                                                return Json(new { Result = "Success" }, JsonRequestBehavior.AllowGet);
                                            }

                                            else
                                            {
                                                _Logger.Info("Register Failed! complete SFF");
                                                return Json(new { Result = "Register Failed! complete SFF" }, JsonRequestBehavior.AllowGet);
                                            }
                                        }// chk batch sff
                                        else
                                        {
                                            //SendToCompleteSFF(Bulk_No, "Fail", "evOMNewRegisMultiInstanceQuery ReturnCode = -1");
                                            _Logger.Info("Register Failed! Batch SFF");
                                            return Json(new { Result = "Register Failed! Batch SFF" }, JsonRequestBehavior.AllowGet);
                                        }// chk batch sff
                                    }
                                    catch (Exception ex4)
                                    {
                                        //SendToCompleteSFF(Bulk_No, "Fail", "evOMNewRegisMultiInstanceQuery exception");
                                        _Logger.Info("Error when call evOMNewRegisMultiInstanceQuery " + ex4.GetErrorMessage());
                                        //var modelResponse = new { status = false, message = "Register Failed! Workflow", filename = "" };
                                        return Json(new { Result = "Register Failed! SFF" }, JsonRequestBehavior.AllowGet);
                                    }// chk call service evOMNewRegisMultiInstanceQuery
                                }
                                else
                                {
                                    //SendToCompleteWorkflow(Bulk_No, "-1", "Fail");
                                    _Logger.Info("Register Failed! Workflow");
                                    //var modelResponse = new { status = false, message = "Register Failed!", filename = "" };
                                    return Json(new { Result = "Register Failed! Workflow" }, JsonRequestBehavior.AllowGet);
                                }// chk Workflow

                            }
                            else
                            {
                                SendToCompleteWorkflow(Bulk_No, "-1", "Fail");
                                _Logger.Info("Register Failed! Batch Workflow");
                                //var modelResponse = new { status = false, message = "Register Failed! Batch Workflow", filename = "" };
                                return Json(new { Result = "Register Failed! Batch Workflow" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else
                        {
                            SendToCompleteSFF(Bulk_No, "Fail", "GetScreenBulkCorpSFFQuery ");
                            _Logger.Info("Register Failed! SFF");
                            //var modelResponse = new { status = false, message = "Register Failed! SFF", filename = "" };
                            return Json(new { Result = "Register Failed! SFF" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex2)
                    {
                        SendToCompleteSFF(Bulk_No, "Fail", "Error Call GetScreenBulkCorpSFFQuery");
                        _Logger.Info("Error when call GetScreenBulkCorpSFFQuery " + ex2.GetErrorMessage());
                        // var modelResponse = new { status = false, message = "Register Failed! SFF", filename = "" };
                        return Json(new { Result = "Register Failed! SFF" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    SendToCompleteWorkflow(Bulk_No, "-1", "Fail");
                    _Logger.Info("Register Failed! Workflow");
                    //var modelResponse = new { status = false, message = "Register Failed! Workflow", filename = "" };
                    return Json(new { Result = "Register Failed! Workflow" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex1)
            {
                SendToCompleteWorkflow(Bulk_No, "-1", "Fail");
                _Logger.Info("Error when call GetScreenBulkCorpQuery " + ex1.GetErrorMessage());
                // var modelResponse = new { status = false, message = "Register Failed! Workflow", filename = "" };
                return Json(new { Result = "Register Failed! Workflow" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ReturnCompletewf SendToCompleteWorkflow(string Bulk_No, string ret_code, string ret_msg)
        {
            try
            {
                var querycompletewf = new RegisterBulkCompleteWorkflowQuery()
                {
                    p_bulk_number = Bulk_No,
                    p_return_message = ret_msg,
                    p_return_code = ret_code
                };

                var datacomplwf = _queryProcessor.Execute(querycompletewf);

                if (null != datacomplwf)
                {
                    return datacomplwf;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception Exc)
            {
                _Logger.Info("Error when call RegisterBulkCompleteWorkflowQuery " + Exc.GetErrorMessage());
                return null;
            }
        }

        public ReturnCompletesff SendToCompleteSFF(string Bulk_No, string ret_result, string ret_errorreason)
        {
            try
            {
                if (ret_result == "0" || ret_result == "Success")
                {
                    ret_result = "Success";
                }
                else
                {
                    ret_result = "Fail";
                }

                var querycompletesff = new RegisterBulkCompleteSFFQuery()
                {
                    p_bulk_number = Bulk_No,
                    p_result = ret_result,
                    p_errorreason = ret_errorreason
                };

                var datacomplsff = _queryProcessor.Execute(querycompletesff);

                if (null != datacomplsff)
                {
                    return datacomplsff;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception Exc)
            {
                _Logger.Info("Error when call RegisterBulkCompleteSFFQuery " + Exc.GetErrorMessage());
                return null;
            }
        }

        public JsonResult DeleteExcelRegis(string Bulk_No)
        {
            try
            {
                var querydel = new DeleteRegisterBulkCorpQuery()
                {
                    p_bulk_number = Bulk_No

                };

                var datadel = _queryProcessor.Execute(querydel);

                if (null != datadel)
                {
                    return Json(datadel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception Exc)
            {
                _Logger.Info("Error when call DeleteRegisterBulkCorpQuery " + Exc.GetErrorMessage());
                return null;
            }
        }

        #endregion


        struct MyObj
        {
            public string output1 { get; set; }
            public string output2 { get; set; }
            public string output3 { get; set; }
            public string output4 { get; set; }
            public string output5 { get; set; }
        }

        [HttpPost]
        public JsonResult CheckEmpCode(string empCode = "")
        {
            string resultQuery = "N";
            string URL = "http://emmobile.ais.co.th/api/fbb/profilefbbon90day";
            string X_Token = "N=fbbon90day";
            string urlParameters = "?Key1=";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-Token", X_Token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("Key1", empCode.PadLeft(8, '0'))
                    });


                var resultData = client.PostAsync(urlParameters, content).Result;
                if (resultData.IsSuccessStatusCode)
                {
                    string resultContent = resultData.Content.ReadAsStringAsync().Result;
                    var resultObj = JsonConvert.DeserializeObject<MyObj>(resultContent);
                    if (resultObj.output1 == "000")
                    {
                        resultQuery = "Y";
                    }
                }

            }
            catch (Exception ex)
            {
                resultQuery = "Y";
            }

            return Json(resultQuery, JsonRequestBehavior.AllowGet);
        }
    }
}
