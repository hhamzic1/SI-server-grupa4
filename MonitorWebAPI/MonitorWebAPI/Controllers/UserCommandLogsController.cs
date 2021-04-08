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
    public class UserCommandLogsController : Controller
    {
        private HelperMethods helperMethod;
        private readonly monitorContext mc;
        public UserCommandLogsController()
        {
            helperMethod = new HelperMethods();
            mc = new monitorContext();
        }

        [HttpPost]
        [Route("/api/user-comand-logs")]
        public async Task<ActionResult> SaveCommandLog([FromHeader] string Authorization, [FromBody] UserCommandsLog ucl)
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
                var userRoleName = mc.Roles.Where(x => x.RoleId == vu.roleId).FirstOrDefault().Name;
                if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && helperMethod.CheckIfDeviceBelongsToUsersTree(vu, ucl.DeviceId)))
                {
                    try
                    {
                        mc.UserCommandsLogs.Add(ucl);
                        await mc.SaveChangesAsync();
                        return Ok("Command log added successfully");
                    }
                    catch(Exception e)
                    {
                        return BadRequest("Log couldn't be added: " + e.Message);
                    }

                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
