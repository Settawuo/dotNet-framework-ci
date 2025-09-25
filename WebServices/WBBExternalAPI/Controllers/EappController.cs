using System.Web.Http;
using WBBContract.Queries.ExWebApi;
using WBBExternalAPI.Contacts;
using WBBExternalAPI.Models.Request.EappController;
using WBBExternalAPI.Models.Response.EappController;

namespace WBBExternalAPI.Controllers
{
    public class EappController : ApiController
    {
        [HttpPost]
        public GetListWebsiteConfigurationResponse GetListWebsiteConfiguration([FromBody] GetListWebsiteConfigurationRequest dto)
        {
            var resultBiz = CallBusinessLayer.ExecuteQuery<GetListWebsiteConfigurationResponse>(new GetListWebsiteConfigurationQuery()
            {
                TransactionId = dto.TRANSACTION_ID,
                ColumnName = dto.LOV_NAME,
                LovType = dto.LOV_TYPE,
                LovName = dto.PACKAGE_CODE,
            });

            return resultBiz;
        }

        [HttpGet]
        public bool HealthCheck()
        {
            return true;
        }
    }
}