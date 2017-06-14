using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedService.DbModels;
using Microsoft.AspNetCore.Authorization;

namespace FeedService.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Collections")]
    public class CollectionsController : Controller
    {
        private readonly FeedServiceContext _context;

        public CollectionsController(FeedServiceContext context)
        {
            _context = context;
        }

        // GET: api/Collections
        [HttpGet]
        public IEnumerable<Collection> GetCollections()
        {
            return _context.Collections;
        }

        // GET: api/Collections/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollection([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collection = await _context.Collections.SingleOrDefaultAsync(m => m.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            return Ok(collection);
        }

        // PUT: api/Collections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection([FromRoute] int id, [FromBody] Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != collection.Id)
            {
                return BadRequest();
            }

            _context.Entry(collection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
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

        // POST: api/Collections
        [HttpPost]
        public async Task<IActionResult> PostCollection([FromBody] Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCollection", new { id = collection.Id }, collection);
        }

        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collection = await _context.Collections.SingleOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();

            return Ok(collection);
        }

        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }
    }
}