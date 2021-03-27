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
        public async Task<ActionResult<ResponseModel<GroupHierarchyModel>>> MyAssignedGroups([FromHeader] string Authorization)
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
                    var helperMethod = new HelperMethods();
                    if (userRoleName == "SuperAdmin" && helperMethod.CheckIfGroupBelongsToUsersTree(vu, vu.groupId))
                    {
                        Group g = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault();
                        GroupHierarchyModel ghm = helperMethod.FindHierarchyTree(g);
                        return new ResponseModel<GroupHierarchyModel>() { data = ghm, newAccessToken = vu.accessToken };
                    }
                    else
                    {
                        return Forbid("You dont have access to this groups tree");
                    }
            }
            else
            {
                return Unauthorized("Token not valid");
            }
        }

        [Route("/api/group/GroupTreeById/{groupId}")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<GroupHierarchyModel>>> GroupTree([FromHeader] string Authorization, int groupId)
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
                var helperMethod = new HelperMethods();
                try
                {
                    if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && helperMethod.CheckIfGroupBelongsToUsersTree(vu, groupId)))
                    {
                        Group g = mc.Groups.Where(x => x.GroupId == groupId).FirstOrDefault();
                        GroupHierarchyModel ghm = helperMethod.FindHierarchyTree(g);
                        return new ResponseModel<GroupHierarchyModel>() { data = ghm, newAccessToken = vu.accessToken };
                    }
                    else
                    {
                        return Forbid("You dont have access to this groups tree");
                    }
                }
                catch(Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

    }



   
}
