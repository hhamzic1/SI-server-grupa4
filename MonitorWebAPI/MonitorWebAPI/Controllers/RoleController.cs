using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorWebAPI.Controllers
{
    [EnableCors("MonitorPolicy")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly monitorContext mc;
        public RoleController()
        {
            mc = new monitorContext();
        }

        [Route("api/role/GetRoleName")]
        [HttpGet]
        public IEnumerable<Role> GetAllRoles()
        {
            return mc.Roles;
        }

    }
}
