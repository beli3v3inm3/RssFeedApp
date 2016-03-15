using System.Threading.Tasks;
using System.Web.Http;
using RssFeedApp.Models;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly UserRepository _repository;
        private readonly RssConnection _connection;

        public AccountController()
        {
            _repository = new UserRepository();
            _connection = new RssConnection();
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

        public void RssRead()
        {
            _connection.Connection.Open();
            _connection.SqlCommand.Connection = _connection.Connection;
            _connection.SqlCommand.CommandText = "insert into test (id, name) values ('dq1we3', 'd2qwe34')";
            _connection.SqlCommand.ExecuteNonQuery();
            _connection.Dispose();
        }
    }
}
