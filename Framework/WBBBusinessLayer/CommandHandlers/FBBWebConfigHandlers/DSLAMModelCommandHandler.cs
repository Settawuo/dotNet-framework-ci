using ObjectDumper;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class DSLAMModelCommandHandler : ICommandHandler<DSLAMModelCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _dslamModel;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public DSLAMModelCommandHandler(ILogger logger,
            IEntityRepository<FBB_DSLAMMODEL> dslamModel,
            IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
            IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _dslamModel = dslamModel;
            _dslamInfo = dslamInfo;
            _historyLog = historyLog;
            _uow = uow;
        }

        public void Handle(DSLAMModelCommand command)
        {
            try
            {
                _logger.Info(command.DumpToString(command.GetType().Name));

                var dateNow = DateTime.Now;
                var description = "";

                var DSLAMMODEL = _dslamModel.Get(c => c.DSLAMMODELID == command.DSLAMMODELID).FirstOrDefault();
                var updatedslammodel = _dslamModel.Get(u => u.DSLAMMODELID == command.DSLAMMODELID).FirstOrDefault();

                if (command.ResultCommand == "ADD") //Insert DSLAMModel -- Advicer: P'Sam, P'Yeen, P'Boy, P'Copter.
                {
                    #region Insert _dslamModel 

                    var MODELID = _dslamModel.Get().Max(r => r.DSLAMMODELID) + 1;

                    var a = from r in _dslamModel.Get() //Check Duplicated After Create DSLAMModel
                            where r.MODEL == command.MODEL && r.ACTIVEFLAG == "Y"
                            select r;
                    if (a.Any())
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Model Already Existing.";
                    }
                    else
                    {

                        _logger.Info("SaveDSLAMModelCommand");

                        var insertdslamModel = new FBB_DSLAMMODEL();
                        // var DSLAMMODELIDMax = _dslamModel.Get(r => r.DSLAMMODELID == command.DSLAMMODELID)
                        insertdslamModel.DSLAMMODELID = MODELID;
                        insertdslamModel.CREATED_BY = command.CREATED_BY;
                        insertdslamModel.CREATED_DATE = command.CREATED_DATE;
                        insertdslamModel.UPDATED_BY = command.UPDATED_BY;
                        insertdslamModel.UPDATED_DATE = command.UPDATED_DATE;

                        insertdslamModel.SLOTSTARTINDEX = command.SLOTSTARTINDEX;
                        insertdslamModel.MAXSLOT = command.MAXSLOT;

                        insertdslamModel.MODEL = command.MODEL;
                        insertdslamModel.BRAND = command.BRAND;
                        insertdslamModel.SH_BRAND = command.SH_BRAND;
                        insertdslamModel.ACTIVEFLAG = "Y";
                        // insertdslamModel.ACTIVEFLAG = command.ACTIVEFLAG;

                        //data.SELECTPRODUCT = command.PRODUCTTYPE;
                        description = "MODEL: " + command.MODEL;
                        //description = insertdslamModel.ObjectToString();
                        //description = "DSLAM Model" + command.MODEL;

                        _dslamModel.Create(insertdslamModel);
                        _uow.Persist();

                        #region Insert FBB_HISTORY_LOG

                        var historyLogItem = new FBB_HISTORY_LOG(); //P'Yeen
                        historyLogItem.ACTION = ActionHistory.ADD.ToString();

                        historyLogItem.APPLICATION = "FBB_CFG003_1";
                        historyLogItem.CREATED_BY = command.CREATED_BY.ToSafeString();
                        historyLogItem.CREATED_DATE = dateNow;
                        historyLogItem.DESCRIPTION = description;
                        historyLogItem.REF_KEY = command.MODEL.ToSafeString();
                        historyLogItem.REF_NAME = "DSLAM MODEL";

                        _historyLog.Create(historyLogItem);
                        _uow.Persist();
                        #endregion

                        command.Return_Code = 1;
                        command.Return_Desc = "Saved Complete";
                    }
                    #endregion Insert FBB_DSLAMMODEL
                }

                else if (command.ResultCommand == "UPDATE") //Update DSLAMModel -- Advicer: P'Sam, P'Yeen, P'Boy, P'Copter.
                {

                    #region Update DSLAMModel 

                    var b = (from r in _dslamInfo.Get()
                                 //Check Update DSLAMModel_1 (Case: Edit Name After Used Already. Check in the DSLAM_Info.)
                             where r.DSLAMMODELID == command.DSLAMMODELID && r.ACTIVEFLAG == "Y"
                             select r).ToList();

                    if (b.Any())
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Cannot Updated because Model Used.";
                    }

                    else
                    {
                        var c = (from r in _dslamModel.Get()
                                     //Check Update DSLAMModel_2 (Case: Edit Name never use in DSLAM_Info. Check in the DSLAM_Model.)
                                 where r.MODEL == command.MODEL && r.ACTIVEFLAG == "Y" && r.DSLAMMODELID != command.DSLAMMODELID
                                 select r).ToList();
                        if (c.Any())
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Model Already Existing.";
                        }
                        else
                        {

                            _logger.Info("Update DSLAM Result.");

                            if (null == DSLAMMODEL)
                            {
                                _logger.Info("Cannot update FBB_DSLAMMODEL with result id = " + command.DSLAMMODELID);
                                return;
                            }

                            var oldDSLAMModel = new FBB_DSLAMMODEL()
                            {
                                UPDATED_BY = updatedslammodel.UPDATED_BY,
                                UPDATED_DATE = updatedslammodel.UPDATED_DATE,
                                SLOTSTARTINDEX = updatedslammodel.SLOTSTARTINDEX,
                                MAXSLOT = updatedslammodel.MAXSLOT,
                                MODEL = updatedslammodel.MODEL,
                                BRAND = updatedslammodel.BRAND,
                                SH_BRAND = updatedslammodel.SH_BRAND,
                                ACTIVEFLAG = updatedslammodel.ACTIVEFLAG,
                                DSLAMMODELID = updatedslammodel.DSLAMMODELID
                            };

                            // updatedslammodel.DSLAMMODELID = command.DSLAMMODELID;
                            // updatedslammodel.CREATED_BY = command.CREATED_BY;
                            // updatedslammodel.CREATED_DATE = command.CREATED_DATE;
                            updatedslammodel.UPDATED_BY = command.UPDATED_BY;
                            updatedslammodel.UPDATED_DATE = command.UPDATED_DATE;

                            updatedslammodel.MODEL = command.MODEL;
                            updatedslammodel.BRAND = command.BRAND;
                            updatedslammodel.SH_BRAND = command.SH_BRAND;

                            updatedslammodel.SLOTSTARTINDEX = command.SLOTSTARTINDEX;
                            updatedslammodel.MAXSLOT = command.MAXSLOT;

                            updatedslammodel.ACTIVEFLAG = "Y";

                            description = "MODEL: " + command.MODEL + " " + "=>" + " " + WBBExtensions.CompareObjectToString(oldDSLAMModel, updatedslammodel); //By N'Kiakez
                            //description = "Model: " + command.MODEL;

                            _dslamModel.Update(updatedslammodel);
                            _uow.Persist();

                            #region INSERT FBB_HISTORY_LOG

                            var historyLogItem = new FBB_HISTORY_LOG(); //P'Yeen
                            historyLogItem.ACTION = ActionHistory.UPDATE.ToString();

                            historyLogItem.APPLICATION = "FBB_CFG003_1";
                            historyLogItem.CREATED_BY = command.CREATED_BY.ToSafeString();
                            historyLogItem.CREATED_DATE = dateNow;
                            historyLogItem.DESCRIPTION = description;
                            historyLogItem.REF_KEY = command.MODEL.ToSafeString();
                            historyLogItem.REF_NAME = "DSLAM MODEL";

                            _historyLog.Update(historyLogItem);

                            if (description != string.Empty)
                            {
                                _historyLog.Create(historyLogItem);
                                _uow.Persist();
                            }
                            #endregion

                            command.Return_Code = 1;
                            command.Return_Desc = "Saved Complete";
                        }

                    }
                    #endregion Update FBB_DSLAMMODEL
                }

                else if (command.ResultCommand == "DELETE") //Delete DSLAMModel -- Advicer: P'Sam, P'Yeen, P'Boy, P'Copter.
                {

                    #region Delete DSLAMModel 

                    //var x = _dslamInfo.Get(r => r.DSLAMMODELID == command.DSLAMMODELID && r.ACTIVEFLAG == "Y").FirstOrDefault().ACTIVEFLAG;

                    var d = from r in _dslamInfo.Get()
                            where r.DSLAMMODELID == command.DSLAMMODELID && r.ACTIVEFLAG == "Y"
                            select r;
                    if (d.Any())
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Cannot deleted because Model Used.";
                    }
                    else
                    {

                        updatedslammodel.DSLAMMODELID = command.DSLAMMODELID;
                        // updatedslammodel.CREATED_BY = command.CREATED_BY;
                        // updatedslammodel.CREATED_DATE = command.CREATED_DATE;
                        updatedslammodel.UPDATED_BY = command.UPDATED_BY;
                        updatedslammodel.UPDATED_DATE = command.UPDATED_DATE;

                        updatedslammodel.SLOTSTARTINDEX = command.SLOTSTARTINDEX;
                        updatedslammodel.MAXSLOT = command.MAXSLOT;

                        updatedslammodel.MODEL = command.MODEL;
                        updatedslammodel.BRAND = command.BRAND;
                        updatedslammodel.SH_BRAND = command.SH_BRAND;

                        updatedslammodel.ACTIVEFLAG = "N";

                        description = "MODEL: " + command.MODEL;

                        _dslamModel.Update(updatedslammodel);
                        _uow.Persist();

                        #region Insert FBB_HISTORY_LOG

                        var historyLogItem = new FBB_HISTORY_LOG(); //P'Yeen
                        historyLogItem.ACTION = ActionHistory.DELETE.ToString();

                        historyLogItem.APPLICATION = "FBB_CFG003_1";
                        historyLogItem.CREATED_BY = command.CREATED_BY.ToSafeString();
                        historyLogItem.CREATED_DATE = dateNow;
                        historyLogItem.DESCRIPTION = description;
                        historyLogItem.REF_KEY = command.MODEL.ToSafeString();
                        historyLogItem.REF_NAME = "DSLAM MODEL";

                        _historyLog.Create(historyLogItem);
                        _uow.Persist();

                        #endregion

                        //if (null == updatedslammodel)
                        //{
                        //    _logger.Info("Cannot delete FBB_DSLAMMODEL with result id = " + command.DSLAMMODELID);
                        //    return;
                        //}

                        command.Return_Code = 1;
                        command.Return_Desc = "Deleted Complete";
                    }
                    #endregion Delete FBB_DSLAMMODEL
                }

                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {

                _logger.Info("Error occured when handle DSLAMModelCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
                //command.Return_Code = -1;
                //command.Return_Desc = "Error call save dslamModel  : " + ex.GetErrorMessage();

            }
        }
    }
}

