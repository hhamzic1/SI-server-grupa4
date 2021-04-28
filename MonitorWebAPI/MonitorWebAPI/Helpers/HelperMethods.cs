using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MonitorWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Net.Mime;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;

namespace MonitorWebAPI.Helpers
{
    public class HelperMethods
    {
        public GroupHierarchyModel FindHierarchyTree(Models.Group g)
        {
            monitorContext mc = new monitorContext();
            GroupHierarchyModel ghm = new GroupHierarchyModel() { GroupId = g.GroupId, parentGroupId=null, Name = g.Name, SubGroups = new List<GroupHierarchyModel>() };
            findSubgroups(ghm, mc);
            return ghm;
        }

        void findSubgroups(GroupHierarchyModel ghm, monitorContext mc)
        {
            var tempList = mc.Groups.Where(x => x.ParentGroup == ghm.GroupId);
            foreach(var group in tempList)
            {
                ghm.SubGroups.Add(new GroupHierarchyModel { GroupId = group.GroupId, parentGroupId = ghm.GroupId, Name = group.Name, SubGroups = new List<GroupHierarchyModel>() });
            }
            foreach (var tempGhm in ghm.SubGroups)
            {
                findSubgroups(tempGhm, mc);
            }
        }

        public GroupHierarchyModel FindHierarchyTreeWithDevices(Models.Group g)
        {
            monitorContext mc = new monitorContext();
            GroupHierarchyModel ghm = new GroupHierarchyModel() { GroupId = g.GroupId, Name = g.Name, SubGroups = new List<GroupHierarchyModel>() };
            findSubgroupsWithDevices(ghm, mc);
            return ghm;
        }

        void findSubgroupsWithDevices(GroupHierarchyModel ghm, monitorContext mc)
        {
            var tempList = mc.Groups.Where(x => x.ParentGroup == ghm.GroupId);
            foreach (var group in tempList)
            {
                ghm.SubGroups.Add(new GroupHierarchyModel { GroupId = group.GroupId, Name = group.Name, SubGroups = new List<GroupHierarchyModel>() });
            }
            foreach (var tempGhm in ghm.SubGroups)
            {
                findSubgroupsWithDevices(tempGhm, mc);
            }
            ghm.Devices = (from dg in mc.DeviceGroups
                           join d in mc.Devices on dg.DeviceId equals d.DeviceId
                           where dg.GroupId == ghm.GroupId
                           select d).ToList();
        }



        // ----------- da li uređaj pripada korisnikovom stablu
        public bool CheckIfDeviceBelongsToUsersTree(VerifyUserModel vu, int deviceId)
        {
            monitorContext mc = new monitorContext();
            string groupName = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault().Name;
            bool belongs = false;
            DeviceGroup deviceGroup = mc.DeviceGroups.Where(x => x.DeviceId == deviceId).FirstOrDefault();
            if(deviceGroup == null) {
                throw new NullReferenceException("Device with deviceId doesn't belong to any group!");
            }
            int? deviceGroupId = deviceGroup.GroupId;
            GroupHierarchyModel ghm = new GroupHierarchyModel() { GroupId = vu.groupId, Name = groupName, SubGroups = new List<GroupHierarchyModel>() };
            checkSubgroup(ref belongs, ghm, mc, deviceGroupId);
            return belongs;
        }

        void checkSubgroup(ref bool belongs, GroupHierarchyModel ghm, monitorContext mc, int? deviceGroupId)
        {
            var tempList = mc.Groups.Where(x => x.ParentGroup == ghm.GroupId);
            foreach (var group in tempList)
            {
                if(deviceGroupId==group.GroupId)
                {
                    belongs = true;
                    return;
                }
                ghm.SubGroups.Add(new GroupHierarchyModel { GroupId = group.GroupId, Name = group.Name, SubGroups = new List<GroupHierarchyModel>() });
            }
            if(belongs!=true)
            {
                foreach (var tempGhm in ghm.SubGroups)
                {
                    checkSubgroup(ref belongs, tempGhm, mc, deviceGroupId);
                }
            }
        }
        // ------------


