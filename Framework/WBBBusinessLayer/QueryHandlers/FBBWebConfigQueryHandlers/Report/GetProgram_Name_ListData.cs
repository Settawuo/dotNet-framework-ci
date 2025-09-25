using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.Report;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.Report
{
    public class GetProgram_Name_ListData : IQueryHandler<GetPrograme_Name, List<ListProgram>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_PROGRAM> _FBB_PROGRAM;

        public GetProgram_Name_ListData(ILogger logger
         , IEntityRepository<FBB_PROGRAM> FBB_PROGRAM)
        {
            _logger = logger;
            _FBB_PROGRAM = FBB_PROGRAM;
        }

        public List<ListProgram> Handle(GetPrograme_Name query)
        {

            var list2 = (from p in _FBB_PROGRAM.Get()
                         where p.PROGRAM_CODE == query.Progrmer_Code
                         select new ListProgram
                         {
                             program_name = p.PROGRAM_NAME,
                             program_descriptiontext = p.PROGRAM_DESCRIPTION

                         });


            return list2.ToList(); ;


        }

    }
}
