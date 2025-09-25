using FBBAutoCheckCoverageBatch.CompositionRoot;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBAutoCheckCoverageBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            Console.WriteLine("START JOB AUTO_CHECK_COVERAGE_BATCH");
            var job = Bootstrapper.GetInstance<AutoCheckCoverageJob>();
            job.LogMsg("START JOB AUTO_CHECK_COVERAGE_BATCH");
            string Url = "";
            Url = job.GetLovList("CHECK_COVERAGE_MAP_SERVICE");
            var listItemCoverage = job.Execute();
            if (listItemCoverage != null && listItemCoverage.Count > 0)
            {
                job.LogMsg("START Validate Rest API.");
                foreach (var item in listItemCoverage)
                {
                    var val = ValidateDataRestAPI(Url, item).Result;
                    if (val)
                    {
                        //Save Data to Table.
                        InstallLeaveMessageModel message = job.InstallLeaveMessageData(item.RESULT_ID);
                        Console.WriteLine("Install-Item : " + message.ret_message + " Result_Id : " + item.RESULT_ID);
                        job.LogMsg("Install-Item : " + message.ret_message + " Result_Id : " + item.RESULT_ID);
                    }
                    else
                    {
                        Console.WriteLine("Fail Result_Id : " + item.RESULT_ID);
                        job.LogMsg("Fail Result_Id : " + item.RESULT_ID);
                    }
                }
                job.LogMsg("END Validate Rest API.");
            }
            else
            {
                job.LogMsg("Get Item CheckCoverage NULL.");
            }
            job.LogMsg("END JOB AUTO_CHECK_COVERAGE_BATCH");
            Console.WriteLine("END JOB AUTO_CHECK_COVERAGE_BATCH");

        }

        public static async Task<bool> ValidateDataRestAPI(string Url, NoCoverageModel item)
        {
            string dataURL = Url; //"http://staging-mapfibrews.ais.co.th/ESRIWebService/services/dataservice.svc";
            bool dataResult = false;
            try
            {
                var randomNo = new Random();
                string genTransactionId = DateTime.Now.ToString("yyyyMMddHHmmss") + randomNo.Next(999).ToString();

                var q = new RequestParam
                {
                    latitude = item.LATITUDE,
                    longitude = item.LONGITUDE,
                    transactionId = genTransactionId,
                    source = "AIS FIBRE"
                };

                var client = new RestClient(dataURL); //"https://chillchill.ais.co.th:8002"
                // client.Authenticator = new HttpBasicAuthenticator(username, password);
                var request = new RestRequest("CheckCoverageMapService", Method.POST) //"/mpay-unified-qr-code/qr"
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new BatchRestSharpJsonSerializer()

                };
                request.AddBody(q);

                // execute the request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);

                Response res = JsonConvert.DeserializeObject<Response>(response.Content) ?? new Response();

                Response objRes = new Response
                {
                    returnCode = res.returnCode,
                    returnMessage = res.returnMessage,
                    coverage = res.coverage,
                    status = res.status,
                    addressId = res.addressId,
                    inserviceDate = res.inserviceDate,
                    splitterList = res.splitterList
                };

                if (objRes.status.Equals("ON_SERVICE")) // R24.01 Edit Change if (objRes.coverage.Equals("ON_SERVICE"))
                    dataResult = true;//ChangeData.

                //#Rest P'TUM
                //var contents = new FormUrlEncodedContent(new[]
                //    {
                //        new KeyValuePair<string, string>("latitude", item.LATITUDE),
                //        new KeyValuePair<string, string>("longitude", item.LONGITUDE)
                //    });

                //using (var client = new HttpClient())
                //{

                //    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //    HttpResponseMessage response = await client.PostAsync(dataURL, contents);

                //    response.EnsureSuccessStatusCode();

                //    using (HttpContent content = response.Content)
                //    {
                //        string responseBody = await response.Content.ReadAsStringAsync();

                //        //XmlSerializer serializer = new XmlSerializer(typeof(response), new XmlRootAttribute("response"));
                //        //StringReader stringReader = new StringReader(responseBody);
                //        //result = (response)serializer.Deserialize(stringReader);

                //    }

                //}

            }
            catch (Exception ex)
            {
                ex.GetBaseException();
            }
            return dataResult;
        }

    }

    public class RequestParam
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string transactionId { get; set; }
        public string source { get; set; }
    }

    [Serializable()]
    public class Response
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string coverage { get; set; }
        public string status { get; set; }
        public string addressId { get; set; }
        public string inserviceDate { get; set; }
        public List<Splitter> splitterList { get; set; }
    }

    public class Splitter
    {
        public string Name { get; set; }
        public string Distance { get; set; }
        public string DistanceType { get; set; }
    }
}
