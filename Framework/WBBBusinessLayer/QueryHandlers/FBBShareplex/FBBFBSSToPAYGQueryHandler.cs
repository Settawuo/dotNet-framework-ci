using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Oracle.DataAccess.Client;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBShareplex;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBBusinessLayer.QueryHandlers.FBBShareplex
{
    public class FBBFBSSToPAYGQueryHandler : IQueryHandler<FBBFBSSToPAYGQuery, FBBFBSSToPAYGModel>
    {
        private readonly IEntityRepository<DataTable> _repositoryDataTable;
        private readonly IFBBShareplexEntityRepository<FBBFBSSToPAYGModel> _repositoryConfigurationFileQueryModel;

        public FBBFBSSToPAYGQueryHandler(IEntityRepository<DataTable> repositoryDataTable,
            IFBBShareplexEntityRepository<FBBFBSSToPAYGModel> repositoryConfigurationFileQueryModel)
        //IFBBShareplexEntityRepository<ConfigurationFileQueryModel> repositoryConfigurationFileQueryModel)
        {
            _repositoryDataTable = repositoryDataTable;
            _repositoryConfigurationFileQueryModel = repositoryConfigurationFileQueryModel;
        }

        public FBBFBSSToPAYGModel Handle(FBBFBSSToPAYGQuery query)
        {
            var returnFBBFBSSToPAYGModel = new FBBFBSSToPAYGModel();

            try
            {
                var stringQuery = query.ReturnCode;
                var configurationFileModel = _repositoryConfigurationFileQueryModel.SqlQuery(stringQuery).ToList();

                var consolidated =
                from c in configurationFileModel
                group c by new
                {
                    c.ReturnCode
                } into gcs
                select new FBBFBSSToPAYGModel()
                {
                    ReturnCode = gcs.ToString(),
                    ReturnMessage = gcs.ToString()
                };
                
            }
            catch (Exception ex)
            {
                returnFBBFBSSToPAYGModel.ReturnCode = "-1";
                returnFBBFBSSToPAYGModel.ReturnMessage = ex.Message;
            }

            return returnFBBFBSSToPAYGModel;
        }
    }
}
