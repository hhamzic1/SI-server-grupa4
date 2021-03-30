using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class Report
    {
        public Report()
        {
            ReportInstances = new HashSet<ReportInstance>();
        }

        public int ReportId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime NextDate { get; set; }
        public string Frequency { get; set; }
        public string Query { get; set; }
        public bool? SendEmail { get; set; }
        public bool? Deleted { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<ReportInstance> ReportInstances { get; set; }
    }
}
