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
using static WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers.AddConfigurationLookupCommandHandler;
using WBBContract.Commands;
using WBBBusinessLayer.QueryHandlers;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class UpdateConfigurationLookupCommandHandler : ICommandHandler<UpdateConfigurationLookupCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public UpdateConfigurationLookupCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(UpdateConfigurationLookupCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, "", "UpdateConfigurationLookupCommandHandler", "UpdateConfigurationLookupCommandHandler", "", "FBB", "WEB_CONFIG");


                var p_lookup_name = new OracleParameter();
                p_lookup_name.ParameterName = "p_lookup_name";
                p_lookup_name.Size = 2000;
                p_lookup_name.OracleDbType = OracleDbType.Varchar2;
                p_lookup_name.Direction = ParameterDirection.Input;
                p_lookup_name.Value = command.lookup_name;

                var p_lookup_ontopflag = new OracleParameter();
                p_lookup_ontopflag.ParameterName = "p_lookup_ontopflag";
                p_lookup_ontopflag.Size = 2000;
                p_lookup_ontopflag.OracleDbType = OracleDbType.Varchar2;
                p_lookup_ontopflag.Direction = ParameterDirection.Input;
                p_lookup_ontopflag.Value = command.lookup_ontopflag;

                var p_lookup_ontop = new OracleParameter();
                p_lookup_ontop.ParameterName = "p_lookup_ontop";
                p_lookup_ontop.Size = 2000;
                p_lookup_ontop.OracleDbType = OracleDbType.Varchar2;
                p_lookup_ontop.Direction = ParameterDirection.Input;
                p_lookup_ontop.Value = command.lookup_ontop;

                var p_create_by = new OracleParameter();
                p_create_by.ParameterName = "p_create_by";
                p_create_by.Size = 2000;
                p_create_by.OracleDbType = OracleDbType.Varchar2;
                p_create_by.Direction = ParameterDirection.Input;
                p_create_by.Value = command.create_by;

                var p_modify_by = new OracleParameter();
                p_modify_by.ParameterName = "p_modify_by";
                p_modify_by.Size = 2000;
                p_modify_by.OracleDbType = OracleDbType.Varchar2;
                p_modify_by.Direction = ParameterDirection.Input;
                p_modify_by.Value = command.modify_by;

                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.BinaryFloat;
                return_code.Direction = ParameterDirection.Output;

                var return_msg = new OracleParameter();
                return_msg.ParameterName = "return_msg";
                return_msg.OracleDbType = OracleDbType.Varchar2;
                return_msg.Size = 2000;
                return_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var lookupObjectModel = new LookupObjectModel();

                lookupObjectModel.p_lookup_list = new fbbpayg_insert_lookup_list_Mapping[command.lookup_list.Count()];
                int count_lookup = 0;

                foreach (var item in command.lookup_list)
                {
                    var lookupHeaderObjectModel = new LookupHeaderObjectModel();
                    lookupHeaderObjectModel.lookup_header_list = new WBB_Payg_Lookup_Header_PackageOracleTypeMapping[item.lookup_header_list.Count()];
                    int count_header = 0;
                    foreach (var item_header in item.lookup_header_list)
                    {
                        var lookup_header = new WBB_Payg_Lookup_Header_PackageOracleTypeMapping()
                        {
                            parameter_name = item_header.parameter_name.ToSafeString(),
                            lookup_flag = item_header.lookup_flag.ToSafeString(),
                            parameter_value = item_header.parameter_value.ToSafeString()
                        };
                        lookupHeaderObjectModel.lookup_header_list[count_header] = lookup_header;
                        count_header++;
                    }
                    var lookupObjectMapping = new fbbpayg_insert_lookup_list_Mapping()
                    {
                        lookup_id = item.lookup_id.ToSafeString(),
                        lookup_status = item.lookup_status.ToSafeString(),
                        lookup_header_list = lookupHeaderObjectModel
                    };
                    lookupObjectModel.p_lookup_list[count_lookup] = lookupObjectMapping;
                    count_lookup++;
                }

                var lookupModelParameter = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_lookup_list", "FBBPAYG_INSERT_LOOKUP_LIST", lookupObjectModel); 
                var executeResult = _objService.ExecuteStoredProc("wbb.pkg_fixed_asset_prioritylookup.p_update_lookup",
                out paramOut,
                   new
                   {
                       //List
                       p_lookup_name,
                       p_lookup_ontopflag,
                       p_lookup_ontop,
                       lookupModelParameter,
                       p_create_by,
                       p_modify_by,
                       //Return
                       return_code,
                       return_msg

                   });

                command.return_code = return_code.Value.ToSafeString();
                command.return_msg = return_msg.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, return_code, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                command.return_code = "-1";
                command.return_msg = "Error call save event service " + ex.Message;
            }
        }
        #region LookupHeaderObjectModel
        //public class LookupHeaderObjectModel : INullable, IOracleCustomType
        //{
        //    [OracleArrayMapping()]
        //    public WBB_Payg_Lookup_Header_PackageOracleTypeMapping[] lookup_header_list { get; set; }


        //    private bool objectIsNull;

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public static LookupHeaderObjectModel Null
        //    {
        //        get
        //        {
        //            var obj = new LookupHeaderObjectModel();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, 0, lookup_header_list);
        //    }

        //    public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        lookup_header_list = (WBB_Payg_Lookup_Header_PackageOracleTypeMapping[])OracleUdt.GetValue(con, pUdt, 0);
        //    }
        //}

        //[OracleCustomTypeMapping("FBBPAYG_LOOKUP_HEADER_REC")]
        //public class WBB_Payg_Lookup_Header_PackageOracleTypeMappingFactory : IOracleCustomTypeFactory
        //{
        //    public IOracleCustomType CreateObject()
        //    {
        //        return new WBB_Payg_Lookup_Header_PackageOracleTypeMapping();
        //    }
        //}

        //[OracleCustomTypeMapping("FBBPAYG_LOOKUP_HEADER_LIST")]
        //public class WBB_Payg_Lookup_Header_PackageOracleModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        //{
        //    #region IOracleCustomTypeFactory Members

        //    public IOracleCustomType CreateObject()
        //    {
        //        return new LookupHeaderObjectModel();
        //    }

        //    #endregion IOracleCustomTypeFactory Members

        //    #region IOracleArrayTypeFactory Members

        //    public Array CreateArray(int numElems)
        //    {
        //        return new WBB_Payg_Lookup_Header_PackageOracleTypeMapping[numElems];
        //    }

        //    public Array CreateStatusArray(int numElems)
        //    {
        //        return null;
        //    }

        //    #endregion IOracleArrayTypeFactory Members
        //}

        //public class WBB_Payg_Lookup_Header_PackageOracleTypeMapping : INullable, IOracleCustomType
        //{
        //    private bool objectIsNull;

        //    #region Attribute Mapping

        //    [OracleObjectMappingAttribute("PARAMETER_NAME")]
        //    public string parameter_name { get; set; }
        //    [OracleObjectMappingAttribute("LOOKUP_FLAG")]
        //    public string lookup_flag { get; set; }
        //    [OracleObjectMappingAttribute("PARAMETER_VALUE")]
        //    public string parameter_value { get; set; }


        //    #endregion Attribute Mapping

        //    public static WBB_Payg_Lookup_Header_PackageOracleTypeMapping Null
        //    {
        //        get
        //        {
        //            var obj = new WBB_Payg_Lookup_Header_PackageOracleTypeMapping();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, "PARAMETER_NAME", parameter_name);
        //        OracleUdt.SetValue(con, pUdt, "LOOKUP_FLAG", lookup_flag);
        //        OracleUdt.SetValue(con, pUdt, "PARAMETER_VALUE", parameter_value);

        //    }

        //    public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //#endregion

        //#region LookupObjectModel
        //public class LookupObjectModel : INullable, IOracleCustomType
        //{
        //    [OracleArrayMapping()]
        //    public fbbpayg_insert_lookup_list_Mapping[] p_lookup_list { get; set; }


        //    private bool objectIsNull;

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public static LookupObjectModel Null
        //    {
        //        get
        //        {
        //            var obj = new LookupObjectModel();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, 0, p_lookup_list);
        //    }

        //    public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        p_lookup_list = (fbbpayg_insert_lookup_list_Mapping[])OracleUdt.GetValue(con, pUdt, 0);
        //    }
        //}

        //[OracleCustomTypeMapping("FBBPAYG_INSERT_LOOKUP_REC")]
        //public class PackageObjectModelFactoryTypeMappingFactory : IOracleCustomTypeFactory
        //{
        //    public IOracleCustomType CreateObject()
        //    {
        //        return new fbbpayg_insert_lookup_list_Mapping();
        //    }
        //}

        //[OracleCustomTypeMapping("FBBPAYG_INSERT_LOOKUP_LIST")]
        //public class PackageObjectModelFactoryObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        //{
        //    #region IOracleCustomTypeFactory Members

        //    public IOracleCustomType CreateObject()
        //    {
        //        return new LookupObjectModel();
        //    }

        //    #endregion IOracleCustomTypeFactory Members

        //    #region IOracleArrayTypeFactory Members

        //    public Array CreateArray(int numElems)
        //    {
        //        return new fbbpayg_insert_lookup_list_Mapping[numElems];
        //    }

        //    public Array CreateStatusArray(int numElems)
        //    {
        //        return null;
        //    }

        //    #endregion IOracleArrayTypeFactory Members
        //}

        //public class fbbpayg_insert_lookup_list_Mapping : INullable, IOracleCustomType
        //{
        //    private bool objectIsNull;

        //    #region Attribute Mapping

        //    [OracleObjectMappingAttribute("LOOKUP_ID")] //OracleObjectMappingAttribute
        //    public string lookup_id { get; set; }

        //    [OracleObjectMappingAttribute("LOOKUP_STATUS")]
        //    public string lookup_status { get; set; }

        //    [OracleObjectMappingAttribute("LOOKUP_HEADER_LIST")]
        //    public LookupHeaderObjectModel lookup_header_list { get; set; }

        //    #endregion Attribute Mapping

        //    public static fbbpayg_insert_lookup_list_Mapping Null
        //    {
        //        get
        //        {
        //            var obj = new fbbpayg_insert_lookup_list_Mapping();
        //            obj.objectIsNull = true;
        //            return obj;
        //        }
        //    }

        //    public bool IsNull
        //    {
        //        get { return objectIsNull; }
        //    }

        //    public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        OracleUdt.SetValue(con, pUdt, "LOOKUP_ID", lookup_id);
        //        OracleUdt.SetValue(con, pUdt, "LOOKUP_STATUS", lookup_status);
        //        OracleUdt.SetValue(con, pUdt, "LOOKUP_HEADER_LIST", lookup_header_list);
        //    }

        //    public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        #endregion
    }
}
