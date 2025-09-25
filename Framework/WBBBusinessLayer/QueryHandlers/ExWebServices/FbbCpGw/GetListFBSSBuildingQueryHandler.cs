using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public class GetListFBSSBuildingQueryHandler : IQueryHandler<GetListFBSSBuildingQuery, List<FBSSBuildingModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _fbbZipcode;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _fbssListBv;

        public GetListFBSSBuildingQueryHandler(ILogger logger,
            IEntityRepository<FBB_ZIPCODE> fbbZipcode,
            IEntityRepository<FBB_FBSS_LISTBV> fbssListBv)
        {
            _logger = logger;
            _fbbZipcode = fbbZipcode;
            _fbssListBv = fbssListBv;
        }

        public List<FBSSBuildingModel> Handle(GetListFBSSBuildingQuery query)
        {
            var bvList = new List<FBSSBuildingModel>();
            List<FBB_FBSS_LISTBV> fbbListBvRow = null;
            try
            {
                if (!string.IsNullOrEmpty(query.SubDistrict))
                {
                    fbbListBvRow = (from t in _fbssListBv.Get()
                                    where t.POSTAL_CODE == query.PostalCode && t.SUB_DISTRICT == query.SubDistrict
                                    select t).ToList();
                }
                else
                {
                    fbbListBvRow = (from t in _fbssListBv.Get()
                                    where t.POSTAL_CODE == query.PostalCode
                                    select t).ToList(); ;
                }

                bvList.AddRange(fbbListBvRow.Select(t => new FBSSBuildingModel
                {
                    AddressId = t.ADDRESS_ID,
                    AddressType = t.ADDRESS_TYPE,
                    BuildingName = t.BUILDING_NAME,
                    BuildingNo = t.BUILDING_NO,
                    Language = t.LANGUAGE,
                }).ToList());
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