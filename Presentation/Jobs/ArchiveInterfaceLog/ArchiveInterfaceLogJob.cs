using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveInterfaceLog
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.FBBWebConfigCommands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.FBBWebConfigModels;

    public class ArchiveInterfaceLogJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<ArchiveInterfaceLogCommand> _ArchiveInterfaceLog;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private Stopwatch _timer;

        public ArchiveInterfaceLogJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<ArchiveInterfaceLogCommand> ArchiveInterfaceLog,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _ArchiveInterfaceLog = ArchiveInterfaceLog;
            _sendMail = sendMail;
        }

        public void Execute()
        {
            _logger.Info("Archive Interface Log.");

            try
            {
                var ArchiveDateVar = GetLovList("FBB_ARC_CONSTANT", "P_ARC_DT").FirstOrDefault();

                if (ArchiveDateVar == null || ArchiveDateVar.LovValue1.ToSafeString() == "" || ArchiveDateVar.LovValue4.ToSafeString() == "")
                {
                    _logger.Info("Please Verify P_ARC_DT value in table FBB_CFGG_LOV.");
                    StopWatching("Archive Interface Log take");
                }
                else
                {
                    StartWatching();

                    //var command = new ArchiveInterfaceLogCommand
                    //{
                    //    p_date = ArchiveDateVar.LovValue1.ToSafeDecimal(),
                    //    p_type = ArchiveDateVar.LovValue4.ToSafeString()
                    //};
                    //_ArchiveInterfaceLog.Handle(command);
                    //_logger.Info(string.Format("Archive Interface Log in {0} {1} : {2}", ArchiveDateVar.LovValue1.ToSafeString(), 
                    //    ArchiveDateVar.LovValue4.ToSafeString(), command.Return_Message));

                    var query = new ArchiveInterfaceLogQuery
                    {
                        p_date = ArchiveDateVar.LovValue1.ToSafeDecimal(),
                        p_type = ArchiveDateVar.LovValue4.ToSafeString()
                    };

                    var result = _queryProcessor.Execute(query);
                    if (result != null && result.Count() > 0)
                    {
                        var sendMailDetaillist = result.Select(x => new ArchiveInterfaceLogSendMailDetailList()
                        {
                            ret_code = x.ret_code,
                            ret_msg = x.ret_msg.ToSafeString(),
                            p_subject = x.p_subject.ToSafeString(),
                            p_body_h = x.p_body_h.ToSafeString(),
                            p_body_result = x.p_body_result.ToSafeString(),
                            p_body_summary = x.p_body_summary.ToSafeString(),
                            p_body_signature = x.p_body_signature.ToSafeString()
                        }).FirstOrDefault();

                        _logger.Info(string.Format("Archive Interface Log in {0} {1} : {2}", ArchiveDateVar.LovValue1.ToSafeString(),
                            ArchiveDateVar.LovValue4.ToSafeString(), sendMailDetaillist.ret_msg.ToSafeString()));
                    }
                    else
                    {
                        _logger.Info(string.Format("Archive Interface Log in {0} {1} : {2}", ArchiveDateVar.LovValue1.ToSafeString(),
                            ArchiveDateVar.LovValue4.ToSafeString(), "No Data Found."));
                    }
                    StopWatching("Archive Interface Log take");
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Archive Interface Log :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("Archive Interface Log take");
            }
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string getLov)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} : {1}", getLov, _timer.Elapsed));
        }

        public List<LovValueModel> GetLovList(string type, string name)
        {
            _logger.Info("Get parameter from Lov.");
            StartWatching();
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                _logger.Info(lov);
                StopWatching("Get parameter from Lov take");
                return lov;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("Get parameter from Lov take");
                return new List<LovValueModel>();
            }
        }
    }
}
