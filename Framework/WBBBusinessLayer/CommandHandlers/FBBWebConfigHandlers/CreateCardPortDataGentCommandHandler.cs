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
    public class CreateCardPortDataGentCommandHandler : ICommandHandler<CreateCardPortDataCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_CARD_INFO> _FBB_CARD_INFO;
        private readonly IEntityRepository<FBB_CARDMODEL> _FBB_CARDMODEL;
        private readonly IEntityRepository<FBB_PORT_INFO> _FBB_PORT_INFO;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;

        public CreateCardPortDataGentCommandHandler(ILogger logger,
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

        public void Handle(CreateCardPortDataCommand command)
        {

            try

            {


                var PortID = _FBB_PORT_INFO.Get().Max(d => d.PORTID) + 1;
                var For_Info = (from dl in _FBB_DSLAM_INFO.Get()
                                join cardi in _FBB_CARD_INFO.Get() on dl.DSLAMID equals cardi.DSLAMID
                                join cardm in _FBB_CARDMODEL.Get() on cardi.CARDMODELID equals cardm.CARDMODELID
                                join dslm in _FBB_DSLAMMODEL.Get() on cardm.CARDMODELID equals dslm.DSLAMMODELID
                                where dl.ACTIVEFLAG == "Y" && cardi.ACTIVEFLAG == "Y" && dl.DSLAMID == command.DSalamModelID

                                select new CoveragePortPanelGrid()
                                {
                                    Number = cardi.CARDNUMBER,
                                    CARDID = cardi.CARDID,
                                    CardModel = cardm.MODEL,
                                    CardType = cardm.DATAONLY_FLAG,
                                    Reseve = cardi.RESERVE_TECHNOLOGY,
                                    Maxslot = dslm.MAXSLOT
                                }
              ).FirstOrDefault();

                var result = new List<CoveragePortPanelGrid>();


                var temp = (from dl in _FBB_DSLAM_INFO.Get()

                            join cardi in _FBB_CARD_INFO.Get() on dl.DSLAMID equals cardi.DSLAMID
                            join cardm in _FBB_CARDMODEL.Get() on cardi.CARDMODELID equals cardm.CARDMODELID
                            where dl.ACTIVEFLAG == "Y" && cardi.ACTIVEFLAG == "Y" && dl.DSLAMID == command.DSalamModelID

                            select new CoveragePortPanelGrid()
                            {
                                Number = cardi.CARDNUMBER,
                                CARDID = cardi.CARDID,
                                CardModel = cardm.MODEL,
                                CardType = cardm.DATAONLY_FLAG,
                                Reseve = cardi.RESERVE_TECHNOLOGY,


                            }
                          );
                result = temp.ToList();
                ///  var model = command.CoverageCardPortgRIDdata;

                var dup = from p in _FBB_PORT_INFO.Get()
                          where p.PORTSTATUSID != 1 && p.CARDID == command.CARDID
                          select p;


                var MaxPort = (from d in _FBB_DSLAMMODEL.Get()
                               join ds in _FBB_DSLAM_INFO.Get() on d.DSLAMMODELID equals ds.DSLAMMODELID
                               where ds.DSLAMID == command.DSalamModelID

                               select new CardPortModel
                               {
                                   CardModelID = d.DSLAMMODELID,
                                   MaxPort = d.MAXSLOT

                               }).ToList();

                // var maxPortdata = MaxPort + 1;




                if (dup.Any())
                {
                    command.Return_Code = 0;
                    command.Return_Desc = "CARDID Already Existing.";
                }

                //if (temp.Count()>=For_Info.Maxslot)
                //{

                //    command.Return_Code = 0;
                //    command.Return_Desc = "MaxPort Most  Existing.";
                //}

                #region else definition
                else
                {
                    //decimal dslamModelId = (from r in _FBB_DSLAMMODEL.Get()
                    //                       where r.MODEL == model.DSLAMModel
                    //                       select r.DSLAMMODELID).FirstOrDefault();
                    #region for data
                    //decimal i;

                    var cardModel = (from r in _FBB_CARDMODEL.Get()
                                     where r.CARDMODELID == command.CardModelID
                                     select new CardPortModel
                                     {
                                         CardModelID = r.CARDMODELID,
                                         PortStartIndex = r.PORTSTARTINDEX,
                                         MaxPort = r.MAXPORT,
                                         ReservePortSpare = r.RESERVEPORTSPARE
                                     }).FirstOrDefault();

                    var cardId = (from r in _FBB_CARD_INFO.Get()
                                  orderby r.CARDID descending
                                  select r.CARDID).FirstOrDefault();





                    decimal portNo = cardModel.PortStartIndex;
                    decimal portNod = cardModel.PortStartIndex;
                    decimal portSpareNo = cardModel.MaxPort - cardModel.ReservePortSpare;
                    string portType;
                    decimal p = 1;
                    if (portNo >= cardModel.MaxPort)
                    {


                        for (p = portNod; p < cardModel.MaxPort + portNod; p++)
                        {
                            portType = "SELL";
                            if (p > portSpareNo)
                                portType = "SPARE";

                            var fbbPort = new FBB_PORT_INFO
                            {
                                CREATED_BY = command.Create_BY,
                                CREATED_DATE = DateTime.Now,
                                UPDATED_BY = command.Update_BY,
                                UPDATED_DATE = DateTime.Now,
                                CARDID = command.CARDID,
                                PORTNUMBER = portNo,
                                PORTSTATUSID = 1,
                                PORTTYPE = portType,
                                ACTIVEFLAG = "Y"
                            };
                            _FBB_PORT_INFO.Create(fbbPort);


                            portNo++;
                            command.CARDID = fbbPort.CARDID;
                            command.ResultCommand = "CARIDGRID";
                            command.Return_Code = 1;
                        }
                        _uow.Persist();

                        command.CARDID = command.CARDID;
                        command.ResultCommand = "CARIDGRID";
                        command.Return_Code = 1;

                    }
                    else
                    {


                        for (p = portNod; p < cardModel.MaxPort + portNod; p++)
                        {
                            portType = "SELL";
                            if (p > portSpareNo)
                                portType = "SPARE";

                            var fbbPort = new FBB_PORT_INFO
                            {
                                CREATED_BY = command.Create_BY,
                                CREATED_DATE = DateTime.Now,
                                UPDATED_BY = command.Update_BY,
                                UPDATED_DATE = DateTime.Now,
                                CARDID = command.CARDID,
                                PORTNUMBER = portNo,
                                PORTSTATUSID = 1,
                                PORTTYPE = portType,
                                ACTIVEFLAG = "Y"
                            };
                            _FBB_PORT_INFO.Create(fbbPort);
                            portNo++;

                        }
                        _uow.Persist();


                    }


                    command.CARDID = command.CARDID;
                    command.ResultCommand = "CARIDGRID";
                    ///command.Return_Code = 1;






                }



                /// }

                command.Return_Code = 1;
                command.Return_Desc = "Generate Port Compate.";

                #endregion
                #endregion
                //end try
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }

        }

    }
}
