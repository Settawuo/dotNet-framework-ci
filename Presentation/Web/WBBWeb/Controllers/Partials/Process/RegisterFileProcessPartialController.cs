using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    public partial class ProcessController : WBBController
    {
        public JsonResult GetCheckMode()
        {
            var result = false;
            if (null != Session["base64photo"])
            {
                result = true;
            }

            return Json(new { data = result, }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCheckTakePhoto(string mode)
        {
            var data = "";
            if (null != Session["base64photo"])
            {
                var fileList = Session["base64photo"] as Dictionary<string, string>;
                if (fileList.Count > 0)
                {
                    data = "Y";
                }
            }
            else
            {
                data = "N";
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMode(string mode)
        {
            Session["mode_app"] = mode;
            var data = mode;
            return Json(data, JsonRequestBehavior.AllowGet);
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
                  new File_Remove {file_name="fileG4TakePhoto"},
                  new File_Remove {file_name="fileG5TakePhoto"},
                  new File_Remove {file_name="fileG6TakePhoto"},
                  new File_Remove {file_name="fileB1TakePhoto"},
                  new File_Remove {file_name="fileB2TakePhoto"},
                  new File_Remove {file_name="fileB3TakePhoto"},
                  new File_Remove {file_name="fileB4TakePhoto"},
                  new File_Remove {file_name="fileB5TakePhoto"},
                  new File_Remove {file_name="fileB6TakePhoto"},
                  new File_Remove {file_name="fileB7TakePhoto"}
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
                  new File_Remove {file_name="fileR2TakePhoto"},
                  new File_Remove {file_name="fileR3TakePhoto"},
                  new File_Remove {file_name="fileR4TakePhoto"},
                  new File_Remove {file_name="fileBillTakePhoto"},
                  new File_Remove {file_name="fileB1TakePhoto"},
                  new File_Remove {file_name="fileB2TakePhoto"},
                  new File_Remove {file_name="fileB3TakePhoto"},
                  new File_Remove {file_name="fileB4TakePhoto"},
                  new File_Remove {file_name="fileB5TakePhoto"},
                  new File_Remove {file_name="fileB6TakePhoto"},
                  new File_Remove {file_name="fileB7TakePhoto"}
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
                  new File_Remove {file_name="fileR2TakePhoto"},
                  new File_Remove {file_name="fileR3TakePhoto"},
                  new File_Remove {file_name="fileR4TakePhoto"},
                  new File_Remove {file_name="fileBillTakePhoto"},
                  new File_Remove {file_name="fileG1TakePhoto"},
                  new File_Remove {file_name="fileG2TakePhoto"},
                  new File_Remove {file_name="fileG3TakePhoto"},
                  new File_Remove {file_name="fileG4TakePhoto"},
                  new File_Remove {file_name="fileG5TakePhoto"},
                  new File_Remove {file_name="fileG6TakePhoto"}
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
        [HttpPost]
        public ActionResult RemovePhotoAll(string type)
        {
            if (null != Session["base64photo"])
            {
                var fileList = Session["base64photo"] as Dictionary<string, string>;
                if (fileList != null) fileList.Clear();
            }

            return Json(new { result = true, }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult StoreBase64Photo(string base64photo, string fileType)
        {
            Logger.Info("Start Add Session list :");
            if (null == Session["base64photo"])
            {
                Session["base64photo"] = new Dictionary<string, string>();
            }

            var listOfBase64Photo = Session["base64photo"] as Dictionary<string, string>;

            listOfBase64Photo[fileType] = base64photo;

            foreach (var item in listOfBase64Photo)
            {
                base.Logger.Info(item.Key);
            }
            Logger.Info("End Add Session list :");
            return Json(new { result = true, }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveStoreBase64Photo(string fileType)
        {
            Logger.Info("Start Add Session list :");
            if (null == Session["base64photo"])
            {
                Session["base64photo"] = new Dictionary<string, string>();
            }

            var listOfBase64Photo = Session["base64photo"] as Dictionary<string, string>;

            if (listOfBase64Photo != null)
            {
                listOfBase64Photo.Remove(fileType);
                foreach (var item in listOfBase64Photo)
                {
                    Logger.Info(item.Key);
                }
            }

            Logger.Info("End Add Session list :");
            return Json(new { result = true, }, JsonRequestBehavior.AllowGet);
        }

        private List<UploadImage> ConvertBase64PhotoToUploadImage(
            Dictionary<string, string> base64photoDict,
            QuickWinPanelModel model,
            string imagepathimer)
        {

            string logStep = "Start";
            string cardNo = model.CustomerRegisterPanelModel.L_CARD_NO;

            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertBase64PhotoToUploadImage", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, cardNo, "WEB");

            try
            {

                if (null == base64photoDict)
                {
                    Logger.Info("base64photo not contain valid files");
                    EndInterface("Base64 Null", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");
                    return new List<UploadImage>();
                }

                logStep = "Generate FileName";
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
                    logStep = "add image to tempfile" + fileIndex;
                    path = Path.Combine(imagepathimer, resultFormat[fileIndex].file_name);
                    var uploadImageWithGeneratedName = new UploadImage();
                    uploadImageWithGeneratedName.FileName = path;
                    tempfile.Add(uploadImageWithGeneratedName);

                    var imgBytes = Convert.FromBase64String(base64photoDict[item.Key]);
                    imgBytes = CreateThumbnail(imgBytes, 1920);
                    System.IO.File.WriteAllBytes(path, imgBytes);
                    fileIndex++;
                }

                EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");

                return tempfile;
            }
            catch (Exception ex)
            {
                EndInterface(logStep, log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());

                Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new List<UploadImage>();
            }
        }

        private List<UploadImage> ConvertHttpPostedFileBaseToUploadImage(
            HttpPostedFileBase[] files,
            QuickWinPanelModel model,
            string imagepathimer)
        {

            string logStep = "Start";
            string cardNo = model.CustomerRegisterPanelModel.L_CARD_NO;

            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertHttpPostedFileBaseToUploadImage", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, cardNo, "WEB");
            try
            {
                var width = 750;
                var height = 562;

                if (files.Count() <= 0)
                {
                    Logger.Info("base64photo not contain valid files");
                    EndInterface("FileImage is Null", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");
                    return new List<UploadImage>();
                }

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            logStep = "Generate File Name" + i;
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

                        if (files[i].ContentLength > 2097152 && type != "pdf") //204800
                        {
                            logStep = "Resize" + i;
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
                EndInterface(logStep, log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());

                Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new List<UploadImage>();
            }


        }

        private QuickWinPanelModel SaveFileImage(HttpPostedFileBase[] files, QuickWinPanelModel model)
        {
            string logStep = "Start";
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
                EndInterface("", log2, transactionId, "Success", "");

                imagepathimer = imagepathimerTemp;
                Logger.Info("Start Impersonate:");

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    System.IO.Directory.CreateDirectory(imagepathimer);
                    var base64photoDict = Session["base64photo"] as Dictionary<string, string>;
                    if (string.IsNullOrEmpty(model.Register_device))
                    {
                        logStep = "Unknow Device";
                        model.CustomerRegisterPanelModel.ListImageFile
                                = ConvertHttpPostedFileBaseToUploadImage(files, model, imagepathimer);

                        if (!model.CustomerRegisterPanelModel.ListImageFile.Any())
                        {
                            model.CustomerRegisterPanelModel.ListImageFile
                                = ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer);
                        }
                    }
                    else if (model.Register_device == "MOBILE APP")
                    {
                        logStep = "Mobile App";
                        model.CustomerRegisterPanelModel.ListImageFile
                            = ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer);
                    }
                    else
                    {
                        logStep = "Web Browser";
                        if (base64photoDict.Count > 0)
                        {
                            logStep = "File From Session (Web Browser)";
                            var listUpload = new List<UploadImage>();
                            var fileTemp = files.Where(w => w.ContentType == "application/pdf").ToArray();
                            if (fileTemp.Any())
                            {
                                int i = 0;
                                var fileNotPic = new HttpPostedFileBase[fileTemp.Count()];
                                foreach (var f in fileTemp)
                                {
                                    fileNotPic[i] = f;
                                    i++;
                                }

                                listUpload = ConvertHttpPostedFileBaseToUploadImage(fileNotPic, model, imagepathimer);
                                model.CustomerRegisterPanelModel.ListImageFile = new List<UploadImage>();
                            }

                            listUpload.AddRange(ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer));
                            model.CustomerRegisterPanelModel.ListImageFile = listUpload;
                        }
                        else
                        {
                            logStep = "File From Browse (Web Browser)";
                            model.CustomerRegisterPanelModel.ListImageFile
                            = ConvertHttpPostedFileBaseToUploadImage(files, model, imagepathimer);
                        }
                    }

                    Logger.Info("End Impersonate:");

                    //Session["base64photo"] = null;

                    EndInterface("", log, transactionId, "Success", "");

                    return model;

                }
            }
            catch (Exception ex)
            {
                EndInterface(logStep, log, transactionId, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());

                Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return model;
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

        public static byte[] CreateThumbnail(byte[] PassedImage, int LargestSide)
        {
            byte[] ReturnedThumbnail;

            using (MemoryStream StartMemoryStream = new MemoryStream(),
                                NewMemoryStream = new MemoryStream())
            {
                // write the string to the stream  
                StartMemoryStream.Write(PassedImage, 0, PassedImage.Length);

                // create the start Bitmap from the MemoryStream that contains the image  
                Bitmap startBitmap = new Bitmap(StartMemoryStream);

                // set thumbnail height and width proportional to the original image.  
                int newHeight;
                int newWidth;
                double HW_ratio;
                if (startBitmap.Height > startBitmap.Width)
                {
                    if (startBitmap.Height >= LargestSide)
                    {
                        newHeight = LargestSide;
                        HW_ratio = (double)((double)LargestSide / (double)startBitmap.Height);
                        newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                    }
                    else
                    {
                        newHeight = startBitmap.Height;
                        newWidth = startBitmap.Width;
                    }


                }
                else
                {
                    if (startBitmap.Width >= LargestSide)
                    {
                        newWidth = LargestSide;
                        HW_ratio = (double)((double)LargestSide / (double)startBitmap.Width);
                        newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                    }
                    else
                    {
                        newHeight = startBitmap.Height;
                        newWidth = startBitmap.Width;
                    }
                }

                // create a new Bitmap with dimensions for the thumbnail.  
                Bitmap newBitmap = new Bitmap(newWidth, newHeight);

                // Copy the image from the START Bitmap into the NEW Bitmap.  
                // This will create a thumnail size of the same image.  
                newBitmap = ResizeBitmapImage(startBitmap, newWidth, newHeight);

                // Save this image to the specified stream in the specified format.  
                newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Fill the byte[] for the thumbnail from the new MemoryStream.  
                ReturnedThumbnail = NewMemoryStream.ToArray();
            }

            // return the resized image as a string of bytes.  
            return ReturnedThumbnail;
        }

        // Resize a Bitmap  
        private static Bitmap ResizeBitmapImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new System.Drawing.Rectangle(0, 0, width, height),
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static string GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            int weekNo = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return weekNo.ToString("00");
        }

        [HttpPost]
        public JsonResult GenQrCodeLine(string MobileNo = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            string resultQrCode = "";
            string resultEncode = "";
            string urlEncode = "";
            string linetmpid = "";
            try
            {
                // get config
                GetConfigGenLineQrCodeQuery query = new GetConfigGenLineQrCodeQuery()
                {
                    MobileNo = MobileNo,
                    FullUrl = FullUrl
                };

                var result = _queryProcessor.Execute(query);

                if (result != null && result.ret_code == "0")
                {
                    if (result.payload_linetempid != "" && result.payload_ch != "" && result.verify_signature != "" && result.url != "")
                    {
                        linetmpid = result.payload_linetempid;
                        Dictionary<string, object> payload = new Dictionary<string, object>
                        {
                            { "linetmpid", linetmpid },
                            { "ch", result.payload_ch }
                        };
                        resultEncode = JwtEncode(payload, result.verify_signature);
                        urlEncode = result.url + resultEncode;
                        resultQrCode = GenerateQRCode(urlEncode);
                    }
                }
            }
            catch (Exception)
            { }
            return Json(new { resultQrCode = resultQrCode, linetmpid = linetmpid }, JsonRequestBehavior.AllowGet);
        }
        //R23.05 CheckFraud
        private QuickWinPanelModel SaveFileImageFraud(HttpPostedFileBase[] files, QuickWinPanelModel model)
        {
            string logStep = "Start";
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
                EndInterface("", log2, transactionId, "Success", "");

                imagepathimer = imagepathimerTemp;
                Logger.Info("Start Impersonate:");

                using (var impersonator = new Impersonator(user, ip, pass, false))
                {
                    System.IO.Directory.CreateDirectory(imagepathimer);
                    var base64photoDict = Session["base64photo"] as Dictionary<string, string>;
                    if (string.IsNullOrEmpty(model.Register_device))
                    {
                        logStep = "Unknow Device";
                        model.CustomerRegisterPanelModel.ListImageFile
                                = ConvertHttpPostedFileBaseToUploadImageFraud(files, model, imagepathimer);

                        if (!model.CustomerRegisterPanelModel.ListImageFile.Any())
                        {
                            model.CustomerRegisterPanelModel.ListImageFile
                                = ConvertBase64PhotoToUploadImageFraud(base64photoDict, model, imagepathimer);
                        }
                    }
                    else if (model.Register_device == "MOBILE APP")
                    {
                        logStep = "Mobile App";
                        model.CustomerRegisterPanelModel.ListImageFile
                            = ConvertBase64PhotoToUploadImage(base64photoDict, model, imagepathimer);
                    }
                    else
                    {
                        logStep = "Web Browser";
                        if (base64photoDict.Count > 0)
                        {
                            logStep = "File From Session (Web Browser)";
                            var listUpload = new List<UploadImage>();
                            var fileTemp = files.Where(w => w.ContentType == "application/pdf").ToArray();
                            if (fileTemp.Any())
                            {
                                int i = 0;
                                var fileNotPic = new HttpPostedFileBase[fileTemp.Count()];
                                foreach (var f in fileTemp)
                                {
                                    fileNotPic[i] = f;
                                    i++;
                                }

                                listUpload = ConvertHttpPostedFileBaseToUploadImageFraud(fileNotPic, model, imagepathimer);
                                model.CustomerRegisterPanelModel.ListImageFile = new List<UploadImage>();
                            }

                            listUpload.AddRange(ConvertBase64PhotoToUploadImageFraud(base64photoDict, model, imagepathimer));
                            model.CustomerRegisterPanelModel.ListImageFile = listUpload;
                        }
                        else
                        {
                            logStep = "File From Browse (Web Browser)";
                            model.CustomerRegisterPanelModel.ListImageFile
                            = ConvertHttpPostedFileBaseToUploadImageFraud(files, model, imagepathimer);
                        }
                    }

                    Logger.Info("End Impersonate:");

                    //Session["base64photo"] = null;

                    EndInterface("", log, transactionId, "Success", "");

                    return model;

                }
            }
            catch (Exception ex)
            {
                EndInterface(logStep, log, transactionId, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());

                Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return model;
            }
        }
        //R23.05 CheckFraud
        private List<UploadImage> ConvertBase64PhotoToUploadImageFraud(Dictionary<string, string> base64photoDict,QuickWinPanelModel model,string imagepathimer)
        {
            string logStep = "Start";
            string cardNo = model.CustomerRegisterPanelModel.L_CARD_NO;

            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertBase64PhotoToUploadImage", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, cardNo, "WEB");

            try
            {

                if (null == base64photoDict)
                {
                    Logger.Info("base64photo not contain valid files");
                    EndInterface("Base64 Null", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");
                    return new List<UploadImage>();
                }

                logStep = "Generate FileName";
                foreach (var item in base64photoDict)
                {
                    var uploadImage = new UploadImage();
                    uploadImage.FileName = item.Key + Guid.NewGuid().ToString() + ".jpg";
                    model.CustomerRegisterPanelModel.ListImageFile.Add(uploadImage);
                }

                var path = "";
                var resultFormat = GetFormatFileFraud(model);
                var tempfile = new List<UploadImage>();

                foreach (var item in base64photoDict)
                {
                    logStep = "add image to tempfile";
                    path = Path.Combine(imagepathimer, resultFormat.file_name);
                    var uploadImageWithGeneratedName = new UploadImage();
                    uploadImageWithGeneratedName.FileName = path;
                    tempfile.Add(uploadImageWithGeneratedName);

                    var imgBytes = Convert.FromBase64String(base64photoDict[item.Key]);
                    imgBytes = CreateThumbnail(imgBytes, 1920);
                    System.IO.File.WriteAllBytes(path, imgBytes);
                }

                EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");

                return tempfile;
            }
            catch (Exception ex)
            {
                EndInterface(logStep, log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());

                Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new List<UploadImage>();
            }
        }
        private List<UploadImage> ConvertHttpPostedFileBaseToUploadImageFraud(HttpPostedFileBase[] files,QuickWinPanelModel model,string imagepathimer)
        {

            string logStep = "Start";
            string cardNo = model.CustomerRegisterPanelModel.L_CARD_NO;

            InterfaceLogCommand log = null;
            log = StartInterface("IdcardNo:" + cardNo + "\r\n", "ConvertHttpPostedFileBaseToUploadImage", model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, cardNo, "WEB");
            try
            {
                var width = 750;
                var height = 562;

                if (files.Count() <= 0)
                {
                    Logger.Info("base64photo not contain valid files");
                    EndInterface("FileImage is Null", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");
                    return new List<UploadImage>();
                }

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            logStep = "Generate File Name" + i;
                            var varfileName = Path.GetFileName(files[i].FileName);
                            var p = new UploadImage();
                            p.FileName = varfileName;
                            model.CustomerRegisterPanelModel.ListImageFile.Add(p);
                        }
                    }
                }

                var resultFormat = GetFormatFileFraud(model);
                var tempfile = new List<UploadImage>();

                for (var i = 0; i < files.Count(); i++)
                {
                    if (files[i] != null)
                    {
                        var path = Path.Combine(imagepathimer, resultFormat.file_name);
                        var type = resultFormat.file_name.Substring(resultFormat.file_name.IndexOf(".") + 1).ToLower();
                        var p2 = new UploadImage();


                        p2.FileName = path;
                        tempfile.Add(p2);

                        if (files[i].ContentLength > 2097152 && type != "pdf") //204800
                        {
                            logStep = "Resize" + i;
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
                    }
                }
                EndInterface("", log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "Success", "");
                return tempfile;
            }
            catch (Exception ex)
            {
                EndInterface(logStep, log, model.CoveragePanelModel.L_CONTACT_PHONE + model.ClientIP, "ERROR", "Error Message: " + ex.GetErrorMessage() + "\r\nStack Trace: " + ex.RenderExceptionMessage());

                Logger.Info("Error Upload Image:" + ex.GetErrorMessage());
                Logger.Info("Error Upload Image With Stack Trace : " + ex.RenderExceptionMessage());
                return new List<UploadImage>();
            }


        }

    }
}