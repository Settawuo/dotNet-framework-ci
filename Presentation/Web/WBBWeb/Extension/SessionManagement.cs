using System.Web;

namespace WBBWeb.Extension
{
    public class SessionManagement
    {
        public void ClearAllSession()
        {
            HttpContext.Current.Session.Remove(WebConstants.FBBConfigSessionKeys.UserPaymentPromblemReport);
            //HttpContext.Current.Session.Clear();
            //HttpContext.Current.Session.Abandon();
        }

        public void SetNewSessionId()
        {
            //bool isAdd;
            //bool isRedir;

            //var manager = new SessionIDManager();
            //var newId = manager.CreateSessionID(HttpContext.Current);
            //manager.SaveSessionID(HttpContext.Current, newId, out isRedir, out isAdd);
        }

        public static string GetSessionId()
        {
            var sessionId = HttpContext.Current.Session.SessionID;
            return sessionId ?? string.Empty;
        }
    }
}