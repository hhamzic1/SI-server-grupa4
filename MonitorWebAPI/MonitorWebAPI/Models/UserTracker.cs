using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class UserTracker
    {
        public int Id { get; set; }
        public int UserTaskId { get; set; }
        public double LocationLongitutde { get; set; }
        public double LocationLatitude { get; set; }
        public DateTime Time { get; set; }

        public virtual UserTask UserTask { get; set; }
    }
}
