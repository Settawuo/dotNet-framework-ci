using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class GetDisplayContractHandler : IQueryHandler<GetDisplayContractQuery, GetDisplayContractModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DisplayContractData> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public GetDisplayContractHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<DisplayContractData> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public GetDisplayContractModel Handle(GetDisplayContractQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_FIBRENET_ID, "GetDisplayContract", "GetDisplayContractHandler", null, "FBB", "");


            FbbDisplayObjectModel fbb_display_array = new FbbDisplayObjectModel();
            fbb_display_array.REC_FBB_DISPLAY = query.FbbDisplayDatas.Select(p => new Fbb_DisplayOracleTypeMapping
            {
                penalty = p.PENALTY.ToSafeString(),
                tdmcontractid = p.TDMCONTRACTID.ToSafeString(),
                contractno = p.CONTRACTNO.ToSafeString(),
                duration = p.DURATION.ToSafeString(),
                contractname = p.CONTRACTNAME.ToSafeString()
            }).ToArray();
            var p_fbb_display_array = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_FBB_DISPLAY_ARRAY", "FBB_DISPLAY_ARRAY", fbb_display_array);


            OracleParameter P_FIBRENET_ID = new OracleParameter();
            P_FIBRENET_ID.ParameterName = "P_FIBRENET_ID";
            P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
            P_FIBRENET_ID.Size = 2000;
            P_FIBRENET_ID.Direction = ParameterDirection.Input;
            P_FIBRENET_ID.Value = query.P_FIBRENET_ID.ToSafeString();

            OracleParameter P_LANGUAGE = new OracleParameter();
            P_LANGUAGE.ParameterName = "P_LANGUAGE";
            P_LANGUAGE.OracleDbType = OracleDbType.Varchar2;
            P_LANGUAGE.Size = 2000;
            P_LANGUAGE.Direction = ParameterDirection.Input;
            P_LANGUAGE.Value = query.P_LANGUAGE.ToSafeString();

            OracleParameter RETURN_CODE = new OracleParameter();
            RETURN_CODE.ParameterName = "RETURN_CODE";
            RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
            RETURN_CODE.Size = 2000;
            RETURN_CODE.Direction = ParameterDirection.Output;

            OracleParameter RETURN_MESSAGE = new OracleParameter();
            RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
            RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
            RETURN_MESSAGE.Size = 2000;
            RETURN_MESSAGE.Direction = ParameterDirection.Output;

            OracleParameter RETURN_DISPLAY_CURROR = new OracleParameter();
            RETURN_DISPLAY_CURROR.ParameterName = "RETURN_DISPLAY_CURROR";
            RETURN_DISPLAY_CURROR.OracleDbType = OracleDbType.RefCursor;
            RETURN_DISPLAY_CURROR.Direction = ParameterDirection.Output;

            GetDisplayContractModel Result = new GetDisplayContractModel();

            try
            {
                List<DisplayContractData> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_QUERY_CONFIG_TIMESLOT.PROC_DISPLAY_CONTRACT",
                     new
                     {
                         P_FIBRENET_ID,
                         P_LANGUAGE,
                         p_fbb_display_array,

                         // out
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_DISPLAY_CURROR
                     }).ToList();

                Result.RETURN_CODE = (RETURN_CODE.Value == null || RETURN_CODE.Value.ToSafeString() == "null") ? "" : RETURN_CODE.Value.ToSafeString();
                Result.RETURN_MESSAGE = (RETURN_MESSAGE.Value == null || RETURN_MESSAGE.Value.ToSafeString() == "null") ? "" : RETURN_MESSAGE.Value.ToSafeString();
                Result.DisplayContractDatas = executeResult;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Result, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                Result.RETURN_CODE = "-1";
                Result.RETURN_MESSAGE = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
            }

            return Result;
        }

    }

    #region Mapping FBB_DISPLAY_ARRAY Type Oracle
    public class FbbDisplayObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Fbb_DisplayOracleTypeMapping[] REC_FBB_DISPLAY { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static FbbDisplayObjectModel Null
        {
            get
            {
                FbbDisplayObjectModel obj = new FbbDisplayObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, REC_FBB_DISPLAY);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            REC_FBB_DISPLAY = (Fbb_DisplayOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("FBB_DP_RECORD")]
    public class Fbb_DisplayOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Fbb_DisplayOracleTypeMapping();
        }

        #endregion
    }
    [OracleCustomTypeMapping("FBB_DISPLAY_ARRAY")]
    public class FbbDisplayObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new FbbDisplayObjectModel();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Fbb_DisplayOracleTypeMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }

    public class Fbb_DisplayOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping
        [OracleObjectMappingAttribute("PENALTY")]
        public string penalty { get; set; }
        [OracleObjectMappingAttribute("TDMCONTRACTID")]
        public string tdmcontractid { get; set; }
        [OracleObjectMappingAttribute("CONTRACTNO")]
        public string contractno { get; set; }
        [OracleObjectMappingAttribute("DURATION")]
        public string duration { get; set; }
        [OracleObjectMappingAttribute("CONTRACTNAME")]
        public string contractname { get; set; }



        #endregion

        public static Fbb_DisplayOracleTypeMapping Null
        {
            get
            {
                Fbb_DisplayOracleTypeMapping obj = new Fbb_DisplayOracleTypeMapping();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "PENALTY", penalty);
            OracleUdt.SetValue(con, udt, "TDMCONTRACTID", tdmcontractid);
            OracleUdt.SetValue(con, udt, "CONTRACTNO", contractno);
            OracleUdt.SetValue(con, udt, "DURATION", duration);
            OracleUdt.SetValue(con, udt, "CONTRACTNAME", contractname);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
