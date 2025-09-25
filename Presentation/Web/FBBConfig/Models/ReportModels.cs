using System.ComponentModel.DataAnnotations.Schema;

namespace FBBConfig.Models
{

    [Table("Report")]
    public class Report
    {

        public string Region { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }


}
