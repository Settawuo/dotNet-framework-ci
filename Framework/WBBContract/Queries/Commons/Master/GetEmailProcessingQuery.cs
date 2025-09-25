using WBBEntity.Models;

namespace WBBContract.Queries.Commons.Masters
{
    public class GetEmailProcessingQuery : IQuery<FBB_EMAIL_PROCESSING>
    {
        public string CreateBy { get; set; }
        public string ProcessName { get; set; }
    }
}
