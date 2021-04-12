using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace MonitorWebAPI.Helpers
{
    public class BlobUpload
    {
        
        public static async System.Threading.Tasks.Task<string> UploadPDFAsync(string fileName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=si2021storage;AccountKey=PosXPJcHI+74hYfFB6U4uncozVIxQSCHfKrueABlsrxup1NKUqlX2l+CtaI/GbkUPZ86Qs9QwQkEnFDM/Lkx2A==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("si2021pdf");
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
