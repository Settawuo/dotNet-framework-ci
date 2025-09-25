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
    public class CoverageAreaCommandHandler : ICommandHandler<CoverageAreaCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_COVERAGE_ZIPCODE> _coverageZipCode;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageRelation;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfo;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _coverageAreaBuilding;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipCode;

        public CoverageAreaCommandHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA> coverageArea,
                                                            IWBBUnitOfWork uow,
                                                            IEntityRepository<FBB_COVERAGE_ZIPCODE> coverageZipCode,
                                                            IEntityRepository<FBB_ZIPCODE> zipcode,
                                                            IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageRelation,
                                                            IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                            IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                            IEntityRepository<FBB_PORT_INFO> portInfo,
                                                            IEntityRepository<FBB_COVERAGEAREA_BUILDING> coverageAreaBuilding,
                                                            IEntityRepository<FBB_HISTORY_LOG> historyLog,
                                                            IEntityRepository<FBB_ZIPCODE> zipCode)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _uow = uow;
            _coverageZipCode = coverageZipCode;
            _zipcode = zipcode;
            _coverageRelation = coverageRelation;
            _cardInfo = cardInfo;
            _dslamInfo = dslamInfo;
            _portInfo = portInfo;
            _coverageAreaBuilding = coverageAreaBuilding;
            _historyLog = historyLog;
            _zipCode = zipCode;
        }

        public void Handle(CoverageAreaCommand command)
        {
            try
            {
                var dateNow = DateTime.Now;
                decimal cvrid = 0;
                decimal? contactId = 0;

                if (command.ActionType == WBBContract.Commands.ActionType.Insert)
                {
                    #region insert

                    var chkDupCoverage = from ca in _coverageArea.Get()
                                         where ca.ACTIVEFLAG == "Y" && ((ca.NODENAME_TH == command.CoverageAreaPanel.NodeNameTH.Trim())
                                         || (ca.NODENAME_EN.ToUpper() == command.CoverageAreaPanel.NodeNameEN.Trim().ToUpper()))
                                         select ca;

                    if (chkDupCoverage != null)
                    {
                        if (chkDupCoverage.Count() > 0)
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Node name is duplicated.";
                            return;
                        }
                    }

                    if (command.CoverageAreaPanel.ContactId != 0 && command.CoverageAreaPanel.ContactId != null)
                    {
                        var data = from ca in _coverageArea.Get()
                                   where ca.CONTACT_ID == command.CoverageAreaPanel.ContactId
                                   && ca.ACTIVEFLAG == "Y"
                                   select ca;

                        if (data.Any())
                        {
                            var coverageArea = data.FirstOrDefault();

                            cvrid = coverageArea.CVRID;

                            coverageArea.BUILDINGCODE = command.CoverageAreaPanel.BuildingCode.ToSafeString();
                            coverageArea.ONTARGET_DATE_IN = null;
                            coverageArea.ONTARGET_DATE_EX = null;
                            coverageArea.NODESTATUS = command.CoverageAreaPanel.Status.ToSafeString();
                            coverageArea.ACTIVEFLAG = "Y";
                            coverageArea.COMPLETE_FLAG = "N";
                            coverageArea.TIE_FLAG = command.CoverageAreaPanel.TieFlag == true ? "Y" : "N";
                            coverageArea.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                            coverageArea.CREATED_DATE = dateNow;
                            coverageArea.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                            coverageArea.UPDATED_DATE = dateNow;
                            coverageArea.CVRID = 0;

                            _coverageArea.Create(coverageArea);
                            _uow.Persist();

                            var getZipCode = from z in _coverageZipCode.Get()
                                             where z.CVRID == cvrid
                                             select z;

                            if (getZipCode.Any())
                            {
                                var listZipCode = getZipCode.ToList();
                                var lastCvrId = from c in _coverageArea.Get()
                                                where c.CREATED_DATE == dateNow
                                                select c.CVRID;

                                cvrid = lastCvrId.Any() ? lastCvrId.FirstOrDefault() : 0;

                                foreach (var item in listZipCode)
                                {
                                    item.CVRID = cvrid;
                                    item.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    item.CREATED_DATE = dateNow;
                                    item.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    item.UPDATED_DATE = dateNow;
                                    _coverageZipCode.Create(item);
                                }
                                _uow.Persist();
                            }

                            #region Add FBB_HISTORY_LOG
                            var historyLogItem = new FBB_HISTORY_LOG();
                            historyLogItem.ACTION = ActionHistory.ADD.ToString();
                            historyLogItem.APPLICATION = "FBB_CFG001_2_Coverage";
                            historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                            historyLogItem.CREATED_DATE = dateNow;
                            historyLogItem.DESCRIPTION = "Node Name TH:" + coverageArea.NODENAME_TH + "Building Code: " + coverageArea.BUILDINGCODE;
                            historyLogItem.REF_KEY = coverageArea.NODENAME_TH;
                            historyLogItem.REF_NAME = "Node Name TH";
                            _historyLog.Create(historyLogItem);
                            _uow.Persist();
                            #endregion
                        }
                        else
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Cannot find the data for " + command.ActionType.ToString();
                            return;
                        }
                    }
                    else
                    {
                        #region sql for insert CoverageArea
                        //insert into FBB_COVERAGEAREA (CVRID, CREATED_BY, CREATED_DATE, UPDATED_BY, UPDATED_DATE, LOCATIONCODE, BUILDINGCODE, 
                        //NODENAME_EN, NODENAME_TH, NODETYPE, NODESTATUS, ACTIVEFLAG, MOO, SOI_TH, ROAD_TH, SOI_EN, ROAD_EN, ZIPCODE, IPRAN_CODE, 
                        //CONTACT_NUMBER, FAX_NUMBER, ONTARGET_DATE, REGION_CODE, CONTACT_ID, LATITUDE, LONGITUDE, COMPLETE_FLAG
                        //)
                        //values 
                        //(
                        //FBB_SEQ_CVRID.NextVal, 'CHANAWUN', to_date('13-06-2014 11:16:05', 'dd-mm-yyyy hh24:mi:ss'), null, null, 'LPOR', '1', 'Supalai Park Downtown', 'ศุภาลัย ปาร์ค ดาวน์ทาวน์',
                        //'CONDOMINIUM', 'ON_SITE', 'Y', null, null, 'มนตรี', null, 'Montri', '83000', null, null, null, to_date('01-07-2014', 'dd-mm-yyyy'), 'BKK', 5, null, null, 'Y');

                        //insert into FBB_COVERAGE_ZIPCODE 
                        //(CVRID, CREATED_BY, CREATED_DATE, UPDATED_BY, UPDATED_DATE, ZIPCODE_ROWID_EN, ZIPCODE_ROWID_TH
                        //)
                        //values 
                        //(104, 'Chanawun', to_date('13-06-2014 11:43:29', 'dd-mm-yyyy hh24:mi:ss'), null, null, '779AD8C5766E48DDE0440000BEA816B7', '6A3C56EE2C4D61C0E0440000BEA816B7');
                        #endregion

                        var itemCoverageArea = new FBB_COVERAGEAREA();
                        itemCoverageArea.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                        itemCoverageArea.CREATED_DATE = dateNow;
                        itemCoverageArea.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                        itemCoverageArea.UPDATED_DATE = dateNow;
                        itemCoverageArea.NODENAME_EN = command.CoverageAreaPanel.NodeNameEN.ToSafeString().Trim();
                        itemCoverageArea.NODENAME_TH = command.CoverageAreaPanel.NodeNameTH.ToSafeString().Trim();
                        itemCoverageArea.NODETYPE = command.CoverageAreaPanel.NodeType.ToSafeString();
                        itemCoverageArea.ACTIVEFLAG = "Y";
                        itemCoverageArea.MOO = command.CoverageAreaPanel.MooTH;
                        itemCoverageArea.SOI_TH = command.CoverageAreaPanel.SoiTH.ToSafeString();
                        itemCoverageArea.ROAD_TH = command.CoverageAreaPanel.RoadTH.ToSafeString();
                        itemCoverageArea.SOI_EN = command.CoverageAreaPanel.SoiEN.ToSafeString();
                        itemCoverageArea.ROAD_EN = command.CoverageAreaPanel.RoadEN.ToSafeString();
                        itemCoverageArea.ZIPCODE = command.CoverageAreaPanel.ZipCodeTH.ToSafeString();
                        itemCoverageArea.CONTACT_NUMBER = command.CoverageAreaPanel.ContactNumber.ToSafeString();
                        itemCoverageArea.FAX_NUMBER = command.CoverageAreaPanel.FaxNumber.ToSafeString();
                        itemCoverageArea.REGION_CODE = command.CoverageAreaPanel.RegionCode.ToSafeString();

                        var maxContactId = from ca in _coverageArea.Get()
                                           select ca.CONTACT_ID;

                        contactId = maxContactId.Any() ? maxContactId.Max() + 1 : 0;
                        itemCoverageArea.CONTACT_ID = contactId;

                        command.CoverageAreaPanel.ContactId = contactId; //set for return

                        //itemCoverageArea.IPRAN_CODE = command.CoverageAreaPanel.IpRanSiteCode.ToSafeString(); // site 
                        itemCoverageArea.LATITUDE = command.CoverageAreaPanel.Lat.ToSafeString(); // site 
                        itemCoverageArea.LONGITUDE = command.CoverageAreaPanel.Long.ToSafeString(); // site 
                        //itemCoverageArea.LOCATIONCODE = command.CoverageAreaPanel.CondoCode.ToSafeString(); // site 

                        itemCoverageArea.NODESTATUS = "ON_PROGRESS"; // coverage info
                        //itemCoverageArea.BUILDINGCODE = command.CoverageAreaPanel.BuildingCode.ToSafeString(); // coverage info
                        //itemCoverageArea.ONTARGET_DATE = command.CoverageAreaPanel.OnTargetDate; // coverage info
                        itemCoverageArea.COMPLETE_FLAG = "N"; // coverage info
                        itemCoverageArea.TIE_FLAG = command.CoverageAreaPanel.TieFlag == true ? "Y" : "N";
                        _coverageArea.Create(itemCoverageArea);
                        _uow.Persist();

                        #region Insert Zipcode
                        var lastCvrId = from c in _coverageArea.Get()
                                        where c.CREATED_DATE == dateNow
                                        select c.CVRID;

                        cvrid = lastCvrId.Any() ? lastCvrId.FirstOrDefault() : 0;

                        #region sql for check zipcode and get zipcoderowid
                        //select * from FBB_Zipcode z
                        //where z.lang_flag='N'
                        //and z.province like 'กรุงเทพ' and z.amphur  like 'ดอนเมือง%' and z.tumbon like 'สีกัน';

                        //select * from FBB_Zipcode z
                        //where z.lang_flag = 'Y'
                        //and z.province like 'Bangkok' and z.amphur like 'Don Mueang%' and z.tumbon like 'Si Kan';
                        #endregion

                        var zipcodeTH = from z in _zipcode.Get()
                                        where z.LANG_FLAG == "N" && z.PROVINCE == command.CoverageAreaPanel.ProvinceTH
                                        && z.AMPHUR.StartsWith(command.CoverageAreaPanel.AmphurTH) && z.TUMBON == command.CoverageAreaPanel.TumbonTH
                                        select z.ZIPCODE_ROWID;

                        var zipcodeEN = from z in _zipcode.Get()
                                        where z.LANG_FLAG == "Y" && z.PROVINCE == command.CoverageAreaPanel.ProvinceEN
                                        && z.AMPHUR.StartsWith(command.CoverageAreaPanel.AmphurEN) && z.TUMBON == command.CoverageAreaPanel.TumbonEN
                                        select z.ZIPCODE_ROWID;

                        #region sql for insert coverage zipcode
                        //insert into FBB_COVERAGE_ZIPCODE 
                        //(CVRID, CREATED_BY, CREATED_DATE, UPDATED_BY, UPDATED_DATE, ZIPCODE_ROWID_EN, ZIPCODE_ROWID_TH
                        //)
                        //values 
                        //(104, 'Chanawun', to_date('13-06-2014 11:43:29', 'dd-mm-yyyy hh24:mi:ss'), null, null, 
                        //'779AD8C5766E48DDE0440000BEA816B7', '6A3C56EE2C4D61C0E0440000BEA816B7');
                        #endregion

                        if (zipcodeTH.Any() && zipcodeEN.Any())
                        {
                            var listZipcodeTH = zipcodeTH.ToList();
                            var listZipCodeEN = zipcodeEN.ToList();

                            var countTH = zipcodeTH.Count();
                            var countEN = zipcodeEN.Count();

                            for (int i = 0; i < listZipcodeTH.Count; i++)
                            {
                                var zipEn = i < countEN ? listZipCodeEN[i] : "";
                                var zipTH = i < countTH ? listZipcodeTH[i] : "";

                                var itemCoverageZipcode = new FBB_COVERAGE_ZIPCODE()
                                {
                                    CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString(),
                                    CREATED_DATE = dateNow,
                                    CVRID = cvrid,
                                    UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString(),
                                    UPDATED_DATE = dateNow,
                                    ZIPCODE_ROWID_EN = zipEn != null ? zipEn : "",
                                    ZIPCODE_ROWID_TH = zipTH != null ? zipTH : ""
                                };

                                _coverageZipCode.Create(itemCoverageZipcode);
                            }

                            _uow.Persist();
                        }
                        #endregion

                        #region Add FBB_HISTORY_LOG
                        var historyLogItem = new FBB_HISTORY_LOG();
                        historyLogItem.ACTION = ActionHistory.ADD.ToString();
                        historyLogItem.APPLICATION = "FBB_CFG001_2_SiteInformation";
                        historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                        historyLogItem.CREATED_DATE = dateNow;
                        historyLogItem.DESCRIPTION = "Node Name TH: " + itemCoverageArea.NODENAME_TH;
                        historyLogItem.REF_KEY = command.CoverageAreaPanel.NodeNameTH.ToSafeString();
                        historyLogItem.REF_NAME = "Node Name TH";
                        _historyLog.Create(historyLogItem);
                        _uow.Persist();
                        #endregion
                    }
                    #endregion
                }
                else if (command.ActionType == WBBContract.Commands.ActionType.Update)
                {
                    #region update

                    if (command.UpdateCoverageType == UpdateCoverageType.SiteInformation || command.UpdateCoverageType == UpdateCoverageType.SiteCode)
                    {
                        var listCoverage = from ca in _coverageArea.Get()
                                           where ca.CONTACT_ID == command.CoverageAreaPanel.ContactId
                                           select ca;

                        if (listCoverage.Any())
                        {
                            if (command.UpdateCoverageType == UpdateCoverageType.SiteInformation)
                            {
                                #region update Site Information

                                var oldCoveragePanel = new CoverageAreaPanel();
                                var newCoveragePanel = new CoverageAreaPanel();

                                foreach (var item in listCoverage.ToList())
                                {
                                    #region Set Object For Compare
                                    //Set Old Object
                                    oldCoveragePanel.NodeType = item.NODETYPE;
                                    oldCoveragePanel.NodeNameTH = item.NODENAME_TH;
                                    oldCoveragePanel.NodeNameEN = item.NODENAME_EN;
                                    oldCoveragePanel.ContactNumber = item.CONTACT_NUMBER;
                                    oldCoveragePanel.FaxNumber = item.FAX_NUMBER;
                                    oldCoveragePanel.MooTH = item.MOO;
                                    oldCoveragePanel.SoiTH = item.SOI_TH;
                                    oldCoveragePanel.RoadTH = item.ROAD_TH;
                                    oldCoveragePanel.ZipCodeTH = item.ZIPCODE;
                                    oldCoveragePanel.MooEN = item.MOO;
                                    oldCoveragePanel.TieFlag = item.TIE_FLAG == "Y" ? true : false;
                                    oldCoveragePanel.SoiEN = item.SOI_EN;
                                    oldCoveragePanel.RoadEN = item.ROAD_EN;
                                    oldCoveragePanel.ZipCodeEN = item.ZIPCODE;
                                    oldCoveragePanel.IpRanSiteCode = item.IPRAN_CODE;
                                    oldCoveragePanel.CondoCode = item.LOCATIONCODE;
                                    oldCoveragePanel.Lat = item.LATITUDE;
                                    oldCoveragePanel.Long = item.LONGITUDE;
                                    oldCoveragePanel.OnTargetDateIn = item.ONTARGET_DATE_IN;
                                    oldCoveragePanel.OnTargetDateEx = item.ONTARGET_DATE_EX;
                                    //oldCoveragePanel.Status = item.NODESTATUS;                                    
                                    oldCoveragePanel.ConfigComplete = item.COMPLETE_FLAG == "Y" ? true : false;
                                    oldCoveragePanel.RegionCode = item.REGION_CODE;

                                    var addressList = from cz in _coverageZipCode.Get()
                                                      join z in _zipCode.Get() on cz.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                      join z2 in _zipCode.Get() on cz.ZIPCODE_ROWID_EN equals z2.ZIPCODE_ROWID
                                                      where cz.CVRID == item.CVRID
                                                      select new
                                                      {
                                                          PROVINCE_TH = z.PROVINCE,
                                                          AMPHUR_TH = z.AMPHUR,
                                                          TUMBON_TH = z.TUMBON,
                                                          PROVINCE_EN = z2.PROVINCE,
                                                          AMPHUR_EN = z2.AMPHUR,
                                                          TUMBON_EN = z2.TUMBON
                                                      };

                                    if (addressList.Any())
                                    {
                                        var address = addressList.FirstOrDefault();
                                        oldCoveragePanel.ProvinceTH = address.PROVINCE_TH;
                                        oldCoveragePanel.AmphurTH = address.AMPHUR_TH;
                                        oldCoveragePanel.TumbonTH = address.TUMBON_TH;
                                        oldCoveragePanel.ProvinceEN = address.PROVINCE_EN;
                                        oldCoveragePanel.AmphurEN = address.AMPHUR_EN;
                                        oldCoveragePanel.TumbonEN = address.TUMBON_EN;
                                    }

                                    //Set New Object
                                    newCoveragePanel = command.CoverageAreaPanel;

                                    #endregion

                                    item.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    item.UPDATED_DATE = dateNow;

                                    item.NODENAME_EN = command.CoverageAreaPanel.NodeNameEN.ToSafeString();
                                    item.NODENAME_TH = command.CoverageAreaPanel.NodeNameTH.ToSafeString();
                                    item.NODETYPE = command.CoverageAreaPanel.NodeType.ToSafeString();
                                    //item.ACTIVEFLAG = "Y";                                    
                                    item.MOO = command.CoverageAreaPanel.MooTH;
                                    item.SOI_TH = command.CoverageAreaPanel.SoiTH.ToSafeString();
                                    item.ROAD_TH = command.CoverageAreaPanel.RoadTH.ToSafeString();
                                    item.SOI_EN = command.CoverageAreaPanel.SoiEN.ToSafeString();
                                    item.ROAD_EN = command.CoverageAreaPanel.RoadEN.ToSafeString();
                                    item.ZIPCODE = command.CoverageAreaPanel.ZipCodeTH.ToSafeString();
                                    item.CONTACT_NUMBER = command.CoverageAreaPanel.ContactNumber.ToSafeString();
                                    item.FAX_NUMBER = command.CoverageAreaPanel.FaxNumber.ToSafeString();
                                    item.REGION_CODE = command.CoverageAreaPanel.RegionCode.ToSafeString();
                                    //item.CONTACT_ID = command.CoverageAreaPanel.ContactId.ToSafeDecimal();

                                    item.LATITUDE = command.CoverageAreaPanel.Lat.ToSafeString();
                                    item.LONGITUDE = command.CoverageAreaPanel.Long.ToSafeString();

                                    _coverageArea.Update(item);
                                    _uow.Persist();

                                    #region Update Zipcode

                                    var zipcodeTH = from z in _zipcode.Get()
                                                    where z.LANG_FLAG == "N" && z.PROVINCE.StartsWith(command.CoverageAreaPanel.ProvinceTH)
                                                    && z.AMPHUR.StartsWith(command.CoverageAreaPanel.AmphurTH) && z.TUMBON == command.CoverageAreaPanel.TumbonTH
                                                    select z.ZIPCODE_ROWID;

                                    var zipcodeEN = from z in _zipcode.Get()
                                                    where z.LANG_FLAG == "Y" && z.PROVINCE.StartsWith(command.CoverageAreaPanel.ProvinceEN)
                                                    && z.AMPHUR.StartsWith(command.CoverageAreaPanel.AmphurEN) && z.TUMBON == command.CoverageAreaPanel.TumbonEN
                                                    select z.ZIPCODE_ROWID;

                                    if (zipcodeTH.Any() && zipcodeEN.Any())
                                    {
                                        #region Delect before create new
                                        var coverageZipcode = from cz in _coverageZipCode.Get()
                                                              where cz.CVRID == item.CVRID
                                                              select cz;

                                        if (coverageZipcode.Any())
                                        {
                                            foreach (var itemzipcode in coverageZipcode)
                                            {
                                                var iz = _coverageZipCode.Get(a => a.CVRID == itemzipcode.CVRID);

                                                if (iz.Any())
                                                {
                                                    foreach (var cZip in iz)
                                                    {
                                                        _coverageZipCode.Delete(cZip);
                                                    }
                                                }
                                                //var iz = _coverageZipCode.Get(a => a.CVRID == itemzipcode.CVRID).FirstOrDefault();
                                                //if (iz != null)
                                                //    _coverageZipCode.Delete(iz);
                                            }

                                            _uow.Persist();
                                        }
                                        #endregion

                                        var listZipcodeTH = zipcodeTH.ToList();
                                        var listZipCodeEN = zipcodeEN.ToList();

                                        var countTH = zipcodeTH.Count();
                                        var countEN = zipcodeEN.Count();

                                        for (int i = 0; i < listZipcodeTH.Count; i++)
                                        {
                                            var itemCoverageZipcode = new FBB_COVERAGE_ZIPCODE()
                                            {
                                                CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString(),
                                                CREATED_DATE = dateNow,
                                                CVRID = item.CVRID,
                                                UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString(),
                                                UPDATED_DATE = dateNow,
                                                ZIPCODE_ROWID_EN = i < countEN ? listZipCodeEN[i] : "",
                                                ZIPCODE_ROWID_TH = i < countTH ? listZipcodeTH[i] : ""
                                            };

                                            _coverageZipCode.Create(itemCoverageZipcode);
                                        }
                                    }
                                    #endregion

                                    _uow.Persist();
                                }

                                #region Update FBB_HISTORY_LOG
                                var historyLogItem = new FBB_HISTORY_LOG();
                                historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                                historyLogItem.APPLICATION = "FBB_CFG001_2_SiteInformation";
                                historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                historyLogItem.CREATED_DATE = dateNow;
                                historyLogItem.DESCRIPTION = "Node Name TH: " + oldCoveragePanel.NodeNameTH + " => " + WBBExtensions.CompareObjectToString(oldCoveragePanel, newCoveragePanel);
                                historyLogItem.REF_KEY = newCoveragePanel.NodeNameTH;
                                historyLogItem.REF_NAME = "Node Name TH";

                                if (historyLogItem.DESCRIPTION != string.Empty)
                                {
                                    _historyLog.Create(historyLogItem);
                                    _uow.Persist();
                                }
                                #endregion

                                #endregion
                            }
                            else
                            {
                                #region Update Site Code
                                foreach (var item in listCoverage)
                                {
                                    var oldCoveragePanel = new CoverageAreaPanel();
                                    var newCoveragePanel = new CoverageAreaPanel();

                                    #region Set Object For Compare
                                    //Set Old Object        
                                    oldCoveragePanel.IpRanSiteCode = item.IPRAN_CODE;
                                    oldCoveragePanel.CondoCode = item.LOCATIONCODE;

                                    newCoveragePanel.IpRanSiteCode = command.CoverageAreaPanel.IpRanSiteCode.ToSafeString();
                                    newCoveragePanel.CondoCode = command.CoverageAreaPanel.CondoCode.ToSafeString();
                                    #endregion

                                    item.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    item.UPDATED_DATE = dateNow;

                                    item.LOCATIONCODE = command.CoverageAreaPanel.CondoCode.ToSafeString(); // site                         
                                    item.IPRAN_CODE = command.CoverageAreaPanel.IpRanSiteCode.ToSafeString(); // site 
                                    //item.LATITUDE = command.CoverageAreaPanel.Lat.ToSafeString(); // site 
                                    //item.LONGITUDE = command.CoverageAreaPanel.Long.ToSafeString(); // site 

                                    _coverageArea.Update(item);
                                    _uow.Persist();

                                    #region Update FBB_HISTORY_LOG
                                    var historyLogItem = new FBB_HISTORY_LOG();
                                    historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                                    historyLogItem.APPLICATION = "FBB_CFG001_2_SiteInformation";
                                    historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    historyLogItem.CREATED_DATE = dateNow;
                                    historyLogItem.DESCRIPTION = "Node Name TH: " + command.CoverageAreaPanel.NodeNameTH.ToSafeString() + " => " + WBBExtensions.CompareObjectToString(oldCoveragePanel, newCoveragePanel);
                                    historyLogItem.REF_KEY = command.CoverageAreaPanel.NodeNameTH.ToSafeString();
                                    historyLogItem.REF_NAME = "Node Name TH";

                                    if (historyLogItem.DESCRIPTION != string.Empty)
                                    {
                                        _historyLog.Create(historyLogItem);
                                        _uow.Persist();
                                    }
                                    #endregion
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Cannot find the data for " + command.ActionType.ToString();
                            return;
                        }
                    }
                    else if (command.UpdateCoverageType == UpdateCoverageType.CoverageInformation)
                    {
                        #region Update CoverageInformation

                        var listCoverage = from ca in _coverageArea.Get()
                                           where ca.CONTACT_ID == command.CoverageAreaPanel.ContactId
                                           && ca.CVRID == command.CoverageAreaPanel.CVRId
                                           select ca;

                        if (listCoverage.Any())
                        {
                            var itemCoverageArea = listCoverage.FirstOrDefault();

                            var oldCoveragePanel = new CoverageAreaPanel();
                            var newCoveragePanel = new CoverageAreaPanel();

                            #region Set old object.
                            oldCoveragePanel.Status = itemCoverageArea.NODESTATUS;
                            oldCoveragePanel.ConfigComplete = itemCoverageArea.COMPLETE_FLAG == "Y" ? true : false;
                            oldCoveragePanel.OnTargetDateIn = itemCoverageArea.ONTARGET_DATE_IN;
                            oldCoveragePanel.OnTargetDateEx = itemCoverageArea.ONTARGET_DATE_EX;
                            oldCoveragePanel.BuildingCode = itemCoverageArea.BUILDINGCODE;
                            oldCoveragePanel.TieFlag = itemCoverageArea.TIE_FLAG == "Y" ? true : false;

                            newCoveragePanel.Status = command.CoverageAreaPanel.Status.ToSafeString();
                            newCoveragePanel.ConfigComplete = command.CoverageAreaPanel.ConfigComplete;
                            newCoveragePanel.OnTargetDateIn = command.CoverageAreaPanel.OnTargetDateIn;
                            newCoveragePanel.OnTargetDateEx = command.CoverageAreaPanel.OnTargetDateEx;
                            newCoveragePanel.BuildingCode = command.CoverageAreaPanel.BuildingCode.ToSafeString();
                            newCoveragePanel.TieFlag = command.CoverageAreaPanel.TieFlag;
                            #endregion

                            itemCoverageArea.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                            itemCoverageArea.UPDATED_DATE = dateNow;

                            itemCoverageArea.NODESTATUS = command.CoverageAreaPanel.Status.ToSafeString(); // coverage info
                            itemCoverageArea.COMPLETE_FLAG = command.CoverageAreaPanel.ConfigComplete == true ? "Y" : "N"; // coverage info
                            itemCoverageArea.TIE_FLAG = command.CoverageAreaPanel.TieFlag == true ? "Y" : "N"; // coverage info
                            itemCoverageArea.ONTARGET_DATE_IN = command.CoverageAreaPanel.OnTargetDateIn; // coverage info
                            itemCoverageArea.ONTARGET_DATE_EX = command.CoverageAreaPanel.OnTargetDateEx; // coverage info
                            itemCoverageArea.BUILDINGCODE = command.CoverageAreaPanel.BuildingCode.ToSafeString(); // coverage info

                            _coverageArea.Update(itemCoverageArea);
                            _uow.Persist();

                            #region Insert FBB_HISTORY_LOG
                            var historyLogItem = new FBB_HISTORY_LOG();
                            historyLogItem.ACTION = ActionHistory.UPDATE.ToString();
                            historyLogItem.APPLICATION = "FBB_CFG001_2_Coverage";
                            historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                            historyLogItem.CREATED_DATE = dateNow;
                            historyLogItem.DESCRIPTION = "Building: " + newCoveragePanel.BuildingCode + " => " + WBBExtensions.CompareObjectToString(oldCoveragePanel, newCoveragePanel);
                            historyLogItem.REF_KEY = itemCoverageArea.NODENAME_TH;
                            historyLogItem.REF_NAME = "Node Name TH";

                            if (historyLogItem.DESCRIPTION != string.Empty)
                            {
                                _historyLog.Create(historyLogItem);
                                _uow.Persist();
                            }
                            #endregion
                        }
                        else
                        {
                            command.Return_Code = 0;
                            command.Return_Desc = "Cannot find the data for " + command.ActionType.ToString();
                            return;
                        }
                        #endregion
                    }
                    else
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Cannot " + command.ActionType.ToString();
                        return;
                    }

                    #endregion
                }
                else if (command.ActionType == WBBContract.Commands.ActionType.Delete)
                {
                    #region delete

                    var listCoverageAll = from ca in _coverageArea.Get()
                                          where ca.CONTACT_ID == command.CoverageAreaPanel.ContactId
                                          select ca;

                    if (listCoverageAll.Any())
                    {
                        if (listCoverageAll.Count() == 1 && command.FlagDelectAll == false)
                        {
                            command.FlagDelectAll = true;
                        }

                        if (command.FlagDelectAll == true)
                        {
                            #region delect all

                            if (command.FlagDelectAll == true)
                            {
                                foreach (var coverage in listCoverageAll)
                                {
                                    if (coverage.NODESTATUS != "ON_PROGRESS")
                                    {
                                        command.Return_Code = 0;
                                        command.Return_Desc = "Cannot delete because staus is not 'On_Progress'";
                                        return;
                                    }
                                }

                                foreach (var item in listCoverageAll)
                                {
                                    #region delect FBB_COVERAGEAREA
                                    item.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    item.UPDATED_DATE = dateNow;
                                    item.ACTIVEFLAG = "N";
                                    _coverageArea.Update(item);
                                    #endregion
                                    #region delect FBB_COVERAGEAREA_RELATION
                                    var itemCoverageAreaRelation = from cr in _coverageRelation.Get()
                                                                   where cr.CVRID == item.CVRID
                                                                   && cr.ACTIVEFLAG == "Y"
                                                                   select cr;

                                    if (itemCoverageAreaRelation.Any())
                                    {
                                        var coverageAreaRelation = itemCoverageAreaRelation.FirstOrDefault();
                                        coverageAreaRelation.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                        coverageAreaRelation.UPDATED_DATE = dateNow;
                                        coverageAreaRelation.ACTIVEFLAG = "N";
                                        _coverageRelation.Update(coverageAreaRelation);
                                    }
                                    #endregion
                                    #region delect FBB_COVERAGE_ZIPCODE
                                    var coverageZipcode = from cz in _coverageZipCode.Get()
                                                          where cz.CVRID == item.CVRID
                                                          select cz;

                                    if (coverageZipcode.Any())
                                    {
                                        foreach (var itemzipcode in coverageZipcode)
                                        {
                                            var iz = _coverageZipCode.Get(a => a.CVRID == itemzipcode.CVRID);

                                            if (iz.Any())
                                            {
                                                foreach (var cZip in iz)
                                                {
                                                    _coverageZipCode.Delete(cZip);
                                                }
                                            }
                                            //var iz = _coverageZipCode.Get(a => a.CVRID == itemzipcode.CVRID).FirstOrDefault();
                                            //if (iz != null)
                                            //    _coverageZipCode.Delete(iz);
                                        }
                                        _uow.Persist();
                                    }
                                    #endregion

                                    List<decimal> listDslamId = new List<decimal>();
                                    List<decimal> listCardId = new List<decimal>();

                                    var coverageRelation = from cr in _coverageRelation.Get()
                                                           where cr.CVRID == item.CVRID
                                                           select cr.DSLAMID;

                                    if (coverageRelation.Any())
                                    {
                                        listDslamId = coverageRelation.ToList();

                                        #region delete FBB_DSLAM_INFO
                                        var itemDSLAMInfo = from di in _dslamInfo.Get()
                                                            where listDslamId.Contains(di.DSLAMID)
                                                            select di;

                                        if (itemDSLAMInfo.Any())
                                        {
                                            foreach (var itemDslam in itemDSLAMInfo.ToList())
                                            {
                                                itemDslam.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                                itemDslam.UPDATED_DATE = dateNow;
                                                itemDslam.ACTIVEFLAG = "N";
                                                _dslamInfo.Update(itemDslam);
                                            }
                                        }
                                        #endregion
                                        #region delete FBB_CARD_INFO
                                        var itemCardInfo = from ci in _cardInfo.Get()
                                                           where listDslamId.Contains(ci.DSLAMID)
                                                           select ci;

                                        if (itemCardInfo.Any())
                                        {
                                            foreach (var itemCard in itemCardInfo)
                                            {
                                                itemCard.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                                itemCard.UPDATED_DATE = dateNow;
                                                itemCard.ACTIVEFLAG = "N";
                                                _cardInfo.Update(itemCard);
                                                listCardId.Add(itemCard.CARDID);
                                            }
                                        }

                                        #endregion
                                        #region delect FBB_PORT_INFO
                                        if (listCardId.Count() > 0)
                                        {
                                            var itemPortInfo = from pi in _portInfo.Get()
                                                               where listCardId.Contains(pi.CARDID)
                                                               select pi;

                                            if (itemPortInfo.Any())
                                            {
                                                foreach (var itemPort in itemPortInfo.ToList())
                                                {
                                                    itemPort.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                                    itemPort.UPDATED_DATE = dateNow;
                                                    itemPort.ACTIVEFLAG = "N";
                                                    _portInfo.Update(itemPort);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    _uow.Persist();

                                    #region Delete FBB_HISTORY_LOG
                                    var historyLogItem = new FBB_HISTORY_LOG();
                                    historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                                    historyLogItem.APPLICATION = "FBB_CFG001_2_SiteInformation";
                                    historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    historyLogItem.CREATED_DATE = dateNow;
                                    historyLogItem.DESCRIPTION = "Node Name TH: " + item.NODENAME_TH + ", Building Code: " + item.BUILDINGCODE;
                                    historyLogItem.REF_KEY = item.NODENAME_TH;
                                    historyLogItem.REF_NAME = "Node Name TH";
                                    _historyLog.Create(historyLogItem);
                                    _uow.Persist();
                                    command.CoverageAreaPanel.NodeNameTH = item.NODENAME_TH;
                                    #endregion
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region delect single

                            var listCoverage = from ca in _coverageArea.Get()
                                               where ca.CVRID == command.CoverageAreaPanel.CVRId
                                               select ca;

                            if (listCoverage.Any())
                            {
                                var coverage = listCoverage.FirstOrDefault();

                                if (coverage.NODESTATUS == "ON_PROGRESS")
                                {
                                    #region delect FBB_COVERAGEAREA
                                    coverage.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    coverage.UPDATED_DATE = dateNow;
                                    coverage.ACTIVEFLAG = "N";
                                    _coverageArea.Update(coverage);
                                    #endregion
                                    #region delect FBB_COVERAGEAREA_RELATION
                                    var itemCoverageAreaRelation = from cr in _coverageRelation.Get()
                                                                   where cr.CVRID == coverage.CVRID
                                                                   select cr;

                                    if (itemCoverageAreaRelation.Any())
                                    {
                                        foreach (var item in itemCoverageAreaRelation.ToList())
                                        {
                                            item.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                            item.UPDATED_DATE = dateNow;
                                            item.ACTIVEFLAG = "N";
                                            _coverageRelation.Update(item);
                                        }
                                    }
                                    #endregion
                                    #region delect FBB_COVERAGE_ZIPCODE
                                    var coverageZipcode = from cz in _coverageZipCode.Get()
                                                          where cz.CVRID == coverage.CVRID
                                                          select cz;

                                    if (coverageZipcode.Any())
                                    {
                                        foreach (var itemzipcode in coverageZipcode)
                                        {
                                            var iz = _coverageZipCode.Get(a => a.CVRID == itemzipcode.CVRID);

                                            if (iz.Any())
                                            {
                                                foreach (var cZip in iz)
                                                {
                                                    _coverageZipCode.Delete(cZip);
                                                }
                                            }
                                            //var iz = _coverageZipCode.Get(a => a.CVRID == itemZipCode.CVRID).FirstOrDefault();
                                            //if (iz != null)
                                            //{
                                            //    _coverageZipCode.Delete(iz);
                                            //    _uow.Persist();
                                            //}
                                        }
                                    }
                                    #endregion

                                    List<decimal> listDslamId = new List<decimal>();
                                    List<decimal> listCardId = new List<decimal>();

                                    var coverageRelation = from cr in _coverageRelation.Get()
                                                           where cr.CVRID == coverage.CVRID
                                                           select cr.DSLAMID;

                                    if (coverageRelation.Any())
                                    {
                                        listDslamId = coverageRelation.ToList();

                                        #region delete FBB_DSLAM_INFO
                                        var itemDSLAMInfo = from di in _dslamInfo.Get()
                                                            where listDslamId.Contains(di.DSLAMID)
                                                            select di;

                                        if (itemDSLAMInfo.Any())
                                        {
                                            foreach (var itemDslam in itemDSLAMInfo.ToList())
                                            {
                                                itemDslam.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                                itemDslam.UPDATED_DATE = dateNow;
                                                itemDslam.ACTIVEFLAG = "N";
                                                _dslamInfo.Update(itemDslam);
                                            }
                                        }
                                        #endregion
                                        #region delete FBB_CARD_INFO
                                        var itemCardInfo = from ci in _cardInfo.Get()
                                                           where listDslamId.Contains(ci.DSLAMID)
                                                           select ci;

                                        if (itemCardInfo.Any())
                                        {
                                            foreach (var itemCard in itemCardInfo)
                                            {
                                                itemCard.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                                itemCard.UPDATED_DATE = dateNow;
                                                itemCard.ACTIVEFLAG = "N";
                                                _cardInfo.Update(itemCard);
                                                listCardId.Add(itemCard.CARDID);
                                            }
                                        }

                                        #endregion
                                        #region delect FBB_PORT_INFO
                                        if (listCardId.Count() > 0)
                                        {
                                            var itemPortInfo = from pi in _portInfo.Get()
                                                               where listCardId.Contains(pi.CARDID)
                                                               select pi;

                                            if (itemPortInfo.Any())
                                            {
                                                foreach (var itemPort in itemPortInfo.ToList())
                                                {
                                                    itemPort.UPDATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                                    itemPort.UPDATED_DATE = dateNow;
                                                    itemPort.ACTIVEFLAG = "N";
                                                    _portInfo.Update(itemPort);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    _uow.Persist();

                                    #region Insert FBB_HISTORY_LOG
                                    var historyLogItem = new FBB_HISTORY_LOG();
                                    historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                                    historyLogItem.APPLICATION = "FBB_CFG001_2_Coverage";
                                    historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                    historyLogItem.CREATED_DATE = dateNow;
                                    historyLogItem.DESCRIPTION = "Node Name TH: " + coverage.NODENAME_TH + ", Building Code: " + coverage.BUILDINGCODE;
                                    historyLogItem.REF_KEY = coverage.NODENAME_TH;
                                    historyLogItem.REF_NAME = "Node Name TH";
                                    _historyLog.Create(historyLogItem);
                                    _uow.Persist();
                                    #endregion
                                }
                                else
                                {
                                    command.Return_Code = 0;
                                    command.Return_Desc = "Cannot delete because staus is not 'On_Progress'";
                                    return;
                                }
                            }
                            else
                            {
                                command.Return_Code = 0;
                                command.Return_Desc = "Cannot delete because not found the data for delete.";
                                return;
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "Cannot delete because not found the data for delete.";
                        return;
                    }
                    #endregion
                }

                #region delect FBB_COVERAGEAREA_BUILDING
                if (command.ActionType == WBBContract.Commands.ActionType.Delete && command.FlagDelectAll == true)
                {
                    var itemBuilding = from b in _coverageAreaBuilding.Get()
                                       where b.CONTACT_ID == command.CoverageAreaPanel.ContactId
                                       select b;

                    if (itemBuilding.Any())
                    {
                        foreach (var item in itemBuilding.ToList())
                        {
                            var bd = _coverageAreaBuilding.Get(b => b.CONTACT_ID == item.CONTACT_ID).FirstOrDefault();
                            if (bd != null)
                            {
                                _coverageAreaBuilding.Delete(bd);
                                _uow.Persist();

                                #region Delete FBB_HISTORY_LOG
                                var historyLogItem = new FBB_HISTORY_LOG();
                                historyLogItem.ACTION = ActionHistory.DELETE.ToString();
                                historyLogItem.APPLICATION = "FBB_CFG001_2_Building";
                                historyLogItem.CREATED_BY = command.CoverageAreaPanel.CreateBy.ToSafeString();
                                historyLogItem.CREATED_DATE = dateNow;
                                historyLogItem.DESCRIPTION = "Building: " + item.BUILDING;
                                historyLogItem.REF_KEY = command.CoverageAreaPanel.NodeNameTH;
                                historyLogItem.REF_NAME = "Node Name TH";
                                _historyLog.Create(historyLogItem);
                                _uow.Persist();
                                #endregion
                            }
                        }
                    }
                }
                #endregion

                command.Return_Code = 1;
                command.Return_Desc = command.ActionType + " success.";
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
                command.Return_Code = -1;
                command.Return_Desc = "Error call CoverageAreaCommand - " + command.ActionType + " : " + ex.GetErrorMessage();
            }
        }
    }
}

