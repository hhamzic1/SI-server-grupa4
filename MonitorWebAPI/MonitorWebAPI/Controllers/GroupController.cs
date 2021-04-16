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
        private HelperMethods helperMethod;
        public GroupController()
        {
            mc = new monitorContext();
            helperMethod = new HelperMethods();
        }

        [Route("api/group/MyGroup")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<Models.Group>>> MyGroup([FromHeader] string Authorization)
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
                return new ResponseModel<Models.Group>() { data = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault(), newAccessToken = vu.accessToken };
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
                    if (userRoleName == "SuperAdmin" && helperMethod.CheckIfGroupBelongsToUsersTree(vu, vu.groupId))
                    {
                    Group g = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault();
                        GroupHierarchyModel ghm = helperMethod.FindHierarchyTree(g);
                        return new ResponseModel<GroupHierarchyModel>() { data = ghm, newAccessToken = vu.accessToken };
                    }
                    else
                    {
                        return StatusCode(403);
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
                try
                {
                    if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && helperMethod.CheckIfGroupBelongsToUsersTree(vu, groupId)))
                    {
                        Models.Group g = mc.Groups.Where(x => x.GroupId == groupId).FirstOrDefault();
                        GroupHierarchyModel ghm = helperMethod.FindHierarchyTree(g);
                        return new ResponseModel<GroupHierarchyModel>() { data = ghm, newAccessToken = vu.accessToken };
                    }
                    else
                    {
                        return StatusCode(403);
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

        [Route("/api/group/CreateGroup")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<Models.Group>>> CreateGroup([FromHeader] string Authorization, [FromBody] Models.Group group)
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
                try
                {
                    if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && helperMethod.CheckIfGroupBelongsToUsersTree(vu, group.ParentGroup)))
                    {
                        mc.Groups.Add(group);
                        await mc.SaveChangesAsync();
                        int id = group.GroupId;

                        Models.Group tempGroup = mc.Groups.Where(x => x.GroupId == id).FirstOrDefault();
                        if (tempGroup != null)
                        {
                            var deviceGroups = mc.DeviceGroups.Where(x => x.GroupId == tempGroup.ParentGroup).ToList();
                            foreach (var temp in deviceGroups)
                            {
                                temp.GroupId = id;
                                mc.DeviceGroups.Attach(temp).Property(y => y.GroupId).IsModified = true;
                                mc.SaveChanges();
                            }
                            return new ResponseModel<Models.Group>() { data = tempGroup, newAccessToken = vu.accessToken };
                        }
                        throw new Exception("Group wasn't added succesfully");
                    }
                    else
                    {
                        return StatusCode(403);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut("/api/group/{groupId}")]
        public async Task<ActionResult<ResponseModel<Models.Group>>> PutGroup(int groupId, [FromBody] Models.Group newGroup, [FromHeader] string Authorization)
        {
            string JWT = JWTVerify.GetToken(Authorization);
            if (JWT == null)
            {
                return Unauthorized();
            }
            HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);
                    var userRoleName = mc.Roles.Where(x => x.RoleId == vu.roleId).FirstOrDefault().Name;
                    if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && helperMethod.CheckIfGroupBelongsToUsersTree(vu, groupId)))
                    {
                        Models.Group group = mc.Groups.Where(x => x.GroupId == groupId).FirstOrDefault();
                        if (group == null)
                        {
                            throw new Exception("Group with that id doesn't exist");
                        }
                        mc.Groups.Attach(group);
                        group.Name = newGroup.Name;
                        mc.SaveChanges();
                        return new ResponseModel<Models.Group>() { data = group, newAccessToken = vu.accessToken };
                    }
                    else
                    {
                        return StatusCode(403, "You don't have access to this group!");
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(404,"Group name wasn't changed successfully. "+e.Message);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

    }



   
}
