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
    public class CoverageAreaBuildingCommandHandler : ICommandHandler<CoverageAreaBuildingCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _building;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageRelation;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IWBBUnitOfWork _uow;

        public CoverageAreaBuildingCommandHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA_BUILDING> building,
                                                    IEntityRepository<FBB_HISTORY_LOG> historyLog, IEntityRepository<FBB_COVERAGEAREA> coverageArea,
                                                    IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageRelation, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _building = building;
            _historyLog = historyLog;
            _coverageRelation = coverageRelation;
            _coverageArea = coverageArea;
            _uow = uow;
        }

        public void Handle(CoverageAreaBuildingCommand command)
        {
            try
            {
                if (command.ActionType != WBBContract.Commands.ActionType.None)
                {
                    var dateNow = DateTime.Now;
                    var description = "";

                    var coverageAreaBuilding = from b in _building.Get()
                                               where b.BUILDING == command.BUILDING
                                               && b.CONTACT_ID == command.CONTACT_ID
                                               && b.ACTIVE_FLAG == "Y"
                                               select b;

                    if (command.ActionType == WBBContract.Commands.ActionType.Insert)
                    {
                        if (coverageAreaBuilding.Any())
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Building is duplicate, please try again.";
                            return;
                        }
                        else
                        {
                            var building = new FBB_COVERAGEAREA_BUILDING();
                            building.BUILDING_EN = command.BUILDING_EN.ToSafeString();
                            building.BUILDING_TH = command.BUILDING_TH.ToSafeString();
                            building.INSTALL_NOTE = command.INSTALLNOTE.ToSafeString();
                            building.CONTACT_ID = command.CONTACT_ID;
                            building.UPDATED_BY = command.CREATED_BY.ToSafeString();
                            building.UPDATED_DATE = dateNow;
                            building.ACTIVE_FLAG = "Y";
                            building.BUILDING = command.BUILDING.ToSafeString();
                            building.CREATED_BY = command.CREATED_BY.ToSafeString();
                            building.CREATED_DATE = dateNow;
                            _building.Create(building);

                            description = "Building: " + building.BUILDING;
                        }
                    }
                    else
                    {
                        if (coverageAreaBuilding.Any())
                        {
                            var building = coverageAreaBuilding.FirstOrDefault();

                            if (command.ActionType == WBBContract.Commands.ActionType.Update)
                            {
                                var oldObject = new FBB_COVERAGEAREA_BUILDING()
                                {
                                    ACTIVE_FLAG = building.ACTIVE_FLAG,
                                    BUILDING = building.BUILDING,
                                    BUILDING_EN = building.BUILDING_EN,
                                    BUILDING_TH = building.BUILDING_TH,
                                    INSTALL_NOTE = building.INSTALL_NOTE,
                                    CONTACT_ID = building.CONTACT_ID,
                                    CREATED_BY = building.CREATED_BY,
                                    CREATED_DATE = building.CREATED_DATE,
                                    UPDATED_BY = building.UPDATED_BY,
                                    UPDATED_DATE = building.UPDATED_DATE
                                };

                                building.ACTIVE_FLAG = "Y";
                                //building.BUILDING = command.BUILDING.ToSafeString();
                                building.BUILDING_EN = command.BUILDING_EN.ToSafeString();
                                building.BUILDING_TH = command.BUILDING_TH.ToSafeString();
                                building.INSTALL_NOTE = command.INSTALLNOTE.ToSafeString();
                                building.UPDATED_BY = command.CREATED_BY.ToSafeString();
                                building.UPDATED_DATE = dateNow;

                                description = "Building: " + oldObject.BUILDING + " => " + WBBExtensions.CompareObjectToString(oldObject, building);
                            }
                            else if (command.ActionType == WBBContract.Commands.ActionType.Delete)
                            {
                                #region  sql
                                //select cb.building from fbb_coveragearea_building cb
                                //where 
                                //cb.contact_id = 1
                                //and cb.active_flag = 'Y'
                                //and cb.building_en not in 
                                //(select cr.towername_en from fbb_coveragearea_relation cr,fbb_coveragearea c
                                //where 
                                //c.cvrid=cr.cvrid
                                //and c.activeflag = 'Y'
                                //and cr.activeflag = 'Y'
                                //and c.contact_id = 1)
                                #endregion

                                var listCoverageRelation = (from ca in _coverageArea.Get()
                                                            join cr in _coverageRelation.Get() on ca.CVRID equals cr.CVRID
                                                            where ca.ACTIVEFLAG == "Y" && cr.ACTIVEFLAG == "Y"
                                                            && ca.CONTACT_ID == command.CONTACT_ID
                                                            select cr.TOWERNAME_EN);

                                var listBuilding = new List<string>();

                                if (listCoverageRelation.Any())
                                {
                                    listBuilding = (from cb in _building.Get()
                                                    where cb.CONTACT_ID == command.CONTACT_ID && cb.ACTIVE_FLAG == "Y"
                                                    && !listCoverageRelation.Contains(cb.BUILDING) && cb.BUILDING == command.BUILDING
                                                    select cb.BUILDING).ToList();
                                }

                                if (listBuilding.Count() > 0)
                                {
                                    building.UPDATED_BY = command.CREATED_BY.ToSafeString();
                                    building.UPDATED_DATE = dateNow;
                                    building.ACTIVE_FLAG = "N";

                                    description = "Building: " + building.BUILDING;
                                }
                                else
                                {
                                    command.Return_Code = 0;
                                    command.Return_Desc = "Cannot delete because this building is using.";
                                    return;
                                }
                            }

                            _building.Update(building);
                        }
                        else
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Cannot find the data for " + command.ActionType.ToString();
                            return;
                        }
                    }

                    _uow.Persist();

                    #region Insert FBB_HISTORY_LOG
                    var historyLogItem = new FBB_HISTORY_LOG();

                    switch (command.ActionType)
                    {
                        case WBBContract.Commands.ActionType.Insert:
                            historyLogItem.ACTION = ActionHistory.ADD.ToString();
                            break;
                        case WBBContract.Commands.ActionType.Update:
                            historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                            break;
                        case WBBContract.Commands.ActionType.Delete:
                            historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                            break;
                        default:
                            historyLogItem.ACTION = ActionHistory.NONE.ToString();
                            break;
                    };

                    historyLogItem.APPLICATION = "FBB_CFG001_2_Building";
                    historyLogItem.CREATED_BY = command.CREATED_BY.ToSafeString();
                    historyLogItem.CREATED_DATE = dateNow;
                    historyLogItem.DESCRIPTION = description;
                    historyLogItem.REF_KEY = command.RefKey.ToSafeString();
                    historyLogItem.REF_NAME = "Node Name TH";

                    if (command.ActionType == WBBContract.Commands.ActionType.Update)
                    {
                        if (description != string.Empty)
                        {
                            _historyLog.Create(historyLogItem);
                            _uow.Persist();
                        }
                    }
                    else
                    {
                        _historyLog.Create(historyLogItem);
                        _uow.Persist();
                    }

                    #endregion

                    command.Return_Code = 1;
                    command.Return_Desc = command.ActionType.ToString() + "d complete.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                command.Return_Code = -1;
                command.Return_Desc = "Error handle CoverageAreaBuilding, Action : " + command.ActionType.ToString();
            }
        }
    }
}
