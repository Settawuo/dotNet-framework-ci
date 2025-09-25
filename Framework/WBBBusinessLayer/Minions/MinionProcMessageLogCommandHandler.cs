using System;
using WBBContract;
using WBBContract.Minions;
using WBBData.Repository;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.Minions
{
    public class MinionProcMessageLogCommandHandler : ICommandHandler<MinionProcMessageLogCommand>
    {
        private readonly IEntityRepository<string> _objService;

        public MinionProcMessageLogCommandHandler(IEntityRepository<string> objService)
        {
            _objService = objService;
        }

        public void Handle(MinionProcMessageLogCommand command)
        {
            try
            {
                _objService.ExecuteStoredProc("WBB.PKG_FBB_INS_PROFILE.PROC_MESSAGE_LOG",

                   new
                   {
                       p_cust_non_mobile = command.Cust_Non_Mobile,
                       p_process_name = command.Process_Name,
                       p_create_user = command.Create_User,
                       p_create_date = command.Create_Date,
                       p_return_code = command.Return_Code,
                       p_return_desc = command.Return_Desc,
                       p_remark = command.Remark,
                   });


                command.Response_Code = 0;
                command.Response_Message = "null";

            }
            catch (Exception ex)
            {
                command.Response_Code = -1;
                command.Response_Message = "Error call WBB.PKG_FBB_INS_PROFILE.PROC_MESSAGE_LOG " + ex.GetErrorMessage();
            }
        }
    }
}
