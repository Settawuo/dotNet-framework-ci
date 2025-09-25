
using System;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYGLoadSIM
{
    public class FBBPAYGLoadSIMBatch
    {

        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<TruncateSIMSlocTempCommand> _SIMSloc;
        private readonly ICommandHandler<TruncateSIMSlocTempHVRCommand> _SIMSlocHVR;
        private readonly ICommandHandler<LoadFileSimCommand> _LoadSim;
        private readonly ICommandHandler<LoadFileSimHVRCommand> _LoadSimHVR;

        public FBBPAYGLoadSIMBatch(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<TruncateSIMSlocTempCommand> SIMSloc,
            ICommandHandler<TruncateSIMSlocTempHVRCommand> SIMSlocHVR,
            ICommandHandler<LoadFileSimCommand> LoadSim,
            ICommandHandler<LoadFileSimHVRCommand> LoadSimHVR)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _SIMSloc = SIMSloc;
            _SIMSlocHVR = SIMSlocHVR;
            _LoadSim = LoadSim;
            _LoadSimHVR = LoadSimHVR;
        }

        #region ProcessLoadSIM
        public void ProcessLoadSIM()
        {
            var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
            if (program_process.DISPLAY_VAL == "Y")
            {
                try
                {
                    Console.WriteLine($" HVR");
                    //Step 1 truncate temp table fbbpayg_sim_sloc_temp before insert data & Connect HVR table name wfm_r8.wfs_team_attr (C#)
                    var command = new TruncateSIMSlocTempHVRCommand { };
                    _SIMSlocHVR.Handle(command);

                    if (command.RET_CODE == "0")
                    {
                        _logger.Info("TruncateSIMSlocTempHVRCommand : " + command.RET_MSG);
                        _logger.Info("Data From HVR Total : " + command.Total);
                        Console.WriteLine($"TruncateSIMSlocTempHVRCommand : " + command.RET_MSG);
                        Console.WriteLine($"Data From HVR Total : " + command.Total);

                        //Step 2 Call package PKG_FBBPayG_Load_SIM
                        var command2 = new LoadFileSimHVRCommand { };
                        _LoadSimHVR.Handle(command2);
                        if (command2.RET_CODE == "0")
                        {
                            _logger.Info("LoadFileSimHVRCommand : " + command2.RET_MSG);
                            Console.WriteLine($"LoadFileSimHVRCommand : " + command2.RET_MSG);
                        }
                        else
                        {
                            _logger.Info("LoadFileSimHVRCommand : Failed " + command2.RET_MSG);
                            Console.WriteLine($"LoadFileSimHVRCommand : Failed" + command2.RET_MSG);
                        }
                    }
                    else
                    {
                        _logger.Info("TruncateSIMSlocTempHVRCommand : Failed " + command.RET_MSG);
                        Console.WriteLine($"TruncateSIMSlocTempHVRCommand : Failed" + command.RET_MSG);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info("ProcessLoadSIM Exception : " + ex.GetErrorMessage());
                    Console.WriteLine($"Exception:" + ex.GetErrorMessage());
                }
            }
            else
            {
                try
                {
                    Console.WriteLine($" Shareplex");
                    //Step 1 truncate temp table fbbpayg_sim_sloc_temp before insert data & Connect share plex table name wfm_r8.wfs_team_attr (C#)
                    var command = new TruncateSIMSlocTempCommand { };
                    _SIMSloc.Handle(command);
                    if (command.RET_CODE == "0")
                    {
                        _logger.Info("$TruncateSIMSlocTempCommand : " + command.RET_MSG);
                        _logger.Info("Data From Shareplex Total : " + command.Total);
                        Console.WriteLine($"TruncateSIMSlocTempCommand : " + command.RET_MSG);
                        Console.WriteLine($"Data From Shareplex Total : " + command.Total);

                        //Step 2 Call package PKG_FBBPayG_Load_SIM
                        var command2 = new LoadFileSimCommand { };
                        _LoadSim.Handle(command2);
                        if (command2.RET_CODE == "0")
                        {
                            _logger.Info("LoadFileSimCommand : " + command2.RET_MSG);
                            Console.WriteLine($"LoadFileSimCommand : " + command2.RET_MSG);
                        }
                        else
                        {
                            _logger.Info("LoadFileSimCommand : Failed " + command2.RET_MSG);
                            Console.WriteLine($"LoadFileSimCommand : Failed" + command2.RET_MSG);
                        }
                    }
                    else
                    {
                        _logger.Info("$TruncateSIMSlocTempCommand : Failed " + command.RET_MSG);
                        Console.WriteLine($"TruncateSIMSlocTempCommand : Failed" + command.RET_MSG);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info("ProcessLoadSIM Exception : " + ex.GetErrorMessage());
                    Console.WriteLine($"Exception:" + ex.GetErrorMessage());
                }
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