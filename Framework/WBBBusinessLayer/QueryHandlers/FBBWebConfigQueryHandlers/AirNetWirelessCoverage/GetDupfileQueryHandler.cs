using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetDupfileQueryHandler : IQueryHandler<GetDupfileQuery, Dupfile>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_MESSAGE_LOG> _messagelog;


        public GetDupfileQueryHandler(ILogger logger, IEntityRepository<FBB_MESSAGE_LOG> messagelog)
        {
            _logger = logger;
            _messagelog = messagelog;
        }

        public Dupfile Handle(GetDupfileQuery query)
        {
            Dupfile temp = new Dupfile();
            temp.dup = false;

            var result = from a in _messagelog.Get()
                         where a.PROCESS_NAME == "PKG_FBBCFG006" && a.RETURN_DESC == query.file_name && a.RETURN_CODE == 0
                         select a;

            if (result.Any())
            {
                temp.dup = true;
            }

            return temp;

        }
    }
}
