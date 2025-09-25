using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetZipCodeAirQuery : IQuery<List<ZipCodeModel>>
    {
        public enum TypeName
        {
            Province = 0, Amphur = 1, Tumbon = 2
        }

        public TypeName Type { get; set; }

        public int CurrentCulture { get; set; }
        public string Regioncode { get; set; }

        private string _provinceFilter = string.Empty;
        public string ProvinceFilter
        {
            get { return _provinceFilter; }
            set { _provinceFilter = value; }
        }

        private string _amphurFilter = string.Empty;
        public string AmphurFilter
        {
            get { return _amphurFilter; }
            set { _amphurFilter = value; }
        }

        private string _tumbonFilter = string.Empty;
        public string TumbonFilter
        {
            get { return _tumbonFilter; }
            set { _tumbonFilter = value; }
        }
    }
}
