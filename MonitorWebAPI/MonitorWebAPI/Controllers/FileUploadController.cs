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
                catch (Exception)
                {
                    return StatusCode(500, "Internal server error");
                }
            }
            else
            {
                return Unauthorized();
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