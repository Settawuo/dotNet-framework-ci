using FBBConfig.Controllers;
using FBBConfig.Extensions;
using FBBConfig.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
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


    //public class MemoryPostedFile : HttpPostedFileBase
    //{
    //    private readonly byte[] fileBytes;

    //    public MemoryPostedFile(byte[] fileBytes, string fileName = null)
    //    {
    //        this.fileBytes = fileBytes;
    //        //this.FileName = fileName;
    //        //this.InputStream = new MemoryStream(fileBytes);
    //    }

    //    public override int ContentLength {
    //        get { return fileBytes.Length; }
    //    }

    //    public override string FileName { get; }

    //    public override Stream InputStream { get {
    //        return new MemoryStream(this.fileBytes);
    //    } }
    //}

    [CustomActionFilter]
    [CustomHandleError]
    [IENoCache]
    public partial class ProcessController : FBBConfigController
    {
        private readonly ICommandHandler<ChangeContactMobileCommand> _changeContactMobileCommand;
        private readonly ICommandHandler<UpdateFileNameCommand> _updateFileNameCommand;
        private readonly ICommandHandler<CustRegisterCommand> _custRegCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<MailLogCommand> _mailLogCommand;
        private readonly ICommandHandler<NotificationCommand> _noticeCommand;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<StatLogCommand> _StatLogCommand;

        public ProcessController(IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<CustRegisterCommand> custRegCommand,
            ICommandHandler<NotificationCommand> noticeCommand,
            ICommandHandler<MailLogCommand> mailLogCommand,
            ICommandHandler<StatLogCommand> StatLogCommand,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
            ICommandHandler<ChangeContactMobileCommand> ChangeContactMobileCommand,
            ICommandHandler<UpdateFileNameCommand> UpdateFileNameCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
            _custRegCommand = custRegCommand;
            _noticeCommand = noticeCommand;
            _mailLogCommand = mailLogCommand;
            _StatLogCommand = StatLogCommand;
            _sendSmsCommand = SendSmsCommand;
            _changeContactMobileCommand = ChangeContactMobileCommand;
            _updateFileNameCommand = UpdateFileNameCommand;
            base._Logger = logger;
        }

        public class File_Remove
        {
            public string file_name { get; set; }
        }

        private string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }



        public ActionResult ValidateCaptcha(string inputUser, string inputHash)
        {
            //var hash = 5381;
            //inputUser = inputUser.ToUpper().Trim() + "978692";
            //for(int i = 0; i < inputUser.Length; i++) {
            //    hash = ((hash << 5) + hash) + inputUser[i];
            //}
            string result = CreateToken(inputUser, "978692");

            if (result == inputHash)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
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

        #region New Copy from WBB Controller 2017/03/23



        private HttpPostedFileBase[] filesPostedRegisterTempStep;
        [HttpPost]
        public ActionResult UploadImage(string cateType, string cardNo, string cardType, string register_dv, string AisAirNumber)
        {
            if (Request.Files.Count == 0)
            {
                return Json(true);
            }
            else if (Request.Files.Count > 0)
            {
                try
                {
                    #region Get IP Address Interface Log (Update 17.2)

                    // Get IP Address
                    string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    #endregion

                    List<string> Arr_files = new List<string>();
                    QuickWinPanelModel model = new QuickWinPanelModel();
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase[] filesPosted = new HttpPostedFileBase[files.Count];

                    for (int i = 0; i < files.Count; i++)
                    {
                        filesPosted[i] = files[i];
                    }
                    model.Register_device = register_dv;
                    model.CustomerRegisterPanelModel.CateType = cateType;
                    model.CustomerRegisterPanelModel.L_CARD_NO = cardNo;
                    model.CustomerRegisterPanelModel.L_CARD_TYPE = cardType;

                    model.CoveragePanelModel.L_CONTACT_PHONE = AisAirNumber;
                    model.ClientIP = ipAddress;

                    filesPostedRegisterTempStep = filesPosted;
                    model = SaveFileImage(filesPosted, model);
                    if (model.CustomerRegisterPanelModel.ListImageFile.Any())
                        return Json(model.CustomerRegisterPanelModel.ListImageFile);
                    else
                        throw new Exception("Null ListImageFile");
                }
                catch (Exception ex)
                {
                    _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                    _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                    return Json(false);
                }
            }
            else
            {
                return Json(false);
            }
        }

        private QuickWinPanelModel SaveFileImage(HttpPostedFileBase[] files, QuickWinPanelModel model)
        {
            string cardNo = model.CustomerRegisterPanelModel.L_CARD_NO;

            InterfaceLogCommand log = null;
            InterfaceLogCommand log2 = null;

            string transactionId = (model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP).ToSafeString();

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
                        model.CustomerRegisterPanelModel.ListImageFile
                                = ConvertHttpPostedFileBaseToUploadImage(files, model, imagepathimer);

                        if (!model.CustomerRegisterPanelModel.ListImageFile.Any())
                        {
                            var base64photoDict = Session["base64photo"] as Dictionary<string, string>;
                            model.CustomerRegisterPanelModel.ListImageFile
                                = ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer);
                        }
                    }
                    else if (model.Register_device == "MOBILE APP")
                    {
                        var base64photoDict = Session["base64photo"] as Dictionary<string, string>;
                        model.CustomerRegisterPanelModel.ListImageFile
                            = ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer);
                    }
                    else
                    {
                        model.CustomerRegisterPanelModel.ListImageFile
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


        private List<UploadImage> ConvertHttpPostedFileBaseToUploadImage(
           HttpPostedFileBase[] files,
           QuickWinPanelModel model,
           string imagepathimer)
        {
            string cardNo = model.CustomerRegisterPanelModel.L_CARD_NO;

            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertHttpPostedFileBaseToUploadImage", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, cardNo, "WEB");
            try
            {
                var width = 750;
                var height = 562;

                if (files.Count() <= 0)
                {
                    _Logger.Info("base64photo not contain valid files");
                    return new List<UploadImage>();
                }

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            var varfileName = Path.GetFileName(files[i].FileName);
                            var p = new UploadImage();
                            p.FileName = varfileName;
                            model.CustomerRegisterPanelModel.ListImageFile.Add(p);
                        }
                    }
                }

                var resultFormat = GetFormatFile(model);
                var tempfile = new List<UploadImage>();
                var j = 0;

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        var path = Path.Combine(imagepathimer, resultFormat[j].file_name);
                        var type = resultFormat[j].file_name.Substring(resultFormat[j].file_name.IndexOf(".") + 1).ToLower();
                        var p2 = new UploadImage();


                        p2.FileName = path;
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
                EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");
                return tempfile;
            }
            catch (Exception ex)
            {
                EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "ERROR", ex.GetErrorMessage());

                _Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                _Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new List<UploadImage>();
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

        private List<UploadImage> ConvertBase64PhotoToUploadImage(
            Dictionary<string, string> base64photoDict,
            QuickWinPanelModel model,
            string imagepathimer)
        {
            if (null == base64photoDict)
            {
                _Logger.Info("base64photo not contain valid files");
                return new List<UploadImage>();
            }

            foreach (var item in base64photoDict)
            {
                var uploadImage = new UploadImage();
                uploadImage.FileName = item.Key + Guid.NewGuid().ToString() + ".jpg";
                model.CustomerRegisterPanelModel.ListImageFile.Add(uploadImage);
            }

            var path = "";
            var resultFormat = GetFormatFile(model);
            var tempfile = new List<UploadImage>();

            var fileIndex = 0;
            foreach (var item in base64photoDict)
            {
                path = Path.Combine(imagepathimer, resultFormat[fileIndex].file_name);
                var uploadImageWithGeneratedName = new UploadImage();
                uploadImageWithGeneratedName.FileName = path;
                tempfile.Add(uploadImageWithGeneratedName);

                var imgBytes = Convert.FromBase64String(base64photoDict[item.Key]);
                System.IO.File.WriteAllBytes(path, imgBytes);
                fileIndex++;
            }

            return tempfile;
        }

        private List<FileFormatModel> GetFormatFile(QuickWinPanelModel model)
        {
            var lang = (SiteSession.CurrentUICulture.IsThaiCulture() ? "THAI" : "ENG");

            var query = new GetFormatFileNameQuery
            {
                language = lang,
                ID_CardType = model.CustomerRegisterPanelModel.L_CARD_TYPE.ToSafeString(),
                ID_CardNo = model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString(),
                ListFilename = model.CustomerRegisterPanelModel.ListImageFile
            };

            var result = _queryProcessor.Execute(query);

            return result;
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

        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo, string INTERFACE_NODE)
        {
            Session["FullUrl"] = this.Url.Action("OnSave", "ResendOrder", null, this.Request.Url.Scheme);
            string FullUrl = "";
            if (Session["FullUrl"] != null)
            {
                FullUrl = Session["FullUrl"].ToSafeString();
                INTERFACE_NODE = INTERFACE_NODE + "|" + FullUrl;
            }
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

        #endregion
    }
}