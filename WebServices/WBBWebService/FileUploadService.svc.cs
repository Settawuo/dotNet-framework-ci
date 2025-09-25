//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Data.Services;
using System.Data.Services.Common;
using System.IO;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Activation;
using WBBEntity.Minions;

namespace WBBWebService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class FileUploadService : IFileUploadService
    {

        public FileDownloadReturnMessage DownloadFile(FileDownloadMessage request)
        {
            throw new System.NotImplementedException();
        }

        public void AttemptToCloseStream(string authenticationKey)
        {
            throw new System.NotImplementedException();
        }

        public void UploadFile(FileUploadMessage request)
        {
            Stream fileStream = null;
            Stream outputStream = null;
 
            try
            {
                fileStream = request.FileByteStream;

                string rootPath = @"D:\Temp";
 
                DirectoryInfo dirInfo = new DirectoryInfo(rootPath);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
 
                //Create the file in the filesystem - change the extension if you wish, or use a passed in value from metadata ideally
                string newFileName = Path.Combine(rootPath, Guid.NewGuid() + ".VOB");
 
                outputStream = new FileInfo(newFileName).OpenWrite();
                const int bufferSize = 1024;
                byte[] buffer = new byte[bufferSize];
 
                int bytesRead = fileStream.Read(buffer, 0, bufferSize);
 
                while (bytesRead > 0)
                {
                    outputStream.Write(buffer, 0, bufferSize);
                    bytesRead = fileStream.Read(buffer, 0, bufferSize);
                }
            }
            catch (IOException ex)
            {
                throw new FaultException<IOException>(ex, new FaultReason(ex.Message));
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                }
            }
        }
    }
}
