using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class ConfigJS
    {

        public string name { get; set; }
        public string location { get; set; }
        public Guid deviceUid { get; set; }
        public double keepAlive { get; set; }
        public string webSocketUrl { get; set; }
        public string pingUri { get; set; }
        public string mainUri { get; set; }

        public string errorUri { get; set; }

        public string fileUri { get; set; }

        public string installationCode { get; set; }
        public string path { get; set; }

        public class FileInfo 
        {
            public string path { get; set; }
            public int minutes { get; set; }

        }

        public class FileLocations
        {
            public FileInfo File1 { get; set; }
            public FileInfo File2 { get; set; }
            public FileInfo File3 { get; set; }
            public FileInfo File4 { get; set; }
            public FileInfo File5 { get; set; }
        }


        public FileLocations fileLocations { get; set; }

    }
}




