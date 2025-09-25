using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class DormitoryReportQuery
    {
    }

    public class GetCustNotRegisQuery : IQuery<List<CusNotRegisList>>
    {
        public Nullable<DateTime> dateFrom { get; set; }
        public Nullable<DateTime> dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

    }

    public class GetOverviewStatusQuery : IQuery<List<OverviewStatusList>>
    {
        public Nullable<DateTime> os_DateFrom { get; set; }
        public Nullable<DateTime> os_DateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class GetSumInstallQuery : IQuery<List<SumInstallPerformanceList>>
    {
        public Nullable<DateTime> dateFrom { get; set; }
        public Nullable<DateTime> dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class GetCheckDuplicateMobileQuery : IQuery<List<DuplicateMobileList>>
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class GetCusInstallTrackQuery : IQuery<List<CusInstallTrackList>>
    {
        public Nullable<DateTime> dateFrom { get; set; }
        public Nullable<DateTime> dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class GetExtractDormitoryMaster : IQuery<List<string>>
    {
        public string ErrorMessage { get; set; }
    }

    public class GetExtractDormitoryDTL : IQuery<List<string>>
    {
        public string ErrorMessage { get; set; }
    }

    public class GetExtractDormitoryZipCode : IQuery<List<string>>
    {
        public string ErrorMessage { get; set; }
    }
}
