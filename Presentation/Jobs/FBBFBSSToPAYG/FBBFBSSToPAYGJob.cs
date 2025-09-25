using System;
using System.Collections.Generic;
using System.Linq;

namespace FBBFBSSToPAYG
{
    using WBBBusinessLayer;
    using WBBContract;
    using WBBEntity.Extensions;
    using WBBContract.Queries.FBBWebConfigQueries;
    using System.Diagnostics;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBContract.Commands;
    using System.Threading;
    using WBBEntity.PanelModels.WebServiceModels;
    using WBBContract.Queries.WebServices;
    using WBBEntity.PanelModels;
    using WBBContract.Queries.FBBShareplex;
    using WBBBusinessLayer.QueryHandlers.FBBShareplex;

    public class FBBFBSSToPAYGJob
    {
        #region Property

        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FBBFBSSToPAYGQuery> _FBBFBSSToPAYGQuery;

        #endregion
        #region Constructor

        public FBBFBSSToPAYGJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<FBBFBSSToPAYGQuery> FBBFBSSToPAYGQuery
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _FBBFBSSToPAYGQuery = FBBFBSSToPAYGQuery;
        }

        #endregion
        public void Execute()
        {
            
        }
       

    }
}
