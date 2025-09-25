using System.Collections.Generic;
using WBBEntity.PanelModels;

//From Config Query Report - WBBContract
namespace WBBContract.Queries.FBBWebConfigQueries.ReportPortAssignment
{
    public class GetNodeIDQuery : IQuery<List<DropdownModel>>
    {
        public string NodeNameTH { get; set; }
    }
}
