using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebApi;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebApiModel;

namespace WBBBusinessLayer.QueryHandlers.ExWebApi
{
    public class RetrieveAddressQueryHandler : IQueryHandler<RetrieveAddressQuery, RetrieveAddressQueryModel>
    {
        private readonly IWBBUnitOfWork _unitOfWork;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _repositoryLog;
        private readonly IEntityRepository<object> _storeRepository;

        public RetrieveAddressQueryHandler(IWBBUnitOfWork unitOfWork, IEntityRepository<FBB_INTERFACE_LOG> repositoryLog, IEntityRepository<object> storeRepository)
        {
            _unitOfWork = unitOfWork;
            _repositoryLog = repositoryLog;
            _storeRepository = storeRepository;
        }

        public RetrieveAddressQueryModel Handle(RetrieveAddressQuery query)
        {
            string jsonQuery = JsonConvert.SerializeObject(query);
            InterfaceLogCommand interfaceLog = null;
            var result = new RetrieveAddressQueryModel();
            try
            {
                interfaceLog = InterfaceLogServiceHelper.StartInterfaceLog(
                    _unitOfWork,
                    _repositoryLog,
                    jsonQuery,
                    "",
                    "GetListDataLocalizeQueryHandler",
                    "GetListDataLocalizeQueryHandler",
                    "",
                    "FBB",
                    "ExternalApi"
                    );

                var IPostal_code = new OracleParameter()
                {
                    ParameterName = "p_postal_code",
                    Value = query.postal_code,
                    Direction = ParameterDirection.Input
                };

                var IAddress_id = new OracleParameter()
                {
                    ParameterName = "p_address_id",
                    Value = query.address_id,
                    Direction = ParameterDirection.Input
                };

                var ORet_address = new OracleParameter()
                {
                    ParameterName = "ret_address",
                    Direction = ParameterDirection.Output,
                    OracleDbType = OracleDbType.RefCursor
                };

                var storeResult = _storeRepository.ExecuteStoredProcMultipleCursor("wbb.localize_retrieve_address.localize_addresslist", new object[]
               {
                    IPostal_code,
                    IAddress_id,
                    ORet_address
               });

                var DataList = ConvertToList(storeResult[0]);
                if (DataList.Count > 0)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(
                       _unitOfWork,
                       _repositoryLog,
                       ResultMessageEnum.SUCCESS.ToString(),
                       interfaceLog,
                       "Success",
                       "",
                       "ExternalApi");

                    result.RESULT_CODE = Convert.ToString((int)ResultMessageEnumLocalize.SUCCESS);
                    result.RESULT_DESC = ResultMessageEnumLocalize.SUCCESS.ToString();
                    result.AddressList = DataList;
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(
                       _unitOfWork,
                       _repositoryLog,
                       ResultMessageEnum.DataNotFound.ToString(),
                       interfaceLog,
                       "DataNotFound",
                       "",
                       "ExternalApi");

                    result.RESULT_CODE = Convert.ToString((int)ResultMessageEnumLocalize.DataNotFound);
                    result.RESULT_DESC = ResultMessageEnumLocalize.DataNotFound.ToString();
                    result.AddressList = DataList;
                }
                return result;
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(
                    _unitOfWork,
                    _repositoryLog,
                    ex,
                    interfaceLog,
                    "Failed",
                    ex.GetErrorMessage(),
                    "ExternalApi"
                    );

                result.RESULT_CODE = Convert.ToString((int)ResultMessageEnumLocalize.SystemNotExit);
                result.RESULT_DESC = ResultMessageEnumLocalize.SystemNotExit.ToString();

                return result;
            }
        }
        private List<DataLocalizeResponse> ConvertToList(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<List<DataLocalizeResponse>>(json);
        }
    }
}
