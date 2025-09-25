using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class DeleteDSLAMMasterCommandHandler : ICommandHandler<DeleteDSLAMMasterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_CARD_INFO> _FBB_CARD_INFO;
        private readonly IEntityRepository<FBB_PORT_INFO> _FBB_PORT_INFO;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public DeleteDSLAMMasterCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
            IEntityRepository<FBB_CARD_INFO> FBB_CARD_INFO,
            IEntityRepository<FBB_PORT_INFO> FBB_PORT_INFO,
            IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_CARD_INFO = FBB_CARD_INFO;
            _FBB_PORT_INFO = FBB_PORT_INFO;
            _historyLog = historyLog;
        }

        public void Handle(DeleteDSLAMMasterCommand command)
        {
            try
            {
                if (command.FlagUpdate != true)
                {
                    decimal s1;
                    decimal s2;

                    s1 = (from r in _FBB_DSLAM_INFO.Get()
                          where r.LOT_NUMBER == command.Lot
                          select r).Count();

                    s2 = (from r in _FBB_DSLAM_INFO.Get()
                          where r.LOT_NUMBER == command.Lot && r.NODEID == null
                          select r).Count();

                    if (s1 == s2)
                    {
                        var cardId = from a in _FBB_CARD_INFO.Get()
                                     join b in _FBB_DSLAM_INFO.Get() on a.DSLAMID equals b.DSLAMID
                                     where b.LOT_NUMBER == command.Lot && b.REGION_CODE == command.RegionCode && b.ACTIVEFLAG == "Y" && b.NODEID == null
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

                        var dslamInfo = from r in _FBB_DSLAM_INFO.Get()
                                        where r.LOT_NUMBER == command.Lot && r.REGION_CODE == command.RegionCode && r.ACTIVEFLAG == "Y" && r.NODEID == null
                                        select r;

                        var fbbCard = (from r in _FBB_CARD_INFO.Get()
                                       where dslamInfo.Select(s => s.DSLAMID).Contains(r.DSLAMID) && r.ACTIVEFLAG == "Y"
                                       select r).ToList();

                        foreach (var a in fbbCard)
                        {
                            a.ACTIVEFLAG = "N";
                            a.UPDATED_BY = command.Username;
                            a.UPDATED_DATE = DateTime.Now;
                            _FBB_CARD_INFO.Update(a);
                        }

                        foreach (var a in dslamInfo)
                        {
                            a.ACTIVEFLAG = "N";
                            a.UPDATED_BY = command.Username;
                            a.UPDATED_DATE = DateTime.Now;
                            _FBB_DSLAM_INFO.Update(a);
                        }

                    }
                    else
                    {
                        command.FlagNot = true;
                    }
                }
                else if (command.FlagUpdate == true)
                {
                    var dslamInfo = from r in _FBB_DSLAM_INFO.Get()
                                    where r.LOT_NUMBER == command.Lot && r.REGION_CODE == command.RegionCode && r.ACTIVEFLAG == "Y" && r.NODEID == null
                                    select r;
                    int i = 0;
                    var dslamIdList = new List<decimal>();
                    foreach (var a in dslamInfo)
                    {
                        if (i < command.Loop)
                            dslamIdList.Add(a.DSLAMID);
                        else
                            break;

                        i++;
                    }

                    dslamInfo = from r in dslamInfo where dslamIdList.Contains(r.DSLAMID) select r;

                    var fbbCard = from a in _FBB_CARD_INFO.Get()
                                  where dslamIdList.Contains(a.DSLAMID)
                                  select a;

                    var fbbPort = (from r in _FBB_PORT_INFO.Get()
                                   where fbbCard.Select(s => s.CARDID).Contains(r.CARDID) && r.ACTIVEFLAG == "Y"
                                   select r).ToList();

                    foreach (var a in fbbPort)
                    {
                        a.ACTIVEFLAG = "N";
                        a.UPDATED_BY = command.Username;
                        a.UPDATED_DATE = DateTime.Now;
                        _FBB_PORT_INFO.Update(a);
                    }

                    foreach (var a in fbbCard)
                    {
                        a.ACTIVEFLAG = "N";
                        a.UPDATED_BY = command.Username;
                        a.UPDATED_DATE = DateTime.Now;
                        _FBB_CARD_INFO.Update(a);
                    }

                    foreach (var a in dslamInfo)
                    {
                        a.ACTIVEFLAG = "N";
                        a.UPDATED_BY = command.Username;
                        a.UPDATED_DATE = DateTime.Now;
                        _FBB_DSLAM_INFO.Update(a);
                    }

                }

                _uow.Persist();

                #region history Delete
                if (command.FlagUpdate == true)
                {
                    var description = "Region: " + command.RegionCode + ", Lot: " + command.Lot + ", Current Number: " + command.OldNo + " to " + command.NewNo;

                    var historyLog = new FBB_HISTORY_LOG();
                    historyLog.ACTION = ActionHistory.DELETE.ToString();
                    historyLog.APPLICATION = "FBB_CFG002_1";
                    historyLog.CREATED_BY = command.Username;
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = description;
                    historyLog.REF_KEY = command.Lot;
                    historyLog.REF_NAME = "Lot";

                    if (description != string.Empty)
                    {
                        _historyLog.Create(historyLog);
                        _uow.Persist();
                    }
                }
                else
                {
                    if (command.FlagNot != true)
                    {
                        var description = "Region: " + command.RegionCode + ", Lot: " + command.Lot;

                        var historyLog = new FBB_HISTORY_LOG();
                        historyLog.ACTION = ActionHistory.DELETE.ToString();
                        historyLog.APPLICATION = "FBB_CFG002_1";
                        historyLog.CREATED_BY = command.Username;
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = description;
                        historyLog.REF_KEY = command.Lot;
                        historyLog.REF_NAME = "Lot";

                        if (description != string.Empty)
                        {
                            _historyLog.Create(historyLog);
                            _uow.Persist();
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }

        }

    }
}
