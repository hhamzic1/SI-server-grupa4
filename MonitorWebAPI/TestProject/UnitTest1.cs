using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonitorWebAPI.Controllers;
using MonitorWebAPI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace TestProject
{

    [TestClass]


    public class UnitTest2
    {

        [Fact]
        public async Task GetAllDevicesNullToken()
        {
            //arrange

            var controller = new DeviceController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = controller.ControllerContext.HttpContext.Response;
            //act
            string showsInfo = "m";
            var result = await controller.GetAllDevices(Authorization: showsInfo);
            //assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }



        [Fact]

        public async Task GetAllDevicesTestAsync()
        {
            var controller = new DeviceController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = controller.ControllerContext.HttpContext.Response;
            //act
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            //  s[1].ToString().Remove('"');
            var token = "e " + v[0];
            var result = await controller.GetAllDevices(Authorization: token);
            //assert
            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(result);
        }

        [Fact]

        public async Task GetAllDevicesTestAsync2()
        {
            var controller = new DeviceController();
            //initialise the ControllerContext to set the Response 
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllDevices(Authorization: token);
            //assert
            Assert.NotNull(result);
        }

        [Fact]

        public async Task GetAllDevicesForUserAsync()
        {


            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            string name = null;
            var status = "active";
            int? groupId = 0;
            var sort_by = "ascending";
            string location = null;
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            //  s[1].ToString().Remove('"');
            var token = "e " + v[0];
            // Act
            var result = await controller.AllDevicesForUser(page, per_page, name, status, groupId, sort_by, location, Authorization: token);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]

        public async Task GetAllDevicesForUserAsync2()
        {


            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            string name = null;
            var status = "active";
            int? groupId = 17;
            var sort_by = "name_asc";
            string location = null;
            //initialise the ControllerContext to set the Response 
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            // Act
            var result = await controller.AllDevicesForUser(page, per_page, name, status, groupId, sort_by, location, Authorization: token);
            // Assert
            Assert.IsType<DevicePagingModel>(result.Value.data);
        }


        [Fact]

        public async Task GetDeviceByInstallationCodeNull()
        {


            var controller = new DeviceController();
            string token = null;
            var result = await controller.GetDeviceByInstallationCode(token);
            Assert.IsType<BadRequestObjectResult>(result.Result);

        }
        //ne radi
        [Fact]

        public async Task GetDeviceByInstallationCodeNotNullButInvalid()
        {


            var controller = new DeviceController();
            var result = await controller.GetDeviceByInstallationCode("kod");
            Assert.IsType<NotFoundResult>(result.Result);

        }


        [Fact]

        public async Task CheckIfDeviceBelongsToUserAuth()
        {
            var controller = new DeviceController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            //  s[1].ToString().Remove('"');
            var token = "e " + v[0];
            Guid uid = new Guid("eba54ce1-1df9-49ca-b104-801a8827f911");
            var response = controller.ControllerContext.HttpContext.Response;

            var result = await controller.CheckIfDeviceBelongsToUser(Authorization: token, uid);
            Assert.NotNull(result);


        }

        [Fact]

        public async Task CheckIfDeviceBelongsToUserNulltoken()
        {

            var controller = new DeviceController();
            Guid uid = new Guid();
            var result = await controller.CheckIfDeviceBelongsToUser(Authorization: null, uid);
            Assert.IsType<UnauthorizedResult>(result);


        }

        [Fact]
        public async Task GetDeviceLogs1()
        {
            var controller = new DeviceController();
            var result = await controller.GetDeviceLogs(Authorization: null, 17, null, null);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetDeviceLogsAuth()
        {

            var controller = new DeviceController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            //  s[1].ToString().Remove('"');
            var token = "e " + v[0];
            var result = await controller.GetDeviceLogs(Authorization: token, 17, null, null);
            Assert.NotNull(result.Value);

        }

        [Fact]
        public async Task GetDeviceLogsAuth2()
        {

            var controller = new DeviceController();
            string startDate = "datum1";
            string endDate = "datum2";
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            //  s[1].ToString().Remove('"');
            var token = "e " + v[0];
            var result = await controller.GetDeviceLogs(Authorization: token, 17, startDate, endDate);
            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public async Task GetDeviceLogsNullToken()
        {

            var controller = new DeviceController();
            string? startDate = null;
            string? endDate = null;
            var result = await controller.GetDeviceLogs(Authorization: null, startDate, endDate);
            Assert.IsType<UnauthorizedResult>(result.Result);

        }

        [Fact]
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
            Assert.IsType<UnauthorizedResult>(result.Result);

        }

        [Fact]
        public async Task AllDevicesForGroupAuth()
        {

            var controller = new DeviceController();
            var page = 1;
            var per_page = 1;
            var name = "";
            var status = "active";
            var groupId = 1;
            var sort_by = "name_asc";
            var location = "sarajevo";
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            //  s[1].ToString().Remove('"');
            var token = "e " + v[0];
            var result = await controller.AllDevicesForGroup(page, per_page, name, status, groupId, sort_by, location, Authorization: token);
            Assert.NotNull(result);


        }

        [Fact]

        public async Task MyGroupUnauthorized()
        {
            var controller = new GroupController();
            string token = "";
            var result = await controller.MyGroup(Authorization: token);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task MyGroupAuth()
        {
            var controller = new GroupController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.MyGroup(Authorization: token);
            Assert.Null(result.Result);
        }

        [Fact]

        public async Task MyAssignedGroupsNullToken()
        {
            var controller = new GroupController();
            var result = await controller.MyAssignedGroups(Authorization: null);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task MyAssignedGroupsAuth()
        {
            var controller = new GroupController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.MyAssignedGroups(Authorization: token);
            Assert.Equal("Imtec", result.Value.data.Name);
        }

        [Fact]

        public async Task GetAllGroupsNullToken()
        {
            var controller = new GroupController();
            var result = await controller.GetAllGroups(Authorization: null);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task GetallGroupsAuth()
        {
            var controller = new GroupController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllGroups(Authorization: token);
            Assert.Null(result.Value);

        }

        [Fact]

        public async Task GetallGroupsAuth2()
        {
            var controller = new GroupController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllGroups(Authorization: token);
            Assert.NotNull(result.Value);

        }


        [Fact]

        public async Task GroupTreeNullToken()
        {
            var controller = new GroupController();
            var result = await controller.GroupTree(Authorization: null, 1);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task GroupTreeAuth()
        {
            var controller = new GroupController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GroupTree(Authorization: token, 1);
            Assert.NotNull(result);
        }

        [Fact]

        public async Task GroupTreeAuthInvalidGroup()
        {
            var controller = new GroupController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GroupTree(Authorization: token, 256);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]

        public async Task CreateGroupNullToken()
        {
            var controller = new GroupController();
            var group = new Group();
            var result = await controller.CreateGroup(Authorization: null, group);
            Assert.IsType<UnauthorizedResult>(result.Result);

        }


        [Fact]

        public async Task RoleInvalidToken()
        {
            var controller = new RoleController();
            var result = await controller.GetAllRoles(Authorization: "");
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task RoleAuth()
        {
            var controller = new RoleController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllRoles(Authorization: token);
            Assert.Equal(7, result.Value.data.Count);
        }

        [Fact]

        public async Task CurrentUserNullToken()
        {
            var controller = new UserController();
            var result = await controller.CurrentUser(Authorization: null);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task UserTest()
        {
            var controller = new UserController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.CurrentUser(Authorization: token);
            Assert.Equal(1, result.Value.data.UserId);
        }

        [Fact]

        public async Task CurrentUserExtendedInfoNullToken()
        {
            var controller = new UserController();
            var result = await controller.CurrentUserExtendedInfo(Authorization: null);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task CurrentUserExtendedInfoAuth()
        {
            var controller = new UserController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.CurrentUserExtendedInfo(Authorization: token);
            Assert.Equal(1, result.Value.data.UserId);
        }

        [Fact]

        public async Task GetAllUsersNullToken()
        {
            var controller = new UserController();
            var result = await controller.GetAllUsers(Authorization: null, true, 1, 1, "name", "name", "email", "adress", "name_asc");
            Assert.IsType<UnauthorizedResult>(result.Result);
        }


        [Fact]

        public async Task GetAllUsersTasksNullToken()
        {
            var controller = new UserController();
            var result = await controller.GetAllUserTasks(Authorization: null);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task GetAllUsersTasksAuth()
        {
            var controller = new UserController();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"whoso@whoso.com\",\r\n    \"password\":\"sifra123\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllUserTasks(Authorization: token);
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]

        public async Task GetAllUsersTasksAuth2()
        {
            var controller = new UserController();
            //initialise the ControllerContext to set the Response 
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            //  s[1].ToString().Remove('"');
            var token = "e " + v[0];
            var result = await controller.GetAllUserTasks(Authorization: token);
            Assert.IsAssignableFrom<IEnumerable<User>>(result.Value.data);
        }

        [Fact]

        public async Task SaveCommandLogNulltoken()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            var result = await controller.SaveCommandLog(Authorization: null, usercommand);
            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]

        public async Task GetAllCommandLogsForDeviceNullToken()
        {
            var controller = new UserCommandLogsController();
            var result = await controller.GetAllCommandLogsForDevice(Authorization: null, deviceId: 1);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]

        public async Task GetAllCommandLogsForDeviceAuth()
        {
            var controller = new UserCommandLogsController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllCommandLogsForDevice(Authorization: token, deviceId: 1);
            Assert.Null(result.Value);
        }

        [Fact]

        public async Task GetAllCommandLogsForDeviceAuth2()
        {
            var controller = new UserCommandLogsController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllCommandLogsForDevice(Authorization: token, deviceId: 17);
            Assert.NotNull(result.Value);
        }

        [Fact]

        public async Task GetAllCommandLogsForDeviceAndUserNullToken()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: null, deviceId: 1, userId: 1);
            Assert.IsType<UnauthorizedResult>(result.Result);

        }

        [Fact]

        public async Task GetAllCommandLogsForDeviceAndUserAuth()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 1, userId: 1);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]

        public async Task GetAllCommandLogsForDeviceAndUserAuth2()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 17, userId: 1);
            Assert.Empty(result.Value.data);
        }

        [Fact]

        public async Task GetAllCommandLogsForDeviceAndUserAuth3()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 256, userId: 1);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]

        public async Task GetAllCommandLogsForDeviceAndUserAuth4()
        {
            var controller = new UserCommandLogsController();
            var usercommand = new UserCommandsLog();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"email\":\"aayiæ@aya.com\",\r\n    \"password\":\"sifraABC\"\r\n}", ParameterType.RequestBody);
            IRestResponse responseToken = client.Execute(request);
            var s = responseToken.Content.ToString().Split("\":\"");
            var t = s[1].ToString().Split("}");
            var v = t[0].ToString().Split('"');
            var token = "e " + v[0];
            var result = await controller.GetAllCommandLogsForDeviceAndUser(Authorization: token, deviceId: 17, userId: 256);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }





    }
}

