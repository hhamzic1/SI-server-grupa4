using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Controllers;
using MonitorWebAPI.Models;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProjectNUnit
{
    public class Tests
    {
        private string token;
        private string token2;
        [SetUp]
        public void Setup()
        {
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            token = "e " + v[0];

            client.Timeout = -1;
            var request2 = new RestRequest(Method.POST);
            request2.AddHeader("Content-Type", "application/json");
            request2.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken2 = client.Execute(request2);
            var s2 = responseToken2.Content.ToString().Split("\":\"");
            var t2 = s2[1].ToString().Split("}");
            var v2 = t2[0].ToString().Split('"');
            token2 = "e " + v2[0];
        }

        [Test]
        public async System.Threading.Tasks.Task TestGetAllDevicesWithAuth()
        {
            var controller = new DeviceController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllDevices(Authorization: token);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ResponseModel<List<Device>>>(result.Value);
        }

        [Test]
        public async Task GetAllDevicesTestAsync2()
        {
            var controller = new DeviceController();
            var result = await controller.GetAllDevices(Authorization: token2);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ResponseModel<List<Device>>>(result.Value);
        }

        [Test]

        public async Task GetAllDevicesForUserAsync()
        {


            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            string name = null;
            var status = "active";
            int? groupId = null;
            var sort_by = "ascending";
            string location = null;
            var result = await controller.AllDevicesForUser(page, per_page, name, status, groupId, sort_by, location, Authorization: token);
            Assert.IsInstanceOf<ResponseModel<DevicePagingModel>>(result.Value);
        }

        [Test]

        public async Task GetAllDevicesForUserAsync2()
        {


            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            string name = null;
            var status = "active";
            int? groupId = 17;
            var sort_by = "name_asc";
            string location = "sarajevo";
            var result = await controller.AllDevicesForUser(page, per_page, name, status, groupId, sort_by, location, Authorization: token);
            Assert.IsNotNull(result.Value);

        }

        [Test]

        public async Task GetAllDevicesForUserAsync3()
        {


            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            string name = null;
            var status = "active";
            int? groupId = 17;
            var sort_by = "name_asc";
            string location = "sarajevo";
            var result = await controller.AllDevicesForUser(page, per_page, name, status, groupId, sort_by, location, Authorization: token2);
            Assert.IsNotNull(result.Value);

        }

        [Test]

        public async Task GetAllDevicesForUserAsync4()
        {


            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            string name = null;
            var status = "active";
            int? groupId = null;
            var sort_by = "ascending";
            string location = null;
            var result = await controller.AllDevicesForUser(page, per_page, name, status, groupId, sort_by, location, Authorization: token2);
            Assert.IsInstanceOf<ResponseModel<DevicePagingModel>>(result.Value);
        }

        [Test]

        public async Task GetDeviceByInstallationCodeNull()
        {


            var controller = new DeviceController();
            var result = await controller.GetDeviceByInstallationCode(null);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

        }
        [Test]

        public async Task GetDeviceByInstallationCodeNotNullButInvalid()
        {


            var controller = new DeviceController();
            var result = await controller.GetDeviceByInstallationCode("kod");
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }


        [Test]

        public async Task CheckIfDeviceBelongsToUserWithAuth()
        {
            var controller = new DeviceController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            Guid uid = new Guid("eba54ce1-1df9-49ca-b104-801a8827f911");
            var response = controller.ControllerContext.HttpContext.Response;
            var result = await controller.CheckIfDeviceBelongsToUser(Authorization: token, uid);
            Assert.NotNull(result);
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]

        public async Task CheckIfDeviceBelongsToUserNullToken()
        {

            var controller = new DeviceController();
            Guid uid = new Guid();
            var result = await controller.CheckIfDeviceBelongsToUser(Authorization: null, uid);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task GetDeviceLogs1()
        {
            var controller = new DeviceController();
            var result = await controller.GetDeviceLogs(Authorization: null, 17, null, null);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]
        public async Task GetDeviceLogsWithAuth()
        {

            var controller = new DeviceController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetDeviceLogs(Authorization: token, 17, null, null);
            Assert.NotNull(result.Value);
        }

        [Test]
        public async Task GetDeviceLogsWithAuth2()
        {

            var controller = new DeviceController();
            string startDate = "datum1";
            string endDate = "datum2";
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetDeviceLogs(Authorization: token, 17, startDate, endDate);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

        }

        [Test]
        public async Task GetDeviceLogsNullToken()
        {

            var controller = new DeviceController();
            string? startDate = null;
            string? endDate = null;
            var result = await controller.GetDeviceLogs(Authorization: null, startDate, endDate);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);

        }

        [Test]
        public async Task AllDevicesForGroupNullToken()
        {

            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            var name = "name";
            var status = "active";
            var groupId = 1;
            var sort_by = "name_asc";
            var location = "sarajevo";
            var result = await controller.AllDevicesForGroup(page, per_page, name, status, groupId, sort_by, location, Authorization: null);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]
        public async Task AllDevicesForGroupWithAuth()
        {

            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            var name = "";
            var status = "active";
            var groupId = 1;
            var sort_by = "name_asc";
            var location = "sarajevo";
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.AllDevicesForGroup(page, per_page, name, status, groupId, sort_by, location, Authorization: token);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ResponseModel<DevicePagingModel>>(result.Value);
        }

        [Test]
        public async Task AllDevicesForGroupWithAuth2()
        {

            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            var name = "";
            var status = "active";
            var groupId = 1;
            var sort_by = "name_asc";
            var location = "sarajevo";
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.AllDevicesForGroup(page, per_page, name, status, groupId, sort_by, location, Authorization: token2);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ResponseModel<DevicePagingModel>>(result.Value);
        }

        [Test]

        public async Task MyGroupUnauthorized()
        {
            var controller = new GroupController();
            string pomToken = "";
            var result = await controller.MyGroup(Authorization: pomToken);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task MyGroupWithAuth()
        {
            var controller = new GroupController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.MyGroup(Authorization: token);
            Assert.Null(result.Result);
        }

        [Test]

        public async Task MyAssignedGroupsNullToken()
        {
            var controller = new GroupController();
            var result = await controller.MyAssignedGroups(Authorization: null);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task MyAssignedGroupsWithAuth()
        {
            var controller = new GroupController();
            var result = await controller.MyAssignedGroups(Authorization: token2);
            Assert.AreEqual("Imtec", result.Value.data.Name);
            Assert.IsInstanceOf<ResponseModel<GroupHierarchyModel>>(result.Value);
        }

        [Test]

        public async Task GetAllGroupsNullToken()
        {
            var controller = new GroupController();
            var result = await controller.GetAllGroups(Authorization: null);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task GetAllGroupsWithAuth()
        {
            var controller = new GroupController();
            var result = await controller.GetAllGroups(Authorization: token2);
            Assert.Null(result.Value);

        }

        [Test]

        public async Task GetallGroupsWithAuth2()
        {
            var controller = new GroupController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllGroups(Authorization: token);
            Assert.NotNull(result.Value);
            Assert.IsInstanceOf<ResponseModel<List<GroupHierarchyModel>>>(result.Value);
        }


        [Test]

        public async Task GroupTreeNullToken()
        {
            var controller = new GroupController();
            var result = await controller.GroupTree(Authorization: null, 1);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task GroupTreeWithAuth()
        {
            var controller = new GroupController();
            var result = await controller.GroupTree(Authorization: token2, 1);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ResponseModel<GroupHierarchyModel>>(result.Value);
        }

        [Test]

        public async Task GroupTreeAuthInvalidGroup()
        {
            var controller = new GroupController();
            var result = await controller.GroupTree(Authorization: token2, 25658564);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]

        public async Task CreateGroupNullToken()
        {
            var controller = new GroupController();
            var group = new Group();
            var result = await controller.CreateGroup(Authorization: null, group);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);

        }


        [Test]

        public async Task RoleInvalidToken()
        {
            var controller = new RoleController();
            var result = await controller.GetAllRoles(Authorization: "");
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task RoleWithAuth()
        {
            var controller = new RoleController();
            var result = await controller.GetAllRoles(Authorization: token2);
            Assert.AreNotEqual(0, result.Value.data.Count);
            Assert.NotNull(result);
            Assert.IsInstanceOf<ResponseModel<List<Role>>>(result.Value);
        }

        [Test]

        public async Task CurrentUserNullToken()
        {
            var controller = new UserController();
            var result = await controller.CurrentUser(Authorization: null);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task UserTestWithAuth()
        {
            var controller = new UserController();
            var result = await controller.CurrentUser(Authorization: token2);
            result.GetType();
            Assert.AreEqual(1, result.Value.data.UserId);
            Assert.IsInstanceOf<ResponseModel<User>>(result.Value);
        }

        [Test]

        public async Task CurrentUserExtendedInfoNullToken()
        {
            var controller = new UserController();
            var result = await controller.CurrentUserExtendedInfo(Authorization: null);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task CurrentUserExtendedInfoWithAuth()
        {
            var controller = new UserController();
            var result = await controller.CurrentUserExtendedInfo(Authorization: token2);
            Assert.AreEqual(1, result.Value.data.UserId);
            Assert.IsInstanceOf<ResponseModel<User>>(result.Value);
        }

        [Test]

        public async Task GetAllUsersNullToken()
        {
            var controller = new UserController();
            var result = await controller.GetAllUsers(Authorization: null, true, 1, 1, "name", "name", "email", "adress", "name_asc");
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }


        [Test]

        public async Task GetAllUsersTasksNullToken()
        {
            var controller = new UserController();
            var result = await controller.GetAllUserTasks(Authorization: null);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task GetAllUsersTasksWithAuth()
        {
            var controller = new UserController();
            var result = await controller.GetAllUserTasks(Authorization: token2);
            Assert.IsInstanceOf<ForbidResult>(result.Result);
        }

        [Test]

        public async Task GetAllUsersTasksWithAuth2()
        {
            var controller = new UserController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllUserTasks(Authorization: token);
            Assert.IsInstanceOf<ResponseModel<IEnumerable<User>>>(result.Value);
        }

        [Test]

        public async Task SaveCommandLogNulltoken()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            var result = await controller.SaveCommandLog(Authorization: null, usercommand);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }


        [Test]

        public async Task GetAllCommandLogsForDeviceNullToken()
        {
            var controller = new UserCommandLogsController();
            var result = await controller.GetAllCommandLogsForDevice(Authorization: null, deviceId: 1);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);
        }

        [Test]

        public async Task GetAllCommandLogsForDeviceWithAuth()
        {
            var controller = new UserCommandLogsController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllCommandLogsForDevice(Authorization: token, deviceId: 101861875);
            Assert.Null(result.Value);
        }

        [Test]

        public async Task GetAllCommandLogsForDeviceWithAuth2()
        {
            var controller = new UserCommandLogsController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllCommandLogsForDevice(Authorization: token, deviceId: 17);
            Assert.NotNull(result.Value);
        }

        [Test]

        public async Task GetAllCommandLogsForDeviceAndUserNullToken()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: null, deviceId: 1, userId: 1);
            Assert.IsInstanceOf<UnauthorizedResult>(result.Result);

        }

        [Test]

        public async Task GetAllCommandLogsForDeviceAndUserWithAuth()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 1, userId: 1);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]

        public async Task GetAllCommandLogsForDeviceAndUserWithAuth2()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 17, userId: 1);
            Assert.IsEmpty(result.Value.data);
        }

        [Test]

        public async Task GetAllCommandLogsForDeviceAndUserWithAuth3InvalidDeviceId()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 1259451256, userId: 1);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]

        public async Task GetAllCommandLogsForDeviceAndUserWithAuth4InvalidUserId()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 17, userId: 1254574256);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }


    }
}