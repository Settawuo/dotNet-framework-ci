using System;
using WBBEntity.Extensions;
//using ReconcileStandardAddress.Security;

namespace ReconcileStandardAddress
{
    using ReconcileStandardAddress.CompositionRoot;

    class Program
    {
        private static string _errorMessage = "";
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var temp = Bootstrapper.GetInstance<ReconcileStandardAddressJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                //Call reconcile.
                temp.ReconcileStandardAddress();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At ReconcileStandardAddress");
                logger.Info(_errorMessage);
            }
        }


        #region Test code
        //private static string _filePath = "";
        //private static string _errorMessage = "";
        //private static string proName = "PKG_FBB_RECONCILE_STD_ADDRESS";
        //private static string proError = "SEND_EMAIL_ERROR_CASE";
        //static void Main(string[] args)
        //{
        //    Bootstrapper.Bootstrap();

        //    var temp = Bootstrapper.GetInstance<ReconcileStandardAddressJob>();
        //    var logger = Bootstrapper.GetInstance<DebugLogger>();

        //    try
        //    {
        //        //testcode
        //        //temp._subjectFiles = new string[] { "Reconcile Standard Address" };
        //        //temp._attachFiles = null;
        //        //temp.Sendmail("Error", "Error When ReconcileStandardAddress Handler : Test code", proError, proName);
        //        //return;

        //        //Call reconcile.
        //        //_reconcileSuccess = temp.ReconcileStandardAddress();

        //        #region temp code
        //        //get nas path.
        //        //var nasPathValue = temp.GetLovList("FBB_CONSTANT", "LOAD_SUBCONTRACTOR_TIMESLOT").Where(t => t.Text == "INPUT_PATH")
        //        //                        .Select(t => t).FirstOrDefault();

        //        //Prepare attach file 
        //        //temp.PrepareAttachFile(nasPathValue.DefaultValue);
        //        #endregion

        //        //get nas path.
        //        var ImpersonateVar = temp
        //                                .GetLovList("FBB_CONSTANT", proName).Where(t => t.Text == "INPUT_PATH")
        //                                .Select(t => t).FirstOrDefault();                

        //        var AttachFile = temp
        //                                .GetLovList("FBB_CONSTANT", proName).Where(t => t.Text == "EMAIL_ALERT")
        //                                .Select(t => t).FirstOrDefault();  

        //        if (ImpersonateVar == null || ImpersonateVar.LovValue4.ToSafeString() == "")
        //        {
        //            logger.Info("Path not found.");
        //            temp.Sendmail("Error", "Path not found.", proError, proName);
        //        }
        //        else
        //        {
        //            var imagepathimer = @ImpersonateVar.LovValue4;
        //            var user = ImpersonateVar.LovValue1;
        //            var pass = ImpersonateVar.LovValue2;
        //            var ip = ImpersonateVar.LovValue3;

        //            #region temp path for test
        //            //var imagepathimer = @"\\10.252.160.97\VirtualNAS";
        //            //string user = "administrator";
        //            //string pass = "P@ssw0rd";
        //            //string ip = "10.252.160.97";
        //            //var zteExportPath = @"\ExportTimeslot_zte";

        //            //var imagepathimer = @"\\10.252.167.22\FBSS_NDEV001B";
        //            //string user = @"nas_fbbweb";
        //            //string pass = "Fbb1013@Ais";
        //            //string ip = "10.252.167.22";
        //            //var zteExportPath = @"\ExportTimeslot_zte";

        //            //var imagepathimer = @"\\10.235.152.11\fbss_nlog801a";
        //            //string user = "nas_fbbweb";
        //            //string pass = "Fbb1013@Ais";
        //            //string ip = "10.235.152.11";
        //            //var zteExportPath = @"\ExportTimeslot_zte";
        //            #endregion

        //            logger.Info("Path : " + imagepathimer);
        //            logger.Info("User : " + user);
        //            logger.Info("Pass : " + pass);
        //            logger.Info("Ip : " + ip);

        //            using (var unc = new UNCAccessWithCredentials())
        //            {
        //                if (unc.NetUseWithCredentials(imagepathimer, user, ip, pass))
        //                {
        //                    var name = "";

        //                    if (unc.NetUseGetUserInfo(ref name))
        //                    {
        //                        logger.Info("After Impersonated : " + name);
        //                    }
        //                    else
        //                    {
        //                        logger.Info("Error When Get Impersonated User : " + unc.LastError);
        //                    }

        //                    logger.Info("Accessing Path : " + imagepathimer);

        //                    _filePath = imagepathimer;

        //                    if (!Directory.Exists(_filePath))
        //                        logger.Info("Path not Exsits.");
        //                    else
        //                        logger.Info("Path Exsits");

        //                    var Datenow = DateTime.Now.ToString("yyyyMMdd");

        //                    if (_reconcileSuccess)
        //                    {                                
        //                        logger.Info("Reconcile Standard Address Data Date : " + Datenow);

        //                        var fileName = (AttachFile == null || AttachFile.LovValue3.ToSafeString() == "") ? AttachFile.LovValue3 : "FBB_RECONCILE_STD_ADDRESS_{0}.txt";
        //                        var fileList = (from file in Directory.EnumerateFiles(_filePath, string.Format(fileName, Datenow)) select file);
        //                        var tmpAttachFile = string.Join(", ", fileList);

        //                        logger.Info("Reconcile Standard Address File(s) : " + tmpAttachFile);

        //                        temp._attachFiles = tmpAttachFile.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //                        //send mail.
        //                        temp._subjectFiles = new string[] { Datenow };
        //                        temp.Sendmail("Success", "", proName, proName);

        //                        // delete old file first monday.
        //                        temp.DeleteOldData(_filePath, string.Format(fileName, "*"));
        //                    }
        //                    else
        //                    {
        //                        temp._subjectFiles = new string[] { Datenow };
        //                        temp.Sendmail("Error", "", proName, proName);
        //                    }

        //                }
        //                else
        //                {
        //                    logger.Info("Imperosnate Error : " + unc.LastError);
        //                    temp._attachFiles = null;
        //                    temp.Sendmail("Error", "Imperosnate Error : " + unc.LastError, proError, proName);
        //                }
        //            }
        //        }                
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorMessage = ex.RenderExceptionMessage();
        //        logger.Info("Error At ReconcileStandardAddress");
        //        logger.Info(_errorMessage);
        //        temp._attachFiles = null;
        //        temp.Sendmail("Error", "Error When ReconcileStandardAddress Handler : " + _errorMessage, proError, proName);
        //    }
        //}
        #endregion Test code
    }
}
