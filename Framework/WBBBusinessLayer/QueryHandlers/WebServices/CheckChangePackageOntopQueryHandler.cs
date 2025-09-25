using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using System.Net;
using WBBBusinessLayer.SBNNewWebService;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class CheckChangePackageOntopQueryHandler : IQueryHandler<CheckChangePackageOntopQuery, CheckChangePackageOntopModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<CheckChangePackageOntopModel> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public CheckChangePackageOntopQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IAirNetEntityRepository<CheckChangePackageOntopModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public CheckChangePackageOntopModel Handle(CheckChangePackageOntopQuery query)
        {
            InterfaceLogCommand log = null;
            DateTime Curr_DateTime = DateTime.Now;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_non_mobile_no, "CheckChangePackageOntopQuery", "AIR_ADMIN.PKG_AIROR905.CHECK_CHANGE_PACKAGE_ONTOP", null, "FBB", "FBBOR016");
            try
            {

                airChangePackageRecord[] chkOldChangePackage = query.AirChangeOldPackageArray.Select(a => new airChangePackageRecord
                {
                    sffPromotionCode = a.sff_promotion_code,
                    startdt = a.startdt,
                    enddt = a.enddt,
                    productSeq = a.product_seq
                }).ToArray();

                airChangePackageRecord[] chkNewChangePackage = query.AirChangeNewPackageArray.Select(a => new airChangePackageRecord
                {
                    sffPromotionCode = a.sff_promotion_code,
                    startdt = a.startdt,
                    enddt = a.enddt,
                    productSeq = a.product_seq
                }).ToArray();

                checkChangePackageOntopResults objResp = new checkChangePackageOntopResults();

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var service = new SBNNewWebService.SBNWebServiceService())
                {
                    service.Timeout = 600000;
                    objResp = service.checkChangePackageOntop(query.p_non_mobile_no, chkOldChangePackage, chkNewChangePackage);
                }
                CheckChangePackageOntopModel result = new CheckChangePackageOntopModel();

                if (objResp != null)
                {
                    result.o_result = objResp.result;
                    result.o_errorReason = objResp.errorReason;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, objResp, log, "Success", "", "");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("ex message" + ex.Message + " error inner" + ex.InnerException);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
                return null;
            }


        }


        #region Mapping air_change_package_array Type Oracle

        public class AirChagePackageObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Air_Change_Package_ArrayMapping[] AIR_CHANGE_PACKAGE_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static AirChagePackageObjectModel Null
            {
                get
                {
                    AirChagePackageObjectModel obj = new AirChagePackageObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, AIR_CHANGE_PACKAGE_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                AIR_CHANGE_PACKAGE_ARRAY = (Air_Change_Package_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_CHANGE_PACKAGE_RECORD")]
        public class AirChagePackageOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Air_Change_Package_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("AIR_CHANGE_PACKAGE_ARRAY")]
        public class AirChagePackageObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new AirChagePackageObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Air_Change_Package_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Air_Change_Package_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SFF_PROMOTION_CODE")]
            public string SFF_PROMOTION_CODE { get; set; }
            [OracleObjectMappingAttribute("STARTDT")]
            public string startDt { get; set; }
            [OracleObjectMappingAttribute("ENDDT")]
            public string endDt { get; set; }
            [OracleObjectMappingAttribute("PRODUCT_SEQ")]
            public string PRODUCT_SEQ { get; set; }

            #endregion Attribute Mapping

            public static Air_Change_Package_ArrayMapping Null
            {
                get
                {
                    Air_Change_Package_ArrayMapping obj = new Air_Change_Package_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "SFF_PROMOTION_CODE", SFF_PROMOTION_CODE);
                OracleUdt.SetValue(con, udt, "STARTDT", startDt);
                OracleUdt.SetValue(con, udt, "ENDDT", endDt);
                OracleUdt.SetValue(con, udt, "PRODUCT_SEQ", PRODUCT_SEQ);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  fbb_event_sub_array Type Oracle


        public IAirNetEntityRepository<CheckChangePackageOntopModel> _checkChangePackageOntopModel { get; set; }
    }
}
