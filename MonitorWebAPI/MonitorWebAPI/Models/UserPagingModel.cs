using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Models
{
    public class UserPagingModel
    {
        public IQueryable users;
        public int userCount;
    }
}