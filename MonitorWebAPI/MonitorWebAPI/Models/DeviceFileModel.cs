using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class DeviceFileModel
    {
        public string FileData { get; set; }
        public Guid DeviceUID { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }
    }
}
