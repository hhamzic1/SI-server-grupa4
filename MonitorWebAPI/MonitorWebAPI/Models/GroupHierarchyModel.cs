using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class GroupHierarchyModel
    {
        public int? GroupId { get; set; }
        public string Name { get; set; }
        public List<GroupHierarchyModel> SubGroups { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
    }
}
