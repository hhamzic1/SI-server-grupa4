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

        [HttpGet]
        [Route("/api/user-command-logs/CommandLogsForDevice")]
        public async Task<ActionResult<ResponseModel<List<UserCommandsLog>>>> GetAllCommandLogsForDevice([FromHeader] string Authorization, [FromQuery] int deviceId) {
            string JWT = JWTVerify.GetToken(Authorization);
            if (JWT == null) {
                return Unauthorized();
            }
            HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
            if (!response.IsSuccessStatusCode) {
                return Unauthorized();
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);

            if (checkIfDeviceWithIdExists(deviceId)) {
                List<UserCommandsLog> listLogs = mc.UserCommandsLogs.Where(x => x.DeviceId == deviceId).ToList();

                Device tempDev = mc.Devices.Where(x => x.DeviceId == deviceId).FirstOrDefault();
                foreach (UserCommandsLog item in listLogs) {
                    User tempUser = mc.Users.Where(x => x.UserId == item.UserId).FirstOrDefault();
                    item.User = tempUser;
                    item.Device = tempDev;
                }

                return new ResponseModel<List<UserCommandsLog>>() {
                    data = listLogs,
                    newAccessToken = vu.accessToken
                };
            }
            return BadRequest("Device with said id does not exist");
        }

        [HttpGet]
        [Route("api/user-command-logs/CommandLogsForDeviceAndUser")]
        public async Task<ActionResult<ResponseModel<List<UserCommandsLog>>>> GetAllCommandLogsForDeviceAndUser([FromHeader] string Authorization, [FromQuery] int deviceId, [FromQuery] int userId) {
            string JWT = JWTVerify.GetToken(Authorization);
            if (JWT == null) {
                return Unauthorized();
            }
            HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
            if (!response.IsSuccessStatusCode) {
                return Unauthorized();
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);

            string message = "";
            if (checkIfDeviceWithIdExists(deviceId)) {

                if (checkIfUserWithIdExists(userId)) {
                    List<UserCommandsLog> listLogs = mc.UserCommandsLogs.Where(x => x.DeviceId == deviceId)
                                                    .Where(x => x.UserId == userId)
                                                    .ToList();

                    Device tempDev = mc.Devices.Where(x => x.DeviceId == deviceId).FirstOrDefault();
                    User tempUser = mc.Users.Where(x => x.UserId == userId).FirstOrDefault();
                    foreach (UserCommandsLog item in listLogs) {
                        item.User = tempUser;
                        item.Device = tempDev;
                    }

                    return new ResponseModel<List<UserCommandsLog>>() {
                        data = listLogs,
                        newAccessToken = vu.accessToken
                    };

                } else {
                    message = "User with said id does not exist";
                }

            } else {
                message = "Device with said id does not exist";
            }
            return BadRequest(message);
        }


        private bool checkIfDeviceWithIdExists(int id) {
            return mc.Devices.Where(x => x.DeviceId == id).FirstOrDefault() != null;
        }

        private bool checkIfUserWithIdExists(int id) {
            return mc.Users.Where(x => x.UserId == id).FirstOrDefault() != null;
        }
    }
}
