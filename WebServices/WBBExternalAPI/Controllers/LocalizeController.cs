using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using WBBContract.Queries.ExWebApi;
using WBBExternalAPI.Contacts;
using WBBExternalAPI.Models.Request.LocalizeController;
using WBBExternalAPI.Models.Response.LocalizeController;

namespace WBBExternalAPI.Controllers
{
    public class LocalizeController : ApiController
    {
        [HttpPost]
        public RetrieveAddressResponse RetrieveAddress([FromBody] RetrieveAddressRequest query)
        {
            var result = new HttpResponseMessage();
            RetrieveAddressResponse response = new RetrieveAddressResponse();
            try
            {
                string dataHeader = Request.Headers.GetValues("TRANSACTION_ID").First().ToString();
                if (dataHeader == "")
                {
                    response = new RetrieveAddressResponse();
                    response.RESULT_CODE = Convert.ToString((int)ResultMessageEnumLocalize.SystemNotExit);
                    response.RESULT_DESC = ResultMessageEnumLocalize.SystemNotExit.ToString();
                    response.TRANSACTION_ID = null;
                    response.AddressList = null;
                }
                else
                {
                    if ((query.postal_code != "" && query.postal_code != null && (query.address_id == "" || query.address_id == null)) || (query.address_id != "" && query.address_id != null && (query.postal_code == "" || query.postal_code == null)))
                    {
                        #region Contains(,)
                        //if (query.postal_code.Contains(","))
                        //{
                        //    string[] postal_code_list = query.postal_code.Split(',');

                        //    for (int i = 0; i < postal_code_list.Count(); i++)
                        //    {
                        //        Str_postal_code += "'" + postal_code_list[i] + "'" + ",";
                        //    }
                        //    //Str_postal_code = Str_postal_code.Replace("','", ",");
                        //    Str_postal_code = Str_postal_code.Substring(0, Str_postal_code.Length - 1);
                        //}
                        //else
                        //{
                        //    Str_postal_code = query.postal_code;
                        //}
                        #endregion
                        var resultLocalize = CallBusinessLayer.ExecuteQuery<RetrieveAddressResponse>(new RetrieveAddressQuery()
                        {
                            postal_code = query.postal_code == "" || query.postal_code == null ? "null" : query.postal_code,
                            address_id = query.address_id == "" || query.address_id == null ? "null" : query.address_id
                        });
                        response = resultLocalize;
                    }
                    else
                    {
                        response = new RetrieveAddressResponse();
                        response.RESULT_CODE = Convert.ToString((int)ResultMessageEnumLocalize.IncorrectRequest);
                        response.RESULT_DESC = ResultMessageEnumLocalize.IncorrectRequest.ToString();
                        response.AddressList = null;
                    }
                    response.TRANSACTION_ID = dataHeader;
                }

            }
            catch (Exception ex)
            {
                response = new RetrieveAddressResponse();
                response.RESULT_CODE = Convert.ToString((int)ResultMessageEnumLocalize.SystemNotExit);
                response.RESULT_DESC = ResultMessageEnumLocalize.SystemNotExit.ToString();
                response.AddressList = null;
            }
            return response;
        }
        [HttpGet]
        public string CheckApiLocalize()
        {
            return "Success";
        }

    }
}