using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class Group
    {
        public Group()
        {
            DeviceGroups = new HashSet<DeviceGroup>();
            InverseParentGroupNavigation = new HashSet<Group>();
            UserGroups = new HashSet<UserGroup>();
        }

        public int GroupId { get; set; }
        public string Name { get; set; }
        public int? ParentGroup { get; set; }

        public virtual Group ParentGroupNavigation { get; set; }
        public virtual ICollection<DeviceGroup> DeviceGroups { get; set; }
        public virtual ICollection<Group> InverseParentGroupNavigation { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}
