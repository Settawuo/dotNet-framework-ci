using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class GetReportPaymentQueryHandler : IQueryHandler<GetReportPaymentQuery, List<ReportPaymentModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReportPaymentModel> _objService;

        public GetReportPaymentQueryHandler(ILogger logger, IEntityRepository<ReportPaymentModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<ReportPaymentModel> Handle(GetReportPaymentQuery query)
        {
            try
            {
                var stringQuery =
                    string.Format(
                        "SELECT * FROM TABLE( PKG_FBB_PAYMENT.Search(iCreatedDateFrom => '{0}',iCreatedDateTo => '{1}',iInternetNo => '{2}', iSortColumn => '{3}', iAscDesc => '{4}', iPageNo => '{5}', iRecordsPerPage => '{6}'))"
                        , (query.DateFrom != null) ? query.DateFrom.Value.Date.ToString("dd/MM/yyyy") : ""
                        , (query.DateTo != null) ? query.DateTo.Value.Date.ToString("dd/MM/yyyy") : ""
                        , query.InternetNo
                        , query.SortColumn
                        , query.SortBy
                        , query.PageNo
                        , query.RecordsPerPage);
                List<ReportPaymentModel> executeResult = _objService.SqlQuery(stringQuery).ToList();

                query.ReturnCode = "0";
                query.ReturnDesc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ReturnCode = "-1";
                query.ReturnDesc = "PKG_FBB_PAYMENT.Search() Error : " + ex.Message;

                return null;
            }

            //try
            //{
            //    //Aware: ทำ Interface เอาไว้ก่อน รอพี่อาร์ทำหน้า Payment เสร็จ มาปรับอีกครั้ง
            //    System.Threading.Thread.Sleep(1000);
            //    var listResult = new List<ReportPaymentModel>();
            //    for (int i = 0; i < 25; i++)
            //    {
            //        var dataIndex = i + 1;
            //        int num = (i > 25) ? 1 : i;
            //        bool caseFail = ((i % 3) == 0);
            //        bool caseChannel = ((i % 5) == 0);
            //        var dataString = Convert.ToString((char)('a' + num)).ToUpper();
            //        var dataPaymentDate = DateTime.Now.AddDays(dataIndex + 5).ToString("dd/MM/yyyy HH:mm:ss");
            //        var dataDueDate = DateTime.Now.AddDays(dataIndex).ToString("dd/MM/yyyy");
            //        var result = new ReportPaymentModel
            //        {
            //            PaymentDate = dataPaymentDate,
            //            DueDate = dataDueDate,
            //            InternetNo = string.Format("{0}{1}", "InternetNo: 000000", dataString),
            //            AmountThb = (15000.4999 + (num*100)).ToString("#,###.00"),
            //            Channel = (caseChannel) ? "Credit" : "mPay",
            //            Status = (caseFail) ? "Failed" : "Completed",
            //            ALL_RECORDS = "1000"
            //        };
            //        listResult.Add(result);
            //    }

            //    command.ReturnCode = "0";
            //    command.ReturnDesc = "Success";

            //    return listResult;
            //}
            //catch (Exception ex)
            //{
            //    _logger.Info(ex.Message);

            //    command.ReturnCode = "-1";
            //    command.ReturnDesc = "Error call save event service " + ex.Message;

            //    return null;
            //}

        }
    }

}
