using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class Device
    {
        public Device()
        {
            DeviceGroups = new HashSet<DeviceGroup>();
            ErrorLogs = new HashSet<ErrorLog>();
            UserCommandsLogs = new HashSet<UserCommandsLog>();
            UserTasks = new HashSet<UserTask>();
        }

        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public float LocationLongitude { get; set; }
        public float LocationLatitude { get; set; }
        public bool Status { get; set; }
        public DateTime LastTimeOnline { get; set; }
        public string InstallationCode { get; set; }
        public Guid DeviceUid { get; set; }

        public virtual ICollection<DeviceGroup> DeviceGroups { get; set; }
        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
        public virtual ICollection<UserCommandsLog> UserCommandsLogs { get; set; }
        public virtual ICollection<UserTask> UserTasks { get; set; }
    }
}
