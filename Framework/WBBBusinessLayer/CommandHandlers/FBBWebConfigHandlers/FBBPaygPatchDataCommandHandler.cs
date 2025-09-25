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
    public class FBBPaygPatchDataUpdateCommandHandler : ICommandHandler<FBBPaygPatchDataUpdateCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPaygPatchDataUpdateCommand> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public FBBPaygPatchDataUpdateCommandHandler(ILogger logger,
            IEntityRepository<FBBPaygPatchDataUpdateCommand> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(FBBPaygPatchDataUpdateCommand command)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_Id, "FBBPaygPatchDataUpdateCommand", "PatchDataUpdate", null, "FBB|" + command.FullUrl, "");
            try
            {
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;


                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_PAYG_PATCH_SN.p_update_patch_sn",
                    out paramOut,
                       new
                       {
                           p_file_name = command.FileName,
                           p_serial_no = command.serialno,
                           p_serial_status = command.snstatus, //เพิ่ม p_SERIAL_STATUS
                           p_patch_status = command.patchstatus,
                           p_REMARK = command.remark,
                           p_CREATE_BY = command.CREATE_BY,

                           ret_code = ret_code,
                           ret_msg = ret_msg
                       });
                command.returnCode = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.returnMsg = ret_msg.Value == null ? "" : ret_msg.Value.ToString();


                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", ex.Message, "");
            }
        }

    }

    public class FBBPaygPatchDataListUpdateCommandHandler : ICommandHandler<FBBPaygPatchDataListUpdateCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBPaygPatchDataListUpdateCommand> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;


        public FBBPaygPatchDataListUpdateCommandHandler(ILogger logger,
            IEntityRepository<FBBPaygPatchDataListUpdateCommand> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(FBBPaygPatchDataListUpdateCommand command)
        {

            // PKG_FBB_PAYG_PATCH_SN.p_update_patch_sn_list
            try
            {
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                //var p_Product_List = new OracleParameter();
                //p_Product_List.ParameterName = "p_Product_List";
                //p_Product_List.OracleDbType = OracleDbType.RefCursor;
                //p_Product_List.Direction = ParameterDirection.Output;

                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBB_patch_sn_list =
                     command.p_Product_List.Select(
                         a => new fbb_patch_sn_update_list_Mapping
                         {
                             FILE_NAME = a.FILE_NAME,
                             SERIAL_NO = a.SERIAL_NO,
                             //INTERNET_NO = a.INTERNET_NO,
                             //STORAGE_LOCATION = a.STORAGE_LOCATION,
                             //MOVEMENT_TYPE = a.MOVEMENT_TYPE,
                             //FOA_CODE = a.FOA_CODE,
                             //SUBMIT_DATE = a.SUBMIT_DATE,
                             //POST_DATE = a.POST_DATE,
                             //SERIAL_STATUS = a.SERIAL_STATUS,
                             PATCH_STATUS = a.PATCH_STATUS,
                             //PATCH_STATUS_DESC = a.PATCH_STATUS_DES,
                             CREATE_BY = a.CREATE_BY
                             //REMARK = a.REMARK

                         }).ToArray()

                };

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Product_List", "WBB.FBB_PATCH_SN_UPDATE_LIST", packageMappingObjectModel);
                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_PAYG_PATCH_SN.p_update_patch_sn_list",
                    new object[]
                    {
                        //List
                        packageMapping,

                        //Return
                        //p_Product_List,
                        ret_code,
                        ret_msg

                    });
                command.ret_code = Convert.ToInt32(ret_code.Value == null ? "0" : ret_code.Value.ToString());
                command.ret_msg = ret_msg.Value == null ? "" : ret_msg.Value.ToString();
            }
            catch (Exception ex)
            {
                var s = ex;
            }

        }

        #region PackageMappingObjectModel
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public fbb_patch_sn_update_list_Mapping[] FBB_patch_sn_list { get; set; }


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
                OracleUdt.SetValue(con, udt, 0, FBB_patch_sn_list);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_patch_sn_list = (fbb_patch_sn_update_list_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("WBB.FBB_PATCH_SN_UPDATE_REC")]
        public class fbb_patch_sn_update_list_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new fbb_patch_sn_update_list_Mapping();
            }
        }

        [OracleCustomTypeMapping("WBB.FBB_PATCH_SN_UPDATE_LIST")]
        public class fbb_patch_sn_update_list_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new fbb_patch_sn_update_list_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class fbb_patch_sn_update_list_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("FILE_NAME")]
            public string FILE_NAME { get; set; }

            [OracleObjectMapping("SERIAL_NO")]
            public string SERIAL_NO { get; set; }

            [OracleObjectMapping("PATCH_STATUS")]
            public string PATCH_STATUS { get; set; }

            [OracleObjectMapping("CREATE_BY")]
            public string CREATE_BY { get; set; }
            #endregion Attribute Mapping

            public static fbb_patch_sn_update_list_Mapping Null
            {
                get
                {
                    var obj = new fbb_patch_sn_update_list_Mapping();
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
                OracleUdt.SetValue(con, udt, "SERIAL_NO", SERIAL_NO);
                OracleUdt.SetValue(con, udt, "PATCH_STATUS", PATCH_STATUS);
                OracleUdt.SetValue(con, udt, "CREATE_BY", CREATE_BY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
    public class FBBPaygPatchDataInsertCommandHandler : ICommandHandler<FBBPaygPatchDataInsertCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        //private readonly IEntityRepository<FBBPAYG_PATCH_SN_ERROR_LOG> _snErrLog;
        private readonly IWBBUnitOfWork _uow;


        public FBBPaygPatchDataInsertCommandHandler(ILogger logger,
            IEntityRepository<object> objService,
            //IEntityRepository<FBBPAYG_PATCH_SN_ERROR_LOG> snErrLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            //_snErrLog = snErrLog;
            _uow = uow;
        }

        public void Handle(FBBPaygPatchDataInsertCommand command)
        {

            // PKG_FBB_PAYG_PATCH_SN.p_insert_patch_sn
            try
            {
                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                //var p_Product_List = new OracleParameter();
                //p_Product_List.ParameterName = "p_Product_List";
                //p_Product_List.OracleDbType = OracleDbType.RefCursor;
                //p_Product_List.Direction = ParameterDirection.Output;

                var packageMappingObjectModel = new PackageMappingObjectModel
                {
                    FBB_patch_sn_list =
                     command.p_Product_List.Select(
                         a => new FBB_patch_sn_list_Mapping
                         {
                             FILE_NAME = a.FILE_NAME,
                             SERIAL_NO = a.SERIAL_NO,
                             INTERNET_NO = a.INTERNET_NO,
                             //STORAGE_LOCATION = a.STORAGE_LOCATION,
                             MOVEMENT_TYPE = a.MOVEMENT_TYPE,
                             FOA_CODE = a.FOA_CODE,
                             SUBMIT_DATE = a.SUBMIT_DATE,
                             POST_DATE = a.POST_DATE,
                             SERIAL_STATUS = a.SERIAL_STATUS,
                             //PATCH_STATUS = a.PATCH_STATUS,
                             //PATCH_STATUS_DESC = a.PATCH_STATUS_DES,
                             CREATE_BY = a.CREATE_BY,
                             REMARK = a.REMARK

                         }).ToArray()

                };

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Product_List", "WBB.FBB_PATCH_SN_LIST", packageMappingObjectModel);
                var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_PAYG_PATCH_SN.p_insert_patch_sn",
                    new object[]
                    {
                        //List
                        packageMapping,

                        //Return
                        //p_Product_List,
                        ret_code,
                        ret_msg

                    });

            }
            catch (Exception ex)
            {
                var s = ex;
            }

        }

        public class FBBPaygPatchDataInsertSendMailCommandHandler : ICommandHandler<FBBPaygPatchDataInsertSendMailCommand>
        {
            private readonly ILogger _logger;
            private readonly IEntityRepository<object> _objService;
            //private readonly IEntityRepository<FBBPAYG_PATCH_SN_ERROR_LOG> _snErrLog;
            private readonly IWBBUnitOfWork _uow;


            public FBBPaygPatchDataInsertSendMailCommandHandler(ILogger logger,
                IEntityRepository<object> objService,
                //IEntityRepository<FBBPAYG_PATCH_SN_ERROR_LOG> snErrLog,
                IWBBUnitOfWork uow)
            {
                _logger = logger;
                _objService = objService;
                //_snErrLog = snErrLog;
                _uow = uow;
            }

            public void Handle(FBBPaygPatchDataInsertSendMailCommand command)
            {

                // PKG_FBB_PAYG_PATCH_SN.p_insert_patch_sn_sendmail
                try
                {
                    var outp = new List<object>();
                    var paramOut = outp.ToArray();

                    var ret_code = new OracleParameter();
                    ret_code.OracleDbType = OracleDbType.Decimal;
                    ret_code.Direction = ParameterDirection.Output;

                    var ret_msg = new OracleParameter();
                    ret_msg.OracleDbType = OracleDbType.Varchar2;
                    ret_msg.Size = 2000;
                    ret_msg.Direction = ParameterDirection.Output;

                    var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_PAYG_PATCH_SN.p_insert_patch_sn_sendmail",
                    out paramOut,
                    new
                    {
                        p_FILE_NAME = command.FILE_NAME.ToSafeString(),
                        p_EMAIL = command.EMAIL,
                        p_USER_NAME = command.USER_NAME,
                        p_ACTIVE_FLAG = command.ACTIVE_FLAG,
                        // return 
                        ret_code,
                        ret_msg
                    });

                }
                catch (Exception ex)
                {
                    var s = ex;
                }
            }
        }

        #region PackageMappingObjectModel
        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_patch_sn_list_Mapping[] FBB_patch_sn_list { get; set; }


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
                OracleUdt.SetValue(con, udt, 0, FBB_patch_sn_list);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_patch_sn_list = (FBB_patch_sn_list_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("WBB.FBB_PATCH_SN_REC")]
        public class FBB_patch_sn_list_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_patch_sn_list_Mapping();
            }
        }

        [OracleCustomTypeMapping("WBB.FBB_PATCH_SN_LIST")]
        public class FBB_patch_sn_list_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new FBB_patch_sn_list_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_patch_sn_list_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMapping("FILE_NAME")]
            public string FILE_NAME { get; set; }

            [OracleObjectMapping("SERIAL_NO")]
            public string SERIAL_NO { get; set; }

            [OracleObjectMapping("INTERNET_NO")]
            public string INTERNET_NO { get; set; }

            [OracleObjectMapping("STORAGE_LOCATION")]
            public string STORAGE_LOCATION { get; set; }

            [OracleObjectMapping("MOVEMENT_TYPE")]
            public string MOVEMENT_TYPE { get; set; }

            [OracleObjectMapping("FOA_CODE")]
            public string FOA_CODE { get; set; }

            [OracleObjectMapping("SUBMIT_DATE")]
            public string SUBMIT_DATE { get; set; }

            [OracleObjectMapping("POST_DATE")]
            public string POST_DATE { get; set; }

            [OracleObjectMapping("SERIAL_STATUS")]
            public string SERIAL_STATUS { get; set; }

            [OracleObjectMapping("PATCH_STATUS")]
            public string PATCH_STATUS { get; set; }

            [OracleObjectMapping("PATCH_STATUS_DESC")]
            public string PATCH_STATUS_DESC { get; set; }

            [OracleObjectMapping("CREATE_BY")]
            public string CREATE_BY { get; set; }

            [OracleObjectMapping("REMARK")]
            public string REMARK { get; set; }


            #endregion Attribute Mapping

            public static FBB_patch_sn_list_Mapping Null
            {
                get
                {
                    var obj = new FBB_patch_sn_list_Mapping();
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
                OracleUdt.SetValue(con, udt, "SERIAL_NO", SERIAL_NO);
                OracleUdt.SetValue(con, udt, "INTERNET_NO", INTERNET_NO);
                OracleUdt.SetValue(con, udt, "STORAGE_LOCATION", STORAGE_LOCATION);
                OracleUdt.SetValue(con, udt, "MOVEMENT_TYPE", MOVEMENT_TYPE);
                OracleUdt.SetValue(con, udt, "FOA_CODE", FOA_CODE);
                OracleUdt.SetValue(con, udt, "SUBMIT_DATE", SUBMIT_DATE);
                OracleUdt.SetValue(con, udt, "POST_DATE", POST_DATE);
                OracleUdt.SetValue(con, udt, "SERIAL_STATUS", SERIAL_STATUS);
                OracleUdt.SetValue(con, udt, "PATCH_STATUS", PATCH_STATUS);
                OracleUdt.SetValue(con, udt, "PATCH_STATUS_DESC", PATCH_STATUS_DESC);
                OracleUdt.SetValue(con, udt, "CREATE_BY", CREATE_BY);
                OracleUdt.SetValue(con, udt, "REMARK", REMARK);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}
