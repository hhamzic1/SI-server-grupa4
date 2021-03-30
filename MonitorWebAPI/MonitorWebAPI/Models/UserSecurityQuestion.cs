using System;
using System.Collections.Generic;

#nullable disable

namespace MonitorWebAPI.Models
{
    public partial class UserSecurityQuestion
    {
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
