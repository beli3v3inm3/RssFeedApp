using System.Web.Http;
using RssFeedApp.Models;
using RssFeedApp.Provider;
using StructureMap.Attributes;

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
        public IHttpActionResult Get() => Ok(_rssRepository.GetFeeds());

        [Route("GetRss")]
        public IHttpActionResult GetRssUrl() => Ok(_rssRepository.GetRss());

        public IHttpActionResult SetReadFeeditem(Feed feed)
        {
            _rssRepository.SetReadItem(feed);
            return Ok();
        }
    }
}
