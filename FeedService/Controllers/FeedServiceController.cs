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

namespace FeedService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FeedServiceController : Controller
    {
        FeedServiceContext db;
        IMemoryCache _cache;

        public FeedServiceController(FeedServiceContext context, IMemoryCache cache)
        {
            db = context;
            _cache = cache;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collection = await db.Collections.SingleOrDefaultAsync(m => m.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            List<IFeedItem> news = new List<IFeedItem>();
            foreach (var feed in collection.Feeds)
            {
                IFeedReader reader = FeedsReaderFactory.CreateReader(feed.Type);

                news.AddRange(reader.ReadFeed(feed.Url));
                CacheFeed((IFeed)reader);
            }

            FeedsReaderFactory.CacheNews(_cache);
                /*new List<IFeedItem>();

            foreach(var feed in collection.Feeds)
            {
                IFeedReader reader;
                switch (feed.Type)
                {
                    case FeedType.Atom:
                        reader = new AtomFeedReader();
                        news.AddRange(reader.ReadFeed(feed.Url));
                        break;
                    case FeedType.Rss:
                        reader = new RssFeedReader();
                        news.AddRange(reader.ReadFeed(feed.Url));
                        break;
                }
            }*/

            return Ok(news);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private void CacheFeed(IFeed feed)
        {
            IFeed tmp;
            if (!_cache.TryGetValue(feed.Url, out tmp))
                _cache.Set(feed.Url, feed, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }
    }
}
