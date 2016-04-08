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
            _rssRepository = new RssRepository();
        }

        [Authorize]
        [Route("AddFeed")]
        public IHttpActionResult AddFeedByUrl(UrlFeed urlFeed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssRepository.AddFeed(urlFeed);
            
            return Ok();
        }

        [Authorize]
        public IHttpActionResult Get()
        {
            var rssFeed = new RssFeed();
            return Ok(_rssRepository.GetRssFeed(rssFeed));
        }

        [Authorize]
        [Route("GetRss")]
        public IHttpActionResult GetRssUrl()
        {
            var rssFeed = new RssFeed();
            return Ok(_rssRepository.GetRss(rssFeed));
        }

        [Route("SetRead")]
        public IHttpActionResult SetReadFeeditem(RssFeed rssFeed)
        {
            _rssRepository.SetReadItem(rssFeed);
            return Ok();
        }
    }
}
