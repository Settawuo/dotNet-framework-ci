using System;
using System.Linq;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public enum PortStatus
    {
        None = 0,
        Available = 1,
        Reserved = 2,
        Active = 3,
        OutOfService = 4,
        PendingTerminate = 5,
    }

    public class PortAvaliableQuery
    {
        public bool FlagOnlineNo { get; set; }
        public decimal Cvrid { get; set; }
        public IEntityRepository<FBB_COVERAGEAREA> CovService { get; set; }
        public IEntityRepository<FBB_COVERAGEAREA_RELATION> CovRelService { get; set; }
        public IEntityRepository<FBB_DSLAM_INFO> DslamInfoService { get; set; }
        public IEntityRepository<FBB_CARD_INFO> CardInfoService { get; set; }
        public IEntityRepository<FBB_CARDMODEL> CardModelService { get; set; }
        public IEntityRepository<FBB_PORT_INFO> PortInfoService { get; set; }
        public string Tower { get; set; }
        public IEntityRepository<FBB_COVERAGEAREA_BUILDING> CovAreaBuildingService { get; set; }
        public ILogger Logger { get; set; }
        public bool FlagFromWorkFlow { get; set; }
    }

    public class PortNumberQuery
    {
        public bool FlagOnlineNo { get; set; }
        public string Technology { get; set; }
        public decimal Cvrid { get; set; }
        public IEntityRepository<FBB_COVERAGEAREA> CovService { get; set; }
        public IEntityRepository<FBB_COVERAGEAREA_RELATION> CovRelService { get; set; }
        public IEntityRepository<FBB_DSLAM_INFO> DslamInfoService { get; set; }
        public IEntityRepository<FBB_CARD_INFO> CardInfoService { get; set; }
        public IEntityRepository<FBB_CARDMODEL> CardModelService { get; set; }
        public IEntityRepository<FBB_PORT_INFO> PortInfoService { get; set; }
        public string Tower { get; set; }
        public IEntityRepository<FBB_COVERAGEAREA_BUILDING> CovAreaBuildingService { get; set; }

        public bool IsDescending { get; set; }
    }

    public static class ExWebServiceHelper
    {
        public static bool PortAvaliable(PortAvaliableQuery portAvaQuery)
        {
            var avaliable = false;

            //check Tie
            var chkTie = (from c in portAvaQuery.CovService.Get()
                          where c.TIE_FLAG == "Y" && c.CVRID == portAvaQuery.Cvrid
                          select c);

            portAvaQuery.Logger.Info("CVR: " + portAvaQuery.Cvrid
                + " ,Tower: " + portAvaQuery.Tower
                + " ,Tie: " + chkTie
                + " ,Flag Online No: " + portAvaQuery.FlagOnlineNo);

            var dslamId = from r in portAvaQuery.CovRelService.Get()
                          where r.CVRID == portAvaQuery.Cvrid && r.ACTIVEFLAG == "Y"
                          select r.DSLAMID;

            // clean code

            var data = (from c in portAvaQuery.CovService.Get()
                        join d in portAvaQuery.DslamInfoService.Get() on c.CVRID equals d.CVRID
                        join ci in portAvaQuery.CardInfoService.Get() on d.DSLAMID equals ci.DSLAMID
                        join cm in portAvaQuery.CardModelService.Get() on ci.CARDMODELID equals cm.CARDMODELID
                        join pi in portAvaQuery.PortInfoService.Get() on ci.CARDID equals pi.CARDID
                        where c.CVRID == portAvaQuery.Cvrid
                            && dslamId.Contains(d.DSLAMID)
                            && c.NODESTATUS == "ON_SITE"
                            && pi.PORTSTATUSID == (decimal)PortStatus.Available
                            && c.ACTIVEFLAG == "Y"
                            && d.ACTIVEFLAG == "Y"
                            && ci.ACTIVEFLAG == "Y"
                            && pi.ACTIVEFLAG == "Y"
                        select new { c, d, ci, cm, pi });

            if (chkTie.Any())
            {
                var buildingName = (from c in
                                        (from t in portAvaQuery.CovService.Get()
                                         join t2 in portAvaQuery.CovRelService.Get() on t.CVRID equals t2.CVRID
                                         where t2.ACTIVEFLAG == "Y"
                                            && t.ACTIVEFLAG == "Y"
                                         select t)
                                    join cb in portAvaQuery.CovAreaBuildingService.Get() on c.CONTACT_ID equals cb.CONTACT_ID
                                    where cb.ACTIVE_FLAG == "Y"
                                        && (cb.BUILDING_TH == portAvaQuery.Tower || cb.BUILDING_EN == portAvaQuery.Tower)
                                    select cb.BUILDING)
                                .FirstOrDefault();

                var filterByBuilding = (from t in data where t.ci.BUILDING == buildingName select t);
                data = null;
                data = filterByBuilding;
            }

            avaliable = (from t in data
                         where (!portAvaQuery.FlagOnlineNo
                                    || (portAvaQuery.FlagOnlineNo && t.cm.DATAONLY_FLAG == "N"))
                         && (portAvaQuery.FlagFromWorkFlow
                                    || (!portAvaQuery.FlagFromWorkFlow && t.pi.PORTTYPE == "SELL"))
                         select t).Any();

            return avaliable;
        }

        public static PortValueModel GetPortNo(PortNumberQuery portNoQuery)
        {
            var flagOnlineNumber = portNoQuery.FlagOnlineNo.ToYesNoFlgString();

            var chkTie = (from c in portNoQuery.CovService.Get()
                          where c.TIE_FLAG == "Y" && c.CVRID == portNoQuery.Cvrid
                          select c);

            var dslamId = from r in portNoQuery.CovRelService.Get()
                          where r.CVRID == portNoQuery.Cvrid && r.ACTIVEFLAG == "Y"
                          select r.DSLAMID;

            //IQueryable<PortValueModel> data = null;

            // clean code and add descending
            var queryData = (from c in portNoQuery.CovService.Get()
                             join d in portNoQuery.DslamInfoService.Get() on c.CVRID equals d.CVRID
                             join ci in portNoQuery.CardInfoService.Get() on d.DSLAMID equals ci.DSLAMID
                             join cm in portNoQuery.CardModelService.Get() on ci.CARDMODELID equals cm.CARDMODELID
                             join pi in portNoQuery.PortInfoService.Get() on ci.CARDID equals pi.CARDID
                             orderby d.DSLAMNUMBER, ci.CARDNUMBER, pi.PORTNUMBER
                             where c.CVRID == portNoQuery.Cvrid
                                 && dslamId.Contains(d.DSLAMID)
                                 && c.NODESTATUS == "ON_SITE"
                                 && pi.PORTSTATUSID == (decimal)PortStatus.Available
                                 && c.ACTIVEFLAG == "Y"
                                 && d.ACTIVEFLAG == "Y"
                                 && ci.ACTIVEFLAG == "Y"
                                 && pi.ACTIVEFLAG == "Y"
                             select new { c, d, ci, cm, pi });

            if (portNoQuery.IsDescending)
            {
                var orderDesc = (from t in queryData
                                 orderby t.d.DSLAMNUMBER descending,
                                      t.ci.CARDNUMBER descending,
                                      t.pi.PORTNUMBER descending
                                 select t);
                queryData = orderDesc;
            }

            if (chkTie.Any())
            {
                var buildingName = (from c in
                                        (from t in portNoQuery.CovService.Get()
                                         join t2 in portNoQuery.CovRelService.Get() on t.CVRID equals t2.CVRID
                                         where t2.ACTIVEFLAG == "Y"
                                            && t.ACTIVEFLAG == "Y"
                                         select t)
                                    join cb in portNoQuery.CovAreaBuildingService.Get() on c.CONTACT_ID equals cb.CONTACT_ID
                                    where cb.ACTIVE_FLAG == "Y"
                                        && (cb.BUILDING_TH == portNoQuery.Tower || cb.BUILDING_EN == portNoQuery.Tower)
                                    select cb.BUILDING)
                                .FirstOrDefault();

                var filterByBuilding = (from t in queryData where t.ci.BUILDING == buildingName select t);
                queryData = filterByBuilding;
            }

            var data = (from t in queryData
                        select new PortValueModel
                        {
                            PortId = t.pi.PORTID,
                            DSLAMNumber = t.d.DSLAMNUMBER,
                            CardNumber = t.ci.CARDNUMBER,
                            PortNumber = t.pi.PORTNUMBER,
                            PortType = t.pi.PORTTYPE,
                            NetWorkTechnology = t.ci.RESERVE_TECHNOLOGY,
                            DataOnlyFlag = t.cm.DATAONLY_FLAG,
                            NODEId = t.d.NODEID,
                            MaxPort = t.cm.MAXPORT
                        });

            if (!data.Any())
                return null;

            var dataOnlyFlag = (!portNoQuery.FlagOnlineNo).ToYesNoFlgString();

            if (flagOnlineNumber == "N" && portNoQuery.Technology == "ADSL")
            {
                #region logic 2.1
                if (dataOnlyFlag == "Y")
                {

                    var a1 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "ADSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a1.Any())
                        return a1.FirstOrDefault();

                    var a2 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "XDSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a2.Any())
                        return a2.FirstOrDefault();

                    var a3 = from r in data
                             where r.PortType == "SELL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a3.Any())
                        return a3.FirstOrDefault();
                }
                var a4 = from r in data
                         where r.PortType == "SELL" && r.NetWorkTechnology == "ADSL"
                         select r;
                if (a4.Any())
                    return a4.FirstOrDefault();

                var a5 = from r in data
                         where r.PortType == "SELL" && r.NetWorkTechnology == "XDSL"
                         select r;
                if (a5.Any())
                    return a5.FirstOrDefault();

                var a6 = from r in data
                         where r.PortType == "SELL"
                         select r;
                if (a6.Any())
                    return a6.FirstOrDefault();

                var a7 = from r in data
                         where r.PortType == "SPARE"
                         select r;
                if (a7.Any())
                    return a7.FirstOrDefault();
                #endregion
            }
            else if (flagOnlineNumber == "Y" && portNoQuery.Technology == "ADSL")
            {
                #region logic 2.2
                if (dataOnlyFlag == "N")
                {

                    var a1 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "ADSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a1.Any())
                        return a1.FirstOrDefault();

                    var a2 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "XDSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a2.Any())
                        return a2.FirstOrDefault();

                    var a3 = from r in data
                             where r.PortType == "SELL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a3.Any())
                        return a3.FirstOrDefault();

                    var a4 = from r in data
                             where r.PortType == "SPARE" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a4.Any())
                        return a4.FirstOrDefault();
                }
                #endregion
            }
            else if (flagOnlineNumber == "N" && portNoQuery.Technology == "VDSL")
            {
                #region logic 2.3
                if (dataOnlyFlag == "Y")
                {

                    var a1 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "VDSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a1.Any())
                        return a1.FirstOrDefault();

                    var a2 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "XDSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a2.Any())
                        return a2.FirstOrDefault();

                    var a3 = from r in data
                             where r.PortType == "SELL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a3.Any())
                        return a3.FirstOrDefault();
                }
                var a4 = from r in data
                         where r.PortType == "SELL" && r.NetWorkTechnology == "VDSL"
                         select r;
                if (a4.Any())
                    return a4.FirstOrDefault();

                var a5 = from r in data
                         where r.PortType == "SELL" && r.NetWorkTechnology == "XDSL"
                         select r;
                if (a5.Any())
                    return a5.FirstOrDefault();

                var a6 = from r in data
                         where r.PortType == "SELL"
                         select r;
                if (a6.Any())
                    return a6.FirstOrDefault();

                var a7 = from r in data
                         where r.PortType == "SPARE"
                         select r;
                if (a7.Any())
                    return a7.FirstOrDefault();
                #endregion
            }
            else if (flagOnlineNumber == "Y" && portNoQuery.Technology == "VDSL")
            {
                #region logic 2.4
                if (dataOnlyFlag == "N")
                {

                    var a1 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "VDSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a1.Any())
                        return a1.FirstOrDefault();
                }

                if (dataOnlyFlag == "Y")
                {
                    var a2 = from r in data
                             where r.PortType == "SELL" && r.NetWorkTechnology == "XDSL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a2.Any())
                        return a2.FirstOrDefault();
                }

                if (dataOnlyFlag == "N")
                {
                    var a3 = from r in data
                             where r.PortType == "SELL" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a3.Any())
                        return a3.FirstOrDefault();

                    var a4 = from r in data
                             where r.PortType == "SPARE" && r.DataOnlyFlag == dataOnlyFlag
                             select r;
                    if (a4.Any())
                        return a4.FirstOrDefault();
                }
                #endregion
            }

            return null;
            /* old logic 
            // tech
            var filterByTechData = data
                .Where(d => d.PortType.Equals("SELL")
                    && d.NetWorkTechnology.Equals(portNoQuery.Technology)
                    && d.DataOnlyFlag.Equals(dataOnlyFlag));

            if (filterByTechData.Any())
                return filterByTechData.FirstOrDefault();

            // data only flag
            var filterByDataOnlyFlagData = data
                .Where(d => d.PortType.Equals("SELL")
                    && d.DataOnlyFlag.Equals(dataOnlyFlag));

            if (filterByDataOnlyFlagData.Any())
                return filterByDataOnlyFlagData.FirstOrDefault();

            // spare
            var filterBySpareData = data
                .Where(d => d.PortType.Equals("SPARE")
                    && d.DataOnlyFlag.Equals(dataOnlyFlag));

            if (filterBySpareData.Any())
                return filterBySpareData.FirstOrDefault();

            return data.FirstOrDefault();             
             */
        }

        public static void LogActivePort(int currentPort, string refUser, string refKey,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var portInfToUpdate = portInfoService.Get(p => p.PORTID == (decimal)currentPort).FirstOrDefault();
            if (null == portInfToUpdate)
                throw new Exception("Cannot find port info.");

            // reserved
            portInfToUpdate.PORTSTATUSID = 3;
            portInfToUpdate.UPDATED_BY = refUser;
            portInfToUpdate.UPDATED_DATE = DateTime.Now;

            portInfoService.Update(portInfToUpdate);

            var newPortNote = new FBB_PORT_NOTE();
            newPortNote.PORTID = portInfToUpdate.PORTID;
            newPortNote.NOTE = "ActivePort : Change Status Active";
            newPortNote.CREATED_BY = refUser;
            newPortNote.CREATED_DATE = DateTime.Now;
            newPortNote.REF_KEY = refKey;

            portNoteService.Create(newPortNote);
            uow.Persist();
        }

        public static void LogAvaliablePort(int currentPort, string refUser, string refKey,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var portInfToUpdate = portInfoService.Get(p => p.PORTID == (decimal)currentPort).FirstOrDefault();
            if (null == portInfToUpdate)
                throw new Exception("Cannot find port info.");

            // reserved
            portInfToUpdate.PORTSTATUSID = 1;
            portInfToUpdate.UPDATED_BY = refUser;
            portInfToUpdate.UPDATED_DATE = DateTime.Now;

            portInfoService.Update(portInfToUpdate);

            var newPortNote = new FBB_PORT_NOTE();
            newPortNote.PORTID = portInfToUpdate.PORTID;
            newPortNote.NOTE = "AvaliablePort : Change Status Avaliable";
            newPortNote.CREATED_BY = refUser;
            newPortNote.CREATED_DATE = DateTime.Now;
            newPortNote.REF_KEY = refKey;

            portNoteService.Create(newPortNote);
            uow.Persist();
        }

        public static void LogAssignNewPort(PortValueModel port, string refUser, string refKey,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var portInfToUpdate = portInfoService.Get(p => p.PORTID == port.PortId).FirstOrDefault();
            if (null == portInfToUpdate)
                throw new Exception("Cannot find port info.");

            // reserved
            portInfToUpdate.PORTSTATUSID = 2;
            portInfToUpdate.UPDATED_BY = refUser;
            portInfToUpdate.UPDATED_DATE = DateTime.Now;
            portInfoService.Update(portInfToUpdate);

            var newPortNote = new FBB_PORT_NOTE();
            newPortNote.PORTID = port.PortId;
            newPortNote.NOTE = "AssignNewPort : Change Status Reserved";
            newPortNote.CREATED_BY = refUser;
            newPortNote.CREATED_DATE = DateTime.Now;
            newPortNote.REF_KEY = refKey;

            portNoteService.Create(newPortNote);
            uow.Persist();
        }

        /// <summary>
        /// change the old port's status to specific status
        /// </summary>
        /// <param name="port"></param>
        /// <param name="refUser"></param>
        /// <param name="refKey"></param>
        /// <param name="uow"></param>
        /// <param name="portInfoService"></param>
        /// <param name="portNoteService"></param>
        public static void LogAfterChangePort(decimal oldPortId, PortStatus portStatus, string refUser, string refKey,
           IWBBUnitOfWork uow,
           IEntityRepository<FBB_PORT_INFO> portInfoService,
           IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var cPortInfToUpdate = portInfoService.Get(p => p.PORTID == oldPortId).FirstOrDefault();
            if (null == cPortInfToUpdate)
                throw new Exception("Cannot find current port info.");

            // Available
            cPortInfToUpdate.PORTSTATUSID = (decimal)portStatus;
            cPortInfToUpdate.UPDATED_BY = refUser;
            cPortInfToUpdate.UPDATED_DATE = DateTime.Now;
            portInfoService.Update(cPortInfToUpdate);

            var newPortNote2 = new FBB_PORT_NOTE();
            newPortNote2.PORTID = cPortInfToUpdate.PORTID;
            newPortNote2.NOTE = "ChangePort : Change Status " + portStatus.ToSafeString();
            newPortNote2.CREATED_BY = refUser;
            newPortNote2.CREATED_DATE = DateTime.Now;
            newPortNote2.REF_KEY = refKey;

            portNoteService.Create(newPortNote2);
            uow.Persist();
        }

        /// <summary>
        /// Change the return port's status to specific status
        /// </summary>
        /// <param name="port"></param>
        /// <param name="refUser"></param>
        /// <param name="refKey"></param>
        /// <param name="uow"></param>
        /// <param name="portInfoService"></param>
        /// <param name="portNoteService"></param>
        public static void LogBeforeChangePort(PortValueModel port,
            PortStatus portStatus, string refUser, string refKey,
           IWBBUnitOfWork uow,
           IEntityRepository<FBB_PORT_INFO> portInfoService,
           IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var portInfToUpdate = portInfoService.Get(p => p.PORTID == port.PortId).FirstOrDefault();
            if (null == portInfToUpdate)
                throw new Exception("Cannot find port info.");

            // reserved
            portInfToUpdate.PORTSTATUSID = (decimal)portStatus;
            portInfToUpdate.UPDATED_BY = refUser;
            portInfToUpdate.UPDATED_DATE = DateTime.Now;
            portInfoService.Update(portInfToUpdate);

            var newPortNote = new FBB_PORT_NOTE();
            newPortNote.PORTID = portInfToUpdate.PORTID;
            newPortNote.NOTE = "ChangePort : Change Status " + portStatus.ToSafeString();
            newPortNote.CREATED_BY = refUser;
            newPortNote.CREATED_DATE = DateTime.Now;
            newPortNote.REF_KEY = refKey;

            portNoteService.Create(newPortNote);
            uow.Persist();
        }

        public static void LogAfterChangePortFail(int currentPortId, string refUser, string refKey,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var cPortInfToUpdate = portInfoService.Get(p => p.PORTID == (decimal)currentPortId).FirstOrDefault();
            if (null == cPortInfToUpdate)
                throw new Exception("Cannot find current port info.");

            // out of service
            cPortInfToUpdate.PORTSTATUSID = 4;
            cPortInfToUpdate.UPDATED_BY = refUser;
            cPortInfToUpdate.UPDATED_DATE = DateTime.Now;
            portInfoService.Update(cPortInfToUpdate);

            var newPortNote2 = new FBB_PORT_NOTE();
            newPortNote2.PORTID = cPortInfToUpdate.PORTID;
            newPortNote2.NOTE = "ChangePortFail : Change Status Out Of Service";
            newPortNote2.CREATED_BY = refUser;
            newPortNote2.CREATED_DATE = DateTime.Now;
            newPortNote2.REF_KEY = refKey;

            portNoteService.Create(newPortNote2);
            uow.Persist();
        }

        public static void LogBeforeChangePortFail(PortValueModel port, string refUser, string refKey,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var portInfToUpdate = portInfoService.Get(p => p.PORTID == port.PortId).FirstOrDefault();
            if (null == portInfToUpdate)
                throw new Exception("Cannot find port info.");

            // reserved
            portInfToUpdate.PORTSTATUSID = 2;
            portInfToUpdate.UPDATED_BY = refUser;
            portInfToUpdate.UPDATED_DATE = DateTime.Now;
            portInfoService.Update(portInfToUpdate);

            var newPortNote = new FBB_PORT_NOTE();
            newPortNote.PORTID = portInfToUpdate.PORTID;
            newPortNote.NOTE = "ChangePortFail : Change Status Reserved";
            newPortNote.CREATED_BY = refUser;
            newPortNote.CREATED_DATE = DateTime.Now;
            newPortNote.REF_KEY = refKey;

            portNoteService.Create(newPortNote);
            uow.Persist();
        }

        // new 
        public static void LogReAvailablePort(int currentPort, string refUser, string refKey,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService)
        {
            var portInfToUpdate = portInfoService.Get(p => p.PORTID == currentPort).FirstOrDefault();
            if (null == portInfToUpdate)
                throw new Exception("Cannot find port info.");

            // Available
            portInfToUpdate.PORTSTATUSID = 1;
            portInfToUpdate.UPDATED_BY = refUser;
            portInfToUpdate.UPDATED_DATE = DateTime.Now;
            portInfoService.Update(portInfToUpdate);

            var newPortNote = new FBB_PORT_NOTE();
            newPortNote.PORTID = portInfToUpdate.PORTID;
            newPortNote.NOTE = "ChangePortFail : Change Status Available";
            newPortNote.CREATED_BY = refUser;
            newPortNote.CREATED_DATE = DateTime.Now;
            newPortNote.REF_KEY = refKey;

            portNoteService.Create(newPortNote);
            uow.Persist();
        }
    }
}
