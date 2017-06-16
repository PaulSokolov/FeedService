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
        private readonly IRepository<Collection> _collectionRepository;
        private readonly IRepository<Feed> _feedRepository;
        private readonly IRepository<CollectionFeed> _collectionFeedRepository;

        public CollectionsController(IRepository<Collection> collectionRepository, IRepository<Feed> feedRepository, IRepository<CollectionFeed> collectionFeedRepository)
        {
            _collectionRepository = collectionRepository;
            _feedRepository = feedRepository;
            _collectionFeedRepository = collectionFeedRepository;
        }

        // GET: api/Collections
        [HttpGet]
        public IEnumerable<Collection> GetCollections()
        {
            return _collectionRepository.GetAll();
        }

        // GET: api/Collections/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollection([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collection = await _collectionRepository.GetAll().SingleOrDefaultAsync(m => m.Id == id);

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
                return BadRequest(ModelState);
            }

            if (id != collection.Id)
            {
                return BadRequest();
            }

            _collectionRepository.Edit(collection);

            try
            {
                await _collectionRepository.SaveAsync();
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

            return Ok("Collection edited successfully");
        }

        [Route("/AddToCollection/{id}")]
        [HttpPut]
        public async Task<IActionResult> AddFeedToCollection([FromRoute] int id, [FromBody] Feed feed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Collection col = await _collectionRepository.GetAll().FirstOrDefaultAsync(c =>  c.Id == id);

            if (col == null)
            {
                return BadRequest(new { Error = "There is no collection with such id" });
            }

            Feed _feed = await _feedRepository.GetAll().FirstOrDefaultAsync(f => f.Url == feed.Url);

            if (_feed == null)
            {
                col.CollectionFeeds.Add(new CollectionFeed { Feed = feed, Collection = col });
                _collectionRepository.Edit(col);
            }
            else
            {
                col.CollectionFeeds.Add(new CollectionFeed { Feed = _feed, Collection = col });
                _collectionRepository.Edit(col);
            }

            try
            {
                await _collectionRepository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedExists(id))
                {
                    return NotFound(new { Error = "There is no such feed" });
                }
                else
                {
                    throw;
                }
            }

            return Ok("Feed added to collection successfully");
        }

        // POST: api/Collections
        [HttpPost]
        public async Task<IActionResult> PostCollection([FromBody] Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _collectionRepository.AddAsync(collection);
            await _collectionRepository.SaveAsync();

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

            var collection = await _collectionRepository.GetAll().SingleOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound(new { Error = "There is no such collection" });
            }

            _collectionRepository.Delete(collection);
            await _collectionRepository.SaveAsync();

            return Ok(collection);
        }

        private bool CollectionExists(int id)
        {
            return _collectionRepository.GetAll().Any(e => e.Id == id);
        }

        private bool FeedExists(int id)
        {
            return _feedRepository.GetAll().Any(e => e.Id == id);
        }
    }
}