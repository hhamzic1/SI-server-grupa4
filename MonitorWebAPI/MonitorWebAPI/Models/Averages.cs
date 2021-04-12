using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class Averages
    {
        public double CpuUsage { get; set; }
        public double RamUsage { get; set; }
        public double Hddusage { get; set; }
        public double Gpuusage { get; set; }
    }
}
