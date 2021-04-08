using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class UserCommandsLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DeviceId { get; set; }
        public string Command { get; set; }
        public string Response { get; set; }
        public DateTime Time { get; set; }

        public virtual Device Device { get; set; }
        public virtual User User { get; set; }
    }
}
