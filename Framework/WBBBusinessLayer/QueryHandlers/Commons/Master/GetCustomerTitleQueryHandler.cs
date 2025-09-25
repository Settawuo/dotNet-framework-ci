using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCustomerTitleQueryHandler : IQueryHandler<GetCustomerTitleQuery, List<CustomerTitleModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_TITLE_MASTER> _titleMasService;

        public GetCustomerTitleQueryHandler(ILogger logger,
            IEntityRepository<FBB_TITLE_MASTER> titleMasService)
        {
            _logger = logger;
            _titleMasService = titleMasService;
        }

        public List<CustomerTitleModel> Handle(GetCustomerTitleQuery query)
        {
            var data = _titleMasService
                .Get(t => (!string.IsNullOrEmpty(t.CUSTOMER_TYPE)
                    && t.CUSTOMER_TYPE.Equals(query.CustomerType)
                    && t.STATUS == Constants.DbConstants.ActiveStatus))
                .OrderBy(t => t.TITLE_DESC);

            IQueryable<FBB_TITLE_MASTER> filteredByLang = null;
            List<CustomerTitleModel> result;
            if (query.CurrentCulture.ToSafeString() == "0") // all thai and english
            {
                filteredByLang = data;

                result = (from f in filteredByLang
                          orderby f.TITLE_CODE
                          select new CustomerTitleModel
                          {
                              Title = f.TITLE_DESC,
                              TitleCode = f.TITLE_CODE,
                              DefaultValue = f.DEFAULT_VALUE_TH
                          }).ToList();
            }
            else if (query.CurrentCulture.IsThaiCulture())
            {
                filteredByLang = data.Where(t => !string.IsNullOrEmpty(t.ENG_FLAG) && t.ENG_FLAG.Equals("N"));
                //result = filteredByLang.Select(t =>
                //new CustomerTitleModel
                //{
                //    Title = t.TITLE_DESC,
                //    TitleCode = t.TITLE_CODE,
                //    DefaultValue = t.DEFAULT_VALUE_TH,
                //}).ToList();

                result = (from f in filteredByLang
                          orderby f.TITLE_CODE
                          select new CustomerTitleModel
                          {
                              Title = f.TITLE_DESC,
                              TitleCode = f.TITLE_CODE,
                              DefaultValue = f.DEFAULT_VALUE_TH
                          }).ToList();
            }
            else
            {
                filteredByLang = data.Where(t => !string.IsNullOrEmpty(t.ENG_FLAG) && t.ENG_FLAG.Equals("Y"));
                //result = filteredByLang.Select(t =>
                //new CustomerTitleModel
                //{
                //    Title = t.TITLE_DESC,
                //    TitleCode = t.TITLE_CODE,
                //    DefaultValue = t.DEFAULT_VALUE_EN,
                //}).ToList();

                result = (from f in filteredByLang
                          orderby f.TITLE_CODE
                          select new CustomerTitleModel
                          {
                              Title = f.TITLE_DESC,
                              TitleCode = f.TITLE_CODE,
                              DefaultValue = f.DEFAULT_VALUE_EN
                          }).ToList();
            }

            return result;

            //using (var service = new SBNServices.SBNWebServiceService())
            //{

            //    var data = service.listTitleNew(query.CustomerType, (query.CurrentCulture.IsThaiCulture() ? "N" : "Y"));

            //    if (data.RETURN_CODE != 0)
            //    {
            //        _logger.Info(data.RETURN_MESSAGE);
            //        return new List<CustomerTitleModel>();
            //    }

            //    var result = data.TitleArrayNew
            //        .Select(t => new CustomerTitleModel
            //        {
            //            Title = t.TITLE_DESC,
            //            TitleCode = t.TITLE_CODE,
            //        }).ToList();

            //    return result;
            //}
        }
    }
}
