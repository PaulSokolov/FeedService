using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedService.DbModels;
using Microsoft.AspNetCore.Authorization;
using FeedService.DbModels.Interfaces;

namespace FeedService.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Collections")]
    public class CollectionsController : Controller
    {
        private readonly IFeedServiceUoW _db;

        public CollectionsController(IFeedServiceUoW db)
        {
            _db = db;
        }

        // GET: api/Collections
        [HttpGet]
        public IEnumerable<Collection> GetCollections()
        {
            return _db.Collections.GetAll();
        }

        // GET: api/Collections/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollection([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request parameters", ModelState = ModelState });
            }

            var collection = await _db.Collections.GetAll().SingleOrDefaultAsync(m => m.Id == id);

            if (collection == null)
            {
                return NotFound(new { Error = "There is no such collection." });
            }

            return Ok(collection);
        }

        // PUT: api/Collections/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection([FromRoute] int id, [FromBody] Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request parameters", ModelState = ModelState });
            }

            if (id != collection.Id)
            {
                return BadRequest(new { Error = "Collection ids doesn't match" });
            }

            _db.Collections.Edit(collection);

            try
            {
                await _db.Collections.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
                {
                    return NotFound(new { Error = "Collection not found. Try later" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Success = "Collection edited successfully" });
        }

        [Route("/AddToCollection/{id}")]
        [HttpPut]
        public async Task<IActionResult> AddFeedToCollection([FromRoute] int id, [FromBody] Feed feed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid requst parameters", ModelState = ModelState });
            }

            Collection col = await _db.Collections.GetAll().FirstOrDefaultAsync(c =>  c.Id == id);

            if (col == null)
            {
                return BadRequest(new { Error = "There is no collection with such id" });
            }

            Feed _feed = await _db.Feeds.GetAll().FirstOrDefaultAsync(f => f.Url == feed.Url);

            if (_feed == null)
            {
                col.CollectionFeeds.Add(new CollectionFeed { Feed = feed, Collection = col });
                _db.Collections.Edit(col);
            }
            else
            {
                col.CollectionFeeds.Add(new CollectionFeed { Feed = _feed, Collection = col });
                _db.Collections.Edit(col);
            }

            try
            {
                await _db.Collections.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedExists(id))
                {
                    return NotFound(new { Error = "There is no such feed. Try later" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Success = "Feed added to collection successfully" });
        }

        // POST: api/Collections
        [HttpPost]
        public async Task<IActionResult> PostCollection([FromBody] Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid reques parameters", ModelState = ModelState });
            }

            await _db.Collections.AddAsync(collection);
            await _db.Collections.SaveAsync();

            return CreatedAtAction("GetCollection", new { id = collection.Id }, collection);
        }

        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request parameters", ModelState = ModelState });
            }

            var collection = await _db.Collections.GetAll().SingleOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound(new { Error = "There is no such collection" });
            }

            _db.Collections.Delete(collection);
            await _db.Collections.SaveAsync();

            return Ok(collection);
        }

        private bool CollectionExists(int id)
        {
            return _db.Collections.GetAll().Any(e => e.Id == id);
        }

        private bool FeedExists(int id)
        {
            return _db.Feeds.GetAll().Any(e => e.Id == id);
        }
    }
}