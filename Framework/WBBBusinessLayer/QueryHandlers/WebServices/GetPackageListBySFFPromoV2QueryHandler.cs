using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
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
    public class GetPackageListBySFFPromoV2QueryHandler : IQueryHandler<GetPackageListBySFFPromoV2Query, List<PackageModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetPackageListBySFFPromoV2QueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
        }

        public List<PackageModel> Handle(GetPackageListBySFFPromoV2Query query)
        {
            InterfaceLogCommand log = null;
            //List<FBB_CFG_LOV> loveList = null;
            var packages = new List<PackageModel>();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "list_package_by_sffpromo_v2", "GetPackageListBySFFPromoQueryV2Handler", null, "FBB|" + query.FullUrl, "");

                OnlineQueryConfigModel config = new OnlineQueryConfigModel();
                List<FBB_CFG_LOV> loveConfigList = null;
                loveConfigList = _lov
                .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBB_CONFIG")).ToList();

                if (loveConfigList != null && loveConfigList.Count() > 0)
                {
                    config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO").LOV_VAL1 : "";
                    config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO").LOV_VAL2 : "";
                    config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType").LOV_VAL1 : "";
                    config.Channel = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel").LOV_VAL1 : "";


                    if (config.Url != "")
                    {
                        packages = GetPackageListHelper.GetPackageListbySFFPromoOnline(_uow, _intfLog, _logger, query, _lov, config);
                    }
                }

                //string OnlineQueryUse = "A";

                //loveList = _lov
                //  .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME.Equals("CALL_LIST_PACKSFFPROMO")).ToList();

                //if (loveList != null && loveList.Count() > 0)
                //{
                //    OnlineQueryUse = loveList.FirstOrDefault().LOV_VAL1.ToSafeString();
                //}

                //if (OnlineQueryUse == "A")
                //{
                //    packages = GetPackageListHelper.GetPackageListbySFFPromoV2(_logger, query, _lov);
                //}
                //else
                //{
                //    OnlineQueryConfigModel config = new OnlineQueryConfigModel();
                //    List<FBB_CFG_LOV> loveConfigList = null;
                //    loveConfigList = _lov
                //    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBB_CONFIG")).ToList();

                //    if (loveConfigList != null && loveConfigList.Count() > 0)
                //    {
                //        config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO").LOV_VAL1 : "";
                //        config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PACKSFFPROMO").LOV_VAL2 : "";
                //        config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType").LOV_VAL1 : "";
                //        config.Channel = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel").LOV_VAL1 : "";


                //        if (config.Url != "")
                //        {
                //            packages = GetPackageListHelper.GetPackageListbySFFPromoOnline(_uow, _intfLog, _logger, query, _lov, config);
                //        }
                //    }
                //}

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packages, log, "Success", "", "");

                return packages;
            }
            catch (System.Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.Message, "");

                throw ex;
            }


        }

    }
}
