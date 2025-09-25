using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class SubmitFOAGetErrorDescQueryHandler : IQueryHandler<SubmitFOAGetErrorDescQuery, SubmitFOAGetErrorDesc>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SubmitFOAGetErrorDesc> _submitfoageterrordesc;
        public SubmitFOAGetErrorDescQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAGetErrorDesc> submitfoageterrordesc)
        {
            _logger = logger;
            _submitfoageterrordesc = submitfoageterrordesc;
        }

        public SubmitFOAGetErrorDesc Handle(SubmitFOAGetErrorDescQuery query)
        {
            SubmitFOAGetErrorDesc result = new SubmitFOAGetErrorDesc();
            var v_err_msg = new OracleParameter();
            v_err_msg.ParameterName = "v_err_msg";
            v_err_msg.OracleDbType = OracleDbType.Varchar2;
            v_err_msg.Direction = ParameterDirection.Output;
            v_err_msg.Size = 2000;

            try
            {
                var executeResult = _submitfoageterrordesc.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.F_GET_ERROR_DESC",
                    new
                    {
                        p_material = query.P_MATERIAL,
                        p_serial = query.P_SERIAL,
                        p_location = query.P_LOCATION,
                        p_plant = query.P_PLANT,
                        p_err_msg = query.P_ERR_MSG,

                        //output
                        v_err_msg = v_err_msg,

                    }).ToList();
                result.V_ERR_MSG = v_err_msg.Value != null ? v_err_msg.Value.ToString() : "";
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return null;
            }
        }
    }
}
