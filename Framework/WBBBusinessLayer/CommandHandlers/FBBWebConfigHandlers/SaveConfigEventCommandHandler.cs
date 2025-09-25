using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SaveConfigEventCommandHandler : ICommandHandler<SaveConfigEventCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public SaveConfigEventCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(SaveConfigEventCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                var o_event_code = new OracleParameter();
                o_event_code.OracleDbType = OracleDbType.Varchar2;
                o_event_code.Size = 2000;
                o_event_code.Direction = ParameterDirection.Output;

                var o_return_code = new OracleParameter();
                o_return_code.OracleDbType = OracleDbType.Decimal;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_msg = new OracleParameter();
                o_return_msg.OracleDbType = OracleDbType.Varchar2;
                o_return_msg.Size = 2000;
                o_return_msg.Direction = ParameterDirection.Output;

                var eventSubObjectModel = new EventSubObjectModel();
                eventSubObjectModel.FBB_EVENT_SUB_ARRAY = command.fbbEventSubArray.Select(a => new Fbb_Event_Sub_ArrayMapping
                {
                    SERVICE_OPTION = a.SERVICE_OPTION.ToSafeString(),
                    EVENT_CODE = a.EVENT_CODE.ToSafeString(),
                    SUB_LOCATION_ID = a.SUB_LOCATION_ID.ToSafeString(),
                    SUB_CONTRACT_NAME = a.SUB_CONTRACT_NAME.ToSafeString(),
                    SUB_TEAM_ID = a.SUB_TEAM_ID.ToSafeString(),
                    SUB_TEAM_NAME = a.SUB_TEAM_NAME.ToSafeString(),
                    INSTALL_STAFF_ID = a.INSTALL_STAFF_ID.ToSafeString(),
                    INSTALL_STAFF_NAME = a.INSTALL_STAFF_NAME.ToSafeString(),
                    EVENT_START_DATE = a.EVENT_START_DATE.ToSafeString(),
                    EVENT_END_DATE = a.EVENT_END_DATE.ToSafeString(),
                    SUB_ROW_ID = a.SUB_ROW_ID.ToSafeString()
                }).ToArray();

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                string strP_plug_and_play_flag = "";
                strP_plug_and_play_flag = command.plug_and_play_flag.ToSafeString();
                //if (!string.IsNullOrEmpty(command.event_code.ToSafeString()))
                //{
                //    strP_plug_and_play_flag = command.plug_and_play_flag.ToSafeString();
                //}

                var eventSub = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_FBB_EVENT_SUB_ARRAY", "FBB_EVENT_SUB_ARRAY", eventSubObjectModel);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR013.save_event",
                out paramOut,
                   new
                   {
                       p_service_option = command.service_option.ToSafeString(),
                       p_event_code = command.event_code.ToSafeString(),
                       p_effective_date = command.effective_date.ToSafeString(),
                       p_expire_date = command.expire_date.ToSafeString(),
                       p_user = command.user.ToSafeString(),
                       p_technology = command.technology.ToSafeString(),
                       p_provice = command.provice.ToSafeString(),
                       p_amphur = command.amphur.ToSafeString(),
                       p_tumbon = command.tumbon.ToSafeString(),
                       p_zipcode = command.zipcode.ToSafeString(),
                       p_plug_and_play_flag = strP_plug_and_play_flag,//command.plug_and_play_flag.ToSafeString(),
                       /// array
                       p_fbb_event_sub_array = eventSub,
                       /// Out
                       /// 
                       o_event_code = o_event_code,
                       o_return_code = o_return_code,
                       o_return_msg = o_return_msg

                   });
                command.event_code = o_event_code.Value.ToSafeString();
                command.return_msg = o_return_msg.Value.ToSafeString();
                string return_code = o_return_code.Value.ToSafeString();
                command.return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.return_code = -1;
                command.return_msg = "Error call save event service " + ex.Message;
            }
        }

        #region Mapping fbb_event_sub_array Type Oracle

        public class EventSubObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Fbb_Event_Sub_ArrayMapping[] FBB_EVENT_SUB_ARRAY { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static EventSubObjectModel Null
            {
                get
                {
                    EventSubObjectModel obj = new EventSubObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBB_EVENT_SUB_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_EVENT_SUB_ARRAY = (Fbb_Event_Sub_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("FBB_EVENT_SUB_RECORD")]
        public class Fbb_Event_SubOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Fbb_Event_Sub_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("FBB_EVENT_SUB_ARRAY")]
        public class FbbEventSubObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new EventSubObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Fbb_Event_Sub_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Fbb_Event_Sub_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SERVICE_OPTION")]
            public string SERVICE_OPTION { get; set; }

            [OracleObjectMappingAttribute("EVENT_CODE")]
            public string EVENT_CODE { get; set; }

            [OracleObjectMappingAttribute("SUB_LOCATION_ID")]
            public string SUB_LOCATION_ID { get; set; }

            [OracleObjectMappingAttribute("SUB_CONTRACT_NAME")]
            public string SUB_CONTRACT_NAME { get; set; }

            [OracleObjectMappingAttribute("SUB_TEAM_ID")]
            public string SUB_TEAM_ID { get; set; }

            [OracleObjectMappingAttribute("SUB_TEAM_NAME")]
            public string SUB_TEAM_NAME { get; set; }

            [OracleObjectMappingAttribute("INSTALL_STAFF_ID")]
            public string INSTALL_STAFF_ID { get; set; }

            [OracleObjectMappingAttribute("INSTALL_STAFF_NAME")]
            public string INSTALL_STAFF_NAME { get; set; }

            [OracleObjectMappingAttribute("EVENT_START_DATE")]
            public string EVENT_START_DATE { get; set; }

            [OracleObjectMappingAttribute("EVENT_END_DATE")]
            public string EVENT_END_DATE { get; set; }

            [OracleObjectMappingAttribute("SUB_ROW_ID")]
            public string SUB_ROW_ID { get; set; }

            #endregion Attribute Mapping

            public static Fbb_Event_Sub_ArrayMapping Null
            {
                get
                {
                    Fbb_Event_Sub_ArrayMapping obj = new Fbb_Event_Sub_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "SERVICE_OPTION", SERVICE_OPTION);
                OracleUdt.SetValue(con, udt, "EVENT_CODE", EVENT_CODE);
                OracleUdt.SetValue(con, udt, "SUB_LOCATION_ID", SUB_LOCATION_ID);
                OracleUdt.SetValue(con, udt, "SUB_CONTRACT_NAME", SUB_CONTRACT_NAME);
                OracleUdt.SetValue(con, udt, "SUB_TEAM_ID", SUB_TEAM_ID);
                OracleUdt.SetValue(con, udt, "SUB_TEAM_NAME", SUB_TEAM_NAME);
                OracleUdt.SetValue(con, udt, "INSTALL_STAFF_ID", INSTALL_STAFF_ID);
                OracleUdt.SetValue(con, udt, "INSTALL_STAFF_NAME", INSTALL_STAFF_NAME);
                OracleUdt.SetValue(con, udt, "EVENT_START_DATE", EVENT_START_DATE);
                OracleUdt.SetValue(con, udt, "EVENT_END_DATE", EVENT_END_DATE);
                OracleUdt.SetValue(con, udt, "SUB_ROW_ID", SUB_ROW_ID);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  fbb_event_sub_array Type Oracle
    }
}
