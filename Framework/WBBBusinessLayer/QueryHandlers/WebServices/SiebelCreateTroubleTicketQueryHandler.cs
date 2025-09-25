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
    public class SiebelCreateTroubleTicketQueryHandler : IQueryHandler<SiebelCreateTroubleTicketQuery, SiebelCreateTroubleTicketModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;

        public SiebelCreateTroubleTicketQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
        }

        public SiebelCreateTroubleTicketModel Handle(SiebelCreateTroubleTicketQuery query)
        {
            string outParam1, outParam2, outParam3, outParam4,
                   outParam5, OutResult, OutTTNumber;

            var resultnop = "";
            var modelResult = new SiebelCreateTroubleTicketModel();
            InterfaceLogCommand log = null;

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.InMobileNumber, "SiebelCreateTroubleTicketQuery", "Siebel_CreateTroubleTicketService.AIS_spcCreate_spcTrouble_spcTicket", "", "FBB|" + query.FullURL, "");

                using (var service = new Siebel_CreateTroubleTicketService.AIS_spcCreate_spcTrouble_spcTicket())
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

                    var result = service.CreateTT(query.InProblemDate_End, query.InMooban, query.InMobileNumber, query.InIndoor, query.InDestModel, query.InParam1
                        , query.InFloor, query.InAssetId, query.InTumbol, query.InRefArea, query.InCurrentSignalLevel, query.InStreet, query.InParam2
                        , query.InDestMobileNumber, query.InAccountId, query.InUsedCountry, query.InChannel, query.InBuilding, query.InSoi, query.InParam3, query.InModel
                        , query.InSubCategory, query.InProvince, query.InProblemDate, query.InPath, query.InDescription, query.InOption, query.InMaxSignalLevel
                        , query.InHouseNumber, query.InProductName, query.InDestBrand, query.InAmphur, query.InParam5, query.InParam4, query.InContactId
                        , query.InSymptomNote, query.InOtherContactPhone, query.InOperatorName, query.InBrand, query.InContentProvider, query.InCategory
                        , out outParam1, out outParam2, out outParam3, out outParam4, out outParam5, out OutResult, out OutTTNumber);

                    resultnop = result;

                    if (OutResult == "Success")
                    {
                        modelResult.OutParam1 = outParam1;
                        modelResult.OutParam2 = outParam2;
                        modelResult.OutParam3 = outParam3;
                        modelResult.OutParam4 = outParam4;
                        modelResult.OutParam5 = outParam5;
                        modelResult.OutResult = OutResult;
                        modelResult.OutTTNumber = OutTTNumber;

                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, modelResult, log, "Success", "", "");
                    }
                    else
                    {
                        modelResult.OutResult = "Unsuccess";

                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultnop, log, "Failed", "Unsuccess", "");
                    }

                }


            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                modelResult.OutResult = "Unsuccess";
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultnop, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return modelResult;

        }
    }
}
