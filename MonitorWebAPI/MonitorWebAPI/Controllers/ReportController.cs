using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MonitorWebAPI.Controllers
{
    [EnableCors("MonitorPolicy")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly monitorContext mc;
        public ReportController()
        {
            mc = new monitorContext();
        }

        [Route("api/report/AllReportsForUser")] //dodajapi/
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ReportResponseModel>>>> ReportsForUserById([FromHeader] string Authorization)
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

                List<Report> allReports = mc.Reports.Where((x => x.UserId == vu.id)).Where(y => y.Deleted == false).ToList();

                List<ReportResponseModel> reportList = new List<ReportResponseModel>();

                foreach (var report in allReports)
                {
                    reportList.Add(new ReportResponseModel()
                    {
                        ReportId = report.ReportId,
                        Name = report.Name,
                        Query = report.Query,
                        Frequency = report.Frequency,
<<<<<<< HEAD
                        //StartDate = report.NextDate,
=======
                        StartDate = report.NextDate,
>>>>>>> 1aabd4f6f55bff7a727267f2bea580a242f843c2
                        UserId = vu.id
                    });
                }

                return new ResponseModel<List<ReportResponseModel>>() { data = reportList, newAccessToken = vu.accessToken };

            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/report/GetReports")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ReportResponseModel>>>> GetReports([FromHeader] string Authorization, [FromQuery] ReportResponseModel queryReport)
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

                List<Report> allReports = mc.Reports.ToList().Where(x => x.Deleted == false).ToList();

                if (queryReport.Frequency != null)
                {
                    allReports = allReports.Where(x => x.Frequency == queryReport.Frequency).ToList();
                }
                if (string.IsNullOrEmpty(queryReport.ReportId.ToString()))
                {
                    allReports = allReports.Where(x => x.ReportId == queryReport.ReportId).ToList();
                }
                if (queryReport.Name != null)
                {
                    allReports = allReports.Where(x => x.Name == queryReport.Name).ToList();
                }
                if (!string.IsNullOrEmpty(queryReport.UserId.ToString()))
                {
                    allReports = allReports.Where(x => x.UserId == queryReport.UserId).ToList();
                }
                if (queryReport.StartDate != DateTime.MinValue)
                {
                    allReports = allReports.Where(x => x.NextDate.Ticks > queryReport.StartDate.Ticks).ToList();
                }
                if (!string.IsNullOrEmpty(queryReport.UserId.ToString()))
                {
                    allReports = allReports.Where(x => x.UserId == queryReport.UserId).ToList();
                }

                List<ReportResponseModel> reportList = new List<ReportResponseModel>();

                foreach (var report in allReports)
                {
                    reportList.Add(new ReportResponseModel()
                    {
                        ReportId = report.ReportId,
                        Name = report.Name,
                        Query = report.Query,
                        Frequency = report.Frequency,
<<<<<<< HEAD
                        //StartDate = report.NextDate,
=======
                        StartDate = report.NextDate,
>>>>>>> 1aabd4f6f55bff7a727267f2bea580a242f843c2
                        UserId = vu.id
                    });
                }

                return new ResponseModel<List<ReportResponseModel>>() { data = reportList, newAccessToken = vu.accessToken };

            }
            else
            {
                return Unauthorized();
            }
        }

        //[Route("api/report/CreateReport")]
        //[HttpPost]
        //public async Task<ActionResult<ResponseModel<ReportResponseModel>>> CreateReport([FromHeader] string Authorization, [FromBody] Report report)
        //{
        //    string JWT = JWTVerify.GetToken(Authorization);
        //    if (JWT == null)
        //    {
        //        return Unauthorized();
        //    }
        //    HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);
        //        try
        //        {
        //            report.User = mc.Users.Where(x => x.UserId == vu.id).FirstOrDefault();
        //            mc.Reports.Add(report);
        //                await mc.SaveChangesAsync();
        //                ReportResponseModel tempReport = new ReportResponseModel()
        //                {
        //                    ReportId = report.ReportId,
        //                    Name = report.Name,
        //                    Query = report.Query,
        //                    Frequency = report.Frequency,
<<<<<<< HEAD
        //                    //StartDate = report.NextDate,
=======
        //                    StartDate = report.NextDate,
>>>>>>> 1aabd4f6f55bff7a727267f2bea580a242f843c2
        //                    UserId = vu.id
        //                };

        //                if (tempReport != null)
        //                {
        //                    await mc.SaveChangesAsync();
        //                    return new ResponseModel<ReportResponseModel>() { data = tempReport, newAccessToken = vu.accessToken };
        //                }
        //                throw new Exception("Report wasn't added succesfully");

        //        }
        //        catch (Exception e)
        //        {
        //            return BadRequest(e.Message + "\n" + e.InnerException);
        //        }
        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }
        //}

        [Route("api/report/StopReport/{reportId}")]
        [HttpPut]
        public async Task<ActionResult<ResponseModel<List<ReportResponseModel>>>> StopReport([FromHeader] string Authorization, int reportId)
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

                List<Report> allReports = mc.Reports.Where(x => x.Deleted == false).ToList();

                var existingReport = allReports.Where(x => x.ReportId == reportId).FirstOrDefault();

                if (existingReport != null)
                {
                    existingReport.Deleted = true;
                    await mc.SaveChangesAsync();
                }
                List<ReportResponseModel> reportList = new List<ReportResponseModel>();

                foreach (var report in allReports)
                {
                    reportList.Add(new ReportResponseModel()
                    {
                        ReportId = report.ReportId,
                        Name = report.Name,
                        Query = report.Query,
                        Frequency = report.Frequency,
<<<<<<< HEAD
                        //StartDate = report.NextDate,
=======
                        StartDate = report.NextDate,
>>>>>>> 1aabd4f6f55bff7a727267f2bea580a242f843c2
                        UserId = vu.id,
                    });
                }

                return new ResponseModel<List<ReportResponseModel>>() { data = reportList, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        //[Route("api/report/GetReportsByInstanceId/{instanceID}")]
        //[HttpGet]
        //public async Task<ActionResult<ResponseModel<List<ReportResponseModel>>>> GetReportsByInstanceID([FromHeader] string Authorization, [FromQuery] int instanceID)
        //{
        //    string JWT = JWTVerify.GetToken(Authorization);
        //    if (JWT == null)
        //    {
        //        return Unauthorized();
        //    }

        //    HttpResponseMessage response = JWTVerify.VerifyJWT(JWT).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        VerifyUserModel vu = JsonConvert.DeserializeObject<VerifyUserModel>(responseBody);

        //        List<Report> allReports = mc.Reports.ToList();

        //        List<ReportResponseModel> reportList = new List<ReportResponseModel>();

        //        foreach (var report in allReports)
        //        {
        //            reportList.Add(new ReportResponseModel()
        //            {
        //                ReportId = report.ReportId,
        //                Name = report.Name,
        //                Query = report.Query,
        //                Frequency = report.Frequency,
<<<<<<< HEAD
        //                //StartDate = report.NextDate,
=======
        //                StartDate = report.NextDate,
>>>>>>> 1aabd4f6f55bff7a727267f2bea580a242f843c2
        //                UserId = vu.id
        //            });
        //        }

        //        return new ResponseModel<List<ReportResponseModel>>() { data = reportList, newAccessToken = vu.accessToken };

        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }
        //}
    }
}
