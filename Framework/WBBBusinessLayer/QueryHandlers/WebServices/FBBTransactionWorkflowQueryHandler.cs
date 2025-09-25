using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    class FBBTransactionWorkflowQueryHandler : IQueryHandler<FBBTransactionWorkflowQuery, FBBTransactionWorkflowModel>
    {

        private readonly ILogger logger;
        private readonly FBBShareplexEntityRepository<String> objSubJ;//sharePlex Package.
        private readonly IEntityRepository<ConfigurationFileQueryModel> repositoryConfigurationFileQueryModel;//WBB.
        private readonly IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV;//Get LOV.
        //private string packageName_Reg = "AIR_ADMIN.B_TRANSACTION_WORKFLOW_RT.Get_Register_Transaction_list";
        //private string packageName_Comp = "AIR_ADMIN.B_TRANSACTION_WORKFLOW_RT.Get_Complete_Transaction_list";
        //private string packageName_Canc = "AIR_ADMIN.B_TRANSACTION_WORKFLOW_RT.Get_Cancel_Transaction_list";
        private string packageName_Reg = "fbbadm.PKG_WF_Transaction_Report.Get_Register_Transaction_list";
        private string packageName_Comp = "fbbadm.PKG_WF_Transaction_Report.Get_Complete_Transaction_list";
        private string packageName_Canc = "fbbadm.PKG_WF_Transaction_Report.Get_Cancel_Transaction_list";
        private string zipFileName = "TransactionWorkflow";
        private string appendLog = "";

        public FBBTransactionWorkflowQueryHandler(ILogger logger, FBBShareplexEntityRepository<String> objSubJ,
            IEntityRepository<ConfigurationFileQueryModel> repositoryConfigurationFileQueryModel,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            this.logger = logger;
            this.objSubJ = objSubJ;
            this.repositoryConfigurationFileQueryModel = repositoryConfigurationFileQueryModel;
            this.FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public FBBTransactionWorkflowModel Handle(FBBTransactionWorkflowQuery query)
        {
            FBBTransactionWorkflowModel Oo = new FBBTransactionWorkflowModel();
            try
            {
                //var stringQuery = string.Format("SELECT LOV_NAME, DISPLAY_VAL FROM WBB.FBB_CFG_LOV WHERE LOV_TYPE = 'TRANSACTION_WORKFLOW_RT'");
                //var configurationFileModel = repositoryConfigurationFileQueryModel.SqlQuery(stringQuery).ToList();
                var strQuery = (from A in FBB_CFG_LOV.Get()
                                where A.LOV_TYPE == "TRANSACTION_WORKFLOW_RT"
                                select new LOVWorkflow
                                {
                                    LOV_NAME = A.LOV_NAME,
                                    DISPLAY_VAL = A.DISPLAY_VAL
                                }).ToList();
                logger.Info("Query LOV.NAME-LOV.DISPLAY Success.");
                appendLog += "Query LOV.NAME-LOV.DISPLAY Success.|| ";

                if (strQuery.Any())
                {
                    //#Get All file Path Domain.
                    var getFileName = FileHelper.GetAllFile(query.UserHost
                                                                , query.PassHost
                                                                , query.DomainHost
                                                                , query.TargetDomainPath);
                    logger.Info("Initial GetFilePath Success.");
                    appendLog += "Initial GetFilePath Success.|| ";
                    //#Check Emptry Data?. 
                    if (getFileName.Any())
                    {
                        //#intial Create FileInfo.
                        List<FileInfo> listfileName = new List<FileInfo>();
                        var zipFullFilename = string.Format("{0}{1}", query.TargetArchivePath,
                            string.Format(@"{0}_{1}{2}", zipFileName, DateTime.Now.ToString("yyyyMMddHHmmss"), ".zip"));
                        foreach (var configFileModel in strQuery)
                        {
                            foreach (var item in getFileName)
                            {
                                if (item.IndexOf(configFileModel.DISPLAY_VAL) >= 0)
                                {
                                    listfileName.Add(new FileInfo(item));
                                }

                            }
                        }

                        //#File filter LOVDisplay.txt All In FileInfo?.
                        if (listfileName.Any())
                        {
                            //#Insert fileZip to Path Archive.
                            var isCopyFileZips = FileHelper.ZipMultipleFile(query.UserHost
                                                                        , query.PassHost
                                                                        , query.DomainHost
                                                                        , zipFullFilename
                                                                        , query.TargetDomainPath
                                                                        , listfileName);
                            logger.Info("Initial ZipFile Success.");
                            appendLog += "Initial ZipFile Success.|| ";
                            if (!isCopyFileZips)
                            {
                                throw new Exception("cannot authentication user/password zip path file.");
                            }
                            else
                            {
                                //#Remove FileInfo to Path After 'Zip' Already.
                                foreach (var item in listfileName)
                                {
                                    var removeFullPath = string.Format("{0}{1}", query.TargetDomainPath, item.Name);
                                    var isRemoveFileZip = FileHelper.RemoveFile(query.UserHost
                                                                                  , query.PassHost
                                                                                  , query.DomainHost
                                                                                  , query.TargetDomainPath
                                                                                  , item.Name);
                                    if (!isRemoveFileZip)
                                    {
                                        throw new Exception("cannot authentication user/password zip path file.");
                                    }
                                }
                            }//#isCopyFileZips(Condition)

                        }//#listfileName(Condition)

                    }//#getFileName(Condition)

                    //#Insert Data.Txt to Path Domain.
                    foreach (var configFileModel in strQuery)
                    {
                        if (configFileModel.LOV_NAME == "FBSS_REGISTER")
                        {
                            registerTransactionReport(query.UserHost, query.PassHost, query.DomainHost, query.TargetDomainPath, configFileModel.DISPLAY_VAL.ToString());
                            logger.Info("Create File FBSS_REGISTER Success.");
                            appendLog += "Create File FBSS_REGISTER Success.|| ";
                        }
                        else if (configFileModel.LOV_NAME == "FBSS_COMPLETE")
                        {
                            completeTransactionReport(query.UserHost, query.PassHost, query.DomainHost, query.TargetDomainPath, configFileModel.DISPLAY_VAL.ToString());
                            logger.Info("Create File FBSS_COMPLETE Success.");
                            appendLog += "Create File FBSS_COMPLETE Success.|| ";
                        }
                        else if (configFileModel.LOV_NAME == "FBSS_CANCEL")
                        {
                            cancelTransactionReport(query.UserHost, query.PassHost, query.DomainHost, query.TargetDomainPath, configFileModel.DISPLAY_VAL.ToString());
                            logger.Info("Create File FBSS_CANCEL Success.");
                            appendLog += "Create File FBSS_CANCEL Success.|| ";
                        }
                    }

                    Oo.ReturnCode = "0";
                    Oo.ReturnMessage = "\n" + appendLog;
                    return Oo;
                }//#configurationFileModel(Condition)

                Oo.ReturnCode = "-1";
                Oo.ReturnMessage = "failed : null configuration.";
                return Oo;
            }
            catch (Exception err)
            {
                logger.Error(err.Message);
                Oo.ReturnCode = "-1";
                Oo.ReturnMessage = err.Message + " " + appendLog;
                return Oo;
            }
        }

        public FBBTransactionWorkflowModel registerTransactionReport(string username, string pwd, string domain, string domainPath, string lovName)
        {
            FBBTransactionWorkflowModel retModel = new FBBTransactionWorkflowModel();
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "return_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "return_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var ret_header = new OracleParameter();
                ret_header.ParameterName = "return_header";
                ret_header.Size = 2000;
                ret_header.OracleDbType = OracleDbType.Varchar2;
                ret_header.Direction = ParameterDirection.Output;

                var ret_reg_transaction_cur = new OracleParameter();
                ret_reg_transaction_cur.ParameterName = "register_transaction_cur";
                ret_reg_transaction_cur.OracleDbType = OracleDbType.RefCursor;
                ret_reg_transaction_cur.Direction = ParameterDirection.Output;

                logger.Info("START CALL PACKAGE Get_Register_Transaction_list.");
                List<String> excute =
                    objSubJ.ExecuteReadStoredProc(packageName_Reg,
                    new
                    {
                        ret_code,
                        ret_message,
                        ret_header,
                        ret_reg_transaction_cur
                    }).ToList();


                retModel.ReturnCode = ret_code.Value.ToString();
                retModel.ReturnMessage = ret_message.Value.ToString();
                //initialTextUnix(ret_header.Value.ToString(), excute, "", "UnixTxtTest");
                //"nast_wibb", "nas1004ais", "CORP-AIS900"
                string fileName = lovName + DateTime.Now.ToString("yyyyMMdd");
                var result = FileHelper.WriteTextfileListString(username, pwd, domain, ret_header.Value.ToString(), excute,
                    domainPath, fileName, Encoding.GetEncoding(874));
            }
            catch (Exception err)
            {

                logger.Error("Error : " + err.Message);
            }
            logger.Info("END CALL PACKAGE Get_Register_Transaction_list.");

            return retModel;
        }

        public FBBTransactionWorkflowModel completeTransactionReport(string username, string pwd, string domain, string domainPath, string lovName)
        {
            FBBTransactionWorkflowModel retModel = new FBBTransactionWorkflowModel();
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "return_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "return_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var ret_header = new OracleParameter();
                ret_header.ParameterName = "return_header";
                ret_header.Size = 2000;
                ret_header.OracleDbType = OracleDbType.Varchar2;
                ret_header.Direction = ParameterDirection.Output;

                var ret_reg_transaction_cur = new OracleParameter();
                ret_reg_transaction_cur.ParameterName = "complete_transaction_cur";
                ret_reg_transaction_cur.OracleDbType = OracleDbType.RefCursor;
                ret_reg_transaction_cur.Direction = ParameterDirection.Output;

                logger.Info("START CALL PACKAGE Get_Complete_Transaction_list.");
                List<String> excute =
                    objSubJ.ExecuteReadStoredProc(packageName_Comp,
                    new
                    {
                        ret_code,
                        ret_message,
                        ret_header,
                        ret_reg_transaction_cur
                    }).ToList();


                retModel.ReturnCode = ret_code.Value.ToString();
                retModel.ReturnMessage = ret_message.Value.ToString();
                //initialTextUnix(ret_header.Value.ToString(), excute, "", "UnixTxtTest");
                //"nast_wibb", "nas1004ais", "CORP-AIS900"
                string fileName = lovName + DateTime.Now.ToString("yyyyMMdd");
                var result = FileHelper.WriteTextfileListString(username, pwd, domain, ret_header.Value.ToString(), excute,
                    domainPath, fileName, Encoding.GetEncoding(874));
            }
            catch (Exception err)
            {

                logger.Error("Error : " + err.Message);
            }
            logger.Info("END CALL PACKAGE Get_Complete_Transaction_list.");

            return retModel;
        }

        public class LOVWorkflow
        {
            public string LOV_NAME { get; set; }
            public string DISPLAY_VAL { get; set; }
        }

        public FBBTransactionWorkflowModel cancelTransactionReport(string username, string pwd, string domain, string domainPath, string lovName)
        {
            FBBTransactionWorkflowModel retModel = new FBBTransactionWorkflowModel();
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "return_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "return_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var ret_header = new OracleParameter();
                ret_header.ParameterName = "return_header";
                ret_header.Size = 2000;
                ret_header.OracleDbType = OracleDbType.Varchar2;
                ret_header.Direction = ParameterDirection.Output;

                var ret_reg_transaction_cur = new OracleParameter();
                ret_reg_transaction_cur.ParameterName = "cancel_transaction_cur";
                ret_reg_transaction_cur.OracleDbType = OracleDbType.RefCursor;
                ret_reg_transaction_cur.Direction = ParameterDirection.Output;

                logger.Info("START CALL PACKAGE Get_Cancel_Transaction_list.");
                List<String> excute =
                    objSubJ.ExecuteReadStoredProc(packageName_Canc,
                    new
                    {
                        ret_code,
                        ret_message,
                        ret_header,
                        ret_reg_transaction_cur
                    }).ToList();


                retModel.ReturnCode = ret_code.Value.ToString();
                retModel.ReturnMessage = ret_message.Value.ToString();
                //initialTextUnix(ret_header.Value.ToString(), excute, "", "UnixTxtTest");
                //"nast_wibb", "nas1004ais", "CORP-AIS900"
                string fileName = lovName + DateTime.Now.ToString("yyyyMMdd");
                var result = FileHelper.WriteTextfileListString(username, pwd, domain, ret_header.Value.ToString(), excute,
                    domainPath, fileName, Encoding.GetEncoding(874));
            }
            catch (Exception err)
            {

                logger.Error("Error : " + err.Message);
            }
            logger.Info("END CALL PACKAGE Get_Cancel_Transaction_list.");

            return retModel;
        }

        //public void initialTextUnix(string header, List<String> strDataList, string pathFileName, string fileName)
        //{

        //    string filepath = @"D:\";
        //    filepath += pathFileName + fileName + ".txt";
        //    if (File.Exists(filepath))
        //        File.Delete(filepath);
        //    //==========================================
        //    FileStream fs = new FileStream(filepath,
        //        FileMode.Create,
        //        FileAccess.Write, FileShare.None);
        //    //==========================================

        //    using (TextWriter objFile = new StreamWriter(fs, Encoding.GetEncoding(874)))//ANSI/OEM Thai (ISO 8859-11); Thai (Windows)
        //    {
        //        if (!string.IsNullOrEmpty(header))
        //            objFile.WriteLine(header);

        //        foreach (string val in strDataList)
        //        {
        //            objFile.WriteLine(val);
        //        }
        //    }
        //}

    }
}
