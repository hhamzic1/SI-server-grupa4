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
        private HelperMethods helperMethod;
        public UserController()
        {
            mc = new monitorContext();
            helperMethod = new HelperMethods();
        }

        [Route("/api/hello-world")]
        [HttpGet]
        public ActionResult HelloWorld()
        {
            return Ok("Hello world");
        }

        [Route("/api/uploadtest")]
        [HttpGet]
        public ActionResult UploadTestRoute()
        {
            return Ok(helperMethod.SendEmailTest(2));
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
        public async Task<ActionResult<ResponseModel<UserPagingModel>>> GetAllUsers([FromHeader] string Authorization, [FromQuery] bool? enable_pagination, [FromQuery] int? page, [FromQuery] int? per_page, [FromQuery] string? name, [FromQuery] string? last_name, [FromQuery] string? email, [FromQuery] string? adress, [FromQuery] string? sort_by)
        {
            string JWT = JWTVerify.GetToken(Authorization);
            bool parsedEP = enable_pagination == null ? false : (bool)enable_pagination;
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
                         .Select(x => new
                         {
                             x.Name,
                             x.Lastname,
                             x.Email,
                             x.UserId,
                             x.Phone,
                             x.Address,
                             x.RoleId,
                             groupId = (from uGroup in mc.UserGroups where uGroup.UserId == x.UserId select uGroup.GroupId).ToList()
                         });
                if (!parsedEP)
                {
                    return new ResponseModel<UserPagingModel>() { data = new UserPagingModel { users = users, userCount = users.ToArray().Length }, newAccessToken = vu.accessToken };
                }
                else
                {
                    int parsedPage = page == null ? 1 : (int)page;
                    int parsedPerPage = per_page == null ? 10 : (int)per_page;
                    int skip = (parsedPage - 1) * parsedPerPage;
                    string parsedName = name == null ? "" : name;
                    string parsedLastName = last_name == null ? "" : last_name;
                    string parsedEmail = email == null ? "" : email;
                    string parsedAdress = adress == null ? "" : adress;
                    string parsedSortBy = sort_by == null ? "" : sort_by;
                    var filteredList = users;
                    if (name != null || last_name != null || email != null || adress != null)
                    {
                        filteredList = filteredList.Where(x => (name != null && x.Name.ToLower().Contains(parsedName.ToLower()))
                                                            || (last_name != null && x.Lastname.ToLower().Contains(parsedLastName.ToLower()))
                                                            || (email != null && x.Email.ToLower().Contains(parsedEmail.ToLower()))
                                                            || (adress != null && x.Address.ToLower().Contains(parsedAdress.ToLower())));
                    }
                    switch (parsedSortBy)
                    {
                        case "name_asc":
                            filteredList = filteredList.OrderBy(x => x.Name);
                            break;
                        case "name_desc":
                            filteredList = filteredList.OrderByDescending(x => x.Name);
                            break;
                        case "lastname_asc":
                            filteredList = filteredList.OrderBy(x => x.Lastname);
                            break;
                        case "lastname_desc":
                            filteredList = filteredList.OrderByDescending(x => x.Lastname);
                            break;
                        case "email_asc":
                            filteredList = filteredList.OrderBy(x => x.Email);
                            break;
                        case "email_desc":
                            filteredList = filteredList.OrderByDescending(x => x.Email);
                            break;
                        case "adress_asc":
                            filteredList = filteredList.OrderBy(x => x.Address);
                            break;
                        case "adress_desc":
                            filteredList = filteredList.OrderByDescending(x => x.Address);
                            break;
                        default:
                            filteredList = filteredList.OrderBy(x => x.UserId);
                            break;
                    }
                    int userCount = filteredList.ToArray().Length;
                    filteredList = filteredList.Skip(skip).Take(parsedPerPage);
                    return new ResponseModel<UserPagingModel>() { data = new UserPagingModel { users = filteredList, userCount = userCount }, newAccessToken = vu.accessToken };
                }
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
                    return Forbid();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
