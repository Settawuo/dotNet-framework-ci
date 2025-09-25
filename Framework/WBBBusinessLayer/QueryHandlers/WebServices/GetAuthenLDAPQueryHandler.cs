using System;
using System.Linq;
using System.Net;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetAuthenLDAPQueryHandler : IQueryHandler<GetAuthenLDAPQuery, bool>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetAuthenLDAPQueryHandler(ILogger logger, 
            IEntityRepository<FBB_CFG_LOV> fbb_CFG_LOV,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _FBB_CFG_LOV = fbb_CFG_LOV;
            _lov = lov;
        }

        public bool Handle(GetAuthenLDAPQuery query)
        {
            try
            {
                var queryInfo = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("WS_GEN_AuthenLDAP"));
                var projectCode = queryInfo.Where(x => x.DISPLAY_VAL == "ProjectCode").Select(x => x.LOV_VAL1).FirstOrDefault();
                var projectLevel = Convert.ToInt32(!string.IsNullOrEmpty(queryInfo.Where(x => x.DISPLAY_VAL == "ProjectLevel").Select(x => x.LOV_VAL1).FirstOrDefault())
                    ? queryInfo.Where(x => x.DISPLAY_VAL == "ProjectLevel").Select(x => x.LOV_VAL1).FirstOrDefault() : "0");

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new AuthenLDAPServices.CorporateService())
                {
                    string tmpUrl = (from r in _FBB_CFG_LOV.Get()
                                     where r.LOV_NAME == "URLAuthenLDAP" && r.ACTIVEFLAG == "Y"
                                     select r.LOV_VAL1).FirstOrDefault().ToSafeString();
                    if (tmpUrl != "")
                    {
                        service.Url = tmpUrl;
                    }

                    if (!string.IsNullOrEmpty(projectCode))
                        query.ProjectCode = projectCode;

                    var authenLDAPResult = service.WS_GEN_AuthenLDAP(query.UserName, query.Password, query.ProjectCode, projectLevel);
                    var responseStatus = authenLDAPResult.SelectSingleNode("RES_MSG/STATUS").InnerText;
                    var responseMessage = authenLDAPResult.SelectSingleNode("RES_MSG/DETAIL").InnerText;
                    _logger.Info("User Authen Status " + responseStatus);
                    _logger.Info("User Authen Message " + responseMessage);
                    //if (responseStatus == ServicesConstants.LDAPWSAuthenReturnStatus.Success)
                    if(responseStatus == "0000")
                        return true;

                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Info("User Authen Message " + ex.Message);
                return false;
            }
        }

    }
}
