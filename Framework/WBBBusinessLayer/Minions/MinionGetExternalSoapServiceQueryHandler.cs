using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using WBBContract;
using WBBContract.Minions;
using WBBEntity.Minions;

namespace WBBBusinessLayer.Minions
{
    public class MinionGetExternalSoapServiceQueryHandler : IQueryHandler<MinionGetExternalSoapServiceQuery, MinionGetExternalSoapServiceQueryModel>
    {

        public MinionGetExternalSoapServiceQueryModel Handle(MinionGetExternalSoapServiceQuery query)
        {
            var minionGetExternalSoapServiceQueryModel = new MinionGetExternalSoapServiceQueryModel();
            try
            {
                var request = CreateSoapWebRequest(query.UrlEnpoint, query.SoapHeader, query.ContentType, query.Charset, query.Method);

                if (query.SoapHeader.ToUpper() == "SOAPACTION")
                {
                    var payload = query.RequestData;

                    byte[] byteArray = Encoding.UTF8.GetBytes(payload);
                    request.ContentLength = byteArray.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(byteArray, 0, byteArray.Length);
                    }
                }
                else
                {
                    var soapEnvelopeDocument = new XmlDocument();
                    var decodexml = HttpUtility.HtmlDecode(query.RequestData);
                    soapEnvelopeDocument.LoadXml(decodexml);

                    using (var stream = request.GetRequestStream())
                    {
                        soapEnvelopeDocument.Save(stream);
                    }
                }

                //Geting response from request    
                using (var serviceres = request.GetResponse())
                {

                    using (var rd = new StreamReader(serviceres.GetResponseStream()))
                    {
                        //reading stream    
                        var serviceResult = rd.ReadToEnd();

                        minionGetExternalSoapServiceQueryModel.StatusCode = "0";
                        minionGetExternalSoapServiceQueryModel.StatusMessage = "";
                        minionGetExternalSoapServiceQueryModel.ResponseData = serviceResult;
                    }
                }

            }
            catch (Exception ex)
            {
                minionGetExternalSoapServiceQueryModel.StatusCode = "-1";
                minionGetExternalSoapServiceQueryModel.StatusMessage = ex.Message;
                minionGetExternalSoapServiceQueryModel.ResponseData = "";
            }

            return minionGetExternalSoapServiceQueryModel;
        }

        public HttpWebRequest CreateSoapWebRequest(string uriString, string header, string contentType, string charset, string method)
        {
            //Making Web Request    
            var httpRequest = (HttpWebRequest)WebRequest.Create(uriString);
            //SOAPAction    
            httpRequest.Headers.Add(header, null);
            //Content_type    
            httpRequest.ContentType = string.Format("{0};charset=\"{1}\"", contentType, charset);
            httpRequest.Accept = contentType;
            //HTTP method    
            httpRequest.Method = method;
            //return HttpWebRequest    
            return httpRequest;
        }
    }
}
