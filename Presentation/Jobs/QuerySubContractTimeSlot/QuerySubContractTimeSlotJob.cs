using System;
using System.Collections.Generic;

namespace QuerySubContractTimeSlot
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.FBSS;
    using WBBContract.Queries.Commons.Masters;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;

    public class QuerySubContractTimeSlotJob
    {
        public ILogger _logger;
        private readonly ICommandHandler<FBSSSubContractTimslotInterfaceLogCommand> _InterfaceLog;
        private readonly ICommandHandler<FBSSSubContractTimeslotInsertTableCommand> _Table;
        private readonly ICommandHandler<NotificationBatchCommand> _Sendmail;
        private readonly ICommandHandler<FBSSSubContractTimeslotRollbackCommand> _rollbackCommand;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;

        public QuerySubContractTimeSlotJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<NotificationBatchCommand> Sendmail,
            ICommandHandler<FBSSSubContractTimslotInterfaceLogCommand> InterfaceLog,
            ICommandHandler<FBSSSubContractTimeslotInsertTableCommand> Table,
            ICommandHandler<FBSSSubContractTimeslotRollbackCommand> rollbackCommand)
        {
            _logger = logger;
            _InterfaceLog = InterfaceLog;
            _Table = Table;
            _queryProcessor = queryProcessor;
            _rollbackCommand = rollbackCommand;
            _Sendmail = Sendmail;
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("Take " + _timer.Elapsed);
        }

        public void InsertInterfaceLog(string filepath, string error)
        {
            _logger.Info("QuerySubContractTimeSlotJob Insert InterfaceLog.");
            StartWatching();
            try
            {
                var command = new FBSSSubContractTimslotInterfaceLogCommand
                {
                    Filepath = filepath,
                    ErrorResult = error
                };

                _InterfaceLog.Handle(command);
                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("QuerySubConTractJob Insert InterfaceLog" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching();
                throw ex;
            }
        }

        public void InsertTimeSlot(string filepath, string filename, string bufferdata, string datadate)
        {
            _logger.Info("QuerySubContractTimeSlotJob Insert TimslotTable.");
            StartWatching();
            try
            {
                var command = new FBSSSubContractTimeslotInsertTableCommand
                {
                    Filepath = filepath,
                    Filename = filename,
                    Bufferdata = bufferdata,
                    DataDate = datadate
                };

                _Table.Handle(command);
                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("QuerySubContractTimeSlotJob Insert TimslotTable" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                RollbackTimeSlot(filepath, filename, "Insert Timslot Error Then Rollback : " + ex.RenderExceptionMessage());
                StopWatching();
                throw ex;
            }
        }

        public void RollbackTimeSlot(string filepath, string filename, string errorMessage)
        {
            _logger.Info("QuerySubContractTimeSlotJob RollbackTimeSlot.");
            StartWatching();
            try
            {
                var command = new FBSSSubContractTimeslotRollbackCommand
                {
                    FilePath = filepath,
                    FileName = filename,
                    ErrorMessage = errorMessage,
                };

                _rollbackCommand.Handle(command);
                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("QuerySubContractTimeSlotJob RollbackTimeSlot "
                    + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                InsertInterfaceLog(filepath, "QuerySubContractTimeSlotJob RollbackTimeSlot Error : " + ex.RenderExceptionMessage());
                StopWatching();
                //throw ex;
            }
        }

        public void Sendmail(string Cause)
        {
            _logger.Info("QuerySubContractTimeSlotJob sending an Email.");
            StartWatching();
            try
            {
                var command = new NotificationBatchCommand
                {
                    Cause = Cause
                };

                _Sendmail.Handle(command);
                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("QuerySubContractTimeSlotJob Sending an Email" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching();
                throw ex;
            }
        }

        public List<LovValueModel> GetLovList(string type, string name)
        {
            _logger.Info("GetLovList");
            StartWatching();

            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                StopWatching();
                return lov;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching();
                return new List<LovValueModel>();
            }
        }
    }
}
