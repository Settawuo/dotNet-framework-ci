using Microsoft.AspNet.SignalR;

namespace WBBWeb.Hubs
{
    public class BaseHub : Hub
    {
        public void Send(string orderId, string tranId)
        {
            Clients.All.addNewMessageToPage(orderId, tranId);
        }
    }
}