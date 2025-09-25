using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetCostInstallationTableQueryHandler : IQueryHandler<GetCostInstallationTableQuery, ConfigurationCostInstallationView>
    {
        private readonly ILogger _logger;
        //  private readonly IEntityRepository<CostInstallationtable1> _objService;
        private readonly IEntityRepository<object> _objService;
        public GetCostInstallationTableQueryHandler(ILogger logger, IEntityRepository<object> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public ConfigurationCostInstallationView Handle(GetCostInstallationTableQuery query)
        {
            ConfigCostResponse executeResults = new ConfigCostResponse();
            var returnForm = new ConfigurationCostInstallationView();
            try
            {

                var p_TABLE = new OracleParameter();
                p_TABLE.ParameterName = "p_TABLE";
                p_TABLE.Size = 2000;
                p_TABLE.OracleDbType = OracleDbType.Varchar2;
                p_TABLE.Direction = ParameterDirection.Input;
                p_TABLE.Value = query.TB_NAME;

                var p_RULE = new OracleParameter();
                p_RULE.ParameterName = "p_RULE";
                p_RULE.Size = 2000;
                p_RULE.OracleDbType = OracleDbType.Varchar2;
                p_RULE.Direction = ParameterDirection.Input;
                p_RULE.Value = query.RULE_ID;

                var P_SUBCONT = new OracleParameter();
                P_SUBCONT.ParameterName = "P_SUBCONT";
                P_SUBCONT.Size = 2000;
                P_SUBCONT.OracleDbType = OracleDbType.Varchar2;
                P_SUBCONT.Direction = ParameterDirection.Input;
                P_SUBCONT.Value = query.SUBCONTTYPE;

                var p_SUBCONTRACT_LOCATION = new OracleParameter();
                p_SUBCONTRACT_LOCATION.ParameterName = "P_SUB_LOCATION";
                p_SUBCONTRACT_LOCATION.Size = 2000;
                p_SUBCONTRACT_LOCATION.OracleDbType = OracleDbType.Varchar2;
                p_SUBCONTRACT_LOCATION.Direction = ParameterDirection.Input;
                p_SUBCONTRACT_LOCATION.Value = query.SUBCONTRACT_LOCATION;

                var P_COMPANY_NAME = new OracleParameter();
                P_COMPANY_NAME.ParameterName = "P_COMPANY_NAME";
                P_COMPANY_NAME.Size = 2000;
                P_COMPANY_NAME.OracleDbType = OracleDbType.Varchar2;
                P_COMPANY_NAME.Direction = ParameterDirection.Input;
                P_COMPANY_NAME.Value = query.COMPANY_NAME;

                var P_ORDER_TYPE = new OracleParameter();
                P_ORDER_TYPE.ParameterName = "P_ORDER_TYPE";
                P_ORDER_TYPE.Size = 2000;
                P_ORDER_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_ORDER_TYPE.Direction = ParameterDirection.Input;
                P_ORDER_TYPE.Value = query.ORD_TYPE;

                var P_TECHNOLOGY = new OracleParameter();
                P_TECHNOLOGY.ParameterName = "P_TECHNOLOGY";
                P_TECHNOLOGY.Size = 2000;
                P_TECHNOLOGY.OracleDbType = OracleDbType.Varchar2;
                P_TECHNOLOGY.Direction = ParameterDirection.Input;
                P_TECHNOLOGY.Value = query.TECH_TYPE;




                var p_DATE_FROM = new OracleParameter();
                p_DATE_FROM.ParameterName = "p_DATE_FROM";
                p_DATE_FROM.Size = 2000;
                p_DATE_FROM.OracleDbType = OracleDbType.Varchar2;
                p_DATE_FROM.Direction = ParameterDirection.Input;
                p_DATE_FROM.Value = query.DATE_FROM;

                var p_DATE_TO = new OracleParameter();
                p_DATE_TO.ParameterName = "p_DATE_TO";
                p_DATE_TO.Size = 2000;
                p_DATE_TO.OracleDbType = OracleDbType.Varchar2;
                p_DATE_TO.Direction = ParameterDirection.Input;
                p_DATE_TO.Value = query.DATE_TO;

                var p_EXPDATE_FROM = new OracleParameter();
                p_EXPDATE_FROM.ParameterName = "p_EXPDATE_FROM";
                p_EXPDATE_FROM.Size = 2000;
                p_EXPDATE_FROM.OracleDbType = OracleDbType.Varchar2;
                p_EXPDATE_FROM.Direction = ParameterDirection.Input;
                p_EXPDATE_FROM.Value = query.EXPDATE_FROM;

                var p_EXPDATE_TO = new OracleParameter();
                p_EXPDATE_TO.ParameterName = "p_EXPDATE_TO";
                p_EXPDATE_TO.Size = 2000;
                p_EXPDATE_TO.OracleDbType = OracleDbType.Varchar2;
                p_EXPDATE_TO.Direction = ParameterDirection.Input;
                p_EXPDATE_TO.Value = query.EXPDATE_TO;

                var p_PAGE_INDEX = new OracleParameter();
                p_PAGE_INDEX.ParameterName = "p_PAGE_INDEX";

                p_PAGE_INDEX.OracleDbType = OracleDbType.Decimal;
                p_PAGE_INDEX.Direction = ParameterDirection.Input;
                p_PAGE_INDEX.Value = query.PAGE_INDEX;

                var p_PAGE_SIZE = new OracleParameter();
                p_PAGE_SIZE.ParameterName = "p_PAGE_SIZE";

                p_PAGE_SIZE.OracleDbType = OracleDbType.Decimal;
                p_PAGE_SIZE.Direction = ParameterDirection.Input;
                p_PAGE_SIZE.Value = query.PAGE_SIZE;

                //var ret_code = new OracleParameter();
                //ret_code.ParameterName = "ret_code";
                //ret_code.Size = 2000;
                //ret_code.OracleDbType = OracleDbType.Varchar2;
                //ret_code.Direction = ParameterDirection.Output;

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;



                var curt1 = new OracleParameter();
                curt1.ParameterName = "curt1";
                curt1.OracleDbType = OracleDbType.RefCursor;
                curt1.Direction = ParameterDirection.Output;

                var curt2 = new OracleParameter();
                curt2.ParameterName = "curt2";
                curt2.OracleDbType = OracleDbType.RefCursor;
                curt2.Direction = ParameterDirection.Output;




                var curt3 = new OracleParameter();
                curt3.ParameterName = "curt3";
                curt3.OracleDbType = OracleDbType.RefCursor;
                curt3.Direction = ParameterDirection.Output;

                var curt4 = new OracleParameter();
                curt4.ParameterName = "curt4";
                curt4.OracleDbType = OracleDbType.RefCursor;
                curt4.Direction = ParameterDirection.Output;

                var curt5 = new OracleParameter();
                curt5.ParameterName = "curt5";
                curt5.OracleDbType = OracleDbType.RefCursor;
                curt5.Direction = ParameterDirection.Output;

                var curt6 = new OracleParameter();
                curt6.ParameterName = "curt6";
                curt6.OracleDbType = OracleDbType.RefCursor;
                curt6.Direction = ParameterDirection.Output;

                _logger.Info("StartPKG_FIXED_ASSET_CONFIG_INSTALL");


                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FIXED_ASSET_CONFIG_INSTALL.p_getconfiguration_cost",
                        new object[]
                      {
                        p_TABLE,
                        p_RULE,
                        P_SUBCONT,
                        p_SUBCONTRACT_LOCATION,
                        P_COMPANY_NAME,
                        P_ORDER_TYPE,
                        P_TECHNOLOGY,
                        p_DATE_FROM,
                        p_DATE_TO,
                        p_EXPDATE_FROM,
                        p_EXPDATE_TO,
                        p_PAGE_INDEX,
                        p_PAGE_SIZE,
                        ret_code,
                        curt1,
                        curt2,
                       curt3,
                       curt4,
                        curt5,
                        curt6
                       });

                if (result != null)
                {
                    if (query.TB_NAME == "T1")
                    {
                        DataTable dtTable1Respones = (DataTable)result[1];
                        List<CostInstallationtable1> ListTable1Respones = new List<CostInstallationtable1>();
                        ListTable1Respones = dtTable1Respones.DataTableToList<CostInstallationtable1>();
                        executeResults.curt1 = ListTable1Respones;
                        returnForm.CostInstallationtable1 = executeResults.curt1;
                    }
                    if (query.TB_NAME == "T2")
                    {
                        DataTable dtTable2Respones = (DataTable)result[2];
                        List<CostInstallationtable2> ListTable2Respones = new List<CostInstallationtable2>();
                        ListTable2Respones = dtTable2Respones.DataTableToList<CostInstallationtable2>();
                        executeResults.curt2 = ListTable2Respones;
                        returnForm.CostInstallationtable2 = executeResults.curt2;
                    }
                    if (query.TB_NAME == "T3")
                    {
                        DataTable dtTable3Respones = (DataTable)result[3];
                        List<CostInstallationtable3> ListTable3Respones = new List<CostInstallationtable3>();
                        ListTable3Respones = dtTable3Respones.DataTableToList<CostInstallationtable3>();
                        executeResults.curt3 = ListTable3Respones;
                        returnForm.CostInstallationtable3 = executeResults.curt3;
                    }
                    if (query.TB_NAME == "T4")
                    {
                        DataTable dtTable4Respones = (DataTable)result[4];
                        List<CostInstallationtable4> ListTable4Respones = new List<CostInstallationtable4>();
                        ListTable4Respones = dtTable4Respones.DataTableToList<CostInstallationtable4>();
                        executeResults.curt4 = ListTable4Respones;
                        returnForm.CostInstallationtable4 = executeResults.curt4;
                    }
                    if (query.TB_NAME == "T5")
                    {
                        DataTable dtTable5Respones = (DataTable)result[5];
                        List<CostInstallationtable5> ListTable5Respones = new List<CostInstallationtable5>();
                        ListTable5Respones = dtTable5Respones.DataTableToList<CostInstallationtable5>();
                        executeResults.curt5 = ListTable5Respones;
                        returnForm.CostInstallationtable5 = executeResults.curt5;
                    }
                    if (query.TB_NAME == "T6")
                    {
                        DataTable dtTable6Respones = (DataTable)result[6];
                        List<CostInstallationtable6> ListTable6Respones = new List<CostInstallationtable6>();
                        ListTable6Respones = dtTable6Respones.DataTableToList<CostInstallationtable6>();
                        executeResults.curt6 = ListTable6Respones;
                        returnForm.CostInstallationtable6 = executeResults.curt6;
                    }

                }


                return returnForm;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FIXED_ASSET_CONFIG_INSTALL handles : " + ex.Message);


                return null;
            }
        }


    }



}
