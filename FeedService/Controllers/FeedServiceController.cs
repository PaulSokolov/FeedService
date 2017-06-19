using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FeedService.DbModels;
using Microsoft.EntityFrameworkCore;
using FeedService.Intrefaces;
using FeedService.Models;
using Microsoft.Extensions.Caching.Memory;
using FeedService.DbModels.Interfaces;
using Microsoft.Extensions.Logging;
using FeedService.Infrastructure.Response;

namespace FeedService.Controllers
{
    [Authorize]
    public class FeedServiceController : Controller
    {
        IFeedServiceUoW _db;
        IMemoryCache _cache;
        ILogger _logger;

        public FeedServiceController(IFeedServiceUoW db, IMemoryCache cache, ILogger<FeedServiceController> logger)
        {
            _db = db;
            _cache = cache;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("/GetNews/{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            List<Feed> feeds = null;
            List<string> errors = new List<string>();

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObject(ErrorMessages.BAD_REQUEST_ERROR){ ModelState = ModelState });
            }

            try
            {

                var collection = await _db.Collections.GetAll().Include(c => c.User).FirstOrDefaultAsync(c => c.User.Login == User.Identity.Name);
                if (collection == null)
                {
                    return NotFound(new ErrorObject(ErrorMessages.COLLECTION_NOT_FOUND_ERROR));
                }
                feeds = await _db.CollectionsFeeds.GetAll().Where(m => m.CollectionId == id).Select(cf => cf.Feed).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }           

            List<IFeedItem> news = new List<IFeedItem>();
            try
            {

                foreach (var feed in feeds)
                {
                    IFeedReader reader = FeedsReaderFactory.CreateReader(feed.Type);
                    if (IsInCache(feed.Url))
                    {
                        IEnumerable<IFeedItem> posts = null;
                        try
                        {
                            posts = reader.ReadFeed(feed.Url);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                            errors.Add($"Problems with 3-rd party resource. Try later and check feed` url - {feed.Url}.");
                        }
                        if (posts != null)
                        {
                            news.AddRange(posts);
                            CacheFeed(reader);
                        }
                    }
                    else
                        news.AddRange(GetFromCache(feed.Url));

                }

            }
            catch(NotImplementedException ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                errors.Add(ErrorMessages.FUNCTIONALITY_NOT_IMPLEMENTED_ERROR);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new ErrorObject(ErrorMessages.SERVER_ERROR));
            }

            return Ok(new SuccessObject { Result = new { News = news, Errors = errors } });
        }

        private IEnumerable<IFeedItem> GetFromCache(string url)
        {
            IFeed tmp;
            _cache.TryGetValue(url, out tmp);

            return tmp.Items;
        } 

        private bool IsInCache(string url)
        {
            IFeed tmp;
            return !_cache.TryGetValue(url, out tmp);
        }

        private void CacheFeed(IFeed feed)
        {
            IFeed tmp;
            if (!_cache.TryGetValue(feed.Url, out tmp))
                _cache.Set(feed.Url, (IFeedReader)feed, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }
    }
}
