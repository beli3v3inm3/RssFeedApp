using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MongoDB.Bson;
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
            _rssProvider = new RssProvider();
        }

        [Authorize]
        [Route("AddFeed")]
        public IHttpActionResult AddFeed(RssFeed rssFeed)
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
    }
}
