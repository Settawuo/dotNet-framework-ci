namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WBBContract;
    using WBBContract.Queries.ExWebServices.FbbCpGw;
    using WBBData.Repository;
    using WBBEntity.Extensions;
    using WBBEntity.Models;
    using WBBEntity.PanelModels.ExWebServiceModels;

    public class GetListBuildingVillageQueryHandler : IQueryHandler<GetListBuildingVillageQuery, List<ListBuildingVillageModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _fbb_coveragearea;
        private readonly IEntityRepository<FBB_COVERAGE_ZIPCODE> _fbb_coverage_zipcode;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _fbb_coverage_region;
        private readonly IEntityRepository<FBB_ZIPCODE> _fbb_zipcode;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _fbb_coveragearea_relation;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _fbb_fbss_listbv;
        public GetListBuildingVillageQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> fbb_coveragearea,
            IEntityRepository<FBB_COVERAGE_ZIPCODE> fbb_coverage_zipcode,
            IEntityRepository<FBB_COVERAGE_REGION> fbb_coverage_region,
            IEntityRepository<FBB_ZIPCODE> fbb_zipcode,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> fbb_coveragearea_relation,
            IEntityRepository<FBB_FBSS_LISTBV> fbb_fbss_listbv)
        {
            _logger = logger;
            _fbb_coveragearea = fbb_coveragearea;
            _fbb_coverage_zipcode = fbb_coverage_zipcode;
            _fbb_coverage_region = fbb_coverage_region;
            _fbb_zipcode = fbb_zipcode;
            _fbb_coveragearea_relation = fbb_coveragearea_relation;
            _fbb_fbss_listbv = fbb_fbss_listbv;
        }

        public List<ListBuildingVillageModel> Handle(GetListBuildingVillageQuery query)
        {
            var bvList = new List<ListBuildingVillageModel>();
            var isThai = query.Language.ToCultureCode().IsThaiCulture();
            var strLanguage = "";
            if (query.Language == "THA")
            {
                strLanguage = "T";
            }
            else if (query.Language == "ENG")
            {
                strLanguage = "E";
            }
            try
            {
                var allAddressId = (from z in _fbb_zipcode.Get()
                                    join l in _fbb_fbss_listbv.Get()
                                    on new
                                    {
                                        tumbon = z.TUMBON,
                                        zipcode = z.ZIPCODE
                                    } equals new
                                    {
                                        tumbon = l.SUB_DISTRICT,
                                        zipcode = l.POSTAL_CODE
                                    }
                                    where
                                      !z.AMPHUR.Contains("(PO.)") &&
                                      !z.AMPHUR.Contains("(ปณ.)") &&
                                      l.ADDRESS_TYPE == query.AddressType &&
                                      l.LANGUAGE == strLanguage &&
                                      l.ACTIVE_FLAG == "Y"
                                    select new ListBuildingVillageModel
                                    {
                                        AddressId = l.ADDRESS_ID,
                                        BuildingName = l.BUILDING_NAME,
                                        BuildingNo = l.BUILDING_NO,
                                        SubDistrict = z.TUMBON,
                                        District = z.AMPHUR,
                                        Province = z.PROVINCE,
                                        Postcode = z.ZIPCODE,
                                        AccessMode = l.ACCESS_MODE,
                                        PartnerName = l.PARTNER,
                                        SiteCode = l.SITE_CODE,
                                        Latitude = l.LATITUDE,
                                        Longtitude = l.LONGTITUDE
                                    });
                if (string.IsNullOrEmpty(query.AddressId))
                {
                    bvList = allAddressId.ToList();
                }
                else
                {
                    bvList = allAddressId.Where(b => b.AddressId == query.AddressId).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                throw ex;
            }

            return bvList;
        }
    }
}