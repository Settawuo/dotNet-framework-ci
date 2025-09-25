using System.Globalization;
using System.Threading;

namespace WBBWeb.Models
{
    public class SiteSession
    {
        public static int CurrentUICulture
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.Name == "th-TH")
                    return 1;
                else if (Thread.CurrentThread.CurrentUICulture.Name == "en-US")
                    return 2;
                else
                    return 0;
            }
            set
            {
                if (value == 1)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", "th", "TH"));
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", "th", "TH"));
                }
                else if (value == 2)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", "en", "US"));
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", "en", "US"));
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                }

                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }

        public static int LatestUICulture { get; set; }
    }
}