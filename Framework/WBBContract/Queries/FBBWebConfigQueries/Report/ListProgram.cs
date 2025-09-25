using System;

namespace WBBContract.Queries.FBBWebConfigQueries.Report
{
    public class ListProgram
    {

        public string program_descriptiontext { get; set; }
        public string program_name { get; set; }
        public string program_Code { get; set; }
        public listProgramer programe_Description { get; set; }



        public class listProgramer
        {

            public String Programer_Code { get; set; }
            public String Progrmer_Desption { get; set; }

        }

    }
}
