using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MonitorWebAPI.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadFileBlobAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("si2021pdf");
            string localPath = "./data/";
            string localFilePath = Path.Combine(localPath, fileName);
            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
            return "https://si2021storage.blob.core.windows.net/si2021pdf/" + fileName;
        }
    }
}
