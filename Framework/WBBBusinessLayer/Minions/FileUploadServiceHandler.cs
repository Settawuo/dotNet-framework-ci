using System.IO;
using WBBContract;
using WBBContract.Minions;
using WBBEntity.Minions;

namespace WBBBusinessLayer.Minions
{
    public class FileUploadServiceHandler : IQueryHandler<StreamQuery, UploadFileResponse>
    {
        public UploadFileResponse Handle(StreamQuery query)
        {
            var response = new UploadFileResponse();

            using (var fileStream = File.Create("teststream.VOB"))
            {
                query.stream.CopyTo(fileStream);
            }

            response.Message = "Successfully completed";
            return response;
        }
    }
}
