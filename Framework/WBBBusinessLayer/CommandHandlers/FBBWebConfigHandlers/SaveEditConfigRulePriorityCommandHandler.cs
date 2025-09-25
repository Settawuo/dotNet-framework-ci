using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Types;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using System.Linq;
using System.Text;
using System.IO;
using static WBBBusinessLayer.CommandHandlers.SaveConfigRulePriorityCommandHandler;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SaveEditConfigRulePriorityCommandHandler : ICommandHandler<SaveEditConfigRulePriorityCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public SaveEditConfigRulePriorityCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(SaveEditConfigRulePriorityCommand command)
        {
            //InterfaceLogCommand log = null;
            try
            {
                //var o_event_code = new OracleParameter();
                //o_event_code.OracleDbType = OracleDbType.Varchar2;
                //o_event_code.Size = 2000;
                //o_event_code.Direction = ParameterDirection.Output;

                //var o_return_code = new OracleParameter();
                //o_return_code.OracleDbType = OracleDbType.Decimal;
                //o_return_code.Direction = ParameterDirection.Output;

                //var o_return_msg = new OracleParameter();
                //o_return_msg.OracleDbType = OracleDbType.Varchar2;
                //o_return_msg.Size = 2000;
                //o_return_msg.Direction = ParameterDirection.Output;

                var p_rule_id = new OracleParameter();
                p_rule_id.ParameterName = "p_rule_id";
                p_rule_id.Size = 2000;
                p_rule_id.OracleDbType = OracleDbType.Varchar2;
                p_rule_id.Direction = ParameterDirection.Input;
                p_rule_id.Value = command.Rule_id.ToSafeString();

                var p_rule_name = new OracleParameter();
                p_rule_name.ParameterName = "p_rule_name";
                p_rule_name.Size = 2000;
                p_rule_name.OracleDbType = OracleDbType.Varchar2;
                p_rule_name.Direction = ParameterDirection.Input;
                p_rule_name.Value = command.Rule_name.ToSafeString();

                var p_priority = new OracleParameter();
                p_priority.ParameterName = "p_priority";
                p_priority.Size = 2000;
                p_priority.OracleDbType = OracleDbType.BinaryFloat;
                p_priority.Direction = ParameterDirection.Input;
                p_priority.Value = command.Priority.ToSafeString();

                var p_lookup_name = new OracleParameter();
                p_lookup_name.ParameterName = "p_lookup_name";
                p_lookup_name.Size = 2000;
                p_lookup_name.OracleDbType = OracleDbType.Varchar2;
                p_lookup_name.Direction = ParameterDirection.Input;
                p_lookup_name.Value = command.Lookup_name.ToSafeString();

                var p_effective_start = new OracleParameter();
                p_effective_start.ParameterName = "p_effective_start";
                p_effective_start.Size = 2000;
                p_effective_start.OracleDbType = OracleDbType.Varchar2;
                p_effective_start.Direction = ParameterDirection.Input;
                p_effective_start.Value = command.effective_start.ToSafeString();

                var p_effective_end = new OracleParameter();
                p_effective_end.ParameterName = "p_effective_end";
                p_effective_end.Size = 2000;
                p_effective_end.OracleDbType = OracleDbType.Varchar2;
                p_effective_end.Direction = ParameterDirection.Input;
                p_effective_end.Value = command.effective_end.ToSafeString();

                var p_lmr_flag = new OracleParameter();
                p_lmr_flag.ParameterName = "p_lmr_flag";
                p_lmr_flag.Size = 2000;
                p_lmr_flag.OracleDbType = OracleDbType.Varchar2;
                p_lmr_flag.Direction = ParameterDirection.Input;
                p_lmr_flag.Value = command.Lmr_flag.ToSafeString();

                var p_create_by = new OracleParameter();
                p_create_by.ParameterName = "p_create_by";
                p_create_by.Size = 2000;
                p_create_by.OracleDbType = OracleDbType.Varchar2;
                p_create_by.Direction = ParameterDirection.Input;
                p_create_by.Value = command.Create_by.ToSafeString();

                var p_modified_by = new OracleParameter();
                p_modified_by.ParameterName = "p_modified_by";
                p_modified_by.Size = 2000;
                p_modified_by.OracleDbType = OracleDbType.Varchar2;
                p_modified_by.Direction = ParameterDirection.Input;
                p_modified_by.Value = command.Create_by.ToSafeString();

                //Return
                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.Decimal;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.Size = 2000;
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Direction = ParameterDirection.Output;

                var eventSubObjectModel = new EventSubObjectModel();
                eventSubObjectModel.FBB_EVENT_SUB_ARRAY = command.Lookup_param_list_Edit.Select(a => new Fbb_Event_Sub_ArrayMapping
                {
                    Param_rule_id = a.Param_rule_id.ToSafeInteger(),
                    Param_name = a.Param_name.ToSafeString(),
                    Param_flag = a.Param_flag.ToSafeString()

                }).ToArray();

                var eventSubObjectModel2 = new EventSubObjectModel2();
                eventSubObjectModel2.FBB_EVENT_SUB_ARRAY2 = command.Condition_list_Edit.Select(a => new Fbb_Event_Sub_ArrayMapping2
                {
                    Condition_id = a.Condition_id.ToSafeInteger(),
                    Condition_parameter = a.Condition_parameter.ToSafeString(),
                    Conditaion_operator = a.Conditaion_operator.ToSafeString(),
                    Conditaion_value = a.Conditaion_value.ToSafeString(),
                    Condition_flag = a.Condition_flag.ToSafeString(),

                }).ToArray();

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var eventSub = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_lookup_param_list", "FBBPAYG_GET_LOOKUP_PARAM_LIST", eventSubObjectModel);
                var eventSub2 = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_condition_list", "FBBPAYG_GET_CONDITION_LIST", eventSubObjectModel2);

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_PRIORITYLOOKUP.p_update_priority",
                out paramOut,
                   new
                   {
                       p_rule_id,
                       p_rule_name,
                       p_priority,
                       p_lookup_name,
                       /// array
                       p_lookup_param_list = eventSub,
                       p_effective_start,
                       p_effective_end,
                       p_lmr_flag,
                       p_condition_list = eventSub2,
                       p_create_by,
                       p_modified_by,
                       /// Out
                       return_code,
                       return_msg

                   });
                command.return_code = return_code.Value.ToSafeString();
                command.return_msg = return_msg.Value.ToSafeString();
                //string return_code = o_return_code.Value.ToSafeString();
                //command.return_code = o_return_code.Value.ToSafeString() != "null" ? decimal.Parse(o_return_code.Value.ToSafeString()) : 0;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.return_code = "-1";
                command.return_msg = "Error call save event service " + ex.Message;
            }
        }

        //[OracleCustomTypeMappingAttribute("FBBPAYG_GET_LOOKUP_PARAM_REC")]
        //public class Fbb_Event_SubOracleTypeMappingFactory : IOracleCustomTypeFactory
        //{
        //    public IOracleCustomType CreateObject()
        //    {
        //        return new Fbb_Event_Sub_ArrayMapping();
        //    }
        //}

        //[OracleCustomTypeMapping("FBBPAYG_GET_LOOKUP_PARAM_LIST")]
        //public class FbbEventSubObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        //{
        //    #region IOracleCustomTypeFactory Members

        //    public IOracleCustomType CreateObject()
        //    {
        //        return new EventSubObjectModel();
        //    }

        //    #endregion IOracleCustomTypeFactory Members

        //    #region IOracleArrayTypeFactory Members

        //    public Array CreateArray(int numElems)
        //    {
        //        return new Fbb_Event_Sub_ArrayMapping[numElems];
        //    }

        //    public Array CreateStatusArray(int numElems)
        //    {
        //        return null;
        //    }

        //    #endregion IOracleArrayTypeFactory Members
        //}

        ////list1
        //public class EventSubObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        //{
        //    [OracleArrayMapping()]
        //    public Fbb_Event_Sub_ArrayMapping[] FBB_EVENT_SUB_ARRAY { get; set; }

        //    private bool objectIsNull;

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public static EventSubObjectModel Null
        //    {
        //        get
        //        {
        //            EventSubObjectModel obj = new EventSubObjectModel();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, 0, FBB_EVENT_SUB_ARRAY);
        //    }

        //    public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        FBB_EVENT_SUB_ARRAY = (Fbb_Event_Sub_ArrayMapping[])OracleUdt.GetValue(con, pUdt, 0);
        //    }
        //}


        //public class Fbb_Event_Sub_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        //{
        //    private bool objectIsNull;

        //    #region Attribute Mapping

        //    [OracleObjectMappingAttribute("PARAM_RULE_ID")]
        //    public int Param_rule_id { get; set; }

        //    [OracleObjectMappingAttribute("LOOKUP_PARAMETER")]
        //    public string Param_name { get; set; }

        //    [OracleObjectMappingAttribute("RECORD_STATUS")]
        //    public string Param_flag { get; set; }


        //    #endregion Attribute Mapping

        //    public static Fbb_Event_Sub_ArrayMapping Null
        //    {
        //        get
        //        {
        //            Fbb_Event_Sub_ArrayMapping obj = new Fbb_Event_Sub_ArrayMapping();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public void FromCustomObject(Oracle.ManagedDataAccess.Client.OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, "PARAM_RULE_ID", Param_rule_id);
        //        OracleUdt.SetValue(con, pUdt, "LOOKUP_PARAMETER", Param_name);
        //        OracleUdt.SetValue(con, pUdt, "RECORD_STATUS", Param_flag);

        //    }

        //    public void ToCustomObject(Oracle.ManagedDataAccess.Client.OracleConnection con, IntPtr pUdt)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //[OracleCustomTypeMappingAttribute("FBBPAYG_GET_CONDITION_REC")]
        //public class Fbb_Event_SubOracleTypeMappingFactory2 : IOracleCustomTypeFactory
        //{
        //    public IOracleCustomType CreateObject()
        //    {
        //        return new Fbb_Event_Sub_ArrayMapping2();
        //    }
        //}

        //[OracleCustomTypeMapping("FBBPAYG_GET_CONDITION_LIST")]
        //public class FbbEventSubObjectModelFactory2 : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        //{
        //    #region IOracleCustomTypeFactory Members

        //    public IOracleCustomType CreateObject()
        //    {
        //        return new EventSubObjectModel2();
        //    }

        //    #endregion IOracleCustomTypeFactory Members

        //    #region IOracleArrayTypeFactory Members

        //    public Array CreateArray(int numElems)
        //    {
        //        return new Fbb_Event_Sub_ArrayMapping2[numElems];
        //    }

        //    public Array CreateStatusArray(int numElems)
        //    {
        //        return null;
        //    }

        //    #endregion IOracleArrayTypeFactory Members
        //}

        ////list2
        //public class EventSubObjectModel2 : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        //{
        //    [OracleArrayMapping()]
        //    public Fbb_Event_Sub_ArrayMapping2[] FBB_EVENT_SUB_ARRAY2 { get; set; }

        //    private bool objectIsNull;

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public static EventSubObjectModel2 Null
        //    {
        //        get
        //        {
        //            EventSubObjectModel2 obj = new EventSubObjectModel2();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, 0, FBB_EVENT_SUB_ARRAY2);
        //    }

        //    public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        FBB_EVENT_SUB_ARRAY2 = (Fbb_Event_Sub_ArrayMapping2[])OracleUdt.GetValue(con, pUdt, 0);
        //    }
        //}


        //public class Fbb_Event_Sub_ArrayMapping2 : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        //{
        //    private bool objectIsNull;

        //    [OracleObjectMappingAttribute("CONDITION_ID")]
        //    public int Condition_id { get; set; }

        //    [OracleObjectMappingAttribute("CONDITION_PARAMETER")]
        //    public string Condition_parameter { get; set; }

        //    [OracleObjectMappingAttribute("OPERATOR")]
        //    public string Conditaion_operator { get; set; }

        //    [OracleObjectMappingAttribute("VALUE")]
        //    public string Conditaion_value { get; set; }

        //    [OracleObjectMappingAttribute("RECORD_STATUS")]
        //    public string Condition_flag { get; set; }



        //    public static Fbb_Event_Sub_ArrayMapping2 Null
        //    {
        //        get
        //        {
        //            Fbb_Event_Sub_ArrayMapping2 obj = new Fbb_Event_Sub_ArrayMapping2();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public void FromCustomObject(Oracle.ManagedDataAccess.Client.OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, "CONDITION_ID", Condition_id);
        //        OracleUdt.SetValue(con, pUdt, "CONDITION_PARAMETER", Condition_parameter);
        //        OracleUdt.SetValue(con, pUdt, "OPERATOR", Conditaion_operator);
        //        OracleUdt.SetValue(con, pUdt, "VALUE", Conditaion_value);
        //        OracleUdt.SetValue(con, pUdt, "RECORD_STATUS", Condition_flag);

        //    }

        //    public void ToCustomObject(Oracle.ManagedDataAccess.Client.OracleConnection con, IntPtr pUdt)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}


    }
}
