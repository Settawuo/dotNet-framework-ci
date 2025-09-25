using System;
using System.Collections.Generic;
using System.Data;
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

namespace WBBBusinessLayer.QueryHandlers.GetListPackageV2
{
    public class GetListPackageByServiceV2QueryHandler : IQueryHandler<GetListPackageByServiceV2Query, PackageDataV2Model>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _obj;

        public GetListPackageByServiceV2QueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<object> obj)
        {
            _logger = logger;
            _lov = lov;
            _intfLog = intfLog;
            _uow = uow;
            _obj = obj;
        }

        public PackageDataV2Model Handle(GetListPackageByServiceV2Query query)
        {
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "GetListPackageByServiceV2", "GetListPackageByServiceV2QueryHandler", null, "FBB|" + query.FullUrl, "");
            PackageDataV2Model packageDataV2 = new PackageDataV2Model();
            List<PackageGroupV2Model> packageGroups = new List<PackageGroupV2Model>();
            List<PackageV2Model> packages = new List<PackageV2Model>();

            try
            {
                List<FBB_CFG_LOV> loveList = null;
                string OnlineQueryUse = "A";

                loveList = _lov
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_NAME.Equals("CALL_LIST_PROMOTION")).ToList();

                if (loveList != null && loveList.Count() > 0)
                {
                    OnlineQueryUse = loveList.FirstOrDefault().LOV_VAL1.ToSafeString();
                }

                if (OnlineQueryUse == "A")
                {
                    string tmpUrl = (from r in _lov.Get()
                                     where r.LOV_NAME == "SaveOrderNewURL" && r.ACTIVEFLAG == "Y"
                                     select r.LOV_VAL1).FirstOrDefault().ToSafeString();

                    packages = GetPackageListHelper.GetPackageListV2(_logger, query, tmpUrl);
                }
                else
                {
                    OnlineQueryConfigModel config = new OnlineQueryConfigModel();
                    List<FBB_CFG_LOV> loveConfigList = null;
                    loveConfigList = _lov
                    .Get(lov => lov.ACTIVEFLAG.Equals("Y") && lov.LOV_TYPE.Equals("FBB_CONFIG")).ToList();

                    if (loveConfigList != null && loveConfigList.Count() > 0)
                    {
                        config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PROMOTION") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PROMOTION").LOV_VAL1 : "";
                        config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PROMOTION") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_CALL_PROMOTION").LOV_VAL2 : "";
                        config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ContentType").LOV_VAL1 : "";
                        config.Channel = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "x-online-query-channel").LOV_VAL1 : "";

                        if (config.Url != "")
                        {
                            //GetProductSubtype by P_OWNER_PRODUCT and P_Address_Id
                            if (query.P_PRODUCT_SUBTYPE.ToSafeString() == "")
                            {
                                query.P_PRODUCT_SUBTYPE = GetPackageListHelper.GetProductSubtype(_obj, query.P_OWNER_PRODUCT.ToSafeString(), query.P_Address_Id.ToSafeString());
                            }
                            packages = GetPackageListHelper.GetPackageListOnlineQuery(_uow, _intfLog, _logger, query, config);
                        }
                    }

                }

                string[] PRODUCT_SUBTYPE_ARRY = new string[4];
                PRODUCT_SUBTYPE_ARRY[0] = "WireBB";
                PRODUCT_SUBTYPE_ARRY[1] = "FTTx";
                PRODUCT_SUBTYPE_ARRY[2] = "FTTR";
                PRODUCT_SUBTYPE_ARRY[3] = "WTTx";

                var mainPackage = packages.Where(p => p.PACKAGE_TYPE == "1" && PRODUCT_SUBTYPE_ARRY.Contains(p.PRODUCT_SUBTYPE) && string.IsNullOrEmpty(p.AUTO_MAPPING_PROMOTION_CODE)).ToList();

                var topNoGroup = mainPackage.Where(p => string.IsNullOrEmpty(p.PACKAGE_GROUP)).ToList();
                if (topNoGroup != null && topNoGroup.Count > 0)
                {
                    var tmpTopNoGroup = topNoGroup.GroupBy(g => new { g.PRODUCT_SUBTYPE })
                    .Select(s => new { s.Key.PRODUCT_SUBTYPE }).ToList();

                    if (tmpTopNoGroup.Any())
                        foreach (var a in tmpTopNoGroup)
                            packageGroups.Add(new PackageGroupV2Model
                            {
                                PACKAGE_GROUP = "",
                                PACKAGE_GROUP_DESC_ENG = "",
                                PACKAGE_GROUP_DESC_THA = "",
                                PACKAGE_REMARK_ENG = "",
                                PACKAGE_REMARK_THA = "",
                                PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE,
                                PackageItems = mainPackage.Where(t => string.IsNullOrEmpty(t.PACKAGE_GROUP) && t.PRODUCT_SUBTYPE == a.PRODUCT_SUBTYPE).ToList()
                            });
                }

                var topHaveGroup = mainPackage.Where(p => !string.IsNullOrEmpty(p.PACKAGE_GROUP))
                    .GroupBy(g => new
                    {
                        g.PACKAGE_GROUP,
                        g.PACKAGE_GROUP_DESC_ENG,
                        g.PACKAGE_GROUP_DESC_THA,
                        g.PACKAGE_REMARK_ENG,
                        g.PACKAGE_REMARK_THA,
                        g.PRODUCT_SUBTYPE
                    })
                    .Select(s => new
                    {
                        s.Key.PACKAGE_GROUP,
                        s.Key.PACKAGE_GROUP_DESC_ENG,
                        s.Key.PACKAGE_GROUP_DESC_THA,
                        s.Key.PACKAGE_REMARK_ENG,
                        s.Key.PACKAGE_REMARK_THA,
                        s.Key.PRODUCT_SUBTYPE
                    }).ToList();

                if (topHaveGroup != null && topHaveGroup.Count > 0)
                {
                    if (topHaveGroup.Any())
                        foreach (var a in topHaveGroup)
                            packageGroups.Add(new PackageGroupV2Model
                            {
                                PACKAGE_GROUP = a.PACKAGE_GROUP,
                                PACKAGE_GROUP_DESC_ENG = a.PACKAGE_GROUP_DESC_ENG,
                                PACKAGE_GROUP_DESC_THA = a.PACKAGE_GROUP_DESC_THA,
                                PACKAGE_REMARK_ENG = a.PACKAGE_REMARK_ENG,
                                PACKAGE_REMARK_THA = a.PACKAGE_REMARK_THA,
                                PRODUCT_SUBTYPE = a.PRODUCT_SUBTYPE,
                                PackageItems = mainPackage.Where(t => !string.IsNullOrEmpty(t.PACKAGE_GROUP) && t.PACKAGE_GROUP == a.PACKAGE_GROUP && t.PRODUCT_SUBTYPE == a.PRODUCT_SUBTYPE).ToList()
                            });
                }

                packageDataV2.PackageGroupList = packageGroups;
                packageDataV2.PackageList = packages;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, packageDataV2.PackageList, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info("ex message" + ex.Message + " error inner" + ex.InnerException);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
            }

            return packageDataV2;
        }

    }
}
