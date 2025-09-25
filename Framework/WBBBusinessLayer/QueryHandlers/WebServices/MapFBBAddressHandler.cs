using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBContract.QueryModels;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class MapFBBAddressHandler : IQueryHandler<MapFBBAddressQuery, MapFBBAddressModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _obj;

        public MapFBBAddressHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> obj)
        {
            _uow = uow;
            _intfLog = intfLog;
            _obj = obj;
        }

        public MapFBBAddressModel Handle(MapFBBAddressQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new MapFBBAddressModel();
            var logStatus = "Failed";
            var logDesc = "Failed";
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_id, "MapFBBAddress", "MapFBBAddressHandler", "", "FBB|" + query.FullUrl, "WEB");

                var p_province = new OracleParameter();
                p_province.ParameterName = "p_province";
                p_province.OracleDbType = OracleDbType.Varchar2;
                p_province.Direction = ParameterDirection.Input;

                var p_amphur = new OracleParameter();
                p_amphur.ParameterName = "p_amphur";
                p_amphur.OracleDbType = OracleDbType.Varchar2;
                p_amphur.Size = 2000;
                p_amphur.Direction = ParameterDirection.Input;

                var p_tumbon = new OracleParameter();
                p_tumbon.ParameterName = "p_tumbon";
                p_tumbon.OracleDbType = OracleDbType.Varchar2;
                p_tumbon.Size = 2000;
                p_tumbon.Direction = ParameterDirection.Input;

                var p_zipcode = new OracleParameter();
                p_zipcode.ParameterName = "p_zipcode";
                p_zipcode.OracleDbType = OracleDbType.Varchar2;
                p_zipcode.Size = 2000;
                p_zipcode.Direction = ParameterDirection.Input;

                var p_address_id = new OracleParameter();
                p_address_id.ParameterName = "p_address_id";
                p_address_id.OracleDbType = OracleDbType.Varchar2;
                p_address_id.Size = 2000;
                p_address_id.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "p_zipcode";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var executeResult = _obj.ExecuteReadStoredProc("WBB.PKG_FBB3BB_ADDRESS_ID.PROC_FBB3BB_ADDRESS_ID",
                   new
                   {
                       p_province = query.p_province,
                       p_amphur = query.p_amphur,
                       p_tumbon = query.p_tumbon,
                       p_zipcode = query.p_zipcode,
                       // out
                       p_address_id = p_address_id,
                       ret_code = ret_code,
                       ret_msg = ret_msg

                   }).ToList();

                result.ret_code = ret_code != null ? ret_code.Value.ToSafeString() : "-1";
                result.ret_msg = ret_msg != null ? ret_msg.Value.ToSafeString() : "";
                result.p_address_id = p_address_id != null ? p_address_id.Value.ToSafeString() : "";

                if (result.ret_code == "0")
                {
                    logStatus = "Success";
                    logDesc = "";
                }
                else
                {
                    logStatus = "Failed";
                    logDesc = String.Format("ret_code={0}:ret_message={1}",
                        result.ret_code,
                        result.ret_msg);
                }
            }
            catch (Exception ex)
            {
                logStatus = "Failed";
                logDesc = ex.GetErrorMessage();
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, logStatus, logDesc, "");
            }
            return result;
        }
    }


}
