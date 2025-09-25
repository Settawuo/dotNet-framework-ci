using log4net.Appender;
using System.IO;
using System.Linq;
using System.Text;
using WBBContract;
using WBBContract.Queries.Commons.Master;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetLogQueryHandler : IQueryHandler<GetLogQuery, string>
    {
        private readonly ILogger _logger;

        public GetLogQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public string Handle(GetLogQuery query)
        {
            var repo = _logger.Logger.Repository;
            var logAppender = repo.GetAppenders().OfType<FileAppender>().Where(t => t.Name == "RollingLogFileAppender").FirstOrDefault();

            string filename = logAppender != null ? logAppender.File : string.Empty;

            if (!string.IsNullOrEmpty(query.Date))
            {
                filename += query.Date;
            }

            if (!string.IsNullOrEmpty(query.LogAppendedNo))
            {
                filename += "." + query.LogAppendedNo;
            }

            var result = new StringBuilder();
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var lot = "";
                    while ((lot = sr.ReadLine()) != null)
                    {
                        result.AppendLine(lot);
                    }
                }
            }
            return result.ToString();
        }
    }
}
