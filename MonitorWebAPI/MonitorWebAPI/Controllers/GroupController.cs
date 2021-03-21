using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace MonitorWebAPI.Controllers
{
    [EnableCors("MonitorPolicy")]
    [ApiController]
    public class GroupController : ControllerBase
    {

        private readonly monitorContext mc;
        public GroupController()
        {
            mc = new monitorContext();
        }

        [Route("api/group/MyGroup")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<Group>>> MyGroup([FromHeader] string Authorization)
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
                return new ResponseModel<Group>() { data = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault(), newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("/api/group/MyAssignedGroups")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<Group>>>> MyAssignedGroups([FromHeader] string Authorization)
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
                if (userRoleName == "MonitorSuperAdmin")
                {
                    return new ResponseModel<List<Group>>() { data = mc.Groups.ToList(), newAccessToken = vu.accessToken };
                }
                else if (userRoleName == "SuperAdmin")
                {
                    List<Group> subGroups = mc.Groups.Where(x => x.ParentGroup == vu.groupId).ToList();
                    subGroups.Add(mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault());
                    return new ResponseModel<List<Group>>() { data = subGroups, newAccessToken = vu.accessToken };
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }



   
}
