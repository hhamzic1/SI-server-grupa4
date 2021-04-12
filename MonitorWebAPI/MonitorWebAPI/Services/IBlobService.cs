using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace MonitorWebAPI.Services
{
    public interface IBlobService
    {
        public Task<string> UploadFileBlobAsync(string fileName);
    }
}
