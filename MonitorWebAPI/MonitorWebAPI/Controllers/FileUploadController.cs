using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonitorWebAPI.Controllers
{

    public class FIleUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly monitorContext mc;
        private readonly HelperMethods hm;

        public FIleUploadController(IWebHostEnvironment he)
        {
            hostingEnvironment = he;
            mc = new monitorContext();
            hm = new HelperMethods();
        }


        [Route("api/upload/UploadFile")]
        [HttpPost]
        public async Task<ActionResult> UploadFileFromFormData([FromForm] IFormCollection form, [FromHeader] string Authorization, Guid deviceUid, int taskId)
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
                    int deviceId = mc.Devices.Where(x => x.DeviceUid == deviceUid).FirstOrDefault().DeviceId;
                    var tempTask = mc.UserTasks.Where(x => x.UserId == vu.id && x.TaskId == taskId).FirstOrDefault();

                    if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && tempTask != null && hm.CheckIfDeviceBelongsToUsersTree(vu, deviceId)))
                    {
                        var files = form.Files;
                        var folderName = Path.Combine("Uploads", "Files");
                        var pathToSave = Path.Combine(hostingEnvironment.ContentRootPath, folderName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        if (files.Any(f => f.Length == 0 || String.IsNullOrEmpty(f.Name)))
                        {
                            return BadRequest();
                        }

                        foreach (var file in files)
                        {
                            var fileName = file.Name + Path.GetExtension(file.FileName);
                            var fullPath = Path.Combine(pathToSave, fileName);
                            var dbPath = Path.Combine(folderName, fileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                        return Ok("All files are successfully uploaded.");
                    }
                    else
                    {
                        return StatusCode(403);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/upload/GetImagesForUserTask")]
        public async Task<ActionResult<ResponseModel<List<ImageModel>>>> GetImagesForUserTask([FromHeader] string Authorization, Guid deviceUid, int taskId)
        {
            List<ImageModel> images = new List<ImageModel>();
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

                    int deviceId = mc.Devices.Where(x => x.DeviceUid == deviceUid).FirstOrDefault().DeviceId;
                    var tempTask = mc.UserTasks.Where(x => x.UserId == vu.id && x.TaskId == taskId).FirstOrDefault();
                    if (userRoleName == "MonitorSuperAdmin" || (userRoleName == "SuperAdmin" && tempTask != null && hm.CheckIfDeviceBelongsToUsersTree(vu, deviceId)))
                    {

                        var folderName = Path.Combine("Uploads", "Files");
                        var loadPath = Path.Combine(hostingEnvironment.ContentRootPath, folderName);
                        string matching = deviceUid + "_" + taskId + '*';
                        foreach (string file in Directory.GetFiles(loadPath, matching))
                        {
                            byte[] bytes = System.IO.File.ReadAllBytes(file);
                            images.Add(new ImageModel
                            {
                                Name = Path.GetFileName(file),
                                Base64 = Convert.ToBase64String(bytes, 0, bytes.Length)
                            });
                        }
                        return new ResponseModel<List<ImageModel>> { data = images, newAccessToken = vu.accessToken };
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
    }
}