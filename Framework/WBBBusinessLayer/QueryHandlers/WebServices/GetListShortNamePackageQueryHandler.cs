using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WBBBusinessLayer.SBNNewWebService;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetListShortNamePackageQueryHandler : IQueryHandler<GetListShortNamePackageQuery, List<ListShortNameModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public GetListShortNamePackageQueryHandler(
            ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
        }

        public List<ListShortNameModel> Handle(GetListShortNamePackageQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_id, "GetShortName", "GetListShortNamePackageQueryHandler", null, "FBB|" + query.FullUrl, "");
            List<ListShortNameModel> result = null;
            try
            {
                //AIR_CHANGE_OLD_PACKAGE_ARRAY[] package = query.airChangePromotionCode_List.Select(a => new AIR_CHANGE_OLD_PACKAGE_ARRAY
                //{
                //    enddt = a.enddt,
                //    productSeq = a.productSeq,
                //    sffPromotionCode = a.sffPromotionCode,
                //    startdt = a.startdt

                //}).ToArray();


                airChangePackageRecord[] changeOldPackage = query.airChangePromotionCode_List.Select(a => new airChangePackageRecord
                {
                    enddt = a.enddt,
                    productSeq = a.productSeq,
                    sffPromotionCode = a.sffPromotionCode,
                    startdt = a.startdt
                }).ToArray();

                List<listPackageShortNameResult> objShortName = new List<listPackageShortNameResult>();
                //ListShortNameModel result = new ListShortNameModel();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNNewWebService.SBNWebServiceService())
                {
                    service.Timeout = 600000;
                    //objShortName = service.ListPackageShortName("", changeOldPackage).ToList();
                    objShortName = service.ListPackageShortName("", changeOldPackage).ToList();
                }

                result = objShortName.Select(r => new ListShortNameModel
                {
                    package_class = r.packageClass,
                    package_Short_Name_EN = r.packageShortNameEN,
                    package_Short_Name_TH = r.packageShortNameTH,
                    sff_promotion_code = r.sffPromotionCode

                }).ToList();

                _uow.Persist();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Success", "", "");

                return result;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");
                return result;
            }
        }
    }
}
