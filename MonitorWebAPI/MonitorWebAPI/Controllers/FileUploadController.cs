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

        public FIleUploadController(IWebHostEnvironment he)
        {
            hostingEnvironment = he;
            mc = new monitorContext();

        }


        [Route("api/upload/UploadFile")]
        [HttpPost]
        public ActionResult UploadFileFromFormData([FromForm] IFormCollection form, [FromHeader] string Authorization)
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
                    return Ok("All the files are successfully uploaded.");
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


        [Route("api/upload/GetFile")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<List<ImageResponseModel>>>> GetFileFromFolder([FromBody] ImageRequestModel ids, [FromHeader] string Authorization)
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

                    List<ImageResponseModel> images = new List<ImageResponseModel>();


                    var folderName = Path.Combine("Uploads", "Files");
                    var path = Path.Combine(hostingEnvironment.ContentRootPath, folderName);


                    foreach (string file in Directory.GetFiles(path,ids.machineUid+"-"+ids.taskId+"*"))
                    {
                        
                        byte[] bytes = System.IO.File.ReadAllBytes(file);

                        
                        images.Add(new ImageResponseModel
                        {
                            Name = Path.GetFileName(file),
                            Data = Convert.ToBase64String(bytes, 0, bytes.Length)
                        });
                    }


                    return new ResponseModel<List<ImageResponseModel>>() { data = images, newAccessToken = vu.accessToken };
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

        [Route("api/upload/GetFileTest")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<String>>>> GetFileTest()
        {
                try
                {
                    
                    List<String> images = new List<String>();


                    var folderName = Path.Combine("Uploads", "Files");
                    var path = Path.Combine(hostingEnvironment.ContentRootPath, folderName);


                    foreach (string file in Directory.GetFiles(path))
                    { 

                        images.Add(file);
                    }


                    return new ResponseModel<List<String>>() { data = images, newAccessToken = "123" };
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
        }
           
        

        /*
        [Route("api/upload/UploadFileByteArray")]
        [HttpPost]
        public ActionResult UploadFile([FromBody]byte[] f, string fileName)
        {
            try
            {
                var folderName = Path.Combine("Uploads", "Files");
                var pathToSave = Path.Combine(hostingEnvironment.ContentRootPath, folderName);
                MemoryStream ms = new MemoryStream(f);
                FileStream fs = new FileStream(pathToSave, FileMode.Create);
                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();
                return Ok("All good");
            }
            catch (Exception)
            {
                return BadRequest("Not good at all");
            }
        }*/
    }
}