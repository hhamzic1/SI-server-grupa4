using System;
using System.Collections.Generic;

namespace MonitorWebAPI.Models
{
    public class ReportResponseModel
    {
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public string Frequency { get; set; }
        public DateTime NextDate { get; set; }
        public int UserId { get; set; }
        public ICollection<ReportInstance> ReportInstances { get; set; }
    }
}
