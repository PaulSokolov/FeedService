using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedService.DbModels;
using Microsoft.AspNetCore.Authorization;
using FeedService.DbModels.Interfaces;
using Microsoft.Extensions.Logging;
using FeedService.Infrastructure.Response;

namespace FeedService.Controllers
{
    [Authorize]
    //[Produces("application/json")]
    [Route("Collections")]
    public class CollectionsController : Controller
    {
        private readonly IFeedServiceUoW _db;
        ILogger _logger;

        public CollectionsController(IFeedServiceUoW db, ILogger<CollectionsController> logger)
        {
            _db = db;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult GetCollections()
        {
            object col = null;
            try
            {
                col = _db.Collections.GetAll().Where(u => u.User.Login == User.Identity.Name).Select(c => new { Id = c.Id, Name = c.Name });
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            return Ok(new SuccessObject { Result = col });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollection([FromRoute] int id)
        {
            object collection = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObject(ErrorMessages.BAD_REQUEST_ERROR) { ModelState = ModelState });
            }

            try
            {
                collection = await _db.Collections.GetAll().SingleOrDefaultAsync(m => m.Id == id && m.User.Login == User.Identity.Name);
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            if (collection == null)
            {
                return NotFound(new ErrorObject(ErrorMessages.COLLECTION_NOT_FOUND_ERROR));
            }

            return Ok(new SuccessObject { Result = collection });
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection([FromRoute] int id, [FromBody] Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObject(ErrorMessages.BAD_REQUEST_ERROR) { ModelState = ModelState });
            }

            if (id != collection.Id)
            {
                return BadRequest(new ErrorObject(ErrorMessages.BAD_REQUEST_ERROR));
            }            

            try
            {
                _db.Collections.Edit(collection);
                await _db.Collections.SaveAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CollectionExists(id))
                {
                    return NotFound(new ErrorObject(ErrorMessages.COLLECTION_NOT_FOUND_ERROR));
                }
                else
                {
                    _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                    return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            return Ok(new SuccessObject(SuccessMessages.COLLECTION_EDITED_SUCCESSFULLY));
        }

        [Route("/AddFeed/{id}")]
        [HttpPut]
        public async Task<IActionResult> AddFeedToCollection([FromRoute] int id, [FromBody] Feed feed)
        {
            Collection collection = null;
            Feed _feed = null;
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid requst parameters", ModelState = ModelState });
            }
            try
            {
                collection = await _db.Collections.GetAll().Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id && c.User.Login == User.Identity.Name);
                _feed = await _db.Feeds.GetAll().FirstOrDefaultAsync(f => f.Url == feed.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            if (collection == null)
            {
                return NotFound(new ErrorObject(ErrorMessages.COLLECTION_NOT_FOUND_ERROR));
            }

            if (_feed == null)
            {
                try
                {
                    if (_db.CollectionsFeeds.GetAll().Any(cf => cf.CollectionId == collection.Id && cf.FeedId == feed.Id))
                        return BadRequest(new ErrorObject(ErrorMessages.FEED_ALREADY_EXISTS_ERROR));

                    collection.CollectionFeeds.Add(new CollectionFeed { Feed = feed, Collection = collection });
                    _db.Collections.Edit(collection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                    return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
                }
            }
            else
            {
                try
                {
                    if (_db.CollectionsFeeds.GetAll().Any(cf => cf.CollectionId == collection.Id && cf.FeedId == _feed.Id))
                        return BadRequest(new ErrorObject(ErrorMessages.FEED_ALREADY_EXISTS_ERROR));

                    collection.CollectionFeeds.Add(new CollectionFeed { Feed = _feed, Collection = collection });
                    _db.Collections.Edit(collection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                    return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
                }
            }

            try
            {
                await _db.Collections.SaveAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FeedExists(id))
                {
                    return NotFound(new ErrorObject(ErrorMessages.FEED_NOT_FOUND_ERROR));
                }
                else
                {
                    _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                    return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            return Ok(new SuccessObject(SuccessMessages.FEED_ADDED_SUCCESSFULLY));
        }
        
        [HttpPost]
        public async Task<IActionResult> PostCollection([FromBody] Collection collection)
        {
            User user = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObject("Invalid reques parameters"){ ModelState = ModelState });
            }

            try
            {
                user = await _db.Users.GetAll().FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                user.Collections.Add(collection);
                _db.Users.Edit(user);
                await _db.Collections.SaveAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            return CreatedAtAction("GetCollection", new { id = collection.Id }, new SuccessObject { Result = collection });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection([FromRoute] int id)
        {
            Collection collection = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObject(ErrorMessages.BAD_REQUEST_ERROR){ ModelState = ModelState });
            }

            try
            {
                collection = await _db.Collections.GetAll().SingleOrDefaultAsync(m => m.Id == id && m.User.Login == User.Identity.Name);

            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            if (collection == null)
            {
                return NotFound(new ErrorObject(ErrorMessages.COLLECTION_NOT_FOUND_ERROR));
            }

            try
            {
                _db.Collections.Delete(collection);
                await _db.Collections.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            return Ok(new SuccessObject(SuccessMessages.COLLECTION_DELETED_SUCCESSFULLY) { Result = collection });
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