using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetImagePOIQueryHandler : IQueryHandler<GetImagePOIQuery, ImageResultModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ListImageLink> _listimage;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetImagePOIQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<ListImageLink> listimage,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _listimage = listimage;
            _intfLog = intfLog;
            _uow = uow;
        }

        public ImageResultModel Handle(GetImagePOIQuery query)
        {
            InterfaceLogCommand log = null;

            try
            {
                ImageResultModel resultModel = new ImageResultModel();
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var v_error_msg = new OracleParameter();
                v_error_msg.OracleDbType = OracleDbType.Varchar2;
                v_error_msg.Size = 2000;
                v_error_msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var cursor = new OracleParameter();
                cursor.OracleDbType = OracleDbType.RefCursor;
                cursor.Direction = ParameterDirection.Output;

                //log = SffServiceConseHelper.StartInterfaceSffLog(_uow, _intfLog, query,"","GetImagePOIQuery","GetImagePOIQueryHandler","");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GetImagePOIQuery", "GetImagePOIQueryHandler", null, "FBB", "");


                List<ListImageLink> executeResult = _listimage.ExecuteReadStoredProc("PKG_FBB_LIST_PICTURE.PROC_LIST_COVERAGE_PICTURE",
                   new
                   {
                       p_transaction_id = query.model.transaction_id,
                       p_user = query.model.user,
                       p_language = query.model.language,
                       p_site_code = query.model.site_code,
                       p_sub_district = query.model.sub_district,
                       p_zip_code = query.model.zip_code,

                       p_return_code = ret_code,
                       p_return_message = v_error_msg,
                       p_list_coverage_picture = cursor

                   }).ToList();

                resultModel.ReturnCode = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
                resultModel.ReturnMassage = v_error_msg.Value.ToSafeString();
                resultModel.PicList = executeResult;

                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, executeResult, log, "Success", resultModel.ReturnMassage);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", resultModel.ReturnMassage, "");

                return resultModel;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                //SffServiceConseHelper.EndInterfaceSffLog(_uow, _intfLog, "ERROR EXEPTION", log,"Failed", ex.GetErrorMessage());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "ERROR EXEPTION", log, "Failed", ex.GetErrorMessage(), "");
                return new ImageResultModel();
            }

        }
    }
}
