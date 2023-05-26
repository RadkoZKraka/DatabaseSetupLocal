using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseSetupLocal.Data;
using DatabaseSetupLocal.Models;
using Microsoft.AspNetCore.Authorization;

namespace DatabaseSetupLocal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ShotsContext _context;

        public ApiController(ShotsContext context)
        {
            _context = context;
        }

        // GET: api/Api
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserShots>>> GetUserShotsModel()
        {
            return await _context.UserShotsModel.ToListAsync();
        }

        // GET: api/Api/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserShots>> GetUserShots(string id)
        {
            if (_context.UserShotsModel == null)
            {
                return NotFound();
            }

            var userShots = await _context.UserShotsModel.FindAsync(id);

            if (userShots == null)
            {
                return NotFound();
            }

            return userShots;
        }

        // PUT: api/Api/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserShots(string id, UserShots userShots)
        {
            if (id != userShots.Id)
            {
                return BadRequest();
            }

            _context.Entry(userShots).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserShotsExists(id))
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

        // POST: api/Api
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserShots>> PostUserShots(UserShots userShots)
        {
            if (_context.UserShotsModel == null)
            {
                return Problem("Entity set 'ShotsContext.UserShotsModel'  is null.");
            }

            _context.UserShotsModel.Add(userShots);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserShots", new {id = userShots.Id}, userShots);
        }

        // DELETE: api/Api/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserShots(string id)
        {
            if (_context.UserShotsModel == null)
            {
                return NotFound();
            }

            var userShots = await _context.UserShotsModel.FindAsync(id);
            if (userShots == null)
            {
                return NotFound();
            }

            _context.UserShotsModel.Remove(userShots);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserShotsExists(string id)
        {
            return (_context.UserShotsModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}