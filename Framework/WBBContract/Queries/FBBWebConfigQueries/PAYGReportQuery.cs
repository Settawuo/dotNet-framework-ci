using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    class PAYGReportQuery
    {
    }


    public class LastmileAndCPETransactionsQuery : IQuery<List<LastmileAndCPEReportList>>
    {
        public string oltbrand { get; set; }
        public string phase { get; set; }
        public string region { get; set; }
        public string product { get; set; }
        public string addressid { get; set; }

        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class DetailLastMileCPEQuery : IQuery<List<DetailLastmileAndCPEReportList>>
    {
        public string oltbrand { get; set; }
        public string phase { get; set; }
        public string region { get; set; }
        public string product { get; set; }
        public string addressid { get; set; }

        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int P_PAGE_INDEX { get; set; }
        public int P_PAGE_SIZE { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class DetailLastMilecpeQuery : IQuery<List<DetailLastmileAndCPEReportList>>
    {
        public string oltbrand { get; set; }
        public string phase { get; set; }
        public string region { get; set; }
        public string product { get; set; }
        public string addressid { get; set; }
        public List<string> address { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int P_PAGE_INDEX { get; set; }
        public int P_PAGE_SIZE { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
    /// <summary>
    /// Address id for query pkg
    /// </summary>
    public class DetailLastMilecpeAddressidListQuery : IQuery<List<DetailLastmileAndCPEReportList>>
    {
        public string oltbrand { get; set; }
        public string phase { get; set; }
        public string region { get; set; }
        public string product { get; set; }
        public string addressid { get; set; }
        public List<string> address { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int P_PAGE_INDEX { get; set; }
        public int P_PAGE_SIZE { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
    public class SupContractorReportQuery : IQuery<List<SupContractorReportList>>
    {
        public string ORG_ID { get; set; }
        public string SUB_CONTRACTOR_NAME_TH { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string PHASE { get; set; }
        public string REQUEST_BY { get; set; }
    }

    public class OLTQuery : IQuery<List<OLTList>>
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class OSPQuery : IQuery<List<OSPList>>
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class FBBPAYG_DropdownSUBSONTRACTQuery : IQuery<List<FBBPAYG_Dropdown>>
    {
        public string PackageName { get; set; }
        public string ProcName { get; set; }
        public string CurName { get; set; }

        public string LOV_NAME { get; set; }
        public string VALUE { get; set; }
    }

    public class FBBPAYG_SUMLASTMILEQuery : IQuery<List<FBBPAYG_Dropdown>>
    {
        public string ProcName { get; set; }
        public string Region { get; set; }

        public int LOV_NAME { get; set; }
        public string VALUE { get; set; }
    }
    public class UpdateScreenQuery : IQuery<List<UpdateScreenList>>
    {
        public string InternatNo { get; set; }
        public string PO { get; set; }
        public string Invoice { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public decimal CHKINDOOR { get; set; }
        public decimal CHKOUTDOOR { get; set; }
        public decimal CHKONT { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class PAYGLoadFilToTableQuery : IQuery<List<string>>
    {
        public string ErrorMessage { get; set; }
    }
    public class PAYGCheckLoadFileQuery : IQuery<List<string>>
    {
        public string ErrorMessage { get; set; }
    }

    public class StandardFullConQuery : IQuery<List<StandardFullConList>>
    {
        public string region { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
}