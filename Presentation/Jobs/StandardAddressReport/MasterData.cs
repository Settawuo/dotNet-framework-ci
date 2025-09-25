using System;
using System.Collections.Generic;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace StandardAddressReport
{
    public class MasterData
    {
        private readonly IQueryProcessor _queryProcessor;
        public ILogger _logger;
        private Stopwatch _timer;

        public MasterData(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public List<ZipCodeModel> GetZipCodeList(int currentCulture)
        {
            try
            {
                var query = new GetZipCodeQuery
                {
                    CurrentCulture = currentCulture,
                };

                var zipCodeList = _queryProcessor.Execute(query);

                return zipCodeList;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return new List<ZipCodeModel>();
            }
        }

        public List<ZipCodeModel> GetZipCodeAirList(int currentCulture, string regioncode)
        {
            try
            {
                var query = new GetZipCodeAirQuery
                {
                    CurrentCulture = currentCulture,
                    Regioncode = regioncode
                };

                var zipCodeList = _queryProcessor.Execute(query);

                return zipCodeList;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return new List<ZipCodeModel>();
            }
        }

        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public List<LovModel> GetLovByTypeAndLovVal5(string type, string lovval5)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = type,
                LOV_VAL5 = lovval5
            };
            var data = _queryProcessor.Execute(query);

            return data;
        }

    }
}
