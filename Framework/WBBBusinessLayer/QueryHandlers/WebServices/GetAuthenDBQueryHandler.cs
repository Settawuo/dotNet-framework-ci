using WBBBusinessLayer.AuthenDBServices;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetAuthenDBQueryHandler : IQueryHandler<GetAuthenDBQuery, string>
    {
        private readonly ILogger _logger;

        public GetAuthenDBQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public string Handle(GetAuthenDBQuery query)
        {
            _logger.Info("GetAuthenDBQuery - Start");

            using (var service = new AuthenDBServices.WebDBConfigService())
            {
                var connectionString = "";
                var dbAccount = service.WS_AUTHENDB_DBConfig(query.ProjectCode);

                if (dbAccount.Status == Constants.AuthenDBReturnStatus.Success)
                {
                    _logger.Info("GetADBConnectionString Start");
                    connectionString = GetADBConnectionString(connectionString,
                        dbAccount, query.Template);
                    _logger.Info("GetADBConnectionString End");
                }

                _logger.Info("GetAuthenDBQuery - End");
                return connectionString;
            }
        }

        private static string GetADBConnectionString(object connectionString, DatabaseAccount dbAccount, string dbConstringTemplate)
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

            connectionString = dbConstringTemplate
                .Replace("{ServerName}", decryptDbAccount.ServerName)
                .Replace("{DatabaseName}", decryptDbAccount.DatabaseName)
                .Replace("{UserName}", decryptDbAccount.UserName)
                .Replace("{Password}", decryptDbAccount.Password);

            return connectionString.ToString();
        }
    }
}
