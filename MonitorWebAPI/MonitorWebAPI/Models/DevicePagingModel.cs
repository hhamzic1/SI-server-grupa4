using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class DevicePagingModel
    {
        public List<DeviceResponseModel> Devices { get; set; }
        public int DeviceCount { get; set; }
    }
}
