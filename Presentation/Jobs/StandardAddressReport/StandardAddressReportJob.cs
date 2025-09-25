using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace StandardAddressReport
{
    using FBBPayGTransAirnet;
    using StandardAddressReport.CompositionRoot;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.FBBWebConfigModels;


    public class StandardAddressReportJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;

        protected string _fbbFullConnPath = ConfigurationManager.AppSettings["FBB_FULL_CONN_PATH"].ToSafeString();
        protected string _fbbFullConnDomain = ConfigurationManager.AppSettings["FCP_DOMAIN"].ToSafeString();
        protected string _fbbFullConnUser = ConfigurationManager.AppSettings["FCP_USER"].ToSafeString();
        protected string _fbbFullConnPwd = ConfigurationManager.AppSettings["FCP_PWD"].ToSafeString();

        private string errorMsg = string.Empty;
        public StandardAddressReportJob(
            ILogger logger,
            IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }


        public void StandardAddressReport()
        {
            StartWatching();

            string filename = GetExcelName("FBSS_FULLCONN");

            //string tempPath = Path.GetTempPath();
            StdAddressFullConListResult StdAddressFullConResult = QueryBuild(1);
            _logger.Info("Get Data Success!.");
            _logger.Info("RESULT CODE = " + StdAddressFullConResult.Return_Code);
            _logger.Info("RESULT MSG = " + StdAddressFullConResult.Return_Desc);
            List<StdAddressFullConList> curData = StdAddressFullConResult.Data;

            if (curData != null && curData.Any())
            {
                _logger.Info("Conut Data = " + curData.Count);
                var count = 1;
                if (StdAddressFullConResult.Return_Code != 1 && StdAddressFullConResult.Return_Code != -1)
                {
                    bool datas = StdAddressFullConResult.Return_Code == 0 ? false : true;
                    var rowLenght = StdAddressFullConResult.Return_Code;
                    _logger.Info("result code = " + rowLenght);
                    while (datas)
                    {
                        StdAddressFullConListResult dataCon = QueryBuild(rowLenght);
                        if (dataCon.Data != null && dataCon.Data.Any())
                        {
                            curData = ConcatRow(curData, dataCon.Data);
                            rowLenght = dataCon.Return_Code;
                            if (dataCon.Return_Code == 0 || dataCon.Return_Code == 1 || dataCon.Return_Code == -1)
                            {
                                datas = false;
                            }
                            count++;
                        }
                        else
                        {
                            datas = false;
                            _logger.Info("Fatch null : " + rowLenght);
                        }

                    }
                    _logger.Info("Generate. " + count);
                    GenerateSTDEntitytoExcel<StdAddressFullConList>(curData, filename, "FULLCONNECTION");
                    StopWatching("Close!");
                    _logger.Info("== End ==");
                    return;
                }

            }
            GenerateSTDEntitytoExcel<StdAddressFullConList>(curData, filename, "FULLCONNECTION");
            StopWatching("Data is null!");
            _logger.Info("== End ==");

        }

        private StdAddressFullConListResult QueryBuild(int row)
        {
            var query = new STDFullConQuery();
            query.Return_Code = row;
            errorMsg = query.Return_Desc;
            return _queryProcessor.Execute(query);
        }

        private List<DirectoryList> GetDirectory()
        {
            var query = new GetDirectoryQuery();
            return _queryProcessor.Execute(query);
        }

        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");//HHmmss
            string dateNow = currDateTime.ToString("yyyyMMdd");//ddMMyyyy

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        private List<StdAddressFullConList> ConcatRow(List<StdAddressFullConList> first, List<StdAddressFullConList> sec)
        {
            if (sec.Any())
            {
                first.AddRange(sec);
            }
            return first;
        }

        public void GenerateSTDEntitytoExcel<T>(List<T> data, string fileName, string lovVal5)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            _logger.Info("Generate Excel!. : " + fileName + " lov = " + lovVal5);
            var groupHeaderList = new List<string>();
            var colHeaderList = new List<string>();

            string tmp = string.Empty;

            //var criterList = new List<LovValueModel>();
            SetColumnForExcel(lovVal5, ref table);
            _logger.Info("fetch data.");
            //Replace Value.
            object[] values = new object[props.Count];

            if (data != null && data.Any())
            {
                int count = 1;
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i.Equals(0))
                        {
                            values[i] = count++;
                            values[++i] = props[i - 1].GetValue(item);
                        }
                        else
                        {
                            values[i] = props[i - 1].GetValue(item);
                        }

                    }
                    table.Rows.Add(values);
                }
            }


            // Get current path
            //string fbbFullConnPath = GetCurrentPath(GetDirectory(), _fbbFullConnPath);
            _logger.Info("map patth. -> " + _fbbFullConnPath);
            GenerateExcelForStandAddress(table, "WorkSheet", fileName, lovVal5, (props.Count + 1));
            //GenerateDATForStandAddress(table, fileName);
        }


        public void SetColumnForExcel(string lovVal5, ref DataTable table)
        {
            string[] colHeader = null;
            string[] typeCol = null;

            int index = 0;
            var colHeaderList = new List<string>();
            var tmpVal = new List<LovValueModel>();
            var colVal = string.Empty;
            var dupVal = string.Empty;
            _logger.Info("call lov");
            var lovDataScreen = this.LovData.Where(p => p.Type == "FBBPAYG_SCREEN" && p.LovValue5 == lovVal5 && p.Name.Contains("R_HEADER_"))
                                .OrderBy(p => p.OrderBy).ToList();
            _logger.Info("lovDataScreen : " + lovDataScreen.Count);
            switch (lovVal5)
            {
                case "FULLCONNECTION":

                    colHeader = new string[] { "NO", "R_HEADER_region", "R_HEADER_olt_no", "R_HEADER_olt_port", "R_HEADER_odf_no", "R_HEADER_odf_port_out", "R_HEADER_site_no",
                                               "R_HEADER_sp1_no", "R_HEADER_sp1_port_out", "R_HEADER_sp1_laltitude", "R_HEADER_sp1_longitude", "R_HEADER_sp2_no",
                                               "R_HEADER_available_port", "R_HEADER_used_port", "R_HEADER_sp2_alias", "R_HEADER_sp2_laltitude", "R_HEADER_sp2_longitude",
                                               "R_HEADER_addr_id", "R_HEADER_addr_name_en", "R_HEADER_addr_name_th", "R_HEADER_sp2_create_dt"};

                    typeCol = new string[] { "System.String", "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String",
                                             "System.String", "System.String", "System.String", "System.String", "System.String" };

                    break;
            }

            if (colHeader != null && colHeader.Length != 0)
            {
                for (index = 0; index < colHeader.Length; index++)
                {
                    tmpVal = lovDataScreen.Where(p => p.Name == colHeader[index]).ToList();
                    if (tmpVal.Any())
                    {
                        colHeaderList.Add(tmpVal.FirstOrDefault().LovValue1.ToSafeString());
                    }
                    else
                    {
                        colHeaderList.Add(colHeader[index].ToSafeString());
                    }
                }
            }

            DataColumn col = new DataColumn();
            for (index = 0; index < colHeaderList.Count; index++)
            {
                colVal = colHeaderList[index].ToSafeString();
                if (!table.Columns.Contains(colVal))
                {
                    table.Columns.Add(colVal, System.Type.GetType(typeCol[index].ToSafeString()));
                }
                else
                {
                    dupVal += " ";
                    table.Columns.Add(colVal + dupVal, System.Type.GetType(typeCol[index].ToSafeString()));
                }
            }
        }

        private void GenerateExcelForStandAddress(DataTable dataToExcel, string excelSheetName, string fileName, string lovVal5, int columnCount)
        {
            CredentialHelper cdh = new CredentialHelper(_logger);
            bool result = cdh.WriteFileExcel(
                    _fbbFullConnUser,
                    _fbbFullConnPwd,
                    _fbbFullConnDomain,
                    _fbbFullConnPath,
                    fileName,
                    dataToExcel,
                    excelSheetName,
                    lovVal5,
                    columnCount
                );

            if (!result)
            {
                _logger.Info("Generate Excel => Fail!");
                return;
            }

            _logger.Info("Generate Excel => Success");
        }

        //private void GenerateDATForStandAddress(DataTable data, string fileName)
        //{
        //    CredentialHelper cdh = new CredentialHelper(_logger);
        //    bool result = cdh.WriteFile(
        //            _fbbFullConnUser,
        //            _fbbFullConnPwd,
        //            _fbbFullConnDomain,
        //            _fbbFullConnPath,
        //            fileName,
        //            data
        //         );

        //    if (!result){
        //        _logger.Info("Generate .dat => Fail!"); 
        //        return;
        //    }
        //    _logger.Info("Generate .dat => Success");

        //}

        public List<LovValueModel> LovData
        {
            get
            {
                var master = Bootstrapper.GetInstance<MasterData>();
                var lov = master.GetLovList("", "");
                return (List<LovValueModel>)lov;
            }
        }

        public string GetCurrentPath(List<DirectoryList> data, string path)
        {
            return data.Where(m => m.DIRECTORY_NAME == path).Select(m => m.DIRECTORY_PATH).SingleOrDefault();
        }

    }
}
