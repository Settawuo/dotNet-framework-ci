using iWorkflowsContract.Queries.WebServices;
using WBBBusinessLayer;
using WBBBusinessLayer.AuthenDBServices;
using WBBContract;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServices;

namespace iWorkflowsBusinessLayer.QueryHandlers.WebServices
{
    public class GetDbAccountQueryHandle : IQueryHandler<GetDbAccountQuery, WBBDbAccount>
    {
        private readonly ILogger _logger;

        public GetDbAccountQueryHandle(ILogger logger)
        {
            _logger = logger;
        }

        public WBBDbAccount Handle(GetDbAccountQuery query)
        {
            var iwfDbAccount = new WBBDbAccount();
            using (var service = new WBBBusinessLayer.AuthenDBServices.WebDBConfigService())
            {
                var dbAccount = service.WS_AUTHENDB_DBConfig(query.ProjectCode);

                if (dbAccount.Status == Constants.AuthenDBReturnStatus.Success)
                {
                    iwfDbAccount = GetADBAccount(dbAccount);
                }
                else
                {
                    iwfDbAccount.UserName = "";
                    iwfDbAccount.Password = "";
                    iwfDbAccount.ServerName = "";
                }

                return iwfDbAccount;
            }
        }

        private WBBDbAccount GetADBAccount(DatabaseAccount dbAccount)
        {
            var decryptDbAccount = new DatabaseAccount()
            {
                UserName = AuthenDecrypt.Decrypt.textDecrpyt(dbAccount.UserName),
                Password = AuthenDecrypt.Decrypt.textDecrpyt(dbAccount.Password),
                DatabaseName = AuthenDecrypt.Decrypt.textDecrpyt(dbAccount.DatabaseName),
                ServerName = AuthenDecrypt.Decrypt.textDecrpyt(dbAccount.ServerName),
                Status = dbAccount.Status,
                Detail = dbAccount.Detail
            };

            var iwfDbAccount = new WBBDbAccount();
            iwfDbAccount.UserName = decryptDbAccount.UserName;
            iwfDbAccount.Password = decryptDbAccount.Password;
            iwfDbAccount.ServerName = decryptDbAccount.ServerName;

            return iwfDbAccount;
        }
    }
}
