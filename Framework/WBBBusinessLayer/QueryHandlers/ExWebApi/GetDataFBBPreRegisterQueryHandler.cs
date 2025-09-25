using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract.Commands;
using WBBContract.Queries.ExWebApi;
using WBBContract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebApiModel;
using System.Globalization;
using System.Threading;

namespace WBBBusinessLayer.QueryHandlers.ExWebApi
{
    public class GetDataFBBPreRegisterQueryHandler : IQueryHandler<GetDataFBBPreRegisterQuery, GetDataFBBPreRegisterQueryModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_PRE_REGISTER> _PREREGISTERService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetDataFBBPreRegisterQueryHandler(IWBBUnitOfWork uow,
           IEntityRepository<FBB_PRE_REGISTER> PREREGISTERService,
           IEntityRepository<FBB_INTERFACE_LOG> intfLog,
           IEntityRepository<FBB_CFG_LOV> lov)
        {
            _uow = uow;
            _PREREGISTERService = PREREGISTERService;
            _intfLog = intfLog;
            _lov = lov;
        }

        public GetDataFBBPreRegisterQueryModel Handle(GetDataFBBPreRegisterQuery query)
        {
            InterfaceLogCommand log = null;
            GetDataFBBPreRegisterQueryModel result = new GetDataFBBPreRegisterQueryModel();
            GetDataFBBPreRegisterQueryLog resultlog = new GetDataFBBPreRegisterQueryLog();
            //var yesterday = DateTime.Now.AddDays(-1).Date;
            // 03/12 add LOV z
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            DateTime timeThreshold = DateTime.MinValue;
            var LOV_yesterday = _lov.Get(x => x.LOV_TYPE.Equals("SCREEN") && x.LOV_NAME.Equals("TIME_BATCH_PRE_REGISTER")).ToList();
            var LOV_yesterday1 = LOV_yesterday.Where(x => !string.IsNullOrEmpty(x.LOV_VAL1)).FirstOrDefault();

            if (LOV_yesterday1 != null && int.TryParse(LOV_yesterday1.LOV_VAL1, out int minutes))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                timeThreshold = DateTime.Now.AddMinutes(-minutes);
            }
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "GetDataFBBPreRegisterQuery", "GetDataFBBPreRegisterQueryHandler", query.TRANSACTION_ID, "FBB", "WEB");

                //var dataYesterday = _PREREGISTERService.Get(o => o.CREATE_DTM >= yesterday).ToList();
                // 03/12 add LOV
                var dataYesterday = _PREREGISTERService.Get(o => o.CREATE_DTM >= timeThreshold).ToList();
                if (dataYesterday != null && dataYesterday.Count() > 0)
                {
                    //return result
                    result.RESULT_CODE = resultlog.RESULT_CODE = "0";
                    result.RESULT_DESC = resultlog.RESULT_DESC = "SUCCESS";
                    result.TRANSACTION_ID = resultlog.TRANSACTION_ID = query.TRANSACTION_ID;
                    result.COUNT = resultlog.COUNT = dataYesterday.Count();
                    result.PRE_REGITER_LIST = dataYesterday;

                    //log out xml
                    resultlog.PRE_REGITER_LIST.AddRange(dataYesterday.Select(item => new FBB_PRE_REGISTER_LOG { PRE_REG_ID = item.PRE_REG_ID }));

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultlog, log, "Success", "", "");
                }
                else
                {
                    result.RESULT_CODE = "0";
                    result.RESULT_DESC = "SUCCESS";
                    result.TRANSACTION_ID = query.TRANSACTION_ID;
                    result.COUNT = dataYesterday.Count();
                    result.PRE_REGITER_LIST = new List<FBB_PRE_REGISTER>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "No data", "");
                }
            }
            catch (Exception ex)
            {
                result.RESULT_CODE = "-1";
                result.RESULT_DESC = "FAILED";
                result.TRANSACTION_ID = query.TRANSACTION_ID;
                result.COUNT = 0;
                result.PRE_REGITER_LIST = new List<FBB_PRE_REGISTER>();
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");

            }
            return result;
        }
    }
}
