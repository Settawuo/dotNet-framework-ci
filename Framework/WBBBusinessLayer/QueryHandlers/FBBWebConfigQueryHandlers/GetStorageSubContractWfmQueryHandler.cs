using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetStorageSubContractWfmQueryHandler : IQueryHandler<GetStorageSubContractWfmQuery, List<LovModel>>
    {
        //GetStorageSubContractWfmQuery

        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> _SubPaygWFG;

        public GetStorageSubContractWfmQueryHandler(ILogger logger, IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> SubPaygWFG)
        {
            _logger = logger;
            _SubPaygWFG = SubPaygWFG;
        }
        public List<LovModel> Handle(GetStorageSubContractWfmQuery query)
        {
            var modelLov = new List<LovModel>();
            try
            {

                if (!string.IsNullOrEmpty(query.p_storage_location))
                {
                    modelLov = (from s in _SubPaygWFG.Get()
                                where (s.STORAGE_LOCATION.Contains(query.p_storage_location))
                                select new LovModel { LOV_NAME = s.STORAGE_LOCATION, LOV_VAL1 = s.STORAGE_LOCATION }).Distinct().ToList();
                }
                else
                {
                    modelLov = (from s in _SubPaygWFG.Get()
                                select new LovModel { LOV_NAME = s.STORAGE_LOCATION, LOV_VAL1 = s.STORAGE_LOCATION }).Distinct().ToList();
                }

                return modelLov;
            }
            catch (Exception ex)
            {
                _logger.Info("Error get stroge" + ex.Message.ToString());
            }

            return modelLov;
        }

    }
}
