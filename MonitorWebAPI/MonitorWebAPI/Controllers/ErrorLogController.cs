using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonitorWebAPI.Controllers
{
    [EnableCors("MonitorPolicy")]
    [ApiController]
    public class ErrorLogController : ControllerBase
    {
        private readonly monitorContext mc;
        private readonly string SUPER_ADMIN = "SuperAdmin";
        private readonly string MONITOR_SUPER_ADMIN = "MonitorSuperAdmin";
        private HelperMethods helperMethod;

        public ErrorLogController()
        {
            mc = new monitorContext();
            helperMethod = new HelperMethods();
        }

        [Route("api/error/DateInterval")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ErrorInfo>>>> GetErrorsInDateInterval([FromBody] ErrorDateInterval errorDateInterval)
        {
            List<ErrorLog> errorLogs = mc.ErrorLogs.Where(x => x.Device.DeviceUid == errorDateInterval.DeviceUID && x.ErrorTime >= errorDateInterval.StartDate && x.ErrorTime <= errorDateInterval.EndDate).ToList();

            List<ErrorInfo> errorInfos = new List<ErrorInfo>();

            foreach(var x in errorLogs)
            {
                ErrorDictionary errorDictionary = mc.ErrorDictionaries.Where(y => y.Id == x.ErrorTypeId).FirstOrDefault();
                errorInfos.Add(new ErrorInfo() { 
                    ErrorTime = x.ErrorTime,
                    Message = x.Message,
                    Code = errorDictionary.Code,
                    Description = errorDictionary.Description,
                    Type = errorDictionary.Type,
                    DeviceUID = errorDateInterval.DeviceUID
                });
            }

            return new ResponseModel<List<ErrorInfo>>() { data = errorInfos, newAccessToken = null };
        }

        //funkcija CheckIfDeviceBelongsToUsersTree
        [Route("api/error/GroupErrors")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ErrorInfo>>>> GetErrorsFromOneGroup([FromHeader] string Authorization)
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

                List<ErrorLog> tmp = mc.ErrorLogs.ToList();
                List<ErrorLog> errorLogs = new List<ErrorLog>();
                foreach(var err in tmp)
                {
                    if(helperMethod.CheckIfDeviceBelongsToUsersTree(vu, err.DeviceId))
                    {
                        errorLogs.Add(err);
                    }
                }

                List<ErrorInfo> errorInfos = new List<ErrorInfo>();

                foreach (var x in errorLogs)
                {
                    Guid guid = mc.Devices.Where(y => y.DeviceId == x.DeviceId).FirstOrDefault().DeviceUid;
                    if (x.ErrorTypeId == null)
                    {
                        errorInfos.Add(new ErrorInfo()
                        {
                            ErrorTime = x.ErrorTime,
                            Message = x.Message,
                            Code = null,
                            Description = null,
                            Type = null,
                            DeviceUID = guid
                        });
                        continue;
                    }
                    ErrorDictionary errorDictionary = mc.ErrorDictionaries.Where(y => y.Id == x.ErrorTypeId).FirstOrDefault();
                    errorInfos.Add(new ErrorInfo()
                    {
                        ErrorTime = x.ErrorTime,
                        Message = x.Message,
                        Code = errorDictionary.Code,
                        Description = errorDictionary.Description,
                        Type = errorDictionary.Type,
                        DeviceUID = guid
                    });
                }

                return new ResponseModel<List<ErrorInfo>>() { data = errorInfos, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
