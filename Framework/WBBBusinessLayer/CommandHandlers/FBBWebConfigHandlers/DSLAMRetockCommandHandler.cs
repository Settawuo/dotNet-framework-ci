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
    public class DSLAMRetockCommandHandler : ICommandHandler<DSLAMRetockCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_PORT_INFO> _FBB_PORT_INFO;
        private readonly IEntityRepository<FBB_CARD_INFO> _FBB_CARD_INFO;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public DSLAMRetockCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> FBB_PORT_INFO,
            IEntityRepository<FBB_CARD_INFO> FBB_CARD_INFO,
            IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
            IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _FBB_PORT_INFO = FBB_PORT_INFO;
            _FBB_CARD_INFO = FBB_CARD_INFO;
            _historyLog = historyLog;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
        }

        public void Handle(DSLAMRetockCommand command)
        {
            try
            {
                var countPort = (from p in _FBB_PORT_INFO.Get()
                                 join ci in _FBB_CARD_INFO.Get() on p.CARDID equals ci.CARDID
                                 where p.PORTSTATUSID != 1 && ci.DSLAMID == command.DSLAMID
                                 select p).Count();

                if (countPort > 0)
                {
                    command.FlagNot = true;
                }
                else
                {
                    if (command.Action == "Restock")
                    {
                        var fbbDslamInfo = from r in _FBB_DSLAM_INFO.Get()
                                           where r.DSLAMID == command.DSLAMID
                                           select r;

                        foreach (var a in fbbDslamInfo)
                        {
                            a.NODEID = null;
                            a.DSLAMNUMBER = 0;
                            a.CVRID = 0;
                            a.UPDATED_BY = command.Username;
                            a.UPDATED_DATE = DateTime.Now;

                            _FBB_DSLAM_INFO.Update(a);
                        }
                        //update fbb_dslam_info a set a.nodeid = null,a.dslamnumber = 0,a.cvrid = Null ,a.updated_by = 'chanawun',a.updated_date= SYSDATE
                        //where a.dslamid = 1061;
                        string description = "Restock Node ID: " + command.NodeID;
                        string action = ActionHistory.UPDATE.ToString();

                        var historyLog = new FBB_HISTORY_LOG();
                        historyLog.ACTION = action;
                        historyLog.APPLICATION = "FBB_CFG001_3_DSLAM_RESTOCK";
                        historyLog.CREATED_BY = command.Username;
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = description;
                        historyLog.REF_KEY = command.NodeTH;
                        historyLog.REF_NAME = "Node Name TH";

                        if (description != string.Empty)
                            _historyLog.Create(historyLog);
                    }
                    else if (command.Action == "Delete")
                    {
                        //delete dslam
                        var fbbDslamInfo = from r in _FBB_DSLAM_INFO.Get()
                                           where r.DSLAMID == command.DSLAMID
                                           select r;
                        foreach (var a in fbbDslamInfo)
                        {
                            a.ACTIVEFLAG = "N";
                            a.UPDATED_BY = command.Username;
                            a.UPDATED_DATE = DateTime.Now;
                            _FBB_DSLAM_INFO.Update(a);
                        }

                        //delete card
                        var fbbCard = (from r in _FBB_CARD_INFO.Get()
                                       where fbbDslamInfo.Select(s => s.DSLAMID).Contains(r.DSLAMID) && r.ACTIVEFLAG == "Y"
                                       select r).ToList();
                        foreach (var a in fbbCard)
                        {
                            a.ACTIVEFLAG = "N";
                            a.UPDATED_BY = command.Username;
                            a.UPDATED_DATE = DateTime.Now;
                            _FBB_CARD_INFO.Update(a);
                        }

                        //delete port
                        var cardId = from a in _FBB_CARD_INFO.Get()
                                     join b in _FBB_DSLAM_INFO.Get() on a.DSLAMID equals b.DSLAMID
                                     where b.DSLAMID == command.DSLAMID
                                     select a.CARDID;

                        var fbbPort = (from r in _FBB_PORT_INFO.Get()
                                       where cardId.Contains(r.CARDID) && r.ACTIVEFLAG == "Y"
                                       select r).ToList();
                        foreach (var a in fbbPort)
                        {
                            a.ACTIVEFLAG = "N";
                            a.UPDATED_BY = command.Username;
                            a.UPDATED_DATE = DateTime.Now;
                            _FBB_PORT_INFO.Update(a);
                        }

                        string description = "Node ID: " + command.NodeID;
                        string action = ActionHistory.DELETE.ToString();

                        var historyLog = new FBB_HISTORY_LOG();
                        historyLog.ACTION = action;
                        historyLog.APPLICATION = "FBB_CFG001_3_DSLAM_RESTOCK";
                        historyLog.CREATED_BY = command.Username;
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = description;
                        historyLog.REF_KEY = command.NodeTH;
                        historyLog.REF_NAME = "Node Name TH";

                        if (description != string.Empty)
                            _historyLog.Create(historyLog);
                    }

                    _uow.Persist();
                }
                //end try
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }

        }

    }
}
