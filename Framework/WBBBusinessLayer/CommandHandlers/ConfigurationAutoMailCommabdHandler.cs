using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers
{
    public class ConfigurationAutoMailCommabdHandler : ICommandHandler<ConfigurationAutoMailCommand>
    {
        private readonly IEntityRepository<string> _objService;

        public ConfigurationAutoMailCommabdHandler(IEntityRepository<string> objService)
        {
            _objService = objService;
        }

        public void Handle(ConfigurationAutoMailCommand command)
        {
            try
            {
                var return_code = new OracleParameter();
                return_code.OracleDbType = OracleDbType.Decimal;
                return_code.ParameterName = "return_code";
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.ParameterName = "return_message";
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBB_CONFIG_QUERY_ARRAY =
                        command.ConfigurationQueryList.Select(
                            a => new FBB_CONFIG_QUERY_ARRAYMapping
                            {
                                query_id = a.query_id,
                                report_id = command.report_id,
                                sheet_name = a.sheet_name,
                                owner_db = a.owner_db,
                                query_1 = a.query_1,
                                query_2 = a.query_2,
                                query_3 = a.query_3,
                                query_4 = a.query_4,
                                query_5 = a.query_5,
                                type = a.query_type
                            }).ToArray()
                };

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_config_query", "FBB_CONFIG_QUERY_ARRAY", packageMappingObjectModel);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_AUTOMAIL_REPORT.Report",
                out paramOut,
                   new
                   {
                       //in 
                       p_report_id = command.report_id,
                       p_report_name = command.report_name,
                       p_scheduler = command.scheduler,
                       p_day_of_week = command.day_of_week,
                       p_email_to = command.email_to,
                       p_email_from = command.email_from,
                       p_email_cc = command.email_cc,
                       p_email_subject = command.email_subject,
                       p_email_content = command.email_content,
                       p_email_to_admin = command.email_to_admin,
                       p_active_flag = command.active_flag,
                       p_type = command.report_type,
                       p_created_by = command.created_by,
                       p_config_query = packageMapping,
                       p_month_of_year = command.month_of_year,
                       p_day_of_month = command.day_of_month,

                       /// Out
                       return_code = return_code,
                       return_message = return_message

                   });
                command.return_msg = return_message.Value.ToSafeString();
                command.return_code = return_code.Value.ToSafeString() != "null" ? decimal.Parse(return_code.Value.ToSafeString()) : 0;


            }
            catch (Exception ex)
            {
                command.return_code = -1;
                command.return_msg = "Error Save Configuration Auto Mail : " + ex.Message;
            }
        }


        #region Mapping FBB_CONFIG_QUERY_ARRAY Type Oracle

        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_CONFIG_QUERY_ARRAYMapping[] FBB_CONFIG_QUERY_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModel Null
            {
                get
                {
                    var obj = new PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBB_CONFIG_QUERY_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_CONFIG_QUERY_ARRAY = (FBB_CONFIG_QUERY_ARRAYMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBB_CONFIG_QUERY_REPORT_RECORD")]
        public class Air_Package_MGM_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_CONFIG_QUERY_ARRAYMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_CONFIG_QUERY_ARRAY")]
        public class AirPackageMGMMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new FBB_CONFIG_QUERY_ARRAYMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_CONFIG_QUERY_ARRAYMapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("QUERY_ID")]
            public decimal query_id { get; set; }

            [OracleObjectMapping("REPORT_ID")]
            public decimal report_id { get; set; }

            [OracleObjectMapping("SHEET_NAME")]
            public string sheet_name { get; set; }

            [OracleObjectMapping("OWNER_DB")]
            public string owner_db { get; set; }

            [OracleObjectMapping("QUERY_1")]
            public string query_1 { get; set; }

            [OracleObjectMapping("QUERY_2")]
            public string query_2 { get; set; }

            [OracleObjectMapping("QUERY_3")]
            public string query_3 { get; set; }

            [OracleObjectMapping("QUERY_4")]
            public string query_4 { get; set; }

            [OracleObjectMapping("QUERY_5")]
            public string query_5 { get; set; }

            [OracleObjectMapping("TYPE")]
            public string type { get; set; }


            #endregion Attribute Mapping

            public static FBB_CONFIG_QUERY_ARRAYMapping Null
            {
                get
                {
                    var obj = new FBB_CONFIG_QUERY_ARRAYMapping();
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
                OracleUdt.SetValue(con, udt, "QUERY_ID", query_id);
                OracleUdt.SetValue(con, udt, "REPORT_ID", report_id);
                OracleUdt.SetValue(con, udt, "SHEET_NAME", sheet_name);
                OracleUdt.SetValue(con, udt, "OWNER_DB", owner_db);
                OracleUdt.SetValue(con, udt, "QUERY_1", query_1);
                OracleUdt.SetValue(con, udt, "QUERY_2", query_2);
                OracleUdt.SetValue(con, udt, "QUERY_3", query_3);
                OracleUdt.SetValue(con, udt, "QUERY_4", query_4);
                OracleUdt.SetValue(con, udt, "QUERY_5", query_5);
                OracleUdt.SetValue(con, udt, "TYPE", type);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_CONFIG_QUERY_ARRAY Type Oracle


    }
}
