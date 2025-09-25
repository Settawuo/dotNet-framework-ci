using RestSharp;

namespace WBBBusinessLayer.Extension
{
    public static class RestRequest_Extension
    {
        public static void ReqHeader(this RestRequest req, string pName, string pValue)
        {
            if (!string.IsNullOrEmpty(pValue))
            {
                req.AddHeader(pName, pValue);
            }
        }

        public static void ReqQueryParameter(this RestRequest req, string pName, string pValue)
        {
            if (!string.IsNullOrEmpty(pValue))
            {
                req.AddHeader(pName, pValue);
            }
        }

        // request.AddQueryParameter(nameof(query.filter), query.filter)
        //nameof(query.channel), query.channel
    }
}
