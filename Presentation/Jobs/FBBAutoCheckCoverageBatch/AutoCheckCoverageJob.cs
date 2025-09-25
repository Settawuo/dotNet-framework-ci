using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBAutoCheckCoverageBatch
{

    public class AutoCheckCoverageJob
    {
        #region Propoties

        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;

        #endregion

        #region Constructor

        public AutoCheckCoverageJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        #endregion

        #region Public Method

        public List<NoCoverageModel> Execute()
        {
            _logger.Info("START Execute GetNoCheckCoverage");
            StartWatching();

            var resultCoverage = GetNoCheckCoverage();

            _logger.Info("STOP Execute GetNoCheckCoverage");
            StopWatching("AutoCheckCoverageJob");

            if (resultCoverage != null && resultCoverage.NO_COVERAGE_RESULT.Count > 0)
                return resultCoverage.NO_COVERAGE_RESULT;
            else
                return null;
        }

        public InstallLeaveMessageModel InstallLeaveMessageData(string RESULT_ID)
        {
            var query = new InstallLeaveMessageQuery()
            {
                p_result_id = RESULT_ID, //Add Param ResultId.
                p_status = string.Empty //R23.01 Add new parameter from InstallLeaveMessageQueryHandler (onservice special)
            };
            return _queryProcessor.Execute(query); ;
        }

        public string GetLovList(string name = "")
        {
            string lovData = "";
            try
            {
                var query = new GetLovQuery
                {
                    LovType = "SCREEN",
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);

                if (lov != null && lov.Count > 0)
                {
                    lovData = lov.FirstOrDefault().LovValue1.ToSafeString();
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetLovList : " + ex.GetErrorMessage());
            }
            return lovData;
        }

        public void LogMsg(string Msg)
        {
            _logger.Info(Msg);
        }

        #endregion

        #region Private Method

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
        }

        private AutoCheckCoverageModel GetNoCheckCoverage()
        {
            var query = new GetListNoCoverageQuery() { };
            return _queryProcessor.Execute(query);
        }

        #endregion

    }

    public class BatchRestSharpJsonSerializer : RestSharp.Serializers.ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _jsonSerializer;

        public BatchRestSharpJsonSerializer()
        {
            ContentType = "application/json";

            _jsonSerializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Include,
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include
            };
        }

        public BatchRestSharpJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json";
            _jsonSerializer = serializer;
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';
                    _jsonSerializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        public string DateFormat { get; set; }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string ContentType { get; set; }

    }
}
