using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    class FBBResetSequenceQueryHandler : IQueryHandler<FBBResetSequenceQuery, FBBResetSequenceModel>
    {
        private readonly ILogger logger;
        private readonly IAirNetEntityRepository<String> objSubJ;//AIRWorkflow

        public FBBResetSequenceQueryHandler(ILogger logger, IAirNetEntityRepository<String> objSubJ)
        {

            this.logger = logger;
            this.objSubJ = objSubJ;

        }

        public FBBResetSequenceModel Handle(FBBResetSequenceQuery query)
        {
            FBBResetSequenceModel res = new FBBResetSequenceModel();
            try
            {
                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.Size = 2000;
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                logger.Info("START CALL PACKAGE AIR_RESET_TRANSACTION_ID_SEQ.");
                objSubJ.ExecuteStoredProc("AIR_ADMIN.PKG_RESET_SEQ.AIR_RESET_TRANSACTION_ID_SEQ", new
                {
                    RETURN_CODE,
                    RETURN_MESSAGE
                });

                res.ReturnCode = RETURN_CODE.Value.ToString();
                res.ReturnMessage = RETURN_MESSAGE.Value.ToString();
            }
            catch (Exception err)
            {
                logger.Error(err.Message);
                res.ReturnCode = "-1";
                res.ReturnMessage = err.Message;
            }
            logger.Info("END CALL PACKAGE AIR_RESET_TRANSACTION_ID_SEQ.");
            return res;
        }

    }
}
