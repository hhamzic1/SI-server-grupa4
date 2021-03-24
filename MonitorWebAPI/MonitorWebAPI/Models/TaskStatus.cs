using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class TaskStatus
    {
        public TaskStatus()
        {
            UserTasks = new HashSet<UserTask>();
        }

        public int StatusId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserTask> UserTasks { get; set; }
    }
}
