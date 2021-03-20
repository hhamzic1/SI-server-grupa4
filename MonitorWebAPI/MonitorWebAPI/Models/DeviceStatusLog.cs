using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class DeviceStatusLog
    {
        public DateTime TimeStamp { get; set; }
        public int DeviceId { get; set; }
        public string Message { get; set; }

        public virtual Device Device { get; set; }
    }
}
