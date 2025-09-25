using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetSendMailQueryHandler : IQueryHandler<GetSendMailQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _mailService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public GetSendMailQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService,
            IEntityRepository<string> mailService)
        {
            _logger = logger;
            _lovService = lovService;
            _emailProcService = emailProcService;
            _mailService = mailService;
        }

        public string Handle(GetSendMailQuery query)
        {
            var subject = "";
            if (query.CurrentCulture.IsThaiCulture())
                subject = _lovService.Get(l => l.LOV_TYPE.Equals("EMAIL")).Select(l => l.LOV_VAL1).FirstOrDefault();
            else
                subject = _lovService.Get(l => l.LOV_TYPE.Equals("EMAIL")).Select(l => l.LOV_VAL2).FirstOrDefault();

            var emailData = _emailProcService.Get().FirstOrDefault();

            var executeResult = _mailService.ExecuteStoredProc("FBB_SEND_MAIL",
                  new
                  {
                      p_sender = emailData.SEND_FROM,
                      p_to = query.SendTo,
                      p_cc = emailData.SEND_CC,
                      p_subject = subject,
                      p_content = "",
                      p_attachments = query.FilePath,
                  });

            return "";
        }
    }
}
