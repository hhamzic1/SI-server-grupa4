using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class Component
    {
        public int ComponentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int TaskId { get; set; }

        public virtual UserTask Task { get; set; }
    }
}
