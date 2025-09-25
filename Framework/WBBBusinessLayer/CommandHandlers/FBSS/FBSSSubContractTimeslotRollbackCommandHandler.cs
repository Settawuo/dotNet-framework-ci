using WBBContract;
using WBBContract.Commands.FBSS;
using WBBData.Repository;

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public sealed class FBSSSubContractTimeslotRollbackCommandHandler : ICommandHandler<FBSSSubContractTimeslotRollbackCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public FBSSSubContractTimeslotRollbackCommandHandler(ILogger logger,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public void Handle(FBSSSubContractTimeslotRollbackCommand command)
        {
            object[] paramOut;

            var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_TIME_SLOT.ROLLBACK_BATCH",
            out paramOut,
                new
                {
                    p_file_path = command.FilePath,
                    p_file_name = command.FileName,
                    p_error_message = command.ErrorMessage,
                    p_user = "FBB_BATCH"
                });
        }
    }
}
