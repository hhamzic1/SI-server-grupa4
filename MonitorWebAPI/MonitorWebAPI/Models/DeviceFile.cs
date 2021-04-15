using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class DeviceFile
    {
        public int FileId { get; set; }
        public byte[] FileData { get; set; }
        public int DeviceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }

        public virtual Device Device { get; set; }
    }
}
