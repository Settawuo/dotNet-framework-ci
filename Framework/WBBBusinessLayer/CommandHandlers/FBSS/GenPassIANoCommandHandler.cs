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

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class GenPassIANoCommandHandler : ICommandHandler<GenPassIANoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        public GenPassIANoCommandHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;

        }



        public void Handle(GenPassIANoCommand command)
        {           
            InterfaceLogCommand log = null;
            PreregisterModel Premodel = new PreregisterModel();
            //Start Interface Log
           // log = FBSSExtensions.StartInterfaceFBSSLog(_uow, _intfLog, command, "", "CreateOderPreRegister", "CreateOderPreRegisterCommandHandler");


            var ret_seq = new OracleParameter();
            ret_seq.OracleDbType = OracleDbType.Varchar2;
            ret_seq.Size = 2000;
            ret_seq.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var executeResult = _objService.ExecuteStoredProc("WBB.fbbdorm_password.encrypt",
            out paramOut,
                new
                {
                  //  ret_seq = ret_seq

                });


            //try
            //{               
                 
            //    string RESULT_DESC = "";
            //    //string result = "";

            //    using (var service = new OrderService())
            //    {                    
            //        Premodel.Result = service.createOrder(fbssCustorder, out RESULT_DESC);
            //    }

            //    //FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, fbssCustorder, log, "Success", Premodel.Result+ " " + RESULT_DESC);
            //}
            //catch (Exception ex)
            //{
            //    if (null != log)
            //    {
            //       // FBSSExtensions.EndInterfaceFBSSLog(_uow, _intfLog, "", log,"Failed", ex.Message);
            //    }

            //    throw ex;
            //}
        }

      

    }
}
