
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.Commons.Master
{  
    public class GenPassIANoQuery : IQuery<GenpassIaNoModel>
    {
        public string no { get; set; }
    }
}
