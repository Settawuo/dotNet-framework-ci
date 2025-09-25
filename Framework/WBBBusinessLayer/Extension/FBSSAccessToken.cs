using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WBBBusinessLayer.Extension
{
    public class FBSSAccessToken
    {
        public static string channelFBB = "FBBWEB";

        public class CustomOrderService : FBSSOrderServices.OrderService
        {
            private string _bearerToken;

            public CustomOrderService(string bearerToken)
            {
                _bearerToken = bearerToken;
            }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest request = base.GetWebRequest(uri);
                if (request is HttpWebRequest)
                {
                    request.Headers.Add("Authorization", "Bearer " + _bearerToken);
                }
                return request;
            }
        }

        public class CustomTTComplainSheet : TTComplainSheet.TTComplainSheet
        {
            private string _bearerToken;

            public CustomTTComplainSheet(string bearerToken)
            {
                _bearerToken = bearerToken;
            }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest request = base.GetWebRequest(uri);
                if (request is HttpWebRequest)
                {
                    request.Headers.Add("Authorization", "Bearer " + _bearerToken);
                }
                return request;
            }
        }
    }
}
