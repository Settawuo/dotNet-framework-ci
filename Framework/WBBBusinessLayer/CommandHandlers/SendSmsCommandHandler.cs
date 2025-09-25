using Inetlab.SMPP;
using Inetlab.SMPP.Common;
using Inetlab.SMPP.PDU;
using System;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SendSmsCommandHandler : ICommandHandler<SendSmsCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

        public SendSmsCommandHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
        {
            _logger = logger;
            _lov = lov;
            _uow = uow;

            _interfaceLog = interfaceLog;
        }

        public void Handle(SendSmsCommand command)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _interfaceLog, command, command.Transaction_Id, "SendSmsCommand", "SendSms", null, "FBB|" + command.FullUrl, "SMS");
            try
            {
                var SmsConfig = _lov.Get(l => l.LOV_TYPE.Equals("FBB_CONSTANT") && l.LOV_NAME.Equals("SMPP_CONFIGURATION"));
                string Host = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("HOST")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("HOST")).FirstOrDefault().LOV_VAL1;
                string Port = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PORT")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PORT")).FirstOrDefault().LOV_VAL1;
                string System_id = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMID")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMID")).FirstOrDefault().LOV_VAL1;
                string Password = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PASSWORD")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("PASSWORD")).FirstOrDefault().LOV_VAL1;
                string System_type = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMTYPE")).FirstOrDefault().LOV_VAL1) ? "" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("SYSTEMTYPE")).FirstOrDefault().LOV_VAL1;
                string Addr_Ton = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRNPI")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRNPI")).FirstOrDefault().LOV_VAL1;
                string Addr_Npi = String.IsNullOrEmpty(SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRTON")).FirstOrDefault().LOV_VAL1) ? "0" : SmsConfig.Where(l => l.DISPLAY_VAL.Equals("ADDRTON")).FirstOrDefault().LOV_VAL1;
                //chist699 20/02/2024 senderSMS
                if (command.Source_Addr.ToUpper() == "AISFIBRE")
                {
                    var GetsenderName = _lov.Get(l => l.LOV_TYPE.Equals("CONFIG") && l.LOV_NAME.Equals("SENDER_SMS")).FirstOrDefault().LOV_VAL1;
                    string senderName = !string.IsNullOrEmpty(GetsenderName) ? GetsenderName : "AISFIBRE3";
                    command.Source_Addr = senderName;
                }

                if(command.Source_Addr.ToUpper() == "AISFIBRE")
                {
                    var senderName = _lov.Get(l => l.LOV_TYPE.Equals("CONFIG") && l.LOV_NAME.Equals("SENDER_SMS")).FirstOrDefault().LOV_VAL1;
                    command.Source_Addr = senderName;
                }

                using (SmppClient client = new SmppClient())
                {
                    //client.AddrNpi = Convert.ToByte(Addr_Npi);
                    //client.AddrTon = Convert.ToByte(Addr_Ton);
                    //client.SystemType = System_type;
                    if (client.Connect(Host, int.Parse(Port)))
                    {
                        BindResp bindResp = client.Bind(System_id, Password);

                        if (bindResp.Status == CommandStatus.ESME_ROK)
                        {
                            var submitResp = client.Submit(
                                SMS.ForSubmit()
                                    .From(command.Source_Addr)
                                    .To(command.Destination_Addr)
                                    .Coding(DataCodings.UCS2)
                                    .Text(command.Message_Text));
                            if (submitResp.All(x => x.Status == CommandStatus.ESME_ROK))
                            {
                                command.return_status = "Success";
                                //Success case
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Success", "", "SMS");
                            }
                            else
                            {
                                command.return_status = "Failed";
                                //Fail case
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", "Connection Status is not open", "SMS");
                            }
                        }

                        client.Disconnect();
                    }
                }

                //SmppClient client = new SmppClient();
                //client.Connect(Host, Convert.ToInt32(Port));
                //client.AddrNpi = Convert.ToByte(Addr_Npi);
                //client.AddrTon = Convert.ToByte(Addr_Ton);
                //client.SystemType = System_type;

                //if (client.Status == ConnectionStatus.Open)
                //{
                //    client.Bind(System_id, Password, ConnectionMode.Transmitter);
                //    if (client.Status == ConnectionStatus.Bound)
                //    {
                //        DataCodings dc = DataCodings.Default;
                //        if(isThai(command.Message_Text))
                //        {
                //            dc = DataCodings.UCS2;
                //        }
                //        else
                //        {
                //            dc = DataCodings.UCS2;
                //        }

                //        SubmitSm sm = new SubmitSm();
                //        sm.UserDataPdu.ShortMessage = client.GetMessageBytes(command.Message_Text, dc);
                //        sm.SourceAddr = command.Source_Addr;
                //        sm.SourceAddrTon = 5;
                //        sm.SourceAddrNpi = 1;
                //        sm.DestAddr = command.Destination_Addr;
                //        sm.DestAddrTon = 1;
                //        sm.DestAddrNpi = 1;
                //        //sm.DataCoding = DataCodings.Default;
                //        sm.RegisteredDelivery = 1;
                //        sm.MessageMode = MessageModes.Default;
                //        sm.MessageType = MessageTypes.Default;
                //        sm.ProtocolId = 0;
                //        sm.PriorityFlag = 0;
                //        sm.ScheduleDeliveryTime = "";
                //        sm.RegisteredDelivery = 1;
                //        sm.SMSCReceipt = SMSCDeliveryReceipt.SuccessFailure;
                //        sm.Acknowledgement = SMEAcknowledgement.Delivery;
                //        sm.Notification = IntermediateNotification.NotRequested;
                //        sm.ReplaceIfPresent = 1;
                //        sm.DefaultMessageId = 0;
                //        SubmitSmResp response = client.Submit(sm);
                //        command.return_status = response.Status.ToString();
                //        if(response.Status == CommandStatus.ESME_ROK)
                //        {
                //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Success", "", "SMS");
                //        }
                //        else
                //        {
                //            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", response.Status.ToString(), "SMS");
                //        }

                //        /////////////////Use for Bulk Send////////////////
                //        //IList<SubmitSmResp> responses = client.Submit(
                //        //    Inetlab.SMPP.SMS.ForSubmit()
                //        //    .Text(command.Message_Text)
                //        //    .From(command.Source_Addr, 5, 1)
                //        //    .To(command.Destination_Addr, 1, 1)
                //        //    .Coding(dc)
                //        //    .ProtocolId(0)
                //        //    .DeliveryReceipt()
                //        //);
                //        client.Disconnect();
                //    }
                //    else
                //    {
                //        command.return_status = ConnectionStatus.Outbound.ToString();
                //        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", "Connection Status is Out Bound", "SMS");
                //    }
                //}
                //else
                //{
                //    command.return_status = "";
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, command, log, "Failed", "Connection Status is not open", "SMS");
                //}
            }
            catch (Exception ex)
            {
                command.return_status = "";
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _interfaceLog, "", log, "Failed", ex.Message, "SMS");
            }
        }

        public bool isThai(String msg)
        {
            if (msg != null)
            {
                for (int i = 0; i < msg.Length; i++)
                {
                    char code = msg[i];
                    if ((161 <= code) && (code <= 251) || (3585 <= code)
                            && (code <= 3675))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
