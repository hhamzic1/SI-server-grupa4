using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class AllInfoForDevice
    {
        public Device Device { get; set; }
        public Averages Averages { get; set; }
        public string GroupName { get; set; }

    }
}
