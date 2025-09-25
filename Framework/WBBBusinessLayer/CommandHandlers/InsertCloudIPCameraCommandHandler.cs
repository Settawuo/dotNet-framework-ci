using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.CommandHandlers
{
    public class InsertCloudIPCameraCommandHandler : ICommandHandler<InsertCloudIPCameraCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;
        private readonly IEntityRepository<string> _objService;

        public InsertCloudIPCameraCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
            _objService = objService;
        }

        public void Handle(InsertCloudIPCameraCommand command)
        {
            //InterfaceLogCommand log = null;
            //log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command, command.p_return_order, "InsertCloudIPCamera", "InsertCloudIPCameraCommandHandler", null, "FBB", "");

            //R23.06 IP Camera
            try
            {
                #region Check & Get quickPanelModel
                var P_AIS_MOBILE = "";
                var P_AIS_NONMOBILE = "";
                var quickWinModel = command.QuickWinPanelModel;

                // check null.
                if (null == quickWinModel)
                    throw new Exception("Model is null.");
                if (null == quickWinModel.CoveragePanelModel)
                    throw new Exception("CoveragePanelModel is null.");
                if (null == quickWinModel.CustomerRegisterPanelModel)
                    throw new Exception("CustomerRegisterPanelModel is null.");
                if (null == quickWinModel.SummaryPanelModel)
                    throw new Exception("SummaryPanelModel is null.");

                var cpm = quickWinModel.CoveragePanelModel;
                var crpm = quickWinModel.CustomerRegisterPanelModel;
                var spm = quickWinModel.SummaryPanelModel;

                //get number
                var phoneNo = crpm.L_MOBILE.ToSafeString() == "" ? crpm.L_CONTACT_PHONE.ToSafeString() : crpm.L_MOBILE.ToSafeString();
                if (cpm.P_MOBILE.ToSafeString() != "")
                {
                    cpm.P_MOBILE = cpm.P_MOBILE.Replace("|", "");
                    if (cpm.P_MOBILE != "")
                    {
                        if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) == "0")
                        {
                            P_AIS_NONMOBILE = "";
                            P_AIS_MOBILE = cpm.P_MOBILE.ToSafeString();
                        }
                        else if (cpm.P_MOBILE.ToSafeString().Substring(0, 1) != "0")
                        {
                            P_AIS_MOBILE = "";
                            P_AIS_NONMOBILE = cpm.P_MOBILE.ToSafeString();
                        }
                    }
                }
                #endregion

                if (command.ActionType == ActionType.Insert)
                {
                    //get ipcamera list
                    var packageModelList = new List<PackageModel>();
                    packageModelList = spm.PackageModelList;
                    packageModelList = packageModelList
                        .Where(p => p.PRODUCT_SUBTYPE == "IP_CAMERA")
                        .OrderBy(p => p.PACKAGE_SERVICE_CODE).ThenBy(p => p.PACKAGE_TYPE).ToList();

                    foreach (var itemPack in packageModelList)
                    {
                        command.p_package_service_code = itemPack.PACKAGE_SERVICE_CODE.ToSafeString();
                        command.p_product_subtype = itemPack.PRODUCT_SUBTYPE.ToSafeString();
                        command.p_package_type = itemPack.PACKAGE_TYPE.ToSafeString();
                        command.p_package_code = itemPack.SFF_PROMOTION_CODE.ToSafeString();
                        command.p_package_price = itemPack.PRICE_CHARGE;
                        command.p_package_count = itemPack.PACKAGE_COUNT.ToSafeDecimal();
                        command.p_created_by = phoneNo != null ? phoneNo.ToSafeString() : P_AIS_MOBILE != null ? P_AIS_MOBILE.ToSafeString() : P_AIS_NONMOBILE.ToSafeString();
                        command.p_fibrenet_id = crpm.FIBRE_ID == "" ? string.Empty : crpm.FIBRE_ID;

                        #region Call PKK Stored
                        var p_cust_row_id = new OracleParameter();
                        p_cust_row_id.ParameterName = "p_cust_row_id";
                        p_cust_row_id.Size = 2000;
                        p_cust_row_id.OracleDbType = OracleDbType.Varchar2;
                        p_cust_row_id.Direction = ParameterDirection.Input;
                        p_cust_row_id.Value = command.p_cust_row_id.ToSafeString();

                        var p_register_flag = new OracleParameter();
                        p_register_flag.ParameterName = "p_register_flag";
                        p_register_flag.Size = 2000;
                        p_register_flag.OracleDbType = OracleDbType.Varchar2;
                        p_register_flag.Direction = ParameterDirection.Input;
                        p_register_flag.Value = command.p_register_flag.ToSafeString();

                        var p_package_service_code = new OracleParameter();
                        p_package_service_code.ParameterName = "p_package_service_code";
                        p_package_service_code.Size = 2000;
                        p_package_service_code.OracleDbType = OracleDbType.Varchar2;
                        p_package_service_code.Direction = ParameterDirection.Input;
                        p_package_service_code.Value = command.p_package_service_code.ToSafeString();

                        var p_product_subtype = new OracleParameter();
                        p_product_subtype.ParameterName = "p_product_subtype";
                        p_product_subtype.Size = 2000;
                        p_product_subtype.OracleDbType = OracleDbType.Varchar2;
                        p_product_subtype.Direction = ParameterDirection.Input;
                        p_product_subtype.Value = command.p_product_subtype.ToSafeString();

                        var p_package_type = new OracleParameter();
                        p_package_type.ParameterName = "p_package_type";
                        p_package_type.Size = 2000;
                        p_package_type.OracleDbType = OracleDbType.Varchar2;
                        p_package_type.Direction = ParameterDirection.Input;
                        p_package_type.Value = command.p_package_type.ToSafeString();

                        var p_package_code = new OracleParameter();
                        p_package_code.ParameterName = "p_package_code";
                        p_package_code.Size = 2000;
                        p_package_code.OracleDbType = OracleDbType.Varchar2;
                        p_package_code.Direction = ParameterDirection.Input;
                        p_package_code.Value = command.p_package_code.ToSafeString();

                        var p_package_price = new OracleParameter();
                        p_package_price.ParameterName = "p_package_price";
                        p_package_price.OracleDbType = OracleDbType.Decimal;
                        p_package_price.Direction = ParameterDirection.Input;
                        p_package_price.Value = command.p_package_price;

                        var p_package_count = new OracleParameter();
                        p_package_count.ParameterName = "p_package_count";
                        p_package_count.OracleDbType = OracleDbType.Decimal;
                        p_package_count.Direction = ParameterDirection.Input;
                        p_package_count.Value = command.p_package_count;

                        var p_return_order = new OracleParameter();
                        p_return_order.ParameterName = "p_return_order";
                        p_return_order.Size = 2000;
                        p_return_order.OracleDbType = OracleDbType.Varchar2;
                        p_return_order.Direction = ParameterDirection.Input;
                        p_return_order.Value = command.p_return_order.ToSafeString();

                        var p_created_by = new OracleParameter();
                        p_created_by.ParameterName = "p_created_by";
                        p_created_by.Size = 2000;
                        p_created_by.OracleDbType = OracleDbType.Varchar2;
                        p_created_by.Direction = ParameterDirection.Input;
                        p_created_by.Value = command.p_created_by.ToSafeString();

                        var p_fibrenet_id = new OracleParameter();
                        p_fibrenet_id.ParameterName = "p_fibrenet_id";
                        p_fibrenet_id.Size = 2000;
                        p_fibrenet_id.OracleDbType = OracleDbType.Varchar2;
                        p_fibrenet_id.Direction = ParameterDirection.Input;
                        p_fibrenet_id.Value = command.p_fibrenet_id.ToSafeString();

                        var ret_code = new OracleParameter();
                        ret_code.ParameterName = "ret_code";
                        ret_code.OracleDbType = OracleDbType.Decimal;
                        ret_code.Direction = ParameterDirection.Output;

                        var ret_msg = new OracleParameter();
                        ret_msg.ParameterName = "ret_msg";
                        ret_msg.Size = 2000;
                        ret_msg.OracleDbType = OracleDbType.Varchar2;
                        ret_msg.Direction = ParameterDirection.Output;

                        var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_CLOUD_IP_CAMERA.PROC_INSERT_CLOUD_IP_CAMERA",
                            new
                            {
                                //Input
                                p_cust_row_id,
                                p_register_flag,
                                p_package_service_code,
                                p_product_subtype,
                                p_package_type,
                                p_package_code,
                                p_package_price,
                                p_package_count,
                                p_return_order,
                                p_created_by,
                                p_fibrenet_id,

                                //Return
                                ret_code,
                                ret_msg
                            });

                        command.ret_code = ret_code.Value != null ? int.Parse(ret_code.Value.ToSafeString()) : -1; //ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString() == "null" ? "0" : ret_code.Value.ToSafeString()) : -1;
                        command.ret_msg = ret_msg.Value.ToSafeString();
                        #endregion
                    }
                }
                else
                {
                    throw new Exception("Error InsertCloudIPCameraCommand");
                }

                //if (command.ret_code != -1)
                //{
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Success", "", "");
                //}
                //else
                //{
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", command.ret_msg, "");
                //}
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.ret_code = -1;
                command.ret_msg = ex.Message.ToSafeString();
                //InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", command.ret_msg, "");
            }
        }
    }
}
