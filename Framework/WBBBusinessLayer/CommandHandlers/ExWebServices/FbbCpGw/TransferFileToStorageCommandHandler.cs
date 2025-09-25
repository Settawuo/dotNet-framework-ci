using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw
{
    public class TransferFileToStorageCommandHandler : ICommandHandler<TransferFileToStorageCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public TransferFileToStorageCommandHandler(ILogger logger,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
            _cfgLov = cfgLov;
        }

        public void Handle(TransferFileToStorageCommand command)
        {
            InterfaceLogCommand log = null;
            string logDataRequestAll = _cfgLov.Get(x => x.LOV_TYPE == "CONFIG" && x.LOV_NAME == "TRANSFERFILE_LOG_NOLIMIT" && x.ACTIVEFLAG == "Y").FirstOrDefault()?.LOV_VAL1 ?? "Y";
            string logDataResponseAll = _cfgLov.Get(x => x.LOV_TYPE == "CONFIG" && x.LOV_NAME == "TRANSFERFILE_LOG_NOLIMIT" && x.ACTIVEFLAG == "Y").FirstOrDefault()?.LOV_VAL2 ?? "N";
            string logLimitText = _cfgLov.Get(x => x.LOV_TYPE == "CONFIG" && x.LOV_NAME == "TRANSFERFILE_LOG_NOLIMIT" && x.ACTIVEFLAG == "Y").FirstOrDefault()?.LOV_VAL3 ?? "100";

            try
            {
                var lov = _cfgLov.Get(x => x.LOV_TYPE == "FBB_CONSTANT" && x.LOV_NAME == "Impersonate_App_TransferFile" && x.ACTIVEFLAG == "Y").FirstOrDefault();
                string username = lov.LOV_VAL1;
                string password = lov.LOV_VAL2;
                string ipAddress = lov.LOV_VAL3;

                #region Option

                if (command.Option == "2")
                {
                    #region Call DB Option 2

                    var fbb_insert_array = new PackageMappingObjectModel
                    {
                        FBB_INSERT_ARRAY =
                          command.FileList.Select(
                              p => new FBB_INSERT_ARRAY_Mapping
                              {
                                  OrderNo = p.OrderNo.ToSafeString(),
                                  Action = p.Action.ToSafeString(),
                                  Filename = p.FileName.ToSafeString(),
                                  DataFile = "" //p.DataFile.ToSafeString()

                              }).ToArray()
                    };

                    var FILE_ARRAY = OracleCustomTypeUtilities.CreateUDTArrayParameter("FILE_ARRAY", "FBB_INSERT_ARRAY", fbb_insert_array);

                    //Input
                    var OPTION_WS = new OracleParameter();
                    OPTION_WS.ParameterName = "OPTION_WS";
                    OPTION_WS.Size = 2000;
                    OPTION_WS.OracleDbType = OracleDbType.Varchar2;
                    OPTION_WS.Direction = ParameterDirection.Input;
                    OPTION_WS.Value = command.Option.ToSafeString();

                    var USER_NAME = new OracleParameter();
                    USER_NAME.ParameterName = "USER_NAME";
                    USER_NAME.Size = 2000;
                    USER_NAME.OracleDbType = OracleDbType.Varchar2;
                    USER_NAME.Direction = ParameterDirection.Input;
                    USER_NAME.Value = command.UserName.ToSafeString();

                    //Output
                    OracleParameter ResultCode = new OracleParameter();
                    ResultCode.ParameterName = "ResultCode";
                    ResultCode.OracleDbType = OracleDbType.Int32;
                    ResultCode.Direction = ParameterDirection.Output;

                    OracleParameter ResultDesc = new OracleParameter();
                    ResultDesc.ParameterName = "ResultDesc";
                    ResultDesc.OracleDbType = OracleDbType.Varchar2;
                    ResultDesc.Size = 2000;
                    ResultDesc.Direction = ParameterDirection.Output;

                    OracleParameter TRANSACTION_ID = new OracleParameter();
                    TRANSACTION_ID.ParameterName = "TRANSACTION_ID";
                    TRANSACTION_ID.OracleDbType = OracleDbType.Varchar2;
                    TRANSACTION_ID.Size = 2000;
                    TRANSACTION_ID.Direction = ParameterDirection.Output;

                    OracleParameter SEND_DETAIL_FILE = new OracleParameter();
                    SEND_DETAIL_FILE.ParameterName = "SEND_DETAIL_FILE";
                    SEND_DETAIL_FILE.OracleDbType = OracleDbType.RefCursor;
                    SEND_DETAIL_FILE.Direction = ParameterDirection.Output;

                    var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_MICROSITE.PROC_INSERT_FILE",
                        new object[]
                        {
                           OPTION_WS,
                           FILE_ARRAY,
                           USER_NAME,

                           ResultCode,
                           ResultDesc,
                           TRANSACTION_ID,
                           SEND_DETAIL_FILE
                        });

                    #endregion Call DB Option 2

                    if (executeResult != null)
                    {
                        command.Transaction_id = executeResult[2] != null ? executeResult[2].ToSafeString() : "";

                        if (log == null)
                        {
                            log = logDataRequestAll == "Y" ? InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB")
                                        : InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, LogTransferFile(command, logLimitText), command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB");
                        }

                        command.Return_code = executeResult[0] != null ? executeResult[0].ToSafeString() : "50001";
                        command.Return_message = executeResult[1] != null ? executeResult[1].ToSafeString() : "Failed";

                        if (command.Return_code == "20000")
                        {
                            var d_list_send_detail_file_cur = (DataTable)executeResult[3];
                            var tmpResultFileList = new List<tbFileListData>();
                            foreach (DataRow item in d_list_send_detail_file_cur.Rows)
                            {
                                tmpResultFileList.Add(new tbFileListData { OrderNo = item[0].ToString(), Action = item[1].ToString(), Filename = item[2].ToString(), Result = item[3].ToString(), ErrorDesc = item[4].ToString() });
                            }

                            List<FileListData> _resultFileList = tmpResultFileList.Select(d => new FileListData
                            {
                                OrderNo = d.OrderNo,
                                Action = d.Action,
                                FileName = d.Filename,
                                DataFile = null,
                                Result = d.Result,
                                ErrorDesc = d.ErrorDesc
                            }).ToList();

                            #region option 2 upload to nas
                            using (var impersonator = new Impersonator(username, ipAddress, password, false))
                            {
                                foreach (var itemRequest in command.FileList)
                                {
                                    int i = -1; int cycle = 0;
                                    foreach (var itemResponse in _resultFileList)
                                    {
                                        i++; cycle++; string responseFileName = Path.GetFileName(itemResponse.FileName);
                                        if (responseFileName.OrderBy(x => x).SequenceEqual(itemRequest.FileName.OrderBy(x => x)))
                                        {
                                            if (itemResponse.Result == "0" && !string.IsNullOrEmpty(itemRequest.Action) && !string.IsNullOrEmpty(itemRequest.FileName) && !string.IsNullOrEmpty(itemRequest.DataFile))
                                            {
                                                try
                                                {
                                                    // to account storage
                                                    byte[] imageBytesFile = Convert.FromBase64String(itemRequest.DataFile); //Base64 string to byte array

                                                    string filePath = itemResponse.FileName.ToSafeString();
                                                    string directoryPath = Path.GetDirectoryName(filePath);

                                                    #region gen pathNas
                                                    if (!Directory.Exists(directoryPath))
                                                    {
                                                        Directory.CreateDirectory(directoryPath);
                                                    }
                                                    #endregion

                                                    bool isExistFile = File.Exists(filePath) == true ? true : false;
                                                    if (isExistFile != true)
                                                    {
                                                        using (FileStream imageFile = new FileStream(filePath, FileMode.Create))
                                                        {
                                                            imageFile.Write(imageBytesFile, 0, imageBytesFile.Length);
                                                        }

                                                        itemRequest.OrderNo = itemResponse.OrderNo;
                                                        itemRequest.Action = itemResponse.Action;
                                                        itemRequest.FileName = filePath;
                                                        itemRequest.DataFile = null;
                                                        itemRequest.Result = itemResponse.Result;
                                                        itemRequest.ErrorDesc = ActionFileResult(itemRequest.Result);
                                                    }
                                                    else
                                                    {
                                                        itemRequest.OrderNo = itemResponse.OrderNo;
                                                        itemRequest.Action = itemResponse.Action;
                                                        itemRequest.FileName = filePath;
                                                        itemRequest.DataFile = null;
                                                        itemRequest.Result = "-1";
                                                        itemRequest.ErrorDesc = ActionFileResult(itemRequest.Result);
                                                    }
                                                }
                                                catch (Exception innerEx)
                                                {
                                                    itemRequest.OrderNo = itemResponse.OrderNo.ToSafeString();
                                                    itemRequest.Action = itemResponse.Action.ToSafeString();
                                                    itemRequest.FileName = itemRequest.FileName.ToSafeString();
                                                    itemRequest.DataFile = null;
                                                    itemRequest.Result = "-2";
                                                    itemRequest.ErrorDesc = ActionFileResult(itemRequest.Result);

                                                    throw innerEx;
                                                }
                                            }
                                            else
                                            {
                                                itemRequest.OrderNo = itemResponse.OrderNo;
                                                itemRequest.Action = itemResponse.Action;
                                                itemRequest.FileName = itemResponse.FileName;
                                                itemRequest.DataFile = null;
                                                itemRequest.Result = itemResponse.Result;
                                                itemRequest.ErrorDesc = ActionFileResult(itemResponse.Result);
                                            }

                                            _resultFileList.RemoveAt(i);
                                            break;
                                        }
                                        else if ((cycle == _resultFileList.Count()) && (command.FileList.Count() != _resultFileList.Count()))
                                        {
                                            bool isExistFile = File.Exists(itemResponse.FileName) == true ? true : false;
                                            if (isExistFile == true)
                                            {
                                                itemRequest.OrderNo = itemResponse.OrderNo.ToSafeString();
                                                itemRequest.Action = itemResponse.Action.ToSafeString();
                                                itemRequest.FileName = itemRequest.FileName.ToSafeString();
                                                itemRequest.DataFile = null;
                                                itemRequest.Result = "-1";
                                                itemRequest.ErrorDesc = ActionFileResult(itemRequest.Result);
                                            }
                                        }
                                    }
                                }
                                if (command.FileList != null && command.FileList.Count > 0)
                                {
                                    foreach (var itemCheck in command.FileList)
                                    {
                                        if (itemCheck.DataFile != null)
                                        {
                                            itemCheck.DataFile = null;
                                            itemCheck.Result = "-2";
                                            itemCheck.ErrorDesc = ActionFileResult(itemCheck.Result);
                                        }
                                    }
                                }
                            }
                            #endregion option 2 upload to nas

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                        }
                        else
                        {
                            command.FileList = null;
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.Return_message, "");
                        }
                    }
                    else
                    {
                        command.Transaction_id = ResultDesc.Value != null ? ResultDesc.Value.ToSafeString() : "";

                        if (log == null)
                        {
                            log = logDataRequestAll == "Y" ? InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB")
                                        : InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, LogTransferFile(command, logLimitText), command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB");
                        }

                        command.Return_code = ResultCode.Value != null ? ResultCode.Value.ToSafeString() : "50001";
                        command.Return_message = ResultDesc.Value != null ? ResultDesc.Value.ToSafeString() : "Failed";
                        command.FileList = null;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.Return_message, "");
                    }

                }
                else if (command.Option == "12")
                {
                    string convertPathCondition = _cfgLov.Get(x => x.LOV_TYPE == "CONFIG" && x.LOV_NAME == "TRANSFERFILE_CONVERT_PATH_NAS" && x.ACTIVEFLAG == "Y").FirstOrDefault()?.LOV_VAL1 ?? "NAS_FBB_IDCARD";
                    string convertPathChange = _cfgLov.Get(x => x.LOV_TYPE == "CONFIG" && x.LOV_NAME == "TRANSFERFILE_CONVERT_PATH_NAS" && x.ACTIVEFLAG == "Y").FirstOrDefault()?.LOV_VAL2 ?? "FBB_IDCARD_NDEV001B";

                    #region Call DB Option 12

                    //Input
                    var OPTION_WS = new OracleParameter();
                    OPTION_WS.ParameterName = "OPTION_WS";
                    OPTION_WS.Size = 2000;
                    OPTION_WS.OracleDbType = OracleDbType.Varchar2;
                    OPTION_WS.Direction = ParameterDirection.Input;
                    OPTION_WS.Value = command.Option.ToSafeString();

                    var ORDERNO = new OracleParameter();
                    ORDERNO.ParameterName = "ORDERNO";
                    ORDERNO.Size = 2000;
                    ORDERNO.OracleDbType = OracleDbType.Varchar2;
                    ORDERNO.Direction = ParameterDirection.Input;
                    ORDERNO.Value = command.OrderNo.ToSafeString();

                    var USER_NAME = new OracleParameter();
                    USER_NAME.ParameterName = "USER_NAME";
                    USER_NAME.Size = 2000;
                    USER_NAME.OracleDbType = OracleDbType.Varchar2;
                    USER_NAME.Direction = ParameterDirection.Input;
                    USER_NAME.Value = command.UserName.ToSafeString();

                    //Output
                    OracleParameter ResultCode = new OracleParameter();
                    ResultCode.ParameterName = "ResultCode";
                    ResultCode.OracleDbType = OracleDbType.Int32;
                    ResultCode.Direction = ParameterDirection.Output;

                    OracleParameter ResultDesc = new OracleParameter();
                    ResultDesc.ParameterName = "ResultDesc";
                    ResultDesc.OracleDbType = OracleDbType.Varchar2;
                    ResultDesc.Size = 2000;
                    ResultDesc.Direction = ParameterDirection.Output;

                    OracleParameter TRANSACTION_ID = new OracleParameter();
                    TRANSACTION_ID.ParameterName = "TRANSACTION_ID";
                    TRANSACTION_ID.OracleDbType = OracleDbType.Varchar2;
                    TRANSACTION_ID.Size = 2000;
                    TRANSACTION_ID.Direction = ParameterDirection.Output;

                    OracleParameter RETURN_DETAIL_FILE = new OracleParameter();
                    RETURN_DETAIL_FILE.ParameterName = "RETURN_DETAIL_FILE";
                    RETURN_DETAIL_FILE.OracleDbType = OracleDbType.RefCursor;
                    RETURN_DETAIL_FILE.Direction = ParameterDirection.Output;

                    var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_MICROSITE.PROC_QUERY_FILE",
                        new object[]
                        {
                           OPTION_WS,
                           ORDERNO,
                           USER_NAME,

                           ResultCode,
                           ResultDesc,
                           TRANSACTION_ID,
                           RETURN_DETAIL_FILE
                        });

                    #endregion Call DB Option 12

                    if (executeResult != null)
                    {
                        command.Transaction_id = executeResult[2] != null ? executeResult[2].ToSafeString() : "";

                        if (log == null)
                        {
                            log = logDataRequestAll == "Y" ? InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB")
                                        : InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, LogTransferFile(command, logLimitText), command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB");
                        }

                        command.Return_code = executeResult[0] != null ? executeResult[0].ToSafeString() : "50001";
                        command.Return_message = executeResult[1] != null ? executeResult[1].ToSafeString() : "Failed";

                        if (command.Return_code == "20000")
                        {
                            var d_list_return_detail_file_cur = (DataTable)executeResult[3];
                            var tmpResultFileList = new List<tbRETURN_DETAIL_FILE_CUR>();
                            foreach (DataRow item in d_list_return_detail_file_cur.Rows)
                            {
                                tmpResultFileList.Add(new tbRETURN_DETAIL_FILE_CUR { OrderNo = item[0].ToString(), Action = item[1].ToString(), Filename = item[2].ToString() });
                            }

                            List<FileListData> _resultFileList = tmpResultFileList.Select(d => new FileListData
                            {
                                OrderNo = d.OrderNo,
                                Action = d.Action,
                                FileName = d.Filename,
                                DataFile = null,
                                Result = null
                            }).ToList();

                            #region option 12 get base64
                            using (var impersonator = new Impersonator(username, ipAddress, password, false))
                            {
                                foreach (var itemResponse in _resultFileList)
                                {
                                    if ((itemResponse.FileName != ""))
                                    {
                                        string subPath = itemResponse.FileName.Replace($"/{convertPathCondition}/", $"/{convertPathChange}/");
                                        string pathCombine = "\\\\" + ipAddress + subPath.Replace("/", "\\"); //Path.Combine($"\\\\{ipAddress}", subPath.Replace('/', '\\'));
                                        string filePath = pathCombine.ToSafeString();
                                        bool isExistFile = File.Exists(filePath) == true ? true : false;
                                        if (isExistFile == true)
                                        {
                                            using (var imageFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                                            {
                                                byte[] buffer = new byte[imageFile.Length];

                                                int bytesRead = imageFile.Read(buffer, 0, buffer.Length);

                                                itemResponse.OrderNo = itemResponse.OrderNo;
                                                itemResponse.FileName = filePath;
                                                itemResponse.DataFile = Convert.ToBase64String(buffer, 0, bytesRead);
                                                itemResponse.Result = "0";
                                            }
                                        }
                                        else
                                        {
                                            itemResponse.Result = "-5";
                                        }
                                    }
                                    else
                                    {
                                        itemResponse.Result = "-5";
                                    }
                                    itemResponse.Action = null;
                                    itemResponse.Result = itemResponse.Result != null ? itemResponse.Result : "";
                                    itemResponse.ErrorDesc = ActionFileResult(itemResponse.Result);
                                }
                                command.FileList = _resultFileList;
                            }
                            #endregion option 12 get base64

                            if (logDataResponseAll == "Y")
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                            }
                            else
                            {
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, LogTransferFile(command, logLimitText), log, "Success", "", "");
                            }
                        }
                        else
                        {
                            command.FileList = null;

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.Return_message, "");
                        }
                    }
                    else
                    {
                        command.Transaction_id = TRANSACTION_ID.Value != null ? TRANSACTION_ID.Value.ToSafeString() : "";

                        if (log == null)
                        {
                            log = logDataRequestAll == "Y" ? InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB")
                                        : InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, LogTransferFile(command, logLimitText), command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB");
                        }

                        command.Return_code = ResultCode.Value != null ? ResultCode.Value.ToSafeString() : "50001";
                        command.Return_message = ResultDesc.Value != null ? ResultDesc.Value.ToSafeString() : "Failed";
                        command.FileList = null;

                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Failed", command.Return_message, "");
                    }
                }
                else
                {
                    command.Transaction_id = command.Transaction_id != null ? command.Transaction_id : string.Empty;

                    if (log == null)
                    {
                        log = logDataRequestAll == "Y" ? InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB")
                                    : InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, LogTransferFile(command, logLimitText), command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB");
                    }

                    command.Return_code = "40002";
                    command.Return_message = "Event not found";
                    command.FileList = null;

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "Event not found", log, "Failed", "Failed", "");
                }
                #endregion
            }
            catch (Exception ex)
            {
                command.Transaction_id = command.Transaction_id != null ? command.Transaction_id : string.Empty;

                if (log == null)
                {
                    log = logDataRequestAll == "Y" ? InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB")
                                : InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, LogTransferFile(command, logLimitText), command.Transaction_id, "TransferFileToStorageCommandHandler", "TransferFileToStorageCommandHandler", command.Transaction_id, "FBB", "WEB");
                }

                command.Return_code = "50001";
                command.Return_message = ex.Message;
                command.FileList = null;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", ex.GetErrorMessage(), "");
            }
        }

        private string ActionFileResult(String actionFile)
        {
            string actionResult = "";
            if (actionFile == "0")
            {
                actionResult = "";
            }
            else if (actionFile == "-1")
            {
                actionResult = "File name Duplicate";
            }
            else if (actionFile == "-2")
            {
                actionResult = "Can not Change format File";
            }
            else if (actionFile == "-3")
            {
                actionResult = "File over size";
            }
            else if (actionFile == "-4")
            {
                actionResult = "Can not Delete File";
            }
            else if (actionFile == "-5")
            {
                actionResult = "Data not Found";
            }

            return actionResult;
        }

        #region Mapping FBB_INSERT_ARRAY Type Oracle

        public class PackageMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public FBB_INSERT_ARRAY_Mapping[] FBB_INSERT_ARRAY { get; set; }

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
                OracleUdt.SetValue(con, udt, 0, FBB_INSERT_ARRAY);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_INSERT_ARRAY = (FBB_INSERT_ARRAY_Mapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMapping("FBB_INSERT_RECORD")]
        public class FBB_INSERT_ARRAY_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new FBB_INSERT_ARRAY_Mapping();
            }
        }

        [OracleCustomTypeMapping("FBB_INSERT_ARRAY")]
        public class FBB_INSERT_ARRAY_MappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
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
                return new FBB_INSERT_ARRAY_Mapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class FBB_INSERT_ARRAY_Mapping : INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("ORDERNO")]
            public string OrderNo { get; set; }

            [OracleObjectMappingAttribute("ACTION")]
            public string Action { get; set; }

            [OracleObjectMappingAttribute("FILENAME")]
            public string Filename { get; set; }

            [OracleObjectMappingAttribute("DATAFILE")]
            public string DataFile { get; set; }


            #endregion Attribute Mapping

            public static FBB_INSERT_ARRAY_Mapping Null
            {
                get
                {
                    var obj = new FBB_INSERT_ARRAY_Mapping();
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
                OracleUdt.SetValue(con, udt, "ORDERNO", OrderNo);
                OracleUdt.SetValue(con, udt, "ACTION", Action);
                OracleUdt.SetValue(con, udt, "FILENAME", Filename);
                OracleUdt.SetValue(con, udt, "DATAFILE", DataFile);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping FBB_INSERT_ARRAY Type Oracle

        private TransferFileToStorageCommand LogTransferFile(TransferFileToStorageCommand request, string limitText)
        {
            TransferFileToStorageCommand resultLog = new TransferFileToStorageCommand()
            {
                Transaction_id = request.Transaction_id,
                Channel = request.Channel,
                Option = request.Option,
                OrderNo = request.OrderNo,
                FileName = request.FileName,
                FileList = request.FileList?.Select(x => new FileListData
                {
                    OrderNo = x.OrderNo,
                    Action = x.Action,
                    FileName = x.FileName,
                    DataFile = x.DataFile?.Substring(0, Math.Min(x.DataFile.Length, limitText.ToSafeInteger())),
                    Result = x.Result,
                    ErrorDesc = x.ErrorDesc
                }).ToList(),
                Return_code = request.Return_code,
                Return_message = request.Return_message
            };

            return resultLog;
        }
    }
}
