using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class CreateDSLAMMasterCommandHandler : ICommandHandler<CreateDSLAMMasterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_CARD_INFO> _FBB_CARD_INFO;
        private readonly IEntityRepository<FBB_CARDMODEL> _FBB_CARDMODEL;
        private readonly IEntityRepository<FBB_PORT_INFO> _FBB_PORT_INFO;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public CreateDSLAMMasterCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
            IEntityRepository<FBB_CARD_INFO> FBB_CARD_INFO,
            IEntityRepository<FBB_CARDMODEL> FBB_CARDMODEL,
            IEntityRepository<FBB_PORT_INFO> FBB_PORT_INFO,
            IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL,
            IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_CARD_INFO = FBB_CARD_INFO;
            _FBB_CARDMODEL = FBB_CARDMODEL;
            _FBB_PORT_INFO = FBB_PORT_INFO;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
            _historyLog = historyLog;
        }

        public void Handle(CreateDSLAMMasterCommand command)
        {
            try
            {
                var model = command.DSLAMMasterModel;

                var dup = new List<FBB_DSLAM_INFO>();

                if (command.FlagUpdate == true)
                {
                    var t1 = (from di in _FBB_DSLAM_INFO.Get()
                              join d in _FBB_DSLAMMODEL.Get() on di.DSLAMMODELID equals d.DSLAMMODELID
                              where di.LOT_NUMBER == model.Lot && di.REGION_CODE == model.Region
                              select new DSLAMTempModel
                              {
                                  DSLAMID = di.DSLAMID,
                                  Model = d.MODEL
                              }).FirstOrDefault();

                    model.DSLAMModel = t1.Model;

                    model.CardViewModel = (from c in _FBB_CARDMODEL.Get()
                                           join ci in _FBB_CARD_INFO.Get() on c.CARDMODELID equals ci.CARDMODELID
                                           where ci.DSLAMID == t1.DSLAMID
                                           select new CardViewModel
                                           {
                                               model = c.MODEL,
                                               reserve = ci.RESERVE_TECHNOLOGY
                                           }).ToList();


                }
                else
                {
                    dup = (from r in _FBB_DSLAM_INFO.Get()
                           where r.LOT_NUMBER == model.Lot && r.REGION_CODE == model.Region && r.ACTIVEFLAG == "Y"
                           select r).ToList();
                }

                if (dup.Any() && command.FlagUpdate != true)
                {
                    command.FlagDup = true;
                }
                else
                {
                    var dslamModel = (from r in _FBB_DSLAMMODEL.Get()
                                      where r.MODEL == model.DSLAMModel
                                      select r).FirstOrDefault();

                    decimal i;
                    for (i = 1; i <= model.DSLAMNo; i++)
                    {
                        //insert fbb_dslam_info
                        var fbbDSLAM = new FBB_DSLAM_INFO
                        {
                            CREATED_BY = model.Username,
                            CREATED_DATE = DateTime.Now,
                            UPDATED_BY = model.Username,
                            UPDATED_DATE = DateTime.Now,
                            DSLAMNUMBER = 0,
                            DSLAMMODELID = dslamModel.DSLAMMODELID,
                            ACTIVEFLAG = "Y",
                            NODEID = null,
                            REGION_CODE = model.Region,
                            LOT_NUMBER = model.Lot
                        };

                        _FBB_DSLAM_INFO.Create(fbbDSLAM);
                        _uow.Persist();

                        //insert fbb_card_info
                        var dslamId = (from r in _FBB_DSLAM_INFO.Get()
                                       orderby r.DSLAMID descending
                                       select r.DSLAMID).FirstOrDefault();

                        decimal cardNo = dslamModel.SLOTSTARTINDEX;
                        foreach (var c in model.CardViewModel)
                        {
                            var cardModel = (from r in _FBB_CARDMODEL.Get()
                                             where r.MODEL == c.model
                                             select new CardPortModel
                                             {
                                                 CardModelID = r.CARDMODELID,
                                                 PortStartIndex = r.PORTSTARTINDEX,
                                                 MaxPort = r.MAXPORT,
                                                 ReservePortSpare = r.RESERVEPORTSPARE
                                             }).FirstOrDefault();

                            var fbbCard = new FBB_CARD_INFO
                            {
                                CREATED_BY = model.Username,
                                CREATED_DATE = DateTime.Now,
                                UPDATED_BY = model.Username,
                                UPDATED_DATE = DateTime.Now,
                                DSLAMID = dslamId,
                                CARDNUMBER = cardNo,
                                CARDMODELID = cardModel.CardModelID,
                                ACTIVEFLAG = "Y",
                                RESERVE_TECHNOLOGY = c.reserve
                            };

                            _FBB_CARD_INFO.Create(fbbCard);
                            _uow.Persist();

                            //insert fbb_port_info
                            var cardId = (from r in _FBB_CARD_INFO.Get()
                                          orderby r.CARDID descending
                                          select r.CARDID).FirstOrDefault();

                            decimal portNo = cardModel.PortStartIndex;
                            decimal portSpareNo = cardModel.MaxPort - cardModel.ReservePortSpare;
                            string portType;
                            int p;
                            for (p = 1; p <= cardModel.MaxPort; p++)
                            {
                                portType = "SELL";
                                if (p > portSpareNo)
                                    portType = "SPARE";

                                var fbbPort = new FBB_PORT_INFO
                                {
                                    CREATED_BY = model.Username,
                                    CREATED_DATE = DateTime.Now,
                                    UPDATED_BY = model.Username,
                                    UPDATED_DATE = DateTime.Now,
                                    CARDID = cardId,
                                    PORTNUMBER = portNo,
                                    PORTSTATUSID = 1,
                                    PORTTYPE = portType,
                                    ACTIVEFLAG = "Y"
                                };
                                _FBB_PORT_INFO.Create(fbbPort);
                                portNo++;
                            }
                            _uow.Persist();

                            cardNo++;
                        }


                    }


                    if (command.FlagUpdate != true && command.FlagDup != true)
                    {
                        #region history ADD
                        var dslamLog = new DSLAMMasterHistoryModel
                        {
                            DSLAMMODEL = dslamModel.MODEL,
                            REGION_CODE = model.Region,
                            LOT_NUMBER = model.Lot,
                            DSLAMNUMBER = model.DSLAMNo,
                            CREATED_BY = model.Username
                        };

                        var description = "DSLAM Model: " + dslamLog.DSLAMMODEL + ", Region: " + dslamLog.REGION_CODE + ", Lot: " + dslamLog.LOT_NUMBER +
                            ", Number of DSLAM: " + dslamLog.DSLAMNUMBER;

                        var historyLog = new FBB_HISTORY_LOG();
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "FBB_CFG002_1";
                        historyLog.CREATED_BY = model.Username;
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = description;
                        historyLog.REF_KEY = model.Lot;
                        historyLog.REF_NAME = "Lot";

                        if (description != string.Empty)
                        {
                            _historyLog.Create(historyLog);
                            _uow.Persist();
                        }
                        #endregion
                    }

                    else if (command.FlagUpdate == true)
                    {
                        #region history Edit
                        var description = "Region: " + model.Region + ", Lot: " + model.Lot + ", Current Number: " + command.OldNo + " to " + command.NewNo;

                        var historyLog = new FBB_HISTORY_LOG();
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "FBB_CFG002_1";
                        historyLog.CREATED_BY = model.Username;
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = description;
                        historyLog.REF_KEY = model.Lot;
                        historyLog.REF_NAME = "Lot";

                        if (description != string.Empty)
                        {
                            _historyLog.Create(historyLog);
                            _uow.Persist();
                        }

                        #endregion
                    }


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
