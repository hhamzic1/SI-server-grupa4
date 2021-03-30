using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class ErrorDictionary
    {
        public ErrorDictionary()
        {
            ErrorLogs = new HashSet<ErrorLog>();
        }

        public int Id { get; set; }
        public int? Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
    }
}
