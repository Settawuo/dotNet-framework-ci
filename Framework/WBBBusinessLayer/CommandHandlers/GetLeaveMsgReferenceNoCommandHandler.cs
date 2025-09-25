using System;
using WBBBusinessLayer.IMService;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
namespace WBBBusinessLayer.CommandHandlers
{
    public class GetLeaveMsgReferenceNoCommandHandler : ICommandHandler<GetLeaveMsgReferenceNoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetLeaveMsgReferenceNoCommandHandler(ILogger logger,
              IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
        }
        public void Handle(GetLeaveMsgReferenceNoCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                string IN_TRANSACTION_ID = (command.referenceNo == null || command.referenceNo == "") ? (command.RESULTID.ToSafeString()) : command.referenceNo;
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, IN_TRANSACTION_ID,
                   "GetLeaveMsgReferenceNoCommandHandler", "GetLeaveMsgReferenceNoCommandHandler", command.referenceNo,
                   "IM_CALL_CENTER", "WEB");

                ServicesService a = new ServicesService();
                GetLeaveMsgReferenceNoRequestType reqIM = new GetLeaveMsgReferenceNoRequestType();
                reqIM.ReferenceNoStatus = command.referenceNoStatus;
                reqIM.ReferenceNo = command.referenceNo;
                reqIM.CaseID = command.caseID;
                reqIM.ContactMobileNo = command.contactMobileNo;
                reqIM.CustomerName = command.customerName;
                reqIM.CustomerLastName = command.customerLastName;
                reqIM.InService = command.inService;
                reqIM.AddressAmphur = command.addressAmphur;
                reqIM.AddressBuilding = command.addressBuilding;
                reqIM.AddressFloor = command.addressFloor;
                reqIM.AddressMoo = command.addressMoo;
                reqIM.AddressMooban = command.addressMooban;
                reqIM.AddressNo = command.addressNo;
                reqIM.AddressPostCode = command.addressPostCode;
                reqIM.AddressProvince = command.addressProvince;
                reqIM.AddressRoad = command.addressRoad;
                reqIM.AddressSoi = command.addressSoi;
                reqIM.AddressTumbol = command.addressTumbol;
                reqIM.ProductType = command.productType;

                GetLeaveMsgReferenceNoResponseType resIM = a.GetLeaveMsgReferenceNo(reqIM);

                command.Return_Code = resIM.ReturnCode;
                command.Return_Desc = resIM.ReturnMessage;

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resIM, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(),
                           "");
                }
                command.Return_Code = "003";
                command.Return_Desc = "Error GetLeaveMsgReferenceNoCommand " + ex.Message;
            }
        }
    }
}
