using iWorkflowsContract.Queries.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;

namespace WBBWeb.Controllers
{
    [CustomActionFilter]
    [CustomHandleError]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class RegisterSummaryController : WBBController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CustRegisterCommand> _custRegCommand;
        private readonly ICommandHandler<CoverageResultCommand> _covResultCommand;

        public RegisterSummaryController(IQueryProcessor queryProcessor,
            ICommandHandler<CustRegisterCommand> custRegCommand,
            ICommandHandler<CoverageResultCommand> covResultCommand,
            ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _custRegCommand = custRegCommand;
            _covResultCommand = covResultCommand;
            base.Logger = logger;
        }

        private void MoveFilesToUploadPath(List<FileUploadModel> attachments, bool useVirtualDirectory)
        {
            if (attachments != null)
            {
                var subFolder = "CustomerRegisterFile";
                var tempDirectory = System.IO.Path.Combine(Configurations.UploadFileTempPath, subFolder);
                var targetDirectory = string.Empty;
                var nasDir = string.Empty;

                if (Configurations.UploadFileByVirtualDir)
                    targetDirectory = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~\\" + nasDir), subFolder);
                else
                    targetDirectory = System.IO.Path.Combine(Configurations.UploadFilePath, subFolder);

                string tempFilePath = string.Empty;
                string targetfilePath = string.Empty;

                if (Configurations.UploadFileByImpersonate)
                {
                    try
                    {
                        var query = new GetDbAccountQuery
                        {
                            ProjectCode = Configurations.ProjectCodeNAS
                        };

                        var account = _queryProcessor.Execute(query);

                        if (account != null)
                        {
                            string decryptUsername = account.UserName;
                            string decryptPassword = account.Password;
                            string decryptServername = account.ServerName;

                            using (var impersonator = new Impersonator(decryptUsername, decryptPassword, decryptServername))
                            {
                                foreach (var file in attachments)
                                {
                                    tempFilePath = System.IO.Path.Combine(tempDirectory, file.FileName);
                                    targetfilePath = System.IO.Path.Combine(targetDirectory, file.FileName);

                                    if (System.IO.File.Exists(tempFilePath))
                                    {
                                        if (!System.IO.Directory.Exists(targetDirectory))
                                            System.IO.Directory.CreateDirectory(targetDirectory);

                                        System.IO.File.Move(tempFilePath, targetfilePath);
                                    }
                                    else
                                    {
                                        throw new Exception("File Not Found :" + tempFilePath);
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Don't have account for impersonate");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info(ex.GetErrorMessage());
                    }
                }
                else
                {
                    // map drive
                    foreach (var file in attachments)
                    {
                        try
                        {
                            tempFilePath = System.IO.Path.Combine(tempDirectory, file.FileName);
                            targetfilePath = System.IO.Path.Combine(targetDirectory, file.FileName);

                            if (System.IO.File.Exists(tempFilePath))
                            {
                                if (!System.IO.Directory.Exists(targetDirectory))
                                    System.IO.Directory.CreateDirectory(targetDirectory);

                                System.IO.File.Move(tempFilePath, targetfilePath);
                                file.FilePath = targetfilePath;
                            }
                            else
                            {
                                throw new Exception("File Not Found :" + tempFilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info(ex.GetErrorMessage());
                        }
                    }
                }
            }
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 30)]
        public JsonResult GetPanelPreviewFile(string Packgrop, string line)
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "SHOW_PANEL_PREVIEW_FILE_" + line).ToList();

            List<FbbConstantModel> screenValue = config.Select(l => new FbbConstantModel
            {
                Validation = l.LovValue1
            }).ToList();

            return Json(screenValue, JsonRequestBehavior.AllowGet);

        }

    }
}
