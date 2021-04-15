using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MonitorWebAPI.Helpers;
using MonitorWebAPI.Models;
using Newtonsoft.Json;


namespace MonitorWebAPI.Controllers
{
    [EnableCors("MonitorPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        private readonly monitorContext mc;
        private HelperMethods helperMethods = new HelperMethods();

        public ComponentsController() => mc = new monitorContext();

        
        // GET: api/Components/5
        
        [HttpGet("{taskId}")]
        public async Task<ActionResult<ResponseModel<List<Component>>>> GetComponent(int taskId, [FromHeader] string Authorization)
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

                var component = mc.Components.Where(c => c.TaskId == taskId).ToList();

                if (component.Count == 0)
                {
                    return NotFound();
                }

                return new ResponseModel<List<Component>>() { data = component, newAccessToken = vu.accessToken};
            }
            else
            {
                return Unauthorized();
            }
        }


        // POST: api/Components
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ResponseModel<List<Component>>>> PostComponents([FromBody] List<Component> components, [FromHeader] string Authorization)
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

                foreach (Component com in components)
                {
                    mc.Components.Add(com);
                }

                await mc.SaveChangesAsync();

                return new ResponseModel<List<Component>>() { data = components, newAccessToken = vu.accessToken };
            }
            else
            {
                return Unauthorized();
            }
        }




        // GET: api/Components
        /*
        [Route("/")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Component>>> GetComponents()
        {
            return await mc.Components.ToListAsync();
        }
        */


        // PUT: api/Components/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        /*
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComponent(int id, Component component)
        {
            if (id != component.ComponentId)
            {
                return BadRequest();
            }

            mc.Entry(component).State = EntityState.Modified;

            try
            {
                await mc.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComponentExists(id))
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
        */

        // DELETE: api/Components/5
        /*
        [HttpDelete("{id}")]
        public async Task<ActionResult<Component>> DeleteComponent(int id)
        {
            var component = await mc.Components.FindAsync(id);
            if (component == null)
            {
                return NotFound();
            }

            mc.Components.Remove(component);
            await mc.SaveChangesAsync();

            return component;
        }
        */

        private bool ComponentExists(int id)
        {
            return mc.Components.Any(e => e.ComponentId == id);
        }
    }
}
