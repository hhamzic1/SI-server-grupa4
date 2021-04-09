using System;
using System.Collections.Generic;

namespace MonitorWebAPI.Models
{
    public class ReportInstanceResponseModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string UriLink { get; set; }
        public DateTime? Date { get; set; }
    }
}
