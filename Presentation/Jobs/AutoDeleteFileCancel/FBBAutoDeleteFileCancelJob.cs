using System;
using System.Collections.Generic;
using System.IO;

namespace AutoDeleteFileCancel
{
    using CompositionRoot;
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;

    public class FBBAutoDeleteFileCancelJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private string _outErrorResult = string.Empty;
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }
        public FBBAutoDeleteFileCancelJob(
            ILogger logger,
            IQueryProcessor queryProcessor,

            ICommandHandler<InterfaceLogCommand> intfLogCommand
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;

            _intfLogCommand = intfLogCommand;

        }

        public bool AutoDeleteFileCancel()
        {
            _logger.Info("Start AutoMovefile");
            StartWatching();
            try
            {

                //var lovSourceNAS = GetLovList("FBB_CONSTANT", "Impersonate");
                //var lovSourceNASApp = GetLovList("FBB_CONSTANT", "Impersonate_App");
                //var lovDestNAS = GetLovList("FBB_CONSTANT", "Impersonate_New");
                //var lovDestNASApp = GetLovList("FBB_CONSTANT", "Impersonate_App_New");

                //string username = lovSourceNAS[0].LovValue1;
                //string password = lovSourceNAS[0].LovValue2;
                //string ipAddress = lovSourceNAS[0].LovValue3;
                //string sourceNAS = lovSourceNAS[0].LovValue4;
                //string destNAS = sourceNAS;

                string username = "nas_fixedbb";
                string fromPass = "Fixe1012@Ais";// Fixed Code scan : string password = "Fixe1012@Ais";
                string ipAddress = "10.252.167.22";
                string sourceNAS = @"\\10.252.167.22\fbb_idcard_ndev001b";
                string destNAS = sourceNAS;

                DateTime AddYear = DateTime.Now.AddYears(1);
                string Year = AddYear.Year.ToString();
                string month = string.Empty; string FolderName = string.Empty;
                using (var impersonator = new Impersonator(username, ipAddress, fromPass, false))
                {
                    _logger.Info("AuthenIP :" + ipAddress);

                    if (Directory.Exists(destNAS))
                    {
                        _logger.Info("Found  :" + destNAS);

                        for (int i = 1; i <= 12; i++)
                        {
                            if (i < 10)
                            {
                                month = "0" + i.ToSafeString();
                            }
                            else
                            {
                                month = i.ToSafeString();
                            }
                            FolderName = Year + month;
                            string FullPath = Path.Combine(destNAS, FolderName);
                            //  string _FullPath  = Server.Map
                            // string _path = 
                            if (!Directory.Exists(FullPath))
                            {
                                _logger.Info("AutoDeleteFileCancel :" + FolderName);

                            }
                            else
                            {
                                _logger.Info("Found :" + FolderName + "Skip for Create");
                            }
                        }
                        // Directory.CreateDirectory();
                        _logger.Info("AutoDeleteFileCancel SuccessFull !!!");
                        return true;
                    }
                    else
                    {
                        _logger.Info("NotFound :" + destNAS);
                        return false;
                    }

                }

            }
            catch (Exception ex)
            {
                _outErrorResult = "Error AutoDeleteFileCancel : " + ex.GetErrorMessage();
                _logger.Info("Error AutoDeleteFileCancel : " + ex.GetErrorMessage());


                return false;
            }

        }
        private InterfaceLogCommand StartInterfaceLog<T>(T inXmlParam, string methodName, string outResult)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = "",
                METHOD_NAME = methodName,
                SERVICE_NAME = "Auto_Move_File_Batch",
                IN_ID_CARD_NO = "",
                IN_XML_PARAM = inXmlParam.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "Batch",
                CREATED_DATE = DateTime.Now
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterfaceLog<T>(T outXmlParam, string outResult, InterfaceLogCommand log)
        {
            var dbEndIntfCmd = new InterfaceLogCommand
            {
                ActionType = ActionType.Update,
                IN_TRANSACTION_ID = "",
                OutInterfaceLogId = log.OutInterfaceLogId,
                REQUEST_STATUS = _outErrorResult != "" ? "Error" : "Success",
                OUT_RESULT = outResult,
                OUT_ERROR_RESULT = _outErrorResult,
                OUT_XML_PARAM = outXmlParam.DumpToXml(),
                UPDATED_BY = "Batch",
                UPDATED_DATE = DateTime.Now
            };

            _intfLogCommand.Handle(dbEndIntfCmd);
        }


        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                //_logger.Info("Error GetLovList : " + ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }
    }
}
