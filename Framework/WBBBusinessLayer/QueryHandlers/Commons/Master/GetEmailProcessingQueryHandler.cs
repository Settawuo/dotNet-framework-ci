using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetEmailProcessingQueryHandler : IQueryHandler<GetEmailProcessingQuery, FBB_EMAIL_PROCESSING>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public GetEmailProcessingQueryHandler(ILogger logger, IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService)
        {
            _logger = logger;
            _emailProcService = emailProcService;
        }

        public FBB_EMAIL_PROCESSING Handle(GetEmailProcessingQuery query)
        {
            FBB_EMAIL_PROCESSING emailData;

            if (string.IsNullOrEmpty(query.CreateBy) && !string.IsNullOrEmpty(query.ProcessName))
                emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals(query.ProcessName)).FirstOrDefault();
            else if (string.IsNullOrEmpty(query.CreateBy) && string.IsNullOrEmpty(query.ProcessName))
                emailData = _emailProcService.Get().FirstOrDefault();
            else
                emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals(query.ProcessName) && e.CREATE_BY.Equals(query.CreateBy)).FirstOrDefault();

            return emailData;
        }
    }
}
