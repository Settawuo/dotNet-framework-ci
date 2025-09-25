using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SaveMemberGetMemberCommandHandler : ICommandHandler<SaveMemberGetMemberCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommandHandler;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly Object _thisLock = new Object();

        public SaveMemberGetMemberCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            ICommandHandler<SendSmsCommand> sendSmsCommandHandler,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
            _sendSmsCommandHandler = sendSmsCommandHandler;
            _lov = lov;
        }

        public void Handle(SaveMemberGetMemberCommand command)
        {

            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_referral_contact_mobile_no, "SaveMemberGetMemberCommandHandler", "SaveMemberGetMemberCommandHandler", command.p_referral_contact_mobile_no, "FBB", "WEB");

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

                var PackageMGMMappingObjectModel = new SaveMemberGetMemberCommandHandler.PackageMGMMappingObjectModel();
                PackageMGMMappingObjectModel.FBB_CAMPAIGN_MGM_ARRAY = command.p_rec_campaign_mgm.Select(a => new SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping
                {
                    language = a.language,
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
                    line_id = a.lineId,
                    voucher_desc = a.voucher_desc,
                    full_address = a.full_address,
                }).ToArray();

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_rec_campaign_mgm", "FBB_CAMPAIGN_MGM_ARRAY", PackageMGMMappingObjectModel);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR021.PROC_REG_CAMPAIGN_MGM",
                out paramOut,
                   new
                   {
                       //in 
                       p_referral_name = command.p_referral_name,
                       p_referral_surname = command.p_referral_surname,
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
                       p_rec_campaign_mgm = packageMapping,

                       //out
                       return_code = return_code,
                       return_message = return_message

                   });
                command.return_msg = return_message.Value.ToSafeString();
                command.return_code = return_code.Value.ToSafeString() != "null" ? decimal.Parse(return_code.Value.ToSafeString()) : 0;

                if (return_code.Value.ToSafeString() == "0")
                {
                    #region Send SMS

                    lock (_thisLock)
                    {
                        //Friends
                        foreach (var item in command.p_rec_campaign_mgm)
                        {
                            var mobileNo = item.contact_mobile_no;
                            if (!string.IsNullOrEmpty(mobileNo) && mobileNo.Length > 2)
                            {
                                if (mobileNo.Substring(0, 2) != "66")
                                {
                                    if (mobileNo.Substring(0, 1) == "0")
                                    {
                                        mobileNo = "66" + mobileNo.Substring(1);
                                    }
                                }
                            }
                            var lovSms =
                                (from lov in _lov.Get()
                                 where lov.LOV_NAME == "SMS_REGISTER_MEMBER_GET_MEMBER"
                                 select lov).SingleOrDefault() ??
                                new FBB_CFG_LOV();
                            var spSms = item.language == "T"
                                ? lovSms.LOV_VAL1.Replace("{0}", command.p_referral_name).Split(',')
                                : lovSms.LOV_VAL2.Replace("{0}", command.p_referral_name).Split(',');
                            foreach (
                                var sendSmsCommand in
                                    spSms.Select(
                                        sms =>
                                            new SendSmsCommand
                                            {
                                                Destination_Addr = mobileNo,
                                                Source_Addr = "AISFIBRE",
                                                Message_Text = sms,
                                                Transaction_Id = command.p_referral_contact_mobile_no
                                            }))
                            {
                                _sendSmsCommandHandler.Handle(sendSmsCommand);
                                //Thread.Sleep(15000);
                            }
                        }
                    }

                    #endregion
                }

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


        #region Mapping FBB_CAMPAIGN_MGM_ARRAY Type Oracle

        public class PackageMGMMappingObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping[] FBB_CAMPAIGN_MGM_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static SaveMemberGetMemberCommandHandler.PackageMGMMappingObjectModel Null
            {
                get
                {
                    SaveMemberGetMemberCommandHandler.PackageMGMMappingObjectModel obj = new SaveMemberGetMemberCommandHandler.PackageMGMMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBB_CAMPAIGN_MGM_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_CAMPAIGN_MGM_ARRAY = (SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBB_CAMPAIGN_MGM_RECORD")]
        public class Air_Package_MGM_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_CAMPAIGN_MGM_ARRAY")]
        public class AirPackageMGMMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new SaveMemberGetMemberCommandHandler.PackageMGMMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("LANGUAGE")]
            public string language { get; set; }

            [OracleObjectMapping("CUST_NAME")]
            public string cust_name { get; set; }

            [OracleObjectMapping("CUST_SURNAME")]
            public string cust_surname { get; set; }

            [OracleObjectMapping("CONTACT_MOBILE_NO")]
            public string contact_mobile_no { get; set; }

            [OracleObjectMapping("IS_AIS_MOBILE")]
            public string is_ais_mobile { get; set; }

            [OracleObjectMapping("CONTACT_EMAIL")]
            public string contact_email { get; set; }

            [OracleObjectMapping("ADDRESS_TYPE")]
            public string address_type { get; set; }

            [OracleObjectMapping("BUILDING_NAME")]
            public string building_name { get; set; }

            [OracleObjectMapping("HOUSE_NO")]
            public string house_no { get; set; }

            [OracleObjectMapping("SOI")]
            public string soi { get; set; }

            [OracleObjectMapping("ROAD")]
            public string road { get; set; }

            [OracleObjectMapping("SUB_DISTRICT")]
            public string sub_district { get; set; }

            [OracleObjectMapping("DISTRICT")]
            public string district { get; set; }

            [OracleObjectMapping("PROVINCE")]
            public string province { get; set; }

            [OracleObjectMapping("POSTAL_CODE")]
            public string postal_code { get; set; }


            [OracleObjectMapping("CONTACT_TIME")]
            public string contact_time { get; set; }

            [OracleObjectMapping("LINE_ID")]
            public string line_id { get; set; }

            [OracleObjectMapping("VOUCHER_DESC")]
            public string voucher_desc { get; set; }

            [OracleObjectMapping("FULL_ADDRESS")]
            public string full_address { get; set; }

            #endregion Attribute Mapping

            public static SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping Null
            {
                get
                {
                    SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping obj = new SaveMemberGetMemberCommandHandler.FBB_CAMPAIGN_NEIGHBOR_MGM_ARRAYMapping();
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
                OracleUdt.SetValue(con, udt, "LINE_ID", line_id);
                OracleUdt.SetValue(con, udt, "VOUCHER_DESC", voucher_desc);
                OracleUdt.SetValue(con, udt, "FULL_ADDRESS", full_address);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  FBB_CAMPAIGN_MGM_ARRAY Type Oracle
    }
}
