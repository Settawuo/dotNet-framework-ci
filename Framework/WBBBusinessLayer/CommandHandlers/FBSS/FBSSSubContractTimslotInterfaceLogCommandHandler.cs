using WBBContract;
using WBBContract.Commands.FBSS;
using WBBData.Repository;

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class FBSSSubContractTimslotInterfaceLogCommandHandler : ICommandHandler<FBSSSubContractTimslotInterfaceLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public FBSSSubContractTimslotInterfaceLogCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(FBSSSubContractTimslotInterfaceLogCommand command)
        {
            object[] paramOut;

            var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_TIME_SLOT.INSERT_INTERFACE_LOG",
            out paramOut,
                new
                {
                    p_file_path = command.Filepath,
                    p_file_name = "",
                    p_method_name = "LOAD_SUBCONTRACTOR_TIMESLOT",
                    p_result = "Fail",
                    p_error_result = command.ErrorResult,
                    p_user = "FBB_BATCH"

                });

        }
    }
}
