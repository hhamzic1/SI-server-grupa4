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
    public class UserController : ControllerBase
    {
        private readonly monitorContext mc;
        public UserController()
        {
            mc = new monitorContext();
        }

        [Route("/api/hello-world")]
        [HttpGet]
        public ActionResult HelloWorld()
        {
            return Ok("Hello world");
        }


        [Route("api/user/Me")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<User>>> CurrentUser([FromHeader] string Authorization)
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
                return new ResponseModel<User>() { data = mc.Users.Where(x => x.UserId == vu.id).FirstOrDefault(), newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/user/MeExtendedInfo")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<User>>> CurrentUserExtendedInfo([FromHeader] string Authorization)
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
                User currentUser = mc.Users.Where(x => x.UserId == vu.id).FirstOrDefault();
                if(currentUser==null)
                {
                    return NotFound();
                }

                currentUser.UserGroups = mc.UserGroups.Where(x => x.UserId == currentUser.UserId).ToList();
                currentUser.Role = mc.Roles.Where(x => x.RoleId == currentUser.RoleId).FirstOrDefault();

                return new ResponseModel<User>() { data = currentUser, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/user/GetAllUsers")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IQueryable>>> GetAllUsers([FromHeader] string Authorization)
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
                var users = mc.Users
                     .Select(x => new {
                         x.Name,
                         x.Lastname,
                         x.Email,
                         x.UserId,
                         x.Phone,
                         x.Address
                     });
                return new ResponseModel<IQueryable>() { data = users, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }


        [Route("api/user/All")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<User>>>> GetAllUserTasks([FromHeader] string Authorization)
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
                if (mc.Roles.Find(vu.roleId).Name == "MonitorSuperAdmin")
                {
                    var users = mc.Users;
                    foreach (User user in users)
                    {
                        user.Password = null;
                        user.Role = null;
                    }
                    return new ResponseModel<IEnumerable<User>>() { data = users, newAccessToken = vu.accessToken };
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
