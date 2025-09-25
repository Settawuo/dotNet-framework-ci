using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FbbIqsFoaModel
    {
        public FbbIqsFoaModel()
        {
            RecordCount = 0;
            WriteFileResult = false;
            SendEmailResult = false;
            FullPathName = "";
            FileName = "";
            Data = new List<FbbIqsFoaData>();
            ReturnCode = -1;
            ReturnDesc = "";
        }

        public decimal RecordCount { get; set; }

        public bool WriteFileResult { get; set; }

        public bool SendEmailResult { get; set; }

        public string FullPathName { get; set; }

        public string FileName { get; set; }

        public List<FbbIqsFoaData> Data { get; set; }

        public decimal ReturnCode { get; set; }

        public string ReturnDesc { get; set; }
    }

    public class FbbIqsFoaData
    {
        public decimal ROWNUMBER { get; set; }

        public string RAWDATA { get; set; }

        public string GENERATE_DATE { get; set; }
    }

}
