using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Controllers;
using MonitorWebAPI.Models;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectNUnit
{
    public class ErrorLogControllerTests
    {
        private string token;
        private ErrorLogController errorLogController;

        [SetUp]
        public void Setup()
        {
            errorLogController = new ErrorLogController();
            errorLogController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var client = new RestClient("https://si-2021.167.99.244.168.nip.io:3333/login")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var credentials = new { email = "kdokic1@etf.unsa.ba", password = "monitor" };
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(credentials);
            request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string resp = response.Content.ToString();
            token = "e " + resp.Substring(16, resp.Length - 18);
        }

        [Test] //test device with ordinary errors
        public async Task GetErrorsForDeviceInDateIntervalTest1()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            string deviceUID = "fc548ecb-12ec-4ad5-8672-9d5a9565ff60";
            DateTime startDate = DateTime.Parse("2021-03-05");
            DateTime endDate = DateTime.Parse("2021-04-05");
            var result = await errorLogController.GetErrorsInDateInterval(deviceUID, startDate.ToString(), endDate.ToString());
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsInstanceOf<ResponseModel<DeviceErrorInfo>>(result.Value);
            int errorNumber = 4;
            Assert.AreEqual(errorNumber, result.Value.data.ErrorNumber);
            Assert.True(result.Value.data.errorInfo.Count == errorNumber);
        }

        [Test] //test device with runtime errors
        public async Task GetErrorsForDeviceInDateIntervalTest2()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            string deviceUID = "92649d24-fc46-44c6-b085-356977dbb782";
            DateTime startDate = DateTime.Parse("2021-04-05");
            DateTime endDate = DateTime.Parse("2021-04-07");
            var result = await errorLogController.GetErrorsInDateInterval(deviceUID, startDate.ToString(), endDate.ToString());
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsInstanceOf<ResponseModel<DeviceErrorInfo>>(result.Value);
            int errorNumber = 2;
            Assert.AreEqual(errorNumber, result.Value.data.ErrorNumber);
            Assert.True(result.Value.data.errorInfo.Count == errorNumber);
        }

        [Test] //test device with null error
        public async Task GetErrorsForDeviceInDateIntervalTest3()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            string deviceUID = "eed19402-67c5-4978-9ad2-513cd5db6376";
            DateTime startDate = DateTime.Parse("2021-03-15");
            DateTime endDate = DateTime.Parse("2021-03-17");
            var result = await errorLogController.GetErrorsInDateInterval(deviceUID, startDate.ToString(), endDate.ToString());
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsInstanceOf<ResponseModel<DeviceErrorInfo>>(result.Value);
            int errorNumber = 3;
            Assert.AreEqual(errorNumber, result.Value.data.ErrorNumber);
            Assert.True(result.Value.data.errorInfo.Count == errorNumber);
        }

        [Test] //Should return code 200 with list of all errors for device if date parameters are null
        public async Task GetErrorsForDeviceInDateIntervalTest4()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            string deviceUID = "c2451452-7783-4ae6-98c5-17a4e7e687ab";
            var result = await errorLogController.GetErrorsInDateInterval(deviceUID, null, null);
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsInstanceOf<ResponseModel<DeviceErrorInfo>>(result.Value);
            int errorNumber = 9;
            Assert.True(result.Value.data.ErrorNumber >= errorNumber);
            Assert.True(result.Value.data.errorInfo.Count >= errorNumber);
        }

        [Test] //test device with no error in date interval
        public async Task GetErrorsForDeviceInDateIntervalTest5()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            string deviceUID = "fc548ecb-12ec-4ad5-8672-9d5a9565ff60";
            DateTime startDate = DateTime.Parse("2021-04-10");
            DateTime endDate = DateTime.Parse("2021-04-25");
            var result = await errorLogController.GetErrorsInDateInterval(deviceUID, startDate.ToString(), endDate.ToString());
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsInstanceOf<ResponseModel<DeviceErrorInfo>>(result.Value);
            int errorNumber = 0;
            Assert.AreEqual(errorNumber, result.Value.data.ErrorNumber);
            Assert.True(result.Value.data.errorInfo.Count == errorNumber);
        }

        [Test]
        public async Task GetErrorsInDateIntervalTest1()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            DateTime startDate = DateTime.Parse("2021-03-06");
            DateTime endDate = DateTime.Parse("2021-04-19");
            var result = await errorLogController.Proba(startDate.ToString(), endDate.ToString());
            Assert.AreEqual(200, response.StatusCode);
            Assert.NotNull(result.Value);
            Assert.IsInstanceOf<ResponseModel<List<DeviceErrorInfo>>>(result.Value);
            Assert.True(result.Value.data.Count >= 21);
            int errorCount = 0;
            foreach (var x in result.Value.data)
            {
                errorCount += x.ErrorNumber;
            }
            int errorNumber = 90;
            Assert.True(errorCount >= errorNumber);
        }

        [Test] //date parameters are null, return errors for all devices(21) in DB
        public async Task GetErrorsInDateIntervalTest2()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            var result = await errorLogController.Proba(null, null);
            Assert.AreEqual(200, response.StatusCode);
            Assert.NotNull(result.Value);
            Assert.IsInstanceOf<ResponseModel<List<DeviceErrorInfo>>>(result.Value);
            Assert.True(result.Value.data.Count >= 21);
            int errorCount = 0;
            foreach (var x in result.Value.data)
            {
                errorCount += x.ErrorNumber;
            }
            int errorNumber = 333;
            Assert.True(errorCount >= errorNumber);
        }
        /*
        [Test]
        public async Task GetErrorsFromOneGroupTest1()
        {
            var response = errorLogController.ControllerContext.HttpContext.Response;
            var result = await errorLogController.GetErrorsFromOneGroup(token);
            Assert.AreEqual(200, response.StatusCode);
            Assert.NotNull(result.Value);
            Assert.IsInstanceOf<ResponseModel<List<ErrorInfo>>>(result.Value);
            Assert.True(result.Value.data.Count >= 349);
        }*/

        [Test]//invalid token, result Unauthorized
        public async Task GetErrorsFromOneGroupTest2()
        {
            string token = "eyJhbGciOiJIUzI1N.eyJpZm91AwMzMsImV4cCI6MTYxOTkyMT.zaUjl7yF81GZrjgGJPUwq4";

            var response = errorLogController.ControllerContext.HttpContext.Response;
            var result = await errorLogController.GetErrorsFromOneGroup(token);
            Assert.AreEqual(200, response.StatusCode);
            Assert.Null(result.Value);
        }
    }
}
