using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GenerateFilenamesForWorkflowQueryHandler : IQueryHandler<GenerateFilenamesForWorkflowQuery, GenerateFilenamesForWorkflowModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<out_result_data> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public GenerateFilenamesForWorkflowQueryHandler(ILogger logger
            , IAirNetEntityRepository<out_result_data> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }


        public GenerateFilenamesForWorkflowModel Handle(GenerateFilenamesForWorkflowQuery query)
        {
            GenerateFilenamesForWorkflowModel result = new GenerateFilenamesForWorkflowModel();
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GenerateFilenamesForWorkflowQuery", "GenerateFilenamesForWorkflowQueryHandler", null, "FBB|", "WEB");
            try
            {
                var in_order_no = new OracleParameter
                {
                    ParameterName = "in_order_no",
                    Size = 2000,
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = ParameterDirection.Input,
                    Value = query.in_order_no
                };

                var in_user_name = new OracleParameter
                {
                    ParameterName = "in_user_name",
                    Size = 2000,
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = ParameterDirection.Input,
                    Value = query.in_user_name
                };

                var filenamesForWorkflowObjectModel = new FilenamesForWorkflowObjectModel();
                filenamesForWorkflowObjectModel.FilenamesForWorkflow = query.in_filenames.Select(a => new FilenamesForWorkflowOracleTypeMapping
                {
                    FILE_NAME = a.FILE_NAME.ToSafeString()
                }).ToArray();

                var in_filenames = OracleCustomTypeUtilities.CreateUDTArrayParameter("in_filenames", "T_AIR_FILENAMES_TYPE", filenamesForWorkflowObjectModel);

                var out_return_code = new OracleParameter
                {
                    ParameterName = "out_return_code",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var out_result = new OracleParameter
                {
                    ParameterName = "out_result",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };


                var executeResult = _objService.ExecuteStoredProcMultipleCursor("AIR_ADMIN.PKG_AIRDOCNOTI.GENERATE_FILENAMES",
                    new object[]
                    {
                        in_order_no,
                        in_filenames,
                        in_user_name,
                        //// return 
                        out_return_code,
                        out_result
                    });

                if (executeResult != null)
                {
                    result.out_return_code = executeResult[0] != null ? executeResult[0].ToString() : "-1";

                    var d_ReturnFileDetailData = (DataTable)executeResult[1];
                    var ReturnFileDetailData = d_ReturnFileDetailData.DataTableToList<out_result_data>();

                    if (result.out_return_code == "0" && ReturnFileDetailData != null && ReturnFileDetailData.Count > 0)
                    {
                        result.out_result = ReturnFileDetailData;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", "No data found.", "");
                    }
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
                    result.out_return_code = "-1";
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                result.out_return_code = "-1";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
            }

            return result;

        }

        #region Mapping FilenamesForWorkflow Type Oracle
        public class FilenamesForWorkflowObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FilenamesForWorkflowOracleTypeMapping[] FilenamesForWorkflow { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static FilenamesForWorkflowObjectModel Null
            {
                get
                {
                    FilenamesForWorkflowObjectModel obj = new FilenamesForWorkflowObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FilenamesForWorkflow);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FilenamesForWorkflow = (FilenamesForWorkflowOracleTypeMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("AIR_ADMIN.R_AIR_FILENAME_TYPE")]
        public class Rec_Cust_SplitterOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new FilenamesForWorkflowOracleTypeMapping();
            }

            #endregion IOracleCustomTypeFactory Members
        }

        [OracleCustomTypeMapping("T_AIR_FILENAMES_TYPE")]
        public class FilenamesForWorkflowObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new FilenamesForWorkflowObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FilenamesForWorkflowObjectModel[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FilenamesForWorkflowOracleTypeMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("FILE_NAME")]
            public string FILE_NAME { get; set; }

            #endregion Attribute Mapping

            public static FilenamesForWorkflowOracleTypeMapping Null
            {
                get
                {
                    FilenamesForWorkflowOracleTypeMapping obj = new FilenamesForWorkflowOracleTypeMapping();
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
                OracleUdt.SetValue(con, udt, "FILE_NAME", FILE_NAME);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion Mapping FilenamesForWorkflow Type Oracle

    }
}
