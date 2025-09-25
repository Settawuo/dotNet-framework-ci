using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetQuestionByChannelQueryHandler : IQueryHandler<GetQuestionByChannelQuery, GetQuestionByChannelModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetQuestionByChannelModel> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        public GetQuestionByChannelQueryHandler(ILogger logger, IEntityRepository<GetQuestionByChannelModel> objService
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _uow = uow;
            _intfLog = intfLog;
        }
        public GetQuestionByChannelModel Handle(GetQuestionByChannelQuery query)
        {
            InterfaceLogCommand log = null;
            GetQuestionByChannelModel ResultData = new GetQuestionByChannelModel();
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MobileNo, "GetQuestionByChannelQuery", "GetQuestionByChannelQueryHandler", query.MobileNo, "FBB|", "WBB");
            try
            {
                var p_order_type = new OracleParameter();
                p_order_type.ParameterName = "p_order_type";
                p_order_type.Size = 2000;
                p_order_type.OracleDbType = OracleDbType.Varchar2;
                p_order_type.Direction = ParameterDirection.Input;
                p_order_type.Value = query.p_order_type;

                var p_channel = new OracleParameter();
                p_channel.ParameterName = "p_channel";
                p_channel.Size = 2000;
                p_channel.OracleDbType = OracleDbType.Varchar2;
                p_channel.Direction = ParameterDirection.Input;
                p_channel.Value = query.p_channel;

                var p_technology = new OracleParameter();
                p_technology.ParameterName = "p_technology";
                p_technology.Size = 2000;
                p_technology.OracleDbType = OracleDbType.Varchar2;
                p_technology.Direction = ParameterDirection.Input;
                p_technology.Value = query.p_technology;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.Varchar2;
                return_code.Size = 2000;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.ParameterName = "return_message";
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var list_question = new OracleParameter();
                list_question.ParameterName = "list_question";
                list_question.OracleDbType = OracleDbType.RefCursor;
                list_question.Direction = ParameterDirection.Output;

                var list_answer = new OracleParameter();
                list_answer.ParameterName = "list_answer";
                list_answer.OracleDbType = OracleDbType.RefCursor;
                list_answer.Direction = ParameterDirection.Output;

                var list_sub_answer = new OracleParameter();
                list_sub_answer.ParameterName = "list_sub_answer";
                list_sub_answer.OracleDbType = OracleDbType.RefCursor;
                list_sub_answer.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_CUSTOMER_INSIGHT.LIST_QUESTION_BY_CHANNEL",
                   new object[]
                   {
                       // in
                       p_order_type,
                       p_channel,
                       p_technology,
                       // out
                       return_code,
                       return_message,
                       list_question,
                       list_answer,
                       list_sub_answer
                   });

                ResultData.ret_code = executeResult[0] != null ? executeResult[0].ToString() : "-1";
                ResultData.ret_message = executeResult[1] != null ? executeResult[1].ToString() : "";

                DataTable dtExecuteResult1 = (DataTable)executeResult[2];
                List<QuestionData> ListQuestionData = dtExecuteResult1.DataTableToList<QuestionData>();

                if (ListQuestionData.Count > 0)
                {
                    ResultData.questionDatas = ListQuestionData;
                }

                DataTable dtExecuteResult2 = (DataTable)executeResult[3];
                List<AnswerData> ListAnswerData = dtExecuteResult2.DataTableToList<AnswerData>();

                if (ListAnswerData.Count > 0)
                {
                    ResultData.answerDatas = ListAnswerData;
                }

                DataTable dtExecuteResult3 = (DataTable)executeResult[4];
                List<SubAnswerData> ListSubAnswerData = dtExecuteResult3.DataTableToList<SubAnswerData>();

                if (ListSubAnswerData.Count > 0)
                {
                    ResultData.subAnswerDatas = ListSubAnswerData;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ResultData, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                ResultData.ret_code = "-1";
                ResultData.ret_message = ex.Message;
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return ResultData;
        }

    }
}
