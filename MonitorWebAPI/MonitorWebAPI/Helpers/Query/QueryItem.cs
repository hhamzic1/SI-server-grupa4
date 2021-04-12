using MonitorWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Helpers.Query
{
    interface QueryItem
    {
        bool IsGroup();
        bool IsRule();
        bool Eval(AllInfoForDevice d);
    }
}
