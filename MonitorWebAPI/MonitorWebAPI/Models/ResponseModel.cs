using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class ResponseModel<TValue>
    {
        public TValue data { get; set; }
        public string newAccessToken { get; set; }
    }
}
