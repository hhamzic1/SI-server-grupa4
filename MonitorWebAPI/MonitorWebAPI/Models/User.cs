using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class User
    {
        public User()
        {
            Reports = new HashSet<Report>();
            UserCommandsLogs = new HashSet<UserCommandsLog>();
            UserGroups = new HashSet<UserGroup>();
            UserTasks = new HashSet<UserTask>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public string Password { get; set; }
        public string QrSecret { get; set; }
        public string Address { get; set; }
        public bool? EmailVerified { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<UserCommandsLog> UserCommandsLogs { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<UserTask> UserTasks { get; set; }
    }
}
