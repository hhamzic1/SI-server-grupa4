using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class DeviceController : ControllerBase
    {
        private readonly monitorContext mc;
        private readonly string SUPER_ADMIN = "SuperAdmin";
        private readonly string MONITOR_SUPER_ADMIN = "MonitorSuperAdmin";
        public DeviceController()
        {
            mc = new monitorContext();
        }

        [Route("api/device/AllDevices")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<Device>>>> GetAllDevices([FromHeader] string Authorization)
        {
            string JWT = JWTVerify.GetToken(Authorization);
            if (JWT == null)
            {
                return Unauthorized();
            }
            HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
            if(response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);
                var userRoleName = mc.Roles.Where(x => x.RoleId == vu.roleId).FirstOrDefault().Name;
                if(userRoleName=="MonitorSuperAdmin")
                {
                    
                    return new ResponseModel<List<Device>>() { data = mc.Devices.ToList(), newAccessToken = vu.accessToken };
                }
                else
                {
                    List<Device> allDevices = mc.Devices.ToList();
                    HelperMethods hm = new HelperMethods();
                    List<Device> myDevices = new List<Device>();

                    foreach(Device dev in allDevices)
                    {
                        try
                        {
                            if (hm.CheckIfDeviceBelongsToUsersTree(vu, dev.DeviceId))
                            {
                                myDevices.Add(dev);
                            }
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }

                    }

                    return new ResponseModel<List<Device>>() { data = myDevices, newAccessToken = vu.accessToken };
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/device/CreateDevice")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<Device>>> CreateDevice([FromBody] Device device, [FromQuery] int groupId, [FromHeader] string Authorization)
        {
            string JWT = JWTVerify.GetToken(Authorization);
            if(JWT==null)
            {
                return Unauthorized();
            }
            HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);
                HelperMethods helperMethod = new HelperMethods();
                var userRoleName = mc.Roles.Where(x => x.RoleId == vu.roleId).FirstOrDefault().Name;
                try
                {
                    if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && helperMethod.CheckIfGroupBelongsToUsersTree(vu, groupId)))
                    {
                        device.Status = true;
                        device.LastTimeOnline = DateTime.Now;

                        mc.Devices.Add(device);
                        await mc.SaveChangesAsync();
                        Device tempDevice = mc.Devices.Where(x => x.Name == device.Name && x.Location == device.Location).FirstOrDefault();
                        if (tempDevice != null)
                        {
                            DeviceGroup dg = new DeviceGroup() { DeviceId = tempDevice.DeviceId, GroupId = groupId };
                            mc.DeviceGroups.Add(dg);
                            await mc.SaveChangesAsync();
                            return new ResponseModel<Device>() { data = tempDevice, newAccessToken = vu.accessToken };
                        }
                        throw new Exception("Device wasn't added succesfully");
                    }
                    else
                    {
                        return StatusCode(403);
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message +"\n" +e.InnerException);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/device/AllDevicesForGroup")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<DevicePagingModel>>> AllDevicesForGroup([FromQuery] int? page, [FromQuery] int? per_page, [FromQuery] string? name, [FromQuery] string? status, [FromQuery] int groupId, string? sort_by, [FromHeader] string Authorization)
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
                        if (g == null)
                        {
                            return NotFound();
                        }
                        GroupHierarchyModel ghm = helperMethod.FindHierarchyTreeWithDevices(g);
                        List<Device> allDevices = ghm.Devices.ToList();

                        List<DeviceResponseModel> drmList = new List<DeviceResponseModel>();
                        foreach (var dev in allDevices)
                        {
                            drmList.Add(new DeviceResponseModel()
                            {
                                DeviceId = dev.DeviceId,
                                Name = dev.Name,
                                Location = dev.Location,
                                LocationLatitude = dev.LocationLatitude,
                                LocationLongitude = dev.LocationLongitude,
                                Status = dev.Status,
                                LastTimeOnline = dev.LastTimeOnline,
                                GroupId = (from x in mc.DeviceGroups.OfType<DeviceGroup>() where x.DeviceId == dev.DeviceId select x.GroupId).FirstOrDefault()
                            });
                        }

                        int parsedPage = (page == null || page <= 0) ? 1 : (int)page;
                        int parsedPerPage = (per_page == null || per_page <= 0) ? 10 : (int)per_page;
                        string parsedName = name == null ? "" : name;
                        string parsedSortBy = sort_by == null ? "" : sort_by;
                        int skip = (parsedPage - 1) * parsedPerPage;
                        var filteredList = drmList.Skip(skip).Take(parsedPerPage).ToList();
                        bool onlineStatus = true;
                        if (status == "active") onlineStatus = true;
                        else if (status == "notactive") onlineStatus = false;

                        filteredList = filteredList.FindAll(x => x.Name.Contains(parsedName)).FindAll(s => s.Status == onlineStatus).ToList();
                        switch (parsedSortBy)
                        {
                            case "name_asc":
                                filteredList = filteredList.OrderBy(x => x.Name).ToList();
                                break;
                            case "name_desc":
                                filteredList = filteredList.OrderByDescending(x => x.Name).ToList();
                                break;
                            case "location_asc":
                                filteredList = filteredList.OrderBy(x => x.Location).ToList();
                                break;
                            case "location_desc":
                                filteredList = filteredList.OrderByDescending(x => x.Location).ToList();
                                break;
                            case "status_asc":
                                filteredList = filteredList.OrderBy(x => x.Status).ToList();
                                break;
                            case "status_desc":
                                filteredList = filteredList.OrderByDescending(x => x.Status).ToList();
                                break;
                            default:
                                filteredList = filteredList.OrderBy(x => x.DeviceId).ToList();
                                break;
                        }

                        return new ResponseModel<DevicePagingModel>() { data = new DevicePagingModel() { Devices=filteredList, DeviceCount=allDevices.Count}, newAccessToken = vu.accessToken };
                    }
                    else
                    {
                        return StatusCode(403);
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/device/GetDeviceLogs")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<DeviceStatusLogsWithAverageHardwareUsage>>> GetDeviceLogs([FromHeader] String Authorization, [FromQuery] int deviceId, [FromQuery] String? startDate, [FromQuery] String? endDate)
        {
            String JWT = JWTVerify.GetToken(Authorization);
            if (JWT == null)
            {
                return Unauthorized();
            }

            HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }

            String responseBody = await response.Content.ReadAsStringAsync();
            VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);
            var userRoleName = mc.Roles.Where(x => x.RoleId == vu.roleId).FirstOrDefault().Name;
            HelperMethods hm = new HelperMethods();

            try {
                if (userRoleName == MONITOR_SUPER_ADMIN || (userRoleName == SUPER_ADMIN && hm.CheckIfDeviceBelongsToUsersTree(vu, deviceId))) {
                    //if startDate isn't passed startDateParsed is set to the old times to get all device logs for passed device
                    //if endDate isn't passed endDateParsed is set to now to get all device logs for passed device
                    DateTime startDateParsed = startDate != null ? DateTime.Parse(startDate) : DateTime.Now.AddYears(-1500);
                    DateTime endDateParsed = endDate != null ? DateTime.Parse(endDate) : DateTime.Now;

                    List<DeviceStatusLog> dslList = mc.DeviceStatusLogs
                        .Where(x => x.DeviceId == deviceId)
                        .ToList()
                        .Where(x => DateTime.Compare(x.TimeStamp, startDateParsed) >= 0 && DateTime.Compare(x.TimeStamp, endDateParsed) <= 0)
                        .ToList();

                    DeviceStatusLogsWithAverageHardwareUsage returnData = new DeviceStatusLogsWithAverageHardwareUsage(dslList);
                    return new ResponseModel<DeviceStatusLogsWithAverageHardwareUsage>() { data = returnData, newAccessToken = vu.accessToken };
                }

            } catch (NullReferenceException e) {
                return NotFound(e.Message);
            } catch(FormatException) {
                return BadRequest("startDate and/or endDate are in wrong DateTime format. Use yyyy-MM-dd HH:mm:ss");
            }
            return StatusCode(403);
        }
    }
}
