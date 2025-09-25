using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class DeleteAWCInfoCommandHandler : ICommandHandler<DeleteAWCInfoCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_APCOVERAGE> _cov;
        private readonly IEntityRepository<FBB_AP_INFO> _FBB_AP_INFO;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;

        public DeleteAWCInfoCommandHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_APCOVERAGE> cov, IEntityRepository<FBB_AP_INFO> FBB_AP_INFO, IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _cov = cov;
            _FBB_AP_INFO = FBB_AP_INFO;
            _historyLog = historyLog;
        }
        public void Handle(DeleteAWCInfoCommand command)
        {
            try
            {
                #region Update

                _logger.Info("Delete AWC APcoverage Info.");

                var info = from r in _cov.Get()
                           where r.APPID == command.APP_ID && r.ACTIVE_FLAG == "Y"
                           select r;

                var modelap = command.model;

                foreach (var a in info)
                {

                    foreach (var a2 in modelap)
                    {
                        var info2 = (from r in _FBB_AP_INFO.Get()
                                     where r.AP_ID == a2.AP_ID
                                     select r).ToList();
                        if (info2.Count != 0)
                        {
                            foreach (var b in info2)
                            {
                                b.UPDATED_BY = a2.user;
                                b.UPDATED_DATE = DateTime.Now;
                                b.ACTIVE_FLAG = "N";
                                _FBB_AP_INFO.Update(b);
                                _uow.Persist();

                                #region Delete FBB_HISTORY_LOG
                                var historyLogItem2 = new FBB_HISTORY_LOG();
                                historyLogItem2.ACTION = ActionHistory.DELETE.ToString();
                                historyLogItem2.APPLICATION = "FBB_CFG006_1";
                                historyLogItem2.CREATED_BY = command.UPDATED_BY;
                                historyLogItem2.CREATED_DATE = DateTime.Now;
                                historyLogItem2.DESCRIPTION = "BASEL2: " + a.BASEL2 + " => " + "AP_Name: " + b.AP_NAME.ToSafeString();
                                historyLogItem2.REF_KEY = b.AP_NAME.ToSafeString();
                                historyLogItem2.REF_NAME = "AP Name";
                                _historyLog.Create(historyLogItem2);
                                _uow.Persist();
                                #endregion
                            }
                        }
                    }

                    a.ACTIVE_FLAG = "N";
                    a.UPDATED_BY = command.UPDATED_BY;
                    a.UPDATED_DATE = DateTime.Now;
                    _cov.Update(a);
                    _uow.Persist();

                    #region Delete FBB_HISTORY_LOG
                    var historyLogItem = new FBB_HISTORY_LOG();
                    historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                    historyLogItem.APPLICATION = "FBB_CFG006_1";
                    historyLogItem.CREATED_BY = command.UPDATED_BY;
                    historyLogItem.CREATED_DATE = DateTime.Now;
                    historyLogItem.DESCRIPTION = "BaseL2: " + a.BASEL2 + ", " + "SITENAME: " + a.SITENAME;
                    historyLogItem.REF_KEY = a.BASEL2;
                    historyLogItem.REF_NAME = "Base L2";
                    _historyLog.Create(historyLogItem);
                    _uow.Persist();
                    #endregion
                }






                #endregion Update
            }
            catch (Exception ex)
            {
                _logger.Info("Error occured when handle DeleteAWCconfigCommandHandler");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }
        }
    }


}
