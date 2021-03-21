using System;
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
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MonitorPolicy")]
    public class UserTasksController : ControllerBase
    {
        private readonly monitorContext _context;

        public UserTasksController()
        {
            _context = new monitorContext();
        }

        // GET: api/UserTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetUserTasks([FromHeader] string Authorization)
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
                return await _context.UserTasks.Where(t => t.UserId == vu.id).ToListAsync();
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET: api/UserTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTask>> GetUserTask(int id, [FromHeader] string Authorization)
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
                else if(userTask.UserId != vu.id)
                {
                    return Unauthorized();
                }

                return userTask;
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
        public async Task<IActionResult> PutUserTask(int id, UserTask userTask, [FromHeader] string Authorization)
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

                if (id != userTask.TaskId)
                {
                    return BadRequest();
                }
                else if(userTask.UserId != vu.id)
                {
                    return Unauthorized();
                }

                _context.Entry(userTask).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
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

                return NoContent();
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
        public async Task<ActionResult<UserTask>> PostUserTask(UserTask userTask, [FromHeader] string Authorization)
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
                _context.UserTasks.Add(userTask);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUserTask", new { id = userTask.TaskId }, userTask);
            }
            else
            {
                return Unauthorized();
            }
        }

        // DELETE: api/UserTasks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserTask>> DeleteUserTask(int id, [FromHeader] string Authorization)
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
                else if(userTask.UserId != vu.id)
                {
                    return Unauthorized();
                }

                _context.UserTasks.Remove(userTask);
                await _context.SaveChangesAsync();

                return userTask;
            }
            else
            {
                return Unauthorized();
            }
        }

        private bool UserTaskExists(int id)
        {
            return _context.UserTasks.Any(e => e.TaskId == id);
        }
    }
}
