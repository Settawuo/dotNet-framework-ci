using ObjectDumper;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SaveCardModelCommandHandler : ICommandHandler<SaveCardModelCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CARDMODEL> _CARDMODEService;
        private readonly IEntityRepository<FBB_CARD_INFO> _FBB_CARD_INFO;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;

        public SaveCardModelCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CARDMODEL> CARDMODEService,
            IEntityRepository<FBB_CARD_INFO> FBB_CARD_INFO,
            IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _CARDMODEService = CARDMODEService;
            _FBB_CARD_INFO = FBB_CARD_INFO;
            _historyLog = historyLog;
        }

        public void Handle(SaveCardModelCommand command)
        {
            try
            {
                _logger.Info(command.DumpToString(command.GetType().Name));

                var dateNow = DateTime.Now;
                var description = "";

                var DSLAMMODELID = _CARDMODEService.Get().Max(d => d.CARDMODELID) + 1;
                var UpdateCradModel = _CARDMODEService.Get(c => c.CARDMODELID == command.CARDMODELID).FirstOrDefault();
                var FlagCardModelInfo = _FBB_CARD_INFO.Get(fb => fb.CARDMODELID == command.CARDMODELID).FirstOrDefault();
                ///var CARDMODEServicedata = _CARDMODEService.Get().ToList();
                var data = new FBB_CARDMODEL();
                if (command.ResultCommand == "ADD")
                {
                    _logger.Info("SaveCardModelCommand");

                    #region Insert_CARDMODELService

                    var MDuplicated = from r in _CARDMODEService.Get() //Check Duplicated After Create DSLAMModel
                                      where r.MODEL == command.MODEL && r.ACTIVEFLAG == "Y"
                                      select r;
                    if (MDuplicated.Any())
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Model Already Existing.";
                    }
                    else
                    {

                        data.CARDMODELID = DSLAMMODELID;
                        data.MODEL = command.MODEL;
                        data.CREATED_BY = command.CREATED_BY;
                        data.CREATED_DATE = command.CREATED_DATE;
                        data.UPDATED_DATE = command.UPDATED_DATE;
                        data.UPDATED_BY = command.UPDATED_BY;
                        data.BRAND = command.BRAND;
                        data.PORTSTARTINDEX = command.POSTSTARTINDEX;
                        data.MAXPORT = command.MAXSLOT;
                        data.RESERVEPORTSPARE = command.RESERVEPORTSPARE;
                        data.DATAONLY_FLAG = command.DATAONLY_FLANG;
                        data.ACTIVEFLAG = "Y";

                        //data.SELECTPRODUCT = command.PRODUCTTYPE;
                        description = "MODEL: " + command.MODEL;


                        _CARDMODEService.Create(data);
                        _uow.Persist();

                        #region Insert FBB_HISTORY_LOG

                        var historyLogItem = new FBB_HISTORY_LOG(); //P'Yeen
                        historyLogItem.ACTION = ActionHistory.ADD.ToString();

                        historyLogItem.APPLICATION = "FBB_CFG004_1";
                        historyLogItem.CREATED_BY = command.CREATED_BY.ToSafeString();
                        historyLogItem.CREATED_DATE = dateNow;
                        historyLogItem.DESCRIPTION = description;
                        historyLogItem.REF_KEY = command.MODEL.ToSafeString();
                        historyLogItem.REF_NAME = "CARD MODEL";

                        _historyLog.Create(historyLogItem);
                        _uow.Persist();
                        #endregion

                        command.Return_Code = 1;
                        command.Return_Desc = "Saved Complete.";

                    }

                }

                #endregion Insert FBB_CARDMODEL

                #region Update_CARDMODELService
                else if (command.CARDMODELID == UpdateCradModel.CARDMODELID && command.ResultCommand == "UPDATE")
                {

                    _logger.Info("Update Coverage Result.");


                    if (DSLAMMODELID <= 0)
                    {
                        _logger.Info("Cannot update FBB_COVERAGEAREA_RESULT with result id = " + command.CARDMODELID);
                        return;
                    }
                    var MUDuplicatedT = (from r in _CARDMODEService.Get() //Check Update CardModel_1
                                         where r.MODEL == command.MODEL && r.ACTIVEFLAG == "Y" && r.CARDMODELID != command.CARDMODELID
                                         select r).ToList();

                    var MUDuplicated = (from r in _FBB_CARD_INFO.Get() //Check Update CardModel_2
                                        where r.CARDMODELID == command.CARDMODELID && r.ACTIVEFLAG == "Y"
                                        select r).ToList();

                    if (MUDuplicated.Any())
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Cannot Updated because Model Used.";
                    }
                    else if (MUDuplicatedT.Any())
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Model Already Existing.";

                    }
                    else
                    {
                        //string databutton = "";

                        //if (command.DATAONLY_FLANG == "Y")
                        //{
                        //    databutton = "Data+Voice";
                        //}
                        //else if (command.DATAONLY_FLANG == "N")
                        //{
                        //    databutton = "DATA";
                        //}

                        var oldCardModel = new FBB_CARDMODEL()
                        {
                            UPDATED_BY = UpdateCradModel.UPDATED_BY,
                            UPDATED_DATE = UpdateCradModel.UPDATED_DATE,
                            PORTSTARTINDEX = UpdateCradModel.PORTSTARTINDEX,
                            MAXPORT = UpdateCradModel.MAXPORT,
                            RESERVEPORTSPARE = UpdateCradModel.RESERVEPORTSPARE,
                            MODEL = UpdateCradModel.MODEL,
                            BRAND = UpdateCradModel.BRAND,
                            ACTIVEFLAG = UpdateCradModel.ACTIVEFLAG,
                            DATAONLY_FLAG = UpdateCradModel.DATAONLY_FLAG == "Y" ? "Data" : "Data+Voice",
                            CARDMODELID = UpdateCradModel.CARDMODELID
                        };

                        UpdateCradModel.UPDATED_BY = command.UPDATED_BY;
                        UpdateCradModel.UPDATED_DATE = command.UPDATED_DATE;
                        UpdateCradModel.MODEL = command.MODEL;
                        UpdateCradModel.BRAND = command.BRAND;
                        UpdateCradModel.PORTSTARTINDEX = command.POSTSTARTINDEX;
                        UpdateCradModel.MAXPORT = command.MAXSLOT;
                        UpdateCradModel.RESERVEPORTSPARE = command.RESERVEPORTSPARE;
                        UpdateCradModel.DATAONLY_FLAG = command.DATAONLY_FLANG;
                        UpdateCradModel.ACTIVEFLAG = "Y";

                        _CARDMODEService.Update(UpdateCradModel);
                        _uow.Persist();


                        UpdateCradModel.DATAONLY_FLAG = UpdateCradModel.DATAONLY_FLAG == "Y" ? "Data" : "Data+Voice";
                        description = "MODEL: " + command.MODEL + " => " + WBBExtensions.CompareObjectToString(oldCardModel, UpdateCradModel);

                        #region INSERT FBB_HISTORY_LOG

                        var historyLogItem = new FBB_HISTORY_LOG(); //P'Yeen
                        historyLogItem.ACTION = ActionHistory.UPDATE.ToString();

                        historyLogItem.APPLICATION = "FBB_CFG004_1";
                        historyLogItem.CREATED_BY = command.CREATED_BY.ToSafeString();
                        historyLogItem.CREATED_DATE = dateNow;
                        historyLogItem.DESCRIPTION = description;
                        historyLogItem.REF_KEY = command.MODEL.ToSafeString();
                        historyLogItem.REF_NAME = "CARD MODEL";

                        _historyLog.Update(historyLogItem);

                        if (description != string.Empty)
                        {
                            _historyLog.Create(historyLogItem);
                            _uow.Persist();
                        }

                        #endregion

                        command.Return_Code = 1;
                        command.Return_Desc = "Saved Complete.";
                    }

                }

                #endregion Update FBB_CARDMODEL

                #region Delete_CARDMODELService

                else
                {

                    if (command.ResultCommand == "DELETE")
                    {

                        var deleteDuplicated = (from r in _FBB_CARD_INFO.Get()
                                                where r.CARDMODELID == command.CARDMODELID && r.ACTIVEFLAG == "Y"
                                                select r).ToList();
                        //var checkdataportstutas=(from rr in _);
                        if (deleteDuplicated.Any())
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Cannot delete because Model Used.";
                        }
                        else
                        {

                            UpdateCradModel.CARDMODELID = command.CARDMODELID;
                            //UpdateCradModel.CREATED_BY = command.CREATED_BY;
                            //UpdateCradModel.CREATED_DATE = command.CREATED_DATE;
                            UpdateCradModel.UPDATED_BY = command.UPDATED_BY;
                            UpdateCradModel.UPDATED_DATE = command.UPDATED_DATE;
                            UpdateCradModel.MODEL = command.MODEL;
                            UpdateCradModel.BRAND = command.BRAND;
                            UpdateCradModel.PORTSTARTINDEX = command.POSTSTARTINDEX;
                            UpdateCradModel.MAXPORT = command.MAXSLOT;
                            UpdateCradModel.RESERVEPORTSPARE = 1;
                            UpdateCradModel.DATAONLY_FLAG = command.DATAONLY_FLANG;
                            UpdateCradModel.ACTIVEFLAG = "N";

                            description = "MODEL: " + command.MODEL;

                            _CARDMODEService.Update(UpdateCradModel);
                            _uow.Persist();

                            #region Insert FBB_HISTORY_LOG

                            var historyLogItem = new FBB_HISTORY_LOG(); //P'Yeen
                            historyLogItem.ACTION = ActionHistory.DELETE.ToString();

                            historyLogItem.APPLICATION = "FBB_CFG004_1";
                            historyLogItem.CREATED_BY = command.CREATED_BY.ToSafeString();
                            historyLogItem.CREATED_DATE = dateNow;
                            historyLogItem.DESCRIPTION = description;
                            historyLogItem.REF_KEY = command.MODEL.ToSafeString();
                            historyLogItem.REF_NAME = "CARD MODEL";

                            _historyLog.Create(historyLogItem);
                            _uow.Persist();

                            #endregion

                            command.Return_Code = 1;
                            command.Return_Desc = "Delete Complete.";

                        }

                    }

                }

                #endregion Delete FBB_CARDMODEL
            }

            catch (Exception ex)
            {

                _logger.Info("Error occured when handle CardModelCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);

            }
        }
    }
}