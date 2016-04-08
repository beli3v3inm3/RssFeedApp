﻿using System.Threading.Tasks;
using System.Web.Http;
using RssFeedApp.Models;
using RssFeedApp.Provider;

namespace RssFeedApp.Controller
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly UserRepository _repository;

        public AccountController()
        {
            _repository = UserRepository.GetInstance;
        }

        [AllowAnonymous]
        [Route("Register")]
        public IHttpActionResult RegisterTask(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.RegisterTask(userModel);

            return Ok();
        }
    }
}
