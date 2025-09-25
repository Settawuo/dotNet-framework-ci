using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetVersionQueryHandler : IQueryHandler<GetVersionQuery, VersionModel>
    {
        private readonly ILogger _logger;

        public GetVersionQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public VersionModel Handle(GetVersionQuery query)
        {
            VersionModel version = new VersionModel();
            version.InternalServiceVersion = "3.0"; // 20/01/2015
            version.ExternalServiceVersion = "e1.0";
            version.FBBVersion = "f1.0";
            version.AdminVersion = "a1.0";

            return version;
        }
    }
}
