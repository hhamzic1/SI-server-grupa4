using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class UserTask
    {
        public UserTask()
        {
            UserTrackers = new HashSet<UserTracker>();
        }

        public int TaskId { get; set; }
        public int UserId { get; set; }
        public int? DeviceId { get; set; }
        public DateTime? StartTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime? EndTime { get; set; }
        public int StatusId { get; set; }

        public virtual Device Device { get; set; }
        public virtual TaskStatus Status { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserTracker> UserTrackers { get; set; }
    }
}
