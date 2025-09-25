using System;
using System.Globalization;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdateAWCInfoCommandHandler : ICommandHandler<UpdateAWCInfoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _FBB_APCOVERAGE;
        private readonly IEntityRepository<FBB_AP_INFO> _FBB_AP_INFO;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;

        public UpdateAWCInfoCommandHandler(ILogger logger,
            IWBBUnitOfWork uow, IEntityRepository<FBB_APCOVERAGE> FBB_APCOVERAGE, IEntityRepository<FBB_AP_INFO> FBB_AP_INFO
            , IEntityRepository<FBB_HISTORY_LOG> historyLog)

        {
            _logger = logger;
            _uow = uow;
            _FBB_APCOVERAGE = FBB_APCOVERAGE;
            _FBB_AP_INFO = FBB_AP_INFO;
            _historyLog = historyLog;

        }
        private static string CompareObjectToString(object oldObject, object newObject)
        {
            var result = "";

            var oldObj = oldObject.GetType().GetProperties();
            var newObj = newObject.GetType().GetProperties();

            for (int i = 0; i < oldObj.Count(); i++)
            {
                var oldVal = oldObj[i].GetValue(oldObject);
                var newVal = newObj[i].GetValue(newObject);

                if (oldVal.ToSafeString() != newVal.ToSafeString())
                {
                    var objName = oldObj[i].Name.ToUpper();
                    var objName2 = oldObj[i].Name.ToUpper();
                    CultureInfo properCase = System.Threading.Thread.CurrentThread.CurrentCulture;
                    TextInfo currentInfo = properCase.TextInfo;
                    objName2 = currentInfo.ToTitleCase(currentInfo.ToLower(objName2));

                    if (objName == "IMPLEMENT_DATE" || objName == "ON_SERVICE_DATE")
                    {
                        if (result == "")
                            result = oldObj[i].Name + ": " + String.Format("{0:dd/MM/yyyy}", oldVal) + " to " + String.Format("{0:dd/MM/yyyy}", newVal);
                        else
                            result = result + ", " + oldObj[i].Name + ": " + String.Format("{0:dd/MM/yyyy}", oldVal) + " to " + String.Format("{0:dd/MM/yyyy}", newVal);
                    }
                    else if (objName != "CREATED_BY" && objName != "UPDATED_BY" && objName != "CREATED_DATE" && objName != "UPDATED_DATE" && objName != "APPID" && objName != "AP_ID" && objName != "ACTIVE_FLAG" && objName != "SITE_ID")
                    {
                        if (objName == "LAT")
                        {
                            if (result == "")
                                result = "Lattitude" + ": " + oldVal + " to " + newVal;
                            else
                                result = result + ", " + "Lattitude" + ": " + oldVal + " to " + newVal;
                        }
                        else if (objName == "LNG")
                        {
                            if (result == "")
                                result = "Lontitude " + ": " + oldVal + " to " + newVal;
                            else
                                result = result + ", " + "Lontitude " + ": " + oldVal + " to " + newVal;
                        }
                        else if (objName == "AP_COMMENT")
                        {
                            if (result == "")
                                result = "Comment " + ": " + oldVal + " to " + newVal;
                            else
                                result = result + ", " + "Comment " + ": " + oldVal + " to " + newVal;
                        }
                        else if (objName == "AP_COMPANY")
                        {
                            if (result == "")
                                result = "Company " + ": " + oldVal + " to " + newVal;
                            else
                                result = result + ", " + "Company " + ": " + oldVal + " to " + newVal;
                        }
                        else
                        {
                            if (result == "")
                                result = objName2.Replace("_", " ") + ": " + oldVal + " to " + newVal;
                            else
                                result = result + ", " + objName2.Replace("_", " ") + ": " + oldVal + " to " + newVal;
                        }

                    }
                }
            }

            return result;
        }
        public void Handle(UpdateAWCInfoCommand command)
        {
            try
            {
                var model = command.awcmodel;
                var modelconfig = command.awcmodelconfig;

                var check = (from r in _FBB_APCOVERAGE.Get()
                             where r.APPID == model.APP_ID
                             select r);
                if ((command.oldbasename != model.Base_L2))//|| (command.oldsitename != model.Site_Name))
                {
                    int countbase = 0;
                    //int countsite = 0;
                    var checkbase = (from r in _FBB_APCOVERAGE.Get()
                                     where r.BASEL2.Equals(model.Base_L2) && r.ACTIVE_FLAG == "Y"
                                     select r);

                    //var checksite = (from r in _FBB_APCOVERAGE.Get()
                    //                 where r.SITENAME.Equals(model.Site_Name) && r.ACTIVE_FLAG == "Y"
                    //                 select r);
                    if (command.oldbasename != model.Base_L2)
                    {
                        countbase = checkbase.Count();
                    }
                    //if (command.oldsitename != model.Site_Name)
                    //{
                    //    countsite = checksite.Count();
                    //}

                    //if (countbase > 0 && countsite > 0)
                    //{
                    //    command.dupname = "BaseL2 and Site name are";
                    //    command.FlagDup = true;
                    //}
                    if (countbase > 0)
                    {
                        command.dupname = "BaseL2 is";
                        command.FlagDup = true;
                    }
                    //else if (countsite > 0)
                    //{
                    //    command.dupname = "Site name is";
                    //    command.FlagDup = true;
                    //}
                    else
                    {
                        //insert 
                        foreach (var a in check)
                        {
                            var oldCoverage = new FBB_APCOVERAGE()
                            {
                                UPDATED_BY = a.UPDATED_BY,
                                UPDATED_DATE = a.UPDATED_DATE,
                                BASEL2 = a.BASEL2,
                                SITENAME = a.SITENAME,
                                SUB_DISTRICT = a.SUB_DISTRICT,
                                DISTRICT = a.DISTRICT,
                                PROVINCE = a.PROVINCE,
                                ZONE = a.ZONE,
                                LAT = a.LAT,
                                LNG = a.LNG,
                                VLAN = a.VLAN,
                                SUBNET_MASK_26 = a.SUBNET_MASK_26,
                                GATEWAY = a.GATEWAY,
                                AP_COMMENT = a.AP_COMMENT,
                                TOWER_TYPE = a.TOWER_TYPE,
                                TOWER_HEIGHT = a.TOWER_HEIGHT,
                            };

                            a.UPDATED_BY = model.user;
                            a.UPDATED_DATE = DateTime.Now;
                            a.BASEL2 = model.Base_L2.Trim();
                            a.SITENAME = model.Site_Name.Trim();
                            a.SUB_DISTRICT = model.Tumbon;
                            a.DISTRICT = model.Aumphur;
                            a.PROVINCE = model.Province;
                            a.ZONE = model.Zone;
                            a.LAT = model.Lat;
                            a.LNG = model.Lon;
                            a.VLAN = model.VLAN;
                            a.SUBNET_MASK_26 = model.subnet_mask_26;
                            a.GATEWAY = model.gateway;
                            a.AP_COMMENT = model.ap_comment;
                            a.TOWER_TYPE = model.tower_type;
                            a.TOWER_HEIGHT = model.tower_height;//decimal.Parse(model.tower_height);


                            _FBB_APCOVERAGE.Update(a);
                            _uow.Persist();

                            #region Update FBB_HISTORY_LOG
                            var historyLogItem = new FBB_HISTORY_LOG();
                            historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                            historyLogItem.APPLICATION = "FBB_CFG006_1";
                            historyLogItem.CREATED_BY = model.user;
                            historyLogItem.CREATED_DATE = DateTime.Now;
                            historyLogItem.DESCRIPTION = CompareObjectToString(oldCoverage, a);
                            historyLogItem.REF_KEY = a.BASEL2;
                            historyLogItem.REF_NAME = "Base L2";

                            if (historyLogItem.DESCRIPTION != string.Empty)
                            {
                                _historyLog.Create(historyLogItem);
                                _uow.Persist();
                            }
                            #endregion
                        }


                        if (modelconfig != null)
                        {
                            //var listapname = from r in _FBB_AP_INFO.Get()
                            //             select r.AP_NAME;
                            foreach (var a in modelconfig)
                            {
                                var info = (from r in _FBB_AP_INFO.Get()
                                            where r.AP_ID == a.AP_ID
                                            select r).ToList();
                                if (info.Count != 0)
                                {
                                    if (a.ACTIVE_FLAGINFO == "Y")
                                    {
                                        foreach (var b in info)
                                        {
                                            var oldCoverage = new FBB_AP_INFO()
                                            {
                                                UPDATED_BY = b.UPDATED_BY,
                                                UPDATED_DATE = b.UPDATED_DATE,
                                                AP_NAME = b.AP_NAME,
                                                SECTOR = b.SECTOR,
                                                ACTIVE_FLAG = b.ACTIVE_FLAG,
                                                IP_ADDRESS = b.IP_ADDRESS,
                                                STATUS = b.STATUS,
                                                IMPLEMENT_PHASE = b.IMPLEMENT_PHASE,
                                                PO_NUMBER = b.PO_NUMBER,
                                                AP_COMPANY = b.AP_COMPANY,
                                                AP_LOT = b.AP_LOT,
                                                IMPLEMENT_DATE = b.IMPLEMENT_DATE,
                                                ON_SERVICE_DATE = b.ON_SERVICE_DATE
                                            };

                                            b.UPDATED_BY = a.user;
                                            b.UPDATED_DATE = a.updatedate;
                                            b.AP_NAME = a.AP_Name.Trim();
                                            b.SECTOR = a.Sector;
                                            b.ACTIVE_FLAG = a.ACTIVE_FLAGINFO;

                                            b.IP_ADDRESS = a.ip_address;
                                            b.STATUS = a.status;
                                            b.IMPLEMENT_PHASE = a.implement_phase;
                                            b.PO_NUMBER = a.po_number;
                                            b.AP_COMPANY = a.ap_company;
                                            b.AP_LOT = a.ap_lot;
                                            //string d = a.implement_date.ToSafeString();
                                            b.IMPLEMENT_DATE = a.implement_date;//DateTime.ParseExact(a.implement_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            b.ON_SERVICE_DATE = a.onservice_date;//DateTime.ParseExact(a.onservice_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                            _FBB_AP_INFO.Update(b);
                                            _uow.Persist();

                                            #region Update FBB_HISTORY_LOG
                                            var historyLogItem = new FBB_HISTORY_LOG();
                                            historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                                            historyLogItem.APPLICATION = "FBB_CFG006_1";
                                            historyLogItem.CREATED_BY = model.user;
                                            historyLogItem.CREATED_DATE = DateTime.Now;
                                            historyLogItem.DESCRIPTION = "BASEL2: " + model.Base_L2 + " => " + CompareObjectToString(oldCoverage, b);
                                            historyLogItem.REF_KEY = b.AP_NAME;
                                            historyLogItem.REF_NAME = "AP Name";

                                            if (CompareObjectToString(oldCoverage, b) != string.Empty)
                                            {
                                                _historyLog.Create(historyLogItem);
                                                _uow.Persist();
                                            }
                                            #endregion

                                        }
                                    }
                                    else if (a.ACTIVE_FLAGINFO == "N")
                                    {
                                        foreach (var b in info)
                                        {

                                            b.UPDATED_BY = a.user;
                                            b.UPDATED_DATE = a.updatedate;
                                            b.AP_NAME = a.AP_Name.Trim();
                                            b.SECTOR = a.Sector;
                                            b.ACTIVE_FLAG = a.ACTIVE_FLAGINFO;

                                            b.IP_ADDRESS = a.ip_address;
                                            b.STATUS = a.status;
                                            b.IMPLEMENT_PHASE = a.implement_phase;
                                            b.PO_NUMBER = a.po_number;
                                            b.AP_COMPANY = a.ap_company;
                                            b.AP_LOT = a.ap_lot;
                                            b.IMPLEMENT_DATE = a.implement_date;//DateTime.ParseExact(a.implement_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            b.ON_SERVICE_DATE = a.onservice_date;//DateTime.ParseExact(a.onservice_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                            _FBB_AP_INFO.Update(b);
                                            _uow.Persist();

                                            #region Delete FBB_HISTORY_LOG
                                            var historyLogItem2 = new FBB_HISTORY_LOG();
                                            historyLogItem2.ACTION = ActionHistory.DELETE.ToString();
                                            historyLogItem2.APPLICATION = "FBB_CFG006_1";
                                            historyLogItem2.CREATED_BY = a.user;
                                            historyLogItem2.CREATED_DATE = DateTime.Now;
                                            historyLogItem2.DESCRIPTION = "BASEL2: " + model.Base_L2 + " => " + "AP_Name: " + a.AP_Name.ToSafeString();
                                            historyLogItem2.REF_KEY = a.AP_Name.ToSafeString();
                                            historyLogItem2.REF_NAME = "AP Name";
                                            _historyLog.Create(historyLogItem2);
                                            _uow.Persist();
                                            #endregion

                                        }
                                    }
                                }
                                if (a.AP_ID == 0 && a.ACTIVE_FLAGINFO == "Y")
                                {
                                    var config = new FBB_AP_INFO
                                    {
                                        CREATED_BY = a.user,
                                        CREATED_DATE = DateTime.Now,
                                        UPDATED_BY = a.user,
                                        UPDATED_DATE = DateTime.Now,
                                        SECTOR = a.Sector,
                                        AP_NAME = a.AP_Name.Trim(),
                                        ACTIVE_FLAG = "Y",
                                        SITE_ID = model.APP_ID,

                                        IP_ADDRESS = a.ip_address,
                                        STATUS = a.status,
                                        IMPLEMENT_PHASE = a.implement_phase,
                                        PO_NUMBER = a.po_number,
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
                else
                {
                    //insert 
                    foreach (var a in check)
                    {
                        var oldCoverage = new FBB_APCOVERAGE()
                        {
                            UPDATED_BY = a.UPDATED_BY,
                            UPDATED_DATE = a.UPDATED_DATE,
                            BASEL2 = a.BASEL2,
                            SITENAME = a.SITENAME,
                            SUB_DISTRICT = a.SUB_DISTRICT,
                            DISTRICT = a.DISTRICT,
                            PROVINCE = a.PROVINCE,
                            ZONE = a.ZONE,
                            LAT = a.LAT,
                            LNG = a.LNG,
                            VLAN = a.VLAN,
                            SUBNET_MASK_26 = a.SUBNET_MASK_26,
                            GATEWAY = a.GATEWAY,
                            AP_COMMENT = a.AP_COMMENT,
                            TOWER_TYPE = a.TOWER_TYPE,
                            TOWER_HEIGHT = a.TOWER_HEIGHT,
                        };

                        a.UPDATED_BY = model.user;
                        a.UPDATED_DATE = DateTime.Now;
                        a.BASEL2 = model.Base_L2.Trim();
                        a.SITENAME = model.Site_Name.Trim();
                        a.SUB_DISTRICT = model.Tumbon;
                        a.DISTRICT = model.Aumphur;
                        a.PROVINCE = model.Province;
                        a.ZONE = model.Zone;
                        a.LAT = model.Lat;
                        a.LNG = model.Lon;
                        a.VLAN = model.VLAN;
                        a.SUBNET_MASK_26 = model.subnet_mask_26;
                        a.GATEWAY = model.gateway;
                        a.AP_COMMENT = model.ap_comment;
                        a.TOWER_TYPE = model.tower_type;
                        a.TOWER_HEIGHT = model.tower_height;//decimal.Parse(model.tower_height);

                        _FBB_APCOVERAGE.Update(a);
                        _uow.Persist();

                        #region Update FBB_HISTORY_LOG
                        var historyLogItem = new FBB_HISTORY_LOG();
                        historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                        historyLogItem.APPLICATION = "FBB_CFG006_1";
                        historyLogItem.CREATED_BY = model.user;
                        historyLogItem.CREATED_DATE = DateTime.Now;
                        historyLogItem.DESCRIPTION = "BASEL2: " + model.Base_L2 + " => " + CompareObjectToString(oldCoverage, a);
                        historyLogItem.REF_KEY = a.BASEL2.ToSafeString();
                        historyLogItem.REF_NAME = "Base L2";

                        if (CompareObjectToString(oldCoverage, a) != string.Empty)
                        {
                            _historyLog.Create(historyLogItem);
                            _uow.Persist();
                        }
                        #endregion
                    }


                    if (modelconfig != null)
                    {
                        //var listapname = from r in _FBB_AP_INFO.Get()
                        //             select r.AP_NAME;
                        foreach (var a in modelconfig)
                        {
                            var info = (from r in _FBB_AP_INFO.Get()
                                        where r.AP_ID == a.AP_ID
                                        select r).ToList();
                            if (info.Count != 0)
                            {
                                if (a.ACTIVE_FLAGINFO == "Y")
                                {
                                    foreach (var b in info)
                                    {
                                        var oldCoverage = new FBB_AP_INFO()
                                        {
                                            UPDATED_BY = b.UPDATED_BY,
                                            UPDATED_DATE = b.UPDATED_DATE,
                                            AP_NAME = b.AP_NAME,
                                            SECTOR = b.SECTOR,
                                            ACTIVE_FLAG = b.ACTIVE_FLAG,
                                            IP_ADDRESS = b.IP_ADDRESS,
                                            STATUS = b.STATUS,
                                            IMPLEMENT_PHASE = b.IMPLEMENT_PHASE,
                                            PO_NUMBER = b.PO_NUMBER,
                                            AP_COMPANY = b.AP_COMPANY,
                                            AP_LOT = b.AP_LOT,
                                            IMPLEMENT_DATE = b.IMPLEMENT_DATE,
                                            ON_SERVICE_DATE = b.ON_SERVICE_DATE
                                        };

                                        b.UPDATED_BY = a.user;
                                        b.UPDATED_DATE = a.updatedate;
                                        b.AP_NAME = a.AP_Name.Trim();
                                        b.SECTOR = a.Sector;
                                        b.ACTIVE_FLAG = a.ACTIVE_FLAGINFO;

                                        b.IP_ADDRESS = a.ip_address;
                                        b.STATUS = a.status;
                                        b.IMPLEMENT_PHASE = a.implement_phase;
                                        b.PO_NUMBER = a.po_number;
                                        b.AP_COMPANY = a.ap_company;
                                        b.AP_LOT = a.ap_lot;
                                        //string d = String.Format("{0:dd/MM/yyyy}", a.implement_date);
                                        b.IMPLEMENT_DATE = a.implement_date;//DateTime.Parse(d);
                                                                            //b.IMPLEMENT_DATE = a.implement_date;//DateTime.ParseExact(a.implement_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        b.ON_SERVICE_DATE = a.onservice_date;//DateTime.ParseExact(a.onservice_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                        _FBB_AP_INFO.Update(b);
                                        _uow.Persist();

                                        #region Update FBB_HISTORY_LOG
                                        var historyLogItem = new FBB_HISTORY_LOG();
                                        historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                                        historyLogItem.APPLICATION = "FBB_CFG006_1";
                                        historyLogItem.CREATED_BY = model.user;
                                        historyLogItem.CREATED_DATE = DateTime.Now;
                                        historyLogItem.DESCRIPTION = "BASEL2: " + model.Base_L2 + " => " + CompareObjectToString(oldCoverage, b);
                                        historyLogItem.REF_KEY = b.AP_NAME.ToSafeString();
                                        historyLogItem.REF_NAME = "AP Name";

                                        if (CompareObjectToString(oldCoverage, b) != string.Empty)
                                        {
                                            _historyLog.Create(historyLogItem);
                                            _uow.Persist();
                                        }
                                        #endregion

                                    }
                                }
                                else if (a.ACTIVE_FLAGINFO == "N")
                                {
                                    foreach (var b in info)
                                    {

                                        b.UPDATED_BY = a.user;
                                        b.UPDATED_DATE = a.updatedate;
                                        b.AP_NAME = a.AP_Name.Trim();
                                        b.SECTOR = a.Sector;
                                        b.ACTIVE_FLAG = a.ACTIVE_FLAGINFO;

                                        b.IP_ADDRESS = a.ip_address;
                                        b.STATUS = a.status;
                                        b.IMPLEMENT_PHASE = a.implement_phase;
                                        b.PO_NUMBER = a.po_number;
                                        b.AP_COMPANY = a.ap_company;
                                        b.AP_LOT = a.ap_lot;
                                        b.IMPLEMENT_DATE = a.implement_date;//DateTime.ParseExact(a.implement_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        b.ON_SERVICE_DATE = a.onservice_date;//DateTime.ParseExact(a.onservice_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                        _FBB_AP_INFO.Update(b);
                                        _uow.Persist();

                                        #region Delete FBB_HISTORY_LOG
                                        var historyLogItem2 = new FBB_HISTORY_LOG();
                                        historyLogItem2.ACTION = ActionHistory.DELETE.ToString();
                                        historyLogItem2.APPLICATION = "FBB_CFG006_1";
                                        historyLogItem2.CREATED_BY = a.user;
                                        historyLogItem2.CREATED_DATE = DateTime.Now;
                                        historyLogItem2.DESCRIPTION = "BASEL2: " + model.Base_L2 + " => " + "AP_Name: " + b.AP_NAME.ToSafeString();
                                        historyLogItem2.REF_KEY = b.AP_NAME.ToSafeString();
                                        historyLogItem2.REF_NAME = "AP Name";
                                        _historyLog.Create(historyLogItem2);
                                        _uow.Persist();
                                        #endregion

                                    }
                                }
                            }
                            if (a.AP_ID == 0 && a.ACTIVE_FLAGINFO == "Y")
                            {
                                var config = new FBB_AP_INFO
                                {
                                    CREATED_BY = a.user,
                                    CREATED_DATE = DateTime.Now,
                                    UPDATED_BY = a.user,
                                    UPDATED_DATE = DateTime.Now,
                                    SECTOR = a.Sector,
                                    AP_NAME = a.AP_Name.Trim(),
                                    ACTIVE_FLAG = "Y",
                                    SITE_ID = model.APP_ID,

                                    IP_ADDRESS = a.ip_address,
                                    STATUS = a.status,
                                    IMPLEMENT_PHASE = a.implement_phase,
                                    PO_NUMBER = a.po_number,
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
                                historyLogItem2.REF_KEY = a.AP_Name.ToSafeString();
                                historyLogItem2.REF_NAME = "AP Name";
                                _historyLog.Create(historyLogItem2);
                                _uow.Persist();
                                #endregion
                            }
                            //    if (listapname.Contains(a.AP_Name))
                            //    {                               
                            //        var info = (from r in _FBB_AP_INFO.Get()
                            //                    where r.AP_ID == a.AP_ID
                            //                    select r);

                            //        foreach (var b in info)
                            //        {
                            //            b.UPDATED_BY = a.user;
                            //            b.UPDATED_DATE = DateTime.Now;
                            //            b.AP_NAME = a.AP_Name.Trim();
                            //            b.SECTOR = a.Sector;
                            //            _FBB_AP_INFO.Update(b);

                            //        }
                            //    }
                            //    else
                            //    {
                            //        var config = new FBB_AP_INFO
                            //        {
                            //            CREATED_BY = a.user,
                            //            CREATED_DATE = DateTime.Now,
                            //            UPDATED_BY = a.user,
                            //            UPDATED_DATE = DateTime.Now,
                            //            SECTOR = a.Sector,
                            //            AP_NAME = a.AP_Name.Trim(),
                            //            ACTIVE_FLAG = "Y",
                            //            SITE_ID = model.APP_ID
                            //        };

                            //        _FBB_AP_INFO.Create(config);
                            //    }

                        }


                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Info("Error occured when handle DeleteAWCconfigCommandHandler");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }
        }
    }
}
