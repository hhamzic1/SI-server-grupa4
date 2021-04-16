﻿using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        [Route("api/report/AllReportsForUser")] 
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

                List<Report> allReports = mc.Reports
                    .Where(x => x.UserId == vu.id)
                    .Where(x => x.Deleted == false)
                    .ToList();

                List<ReportResponseModel> reportList = new List<ReportResponseModel>();

                foreach (var report in allReports)
                {
                    reportList.Add(new ReportResponseModel()
                    {
                        ReportId = report.ReportId,
                        Name = report.Name,
                        Query = report.Query,
                        Frequency = report.Frequency,
                        ReportInstances = mc.ReportInstances.Where(x => x.ReportId == report.ReportId).ToList(),
                        NextDate = report.NextDate,
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
        public async Task<ActionResult<ResponseModel<List<Report>>>> GetReports([FromHeader] string Authorization, [FromQuery] ReportResponseModel queryReport)
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

                List<Report> allReports = mc.Reports.ToList()
                    .Where(x => x.UserId == vu.id)
                    .Where(x => x.Deleted == false)
                    .ToList();

                if (!string.IsNullOrEmpty(queryReport.Frequency))
                {
                    allReports = allReports.Where(x => x.Frequency.Contains(queryReport.Frequency)).ToList();
                }
                if (!string.IsNullOrEmpty(queryReport.ReportId.ToString()) && queryReport.ReportId != 0)
                {
                    allReports = allReports.Where(x => x.ReportId == queryReport.ReportId).ToList();
                }
                if (!string.IsNullOrEmpty(queryReport.Name))
                {
                    allReports = allReports.Where(x => x.Name.Contains(queryReport.Name)).ToList();
                }
                if (!string.IsNullOrEmpty(queryReport.Query))
                {
                    allReports = allReports.Where(x => x.Query.Contains(queryReport.Query)).ToList();
                }
                if (!string.IsNullOrEmpty(queryReport.UserId.ToString()) && queryReport.UserId != 0)
                {
                    allReports = allReports.Where(x => x.UserId == queryReport.UserId).ToList();
                }


                return new ResponseModel<List<Report>>() { data = allReports, newAccessToken = vu.accessToken };

            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/report/CreateReport")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<Report>>> CreateReport([FromHeader] string Authorization, [FromBody] Report report)
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
                try
                {
                    report.User = mc.Users.Where(x => x.UserId == vu.id).FirstOrDefault();
                    mc.Reports.Add(report);
                    await mc.SaveChangesAsync();

                    return new ResponseModel<Report>() { data = report, newAccessToken = vu.accessToken };

                }
                catch (Exception e)
                {
                    return BadRequest(e.Message + "\n" + e.InnerException);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/report/StopReport/{reportId}")]
        [HttpPatch]
        public async Task<ActionResult<ResponseModel<List<Report>>>> StopReport([FromHeader] string Authorization, int reportId)
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

                List<Report> allReports = mc.Reports
                    .Where(x => x.UserId == vu.id)
                    .Where(x => x.Deleted == false)
                    .ToList();

                var existingReport = allReports.Where(x => x.ReportId == reportId).FirstOrDefault();

                if (existingReport != null)
                {
                    existingReport.Deleted = true;
                    await mc.SaveChangesAsync();
                }

                return new ResponseModel<List<Report>>() { data = allReports, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        [Route("api/report/GetReportsByReportInstanceId/{instanceID}")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ReportResponseModel>>>> GetReportsByReportInstanceID([FromHeader] string Authorization, int instanceID)
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

                List<Report> allReports = mc.Reports
                    .Where(x => x.ReportInstances.All(y => y.Id.Equals(instanceID)))
                    .Where(x => x.Deleted == false)
                    .ToList();

                List<ReportResponseModel> reportList = new List<ReportResponseModel>();

                foreach (var report in allReports)
                {
                    reportList.Add(new ReportResponseModel()
                    {
                        ReportId = report.ReportId,
                        Name = report.Name,
                        Query = report.Query,
                        Frequency = report.Frequency,
                        ReportInstances = mc.ReportInstances.Where(x => x.ReportId == report.ReportId).ToList(),
                        NextDate = report.NextDate,
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

        [Route("api/report/GetReportInstances")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<ReportInstanceResponseModel>>>> GetReportInstances([FromHeader] string Authorization, [FromQuery] ReportInstanceResponseModel queryReportInstance)
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


                List<ReportInstance> allReportInstances = mc.ReportInstances.ToList();
                List<Report> allReports = mc.Reports.Where(x => x.Deleted == false && x.UserId == vu.id).ToList();

                if (!string.IsNullOrEmpty(queryReportInstance.UriLink))
                {
                    allReportInstances = allReportInstances.Where(x => x.UriLink.Contains(queryReportInstance.UriLink)).ToList();
                }
                if (!string.IsNullOrEmpty(queryReportInstance.ReportId.ToString()) && queryReportInstance.ReportId != 0)
                {
                    allReportInstances = allReportInstances.Where(x => x.ReportId == queryReportInstance.ReportId).ToList();
                }
                if (!string.IsNullOrEmpty(queryReportInstance.Name))
                {
                    allReportInstances = allReportInstances.Where(x => x.Name.Contains(queryReportInstance.Name)).ToList();
                }

                List<ReportInstanceResponseModel> reportList = new List<ReportInstanceResponseModel>();

                foreach (var reportInst in allReportInstances)
                    foreach (var report in allReports)
                    {
                        if (report.ReportId == reportInst.ReportId)
                        {
                            reportList.Add(new ReportInstanceResponseModel()
                            {
                                ReportId = reportInst.ReportId,
                                Name = reportInst.Name,
                                UriLink = reportInst.UriLink,
                                Date = reportInst.Date
                            });
                            break;
                        }
                            
                    }

                return new ResponseModel<List<ReportInstanceResponseModel>>() { data = reportList, newAccessToken = vu.accessToken };

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPut("/api/report/ChangeSendingEmail/{reportId}")]
        public async Task<ActionResult<ResponseModel<List<Report>>>> ChangeSendingEmail([FromHeader] string Authorization, int reportId)
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

                List<Report> allReports = mc.Reports
                    .Where(x => x.Deleted == false && x.UserId == vu.id)
                    .ToList();

                var existingReport = allReports.Where(x => x.ReportId == reportId).FirstOrDefault();

                if (existingReport != null)
                {
                    existingReport.SendEmail = ! existingReport.SendEmail;
                    await mc.SaveChangesAsync();
                }

                return new ResponseModel<List<Report>>() { data = allReports, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut("/api/report/EditReport")]
        public async Task<ActionResult<ResponseModel<List<Report>>>> EditReport([FromHeader] string Authorization, [FromBody] Report report)
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

                List<Report> allReports = mc.Reports
                    .Where(x => x.Deleted == false && x.UserId == vu.id)
                    .ToList();

                var existingReport = allReports.Where(x => x.ReportId == report.ReportId).FirstOrDefault();

                if (existingReport != null)
                {
                    existingReport.Name = report.Name;
                    existingReport.Frequency = report.Frequency;
                    existingReport.NextDate = report.NextDate;
                    existingReport.Query = report.Query;
                    existingReport.SendEmail = report.SendEmail;
                    await mc.SaveChangesAsync();
                }

                return new ResponseModel<List<Report>>() { data = allReports, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}