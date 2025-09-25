
using System;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYGExpireSIM
{
    public class FBBPAYGExpireSIMBatch
    {

        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<GenfileExpireSIMCommand> _genfile;

        public FBBPAYGExpireSIMBatch(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<GenfileExpireSIMCommand> genfile)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _genfile = genfile;
        }

        #region Gen Files
        public void GenFiles()
        {
            try
            {
                var command = new GenfileExpireSIMCommand()
                {
                    in_report_name = "FBBPAYG_REPORT_EXPIRE_SIM"
                };

                _genfile.Handle(command);

                if (command.ret_code != "0")
                {
                    _logger.Info("GenFiles Result : " + command.ret_msg);
                    Console.WriteLine($"GenFiles Result : " + command.ret_msg);
                }
                else
                {
                    _logger.Info("GenFiles Result : " + command.ret_msg);
                    Console.WriteLine($"GenFiles Result : " + command.ret_msg);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("GenFiles Result : " + ex.GetErrorMessage());
                Console.WriteLine($"GenFiles Result : " + ex.GetErrorMessage());
            }


        }
        #endregion

        #region Middle Functions
        public bool CheckProcessBatch()
        {
            bool result = false;
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_REPORT_EXPIRE_SIM", "PROCESS").FirstOrDefault();
                _logger.Info("PROGRAM_PROCESS: " + program_process.ACTIVEFLAG);
                if (program_process.ACTIVEFLAG == "Y")
                    result = true;

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckProcessBatch : " + ex.Message);
                return result;
            }

        }

        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }
        #endregion

    }
}