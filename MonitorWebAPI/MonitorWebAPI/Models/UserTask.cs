using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class UserTask
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public int? DeviceId { get; set; }
        public DateTime? Time { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public virtual Device Device { get; set; }
        public virtual User User { get; set; }
    }
}
