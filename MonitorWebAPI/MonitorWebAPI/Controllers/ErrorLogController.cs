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
        public async Task<ActionResult<ResponseModel<DeviceErrorInfo>>> GetErrorsInDateInterval([FromQuery] String DeviceUID, [FromQuery] String? StartDate, [FromQuery] String? EndDate)
        {
            DateTime StartDateParsed = StartDate != null ? DateTime.Parse(StartDate) : DateTime.Now.AddYears(-1500);
            DateTime EndDateParsed = EndDate != null ? DateTime.Parse(EndDate) : DateTime.Now;

            List<ErrorLog> errorLogs = mc.ErrorLogs.Where(x => x.Device.DeviceUid == Guid.Parse(DeviceUID) && x.ErrorTime >= StartDateParsed && x.ErrorTime <= EndDateParsed).ToList();
            int deviceId = mc.Devices.Where(x => x.DeviceUid == Guid.Parse(DeviceUID)).FirstOrDefault().DeviceId;
            List<ErrorInfo> errorInfos = new List<ErrorInfo>();

            foreach(var x in errorLogs)
            {
                if (x.ErrorTypeId == null)
                {
                    errorInfos.Add(new ErrorInfo()
                    {
                        ErrorTime = x.ErrorTime,
                        Message = x.Message,
                        Code = null,
                        Description = null,
                        Type = null
                    });
                    continue;
                }
                ErrorDictionary errorDictionary = mc.ErrorDictionaries.Where(y => y.Id == x.ErrorTypeId).FirstOrDefault();
                

                errorInfos.Add(new ErrorInfo() { 
                    ErrorTime = x.ErrorTime,
                    Message = x.Message,
                    Code = errorDictionary.Code,
                    Description = errorDictionary.Description,
                    Type = errorDictionary.Type
                });
            }

            List<int?> types = new List<int?>();
            foreach (var temp in errorLogs) types.Add(temp.ErrorTypeId);
            types = types.Distinct().ToList();
            List<ErrorTypeInfo> errorTypeInfos = new List<ErrorTypeInfo>();
            foreach( var type in types)
            {
                int? tempCode = null;
                if(type != null)
                {
                    tempCode = mc.ErrorDictionaries.Where(x => x.Id == type).FirstOrDefault().Code;
                }

                int tempErrorCodeNumber = errorLogs.Where(x => x.ErrorTypeId == type).Count();


                errorTypeInfos.Add(new ErrorTypeInfo() {

                    Code = tempCode,
                    ErrorCodeNumber = tempErrorCodeNumber
                });
            }
           



            DeviceErrorInfo deviceErrorInfo = new DeviceErrorInfo()
            {
                DeviceUID = Guid.Parse(DeviceUID),
                ErrorNumber = errorLogs.Count,
                errorInfo = errorInfos,
                errorTypeInfos = errorTypeInfos
            };

            return new ResponseModel<DeviceErrorInfo> () { data = deviceErrorInfo, newAccessToken = null };
        }

        [Route("api/error/AllDateInterval")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<DeviceErrorInfo>>>> Proba([FromQuery] String? StartDate, [FromQuery] String? EndDate)
        {
            DateTime StartDateParsed = StartDate != null ? DateTime.Parse(StartDate) : DateTime.Now.AddYears(-1500);
            DateTime EndDateParsed = EndDate != null ? DateTime.Parse(EndDate) : DateTime.Now;


            List<DeviceErrorInfo> deviceErrorInfos = new List<DeviceErrorInfo>();
            List<Device> devices = mc.Devices.ToList();
            foreach(var device in devices)
            {
                List<ErrorLog> errorLogs = mc.ErrorLogs.Where(x => x.Device.DeviceUid == device.DeviceUid && x.ErrorTime >= StartDateParsed && x.ErrorTime <= EndDateParsed).ToList();

                List<ErrorInfo> errorInfos = new List<ErrorInfo>();

                foreach (var x in errorLogs)
                {
                    if (x.ErrorTypeId == null)
                    {
                        errorInfos.Add(new ErrorInfo()
                        {
                            ErrorTime = x.ErrorTime,
                            Message = x.Message,
                            Code = null,
                            Description = null,
                            Type = null
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
                        Type = errorDictionary.Type
                    });
                }


                List<int?> types = new List<int?>();
                foreach (var temp in errorLogs) types.Add(temp.ErrorTypeId);
                types = types.Distinct().ToList();
                List<ErrorTypeInfo> errorTypeInfos = new List<ErrorTypeInfo>();
                foreach (var type in types)
                {
                    int? tempCode = null;
                    if (type != null)
                    {
                        tempCode = mc.ErrorDictionaries.Where(x => x.Id == type).FirstOrDefault().Code;
                    }

                    int tempErrorCodeNumber = errorLogs.Where(x => x.ErrorTypeId == type).Count();


                    errorTypeInfos.Add(new ErrorTypeInfo()
                    {

                        Code = tempCode,
                        ErrorCodeNumber = tempErrorCodeNumber
                    });
                }


                deviceErrorInfos.Add(new DeviceErrorInfo()
                {
                    DeviceUID = device.DeviceUid,
                    ErrorNumber = errorLogs.Count,
                    errorInfo = errorInfos,
                    errorTypeInfos = errorTypeInfos
                });
            }
            return new ResponseModel<List<DeviceErrorInfo>>() { data = deviceErrorInfos, newAccessToken = null };
        }

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
                            Type = null
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
                        Type = errorDictionary.Type
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
