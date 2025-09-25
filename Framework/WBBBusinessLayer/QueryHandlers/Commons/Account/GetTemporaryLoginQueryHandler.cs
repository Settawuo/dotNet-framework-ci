using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Account;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Account
{
    public class GetTemporaryLoginQueryHandler : IQueryHandler<GetTemporaryLoginQuery, List<object>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        protected Stopwatch timer;

        public GetTemporaryLoginQueryHandler(ILogger logger, IEntityRepository<FBB_CFG_LOV> lovService)
        {
            _logger = logger;
            _lovService = lovService;
        }

        private void StartWatch()
        {
            timer = Stopwatch.StartNew();
        }

        private void StopWatch(string actionName)
        {
            timer.Stop();
            _logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }

        public List<object> Handle(GetTemporaryLoginQuery query)
        {
            StartWatch();

            var configTempLoginEnable = (from c in _lovService.Get()
                                         where c.LOV_NAME == "CONFIG_TEMP_LOGIN_PAGE"
                                         select c).FirstOrDefault();

            var configSSOLoginUrl = (from c in _lovService.Get()
                                     where c.LOV_NAME == "SSO_LogOn_Url"
                                     select c).FirstOrDefault();

            StopWatch("Get Constants");

            if (configTempLoginEnable != null && configTempLoginEnable.LOV_VAL1 == "Y")
            {
                return new List<object> { true, "" };
            }
            else
            {
                if (configSSOLoginUrl != null)
                {
                    return new List<object> { false, configSSOLoginUrl.LOV_VAL1 };
                }
                else
                {
                    //no any config, this case should not be reached
                    return new List<object> { false, "" };
                }
            }

        }


    }
}
