using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetScreenBulkCorpSFFQueryHandler : IQueryHandler<GetScreenBulkCorpSFFQuery, BatchBulkCorpSFFModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DetailBulkCorpSFF> _objServiceSubj;
        private readonly IEntityRepository<object> _fbblovRepository;
        public GetScreenBulkCorpSFFQueryHandler(ILogger logger, IEntityRepository<DetailBulkCorpSFF> objServiceSubj
            , IEntityRepository<object> fbblovRepository)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _fbblovRepository = fbblovRepository;
        }

        public BatchBulkCorpSFFModel Handle(GetScreenBulkCorpSFFQuery query)
        {
            List<DetailBulkCorpSFF> executeResult = new List<DetailBulkCorpSFF>();
            BatchBulkCorpSFFModel executeResults = new BatchBulkCorpSFFModel();

            try
            {
                var p_bulk_number = new OracleParameter();
                p_bulk_number.ParameterName = "p_bulk_number";
                p_bulk_number.OracleDbType = OracleDbType.Varchar2;
                p_bulk_number.Size = 2000;
                p_bulk_number.Direction = ParameterDirection.Input;
                p_bulk_number.Value = query.P_BULK_NUMBER;

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "OUTPUT_return_code";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "OUTPUT_return_message";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var p_call_SFF = new OracleParameter();
                p_call_SFF.ParameterName = "p_call_SFF";
                p_call_SFF.OracleDbType = OracleDbType.RefCursor;
                p_call_SFF.Direction = ParameterDirection.Output;

                var p_list_service_vdsl = new OracleParameter();
                p_list_service_vdsl.ParameterName = "p_list_service_vdsl";
                p_list_service_vdsl.OracleDbType = OracleDbType.RefCursor;
                p_list_service_vdsl.Direction = ParameterDirection.Output;

                var p_list_service_vdsl_router = new OracleParameter();
                p_list_service_vdsl_router.ParameterName = "p_list_service_vdsl_router";
                p_list_service_vdsl_router.OracleDbType = OracleDbType.RefCursor;
                p_list_service_vdsl_router.Direction = ParameterDirection.Output;

                //17.7
                var p_list_service_appoint = new OracleParameter();
                p_list_service_appoint.ParameterName = "p_list_service_appoint";
                p_list_service_appoint.OracleDbType = OracleDbType.RefCursor;
                p_list_service_appoint.Direction = ParameterDirection.Output;
                /////

                var p_sff_promotion_cur = new OracleParameter();
                p_sff_promotion_cur.ParameterName = "p_sff_promotion_cur";
                p_sff_promotion_cur.OracleDbType = OracleDbType.RefCursor;
                p_sff_promotion_cur.Direction = ParameterDirection.Output;

                var p_sff_promotion_ontop_cur = new OracleParameter();
                p_sff_promotion_ontop_cur.ParameterName = "p_sff_promotion_ontop_cur";
                p_sff_promotion_ontop_cur.OracleDbType = OracleDbType.RefCursor;
                p_sff_promotion_ontop_cur.Direction = ParameterDirection.Output;

                var p_list_instance_cur = new OracleParameter();
                p_list_instance_cur.ParameterName = "p_list_instance_cur";
                p_list_instance_cur.OracleDbType = OracleDbType.RefCursor;
                p_list_instance_cur.Direction = ParameterDirection.Output;


                _logger.Info("Start PROC_DETAIL_SFF");



                var result = _fbblovRepository.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_DETAIL_SFF",
                       new object[]
                    {
                        p_bulk_number,
                          //return code
                        p_return_code,
                        p_return_message,
                        p_call_SFF,
                        p_list_service_vdsl,
                        p_list_service_vdsl_router,
                        p_list_service_appoint,
                        p_sff_promotion_cur,
                        p_sff_promotion_ontop_cur,
                        p_list_instance_cur
                    });
                executeResults.OUTPUT_return_code = result[0] != null ? result[0].ToString() : "-1";
                executeResults.OUTPUT_return_message = result[1].ToString();

                DataTable sff = (DataTable)result[2];
                List<DetailBulkCorpSFF> P_CALL_SFF = sff.DataTableToList<DetailBulkCorpSFF>();
                executeResults.P_CALL_SFF = P_CALL_SFF;

                DataTable list_service_vdsl = (DataTable)result[3];
                List<DetailBulkCorpListServiceVdsl> P_LIST_SERVICE_VDSL = list_service_vdsl.DataTableToList<DetailBulkCorpListServiceVdsl>();
                executeResults.P_LIST_SERVICE_VDSL = P_LIST_SERVICE_VDSL;

                DataTable list_service_vdsl_router = (DataTable)result[4];
                List<DetailBulkCorpListServiceVdslRouter> P_LIST_SERVICE_VDSL_ROUTER = list_service_vdsl_router.DataTableToList<DetailBulkCorpListServiceVdslRouter>();
                executeResults.P_LIST_SERVICE_VDSL_ROUTER = P_LIST_SERVICE_VDSL_ROUTER;

                //17.7
                DataTable list_service_appoint = (DataTable)result[5];
                List<DetailBulkCorpListServiceAppoint> P_LIST_SERVICE_APPOINT = list_service_appoint.DataTableToList<DetailBulkCorpListServiceAppoint>();
                executeResults.P_LIST_SERVICE_APPOINT = P_LIST_SERVICE_APPOINT;
                ////

                DataTable sff_promotion_cur = (DataTable)result[6];
                List<DetailBulkCorpSffPromotionCur> P_SFF_PROMOTION_CUR = sff_promotion_cur.DataTableToList<DetailBulkCorpSffPromotionCur>();
                executeResults.P_SFF_PROMOTION_CUR = P_SFF_PROMOTION_CUR;

                DataTable sff_promotion_ontop_cur = (DataTable)result[7];
                List<DetailBulkCorpSffPromotionOntopCur> P_SFF_PROMOTION_ONTOP_CUR = sff_promotion_ontop_cur.DataTableToList<DetailBulkCorpSffPromotionOntopCur>();
                executeResults.P_SFF_PROMOTION_ONTOP_CUR = P_SFF_PROMOTION_ONTOP_CUR;

                DataTable list_instance_cur = (DataTable)result[8];
                List<DetailBulkCorpListInstanceCur> P_LIST_INSTANCE_CUR = list_instance_cur.DataTableToList<DetailBulkCorpListInstanceCur>();
                executeResults.P_LIST_INSTANCE_CUR = P_LIST_INSTANCE_CUR;

                _logger.Info("End PROC_DETAIL_SFF " + p_return_message.Value.ToString());

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PROC_DETAIL_SFF handles : " + ex.Message);

                return null;
            }
            return executeResults;
        }
    }
}
