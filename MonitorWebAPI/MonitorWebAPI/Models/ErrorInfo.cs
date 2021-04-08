using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class ErrorInfo
    {
        public DateTime ErrorTime { get; set; }
        public string Message { get; set; }
        public int? Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Guid DeviceUID { get; set; }
    }
}
