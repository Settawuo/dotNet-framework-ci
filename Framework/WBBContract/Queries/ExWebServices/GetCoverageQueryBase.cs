using System.Diagnostics;

namespace WBBContract.Queries.ExWebServices
{
    [DebuggerDisplay("CVRID={CVRID}, FLAGONLINENUMBER={FLAGONLINENUMBER}, TECHNOLOGY={TECHNOLOGY}, REFF_KEY={REFF_KEY}, REFF_USER={REFF_USER}")]
    public class GetCoverageQueryBase
    {
        public int CVRID { get; set; }
        public string FLAGONLINENUMBER { get; set; }
        public string NETWORKTECHNOLOGY { get; set; }
        public string REFF_KEY { get; set; }
        public string REFF_USER { get; set; }
        public string TOWER { get; set; }
    }
}
