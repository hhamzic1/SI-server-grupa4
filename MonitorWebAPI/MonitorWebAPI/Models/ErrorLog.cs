using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int DeviceId { get; set; }
        public int ErrorTypeId { get; set; }
        public DateTime ErrorTime { get; set; }

        public virtual Device Device { get; set; }
        public virtual ErrorDictionary ErrorType { get; set; }
    }
}
