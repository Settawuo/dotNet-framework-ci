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
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetListMenuAuthorizeQueryHandler : IQueryHandler<GetListMenuAuthorizeQuery, MenuAuthorizeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<MenuAuthorize> _objServiceSubj;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        public GetListMenuAuthorizeQueryHandler(ILogger logger, IEntityRepository<MenuAuthorize> objServiceSubj
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _uow = uow;
            _intfLog = intfLog;
        }
        public MenuAuthorizeModel Handle(GetListMenuAuthorizeQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var ret_data = new OracleParameter();
            ret_data.OracleDbType = OracleDbType.RefCursor;
            ret_data.Direction = ParameterDirection.Output;

            MenuAuthorizeModel ResultData = new MenuAuthorizeModel();
            List<MenuAuthorize> executeResult = new List<MenuAuthorize>();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "PROC_MENU_AUTHORIZE", "GetListMenuAuthorize", "", "", "WEB");
                // 
                executeResult = _objServiceSubj.ExecuteReadStoredProc("WBB.PKG_FBB_EXISTING_FIBRE.PROC_MENU_AUTHORIZE",
                   new
                   {
                       p_partner_type = query.P_PARTNER_TYPE,
                       p_partner_subtype = query.P_PARTNER_SUBTYPE,
                       // out
                       return_code = ret_code,
                       return_message = ret_msg,
                       list_menu_authorize = ret_data
                   }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                decimal ret = Decimal.Parse(ret_code.Value.ToSafeString());
                ResultData.return_code = ret;
                ResultData.return_message = ret_msg.Value.ToSafeString();
                if (executeResult.Count > 0)
                {
                    ResultData.ListMenuAuthorize = executeResult;
                }

            }
            catch (Exception ex)
            {
                ResultData.return_code = -1;
                ResultData.return_message = ex.Message;
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return ResultData;
        }



    }

    public class PackageTopupInternetNotUseQueryHandler : IQueryHandler<PackageTopupInternetNotUseQuery, PackageTopupInternetNotUseModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<PackageTopupInternetNotUse> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        public PackageTopupInternetNotUseQueryHandler(ILogger logger, IEntityRepository<PackageTopupInternetNotUse> objService
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _uow = uow;
            _intfLog = intfLog;
        }
        public PackageTopupInternetNotUseModel Handle(PackageTopupInternetNotUseQuery query)
        {
            InterfaceLogCommand log = null;
            PackageTopupInternetNotUseModel ResultData = new PackageTopupInternetNotUseModel();
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.NonMobileNo, "PackageTopupInternetNotUseQuery", "PackageTopupInternetNotUseQueryHandler", query.NonMobileNo, "FBB|", "WBB");
            try
            {
                var return_code = new OracleParameter();
                return_code.OracleDbType = OracleDbType.Varchar2;
                return_code.Size = 2000;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var ret_data = new OracleParameter();
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                FbbCurrentPromotionObjectModel fbbCurrentPromotionObjectModel = new FbbCurrentPromotionObjectModel();
                fbbCurrentPromotionObjectModel.FBB_CURRENT_PROMOTION_ARRAY = query.ListCurrentPromotion.Select(a => new Fbb_Current_Promotion_ArrayMapping
                {
                    PRODUCT_CD = a.product_cd,
                    PRODUCT_CLASS = a.product_class,
                    START_DATE = a.start_date,
                    END_DATE = a.end_date,
                    PRODUCT_STATUS = a.product_status
                }).ToArray();

                var currentPromotions = OracleCustomTypeUtilities.CreateUDTArrayParameter(" p_REC_CURRENT_PROMOTION", "FBB_CURRENT_PROMOTION_ARRAY", fbbCurrentPromotionObjectModel);

                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_EXISTING_FIBRE.PROC_CHECK_TOPUP_INTERNET",
                   new object[]
                   {
                       currentPromotions,
                       // out
                       return_code,
                       return_message,
                       ret_data
                   });

                ResultData.return_code = executeResult[0] != null ? executeResult[0].ToString() : "-1";
                ResultData.return_message = executeResult[1] != null ? executeResult[1].ToString() : "";

                DataTable dtExecuteResult = (DataTable)executeResult[2];
                List<PackageTopupInternetNotUse> ListPackageTopupInternetNotUse = dtExecuteResult.DataTableToList<PackageTopupInternetNotUse>();

                if (ListPackageTopupInternetNotUse.Count > 0)
                {
                    ResultData.ListPackageTopupInternetNotUse = ListPackageTopupInternetNotUse;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ResultData, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                ResultData.return_code = "-1";
                ResultData.return_message = ex.Message;
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return ResultData;
        }

    }

    #region Mapping air_change_package_array Type Oracle

    public class FbbCurrentPromotionObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public Fbb_Current_Promotion_ArrayMapping[] FBB_CURRENT_PROMOTION_ARRAY { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static FbbCurrentPromotionObjectModel Null
        {
            get
            {
                FbbCurrentPromotionObjectModel obj = new FbbCurrentPromotionObjectModel();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, FBB_CURRENT_PROMOTION_ARRAY);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            FBB_CURRENT_PROMOTION_ARRAY = (Fbb_Current_Promotion_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
        }
    }

    [OracleCustomTypeMappingAttribute("WBB.FBB_CURRENT_PROMOTION_RECORD")]
    public class FbbCurrentPromotionOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new Fbb_Current_Promotion_ArrayMapping();
        }
    }

    [OracleCustomTypeMapping("FBB_CURRENT_PROMOTION_ARRAY")]
    public class FbbCurrentPromotionObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new FbbCurrentPromotionObjectModel();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new Fbb_Current_Promotion_ArrayMapping[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class Fbb_Current_Promotion_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMappingAttribute("PRODUCT_CLASS")]
        public string PRODUCT_CLASS { get; set; }
        [OracleObjectMappingAttribute("START_DATE")]
        public string START_DATE { get; set; }
        [OracleObjectMappingAttribute("END_DATE")]
        public string END_DATE { get; set; }
        [OracleObjectMappingAttribute("PRODUCT_CD")]
        public string PRODUCT_CD { get; set; }
        [OracleObjectMappingAttribute("PRODUCT_STATUS")]
        public string PRODUCT_STATUS { get; set; }

        #endregion Attribute Mapping

        public static Fbb_Current_Promotion_ArrayMapping Null
        {
            get
            {
                Fbb_Current_Promotion_ArrayMapping obj = new Fbb_Current_Promotion_ArrayMapping();
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
            OracleUdt.SetValue(con, udt, "PRODUCT_CLASS", PRODUCT_CLASS);
            OracleUdt.SetValue(con, udt, "START_DATE", START_DATE);
            OracleUdt.SetValue(con, udt, "END_DATE", END_DATE);
            OracleUdt.SetValue(con, udt, "PRODUCT_CD", PRODUCT_CD);
            OracleUdt.SetValue(con, udt, "PRODUCT_STATUS", PRODUCT_STATUS);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Mapping  fbb_event_sub_array Type Oracle
}
