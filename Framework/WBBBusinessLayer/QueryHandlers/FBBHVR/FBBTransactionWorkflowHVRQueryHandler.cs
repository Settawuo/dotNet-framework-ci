using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WBBContract;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    class FBBTransactionWorkflowHVRQueryHandler : IQueryHandler<FBBTransactionWorkflowHVRQuery, FBBTransactionWorkflowModel>
    {

        private readonly ILogger logger;
        private readonly IFBBHVREntityRepository<object> objSubHVR;
        private readonly IEntityRepository<ConfigurationFileQueryModel> repositoryConfigurationFileQueryModel;//WBB.
        private readonly IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV;//Get LOV.
        private string packageName_Reg = "air_admin.b_transaction_workflow_rt_get_register_transaction_list";
        private string packageName_Comp = "air_admin.b_transaction_workflow_rt_get_complete_transaction_list";
        private string packageName_Canc = "air_admin.b_transaction_workflow_rt_get_cancel_transaction_list_v8";
        private string zipFileName = "TransactionWorkflow";
        private string appendLog = "";

        public FBBTransactionWorkflowHVRQueryHandler(ILogger logger, IFBBHVREntityRepository<object> objSubHVR,
            IEntityRepository<ConfigurationFileQueryModel> repositoryConfigurationFileQueryModel,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            this.logger = logger;
            this.objSubHVR = objSubHVR;
            this.repositoryConfigurationFileQueryModel = repositoryConfigurationFileQueryModel;
            this.FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public FBBTransactionWorkflowModel Handle(FBBTransactionWorkflowHVRQuery query)
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
                var ret_code = new NpgsqlParameter();
                ret_code.ParameterName = "return_code_cur";
                ret_code.NpgsqlDbType = NpgsqlDbType.Refcursor;
                ret_code.Direction = ParameterDirection.InputOutput;
                ret_code.Value = "return_code_cur";

                var register_transaction_cur = new NpgsqlParameter();
                register_transaction_cur.ParameterName = "register_transaction_cur";
                register_transaction_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
                register_transaction_cur.Direction = ParameterDirection.InputOutput;
                register_transaction_cur.Value = "register_transaction_cur";

                logger.Info("START CALL PACKAGE Get_Register_Transaction_list.");

                var excute = objSubHVR.ExecuteStoredProcMultipleCursorNpgsql(packageName_Reg,
               new object[]
               {
                        //retrun
                        ret_code ,
                        register_transaction_cur
               }).ToList();

                //chist571 11/01/2024
                string headers = string.Empty;
                List<string> Convertexcute = new List<string>();
                if (excute != null && excute.Count > 0)
                {
                    DataTable excute_ret_code = (DataTable)excute[0];
                    foreach (DataRow item in excute_ret_code.Rows)
                    {
                        retModel.ReturnCode = item.ItemArray[0].ToString();
                        retModel.ReturnMessage = item.ItemArray[1].ToString();
                        headers += item.ItemArray[2].ToString();
                        
                    }

                    var indexRow = 0;
                    var excuteResult = (DataTable)excute[1];
                    foreach (DataRow item in excuteResult.Rows)
                    {
                        Convertexcute.Add(item.ItemArray[indexRow].ToString());
                        indexRow++;
                    }
                }
                
                string fileName = lovName + DateTime.Now.ToString("yyyyMMdd");
                var result = FileHelper.WriteTextfileListString(username, pwd, domain, headers, Convertexcute,
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
                var ret_code = new NpgsqlParameter();
                ret_code.ParameterName = "return_code_cur";
                ret_code.NpgsqlDbType = NpgsqlDbType.Refcursor;
                ret_code.Direction = ParameterDirection.InputOutput;
                ret_code.Value = "return_code_cur";

                var complete_transaction_cur = new NpgsqlParameter();
                complete_transaction_cur.ParameterName = "complete_transaction_cur";
                complete_transaction_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
                complete_transaction_cur.Direction = ParameterDirection.InputOutput;
                complete_transaction_cur.Value = "complete_transaction_cur";

                logger.Info("START CALL PACKAGE Get_Complete_Transaction_list.");

                var excute = objSubHVR.ExecuteStoredProcMultipleCursorNpgsql(packageName_Comp,
                new object[]
                {
                        //retrun
                        ret_code ,
                        complete_transaction_cur
                }).ToList();

                //chist571 11/01/2024
                string headers = string.Empty;
                List<string> Convertexcute = new List<string>();
                if (excute != null && excute.Count > 0)
                {
                    DataTable excute_ret_code = (DataTable)excute[0];
                    foreach (DataRow item in excute_ret_code.Rows)
                    {
                        retModel.ReturnCode = item.ItemArray[0].ToString();
                        retModel.ReturnMessage = item.ItemArray[1].ToString();
                        headers += item.ItemArray[2].ToString();

                    }

                    var indexRow = 0;
                    var excuteResult = (DataTable)excute[1];
                    foreach (DataRow item in excuteResult.Rows)
                    {
                        Convertexcute.Add(item.ItemArray[indexRow].ToString());
                        indexRow++;
                    }
                }

                string fileName = lovName + DateTime.Now.ToString("yyyyMMdd");
                var result = FileHelper.WriteTextfileListString(username, pwd, domain, headers, Convertexcute,
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
                var return_code_cur = new NpgsqlParameter();
                return_code_cur.ParameterName = "return_code_cur";
                return_code_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
                return_code_cur.Direction = ParameterDirection.InputOutput;
                return_code_cur.Value = "return_code_cur";

                var cancel_transaction_cur = new NpgsqlParameter();
                cancel_transaction_cur.ParameterName = "cancel_transaction_cur";
                cancel_transaction_cur.NpgsqlDbType = NpgsqlDbType.Refcursor;
                cancel_transaction_cur.Direction = ParameterDirection.InputOutput;
                cancel_transaction_cur.Value = "cancel_transaction_cur";

                logger.Info("START CALL PACKAGE Get_Cancel_Transaction_list.");

                var excute = objSubHVR.ExecuteStoredProcMultipleCursorNpgsql(packageName_Canc,
                     new object[]
                     {
                        //retrun
                        return_code_cur,
                        cancel_transaction_cur
                     });

                //chist571 11/01/2024
                string headers = string.Empty;
                List<string> Convertexcute = new List<string>();
                if (excute != null && excute.Count > 0)
                {
                    DataTable excute_ret_code = (DataTable)excute[0];
                    foreach (DataRow item in excute_ret_code.Rows)
                    {
                        retModel.ReturnCode = item.ItemArray[0].ToString();
                        retModel.ReturnMessage = item.ItemArray[1].ToString();
                        headers += item.ItemArray[2].ToString();

                    }

                    var indexRow = 0;
                    var excuteResult = (DataTable)excute[1];
                    foreach (DataRow item in excuteResult.Rows)
                    {
                        Convertexcute.Add(item.ItemArray[indexRow].ToString());
                        indexRow++;
                    }
                }
                string fileName = lovName + DateTime.Now.ToString("yyyyMMdd");
                var result = FileHelper.WriteTextfileListString(username, pwd, domain, headers, Convertexcute,
                    domainPath, fileName, Encoding.GetEncoding(874));
            }
            catch (Exception err)
            {

                logger.Error("Error : " + err.Message);
            }
            logger.Info("END CALL PACKAGE Get_Cancel_Transaction_list.");

            return retModel;
        }
    }
}
