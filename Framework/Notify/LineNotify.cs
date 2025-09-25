using RestSharp;
using System;
using System.Configuration;

namespace Notify
{
    public class LineNotify
    {
        public static void SendMessage(string token, string medthodName, Exception ex)
        {
            string message = "Error from: " + medthodName + System.Environment.NewLine + "Message: " + ex.Message + System.Environment.NewLine + "StackTrace: " + ex.StackTrace;
            var messageData = new Message();
            messageData.message = message;
            messageData.notificationDisabled = false;

            SendNotifyLine(token, messageData);
        }

        public static void SendMessage(string token, string medthodName, string infoMessage)
        {
            string message = "Info from: " + medthodName + System.Environment.NewLine + "Message: " + infoMessage;
            var messageData = new Message();
            messageData.message = message;
            messageData.notificationDisabled = true;

            SendNotifyLine(token, messageData);
        }

        private static void SendNotifyLine(string token, Message message)
        {
            string LineNotifyEnvironment = ConfigurationManager.AppSettings["LineNotify"];
            if (LineNotifyEnvironment == "PRD")
            {
                RestClient client = new RestClient("https://notify-api.line.me/api/notify");
                RestRequest request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", "Bearer " + token);

                request.AddParameter("application/x-www-form-urlencoded", "message=" + message.message + "&notificationDisabled=" + message.notificationDisabled, ParameterType.RequestBody);

                var response = client.Execute(request);
            }
        }

        protected class Message
        {
            public string message { get; set; }
            public bool notificationDisabled { get; set; }
        }
    }
}
