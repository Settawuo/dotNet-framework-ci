using System;
using System.Linq;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.ServiceHelper
{
    public class SkyLovHelper : ISkyLovHelper
    {
        private const string AtnDomainLovName = "ATN_Domain_Url";
        private string AtnDomain = null;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public SkyLovHelper(IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _cfgLov = cfgLov;
        }

        public string GetDomainName(string lovName)
        {
            if (!string.IsNullOrEmpty(AtnDomain)) return AtnDomain;

            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            return conf?.LOV_VAL1;
        }

        public string GetMobilePackageCurrentUrl(string msisdn)
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_MobilePackageCurrent_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/atn/customers/v1/customerSubscription/mobilePackageCurrent/$msisdn.json
            return string.Concat(domain, conf?.LOV_VAL1).Replace("$msisdn", msisdn);
            ////return "https://test-athena.intra.ais:44300/domain/atn/customers/v1/customerSubscription/mobilePackageCurrent/$msisdn.json".Replace("$msisdn", msisdn);
        }

        public string GetServiceProfileUrl(string msisdn)
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_ServiceProfile_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/atn/customers/v1/customerSubscription/serviceProfile/$msisdn.json
            return string.Concat(domain, conf?.LOV_VAL1).Replace("$msisdn", msisdn);
            ////return "https://test-athena.intra.ais:44300/domain/atn/customers/v1/customerSubscription/serviceProfile/$msisdn.json".Replace("$msisdn", msisdn);
        }

        public string GetSubscriptionAccountUrl(string msisdn, string status, string filter)
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_SubscriptionAccount_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/customer-subscription/v1/subscription-account/msisdn/$msisdn.json?status=$status&filter=$filter
            return string.Concat(domain, conf?.LOV_VAL1).Replace("$msisdn", msisdn);
            ////return "https://test-athena.intra.ais:44300/domain/customers-subscription/v1/subscription-account/msisdn/$msisdn.json".Replace("$msisdn", msisdn);
        }

        public string GetSubScriptionProfileUrl(string keyName, string keyValue)
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_SubScriptionProfile_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/atn/customers/v1/customerSubscription/subScriptionProfile/$key_name/$key_value.json?status=$status&filter=$filter
            return string.Concat(domain, conf?.LOV_VAL1).Replace("$key_name", keyName).Replace("$key_value", keyValue);
            ////return "https://test-athena.intra.ais:44300/domain/atn/customers/v1/customerSubscription/subScriptionProfile/$key_name/$key_value.json".Replace("$key_name", keyName).Replace("$key_value", keyValue);
        }

        public string GetCustomerRiskWatchlistUrl(string keyName, string keyValue)
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_CustomerRiskWatchlist_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/atn/customers/v1/customerRisk/watchlist/$key_name/$key_value.json
            return string.Concat(domain, conf?.LOV_VAL1).Replace("$key_name", keyName).Replace("$key_value", keyValue);
            ////return "https://test-athena.intra.ais:44300/domain/atn/customers/v1/customerRisk/watchlist/$key_name/$key_value.json".Replace("$key_name", keyName).Replace("$key_value", keyValue);
        }

        public string GetQueryContractFbbUrl()
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_QueryContractFbb_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/agreement-query/query-contractfbb.json
            return string.Concat(domain, conf?.LOV_VAL1);
            ////return "https://test-athena.intra.ais:44300/domain/agreement-query/query-contractfbb.json";
        }

        public string GetQueryContractMobileUrl()
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_QueryContractMobile_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/agreement-query/query-contractmobile.json
            return string.Concat(domain, conf?.LOV_VAL1);
            ////return "https://test-athena.intra.ais:44300/domain/agreement-query/query-contractmobile.json";
        }

        public string GetCheckContractDeviceFbbUrl()
        {
            var domain = GetDomainName(AtnDomainLovName);
            var lovName = "ATN_CheckContractDeviceFbb_Url";
            var conf = _cfgLov.Get(x => x.LOV_NAME == lovName && x.ACTIVEFLAG == "Y").FirstOrDefault();
            if (string.IsNullOrEmpty(conf?.LOV_VAL1))
            {
                throw new InvalidOperationException($"Config lov name '{lovName}' should not be null or empty.");
            }
            //domain/agreement-query/check-contractfbb.json
            return string.Concat(domain, conf?.LOV_VAL1);
            ////return "https://test-athena.intra.ais:44300/domain/agreement-query/check-contractfbb.json";
        }
    }

    public interface ISkyLovHelper
    {
        string GetDomainName(string lovName);
        string GetMobilePackageCurrentUrl(string msisdn);
        string GetServiceProfileUrl(string msisdn);
        string GetSubscriptionAccountUrl(string msisdn, string status, string filter);
        string GetSubScriptionProfileUrl(string keyName, string keyValue);
        string GetCustomerRiskWatchlistUrl(string keyName, string keyValue);
        string GetQueryContractFbbUrl();
        string GetQueryContractMobileUrl();
        string GetCheckContractDeviceFbbUrl();
    }
}
