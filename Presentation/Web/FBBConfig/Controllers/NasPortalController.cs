using FBBConfig.Extensions;
using FBBConfig.Models;
using Kendo.Mvc.Extensions;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBContract.Queries.SftpQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class NasPortalController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FbbNasUpdateLogCommand> _nasUploadLogCommand;

        private static NasPortalInterface _nasPortalInterface;

        public NasPortalController(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<FbbNasUpdateLogCommand> fbbNasUpdateLogCommand)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
            _nasUploadLogCommand = fbbNasUpdateLogCommand;
        }

        public ActionResult Index()
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            ViewBag.User = CurrentUser;
       
            return View();
        }

        //------------------------------------------






        public bool ValidateFileNameFormatnew(HttpPostedFileBase inputFile, string pathName, ref string msg)
        {
            bool succ = false;
            if (inputFile != null)
            {
                //string fileName = Path.GetFileNameWithoutExtension(inputFile.FileName).ToLower();
                //string fileExtension = Path.GetExtension(inputFile.FileName).ToLower();
                //long fileSizeBytes = inputFile.ContentLength;

                //var lovList = Get_FBSS_CONFIG_LOV("FBB_NAS_PORTAL", "NAS_PORTAL_PATH_UP")
                //               .Where(r => r.Text == pathName && r.ActiveFlag == "Y")
                //               .ToList();

                //if (lovList == null || lovList.Count == 0)
                //{
                //    msg = $"Path not found: {pathName}";
                //    return false;
                //}


                //bool matched = false;
                //foreach (var lov in lovList)
                //{
                //    string prefix = (lov.LovValue1 ?? "").ToLower();
                //    string[] allowedExts = (lov.LovValue2 ?? "").ToLower().Split(',');
                //    string sizeStr = lov.LovValue3 ?? "0";
                //    string unit = (lov.LovValue4 ?? "MB").ToUpper();

                //    if (fileName.StartsWith(prefix))
                //    {

                //        if (allowedExts.Contains(fileExtension))
                //        {
                //            if (int.TryParse(sizeStr, out int maxSize))
                //            {
                //                long maxBytes = unit == "KB" ? maxSize * 1024 : maxSize * 1024 * 1024;
                //                if (fileSizeBytes <= maxBytes)
                //                {
                //                    matched = true;
                //                    break;
                //                }
                //                else
                //                {
                //                    msg = $"{inputFile.FileName} : Unable to upload this file because it exceeds the maximum file size limit.";
                //                    return false;
                //                }
                //            }

                //        }
                //        else
                //        {
                //            msg = $"{inputFile.FileName} : Unable to upload this file because the file extension is not supported.";
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        msg = $"{inputFile.FileName} : Unable to upload this file because the file name is invalid.";
                //        continue;
                //    }


                //}

                //if (!matched)
                //{
                //    msg = $"{inputFile.FileName} : Unable to upload this file because the file name is invali";
                //}
                //else
                //{
                //    succ = true;
                //}
                var fileName = Path.GetFileName(inputFile?.FileName ?? string.Empty);
                var fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
                var fileExt = (Path.GetExtension(fileName) ?? "")
                                    .TrimStart('.')
                                    .ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(fileNameNoExt))
                {
                    msg = "Invalid file name.";
                    return false;
                }

                // ---------- 2) ดึงคอนฟิก (เทียบแบบไม่สนตัวพิมพ์) ----------
                var lovList = Get_FBSS_CONFIG_LOV("FBB_NAS_PORTAL", "NAS_PORTAL_PATH_UP")
                    .Where(r =>
                        string.Equals(r.Text, pathName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(r.ActiveFlag?.Trim(), "Y", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (lovList.Count == 0)
                {
                    msg = $"No active config for path '{pathName}'.";
                    return false;
                }

                // helper: split CSV ด้วย comma อย่างเดียวแล้ว Trim
                IEnumerable<string> Csv(string s) =>
                    (s ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(x => x.Trim());

                // ---------- 3) หา rule ตาม prefix ----------
                var hit = lovList
                    .SelectMany(r => Csv(r.LovValue1).Select(p => new { Rule = r, Prefix = p }))
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x.Prefix) &&
                                         fileNameNoExt.StartsWith(x.Prefix, StringComparison.OrdinalIgnoreCase));

                if (hit == null)
                {
                    var allPrefixes = lovList.SelectMany(r => Csv(r.LovValue1))
                                             .Where(p => p.Length > 0)
                                             .Distinct(StringComparer.OrdinalIgnoreCase);
                    msg = $"{fileName} Unable to upload this file because the file name is invalid.";
                    return false;
                }

                // ---------- 4) เช็คนามสกุลไฟล์ ----------
                var allowedExts = new HashSet<string>(
                    Csv(hit.Rule.LovValue2).Select(e => e.TrimStart('.').ToLowerInvariant()),
                    StringComparer.OrdinalIgnoreCase);

                // ถ้า config ไม่ได้ระบุเลย = ไม่บังคับ, ถ้าระบุแล้วต้องตรง
                if (allowedExts.Count > 0 && !allowedExts.Contains(fileExt))
                {
                    var allowedForDisplay = allowedExts.Select(x => "." + x);
                    msg = $"{fileName} Unable to upload this file because the file extension is not supported.";
                    return false;
                }

                // ---------- 5) เช็คขนาดไฟล์ ----------
                long? maxBytes = null;
                if (long.TryParse(hit.Rule.LovValue3?.Trim(), out var maxSize) && maxSize > 0)
                {
                    var unit = (hit.Rule.LovValue4 ?? "MB").Trim().ToUpperInvariant();
                    long factor = unit == "KB" ? 1024L : 1024L * 1024L;
                    try { maxBytes = checked(maxSize * factor); }
                    catch (OverflowException)
                    {
                        msg = "Configured max file size is too large.";
                        return false;
                    }
                }
                long fileSizeBytes = inputFile != null ? (long)inputFile.ContentLength : 0L;

                if (maxBytes.HasValue && fileSizeBytes > maxBytes.Value)
                {
                    string lim = maxBytes.Value % (1024L * 1024L) == 0
                        ? $"{maxBytes.Value / (1024L * 1024L)} MB"
                        : $"{maxBytes.Value / 1024L} KB";
                    msg = $"{fileName} Unable to upload this file because it exceeds the maximum file size limit.";
                    return false;
                }


                succ = true;
                
            }
            return succ; 
        }


        public bool ValidateFileNameFormat(HttpPostedFileBase inputFile, string pathName, ref string msg)
        {
            bool succ = false;
            if (inputFile != null)
            {


                string fileName = Path.GetFileNameWithoutExtension(inputFile.FileName).ToLower();
                string fileExtension = Path.GetExtension(inputFile.FileName).ToLower();

                // Regex regex = null;
                string compareFileExtension = null;
                bool compareFile = false;

                switch (pathName.ToUpper())
                {
                    case "SAP_FBB_IN":
                        //regex = new Regex("(1200||1800)\\d+(_AIRNET_INV_)+[0-9]{4}+(1[0-2]|0[1-9])+(3[01]|[12][0-9]|0[1-9])+_+(?:[01]\\d|2[0123])+(?:[012345]\\d)+(?:[012345]\\d)");
                        compareFile = fileName.ToUpper().StartsWith("1200_AIRNET_INV") || fileName.ToUpper().StartsWith("1800_AIRNET_INV");
                        compareFileExtension = ".dat";
                        break;
                    case "REFURBISHED_FBB":
                        //regex = new Regex(@"(CPE_Inv_Control_)+[0-9]{4}+(1[0-2]|0[1-9])+(3[01]|[12][0-9]|0[1-9])+_+(?:[01]\d|2[0123])+(?:[012345]\d)+(?:[012345]\d)");
                        compareFile = fileName.ToUpper().StartsWith("CPE_INV_CONTROL");
                        compareFileExtension = ".dat";
                        break;
                    case "SAP_FBB_OUT":
                        //regex = new Regex(@"(1200||1800)\d+(_INVENTORY_TRANS_MATERIAL_)+[0-9]{4}+(1[0-2]|0[1-9])+(3[01]|[12][0-9]|0[1-9])+_+(?:[01]\d|2[0123])+(?:[012345]\d)+(?:[012345]\d)");
                        compareFile = fileName.ToUpper().StartsWith("1200_INVENTORY_TRANS_MATERIAL") || fileName.ToUpper().StartsWith("1800_INVENTORY_TRANS_MATERIAL");
                        compareFileExtension = ".txt";
                        break;
                    default: msg = "path not found."; break;
                }

                if (fileExtension == compareFileExtension)
                {
                    if (compareFile && compareFileExtension != null)
                    {
                        succ = true;
                    }
                    else
                    {
                        succ = false;
                        msg = inputFile.FileName + " : FileName mismatch format.";
                    }
                }
                else
                    msg = inputFile.FileName + " : fileExtension mismatch format.";
            }
            return succ;
        }

        public ActionResult NasFile_Save(IEnumerable<HttpPostedFileBase> NasFile, NasPortalInterface model)    //HttpPostedFileBase
        {
            string resultMessage = "";
            bool status = false;
            string find_filename = "";

            if (CurrentUser == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            if (NasFile != null)
            {
                try
                {
                    foreach (var item in NasFile)
                    {
                        find_filename = Path.GetFileName(item.FileName);
                        string msg = "";
                        int limitsize = 0;
                        //    var islimit = CheckLimitFileSize(item.ContentLength, out limitsize);
                        //  if (this.ValidateFileNameFormat(item, _nasPortalInterface.NasDisplayVal, ref msg) && islimit)

                        if (this.ValidateFileNameFormatnew(item, _nasPortalInterface.NasDisplayVal, ref msg))

                        {
                            #region Upload
                            var param = new NasPortalGetFileOwnerQuery()
                            {
                                p_file_name = find_filename,
                            };
                            var resultOwner = _queryProcessor.Execute(param);

                            if (resultOwner.ret_file_owner == null || resultOwner.ret_file_owner == "")
                            {
                                var ValueType = "";
                                var AccessType = CheckAccessType(_nasPortalInterface.NasDisplayVal, out ValueType);
                                var result = new UploadFileModel();
                                _Logger.Info($"Type : {ValueType}");

                                if (ValueType == "WINDOW")
                                {
                                    var modelUpload = new NasUploadFileQuery
                                    {
                                        Key = Configurations._keydatapower,
                                        FileName = find_filename,
                                        Username = CurrentUser.UserName,
                                        DataFile = item.InputStream.ToBytes(),
                                        Path = _nasPortalInterface.Path + "\\" + find_filename,
                                        NasType = _nasPortalInterface.NasDisplayVal,
                                        TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                                    };
                                    _Logger.Info($"Transection Id : {modelUpload.TransectionId}");
                                    result = _queryProcessor.Execute(modelUpload);
                                }
                                else
                                {
                                    var modelUpload = new UploadFileQuery
                                    {
                                        Key = Configurations._keydatapower,
                                        FileName = find_filename,
                                        Username = CurrentUser.UserName,
                                        DataFile = item.InputStream.ToBytes(),
                                        Path = _nasPortalInterface.Path + "/" + find_filename,
                                        NasType = _nasPortalInterface.NasDisplayVal,
                                        TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                                    };
                                    _Logger.Info($"Transection Id : {modelUpload.TransectionId}");
                                    result = _queryProcessor.Execute(modelUpload);
                                }

                                _Logger.Info($"Start Upload File : {find_filename}");
                                if (result != null)
                                {
                                    _Logger.Info($"{result.Message}");
                                    status = result.Upload;
                                    resultMessage = status == true ? "" : $"{find_filename} : {result.Message}";
                                }
                                else
                                {
                                    _Logger.Info($"Upload File Not Found");
                                }
                            }
                            else
                            {
                                status = false;
                                resultMessage = "Can not upload file " + find_filename + " because file name is existing. ";
                            }
                            #endregion
                        }
                        else
                        {
                            status = false;
                            // bresultMessage = islimit != false ?
                            //    $"{find_filename} : fileName mismatch format " :
                            //    $"{find_filename} : The Maximun file size is {limitsize}MB. Please check your file size. ";
                            resultMessage = $"{find_filename} : The Maximum file size is {limitsize}MB. Please check your file size.";

                            if (msg != "")
                                resultMessage = msg;
                        }
                    }
                }
                catch
                {
                    status = false;
                    resultMessage = $"{find_filename} : The Maximun file size is 20MB. Please check your file size. ";
                }
            }

            var successResponse = new { status = status, message = resultMessage };
            return Json(successResponse, "text/plain");
        }

        private bool CheckLimitFileSize(int contentLength, out int limitsize)
        {
            var query = new GetLovQuery
            {
                LovName = "FILE_SIZE_UPLOAD",
                LovType = "FBB_NAS_PORTAL"
            };
            var limit = _queryProcessor.Execute(query)
                .Select(s => new
                {
                    s.LovValue1,
                    s.LovValue2,
                    s.ActiveFlag
                }).First();

            if (limit.ActiveFlag.Equals("Y"))
            {
                Int64 length = limit.LovValue2.Equals("MB") ? ((Int64.Parse(limit.LovValue1) * 1024) * 1024) : Int64.Parse(limit.LovValue1);
                limitsize = int.Parse(limit.LovValue1);
                if (contentLength >= length) return false;
                else return true;
            }
            else
            {
                limitsize = 20;
                return true;
            }

        }
        public List<LovValueModel> Get_FBSS_CONFIG_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetLovQuery()
            {
                LovType = _CON_TYPE,
                LovName = _CON_NAME
            };
            var _FbssConfigs = _queryProcessor.Execute(query);

            return _FbssConfigs;
        }
        public ActionResult NasFile_Remove(string[] file)
        {
            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }

        [HttpPost]
        public ActionResult ListFiles(NasPortalInterface input)
        {
            try
            {
                List<ListfilesModels> result = new List<ListfilesModels>();
                _nasPortalInterface = new NasPortalInterface();
                _nasPortalInterface.Host = Configurations.NAS_HOST;//"192.168.1.25";
                _nasPortalInterface.Path = input.Path;
                _nasPortalInterface.NasConnection = input.NasConnection; //"//10.0.4.79/Users/test/Desktop/SAP/"
                _nasPortalInterface.NasDisplayVal = input.NasDisplayVal;

                _Logger.Info($"Start ListFiles Path : {input.Path}");


                var ValueType = "";
                var AccessType = CheckAccessType(_nasPortalInterface.NasDisplayVal, out ValueType);

                _Logger.Info($"Type : {ValueType}");

                if (ValueType == "WINDOW")
                {
                    var model = new GetfilesAllQuery
                    {
                        Key = Configurations._keydatapower,
                        Path = _nasPortalInterface.Path,
                        NasType = _nasPortalInterface.NasDisplayVal,
                        TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                    };
                    _Logger.Info($"Transection Id : {model.TransectionId}");
                    result = _queryProcessor.Execute(model);
                }
                else
                {
                    var model = new ListfilesQuery
                    {
                        Key = Configurations._keydatapower,
                        Path = _nasPortalInterface.Path,
                        NasType = _nasPortalInterface.NasDisplayVal,
                        TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                    };
                    _Logger.Info($"Transection Id : {model.TransectionId}");
                    result = _queryProcessor.Execute(model);
                }
                if (result != null)
                {
                    var filelist = result.Select(s => string.Concat("File Name : ", s.Name)).ToList();
                    _Logger.Info($"Success \r\n{string.Join("\r\n", filelist)}");

                    if (!string.IsNullOrEmpty(input.TextSearch))
                        return Json(result.Where(s => !s.Name.StartsWith(".") & s.Name.Contains(input.TextSearch)).ToList());
                    else
                        return Json(result.Where(s => !s.Name.StartsWith(".")).ToList());
                }
                else
                {
                    _Logger.Info($"File Not found");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                _Logger.Info($"Error : {ex.Message.ToString()}");
                return Json(null);
            }
        }

        public ActionResult DownloadFile(string fileName)
        {
            try
            {
                var ValueType = "";
                var AccessType = CheckAccessType(_nasPortalInterface.NasDisplayVal, out ValueType);
                var result = new DownloadFileModel();
                _Logger.Info($"Start Download File : {fileName}");
                if (ValueType == "WINDOW")
                {
                    var model = new NasDownloadFileQuery
                    {
                        Key = Configurations._keydatapower,
                        NasType = _nasPortalInterface.NasDisplayVal,
                        FileName = fileName,
                        TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                    };
                    _Logger.Info($" Transection Id : {model.TransectionId}");
                    result = _queryProcessor.Execute(model);
                }
                else
                {
                    var model = new DownloadFileQuery
                    {
                        Key = Configurations._keydatapower,
                        NasType = _nasPortalInterface.NasDisplayVal,
                        FileName = fileName,
                        TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                    };
                    _Logger.Info($" Transection Id : {model.TransectionId}");
                    result = _queryProcessor.Execute(model);
                }


                if (result != null)
                {
                    _Logger.Info($"Download File : {fileName} Success.");
                    return File(result.Download, "application/octet-stream", fileName);
                }
                else
                {
                    _Logger.Info($"Download File Not Found");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _Logger.Info($"Error : {ex.Message.ToString()}");
                return null;
            }
        }

        private object CheckAccessType(string displayValue, out string valueType)
        {
            var getLov = Get_LOV("FBB_NAS_PORTAL", "NAS_PORTAL_PATH", displayValue);

            if (getLov == null)
            {
                valueType = "LINUX";
            }
            else
            {
                valueType = getLov.LovValue5;
            }

            return true;
        }

        [HttpPost]
        public ActionResult DeleteFile(string fileName)
        {
            string resultMessage = "";
            try
            {
                var param = new NasPortalGetFileOwnerQuery()
                {
                    p_file_name = Path.GetFileName(fileName).ToString(),
                };
                var resultOwner = _queryProcessor.Execute(param);
                if (resultOwner != null)
                {
                    if (CurrentUser.UserName == resultOwner.ret_file_owner)
                    {
                        var ValueType = "";
                        var AccessType = CheckAccessType(_nasPortalInterface.NasDisplayVal, out ValueType);
                        var result = new DeleteFileModel();
                        _Logger.Info($"Start Download File : {fileName}");

                        if (ValueType == "WINDOW")
                        {
                            //fileName = Path.Combine(_nasPortalInterface.Path, fileName);
                            var model = new NasDeleteFileQuery
                            {
                                Key = Configurations._keydatapower,
                                NasType = _nasPortalInterface.NasDisplayVal,
                                FileName = fileName,
                                TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                            };
                            _Logger.Info($" Transection Id : {model.TransectionId}");
                            result = _queryProcessor.Execute(model);
                        }
                        else
                        {
                            fileName = _nasPortalInterface.Path + "/" + fileName;
                            var model = new DeleteFileQuery
                            {
                                Key = Configurations._keydatapower,
                                NasType = _nasPortalInterface.NasDisplayVal,
                                FileName = fileName,
                                TransectionId = System.Reflection.MethodBase.GetCurrentMethod().GetTransectionId()
                            };
                            _Logger.Info($" Transection Id : {model.TransectionId}");
                            result = _queryProcessor.Execute(model);
                        }

                        _Logger.Info($"Start Delete File : {param.p_file_name}");

                        if (result != null)
                        {
                            if (result.Delete)
                            {
                                return Json(new { success = true, message = "File has been uploaded." });
                            }
                            else
                            {
                                return Json(new { success = false, message = result.Message });
                                _Logger.Info(result.Message);
                            }

                        }
                        else
                        {
                            _Logger.Info($"Delete File Not Found");
                            return null;
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Can not Delete " + param.p_file_name + " because you are not file owner." });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "Can not Delete file " + param.p_file_name + " because file not found, please contact system admin" });
                }
            }
            catch (Exception ex)
            {
                _Logger.Info($"Error : {ex.Message.ToString()}");
                return null;
            }
        }

        //------------------------------------------

        public void SetViewBagLov(string LOV_TYPE, string LOV_NAME, string LOV_VAL5)
        {
            var query = new GetLovQuery()
            {
                LovType = LOV_TYPE,
                LovName = LOV_NAME
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);
        }

        public List<LovValueModel> Get_FBB_CFG_LOV(string LOV_TYPE, string LOV_NAME)
        {
            var query = new GetLovQuery()
            {
                LovType = LOV_TYPE,
                LovName = LOV_NAME,
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);


            return _FbbCfgLov;
        }

        public LovValueModel Get_LOV(string LOV_TYPE, string LOV_NAME, string DISPLAY_VAL)
        {
            var query = new GetLovQuery()
            {
                LovType = LOV_TYPE,
                LovName = LOV_NAME,
            };
            var _FbbCfgLov = _queryProcessor.Execute(query).Where(w => w.Text.Equals(DISPLAY_VAL)).FirstOrDefault();


            return _FbbCfgLov;
        }

        public JsonResult SetDDLNasPortal()
        {
            var NasPortal = Get_FBB_CFG_LOV("FBB_NAS_PORTAL", "NAS_PORTAL_PATH");
            var model = new List<LovModel>();
            if (NasPortal.Count != 0)
            {
                model.Add(new LovModel() { LOV_NAME = "Chaose Path", LOV_VAL1 = "0" });


                foreach (var item in NasPortal)
                {
                   model.Add(new LovModel() { LOV_NAME = item.Text, LOV_VAL1 = item.LovValue1, LOV_VAL2 = item.LovValue2, DISPLAY_VAL = item.Text });


                }
             
            }
            var LovData = model.Select(p => { return new { LOV_NAME = p.LOV_NAME, LOV_VAL1 = p.LOV_VAL1 + "|" + p.LOV_VAL2 }; }).ToList();
            return Json(LovData, JsonRequestBehavior.AllowGet);
        }






        public JsonResult SetDDLNasPortals()
        {
            var NasPortal = Get_FBB_CFG_LOV("FBB_NAS_PORTAL", "NAS_PORTAL_PATH");
            var model = new List<LovModel>();

            if (NasPortal.Count != 0)
            {
                model.Add(new LovModel()
                {
                    LOV_NAME = "Choose Path",
                    LOV_VAL1 = "0",
                    LOV_VAL2 = "",
                    DISPLAY_VAL = "",
                    LOV_VAL3 = "",
                    EXISTS_IN_UPLOAD_PATH = false
                });

                var lovValue = Get_FBB_CFG_LOV("FBB_NAS_PORTAL", "NAS_PORTAL_PATH_UP")
                            .Where(r => r.ActiveFlag == "Y")
                            .ToList();

                foreach (var item in NasPortal)
                {
                    bool exists = false;
                    string extension = "";
                    var dataLov = lovValue.Where(r => r.Text == item.Text).FirstNonDefault();
                    if (dataLov != null)
                    {
                        exists = true;
                        extension = dataLov.LovValue2 + "|"+ dataLov.LovValue3;
                    }

                    model.Add(new LovModel()
                    {
                        LOV_NAME = item.Text,
                        LOV_VAL1 = item.LovValue1,
                        LOV_VAL2 = item.LovValue2,
                        LOV_VAL3 = extension,
                        DISPLAY_VAL = item.Text,
                        EXISTS_IN_UPLOAD_PATH = exists
                    });
                }
            }

            var LovData = model.Select(p => new
            {
                LOV_NAME = p.LOV_NAME,
                LOV_VAL1 = p.LOV_VAL1 + "|" + p.LOV_VAL2,
                LOV_VAL3 = p.LOV_VAL3,
                EXISTS = p.EXISTS_IN_UPLOAD_PATH
            }).ToList();

            return Json(LovData, JsonRequestBehavior.AllowGet);
        }

        public bool Checkupload(string displayVal)
        {
            var lovList = Get_FBB_CFG_LOV("FBB_NAS_PORTAL", "NAS_PORTAL_PATH_UP")
                            .Where(r => r.Text == displayVal &&                                    
                                        r.ActiveFlag == "Y")
                            .ToList();
            if(lovList != null && lovList.Count > 0){
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
