using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class ImageRequestModel
    {
        public string machineUid { get; set; }
        public string taskId { get; set; }
    }
}
