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
        public IHttpActionResult AddFeedByUrl(UrlFeed urlFeed)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rssProvider.AddFeed(urlFeed);
            
            return Ok();
        }

        [Authorize]
        public IHttpActionResult Get()
        {
            var rssFeed = new RssFeed();
            return Ok(_rssProvider.GetRssFeed(rssFeed));
        }

        //[Authorize]
        //public IHttpActionResult GetRssUrl()
        //{
            
        //}
       
        [Route("SetRead")]
        public IHttpActionResult SetReadFeeditem(RssFeed rssFeed)
        {
            _rssProvider.SetReadItem(rssFeed);
            return Ok();
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
