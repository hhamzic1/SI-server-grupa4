using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class ReportInstance
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string UriLink { get; set; }
        public DateTime? Date { get; set; }

        //public virtual Report Report { get; set; }
    }
}
