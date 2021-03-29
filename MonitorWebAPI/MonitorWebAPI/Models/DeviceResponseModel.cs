using System;

namespace MonitorWebAPI.Models
{
    public class DeviceResponseModel
    {
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public float LocationLongitude { get; set; }
        public float LocationLatitude { get; set; }
        public bool? Status { get; set; }
        public DateTime? LastTimeOnline { get; set; }
        public int? GroupId { get; set; }
        public string InstallationCode { get; set; }

    }
}
