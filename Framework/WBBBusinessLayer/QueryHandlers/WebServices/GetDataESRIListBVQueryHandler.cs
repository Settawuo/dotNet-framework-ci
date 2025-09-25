using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetDataESRIListBVQueryHandler : IQueryHandler<GetDataESRIListBVQuery, CoverageAreaResultModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _listbv;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;


        public GetDataESRIListBVQueryHandler(ILogger logger, IEntityRepository<FBB_FBSS_LISTBV> listbv, IEntityRepository<FBB_ZIPCODE> zipcode)
        {
            _logger = logger;
            _listbv = listbv;
            _zipcode = zipcode;
        }

        public CoverageAreaResultModel Handle(GetDataESRIListBVQuery query)
        {
            //var subDistrictInTargetSiteCode = (from l in _listbv.Get()
            //                                   where l.SITE_CODE == query.sitecode
            //                                   && l.LANGUAGE == query.language
            //                                   && l.ACTIVE_FLAG == "Y"
            //                                   && l.ADDRESS_TYPE == "V"
            //                                   select l);
            var subDistrictInTargetSiteCode = (from l in _listbv.Get()
                                               where l.ADDRESS_ID == query.addressid
                                               && l.LANGUAGE == query.language
                                               && l.ACTIVE_FLAG == "Y"
                                               && l.ADDRESS_TYPE == "V"
                                               select l);

            if (subDistrictInTargetSiteCode.Count() > 1)
            {
                //subDistrictInTargetSiteCode = (from l in _listbv.Get()
                //                               where l.SITE_CODE == query.sitecode
                //                               && l.SUB_DISTRICT == query.sub_district
                //                               && l.POSTAL_CODE == query.postcode
                //                               && l.ACTIVE_FLAG == "Y"
                //                               && l.ADDRESS_TYPE == "V"
                //                               select l);
                subDistrictInTargetSiteCode = (from l in _listbv.Get()
                                               where l.ADDRESS_ID == query.addressid
                                               && l.SUB_DISTRICT == query.sub_district
                                               && l.POSTAL_CODE == query.postcode
                                               && l.LANGUAGE == query.language
                                               && l.ACTIVE_FLAG == "Y"
                                               && l.ADDRESS_TYPE == "V"
                                               select l);

                /////////////////////In Feuture change : In 1 provice don't have same site code/////////////////////////
                //if (query.province == "กรุงเทพมหานคร")
                //    query.province = "กรุงเทพ";
                //var joined = from v in _listbv.Get()
                //             join z in _zipcode.Get()
                //             on new { x1 = v.POSTAL_CODE, x2 = v.SUB_DISTRICT } equals new { x1 = z.ZIPCODE, x2 = z.TUMBON }
                //             where v.SITE_CODE == query.sitecode
                //             && v.ACTIVE_FLAG == "Y"
                //             && v.ADDRESS_TYPE == "V"
                //             && z.PROVINCE.ToUpper() == query.province.ToUpper()
                //             select new { v.PARTNER, v.ACCESS_MODE, v.ADDRESS_ID, v.BUILDING_NAME };
                //return joined
                //    .Select(l =>
                //        new CoverageAreaResultModel
                //        {
                //            PARTNER_NAME = l.PARTNER,
                //            ACCESS_MODE_LIST = l.ACCESS_MODE,
                //            ADDRESS_ID = l.ADDRESS_ID,
                //            BUILDING_NAME = l.BUILDING_NAME
                //        }).FirstOrDefault();
            }

            return subDistrictInTargetSiteCode
                    .Select(l =>
                        new CoverageAreaResultModel
                        {
                            PARTNER_NAME = l.PARTNER,
                            ACCESS_MODE_LIST = l.ACCESS_MODE,
                            ADDRESS_ID = l.ADDRESS_ID,
                            BUILDING_NAME = l.BUILDING_NAME
                        }).FirstOrDefault();
        }
    }
}
