using System.Web.Http;
using RssFeedApp.Models;
using RssFeedApp.Provider;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/rssreader")]
    public class RssReaderController : ApiController
    {
        private readonly RssRepository _rssRepository;
       
        public RssReaderController()
        {
            _rssRepository = RssRepository.GetInstance;
        }

        [Authorize]
        [Route("AddFeed")]
        public IHttpActionResult AddFeedByUrl(Rss urlRss)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssRepository.AddFeed(urlRss);
            
            return Ok();
        }

        [Authorize]
        public IHttpActionResult Get()
        {
            var feed = new Feed();
            return Ok(_rssRepository.GetFeeds(feed));
        }

        [Authorize]
        [Route("GetRss")]
        public IHttpActionResult GetRssUrl()
        {
            var rss = new Rss();
            return Ok(_rssRepository.GetRss(rss));
        }

        [Route("SetRead")]
        public IHttpActionResult SetReadFeeditem(Feed feed)
        {
            _rssRepository.SetReadItem(feed);
            return Ok();
        }
    }
}
