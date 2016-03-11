using System.Threading.Tasks;
using System.Web.Http;
using RssFeedApp.Models;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly UserRepository _repository;

        public AccountController()
        {
            _repository = new UserRepository();
        }

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
