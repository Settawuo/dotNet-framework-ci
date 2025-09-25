using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckQueryReportQueryHandler : IQueryHandler<CheckQueryReportQuery, ConfigurationCheckQueryModel>
    {
        private readonly IEntityRepository<object> _objService;
        private readonly IAirNetEntityRepository<object> _airNetobjService;
        public CheckQueryReportQueryHandler(IEntityRepository<object> objService, IAirNetEntityRepository<object> airNetobjService)
        {
            _objService = objService;
            _airNetobjService = airNetobjService;
        }

        public ConfigurationCheckQueryModel Handle(CheckQueryReportQuery query)
        {
            var checkQuery = new ConfigurationCheckQueryModel();
            try
            {
                if (query.Owner.ToUpper() == "WBB")
                {
                    var list = _objService.SqlQuery(query.Query).ToList();
                }
                else
                {
                    var list = _airNetobjService.SqlQuery(query.Query).ToList();
                }

                checkQuery.Status = "Y";
            }
            catch (Exception ex)
            {
                checkQuery.Status = "N";
                checkQuery.Message = ex.Message;
            }

            return checkQuery;
        }
    }
}
