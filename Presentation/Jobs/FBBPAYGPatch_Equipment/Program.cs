using FBBPAYGPatch_Equipment.CompositionRoot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBBusinessLayer.Extension;
using WBBContract;
using System.Runtime.Caching;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.PatchEquipment;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYGPatch_Equipment
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"FBBPAYGPatch_Equipment Start");
            using (var con = new Bootstrapper())
            {
                var jobs = con.GetInstance<FBBPAYGPatchEquipment>();
                //---------step 1  ตรวจสอบ Flag การทำงานของ Batch  DISPLAY_VAL = Y----------
                var process = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "PROGRAM_PROCESS").FirstOrDefault();
                if (process != null)
                {
                    if (process.DISPLAY_VAL == "Y")
                    {
                        //-----------step 2 Query ข้อมูล date_start/date_to/datediv ------------------ 				

                        var date_start = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "DATE_START").FirstOrDefault();
                        var date_to = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "DATE_TO").FirstOrDefault();
                        var date_div = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "DATE_DIV").FirstOrDefault();

                        var Check_DateDiv = date_div.DISPLAY_VAL.ToSafeString() == "Y" ? date_div.VAL1.ToSafeInteger() + 1 : 1;

                        var getData = new PatchEquipmentQuery()
                        {
                            CreateDateFrom = date_start.DISPLAY_VAL.ToSafeString() == "Y" ? date_start.VAL1.Split(' ').First().ToSafeString() : DateTime.Now.AddDays(-Check_DateDiv).ToString("ddMMyyyy"),
                            CreateDateTo = date_to.DISPLAY_VAL.ToSafeString() == "Y" ? date_to.VAL1.Split(' ').First().ToSafeString() : DateTime.Now.ToString("ddMMyyyy"),
                            PatchStatus = "PENDING,PENDING_NEWREGIST", //PENDING,PENDING_NEWREGIST
                            FileName = "ALL",
                            SerialNo = "ALL",
                            InternetNo = "ALL",
                            Flag = "ALL"
                        };

                        //-------------step 3 ระบบจะข้อมูลที่ Query จาก Package -----------------------
                        var result = jobs.getdataPatchEquipment(getData);

                        //-------------step 4 ดำเนินการ Update Config date --------------------------
                        var queryUpdateDateY = new UpdateFbssPatchDataConfigTblCommand()
                        {
                            display_val = "Y",
                            val1 = getData.CreateDateFrom,
                            val2 = getData.CreateDateTo,
                            updated_by = "FBBPAYGPatch_EquipmentBatch"
                        };

                        jobs.TblUpdateData(queryUpdateDateY);

                        if (result.Count() > 0)
                        {
                            List<ResultData> resultDatas = new List<ResultData>();
                            List<RetPatchEquipment> ResultDataList = new List<RetPatchEquipment>();
                            //-------------step 5 โดยตรวจสอบสถานะ Status Serial จาก shareplex --------------------------
                            var flag = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "PROCESS_SNSTATUS").FirstOrDefault();
                            if (flag.DISPLAY_VAL == "Y")  //ถ้าเป็น Y ให้ไป call database SharePlex
                            {
                                var FixSize = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "LOOP_QUERY_SPLX").FirstOrDefault();
                                var total = result.Select(p => p.SERIAL_NO).Count();
                                var Size = FixSize != null ? int.Parse(FixSize.VAL1) : 1;
                                var page = 0;
                                var skip = Size * (page);
                                //---------------

                                do
                                {
                                    page++;
                                    var resultSkip = result.Where(s => s.PATCH_STATUS == "PENDING").Select(p => new
                                    {
                                        p.SERIAL_NO,
                                        p.INTERNET_NO,
                                        p.FOA_CODE,
                                        p.LOCATION_CODE,
                                        p._POST_DATE,
                                        p.MOVEMENT_TYPE,
                                        p._CREATE_DATE,
                                        p.SERIAL_STATUS
                                    }).Skip(skip).Take(Size).ToArray();

                                    List<RetCheckSerialStatus> data = new List<RetCheckSerialStatus>();
                                    var process_hvr = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();

                                    if (process_hvr.DISPLAY_VAL == "Y")
                                    {
                                        var chkserialstatusHVR = new CheckSerialStatusHVRQuery();
                                        chkserialstatusHVR.checkSerials = (from item in resultSkip
                                                                        select new CheckSerialStatus()
                                                                        {
                                                                            SERIAL_NUMBER = item.SERIAL_NO,
                                                                            INTERNET_NO = item.INTERNET_NO,
                                                                            FOA_CODE = item.FOA_CODE,
                                                                            LOCATION_CODE = item.LOCATION_CODE,
                                                                            SERIAL_STATUS = item.SERIAL_STATUS,
                                                                            POST_DATE = item._POST_DATE,
                                                                            MOVEMENT_TYPE = item.MOVEMENT_TYPE,
                                                                            CREATE_DATE = item._CREATE_DATE
                                                                        }).ToList();
                                        data = jobs.CheckSerialStatusHVR(chkserialstatusHVR);
                                    }
                                    else
                                    {
                                        var chkserialstatus = new CheckSerialStatusQuery();
                                        chkserialstatus.checkSerials = (from item in resultSkip
                                                                        select new CheckSerialStatus()
                                                                        {
                                                                            SERIAL_NUMBER = item.SERIAL_NO,
                                                                            INTERNET_NO = item.INTERNET_NO,
                                                                            FOA_CODE = item.FOA_CODE,
                                                                            LOCATION_CODE = item.LOCATION_CODE,
                                                                            SERIAL_STATUS = item.SERIAL_STATUS,
                                                                            POST_DATE = item._POST_DATE,
                                                                            MOVEMENT_TYPE = item.MOVEMENT_TYPE,
                                                                            CREATE_DATE = item._CREATE_DATE
                                                                        }).ToList();
                                        data = jobs.CheckSerialStatus(chkserialstatus);

                                    }

                                    var datalist = (from a in result
                                                    join b in data on a.SERIAL_NO equals b.SN
                                                    select new RetPatchEquipment()
                                                    {
                                                        INTERNET_NO = a.INTERNET_NO,
                                                        SERIAL_NO = a.SERIAL_NO,
                                                        CREATE_DATE = a.CREATE_DATE,
                                                        FOA_CODE = a.FOA_CODE,
                                                        LOCATION_CODE = b.LOCATION_CODE,
                                                        MOVEMENT_TYPE = a.MOVEMENT_TYPE,
                                                        SERIAL_STATUS = b.STATUS,
                                                        POST_DATE = a.POST_DATE,
                                                        CREATE_BY = a.CREATE_BY,
                                                        FILE_NAME = a.FILE_NAME,
                                                        PATCH_STATUS = a.PATCH_STATUS,
                                                        PATCH_STATUS_DESC = a.PATCH_STATUS_DESC,
                                                        STORAGE_LOCATION = a.STORAGE_LOCATION,
                                                        SUBMIT_DATE = a.SUBMIT_DATE,
                                                        UPDATE_BY = a.UPDATE_BY,
                                                        UPDATE_DATE = a.UPDATE_DATE,
                                                        REMARK = b.REMARK
                                                    }).ToList();

                                    ResultDataList.AddRange(datalist);

                                    skip += Size;
                                } while ((Size * page) < total);

                                var resultpending_newre = result.Where(s => s.PATCH_STATUS == "PENDING_NEWREGIST").ToList();
                                ResultDataList.AddRange(resultpending_newre);

                                foreach (var item in ResultDataList)
                                {
                                    var task = Task.Run(async delegate
                                    {
                                        try
                                        {
                                            //Serial-Status-Check
                                            if ((item.SERIAL_STATUS == "Available" || item.SERIAL_STATUS == "In Service") && ( item.REMARK == "" || item.REMARK == null) )
                                            {
                                                var resultprocess = ProcessUpdataCPE(item, jobs);
                                                resultDatas.Add(resultprocess);
                                            }
                                            else
                                            {
                                                // Serial Status != “Available” ให้ระบบเปลี่ยนข้อมูลรายการนั้นจาก PENDING เป็น CANCEL

                                                var commandupdateCANCEL = new FBBPaygPatchDataUpdateCommand()
                                                {
                                                    FileName = item.FILE_NAME,
                                                    serialno = item.SERIAL_NO,
                                                    snstatus = item.SERIAL_STATUS,
                                                    patchstatus = "CANCEL",
                                                    remark = item.REMARK,
                                                    FullUrl = "",
                                                    Transaction_Id = ""
                                                };
                                                jobs.UpdatePatchDataSN(commandupdateCANCEL);

                                                resultDatas.Add(new ResultData { FileName = item.FILE_NAME, result = "CANCEL" });
                                            }
                                        }
                                        catch
                                        {
                                            resultDatas.Add(new ResultData { FileName = item.FILE_NAME, result = "NOTSUCCESS" });
                                        }

                                        await Task.Delay(TimeSpan.FromSeconds(3));
                                    });
                                    task.Wait();
                                }
                            }
                            else //ถ้าไม่ใช้ Update CPE เลย
                            {
                                foreach (var item in result)
                                {
                                    var task = Task.Run(async delegate
                                    {
                                        try
                                        {
                                            var resultprocess = ProcessUpdataCPE(item, jobs);
                                            resultDatas.Add(resultprocess);
                                        }
                                        catch
                                        {
                                            resultDatas.Add(new ResultData { FileName = item.FILE_NAME, result = "NOTSUCCESS" });
                                        }

                                        await Task.Delay(TimeSpan.FromSeconds(3));
                                    });
                                    task.Wait();
                                }
                            }
                            #region PROCESS SENDEMAIL
                            //---------------- CHECK PROCESS SENDEMAIL
                            var sm = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPAYGPATCH_EQUIPMENT_BATCH", "PROCESS_SENDEMAIL").FirstOrDefault();

                            if (sm.DISPLAY_VAL == "Y")
                            {
                                var bodyMail = jobs.Get_FBB_CFG_LOV("Sendmail_Patch_SN").ToList();

                                var resultBoby = resultDatas.GroupBy(g => g.FileName).ToList();
                                var sendmail = new SendMailBatchPatchDataList();
                                List<SendMailBatchPatchList> _SendMail = new List<SendMailBatchPatchList>();
                                foreach (var item in resultBoby)
                                {
                                    //var getMailTo = jobs.GetDataPatchSNsendmail(item.Key);

                                    //foreach (var data in item)
                                    //	data.MailTo = getMailTo;

                                    var results = SetMailBoby(item, bodyMail);
                                    var L_Subject_Email = bodyMail.Where(w => w.LOV_NAME == "L_Subject_Email").FirstOrDefault().LOV_VAL1;

                                    //if (item.AsEnumerable().FirstOrDefault().MailTo.ToSafeString() != "")
                                    //{
                                    SendMailBatchPatchList list = new SendMailBatchPatchList();
                                    list.ProcessName = "SEND_EMAIL_PATCHCPE";
                                    list.SendTo = item.AsEnumerable().FirstOrDefault().MailTo.ToSafeString();
                                    list.SendFrom = "";
                                    list.Subject = L_Subject_Email;
                                    list.Body = results[0].ToString();
                                    list.FromPassword = "";
                                    list.Port = "";
                                    list.Domaim = "";
                                    list.IPMailServer = "";
                                    list.FileName = item.FirstOrDefault().FileName.ToSafeString();
                                    list.SendCC = "";
                                    list.SendBCC = "";


                                    _SendMail.Add(list);

                                    //}
                                }

                                sendmail.sendMailBatchPatchLists = _SendMail;
                                jobs.SendMail(sendmail);
                            }

                            var queryUpdateDateN = new UpdateFbssPatchDataConfigTblCommand()
                            {
                                display_val = "N",
                                val1 = getData.CreateDateFrom,
                                val2 = getData.CreateDateTo,
                                updated_by = "FBBPAYGPatch_EquipmentBatch"
                            };

                            jobs.TblUpdateData(queryUpdateDateN);

                            #endregion
                        }
                        else
                        {
                            var queryUpdateDate = new UpdateFbssPatchDataConfigTblCommand()
                            {
                                display_val = "N",
                                val1 = getData.CreateDateFrom,
                                val2 = getData.CreateDateTo,
                                updated_by = "FBBPAYGPatch_EquipmentBatch"
                            };
                            jobs.TblUpdateData(queryUpdateDate);
                        }
                    }
                }

            }
            Console.WriteLine($"FBBPAYGPatch_Equipment End");
            //Console.ReadLine();

        }

        private static ResultData ProcessUpdataCPE(RetPatchEquipment item, FBBPAYGPatchEquipment jobs)
        {
            try
            {
                var _resultData = new ResultData();
                if (item.PATCH_STATUS == "PENDING")
                {
                    var setPatchupdateCPE = new UpdateCPE();
                    setPatchupdateCPE.ACTION = "update";
                    setPatchupdateCPE.CPE_LIST = (new CPEList
                    {
                        SERIAL_NO = item.SERIAL_NO.ToSafeString(),
                        STATUS = "4",
                        LOCATION = item.LOCATION_CODE.ToSafeString() != "" ? item.LOCATION_CODE.ToSafeString() : null,
                        ACCESS_NO = item.INTERNET_NO.ToSafeString(),
                        CREATE_BY = "FBBPAYGPatch_Equipment_BATCH",
                        CREATE_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") //DD/MM/YYYY HH:MI:SS
                    });

                    var cpe = jobs.CallWSUpdateCPE(setPatchupdateCPE); //call ws 
                    if (cpe != null)
                    {
                        var commandupdate = new FBBPaygPatchDataUpdateCommand()
                        {
                            FileName = item.FILE_NAME,
                            serialno = item.SERIAL_NO,
                            snstatus = item.SERIAL_STATUS,
                            patchstatus = "PENDING_NEWREGIST",
                            remark = item.REMARK,
                            FullUrl = "",
                            Transaction_Id = ""
                        };
                        jobs.UpdatePatchDataSN(commandupdate);
                    }
                    else
                    {
                        return new ResultData { FileName = item.FILE_NAME, result = "CANCEL" };
                    }
                }

                var program_process = jobs.Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
                
                if (program_process.DISPLAY_VAL == "Y")
                {
                   

                    var q_productlist = new GetProductListHVRQuery
                    {
                        INTERNET_NO = item.INTERNET_NO,
                        SERIAL_NO = item.SERIAL_NO,
                        MOVEMENT_TYPE = item.MOVEMENT_TYPE
                    };

                    var getproListHVR = jobs.getProductListHVR(q_productlist);
                    if (getproListHVR.Count() != 0)
                    {

                        //เพิ่ม _fixAssConfig ในการ check CONFIG Handler
                        var resultFixAssConfig = jobs.GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();
                        List<NewRegistForSubmitFOAResponse> responseNewRegistSAP = new List<NewRegistForSubmitFOAResponse>();
                        List<NewRegistForSubmitFOAS4HANAResponse> responsesNewRegistSAPS4HANA = new List<NewRegistForSubmitFOAS4HANAResponse>();

                        jobs._logger.Info("Flag_RollbackSAP : " + resultFixAssConfig.DISPLAY_VAL);
                        if (resultFixAssConfig.DISPLAY_VAL == "Y")
                        {

                           var _main = SetDataHVR(item, getproListHVR); //เตรียมข้อมูล
                           var responssap  = jobs.CallWSNewRegistSAP(_main); //ส่ง SAP
                           responseNewRegistSAP.Add(responssap);//เก็บข้อมูลที่ได้เพื่อไป check
                        }                  
                        else
                        {
                          var _mains4hana = SetDataHVRS4HANA(item, getproListHVR); //เตรียมข้อมูล
                          var responses4hana = jobs.CallWSNewRegistSAPS4HANA(_mains4hana); //ส่ง SAP
                          responsesNewRegistSAPS4HANA.Add(responses4hana); //เก็บข้อมูลที่ได้เพื่อไป check
                        }
                        if (responseNewRegistSAP.Count > 0 || responsesNewRegistSAPS4HANA.Count > 0)
                        {
                            var commandupdate = new FBBPaygPatchDataUpdateCommand()
                            {
                                FileName = item.FILE_NAME,
                                serialno = item.SERIAL_NO,
                                snstatus = item.SERIAL_STATUS,
                                patchstatus = "SUCCESS",
                                remark = item.REMARK,
                                FullUrl = "",
                                Transaction_Id = ""
                            };
                            jobs.UpdatePatchDataSN(commandupdate);

                            return new ResultData { FileName = item.FILE_NAME, result = "SUCCESS" };
                        }
                        else
                        {
                            return new ResultData { FileName = item.FILE_NAME, result = "NOTSUCCESS" };

                        }
                    }
                    else   {
                        return new ResultData { FileName = item.FILE_NAME, result = "NOTSUCCESS", MailTo = "" };
                    }
                }
                else
                {
                    var q_productlist = new GetProductListSharePlexQuery
                    {
                        INTERNET_NO = item.INTERNET_NO,
                        SERIAL_NO = item.SERIAL_NO,
                        MOVEMENT_TYPE = item.MOVEMENT_TYPE
                    };

                     var getproList = jobs.getProductList(q_productlist);

                    if (getproList.Count() != 0)
                    {

                        //เพิ่ม _fixAssConfig ในการ check CONFIG Handler
                        var resultFixAssConfig = jobs.GET_FBSS_FIXED_ASSET_CONFIG("Flag_RollbackSAP").FirstOrDefault();
                        List<NewRegistForSubmitFOAResponse> responseNewRegistSAP = new List<NewRegistForSubmitFOAResponse>();
                        List<NewRegistForSubmitFOAS4HANAResponse> responsesNewRegistSAPS4HANA = new List<NewRegistForSubmitFOAS4HANAResponse>();

                        jobs._logger.Info("Flag_RollbackSAP : " + resultFixAssConfig.DISPLAY_VAL);
                        if (resultFixAssConfig.DISPLAY_VAL == "Y")
                        {

                            var _main = SetData(item, getproList); //เตรียมข้อมูล
                            var responssap = jobs.CallWSNewRegistSAP(_main); //ส่ง SAP
                            responseNewRegistSAP.Add(responssap);//เก็บข้อมูลที่ได้เพื่อไป check
                        }
                        else
                        {
                            var _mains4hana = SetDatas4hana(item, getproList); //เตรียมข้อมูล
                            var responses4hana = jobs.CallWSNewRegistSAPS4HANA(_mains4hana); //ส่ง SAP
                            responsesNewRegistSAPS4HANA.Add(responses4hana); //เก็บข้อมูลที่ได้เพื่อไป check
                        }
                        if (responseNewRegistSAP.Count > 0 || responsesNewRegistSAPS4HANA.Count > 0)
                        {

                           // var _main = SetData(item, getproList); //เตรียมข้อมูล
                       // var response = jobs.CallWSNewRegistSAP(_main); //ส่ง SAP
                       // if (response != null)
                       // {
                            var commandupdate = new FBBPaygPatchDataUpdateCommand()
                            {
                                FileName = item.FILE_NAME,
                                serialno = item.SERIAL_NO,
                                snstatus = item.SERIAL_STATUS,
                                patchstatus = "SUCCESS",
                                remark = item.REMARK,
                                FullUrl = "",
                                Transaction_Id = ""
                            };
                            jobs.UpdatePatchDataSN(commandupdate);

                            return new ResultData { FileName = item.FILE_NAME, result = "SUCCESS" };
                        }
                        else
                            return new ResultData { FileName = item.FILE_NAME, result = "NOTSUCCESS" };
                    }
                    else
                    {
                        //var commandupdate = new FBBPaygPatchDataUpdateCommand()   //29092021
                        //{
                        //	FileName = item.FILE_NAME,
                        //	serialno = item.SERIAL_NO,
                        //	snstatus = item.SERIAL_STATUS,
                        //	patchstatus = "PENDING_NEWREGIST",
                        //	remark = "Getproductlist is null",
                        //	FullUrl = "",
                        //	Transaction_Id = ""
                        //};
                        //jobs.UpdatePatchDataSN(commandupdate);

                        return new ResultData { FileName = item.FILE_NAME, result = "NOTSUCCESS", MailTo = "" };
                    }
                }



            }
            catch
            {
                return new ResultData { FileName = item.FILE_NAME, result = "NOTSUCCESS" };
            }
        }


        private static NewRegistForSubmitFOAQuery SetData(RetPatchEquipment model, List<ProductListSharePlex> ProductList)
        {
            try
            {
                var _main = new NewRegistForSubmitFOAQuery();
                var _product = new List<NewRegistFOAProductList>();

                #region ProcessWs NewRegist

                _main = new NewRegistForSubmitFOAQuery()
                {
                    Access_No = model.INTERNET_NO.ToSafeString(),
                    FOA_Submit_date = model.SUBMIT_DATE,
                    BUILDING_NAME = string.Empty,
                    Mobile_Contact = string.Empty,
                    OLT_NAME = string.Empty,
                    OrderNumber = string.Empty,
                    OrderType = "HumanError",
                    ProductName = ProductList.FirstOrDefault().PRODUCT_NAME.ToSafeString(),
                    RejectReason = ProductList.FirstOrDefault().REJECT_REASON.ToSafeString(),
                    SubcontractorCode = string.Empty,
                    SubcontractorName = string.Empty,
                    SubmitFlag = "PATCH_CPE",
                    Post_Date = model.POST_DATE,
                    Address_ID = string.Empty,
                    ORG_ID = string.Empty,
                    UserName = "FBBPAYGPatch_Equipment"
                };

                _product = ProductList.Select(p =>
                     {
                         return new NewRegistFOAProductList()
                         {
                             SerialNumber = model.SERIAL_NO.ToSafeString(),
                             MaterialCode = p.MATERIAL_CODE.ToSafeString(),
                             CompanyCode = p.COM_CODE.ToSafeString(),
                             Plant = p.PLANT.ToSafeString(),
                             StorageLocation = p.STORAGE_LOCATION.ToSafeString(),
                             SNPattern = p.SN_PATTERN.ToSafeString(),
                             MovementType = p.MOVEMENT_TYPE.ToSafeString()
                         };
                     }).ToList();
                //}

                _main.ProductList = _product;

                var _services = new NewRegistFOAServiceList()
                {
                    ServiceName = null
                };

                List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                _service.Add(new NewRegistFOAServiceList() { ServiceName = string.Empty });

                _main.ServiceList = _service;

                #endregion ProcessWs NewRegist

                return _main;


            }
            catch
            {
                return new NewRegistForSubmitFOAQuery();
                //----------------------------------------------------------------
                //Try for GetProductListQuery,Send To SAP
                //----------------------------------------------------------------
            }
        }

        private static NewRegistForSubmitFOA4HANAQuery SetDatas4hana(RetPatchEquipment model, List<ProductListSharePlex> ProductList)
        {
            try
            {
                var _main = new NewRegistForSubmitFOA4HANAQuery();
                var _product = new List<NewRegistFOAProductList>();

                #region ProcessWs NewRegist

                _main = new NewRegistForSubmitFOA4HANAQuery()
                {
                    Access_No = model.INTERNET_NO.ToSafeString(),
                    FOA_Submit_date = model.SUBMIT_DATE,
                    BUILDING_NAME = string.Empty,
                    Mobile_Contact = string.Empty,
                    OLT_NAME = string.Empty,
                    OrderNumber = string.Empty,
                    OrderType = "HumanError",
                    ProductName = ProductList.FirstOrDefault().PRODUCT_NAME.ToSafeString(),
                    RejectReason = ProductList.FirstOrDefault().REJECT_REASON.ToSafeString(),
                    SubcontractorCode = string.Empty,
                    SubcontractorName = string.Empty,
                    SubmitFlag = "PATCH_CPE",
                    Post_Date = model.POST_DATE,
                    Address_ID = string.Empty,
                    ORG_ID = string.Empty,
                    UserName = "FBBPAYGPatch_Equipment"
                };

                _product = ProductList.Select(p =>
                {
                    return new NewRegistFOAProductList()
                    {
                        SerialNumber = model.SERIAL_NO.ToSafeString(),
                        MaterialCode = p.MATERIAL_CODE.ToSafeString(),
                        CompanyCode = p.COM_CODE.ToSafeString(),
                        Plant = p.PLANT.ToSafeString(),
                        StorageLocation = p.STORAGE_LOCATION.ToSafeString(),
                        SNPattern = p.SN_PATTERN.ToSafeString(),
                        MovementType = p.MOVEMENT_TYPE.ToSafeString()
                    };
                }).ToList();
                //}

                _main.ProductList = _product;

                var _services = new NewRegistFOAServiceList()
                {
                    ServiceName = null
                };

                List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                _service.Add(new NewRegistFOAServiceList() { ServiceName = string.Empty });

                _main.ServiceList = _service;

                #endregion ProcessWs NewRegist

                return _main;


            }
            catch
            {
                return new NewRegistForSubmitFOA4HANAQuery();
                //----------------------------------------------------------------
                //Try for GetProductListQuery,Send To SAP
                //----------------------------------------------------------------
            }
        }





        private static NewRegistForSubmitFOAQuery SetDataHVR(RetPatchEquipment model, List<ProductListHVR> ProductList)
        {
            try
            {
                var _main = new NewRegistForSubmitFOAQuery();
                var _product = new List<NewRegistFOAProductList>();

                #region ProcessWs NewRegist

                _main = new NewRegistForSubmitFOAQuery()
                {
                    Access_No = model.INTERNET_NO.ToSafeString(),
                    FOA_Submit_date = model.SUBMIT_DATE,
                    BUILDING_NAME = string.Empty,
                    Mobile_Contact = string.Empty,
                    OLT_NAME = string.Empty,
                    OrderNumber = string.Empty,
                    OrderType = "HumanError",
                    ProductName = ProductList.FirstOrDefault().PRODUCT_NAME.ToSafeString(),
                    RejectReason = ProductList.FirstOrDefault().REJECT_REASON.ToSafeString(),
                    SubcontractorCode = string.Empty,
                    SubcontractorName = string.Empty,
                    SubmitFlag = "PATCH_CPE",
                    Post_Date = model.POST_DATE,
                    Address_ID = string.Empty,
                    ORG_ID = string.Empty,
                    Product_Owner = string.Empty,
                    Main_Promo_Code = string.Empty,
                    Team_ID = string.Empty,
                    UserName = "FBBPAYGPatch_Equipment"
                };

                _product = ProductList.Select(p =>
                {
                    return new NewRegistFOAProductList()
                    {
                        SerialNumber = model.SERIAL_NO.ToSafeString(),
                        MaterialCode = p.MATERIAL_CODE.ToSafeString(),
                        CompanyCode = p.COM_CODE.ToSafeString(),
                        Plant = p.PLANT.ToSafeString(),
                        StorageLocation = p.STORAGE_LOCATION.ToSafeString(),
                        SNPattern = p.SN_PATTERN.ToSafeString(),
                        MovementType = p.MOVEMENT_TYPE.ToSafeString()
                    };
                }).ToList();
                //}

                _main.ProductList = _product;

                var _services = new NewRegistFOAServiceList()
                {
                    ServiceName = null
                };

                List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                _service.Add(new NewRegistFOAServiceList() { ServiceName = string.Empty });

                _main.ServiceList = _service;

                #endregion ProcessWs NewRegist

                return _main;


            }
            catch
            {
                return new NewRegistForSubmitFOAQuery();
                //----------------------------------------------------------------
                //Try for GetProductListQuery,Send To SAP
                //----------------------------------------------------------------
            }
        }



        private static NewRegistForSubmitFOA4HANAQuery SetDataHVRS4HANA(RetPatchEquipment model, List<ProductListHVR> ProductList)
        {
            try
            {
                var _main = new NewRegistForSubmitFOA4HANAQuery();
                var _product = new List<NewRegistFOAProductList>();

                #region ProcessWs NewRegist

                _main = new NewRegistForSubmitFOA4HANAQuery()
                {
                    Access_No = model.INTERNET_NO.ToSafeString(),
                    FOA_Submit_date = model.SUBMIT_DATE,
                    BUILDING_NAME = string.Empty,
                    Mobile_Contact = string.Empty,
                    OLT_NAME = string.Empty,
                    OrderNumber = string.Empty,
                    OrderType = "HumanError",
                    ProductName = ProductList.FirstOrDefault().PRODUCT_NAME.ToSafeString(),
                    RejectReason = ProductList.FirstOrDefault().REJECT_REASON.ToSafeString(),
                    SubcontractorCode = string.Empty,
                    SubcontractorName = string.Empty,
                    SubmitFlag = "PATCH_CPE",
                    Post_Date = model.POST_DATE,
                    Address_ID = string.Empty,
                    ORG_ID = string.Empty,
                    Product_Owner = string.Empty,
                    Main_Promo_Code = string.Empty,
                    Team_ID = string.Empty,
                    UserName = "FBBPAYGPatch_Equipment"
                };

                _product = ProductList.Select(p =>
                {
                    return new NewRegistFOAProductList()
                    {
                        SerialNumber = model.SERIAL_NO.ToSafeString(),
                        MaterialCode = p.MATERIAL_CODE.ToSafeString(),
                        CompanyCode = p.COM_CODE.ToSafeString(),
                        Plant = p.PLANT.ToSafeString(),
                        StorageLocation = p.STORAGE_LOCATION.ToSafeString(),
                        SNPattern = p.SN_PATTERN.ToSafeString(),
                        MovementType = p.MOVEMENT_TYPE.ToSafeString()
                    };
                }).ToList();
                //}

                _main.ProductList = _product;

                var _services = new NewRegistFOAServiceList()
                {
                    ServiceName = null
                };

                List<NewRegistFOAServiceList> _service = new List<NewRegistFOAServiceList>();
                _service.Add(new NewRegistFOAServiceList() { ServiceName = string.Empty });

                _main.ServiceList = _service;

                #endregion ProcessWs NewRegist

                return _main;


            }
            catch
            {
                return new NewRegistForSubmitFOA4HANAQuery();
                //----------------------------------------------------------------
                //Try for GetProductListQuery,Send To SAP
                //----------------------------------------------------------------
            }
        }

        private static string[] SetMailBoby(IGrouping<string, ResultData> data, List<LovModel> bodyMail)
        {
            string[] result = new string[2];

            var L_Body_Runbatch_Email = bodyMail.Where(w => w.LOV_NAME == "L_Body_Runbatch_Email").FirstOrDefault().LOV_VAL1;
            var L_Body_Filename_Email = bodyMail.Where(w => w.LOV_NAME == "L_Body_Filename_Email").FirstOrDefault().LOV_VAL1;
            var L_Body_Total_Email = bodyMail.Where(w => w.LOV_NAME == "L_Body_Total_Email").FirstOrDefault().LOV_VAL1;
            var L_Body_Success_Email = bodyMail.Where(w => w.LOV_NAME == "L_Body_Success_Email").FirstOrDefault().LOV_VAL1;
            var L_Body_Failed_Email = bodyMail.Where(w => w.LOV_NAME == "L_Body_Failed_Email").FirstOrDefault().LOV_VAL1;
            var L_Body_Cancel_Email = bodyMail.Where(w => w.LOV_NAME == "L_Body_Cancel_Email").FirstOrDefault().LOV_VAL1;

            var resultTotal = data.AsEnumerable().GroupBy(g => g.result).ToList();


            var success = 0;
            var notsuccess = 0;
            var cancel = 0;

            if (resultTotal.Any())
            {
                foreach (var item in resultTotal)
                {
                    if (item.Key == "SUCCESS")
                        success = item.AsEnumerable().Count();
                    else if (item.Key == "NOTSUCCESS")
                        notsuccess = item.AsEnumerable().Count();
                    else if (item.Key == "CANCEL")
                        cancel = item.AsEnumerable().Count();
                }
            }

            StringBuilder body = new StringBuilder(2000);
            var nl = Environment.NewLine;
            body.AppendFormat($"{L_Body_Runbatch_Email} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} <br/>{nl}");
            body.AppendFormat($"{L_Body_Filename_Email} : {data.FirstOrDefault().FileName} <br/>{nl}");
            body.AppendFormat($"{L_Body_Total_Email.Replace("{}", data.Count().ToString())} <br/>{nl}");
            body.AppendFormat($"{L_Body_Success_Email.Replace("{}", success.ToString())} <br/>{nl}");
            body.AppendFormat($"{L_Body_Failed_Email.Replace("{}", notsuccess.ToString())} <br/>{nl}");
            body.AppendFormat($"{L_Body_Cancel_Email.Replace("{}", cancel.ToString())} <br/>{nl}");

            result[0] = body.ToString();
            result[1] = data.FirstOrDefault().MailTo;

            return result;
        }

    }
    public class ResultData
    {
        public string FileName { get; set; }
        public string result { get; set; }
        public string MailTo { get; set; }
    }


 
}
