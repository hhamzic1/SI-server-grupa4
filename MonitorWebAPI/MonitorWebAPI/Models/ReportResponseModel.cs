using System;

namespace MonitorWebAPI.Models
{
    public class ReportResponseModel
    {
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public int UserId { get; set; }
    }
}