        //------------- Da li grupa pripada korisnikovom stablu
        public bool CheckIfGroupBelongsToUsersTree(VerifyUserModel vu, int? groupId)
        {
            monitorContext mc = new monitorContext();
            string groupName = mc.Groups.Where(x => x.GroupId == vu.groupId).FirstOrDefault().Name;

            Models.Group tempGroup  = mc.Groups.Where(x => x.GroupId == groupId).FirstOrDefault();
            if (tempGroup == null)
            {
                throw new NullReferenceException("Group with that id doesn't exist!");
            }

            bool belongs = vu.groupId==groupId;
            GroupHierarchyModel ghm = new GroupHierarchyModel() { GroupId = vu.groupId, Name = groupName, SubGroups = new List<GroupHierarchyModel>() };
            ifGroupBelongsToTree(ref belongs, ghm, mc, groupId);
            return belongs;
        }

        void ifGroupBelongsToTree(ref bool belongs, GroupHierarchyModel ghm, monitorContext mc, int? groupId)
        {
            if(ghm.GroupId==groupId)
            {
                belongs = true;
                return;
            }
            var tempList = mc.Groups.Where(x => x.ParentGroup == ghm.GroupId);
            foreach (var group in tempList)
            {
                if (groupId == group.GroupId)
                {
                    belongs = true;
                    return;
                }
                ghm.SubGroups.Add(new GroupHierarchyModel { GroupId = group.GroupId, Name = group.Name, SubGroups = new List<GroupHierarchyModel>() });
            }
            foreach (var tempGhm in ghm.SubGroups)
            {
                ifGroupBelongsToTree(ref belongs, tempGhm, mc, groupId);
            }
        }
        //-------------


        private void GetDeviceList(GroupHierarchyModel? ghm, ref List<Device> deviceList)
        {
            if (ghm == null)
                return;

            if (ghm.SubGroups != null && ghm.SubGroups.Count != 0)
            {
                foreach (var entry in ghm.SubGroups)
                {
                    GetDeviceList(entry, ref deviceList);
                }
            }
            else
            {
                if (ghm.Devices != null)
                    deviceList.AddRange(ghm.Devices);
            }
        }

        public string SendEmailTest(int id)
        {
            return sendEmail(id, "test@test.com");
        }

        public static string sendEmail(int id, String email)
        {
            monitorContext mc = new monitorContext();

            var smptClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("reporting.monitor@gmail.com", "monitor2021"),
                EnableSsl = true
            };

            var instanceName = CreatePDF.GenerateInstanceName(id);
            var pdfDocument = CreatePDF.createPDF(id, instanceName);
            var linkToAzure = BlobUpload.UploadPDFAsync(instanceName);

            MailMessage message = new MailMessage(
                "reporting.monitor@gmail.com",
                email,
                "Monitor app report",
                "Report for your report request with id " + id);

            message.Attachments.Add(new Attachment(Directory.GetCurrentDirectory() + "/data/" + instanceName));

            smptClient.Send(message);

            return instanceName;
        }

        public static void CronJob()
        {
            DateTime now = DateTime.Now;
            DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            monitorContext mc = new monitorContext();
            List<Report> reports = mc.Reports.ToList();

            foreach (var rep in reports)
            {
                DateTime dateToCompare = new DateTime(rep.NextDate.Year, rep.NextDate.Month, rep.NextDate.Day, rep.NextDate.Hour, 0, 0);
                int res = DateTime.Compare(dateTime, TimeZoneInfo.ConvertTimeToUtc(dateToCompare));

                if (res == 0)
                {
                    string linkToAzure = "";

                    if (rep.SendEmail.Equals(true))
                    {
                        var email = mc.Users.Where(x => x.UserId == rep.UserId).FirstOrDefault().Email;

                        linkToAzure = sendEmail(rep.ReportId, email);
                    }

                    mc.ReportInstances.Add(new ReportInstance() { Name = rep.Name + " instance", ReportId = rep.ReportId, UriLink = "https://si2021storage.blob.core.windows.net/si2021pdf/" + linkToAzure, Date = TimeZoneInfo.ConvertTimeToUtc(rep.NextDate) });
                    if (rep.Frequency.Equals("Weekly", StringComparison.InvariantCultureIgnoreCase))
                    {
                        rep.NextDate = rep.NextDate.AddDays(7);

                    } else if (rep.Frequency.Equals("Monthly", StringComparison.InvariantCultureIgnoreCase))
                    {
                        rep.NextDate = rep.NextDate.AddMonths(1);

                    } else if (rep.Frequency.Equals("Daily", StringComparison.InvariantCultureIgnoreCase))
                    {
                        rep.NextDate = rep.NextDate.AddDays(1);

                    } else if (rep.Frequency.Equals("Yearly", StringComparison.InvariantCultureIgnoreCase))
                    {
                        rep.NextDate = rep.NextDate.AddYears(1);

                    }

                    mc.SaveChanges();

                }

            }

        }

