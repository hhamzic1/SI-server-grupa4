﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using Newtonsoft.Json;

namespace MonitorWebAPI.Controllers
{
    [EnableCors("MonitorPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserTasksController : ControllerBase
    {
        private readonly monitorContext _context;

        public UserTasksController()
        {
            _context = new monitorContext();
        }

        // GET: api/UserTasks
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<UserTask>>>> GetUserTasks([FromHeader] string Authorization)
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
                var tasks = await _context.UserTasks.Where(t => t.UserId == vu.id).ToListAsync();
                foreach (UserTask task in tasks)
                {
                    if (task.DeviceId != null)
                    {
                        task.Device = await _context.Devices.FindAsync(task.DeviceId);
                    }
                }
                return new ResponseModel<IEnumerable<UserTask>>() { data = tasks, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET: api/UserTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<UserTask>>> GetUserTask(int id, [FromHeader] string Authorization)
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
                var userTask = await _context.UserTasks.FindAsync(id);

                if (userTask == null)
                {
                    return NotFound();
                }
                else if (userTask.UserId != vu.id)
                {
                    return Unauthorized();
                }
                else if (userTask.DeviceId != null)
                {
                    userTask.Device = await _context.Devices.FindAsync(userTask.DeviceId);
                }

                return new ResponseModel<UserTask>() { data = userTask, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        // PUT: api/UserTasks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<UserTask>>> PutUserTask(int id, UserTask userTask, [FromHeader] string Authorization)
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
                userTask.UserId = vu.id;
                userTask.TaskId = id;

                var oldTask = _context.UserTasks.AsNoTracking().Where(u => u.TaskId == id).FirstOrDefault();

                if (oldTask == null || oldTask.UserId != vu.id)
                {
                    return BadRequest();
                }

                _context.Entry(userTask).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    return new ResponseModel<UserTask>() { data = userTask, newAccessToken = vu.accessToken };
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserTaskExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //return NoContent();
            }
            else
            {
                return Unauthorized();
            }
        }

        // POST: api/UserTasks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ResponseModel<UserTask>>> PostUserTask(UserTask userTask, [FromHeader] string Authorization)
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

                if (userTask.DeviceId != null && _context.Devices.Find(userTask.DeviceId) == null ||
                    userTask.DeviceId == null && userTask.Location == null ||
                    userTask.Location != null && userTask.DeviceId != null)
                {
                    return BadRequest();
                }


                userTask.UserId = vu.id;
                _context.UserTasks.Add(userTask);
                await _context.SaveChangesAsync();
                return new ResponseModel<UserTask>() { data = userTask, newAccessToken = vu.accessToken };
                //return CreatedAtAction("GetUserTask", new { id = userTask.TaskId }, userTask);
            }
            else
            {
                return Unauthorized();
            }
        }

        // DELETE: api/UserTasks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<UserTask>>> DeleteUserTask(int id, [FromHeader] string Authorization)
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

                var userTask = await _context.UserTasks.FindAsync(id);
                if (userTask == null)
                {
                    return NotFound();
                }
                else if (userTask.UserId != vu.id)
                {
                    return Unauthorized();
                }

                _context.UserTasks.Remove(userTask);
                await _context.SaveChangesAsync();

                return new ResponseModel<UserTask>() { data = userTask, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET: api/TasksStatus
        [HttpGet("Status")]
        public async Task<ActionResult<IEnumerable<Models.TaskStatus>>> GetTasksStatus()
        {
            return await _context.TaskStatuses.ToListAsync();
        }

        // GET: api/TasksStatus/id
        [HttpGet("Status/{id}")]
        public async Task<ActionResult<Models.TaskStatus>> GetTasksStatus(int id)
        {
            return _context.TaskStatuses.Find(id);
        }

        private bool UserTaskExists(int id)
        {
            return _context.UserTasks.Any(e => e.TaskId == id);
        }
    }
}
