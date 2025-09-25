using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace WBBWebService
{
    [MessageContract]
    public class FileUploadMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string AuthenticationKey;
        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
    }

    [MessageContract]
    public class FileDownloadMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string AuthenticationKey;
    }

    [MessageContract]
    public class FileDownloadReturnMessage
    {
        public FileDownloadReturnMessage(Stream stream)
        {
            this.FileByteStream = stream;
        }

        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
    }
}