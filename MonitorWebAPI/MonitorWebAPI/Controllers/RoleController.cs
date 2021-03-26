using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        [Route("api/role/GetRoles")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<Role>>>> GetAllRoles([FromHeader] string Authorization)
        {
            string JWT = JWTVerify.GetToken(Authorization);
            if (JWT == null)
            {
                return Unauthorized();
            }
            HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);
                return new ResponseModel<List<Role>>() { data = mc.Roles.ToList(), newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
