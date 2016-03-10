using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using RssFeedApp.Models;
using System.Web.SessionState;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/rssreader")]

    public class RssReaderController : ApiController
    {
        private readonly UserRepository _repository;
        private const string userKey = "Name";

        public RssReaderController(UserRepository userRepository)
        {
            _repository = userRepository;
        }

        //public async Task<IHttpActionResult> GetDbInfo()
        //{
        //    var buildInfoCOmmand = new BsonDocument("buildinfo", 1);
        //    var buildInfo = await _context.MongoDatabase.RunCommandAsync<BsonDocument>(buildInfoCOmmand);
        //    return Json(buildInfo);
        //}

        [Route("Register")]
        public async Task<IHttpActionResult> RegisterTask(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HttpContext.Current.Session[userKey] = userModel.UserName;

            await _repository.RegisterTask(userModel);

            return Ok();
        }



    }
}
