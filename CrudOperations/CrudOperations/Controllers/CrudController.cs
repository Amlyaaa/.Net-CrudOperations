using CrudOperations.Data;
using CrudOperations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudOperations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        private readonly AppDbContext context;

        public CrudController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/crud?sort=name/email
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers([FromQuery] string? sort)
        {
            var users = context.users.AsQueryable();

            if (sort == "name")
                users = users.OrderBy(u => u.Name);
            else if (sort == "email")
                users = users.OrderBy(u => u.Email);

            return await users.ToListAsync();
        }

        // GET by id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await context.users.FindAsync(id);
            if (user == null)
                return NotFound();
            return user;
        }

        // POST 
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            context.users.Add(user);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = user.ID }, user);
        }

        // PUT 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, User updatedUser)
        {
            if (id != updatedUser.ID)
                return BadRequest("User ID mismatch.");

            context.Entry(updatedUser).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await context.users.AnyAsync(u => u.ID == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE:id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await context.users.FindAsync(id);
            if (user == null)
                return NotFound();

            context.users.Remove(user);
            await context.SaveChangesAsync();
            return NoContent();
        }

        // GET: search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<User>>> SearchByName([FromQuery] string name)
        {
            var users = await context.users
                .Where(u => u.Name.Contains(name))
                .ToListAsync();
            return users;
        }

        // GET: api/crud/filter?email=abc@example.com
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<User>>> FilterByEmail([FromQuery] string email)
        {
            var users = await context.users
                .Where(u => u.Email == email)
                .ToListAsync();
            return users;
        }
    }
}
