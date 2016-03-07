using System.Web.Http;
using RssFeedApp.Models;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/rssreader")]
    public class RssReaderController : ApiController
    {
        private readonly AuthRepository _authRepository;

        public RssReaderController()
        {
            _authRepository = new AuthRepository();
        }

        public IHttpActionResult Get()
        {
            
        }
        
    }
}
