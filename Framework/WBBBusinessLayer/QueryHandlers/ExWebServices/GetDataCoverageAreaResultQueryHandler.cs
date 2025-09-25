using LinqKit;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;
using Newtonsoft.Json;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetDataCoverageAreaResultQueryHandler : IQueryHandler<GetDataCoverageAreaResultQuery, DataCoverageAreaResultModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _coverageResult;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;

        public GetDataCoverageAreaResultQueryHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> coverageResult,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_ZIPCODE> zipcode)
        {
            _logger = logger;
            _uow = uow;
            _coverageResult = coverageResult;
            _objService = objService;
            _intfLog = intfLog;
            _zipcode = zipcode;
        }
        public DataCoverageAreaResultModel Handle(GetDataCoverageAreaResultQuery query)
        {
            var response = new DataCoverageAreaResultModel();
            try
            {
                response = (from c in _coverageResult.Get()
                            join d in _zipcode.Get() on c.ZIPCODE_ROWID equals d.ZIPCODE_ROWID
                            where c.RESULTID == query.RESULTID
                              select new DataCoverageAreaResultModel()
                              {
                                  ADDRRESS_TYPE = c.ADDRRESS_TYPE,
                                  BUILDING_NAME = c.BUILDING_NAME,
                                  BUILDING_NO = c.BUILDING_NO,
                                  FLOOR_NO = c.FLOOR_NO,
                                  ADDRESS_NO = c.ADDRESS_NO,
                                  MOO = c.MOO,
                                  SOI = c.SOI,
                                  ROAD = c.ROAD,
                                  POSTAL_CODE = c.POSTAL_CODE,
                                  TUMBON = d.TUMBON,
                                  AMPHUR = d.AMPHUR,
                                  PROVINCE = d.PROVINCE,
                                  LANGUAGE = c.LANGUAGE,
                                  CONTACTNUMBER = c.CONTACTNUMBER,
                                  COVERAGE_AREA = c.COVERAGE_AREA,
                                  COVERAGE_STATUS = c.COVERAGE_STATUS,
                                  COVERAGE_SUBSTATUS = c.COVERAGE_SUBSTATUS,
                                  COVERAGE_CONTACTEMAIL = c.COVERAGE_CONTACTEMAIL,
                                  COVERAGE_CONTACTTEL = c.COVERAGE_CONTACTTEL,
                                  COVERAGE_GROUPOWNER = c.COVERAGE_GROUPOWNER,
                                  COVERAGE_CONTACTNAME = c.COVERAGE_CONTACTNAME,
                                  COVERAGE_NETWORKPROVIDER = c.COVERAGE_NETWORKPROVIDER,
                                  COVERAGE_FTTHDISPLAYMESSAGE = c.COVERAGE_FTTHDISPLAYMESSAGE,
                                  COVERAGE_WTTXDISPLAYMESSAGE = c.COVERAGE_WTTXDISPLAYMESSAGE

                              }).FirstOrDefault();                                
            }
            catch (Exception ex)
            {
                _logger.Info("Error occured when handle GetCoverageResultCommandHandler");
                _logger.Info(ex.GetErrorMessage() + "Inner: " + ex.InnerException);
                _logger.Info(ex.StackTrace);
                 response.RETURN_CODE = -1;
                 response.RETURN_MESSAGE = ex.Message;
            }
            return response;           
        }
    }
}
