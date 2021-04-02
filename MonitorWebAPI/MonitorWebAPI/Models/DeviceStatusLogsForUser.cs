using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MonitorWebAPI.Models
{
    public class DeviceStatusLogsForUser
    {
        public double averageGPUUsage { get; set; }
        public double averageCPUUsage { get; set; }
        public double averageRamUsage { get; set; }
        public double averageHDDUsage { get; set; }
        public DeviceStatusLogsForUser(List<DeviceStatusLog> deviceStatusLogs)
        {
            this.averageCPUUsage = (double)deviceStatusLogs.Select(x => x.CpuUsage).DefaultIfEmpty(0).Average();
            this.averageGPUUsage = (double)deviceStatusLogs.Select(x => x.Gpuusage).DefaultIfEmpty(0).Average();
            this.averageRamUsage = (double)deviceStatusLogs.Select(x => x.RamUsage).DefaultIfEmpty(0).Average();
            this.averageHDDUsage = (double)deviceStatusLogs.Select(x => x.Hddusage).DefaultIfEmpty(0).Average();
        }
    }
}