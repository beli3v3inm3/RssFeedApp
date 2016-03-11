using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using RssFeedApp.Models;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/rssreader")]
    public class RssReaderController : ApiController
    {
        private readonly UserRepository _repository;

        public RssReaderController()
        {
            _repository = new UserRepository();
        }

        //[Authorize]
        //[Route("")]
        //public async Task<IHttpActionResult> GetDbInfo()
        //{
        //    var buildInfoCOmmand = new BsonDocument("buildinfo", 1);
        //    var buildInfo = await _context.MongoDatabase.RunCommandAsync<BsonDocument>(buildInfoCOmmand);
        //    return Json(buildInfo);
        //}

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> RegisterTask(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.RegisterTask(userModel);

            return Ok();
        }



    }
}
