using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.FBBShareplex;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.FBBHVRModels;
using WBBEntity.Models;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBBusinessLayer.QueryHandlers.FBBHVR
{
    public class GetPremiumAreaHVRQueryHandler : IQueryHandler<GetPremiumAreaHVRQuery, PremiumAreaModel>
    {
        private readonly ILogger _logger;
        private readonly IFBBHVREntityRepository<object> _fbbHVRRepository;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetPremiumAreaHVRQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBHVREntityRepository<object> fbbHVRRepository)
        {
            _logger = logger;
            _fbbHVRRepository = fbbHVRRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public PremiumAreaModel Handle(GetPremiumAreaHVRQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "" + query.ClientIP, "QUERY_PREMIUM_AREA", "GetPremiumAreaQuery", null, "FBB|" + query.FullUrl, "WEB");

            PremiumAreaModel executeResults = new PremiumAreaModel();

            var subDistrict = new NpgsqlParameter();
            subDistrict.ParameterName = "subdistrict";
            subDistrict.Size = 2000;
            subDistrict.NpgsqlDbType = NpgsqlDbType.Text;
            subDistrict.Direction = ParameterDirection.Input;
            subDistrict.Value = query.SubDistrict;

            var district = new NpgsqlParameter();
            district.ParameterName = "district";
            district.Size = 2000;
            district.NpgsqlDbType = NpgsqlDbType.Text;
            district.Direction = ParameterDirection.Input;
            district.Value = query.District;

            var province = new NpgsqlParameter();
            province.ParameterName = "province";
            province.Size = 2000;
            province.NpgsqlDbType = NpgsqlDbType.Text;
            province.Direction = ParameterDirection.Input;
            province.Value = query.Province;

            var postalCode = new NpgsqlParameter();
            postalCode.ParameterName = "postalcode";
            postalCode.Size = 2000;
            postalCode.NpgsqlDbType = NpgsqlDbType.Text;
            postalCode.Direction = ParameterDirection.Input;
            postalCode.Value = query.PostalCode;

            var language = new NpgsqlParameter();
            language.ParameterName = "language";
            language.Size = 2000;
            language.NpgsqlDbType = NpgsqlDbType.Text;
            language.Direction = ParameterDirection.Input;
            language.Value = query.Language;

            var multi_outputmessage = new NpgsqlParameter();
            multi_outputmessage.ParameterName = "multi_outputmessage";
            multi_outputmessage.NpgsqlDbType = NpgsqlDbType.Refcursor;
            multi_outputmessage.Direction = ParameterDirection.InputOutput;
            multi_outputmessage.Value = "multi_outputmessage";

            var returnpremiumconfig = new NpgsqlParameter();
            returnpremiumconfig.ParameterName = "returnpremiumconfig";
            returnpremiumconfig.NpgsqlDbType = NpgsqlDbType.Refcursor;
            returnpremiumconfig.Direction = ParameterDirection.InputOutput;
            returnpremiumconfig.Value = "returnpremiumconfig";

            try
            {
                var result = _fbbHVRRepository.ExecuteStoredProcMultipleCursorNpgsql("fbbadm.pkg_query_premium_area_query_premium_area",
                    new object[]
                    {
                        subDistrict,
                        district,
                        province,
                        postalCode,
                        language,

                        multi_outputmessage,
                        returnpremiumconfig
                    }).ToList();

                if (result != null)
                {
                    DataTable dtr1 = (DataTable)result[0];
                    DataTable dtr2 = (DataTable)result[1];
                    List<PremiumAreaModel> _cur1 = dtr1.ConvertDataTable<PremiumAreaModel>();
                    List<PremiumConfigModel> _cur2 = dtr2.ConvertDataTable<PremiumConfigModel>();

                    var _first = _cur1.FirstOrDefault();
                    executeResults.ReturnCode = _first.ReturnCode != null ? _first.ReturnCode.ToString() : "-1";
                    executeResults.ReturnMessage = _first.ReturnMessage != null ? _first.ReturnMessage.ToString() : "";

                    executeResults.ReturnPremiumConfig = _cur2.Select(t => new PremiumConfigModel()
                    {
                        Region = t.Region,
                        ProvinceTh = t.ProvinceTh,
                        DistrictTh = t.DistrictTh,
                        SubdistrictTh = t.SubdistrictTh,
                        ProvinceEn = t.ProvinceEn,
                        DistrictEn = t.DistrictEn,
                        SubdistrictEn = t.SubdistrictEn,
                        Postcode = t.Postcode
                    }).ToList();

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
                    executeResults.ReturnCode = "-1";
                    executeResults.ReturnMessage = "Error";

                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service fbbadm.pkg_query_premium_area_qurey_premium_area" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
                executeResults.ReturnCode = "-1";
                executeResults.ReturnMessage = "Error";
            }

            return executeResults;
        }
    }
}
