using System.Web.Http;
using RssFeedApp.Models;

namespace RssFeedApp.Controller
{
    [Authorize]
    public class RssReaderController : ApiController
    {
        private readonly IRssRepository _rssRepository;
       
        public RssReaderController(IRssRepository rssRepository)
        {
            _rssRepository = rssRepository;
        }

        public IHttpActionResult AddRss(Rss urlRss)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssRepository.AddFeed(urlRss);
            
            return Ok();
        }

        public IHttpActionResult GetFeeds() => Ok(_rssRepository.GetFeeds());

        public IHttpActionResult GetRss() => Ok(_rssRepository.GetRss());

        public IHttpActionResult SetReadFeeditem(Feed feed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssRepository.SetReadItem(feed);
            return Ok();
        }

        public IHttpActionResult RemoveFeedItem(Feed feed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssRepository.RemoveFeed(feed);
            return Ok();
        }
    }
}
