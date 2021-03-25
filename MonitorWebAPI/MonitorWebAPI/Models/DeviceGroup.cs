using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class DeviceGroup
    {
        public int? DeviceId { get; set; }
        public int? GroupId { get; set; }
        public int Id { get; set; }

        public virtual Device Device { get; set; }
        public virtual Group Group { get; set; }
    }
}
