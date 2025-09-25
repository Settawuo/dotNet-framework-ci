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
    public class CoverageDSLAMBuildingCommandHandler : ICommandHandler<CoverageDSLAMBuildingCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _FBB_COVERAGEAREA_BUILDING;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public CoverageDSLAMBuildingCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION,
            IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
            IEntityRepository<FBB_COVERAGEAREA_BUILDING> FBB_COVERAGEAREA_BUILDING,
            IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA,
            IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _FBB_COVERAGEAREA_RELATION = FBB_COVERAGEAREA_RELATION;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_COVERAGEAREA_BUILDING = FBB_COVERAGEAREA_BUILDING;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
            _historyLog = historyLog;
        }

        public void Handle(CoverageDSLAMBuildingCommand command)
        {
            try
            {
                if (command.Action == "Create")
                {
                    bool isUpdate = false;
                    foreach (var a in command.BuildingUse.Split(','))
                    {
                        var dup = from r in _FBB_COVERAGEAREA_RELATION.Get()
                                  where r.CVRID == command.CVRID && r.DSLAMID == command.DSLAMID && r.TOWERNAME_EN == a && r.ACTIVEFLAG == "Y"
                                  select r;

                        if (dup.Any())
                            command.FlagDup += a + ", ";

                        else
                        {
                            isUpdate = true;

                            var covBuilding = (from r in _FBB_COVERAGEAREA_BUILDING.Get()
                                               where r.CONTACT_ID == command.ContactID && r.BUILDING == a && r.ACTIVE_FLAG == "Y"
                                               select r).FirstOrDefault();

                            var covArea = (from r in _FBB_COVERAGEAREA.Get()
                                           where r.CVRID == command.CVRID
                                           select r).FirstOrDefault();

                            var fbbCovRelation = new FBB_COVERAGEAREA_RELATION
                            {
                                CREATED_BY = command.Username,
                                CREATED_DATE = DateTime.Now,
                                UPDATED_BY = command.Username,
                                UPDATED_DATE = DateTime.Now,
                                CVRID = command.CVRID,
                                DSLAMID = command.DSLAMID,
                                TOWERNAME_TH = covBuilding.BUILDING_TH,
                                TOWERNAME_EN = covBuilding.BUILDING_EN,
                                ACTIVEFLAG = "Y",
                                LATITUDE = covArea.LATITUDE,
                                LONGITUDE = covArea.LONGITUDE
                            };
                            _FBB_COVERAGEAREA_RELATION.Create(fbbCovRelation);

                            _uow.Persist();

                            #region Insert FBB_HISTORY_LOG
                            var historyLogItem = new FBB_HISTORY_LOG();

                            historyLogItem.ACTION = ActionHistory.ADD.ToString();
                            historyLogItem.APPLICATION = "FBB_CFG001_3_DSLAM_RELATION";
                            historyLogItem.CREATED_BY = command.Username;
                            historyLogItem.CREATED_DATE = DateTime.Now;
                            historyLogItem.DESCRIPTION = "Building Code: " + command.BuildingCode + ", Building Use: " + command.BuildingUse + ", NodeId: " + command.NodeId;
                            historyLogItem.REF_KEY = command.NodeNameTH;
                            historyLogItem.REF_NAME = "Node Name TH";
                            _historyLog.Create(historyLogItem);
                            _uow.Persist();
                            #endregion
                        }
                    }

                    if (command.Type == "D" && isUpdate)
                    {
                        var dslamInfo = _FBB_DSLAM_INFO.Get().FirstOrDefault(p => p.DSLAMID == command.DSLAMID);
                        dslamInfo.UPDATED_BY = command.Username;
                        dslamInfo.UPDATED_DATE = DateTime.Now;
                        dslamInfo.DSLAMNUMBER = command.DSLAMNo;
                        dslamInfo.NODEID = command.NodeId;
                        dslamInfo.CVRID = command.CVRID;

                        _FBB_DSLAM_INFO.Update(dslamInfo);
                    }

                    _uow.Persist();
                }

                else if (command.Action == "Delete")
                {
                    var covRelation = _FBB_COVERAGEAREA_RELATION.Get().FirstOrDefault(p => p.CVRRELATIONID == command.CVRRelationID);
                    covRelation.UPDATED_BY = command.Username;
                    covRelation.UPDATED_DATE = DateTime.Now;
                    covRelation.ACTIVEFLAG = "N";

                    _FBB_COVERAGEAREA_RELATION.Update(covRelation);

                    _uow.Persist();

                    #region Insert FBB_HISTORY_LOG
                    var historyLogItem = new FBB_HISTORY_LOG();

                    historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                    historyLogItem.APPLICATION = "FBB_CFG001_3_DSLAM_RELATION";
                    historyLogItem.CREATED_BY = command.Username;
                    historyLogItem.CREATED_DATE = DateTime.Now;
                    historyLogItem.DESCRIPTION = "Building Code: " + command.BuildingCode + ", Building Use: " + command.BuildingCode + ", NodeId: " + command.NodeId;
                    historyLogItem.REF_KEY = command.NodeNameTH;
                    historyLogItem.REF_NAME = "Node Name TH";
                    _historyLog.Create(historyLogItem);
                    _uow.Persist();
                    #endregion                    
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
