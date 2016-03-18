using System.Web.Http;
using RssFeedApp.Models;
using RssFeedApp.Provider;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/rssreader")]
    public class RssReaderController : ApiController
    {
        private readonly RssProvider _rssProvider;
       
        public RssReaderController()
        {
            _rssProvider = RssProvider.GetInstance;
        }

        [Authorize]
        [Route("AddFeed")]
        public IHttpActionResult AddFeedByUrl(RssFeed rssFeed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssProvider.AddFeed(rssFeed);
            
            return Ok();
        }

        [Authorize]
        public IHttpActionResult Get()
        {
            var rssFeed = new RssFeed();
            return Ok(_rssProvider.GetRssFeed(rssFeed));
        }

        [Authorize]
        public IHttpActionResult AddFeedByItem(RssFeed rssFeed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssProvider.AddFeedByItem(rssFeed);

            return Ok();
        }
    }
}
