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
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly monitorContext mc;
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
                    List<Group> subgroupList = mc.Groups.Where(x => x.ParentGroup == vu.groupId).ToList();

                    List<DeviceGroup> dgList = mc.DeviceGroups.Where(x => x.GroupId == vu.groupId).ToList();
                    for (int i = 0; i < subgroupList.Count; i++)
                    {
                        dgList.AddRange(mc.DeviceGroups.Where(x => x.GroupId == subgroupList[i].GroupId));
                    }

                    List<Device> allDevices = new List<Device>();

                    for (int i = 0; i < dgList.Count; i++)
                    {
                        allDevices.AddRange(mc.Devices.Where(x => x.DeviceId == dgList[i].DeviceId));
                    }
                    return new ResponseModel<List<Device>>() { data = allDevices, newAccessToken = vu.accessToken };
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
                if (userRoleName == "MonitorSuperAdmin" || (userRoleName=="SuperAdmin" && vu.groupId==groupId))
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
                        return new ResponseModel<Device>() { data=tempDevice, newAccessToken=vu.accessToken };
                    }
                    return NotFound();
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

        [Route("api/device/AllDevicesForGroup")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<DeviceResponseModel>>>> DeviceForGroupById([FromQuery] int groupId, [FromHeader] string Authorization)
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
                if(userRoleName == "MonitorSistemAdmin" || (userRoleName == "SuperAdmin" && vu.groupId==groupId))
                {
                    List<Group> subgroupList = mc.Groups.Where(x => x.ParentGroup == vu.groupId).ToList();

                    List<DeviceGroup> dgList = mc.DeviceGroups.Where(x => x.GroupId == vu.groupId).ToList();
                    for (int i = 0; i < subgroupList.Count; i++)
                    {
                        dgList.AddRange(mc.DeviceGroups.Where(x => x.GroupId == subgroupList[i].GroupId));
                    }

                    List<Device> allDevices = new List<Device>();

                    for (int i = 0; i < dgList.Count; i++)
                    {
                        allDevices.AddRange(mc.Devices.Where(x => x.DeviceId == dgList[i].DeviceId));
                    }

                    List<DeviceResponseModel> drmList = new List<DeviceResponseModel>();
                    foreach(var dev in allDevices)
                    {
                        drmList.Add(new DeviceResponseModel() { DeviceId = dev.DeviceId, Name = dev.Name, Location = dev.Location, LocationLatitude = dev.LocationLatitude, LocationLongitude = dev.LocationLongitude, Status = dev.Status, LastTimeOnline = dev.LastTimeOnline, GroupId = (from x in dev.DeviceGroups.OfType<DeviceGroup>() where x.DeviceId == dev.DeviceId select x.GroupId).FirstOrDefault() });
                    }
                    return new ResponseModel<List<DeviceResponseModel>>() { data = drmList, newAccessToken = vu.accessToken };
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


        [Route("api/device/GetAllDeviceLogs")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<DeviceStatusLog>>>> GetAllDeviceLogs([FromHeader] string Authorization)
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
                    List<DeviceGroup> dgList = new List<DeviceGroup>();
                    dgList.AddRange(mc.DeviceGroups);
                    List<DeviceStatusLog> dslList = new List<DeviceStatusLog>();
                    foreach (DeviceGroup dg in dgList)
                    {
                        Device tempDevice = mc.Devices.Where(x => x.DeviceId == dg.DeviceId).FirstOrDefault();
                        List<DeviceStatusLog> tempList = mc.DeviceStatusLogs.Where(x => x.DeviceId == dg.DeviceId).ToList();
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            tempList[i].Device = tempDevice;
                        }
                        dslList.AddRange(tempList);
                    }

                    return new ResponseModel<List<DeviceStatusLog>>() { data = dslList, newAccessToken = vu.accessToken };
                }
                else if (userRoleName == "SuperAdmin")
                {
                    List<DeviceGroup> dgList = new List<DeviceGroup>();
                    dgList.AddRange(mc.DeviceGroups.Where(x => x.GroupId == vu.groupId).ToList());
                    List<DeviceStatusLog> dslList = new List<DeviceStatusLog>();
                    foreach (DeviceGroup dg in dgList)
                    {
                        Device tempDevice = mc.Devices.Where(x => x.DeviceId == dg.DeviceId).FirstOrDefault();
                        List<DeviceStatusLog> tempList = mc.DeviceStatusLogs.Where(x => x.DeviceId == dg.DeviceId).ToList();
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            tempList[i].Device = tempDevice;
                        }
                        dslList.AddRange(tempList);
                    }

                    return new ResponseModel<List<DeviceStatusLog>>() { data = dslList, newAccessToken = vu.accessToken };
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
