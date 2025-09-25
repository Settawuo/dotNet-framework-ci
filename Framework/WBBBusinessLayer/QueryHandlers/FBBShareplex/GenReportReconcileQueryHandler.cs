using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBShareplex;
using WBBData.Repository;
using WBBEntity.PanelModels.ShareplexModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.FBBShareplex
{
    public class GenReportReconcileQueryHandler : IQueryHandler<GenReportReconcileQuery, GenReportReconcileModel>
    {
        private readonly IEntityRepository<DataTable> _repositoryDataTable;
        private readonly IFBBShareplexEntityRepository<ConfigurationFileQueryModel> _repositoryConfigurationFileQueryModel;

        public GenReportReconcileQueryHandler(IEntityRepository<DataTable> repositoryDataTable, IFBBShareplexEntityRepository<ConfigurationFileQueryModel> repositoryConfigurationFileQueryModel)
        {
            _repositoryDataTable = repositoryDataTable;
            _repositoryConfigurationFileQueryModel = repositoryConfigurationFileQueryModel;
        }

        public GenReportReconcileModel Handle(GenReportReconcileQuery query)
        {
            var returnGenReportReconcileModel = new GenReportReconcileModel();
            var zipFileName = string.Empty;
            var fileName = string.Empty;

            try
            {
                var stringQuery = string.Format("SELECT LOV_NAME, DISPLAY_VAL, LOV_VAL1 FROM WBB.FBB_CFG_LOV WHERE LOV_TYPE = 'GEN_REPORT_RECONCILE_SUPER_MESH'");

                var configurationFileModel = _repositoryConfigurationFileQueryModel.SqlQuery(stringQuery).ToList();

                if (configurationFileModel.Any())
                {


                    var getFileName = FileHelper.GetAllFile(query.UserTempFile
                                                            , query.PassTempFile
                                                            , query.DomainTempFile
                                                            , query.PathTempFile);

                    if (getFileName.Any())
                    {
                        List<FileInfo> listfileName = new List<FileInfo>();
                        var zipFilename = string.Format(@"{0}_{1}{2}", "Report_Reconcile_zip", DateTime.Now.ToString("yyyyMMddHHmmss"), ".zip");
                        var zipFullFilename = string.Format("{0}{1}", query.TargetArchivePathFile, zipFilename);
                        foreach (var configFileModel in configurationFileModel)
                        {
                            foreach (var item in getFileName)
                            {
                                if (item.IndexOf(configFileModel.LOV_NAME) >= 0)
                                {
                                    listfileName.Add(new FileInfo(item));
                                }

                            }
                        }

                        if (listfileName.Any())
                        {
                            var isCopyFileZips = FileHelper.ZipMultipleFile(query.UserTempFile
                                                                    , query.PassTempFile
                                                                    , query.DomainTempFile
                                                                    , zipFullFilename
                                                                    , query.PathTempFile
                                                                    , listfileName);

                            if (!isCopyFileZips)
                            {
                                throw new Exception("cannot authentication user/password zip path file.");
                            }

                            if (isCopyFileZips)
                            {
                                foreach (var item in listfileName)
                                {
                                    var removeFullPath = string.Format("{0}{1}", query.PathTempFile, item.Name);
                                    var isRemoveFileZip = FileHelper.RemoveFile(query.UserTempFile
                                                                                  , query.PassTempFile
                                                                                  , query.DomainTempFile
                                                                                  , query.PathTempFile
                                                                                  , item.Name);
                                    if (!isRemoveFileZip)
                                    {
                                        throw new Exception("cannot authentication user/password zip path file.");
                                    }
                                }

                            }
                        }

                    }

                    foreach (var configFileModel in configurationFileModel)
                    {
                        try
                        {
                            zipFileName = string.Empty;
                            fileName = configFileModel.LOV_NAME;

                            var dataTable = _repositoryConfigurationFileQueryModel.ExecuteToDataTable(configFileModel.LOV_VAL1.ToString().Trim(), configFileModel.LOV_NAME);
                            var filename = string.Format(@"{0}_{1}{2}", configFileModel.LOV_NAME, DateTime.Now.ToString("yyyyMMdd"), ".xlsx");
                            var fullFilename = string.Format("{0}{1}", query.PathTempFile, filename);
                            var newFile = new FileInfo(fullFilename);

                            using (var package = new ExcelPackage(newFile))
                            {
                                var worksheet =
                                    package.Workbook.Worksheets.Add(configFileModel.DISPLAY_VAL);
                                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true, TableStyles.None);
                                worksheet.Cells["A:Z"].AutoFitColumns();

                                var isCopy = FileHelper.CopyFileDataTableEpPlus(query.UserTempFile,
                                   query.PassTempFile,
                                   query.DomainTempFile, package);
                                if (!isCopy)
                                {
                                    throw new Exception("Cannot authentication user/password path file.");
                                }
                            }
                            returnGenReportReconcileModel.ReturnCode = "0";
                            returnGenReportReconcileModel.ReturnMessage = "Success";
                        }
                        catch (Exception ex)
                        {
                            returnGenReportReconcileModel.ReturnCode = "-1";
                            returnGenReportReconcileModel.ReturnMessage = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnGenReportReconcileModel.ReturnCode = "-1";
                returnGenReportReconcileModel.ReturnMessage = ex.Message;
            }

            return returnGenReportReconcileModel;
        }
    }
}
