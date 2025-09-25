using Quartz;

namespace FBBConfig.Jobs
{
    using WBBBusinessLayer;

    public class WebPing : IJob
    {
        public ILogger _logger;
        public WebPing(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("Pinging The Web : " + context.JobDetail.Key + " executing at " + SystemTime.Now());
        }
    }
}