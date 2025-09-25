using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.PatchEquipment;
using WBBData.Repository;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class PatchEquipmentQueryHandler : IQueryHandler<PatchEquipmentQuery, List<RetPatchEquipment>>
    {

        private readonly ILogger _logger;
        private readonly IEntityRepository<RetPatchEquipment> _service;

        public PatchEquipmentQueryHandler(ILogger logger, IEntityRepository<RetPatchEquipment> service)
        {
            _logger = logger;
            //   _objService = objService;
            _service = service;

        }
        public List<RetPatchEquipment> Handle(PatchEquipmentQuery query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var cur = new OracleParameter();
                cur.ParameterName = "cur";
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.Size = 2000;
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Direction = ParameterDirection.Output;

                var executeResult = _service.ExecuteReadStoredProc("WBB.PKG_FBB_PAYG_PATCH_SN.p_search_patch_sn",
                 new
                 {
                     p_FILE_NAME = query.FileName,
                     p_SERIAL_NO = query.SerialNo,
                     p_INTERNET_NO = query.InternetNo,
                     p_PATCH_STATUS = query.PatchStatus,
                     p_CREATE_DATE_FROM = query.CreateDateFrom,
                     p_CREATE_DATE_TO = query.CreateDateTo,
                     p_FLAG = query.Flag,
                     // return code
                     //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                     cur,
                     ret_code,
                     ret_msg
                 }).ToList();

                return executeResult;

            }
            catch (Exception ex)
            {
                List<RetPatchEquipment> retPatchEquipment = new List<RetPatchEquipment>();
                return retPatchEquipment;
            }
        }

    }
}