using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class SiebelGenActivityQueryHandler : IQueryHandler<SiebelGenActivityQuery, SiebelGenActivityQueryModel>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;

        public SiebelGenActivityQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
        }
        public SiebelGenActivityQueryModel Handle(SiebelGenActivityQuery query)
        {
            var resultnop = "";
            string ERROR_SPCMESSAGE, ERRORMESSAGE, CHECKORDERID = "";
            var modelResult = new SiebelGenActivityQueryModel();
            InterfaceLogCommand log = null;
            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.SRNUMBER, "SiebelGenActivityQuery", "SiebelGenActivity.AIS_spcIM_spcCommon_spcCreate_spcActivity", query.ID_CARD_NO, "FBB|" + query.FullURL, "");

                using (var service = new SiebelGenActivity.AIS_spcIM_spcCommon_spcCreate_spcActivity())
                {
                    string KeyData = "";
                    string UrlData = "";

                    KeyData = (from z in _lov.Get()
                               where z.LOV_NAME == "sobs" && z.ACTIVEFLAG == "Y"
                               select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                    UrlData = (from z in _lov.Get()
                               where z.LOV_NAME == "SiebelService" && z.ACTIVEFLAG == "Y" && z.DISPLAY_VAL == "Endpoint"
                               select z.LOV_VAL1).FirstOrDefault().ToSafeString();

                    service.Url = EncryptionUtility.Decrypt(UrlData, KeyData);

                    CHECKORDERID = query.CHECKORDERID;
                    var result = service.CommonCreateActivity(
                        query.SRNUMBER,
                        query.ORDERID,
                        query.SERIALNUMBER,
                        query.DONE,
                        query.STATUS,
                        query.PLANNED,
                        query.CAMPAIGNID,
                        query.PRIMARYOWNERID,
                        query.COMMENT,
                        query.NOSOONERTHANDATE,
                        query.ACTIVITYCATEGORY,
                        query.REASON,
                        query.TYPE,
                        query.STARTED,
                        query.DOCUMENTNO,
                        query.MOREINFO,
                        ref CHECKORDERID,
                        query.ASSETID,
                        query.ACCOUNTID,
                        query.SUBSTATUS,
                        query.CONTACTID,
                        query.ACTIVITYSUBCATEGORY,
                        query.PRIORITY,
                        query.PRIMARYPRODUCTID,
                        query.OWNERNAME, out ERROR_SPCMESSAGE, out ERRORMESSAGE);

                    resultnop = result;

                    if (ERROR_SPCMESSAGE == "" && ERRORMESSAGE == "")
                    {
                        modelResult.CHECKORDERID = CHECKORDERID;
                        modelResult.ERROR_SPCMESSAGE = ERROR_SPCMESSAGE;
                        modelResult.ERRORMESSAGE = ERRORMESSAGE;

                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, modelResult, log, "Success", "", "");
                    }
                    else
                    {
                        modelResult.CHECKORDERID = CHECKORDERID;
                        modelResult.ERROR_SPCMESSAGE = ERROR_SPCMESSAGE;
                        modelResult.ERRORMESSAGE = ERRORMESSAGE;

                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultnop, log, "Failed", "Unsuccess", "");
                    }

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                modelResult.CHECKORDERID = CHECKORDERID;
                modelResult.ERROR_SPCMESSAGE = "";
                modelResult.ERRORMESSAGE = ex.Message;
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultnop, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return modelResult;
        }

    }
}
