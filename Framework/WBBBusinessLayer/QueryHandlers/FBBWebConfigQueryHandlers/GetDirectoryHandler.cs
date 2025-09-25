using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetDirectoryHandler : IQueryHandler<GetDirectoryQuery, List<DirectoryList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DirectoryList> _objService;

        public GetDirectoryHandler(ILogger logger, IEntityRepository<DirectoryList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<DirectoryList> Handle(GetDirectoryQuery query)
        {
            try
            {
                return _objService.SqlQuery(@"select * from all_directories").ToList();
            }
            catch (Exception ex)
            {
                _logger.Info("GetDirectoryHandler : Error.");
                _logger.Info(ex.Message);

                return null;
            }

        }

    }
}
