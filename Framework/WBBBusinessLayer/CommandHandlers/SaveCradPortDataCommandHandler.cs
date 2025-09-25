using ObjectDumper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SaveCradPortDataCommandHandler : ICommandHandler<SaveCradPortDataCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CARDMODEL> _CARDMODEService;
        private readonly IEntityRepository<FBB_CARD_INFO> _FBB_CARD_INFO;
        private readonly IEntityRepository<FBB_CARDMODEL> _FBB_CARD_Model;
        private readonly IEntityRepository<FBB_PORT_INFO> _FBB_PORT_INFO;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _FBB_DSLAMMODEL;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;


        public SaveCradPortDataCommandHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_CARDMODEL> CARDMODEService,
            IEntityRepository<FBB_CARD_INFO> FBB_CARD_INFO,
            IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
            IEntityRepository<FBB_PORT_INFO> FBB_PORT_INFO,
            IEntityRepository<FBB_DSLAMMODEL> FBB_DSLAMMODEL,
            IEntityRepository<FBB_CARDMODEL> FBB_CARD_Model,
            IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _CARDMODEService = CARDMODEService;
            _FBB_CARD_INFO = FBB_CARD_INFO;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_PORT_INFO = FBB_PORT_INFO;
            _FBB_DSLAMMODEL = FBB_DSLAMMODEL;
            _historyLog = historyLog;
            _FBB_CARD_Model = FBB_CARD_Model;
        }

        public void Handle(SaveCradPortDataCommand command)
        {
            try
            {
                _logger.Info(command.DumpToString(command.GetType().Name));

                var CARDID = _FBB_CARD_INFO.Get().Max(d => d.CARDID) + 1;
                var UpdateCradModel = _FBB_CARD_INFO
                       .Get(c => c.CARDID == command.CARDID).FirstOrDefault();

                ///  var cardmode = _FBB_CARD_Model.Get(c=>c.CARDMODELID==UpdateCradModel.CARDMODELID).FirstOrDefault();
                var UpdateCradModel_Info = _FBB_CARD_INFO.Get(c => c.DSLAMID == command.DSalamModelID).FirstOrDefault();

                var showdataHitory = (from dl in _FBB_DSLAM_INFO.Get()
                                      join cardi in _FBB_CARD_INFO.Get() on dl.DSLAMID equals cardi.DSLAMID
                                      join cardm in _FBB_CARD_Model.Get() on cardi.CARDMODELID equals cardm.CARDMODELID
                                      join dslm in _FBB_DSLAMMODEL.Get() on cardm.CARDMODELID equals dslm.DSLAMMODELID
                                      where dl.ACTIVEFLAG == "Y" && cardi.ACTIVEFLAG == "Y" && dl.DSLAMID == command.DSalamModelID
                                      && cardi.CARDID == command.CARDID

                                      select new CoveragePortPanelGrid()
                                      {
                                          Number = cardi.CARDNUMBER,
                                          CARDID = cardi.CARDID,
                                          CardModel = cardm.MODEL,
                                          CardType = cardm.DATAONLY_FLAG,
                                          Reseve = cardi.RESERVE_TECHNOLOGY,
                                          Maxslot = dslm.MAXSLOT
                                      }).FirstOrDefault();



                /// var FlagCardModelInfo = _FBB_CARD_INFO.Get(fb=>fb.CARDMODELID==command.CARDMODELID).FirstOrDefault();
                ///var CARDMODEServicedata = _CARDMODEService.Get().ToList();
                #region Insert _CARDMODEService
                var data = new FBB_CARD_INFO();
                if (command.ResultCommand == "ADD")
                {
                    _logger.Info("SaveCardModelCommand");









                    var For_Info = (from dl in _FBB_DSLAM_INFO.Get()
                                    join cardi in _FBB_DSLAMMODEL.Get() on dl.DSLAMMODELID equals cardi.DSLAMMODELID

                                    where dl.DSLAMID == command.DSalamModelID && dl.ACTIVEFLAG == "Y" && cardi.ACTIVEFLAG == "Y"


                                    select new CoveragePortPanelGrid()
                                    {
                                        DSalamModelID = dl.DSLAMID,
                                        Maxslot = cardi.MAXSLOT

                                    }
                  ).FirstOrDefault();

                    var result = new List<CoveragePortPanelGrid>();


                    var temp = (from dl in _FBB_DSLAM_INFO.Get()

                                join cardi in _FBB_CARD_INFO.Get() on dl.DSLAMID equals cardi.DSLAMID
                                join cardm in _CARDMODEService.Get() on cardi.CARDMODELID equals cardm.CARDMODELID
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


                    if (result.Any())
                    {
                        if (result.Count() >= For_Info.Maxslot)
                        {

                            command.Return_Code = 0;
                            command.Return_Desc = "This Model Limit " + " " + For_Info.Maxslot + " " + "Card.";
                        }
                        else
                        {

                            data.ACTIVEFLAG = "Y";
                            data.CARDID = CARDID;
                            data.CARDMODELID = command.CardModelID;
                            data.CARDNUMBER = command.CradNumber;
                            data.CREATED_BY = command.UPDATEBY;
                            data.CREATED_DATE = command.CRATE_DATE;
                            data.DSLAMID = command.DSalamModelID;
                            data.RESERVE_TECHNOLOGY = command.RESERVE_TECHNOLOGY;
                            data.UPDATED_BY = command.UPDATEBY;
                            data.UPDATED_DATE = command.UPDATE_DATE;
                            if (command.Building != "")
                            {
                                data.BUILDING = command.Building;
                            }





                            _FBB_CARD_INFO.Create(data);
                            _uow.Persist();

                            command.Return_Code = 1;
                            command.Return_Desc = "Saved Compate.";



                            var Card_Port = new Card_Port
                            {
                                Card_Number = command.CradNumber,
                                Card_Model = command.CardType,
                                Building = command.Building,
                                RESERVE_TECHNOLOGY = command.RESERVE_TECHNOLOGY

                            };

                            var description = "";
                            if (command.Building != "")
                            {
                                description = "Card Number: " + Card_Port.Card_Number + ", Card Model: " + Card_Port.Card_Model + " ,Reserve :" + command.RESERVE_TECHNOLOGY + ", Building:  " + Card_Port.Building;
                            }
                            else
                            {
                                description = "Card Number: " + Card_Port.Card_Number + ", Card Model: " + Card_Port.Card_Model + " ,Reserve :" + command.RESERVE_TECHNOLOGY;
                            }




                            var historyLog = new FBB_HISTORY_LOG();
                            historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                            historyLog.ACTION = ActionHistory.ADD.ToString();
                            historyLog.APPLICATION = "FBB_CFG001_4_Card_Port";
                            historyLog.CREATED_BY = command.UPDATEBY;
                            historyLog.CREATED_DATE = DateTime.Now;
                            historyLog.DESCRIPTION = description;
                            historyLog.REF_KEY = command.Node_ID;
                            historyLog.REF_NAME = "NODEID";

                            if (description != string.Empty)
                            {
                                _historyLog.Create(historyLog);
                                _uow.Persist();
                            }




                        }



                    }
                    else
                    {

                        data.ACTIVEFLAG = "Y";
                        data.CARDID = CARDID;
                        data.CARDMODELID = command.CardModelID;
                        data.CARDNUMBER = command.CradNumber;
                        data.CREATED_BY = command.UPDATEBY;
                        data.CREATED_DATE = command.CRATE_DATE;
                        data.DSLAMID = command.DSalamModelID;
                        data.RESERVE_TECHNOLOGY = command.RESERVE_TECHNOLOGY;
                        data.UPDATED_BY = command.UPDATEBY;
                        data.UPDATED_DATE = command.UPDATE_DATE;
                        if (command.Building != null)
                        {
                            data.BUILDING = command.Building;
                        }



                        _FBB_CARD_INFO.Create(data);
                        _uow.Persist();

                        command.Return_Code = 1;
                        command.Return_Desc = "Saved Compate.";



                        var Card_Port = new Card_Port
                        {
                            Card_Number = command.CradNumber,
                            Card_Model = command.CardType,
                            Building = command.Building,
                            RESERVE_TECHNOLOGY = command.RESERVE_TECHNOLOGY

                        };

                        var description2 = "";
                        if (command.Building != "")
                        {
                            description2 = "Card Number: " + Card_Port.Card_Number + ", Card Model: " + Card_Port.Card_Model + " ,Resever :" + Card_Port.RESERVE_TECHNOLOGY + ", Building:  " + Card_Port.Building;
                        }
                        else
                        {
                            description2 = "Card Number: " + Card_Port.Card_Number + ", Card Model: " + Card_Port.Card_Model + " ,Resever :" + Card_Port.RESERVE_TECHNOLOGY;
                        }






                        var historyLog = new FBB_HISTORY_LOG();
                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "FBB_CFG001_4_Card_Port";
                        historyLog.CREATED_BY = command.UPDATEBY;
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = description2;
                        historyLog.REF_KEY = command.Node_ID;
                        historyLog.REF_NAME = "NODEID";

                        if (description2 != string.Empty)
                        {
                            _historyLog.Create(historyLog);
                            _uow.Persist();
                        }




                    }





                }


                #endregion Insert FBB_CARDMODEL


                #region Update_CARDMODEService
                else if (command.CARDID == UpdateCradModel.CARDID && command.ResultCommand == "UPDATE")
                {
                    _logger.Info("Updat Coverage Result.");

                    if (CARDID <= 0)
                    {
                        _logger.Info("Cannot update FBB_COVERAGEAREA_RESULT with result id = " + command.CARDID);
                        return;
                    }

                    var MUDuplicatedT = (from r in _FBB_CARD_INFO.Get() //Check Update DSLAMModel_1_(Edit Other Name)
                                         join por_I in _FBB_PORT_INFO.Get() on r.CARDID equals por_I.CARDID
                                         where r.CARDID == command.CARDID && r.ACTIVEFLAG == "Y"
                                         select r).ToList();

                    if (MUDuplicatedT.Any())
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "CARDID Already Used.";
                    }

                    var cardModelText = "";

                    if (UpdateCradModel.CARDMODELID != command.CardModelID)
                    {
                        var oldCardModeText = from c in _FBB_CARD_Model.Get()
                                              where c.CARDMODELID == UpdateCradModel.CARDMODELID
                                              select c.MODEL;
                        if (oldCardModeText.Any())
                        {
                            cardModelText = "Card Model: " + oldCardModeText.FirstOrDefault() + " to " + command.CardType;
                        }
                    }

                    var old_Card_Port_Info = new FBB_CARD_INFO
                    {
                        ACTIVEFLAG = UpdateCradModel.ACTIVEFLAG,
                        CARDID = UpdateCradModel.CARDID,
                        CARDMODELID = 0,
                        CARDNUMBER = UpdateCradModel.CARDNUMBER,
                        CREATED_BY = UpdateCradModel.CREATED_BY,
                        CREATED_DATE = UpdateCradModel.CREATED_DATE,
                        DSLAMID = UpdateCradModel.DSLAMID,
                        RESERVE_TECHNOLOGY = UpdateCradModel.RESERVE_TECHNOLOGY,
                        UPDATED_BY = UpdateCradModel.UPDATED_BY,
                        UPDATED_DATE = UpdateCradModel.UPDATED_DATE,
                        BUILDING = UpdateCradModel.BUILDING
                    };

                    UpdateCradModel.ACTIVEFLAG = "Y";
                    UpdateCradModel.CARDID = command.CARDID;
                    UpdateCradModel.CARDMODELID = command.CardModelID;
                    UpdateCradModel.CARDNUMBER = command.CradNumber;
                    UpdateCradModel.CREATED_BY = command.UPDATEBY;
                    UpdateCradModel.CREATED_DATE = command.CRATE_DATE;
                    UpdateCradModel.DSLAMID = command.DSalamModelID;
                    UpdateCradModel.RESERVE_TECHNOLOGY = command.RESERVE_TECHNOLOGY;
                    UpdateCradModel.UPDATED_BY = command.UPDATEBY;
                    UpdateCradModel.UPDATED_DATE = command.UPDATE_DATE;

                    if (command.Building != "")
                    {
                        UpdateCradModel.BUILDING = command.Building;
                    }
                    _FBB_CARD_INFO.Update(UpdateCradModel);
                    _uow.Persist();

                    command.Return_Code = 1;
                    command.Return_Desc = "Update Compate.";

                    UpdateCradModel.CARDMODELID = 0;

                    var desc = WBBExtensions.CompareObjectToString(old_Card_Port_Info, UpdateCradModel);

                    var description = "";
                    var buling = "";
                    if (command.Building != "")
                    {
                        description = "Card Number: " + old_Card_Port_Info.CARDNUMBER + " => " + cardModelText;
                        buling = "Building  :" + old_Card_Port_Info.BUILDING + " to " + command.Building;

                    }
                    else
                    {
                        description = "Card Number: " + old_Card_Port_Info.CARDNUMBER + "  " + cardModelText;

                    }


                    if (!string.IsNullOrEmpty(desc))
                    {
                        if (command.Building != "")
                        {
                            description += " " + desc + " " + buling;
                        }
                        else if (command.RESERVE_TECHNOLOGY != "")
                        {
                            description += "  " + desc;

                        }
                        else
                        {

                            description += "  " + desc;
                        }


                    }
                    else
                    {

                        if (command.Building != "")
                        {
                            description += " " + desc + buling;
                        }
                        else if (command.RESERVE_TECHNOLOGY != "")
                        {
                            description += " " + desc;

                        }
                        else
                        {

                            description += " " + desc;
                        }
                    }

                    var historyLog = new FBB_HISTORY_LOG();
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.UPDATE.ToString();
                    historyLog.APPLICATION = "FBB_CFG001_4_Card_Port";
                    historyLog.CREATED_BY = command.UPDATEBY;
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = description;
                    historyLog.REF_KEY = command.Node_ID;
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();

                    #endregion Update_CARDMODEService

                }
                else
                {

                    #region deleteFBB_CARDMODEL
                    if (command.ResultCommand == "DELETE")
                    {

                        var CardNotPortAvalibel = (from r in _FBB_PORT_INFO.Get()
                                                   where r.PORTSTATUSID != 1
                                                   && r.CARDID == command.CARDID
                                                   select r).ToList();



                        var deleteDuplicated = (from r in _FBB_CARD_INFO.Get() //Check Update DSLAMModel_1_(Edit Other Name)
                                                join por_I in _FBB_PORT_INFO.Get() on r.CARDID equals por_I.CARDID
                                                where r.CARDID == command.CARDID && r.ACTIVEFLAG == "Y"
                                                select r).ToList();

                        List<FBB_PORT_INFO> deleteportstuts = new List<FBB_PORT_INFO>();
                        deleteportstuts = (from r in _FBB_PORT_INFO.Get()
                                           where r.PORTSTATUSID == 1
                                           && r.CARDID == command.CARDID
                                           select r).ToList();

                        if (CardNotPortAvalibel.Any())
                        {
                            //foreach (var item in deleteportstuts)
                            //{
                            //    item.ACTIVEFLAG = item.ACTIVEFLAG = "Y";
                            //    _FBB_PORT_INFO.Update(item);
                            //}

                            command.Return_Code = 0;
                            command.Return_Desc = "CARDID Already Existing.";
                            _uow.Persist();


                        }
                        else
                        {


                            UpdateCradModel.ACTIVEFLAG = "N";
                            UpdateCradModel.CARDID = command.CARDID;
                            UpdateCradModel.CARDMODELID = UpdateCradModel.CARDMODELID;
                            UpdateCradModel.CARDNUMBER = command.CradNumber;
                            UpdateCradModel.CREATED_BY = command.UPDATEBY;
                            UpdateCradModel.CREATED_DATE = command.CRATE_DATE;
                            UpdateCradModel.DSLAMID = command.DSalamModelID;
                            UpdateCradModel.RESERVE_TECHNOLOGY = command.RESERVE_TECHNOLOGY;
                            UpdateCradModel.UPDATED_BY = command.UPDATEBY;
                            UpdateCradModel.UPDATED_DATE = command.UPDATE_DATE;
                            if (command.Building != "" && command.Building != null)
                            {
                                UpdateCradModel.BUILDING = command.Building;
                            }



                            _FBB_CARD_INFO.Update(UpdateCradModel);
                            _uow.Persist();

                            command.Return_Code = 1;
                            command.Return_Desc = "Delete Compate.";





                            var description = "";
                            if (command.Building != "" && command.Building != null)
                            {
                                description = "Card Number: " + UpdateCradModel.CARDNUMBER + ", Card Model: " + UpdateCradModel.CARDMODELID + ", Resever  :" + UpdateCradModel.RESERVE_TECHNOLOGY + " ,Building : " + UpdateCradModel.BUILDING;
                            }
                            else
                            {
                                description = "Card Number: " + UpdateCradModel.CARDNUMBER + ", Card Model: " + UpdateCradModel.CARDMODELID + ", Resever  :" + UpdateCradModel.RESERVE_TECHNOLOGY;
                            }






                            var historyLog = new FBB_HISTORY_LOG();
                            historyLog.ACTION = ActionHistory.DELETE.ToString();
                            historyLog.APPLICATION = "FBB_CFG001_4_Card_Port";
                            historyLog.CREATED_BY = command.UPDATEBY;
                            historyLog.CREATED_DATE = DateTime.Now;
                            historyLog.DESCRIPTION = description;
                            historyLog.REF_KEY = command.Node_ID;
                            historyLog.REF_NAME = "NODEID";

                            if (description != string.Empty)
                            {
                                _historyLog.Create(historyLog);
                                _uow.Persist();
                            }


                        }










                    }
                }









            }
            catch (Exception ex)
            {
                _logger.Info("Error occured when handle CoverageResultCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }
        }
    }
}
#endregion Update_CARDMODEService