using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{



    public class GetIpAddressVasQueryHanders : IQueryHandler<GetIpAddressVasQuery, string>
    {
        private readonly ILogger _logger;

        private readonly IEntityRepository<FBB_VOIP_IPMASTER_INFO> _FBB_VOIP_IPMASTER_INFO;

        public GetIpAddressVasQueryHanders(ILogger logger,
            IEntityRepository<FBB_VOIP_IPMASTER_INFO> FBB_VOIP_IPMASTER_INFO)
        {
            _logger = logger;
            _FBB_VOIP_IPMASTER_INFO = FBB_VOIP_IPMASTER_INFO;
        }

        public string Handle(GetIpAddressVasQuery query)
        {

            var a = (from r in _FBB_VOIP_IPMASTER_INFO.Get()
                     where r.STATUS == "1" && r.ACTIVE_FLAG == "Y"

                     select r.IP_ADDRESS
                    ).ToList().FirstOrDefault();
                    
                if(a.Any()){
                    return a.ToString();
                }else{
                    return "0";
                }
        }
    }
}
