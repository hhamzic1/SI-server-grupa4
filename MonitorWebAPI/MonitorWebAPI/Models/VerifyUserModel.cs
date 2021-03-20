using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class VerifyUserModel
    {
        public int id { get; set; }
        public string email { get; set; }
        public int roleId { get; set; }
        public int? groupId { get; set; }
        public string accessToken {get; set;}

    }
}
