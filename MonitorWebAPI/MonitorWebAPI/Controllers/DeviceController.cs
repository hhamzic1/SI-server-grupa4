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
        private HelperMethods helperMethod;
        public DeviceController()
        {
            mc = new monitorContext();
            helperMethod = new HelperMethods();
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
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);
                var userRoleName = mc.Roles.Where(x => x.RoleId == vu.roleId).FirstOrDefault().Name;
                if (userRoleName == "MonitorSuperAdmin")
                {

                    return new ResponseModel<List<Device>>() { data = mc.Devices.ToList(), newAccessToken = vu.accessToken };
                }
                else
                {
                    List<Device> allDevices = mc.Devices.ToList();
                    HelperMethods hm = new HelperMethods();
                    List<DeviceResponseModel> myDevicesResponseModelList = new List<DeviceResponseModel>();
                    List<DeviceResponseModel> drmList = new List<DeviceResponseModel>();
                    List<int> subgroups = new List<int>();
                    List<int> nonSubgroups = new List<int>();



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
                            InstallationCode = dev.InstallationCode,
                            GroupId = (from x in mc.DeviceGroups.OfType<DeviceGroup>() where x.DeviceId == dev.DeviceId select x.GroupId).FirstOrDefault()
                        });
                    }

                    foreach (DeviceResponseModel dev in drmList)
                    {
                        try
                        {
                            if (subgroups.Contains((int)dev.GroupId))
                            {
                                myDevicesResponseModelList.Add(dev);
                                continue;
                            }
                            if (nonSubgroups.Contains((int)dev.GroupId))
                            {
                                continue;
                            }
                            if (hm.CheckIfDeviceBelongsToUsersTree(vu, dev.DeviceId))
                            {
                                subgroups.Add((int)dev.GroupId);
                                myDevicesResponseModelList.Add(dev);
                            }
                            else
                            {
                                nonSubgroups.Add(((int)dev.GroupId));
                            }
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }

                    }
                    List<Device> myDevices = new List<Device>();

                    foreach (DeviceResponseModel dev in myDevicesResponseModelList)
                    {
                        myDevices.Add(mc.Devices.Where(x => x.DeviceId == dev.DeviceId).FirstOrDefault());
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


        [Route("api/device/GetDeviceByInstallationCode/{code}")]
        [HttpGet]
        public async Task<ActionResult<Device>> GetDeviceByInstallationCode(string code)
        {
            if(code!=null)
            {
                Device dev = mc.Devices.Where(x => x.InstallationCode == code).FirstOrDefault();
                if (dev != null)
                {
                    dev.InstallationCode = null;
                    mc.Devices.Attach(dev);
                    mc.Entry(dev).Property(x => x.InstallationCode).IsModified = true;
                    await mc.SaveChangesAsync();
                    return dev;
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest("Installation code cannot be null!");
            }
        }



        [Route("api/device/CheckIfDeviceBelongsToUser/{deviceId}")]
        [HttpGet]
        public async Task<ActionResult> CheckIfDeviceBelongsToUser([FromHeader] string Authorization, int deviceId)
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
                    if(userRoleName=="MonitorSuperAdmin" || (userRoleName=="SuperAdmin" && helperMethod.CheckIfDeviceBelongsToUsersTree(vu,deviceId)))
                    {
                        return StatusCode(200);
                    }
                    else
                    {
                        return StatusCode(404);
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



        [Route("api/device/GetAverageHardwareUsageForUser")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<DeviceStatusLogsForUser>>> GetDeviceLogs([FromHeader] String Authorization, [FromQuery] String? startDate, [FromQuery] String? endDate)
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

            try
            {
                //if startDate isn't passed startDateParsed is set to the old times to get all device logs for passed device
                //if endDate isn't passed endDateParsed is set to now to get all device logs for passed device
                DateTime startDateParsed = startDate != null ? DateTime.Parse(startDate) : DateTime.Now.AddYears(-1500);
                DateTime endDateParsed = endDate != null ? DateTime.Parse(endDate) : DateTime.Now.AddSeconds(5);

                if (userRoleName == MONITOR_SUPER_ADMIN)
                {
                    List<DeviceStatusLog> dsl = mc.DeviceStatusLogs
                        .Where(x => DateTime.Compare(x.TimeStamp, startDateParsed) >= 0 && DateTime.Compare(x.TimeStamp, endDateParsed) <= 0).ToList();

                    DeviceStatusLogsForUser myStatus = new DeviceStatusLogsForUser(dsl);
                    return new ResponseModel<DeviceStatusLogsForUser>() { data = myStatus, newAccessToken = vu.accessToken };
                }
                else if (userRoleName == SUPER_ADMIN)
                {
                    Group g = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault();

                    GroupHierarchyModel ghm = hm.FindHierarchyTreeWithDevices(g);
                    List<Device> myDevices = hm.GetDevicesForGHM(ghm);


                    List<DeviceStatusLog> logs = new List<DeviceStatusLog>();
                    foreach (var device in myDevices)
                    {
                        logs.AddRange(mc.DeviceStatusLogs
                            .Where(x => x.DeviceId == device.DeviceId).ToList()
                            .Where(x => DateTime.Compare(x.TimeStamp, startDateParsed) >= 0 && DateTime.Compare(x.TimeStamp, endDateParsed) <= 0).ToList());
                    }

                    DeviceStatusLogsForUser myStatus = new DeviceStatusLogsForUser(logs);
                    return new ResponseModel<DeviceStatusLogsForUser>() { data = myStatus, newAccessToken = vu.accessToken };
                }
            }
            catch (NullReferenceException e)
            {
                return NotFound(e.Message);
            }
            catch (FormatException)
            {
                return BadRequest("startDate and/or endDate are in wrong DateTime format. Use yyyy-MM-dd HH:mm:ss");
            }
            return StatusCode(403);
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
                                InstallationCode = dev.InstallationCode,
                                GroupId = (from x in mc.DeviceGroups.OfType<DeviceGroup>() where x.DeviceId == dev.DeviceId select x.GroupId).FirstOrDefault(),
                                DeviceUid = dev.DeviceUid
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
            try {
                if (userRoleName == MONITOR_SUPER_ADMIN || (userRoleName == SUPER_ADMIN && helperMethod.CheckIfDeviceBelongsToUsersTree(vu, deviceId))) {
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
