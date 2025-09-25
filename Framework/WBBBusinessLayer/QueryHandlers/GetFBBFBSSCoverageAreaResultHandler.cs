using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers
{
    public class GetFBBFBSSCoverageAreaResultHandler : IQueryHandler<GetFBBFBSSCoverageAreaResultQuery, GetLeaveMsgReferenceNoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _coverageResult;
        private readonly IEntityRepository<FBB_ZIPCODE> _fbbZipcode;
        public GetFBBFBSSCoverageAreaResultHandler(ILogger logger, IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> coverageResult, IEntityRepository<FBB_ZIPCODE> fbbZipcode)
        {
            _logger = logger;
            _coverageResult = coverageResult;
            _fbbZipcode = fbbZipcode;
        }

        public GetLeaveMsgReferenceNoCommand Handle(GetFBBFBSSCoverageAreaResultQuery query)
        {
            GetLeaveMsgReferenceNoCommand ret = new GetLeaveMsgReferenceNoCommand();
            try
            {
                var queryCoverage = (from r in _coverageResult.Get()
                                     join z in _fbbZipcode.Get()
                                     on r.ZIPCODE_ROWID equals z.ZIPCODE_ROWID
                                     select new { R = r, Z = z });
                if (query.RESULTID == null || query.RESULTID == 0)
                {
                    queryCoverage = queryCoverage.Where(o => o.R.TRANSACTION_ID == query.TRANSACTIONID);
                }
                else
                {
                    queryCoverage = queryCoverage.Where(o => o.R.RESULTID == query.RESULTID);
                }
                var res = queryCoverage.FirstOrDefault();
                ret.addressAmphur = res.Z.AMPHUR.ToSafeString();
                ret.addressBuilding = res.R.BUILDING_NO.ToSafeString();
                ret.addressFloor = res.R.FLOOR_NO.ToSafeString();
                ret.addressMoo = res.R.MOO.ToSafeString();
                ret.addressMooban = res.R.BUILDING_NAME.ToSafeString();
                ret.addressNo = res.R.ADDRESS_NO.ToSafeString();
                ret.addressPostCode = res.R.POSTAL_CODE.ToSafeString();
                ret.addressProvince = res.Z.PROVINCE.ToSafeString();
                ret.addressRoad = res.R.ROAD.ToSafeString();
                ret.addressSoi = res.R.SOI.ToSafeString();
                ret.addressTumbol = res.Z.TUMBON.ToSafeString();
                ret.caseID = query.CASE_ID;
                ret.assetNumber = query.ASSET_NUMBER;
                ret.contactMobileNo = res.R.CONTACTNUMBER.ToSafeString();
                ret.customerLastName = res.R.LASTNAME.ToSafeString();
                ret.customerName = res.R.FIRSTNAME.ToSafeString();
                ret.FullUrl = "";
                ret.inService = res.R.COVERAGE.ToSafeString();
                ret.productType = "";
                ret.referenceNo = "";
                ret.referenceNoStatus = "";
                ret.RESULTID = res.R.RESULTID;
                ret.Return_Code = "0";
                ret.Return_Desc = "Success";
            }
            catch (Exception ex)
            {
                ret.Return_Code = "-1";
                ret.Return_Desc = "Query Error";
            }
            return ret;
        }


    }
}
