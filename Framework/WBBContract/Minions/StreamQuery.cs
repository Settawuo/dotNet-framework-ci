using System.IO;
using WBBEntity.Minions;

namespace WBBContract.Minions
{
    public class StreamQuery : IQuery<UploadFileResponse>
    {
        public Stream stream { get; set; }
    }
}
