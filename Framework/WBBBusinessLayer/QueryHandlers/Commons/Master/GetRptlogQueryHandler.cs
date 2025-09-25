using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetRptLogQueryHandler : IQueryHandler<GetRptLogQuery, List<FBB_RPT_LOG>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_RPT_LOG> _rptLogService;
        private readonly IEntityRepository<FBB_PROGRAM> _PROGRAMService;
        private readonly IEntityRepository<FBB_PROGRAM_PERMISSION> _PROGRAMPERService;
        private readonly IEntityRepository<FBB_GROUP_PERMISSION> _GROUPPERService;
        private readonly IEntityRepository<FBB_USER> _USERService;


        public GetRptLogQueryHandler(ILogger logger,
            IEntityRepository<FBB_RPT_LOG> rptLogService,
            IEntityRepository<FBB_PROGRAM> PROGRAMService,
            IEntityRepository<FBB_PROGRAM_PERMISSION> PROGRAMPERService,
            IEntityRepository<FBB_GROUP_PERMISSION> GROUPPERService,
            IEntityRepository<FBB_USER> USERService)
        {
            _logger = logger;
            _rptLogService = rptLogService;
            _PROGRAMService = PROGRAMService;
            _PROGRAMPERService = PROGRAMPERService;
            _GROUPPERService = GROUPPERService;
            _USERService = USERService;

        }

        public List<FBB_RPT_LOG> Handle(GetRptLogQuery query)
        {
            var reportCode = (from a in _PROGRAMService.Get()
                              join b in _PROGRAMPERService.Get() on a.PROGRAM_ID equals b.PROGRAM_ID
                              join c in _GROUPPERService.Get() on b.GROUP_ID equals c.GROUP_ID
                              join d in _USERService.Get() on c.USER_ID equals d.USER_ID
                              where d.USER_NAME == query.UserName
                              select a.PROGRAM_CODE).ToList();

            //var listLog = ((from rptLog in _rptLogService.Get()
            //                where rptLog.CREATED_BY == query.UserName
            //                select rptLog
            //         ).Union(from rptLog in _rptLogService.Get()
            //                 where rptLog.CREATED_BY == "SYSTEM"
            //                 && reportCode.Contains(rptLog.REPORT_CODE)
            //                 select rptLog)).ToList();

            var listLog = (from rptLog in _rptLogService.Get()
                           where (rptLog.CREATED_BY == query.UserName || rptLog.CREATED_BY == "SYSTEM")
                           && reportCode.Contains(rptLog.REPORT_CODE)
                           orderby rptLog.CREATED_DATE descending
                           select rptLog).ToList();

            //var lstRptLog = (from rptLog in listLog
            //                 orderby rptLog.CREATED_DATE descending
            //                 select rptLog).ToList();


            return listLog;
        }

        private void Union(object p)
        {
            throw new NotImplementedException();
        }

        public string SYSTEM { get; set; }
    }
}
