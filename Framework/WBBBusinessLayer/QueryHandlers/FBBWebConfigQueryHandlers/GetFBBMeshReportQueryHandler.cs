using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFBBMeshReportQueryHandler : IQueryHandler<GetFBBMeshReportQuery, List<FBBMeshReportQueryModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBMeshReportQueryModel> _objService;

        public GetFBBMeshReportQueryHandler(ILogger logger,
            IEntityRepository<FBBMeshReportQueryModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<FBBMeshReportQueryModel> Handle(GetFBBMeshReportQuery query)
        {
            List<FBBMeshReportQueryModel> executeResult = new List<FBBMeshReportQueryModel>();

            try
            {
                _logger.Info("GetFBBMeshReportQueryHandler Start");
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBOR041_REPORT.FBBOR041_REPORT",
                    new
                    {
                        date_to = query.date_to,
                        //out
                        ret_code = ret_code,
                        ret_msg = ret_msg

                    }).ToList();

                //TODO: Send Mail
                //try
                //{
                //    SendMail(executeResult, "");
                //}
                //catch (Exception ex)
                //{
                //    SendMail(executeResult, "Cannot send the e-mail. Error: " + ex.Message);
                //    throw new Exception("Cannot send the e-mail. Error: " + ex.Message);
                //}

                if (ret_code.Value.ToSafeString() == "0") // return 0 pass value to screen 
                {
                    _logger.Info("End WBB.PKG_FBBOR041_REPORT.FBBOR041_REPORT output msg: " + ret_msg);
                    return null;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBOR041_REPORT.FBBOR041_REPORT output msg: " + ret_msg);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBOR041_REPORT.FBBOR041_REPORT" + ex.Message);
                //SendMail(executeResult, "Error call service WBB.PKG_FBBOR041_REPORT.FBBOR041_REPORT" + ex.Message);
                return null;
            }

        }
    }
}