        public List<Device> GetDevicesForGHM(GroupHierarchyModel ghm)
        {
            List<Device> deviceList = new List<Device>();
            GetDeviceList(ghm, ref deviceList);

            return deviceList;
        }

        public static async Task<HttpResponseMessage> GetConfigFile(string JWT, Guid deviceUID, string fileName)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);


            var values = new Dictionary<string, string>
            {
                {"deviceUid", deviceUID.ToString()}, {"fileName", fileName}, {"path", ""}
            };
            var content = JsonConvert.SerializeObject(values, Formatting.Indented);

            var data = new StringContent(content, Encoding.UTF8, "application/json");
            return await client.PostAsync("https://si-grupa5.herokuapp.com/api/web/agent/file/get", data);
        }

        public static async Task<HttpResponseMessage> PostConfigFile(string JWT, Guid deviceUID, string fileName, string base64)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);

            var values = new Dictionary<string, string>
            {
                {"deviceUid", deviceUID.ToString()}, {"fileName", fileName}, {"path", ""}, {"base64", base64}
            };
            var content = JsonConvert.SerializeObject(values, Formatting.Indented);

            var data = new StringContent(content, Encoding.UTF8, "application/json");
            return await client.PostAsync("https://si-grupa5.herokuapp.com/api/web/agent/file/put", data);
        }

        public static DateTime GetStartDate(DateTime endDate, string frequency)
        {
            DateTime startDate = new DateTime();

            if (frequency.ToUpper().Equals("DAILY"))
            {
                startDate = endDate.AddDays(-1);
            }
            else if (frequency.ToUpper().Equals("WEEKLY"))
            {
                startDate = endDate.AddDays(-7);
            }
            else if (frequency.ToUpper().Equals("MONTHLY"))
            {
                startDate = endDate.AddMonths(-1);
            }
            else if (frequency.ToUpper().Equals("YEARLY"))
            {
                startDate = endDate.AddYears(-1);
            }

            return startDate;
        } 

        


        public bool CheckBaseGroup(int? groupId1, int? groupId2)
        {
            monitorContext mc = new monitorContext();
            Group group1 = mc.Groups.Where(x => x.GroupId == groupId1).FirstOrDefault();
            Group group2 = mc.Groups.Where(x => x.GroupId == groupId2).FirstOrDefault();

            while(true)
            {
                Group parentGroup1 = mc.Groups.Where(x => x.GroupId == group1.ParentGroup).FirstOrDefault();
                if(parentGroup1.ParentGroup == null)
                {
                    break;
                }
                group1 = mc.Groups.Where(x => x.GroupId == group1.ParentGroup).FirstOrDefault();
            }

            while (true)
            {
                Group parentGroup2 = mc.Groups.Where(x => x.GroupId == group2.ParentGroup).FirstOrDefault();
                if (parentGroup2.ParentGroup == null)
                {
                    break;
                }
                group2 = mc.Groups.Where(x => x.GroupId == group2.ParentGroup).FirstOrDefault();
            }

            if (group1.Equals(group2))
            {
                return true;
            }

            return false;
        
        }


        public List<DeviceResponseModel> getDRMfromDeviceList(List<Device> devices, monitorContext mc)
        {
            List<DeviceResponseModel> drmList = new List<DeviceResponseModel>();
            foreach (var dev in devices)
            {
                int? groupIdForDevice = (from x in mc.DeviceGroups.OfType<DeviceGroup>() where x.DeviceId == dev.DeviceId select x.GroupId).FirstOrDefault();
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
                    GroupId = groupIdForDevice,
                    GroupName = (from x in mc.Groups.OfType<Group>() where x.GroupId == groupIdForDevice select x.Name).FirstOrDefault(),
                    DeviceUid = dev.DeviceUid
                });
            }
            return drmList;
        }

    }
}
