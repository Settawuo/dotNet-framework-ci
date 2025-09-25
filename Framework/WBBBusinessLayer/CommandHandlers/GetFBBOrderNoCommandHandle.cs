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
    public class GetFBBOrderNoCommandHandler : ICommandHandler<GetFBBOrderNoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetFBBOrderNoCommandHandler(ILogger logger,
              IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
        }
        public void Handle(GetFBBOrderNoCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.orderNo,
                   "GetFBBOrderNoCommandHandler", "GetFBBOrderNoCommandHandler", command.orderNo,
                   "IM_CALL_CENTER", "WEB");

                ServicesService a = new ServicesService();
                GetFBBOrderNoRequestType reqIM = new GetFBBOrderNoRequestType()
                {
                    OrderNoStatus = command.orderNoStatus.ToSafeString(),
                    OrderNo = command.orderNo,
                    CaseID = command.caseID.ToSafeString(),
                    AddressAmphur = command.addressAmphur.ToSafeString(),
                    AddressBuilding = command.addressBuilding.ToSafeString(),
                    AddressFloor = command.addressFloor.ToSafeString(),
                    AddressMoo = command.addressMoo.ToSafeString(),
                    AddressMooban = command.addressMooban.ToSafeString(),
                    AddressNo = command.addressNo.ToSafeString(),
                    AddressPostCode = command.addressPostCode.ToSafeString(),
                    AddressProvince = command.addressProvince.ToSafeString(),
                    AddressRoad = command.addressRoad.ToSafeString(),
                    AddressSoi = command.addressSoi.ToSafeString(),
                    AddressTumbol = command.addressTumbol.ToSafeString()
                };
                GetFBBOrderNoResponseType resIM = a.GetFBBOrderNo(reqIM);

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
                command.Return_Desc = "Error GetFBBOrderNoCommand " + ex.Message;
            }
        }
    }
}
