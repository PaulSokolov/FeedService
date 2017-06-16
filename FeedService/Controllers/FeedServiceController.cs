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

namespace FeedService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FeedServiceController : Controller
    {
        IFeedServiceUoW _db;
        IMemoryCache _cache;

        public FeedServiceController(IFeedServiceUoW db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }
        
        [HttpGet]
        [Route("/GetNews/{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request parameters", ModelState = ModelState });
            }

            var collection =  _db.Collections.GetAll().Include(c=>c.CollectionFeeds).FirstOrDefault(m => m.Id == id);
            _db.Feeds.GetAll().Where(f => f.Id == collection.CollectionFeeds.First(cf => cf.FeedId == f.Id).FeedId).Load();
            if (collection == null)
            {
                return NotFound(new { Error = "There is no collection with such id" });
            }

            List<IFeedItem> news = new List<IFeedItem>();

            foreach (var feed in collection.CollectionFeeds.Select(cf=>cf.Feed))
            {
                IFeedReader reader = FeedsReaderFactory.CreateReader(feed.Type);
                if (IsInCache(feed.Url))
                {
                    news.AddRange(reader.ReadFeed(feed.Url));
                    CacheFeed(reader);
                }
                else
                    news.AddRange(GetFromCache(feed.Url));

            }

            return Ok(news);
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
