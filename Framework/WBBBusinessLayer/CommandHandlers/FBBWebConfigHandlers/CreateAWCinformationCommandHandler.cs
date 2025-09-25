using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class CreateAWCinformationCommandHandler : ICommandHandler<CreateAWCInfoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _FBB_APCOVERAGE;
        private readonly IEntityRepository<FBB_AP_INFO> _FBB_AP_INFO;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;

        public CreateAWCinformationCommandHandler(ILogger logger,
            IWBBUnitOfWork uow, IEntityRepository<FBB_APCOVERAGE> FBB_APCOVERAGE, IEntityRepository<FBB_AP_INFO> FBB_AP_INFO
            , IEntityRepository<FBB_HISTORY_LOG> historyLog)

        {
            _logger = logger;
            _uow = uow;
            _FBB_APCOVERAGE = FBB_APCOVERAGE;
            _FBB_AP_INFO = FBB_AP_INFO;
            _historyLog = historyLog;

        }

        public void Handle(CreateAWCInfoCommand command)
        {
            var model = command.awcmodel;
            var modelconfig = command.awcmodelconfig;
            var check = (from r in _FBB_APCOVERAGE.Get()
                         where (r.BASEL2 == model.Base_L2) && r.ACTIVE_FLAG == "Y"
                         select r);

            if (check.Count() > 0)
            {
                var ba = check.Select(a => a.BASEL2);
                //var si = check.Select(a => a.SITENAME);
                //if (si.Contains(model.Site_Name) && ba.Contains(model.Base_L2))
                //{
                //    command.dupname = "BaseL2 and Site name are";
                //    command.FlagDup = true;
                //}
                if (ba.Contains(model.Base_L2))
                {
                    command.dupname = "BaseL2 is";
                    command.FlagDup = true;
                }
                //else if (si.Contains(model.Site_Name))
                //{
                //    command.dupname = "Site Name is";
                //    command.FlagDup = true;
                //}
            }
            else
            {
                //insert 
                var fbbDSLAM = new FBB_APCOVERAGE
                {
                    CREATED_BY = model.user,
                    CREATED_DATE = DateTime.Now,
                    UPDATED_BY = model.user,
                    UPDATED_DATE = DateTime.Now,
                    BASEL2 = model.Base_L2.Trim(),
                    SITENAME = model.Site_Name.Trim(),
                    ACTIVE_FLAG = "Y",
                    SUB_DISTRICT = model.Tumbon,
                    DISTRICT = model.Aumphur,
                    PROVINCE = model.Province,
                    ZONE = model.Zone,
                    LAT = model.Lat.Trim(),
                    LNG = model.Lon.Trim(),
                    VLAN = model.VLAN.Trim(),
                    SUBNET_MASK_26 = model.subnet_mask_26.Trim(),
                    GATEWAY = model.gateway.Trim(),
                    AP_COMMENT = model.ap_comment.Trim(),
                    TOWER_TYPE = model.tower_type.Trim(),
                    TOWER_HEIGHT = model.tower_height,
                    ONTARGET_DATE_EX = DateTime.Now.Date,
                    ONTARGET_DATE_IN = DateTime.Now.Date,
                    COVERAGE_STATUS = "ON_SITE"
                };

                _FBB_APCOVERAGE.Create(fbbDSLAM);
                _uow.Persist();


                #region Add FBB_HISTORY_LOG
                var historyLogItem = new FBB_HISTORY_LOG();
                historyLogItem.ACTION = ActionHistory.ADD.ToString();
                historyLogItem.APPLICATION = "FBB_CFG006_1";
                historyLogItem.CREATED_BY = model.user;
                historyLogItem.CREATED_DATE = DateTime.Now;
                historyLogItem.DESCRIPTION = "BASEL2: " + model.Base_L2.Trim() + ", " + "SITENAME: " + model.Site_Name.Trim()
                                            + ", " + "Lattitude: " + model.Lat.Trim() + ", " + "Lontitude: " + model.Lon.Trim()
                                            + ", " + "REGION: " + model.Zone.Trim() + ", " + "PROVINCE: " + model.Province.Trim()
                                            + ", " + "DISTRICT: " + model.Aumphur.Trim() + ", " + "SUB-DISTRICT: " + model.Tumbon.Trim();
                historyLogItem.REF_KEY = model.Base_L2.Trim();
                historyLogItem.REF_NAME = "Base L2";
                _historyLog.Create(historyLogItem);
                _uow.Persist();
                #endregion


                var siteid = (from r in _FBB_APCOVERAGE.Get()
                              orderby r.APPID descending
                              select r.APPID).FirstOrDefault();




                foreach (var a in modelconfig)
                {
                    var config = new FBB_AP_INFO
                    {
                        CREATED_BY = a.user,
                        CREATED_DATE = a.updatedate,
                        UPDATED_BY = a.user,
                        UPDATED_DATE = a.updatedate,
                        SECTOR = a.Sector,
                        AP_NAME = a.AP_Name.Trim(),
                        ACTIVE_FLAG = "Y",
                        SITE_ID = siteid,
                        IP_ADDRESS = a.ip_address.Trim(),
                        STATUS = a.status.Trim(),
                        IMPLEMENT_PHASE = a.implement_phase.Trim(),
                        PO_NUMBER = a.po_number.Trim(),
                        AP_COMPANY = a.ap_company,
                        AP_LOT = a.ap_lot,
                        IMPLEMENT_DATE = a.implement_date,//DateTime.ParseExact(a.implement_date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        ON_SERVICE_DATE = a.onservice_date//DateTime.ParseExact(a.onservice_date, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    };

                    _FBB_AP_INFO.Create(config);
                    _uow.Persist();

                    #region Add FBB_HISTORY_LOG
                    var historyLogItem2 = new FBB_HISTORY_LOG();
                    historyLogItem2.ACTION = ActionHistory.ADD.ToString();
                    historyLogItem2.APPLICATION = "FBB_CFG006_1";
                    historyLogItem2.CREATED_BY = model.user;
                    historyLogItem2.CREATED_DATE = DateTime.Now;
                    historyLogItem2.DESCRIPTION = historyLogItem2.DESCRIPTION + "BASEL2: " + model.Base_L2 + " => " + "AP_NAME: " + a.AP_Name.Trim() + ", " + "SECTOR: " + a.Sector.Trim() + " ";
                    historyLogItem2.REF_KEY = a.AP_Name.Trim();
                    historyLogItem2.REF_NAME = "AP Name";
                    _historyLog.Create(historyLogItem2);
                    _uow.Persist();
                    #endregion


                }









            }
        }
    }
}
//var info = from r in _FBB_AP_INFO.Get()
//           where r.SITE_ID == 0 && r.ACTIVE_FLAG == "Y"
//           select r;


//foreach (var a in info)
//{
//    a.CREATED_BY = model.user;
//    a.UPDATED_BY = model.user;
//    a.UPDATED_DATE = DateTime.Now;
//    a.CREATED_DATE = DateTime.Now;
//    a.SITE_ID = siteid;
//    _FBB_AP_INFO.Update(a);
//}
//var temp = (from r in _FBB_AP_INFO.Get()
//            where r.SITE_ID == 0
//            select r).ToList();

//foreach (var a in temp)
//{
//    var xxx = _FBB_AP_INFO.GetByKey(a.AP_ID);
//    if (null != xxx)
//    {
//        _FBB_AP_INFO.Delete(xxx);
//    }

//    //_info.Delete(info);
//}
