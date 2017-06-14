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
    [Route("api/Feeds")]
    public class FeedsController : Controller
    {
        private readonly FeedServiceContext _context;

        public FeedsController(FeedServiceContext context)
        {
            _context = context;
        }

        // GET: api/Feeds
        [HttpGet]
        public IEnumerable<Feed> GetFeeds()
        {
            return _context.Feeds;
        }

        // GET: api/Feeds/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeed([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feed = await _context.Feeds.SingleOrDefaultAsync(m => m.Id == id);

            if (feed == null)
            {
                return NotFound();
            }

            return Ok(feed);
        }

        // PUT: api/Feeds/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeed([FromRoute] int id, [FromBody] Feed feed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feed.Id)
            {
                return BadRequest();
            }

            _context.Entry(feed).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedExists(id))
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

        // POST: api/Feeds
        [HttpPost]
        public async Task<IActionResult> PostFeed([FromBody] Feed feed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Feeds.Add(feed);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeed", new { id = feed.Id }, feed);
        }

        // DELETE: api/Feeds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeed([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feed = await _context.Feeds.SingleOrDefaultAsync(m => m.Id == id);
            if (feed == null)
            {
                return NotFound();
            }

            _context.Feeds.Remove(feed);
            await _context.SaveChangesAsync();

            return Ok(feed);
        }

        private bool FeedExists(int id)
        {
            return _context.Feeds.Any(e => e.Id == id);
        }
    }
}