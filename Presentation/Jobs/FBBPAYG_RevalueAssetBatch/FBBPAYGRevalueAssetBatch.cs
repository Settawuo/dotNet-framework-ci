using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;

namespace FBBPAYG_RevalueAssetBatch
{
    public class FBBPAYGRevalueAssetBatch
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<FBBPAYGRevalueAssetCommand> _BatvhRevalue;
        //private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        //public FBBPAYGRevalueAssetBatch(ILogger logger,
        //    IQueryProcessor queryProcessor,
        //    ICommandHandler<FBBPAYGRevalueAssetBatchCommand> batvhPurge,
        //    ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        //{
        public FBBPAYGRevalueAssetBatch(ILogger logger,
        IQueryProcessor queryProcessor,
        ICommandHandler<FBBPAYGRevalueAssetCommand> batvhRevalue)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _BatvhRevalue = batvhRevalue;
            //_sendMail = sendMail;
        }
        public void ProcessBatch()
        {
            //bool result = false;
            try
            {

                Console.WriteLine($"FBBPAYGRevalueAssetCommand start");
                _logger.Info("Get_FBSS_CONFIG_TBL_LOV Result : FBBPAYGRevalueAssetCommand start");

                var command = new FBBPAYGRevalueAssetCommand();

                _BatvhRevalue.Handle(command);
                _logger.Info("Get_FBSS_CONFIG_TBL_LOV Result : FBBPurgeDateCommand end");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckProcessBatch : " + ex.Message);
                //return result;
            }

        }
    }
}
