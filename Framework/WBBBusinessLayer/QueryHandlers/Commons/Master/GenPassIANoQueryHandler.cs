//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WBBContract;

//using WBBEntity.Models;
//using WBBEntity.PanelModels;
//using WBBEntity.Extensions;
//using WBBData.Repository;

using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBBusinessLayer.FBSSOrderServices;
using WBBBusinessLayer.QueryHandlers.WebServices.FBSS;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBSS;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBContract.Queries.Commons.Master;
namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GenPassIANoQueryHandler : IQueryHandler<GenPassIANoQuery, GenpassIaNoModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public GenPassIANoQueryHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public GenpassIaNoModel Handle(GenPassIANoQuery query)
        { 
            var result = new GenpassIaNoModel();

            var o_return_code = new OracleParameter();
            o_return_code.OracleDbType = OracleDbType.Single;
            o_return_code.Size = 2000;
            o_return_code.Direction = ParameterDirection.Output;

            var o_return_message = new OracleParameter();
            o_return_message.OracleDbType = OracleDbType.Varchar2;
            o_return_message.Size = 2000;
            o_return_message.Direction = ParameterDirection.Output;

            var o_password = new OracleParameter();
            o_password.OracleDbType = OracleDbType.Varchar2;
            o_password.Size = 2000;
            o_password.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var executeResult = _objService.ExecuteStoredProc("air_admin.pkg_airor901.gen_password",
            out paramOut,
                new
                {
                    o_return_code = o_return_code,
                    o_return_message,
                    o_password =o_password
                });


            return result;
        }
    }
}
