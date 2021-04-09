using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class DeviceErrorInfo
    {
        public Guid DeviceUID { get; set; }
        public int ErrorNumber { get; set; }
        public List<ErrorInfo> errorInfo { get; set; }
    }
}
