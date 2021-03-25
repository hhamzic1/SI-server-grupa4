using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class Report
    {
        public int ReportId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public string Frequency { get; set; }
        public string Query { get; set; }

        public virtual User User { get; set; }
    }
}
