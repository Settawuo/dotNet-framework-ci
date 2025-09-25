using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using WBBEntity.Minions;

namespace WBBWebService
{
    [ServiceContract]
    public interface IFileUploadService
    {
        [OperationContract(IsOneWay = false)]
        FileDownloadReturnMessage DownloadFile(FileDownloadMessage request);

        [OperationContract(IsOneWay = true)]
        void UploadFile(FileUploadMessage request);

        [OperationContract]
        void AttemptToCloseStream(string authenticationKey);
    }
}