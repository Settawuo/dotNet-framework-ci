using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SaveCampaignmessageCommandHandler : ICommandHandler<SaveCampaignmessageCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public SaveCampaignmessageCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(SaveCampaignmessageCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_referral_contact_mobile_no, "SaveCampaignmessageCommandHandler", "SaveCampaignmessageCommandHandler", command.p_referral_contact_mobile_no, "FBB", "WEB");

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

                var packageMappingObjectModel = new PackageMappingObjectModel();
                packageMappingObjectModel.FBB_CAMPAIGN_NEIGHBOR_ARRAY = command.p_rec_campaign_netghbor.Select(a => new FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping
                {
                    language = a.language,
                    service_speed = a.service_speed,
                    cust_name = a.cust_name,
                    cust_surname = a.cust_surname,
                    contact_mobile_no = a.contact_mobile_no,
                    is_ais_mobile = a.is_ais_mobile,
                    contact_email = a.contact_email,
                    address_type = a.address_type,
                    building_name = a.building_name,
                    house_no = a.house_no,
                    soi = a.soi,
                    road = a.road,
                    sub_district = a.sub_district,
                    district = a.district,
                    province = a.province,
                    postal_code = a.postal_code,
                    contact_time = a.contact_time,
                }).ToArray();

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_rec_campaign_netghbor", "FBB_CAMPAIGN_NEIGHBOR_ARRAY", packageMappingObjectModel);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR021.PROC_REG_CAMPAIGN_NEIGHBOR",
                out paramOut,
                   new
                   {
                       //in 
                       p_referral_name = command.p_referral_name,
                       p_referral_surname = command.p_referral_surname,
                       p_referral_staff_id = command.p_referral_staff_id,
                       p_referral_internet_no = command.p_referral_internet_no,
                       p_referral_contact_mobile_no = command.p_referral_contact_mobile_no,
                       p_referral_neighbor_no = command.p_referral_neighbor_no,

                       p_location_code = command.p_location_code,
                       p_asc_code = command.p_asc_code,
                       p_emp_id = command.p_emp_id,
                       p_sales_rep = command.p_sales_rep,

                       p_channal = command.p_channal,
                       p_campaign = command.p_campaign,
                       p_url = command.p_url,

                       p_rec_campaign_netghbor = packageMapping,

                       /// Out
                       return_code = return_code,
                       return_message = return_message

                   });
                command.return_msg = return_message.Value.ToSafeString();
                command.return_code = return_code.Value.ToSafeString() != "null" ? decimal.Parse(return_code.Value.ToSafeString()) : 0;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, return_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.return_code = -1;
                command.return_msg = "Error save Campaign service " + ex.Message;
            }
        }


        #region Mapping FBB_CAMPAIGN_NEIGHBOR_ARRAY Type Oracle

        public class PackageMappingObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping[] FBB_CAMPAIGN_NEIGHBOR_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModel Null
            {
                get
                {
                    PackageMappingObjectModel obj = new PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBB_CAMPAIGN_NEIGHBOR_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_CAMPAIGN_NEIGHBOR_ARRAY = (FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_CAMPAIGN_NEIGHBOR_RECORD")]
        public class Air_Package_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_CAMPAIGN_NEIGHBOR_ARRAY")]
        public class AirPackageMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("LANGUAGE")]
            public string language { get; set; }

            [OracleObjectMappingAttribute("SERVICE_SPEED")]
            public string service_speed { get; set; }

            [OracleObjectMappingAttribute("CUST_NAME")]
            public string cust_name { get; set; }

            [OracleObjectMappingAttribute("CUST_SURNAME")]
            public string cust_surname { get; set; }

            [OracleObjectMappingAttribute("CONTACT_MOBILE_NO")]
            public string contact_mobile_no { get; set; }

            [OracleObjectMappingAttribute("IS_AIS_MOBILE")]
            public string is_ais_mobile { get; set; }

            [OracleObjectMappingAttribute("CONTACT_EMAIL")]
            public string contact_email { get; set; }

            [OracleObjectMappingAttribute("ADDRESS_TYPE")]
            public string address_type { get; set; }

            [OracleObjectMappingAttribute("BUILDING_NAME")]
            public string building_name { get; set; }

            [OracleObjectMappingAttribute("HOUSE_NO")]
            public string house_no { get; set; }

            [OracleObjectMappingAttribute("SOI")]
            public string soi { get; set; }

            [OracleObjectMappingAttribute("ROAD")]
            public string road { get; set; }

            [OracleObjectMappingAttribute("SUB_DISTRICT")]
            public string sub_district { get; set; }

            [OracleObjectMappingAttribute("DISTRICT")]
            public string district { get; set; }

            [OracleObjectMappingAttribute("PROVINCE")]
            public string province { get; set; }

            [OracleObjectMappingAttribute("POSTAL_CODE")]
            public string postal_code { get; set; }


            [OracleObjectMappingAttribute("CONTACT_TIME")]
            public string contact_time { get; set; }

            #endregion Attribute Mapping

            public static FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping Null
            {
                get
                {
                    FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping obj = new FBB_CAMPAIGN_NEIGHBOR_ARRAYMapping();
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
                OracleUdt.SetValue(con, udt, "LANGUAGE", language);
                OracleUdt.SetValue(con, udt, "SERVICE_SPEED", service_speed);
                OracleUdt.SetValue(con, udt, "CUST_NAME", cust_name);
                OracleUdt.SetValue(con, udt, "CUST_SURNAME", cust_surname);
                OracleUdt.SetValue(con, udt, "CONTACT_MOBILE_NO", contact_mobile_no);
                OracleUdt.SetValue(con, udt, "IS_AIS_MOBILE", is_ais_mobile);
                OracleUdt.SetValue(con, udt, "CONTACT_EMAIL", contact_email);
                OracleUdt.SetValue(con, udt, "ADDRESS_TYPE", address_type);
                OracleUdt.SetValue(con, udt, "BUILDING_NAME", building_name);
                OracleUdt.SetValue(con, udt, "HOUSE_NO", house_no);
                OracleUdt.SetValue(con, udt, "SOI", soi);
                OracleUdt.SetValue(con, udt, "ROAD", road);
                OracleUdt.SetValue(con, udt, "SUB_DISTRICT", sub_district);
                OracleUdt.SetValue(con, udt, "DISTRICT", district);
                OracleUdt.SetValue(con, udt, "PROVINCE", province);
                OracleUdt.SetValue(con, udt, "POSTAL_CODE", postal_code);
                OracleUdt.SetValue(con, udt, "CONTACT_TIME", contact_time);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_CAMPAIGN_NEIGHBOR_ARRAY Type Oracle
    }
}
