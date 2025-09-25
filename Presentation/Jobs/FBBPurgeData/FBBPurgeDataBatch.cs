
using System;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBEntity.Extensions;

namespace FBBPurgeData
{
    public class FBBPurgeDataBatch
    {

        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FBBPurgeDateCommand> _BatvhPurge;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        public FBBPurgeDataBatch(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<FBBPurgeDateCommand> batvhPurge,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _BatvhPurge = batvhPurge;
            _sendMail = sendMail;
        }

        public void CheckProcessBatch()
        {
            //bool result = false;
            try
            {
                Get_FBSS_CONFIG_TBL_LOV();
                //var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_REPORT_EXPIRE_SIM", "PROCESS").FirstOrDefault();
                //_logger.Info("PROGRAM_PROCESS: " + program_process.ACTIVEFLAG);
                //if (program_process.ACTIVEFLAG == "Y")
                //    result = true;

                //return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckProcessBatch : " + ex.Message);
                //return result;
            }

        }

        public void Get_FBSS_CONFIG_TBL_LOV()
        {
            try
            {
                Console.WriteLine($"FBBPurgeDateCommand start");
                _logger.Info("Get_FBSS_CONFIG_TBL_LOV Result : FBBPurgeDateCommand start");

                var command = new FBBPurgeDateCommand();

                _BatvhPurge.Handle(command);
                _logger.Info("Get_FBSS_CONFIG_TBL_LOV Result : FBBPurgeDateCommand end");

                if (command.iserror != null)
                {
                    if (command.iserror.Count > 0)
                    {
                        string strbody = @"
                                        Dear All<br>
                                        <table>
                                            <tbody>
                                                <tr>
                                                    <th>Type</th>
                                                    <th>Name</th>
                                                    <th>Message/Exception</th>
                                                </tr>
                                            </tbody>
                                            <tbody>";
                        foreach (var item in command.iserror)
                        {
                            strbody += @"
                                    <tr>
                                        <td>" + item.CON_TYPE + @"</td>
                                        <td>" + item.CON_NAME + @"</td>
                                        <td>" + item.ret_msg + @"</td>
                                    </tr>
                                    ";
                        }
                        strbody += @"</tbody>
                            </table> ";
                        _logger.Info("Get_FBSS_CONFIG_TBL_LOV Result : SendMailBatchNotificationCommand Start");
                        var commSendMail = new SendMailBatchNotificationCommand
                        {
                            ProcessName = "SEND_EMAIL_ERROR_PURGE_DATA",
                            Body = strbody
                        };

                        _sendMail.Handle(commSendMail);
                        _logger.Info("SendMail Result : " + commSendMail.ReturnMessage);
                        _logger.Info("Get_FBSS_CONFIG_TBL_LOV Result : SendMailBatchNotificationCommand End");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Get_FBSS_CONFIG_TBL_LOV Result : " + ex.GetErrorMessage());
                Console.WriteLine($"Get_FBSS_CONFIG_TBL_LOV Result : " + ex.GetErrorMessage());
                //Console.ReadLine();
            }
        }

    }
}