using AIRNETEntity.Extensions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw
{
    public class MicrositeWSCommandHandler : ICommandHandler<MicrositeWSCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public MicrositeWSCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(MicrositeWSCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.P_ORDER_NO, "MicrositeWSCommandHandler", "MicrositeWSCommandHandler", command.P_ORDER_NO, "FBB", "WEB");

                //Input
                var P_ORDER_NO = new OracleParameter();
                P_ORDER_NO.ParameterName = "P_ORDER_NO";
                P_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
                P_ORDER_NO.Size = 1000;
                P_ORDER_NO.Direction = ParameterDirection.Input;
                P_ORDER_NO.Value = command.P_ORDER_NO.ToSafeString();

                var P_USER = new OracleParameter();
                P_USER.ParameterName = "P_USER";
                P_USER.OracleDbType = OracleDbType.Varchar2;
                P_USER.Size = 1000;
                P_USER.Direction = ParameterDirection.Input;
                P_USER.Value = command.P_USER.ToSafeString();

                var P_SEGMENT = new OracleParameter();
                P_SEGMENT.ParameterName = "P_SEGMENT";
                P_SEGMENT.OracleDbType = OracleDbType.Varchar2;
                P_SEGMENT.Size = 1000;
                P_SEGMENT.Direction = ParameterDirection.Input;
                P_SEGMENT.Value = command.P_SEGMENT.ToSafeString();

                var P_NUMBER_OF_USER = new OracleParameter();
                P_NUMBER_OF_USER.ParameterName = "P_NUMBER_OF_USER";
                P_NUMBER_OF_USER.OracleDbType = OracleDbType.Varchar2;
                P_NUMBER_OF_USER.Size = 1000;
                P_NUMBER_OF_USER.Direction = ParameterDirection.Input;
                P_NUMBER_OF_USER.Value = command.P_NUMBER_OF_USER.ToSafeString();

                var P_RESIDENTIAL = new OracleParameter();
                P_RESIDENTIAL.ParameterName = "P_RESIDENTIAL";
                P_RESIDENTIAL.OracleDbType = OracleDbType.Varchar2;
                P_RESIDENTIAL.Size = 1000;
                P_RESIDENTIAL.Direction = ParameterDirection.Input;
                P_RESIDENTIAL.Value = command.P_RESIDENTIAL.ToSafeString();

                var P_TYPE_OF_USER = new OracleParameter();
                P_TYPE_OF_USER.ParameterName = "P_TYPE_OF_USER";
                P_TYPE_OF_USER.OracleDbType = OracleDbType.Varchar2;
                P_TYPE_OF_USER.Size = 2000;
                P_TYPE_OF_USER.Direction = ParameterDirection.Input;
                P_TYPE_OF_USER.Value = command.P_TYPE_OF_USER.ToSafeString();

                var P_PACKAGE_NAME = new OracleParameter();
                P_PACKAGE_NAME.ParameterName = "P_PACKAGE_NAME";
                P_PACKAGE_NAME.OracleDbType = OracleDbType.Varchar2;
                P_PACKAGE_NAME.Size = 2000;
                P_PACKAGE_NAME.Direction = ParameterDirection.Input;
                P_PACKAGE_NAME.Value = command.P_PACKAGE_NAME.ToSafeString();

                var P_PACKAGE_CODE = new OracleParameter();
                P_PACKAGE_CODE.ParameterName = "P_PACKAGE_CODE";
                P_PACKAGE_CODE.OracleDbType = OracleDbType.Varchar2;
                P_PACKAGE_CODE.Size = 1000;
                P_PACKAGE_CODE.Direction = ParameterDirection.Input;
                P_PACKAGE_CODE.Value = command.P_PACKAGE_CODE.ToSafeString();

                var P_SPEED = new OracleParameter();
                P_SPEED.ParameterName = "P_SPEED";
                P_SPEED.OracleDbType = OracleDbType.Varchar2;
                P_SPEED.Size = 1000;
                P_SPEED.Direction = ParameterDirection.Input;
                P_SPEED.Value = command.P_SPEED.ToSafeString();

                var P_PLAYBOX_BUNDLE = new OracleParameter();
                P_PLAYBOX_BUNDLE.ParameterName = "P_PLAYBOX_BUNDLE";
                P_PLAYBOX_BUNDLE.OracleDbType = OracleDbType.Varchar2;
                P_PLAYBOX_BUNDLE.Size = 1000;
                P_PLAYBOX_BUNDLE.Direction = ParameterDirection.Input;
                P_PLAYBOX_BUNDLE.Value = command.P_PLAYBOX_BUNDLE.ToSafeString();

                var P_PLAYBOX_ADDON = new OracleParameter();
                P_PLAYBOX_ADDON.ParameterName = "P_PLAYBOX_ADDON";
                P_PLAYBOX_ADDON.OracleDbType = OracleDbType.Varchar2;
                P_PLAYBOX_ADDON.Size = 1000;
                P_PLAYBOX_ADDON.Direction = ParameterDirection.Input;
                P_PLAYBOX_ADDON.Value = command.P_PLAYBOX_ADDON.ToSafeString();

                var P_ROUTER_BUNDLE = new OracleParameter();
                P_ROUTER_BUNDLE.ParameterName = "P_ROUTER_BUNDLE";
                P_ROUTER_BUNDLE.OracleDbType = OracleDbType.Varchar2;
                P_ROUTER_BUNDLE.Size = 1000;
                P_ROUTER_BUNDLE.Direction = ParameterDirection.Input;
                P_ROUTER_BUNDLE.Value = command.P_ROUTER_BUNDLE.ToSafeString();

                var P_ROUTER_ADDON = new OracleParameter();
                P_ROUTER_ADDON.ParameterName = "P_ROUTER_ADDON";
                P_ROUTER_ADDON.OracleDbType = OracleDbType.Varchar2;
                P_ROUTER_ADDON.Size = 1000;
                P_ROUTER_ADDON.Direction = ParameterDirection.Input;
                P_ROUTER_ADDON.Value = command.P_ROUTER_ADDON.ToSafeString();

                var P_PRICE = new OracleParameter();
                P_PRICE.ParameterName = "P_PRICE";
                P_PRICE.OracleDbType = OracleDbType.Varchar2;
                P_PRICE.Size = 1000;
                P_PRICE.Direction = ParameterDirection.Input;
                P_PRICE.Value = command.P_PRICE.ToSafeString();

                var P_FIRST_NAME = new OracleParameter();
                P_FIRST_NAME.ParameterName = "P_FIRST_NAME";
                P_FIRST_NAME.OracleDbType = OracleDbType.Varchar2;
                P_FIRST_NAME.Size = 2000;
                P_FIRST_NAME.Direction = ParameterDirection.Input;
                P_FIRST_NAME.Value = command.P_FIRST_NAME.ToSafeString();

                var P_LAST_NAME = new OracleParameter();
                P_LAST_NAME.ParameterName = "P_LAST_NAME";
                P_LAST_NAME.OracleDbType = OracleDbType.Varchar2;
                P_LAST_NAME.Size = 2000;
                P_LAST_NAME.Direction = ParameterDirection.Input;
                P_LAST_NAME.Value = command.P_LAST_NAME.ToSafeString();

                var P_TELEPHONE = new OracleParameter();
                P_TELEPHONE.ParameterName = "P_TELEPHONE";
                P_TELEPHONE.OracleDbType = OracleDbType.Varchar2;
                P_TELEPHONE.Size = 1000;
                P_TELEPHONE.Direction = ParameterDirection.Input;
                P_TELEPHONE.Value = command.P_TELEPHONE.ToSafeString();

                var P_EMAIL = new OracleParameter();
                P_EMAIL.ParameterName = "P_EMAIL";
                P_EMAIL.OracleDbType = OracleDbType.Varchar2;
                P_EMAIL.Size = 1000;
                P_EMAIL.Direction = ParameterDirection.Input;
                P_EMAIL.Value = command.P_EMAIL.ToSafeString();

                var P_ADDRESS = new OracleParameter();
                P_ADDRESS.ParameterName = "P_ADDRESS";
                P_ADDRESS.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS.Size = 2000;
                P_ADDRESS.Direction = ParameterDirection.Input;
                P_ADDRESS.Value = command.P_ADDRESS.ToSafeString();

                var P_SUB_DISTRICT = new OracleParameter();
                P_SUB_DISTRICT.ParameterName = "P_SUB_DISTRICT";
                P_SUB_DISTRICT.OracleDbType = OracleDbType.Varchar2;
                P_SUB_DISTRICT.Size = 2000;
                P_SUB_DISTRICT.Direction = ParameterDirection.Input;
                P_SUB_DISTRICT.Value = command.P_SUB_DISTRICT.ToSafeString();

                var P_DISTRICT = new OracleParameter();
                P_DISTRICT.ParameterName = "P_DISTRICT";
                P_DISTRICT.OracleDbType = OracleDbType.Varchar2;
                P_DISTRICT.Size = 2000;
                P_DISTRICT.Direction = ParameterDirection.Input;
                P_DISTRICT.Value = command.P_DISTRICT.ToSafeString();

                var P_PROVINCE = new OracleParameter();
                P_PROVINCE.ParameterName = "P_PROVINCE";
                P_PROVINCE.OracleDbType = OracleDbType.Varchar2;
                P_PROVINCE.Size = 2000;
                P_PROVINCE.Direction = ParameterDirection.Input;
                P_PROVINCE.Value = command.P_PROVINCE.ToSafeString();

                var P_ZIPCODE = new OracleParameter();
                P_ZIPCODE.ParameterName = "P_ZIPCODE";
                P_ZIPCODE.OracleDbType = OracleDbType.Varchar2;
                P_ZIPCODE.Size = 1000;
                P_ZIPCODE.Direction = ParameterDirection.Input;
                P_ZIPCODE.Value = command.P_ZIPCODE.ToSafeString();

                var P_MEDIA_STREAMING = new OracleParameter();
                P_MEDIA_STREAMING.ParameterName = "P_MEDIA_STREAMING";
                P_MEDIA_STREAMING.OracleDbType = OracleDbType.Varchar2;
                P_MEDIA_STREAMING.Size = 4000;
                P_MEDIA_STREAMING.Direction = ParameterDirection.Input;
                P_MEDIA_STREAMING.Value = command.P_MEDIA_STREAMING.ToSafeString();

                var P_USER_JOURNEY = new OracleParameter();
                P_USER_JOURNEY.ParameterName = "P_USER_JOURNEY";
                P_USER_JOURNEY.OracleDbType = OracleDbType.Varchar2;
                P_USER_JOURNEY.Size = 4000;
                P_USER_JOURNEY.Direction = ParameterDirection.Input;
                P_USER_JOURNEY.Value = command.P_USER_JOURNEY.ToSafeString();

                var P_CID = new OracleParameter();
                P_CID.ParameterName = "P_CID";
                P_CID.OracleDbType = OracleDbType.Varchar2;
                P_CID.Size = 4000;
                P_CID.Direction = ParameterDirection.Input;
                P_CID.Value = command.P_CID.ToSafeString();

                var P_STATUS_COMPLETE = new OracleParameter();
                P_STATUS_COMPLETE.ParameterName = "P_STATUS_COMPLETE";
                P_STATUS_COMPLETE.OracleDbType = OracleDbType.Varchar2;
                P_STATUS_COMPLETE.Size = 1000;
                P_STATUS_COMPLETE.Direction = ParameterDirection.Input;
                P_STATUS_COMPLETE.Value = command.P_STATUS_COMPLETE.ToSafeString();

                //Return
                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "p_return_code";
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Size = 2000;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "p_return_message";
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Size = 2000;
                p_return_message.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_MICROSITE.PROC_INSERT_MICROSITE",
                    new
                    {
                        //Input
                        P_ORDER_NO,
                        P_USER,
                        P_SEGMENT,
                        P_NUMBER_OF_USER,
                        P_RESIDENTIAL,
                        P_TYPE_OF_USER,
                        P_PACKAGE_NAME,
                        P_PACKAGE_CODE,
                        P_SPEED,
                        P_PLAYBOX_BUNDLE,
                        P_PLAYBOX_ADDON,
                        P_ROUTER_BUNDLE,
                        P_ROUTER_ADDON,
                        P_PRICE,
                        P_FIRST_NAME,
                        P_LAST_NAME,
                        P_TELEPHONE,
                        P_EMAIL,
                        P_ADDRESS,
                        P_SUB_DISTRICT,
                        P_DISTRICT,
                        P_PROVINCE,
                        P_ZIPCODE,
                        P_MEDIA_STREAMING,
                        P_USER_JOURNEY,
                        P_CID,
                        P_STATUS_COMPLETE,

                        //Return
                        p_return_code,
                        p_return_message

                    });

                command.p_return_code = p_return_code.Value.ToSafeString();
                command.p_return_message = p_return_message.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command.p_return_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.p_return_code = "-1";
                command.p_return_message = "Error Service MicrositeWSCommandHandler" + ex.Message;
            }
        }
    }
}
